using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HybridCLR.Editor.DHE
{

    public class InjectRules
    {
        public class StringOrWildcardPattern
        {
            private readonly string _str;
            private readonly Regex _regex;

            public string NameOrPattern => _str;

            public StringOrWildcardPattern(string nameOrPattern)
            {
                _str = nameOrPattern;
                _regex = nameOrPattern.Contains('*') || nameOrPattern.Contains('?') ? new Regex(WildcardToRegex(nameOrPattern)) : null;
            }

            public static string WildcardToRegex(string pattern)
            {
                return "^" + Regex.Escape(pattern).
                Replace("\\*", ".*").
                Replace("\\?", ".") + "$";
            }

            public bool IsMatch(string name)
            {
                if (_regex != null)
                {
                    return _regex.IsMatch(name);
                }
                else
                {
                    return _str == name;
                }
            }
        }

        public class AssemblyConfig
        {
            public StringOrWildcardPattern namePattern;
            public readonly List<TypeConfig> types = new List<TypeConfig>();
        }

        public class TypeConfig
        {
            public StringOrWildcardPattern namePattern;

            public readonly List<MethodConfig> methods = new List<MethodConfig>();
        }

        public enum MethodType
        {
            Method,
            Property,
            Event,
        }
        public enum MethodInjectMode
        {
            None,
            Proxy,
        }

        public class MethodConfig
        {
            public StringOrWildcardPattern namePattern;
            public bool isSignature;
            public MethodType type;
            public MethodInjectMode injectMode;

            public int matchCount;
        }

        private readonly List<AssemblyConfig> _assemblies = new List<AssemblyConfig>();

        public IReadOnlyCollection<AssemblyConfig> Assemblies => _assemblies;

        public void LoadFromXmlFiles(IEnumerable<string> xmlFiles)
        {
            foreach (var xmlFile in xmlFiles)
            {
                LoadFromXmlFile(xmlFile);
            }
        }

        public void LoadFromXmlFile(string xmlFile)
        {
            LoadFromXmlString(System.IO.File.ReadAllText(xmlFile, Encoding.UTF8));
        }

        public void LoadFromXmlString(string xmlString)
        {
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xmlString);
            LoadFromXmlDoc(doc.DocumentElement);
        }

        private void LoadFromXmlDoc(XmlElement root)
        {
            foreach (XmlElement elem in root.ChildNodes.OfType<XmlElement>())
            {
                if (elem.Name == "assembly")
                {
                    LoadAssembly(elem);
                }
                else
                {
                    throw new Exception("Unknown element: " + elem.Name);
                }
            }
        }

        private void LoadAssembly(XmlElement assEle)
        {
            string assName = assEle.GetAttribute("fullname");
            if (string.IsNullOrEmpty(assName))
            {
                throw new Exception($"Assembly element must have fullname attribute");
            }
            var assemblyConfig = new AssemblyConfig()
            {
                namePattern = new StringOrWildcardPattern(assName),
            };
            _assemblies.Add(assemblyConfig);

            foreach (XmlElement ele in assEle.ChildNodes.OfType<XmlElement>())
            {
                if (ele.Name == "type")
                {
                    LoadType(ele, assemblyConfig);
                }
                else
                {
                    throw new Exception("Unknown element: " + ele.Name);
                }
            }
        }

        private static MethodInjectMode ParseInjectMode(string modeStr)
        {
            switch (modeStr)
            {
                case "":
                case "none":
                    return MethodInjectMode.None;
                case "proxy":
                    return MethodInjectMode.Proxy;
                default:
                    throw new Exception($"Unknown inject mode: {modeStr}");
            }
        }

        private void LoadType(XmlElement typeEle, AssemblyConfig assemblyConfig)
        {

            string name = typeEle.GetAttribute("fullname");
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception($"Type element must have fullname attribute");
            }
            //string injectModeStr = typeEle.GetAttribute("inject");
            //MethodProxyMode? injectMode = !string.IsNullOrEmpty(injectModeStr) ? ParseInjectMode(injectModeStr) : null;
            var typeConfig = new TypeConfig()
            {
                namePattern = new StringOrWildcardPattern(name),
            };
            assemblyConfig.types.Add(typeConfig);
            foreach (XmlElement ele in typeEle.ChildNodes.OfType<XmlElement>())
            {
                switch (ele.Name)
                {
                    case "method":
                        LoadMethodOrPropertyOrEvent(ele, typeConfig, MethodType.Method);
                        break;
                    case "property":
                        LoadMethodOrPropertyOrEvent(ele, typeConfig, MethodType.Property);
                        break;
                    case "event":
                        LoadMethodOrPropertyOrEvent(ele, typeConfig, MethodType.Event);
                        break;
                    default:
                        throw new Exception("Unknown element: " + ele.Name);
                }
            }
        }

        private void LoadMethodOrPropertyOrEvent(XmlElement ele, TypeConfig typeConfig, MethodType methodType)
        {
            string name = ele.GetAttribute("name");
            string signature = ele.GetAttribute("signature");
            string injectModeStr = ele.GetAttribute("mode");
            MethodInjectMode injectMode = ParseInjectMode(injectModeStr);
            string nameOrSignature;
            bool isSignature;
            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(signature))
                {
                    throw new Exception($"{methodType} element must have either name or signature attribute");
                }
                isSignature = false;
                nameOrSignature = name;
            }
            else if (!string.IsNullOrEmpty(signature))
            {
                isSignature = true;
                nameOrSignature = signature;
            }
            else
            {
                throw new Exception($"{methodType} element must have either name or signature attribute");
            }
            var methodConfig = new MethodConfig()
            {
                namePattern = new StringOrWildcardPattern(nameOrSignature),
                isSignature = isSignature,
                type = methodType,
                injectMode = injectMode,
            };
            typeConfig.methods.Add(methodConfig);
        }

        public bool IsNotInjectMethod(MethodDef methodDef)
        {
            if (methodDef.Name.ToString() == ".cctor")
            {
                return true;
            }
//#if !UNITY_2023_1_OR_NEWER
            bool? needInject = null;
            foreach (var ass in _assemblies)
            {
                if (!ass.namePattern.IsMatch(methodDef.Module.Assembly.Name))
                {
                    continue;
                }
                bool? ret = IsNotInjectMethod(ass, methodDef);
                needInject = ret ?? needInject;
            }
            return needInject ?? false;
//#else
//            return true;
//#endif
        }

        private static string GetPropertySignature(PropertyDef propertyDef)
        {
            return $"{propertyDef.PropertySig.RetType} {propertyDef.Name}";
        }

        private bool? IsNotInjectMethod(AssemblyConfig assemblyConfig, MethodDef methodDef)
        {
            string declaringTypeFullName = methodDef.DeclaringType.FullName;
            MethodInjectMode? mode = null;
            string methodName = methodDef.Name;
            string methodSignature = FullNameFactory.MethodFullName(null, methodName, methodDef.MethodSig);
            PropertyDef propertyDef = methodDef.IsSpecialName ? methodDef.DeclaringType.Properties.FirstOrDefault(p => p.GetMethod == methodDef || p.SetMethod == methodDef) : null;
            string propertySignature = propertyDef != null ? GetPropertySignature(propertyDef) : null;
            EventDef eventDef = methodDef.IsSpecialName ? methodDef.DeclaringType.Events.FirstOrDefault(p => p.AddMethod == methodDef || p.RemoveMethod == methodDef) : null;
            string eventSignature = eventDef != null ? FullNameFactory.EventFullName(null, eventDef.Name, eventDef.EventType) : null;
            foreach (var type in assemblyConfig.types)
            {
                if (!type.namePattern.IsMatch(declaringTypeFullName))
                {
                    continue;
                }
                foreach (var method in type.methods)
                {
                    switch (method.type)
                    {
                        case MethodType.Method:
                            {
                                if (method.namePattern.IsMatch(!method.isSignature ? methodName : methodSignature))
                                {
                                    mode = method.injectMode;
                                    ++method.matchCount;
                                }
                                break;
                            }
                        case MethodType.Property:
                            {
                                if (propertyDef != null && method.namePattern.IsMatch(!method.isSignature ? propertyDef.Name.ToString() : propertySignature))
                                {
                                    mode = method.injectMode;
                                    ++method.matchCount;
                                }
                                break;
                            }
                        case MethodType.Event:
                            {
                                if (eventDef != null && method.namePattern.IsMatch(!method.isSignature ? eventDef.Name.ToString() : eventSignature))
                                {
                                    mode = method.injectMode;
                                    ++method.matchCount;
                                }
                                break;
                            }
                    }
                }
            }
            return mode != null ? mode != MethodInjectMode.Proxy : (bool?)null;
        }

        void DumpType(TypeConfig typeConfig, string indent)
        {
            foreach (var e in typeConfig.methods)
            {
                UnityEngine.Debug.Log($"{indent}type:{e.type} name:{e.namePattern.NameOrPattern} mode:{e.injectMode}");
            }
        }

        public void Dump()
        {
            foreach (var ass in _assemblies)
            {
                UnityEngine.Debug.Log($"assembly:{ass.namePattern.NameOrPattern}");
                foreach (var type in ass.types)
                {
                    UnityEngine.Debug.Log($"\ttype:{type.namePattern.NameOrPattern}");
                    DumpType(type, "\t\t");
                }
            }
        }
    }

}
