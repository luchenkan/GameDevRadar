#include "Il2CppCompatibleDef.h"

#include "vm/Runtime.h"

#include "metadata/MetadataModule.h"
#include "interpreter/InterpreterModule.h"


namespace hybridclr
{

	Il2CppMethodPointer InitAndGetInterpreterDirectlyCallMethodPointerSlow(MethodInfo* method)
	{
		IL2CPP_ASSERT(!method->initInterpCallMethodPointer);
		method->initInterpCallMethodPointer = 1;
#if HYBRIDCLR_UNITY_2021
		IL2CPP_ASSERT(!il2cpp::vm::Runtime::IsFullGenericSharingEnabled());
#endif
		if (hybridclr::metadata::MetadataModule::IsImplementedByInterpreter(method))
		{
			method->methodPointerCallByInterp = interpreter::InterpreterModule::GetMethodPointer(method);
			bool isAdjustorThunkMethod = IS_CLASS_VALUE_TYPE(method->klass) && hybridclr::metadata::IsInstanceMethod(method);
			if (isAdjustorThunkMethod)
			{
				method->virtualMethodPointerCallByInterp = interpreter::InterpreterModule::GetAdjustThunkMethodPointer(method);
			}
			else
			{
				method->virtualMethodPointerCallByInterp = method->methodPointerCallByInterp;
			}
			if (method->invoker_method == nullptr
#if HYBRIDCLR_UNITY_2021_OR_NEW
				|| method->invoker_method == il2cpp::vm::Runtime::GetMissingMethodInvoker()
#endif
				)
			{
				method->invoker_method = hybridclr::interpreter::InterpreterModule::GetMethodInvoker(method);
			}
#if HYBRIDCLR_UNITY_2021_OR_NEW
			if (method->methodPointer == nullptr)
			{
				method->methodPointer = method->methodPointerCallByInterp;
			}
			if (method->virtualMethodPointer == nullptr)
			{
				method->virtualMethodPointer = method->virtualMethodPointerCallByInterp;
			}
#else
			if (method->methodPointer == nullptr)
			{
				method->methodPointer = method->virtualMethodPointerCallByInterp;
			}
#endif
#if HYBRIDCLR_UNITY_2021
			method->indirect_call_via_invokers = 1;
#endif
			method->isInterpterImpl = 1;
		}
		return method->methodPointerCallByInterp;
	}
}