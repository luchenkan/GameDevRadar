using dnlib.DotNet;
using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace HybridCLR.Editor.DHE
{
    public class SignatureMapper
    {
        private readonly Dictionary<string, int> _signature2IdMap = new Dictionary<string, int>();

        private int _nextId;

        public int GetOrAddSignature(string signature)
        {
            if (!_signature2IdMap.TryGetValue(signature, out var id))
            {
                id = _nextId++;
                _signature2IdMap.Add(signature, id);
            }
            return id;
        }

        private const int kCurrentVersion = 1;

        [Serializable]
        private class SignatureIdPair
        {
            public string signature;
            public int id;
        }

        [Serializable]
        private class SignatureMapperData
        {
            public int version;
            public List<SignatureIdPair> pairs;
        }

        public void WriteFile(string file)
        {
            var pairs = _signature2IdMap.Select(e => new SignatureIdPair { id = e.Value, signature = e.Key }).ToList();
            int writeVersion = HybridCLRSettings.Instance.generateCompatibleMethodSignature ? 0 : kCurrentVersion;
            var jsonStr = JsonUtility.ToJson(new SignatureMapperData { version = writeVersion, pairs = pairs }, true);
            File.WriteAllText(file, jsonStr, Encoding.UTF8);
            Debug.Log($"[SignatureToIdMapper] write to {file}");
        }

        public void ReadFile(string file)
        {
            var jsonStr = File.ReadAllText(file, Encoding.UTF8);
            SignatureMapperData mapperData = JsonUtility.FromJson<SignatureMapperData>(jsonStr);
            int expectedVersion = HybridCLRSettings.Instance.generateCompatibleMethodSignature ? 0 : kCurrentVersion;
            if (mapperData.version != expectedVersion)
            {
                throw new Exception($"[SignatureToIdMapper] version mismatch, expected {kCurrentVersion}, but got {mapperData.version}");
            }
            List<SignatureIdPair> pairs = mapperData.pairs;
            
            foreach (var p in pairs)
            {
                _signature2IdMap.Add(p.signature, p.id);
                _nextId = Math.Max(_nextId, p.id + 1);
            }
        }

        public static string ComputeTypeDefSignature(TypeDef type)
        {
            return type.FullName;
        }

        public static string ComputeMethodDefSignature(MethodDef method)
        {
            var result = new StringBuilder();
            ComputeTypeSigName(method.MethodSig.RetType, result);
            result.Append(" ");
            result.Append(method.DeclaringType.FullName);
            result.Append("::");

            bool generateCompatibleSignature = HybridCLRSettings.Instance.generateCompatibleMethodSignature;
            if (!generateCompatibleSignature && method.IsStatic)
            {
                result.Append("@");
            }
            result.Append(method.Name);
            if (!generateCompatibleSignature && method.HasGenericParameters)
            {
                result.Append($"`{method.GenericParameters.Count}");
            }
            result.Append("(");
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(", ");
                }
                ComputeTypeSigName(method.Parameters[i].Type, result);
            }
            result.Append(")");
            return result.ToString();
        }

        public static string ComputeMethodSpecSignature(TypeSig type)
        {
            var sb = new StringBuilder();
            ComputeTypeSigName(type, sb);
            return sb.ToString();
        }

        public static void ComputeTypeSigName(TypeSig type, StringBuilder result)
        {
            type = type.RemovePinnedAndModifiers();
            switch (type.ElementType)
            {
                case ElementType.Void: result.Append("void"); break;
                case ElementType.Boolean: result.Append("bool"); break;
                case ElementType.Char: result.Append("char"); break;
                case ElementType.I1: result.Append("sbyte"); break;
                case ElementType.U1: result.Append("byte"); break;
                case ElementType.I2: result.Append("short"); break;
                case ElementType.U2: result.Append("ushort"); break;
                case ElementType.I4: result.Append("int"); break;
                case ElementType.U4: result.Append("uint"); break;
                case ElementType.I8: result.Append("long"); break;
                case ElementType.U8: result.Append("ulong"); break;
                case ElementType.R4: result.Append("float"); break;
                case ElementType.R8: result.Append("double"); break;
                case ElementType.String: result.Append("string"); break;
                case ElementType.Ptr:
                    ComputeTypeSigName(((PtrSig)type).Next, result);
                    result.Append("*");
                break;
                case ElementType.ByRef:
                    ComputeTypeSigName(((ByRefSig)type).Next, result);
                    result.Append("&");
                break;
                case ElementType.ValueType:
                case ElementType.Class:
                {
                    var valueOrClassType = type.ToClassOrValueTypeSig();
                    var typeDef = valueOrClassType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    result.Append($"[{typeDef.Module.Assembly.Name}]");
                    result.Append(typeDef.FullName);
                    break;
                }
                case ElementType.GenericInst:
                {
                    var genInst = (GenericInstSig)type;
                    ComputeTypeSigName(genInst.GenericType, result);
                    result.Append("<");
                    for (int i = 0; i < genInst.GenericArguments.Count; i++)
                    {
                        if (i > 0)
                        {
                            result.Append(",");
                        }
                        ComputeTypeSigName(genInst.GenericArguments[i], result);
                    }
                    result.Append(">");
                    break;
                }
                case ElementType.SZArray:
                ComputeTypeSigName(((SZArraySig)type).Next, result);
                result.Append("[]");
                break;
                case ElementType.Array:
                {
                    var arraySig = (ArraySig)type;
                    ComputeTypeSigName(arraySig.Next, result);
                    result.Append("[");
                    for (int i = 0; i < arraySig.Rank; i++)
                    {
                        if (i > 0)
                        {
                            result.Append(",");
                        }
                        //result.Append(arraySig.Sizes[i]);
                    }
                    result.Append("]");
                    break;
                }
                case ElementType.FnPtr:
                {
                    var fnPtr = (FnPtrSig)type;
                    result.Append("(");
                    MethodSig ms = fnPtr.MethodSig;
                    ComputeTypeSigName(ms.RetType, result);
                    result.Append("(");
                    for (int i = 0; i < ms.Params.Count; i++)
                    {
                        if (i > 0)
                        {
                            result.Append(",");
                        }
                        ComputeTypeSigName(ms.Params[i], result);
                    }
                    result.Append(")*");
                    break;
                }
                case ElementType.TypedByRef:
                result.Append("typedref");
                break;
                case ElementType.I:
                result.Append("nint");
                break;
                case ElementType.U:
                result.Append("nuint");
                break;
                case ElementType.Object:
                result.Append("object");
                break;
                case ElementType.Var:
                {
                    var var = (GenericVar)type;
                    result.Append($"!{var.Number}");
                    break;
                }
                case ElementType.MVar:
                {
                    var mvar = (GenericMVar)type;
                    result.Append($"!!{mvar.Number}");
                    break;
                }
                default: throw new NotSupportedException($"[ComputeTypeSigName] not support :{type}");

            }
        }
    }
}
