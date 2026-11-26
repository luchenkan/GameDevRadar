#include "DifferentialHybridImage.h"

#include "vm/Image.h"
#include "vm/MetadataAlloc.h"
#include "vm/MetadataLock.h"
#include "os/Mutex.h"
#include "metadata/GenericMethod.h"
#include "metadata/GenericMetadata.h"

#include "MetadataModule.h"
#include "MetadataPool.h"

namespace hybridclr
{
namespace metadata
{

	const Il2CppGenericInst* DifferentialHybridImage::TranslateGenericInstToDHE(const Il2CppGenericInst* gi)
	{
		if (!gi)
		{
			return gi;
		}
		const Il2CppType* tempTypeArgv[32];
		IL2CPP_ASSERT(gi->type_argc <= 32);
		bool change = false;
		for (uint32_t i = 0; i < gi->type_argc; i++)
		{
			const Il2CppType* argType = gi->type_argv[i];
			const Il2CppType* dhArgType = TranslateIl2CppTypeToDHE(argType);
			change = change || (argType != dhArgType);
			tempTypeArgv[i] = dhArgType;
		}
		if (!change)
		{
			return gi;
		}
		return il2cpp::vm::MetadataCache::GetGenericInst(tempTypeArgv, gi->type_argc);
	}

	const Il2CppType* DifferentialHybridImage::TranslateIl2CppTypeToDHE(const Il2CppType* type)
	{
		switch (type->type)
		{
		case IL2CPP_TYPE_OBJECT:
		case IL2CPP_TYPE_VOID:
		case IL2CPP_TYPE_BOOLEAN:
		case IL2CPP_TYPE_CHAR:
		case IL2CPP_TYPE_I1:
		case IL2CPP_TYPE_U1:
		case IL2CPP_TYPE_I2:
		case IL2CPP_TYPE_U2:
		case IL2CPP_TYPE_I4:
		case IL2CPP_TYPE_U4:
		case IL2CPP_TYPE_I:
		case IL2CPP_TYPE_U:
		case IL2CPP_TYPE_I8:
		case IL2CPP_TYPE_U8:
		case IL2CPP_TYPE_R4:
		case IL2CPP_TYPE_R8:
		case IL2CPP_TYPE_STRING:
		case IL2CPP_TYPE_TYPEDBYREF:
			return type;
		case IL2CPP_TYPE_ARRAY:
		{
			const Il2CppType* eleType = type->data.array->etype;
			const Il2CppType* dheEleType = TranslateIl2CppTypeToDHE(eleType);
			if (eleType == dheEleType)
			{
				return type;
			}
			il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
			return MetadataPool::CreateArrayType(*type, dheEleType);
		}
		case IL2CPP_TYPE_FNPTR:
			IL2CPP_NOT_IMPLEMENTED(Class::FromIl2CppType);
			return NULL; //mono_fnptr_class_get (type->data.method);
		case IL2CPP_TYPE_PTR:
		case IL2CPP_TYPE_SZARRAY:
		{
			const Il2CppType* eleType = type->data.type;
			const Il2CppType* dheEleType = TranslateIl2CppTypeToDHE(eleType);
			if (eleType == dheEleType)
			{
				return type;
			}
			Il2CppType newType = *type;
			newType.data.type = dheEleType;
			il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
			return MetadataPool::GetPooledIl2CppType(newType);
		}
		case IL2CPP_TYPE_CLASS:
		case IL2CPP_TYPE_VALUETYPE:
		{
			Il2CppTypeDefinition* typeDef = (Il2CppTypeDefinition*)type->data.typeHandle;
			if (IsInterpreterType(typeDef))
			{
				return type;
			}
			TypeIndex typeIndex = il2cpp::vm::GlobalMetadata::GetIndexForTypeDefinition(typeDef);
			const Il2CppImage* image = il2cpp::vm::GlobalMetadata::GetImageForTypeDefinitionIndex(typeIndex);
			if (!IsAOTImageDifferentialHybridImage(image))
			{
				return type;
			}
			const Il2CppImage* dhImage = image->assembly->dheAssembly->image;
			DifferentialHybridImage* dheInterImg = ((DifferentialHybridImage*)MetadataModule::GetImage(dhImage));
			if (!dheInterImg)
			{
				return type;
			}
			const Il2CppTypeDefinition* dhTypeDef = dheInterImg->GetInterpreterTypeByOriginType(typeDef);
			if (!dhTypeDef)
			{
				return type;
			}
			il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
			return MetadataPool::CreateClassOrValueTypeType(*type, (Il2CppMetadataTypeHandle)dhTypeDef);
		}
		case IL2CPP_TYPE_GENERICINST:
		{
			Il2CppGenericClass* gc = type->data.generic_class;
			const Il2CppType* dhType = TranslateIl2CppTypeToDHE(gc->type);
			const Il2CppGenericInst* klassGi = TranslateGenericInstToDHE(gc->context.class_inst);
			IL2CPP_ASSERT(gc->context.method_inst == nullptr);
			if (dhType == gc->type && klassGi == gc->context.class_inst)
			{
				return type;
			}
			il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
			return MetadataPool::CreateGenericInstType(*type, il2cpp::metadata::GenericMetadata::GetGenericClass(dhType, klassGi));
		}
		case IL2CPP_TYPE_VAR:
		case IL2CPP_TYPE_MVAR:
			return type;
		default:
			IL2CPP_NOT_IMPLEMENTED(Class::FromIl2CppType);
		}
		return nullptr;
	}

