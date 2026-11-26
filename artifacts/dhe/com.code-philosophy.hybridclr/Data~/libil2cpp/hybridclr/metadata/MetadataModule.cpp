#include "MetadataModule.h"

#include "os/Atomic.h"
#include "os/Mutex.h"
#include "os/File.h"
#include "vm/Exception.h"
#include "vm/String.h"
#include "vm/Assembly.h"
#include "vm/Class.h"
#include "vm/Object.h"
#include "vm/Image.h"
#include "vm/MetadataLock.h"
#include "utils/Logging.h"
#include "utils/MemoryMappedFile.h"
#include "utils/Memory.h"

#include "../interpreter/InterpreterModule.h"

#include "Assembly.h"
#include "InterpreterImage.h"
#include "DifferentialHybridImage.h"
#include "ConsistentAOTHomologousImage.h"
#include "SuperSetAOTHomologousImage.h"
#include "HotfixImage.h"
#include "MetadataPool.h"

using namespace il2cpp;

namespace hybridclr
{

namespace metadata
{

    void MetadataModule::Initialize()
    {
        InterpreterImage::Initialize();
        Assembly::InitializePlaceHolderAssemblies();
        Assembly::InitializeDifferentialHybridAssembles();
    }

    Image* MetadataModule::GetUnderlyingInterpreterImage(const MethodInfo* methodInfo)
    {
        HotfixImage* hotfixImage = HotfixImage::GetHotfixAssemblyByTargetAssembly(methodInfo->klass->image->assembly);
        if (hotfixImage && hotfixImage->NeedHotfixed(methodInfo->token))
        {
            return hotfixImage;
        }
        if (metadata::IsInterpreterMethod(methodInfo))
        {
            return  hybridclr::metadata::MetadataModule::GetImage(methodInfo->klass);
        }
        return (metadata::Image*)hybridclr::metadata::AOTHomologousImage::FindImageByAssembly(
            methodInfo->klass->rank ? il2cpp_defaults.corlib->assembly : methodInfo->klass->image->assembly);
    }

}
}
