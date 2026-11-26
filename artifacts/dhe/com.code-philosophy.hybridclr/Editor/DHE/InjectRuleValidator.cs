using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridCLR.Editor.DHE
{
    public class InjectRuleValidator
    {
        private readonly List<string> _injectRuleFiles;
        private readonly List<string> _dheAssemblyNames;

        public InjectRuleValidator(List<string> injectRuleFiles,  List<string> dheAssemblyNames)
        {
            _injectRuleFiles = injectRuleFiles;
            _dheAssemblyNames = dheAssemblyNames;
        }

        public bool Validate()
        {
            var injectRules = new InjectRules();
            injectRules.LoadFromXmlFiles(_injectRuleFiles);

            foreach (string dheDllPath in _dheAssemblyNames)
            {
                using (var dheMod = ModuleDefMD.Load(dheDllPath))
                {
                    for (uint i = 1, n = dheMod.Metadata.TablesStream.MethodTable.Rows; i <= n; i++)
                    {
                        MethodDef methodDef = dheMod.ResolveMethod(i);
                        injectRules.IsNotInjectMethod(methodDef);
                    }
                }
            }
            bool existRuleItemNotMatchAny = false;

            foreach (var assConfig in injectRules.Assemblies)
            {
                foreach (var typeConfig in assConfig.types)
                {
                    foreach (var methodConfig in typeConfig.methods)
                    {
                        if (methodConfig.matchCount == 0)
                        {
                            UnityEngine.Debug.LogError($"[InjectRuleValidator] assembly:{assConfig.namePattern.NameOrPattern} type:{typeConfig.namePattern.NameOrPattern} method:{methodConfig.namePattern.NameOrPattern} matchCount == 0");
                            existRuleItemNotMatchAny = true;
                        }
                    }
                }
            }
            return !existRuleItemNotMatchAny;
        }
    }
}
