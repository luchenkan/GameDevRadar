
#include "Assembly.h"

#include <cstring>
#include <iostream>
#include <vector>

#include "os/File.h"
#include "utils/MemoryMappedFile.h"
#include "vm/Assembly.h"
#include "vm/Image.h"
#include "vm/Class.h"
#include "vm/String.h"
#include "vm/MetadataLock.h"
#include "vm/MetadataCache.h"
#include "vm/GlobalMetadata.h"
#include "vm/GenericClass.h"
#include "metadata/ArrayMetadata.h"

#include "Image.h"
#include "MetadataModule.h"
#include "MetadataUtil.h"
#include "MetadataDef.h"
#include "BlobReader.h"
#include "DifferentialHybridImage.h"
#include "ConsistentAOTHomologousImage.h"
#include "SuperSetAOTHomologousImage.h"
#include "HotfixImage.h"

namespace hybridclr
{
namespace metadata
{

    std::vector<Il2CppAssembly*> s_placeHolderAssembies;

#if ENABLE_PLACEHOLDER_DLL == 1

    static const char* CreateAssemblyNameWithoutExt(const char* assemblyName)
    {
        const char* extStr = std::strstr(assemblyName, ".dll");
        if (extStr)
        {
            size_t nameLen = extStr - assemblyName;
            char* name = (char*)HYBRIDCLR_MALLOC(nameLen + 1);
            std::strncpy(name, assemblyName, nameLen);
            name[nameLen] = '\0';
            return name;
        }
        else
        {
            return CopyString(assemblyName);
        }
    }

    static Il2CppAssembly* FindPlaceHolderAssemblyLocked(const char* assemblyNameNoExt, il2cpp::os::FastAutoLock& lock)
    {
        for (Il2CppAssembly* ass : s_placeHolderAssembies)
        {
            if (std::strcmp(ass->image->nameNoExt, assemblyNameNoExt) == 0)
            {
                return ass;
            }
        }
        return nullptr;
    }
#else
    static Il2CppAssembly* FindPlaceHolderAssemblyLocked(const char* assemblyNameNoExt, il2cpp::os::FastAutoLock& lock)
    {
        return nullptr;
    }
#endif


    static Il2CppAssembly* CreatePlaceHolderAssembly(const char* assemblyName)
    {
        auto ass = new (HYBRIDCLR_MALLOC_ZERO(sizeof(Il2CppAssembly))) Il2CppAssembly;
        auto image2 = new (HYBRIDCLR_MALLOC_ZERO(sizeof(Il2CppImage))) Il2CppImage;
        ass->image = image2;
        ass->image->name = CopyString(assemblyName);
        ass->image->nameNoExt = ass->aname.name = CreateAssemblyNameWithoutExt(assemblyName);
        image2->assembly = ass;
        return ass;
    }

    void Assembly::InitializePlaceHolderAssemblies()
    {
        for (const char** ptrPlaceHolderName = g_placeHolderAssemblies; *ptrPlaceHolderName; ++ptrPlaceHolderName)
        {
            const char* nameWithExtension = ConcatNewString(*ptrPlaceHolderName, ".dll");
            Il2CppAssembly* placeHolderAss = CreatePlaceHolderAssembly(nameWithExtension);
            s_placeHolderAssembies.push_back(placeHolderAss);
            HYBRIDCLR_FREE((void*)nameWithExtension);
            il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(placeHolderAss);
        }
    }

    static void RunModuleInitializer(Il2CppImage* image)
    {
        Il2CppClass* moduleKlass = il2cpp::vm::Image::ClassFromName(image, "", "<Module>");
        if (!moduleKlass)
        {
            return;
        }
        il2cpp::vm::Runtime::ClassInit(moduleKlass);
    }

    Il2CppAssembly* Assembly::LoadFromBytes(const void* assemblyData, uint64_t length, const void* rawSymbolStoreBytes, uint64_t rawSymbolStoreLength)
    {
        Il2CppAssembly* ass = Create((const byte*)assemblyData, length, (const byte*)rawSymbolStoreBytes, rawSymbolStoreLength);
        RunModuleInitializer(ass->image);
        return ass;
    }

