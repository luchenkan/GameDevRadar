using dnlib.DotNet.Writer;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dnlib.DotNet.Emit;
using HybridCLR.Editor.Meta;
using UnityEngine;

namespace HybridCLR.Editor.Hotfix
{
    public class HotfixAssemblyMetadataStripper
    {
        public byte[] Strip(byte[] assemblyBytes, HotfixManifest manifest)
        {
            var context = ModuleDef.CreateModuleContext();
            var readerOption = new ModuleCreationOptions(context)
            {
                Runtime = CLRRuntimeReaderKind.Mono
            };
            _oldMod = ModuleDefMD.Load(assemblyBytes, readerOption);
            string assName = _oldMod.Assembly.Name;
            _hotfixAss = manifest.assemblies.Find(ass => ass.name == assName);
            if (_hotfixAss == null)
            {
                throw new Exception($"assembly:{assName} not found in hotfix manifest");
            }
            _newMod = CreateStrippedAssembly();
            CollectReferences();
            BuildNewAssembly();
            var writer = new System.IO.MemoryStream();
            _newMod.Write(writer);
            writer.Flush();
            return writer.ToArray();
        }

        private ModuleDef _oldMod;
        private ModuleDef _newMod;
        private HotfixAssembly _hotfixAss;

        private readonly HashSet<TypeDef> _rootTypes = new HashSet<TypeDef>();
        private readonly HashSet<MethodDef> _rootMethods = new HashSet<MethodDef>();
        
        private readonly HashSet<TypeDef> _refTypes = new HashSet<TypeDef>();
        private readonly HashSet<MethodDef> _refMethods = new HashSet<MethodDef>();
        private readonly HashSet<FieldDef> _refFields = new HashSet<FieldDef>();


        private ModuleDef CreateStrippedAssembly()
        {
            var context = ModuleDef.CreateModuleContext();
            var readerOption = new ModuleCreationOptions(context)
            {
                Runtime = CLRRuntimeReaderKind.Mono,
            };

            var strippedModule = new ModuleDefUser(_oldMod.Name, _oldMod.Mvid, _oldMod.CorLibTypes.AssemblyRef);

            return strippedModule;
        }


        private void WalkType(TypeDef typeDef)
        {
            if (typeDef.Module != _oldMod)
            {
                return;
            }
            if (!_refTypes.Add(typeDef))
            {
                return;
            }
            if (typeDef.DeclaringType != null)
            {
                WalkType(typeDef.DeclaringType);
            }
            if (typeDef.BaseType != null)
            {
                CollectType(typeDef.BaseType.ToTypeSig());
            }
            //foreach (var fieldDef in typeDef.Fields)
            //{
            //    CollectType(fieldDef.FieldType);
            //}
        }

        private void CollectType(TypeSig type)
        {
            type = type.RemovePinnedAndModifiers();
            if (type.Next != null)
            {
                CollectType(type.Next);
            }
            switch (type.ElementType)
            {
                case ElementType.Class:
                case ElementType.ValueType:
                {
                    var typeDef = type.ToTypeDefOrRef() as TypeDef;
                    if (typeDef != null)
                    {
                        WalkType(typeDef);
                    }
                    break;
                }
                case ElementType.GenericInst:
                {
                    var genericInst = (GenericInstSig)type;
                    CollectType(genericInst.GenericType);
                    foreach (var arg in genericInst.GenericArguments)
                    {
                        CollectType(arg);
                    }
                    break;
                }
            }
        }

        private void WalkMethod(MethodDef methodDef)
        {
            if (methodDef.Module != _oldMod)
            {
                return;
            }
            if (!_refMethods.Add(methodDef))
            {
                return;
            }
            CollectType(methodDef.MethodSig.RetType);
            foreach (var param in methodDef.MethodSig.Params)
            {
                CollectType(param);
            }
        }

        private void CollectMethod(IMethod method)
        {
            if (method is MethodDef methodDef)
            {
                if (methodDef.DeclaringType != null)
                {
                    WalkType(methodDef.DeclaringType);
                }
                WalkMethod(methodDef);
            }
            else if (method is MemberRef memberRef)
            {
                if (memberRef.Class is ITypeDefOrRef typeDefOrRef)
                {
                    CollectType(typeDefOrRef.ToTypeSig());
                }
                CollectType(memberRef.MethodSig.RetType);
                foreach (var param in memberRef.MethodSig.Params)
                {
                    CollectType(param);
                }
                methodDef = memberRef.ResolveMethodDef();
                if (methodDef != null)
                {
                    WalkMethod(methodDef);
                }
            }
            else if (method is MethodSpec methodSpec)
            {
                CollectMethod(methodSpec.Method);
                foreach (var genericArg in methodSpec.GenericInstMethodSig.GetGenericArguments())
                {
                    CollectType(genericArg);
                }
            }
        }

