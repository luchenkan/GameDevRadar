using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HybridCLR.Editor.DHE
{


    public class TypeDiffInfo
    {
        public TypeDef oldType;
        public TypeDef newType;
        public bool layoutEqual;
        public bool referenceEqual;
        public bool staticLayoutEqual;
        public List<MethodDiffInfo> methods = new List<MethodDiffInfo>();
    }

    public class MethodDiffInfo
    {
        public MethodDef oldMethod;
        public MethodDef newMethod;
        public bool fullEqual;
        public bool signatureEqual;
        public bool bodyEqual;
    }

    public class AssemblyDiffInfo
    {
        public string name;
        public ModuleDefMD oldModule;
        public ModuleDefMD newModule;
        public List<TypeDiffInfo> types = new List<TypeDiffInfo>();
    }

    public class CodeDiffAnalyzer
    {
        private readonly Snapshot _old;
        private readonly Snapshot _new;

        public Snapshot Old => _old;

        public Snapshot New => _new;


        private readonly TypeCompareCache _typeCompareCache;
        private readonly FieldCompareCache _fieldCompareCache;
        private readonly MethodCompareCache _methodCompareCache;



        private List<AssemblyDiffInfo> _assemblies = new List<AssemblyDiffInfo>();

        public IReadOnlyList<AssemblyDiffInfo> AssemblyDiffInfos => _assemblies;

        public CodeDiffAnalyzer(Snapshot old, Snapshot @new)
        {
            _old = old;
            _new = @new;
            _typeCompareCache = new TypeCompareCache(old, @new);
            _fieldCompareCache = new FieldCompareCache(_typeCompareCache);
            _methodCompareCache = new MethodCompareCache(_typeCompareCache, _fieldCompareCache);
            Validation();
        }

        private void Validation()
        {
            if (!_old.DheAssemblyNames.SequenceEqual(_new.DheAssemblyNames))
            {
                throw new Exception("dhe assembly names doesn't match");
            }
        }

        private void PrepareDiffData()
        {
            foreach (string assName in _new.DheAssemblyNames)
            {
                ModuleDefMD oldMod = _old.AssemblyCache.LoadModule(assName);
                ModuleDefMD newMod = _new.AssemblyCache.LoadModule(assName);
                var assDiffData = new AssemblyDiffInfo
                {
                    name = assName,
                    oldModule = oldMod,
                    newModule = newMod,
                };
                foreach (var newType in newMod.GetTypes())
                {
                    uint token = newType.MDToken.Raw;
                    TypeDef oldType = oldMod.Find(newType.FullName, false);
                    var typeDiffData = new TypeDiffInfo { oldType = oldType, newType = newType };
                    assDiffData.types.Add(typeDiffData);
                    foreach (var newMethod in newType.Methods)
                    {
                        MethodDef oldMethod = oldType != null ? FindMatchMethod(newMethod, oldType) : null;
                        var methodDiffData = new MethodDiffInfo() { oldMethod = oldMethod, newMethod = newMethod };
                        typeDiffData.methods.Add(methodDiffData);
                    }
                }
                _assemblies.Add(assDiffData);
            }
        }

        public void RunAnalyze()
        {
            PrepareDiffData();
            AnalyzeTypeDiffs();
            AnalyzeMethodDiffs();
        }

        private MethodDef FindMatchMethod(MethodDef newMethod, TypeDef oldType)
        {
            foreach (var oldMethod in oldType.Methods)
            {
                if (newMethod.Name != oldMethod.Name)
                {
                    continue;
                }

                if (MethodEqualityComparer.DontCompareDeclaringTypes.Equals(newMethod, oldMethod))
                {
                    return oldMethod;
                }
            }
            return null;
        }

        private void AnalyzeTypeDiffs()
        {
            foreach (var assDiffData in _assemblies)
            {
                foreach (var typeDiffData in assDiffData.types)
                {
                    TypeDef oldType = typeDiffData.oldType;
                    TypeDef newType = typeDiffData.newType;
                    if (oldType == null)
                    {
                        typeDiffData.layoutEqual = false;
                        typeDiffData.referenceEqual = false;
                        typeDiffData.staticLayoutEqual = false;
                    }
                    else
                    {
                        var instCmpData = _typeCompareCache.GetInstanceCompareData(oldType, newType);
                        var staticCmpData = _typeCompareCache.GetStaticCompareData(oldType, newType);
                        typeDiffData.layoutEqual = instCmpData.layoutAndTypeSigEqual;
                        typeDiffData.referenceEqual= instCmpData.referenceEqual;
                        typeDiffData.staticLayoutEqual = staticCmpData.layoutEqual;
                    }
                }
            }
        }

        private void AnalyzeMethodDiffs()
        {
            foreach (var assDiffData in _assemblies)
            {
                foreach (var typeDiffData in assDiffData.types)
                {
                    foreach (var methodDiffData in typeDiffData.methods)
                    {
                        if (methodDiffData.oldMethod != null)
                        {
                            _methodCompareCache.CompareMethodFully(methodDiffData.oldMethod, methodDiffData.newMethod);
                        }
                    }
                }
            }

            foreach (var assDiffData in _assemblies)
            {
                foreach (var typeDiffData in assDiffData.types)
                {
                    foreach (var methodDiffData in typeDiffData.methods)
                    {
                        if (methodDiffData.oldMethod != null)
                        {
                            var cmpData = _methodCompareCache.GetCompareData(methodDiffData.oldMethod, methodDiffData.newMethod);
                            methodDiffData.fullEqual = cmpData.fullEqual;
                            methodDiffData.signatureEqual = cmpData.signatureEqual;
                            methodDiffData.bodyEqual = cmpData.bodyEqual;
                        }
                        else
                        {
                            methodDiffData.fullEqual = false;
                            methodDiffData.signatureEqual = false;
                            methodDiffData.bodyEqual = false;
                        }
                    }
                }
            }
        }
    }
}