	const Il2CppMethodDefinition* DifferentialHybridImage::TranslateMethodDefinitionToDHE(const Il2CppMethodDefinition* methodDef)
	{
		Il2CppTypeDefinition* typeDef = (Il2CppTypeDefinition*)il2cpp::vm::GlobalMetadata::GetTypeHandleFromIndex(methodDef->declaringType);

		TypeIndex typeIndex = il2cpp::vm::GlobalMetadata::GetIndexForTypeDefinition(typeDef);
		const Il2CppImage* image = il2cpp::vm::GlobalMetadata::GetImageForTypeDefinitionIndex(typeIndex);
		if (IsAOTImageDifferentialHybridImage(image))
		{
			const Il2CppImage* dhImage = image->assembly->dheAssembly->image;
			DifferentialHybridImage* dheInterImg = ((DifferentialHybridImage*)MetadataModule::GetImage(dhImage));
			if (!dheInterImg)
			{
				return methodDef;
			}
			const Il2CppMethodDefinition* dhMethodDef = dheInterImg->GetInterpreterMethodDefinitionByOriginMethodDefinition(methodDef);
			if (dhMethodDef)
			{
				return dhMethodDef;
			}
		}
		return methodDef;
	}

#if HYBRIDCLR_UNITY_6000_OR_NEW
	const Il2CppGenericMethod DifferentialHybridImage::TranslateGenericMethodToDHE(const Il2CppGenericMethod& genericMethod)
	{
		const Il2CppImage* image = genericMethod.methodDefinition->klass->image;

		const Il2CppGenericContext& gc = genericMethod.context;
		const Il2CppGenericInst* klassGi = TranslateGenericInstToDHE(gc.class_inst);
		const Il2CppGenericInst* methodGi = TranslateGenericInstToDHE(gc.method_inst);
		const MethodInfo* dhMethod = genericMethod.methodDefinition;

		if (IsAOTImageDifferentialHybridImage(image))
		{
			const Il2CppMethodDefinition* dhMethodDef = TranslateMethodDefinitionToDHE((Il2CppMethodDefinition*)dhMethod->methodMetadataHandle);
			if ((Il2CppMetadataMethodDefinitionHandle)dhMethodDef != dhMethod->methodMetadataHandle)
			{
				dhMethod = il2cpp::vm::GlobalMetadata::GetMethodInfoFromMethodHandle((Il2CppMetadataMethodDefinitionHandle)dhMethodDef);
			}
		}

		Il2CppGenericMethod dhGenericMethod = { dhMethod, {klassGi, methodGi} };
		return dhGenericMethod;
	}
#else
	const Il2CppGenericMethod* DifferentialHybridImage::TranslateGenericMethodToDHE(const Il2CppGenericMethod* genericMethod)
	{
		const Il2CppImage* image = genericMethod->methodDefinition->klass->image;

		const Il2CppGenericContext& gc = genericMethod->context;
		const Il2CppGenericInst* klassGi = TranslateGenericInstToDHE(gc.class_inst);
		const Il2CppGenericInst* methodGi = TranslateGenericInstToDHE(gc.method_inst);
		const MethodInfo* dhMethod = genericMethod->methodDefinition;

		if (IsAOTImageDifferentialHybridImage(image))
		{
			const Il2CppMethodDefinition* dhMethodDef = TranslateMethodDefinitionToDHE((Il2CppMethodDefinition*)dhMethod->methodMetadataHandle);
			if ((Il2CppMetadataMethodDefinitionHandle)dhMethodDef != dhMethod->methodMetadataHandle)
			{
				dhMethod = il2cpp::vm::GlobalMetadata::GetMethodInfoFromMethodHandle((Il2CppMetadataMethodDefinitionHandle)dhMethodDef);
			}
		}

		const Il2CppGenericMethod* dhGenericMethod;
		if (dhMethod == genericMethod->methodDefinition && klassGi == gc.class_inst && methodGi == gc.method_inst)
		{
			dhGenericMethod = genericMethod;
		}
		else
		{
			dhGenericMethod = il2cpp::vm::MetadataCache::GetGenericMethod(dhMethod, klassGi, methodGi);
		}
		return dhGenericMethod;
	}
#endif


