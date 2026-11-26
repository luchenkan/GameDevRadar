using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HybridCLR.Editor.DHE
{
    public class AssemblyMetaRetarget
    {
        private readonly ModuleContext _modCtx;
        private readonly AssemblyResolver _asmResolver;

        private readonly ModuleDefMD _oldAss;
        private readonly ModuleDefMD _newAss;

        private readonly Regex _burstDirectCallPattern;
        private readonly Regex _postfixBurstDelegatePattern;
        private readonly Regex _jobReflectionRegistrationOutputPattern;

        private readonly byte[] _newDllBytes;
        private bool _anyRetargeted = false;

        public AssemblyMetaRetarget(byte[] oldDll, byte[] newDll)
        {
            _newDllBytes = newDll;
            _modCtx = ModuleDef.CreateModuleContext();
            _asmResolver = (AssemblyResolver)_modCtx.AssemblyResolver;
            _asmResolver.EnableTypeDefCache = true;
            _asmResolver.UseGAC = false;

            _oldAss = ModuleDefMD.Load(oldDll, _modCtx);
            _newAss = ModuleDefMD.Load(newDll, _modCtx);

            _burstDirectCallPattern = new Regex(@"(.+)_([0-9A-Fa-f]+)\$BurstDirectCall");
            _postfixBurstDelegatePattern = new Regex(@"(.+)_([0-9A-Fa-f]+)\$PostfixBurstDelegate");
            _jobReflectionRegistrationOutputPattern = new Regex(@"__JobReflectionRegistrationOutput__([0-9A-Fa-f]+)");
        }


        class CustomAttributeInfo
        {
            public CustomAttributeCollection customAttributes;
            public int index;
            public List<CAArgument> arguments;
            public List<CANamedArgument> namedArguments;
        }

        public void Retarget()
        {
            foreach (var type in _newAss.GetTypes())
            {
                if (type.DefinitionAssembly == null)
                {
                    continue;
                }
                string typeName = type.Name;
                if (type.DeclaringType != null)
                {
                    Match match = _burstDirectCallPattern.Match(typeName);
                    if (match.Success)
                    {
                        RetargetBurstType(type, _burstDirectCallPattern);
                        continue;
                    }
                    match = _postfixBurstDelegatePattern.Match(typeName);
                    if (match.Success)
                    {
                        RetargetBurstType(type, _postfixBurstDelegatePattern);
                        continue;
                    }
                }
                else
                {
                    Match match = _jobReflectionRegistrationOutputPattern.Match(typeName);
                    if (match.Success)
                    {
                        RetargetJobReflectionRegistrationOutput(type, _jobReflectionRegistrationOutputPattern);
                    }
                }
            }
            // CustomAttribute blobs store System.Type as a type name string.
            // TypeDef.Name retargeting does not rewrite those blobs, so they must be
            // rebuilt after rename. Burst nested typeof() names also incorrectly repeat
            // the namespace, which is corrected here.
            RetargetTypeRefInCustomAttributes();
        }

        private string GetBurstOriginalName(string typeName, Regex pattern)
        {
            Match match = pattern.Match(typeName);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        private MethodDef GetBurstDelegateInvokeMethod(TypeDef type)
        {
            return type.Methods.FirstOrDefault(m => m.Name == "Invoke");
        }

        private HashSet<TypeDef> _oldRetargedTypes = new HashSet<TypeDef>();

        private void RetargetBurstType(TypeDef newType, Regex typeNamPat)
        {
            //UnityEngine.Debug.Log($"retarget type:{newType.FullName}");
            string declaringTypeName = newType.DeclaringType.FullName;
            TypeDef oldDeclaringType = _oldAss.Find(declaringTypeName, false);
            if (oldDeclaringType == null)
            {
                UnityEngine.Debug.LogError($"retarget type:{newType}. cant' find declaringType in old assembly");
                return;
            }
            string burstName = GetBurstOriginalName(newType.Name, typeNamPat);
            MethodDef invokeMethod = GetBurstDelegateInvokeMethod(newType);
            if (invokeMethod == null)
            {
                UnityEngine.Debug.LogError($"retarget type:{newType.FullName} cant' find Invoke method");
                return;
            }
            //UnityEngine.Debug.Log($"retarget type:{newType.FullName} burstName:{burstName}");
            foreach (var oldType in oldDeclaringType.NestedTypes)
            {
                string subBurstName = GetBurstOriginalName(oldType.Name, typeNamPat);
                if (subBurstName == null || burstName != subBurstName)
                {
                    continue;
                }
                MethodDef subInvokeMethod = GetBurstDelegateInvokeMethod(oldType);
                if (subInvokeMethod == null)
                {
                    UnityEngine.Debug.LogError($"retarget type:{newType.FullName} cant' find Invoke method in subType:{oldType.FullName}");
                    continue;
                }
                if (subInvokeMethod.MethodSig.ToString() != invokeMethod.MethodSig.ToString())
                {
                    continue;
                }
                if (!_oldRetargedTypes.Add(oldType))
                {
                    UnityEngine.Debug.LogWarning($"retarget type:{newType.FullName} subType:{oldType.FullName} already retargeted");
                    continue;
                }
                UnityEngine.Debug.Log($"retarget newType:{newType.FullName} ==> oldType:{oldType.FullName}");
                newType.Name = oldType.Name;
                _anyRetargeted = true;
            }
        }

        private MethodDef GetEarlyInitMethod(TypeDef type)
        {
            return type.Methods.FirstOrDefault(m => m.Name == "EarlyInit");
        }

        private void RetargetJobReflectionRegistrationOutput(TypeDef newType, Regex typeNamPat)
        {
            string newBurstName = GetBurstOriginalName(newType.Name, typeNamPat);
            MethodDef newEarlyInitMethod = GetEarlyInitMethod(newType);
            if (newEarlyInitMethod == null)
            {
                UnityEngine.Debug.LogError($"retarget type:{newType.FullName} cant' find EarlyInit method");
                return;
            }
            //UnityEngine.Debug.Log($"retarget type:{newType.FullName} burstName:{burstName}");
            foreach (var oldType in _oldAss.GetTypes())
            {
                string oldBurstName = GetBurstOriginalName(oldType.Name, typeNamPat);
                if (oldBurstName == null)
                {
                    continue;
                }
                if (newBurstName == oldBurstName)
                {
                    break;
                }
                MethodDef oldEarlyInitMethod = GetEarlyInitMethod(oldType);
                if (oldEarlyInitMethod == null)
                {
                    UnityEngine.Debug.LogError($"retarget type:{newType.FullName} cant' find Invoke method in subType:{oldType.FullName}");
                    continue;
                }
                if (!_oldRetargedTypes.Add(oldType))
                {
                    UnityEngine.Debug.LogWarning($"retarget type:{newType.FullName} subType:{oldType.FullName} already retargeted");
                    continue;
                }
                UnityEngine.Debug.Log($"retarget newType:{newType.FullName} ==> oldType:{oldType.FullName}");
                newType.Name = oldType.Name;
                _anyRetargeted = true;
            }
        }

        private static bool MayRenameCustomDataType(ElementType type)
        {
            return type == ElementType.Class || type == ElementType.ValueType || type == ElementType.Object || type == ElementType.SZArray;
        }

        private void CollectCustomAttributeArguments(IHasCustomAttribute meta, List<CustomAttributeInfo> customAttributes)
        {
            int index = 0;
            foreach (CustomAttribute ca in meta.CustomAttributes)
            {
                List<CAArgument> arguments = null;
                if (ca.ConstructorArguments.Any(a => MayRenameCustomDataType(a.Type.ElementType)))
                {
                    arguments = ca.ConstructorArguments.ToList();
                }
                List<CANamedArgument> namedArguments = ca.NamedArguments.Count > 0 ? ca.NamedArguments.ToList() : null;
                if (arguments != null || namedArguments != null)
                {
                    customAttributes.Add(new CustomAttributeInfo
                    {
                        customAttributes = meta.CustomAttributes,
                        index = index,
                        arguments = arguments,
                        namedArguments = namedArguments
                    });
                }
                ++index;
            }
        }

        private List<CustomAttributeInfo> CollectCustomAttributeArguments()
        {
            var customAttributes = new List<CustomAttributeInfo>();
            if (_newAss.Assembly != null)
            {
                CollectCustomAttributeArguments(_newAss.Assembly, customAttributes);
            }
            CollectCustomAttributeArguments(_newAss, customAttributes);
            foreach (TypeDef type in _newAss.GetTypes())
            {
                CollectCustomAttributeArguments(type, customAttributes);
                foreach (FieldDef field in type.Fields)
                {
                    CollectCustomAttributeArguments(field, customAttributes);
                }
                foreach (MethodDef method in type.Methods)
                {
                    CollectCustomAttributeArguments(method, customAttributes);
                    foreach (Parameter param in method.Parameters)
                    {
                        if (param.ParamDef != null)
                        {
                            CollectCustomAttributeArguments(param.ParamDef, customAttributes);
                        }
                    }
                }
                foreach (PropertyDef property in type.Properties)
                {
                    CollectCustomAttributeArguments(property, customAttributes);
                }
                foreach (EventDef eventDef in type.Events)
                {
                    CollectCustomAttributeArguments(eventDef, customAttributes);
                }
            }
            return customAttributes;
        }

        private void RetargetTypeRefInCustomAttributes()
        {
            int changedCount = 0;
            foreach (CustomAttributeInfo cai in CollectCustomAttributeArguments())
            {
                CustomAttribute ca = cai.customAttributes[cai.index];
                bool anyChange = false;
                if (cai.arguments != null)
                {
                    for (int i = 0; i < cai.arguments.Count; i++)
                    {
                        CAArgument oldArg = cai.arguments[i];
                        if (TryRetargetTypeRefInArgument(oldArg, out CAArgument newArg))
                        {
                            anyChange = true;
                            cai.arguments[i] = newArg;
                        }
                    }
                }
                if (cai.namedArguments != null)
                {
                    for (int i = 0; i < cai.namedArguments.Count; i++)
                    {
                        if (TryRetargetTypeRefInNamedArgument(cai.namedArguments[i]))
                        {
                            anyChange = true;
                        }
                    }
                }
                if (anyChange)
                {
                    cai.customAttributes[cai.index] = new CustomAttribute(ca.Constructor,
                        cai.arguments != null ? cai.arguments : ca.ConstructorArguments,
                        cai.namedArguments != null ? cai.namedArguments : ca.NamedArguments);
                    ++changedCount;
                    _anyRetargeted = true;
                }
            }
            if (changedCount > 0)
            {
                UnityEngine.Debug.Log($"assembly:{_newAss.Name} retarget {changedCount} custom attribute System.Type argument(s).");
            }
        }

        private bool TryRetargetTypeRefInArgument(CAArgument oldArg, out CAArgument newArg)
        {
            TypeSig newType = RetargetTypeRefInTypeSig(oldArg.Type);
            object newValue = RetargetTypeRefInTypeSigOfValue(oldArg.Value);
            if (newType != oldArg.Type || oldArg.Value != newValue)
            {
                newArg = new CAArgument(newType, newValue);
                return true;
            }
            newArg = default;
            return false;
        }

        private bool TryRetargetTypeRefInNamedArgument(CANamedArgument arg)
        {
            bool anyChange = false;
            TypeSig newType = RetargetTypeRefInTypeSig(arg.Type);
            if (newType != arg.Type)
            {
                anyChange = true;
                arg.Type = newType;
            }
            if (TryRetargetTypeRefInArgument(arg.Argument, out var newArg))
            {
                arg.Argument = newArg;
                anyChange = true;
            }
            return anyChange;
        }

        private object RetargetTypeRefInTypeSigOfValue(object oldValue)
        {
            if (oldValue == null)
            {
                return null;
            }
            if (oldValue.GetType().IsPrimitive)
            {
                return oldValue;
            }
            if (oldValue is string || oldValue is UTF8String)
            {
                return oldValue;
            }
            if (oldValue is TypeSig typeSig)
            {
                return RetargetTypeRefInTypeSig(typeSig);
            }
            if (oldValue is CAArgument caValue)
            {
                TypeSig newType = RetargetTypeRefInTypeSig(caValue.Type);
                object newValue = RetargetTypeRefInTypeSigOfValue(caValue.Value);
                if (newType != caValue.Type || newValue != caValue.Value)
                {
                    return new CAArgument(newType, newValue);
                }
                return oldValue;
            }
            if (oldValue is IList<CAArgument> oldArr)
            {
                bool anyChange = false;
                var newArr = new List<CAArgument>();
                foreach (CAArgument oldArg in oldArr)
                {
                    if (TryRetargetTypeRefInArgument(oldArg, out var newArg))
                    {
                        anyChange = true;
                        newArr.Add(newArg);
                    }
                    else
                    {
                        newArr.Add(oldArg);
                    }
                }
                return anyChange ? newArr : oldArr;
            }
            throw new NotSupportedException($"type:{oldValue.GetType()} value:{oldValue}");
        }

        private TypeSig RetargetTypeRefInTypeSig(TypeSig type)
        {
            if (type == null)
            {
                return null;
            }
            TypeSig next = type.Next;
            TypeSig newNext = next != null ? RetargetTypeRefInTypeSig(next) : null;
            if (type.IsModifier || type.IsPinned)
            {
                if (next == newNext)
                {
                    return type;
                }
                if (type is CModReqdSig cmrs)
                {
                    return new CModReqdSig(cmrs.Modifier, newNext);
                }
                if (type is CModOptSig cmos)
                {
                    return new CModOptSig(cmos.Modifier, newNext);
                }
                if (type is PinnedSig)
                {
                    return new PinnedSig(newNext);
                }
                throw new NotSupportedException(type.ToString());
            }
            switch (type.ElementType)
            {
                case ElementType.Ptr:
                    return next == newNext ? type : new PtrSig(newNext);
                case ElementType.ByRef:
                    return next == newNext ? type : new ByRefSig(newNext);
                case ElementType.SZArray:
                    return next == newNext ? type : new SZArraySig(newNext);
                case ElementType.Array:
                {
                    if (next == newNext)
                    {
                        return type;
                    }
                    var arraySig = (ArraySig)type;
                    return new ArraySig(newNext, arraySig.Rank, arraySig.Sizes, arraySig.LowerBounds);
                }
                case ElementType.ValueType:
                case ElementType.Class:
                {
                    var vts = (ClassOrValueTypeSig)type;
                    TypeDef typeDef = ResolveTypeDefInCurrentModule(vts.TypeDefOrRef);
                    if (typeDef == null)
                    {
                        return type;
                    }
                    ITypeDefOrRef encoded = CreateEncodedTypeDefOrRef(typeDef);
                    return type.IsClassSig ? (TypeSig)new ClassSig(encoded) : new ValueTypeSig(encoded);
                }
                case ElementType.GenericInst:
                {
                    var gis = (GenericInstSig)type;
                    ClassOrValueTypeSig genericType = gis.GenericType;
                    ClassOrValueTypeSig newGenericType = (ClassOrValueTypeSig)RetargetTypeRefInTypeSig(genericType);
                    bool anyChange = genericType != newGenericType;
                    var genericArgs = new List<TypeSig>();
                    foreach (var arg in gis.GenericArguments)
                    {
                        TypeSig newArg = RetargetTypeRefInTypeSig(arg);
                        anyChange |= newArg != arg;
                        genericArgs.Add(newArg);
                    }
                    if (!anyChange)
                    {
                        return type;
                    }
                    return new GenericInstSig(newGenericType, genericArgs);
                }
                default:
                    return type;
            }
        }

        private TypeDef ResolveTypeDefInCurrentModule(ITypeDefOrRef typeDefOrRef)
        {
            if (typeDefOrRef == null)
            {
                return null;
            }
            if (typeDefOrRef is TypeDef typeDef)
            {
                return typeDef.Module == _newAss ? typeDef : null;
            }
            if (!(typeDefOrRef is TypeRef typeRef))
            {
                return null;
            }
            if (typeRef.ResolutionScope is ITypeDefOrRef)
            {
                TypeDef declaringType = ResolveTypeDefInCurrentModule(typeRef.ResolutionScope as ITypeDefOrRef);
                if (declaringType == null)
                {
                    return null;
                }
                return FindNestedType(declaringType, typeRef.Name);
            }
            TypeDef resolved = _newAss.Find(typeRef);
            if (resolved != null)
            {
                return resolved;
            }
            string fullName = UTF8String.IsNullOrEmpty(typeRef.Namespace) ? typeRef.Name.String : typeRef.Namespace + "." + typeRef.Name;
            return _newAss.Find(fullName, false);
        }

        private TypeDef FindNestedType(TypeDef declaringType, UTF8String name)
        {
            foreach (TypeDef nested in declaringType.NestedTypes)
            {
                if (nested.Name == name)
                {
                    return nested;
                }
            }
            Regex pattern = GetBurstGeneratedTypeNamePattern(name);
            if (pattern == null)
            {
                return null;
            }
            string burstName = GetBurstOriginalName(name, pattern);
            if (burstName == null)
            {
                return null;
            }
            foreach (TypeDef nested in declaringType.NestedTypes)
            {
                if (GetBurstOriginalName(nested.Name, pattern) == burstName)
                {
                    return nested;
                }
            }
            return null;
        }

        private Regex GetBurstGeneratedTypeNamePattern(UTF8String name)
        {
            if (_burstDirectCallPattern.IsMatch(name))
            {
                return _burstDirectCallPattern;
            }
            if (_postfixBurstDelegatePattern.IsMatch(name))
            {
                return _postfixBurstDelegatePattern;
            }
            if (_jobReflectionRegistrationOutputPattern.IsMatch(name))
            {
                return _jobReflectionRegistrationOutputPattern;
            }
            return null;
        }

        private ITypeDefOrRef CreateEncodedTypeDefOrRef(TypeDef typeDef)
        {
            if (typeDef.DeclaringType == null)
            {
                return typeDef;
            }
            // Nested types must be encoded as Outer+Name. Burst incorrectly stores the
            // namespace on nested TypeDefs, which would serialize as Outer+Namespace.Name.
            TypeRef declaringRef = CreateEncodedDeclaringTypeRef(typeDef.DeclaringType);
            return new TypeRefUser(_newAss, UTF8String.Empty, typeDef.Name, declaringRef);
        }

        private TypeRef CreateEncodedDeclaringTypeRef(TypeDef typeDef)
        {
            if (typeDef.DeclaringType != null)
            {
                TypeRef declaringRef = CreateEncodedDeclaringTypeRef(typeDef.DeclaringType);
                return new TypeRefUser(_newAss, UTF8String.Empty, typeDef.Name, declaringRef);
            }
            IResolutionScope scope;
            if (_newAss.Assembly != null)
            {
                scope = _newAss.UpdateRowId(_newAss.Assembly.ToAssemblyRef());
            }
            else
            {
                scope = _newAss;
            }
            return new TypeRefUser(_newAss, typeDef.Namespace, typeDef.Name, scope);
        }

        public void Save(string outputDll)
        {
            if (!_anyRetargeted)
            {
                UnityEngine.Debug.LogWarning($"assembly:{_newAss.Name} no type retargeted");
                File.WriteAllBytes(outputDll, _newDllBytes);
                return;
            }
            UnityEngine.Debug.Log($"assembly:{_newAss.Name} retarget {_oldRetargedTypes.Count} type.");

            var options = new ModuleWriterOptions(_newAss);
            options.MetadataOptions.Flags |= MetadataFlags.PreserveRids;
            _newAss.Write(outputDll, options);
        }
    }
}
