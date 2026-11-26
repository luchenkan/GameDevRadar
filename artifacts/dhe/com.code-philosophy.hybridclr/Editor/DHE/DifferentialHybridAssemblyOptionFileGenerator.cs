using dnlib.DotNet;
using dnlib.DotNet.Emit;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor.Meta;
using HybridCLR.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using IAssemblyResolver = HybridCLR.Editor.Meta.IAssemblyResolver;

namespace HybridCLR.Editor.DHE
{
    public class DifferentialHybridAssemblyOptionFileGenerator
    {
        private readonly string _outputDir;
        private readonly Snapshot _oldSnapshot;
        private readonly Snapshot _newSnapshot;
        private readonly CodeDiffAnalyzer _codeDiffAnalyzer;

        public CodeDiffAnalyzer CodeDiffAnalyzer => _codeDiffAnalyzer;

        public DifferentialHybridAssemblyOptionFileGenerator(Snapshot oldSnapshot, Snapshot newSnapshot, string outputDir)
        {
            _outputDir =outputDir;
            _oldSnapshot = oldSnapshot;
            _newSnapshot = newSnapshot;
            _codeDiffAnalyzer = new CodeDiffAnalyzer(oldSnapshot, newSnapshot);
        }

        public static string CreateMD5Hash(byte[] bytes)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "").ToUpperInvariant();
        }

        private void GenerateData()
        {
            _codeDiffAnalyzer.RunAnalyze();
            foreach (AssemblyDiffInfo ass in _codeDiffAnalyzer.AssemblyDiffInfos)
            {
                string assName = ass.name;
                string outOptionFile = $"{_outputDir}/{assName}.dhao.bytes";

                // 只需要比较内存布局等价以及虚函数等价，
                var changedStructs = ass.types.Where(t => t.newType.IsValueType && !t.newType.IsEnum && !t.layoutEqual).ToList();
                var changedStructTokens = changedStructs.Select(s => s.newType.MDToken.Raw).ToList();
                var changedMethodTokens = ass.types.SelectMany(t => t.methods)
                    .Where(m => !m.fullEqual)
                    .Select(m => m.newMethod.MDToken.Raw).ToList();
                var dhaOptions = new DifferentialHybridAssemblyOptions()
                {
                    OldDllMD5 = CreateMD5Hash(_oldSnapshot.AssemblyCache.GetModFileInfo(ass.oldModule).data),
                    NewDllMD5 = CreateMD5Hash(_newSnapshot.AssemblyCache.GetModFileInfo(ass.newModule).data),
                    ChangedMethodTokens = changedMethodTokens,
                    ChangedStructTokens = changedStructTokens,
                };
                File.WriteAllBytes(outOptionFile, dhaOptions.Marshal());
                Debug.Log($"[AssemblyOptionDataGenerator::GenerateData] assembly:{assName} oldDllMd5:{dhaOptions.OldDllMD5} newDllMd5:{dhaOptions.NewDllMD5} changedStructTypeCount:{changedStructTokens.Count} changedMethodCount:{changedMethodTokens.Count} output:{outOptionFile}");
            }
        }

        private string GetClassType(TypeDef type)
        {
            if (type.IsEnum)
            {
                return "enum";
            }
            if (type.IsValueType)
            {
                return "struct";
            }
            return "class";
        }

        private void GenerateSpec()
        {
            foreach (AssemblyDiffInfo ass in _codeDiffAnalyzer.AssemblyDiffInfos)
            {
                string assName = ass.name;


                List<string> lines = new List<string>();

                lines.Add("// changed classes");
                lines.Add("");
                foreach (TypeDiffInfo t in ass.types)
                {
                    TypeDef newType = t.newType;
                    string classType = GetClassType(newType);
                    if (!t.layoutEqual)
                    {
                        lines.Add($"[INSTANCE] {classType} {newType.FullName} token:{newType.MDToken.Raw}");
                    }
                    if (!t.staticLayoutEqual)
                    {
                        lines.Add($"[STATIC] {classType} {newType.FullName} token:{newType.MDToken.Raw}");
                    }
                }
                lines.Add("");
                lines.Add("// changed methods");

                foreach (MethodDiffInfo method in ass.types.SelectMany(t => t.methods))
                {
                    if (!method.fullEqual)
                    {
                        lines.Add($"[METHOD] {method.newMethod.FullName} token:{method.newMethod.MDToken.Raw}");
                    }
                }

                string outOptionFile = $"{_outputDir}/{assName}.dhao.spec";
                File.WriteAllBytes(outOptionFile, System.Text.Encoding.UTF8.GetBytes(string.Join("\n", lines)));
                Debug.Log($"[AssemblyOptionDataGenerator::GenerateSpec] assembly:{assName} output:{outOptionFile}");
            }
        }

        public void Generate()
        {
            BashUtil.RecreateDir(_outputDir);
            GenerateData();
            GenerateSpec();
        }

        public void GenerateNotAnyChangeData()
        {
            Directory.CreateDirectory(_outputDir);
            foreach (AssemblyDiffInfo ass in _codeDiffAnalyzer.AssemblyDiffInfos)
            {
                string assName = ass.name;
                string outOptionFile = $"{_outputDir}/{assName}.dhao.bytes";

                var dhaOptions = new DifferentialHybridAssemblyOptions()
                {
                    ChangedMethodTokens = new List<uint>(),
                    ChangedStructTokens = new List<uint>(),
                    OldDllMD5 = CreateMD5Hash(_oldSnapshot.AssemblyCache.GetModFileInfo(ass.oldModule).data),
                    NewDllMD5 = CreateMD5Hash(_newSnapshot.AssemblyCache.GetModFileInfo(ass.newModule).data),
                };
                File.WriteAllBytes(outOptionFile, dhaOptions.Marshal());
                Debug.Log($"[AssemblyOptionDataGenerator::GenerateNotAnyChangeData] output:{outOptionFile} oldDllMD5:{dhaOptions.OldDllMD5} newDllMD5:{dhaOptions.NewDllMD5}");
            }
        }
    }
}
