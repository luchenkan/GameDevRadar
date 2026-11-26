using HybridCLR.Editor.Encryption;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor.Meta;
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
    public static class DhaoWorkflow
    {
        public static void CreateAotSnapshot(BuildTarget target, string outputSnapshotDir)
        {
            MetaVersionWorkflow.CreateAotSnapshot(target, outputSnapshotDir);
        }

        public static void GenerateUnchangedDhaoFiles(string aotSnapshotDir, string dhaoOutputDir)
        {
            System.IO.Directory.CreateDirectory(dhaoOutputDir);
            var dheAssemblyNames = SnapshotManifest.Load(Snapshot.GetManifestFile(aotSnapshotDir)).differentialHybridAssemblyNames;
            foreach (string assName in dheAssemblyNames)
            {
                string outOptionFile = $"{dhaoOutputDir}/{assName}.dhao.bytes";
                string dllMd5 = DifferentialHybridAssemblyOptionFileGenerator.CreateMD5Hash(File.ReadAllBytes($"{aotSnapshotDir}/{assName}.dll"));

                var dhaOptions = new DifferentialHybridAssemblyOptions()
                {
                    ForceAllChanged = false,
                    ChangedMethodTokens = new List<uint>(),
                    ChangedStructTokens = new List<uint>(),
                    OldDllMD5 = dllMd5,
                    NewDllMD5 = dllMd5,
                };
                File.WriteAllBytes(outOptionFile, dhaOptions.Marshal());
                Debug.Log($"GenerateUnchangedDhaoFiles assembly:{assName} md5:{dllMd5} output:{outOptionFile}");
            }
        }

        public static void GenerateDhaoFiles(string aotSnapshotDir, string hotUpdateSnapshotDir, string dhaoOutputDir)
        {
            var oldSnapshot = new Snapshot(aotSnapshotDir, SnapshotMode.AOT, null);
            var newSnapshot = new Snapshot(hotUpdateSnapshotDir, SnapshotMode.HotUpdate, oldSnapshot);
            var g = new DifferentialHybridAssemblyOptionFileGenerator(oldSnapshot, newSnapshot, dhaoOutputDir);
            g.Generate();
        }

        public static void MergeDhaoFiles(string[] dhaoFiles, string outputDhaoFile)
        {
            var dhaoList = new List<DifferentialHybridAssemblyOptions>();
            foreach (var file in dhaoFiles)
            {
                var bytes = File.ReadAllBytes(file);
                var opt = new DifferentialHybridAssemblyOptions();
                opt.Unmarshal(bytes);
                dhaoList.Add(opt);
            }

            var changeStructTokens = dhaoList.SelectMany(opt => opt.ChangedStructTokens).Distinct().ToList();
            var changeMethodTokens = dhaoList.SelectMany(opt => opt.ChangedMethodTokens).Distinct().ToList();

            var mergedOpt = new DifferentialHybridAssemblyOptions()
            {
                NewDllMD5 = dhaoList.First().NewDllMD5,
                OldDllMD5 = dhaoList.First().OldDllMD5,
                ChangedMethodTokens = changeMethodTokens,
                ChangedStructTokens = changeStructTokens,
                ForceAllChanged = dhaoList.Any(opt => opt.ForceAllChanged),
            };
            File.WriteAllBytes(outputDhaoFile, mergedOpt.Marshal());
            Debug.Log($"MergeDhaoFiles. output:{outputDhaoFile}");
        }

        public static HashSet<string> ComputeAssembliesLoadedByLoadDifferentialHybridAssembly(IEnumerable<string> changedDheAssemblyList, IEnumerable<string> allDheAssemblyList, string currentAssemblyDir, string[] currentExtraAssemblySearchDirs = null)
        {
            foreach (var assName in changedDheAssemblyList)
            {
                if (!allDheAssemblyList.Contains(assName))
                {
                    throw new Exception($"changedDheAssemblyList contains assembly not in allDheAssemblyList. assembly:{assName}");
                }
            }

            var resultAssemblies = new HashSet<string>(changedDheAssemblyList);

            var curSubResolvers = new List<IAssemblyResolver>()
            {
                new FixedSetAssemblyResolver(currentAssemblyDir, allDheAssemblyList),
                new PathAssemblyResolver(currentAssemblyDir), // for new assemblies
            };
            if (currentExtraAssemblySearchDirs != null)
            {
                curSubResolvers.Insert(1, new PathAssemblyResolver(currentExtraAssemblySearchDirs.ToArray()));
            }
            var curResolver = new CombinedAssemblyResolver(curSubResolvers.ToArray());
            var curAssCache = new AssemblyCache(curResolver);

            // build dependency graph
            var assRefAssemblies = new Dictionary<string, HashSet<string>>();
            foreach (var assName in allDheAssemblyList)
            {
                var refAssemblies = new HashSet<string>();
                var mod = curAssCache.LoadModule(assName, false);
                foreach (var refAss in mod.GetAssemblyRefs())
                {
                    if (allDheAssemblyList.Contains(refAss.Name.ToString()))
                    {
                        refAssemblies.Add(refAss.Name.ToString());
                    }
                }
                assRefAssemblies.Add(assName, refAssemblies);
            }

            bool anyChange = true;
            while (anyChange)
            {
                anyChange = false;

                foreach (var assName in allDheAssemblyList)
                {
                    if (resultAssemblies.Contains(assName))
                    {
                        continue;
                    }
                    if (assRefAssemblies[assName].Any(refAss => resultAssemblies.Contains(refAss)))
                    {
                        resultAssemblies.Add(assName);
                        anyChange = true;
                    }
                }
            }
            return resultAssemblies;
        }
    }
}
