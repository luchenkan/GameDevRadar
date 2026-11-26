#include "UnityEngineDebug.h"

#include "il2cpp-class-internals.h"
#include "vm/Assembly.h"
#include "vm/Image.h"
#include "vm/Class.h"
#include "vm/String.h"
#include "utils/StringUtils.h"

namespace hybridclr
{
namespace utils
{
	static bool s_isInitialized = false;

	typedef void(*LogCallback)(Il2CppString* msg, MethodInfo* method);
	static LogCallback s_log = nullptr;
	static LogCallback s_logError = nullptr;

	static void InitLogCallbacks()
	{
		if (s_isInitialized)
			return;

		s_isInitialized = true;

		const Il2CppAssembly* unityEngineCorModuleAss = il2cpp::vm::Assembly::Load("UnityEngine.CoreModule");
		Il2CppClass* debugClass = il2cpp::vm::Image::ClassFromName(unityEngineCorModuleAss->image, "UnityEngine", "Debug");
		const MethodInfo* logMethod = il2cpp::vm::Class::GetMethodFromName(debugClass, "Log", 1);
		IL2CPP_ASSERT(logMethod);
		s_log = (LogCallback)logMethod->methodPointer;

		const MethodInfo* logErrMethod = il2cpp::vm::Class::GetMethodFromName(debugClass, "LogError", 1);
		IL2CPP_ASSERT(logErrMethod);
		s_logError = (LogCallback)logErrMethod->methodPointer;
	}

	static void WriteToLog(const char* msg, LogCallback callback)
	{
		std::string outputMsg;
		outputMsg.append("[hybridclr] ");
		outputMsg.append(msg);
		Il2CppString* msgStr = il2cpp::vm::String::NewLen(outputMsg.c_str(), (uint32_t)outputMsg.size());
		callback(msgStr, nullptr);
	}

	void UnityEngineDebug::Log(const char* msg)
	{
		if (!s_log)
		{
			InitLogCallbacks();
		}
		WriteToLog(msg, s_log);
	}

	void UnityEngineDebug::LogError(const char* msg)
	{
		if (!s_log)
		{
			InitLogCallbacks();
		}
		WriteToLog(msg, s_logError);
	}

}
}