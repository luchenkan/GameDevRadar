using dnlib.DotNet;
using HybridCLR.Editor.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HybridCLR.Editor.DHE
{
    public class VirtualTableCaculator
    {

        // 注意，这个使用默认Comparer
        private readonly Dictionary<MethodDef, int> _dheMethodVirtualTableSlots = new Dictionary<MethodDef, int>(MethodEqualityComparer.CompareDeclaringTypes);

        private class VirtualMethodImpl : ICloneable
        {
            public readonly ITypeDefOrRef type;
            public readonly MethodDef declaringMethod;
            public MethodDef implMethod;
            public readonly int slot;

            public VirtualMethodImpl(ITypeDefOrRef type, MethodDef declaringMethod, MethodDef implMethod, int index)
            {
                this.type = type;
                this.declaringMethod = declaringMethod;
                this.implMethod = implMethod;
                this.slot = index;
            }

            public object Clone()
            {
                return new VirtualMethodImpl(type, declaringMethod, implMethod, slot);
            }
        }

        private struct InterfaceOffset
        {
            public TypeVirtualTableInfo interfaceType;
            public int offset;

            public InterfaceOffset(TypeVirtualTableInfo interfaceType, int offset) : this()
            {
                this.interfaceType = interfaceType;
                this.offset = offset;
            }
        }

        private class TypeVirtualTableInfo
        {
            public ITypeDefOrRef type;

            public TypeVirtualTableInfo baseTypeVtableInfo;

            public int totalSlotCount;

            //public List<(MethodDef, int)> virtualMethods = new List<(MethodDef, int)>();

            public List<VirtualMethodImpl> vtable = new List<VirtualMethodImpl>();

            public List<InterfaceOffset> interfaceOffsets = new List<InterfaceOffset>();

            public List<TypeVirtualTableInfo> interfaces = new List<TypeVirtualTableInfo>();
        }

        private readonly Dictionary<ITypeDefOrRef, TypeVirtualTableInfo> _dheTypes = new Dictionary<ITypeDefOrRef, TypeVirtualTableInfo>(TypeEqualityComparer.Instance);

        public VirtualTableCaculator()
        {
        }

        private bool IsVirtualMethodOfHierarchyTreeSignatureMatch(MethodDef method, MethodDef oldMethod)
        {
            if (method.GenericParameters.Count != oldMethod.GenericParameters.Count)
            {
                return false;
            }
            if (method.GetParamCount() != oldMethod.GetParamCount())
            {
                return false;
            }
            if (!IsMethodSignatureParamTypeEqual(method.ReturnType, oldMethod.ReturnType))
            {
                return false;
            }
            for (int i = 1 /* skip this */, n = method.Parameters.Count; i < n; i++)
            {
                if (!IsMethodSignatureParamTypeEqual(method.Parameters[i].Type, oldMethod.Parameters[i].Type))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsVirtualGenericMethodOfSameHierarchyTreeSignatureMatch(MethodDef method, MethodDef oldMethod, GenericArgumentContext gac)
        {
            if (method.GenericParameters.Count != oldMethod.GenericParameters.Count)
            {
                return false;
            }
            if (method.GetParamCount() != oldMethod.GetParamCount())
            {
                return false;
            }
            if (!IsMethodSignatureParamTypeEqual(method.ReturnType, MetaUtil.Inflate(oldMethod.ReturnType, gac)))
            {
                return false;
            }
            for (int i = 1 /* skip this */, n = method.Parameters.Count; i < n; i++)
            {
                if (!IsMethodSignatureParamTypeEqual(method.Parameters[i].Type, MetaUtil.Inflate(oldMethod.Parameters[i].Type, gac)))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsMethodSignatureParamTypeEqual(TypeSig t1, TypeSig t2)
        {
            //if (!TypeEqualityComparer.Instance.Equals(t1, t2))
            //{
            //    return false;
            //}
            t1 = t1.RemovePinnedAndModifiers();
            t2 = t2.RemovePinnedAndModifiers();
            return TypeEqualityComparer.Instance.Equals(t1, t2);
        }

        public bool IsMethodSignatureMatch(MethodDef method, MethodDef oldMethod)
        {
            if (method.IsStatic != oldMethod.IsStatic)
            {
                return false;
            }
            if (method.GenericParameters.Count != oldMethod.GenericParameters.Count)
            {
                return false;
            }

            //if (!method.IsStatic && !IsMethodSignatureParamTypeEqual(method.DeclaringType.ToTypeSig(), oldMethod.DeclaringType.ToTypeSig()))
            //{
            //    return false;
            //}
            if (method.GetParamCount() != oldMethod.GetParamCount())
            {
                return false;
            }
            if (!IsMethodSignatureParamTypeEqual(method.ReturnType, oldMethod.ReturnType))
            {
                return false;
            }
            for (int i = 0, n = method.Parameters.Count; i < n; i++)
            {
                if (!IsMethodSignatureParamTypeEqual(method.Parameters[i].Type, oldMethod.Parameters[i].Type))
                {
                    return false;
                }
            }
            return true;
        }

        private int FindOverrideMethodSlot(MethodDef method, ITypeDefOrRef parentType)
        {
            if (parentType == null)
            {
                return -1;
            }
            if (!parentType.IsTypeSpec)
            {
                return FindOverrideMethodSlot(method, parentType.ResolveTypeDefThrow());
            }
            else
            {
                TypeSpec ts = (TypeSpec)parentType;
                GenericClass gc = GenericClass.ResolveClass(ts, null);
                return FindOverrideMethodSlot(method, gc);
            }
        }

        private int FindOverrideMethodSlot(MethodDef method, GenericClass parentType)
        {
            var gac = new GenericArgumentContext(parentType.KlassInst, null);
            TypeDef genericProtoType = parentType.Type;
            foreach (var m in genericProtoType.Methods)
            {
                if (m.IsPrivate || !m.IsNewSlot || m.Name != method.Name)
                {
                    continue;
                }
                if (IsVirtualGenericMethodOfSameHierarchyTreeSignatureMatch(method, m, gac))
                {
                    return GetVirtualTableIndex(m);
                }
            }

            ITypeDefOrRef baseType = genericProtoType.BaseType;
            if (baseType != null)
            {
                if (baseType is TypeSpec ts)
                {
                    return FindOverrideMethodSlot(method, GenericClass.ResolveClass(ts, gac));
                }
                else
                {
                    return FindOverrideMethodSlot(method, baseType.ResolveTypeDefThrow());
                }
            }
            else
            {
                return -1;
            }
            
            
        }

        private int FindOverrideMethodSlot(MethodDef method, TypeDef parentTypeDef)
        {
            foreach (var m in parentTypeDef.Methods)
            {
                if (m.IsPrivate || !m.IsNewSlot || m.Name != method.Name)
                {
                    continue;
                }
                if (IsVirtualMethodOfHierarchyTreeSignatureMatch(method, m))
                {
                    return GetVirtualTableIndex(m);
                }
            }
            if (parentTypeDef.BaseType != null)
            {
                return FindOverrideMethodSlot(method, parentTypeDef.BaseType);
            }
            else
            {
                return -1;
            }
        }

        private TypeVirtualTableInfo ComputeClassVirtualTable(ITypeDefOrRef type)
        {
            if (_dheTypes.TryGetValue(type, out var tvti))
            {
                return tvti;
            }
            if (type is TypeSpec ts)
            {
                tvti = ComputeClassVirtualTable1(ts);
            }
            else
            {
                TypeDef td = type.ResolveTypeDefThrow();
                tvti = ComputeClassVirtualTable0(td);
            }
            _dheTypes.Add(type, tvti);
            return tvti;
        }

        private TypeVirtualTableInfo ComputeClassVirtualTable0(TypeDef type)
        {
            var tvti = new TypeVirtualTableInfo()
            {
                type = type,
                interfaceOffsets = new List<InterfaceOffset>(),
                vtable = new List<VirtualMethodImpl>(),
            };

            ref int slot = ref tvti.totalSlotCount;
            if (type.IsInterface)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.IsVirtual)
                    {
                        continue;
                    }
                    _dheMethodVirtualTableSlots.Add(method, slot);
                    tvti.vtable.Add(new VirtualMethodImpl(type, method, method.HasBody ? method : null, slot));
                    //Debug.Log($"[ComputeClassVirtualTable][interface] method:{method} slot:{slot}");
                    //tvti.virtualMethods.Add((method, slot));
                    ++slot;
                }
            }
            else
            {
                if (type.BaseType != null)
                {
                    TypeVirtualTableInfo baseTvti = tvti.baseTypeVtableInfo = ComputeClassVirtualTable(type.BaseType);
                    slot = baseTvti.totalSlotCount;
                    //tvti.virtualMethods = new List<(MethodDef, int)>(tvti.baseTypeVtableInfo.virtualMethods);
                    tvti.interfaceOffsets.AddRange(baseTvti.interfaceOffsets);
                    tvti.vtable.AddRange(baseTvti.vtable.Select(m => (VirtualMethodImpl)m.Clone()));
                }

                var implInterfaceOffsets = new List<int>();
                foreach (InterfaceImpl interfaceImpl in type.Interfaces)
                {
                    ITypeDefOrRef interfaceType = interfaceImpl.Interface;
                    int interfaceIndex = tvti.interfaceOffsets.FindIndex(f => TypeEqualityComparer.Instance.Equals(f.interfaceType.type, interfaceType));
                    if (interfaceIndex >= 0)
                    {
                        implInterfaceOffsets.Add(interfaceIndex);
                    }
                    else
                    {
                        implInterfaceOffsets.Add(tvti.interfaceOffsets.Count);
                        var interVti = ComputeClassVirtualTable(interfaceType);
                        tvti.interfaceOffsets.Add(new InterfaceOffset(interVti, slot));
                        foreach (var method in interVti.vtable)
                        {
                            tvti.vtable.Add(new VirtualMethodImpl(interfaceType, method.declaringMethod, method.implMethod, slot++));
                        }
                    }
                }

                var explicitOverrideSlots = new Dictionary<MethodDef, VirtualMethodImpl>(MethodEqualityComparer.DontCompareDeclaringTypes);

                // explicit method overrides
                foreach (var method in type.Methods)
                {
                    foreach (var over in method.Overrides)
                    {
                        ITypeDefOrRef overDeclaringType = over.MethodDeclaration.DeclaringType;
                        MethodDef overMethodDef = over.MethodDeclaration.ResolveMethodDef();
                        bool find = false;
                        foreach (var implMethod in tvti.vtable)
                        {
                            ITypeDefOrRef methodDeclaringType = implMethod.type;
                            if (!TypeEqualityComparer.Instance.Equals(methodDeclaringType, overDeclaringType))
                            {
                                continue;
                            }
                            if (MethodEqualityComparer.DontCompareDeclaringTypes.Equals(overMethodDef, implMethod.declaringMethod))
                            {
                                implMethod.implMethod = method;
                                if (method.IsVirtual)
                                {
                                    if (!explicitOverrideSlots.ContainsKey(method))
                                    {
                                        explicitOverrideSlots.Add(method, implMethod);
                                    }
                                }
                                find = true;
                                break;
                            }
                        }
                        if (!find)
                        {
                            throw new Exception($"explicit method:{method} can't find override method:{over.MethodDeclaration}");
                        }
                    }
                }



                foreach (var method in type.Methods)
                {
                    if (!method.IsVirtual)
                    {
                        continue;
                    }
                    if (method.IsNewSlot)
                    {
                        if (method.IsPublic)
                        {
                            // override explicit interface method
                            foreach (int interfaceIdx in implInterfaceOffsets)
                            {
                                InterfaceOffset io = tvti.interfaceOffsets[interfaceIdx];
                                int overrideSlot = FindOverrideMethodSlot(method, io.interfaceType.type);
                                if (overrideSlot >= 0)
                                {
                                    VirtualMethodImpl vmi = tvti.vtable[io.offset + overrideSlot];
                                    if (vmi.implMethod == null || !TypeEqualityComparer.Instance.Equals(vmi.implMethod.DeclaringType, type))
                                    {
                                        vmi.implMethod = method;
                                        if (!explicitOverrideSlots.ContainsKey(method))
                                        {
                                            explicitOverrideSlots.Add(method, vmi);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int overrideSlot = FindOverrideMethodSlot(method, type.BaseType);
                        if (overrideSlot < 0)
                        {
                            Debug.LogError($"override method:{method} can't find override method in {type}. Only tool-generated code might cause this error");
                        }
                        _dheMethodVirtualTableSlots.Add(method, overrideSlot);
                    }
                }
                foreach (var method in type.Methods)
                {
                    if (!method.IsVirtual)
                    {
                        continue;
                    }
                    if (_dheMethodVirtualTableSlots.ContainsKey(method))
                    {
                        continue;
                    }
                    if (method.IsFinal && explicitOverrideSlots.TryGetValue(method, out var vmi))
                    {
                        _dheMethodVirtualTableSlots.Add(method, vmi.slot);
                    }
                    else
                    {
                        _dheMethodVirtualTableSlots.Add(method, slot);
                        tvti.vtable.Add(new VirtualMethodImpl(type, method, method, slot));
                        ++slot;
                    }
                }
            }
            return tvti;
        }

        private TypeVirtualTableInfo ComputeClassVirtualTable1(TypeSpec type)
        {
            GenericInstSig gis = type.TryGetGenericInstSig();
            if (gis == null)
            {
                throw new Exception($"ComputeClassVirtualTable1 fail type:{type}");
            }
            TypeDef genericType = gis.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
            TypeVirtualTableInfo genericTvti = ComputeClassVirtualTable(genericType);
            var gac = new GenericArgumentContext(gis.GenericArguments.ToList(), null);
            var tvti = new TypeVirtualTableInfo();
            tvti.type = type;
            tvti.totalSlotCount = genericTvti.totalSlotCount;
            tvti.interfaceOffsets = genericTvti.interfaceOffsets;
            tvti.baseTypeVtableInfo = genericType.BaseType != null ? ComputeClassVirtualTable(MetaUtil.Inflate(genericType.BaseType, gac)) : null;
            tvti.interfaces = genericTvti.interfaces.Select(t => ComputeClassVirtualTable(MetaUtil.Inflate(t.type, gac))).ToList();
            //ti.virtualMethods = new List<(MethodDef, int)>(genericTvti.virtualMethods.Count);
            tvti.vtable = new List<VirtualMethodImpl>(genericTvti.vtable.Count);
            foreach (var method in genericTvti.vtable)
            {
                tvti.vtable.Add(new VirtualMethodImpl(MetaUtil.Inflate(method.type, gac), method.declaringMethod, method.implMethod, method.slot));
            }
            return tvti;
        }

        public int GetVirtualTableIndex(MethodDef method)
        {
            if (!_dheMethodVirtualTableSlots.TryGetValue(method, out int index))
            {
                ComputeClassVirtualTable(method.DeclaringType);
                return _dheMethodVirtualTableSlots.TryGetValue(method, out index) ? index : -1;
            }
            return index;
        }
    }
}