	const MethodInfo* DifferentialHybridImage::TranslateReverseMethodInfoFromDHE(const MethodInfo* method)
	{
		const Il2CppImage* image = method->klass->image;
		if (!IsDifferentialHybridImage(image))
		{
			return method;
		}
		DifferentialHybridImage* dheImage = (DifferentialHybridImage*)MetadataModule::GetImage(image);
		if (!dheImage)
		{
			return method;
		}
		const Il2CppMethodDefinition* originMethodDef = dheImage->GetUnchangedOriginMethodDefintionByInterpreterMethod((const Il2CppMethodDefinition*)method->methodMetadataHandle);
		return originMethodDef ? il2cpp::vm::GlobalMetadata::GetMethodInfoFromMethodHandle((Il2CppMetadataMethodDefinitionHandle)originMethodDef) : method;
	}

	const MethodInfo* DifferentialHybridImage::TranslateReverseNormalOrGenericMethodInfoFromDHE(const MethodInfo* method)
	{
		if (!method->is_inflated)
		{
			return TranslateReverseMethodInfoFromDHE(method);
		}
		const Il2CppGenericMethod* genericMethod = method->genericMethod;
#if HYBRIDCLR_UNITY_6000_OR_NEW
		const Il2CppGenericMethod dhGenericMethod = TranslateGenericMethodToDHE(*genericMethod);
		return il2cpp::metadata::GenericMethod::GetMethod(dhGenericMethod);
#else
		const Il2CppGenericMethod* dhGenericMethod = TranslateGenericMethodToDHE(genericMethod);
		if (dhGenericMethod == genericMethod)
		{
			return method;
		}
		return il2cpp::metadata::GenericMethod::GetMethod(dhGenericMethod);
#endif
	}

	const Il2CppType* DifferentialHybridImage::TranslateReverseGenericShareIl2CppTypeFromDHE(const Il2CppType* type)
	{
		switch (type->type)
		{
		case IL2CPP_TYPE_OBJECT:
		case IL2CPP_TYPE_VOID:
		case IL2CPP_TYPE_BOOLEAN:
		case IL2CPP_TYPE_CHAR:
		case IL2CPP_TYPE_I1:
		case IL2CPP_TYPE_U1:
		case IL2CPP_TYPE_I2:
		case IL2CPP_TYPE_U2:
		case IL2CPP_TYPE_I4:
		case IL2CPP_TYPE_U4:
		case IL2CPP_TYPE_I:
		case IL2CPP_TYPE_U:
		case IL2CPP_TYPE_I8:
		case IL2CPP_TYPE_U8:
		case IL2CPP_TYPE_R4:
		case IL2CPP_TYPE_R8:
		case IL2CPP_TYPE_STRING:
		case IL2CPP_TYPE_TYPEDBYREF:
		case IL2CPP_TYPE_ARRAY:
			return type;
		case IL2CPP_TYPE_FNPTR:
			IL2CPP_NOT_IMPLEMENTED(Class::FromIl2CppType);
			return NULL; //mono_fnptr_class_get (type->data.method);
		case IL2CPP_TYPE_PTR:
		case IL2CPP_TYPE_SZARRAY:
		case IL2CPP_TYPE_CLASS:
			return type;
		case IL2CPP_TYPE_VALUETYPE:
		{
			Il2CppTypeDefinition* typeDef = (Il2CppTypeDefinition*)type->data.typeHandle;
			if (!IsInterpreterType(typeDef))
			{
				return type;
			}
			InterpreterImage* image = MetadataModule::GetImage(typeDef);
			if (!IsDifferentialHybridImage(image))
			{
				return type;
			}
			DifferentialHybridImage* dhImage = (DifferentialHybridImage*)image;
			const Il2CppTypeDefinition* aotTypeDef = dhImage->GetGenericShareOriginType(typeDef);
			if (!aotTypeDef)
			{
				return type;
			}
			if (!type->byref)
			{
				return il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(aotTypeDef->byvalTypeIndex);
			}
			else
			{
#if HYBRIDCLR_UNITY_2019
				return il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(aotTypeDef->byrefTypeIndex);
#else
				il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
				return MetadataPool::CreateClassOrValueTypeType(*type, (Il2CppMetadataTypeHandle)aotTypeDef);
#endif
			}
		}
		case IL2CPP_TYPE_GENERICINST:
		{
			Il2CppGenericClass* gc = type->data.generic_class;
			if (!IsValueType((Il2CppTypeDefinition*)gc->type->data.typeHandle))
			{
				return type;
			}
			const Il2CppType* dhType = TranslateReverseGenericShareIl2CppTypeFromDHE(gc->type);
			const Il2CppGenericInst* klassGi = TranslateReverseGenericShareGenericInstFromDHE(gc->context.class_inst);
			IL2CPP_ASSERT(!gc->context.method_inst);
			if (dhType == gc->type && klassGi == gc->context.class_inst)
			{
				return type;
			}
			il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
			return MetadataPool::CreateGenericInstType(*type, il2cpp::metadata::GenericMetadata::GetGenericClass(dhType, klassGi));
		}
		case IL2CPP_TYPE_VAR:
		case IL2CPP_TYPE_MVAR:
			return type;
		default:
			IL2CPP_NOT_IMPLEMENTED(Class::FromIl2CppType);
		}
		return nullptr;
	}

