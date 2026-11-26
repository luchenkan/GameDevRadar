using dnlib.DotNet;
using HybridCLR.Editor.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HybridCLR.Editor.DHE
{

    public enum FieldFamily
    {
        Instance,
        Static,
        ThreadStatic,
    }

    public class FieldCompareCache
    {
        private readonly TypeCompareCache _typeCompareCache;

        public FieldCompareCache(TypeCompareCache typeCompareCache)
        {
            _typeCompareCache = typeCompareCache;
        }

        private class FieldPair : IEquatable<FieldPair>
        {
            public readonly IField oldField;
            public readonly IField newField;
            private readonly int _hashCode;

            public FieldPair(IField oldField, IField newField)
            {
                this.oldField = oldField;
                this.newField = newField;
                FieldEqualityComparer cmp = FieldEqualityComparer.CompareDeclaringTypes;
                _hashCode = HashUtil.CombineHash(cmp.GetHashCode(oldField), cmp.GetHashCode(newField));
            }

            public override bool Equals(object obj)
            {
                return Equals((FieldPair)obj);
            }

            public bool Equals(FieldPair a)
            {
                FieldEqualityComparer cmp = FieldEqualityComparer.CompareDeclaringTypes;
                return cmp.Equals(a.oldField, this.oldField) && cmp.Equals(a.newField, this.newField);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override string ToString()
            {
                return $"{oldField}|{newField}";
            }
        }

        private class FieldCompareData
        {
            public IField oldField;
            public IField newField;
            public bool equal;
        }

        private readonly Dictionary<FieldPair, bool> _compareResultCache = new Dictionary<FieldPair, bool>();



        private class FieldMetaInfo
        {
            public IField field;
            public FieldDef fieldDef;
            public ITypeDefOrRef klass; // declaringType after inflated, if need
            public FieldFamily family;
            public TypeSig fieldType;
            public int orderOrOffset;
        }

        private readonly Dictionary<IField, FieldMetaInfo> _oldFieldMap = new Dictionary<IField, FieldMetaInfo>(FieldEqualityComparer.CompareDeclaringTypes);
        private readonly Dictionary<IField, FieldMetaInfo> _newFieldMap = new Dictionary<IField, FieldMetaInfo>(FieldEqualityComparer.CompareDeclaringTypes);

        private FieldMetaInfo GetOrAddFieldMetaInfo(IField field, bool old)
        {
            Dictionary<IField, FieldMetaInfo> fieldMap = old ? _oldFieldMap : _newFieldMap;
            if (fieldMap.TryGetValue(field, out FieldMetaInfo fieldMetaInfo))
            {
                return fieldMetaInfo;
            }
            FieldDef fieldDef = field.ResolveFieldDefThrow();
            FieldFamily family = GetFieldFamily(fieldDef);
            ITypeDefOrRef klass;
            TypeSig fieldType = fieldDef.FieldType;
            if (field.IsMemberRef)
            {
                MemberRef memberRef = (MemberRef)field;
                IMemberRefParent parent = memberRef.Class;
                if (parent is TypeDef typeDef)
                {
                    klass = typeDef;
                }
                else if (parent is TypeRef typeRef)
                {
                    klass = typeRef.ResolveTypeDefThrow();
                }
                else if (parent is TypeSpec typeSpec)
                {
                    klass = typeSpec;
                    if (fieldType.ContainsGenericParameter)
                    {
                        GenericInstSig gis = typeSpec.TryGetGenericInstSig();
                        Assert.IsNotNull(gis);
                        fieldType = MetaUtil.Inflate(fieldType, new GenericArgumentContext(gis.GenericArguments.ToList(), null));
                    }
                }
                else
                {
                    throw new Exception($"GetOrAddFieldMetaInfo field:{field}");
                }
            }
            else
            {
                Assert.IsTrue(field.IsFieldDef);
                klass = fieldDef.DeclaringType;
            }
            fieldMetaInfo = new FieldMetaInfo()
            {
                field = field,
                fieldDef = fieldDef,
                klass = klass,
                fieldType = fieldType,
                family = family,
                orderOrOffset = !fieldDef.IsStatic && fieldDef.DeclaringType.IsExplicitLayout ? (int)fieldDef.FieldOffset : _typeCompareCache.GetFieldIndex(fieldDef, family, old),
            };
            Assert.AreNotEqual(fieldMetaInfo.orderOrOffset, -1);

            fieldMap.Add(field, fieldMetaInfo);
            return fieldMetaInfo;
        }

        private FieldFamily GetFieldFamily(FieldDef field)
        {
            if (!field.IsStatic)
            {
                return FieldFamily.Instance;
            }
            return MetaUtil.IsThreadStaticField(field) ? FieldFamily.ThreadStatic : FieldFamily.Static;
        }

        public bool CompareField(IField f1, IField f2)
        {
            var key = new FieldPair(f1, f2);
            if (!_compareResultCache.TryGetValue(key, out var result))
            {
                result = CompareField0(f1, f2);
                _compareResultCache.Add(key, result);
            }
            return result;
        }

        private static bool IsSameFieldOffset(FieldMetaInfo oldField, FieldMetaInfo newField, int lastLayoutEqualFieldIndex)
        {
            if (oldField.orderOrOffset != newField.orderOrOffset)
            {
                return false;
            }
            return newField.fieldDef.FieldOffset != null || lastLayoutEqualFieldIndex >= oldField.orderOrOffset;
        }

        public bool IsTypeCompatibleOfSameLayoutType(TypeSig oldType, TypeSig newType)
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
                case ElementType.Boolean:
                case ElementType.I1:
                case ElementType.U1:
                case ElementType.I2:
                case ElementType.R4:
                case ElementType.R8:
                case ElementType.R:
                case ElementType.String:
                case ElementType.Ptr:
                case ElementType.Array:
                case ElementType.FnPtr:
                case ElementType.Object:
                case ElementType.SZArray:
                case ElementType.Class:
                case ElementType.ByRef:
                case ElementType.Var:
                case ElementType.MVar:
                case ElementType.TypedByRef:
                    return oldElementType == newElementType;
                case ElementType.Char:
                case ElementType.U2:
                    return newElementType == ElementType.Char || newElementType == ElementType.U2;
                case ElementType.I4:
                case ElementType.U4:
                    return newElementType == ElementType.I4 || newElementType == ElementType.U4;
                case ElementType.I8:
                case ElementType.U8:
                    return newElementType == ElementType.I8 || newElementType == ElementType.U8;
                case ElementType.I:
                case ElementType.U:
                    return newElementType == ElementType.I || newElementType == ElementType.U;
                case ElementType.ValueType:
                {
                    TypeDef oldTypeDef = oldType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    TypeDef newTypeDef = newType.ToTypeDefOrRef().ResolveTypeDefThrow();
                    if (oldTypeDef.IsEnum)
                    {
                        if (newTypeDef.IsEnum)
                        {
                            return IsTypeCompatibleOfSameLayoutType(oldTypeDef.GetEnumUnderlyingType(), newTypeDef.GetEnumUnderlyingType());
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
                    return true;
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
                            return IsTypeCompatibleOfSameLayoutType(oldTypeDef.GetEnumUnderlyingType(), newTypeDef.GetEnumUnderlyingType());
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
                    return true;
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

        private bool CompareField0(IField oldField, IField newField)
        {
            Assert.IsTrue(oldField.IsField);
            Assert.IsTrue(newField.IsField);
            //if (!FieldEqualityComparer.CompareDeclaringTypes.Equals(oldField, newField))
            //{
            //    return false;
            //}
            if (!TypeEqualityComparer.Instance.Equals(oldField.DeclaringType, newField.DeclaringType))
            {
                return false;
            }
            if (oldField.DeclaringType.DefinitionAssembly?.IsCorLib() == true)
            {
                return true;
            }

            var oldFmi = GetOrAddFieldMetaInfo(oldField, true);
            var newFmi = GetOrAddFieldMetaInfo(newField, false);
            if (oldFmi.family != newFmi.family)
            {
                return false;
            }


            switch (oldFmi.family)
            {
                case FieldFamily.Instance:
                {
                    var typeCmpData = _typeCompareCache.GetInstanceCompareData(oldFmi.klass, newFmi.klass);
                    return IsSameFieldOffset(oldFmi, newFmi, typeCmpData.lastLayoutEqualFieldIndex) && IsTypeCompatibleOfSameLayoutType(oldFmi.fieldType, newFmi.fieldType);
                }
                case FieldFamily.Static:
                {
                    if (!CompareStatic(oldFmi.fieldDef, newFmi.fieldDef))
                    {
                        return false;
                    }
                    if (oldFmi.fieldDef.IsLiteral)
                    {
                        return true;
                    }
                    var typeCmpData = _typeCompareCache.GetStaticCompareData(oldFmi.klass, newFmi.klass);
                    return IsSameFieldOffset(oldFmi, newFmi, typeCmpData.lastLayoutEqualFieldIndex) && IsTypeCompatibleOfSameLayoutType(oldFmi.fieldType, newFmi.fieldType);
                }
                case FieldFamily.ThreadStatic:
                {
                    var typeCmpData = _typeCompareCache.GetThreadStaticCompareData(oldFmi.klass, newFmi.klass);
                    return IsSameFieldOffset(oldFmi, newFmi, typeCmpData.lastLayoutEqualFieldIndex) && IsTypeCompatibleOfSameLayoutType(oldFmi.fieldType, newFmi.fieldType);
                }
                default: throw new NotImplementedException();
            }
        }

        private bool CompareStatic(FieldDef oldField, FieldDef newField)
        {
            if (oldField.IsLiteral)
            {
                return newField.IsLiteral && oldField.Constant.Value.Equals(newField.Constant.Value);
            }
            else
            {
                if (newField.IsLiteral)
                {
                    return false;
                }
            }
            if (oldField.HasFieldRVA)
            {
                return newField.HasFieldRVA && oldField.Name == newField.Name;
            }
            else
            {
                if (newField.HasFieldRVA)
                {
                    return false;
                }
            }

            TypeDef td1 = oldField.DeclaringType;
            TypeDef td2 = newField.DeclaringType;
            MethodDef staticCtor1 = td1.FindStaticConstructor();
            MethodDef staticCtor2 = td2.FindStaticConstructor();
            if (staticCtor1 == null && staticCtor2 != null)
            {
                return false;
            }
            return true;
        }
    }
}
