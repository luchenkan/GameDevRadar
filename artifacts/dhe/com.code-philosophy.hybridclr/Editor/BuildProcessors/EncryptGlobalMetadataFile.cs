using HybridCLR.Editor.UnityBinFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;
using UnityFS;
using HybridCLR.Editor.Encryption;
using HybridCLR.Editor.Settings;


#if !UNITY_2023_1_OR_NEWER
using UnityEditor.Il2Cpp;
#endif

namespace HybridCLR.Editor.BuildProcessors
{
    public class EncryptGlobalMetadataFile :
#if UNITY_ANDROID
        IPostGenerateGradleAndroidProject,
#elif UNITY_OPENHARMONY
        UnityEditor.OpenHarmony.IPostGenerateOpenHarmonyProject,
#endif
        IPostprocessBuildWithReport
#if !UNITY_2021_1_OR_NEWER && UNITY_WEBGL
        , IIl2CppProcessor
#endif

#if UNITY_PS5
        , IUnityLinkerProcessor
#endif

    {
        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            // 如果直接打包apk，没有机会在PostprocessBuild中修改ScriptingAssemblies.json。
            // 因此需要在这个时机处理
            // Unity有bug，偶然情况下会传入apk的路径，导致替换失败
            if (Directory.Exists(path))
            {
                ApplyEncryptGlobalMetadataFile(path);
            }
            else
            {
                ApplyEncryptGlobalMetadataFile($"{SettingsUtil.ProjectDir}/Library");
            }
        }

#if UNITY_OPENHARMONY

        public void OnPostGenerateOpenHarmonyProject(string path)
        {
            OnPostGenerateGradleAndroidProject(path);
        }

#endif

        public void OnPostprocessBuild(BuildReport report)
        {
            // 如果target为Android,由于已经在OnPostGenerateGradelAndroidProject中处理过，
            // 这里不再重复处理
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_OPENHARMONY
            ApplyEncryptGlobalMetadataFile(report.summary.outputPath);
#endif
        }

#if UNITY_PS5
        /// <summary>
        /// 打包模式如果是 Package 需要在这个阶段提前处理 .json , PC Hosted 和 GP5 模式不受影响
        /// </summary>

        public string GenerateAdditionalLinkXmlFile(UnityEditor.Build.Reporting.BuildReport report, UnityEditor.UnityLinker.UnityLinkerBuildPipelineData data)
        {
            string path = $"{SettingsUtil.ProjectDir}/Library/PlayerDataCache/PS5/Data"; 
            ApplyEncryptGlobalMetadataFile(path);
            return null;
        }
#endif
        public void ApplyEncryptGlobalMetadataFile(string path)
        {
            if (!SettingsUtil.Enable)
            {
                Debug.Log($"[EncryptionGlobalMetadataFile] disabled");
                return;
            }
            EncryptionSettings settings = SettingsUtil.EncryptionSettings;
            if (!settings.encryptGlobalMetadataData)
            {
                Debug.Log($"[EncryptionGlobalMetadataFile] EncryptGlobalMetadataFile disabled");
                return;
            }
            Debug.Log($"[EncryptionGlobalMetadataFile]. path:{path}");
            if (!Directory.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                Debug.Log($"[EncryptionGlobalMetadataFile] get path parent:{path}");
            }

            var encryptor = new GlobalMetadataDatEncryptor(settings.vmSeed, settings.key);

            foreach (var file in Directory.GetFiles(path, "global-metadata.dat", SearchOption.AllDirectories))
            {
                encryptor.Encrypt(file, file);
            }
        }


#if UNITY_WEBGL && !UNITY_2022_3_OR_NEWER
        public void OnBeforeConvertRun(BuildReport report, Il2CppBuildPipelineData data)
        {
            ApplyEncryptGlobalMetadataFile($"{SettingsUtil.ProjectDir}/Temp/StagingArea/Data");
        }
#endif
    }
}
