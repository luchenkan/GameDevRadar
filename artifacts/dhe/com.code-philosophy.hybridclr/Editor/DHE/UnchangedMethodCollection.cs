using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HybridCLR.Editor.DHE
{
    public class UnchangedMethodCollection
    {
        private readonly HashSet<MethodDef> _unchangedMethods = new HashSet<MethodDef>(MethodEqualityComparer.CompareDeclaringTypes);

        public bool IsUnchangedMethod(MethodDef method)
        {
            return _unchangedMethods.Contains(method);
        }


        private bool HasUnchangedAttribute(CustomAttributeCollection cac)
        {
            var attr = cac.Where(a => a.AttributeType.FullName == "HybridCLR.Runtime.UnchangedAttribute").FirstOrDefault();
            if (attr != null)
            {
                return (bool)attr.ConstructorArguments[0].Value;
            }
            return false;
        }

        public void InitUnchangedMethods(Snapshot newSnapshot)
        {
            foreach (string assName in newSnapshot.DheAssemblyNames)
            {
                ModuleDef mod = newSnapshot.AssemblyCache.LoadModule(assName);
                foreach (var type in mod.GetTypes())
                {
                    foreach (var method in type.Methods)
                    {
                        if (HasUnchangedAttribute(method.CustomAttributes))
                        {
                            _unchangedMethods.Add(method);
                            Debug.Log($"[UnchangedMethodCollection]  unchanged method:{method} {method.MDToken.Raw}");
                        }
                    }
                }
            }
        }
    }
}