        private void WalkField(FieldDef field)
        {
            if (field.Module != _oldMod)
            {
                return;
            }
            if (!_refFields.Add(field))
            {
                return;
            }
            CollectType(field.FieldSig.Type);
            if (field.DeclaringType != null)
            {
                WalkType(field.DeclaringType);
            }
        }

        private void CollectField(IField field)
        {
            if (!field.IsField)
            {
                return;
            }
            
            if (field is FieldDef fieldDef)
            {
                WalkField(fieldDef);
            }
            if (field is MemberRef memberRef)
            {
                if (memberRef.Class is ITypeDefOrRef typeDefOrRef)
                {
                    CollectType(typeDefOrRef.ToTypeSig());
                }
                CollectType(memberRef.FieldSig.Type);
                fieldDef = memberRef.ResolveFieldDef();
                if (fieldDef != null)
                {
                    WalkField(fieldDef);
                }
            }
        }

        private void CollectReferences()
        {
            foreach (var type in _hotfixAss.types)
            {
                var typeDef = _oldMod.Find(type.name, true);
                if (typeDef == null)
                {
                    throw new Exception($"type:{type.name} not found in assembly:{_oldMod.Name}");
                }
                _rootTypes.Add(typeDef);
                foreach (var method in type.methods)
                {
                    bool found = false;
                    foreach (var methodDef in typeDef.Methods)
                    {
                        if (!string.IsNullOrEmpty(method.name))
                        {
                            if (method.name == methodDef.Name)
                            {
                                found = true;
                                _rootMethods.Add(methodDef);
                            }
                        }
                        else if (!string.IsNullOrEmpty(method.signature))
                        {
                            string methodDefSignature = MetaUtil.CreateMethodDefSignature(methodDef);
                            if (methodDefSignature == method.signature)
                            {
                                found = true;
                                _rootMethods.Add(methodDef);
                            }
                        }
                    }
                    if (!found)
                    {
                        foreach (var methodDef in typeDef.Methods)
                        {
                            Debug.Log($"=== {MetaUtil.CreateMethodDefSignature(methodDef)}");
                        }
                        throw new Exception($"method name:`{method.name}` signature:`{method.signature}` not found in type:{type.name}");
                    }
                }
            }
            

            foreach (var typeDef in _rootTypes)
            {
                WalkType(typeDef);
            }

            foreach (var methodDef in _rootMethods)
            {
                WalkMethod(methodDef);
                foreach (var local in methodDef.Body.Variables)
                {
                    CollectType(local.Type);
                }
                foreach (var inst in methodDef.Body.Instructions)
                {
                    object operand = inst.Operand;
                    if (operand is IMethod method)
                    {
                        if (method.IsMethod)
                        {
                            CollectMethod(method);
                        }
                        else if (method.IsField)
                        {
                            CollectField((IField)method);
                        }
                    }
                    else if (operand is ITypeDefOrRef type)
                    {
                        CollectType(type.ToTypeSig());
                    }
                    else if (operand is IField field)
                    {
                        if (field.IsField)
                        {
                            CollectField(field);
                        }
                        else if (field.IsMethod)
                        {
                            CollectMethod((IMethod)field);
                        }
                    }
                }
            }
        }

        private TypeDef GetTypeDefInNewModule(TypeDef oldType)
        {
            //if (oldType.Module != _oldMod)
            //{
            //    return oldType;
            //}
            if (!_old2NewTypeMap.TryGetValue(oldType, out var newType))
            {
                throw new Exception($"type:{oldType.FullName} not found in target module:{_oldMod.Name}");
            }
            return newType;
        }

