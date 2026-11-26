using dnlib.Protection;
using HybridCLR.Editor.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridCLR.Editor.Encryption
{
    public class VmInterpreterGenerator
    {
        private readonly EncryptionInstructionSet _instructionSet;

        public VmInterpreterGenerator(EncryptionInstructionSet instructionSet)
        {
            _instructionSet = instructionSet;
        }

        public void Generate(string templateFile, string outputFile)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"\t\t\t// Seed:{_instructionSet.Seed}");

            int opCode = 0;
            foreach (var inst in _instructionSet.Instructions)
            {
                sb.Append($"\t\t\tcase {opCode}:").Append(inst.GenerateDecryptExpression("data", "dataLength", "key")).Append("break;").AppendLine();
                ++opCode;
            }

            var template = System.IO.File.ReadAllText(templateFile);
            var frr = new FileRegionReplace(template);
            frr.Replace("INSTRUCTIONS", sb.ToString());
            frr.Commit(outputFile);
        }
    }
}
