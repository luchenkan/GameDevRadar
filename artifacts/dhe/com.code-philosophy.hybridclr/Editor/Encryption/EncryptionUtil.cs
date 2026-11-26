using dnlib.DotNet.Writer;
using dnlib.DotNet;
using dnlib.Protection;
using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using Random = System.Random;

namespace HybridCLR.Editor.Encryption
{
    public static class EncryptionUtil
    {
        public static string CreateMD5Hash(byte[] bytes)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "").ToUpperInvariant();
        }

        public static byte[] CreateKey(string keySeed)
        {
            
            var r = new Random(keySeed.GetHashCode());
            var bytes = new List<byte>();
            for (int i = 0; i < EncryptionInfo.KeyLength; i++)
            {
                bytes.Add((byte)r.Next(0, 256));
            }
            return bytes.ToArray();
        }

        public static void EncryptDll(string originalDll, string encryptedDll, EncryptionSettings encryptionSettings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(encryptedDll));
            var mod = ModuleDefMD.Load(File.ReadAllBytes(originalDll));

            string rawKey = encryptionSettings.key ?? "";
            byte[] encryptionKey = CreateKey(rawKey);

            var opt = new EncryptedModuleWriterOptions(mod)
            {
                encryptor = new RandomEncryption(new RandomEncryptionOptions()
                {
                    InstructionSeed = encryptionSettings.vmSeed,
                    MetadataSeed = encryptionSettings.metadataSeed,
                    EncKey = encryptionKey,
                    StringEncCodeLength = encryptionSettings.stringEncCodeLength,
                    BlobEncCodeLength = encryptionSettings.blobEncCodeLength,
                    UserStringEncCodeLength = encryptionSettings.userStringEncCodeLength,
                    TableEncCodeLength = encryptionSettings.tableEncCodeLength,
                    LazyUserStringEncCodeLength = encryptionSettings.lazyUserStringEncCodeLength,
                    LazyTableEncCodeLength = encryptionSettings.lazyTableEncCodeLength,
                    MethodBodyEncCodeLength = encryptionSettings.methodBodyEncCodeLength,
                }),
            };
            opt.MetadataOptions.Flags |= MetadataFlags.PreserveRids;
            var writer = new EncryptedModuleWriter(mod, opt);
            writer.Write(encryptedDll);
            Debug.Log($"EncryptDll. original dll:{originalDll} encrypted dll:{encryptedDll}");
        }
    }
}