        private TypeSig RetargetTypeRefInTypeSig(TypeSig type)
        {
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
                if (type is PinnedSig ps)
                {
                    return new PinnedSig(newNext);
                }
                throw new System.NotSupportedException(type.ToString());
            }
            switch (type.ElementType)
            {
                case ElementType.Ptr:
                {
                    if (next == newNext)
                    {
                        return type;
                    }
                    return new PtrSig(newNext);
                }
                case ElementType.ValueType:
                case ElementType.Class:
                {
                    var vts = type as ClassOrValueTypeSig;
                    if (vts.TypeDefOrRef is TypeRef typeRef)
                    {
                        return vts;
                    }
                    TypeDef typeDef = (TypeDef)vts.TypeDefOrRef;
                    TypeDef newTypeDef = GetTypeDefInNewModule(typeDef);
                    return type.IsClassSig ? (TypeSig)new ClassSig(newTypeDef) : new ValueTypeSig(newTypeDef);
                }
                case ElementType.Array:
                {
                    if (next == newNext)
                    {
                        return type;
                    }
                    return new ArraySig(newNext);
                }
                case ElementType.SZArray:
                {
                    if (next == newNext)
                    {
                        return type;
                    }
                    return new SZArraySig(newNext);
                }
                case ElementType.GenericInst:
                {
                    var gis = type as GenericInstSig;
                    ClassOrValueTypeSig genericType = gis.GenericType;
                    ClassOrValueTypeSig newGenericType = (ClassOrValueTypeSig)RetargetTypeRefInTypeSig(genericType);
                    bool anyChange = genericType != newGenericType;
                    var genericArgs = new List<TypeSig>();
                    foreach (var arg in gis.GenericArguments)
                    {
                        TypeSig newArg = RetargetTypeRefInTypeSig(arg);
                        anyChange |= newArg != genericType;
                        genericArgs.Add(newArg);
                    }
                    if (!anyChange)
                    {
                        return type;
                    }
                    return new GenericInstSig(newGenericType, genericArgs);
                }
                case ElementType.FnPtr:
                {
                    var fp = type as FnPtrSig;
                    MethodSig methodSig = fp.MethodSig;
                    TypeSig newReturnType = RetargetTypeRefInTypeSig(methodSig.RetType);
                    bool anyChange = newReturnType != methodSig.RetType;
                    var newArgs = new List<TypeSig>();
                    foreach (TypeSig arg in methodSig.Params)
                    {
                        TypeSig newArg = RetargetTypeRefInTypeSig(arg);
                        anyChange |= newArg != newReturnType;
                    }
                    if (!anyChange)
                    {
                        return type;
                    }
                    if (methodSig.ParamsAfterSentinel != null)
                    {
                        throw new System.NotSupportedException("FnPtrSig with ParamsAfterSentinel is not supported");
                    }
                    var newMethodSig = new MethodSig(methodSig.CallingConvention, methodSig.GenParamCount, newReturnType, newArgs, null);
                    return new FnPtrSig(newMethodSig);
                }
                case ElementType.ByRef:
                {
                    if (next == newNext)
                    {
                        return type;
                    }
                    return new ByRefSig(newNext);
                }
                default:
                {
                    return type;
                }
            }
        }

        private ITypeDefOrRef RetargetTypeRefInTypeDefOrRef(ITypeDefOrRef type)
        {
            if (type == null)
            {
                return null;
            }
            return RetargetTypeRefInTypeSig(type.ToTypeSig()).ToTypeDefOrRef();
        }

        private readonly Dictionary<TypeDef, TypeDef> _old2NewTypeMap = new Dictionary<TypeDef, TypeDef>();
        private readonly Dictionary<TypeDef, TypeDef> _new2OldTypeMap = new Dictionary<TypeDef, TypeDef>();


        private MethodSig RetargetMethodSig(MethodSig oldMethodSig)
        {
            var newMethodSig = new MethodSig(oldMethodSig.CallingConvention, oldMethodSig.GenParamCount, RetargetTypeRefInTypeSig(oldMethodSig.RetType));
            foreach (var arg in oldMethodSig.Params)
            {
                newMethodSig.Params.Add(RetargetTypeRefInTypeSig(arg));
            }
            if (oldMethodSig.ParamsAfterSentinel != null)
            {
                throw new System.NotSupportedException("ParamsAfterSentinel is not supported");
            }
            return newMethodSig;
        }

        private FieldSig RetargetFieldSig(FieldSig oldFieldSig)
        {
            return new FieldSig(RetargetTypeRefInTypeSig(oldFieldSig.Type));
        }