	const Il2CppGenericInst* DifferentialHybridImage::TranslateReverseGenericShareGenericInstFromDHE(const Il2CppGenericInst* gi)
	{
		if (!gi)
		{
			return gi;
		}
		const Il2CppType* tempTypeArgv[32];
		IL2CPP_ASSERT(gi->type_argc <= 32);
		bool change = false;
		for (uint32_t i = 0; i < gi->type_argc; i++)
		{
			const Il2CppType* argType = gi->type_argv[i];
			const Il2CppType* dhArgType = TranslateReverseGenericShareIl2CppTypeFromDHE(argType);
			change = change || (argType != dhArgType);
			tempTypeArgv[i] = dhArgType;
		}
		if (!change)
		{
			return gi;
		}
		return il2cpp::vm::MetadataCache::GetGenericInst(tempTypeArgv, gi->type_argc);
	}

	void DifferentialHybridImage::BuildIl2CppImage(Il2CppImage* image, Il2CppAssembly* originAssembly)
	{
		InterpreterImage::BuildIl2CppImage(image);
		image->assembly->originAssembly = originAssembly;
		_originAssembly = originAssembly;
		_originImage = originAssembly->image;
	}

	void DifferentialHybridImage::InitTypeMapping(TypeMapping& type, il2cpp::utils::dynamic_array<bool>& typeInitFlags)
	{
		uint32_t typeIndex = (uint32_t)(&type - _typeMappings.data());
		if (typeInitFlags[typeIndex])
		{
			return;
		}
		typeInitFlags[typeIndex] = true;
		const Il2CppTypeDefinition* interpType = &_typesDefines[typeIndex];
		const char* name =_rawImage->GetStringFromRawIndex(DecodeMetadataIndex(interpType->nameIndex));
		if (interpType->declaringTypeIndex != kTypeDefinitionIndexInvalid)
		{
			const Il2CppTypeDefinition* declaringTypeDef = (const Il2CppTypeDefinition*)GetIl2CppTypeFromRawIndex(DecodeMetadataIndex(interpType->declaringTypeIndex))->data.typeHandle;
			uint32_t declaringTypeIndex = GetTypeRawIndex(declaringTypeDef);
			TypeMapping& declaringTm = _typeMappings[declaringTypeIndex];
			InitTypeMapping(declaringTm, typeInitFlags);
			if (declaringTm.aotType == nullptr)
			{
				return;
			}
			void* iter = nullptr;
			for (const Il2CppTypeDefinition* nextTypeDef; (nextTypeDef = (const Il2CppTypeDefinition*)il2cpp::vm::GlobalMetadata::GetNestedTypes((Il2CppMetadataTypeHandle)declaringTm.aotType, &iter));)
			{
				const char* nestedTypeName = il2cpp::vm::GlobalMetadata::GetStringFromIndex(nextTypeDef->nameIndex);
				IL2CPP_ASSERT(nestedTypeName);
				if (!std::strcmp(name, nestedTypeName))
				{
					type.aotType = nextTypeDef;
					break;
				}
			}
		}
		else
		{
			const char* namespaze = il2cpp::vm::GlobalMetadata::GetStringFromIndex(interpType->namespaceIndex);
			const Il2CppTypeDefinition* typeDef = (const Il2CppTypeDefinition*)il2cpp::vm::Image::TypeHandleFromName(_originImage, namespaze, name);
			type.aotType = typeDef;
		}
	}

	void DifferentialHybridImage::InitTypeMappings()
	{
		_typeMappings.resize_initialized(_typeDetails.size());

		il2cpp::utils::dynamic_array<bool> typeInitFlags(_typeMappings.size(), false);
		uint32_t maxAotTypeToken = 0;
		for (TypeMapping& type : _typeMappings)
		{
			InitTypeMapping(type, typeInitFlags);
			if (type.aotType)
			{
				maxAotTypeToken = std::max(maxAotTypeToken, type.aotType->token);
			}
		}

		uint32_t maxAotTypeCount = DecodeTokenRowIndex(maxAotTypeToken);
		_originType2InterpreterTypes.resize_initialized(maxAotTypeCount);
		for (TypeMapping& type : _typeMappings)
		{
			if (type.aotType)
			{
				const Il2CppTypeDefinition* interpType = GetTypeDefinitionByTypeMapping(type);
				uint32_t aotTypeIndex = DecodeTokenRowIndex(type.aotType->token) - 1;
				_originType2InterpreterTypes[aotTypeIndex] = interpType;
			}
		}
	}

