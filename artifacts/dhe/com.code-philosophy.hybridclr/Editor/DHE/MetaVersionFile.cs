using System;
using System.IO;
using System.Text;
using UnityEngine.Windows.Speech;

namespace HybridCLR.Editor.DHE
{

    public class TypeDefVersion
    {
        public int version;
        public int nameId;
    }

    public class MethodDefVersion
    {
        public int version;
        public int signatureId;
    }

    public class MetaVersionFile
    {

        private const string FileSignature = "CPMV";

        private const int SchemaVersion = 1;

        public int fileVersion;

        public TypeDefVersion[] typeDefVersions;

        public MethodDefVersion[] methodDefVersions;


        public TypeDefVersion GetTypeDefVersion(int rid)
        {
            return typeDefVersions[rid - 1];
        }

        public MethodDefVersion GetMethodDefVersion(int rid)
        {
            return methodDefVersions[rid - 1];
        }


        public byte[] Marshal()
        {
            var ms = new MemoryStream();
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(Encoding.UTF8.GetBytes(FileSignature));
                writer.Write(SchemaVersion);
                writer.Write(fileVersion);
                writer.Write(typeDefVersions.Length);
                foreach (var typeDefVersion in typeDefVersions)
                {
                    writer.Write(typeDefVersion.version);
                    writer.Write(typeDefVersion.nameId);
                }
                writer.Write(methodDefVersions.Length);
                foreach (var methodDefVersion in methodDefVersions)
                {
                    writer.Write(methodDefVersion.version);
                    writer.Write(methodDefVersion.signatureId);
                }
                writer.Flush();
                return ms.ToArray();
            }
        }

        public void Unmarshal(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            using (var reader = new BinaryReader(ms))
            {
                var signature = Encoding.UTF8.GetString(reader.ReadBytes(4));
                if (signature != FileSignature)
                {
                    throw new Exception("bad signature");
                }
                var schemaVersion = reader.ReadInt32();
                if (schemaVersion != SchemaVersion)
                {
                    throw new Exception("bad schema version");
                }
                fileVersion = reader.ReadInt32();
                var typeDefCount = reader.ReadInt32();
                typeDefVersions = new TypeDefVersion[typeDefCount];
                for (int i = 0; i < typeDefCount; i++)
                {
                    int version = reader.ReadInt32();
                    int nameId = reader.ReadInt32();
                    typeDefVersions[i] = new TypeDefVersion { version = version, nameId = nameId };
                }
                var methodDefCount = reader.ReadInt32();
                methodDefVersions = new MethodDefVersion[methodDefCount];
                for (int i = 0; i < methodDefCount; i++)
                {
                    int version = reader.ReadInt32();
                    int signatureId = reader.ReadInt32();
                    methodDefVersions[i] = new MethodDefVersion { version = version, signatureId = signatureId };
                }
            }
        }

        public void WriteFile(string filePath)
        {
            File.WriteAllBytes(filePath, Marshal());
        }

        public void ReadFile(string filePath)
        {
            Unmarshal(File.ReadAllBytes(filePath));
        }
    }
}