    Il2CppAssembly* Assembly::Create(const byte* assemblyData, uint64_t length, const byte* rawSymbolStoreBytes, uint64_t rawSymbolStoreLength)
    {
        il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);

        if (!assemblyData)
        {
            il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentNullException("rawAssembly is null"));
        }

        uint32_t imageId = InterpreterImage::AllocImageIndex((uint32_t)length);
        if (imageId == kInvalidImageIndex)
        {
            il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetExecutionEngineException("InterpreterImage::AllocImageIndex failed"));
        }
        InterpreterImage* image = new InterpreterImage(imageId);
        
        assemblyData = (const byte*)CopyBytes(assemblyData, length);
        LoadImageErrorCode err = image->Load(assemblyData, (size_t)length);

        if (err != LoadImageErrorCode::OK)
        {
            TEMP_FORMAT(errMsg, "LoadImageErrorCode:%d", (int)err);
            il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetBadImageFormatException(errMsg));
            // when load a bad image, mean a fatal error. we don't clean image on purpose.
        }

        if (rawSymbolStoreBytes)
        {
            rawSymbolStoreBytes = (const byte*)CopyBytes(rawSymbolStoreBytes, rawSymbolStoreLength);
            err = image->LoadPDB(rawSymbolStoreBytes, (size_t)rawSymbolStoreLength);
            if (err != LoadImageErrorCode::OK)
            {
                TEMP_FORMAT(errMsg, "LoadPDB Error:%d", (int)err);
                il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetBadImageFormatException(errMsg));
            }
        }

        TbAssembly data = image->GetRawImage().ReadAssembly(1);
        const char* nameNoExt = image->GetStringFromRawIndex(data.name);

        Il2CppAssembly* ass;
        Il2CppImage* image2;
        if ((ass = FindPlaceHolderAssemblyLocked(nameNoExt, lock)) != nullptr)
        {
            if (ass->token)
            {
                RaiseExecutionEngineException("reloading placeholder assembly is not supported!");
            }
            image2 = ass->image;
            HYBRIDCLR_FREE((void*)ass->image->name);
            HYBRIDCLR_FREE((void*)ass->image->nameNoExt);
        }
        else
        {
            ass = new (HYBRIDCLR_MALLOC_ZERO(sizeof(Il2CppAssembly))) Il2CppAssembly;
            image2 = new (HYBRIDCLR_MALLOC_ZERO(sizeof(Il2CppImage))) Il2CppImage;
        }

        image->InitBasic(image2);
        image->BuildIl2CppAssembly(ass);
        ass->image = image2;

        image->BuildIl2CppImage(image2);
        image2->name = ConcatNewString(ass->aname.name, ".dll");
        image2->nameNoExt = ass->aname.name;
        image2->assembly = ass;

        image->InitRuntimeMetadatas();

        il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(ass);
        return ass;
    }

    LoadImageErrorCode Assembly::LoadMetadataForAOTAssembly(const void* dllBytes, uint32_t dllSize, HomologousImageMode mode)
    {
        il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);

        AOTHomologousImage* image = nullptr;
        switch (mode)
        {
        case HomologousImageMode::CONSISTENT: image = new ConsistentAOTHomologousImage(); break;
        case HomologousImageMode::SUPERSET: image = new SuperSetAOTHomologousImage(); break;
        default: return LoadImageErrorCode::INVALID_HOMOLOGOUS_MODE;
        }

        LoadImageErrorCode err = image->Load((byte*)CopyBytes(dllBytes, dllSize), dllSize);
        if (err != LoadImageErrorCode::OK)
        {
            delete image;
            return err;
        }

        RawImageBase* rawImage = &image->GetRawImage();
        TbAssembly data = rawImage->ReadAssembly(1);
        const char* assName = rawImage->GetStringFromRawIndex(data.name);
        const Il2CppAssembly* aotAss = il2cpp::vm::Assembly::GetLoadedAssembly(assName);
        // FIXME. not free memory.
        if (!aotAss)
        {
            delete image;
            return LoadImageErrorCode::AOT_ASSEMBLY_NOT_FIND;
        }
        if (hybridclr::metadata::IsInterpreterImage(aotAss->image))
        {
            delete image;
            return LoadImageErrorCode::HOMOLOGOUS_ONLY_SUPPORT_AOT_ASSEMBLY;
        }
        image->SetTargetAssembly(aotAss);
        if (AOTHomologousImage::FindImageByAssemblyLocked(image->GetTargetAssembly(), lock))
        {
            return LoadImageErrorCode::HOMOLOGOUS_ASSEMBLY_HAS_BEEN_LOADED;
        }
        image->InitRuntimeMetadatas();
        AOTHomologousImage::RegisterLocked(image, lock);
        return LoadImageErrorCode::OK;
    }

    void Assembly::HotfixAssembly(const Il2CppAssembly* targetAssembly, Il2CppArray* hotfixAssemblyBytes, Il2CppArray* hotfixMethodTokens)
    {
        il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
        if (!hybridclr::metadata::IsInterpreterImage(targetAssembly->image) && !IsDifferentialHybridAssembly(targetAssembly->image->nameNoExt))
        {
            RaiseExecutionEngineException("HotfixAssembly only support interpreter image or DHE image.");
        }

        uint32_t hotfixAssemblySize = il2cpp::vm::Array::GetLength(hotfixAssemblyBytes);
        byte* hotfixAssemblyData = (byte*)CopyBytes(il2cpp::vm::Array::GetFirstElementAddress(hotfixAssemblyBytes), hotfixAssemblySize);

        uint32_t* hotfixMethodTokensData = (uint32_t*)il2cpp::vm::Array::GetFirstElementAddress(hotfixMethodTokens);
        uint32_t methodCount = il2cpp::vm::Array::GetLength(hotfixMethodTokens);
        std::vector<uint32_t> methodTokens(hotfixMethodTokensData, hotfixMethodTokensData + methodCount);

        HotfixImage* image = new HotfixImage(methodTokens);
        LoadImageErrorCode err = image->Load(hotfixAssemblyData, hotfixAssemblySize);
        if (err != LoadImageErrorCode::OK)
        {
            delete image;
            TEMP_FORMAT(errMsg, "HotfixAssembly load failed. %d", err);
            RaiseExecutionEngineException(errMsg);
        }
        RawImageBase* rawImage = &image->GetRawImage();
        TbModule data = rawImage->ReadModule(1);
        const char* assNameWithExt = rawImage->GetStringFromRawIndex(data.name);
        if (std::strcmp(assNameWithExt, targetAssembly->image->name))
        {
            delete image;
            TEMP_FORMAT(errMsg, "HotfixAssembly assembly name not match. targetAssembly:%s hotfixAssembly:%s", targetAssembly->image->name, assNameWithExt);
            RaiseExecutionEngineException(errMsg);
        }
        image->SetTargetAssembly(targetAssembly);
        image->InitRuntimeMetadatas();
        HotfixImage::RegisteHotfixImage(targetAssembly, image);
        image->HotfixMethods();
    }

    bool Assembly::IsDifferentialHybridAssembly(const char* assemblyName)
    {

        for (const char** ptrAotAss = g_differentialHybridAssemblies; *ptrAotAss; ptrAotAss++)
        {
            if (std::strcmp(*ptrAotAss, assemblyName) == 0)
            {
                return true;
            }
        }
        return false;
    }


    void Assembly::InitializeDifferentialHybridAssembles()
    {
        il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);

        std::vector<Il2CppAssembly*> dheAssemblies;
        for (const char** ptrAotAss = g_differentialHybridAssemblies; *ptrAotAss; ptrAotAss++)
        {
            Il2CppAssembly* aotAss = const_cast<Il2CppAssembly*>(il2cpp::vm::Assembly::GetOriginDifferentialHybridAssembly(*ptrAotAss));
            if (!aotAss)
            {
                continue;
            }
            Il2CppAssembly* dheAss = FindPlaceHolderAssemblyLocked(*ptrAotAss, lock);
            dheAssemblies.push_back(dheAss);
            aotAss->dheAssembly = dheAss;
            aotAss->originAssembly = nullptr;
            dheAss->originAssembly = aotAss;
            dheAss->dheAssembly = nullptr;
        }
        //for (Il2CppAssembly* ass : dheAssemblies)
        //{
        //    il2cpp::vm::Assembly::Register(ass);
        //}
    }



    LoadImageErrorCode LoadDifferentialHybridAssembly(const void* dllBytes, uint32_t dllLength, const void* symbolBytes, uint32_t symbolLength,
        const il2cpp::utils::dynamic_array<uint32_t>& changedClassTokens, const il2cpp::utils::dynamic_array<uint32_t>& changedMethodTokens)
    {
        il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);

        uint32_t imageId = InterpreterImage::AllocImageIndex(dllLength);
        if (imageId > kMaxMetadataImageCount)
        {
            il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetArgumentException("[Assembly::LoadDifferentialHybridAssembly] exceed max image index", ""));
        }

        dllBytes = (const byte*)CopyBytes(dllBytes, dllLength);

        DifferentialHybridImage* image = new DifferentialHybridImage(imageId, changedClassTokens, changedMethodTokens);
        LoadImageErrorCode err = image->Load(dllBytes, (size_t)dllLength);
        if (err != LoadImageErrorCode::OK)
        {
            TEMP_FORMAT(errMsg, "LoadImageErrorCode:%d", (int)err);
            il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetBadImageFormatException(errMsg));
            // when load a bad image, mean a fatal error. we don't clean image on purpose.
        }

        if (symbolBytes)
        {
            symbolBytes = (const byte*)CopyBytes(symbolBytes, symbolLength);
            err = image->LoadPDB(symbolBytes, (size_t)symbolLength);
            if (err != LoadImageErrorCode::OK)
            {
                TEMP_FORMAT(errMsg, "LoadPDB Error:%d", (int)err);
                il2cpp::vm::Exception::Raise(il2cpp::vm::Exception::GetBadImageFormatException(errMsg));
            }
        }

        TbAssembly data = image->GetRawImage().ReadAssembly(1);
        const char* nameNoExt = image->GetStringFromRawIndex(data.name);

        if (!Assembly::IsDifferentialHybridAssembly(nameNoExt))
        {
            return LoadImageErrorCode::DHE_NOT_DIFFERENTIAL_HYBRID_ASSEMBLY;
        }

        Il2CppAssembly* ass = FindPlaceHolderAssemblyLocked(nameNoExt, lock);
        IL2CPP_ASSERT(ass);
        if (ass->token)
        {
            return LoadImageErrorCode::DHE_HAS_BEEN_LOADED;
        }

        Il2CppImage* image2 = ass->image;
        image->InitBasic(image2);
        image->BuildIl2CppAssembly(ass);
        image->BuildIl2CppImage(image2, ass->originAssembly);
        image2->name = ConcatNewString(ass->aname.name, ".dll");
        image2->nameNoExt = ass->aname.name;

        image->InitRuntimeMetadatas();
        il2cpp::vm::Assembly::InvalidateAssemblyList();

        RunModuleInitializer(ass->image);
        return LoadImageErrorCode::OK;
    }

    struct DifferentialHybridOption
    {
        bool forceAllChanged;

        il2cpp::utils::dynamic_array<uint32_t> changeMethodTokens;

        il2cpp::utils::dynamic_array<uint32_t> changedClassTokens;

        bool TryUnmarshal(BlobReader& reader, il2cpp::utils::dynamic_array<uint32_t>& arr)
        {
            uint32_t count;
            if (!reader.TryRead32(count))
            {
                return false;
            }
            for (uint32_t i = 0; i < count; i++)
            {
                uint32_t token;
                if (!reader.TryRead32(token))
                {
                    return false;
                }
                arr.push_back(token);
            }
            return true;
        }

        bool Unmarshal(BlobReader& reader)
        {
            uint32_t signature;
            if (!reader.TryRead32(signature) || signature != 0xABCDABCD)
            {
                return false;
            }

            if (!reader.TrySkipBytes(32))
            {
                return false;
            }

            if (!reader.TrySkipBytes(32))
            {
                return false;
            }

            uint8_t byteForceAllChanged;
            if (!reader.TryReadByte(byteForceAllChanged))
            {
                return false;
            }

            forceAllChanged = byteForceAllChanged != 0;

            if (!TryUnmarshal(reader, changeMethodTokens))
            {
                return false;
            }

            if (!TryUnmarshal(reader, changedClassTokens))
            {
                return false;
            }

            return reader.IsEmpty();
        }
    };

    LoadImageErrorCode Assembly::LoadDifferentialHybridAssemblyWithDHAO(const void* dllBytes, uint32_t dllLength, const void* symbolBytes, uint32_t symbolLength,
        const void* dhaoBytes, uint32_t dhaoLength)
    {
        DifferentialHybridOption options = {};

        BlobReader reader((byte*)dhaoBytes, dhaoLength);
        if (!options.Unmarshal(reader))
        {
            return LoadImageErrorCode::DHE_BAD_DHAO_DATA;
        }
		return LoadDifferentialHybridAssembly(dllBytes, dllLength, symbolBytes, symbolLength, options.changedClassTokens, options.changeMethodTokens);
    }


    struct MetaVersion
    {
        uint32_t version;
        uint32_t signatureId;
    };

    struct MetaVersionFile
    {
        uint32_t fileVersion;

        std::vector<MetaVersion> typeDefVersions;

        std::vector<MetaVersion> methodDefVersions;

        bool TryUnmarshal(BlobReader& reader, std::vector<MetaVersion>& arr)
        {
            uint32_t count;
            if (!reader.TryRead32(count))
            {
                return false;
            }
            for (uint32_t i = 0; i < count; i++)
            {
                uint32_t version;
                if (!reader.TryRead32(version))
                {
                    return false;
                }
                uint32_t signatureId;
                if (!reader.TryRead32(signatureId))
                {
                    return false;
                }
                arr.push_back({ version, signatureId });
            }
            return true;
        }

        bool Unmarshal(BlobReader& reader)
        {
            if (reader.GetDataSize() < 20)
            {
                return false;
            }
            const char* signature = (const char*)reader.GetAndSkipCurBytes(4);
            if (std::strncmp(signature, "CPMV", 4))
            {
                return false;
            }

            uint32_t schemaVersion = reader.Read32();
            if (schemaVersion != 1)
            {
                return false;
            }

            fileVersion = reader.Read32();

            if (!TryUnmarshal(reader, typeDefVersions))
            {
                return false;
            }

            if (!TryUnmarshal(reader, methodDefVersions))
            {
                return false;
            }

            return reader.IsEmpty();
        }
    };

    static void ComputeChangedTokens(const MetaVersionFile& originalMetaVersionFile, const MetaVersionFile& currentMetaVersionFile,
        il2cpp::utils::dynamic_array<uint32_t>& changedClassTokens, il2cpp::utils::dynamic_array<uint32_t>& changedMethodTokens)
    {
        std::unordered_map<uint32_t, const MetaVersion*> originalSignatureId2MetaVersion(originalMetaVersionFile.typeDefVersions.size() + originalMetaVersionFile.methodDefVersions.size());
        for (const MetaVersion& mv : originalMetaVersionFile.typeDefVersions)
        {
            originalSignatureId2MetaVersion[mv.signatureId] = &mv;
        }
        for (const MetaVersion& mv : originalMetaVersionFile.methodDefVersions)
        {
            originalSignatureId2MetaVersion[mv.signatureId] = &mv;
        }

        int32_t rid = 0;
        for (const MetaVersion& mv : currentMetaVersionFile.typeDefVersions)
        {
            ++rid;
            auto it = originalSignatureId2MetaVersion.find(mv.signatureId);
            if (it == originalSignatureId2MetaVersion.end() || it->second->version != mv.version)
            {
                changedClassTokens.push_back(EncodeToken(TableType::TYPEDEF, rid));
            }
        }

        rid = 0;
        for (const MetaVersion& mv : currentMetaVersionFile.methodDefVersions)
        {
            ++rid;
            auto it = originalSignatureId2MetaVersion.find(mv.signatureId);
            if (it == originalSignatureId2MetaVersion.end() || it->second->version != mv.version)
            {
                changedMethodTokens.push_back(EncodeToken(TableType::METHOD, rid));
            }
        }
    }

    LoadImageErrorCode Assembly::LoadDifferentialHybridAssemblyWithMetaVersion(const void* dllBytes, uint32_t dllLength, const void* symbolBytes, uint32_t symbolLength,
        const void* originalMetaVersionBytes, uint32_t originalMetaVersionLength, const void* currentMetaVersionBytes, uint32_t currentMetaVersionLength)
    {
        MetaVersionFile originalMetaVersionFile;
        BlobReader originalMetaVersionReader((byte*)originalMetaVersionBytes, originalMetaVersionLength);
        if (!originalMetaVersionFile.Unmarshal(originalMetaVersionReader))
        {
            return LoadImageErrorCode::DHE_BAD_META_VERSION_FILE;
        }
        MetaVersionFile currentMetaVersionFile;
        BlobReader currentMetaVersionReader((byte*)currentMetaVersionBytes, currentMetaVersionLength);
        if (!currentMetaVersionFile.Unmarshal(currentMetaVersionReader))
        {
            return LoadImageErrorCode::DHE_BAD_META_VERSION_FILE;
        }


        il2cpp::utils::dynamic_array<uint32_t> changedClassTokens;
        il2cpp::utils::dynamic_array<uint32_t> changedMethodTokens;
        ComputeChangedTokens(originalMetaVersionFile, currentMetaVersionFile, changedClassTokens, changedMethodTokens);
        return LoadDifferentialHybridAssembly(dllBytes, dllLength, symbolBytes, symbolLength, changedClassTokens, changedMethodTokens);
    }

    LoadImageErrorCode Assembly::LoadOriginalDifferentialHybridAssembly(const char* assemblyName)
    {
        if (!IsDifferentialHybridAssembly(assemblyName))
        {
            return LoadImageErrorCode::DHE_NOT_DIFFERENTIAL_HYBRID_ASSEMBLY;
        }

        il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
        Il2CppAssembly* finalAss = FindPlaceHolderAssemblyLocked(assemblyName, lock);
        IL2CPP_ASSERT(finalAss);
        if (finalAss->token)
        {
            return LoadImageErrorCode::DHE_HAS_BEEN_LOADED;
        }
        il2cpp::vm::AssemblyVector refAsses;
        il2cpp::vm::Assembly::GetReferencedAssemblies(finalAss->originAssembly, &refAsses);
        for (const Il2CppAssembly* refAss : refAsses)
        {
            if (refAss->dheAssembly)
            {
                const char* refAssName = refAss->dheAssembly->image->nameNoExt;
                TEMP_FORMAT(errMsg, "assembly:%s is loaded by RuntimeApi::LoadOriginalDifferentialHybridAssembly, it can't reference to assembly:%s which was loaded by RuntimeApi::LoadDifferentialHybridAssembly. You should use RuntimeApi::LoadDifferentialHybridAssembly to load it.",
                    assemblyName, refAssName);
				RaiseExecutionEngineException(errMsg);
            }
        }


        Il2CppAssembly* originalAss = finalAss->originAssembly;
        IL2CPP_ASSERT(originalAss);

        Il2CppImage* originalImage = originalAss->image;
        Il2CppImage* finalImage = finalAss->image;
        IL2CPP_ASSERT(originalImage != finalImage);

        il2cpp::vm::MetadataCache::RetargetOriginalImageToFinalImage(originalImage, finalImage);
        il2cpp::vm::GlobalMetadata::RetargetOriginalImageToFinalImage(originalImage, finalImage);
        il2cpp::metadata::ArrayMetadata::RetargetOriginalImageToFinalImage(originalImage, finalImage);
        il2cpp::vm::GenericClass::RetargetOriginalImageToFinalImage(originalImage, finalImage);

        *finalAss = *originalAss;
        *finalImage = *originalImage;

        finalAss->dheAssembly = nullptr;
        finalAss->originAssembly = nullptr;
        finalAss->image = finalImage;
        finalImage->assembly = finalAss;
        il2cpp::vm::Assembly::InvalidateAssemblyList();

        return LoadImageErrorCode::OK;
    }
}
}

