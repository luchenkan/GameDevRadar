using dnlib.Protection;
using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;

namespace HybridCLR.Editor.Encryption
{
    public class GlobalMetadataDatEncryptor
    {
        private readonly EncryptionInstructionSet _instructionSet;

        private readonly byte[] _encryptionOps;

        private readonly byte[] _key;

        private EncryptionMethod _encryptionMethod;

        public GlobalMetadataDatEncryptor(int vmSeed, string keySeed)
        {
            _instructionSet = new EncryptionInstructionSet(vmSeed);
            _key = EncryptionUtil.CreateKey(keySeed);
            _encryptionOps = InitEncryptionOps(ENCRYPTION_OP_LENGTH);
            _encryptionMethod = new EncryptionMethod(_instructionSet, _encryptionOps);
        }

        private byte[] InitEncryptionOps(int opCount)
        {
            var ops = new byte[opCount];
            int x = 0x1122;
            for (int i = 0; i < opCount; i++)
            {
                x = x * 0x4589 + 0x893238;
                ops[i] = (byte)x;
            }
            return ops;
        }

        const int ENCRYPTION_SANITY = 0x1357FEDA;
        const int ENCRYPTION_OP_LENGTH = 64;

        const int ENCRYPTION_SEGMENT_SIZE = 64;

        public void Encrypt(string globalMetadataDatFile, string encryptedGlobalMetadataDatFile)
        {
            UnityEngine.Debug.Log($"[GlobalMetadataDatEncryptor] Encrypting {globalMetadataDatFile}");

            var bytes = File.ReadAllBytes(globalMetadataDatFile);

            int sanity = BitConverter.ToInt32(bytes, 0);
            if (sanity == ENCRYPTION_SANITY)
            {
                UnityEngine.Debug.LogWarning($"[GlobalMetadataDatEncryptor] Already encrypted {globalMetadataDatFile}");
                return;
            }

            var bytesWithSiganture = new byte[bytes.Length + 8];
            Array.Copy(Encoding.UTF8.GetBytes("CODEPHIL"), 0, bytesWithSiganture, 0, 8);
            Array.Copy(bytes, 0, bytesWithSiganture, 8, bytes.Length);

            int encryptedBytesLength = bytesWithSiganture.Length;
            _encryptionMethod.EncryptBySegment(bytesWithSiganture, 0, (uint)encryptedBytesLength, _key, ENCRYPTION_SEGMENT_SIZE);

            var buf = new MemoryStream();
            var writer = new dnlib.DotNet.Writer.DataWriter(buf);
            writer.WriteInt32(ENCRYPTION_SANITY);
            writer.WriteInt32(encryptedBytesLength);
            writer.WriteBytes(_key);
            writer.WriteBytes(_encryptionOps.Reverse().ToArray());
            writer.WriteBytes(bytesWithSiganture);
            buf.Flush();

            File.WriteAllBytes(encryptedGlobalMetadataDatFile, buf.ToArray());

            
            //var bytes = File.ReadAllBytes(path);
            //var encryptedBytes = Encrypt(bytes);
            //File.WriteAllBytes(path, encryptedBytes);
        }
    }
}
