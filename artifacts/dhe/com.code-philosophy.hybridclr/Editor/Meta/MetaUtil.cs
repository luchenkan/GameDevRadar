using dnlib.DotNet;
using HybridCLR.Editor.Meta;
using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace HybridCLR.Editor.Meta
{
    public static class MetaUtil
    {

		public static bool EqualsTypeSig(TypeSig a, TypeSig b)
		{
			if (a == b)
			{
				return true;
			}
			if (a != null && b != null)
			{
				return TypeEqualityComparer.Instance.Equals(a, b);
			}
			return false;
		}

		public static bool EqualsTypeSigArray(List<TypeSig> a, List<TypeSig> b)
		{
			if (a == b)
			{
				return true;
			}
			if (a != null && b != null)
			{
				if (a.Count != b.Count)
				{
					return false;
				}
				for (int i = 0; i < a.Count; i++)
				{
					if (!TypeEqualityComparer.Instance.Equals(a[i], b[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static TypeSig Inflate(TypeSig sig, GenericArgumentContext ctx)
		{
			if (!sig.ContainsGenericParameter)
			{
				return sig;
			}
			return ctx.Resolve(sig);
		}

        public static ITypeDefOrRef Inflate(ITypeDefOrRef type, GenericArgumentContext ctx)
        {
            return Inflate(type.ToTypeSig(), ctx).ToTypeDefOrRef();
        }

		public static TypeSig ToShareTypeSig(ICorLibTypes corTypes, TypeSig typeSig)
        {
			var a = typeSig.RemovePinnedAndModifiers();
			switch (a.ElementType)
			{
				case ElementType.Void: return corTypes.Void;
				case ElementType.Boolean: return corTypes.Byte;
				case ElementType.Char: return corTypes.UInt16;
				case ElementType.I1: return corTypes.SByte;
				case ElementType.U1:return corTypes.Byte;
				case ElementType.I2: return corTypes.Int16;
				case ElementType.U2: return corTypes.UInt16;
				case ElementType.I4: return corTypes.Int32;
				case ElementType.U4: return corTypes.UInt32;
				case ElementType.I8: return corTypes.Int64;
				case ElementType.U8: return corTypes.UInt64;
				case ElementType.R4: return corTypes.Single;
				case ElementType.R8: return corTypes.Double;
				case ElementType.String: return corTypes.Object;
				case ElementType.TypedByRef: return corTypes.TypedReference;
				case ElementType.I: return corTypes.IntPtr;
				case ElementType.U: return corTypes.UIntPtr;
				case ElementType.Object: return corTypes.Object;
				case ElementType.Sentinel: return typeSig;
				case ElementType.Ptr: return corTypes.UIntPtr;
				case ElementType.ByRef: return corTypes.UIntPtr;
				case ElementType.SZArray: return corTypes.Object;
				case ElementType.Array: return corTypes.Object;
				case ElementType.ValueType:
				{
                    TypeDef typeDef = a.ToTypeDefOrRef().ResolveTypeDefThrow();
					if (typeDef.IsEnum)
					{
						return ToShareTypeSig(corTypes, typeDef.GetEnumUnderlyingType());
					}
                    return typeSig;
				}
				case ElementType.Var:
				case ElementType.MVar:
				case ElementType.Class: return corTypes.Object;
				case ElementType.GenericInst:
                {
					var gia = (GenericInstSig)a;
                    TypeDef typeDef = gia.GenericType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    if (typeDef.IsEnum)
                    {
                        return ToShareTypeSig(corTypes, typeDef.GetEnumUnderlyingType());
                    }
                    if (!typeDef.IsValueType)
                    {
                        return corTypes.Object;
                    }
                    return new GenericInstSig(gia.GenericType, gia.GenericArguments.Select(ga => ToShareTypeSig(corTypes, ga)).ToList());
				}
				case ElementType.FnPtr: return corTypes.UIntPtr;
				case ElementType.ValueArray: return typeSig;
				case ElementType.Module: return typeSig;
				default:
					throw new NotSupportedException(typeSig.ToString());
			}
		}
	
		public static List<TypeSig> ToShareTypeSigs(ICorLibTypes corTypes, IList<TypeSig> typeSigs)
        {
			if (typeSigs == null)
            {
				return null;
            }
			return typeSigs.Select(s => ToShareTypeSig(corTypes, s)).ToList();
        }

		public static IAssemblyResolver CreateHotUpdateAssemblyResolver(BuildTarget target, List<string> hotUpdateDlls)
        {
			var externalDirs = HybridCLRSettings.Instance.externalHotUpdateAssembliyDirs;
			var defaultHotUpdateOutputDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
			IAssemblyResolver defaultHotUpdateResolver = new FixedSetAssemblyResolver(defaultHotUpdateOutputDir, hotUpdateDlls);
			if (externalDirs == null || externalDirs.Length == 0)
            {
				return defaultHotUpdateResolver;
            }
			else
            {
				var resolvers = new List<IAssemblyResolver>();
				foreach (var dir in externalDirs)
                {
					resolvers.Add(new FixedSetAssemblyResolver($"{dir}/{target}", hotUpdateDlls));
					resolvers.Add(new FixedSetAssemblyResolver(dir, hotUpdateDlls));
                }
				resolvers.Add(defaultHotUpdateResolver);
				return new CombinedAssemblyResolver(resolvers.ToArray());
            }
		}

		public static IAssemblyResolver CreateAOTAssemblyResolver(BuildTarget target)
        {
			return new PathAssemblyResolver(SettingsUtil.GetAssembliesPostIl2CppStripDir(target));
        }

		public static IAssemblyResolver CreateHotUpdateAndAOTAssemblyResolver(BuildTarget target, List<string> hotUpdateDlls)
        {
			return new CombinedAssemblyResolver(
				CreateHotUpdateAssemblyResolver(target, hotUpdateDlls),
				CreateAOTAssemblyResolver(target)
				);
        }

		public static string ResolveNetStandardAssemblyPath(string assemblyName)
		{
			return $"{SettingsUtil.HybridCLRDataPathInPackage}/NetStandard/{assemblyName}.dll";
		}


        public static  List<TypeSig> CreateDefaultGenericParams(ModuleDef module, int genericParamCount)
        {
            var methodGenericParams = new List<TypeSig>();
            for (int i = 0; i < genericParamCount; i++)
            {
                methodGenericParams.Add(module.CorLibTypes.Object);
            }
            return methodGenericParams;
        }

        public static bool IsThreadStaticField(FieldDef field)
        {
            return field.CustomAttributes.Any(ca => ca.TypeFullName == "System.ThreadStaticAttribute");
        }

        public static bool IsSupportedPInvokeTypeSig(TypeSig typeSig)
        {
            typeSig = typeSig.RemovePinnedAndModifiers();
            if (typeSig.IsByRef)
            {
                return true;
            }
            switch (typeSig.ElementType)
            {
                case ElementType.SZArray:
                case ElementType.Array:
                //case ElementType.Class:
                case ElementType.String:
                //case ElementType.Object:
                return false;
                default: return true;
            }
        }

        public static bool IsSupportedPInvokeMethodSignature(MethodSig methodSig)
        {
            return IsSupportedPInvokeTypeSig(methodSig.RetType) && methodSig.Params.All(p => IsSupportedPInvokeTypeSig(p));
        }

        public static void AppendTypeSigToSignature(StringBuilder sb, TypeSig typeSig, bool fullName)
        {
            typeSig = typeSig.RemovePinnedAndModifiers();

            switch (typeSig.ElementType)
            {
                case ElementType.Void: sb.Append(fullName ? "System.Void" : "Void"); break;
                case ElementType.Boolean: sb.Append(fullName ? "System.Boolean" : "Boolean"); break;
                case ElementType.Char: sb.Append(fullName ? "System.Char" : "Char"); break;
                case ElementType.I1: sb.Append(fullName ? "System.SByte" : "SByte"); break;
                case ElementType.U1: sb.Append(fullName ? "System.Byte" : "Byte"); break;
                case ElementType.I2: sb.Append(fullName ? "System.Int16" : "Int16"); break;
                case ElementType.U2: sb.Append(fullName ? "System.UInt16" : "UInt16"); break;
                case ElementType.I4: sb.Append(fullName ? "System.Int32" : "Int32"); break;
                case ElementType.U4: sb.Append(fullName ? "System.UInt32" : "UInt32"); break;
                case ElementType.I8: sb.Append(fullName ? "System.Int64" : "Int64"); break;
                case ElementType.U8: sb.Append(fullName ? "System.UInt64" : "UInt64"); break;
                case ElementType.R4: sb.Append(fullName ? "System.Single" : "Single"); break;
                case ElementType.R8: sb.Append(fullName ? "System.Double" : "Double"); break;
                case ElementType.String: sb.Append("System.String"); break;
                case ElementType.Ptr: AppendTypeSigToSignature(sb, typeSig.Next, fullName); sb.Append("*"); break;
                case ElementType.ByRef: AppendTypeSigToSignature(sb, typeSig.Next, fullName); sb.Append(" ByRef"); break;
                case ElementType.ValueType:
                case ElementType.Class:
                {
                    var classOrValueTypeSig = (ClassOrValueTypeSig)typeSig;
                    // nested type only show reflection name.
                    sb.Append(fullName || !classOrValueTypeSig.ReflectionFullName.Contains('+') ? classOrValueTypeSig.ReflectionFullName : classOrValueTypeSig.ReflectionName);
                    break;
                }
                case ElementType.GenericInst:
                {
                    var genericInstSig = (GenericInstSig)typeSig;
                    // nested type only show reflection name. ignore generic parameters.
                    if (!fullName && genericInstSig.GenericType.ReflectionFullName.Contains('+'))
                    {
                        sb.Append(genericInstSig.GenericType.ReflectionName);
                        break;
                    }
                    sb.Append(genericInstSig.GenericType.ReflectionFullName);
                    sb.Append("[");
                    for (int i = 0; i < genericInstSig.GenericArguments.Count; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(",");
                        }
                        AppendTypeSigToSignature(sb, genericInstSig.GenericArguments[i], true);
                    }
                    sb.Append("]");
                    break;
                }
                case ElementType.Var:
                case ElementType.MVar:
                {
                    var varSig = (GenericSig)typeSig;
                    sb.Append(varSig.GenericParam.Name);
                    break;
                }
                case ElementType.I: sb.Append(fullName ? "System.IntPtr" : "IntPtr"); break;
                case ElementType.U: sb.Append(fullName ? "System.UIntPtr" : "UIntPtr"); break;
                case ElementType.FnPtr: sb.Append("IntPtr"); break;
                case ElementType.Object: sb.Append("System.Object"); break;
                case ElementType.SZArray:
                {
                    var szArraySig = (SZArraySig)typeSig;
                    AppendTypeSigToSignature(sb, szArraySig.Next, fullName);
                    sb.Append("[]");
                    break;
                }
                case ElementType.Array:
                {
                    var arraySig = (ArraySig)typeSig;
                    AppendTypeSigToSignature(sb, arraySig.Next, fullName);
                    sb.Append("[");
                    for (int i = 0; i < arraySig.Rank - 1; i++)
                    {
                        sb.Append(",");
                    }
                    sb.Append("]");
                    break;
                }
                default:
                throw new NotSupportedException(typeSig.ToString());
            }
        }

        public static string CreateMethodDefSignature(MethodDef method)
        {
            var result = new StringBuilder();
            TypeDef declaringType = method.DeclaringType;


            AppendTypeSigToSignature(result, method.ReturnType, false);
            result.Append(" ");
            result.Append(method.Name);
            if (method.HasGenericParameters)
            {
                result.Append("[");
                result.Append(string.Join(",", method.GenericParameters.Select(p => p.Name)));
                result.Append("]");
            }
            result.Append("(");

            int index = 0;
            foreach (TypeSig p in method.GetParams())
            {
                if (index > 0)
                {
                    result.Append(", ");
                }
                AppendTypeSigToSignature(result, p, false);
                ++index;
            }
            result.Append(")");
            return result.ToString();
        }
    }
}
