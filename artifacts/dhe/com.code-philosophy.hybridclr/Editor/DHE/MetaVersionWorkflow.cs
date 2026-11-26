using HybridCLR.Editor.Encryption;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor.Meta;
using HybridCLR.Editor.Settings;
using HybridCLR.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HybridCLR.Editor.DHE
{

    public static class MetaVersionWorkflow
    {

        private static void BackupInjectRules(string dstDir)
        {
            BashUtil.RecreateDir(dstDir);
            if (SettingsUtil.InjectRulesFiles == null)
            {
                return;
            }
            foreach (var injectRuleFile in SettingsUtil.InjectRulesFiles)
            {
                string dstFile = $"{dstDir}/{Path.GetFileName(injectRuleFile)}";
                File.Copy(injectRuleFile, dstFile, true);
                Debug.Log($"[BackupInjectRules] {injectRuleFile} --> {dstFile}");
            }
        }

        public static void CreateAotSnapshot(BuildTarget target, string outputSnapshotDir)
        {
            BashUtil.CopyDir(SettingsUtil.GetAssembliesPostIl2CppStripDir(target), outputSnapshotDir, true);
            MetaVersionWorkflow.BackupInjectRules(Snapshot.GetInjectRuleDir(outputSnapshotDir));

            var manifest = new SnapshotManifest { differentialHybridAssemblyNames = SettingsUtil.DifferentialHybridAssemblyNames };
            manifest.Save(Snapshot.GetManifestFile(outputSnapshotDir));
        }

        public static void GenerateAotSnapshotMetaVersionFiles(string prevSnapshotDir, string curSnapshotDir)
        {
            if (prevSnapshotDir == null)
            {
                int initialVersion = 0;
                var snapshot = new Snapshot(curSnapshotDir, SnapshotMode.AOT, null);
                var g = new FirstSnapshotMetaVersionFileGenerator(snapshot, initialVersion);
                g.Generate();
            }
            else
            {
                var prevSnapshot = new Snapshot(prevSnapshotDir, SnapshotMode.AOT, null);
                var curSnapshot = new Snapshot(curSnapshotDir, SnapshotMode.AOT, null);
                var g = new SnapshotMetaVersionFileGenerator(prevSnapshot, curSnapshot);
                g.Generate();
            }
        }

        public static void GenerateHotUpdateMetaVersionFiles(string aotSnapshotDir, string hotUpdateSnapshotDir)
        {
            var prevSnapshot = new Snapshot(aotSnapshotDir, SnapshotMode.AOT, null);
            var curSnapshot = new Snapshot(hotUpdateSnapshotDir, SnapshotMode.HotUpdate, prevSnapshot);
            var g = new SnapshotMetaVersionFileGenerator(prevSnapshot, curSnapshot);
            g.Generate();
        }
    }
}