	Il2CppMetadataTypeHandle GetOriginAOTTypeHandle(Il2CppMetadataTypeHandle typeHandle)
	{
		const Il2CppTypeDefinition* typeDef = (const Il2CppTypeDefinition*)typeHandle;
		if (IsInterpreterType(typeDef))
		{
			InterpreterImage* image = MetadataModule::GetImage(typeDef);
			if (IsDifferentialHybridImage(image))
			{
				const Il2CppTypeDefinition* originTypeDef = ((DifferentialHybridImage*)image)->GetOriginType(typeDef);
				if (originTypeDef)
				{
					return (Il2CppMetadataTypeHandle)originTypeDef;
				}
			}
		}
		return typeHandle;
	}

	bool IsDHETypeEqual(const Il2CppType* interpType, const Il2CppType* aotType)
	{
		if (aotType->type != interpType->type)
		{
			return false;
		}

		if (aotType->byref != interpType->byref)
		{
			return false;
		}

		switch (aotType->type)
		{
		case IL2CPP_TYPE_VALUETYPE:
		case IL2CPP_TYPE_CLASS:
			return aotType->data.typeHandle == GetOriginAOTTypeHandle(interpType->data.typeHandle);
		case IL2CPP_TYPE_PTR:
		case IL2CPP_TYPE_SZARRAY:
			return IsDHETypeEqual(interpType->data.type, aotType->data.type);

		case IL2CPP_TYPE_ARRAY:
		{
			if (aotType->data.array->rank != interpType->data.array->rank)
			{
				return false;
			}
			return IsDHETypeEqual(interpType->data.array->etype, aotType->data.array->etype);
		}
		case IL2CPP_TYPE_GENERICINST:
		{
			const Il2CppGenericInst* i1 = aotType->data.generic_class->context.class_inst;
			const Il2CppGenericInst* i2 = interpType->data.generic_class->context.class_inst;

			// this happens when maximum generic recursion is hit
			if (i1 == NULL || i2 == NULL)
			{
				return i1 == i2;
			}

			if (i1->type_argc != i2->type_argc)
				return false;

			if (!IsDHETypeEqual(interpType->data.generic_class->type, aotType->data.generic_class->type))
				return false;

			/* FIXME: we should probably just compare the instance pointers directly.  */
			for (uint32_t i = 0; i < i1->type_argc; ++i)
			{
				if (!IsDHETypeEqual(i2->type_argv[i], i1->type_argv[i]))
				{
					return false;
				}
			}

			return true;
		}
		case IL2CPP_TYPE_VAR:
		case IL2CPP_TYPE_MVAR:
		{
			const Il2CppGenericParameter* interpGenericParam = (const Il2CppGenericParameter*)interpType->data.genericParameterHandle;
			const Il2CppGenericParameter* aotGenericParam = (const Il2CppGenericParameter*)aotType->data.genericParameterHandle;
			return interpGenericParam->num == aotGenericParam->num;
		}
		default:
			return true;
		}
		return false;
	}

	bool IsDHEMethodSigEqual(const Il2CppMethodDefinition* interpMethod, const Il2CppMethodDefinition* aotMethod)
	{
		if (hybridclr::metadata::IsInstanceMethod(interpMethod) != hybridclr::metadata::IsInstanceMethod(aotMethod))
		{
			return false;
		}
		if (interpMethod->parameterCount != aotMethod->parameterCount)
		{
			return false;
		}
		if (interpMethod->genericContainerIndex == kGenericContainerIndexInvalid)
		{
			if (aotMethod->genericContainerIndex != kGenericContainerIndexInvalid)
			{
				return false;
			}
		}
		else
		{
			if (aotMethod->genericContainerIndex == kGenericContainerIndexInvalid)
			{
				return false;
			}
			const Il2CppGenericContainer* interpMethodGenericContainer = (Il2CppGenericContainer*)il2cpp::vm::GlobalMetadata::GetGenericContainerFromIndex(interpMethod->genericContainerIndex);
			const Il2CppGenericContainer* aotMethodGenericContainer = (Il2CppGenericContainer*)il2cpp::vm::GlobalMetadata::GetGenericContainerFromIndex(aotMethod->genericContainerIndex);
			if (interpMethodGenericContainer->type_argc != aotMethodGenericContainer->type_argc)
			{
				return false;
			}
		}

		const Il2CppType* interpReturnType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(interpMethod->returnType);
		const Il2CppType* aotReturnType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(aotMethod->returnType);
		if (!IsDHETypeEqual(interpReturnType, aotReturnType))
		{
			return false;
		}
		for (uint32_t i = 0; i < interpMethod->parameterCount; i++)
		{
			const Il2CppParameterDefinition* interpParam = (const Il2CppParameterDefinition*)il2cpp::vm::GlobalMetadata::GetParameterDefinitionFromIndex(interpMethod, interpMethod->parameterStart + i);
			const Il2CppParameterDefinition* aotParam = (const Il2CppParameterDefinition*)il2cpp::vm::GlobalMetadata::GetParameterDefinitionFromIndex(aotMethod, aotMethod->parameterStart + i);
			const Il2CppType* interpParamType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(interpParam->typeIndex);
			const Il2CppType* aotParamType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(aotParam->typeIndex);
			if (!IsDHETypeEqual(interpParamType, aotParamType))
			{
				return false;
			}
		}
		return true;
	}

