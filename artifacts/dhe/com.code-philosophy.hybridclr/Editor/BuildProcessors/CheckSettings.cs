using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace HybridCLR.Editor.BuildProcessors
{
    internal class CheckSettings : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public static bool DisableMethodBridgeDevelopmentFlagChecking { get; set; }

        
        private static bool ContainsByteSeq(byte[] bytes, byte[] searchBytes)
        {
            for (int i = 0, end = bytes.Length - searchBytes.Length; i < end; i++)
            {
                bool match = true;
                for (int j = 0; j < searchBytes.Length; j++)
                {
                    if (bytes[i + j] != searchBytes[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasReplacedUnityIl2CppDllWithDHEEdition()
        {
            try
            {
                byte[] searchBytes1 = Encoding.UTF8.GetBytes("HybridCLRConfig");
                byte[] dllBytes = File.ReadAllBytes(SettingsUtil.UnityIl2CppDllPath);
                return ContainsByteSeq(dllBytes, searchBytes1);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }



        private static bool DoesUnityIl2CppDllSupportInjectRules()
        {
            try
            {
                byte[] searchBytes2 = Encoding.UTF8.GetBytes("InjectRules");
                byte[] dllBytes = File.ReadAllBytes(SettingsUtil.UnityIl2CppDllPath);
                return ContainsByteSeq(dllBytes, searchBytes2);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            HybridCLRSettings globalSettings = SettingsUtil.HybridCLRSettings;
            if (!globalSettings.enable || globalSettings.useGlobalIl2cpp)
            {
                string oldIl2cppPath = Environment.GetEnvironmentVariable("UNITY_IL2CPP_PATH");
                if (!string.IsNullOrEmpty(oldIl2cppPath))
                {
                    Environment.SetEnvironmentVariable("UNITY_IL2CPP_PATH", "");
                    Debug.Log($"[CheckSettings] clean process environment variable: UNITY_IL2CPP_PATH, old vlaue:'{oldIl2cppPath}'");
                }
            }
            else
            {
                string curIl2cppPath = Environment.GetEnvironmentVariable("UNITY_IL2CPP_PATH");
                if (curIl2cppPath != SettingsUtil.LocalIl2CppDir)
                {
                    Environment.SetEnvironmentVariable("UNITY_IL2CPP_PATH", SettingsUtil.LocalIl2CppDir);
                    Debug.Log($"[CheckSettings] UNITY_IL2CPP_PATH old value:'{curIl2cppPath}'， new value:'{SettingsUtil.LocalIl2CppDir}'");
                }
            }
            if (!globalSettings.enable)
            {
                return;
            }
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
#if UNITY_6000_0_OR_NEWER
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            ScriptingImplementation curScriptingImplementation = PlayerSettings.GetScriptingBackend(namedBuildTarget);
#else
            ScriptingImplementation curScriptingImplementation = PlayerSettings.GetScriptingBackend(buildTargetGroup);
#endif
            ScriptingImplementation targetScriptingImplementation = ScriptingImplementation.IL2CPP;
            if (curScriptingImplementation != targetScriptingImplementation)
            {
                Debug.LogError($"[CheckSettings] current ScriptingBackend:{curScriptingImplementation}，have been switched to:{targetScriptingImplementation} automatically");
#if UNITY_6000_0_OR_NEWER
                PlayerSettings.SetScriptingBackend(namedBuildTarget, targetScriptingImplementation);
#else
                PlayerSettings.SetScriptingBackend(buildTargetGroup, targetScriptingImplementation);
#endif
            }

            var installer = new Installer.InstallerController();
            if (!installer.HasInstalledHybridCLR())
            {
                throw new BuildFailedException($"You have not initialized HybridCLR, please install it via menu 'HybridCLR/Installer'");
            }

            if (installer.PackageVersion != installer.InstalledLibil2cppVersion)
            {
                throw new BuildFailedException($"You must run `HybridCLR/Installer` after upgrading package");
            }

            HybridCLRSettings gs = SettingsUtil.HybridCLRSettings;
            if (((gs.hotUpdateAssemblies?.Length + gs.hotUpdateAssemblyDefinitions?.Length) ?? 0) == 0)
            {
                Debug.LogWarning("[CheckSettings] No hot update modules configured in HybridCLRSettings");
            }

            if (!DisableMethodBridgeDevelopmentFlagChecking)
            {
                string methodBridgeFile = $"{SettingsUtil.GeneratedCppDir}/MethodBridge.cpp";
                var match = Regex.Match(File.ReadAllText(methodBridgeFile), @"// DEVELOPMENT=(\d)");
                if (match.Success)
                {
                    int developmentFlagInMethodBridge = int.Parse(match.Groups[1].Value);
                    int developmentFlagInEditorSettings = EditorUserBuildSettings.development ? 1 : 0;
                    if (developmentFlagInMethodBridge != developmentFlagInEditorSettings)
                    {
                        Debug.LogError($"[CheckSettings] MethodBridge.cpp DEVELOPMENT flag:{developmentFlagInMethodBridge} is inconsistent with EditorUserBuildSettings.development:{developmentFlagInEditorSettings}. Please run 'HybridCLR/Generate/All' before building.");
                    }
                }
                else
                {
                    Debug.LogError("[CheckSettings] MethodBridge.cpp DEVELOPMENT flag not found. Please run 'HybridCLR/Generate/All' before building.");
                }
            }
#if UNITY_2023_1_OR_NEWER
            string deploy2022Dir = $"{SettingsUtil.LocalIl2CppDir}/build/deploy/deploy-2022";
            if (!Directory.Exists(deploy2022Dir))
            {
                throw new BuildFailedException($"{deploy2022Dir} 不存在，请复制Unity 2022.3.54+ Editor安装目录下的`il2cpp/build/deploy`到`{deploy2022Dir}`");
            }
#endif
            if (!HasReplacedUnityIl2CppDllWithDHEEdition())
            {
                throw new BuildFailedException($"{SettingsUtil.UnityIl2CppDllPath} 是原始版本，请替换为DHE版本");
            }
            if (!DoesUnityIl2CppDllSupportInjectRules())
            {
                throw new BuildFailedException($"{SettingsUtil.UnityIl2CppDllPath} 不支持InjectRules，请替换为支持InjectRules的DHE版本");
            }

            string dheAssemblyEnvStr = string.Join(",", gs.differentialHybridAssemblies ?? Array.Empty<string>());
            Debug.Log($"[CheckSettings] DHE_ASSEMBLIES:{dheAssemblyEnvStr}");
            Environment.SetEnvironmentVariable("DHE_ASSEMBLIES", dheAssemblyEnvStr);

            string injectRuleFilesEnvKey = "INJECT_RULE_FILES";
            string injectRuleFilesEnvStr = string.Join(",", gs.injectRuleFiles?.Select(f => Path.GetFullPath(f)) ?? Array.Empty<string>());
            Debug.Log($"[CheckSettings] {injectRuleFilesEnvKey}:{injectRuleFilesEnvStr}");
            Environment.SetEnvironmentVariable(injectRuleFilesEnvKey, injectRuleFilesEnvStr);
        }
    }
}
