using dnlib.DotNet;
using dnlib.DotNet.Emit;
using HybridCLR.Editor.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HybridCLR.Editor.DHE
{
    public enum MethodCompareState
    {
        NotCompared,
        Equal,
        NotEqual,
        Comparing,
    }

    public class MethodCompareCache
    {
        private class MethodBodyCompareData
        {
            public MethodDef oldMethod;
            public MethodDef newMethod;

            public MethodCompareState state;
            private List<MethodBodyCompareData> _dependentMeMethods = new List<MethodBodyCompareData>();
            private List<MethodBodyCompareData> _myDependentMethods = new List<MethodBodyCompareData>();

            public void AddMyDependentMethod(MethodBodyCompareData otherMethod)
            {
                if (_myDependentMethods.Contains(otherMethod))
                {
                    Assert.IsTrue(otherMethod._dependentMeMethods.Contains(this));
                    return;
                }
                _myDependentMethods.Add(otherMethod);
                Assert.IsFalse(otherMethod._dependentMeMethods.Contains(this));
                otherMethod._dependentMeMethods.Add(this);
            }

            public void OnStateConfirm(MethodCompareState finalState)
            {
                Assert.IsTrue(state == MethodCompareState.Comparing);
                Assert.IsTrue(finalState == MethodCompareState.NotEqual || finalState == MethodCompareState.Equal);
                state = finalState;
                _myDependentMethods.Clear();
                if (_dependentMeMethods.Count == 0)
                {
                    return;
                }

                // clean my dependent
                //foreach (var myDependentMethod in _myDependentMethods)
                //{
                //    bool succ = myDependentMethod._dependentMeMethods.Remove(this);
                //    Assert.IsTrue(succ);
                //}

                // fire all dependent me

                var dependentMeMethods = _dependentMeMethods;
                // clean to avoid recursive calls
                _dependentMeMethods = new List<MethodBodyCompareData>();
                foreach (var dependentMeMethod in dependentMeMethods)
                {
                    if (dependentMeMethod.state != MethodCompareState.Comparing)
                    {
                        Assert.IsTrue(dependentMeMethod._myDependentMethods.Count == 0);
                        continue;
                    }
                    Assert.IsTrue(dependentMeMethod._myDependentMethods.Contains(this));
                    if (state == MethodCompareState.NotEqual)
                    {
                        dependentMeMethod.OnStateConfirm(MethodCompareState.NotEqual);
                    }
                    else
                    {
                        bool removed = dependentMeMethod._myDependentMethods.Remove(this);
                        Debug.Assert(removed);
                        if (dependentMeMethod._myDependentMethods.Count == 0)
                        {
                            dependentMeMethod.OnStateConfirm(MethodCompareState.Equal);
                        }
                    }
                }
            }

            public bool HasDependents => _myDependentMethods != null && _myDependentMethods.Count > 0;
        }

        private readonly TypeCompareCache _typeCompareCache;

        private readonly FieldCompareCache _fieldCompareCache;

        private readonly Dictionary<MethodDef, MethodBodyCompareData> _methods = new Dictionary<MethodDef, MethodBodyCompareData>(MethodEqualityComparer.CompareDeclaringTypes);

        private HashSet<UTF8String> _dheAssemblyNames = new HashSet<UTF8String>(UTF8StringEqualityComparer.Instance);

        public MethodCompareCache(TypeCompareCache typeCompareCache, FieldCompareCache fieldCompareCache)
        {
            _typeCompareCache = typeCompareCache;
            _fieldCompareCache = fieldCompareCache;
            _oldVtableCalc = new VirtualTableCaculator();
            _newVtableCalc = new VirtualTableCaculator();
            _newUnchangedMethods = new UnchangedMethodCollection();
            _newUnchangedMethods.InitUnchangedMethods(typeCompareCache.NewSnapshot);
            _oldInjectRules = CreateInjectRules(typeCompareCache.OldSnapshot);
            _newInjectRules = typeCompareCache.NewSnapshot.SnapshotMode == SnapshotMode.AOT ? CreateInjectRules(typeCompareCache.NewSnapshot) : _oldInjectRules;
            
            foreach (var ass in typeCompareCache.NewSnapshot.DheAssemblyNames)
            {
                _dheAssemblyNames.Add(new UTF8String(ass + ".dll"));
            }
        }

        private readonly VirtualTableCaculator _oldVtableCalc;
        private readonly VirtualTableCaculator _newVtableCalc;

        private readonly InjectRules _oldInjectRules;
        private readonly InjectRules _newInjectRules;

        private readonly UnchangedMethodCollection _newUnchangedMethods;


        private InjectRules CreateInjectRules(Snapshot snapshot)
        {
            var ir = new InjectRules();
            if (Directory.Exists(snapshot.InjectRuleDir))
            {
                ir.LoadFromXmlFiles(Directory.GetFiles(snapshot.InjectRuleDir, "*.xml", SearchOption.AllDirectories));
            }
            return ir;
        }

        public class MethodCallCompareData
        {
            public IMethod oldMethod;
            public IMethod newMethod;
            public bool callEqual;
        }

        public class MethodCallVirCompareData
        {
            public IMethod oldMethod;
            public IMethod newMethod;
            public bool callvirEqual;
        }

        public class MethodCompareData
        {
            public IMethod oldMethod;
            public IMethod newMethod;
            public bool fullEqual;
            public bool signatureEqual;
            public bool bodyEqual;
        }


        public MethodCompareData GetCompareData(MethodDef oldMethod, MethodDef newMethod)
        {
            MethodBodyCompareData data = _methods[newMethod];
            bool bodyEqual = data.state != MethodCompareState.NotEqual;
            bool signatureEqual = IsMethodSignatureReferenceEqual(oldMethod, newMethod);
            return new MethodCompareData
            {
                oldMethod = oldMethod,
                newMethod = newMethod,
                bodyEqual = bodyEqual,
                signatureEqual = signatureEqual,
                fullEqual = bodyEqual && signatureEqual,
            };
        }

        public void CompareMethodFully(MethodDef oldMethod, MethodDef newMethod)
        {
            if (!_methods.TryGetValue(newMethod, out var cmpData))
            {
                cmpData = new MethodBodyCompareData
                {
                    oldMethod = oldMethod,
                    newMethod = newMethod,
                    state = MethodCompareState.NotCompared,
                };
                _methods.Add(newMethod, cmpData);
            }
            CompareMethodFully0(oldMethod, newMethod, cmpData);
        }

        private void CompareMethodFully0(MethodDef oldMethod, MethodDef newMethod, MethodBodyCompareData cmpData)
        {
            Assert.AreEqual(cmpData.state, MethodCompareState.NotCompared);
            cmpData.state = MethodCompareState.Comparing;

            bool bodyEqual = CompareMethodBody(oldMethod, newMethod, cmpData);
            if (bodyEqual)
            {
                if (!cmpData.HasDependents)
                {
                    cmpData.OnStateConfirm(MethodCompareState.Equal);
                }
            }
            else
            {
                cmpData.OnStateConfirm(MethodCompareState.NotEqual);
            }
        }


        private bool CompareMethodBody(MethodDef oldMethod, MethodDef newMethod, MethodBodyCompareData data)
        {
            if (_newUnchangedMethods.IsUnchangedMethod(newMethod))
            {
                return true;
            }
            if (!IsMethodSignatureReferenceEqual(oldMethod, newMethod))
                return false;
            if (oldMethod.IsStatic != newMethod.IsStatic)
            {
                return false;
            }

            if (oldMethod.HasBody != newMethod.HasBody)
            {
                return false;
            }
            if (!oldMethod.HasBody)
            {
                return true;
            }
            CilBody b1 = oldMethod.Body;
            CilBody b2 = newMethod.Body;
            if (b1.Variables.Count != b2.Variables.Count)
            {
                return false;
            }
            for (int i = 0, n = b1.Variables.Count; i < n; i++)
            {
                var v1 = b1.Variables[i];
                var v2 = b2.Variables[i];
                if (!_typeCompareCache.CompareAnyReferenceEqual(v1.Type, v2.Type))
                {
                    //Debug.Log($"variable not eqal:{v1.Type} {v2.Type}");
                    return false;
                }
            }
            if (b1.ExceptionHandlers.Count != b2.ExceptionHandlers.Count)
            {
                //Debug.Log($"ExceptionHandlers.Count not equal. {b1.ExceptionHandlers.Count} {b2.ExceptionHandlers.Count}");
                return false;
            }
            for (int i = 0, n = b1.ExceptionHandlers.Count; i < n; i++)
            {
                ExceptionHandler e1 = b1.ExceptionHandlers[i];
                ExceptionHandler e2 = b2.ExceptionHandlers[i];
                if (!CompareExceptionHandler(e1, e2))
                {
                    //Debug.Log($"ExceptionHandler not equal. index:{i}");
                    return false;
                }
            }
            if (b1.Instructions.Count != b2.Instructions.Count)
            {
                //Debug.Log($"Instructions.Count not equal. {b1.Instructions.Count} {b2.Instructions.Count}");
                return false;
            }
            for (int i = 0, n = b1.Instructions.Count; i < n; i++)
            {
                var c1 = b1.Instructions[i];
                var c2 = b2.Instructions[i];
                if (!CompareInstruction(c1, c2, data))
                {
                    //Debug.Log($"Instruction not equal. [{i}] {c1} {c2}");
                    return false;
                }
            }

            return true;
        }

        private bool CompareExceptionHandler(ExceptionHandler e1, ExceptionHandler e2)
        {
            if (e1.HandlerType != e2.HandlerType)
            {
                return false;
            }
            if (e1.TryStart.Offset != e2.TryStart.Offset)
            {
                return false;
            }
            if (e1.TryEnd.Offset != e2.TryEnd.Offset)
            {
                return false;
            }
            if (e1.HandlerStart.Offset != e2.HandlerStart.Offset)
            {
                return false;
            }
            if (e1.HandlerEnd?.Offset != e2.HandlerEnd?.Offset)
            {
                return false;
            }
            if (e1.HandlerType == ExceptionHandlerType.Filter)
            {
                if (e1.FilterStart.Offset != e2.FilterStart.Offset)
                {
                    return false;
                }
            }
            else if (e1.HandlerType == ExceptionHandlerType.Catch)
            {
                if (!TypeEqualityComparer.Instance.Equals(e1.CatchType, e2.CatchType))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CompareInstruction(Instruction oldInst, Instruction newInst, MethodBodyCompareData method)
        {
            OpCode oldOpCode = oldInst.OpCode;
            OpCode newOpCode = newInst.OpCode;
            Code oldCode = oldOpCode.Code;
            if (oldCode != newOpCode.Code)
            {
                return false;
            }
            object oldOp = oldInst.Operand;
            object newOp = newInst.Operand;
            if (oldOp == null)
            {
                return newOp == null;
            }
            if (newOp == null)
            {
                return false;
            }
            // ???
            if (oldOp.GetType() != newOp.GetType())
            {
                return false;
            }
            switch (oldOpCode.OperandType)
            {
                case OperandType.InlineNone:
                    return true;
                case OperandType.InlineI:
                case OperandType.InlineI8:
                case OperandType.InlineR:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineR:
                case OperandType.InlineString:
                    return oldOp.Equals(newOp);
                case OperandType.InlineVar:
                case OperandType.ShortInlineVar:
                {
                    if (oldOp is Local local1)
                    {
                        return local1.Index == ((Local)newOp).Index;
                    }
                    else
                    {
                        return ((Parameter)oldOp).Index == ((Parameter)newOp).Index;
                    }
                }
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    return ((Instruction)oldOp).Offset == ((Instruction)newOp).Offset;
                case OperandType.InlineSwitch:
                {
                    IList<Instruction> case1 = (IList<Instruction>)oldOp;
                    IList<Instruction> case2 = (IList<Instruction>)newOp;
                    if (case1.Count != case2.Count)
                    {
                        return false;
                    }
                    for (int i = 0, n = case1.Count; i < n; i++)
                    {
                        if (case1[i].Offset != case2[i].Offset)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                case OperandType.InlineField:
                    return _fieldCompareCache.CompareField((IField)oldOp, (IField)newOp);
                case OperandType.InlineType:
                {
                    ITypeDefOrRef oldType = (ITypeDefOrRef)oldOp;
                    ITypeDefOrRef newType = (ITypeDefOrRef)newOp;
                    switch (oldCode)
                    {
                        case Code.Initobj:
                        case Code.Cpobj:
                        case Code.Ldobj:
                        case Code.Stobj:
                        {
                            return _typeCompareCache.CompareAnyReferenceEqual(oldType.ToTypeSig(), newType.ToTypeSig());
                        }
                        case Code.Isinst:
                        case Code.Castclass:
                        {
                            return TypeEqualityComparer.Instance.Equals(oldType, newType)
                                && _typeCompareCache.CompareAnyReferenceEqual(oldType.ToTypeSig(), newType.ToTypeSig())
                                && GetTypeIsInstOrCastClassOptimizationLevel(oldType) <= GetTypeIsInstOrCastClassOptimizationLevel(newType);
                        }
                        // TODO more accuracy optimization!!!
                        //case Code.Box:
                        //case Code.Unbox:
                        //case Code.Unbox_Any:
                        //{
                        //    break;
                        //}    
                        //case Code.Newarr:
                        //case Code.Ldelema:
                        //case Code.Ldelem:
                        //case Code.Stelem:
                        //{
                        //    break;
                        //}
                        //case Code.Refanyval:
                        //case Code.Mkrefany:
                        //{
                        //    break;
                        //}
                        //case Code.Sizeof:
                        //{
                        //    break;
                        //}
                        default:
                        {
                            return TypeEqualityComparer.Instance.Equals(oldType, newType)
                                && _typeCompareCache.CompareAnyReferenceEqual(oldType.ToTypeSig(), newType.ToTypeSig());
                        }
                    }
                }
                case OperandType.InlineMethod:
                {
                    IMethod oldMethod = (IMethod)oldOp;
                    IMethod newMethod = (IMethod)newOp;
                    if (!MethodEqualityComparer.CompareDeclaringTypes.Equals(oldMethod, newMethod))
                    {
                        return false;
                    }
                    // FIXME!!! some method (e.g. T[].get) can't get MethodDef
                    switch (oldCode)
                    {
                        case Code.Call:
                        {
                            return CompareCallNotVirtualMethod(oldMethod, newMethod, method, out _);
                        }
                        case Code.Newobj:
                        {
                            return CompareCallNewObj(oldMethod, newMethod, method);
                        }
                        case Code.Ldftn:
                        {
                            return true;
                        }
                        case Code.Ldvirtftn:
                        {
                            return CompareLoadVirtualMethod(oldMethod, newMethod, true, out var needDevirtualCompare);
                        }
                        case Code.Callvirt:
                        {
                            return CompareCallVirtualMethod(oldMethod, newMethod, method);
                        }
                        default: throw new NotSupportedException($"not support instruction:{newInst}");
                    }
                }
                case OperandType.InlineSig:
                    return CompareMethodSigParamReferenceEqual((MethodSig)oldOp, (MethodSig)newOp);
                case OperandType.InlineTok:
                    return CompareToken((ITokenOperand)oldOp, (ITokenOperand)newOp, method);
                //case OperandType.InlinePhi:
                //case OperandType.NOT_USED_8:
                default:
                throw new NotSupportedException($"not support operandType:{oldOpCode.OperandType}");
            }
        }

        private class MethodCompareCacheData
        {
            public MethodCompareState state;
            public MethodBodyCompareData data;
        }

        private readonly Dictionary<IMethod, MethodCompareCacheData> _callCache = new Dictionary<IMethod, MethodCompareCacheData>(MethodEqualityComparer.CompareDeclaringTypes);

        private bool CompareCallNotVirtualMethod(IMethod oldMethod, IMethod newMethod, MethodBodyCompareData caller, out MethodBodyCompareData rely)
        {
            //if (!MethodEqualityComparer.CompareDeclaringTypes.Equals(oldMethod, newMethod))
            //{
            //    return false;
            //}
            rely = null;
            if (_callCache.TryGetValue(newMethod, out var c))
            {
                switch (c.state)
                {
                    case MethodCompareState.Equal: return true;
                    case MethodCompareState.NotEqual: return false;
                    default:
                    {
                        switch (c.data.state)
                        {
                            case MethodCompareState.Equal: return true;
                            case MethodCompareState.NotEqual: return false;
                            default:
                            {
                                rely = c.data;
                                caller.AddMyDependentMethod(c.data); return true;
                            }
                        }
                    }
                }
            }
            if (!CompareCallNotVirtualMethod0(oldMethod, newMethod, caller, out rely))
            {
                _callCache.Add(newMethod, new MethodCompareCacheData { state = MethodCompareState.NotEqual, data = null });
                return false;
            }
            var state = rely == null ? MethodCompareState.Equal : MethodCompareState.Comparing;
            _callCache.Add(newMethod, new MethodCompareCacheData { state = state, data = rely });
            if (state == MethodCompareState.Comparing)
            {
                caller.AddMyDependentMethod(rely);
            }
            return true;
        }

        private bool CompareCallNewObj(IMethod oldMethod, IMethod newMethod, MethodBodyCompareData caller)
        {
            // TODO FIXME. should check Struct ReferenceEqual
            return CompareCallNotVirtualMethod(oldMethod, newMethod, caller, out _);
        }



        private MethodBodyCompareData GetOrInitMethodBodyCompareData(MethodDef oldMethod, MethodDef newMethod)
        {
            if (!_methods.TryGetValue(newMethod, out var data))
            {
                data = new MethodBodyCompareData()
                {
                    oldMethod = oldMethod,
                    newMethod = newMethod,
                    state = MethodCompareState.NotCompared,
                };
                _methods.Add(newMethod, data);
            }
            return data;
        }

        private bool IsDefinedInAotAssembly(MethodDef method)
        {
            return !_dheAssemblyNames.Contains(method.Module.Name);
        }

        private bool CompareCallNotVirtualMethod0(IMethod oldMethod, IMethod newMethod, MethodBodyCompareData callerMethod, out MethodBodyCompareData rely)
        {
            rely = null;
            if (!IsMethodSignatureReferenceEqual(oldMethod, newMethod))
                return false;
            MethodDef oldMethodDef = oldMethod.ResolveMethodDef();
            MethodDef newMethodDef = newMethod.ResolveMethodDef();
            if (oldMethodDef == null)
            {
                return newMethodDef == null;
            }
            if (newMethodDef == null)
            {
                return false;
            }

            if (IsDefinedInAotAssembly(newMethodDef))
            {
                return true;
            }

            bool oldIsNotInject = _oldInjectRules.IsNotInjectMethod(oldMethodDef);
            bool newIsNotInject = _newInjectRules.IsNotInjectMethod(newMethodDef);
            // when change from injection to not injection, we consider it as changed
            if (!oldIsNotInject && newIsNotInject)
            {
                return false;
            }

            var calleeMethod = GetOrInitMethodBodyCompareData(oldMethodDef, newMethodDef);
            // if method is injected, we call it directly
            if (!newIsNotInject)
            {
                return true;
            }
            switch (calleeMethod.state)
            {
                case MethodCompareState.Equal: return true;
                case MethodCompareState.NotEqual: return false;
                case MethodCompareState.NotCompared:
                case MethodCompareState.Comparing:
                {
                    rely = calleeMethod;
                    return true;
                }
                default: throw new Exception();
            }
        }



        private readonly Dictionary<IMethod, MethodCompareCacheData> _callVirCache = new Dictionary<IMethod, MethodCompareCacheData>(MethodEqualityComparer.CompareDeclaringTypes);

        private bool CompareCallVirtualMethod(IMethod oldMethod, IMethod newMethod, MethodBodyCompareData caller)
        {
            if (!IsMethodSignatureReferenceEqual(oldMethod, newMethod))
            {
                return false;
            }

            if (_callVirCache.TryGetValue(newMethod, out MethodCompareCacheData c))
            {
                switch (c.state)
                {
                    case MethodCompareState.Equal: return true;
                    case MethodCompareState.NotEqual: return false;
                    default:
                    {
                        switch (c.data.state)
                        {
                            case MethodCompareState.Equal: return true;
                            case MethodCompareState.NotEqual: return false;
                            default: caller.AddMyDependentMethod(c.data); return true;
                        }
                    }
                }
            }

            var cd = CompareCallVirtualMethod0(oldMethod, newMethod, caller);
            _callVirCache.Add(newMethod, cd);
            return cd.state != MethodCompareState.NotEqual;
        }

        private MethodCompareCacheData CompareCallVirtualMethod0(IMethod oldMethod, IMethod newMethod, MethodBodyCompareData caller)
        {
            var data = new MethodCompareCacheData();

            if (!CompareLoadVirtualMethod(oldMethod, newMethod, false, out var needDevirtualCompare))
            {
                data.state = MethodCompareState.NotEqual;
                return data;
            }
            if (!needDevirtualCompare)
            {
                data.state = MethodCompareState.Equal;
                return data;
            }

            if (!CompareCallNotVirtualMethod(oldMethod, newMethod, caller, out var rely))
            {
                data.state = MethodCompareState.NotEqual;
                return data;
            }
            if (rely == null)
            {
                data.state = MethodCompareState.Equal;
            }
            else
            {
                data.state = MethodCompareState.Comparing;
                data.data = rely;
            }
            return data;
        }

        private bool CompareLoadVirtualMethod(IMethod oldMethod, IMethod newMethod, bool disableDevirtual, out bool needDevirtualCompare)
        {
            needDevirtualCompare = false;
            if (!MethodEqualityComparer.CompareDeclaringTypes.Equals(oldMethod, newMethod))
            {
                return false;
            }

            MethodDef oldMethodDef = oldMethod.ResolveMethodDef();
            MethodDef newMethodDef = newMethod.ResolveMethodDef();
            if (oldMethodDef == null)
            {
                return newMethodDef == null;
            }
            if (newMethodDef == null)
            {
                return false;
            }
            return CompareLoadVirtualMethod0(oldMethodDef, newMethodDef, disableDevirtual, out needDevirtualCompare);
        }

        private bool CompareLoadVirtualMethod0(MethodDef oldMethodDef, MethodDef newMethodDef, bool disableDevirtual, out bool needDevirtualCompare)
        {
            //if (!_typeCompareCache.CompareFieldOrParamOrVariableType(m1.DeclaringType.ToTypeSig(), m2.DeclaringType.ToTypeSig()))
            //{
            //    return false;
            //}
            needDevirtualCompare = false;
            bool deVirtual1 = !oldMethodDef.IsVirtual || (!disableDevirtual && (oldMethodDef.IsFinal || oldMethodDef.IsPrivate || oldMethodDef.DeclaringType.IsSealed));
            bool deVirtual2 = !newMethodDef.IsVirtual || (!disableDevirtual && (newMethodDef.IsFinal || newMethodDef.IsPrivate || newMethodDef.DeclaringType.IsSealed));
            if (deVirtual1 && deVirtual2)
            {
                needDevirtualCompare = true;
                return true;
            }
            if (deVirtual1 ^ deVirtual2)
            {
                return false;
            }
            //if (!IsMethodSignatureMatch2(m1, m2))
            //{
            //    return false;
            //}
            if (oldMethodDef.DeclaringType.IsInterface ^ newMethodDef.DeclaringType.IsInterface)
            {
                return false;
            }

            //bool deVirutal1 = md1.IsFinal || md1.DeclaringType.IsSealed;
            //bool deVirtual2 = md2.IsFinal || md2.DeclaringType.IsSealed;
            //if (deVirutal1 != deVirtual2)
            //{
            //    return false;
            //}
            //if (!_proxyAOTMethod && deVirutal1)
            //{
            //    return CompareMethodDefInternal(md1, md2, callerData, out var _);
            //}

            int index1 = _oldVtableCalc.GetVirtualTableIndex(oldMethodDef);
            int index2 = _newVtableCalc.GetVirtualTableIndex(newMethodDef);
            if (index1 != index2)
            {
                return false;
            }
            if (index1 >= 0)
            {
                return true;
            }
            // some tools may generate virtual method which has not newslot and doesn't ovveride any method.
            // these methods virutal index is -1, so we need to compare them as novirtual method.
            needDevirtualCompare = true;
            return true;
        }



        private enum IsInstOrCastClassOptimizationLevel
        {
            Normal,
            Class,
            Sealed,
        }

        private IsInstOrCastClassOptimizationLevel GetTypeIsInstOrCastClassOptimizationLevel(ITypeDefOrRef type)
        {
            if (type.IsTypeDef || type.IsTypeRef)
            {
                TypeDef td = type.ResolveTypeDefThrow();
                if (td.IsInterface)
                {
                    return IsInstOrCastClassOptimizationLevel.Normal;
                }
                if (td.IsSealed)
                {
                    return IsInstOrCastClassOptimizationLevel.Sealed;
                }
                return IsInstOrCastClassOptimizationLevel.Normal;
            }
            if (type.IsTypeSpec)
            {
                TypeSig sig = type.ToTypeSig().RemovePinnedAndModifiers();
                switch (sig.ElementType)
                {
                    case ElementType.GenericInst: return GetTypeIsInstOrCastClassOptimizationLevel(sig.ToGenericInstSig().GenericType.ToTypeDefOrRef());
                    case ElementType.Var:
                    case ElementType.MVar: return IsInstOrCastClassOptimizationLevel.Normal;
                    case ElementType.Array:
                    case ElementType.SZArray: return IsInstOrCastClassOptimizationLevel.Sealed;
                    default: return IsInstOrCastClassOptimizationLevel.Normal;
                }
            }

            return IsInstOrCastClassOptimizationLevel.Normal;
        }


        private Dictionary<IMethod, bool> _methodSignatureCompareCache = new Dictionary<IMethod, bool>(MethodEqualityComparer.CompareDeclaringTypes);
        
        private bool IsMethodSignatureReferenceEqual(IMethod oldMethod, IMethod newMethod)
        {
            if (!MethodEqualityComparer.CompareDeclaringTypes.Equals(oldMethod, newMethod))
            {
                return false;
            }
            if (!_methodSignatureCompareCache.TryGetValue(newMethod, out var equal))
            {
                equal = IsMethodSignatureReferenceEqual0(oldMethod, newMethod);
                _methodSignatureCompareCache.Add(newMethod, equal);
            }
            return equal;
        }

        private bool IsMethodSignatureReferenceEqual0(IMethod oldMethod, IMethod newMethod)
        {
            if (oldMethod.IsMethodDef)
            {
                return IsMethodDefSignatureReferenceEqual(oldMethod.ResolveMethodDefThrow(), newMethod.ResolveMethodDefThrow());
            }
            else if (oldMethod.IsMemberRef)
            {
                return IsMemberRefSignatureReferenceEqual((MemberRef)oldMethod, (MemberRef)newMethod);
            }
            else if (oldMethod.IsMethodSpec)
            {
                return IsMethodSpecSignatureReferenceEqual((MethodSpec)oldMethod, (MethodSpec)newMethod);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private bool CompareMethodDefCommon(MethodDef oldMethodDef, MethodDef newMethodDef)
        {
            if (oldMethodDef.IsStatic ^ newMethodDef.IsStatic)
            {
                return false;
            }

            if (!IsGenericParamConstraintCompatible(oldMethodDef, newMethodDef))
            {
                return false;
            }
            return true;
        }

        private bool CompareNotGenericMethod(IMethod oldMethod, IMethod newMethod)
        {
            MethodSig oldMethodSig = oldMethod.MethodSig;
            MethodSig newMethodSig = newMethod.MethodSig;
            if (!_typeCompareCache.CompareAnyReferenceEqual(oldMethodSig.RetType, newMethodSig.RetType))
            {
                return false;
            }
            for (int i = 0, n = oldMethodSig.Params.Count; i < n; i++)
            {
                if (!_typeCompareCache.CompareAnyReferenceEqual(oldMethodSig.Params[i], newMethodSig.Params[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CompareTypeSigList(IReadOnlyList<TypeSig> oldTypeSigs, IReadOnlyList<TypeSig> newTypeSigs)
        {
            if (oldTypeSigs == null)
            {
                return newTypeSigs == null;
            }
            else if (newTypeSigs == null)
            {
                return false;
            }
            if (oldTypeSigs.Count != newTypeSigs.Count)
            {
                return false;
            }
            for (int i = 0, n = oldTypeSigs.Count; i < n; i++)
            {
                if (!_typeCompareCache.CompareAnyReferenceEqual(oldTypeSigs[i], newTypeSigs[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CompareGenericArgumentContext(GenericArgumentContext oldGac, GenericArgumentContext newGac)
        {
            return CompareTypeSigList(oldGac.TypeArgsStack, newGac.TypeArgsStack)
                && CompareTypeSigList(oldGac.MethodArgsStack, newGac.MethodArgsStack);
        }

        private bool CompareGenericMethod(IMethod oldMethod, IMethod newMethod, GenericArgumentContext oldGac, GenericArgumentContext newGac)
        {
            if (!CompareGenericArgumentContext(oldGac, newGac))
            {
                return false;
            }
            MethodSig oldMethodSig = oldMethod.MethodSig;
            MethodSig newMethodSig = newMethod.MethodSig;
            if (!_typeCompareCache.CompareAnyReferenceEqual(MetaUtil.Inflate(oldMethodSig.RetType, oldGac), MetaUtil.Inflate(newMethodSig.RetType, newGac)))
            {
                return false;
            }
            for (int i = 0, n = oldMethodSig.Params.Count; i < n; i++)
            {
                if (!_typeCompareCache.CompareAnyReferenceEqual(MetaUtil.Inflate(oldMethodSig.Params[i], oldGac), MetaUtil.Inflate(newMethodSig.Params[i], newGac)))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsMethodDefSignatureReferenceEqual(MethodDef oldMethod, MethodDef newMethod)
        {
            if (!CompareMethodDefCommon(oldMethod, newMethod))
            {
                return false;
            }
            if (!CompareNotGenericMethod(oldMethod, newMethod))
            {
                return false;
            }
            return true;
        }

        private bool IsMemberRefSignatureReferenceEqual(MemberRef oldMethod, MemberRef newMethod)
        {
            // {TypeRef|TypeDef|TypeSpec}::{MethodDef|MdArrayMethod}
            ITypeDefOrRef oldDeclaringType = oldMethod.DeclaringType;
            ITypeDefOrRef newDeclaringType = newMethod.DeclaringType;
            if (oldDeclaringType.IsTypeDef || oldDeclaringType.IsTypeRef)
            {
                return IsMethodDefSignatureReferenceEqual(oldMethod.ResolveMethodDefThrow(), newMethod.ResolveMethodDefThrow());
            }
            else if(oldDeclaringType.IsTypeSpec)
            {
                MethodDef oldMethodDef = oldMethod.ResolveMethodDef();
                MethodDef newMethodDef = newMethod.ResolveMethodDef();
                if (oldMethodDef != null)
                {
                    if (!CompareMethodDefCommon(oldMethodDef, newMethodDef))
                    {
                        return false;
                    }
                }

                TypeSpec oldTypeSpec = (TypeSpec)oldDeclaringType;
                TypeSpec newTypeSpec = (TypeSpec)newDeclaringType;
                GenericInstSig oldGis = oldTypeSpec.TryGetGenericInstSig();
                GenericInstSig newGis = newTypeSpec.TryGetGenericInstSig();
                if (oldGis != null)
                {
                    var oldGac = new GenericArgumentContext(oldGis.GenericArguments.ToList(), null);
                    var newGac = new GenericArgumentContext(newGis.GenericArguments.ToList(), null);
                    return CompareGenericMethod(oldMethod, newMethod, oldGac, newGac);
                }
                else
                {
                    return CompareNotGenericMethod(oldMethod, newMethod);
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private bool IsMethodSpecSignatureReferenceEqual(MethodSpec oldMethod, MethodSpec newMethod)
        {
            // {TypeRef|TypeDef|TypeSpec}::{MethodDef|MdArrayMethod}<T, ...>
            ITypeDefOrRef oldDeclaringType = oldMethod.DeclaringType;
            ITypeDefOrRef newDeclaringType = newMethod.DeclaringType;
            if (oldDeclaringType.IsTypeDef || oldDeclaringType.IsTypeRef)
            {
                var oldGac = new GenericArgumentContext(null, oldMethod.GenericInstMethodSig.GenericArguments.ToList());
                var newGac = new GenericArgumentContext(null, newMethod.GenericInstMethodSig.GenericArguments.ToList());

                MethodDef oldMethodDef = oldMethod.ResolveMethodDefThrow();
                MethodDef newMethodDef = newMethod.ResolveMethodDefThrow();
                if (!CompareMethodDefCommon(oldMethodDef, newMethodDef))
                {
                    return false;
                }
                return CompareGenericMethod(oldMethod, newMethod, oldGac, newGac);
            }
            else if (oldDeclaringType.IsTypeSpec)
            {
                MethodDef oldMethodDef = oldMethod.ResolveMethodDef();
                MethodDef newMethodDef = newMethod.ResolveMethodDef();
                if (oldMethodDef != null)
                {
                    if (!CompareMethodDefCommon(oldMethodDef, newMethodDef))
                    {
                        return false;
                    }
                }

                TypeSpec oldTypeSpec = (TypeSpec)oldDeclaringType;
                TypeSpec newTypeSpec = (TypeSpec)newDeclaringType;
                GenericInstSig oldGis = oldTypeSpec.TryGetGenericInstSig();
                GenericInstSig newGis = newTypeSpec.TryGetGenericInstSig();
                var oldGac = new GenericArgumentContext(oldGis?.GenericArguments.ToList(), oldMethod.GenericInstMethodSig.GenericArguments.ToList());
                var newGac = new GenericArgumentContext(newGis?.GenericArguments.ToList(), newMethod.GenericInstMethodSig.GenericArguments.ToList());
                return CompareGenericMethod(oldMethod, newMethod, oldGac, newGac);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        enum GenericParamConstraintLevel
        {
            Class,
            StructOrAny,
        }

        private static GenericParamConstraintLevel ComputeGenericParamConstraintLevel(GenericParam gp)
        {
            if (gp.HasReferenceTypeConstraint)
            {
                return GenericParamConstraintLevel.Class;
            }
            if (gp.HasNotNullableValueTypeConstraint)
            {
                return GenericParamConstraintLevel.StructOrAny;
            }

            foreach (var c in gp.GenericParamConstraints)
            {
                var ct = c.Constraint.ToTypeSig();
                switch (ct.ElementType)
                {
                    case ElementType.Class:
                    {
                        var baseType = ct.ToTypeDefOrRef().ResolveTypeDefThrow();
                        if (!baseType.IsInterface)
                        {
                            return GenericParamConstraintLevel.Class;
                        }
                        break;
                    }
                    case ElementType.GenericInst:
                    {
                        var baseGenericType = (ct as GenericInstSig).GenericType.ToTypeDefOrRef().ResolveTypeDefThrow();
                        if (!baseGenericType.IsInterface)
                        {
                            return GenericParamConstraintLevel.Class;
                        }
                        break;
                    }
                }
            }
            return GenericParamConstraintLevel.StructOrAny;
        }

        private bool IsGenericParamConstraintCompatible(MethodDef oldMethod, MethodDef newMethod)
        {
            if (oldMethod.GenericParameters.Count != newMethod.GenericParameters.Count)
            {
                return false;
            }

            for (int i = 0; i < newMethod.GenericParameters.Count; i++)
            {
                var oldGp = oldMethod.GenericParameters[i];
                var newGp = newMethod.GenericParameters[i];
                GenericParamConstraintLevel level1 = ComputeGenericParamConstraintLevel(oldGp);
                GenericParamConstraintLevel level2 = ComputeGenericParamConstraintLevel(newGp);
                if (level1 < level2)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareToken(ITokenOperand t1, ITokenOperand t2, MethodBodyCompareData callerMethod)
        {
            if (t1.GetType() != t2.GetType())
            {
                return false;
            }
            if (t1 is ITypeDefOrRef td1 && t2 is ITypeDefOrRef td2)
            {
                //return CompareEqualTypeAndMemoryLayout(td1, td2);
                return TypeEqualityComparer.Instance.Equals(td1, td2);
            }
            if (t1 is IMethod m1 && t2 is IMethod m2)
            {
                //return CompareCallNotVirtualMethod(m1, m2, callerMethod);
                return MethodEqualityComparer.CompareDeclaringTypes.Equals(m1, m2);
            }
            if (t1 is IField f1 && t2 is IField f2)
            {
                //return CompareField(f1, f2);
                return FieldEqualityComparer.CompareDeclaringTypes.Equals(f1, f2);
            }
            return false;
        }

        private bool CompareMethodSigParamReferenceEqual(MethodSig m1, MethodSig m2)
        {
            if (m1.CallingConvention != m2.CallingConvention)
            {
                return false;
            }
            if (m1.Params.Count != m2.Params.Count)
            {
                return false;
            }
            if (!_typeCompareCache.CompareAnyReferenceEqual(m1.RetType, m2.RetType))
            {
                return false;
            }
            for (int i = 0, n = m1.Params.Count; i < n; i++)
            {
                if (!_typeCompareCache.CompareAnyReferenceEqual(m1.Params[i], m2.Params[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