        public IMethod RetargetMethod(IMethod oldMethod)
        {
            if (oldMethod is MethodDef methodDef)
            {
                if (_old2NewMethodMap.TryGetValue(methodDef, out var newMethodDef))
                {
                    return newMethodDef;
                }
                else
                {
                    throw new Exception($"method:{methodDef.FullName} not found in target module:{_oldMod.Name}");
                }
            }
            else if (oldMethod is MemberRef memberRef)
            {
                var newType = RetargetTypeRefInTypeDefOrRef((ITypeDefOrRef)memberRef.Class);
                return new MemberRefUser(_newMod, memberRef.Name, RetargetMethodSig(memberRef.MethodSig), newType);
            }
            else if (oldMethod is MethodSpec methodSpec)
            {
                var newMethod = (IMethodDefOrRef)RetargetMethod(methodSpec.Method);
                var newGenericArgs = new List<TypeSig>();
                foreach (var genericArg in methodSpec.GenericInstMethodSig.GetGenericArguments())
                {
                    newGenericArgs.Add(RetargetTypeRefInTypeSig(genericArg));
                }
                var newGenericInstMethodSig = new GenericInstMethodSig(newGenericArgs);
                return new MethodSpecUser(newMethod, newGenericInstMethodSig);
            }
            else
            {
                throw new System.NotSupportedException(oldMethod.ToString());
            }
        }

        public IField RetargetField(IField oldField)
        {
            if (oldField is FieldDef fieldDef)
            {
                if (_old2NewFieldMap.TryGetValue(fieldDef, out var newFieldDef))
                {
                    return newFieldDef;
                }
                else
                {
                    throw new Exception($"field:{fieldDef.FullName} not found in target module:{_oldMod.Name}");
                }
            }
            else if (oldField is MemberRef memberRef)
            {
                UnityEngine.Debug.Assert(memberRef.IsFieldRef);
                var newType = RetargetTypeRefInTypeDefOrRef((ITypeDefOrRef)memberRef.Class);
                return new MemberRefUser(_newMod, memberRef.Name, RetargetFieldSig(memberRef.FieldSig), newType);
            }
            else
            {
                throw new System.NotSupportedException(oldField.ToString());
            }
        }

        private readonly Dictionary<MethodDef, MethodDef> _old2NewMethodMap = new Dictionary<MethodDef, MethodDef>();
        private readonly Dictionary<FieldDef, FieldDef> _old2NewFieldMap = new Dictionary<FieldDef, FieldDef>();

        private void RecreateTypes()
        {
            foreach (var oldTypeRef in _refTypes)
            {
                var newTypeDef = new TypeDefUser(oldTypeRef.Namespace, oldTypeRef.Name);
                newTypeDef.Attributes = oldTypeRef.Attributes;
                newTypeDef.ClassLayout = oldTypeRef.ClassLayout;

                if (oldTypeRef.GenericParameters != null)
                {
                    foreach (var genericParam in oldTypeRef.GenericParameters)
                    {
                        var newGenericParam = new GenericParamUser(genericParam.Number, genericParam.Flags, genericParam.Name);
                        newTypeDef.GenericParameters.Add(newGenericParam);
                    }
                }
                _old2NewTypeMap[oldTypeRef] = newTypeDef;
                _new2OldTypeMap[newTypeDef] = oldTypeRef;
                if (oldTypeRef.DeclaringType == null)
                {
                    _newMod.Types.Add(newTypeDef);
                }
            }
            foreach (TypeDef newTypeDef in _new2OldTypeMap.Keys)
            {
                TypeDef oldTypeDef = _new2OldTypeMap[newTypeDef];
                if (oldTypeDef.DeclaringType != null)
                {
                    var newDeclaringType = GetTypeDefInNewModule(oldTypeDef.DeclaringType);
                    newTypeDef.DeclaringType = newDeclaringType;
                }
                if (oldTypeDef.BaseType != null)
                {
                    var newBaseType = RetargetTypeRefInTypeSig(oldTypeDef.BaseType.ToTypeSig());
                    newTypeDef.BaseType = newBaseType.ToTypeDefOrRef();
                }
            }
            foreach (TypeDef newTypeDef in _new2OldTypeMap.Keys)
            {
                TypeDef oldTypeDef = _new2OldTypeMap[newTypeDef];
                foreach (FieldDef oldFieldDef in oldTypeDef.Fields)
                {
                    if (!_refFields.Contains(oldFieldDef))
                    {
                        continue; // skip fields that are not referenced
                    }
                    var newFieldDef = new FieldDefUser(oldFieldDef.Name, RetargetFieldSig(oldFieldDef.FieldSig), oldFieldDef.Attributes);
                    _old2NewFieldMap[oldFieldDef] = newFieldDef;
                    newTypeDef.Fields.Add(newFieldDef);
                }
            }
            foreach (MethodDef oldMethodDef in _refMethods)
            {
                var newMethodSig = RetargetMethodSig(oldMethodDef.MethodSig);
                var newMethodDef = new MethodDefUser(oldMethodDef.Name, newMethodSig, oldMethodDef.Attributes);
                newMethodDef.ImplAttributes = oldMethodDef.ImplAttributes;
                
                TypeDef newTypeDef = GetTypeDefInNewModule(oldMethodDef.DeclaringType);
                newTypeDef.Methods.Add(newMethodDef);
                _old2NewMethodMap[oldMethodDef] = newMethodDef;
            }
        }

