#pragma once

namespace hybridclr
{
namespace utils
{
	class UnityEngineDebug
	{
	public:
		static void Log(const char* msg);
		static void LogError(const char* msg);
	};
}
}