using dnlib.Protection;
using HybridCLR.Editor.Encryption;
using HybridCLR.Editor.Settings;
using System;
using UnityEditor;
using UnityEngine;

namespace HybridCLR.Editor.Commands
{
    public static class EncryptionCommand
    {
        [MenuItem("HybridCLR/Generate/EncryptionVM", priority = 106)]
        public static void GenerateEncryptionVM()
        {
            string encryptionVMTemplateFile = $@"{SettingsUtil.TemplatePathInPackage}/EncryptionVM_Decrypt.cpp.tpl";
            string encryptionVMOutputFile = $@"{SettingsUtil.LocalIl2CppDir}/libil2cpp/hybridclr/generated/EncryptionVM_Decrypt.cpp";

            int vmSeed = SettingsUtil.EncryptionSettings.vmSeed;
            var instructionSet = new EncryptionInstructionSet(vmSeed);
            var generator = new VmInterpreterGenerator(instructionSet);
            generator.Generate(encryptionVMTemplateFile, encryptionVMOutputFile);
            Debug.Log($"EncryptionVM vmSeed:{vmSeed} output:{encryptionVMOutputFile}");
        }
    }
}