        private void RecreateMethodBody(MethodDef oldMethodDef, MethodDef newMethodDef)
        {
            var newInsts = new List<Instruction>();
            var newExceptionHandlers = new List<ExceptionHandler>();
            var newLocals = new List<Local>();
            foreach (var oldLocal in oldMethodDef.Body.Variables)
            {
                var newLocal = new Local(RetargetTypeRefInTypeSig(oldLocal.Type));
                newLocals.Add(newLocal);
            }
            var instMap = new Dictionary<Instruction, Instruction>();
            foreach (var oldInst in oldMethodDef.Body.Instructions)
            {
                var newInst = oldInst.Clone();
                instMap[oldInst] = newInst;
                object operand = oldInst.Operand;
                if (operand is ITypeDefOrRef type)
                {
                    newInst.Operand = RetargetTypeRefInTypeDefOrRef(type);
                }
                else if (operand is CorLibTypeSig typeSig)
                {
                    newInst.Operand = typeSig.TypeDefOrRef;
                }
                else if (operand is IMethod method)
                {
                    newInst.Operand = method.IsMethod ? (object)RetargetMethod(method) : RetargetField((IField)method);
                }
                else if (operand is MethodSig methodSig)
                {
                    newInst.Operand = RetargetMethodSig(methodSig);
                }
                else if (operand is IField field)
                {
                    newInst.Operand = RetargetField(field);
                }
                else if (operand is Local local)
                {
                    newInst.Operand = newLocals[local.Index];
                }
                else if (operand is Parameter parameter)
                {
                    newInst.Operand = newMethodDef.Parameters[parameter.Index];
                }
                newInsts.Add(newInst);
            }
            // retarget branches
            foreach (var newInst in newInsts)
            {
                if (newInst.Operand is Instruction targetInst)
                {
                    newInst.Operand = instMap[targetInst];
                }
                else if (newInst.Operand is IList<Instruction> targetInsts)
                {
                    for (int i = 0; i < targetInsts.Count; i++)
                    {
                        targetInsts[i] = instMap[targetInsts[i]];
                    }
                }
            }
            foreach (ExceptionHandler oldExHandler in oldMethodDef.Body.ExceptionHandlers)
            {
                var newExHandler = new ExceptionHandler(oldExHandler.HandlerType)
                {
                    TryStart = instMap[oldExHandler.TryStart],
                    TryEnd = instMap[oldExHandler.TryEnd],
                    HandlerStart = instMap[oldExHandler.HandlerStart],
                    HandlerEnd = instMap[oldExHandler.HandlerEnd],
                    FilterStart = oldExHandler.FilterStart != null ? instMap[oldExHandler.FilterStart] : null,
                    CatchType = RetargetTypeRefInTypeDefOrRef(oldExHandler.CatchType),
                };
                newExceptionHandlers.Add(newExHandler);
            }
            newMethodDef.Body = new CilBody(oldMethodDef.Body.InitLocals, newInsts, newExceptionHandlers, newLocals);
        }

        private void BuildNewAssembly()
        {
            RecreateTypes();
            
            foreach (MethodDef oldMethodDef in _rootMethods)
            {
                MethodDef newMethodDef = _old2NewMethodMap[oldMethodDef];
                RecreateMethodBody(oldMethodDef, newMethodDef);
            }
        }

        public static void StripAssembly(string originalAssemblyPath, HotfixManifest manifest, string strippedAssemblyPath)
        {
            byte[] originDllBytes = System.IO.File.ReadAllBytes(originalAssemblyPath);
            var stripper = new HotfixAssemblyMetadataStripper();
            byte[] strippedDllBytes = stripper.Strip(originDllBytes, manifest);
            UnityEngine.Debug.Log($"aot dll:{originalAssemblyPath}, length: {originDllBytes.Length} -> {strippedDllBytes.Length}, stripping rate:{(originDllBytes.Length - strippedDllBytes.Length) / (double)originDllBytes.Length * 100.0}%");
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(strippedAssemblyPath));
            System.IO.File.WriteAllBytes(strippedAssemblyPath, strippedDllBytes);
        }
    }
}
