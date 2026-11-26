using dnlib.DotNet;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using IAssemblyResolver = HybridCLR.Editor.Meta.IAssemblyResolver;

namespace HybridCLR.Editor.BuildProcessors
{
    internal class CheckHotUpdateAssemblyReferencedByAOTAssembly : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Check();
        }


        static void WalkCheck(string modName, IAssemblyResolver resolver, AssemblyCache assCache, HashSet<string> hotUpdateAsses, HashSet<string> visitedAss)
        {
            //Debug.Log($"walk {modName}");
            if (visitedAss.Contains(modName))
            {
                return;
            }
            visitedAss.Add(modName);

            if (hotUpdateAsses.Contains(modName))
            {
                return;
            }

            string assPath = resolver.ResolveAssembly(modName, false);
            if (string.IsNullOrEmpty(assPath))
            {
                return;
            }

            ModuleDefMD mod = assCache.LoadModule(modName, false);

            foreach (var refAss in mod.GetAssemblyRefs())
            {
                string refAssName = refAss.Name.String;
                if (hotUpdateAsses.Contains(refAssName))
                {
                    throw new BuildFailedException($"AOT程序集:'{modName}' 引用了热更新或DHE程序集:'{refAssName}'");
                }
                WalkCheck(refAssName, resolver, assCache, hotUpdateAsses, visitedAss);
            }
        }

        //[MenuItem("HybridCLR/CheckAssemblyReference")]
        static void Check()
        {
            var hotUpdateAsses = new HashSet<string>(SettingsUtil.HotUpdateAndDHEAssemblyNamesExcludePreserved);
            var assResolver = new PathAssemblyResolver(SettingsUtil.GetHotUpdateDllsOutputDirByTarget(UnityEditor.EditorUserBuildSettings.activeBuildTarget));

            var visitedAsses = new HashSet<string>();
            var assCollector = new AssemblyCache(assResolver);
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assName = ass.GetName().Name;
                WalkCheck(assName, assResolver, assCollector, hotUpdateAsses, visitedAsses);
            }
        }
    }
}
