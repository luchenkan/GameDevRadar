#include "RuntimeApi.h"

#include "vm/Runtime.h"

#include "vm/InternalCalls.h"
#include "vm/Array.h"
#include "vm/Exception.h"
#include "vm/Class.h"

#include "codegen/il2cpp-codegen.h"
#include "metadata/MetadataModule.h"
#include "metadata/MetadataUtil.h"
#include "interpreter/InterpreterModule.h"
#include "RuntimeConfig.h"
#include "metadata/Assembly.h"

namespace hybridclr
{
	void RuntimeApi::RegisterInternalCalls()
	{
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::LoadMetadataForAOTAssembly(System.Byte[],HybridCLR.HomologousImageMode)", (Il2CppMethodPointer)LoadMetadataForAOTAssembly);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::GetRuntimeOption(HybridCLR.RuntimeOptionId)", (Il2CppMethodPointer)GetRuntimeOption);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::SetRuntimeOption(HybridCLR.RuntimeOptionId,System.Int32)", (Il2CppMethodPointer)SetRuntimeOption);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::PreJitClass(System.Type)", (Il2CppMethodPointer)PreJitClass);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::PreJitMethod(System.Reflection.MethodInfo)", (Il2CppMethodPointer)PreJitMethod);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::IsFullGenericSharingEnabled()", (Il2CppMethodPointer)IsFullGenericSharingEnabled);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::HotfixAssembly(System.Reflection.Assembly,System.Byte[],System.Int32[])", (Il2CppMethodPointer)HotfixAssembly);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::LoadDifferentialHybridAssemblyWithDHAOImpl(System.Byte[],System.Byte[],System.Byte[])", (Il2CppMethodPointer)LoadDifferentialHybridAssemblyWithDHAO);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::LoadDifferentialHybridAssemblyWithMetaVersionImpl(System.Byte[],System.Byte[],System.Byte[],System.Byte[])", (Il2CppMethodPointer)LoadDifferentialHybridAssemblyWithMetaVersion);
		il2cpp::vm::InternalCalls::Add("HybridCLR.RuntimeApi::LoadOriginalDifferentialHybridAssembly(System.String)", (Il2CppMethodPointer)LoadOriginalDifferentialHybridAssembly);
	}

	int32_t RuntimeApi::LoadMetadataForAOTAssembly(Il2CppArray* dllBytes, int32_t mode)
	{
		if (!dllBytes)
		{
			il2cpp::vm::Exception::RaiseNullReferenceException();
		}
		return (int32_t)hybridclr::metadata::Assembly::LoadMetadataForAOTAssembly(il2cpp::vm::Array::GetFirstElementAddress(dllBytes), il2cpp::vm::Array::GetByteLength(dllBytes), (hybridclr::metadata::HomologousImageMode)mode);
	}

	int32_t RuntimeApi::GetRuntimeOption(int32_t optionId)
	{
		return hybridclr::RuntimeConfig::GetRuntimeOption((hybridclr::RuntimeOptionId)optionId);
	}

	void RuntimeApi::SetRuntimeOption(int32_t optionId, int32_t value)
	{
		hybridclr::RuntimeConfig::SetRuntimeOption((hybridclr::RuntimeOptionId)optionId, value);
	}

	bool RuntimeApi::IsFullGenericSharingEnabled()
	{
#if !HYBRIDCLR_UNITY_2021_OR_NEW
		return false;
#elif HYBRIDCLR_UNITY_2021
		return il2cpp::vm::Runtime::IsFullGenericSharingEnabled();
#else
		return true;
#endif
	}

	int32_t RuntimeApi::LoadDifferentialHybridAssemblyWithDHAO(Il2CppArray* dllData, Il2CppArray* symbolData, Il2CppArray* dhaoData)
	{
		if (!dllData)
		{
			il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentNullException("dll bytes is null"));
		}
		if (!dhaoData)
		{
			il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentNullException("dhaoData is null"));
		}
		char* symbolDataPtr = symbolData ? il2cpp::vm::Array::GetFirstElementAddress(symbolData) : nullptr;
		uint32_t symbolDataLength = symbolData ? il2cpp::vm::Array::GetByteLength(symbolData) : 0;
		return (int32_t)hybridclr::metadata::Assembly::LoadDifferentialHybridAssemblyWithDHAO(
			il2cpp::vm::Array::GetFirstElementAddress(dllData),
			il2cpp::vm::Array::GetByteLength(dllData),
			symbolDataPtr,
			symbolDataLength,
			il2cpp::vm::Array::GetFirstElementAddress(dhaoData),
			il2cpp::vm::Array::GetByteLength(dhaoData));

	}

	int32_t RuntimeApi::LoadDifferentialHybridAssemblyWithMetaVersion(Il2CppArray* dllData, Il2CppArray* symbolData, Il2CppArray* originalMetaVersionData, Il2CppArray* currentMetaVersionData)
	{
		if (!dllData)
		{
			il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentNullException("dll bytes is null"));
		}
		if (!originalMetaVersionData)
		{
			il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentNullException("originalMetaVersionData is null"));
		}
		if (!currentMetaVersionData)
		{
			il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentNullException("currentMetaVersionData is null"));
		}
		char* symbolDataPtr = symbolData ? il2cpp::vm::Array::GetFirstElementAddress(symbolData) : nullptr;
		uint32_t symbolDataLength = symbolData ? il2cpp::vm::Array::GetByteLength(symbolData) : 0;
		return (int32_t)hybridclr::metadata::Assembly::LoadDifferentialHybridAssemblyWithMetaVersion(
			il2cpp::vm::Array::GetFirstElementAddress(dllData),
			il2cpp::vm::Array::GetByteLength(dllData),
			symbolDataPtr,
			symbolDataLength,
			il2cpp::vm::Array::GetFirstElementAddress(originalMetaVersionData),
			il2cpp::vm::Array::GetByteLength(originalMetaVersionData),
			il2cpp::vm::Array::GetFirstElementAddress(currentMetaVersionData),
			il2cpp::vm::Array::GetByteLength(currentMetaVersionData));
	}

	int32_t RuntimeApi::LoadOriginalDifferentialHybridAssembly(Il2CppString* assemblyName)
	{
		if (!assemblyName)
		{
			il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentNullException("assemblyName is null"));
		}
		std::string cAssName = il2cpp::utils::StringUtils::Utf16ToUtf8(assemblyName->chars, assemblyName->length);
		return (int32_t)hybridclr::metadata::Assembly::LoadOriginalDifferentialHybridAssembly(cAssName.c_str());
	}

	int32_t PreJitMethod0(const MethodInfo* methodInfo);

	int32_t RuntimeApi::PreJitClass(Il2CppReflectionType* type)
	{
		if (metadata::HasNotInstantiatedGenericType(type->type))
		{
			return false;
		}
		Il2CppClass* klass = il2cpp::vm::Class::FromIl2CppType(type->type, false);
		if (!klass)
		{
			return false;
		}
		metadata::Image* image = metadata::MetadataModule::GetImage(klass->image);
		if (!image)
		{
			image = (metadata::Image*)hybridclr::metadata::AOTHomologousImage::FindImageByAssembly(
				klass->rank ? il2cpp_defaults.corlib->assembly : klass->image->assembly);
			if (!image)
			{
				return false;
			}
		}
		for (uint16_t i = 0; i < klass->method_count; i++)
		{
			const MethodInfo* methodInfo = klass->methods[i];
			PreJitMethod0(methodInfo);
		}
		return true;
	}

	int32_t PreJitMethod0(const MethodInfo* methodInfo)
	{
		if (!methodInfo->isInterpterImpl)
		{
			return false;
		}
		if (methodInfo->klass->is_generic)
		{
			return false;
		}
		if (!methodInfo->is_inflated)
		{
			if (methodInfo->is_generic)
			{
				return false;
			}
		}
		else
		{
			const Il2CppGenericMethod* genericMethod = methodInfo->genericMethod;
			if (metadata::HasNotInstantiatedGenericType(genericMethod->context.class_inst) || metadata::HasNotInstantiatedGenericType(genericMethod->context.method_inst))
			{
				return false;
			}
		}

		return interpreter::InterpreterModule::GetInterpMethodInfo(methodInfo) != nullptr;
	}

	int32_t RuntimeApi::PreJitMethod(Il2CppReflectionMethod* method)
	{
		return PreJitMethod0(method->method);
	}

	void RuntimeApi::HotfixAssembly(Il2CppReflectionAssembly* targetAssembly, Il2CppArray* hotfixAssemblyBytes, Il2CppArray* hotfixMethodTokens)
	{
		if (!targetAssembly || !hotfixAssemblyBytes || !hotfixMethodTokens)
		{
			il2cpp::vm::Exception::RaiseNullReferenceException();
		}
		hybridclr::metadata::Assembly::HotfixAssembly(targetAssembly->assembly, hotfixAssemblyBytes, hotfixMethodTokens);
	}
}
