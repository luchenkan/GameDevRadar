#pragma once

#include <stdint.h>
#include "CommonDef.h"

namespace hybridclr
{
	class RuntimeApi
	{
	public:
		static void RegisterInternalCalls();

		static int32_t LoadMetadataForAOTAssembly(Il2CppArray* dllData, int32_t mode);

		static int32_t GetRuntimeOption(int32_t optionId);
		static void SetRuntimeOption(int32_t optionId, int32_t value);

		static int32_t PreJitClass(Il2CppReflectionType* type);
		static int32_t PreJitMethod(Il2CppReflectionMethod* method);

		static bool IsFullGenericSharingEnabled();

		static void HotfixAssembly(Il2CppReflectionAssembly* targetAssembly, Il2CppArray* hotfixAssemblyBytes, Il2CppArray* hotfixMethodTokens);
		static int32_t LoadDifferentialHybridAssemblyWithDHAO(Il2CppArray* dllData, Il2CppArray* symbolData, Il2CppArray* dhaoData);
		static int32_t LoadDifferentialHybridAssemblyWithMetaVersion(Il2CppArray* dllData, Il2CppArray* symbolData, Il2CppArray* originalMetaVersionData, Il2CppArray* currentMetaVersionData);
		static int32_t LoadOriginalDifferentialHybridAssembly(Il2CppString* assemblyName);

	};
}