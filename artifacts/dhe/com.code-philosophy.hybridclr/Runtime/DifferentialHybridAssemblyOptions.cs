using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace HybridCLR.Runtime
{
    public class BadOptionDataException : Exception
    {
        public BadOptionDataException(string err) : base(err)
        {
            
        }
    }
    public class DifferentialHybridAssemblyOptions
    {

        public const uint Signature = 0xABCDABCD;


        public string OldDllMD5;

        public string NewDllMD5;

        public bool ForceAllChanged;

        public List<uint> ChangedMethodTokens;

        public List<uint> ChangedStructTokens;

        public byte[] Marshal()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(Signature);

            if (OldDllMD5.Length != 32 || NewDllMD5.Length != 32)
            {
                throw new BadOptionDataException("bad md5 length");
            }
            writer.Write(Encoding.UTF8.GetBytes(OldDllMD5));
            writer.Write(Encoding.UTF8.GetBytes(NewDllMD5));

            writer.Write(ForceAllChanged);

            writer.Write((uint)ChangedMethodTokens.Count);
            foreach (uint token in ChangedMethodTokens)
            {
                writer.Write(token);
            }

            writer.Write((uint)ChangedStructTokens.Count);
            foreach (uint token in ChangedStructTokens)
            {
                writer.Write(token);
            }

            writer.Flush();
            stream.Flush();
            byte[] result = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(result, 0, result.Length);
            //Debug.Log($"[DifferentialHybridAssemblyOptions] bytes.length:{result.Length}");
            return result;
        }

        public void Unmarshal(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);
            uint signature = reader.ReadUInt32();
            if (signature != Signature)
            {
                throw new BadOptionDataException("bad signature");
            }
            OldDllMD5 = Encoding.UTF8.GetString(reader.ReadBytes(32));
            NewDllMD5 = Encoding.UTF8.GetString(reader.ReadBytes(32));
            ForceAllChanged = reader.ReadBoolean();
            uint changedMethodCount = reader.ReadUInt32();
            ChangedMethodTokens = new List<uint>();
            for (int i = 0 ; i < changedMethodCount; i++)
            {
                ChangedMethodTokens.Add(reader.ReadUInt32());
            }
            uint changedStructCount = reader.ReadUInt32();
            ChangedStructTokens = new List<uint>();
            for (int i = 0 ; i < changedStructCount; i++)
            {
                ChangedStructTokens.Add(reader.ReadUInt32());
            }

            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                throw new BadOptionDataException("remain unread bytes");
            }
        }
    }
}