	const Il2CppMethodDefinition* FindMatchMethod(const Il2CppTypeDefinition* aotType, const Il2CppMethodDefinition* toMatchInterpMethod)
	{
		//const Il2CppGenericContainer* klassGenContainer = aotType ? GetGenericContainerFromIl2CppType(&klass1->byval_arg) : nullptr;
		const char* toMatchMethodName = il2cpp::vm::GlobalMetadata::GetStringFromIndex(toMatchInterpMethod->nameIndex);
		for (uint16_t i = 0; i < aotType->method_count; i++)
		{
			const Il2CppMethodDefinition* aotMethod = il2cpp::vm::GlobalMetadata::GetMethodDefinitionFromIndex(aotType->methodStart + i);
			const char* methodName1 = il2cpp::vm::GlobalMetadata::GetStringFromIndex(aotMethod->nameIndex);
			if (std::strcmp(toMatchMethodName, methodName1))
			{
				continue;
			}
			if (IsDHEMethodSigEqual(toMatchInterpMethod, aotMethod))
			{
				return aotMethod;
			}
		}
		return nullptr;
	}

	FieldIndex FindMatchFieldIndex(const Il2CppTypeDefinition* aotType, const FieldDetail& toMatchInterpField)
	{
		//const Il2CppGenericContainer* klassGenContainer = aotType ? GetGenericContainerFromIl2CppType(&klass1->byval_arg) : nullptr;
		const char* interpFieldName = il2cpp::vm::GlobalMetadata::GetStringFromIndex(toMatchInterpField.fieldDef.nameIndex);
		for (uint16_t i = 0; i < aotType->field_count; i++)
		{
			const Il2CppFieldDefinition* aotField = il2cpp::vm::GlobalMetadata::GetFieldDefinitionFromTypeDefAndFieldIndex(aotType, i);
			const char* aotFieldName = il2cpp::vm::GlobalMetadata::GetStringFromIndex(aotField->nameIndex);
			if (std::strcmp(interpFieldName, aotFieldName))
			{
				continue;
			}
			const Il2CppType* aotFieldType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(aotField->typeIndex);
			const Il2CppType* interpFieldType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(toMatchInterpField.fieldDef.typeIndex);
			if (IsDHETypeEqual(interpFieldType, aotFieldType))
			{
				return (FieldIndex)i;
			}
		}
		return kFieldIndexInvalid;
	}


	void DifferentialHybridImage::InitTypeMethodsMapping(TypeMapping& tm)
	{
		if (tm.initedMethods)
		{
			return;
		}
		tm.initedMethods = true;
		if (!tm.aotType)
		{
			return;
		}
		const Il2CppTypeDefinition* interpType = GetTypeDefinitionByTypeMapping(tm);

		tm.aotMethodOfInterpMethods = (const Il2CppMethodDefinition**)HYBRIDCLR_METADATA_CALLOC(sizeof(const Il2CppMethodDefinition*), interpType->method_count);
		tm.interpMethodOfAotMethods = (const Il2CppMethodDefinition**)HYBRIDCLR_METADATA_CALLOC(sizeof(const Il2CppMethodDefinition*), tm.aotType->method_count);


		int32_t methodStartIndex = DecodeMetadataIndex(interpType->methodStart);
		for (int32_t i = 0; i < interpType->method_count; i++)
		{
			Il2CppMethodDefinition* methodDef = &_methodDefines[methodStartIndex + i];
			if (methodDef->declaringType == kTypeDefinitionIndexInvalid)
			{
				InitMethodDefsOfTypeDef(const_cast<Il2CppTypeDefinition&>(*interpType));
				IL2CPP_ASSERT(methodDef->declaringType != kTypeDefinitionIndexInvalid);
			}

			const Il2CppMethodDefinition* originMethod = FindMatchMethod(tm.aotType, methodDef);
			if (originMethod)
			{
				if (_changedMethodTokens.find(methodDef->token) == _changedMethodTokens.end())
				{
					tm.aotMethodOfInterpMethods[i] = originMethod;
				}
				int32_t aotMethodIndex = il2cpp::vm::GlobalMetadata::GetMethodIndexFromDefinition(originMethod) - tm.aotType->methodStart;
				IL2CPP_ASSERT(aotMethodIndex >= 0 && aotMethodIndex < tm.aotType->method_count);
				tm.interpMethodOfAotMethods[aotMethodIndex] = methodDef;
			}
		}

	}
	const Il2CppMethodDefinition* DifferentialHybridImage::GetInterpreterMethodDefinitionByOriginMethodDefinition(const Il2CppMethodDefinition* methodDef)
	{
		const Il2CppTypeDefinition* aotTypeDef = (const Il2CppTypeDefinition*)il2cpp::vm::GlobalMetadata::GetTypeHandleFromIndex(methodDef->declaringType);
		const Il2CppTypeDefinition* interpTypeDef = GetInterpreterTypeByOriginType(aotTypeDef);
		if (!interpTypeDef)
		{
			return nullptr;
		}
		int32_t typeIndex = GetTypeRawIndex(interpTypeDef);
		TypeMapping& tm = _typeMappings[typeIndex];

		il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
		if (!tm.initedMethods)
		{
			InitTypeMethodsMapping(tm);
		}
		if (!tm.interpMethodOfAotMethods)
		{
			return nullptr;
		}
		MethodIndex aotMethodIndex = il2cpp::vm::GlobalMetadata::GetMethodIndexFromDefinition(methodDef);
		IL2CPP_ASSERT(aotMethodIndex >= aotTypeDef->methodStart && aotMethodIndex < aotTypeDef->methodStart + aotTypeDef->method_count);
		return tm.interpMethodOfAotMethods[aotMethodIndex - aotTypeDef->methodStart];
	}

