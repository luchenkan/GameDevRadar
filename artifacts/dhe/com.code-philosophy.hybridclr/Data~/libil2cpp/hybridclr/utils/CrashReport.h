#pragma once

#include "il2cpp-config.h"

namespace hybridclr
{
namespace utils
{
	typedef void(*CrashReportCallback)(void* data);

	class CrashReport
	{
	public:
		 static void InstallCrashReportCallback(CrashReportCallback callback);
	};
}
}