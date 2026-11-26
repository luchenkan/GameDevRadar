using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace HybridCLR.Editor.DHE
{
    public class TypeMetaSpec
    {
        public string fullName;
        public int token;
        public int version;
    }

    public class MethodMetaSpec
    {
        public string fullName;
        public int token;
        public int version;
    }

    public class MetaVersionSpecFile
    {

        public int fileVersion;

        public List<TypeMetaSpec> typeSpecs;

        public List<MethodMetaSpec> methodSpecs;

        public void WriteFile(string outputFile)
        {
            var lines = new List<string>();
            lines.Add($"FileVersion:{fileVersion}");
            lines.Add("");
            lines.Add("// TypeSpecs");
            foreach (var typeSpec in typeSpecs)
            {
                lines.Add($"[TYPE] {typeSpec.fullName} token:{typeSpec.token} version:{typeSpec.version}");
            }
            lines.Add("");
            lines.Add("// MethodSpecs");
            foreach (var methodSpec in methodSpecs)
            {
                lines.Add($"[METHOD] {methodSpec.fullName} token:{methodSpec.token} version:{methodSpec.version}");
            }
            File.WriteAllBytes(outputFile, Encoding.UTF8.GetBytes(string.Join("\n", lines)));
            Debug.Log($"SaveMetaVersionSpecFile {outputFile}");
        }
    }
}
