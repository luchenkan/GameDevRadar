using HybridCLR.Editor.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HybridCLR.Editor.DHE
{
    public enum SnapshotMode
    {
        AOT,
        HotUpdate,
    }

    [Serializable]
    public class SnapshotManifest
    {
        public List<string> differentialHybridAssemblyNames;


        public void Save(string outputFile)
        {
            File.WriteAllText(outputFile, JsonUtility.ToJson(this), Encoding.UTF8);
            Debug.Log($"SaveSnapshotManifest {outputFile}");
        }

        public static SnapshotManifest Load(string inputFile)
        {
            string json = File.ReadAllText(inputFile, Encoding.UTF8);
            return JsonUtility.FromJson<SnapshotManifest>(json);
        }
    }

    public class SnapshotAssemblyResolver : IAssemblyResolver
    {
        private readonly Snapshot _self;
        private readonly Snapshot _base;

        public SnapshotAssemblyResolver(Snapshot self, Snapshot @base)
        {
            _self = self;
            _base = @base;
        }

        public string ResolveAssembly(string assemblyName, bool throwExIfNotFind)
        {
            string assemblyFile = $"{_self.SnapshotDir}/{assemblyName}.dll";
            // if aot snapshot, find dll in rootDir
            if (_base == null)
            {
                if (File.Exists(assemblyFile))
                {
                    return assemblyFile;
                }
            }
            // if hot update snapshot, find dhe assembly in current directory
            else if (_self.DheAssemblyNames.Contains(assemblyName))
            {
                if (File.Exists(assemblyFile))
                {
                    return assemblyFile;
                }
            }
            // if not, find aot assembly in root dir of base snapshot
            else
            {
                string aotAssemblyFile = $"{_base.SnapshotDir}/{assemblyName}.dll";
                if (File.Exists(aotAssemblyFile))
                {
                    return aotAssemblyFile;
                }
                string extraNewAssemblyFile = $"{_self.SnapshotDir}/{assemblyName}.dll";
                if (File.Exists(extraNewAssemblyFile))
                {
                    return extraNewAssemblyFile;
                }
            }
            if (throwExIfNotFind)
            {
                throw new Exception($"resolve assembly:{assemblyName} failed");
            }
            return null;
        }
    }

    public class Snapshot
    {
        private readonly List<string> _dheAssemblyNames;

        private readonly string _snapshotDir;

        private readonly Snapshot _baseSnapshot;

        private readonly SnapshotMode _snapshotMode;

        private readonly AssemblyCache _assemblyCache;

        public Snapshot(string snapshotDir, SnapshotMode snapshotMode, Snapshot baseSnapshot)
        {
            _dheAssemblyNames = snapshotMode == SnapshotMode.AOT ? SnapshotManifest.Load(GetManifestFile(snapshotDir)).differentialHybridAssemblyNames : baseSnapshot._dheAssemblyNames;
            _snapshotDir = snapshotDir;
            _snapshotMode = snapshotMode;
            _baseSnapshot = baseSnapshot;

            UnityEngine.Debug.Assert(snapshotMode == SnapshotMode.AOT ^ baseSnapshot != null);
            // must be initialized after initializing fields.
            _assemblyCache = new AssemblyCache(new SnapshotAssemblyResolver(this, baseSnapshot));
        }

        public IReadOnlyList<string> DheAssemblyNames => _dheAssemblyNames;

        public string SnapshotDir => _snapshotDir;

        public SnapshotMode SnapshotMode => _snapshotMode;

        public Snapshot BaseSnapshot => _baseSnapshot;

        public AssemblyCache AssemblyCache => _assemblyCache;

        public string InjectRuleDir => GetInjectRuleDir(_snapshotDir);

        public string ManifestFile => GetManifestFile(_snapshotDir);

        public string MetaVersionDir => GetMetaVersionDir(_snapshotDir);

        public string SignatureToIdMapFile => $"{_snapshotDir}/signature-mapper.json";

        public static string GetManifestFile(string snapshotDir)
        {
            return $"{snapshotDir}/manifest.json";
        }

        public static string GetInjectRuleDir(string snapshotDir)
        {
            return $"{snapshotDir}/InjectRules";
        }

        public static string GetMetaVersionDir(string snapshotDir)
        {
            return $"{snapshotDir}/MetaVersions";
        }
    }
}