	const Il2CppMethodDefinition* DifferentialHybridImage::GetUnchangedOriginMethodDefintionByInterpreterMethod(const Il2CppMethodDefinition* methodDef)
	{
		int32_t typeIndex = DecodeMetadataIndex(methodDef->declaringType);
		TypeMapping& tm = _typeMappings[typeIndex];
		il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
		if (!tm.initedMethods)
		{
			InitTypeMethodsMapping(tm);
		}
		if (!tm.aotMethodOfInterpMethods)
		{
			return nullptr;
		}
		Il2CppTypeDefinition* interpTypeDef = &_typesDefines[typeIndex];

		int32_t methodIndex = GetMethodDefinitionRawIndex(methodDef);
		int32_t methodIndexInType = methodIndex - DecodeMetadataIndex(interpTypeDef->methodStart);
		IL2CPP_ASSERT(methodIndexInType >= 0 && methodIndexInType < interpTypeDef->method_count);
		return tm.aotMethodOfInterpMethods[methodIndexInType];
	}

	FieldIndex DifferentialHybridImage::GetInterpreterFieldIndexByOriginFieldIndex(const Il2CppTypeDefinition* interpType, FieldIndex index)
	{
		int32_t typeIndex = GetTypeRawIndex(interpType);
		TypeMapping& tm = _typeMappings[typeIndex];

		const Il2CppTypeDefinition* aotType = tm.aotType;

		if (!aotType)
		{
			return kFieldIndexInvalid;
		}

		IL2CPP_ASSERT(index < aotType->field_count);


		int32_t fieldStartIndex = DecodeMetadataIndex(interpType->fieldStart);

		const Il2CppFieldDefinition* aotField = il2cpp::vm::GlobalMetadata::GetFieldDefinitionFromTypeDefAndFieldIndex(aotType, index);
		const char* aotFieldName = il2cpp::vm::GlobalMetadata::GetStringFromIndex(aotField->nameIndex);
		for (int32_t i = fieldStartIndex, n = fieldStartIndex + interpType->field_count; i < n; i++)
		{
			const Il2CppFieldDefinition& interpFieldDef = _fieldDetails[i].fieldDef;

			const char* interpFieldName = il2cpp::vm::GlobalMetadata::GetStringFromIndex(interpFieldDef.nameIndex);
			if (std::strcmp(interpFieldName, aotFieldName))
			{
				continue;
			}
			const Il2CppType* aotFieldType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(aotField->typeIndex);
			const Il2CppType* interpFieldType = il2cpp::vm::GlobalMetadata::GetIl2CppTypeFromIndex(interpFieldDef.typeIndex);
			return IsDHETypeEqual(interpFieldType, aotFieldType) ? (FieldIndex)(i - fieldStartIndex) : kFieldIndexInvalid;
		}
		return kFieldIndexInvalid;
	}

	void DifferentialHybridImage::PostInitPhase1()
	{
		InitTypeMappings();
	}

	Il2CppMethodPointer DifferentialHybridImage::TryGetMethodPointer(const Il2CppMethodDefinition* method)
	{
		const Il2CppMethodDefinition* aotMethod = GetUnchangedOriginMethodDefintionByInterpreterMethod(method);
		return aotMethod ? il2cpp::vm::MetadataCache::GetMethodPointer(_originImage, aotMethod->token) : nullptr;
	}

