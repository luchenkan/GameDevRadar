using dnlib.DotNet;
using HybridCLR.Editor.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HybridCLR.Editor.DHE
{

    public class TypeCompareCache
    {
        private readonly Snapshot _old;
        private readonly Snapshot _new;

        public Snapshot OldSnapshot => _old;
        public Snapshot NewSnapshot => _new;

        public class TypeInstanceCompareData
        {
            public ITypeDefOrRef oldType;
            public ITypeDefOrRef newType;
            public int lastLayoutEqualFieldIndex;
            public bool layoutEqualIgnoreTypeSigMatch;
            public bool layoutAndTypeSigEqual;
            public bool referenceEqual; // both class or (both struct and layoutEqual) or (both enum and underlyingType equal)
        }

        public class TypeStaticCompareData
        {
            public ITypeDefOrRef oldType;
            public ITypeDefOrRef newType;
            public int lastLayoutEqualFieldIndex;
            public bool layoutEqual;
        }

        private struct TypePair : IEquatable<TypePair>
        {
            public readonly ITypeDefOrRef oldType;
            public readonly ITypeDefOrRef newType;
            private readonly int _hashCode;
            public TypePair(ITypeDefOrRef oldType, ITypeDefOrRef newType)
            {
                this.oldType = oldType;
                this.newType = newType;
                var cmp = TypeEqualityComparer.Instance;
                _hashCode = HashUtil.CombineHash(cmp.GetHashCode(oldType), cmp.GetHashCode(newType));
            }

            public override bool Equals(object obj)
            {
                return Equals((TypePair)obj);
            }

            public bool Equals(TypePair other)
            {
                var cmp = TypeEqualityComparer.Instance;
                return cmp.Equals(other.oldType, this.oldType) && cmp.Equals(other.newType, this.newType);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override string ToString()
            {
                return $"{oldType}|{newType}";
            }
        }

        public class TypeFieldInfo
        {
            public ITypeDefOrRef type;
            public TypeDef selfOrGenericBaseType;
            public ITypeDefOrRef parentType;
            public List<FieldDef> fields;
            public List<FieldDef> instanceFields;
            public List<FieldDef> staticFields;
            public List<FieldDef> threadStaticFields;
            public List<TypeSig> fieldTypes;
            public List<TypeSig> instanceFieldTypes;
            public List<TypeSig> staticFieldTypes;
            public List<TypeSig> threadStaticFieldTypes;
            public LayoutKind layoutKind;

            public List<FieldDef> GetStaticFields(bool threadStatic)
            {
                return threadStatic ? threadStaticFields : staticFields;
            }

            public List<TypeSig> GetStaticFieldTypes(bool threadStatic)
            {
                return threadStatic ? threadStaticFieldTypes : staticFieldTypes;
            }
        }

        private readonly Dictionary<ITypeDefOrRef, TypeFieldInfo> _oldTypeFieldInfos = new Dictionary<ITypeDefOrRef, TypeFieldInfo>(TypeEqualityComparer.Instance);
        private readonly Dictionary<ITypeDefOrRef, TypeFieldInfo> _newTypeFieldInfos = new Dictionary<ITypeDefOrRef, TypeFieldInfo>(TypeEqualityComparer.Instance);


        private readonly Dictionary<TypePair, TypeInstanceCompareData> _typeCompareCache = new Dictionary<TypePair, TypeInstanceCompareData>();
        private readonly Dictionary<TypePair, TypeStaticCompareData> _typeStaticCompareCache = new Dictionary<TypePair, TypeStaticCompareData>();
        private readonly Dictionary<TypePair, TypeStaticCompareData> _typeThreadStaticCompareCache = new Dictionary<TypePair, TypeStaticCompareData>();

        public TypeCompareCache(Snapshot old, Snapshot @new)
        {
            _old = old;
            _new = @new;
        }

        public TypeInstanceCompareData GetInstanceCompareData(ITypeDefOrRef oldType, ITypeDefOrRef newType)
        {
            return TryGetOrAddInstance(oldType, newType);
        }

        public TypeStaticCompareData GetStaticCompareData(ITypeDefOrRef oldType, ITypeDefOrRef newType)
        {
            return TryGetOrAddCompareStatic(oldType, newType, false);
        }

        public TypeStaticCompareData GetThreadStaticCompareData(ITypeDefOrRef oldType, ITypeDefOrRef newType)
        {
            return TryGetOrAddCompareStatic(oldType, newType, true);
        }

        private static LayoutKind GetLayoutKind(TypeDef type)
        {
            if (type.IsAutoLayout)
            {
                return LayoutKind.Auto;
            }
            if (type.IsSequentialLayout)
            {
                return LayoutKind.Sequential;
            }
            if (type.IsExplicitLayout)
            {
                return LayoutKind.Explicit;
            }
            throw new Exception();
        }


        private TypeFieldInfo GetTypeInfo(ITypeDefOrRef type, Dictionary<ITypeDefOrRef, TypeFieldInfo> typeInfos)
        {
            if (typeInfos.TryGetValue(type, out var info))
            {
                return info;
            }
            info = new TypeFieldInfo { type = type };
            TypeSig typeSig = type.ToTypeSig();

            switch (typeSig.ElementType)
            {
                case ElementType.Class:
                case ElementType.ValueType:
                {
                    TypeDef typeDef = type.ResolveTypeDefThrow();
                    info.layoutKind = GetLayoutKind(typeDef);
                    info.selfOrGenericBaseType = typeDef;
                    info.parentType = typeDef.BaseType;
                    info.fields = typeDef.Fields.ToList();
                    info.instanceFields = GetTypeFields(typeDef, FieldLocationType.Instance);
                    info.staticFields = GetTypeFields(typeDef, FieldLocationType.Static);
                    info.threadStaticFields = GetTypeFields(typeDef, FieldLocationType.ThreadStatic);
                    info.fieldTypes = info.fields.Select(f => f.FieldType).ToList();
                    info.instanceFieldTypes = info.instanceFields.Select(f => f.FieldType).ToList();
                    info.staticFieldTypes = info.staticFields.Select(f => f.FieldType).ToList();
                    info.threadStaticFieldTypes = info.threadStaticFields.Select(f => f.FieldType).ToList();
                    break;
                }
                case ElementType.GenericInst:
                {
                    GenericInstSig gis = typeSig.ToGenericInstSig();
                    TypeDef typeDef = gis.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
                    info.layoutKind = GetLayoutKind(typeDef);
                    info.selfOrGenericBaseType = typeDef;
                    info.fields = typeDef.Fields.ToList();
                    info.instanceFields = GetTypeFields(typeDef, FieldLocationType.Instance);
                    info.staticFields = GetTypeFields(typeDef, FieldLocationType.Static);
                    info.threadStaticFields = GetTypeFields(typeDef, FieldLocationType.ThreadStatic);

                    var gac = new GenericArgumentContext(gis.GenericArguments.ToList(), null);
                    info.parentType = typeDef.BaseType != null ? MetaUtil.Inflate(typeDef.BaseType.ToTypeSig(), gac).ToTypeDefOrRef() : null;
                    info.fieldTypes = info.fields.Select(f => MetaUtil.Inflate(f.FieldType, gac)).ToList();
                    info.instanceFieldTypes = info.instanceFields.Select(f => MetaUtil.Inflate(f.FieldType, gac)).ToList();
                    info.staticFieldTypes = info.staticFields.Select(f => MetaUtil.Inflate(f.FieldType, gac)).ToList();
                    info.threadStaticFieldTypes = info.threadStaticFields.Select(f => MetaUtil.Inflate(f.FieldType, gac)).ToList();
                    break;
                }
                default: throw new NotImplementedException(type.ToString());
            }
            typeInfos.Add(type, info);
            return info;
        }


        public int GetFieldIndex(FieldDef field, FieldFamily family, bool old)
        {
            var ti = GetTypeInfo(field.DeclaringType, old ? _oldTypeFieldInfos : _newTypeFieldInfos);
            switch (family)
            {
                case FieldFamily.Instance: return ti.instanceFields.IndexOf(field);
                case FieldFamily.Static: return ti.staticFields.IndexOf(field);
                case FieldFamily.ThreadStatic: return ti.threadStaticFields.IndexOf(field);
                default: throw new Exception();
            }
        }

        //private TypeFieldInfo GetTypeInfo(ITypeDefOrRef type, bool old)
        //{
        //    return GetTypeInfo(type, old ? _oldTypeFieldInfos : _newTypeFieldInfos);
        //}


        private TypeInstanceCompareData TryGetOrAddInstance(ITypeDefOrRef oldType, ITypeDefOrRef newType)
        {
            var key = new TypePair(oldType, newType);
            if (_typeCompareCache.TryGetValue(key, out var compareData))
            {
                return compareData;
            }
            compareData = CompareTypeInstance(oldType, newType);
            _typeCompareCache.Add(key, compareData);
            return compareData;
        }

        private TypeStaticCompareData TryGetOrAddCompareStatic(ITypeDefOrRef oldType, ITypeDefOrRef newType, bool threadStatic)
        {
            var key = new TypePair(oldType, newType);
            Dictionary<TypePair, TypeStaticCompareData> compareCache = threadStatic ? _typeThreadStaticCompareCache : _typeStaticCompareCache;
            if (compareCache.TryGetValue(key, out var compareData))
            {
                return compareData;
            }

            compareData = CompareTypeStatic(oldType, newType, threadStatic);
            compareCache.Add(key, compareData);
            return compareData;
        }

        private bool IsClassOrGenericInstClass(TypeSig typeSig)
        {
            switch (typeSig.ElementType)
            {
                case ElementType.Class: return true;
                case ElementType.ValueType: return false;
                case ElementType.GenericInst: return IsClassOrGenericInstClass(typeSig.ToGenericInstSig().GenericType.ToTypeDefOrRefSig());
                default: return false;
            }
        }

        enum FieldLocationType
        {
            Instance,
            Static,
            ThreadStatic,
        }

        private static bool IsLocationType(FieldDef field, FieldLocationType locationType)
        {
            switch (locationType)
            {
                case FieldLocationType.Static: return field.IsStatic && !field.IsLiteral && !MetaUtil.IsThreadStaticField(field);
                case FieldLocationType.ThreadStatic: return field.IsStatic && !field.IsLiteral && MetaUtil.IsThreadStaticField(field);
                case FieldLocationType.Instance: return !field.IsStatic;
                default: throw new NotImplementedException();
            }
        }

        private List<FieldDef> GetTypeFields(TypeDef type, FieldLocationType locationType)
        {
            return type.Fields.Where(f => IsLocationType(f, locationType)).ToList();
        }


        private bool IsNoneOrObjectType(ITypeDefOrRef type)
        {
            if (type == null)
            {
                return true;
            }
            TypeSig typeSig = type.ToTypeSig();
            return typeSig.ElementType == ElementType.Object;
        }

        private static bool IsSameTypeFamily(TypeDef oldType, TypeDef newType)
        {
            if (oldType.IsEnum ^ newType.IsEnum)
            {
                return false;
            }
            if (oldType.IsValueType ^ newType.IsValueType)
            {
                return false;
            }
            if (oldType.IsClass ^ newType.IsClass)
            {
                return false;
            }
            if (oldType.IsInterface ^ newType.IsInterface)
            {
                return false;
            }
            return true;
        }

        private static bool IsSameStructLayout(TypeDef oldType, TypeDef newType, List<FieldDef> oldInstanceFields, List<FieldDef> newInstanceFields)
        {
            LayoutKind oldLayout = GetLayoutKind(oldType);
            LayoutKind newLayout = GetLayoutKind(newType);
            if (oldLayout != newLayout)
            {
                return false;
            }
            if (oldType.ClassSize != newType.ClassSize)
            {
                return false;
            }
            switch (oldLayout)
            {
                case LayoutKind.Auto:
                {
                    return true;
                }
                case LayoutKind.Sequential:
                {
                    return oldType.PackingSize == newType.PackingSize;
                }
                case LayoutKind.Explicit:
                {
                    if (oldType.PackingSize != newType.PackingSize)
                    {
                        return false;
                    }
                    if (oldInstanceFields.Count != newInstanceFields.Count)
                    {
                        return false;
                    }
                    for (int i = 0; i < oldInstanceFields.Count; i++)
                    {
                        if (oldInstanceFields[i].FieldOffset != newInstanceFields[i].FieldOffset)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                default: throw new NotSupportedException();
            }
        }

        private TypeInstanceCompareData CompareTypeInstance(ITypeDefOrRef oldType, ITypeDefOrRef newType)
        {
            var oldTypeSig = oldType.ToTypeSig();
            var newTypeSig = newType.ToTypeSig();

            var cmpData = new TypeInstanceCompareData()
            {
                oldType = oldType,
                newType = newType,
                lastLayoutEqualFieldIndex = -1
            };
            if (oldTypeSig.ElementType != newTypeSig.ElementType || !TypeEqualityComparer.Instance.Equals(oldType, newType))
            {
                cmpData.referenceEqual = IsClassOrGenericInstClass(oldTypeSig) && IsClassOrGenericInstClass(newTypeSig);
                return cmpData;
            }

            var oldTypeInfo = GetTypeInfo(oldType, _oldTypeFieldInfos);
            var newTypeInfo = GetTypeInfo(newType, _newTypeFieldInfos);
            TypeDef oldTypeDef = oldTypeInfo.selfOrGenericBaseType;
            TypeDef newTypeDef = newTypeInfo.selfOrGenericBaseType;
            if (!IsSameTypeFamily(oldTypeDef, newTypeDef) || !IsSameStructLayout(oldTypeDef, newTypeDef, oldTypeInfo.instanceFields, newTypeInfo.instanceFields))
            {
                cmpData.referenceEqual = IsClassOrGenericInstClass(oldTypeSig) && IsClassOrGenericInstClass(newTypeSig);
                return cmpData;
            }
            switch (oldTypeSig.ElementType)
            {
                case ElementType.Class:
                {
                    cmpData.referenceEqual = true;
                    bool baseTypeLayoutAndTypeSiqEqual = true;
                    if (!IsNoneOrObjectType(oldTypeInfo.parentType) || !IsNoneOrObjectType(newTypeInfo.parentType))
                    {
                        var baseCmpData = TryGetOrAddInstance(oldTypeInfo.parentType, newTypeInfo.parentType);
                        if (!baseCmpData.layoutEqualIgnoreTypeSigMatch)
                        {
                            cmpData.layoutAndTypeSigEqual = false;
                            return cmpData;
                        }
                        baseTypeLayoutAndTypeSiqEqual = baseCmpData.layoutAndTypeSigEqual;
                    }
                    cmpData.layoutAndTypeSigEqual = oldTypeDef.IsInterface ||
                        (CompareFields(oldTypeInfo.instanceFieldTypes, newTypeInfo.instanceFieldTypes, out cmpData.layoutEqualIgnoreTypeSigMatch, out cmpData.lastLayoutEqualFieldIndex) && baseTypeLayoutAndTypeSiqEqual);
                    break;
                }
                case ElementType.ValueType:
                {
                    if (oldTypeDef.IsEnum)
                    {
                        cmpData.layoutAndTypeSigEqual = CompareAnyReferenceEqual(oldTypeDef.GetEnumUnderlyingType(), newTypeDef.GetEnumUnderlyingType());
                    }
                    else
                    {
                        cmpData.layoutAndTypeSigEqual = CompareFields(oldTypeInfo.instanceFieldTypes, newTypeInfo.instanceFieldTypes, out cmpData.layoutEqualIgnoreTypeSigMatch, out cmpData.lastLayoutEqualFieldIndex);
                    }
                    cmpData.referenceEqual = cmpData.layoutAndTypeSigEqual;
                    break;
                }
                case ElementType.GenericInst:
                {
                    if (oldTypeInfo.selfOrGenericBaseType.IsValueType)
                    {
                        if (oldTypeDef.IsEnum)
                        {
                            cmpData.layoutAndTypeSigEqual = CompareAnyReferenceEqual(oldTypeDef.GetEnumUnderlyingType(), newTypeDef.GetEnumUnderlyingType());
                        }
                        else
                        {
                            cmpData.layoutAndTypeSigEqual = CompareFields(oldTypeInfo.instanceFieldTypes, newTypeInfo.instanceFieldTypes, out cmpData.layoutEqualIgnoreTypeSigMatch, out cmpData.lastLayoutEqualFieldIndex);
                        }
                        cmpData.referenceEqual = cmpData.layoutAndTypeSigEqual;
                    }
                    else
                    {
                        cmpData.referenceEqual = true;
                        if (!IsNoneOrObjectType(oldTypeInfo.parentType) || !IsNoneOrObjectType(newTypeInfo.parentType))
                        {
                            var baseCmpData = TryGetOrAddInstance(oldTypeInfo.parentType, newTypeInfo.parentType);
                            if (!baseCmpData.layoutEqualIgnoreTypeSigMatch)
                            {
                                cmpData.layoutAndTypeSigEqual = false;
                                return cmpData;
                            }
                        }
                        cmpData.layoutAndTypeSigEqual = oldTypeDef.IsInterface || CompareFields(oldTypeInfo.instanceFieldTypes, newTypeInfo.instanceFieldTypes, out cmpData.layoutEqualIgnoreTypeSigMatch, out cmpData.lastLayoutEqualFieldIndex);
                    }
                    break;
                }
            }

            return cmpData;
        }

        private bool CompareFields(List<TypeSig> oldFields, List<TypeSig> newFields, out bool layoutEqualIgnoreTypeSigMatch,  out int lastLayoutEqualFieldIndex)
        {
            lastLayoutEqualFieldIndex = -1;
            layoutEqualIgnoreTypeSigMatch = oldFields.Count == newFields.Count;
            for (int i = 0, n = Math.Min(newFields.Count, oldFields.Count); i < n; i++)
            {
                if (!CompareAnyReferenceEqualIgnoreTypeMatch(oldFields[i], newFields[i]))
                {
                    layoutEqualIgnoreTypeSigMatch = false;
                    break;
                }
                else
                {
                    lastLayoutEqualFieldIndex = i;
                }
            }

            for (int i = 0, n = Math.Min(newFields.Count, oldFields.Count); i < n; i++)
            {
                if (!CompareAnyReferenceEqual(oldFields[i], newFields[i]))
                {
                    return false;
                }
            }
            return oldFields.Count == newFields.Count;
        }

        private bool IsClassOrReferenceType(TypeSig typeSig)
        {
            switch (typeSig.ElementType)
            {
                case ElementType.String:
                case ElementType.Ptr:
                case ElementType.Array:
                case ElementType.FnPtr:
                case ElementType.Object:
                case ElementType.SZArray:
                case ElementType.ByRef:
                case ElementType.Class: return true;
                case ElementType.GenericInst:
                {
                    GenericInstSig gis1 = typeSig.ToGenericInstSig();
                    TypeDef gt1 = gis1.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
                    return !gt1.IsValueType;
                }
                //case ElementType.CModReqd:
                //case ElementType.CModOpt:
                //case ElementType.Internal:
                //case ElementType.Module:
                //case ElementType.Sentinel:
                //case ElementType.Pinned:
                default: return false;
            }
        }

        public bool CompareAnyReferenceEqualIgnoreTypeMatch(TypeSig oldType, TypeSig newType)
        {
            oldType = oldType.RemovePinnedAndModifiers();
            newType = newType.RemovePinnedAndModifiers();

            ElementType oldElementType = oldType.ElementType;
            ElementType newElementType = newType.ElementType;
            //if (oldElementType != newElementType)
            //{
            //    return IsClassOrReferenceType(oldType) && IsClassOrReferenceType(newType);
            //}
            switch (oldType.ElementType)
            {
                case ElementType.Void:
                case ElementType.R: 
                case ElementType.TypedByRef: return oldElementType == newElementType;
                case ElementType.Boolean:
                case ElementType.I1:
                case ElementType.U1: return newElementType == ElementType.Boolean || newElementType == ElementType.I1 || newElementType == ElementType.U1;
                case ElementType.Char:
                case ElementType.I2:
                case ElementType.U2: return newElementType == ElementType.Char || newElementType == ElementType.I2 || newElementType == ElementType.U2;
                case ElementType.I4:
                case ElementType.U4:
                case ElementType.R4: return newElementType == ElementType.I4 || newElementType == ElementType.U4 || newElementType == ElementType.R4;
                case ElementType.I8:
                case ElementType.U8:
                case ElementType.R8: return newElementType == ElementType.I8 || newElementType == ElementType.U8 || newElementType == ElementType.R8;
                case ElementType.I:
                case ElementType.U: return newElementType == ElementType.I || newElementType == ElementType.U;
                case ElementType.String:
                case ElementType.Ptr:
                case ElementType.Array:
                case ElementType.FnPtr:
                case ElementType.Object:
                case ElementType.SZArray:
                case ElementType.Class:
                case ElementType.ByRef:
                    return IsClassOrReferenceType(newType);
                case ElementType.ValueType:
                {
                    if (oldElementType != newElementType)
                    {
                        return false;
                    }
                    TypeDef oldTypeDef = oldType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    TypeDef newTypeDef = newType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    if (oldTypeDef.IsEnum)
                    {
                        if (newTypeDef.IsEnum)
                        {
                            return CompareAnyReferenceEqualIgnoreTypeMatch(oldTypeDef.GetEnumUnderlyingType(), newTypeDef.GetEnumUnderlyingType());
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (newTypeDef.IsEnum)
                    {
                        return false;
                    }
                    return TryGetOrAddInstance(oldTypeDef, newTypeDef).referenceEqual;
                }
                case ElementType.Var:
                {
                    if (oldElementType != newElementType)
                    {
                        return false;
                    }
                    return oldType.ToGenericVar().Number == newType.ToGenericVar().Number;
                }
                case ElementType.MVar:
                {
                    if (oldElementType != newElementType)
                    {
                        return false;
                    }
                    return oldType.ToGenericMVar().Number == newType.ToGenericMVar().Number;
                }
                case ElementType.GenericInst:
                {
                    if (oldElementType != newElementType)
                    {
                        return false;
                    }
                    GenericInstSig oldGis = oldType.ToGenericInstSig();
                    GenericInstSig newGis = newType.ToGenericInstSig();
                    TypeDef oldTypeDef = oldGis.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
                    TypeDef newTypeDef = newGis.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
                    if (oldTypeDef.IsEnum)
                    {
                        if (newTypeDef.IsEnum)
                        {
                            return oldTypeDef.GetEnumUnderlyingType().ElementType == newTypeDef.GetEnumUnderlyingType().ElementType;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (newTypeDef.IsEnum)
                    {
                        return false;
                    }
                    if (oldTypeDef.IsValueType ^ newTypeDef.IsValueType)
                    {
                        return false;
                    }
                    if (oldTypeDef.IsValueType)
                    {
                        return TryGetOrAddInstance(oldType.ToTypeDefOrRef(), newType.ToTypeDefOrRef()).referenceEqual;
                    }
                    else
                    {
                        return true;
                    }
                }
                //case ElementType.CModReqd:
                //case ElementType.CModOpt:
                //case ElementType.Internal:
                //case ElementType.Module:
                //case ElementType.Sentinel:
                //case ElementType.Pinned:
                default: throw new NotSupportedException(oldType.ToString());
            }
        }

        public bool CompareAnyReferenceEqual(TypeSig oldType, TypeSig newType)
        {
            oldType = oldType.RemovePinnedAndModifiers();
            newType = newType.RemovePinnedAndModifiers();
            if (oldType.ElementType != newType.ElementType)
            {
                return IsClassOrReferenceType(oldType) && IsClassOrReferenceType(newType);
            }
            switch (oldType.ElementType)
            {
                case ElementType.Void:
                case ElementType.Boolean:
                case ElementType.Char:
                case ElementType.I1:
                case ElementType.U1:
                case ElementType.I2:
                case ElementType.U2:
                case ElementType.I4:
                case ElementType.U4:
                case ElementType.I8:
                case ElementType.U8:
                case ElementType.R4:
                case ElementType.R8:
                case ElementType.I:
                case ElementType.U:
                case ElementType.R:
                case ElementType.TypedByRef:
                    return true;
                case ElementType.String:
                case ElementType.Ptr:
                case ElementType.Array:
                case ElementType.FnPtr:
                case ElementType.Object:
                case ElementType.SZArray:
                case ElementType.Class:
                case ElementType.ByRef:
                    return IsClassOrReferenceType(newType);
                case ElementType.ValueType:
                {
                    TypeDef oldTypeDef = oldType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    TypeDef newTypeDef = newType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    if (oldTypeDef.IsEnum)
                    {
                        if (newTypeDef.IsEnum)
                        {
                            return oldTypeDef.GetEnumUnderlyingType().ElementType == newTypeDef.GetEnumUnderlyingType().ElementType;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (newTypeDef.IsEnum)
                    {
                        return false;
                    }
                    return TryGetOrAddInstance(oldTypeDef, newTypeDef).referenceEqual;
                }
                case ElementType.Var:
                {
                    return oldType.ToGenericVar().Number == newType.ToGenericVar().Number;
                }
                case ElementType.MVar:
                {
                    return oldType.ToGenericMVar().Number == newType.ToGenericMVar().Number;
                }
                case ElementType.GenericInst:
                {
                    GenericInstSig oldGis = oldType.ToGenericInstSig();
                    GenericInstSig newGis = newType.ToGenericInstSig();
                    TypeDef oldTypeDef = oldGis.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
                    TypeDef newTypeDef = newGis.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
                    if (oldTypeDef.IsEnum)
                    {
                        if (newTypeDef.IsEnum)
                        {
                            return oldTypeDef.GetEnumUnderlyingType().ElementType == newTypeDef.GetEnumUnderlyingType().ElementType;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (newTypeDef.IsEnum)
                    {
                        return false;
                    }
                    if (oldTypeDef.IsValueType ^ newTypeDef.IsValueType)
                    {
                        return false;
                    }
                    if (oldTypeDef.IsValueType)
                    {
                        return TryGetOrAddInstance(oldType.ToTypeDefOrRef(), newType.ToTypeDefOrRef()).referenceEqual;
                    }
                    else
                    {
                        return true;
                    }
                }
                //case ElementType.CModReqd:
                //case ElementType.CModOpt:
                //case ElementType.Internal:
                //case ElementType.Module:
                //case ElementType.Sentinel:
                //case ElementType.Pinned:
                default: throw new NotSupportedException(oldType.ToString());
            }
        }

        private TypeStaticCompareData CompareTypeStatic(ITypeDefOrRef oldType, ITypeDefOrRef newType, bool threadStatic)
        {
            var cmpData = new TypeStaticCompareData()
            {
                oldType = oldType,
                newType = newType,
            };
            if (!TypeEqualityComparer.Instance.Equals(oldType, newType))
            {
                cmpData.lastLayoutEqualFieldIndex = -1;
                cmpData.layoutEqual = false;
                return cmpData;
            }
            var oldTypeInfo = GetTypeInfo(oldType, _oldTypeFieldInfos);
            var newTypeInfo = GetTypeInfo(newType, _newTypeFieldInfos);
            cmpData.layoutEqual = CompareFields(oldTypeInfo.GetStaticFieldTypes(threadStatic), newTypeInfo.GetStaticFieldTypes(threadStatic), out _, out cmpData.lastLayoutEqualFieldIndex);
            return cmpData;
        }
    }
}
