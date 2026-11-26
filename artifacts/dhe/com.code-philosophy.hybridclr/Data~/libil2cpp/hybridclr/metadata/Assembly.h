#pragma once

#include "../CommonDef.h"
#include "RawImage.h"
#include "AOTHomologousImage.h"

namespace hybridclr
{
namespace metadata
{

    class Assembly
    {
    public:
        static void InitializePlaceHolderAssemblies();
        static Il2CppAssembly* LoadFromBytes(const void* assemblyData, uint64_t length, const void* rawSymbolStoreBytes, uint64_t rawSymbolStoreLength);
        static LoadImageErrorCode LoadMetadataForAOTAssembly(const void* dllBytes, uint32_t dllSize, HomologousImageMode mode);
        static void HotfixAssembly(const Il2CppAssembly* targetAssembly, Il2CppArray* hotfixAssemblyBytes, Il2CppArray* hotfixMethodTokens);
        static void InitializeDifferentialHybridAssembles();
        static bool IsDifferentialHybridAssembly(const char* assemblyName);
        static LoadImageErrorCode LoadDifferentialHybridAssemblyWithDHAO(const void* dllBytes, uint32_t dllLength, const void* symbolBytes, uint32_t symbolLength,
            const void* dhaoBytes, uint32_t dhaoLength);
        static LoadImageErrorCode LoadDifferentialHybridAssemblyWithMetaVersion(const void* dllBytes, uint32_t dllLength, const void* symbolBytes, uint32_t symbolLength,
            const void* originalMetaVersionBytes, uint32_t originalMetaVersionLength, const void* currentMetaVersionBytes, uint32_t currentMetaVersionLength);
        static LoadImageErrorCode LoadOriginalDifferentialHybridAssembly(const char* assemblyName);
    private:
        static Il2CppAssembly* Create(const byte* assemblyData, uint64_t length, const byte* rawSymbolStoreBytes, uint64_t rawSymbolStoreLength);
    };
}
}