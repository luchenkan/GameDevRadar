#include "il2cpp-config.h"
#include "il2cpp-class-internals.h"
#include "Il2CppGenericContextHash.h"
#include "Il2CppGenericInstHash.h"
#include "Il2CppTypeHash.h"
#include "utils/HashUtils.h"

using il2cpp::utils::HashUtils;

namespace il2cpp
{
namespace metadata
{
    size_t Il2CppGenericContextHash::operator()(const Il2CppGenericContext* context) const
    {
        return Hash(context);
    }

    size_t Il2CppGenericContextHash::Hash(const Il2CppGenericContext* context)
    {
        size_t classInstHash = (size_t)context->class_inst;
        size_t methodInstHash = (size_t)context->method_inst;
        return HashUtils::Combine(classInstHash, methodInstHash);
    }
} /* namespace vm */
} /* namespace il2cpp */
