using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HybridCLR
{
    public class HotfixMethod
    {
        public string name;
        public string signature;
    }

    public class HotfixType
    {
        public string name;
        public List<HotfixMethod> methods;
    }

    public class HotfixAssembly
    {
        public string name;
        public List<HotfixType> types;
        public byte[] assemblyBytes;
    }

    public class HotfixManifest
    {
        public List<HotfixAssembly> assemblies;

        public static HotfixManifest LoadFrom(string manifestXmlStr, Func<string, byte[]> assemblyBytesProvider)
        {
            var manifest = new HotfixManifest()
            {
                assemblies = new List<HotfixAssembly>(),
            };

            var doc = new XmlDocument();
            doc.LoadXml(manifestXmlStr);
            foreach (XmlNode assNode in doc.DocumentElement.ChildNodes)
            {
                if (!(assNode is XmlElement assElem))
                {
                    continue;
                }
                if (assElem.Name != "assembly")
                {
                    throw new Exception($"node name should be 'assembly'");
                }
                string assName = assElem.GetAttribute("fullname");
                if (string.IsNullOrEmpty(assName))
                {
                    throw new Exception($"assembly name can't be empty");
                }
                var ass = new HotfixAssembly()
                {
                    name = assName,
                    types = new List<HotfixType>(),
                    assemblyBytes = assemblyBytesProvider(assName),
                };
                manifest.assemblies.Add(ass);
                foreach (XmlNode typeNode in assElem.ChildNodes)
                {
                    if (!(typeNode is XmlElement typeEle))
                    {
                        continue;
                    }
                    if (typeEle.Name != "type")
                    {
                        throw new Exception($"type node name should be 'type'");
                    }
                    string typeName = typeEle.GetAttribute("fullname");
                    if (string.IsNullOrEmpty(typeName))
                    {
                        throw new Exception($"type name can't be empty");
                    }
                    var type = new HotfixType()
                    {
                        name = typeName,
                        methods = new List<HotfixMethod>(),
                    };
                    ass.types.Add(type);
                    foreach (XmlNode methodNode in typeEle.ChildNodes)
                    {
                        if (!(methodNode is XmlElement methodEle))
                        {
                            continue;
                        }
                        if (methodNode.Name != "method")
                        {
                            throw new Exception("method node name should be 'method'");
                        }
                        string methodName = methodEle.GetAttribute("name");
                        string methodSignature = methodEle.GetAttribute("signature");
                        if (string.IsNullOrEmpty(methodName) && string.IsNullOrEmpty(methodSignature))
                        {
                            throw new Exception("method name or signature can't both be empty");
                        }
                        if (!string.IsNullOrEmpty(methodName) & !string.IsNullOrEmpty(methodSignature))
                        {
                            throw new Exception("method name and signature can't both be set");

                        }
                        var method = new HotfixMethod()
                        {
                            name = methodName,
                            signature = methodSignature,
                        };
                        type.methods.Add(method);
                    }
                }
            }

            return manifest;
        }
    }
}
