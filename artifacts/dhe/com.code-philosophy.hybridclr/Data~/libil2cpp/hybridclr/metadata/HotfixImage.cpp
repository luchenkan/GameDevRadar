#include "HotfixImage.h"

#include <unordered_map>

#include "vm/MetadataLock.h"
#include "vm/GlobalMetadata.h"
#include "vm/Class.h"
#include "vm/Image.h"
#include "vm/Exception.h"
#include "vm/MetadataCache.h"
#include "metadata/GenericMetadata.h"
#include "metadata/GenericMethod.h"
#include "../interpreter/Interpreter.h"
#include "../interpreter/InterpreterModule.h"

#include "MetadataPool.h"
#include "MetadataModule.h"

namespace hybridclr
{
namespace metadata
{
	static std::unordered_map<const Il2CppAssembly*, HotfixImage*> s_hotfixImages;

    HotfixImage* HotfixImage::GetHotfixAssemblyByTargetAssembly(const Il2CppAssembly* ass)
    {
        auto it = s_hotfixImages.find(ass);
        return it != s_hotfixImages.end() ? it->second : nullptr;
    }

    void HotfixImage::RegisteHotfixImage(const Il2CppAssembly* targetAssembly, HotfixImage* hotfixImage)
    {
        s_hotfixImages[targetAssembly] = hotfixImage;

        // FIXME. not delete previous HotfixImage
    }

    void HotfixImage::ResetMethod(const MethodInfo* targetMethod)
    {
        MethodInfo* method = const_cast<MethodInfo*>(targetMethod);
        if (method->isInterpterImpl)
        {
            method->interpData = nullptr;
        }
        else
        {
            method->isInterpterImpl = true;
            method->methodPointerCallByInterp = hybridclr::interpreter::InterpreterModule::GetMethodPointer(method);
            bool isAdjustorThunkMethod = IS_CLASS_VALUE_TYPE(method->klass) && hybridclr::metadata::IsInstanceMethod(method);
            method->virtualMethodPointerCallByInterp = isAdjustorThunkMethod ? hybridclr::interpreter::InterpreterModule::GetAdjustThunkMethodPointer(method) : method->methodPointerCallByInterp;

			// DHE doesn't inject ..ctor, so we have to replace methodPointer and virtualMethodPointer
            if (!std::strcmp(method->name, ".cctor"))
            {
                method->methodPointer = method->methodPointerCallByInterp;
#if HYBRIDCLR_UNITY_2021_OR_NEW
                method->virtualMethodPointer = method->methodPointer;
#endif
            }
        }
    }

    void HotfixImage::HotfixMethods()
    {
        std::unordered_set<const MethodInfo*> genericMethods;
        for (SuperSetMethodDefDetail& method : _methodDefs)
        {
            if (method.aotMethodDef && _fixedMethodTokens.find(method.aotMethodDef->token) != _fixedMethodTokens.end())
            {
                const MethodInfo* methodInfo = il2cpp::vm::GlobalMetadata::GetMethodInfoFromMethodHandle((Il2CppMetadataMethodDefinitionHandle)method.aotMethodDef);
#if HYBRIDCLR_UNITY_2019
                if (methodInfo->is_generic || methodInfo->is_inflated || methodInfo->klass->genericContainerIndex != kGenericContainerIndexInvalid)
#else
                if (methodInfo->is_generic || methodInfo->is_inflated || methodInfo->klass->genericContainerHandle)
#endif
                {
                    genericMethods.insert(methodInfo);
                }
                else
                {
                    ResetMethod(methodInfo);
                }
            }
        }
        if (!genericMethods.empty())
        {
            il2cpp::metadata::GenericMethod::InvalidateMethodInterpreterData(genericMethods);
        }
    }



    Il2CppGenericContainer* HotfixImage::GetGenericContainerByRawIndex(uint32_t index)
    {
        if (IsInterpreterImage(_targetAssembly->image))
        {
            InterpreterImage* interpreterImage = MetadataModule::GetImage(_targetAssembly->image);
            return interpreterImage->GetGenericContainerByRawIndex(index);
        }
        else
        {
            return (Il2CppGenericContainer*)il2cpp::vm::GlobalMetadata::GetGenericContainerFromIndex(index);
        }
    }
}
}