	Il2CppMethodPointer DifferentialHybridImage::TryGetMethodPointer(const MethodInfo* method)
	{
		if (!method->is_inflated)
		{
			if (!method->methodMetadataHandle)
			{
				return nullptr;
			}
			return TryGetMethodPointer((const Il2CppMethodDefinition*)method->methodMetadataHandle);
		}

		const Il2CppMethodDefinition* aotMethodDef = GetUnchangedOriginMethodDefintionByInterpreterMethod((Il2CppMethodDefinition*)method->genericMethod->methodDefinition->methodMetadataHandle);
		if (!aotMethodDef)
		{
			return nullptr;
		}
		const MethodInfo* aotMethod = il2cpp::vm::GlobalMetadata::GetMethodInfoFromMethodHandle((Il2CppMetadataMethodDefinitionHandle)aotMethodDef);
		const Il2CppGenericContext& originCtx = method->genericMethod->context;
		Il2CppGenericContext reverseCtx = { TranslateReverseGenericShareGenericInstFromDHE(originCtx.class_inst), TranslateReverseGenericShareGenericInstFromDHE(originCtx.method_inst) };
#if HYBRIDCLR_UNITY_2021_OR_NEW
		return il2cpp::vm::MetadataCache::GetGenericMethodPointers(aotMethod, &reverseCtx).methodPointer;
#else
		return il2cpp::vm::MetadataCache::GetMethodPointer(aotMethod, &reverseCtx, false, true);
#endif
	}

	Il2CppMethodPointer DifferentialHybridImage::TryGetAdjustThunkMethodPointer(const Il2CppMethodDefinition* method)
	{
		const Il2CppMethodDefinition* aotMethodDef = GetUnchangedOriginMethodDefintionByInterpreterMethod(method);
		if (!aotMethodDef)
		{
			return nullptr;
		}
		const TypeMapping& tm = _typeMappings[DecodeMetadataIndex(method->declaringType)];
		return IsValueType(tm.aotType) ? il2cpp::vm::MetadataCache::GetAdjustorThunk(_originImage, aotMethodDef->token) : nullptr;
	}

	Il2CppMethodPointer DifferentialHybridImage::TryGetAdjustThunkMethodPointer(const MethodInfo* method)
	{
		if (!method->is_inflated)
		{
			if (!method->methodMetadataHandle)
			{
				return nullptr;
			}
			return TryGetAdjustThunkMethodPointer((const Il2CppMethodDefinition*)method->methodMetadataHandle);
		}
		const Il2CppMethodDefinition* aotMethodDef = GetUnchangedOriginMethodDefintionByInterpreterMethod((Il2CppMethodDefinition*)method->genericMethod->methodDefinition->methodMetadataHandle);
		if (!aotMethodDef)
		{
			return nullptr;
		}
		const MethodInfo* aotMethod = il2cpp::vm::GlobalMetadata::GetMethodInfoFromMethodHandle((Il2CppMetadataMethodDefinitionHandle)aotMethodDef);
		const Il2CppGenericContext& originCtx = method->genericMethod->context;
		Il2CppGenericContext reverseCtx = { TranslateReverseGenericShareGenericInstFromDHE(originCtx.class_inst), TranslateReverseGenericShareGenericInstFromDHE(originCtx.method_inst) };
#if HYBRIDCLR_UNITY_2021_OR_NEW
		return il2cpp::vm::MetadataCache::GetGenericMethodPointers(aotMethod, &reverseCtx).virtualMethodPointer;
#else
		return il2cpp::vm::MetadataCache::GetMethodPointer(aotMethod, &reverseCtx, true, false);
#endif
	}

	InvokerMethod DifferentialHybridImage::TryGetMethodInvoker(const Il2CppMethodDefinition* method)
	{
		const Il2CppMethodDefinition* aotMethodDef = GetUnchangedOriginMethodDefintionByInterpreterMethod(method);
		return aotMethodDef ? il2cpp::vm::MetadataCache::GetMethodInvoker(_originImage, aotMethodDef->token) : nullptr;
	}

	InvokerMethod DifferentialHybridImage::TryGetMethodInvoker(const MethodInfo* method)
	{
		if (!method->is_inflated)
		{
			if (!method->methodMetadataHandle)
			{
				return nullptr;
			}
			return TryGetMethodInvoker((const Il2CppMethodDefinition*)method->methodMetadataHandle);
		}
		const Il2CppMethodDefinition* aotMethodDef = GetUnchangedOriginMethodDefintionByInterpreterMethod((Il2CppMethodDefinition*)method->genericMethod->methodDefinition->methodMetadataHandle);
		if (!aotMethodDef)
		{
			return nullptr;
		}
		const MethodInfo* aotMethod = il2cpp::vm::GlobalMetadata::GetMethodInfoFromMethodHandle((Il2CppMetadataMethodDefinitionHandle)aotMethodDef);
		const Il2CppGenericContext& originCtx = method->genericMethod->context;
		Il2CppGenericContext reverseCtx = { TranslateReverseGenericShareGenericInstFromDHE(originCtx.class_inst), TranslateReverseGenericShareGenericInstFromDHE(originCtx.method_inst) };
#if HYBRIDCLR_UNITY_2021_OR_NEW
		return il2cpp::vm::MetadataCache::GetGenericMethodPointers(aotMethod, &reverseCtx).invoker_method;
#else
		return il2cpp::vm::MetadataCache::GetInvokerMethodPointer(aotMethod, &reverseCtx);
#endif
	}
}
}

