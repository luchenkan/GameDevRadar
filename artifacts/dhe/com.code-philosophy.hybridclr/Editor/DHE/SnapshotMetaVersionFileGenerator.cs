using dnlib.DotNet;
using HybridCLR.Editor.Installer;
using HybridCLR.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HybridCLR.Editor.DHE
{
    public class SnapshotMetaVersionFileGenerator
    {
        private readonly Snapshot _oldSnapshot;
        private readonly Snapshot _newSnapshot;

        private readonly List<string> _dheAssemblyNames;

        private readonly CodeDiffAnalyzer _codeDiffAnalyzer;
        private readonly SignatureMapper _signatureToIdMapper = new SignatureMapper();

        public CodeDiffAnalyzer CodeDiffAnalyzer => _codeDiffAnalyzer;

        private Dictionary<string, MetaVersionFile> _oldMetaVersionFiles = new Dictionary<string, MetaVersionFile>();

        public SnapshotMetaVersionFileGenerator(Snapshot oldSnapshot, Snapshot newSnapshot)
        {
            _oldSnapshot = oldSnapshot;
            _newSnapshot = newSnapshot;
            _dheAssemblyNames = oldSnapshot.DheAssemblyNames.ToList();
            _codeDiffAnalyzer = new CodeDiffAnalyzer(oldSnapshot, newSnapshot);
        }

        public void Generate()
        {
            _signatureToIdMapper.ReadFile(_oldSnapshot.SignatureToIdMapFile);
            LoadOldMetaVersionFiles();
            _codeDiffAnalyzer.RunAnalyze();
            GenerateData();
            _signatureToIdMapper.WriteFile(_newSnapshot.SignatureToIdMapFile);
        }

        private void LoadOldMetaVersionFiles()
        {
            foreach (var assName in _dheAssemblyNames)
            {
                var mf = new MetaVersionFile();
                mf.ReadFile($"{_oldSnapshot.MetaVersionDir}/{assName}.mv.bytes");
                _oldMetaVersionFiles.Add(assName, mf);
            }
            VerifyOldMetaVersionFiles();
        }

        private void VerifyOldMetaVersionFiles()
        {
            foreach (var assName in _dheAssemblyNames)
            {
                ModuleDefMD mod = _oldSnapshot.AssemblyCache.LoadModule(assName);
                int assTypeCount = mod.GetTypes().Count();
                int assMethodCount = mod.GetTypes().Sum(t => t.Methods.Count);

                int metaVersionTypeCount = _oldMetaVersionFiles[assName].typeDefVersions.Length;
                int metaVersionMethodCount = _oldMetaVersionFiles[assName].methodDefVersions.Length;
                if (assTypeCount != metaVersionTypeCount)
                {
                    throw new Exception($"[SnapshotMetaVersionFileGenerator::VerifyOldMetaVersionFiles] type count not match {assName}, assembly typeCount:{assTypeCount} metaVersionTypeCount:{metaVersionTypeCount}");
                }
                if (assMethodCount != metaVersionMethodCount)
                {
                    throw new Exception($"[SnapshotMetaVersionFileGenerator::VerifyOldMetaVersionFiles] method count not match {assName}, assembly methodCount:{assMethodCount} metaVersionMethodCount:{metaVersionMethodCount}");
                }
            }
        }

        private const int InitialVersionOfHotUpdateSnapshot = 10000;

        private void GenerateData()
        {
            string outputDir = _newSnapshot.MetaVersionDir;
            BashUtil.RecreateDir(outputDir);
            foreach (AssemblyDiffInfo ass in _codeDiffAnalyzer.AssemblyDiffInfos)
            {
                string assName = ass.name;
                MetaVersionFile oldMetaVerFile = _oldMetaVersionFiles[assName];

                var typeVerMap = new Dictionary<int, TypeDefVersion>();
                var typeVerSpecMap = new Dictionary<int, TypeMetaSpec>();
                var methodVerMap = new Dictionary<int, MethodDefVersion>();
                var methodVerSpecMap = new Dictionary<int, MethodMetaSpec>();

                var changedTypeVerSpecs = new List<TypeMetaSpec>();
                var changedMethodVerSpecs = new List<MethodMetaSpec>();

                bool anyChange = false;
                int versionOfNewFileOrTypeOrMethod = _newSnapshot.SnapshotMode == SnapshotMode.AOT ? oldMetaVerFile.fileVersion + 1 : InitialVersionOfHotUpdateSnapshot;

                var typeSignatureId2TypeDefs = new Dictionary<int, TypeDef>();
                var methodSignatureId2MethodDefs = new Dictionary<int, MethodDef>();
                foreach (TypeDiffInfo type in ass.types)
                {
                    int typeRid = (int)type.newType.Rid;
                    int oldTypeVersion = type.oldType != null ? oldMetaVerFile.GetTypeDefVersion((int)type.oldType.Rid).version : versionOfNewFileOrTypeOrMethod - 1;
                    bool typeChanged = !type.layoutEqual;
                    anyChange = anyChange || typeChanged;
                    int newTypeVersion = !typeChanged ? oldTypeVersion : versionOfNewFileOrTypeOrMethod;
                    int typeSignatureId = _signatureToIdMapper.GetOrAddSignature(SignatureMapper.ComputeTypeDefSignature(type.newType));
                    typeVerMap.Add(typeRid, new TypeDefVersion
                    {
                        version = newTypeVersion,
                        nameId = typeSignatureId,
                    });
                    if (typeSignatureId2TypeDefs.TryGetValue(typeSignatureId, out var existingType))
                    {
                        Debug.LogError($"Duplicate type definition found: {typeSignatureId} for {type.newType.FullName} and {existingType.FullName}");
                    }
                    else
                    {
                        typeSignatureId2TypeDefs.Add(typeSignatureId, type.newType);
                    }
                    var typeMetaSpec = new TypeMetaSpec
                    {
                        fullName = type.newType.FullName,
                        token = (int)type.newType.MDToken.Raw,
                        version = newTypeVersion,
                    };
                    typeVerSpecMap.Add(typeRid, typeMetaSpec);
                    if (typeChanged)
                    {
                        changedTypeVerSpecs.Add(typeMetaSpec);
                    }

                    foreach (MethodDiffInfo method in type.methods)
                    {
                        int methodRid = (int)method.newMethod.Rid;
                        int oldMethodVersion = method.oldMethod != null ? oldMetaVerFile.GetMethodDefVersion((int)method.oldMethod.Rid).version : versionOfNewFileOrTypeOrMethod - 1;
                        bool methodChanged = !method.fullEqual;
                        anyChange = anyChange || methodChanged;
                        int newMethodVersion = !methodChanged ? oldMethodVersion : versionOfNewFileOrTypeOrMethod;
                        int methodSignatureId = _signatureToIdMapper.GetOrAddSignature(SignatureMapper.ComputeMethodDefSignature(method.newMethod));
                        methodVerMap.Add(methodRid, new MethodDefVersion
                        {
                            version = newMethodVersion,
                            signatureId = methodSignatureId,
                        });
                        if (methodSignatureId2MethodDefs.TryGetValue(methodSignatureId, out var existingMethod))
                        {
                            Debug.LogError($"Duplicate method definition found: {methodSignatureId} for {method.newMethod.FullName} and {existingMethod.FullName}");
                        }
                        else
                        {
                            methodSignatureId2MethodDefs.Add(methodSignatureId, method.newMethod);
                        }
                        var methodMetaSpec = new MethodMetaSpec
                        {
                            fullName = method.newMethod.FullName,
                            token = (int)method.newMethod.MDToken.Raw,
                            version = newMethodVersion,
                        };
                        methodVerSpecMap.Add(methodRid, methodMetaSpec);
                        if (methodChanged)
                        {
                            changedMethodVerSpecs.Add(methodMetaSpec);
                        }
                    }
                }

                ModuleDefMD newMod = _newSnapshot.AssemblyCache.LoadModule(assName);
                int assTypeCount = newMod.GetTypes().Count();
                Assert.AreEqual(assTypeCount, typeVerMap.Count);
                int assMethodCount = newMod.GetTypes().Sum(t => t.Methods.Count);
                Assert.AreEqual(assMethodCount, methodVerMap.Count);

                var typeVers = new TypeDefVersion[typeVerMap.Count];
                var typeVerSpecs = new TypeMetaSpec[typeVerMap.Count];
                foreach (var e in typeVerMap)
                {
                    int rid = e.Key;
                    typeVers[rid - 1] = e.Value;
                    typeVerSpecs[rid - 1] = typeVerSpecMap[rid];
                }


                var methodVers = new MethodDefVersion[methodVerMap.Count];
                var methodVerSpecs = new MethodMetaSpec[methodVerMap.Count];
                foreach (var e in methodVerMap)
                {
                    int rid = e.Key;
                    methodVers[rid - 1] = e.Value;
                    methodVerSpecs[rid - 1] = methodVerSpecMap[rid];
                }

                var metaVersionFile = new MetaVersionFile
                {
                    fileVersion = anyChange ? versionOfNewFileOrTypeOrMethod : oldMetaVerFile.fileVersion,
                    typeDefVersions = typeVers,
                    methodDefVersions = methodVers,
                };
                if (!anyChange)
                {
                    Debug.Log($"assembly no change. ass:{assName} version:{metaVersionFile.fileVersion}");
                }
                Debug.Log($"GenerateData. assembly:{assName} changedTypeCount:{changedTypeVerSpecs.Count} changedMethodCount:{changedMethodVerSpecs.Count}");

                string outputMetaVerFile = $"{outputDir}/{assName}.mv.bytes";
                metaVersionFile.WriteFile(outputMetaVerFile);


                var metaVersionSpecFile = new MetaVersionSpecFile
                {
                    fileVersion = metaVersionFile.fileVersion,
                    typeSpecs = typeVerSpecs.ToList(),
                    methodSpecs = methodVerSpecs.ToList(),
                };
                string outputMetaVerSpecFile = $"{outputDir}/{assName}.mv.spec";
                metaVersionSpecFile.WriteFile(outputMetaVerSpecFile);

                var diffMetaVersionSpecFile = new MetaVersionSpecFile
                {
                    fileVersion = metaVersionFile.fileVersion,
                    typeSpecs = changedTypeVerSpecs,
                    methodSpecs = changedMethodVerSpecs,
                };
                string outputDiffMetaVerSpecFile = $"{outputDir}/{assName}.mv.diff.spec";
                diffMetaVersionSpecFile.WriteFile(outputDiffMetaVerSpecFile);
            }
        }
    }
}
