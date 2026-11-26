using dnlib.DotNet;
using HybridCLR.Editor.Installer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HybridCLR.Editor.DHE
{

    public class FirstSnapshotMetaVersionFileGenerator
    {
        private readonly Snapshot _snapshot;
        private readonly int _initialVersion;
        private readonly SignatureMapper _signatureToIdMapper = new SignatureMapper();

        public FirstSnapshotMetaVersionFileGenerator(Snapshot snapshot, int initialVersion)
        {
            _snapshot = snapshot;
            _initialVersion = initialVersion;
        }

        public void Generate()
        {
            string outputDir = _snapshot.MetaVersionDir;
            BashUtil.RecreateDir(outputDir);
            foreach (var assName in _snapshot.DheAssemblyNames)
            {
                var mod = _snapshot.AssemblyCache.LoadModule(assName);
                SaveMetaVersionFile(assName, mod, outputDir);
                SaveMetaVersionSpecFile(assName, mod, outputDir);
            }
            _signatureToIdMapper.WriteFile(_snapshot.SignatureToIdMapFile);
        }

        private void SaveMetaVersionFile(string assName, ModuleDefMD mod, string outputDir)
        {

            var typeDefVersions = new TypeDefVersion[mod.GetTypes().Count()];

            var typeId2TypeDef = new Dictionary<int, TypeDef>();
            foreach (TypeDef type in mod.GetTypes())
            {
                int typeSignatureId = _signatureToIdMapper.GetOrAddSignature(SignatureMapper.ComputeTypeDefSignature(type));
                typeDefVersions[type.Rid - 1] = new TypeDefVersion
                {
                    version = _initialVersion,
                    nameId = typeSignatureId,
                };
                if (typeId2TypeDef.TryGetValue(typeSignatureId, out var existingType))
                {
                    Debug.LogError($"Duplicate type definition found: {typeSignatureId} for {type.FullName} and {existingType.FullName}");
                }
                else
                {
                    typeId2TypeDef[typeSignatureId] = type;
                }
            }

            int methodDefCount = mod.GetTypes().Sum(t => t.Methods.Count);
            var methodDefVersions = new MethodDefVersion[methodDefCount];
            var methodId2MethodDef = new Dictionary<int, MethodDef>();
            foreach (var type in mod.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    int signatureId = _signatureToIdMapper.GetOrAddSignature(SignatureMapper.ComputeMethodDefSignature(method));
                    methodDefVersions[method.Rid - 1] = new MethodDefVersion
                    {
                        version = _initialVersion,
                        signatureId = signatureId,
                    };
                    if (methodId2MethodDef.TryGetValue(signatureId, out var existingMethod))
                    {
                        Debug.LogError($"Duplicate method definition found: {signatureId} for {method.FullName} and {existingMethod.FullName}");
                    }
                    else
                    {
                        methodId2MethodDef[signatureId] = method;
                    }
                }
            }

            var metaVersionFile = new MetaVersionFile
            {
                fileVersion = _initialVersion,
                typeDefVersions = typeDefVersions,
                methodDefVersions = methodDefVersions,
            };
            string outputFile = $"{outputDir}/{assName}.mv.bytes";
            metaVersionFile.WriteFile(outputFile);
            Debug.Log($"SaveMetaVersionFile {outputFile}");
        }

        private void SaveMetaVersionSpecFile(string assName, ModuleDefMD mod, string outputDir)
        {
            var typeSpecs = new List<TypeMetaSpec>();
            foreach (var type in mod.GetTypes())
            {
                var typeSpec = new TypeMetaSpec
                {
                    fullName = type.FullName,
                    token = (int)type.MDToken.Raw,
                    version = _initialVersion,
                };
                typeSpecs.Add(typeSpec);
            }
            var methodSpecs = new List<MethodMetaSpec>();
            foreach (var type in mod.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    var methodSpec = new MethodMetaSpec
                    {
                        fullName = method.FullName,
                        token = (int)method.MDToken.Raw,
                        version = _initialVersion,
                    };
                    methodSpecs.Add(methodSpec);
                }
            }
            var metaVersionSpecFile = new MetaVersionSpecFile
            {
                fileVersion = _initialVersion,
                typeSpecs = typeSpecs,
                methodSpecs = methodSpecs,
            };
            string outputFilePath = $"{outputDir}/{assName}.mv.spec";
            metaVersionSpecFile.WriteFile(outputFilePath);
        }
    }
}
