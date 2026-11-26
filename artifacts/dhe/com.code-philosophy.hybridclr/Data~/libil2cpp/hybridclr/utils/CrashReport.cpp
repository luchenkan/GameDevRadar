#include "CrashReport.h"

#if IL2CPP_TARGET_WINDOWS
#include <windows.h>
#elif IL2CPP_TARGET_ANDROID || IL2CPP_TARGET_IOS
#define HYBRIDCLR_CRASH_POSIX_SIGNAL 1
#include <csignal>
#include <unistd.h>
#else

#endif

namespace hybridclr
{
namespace utils
{
	static CrashReportCallback s_crashReportCallback = nullptr;


	enum class CrashHandlerState
	{
		NONE,
		RUN_CRASH_REPORT_CALLBACK,
		RUN_PREVIOUS_HANDLER,
		EXIT,
	};
	static CrashHandlerState s_handlerState = CrashHandlerState::NONE;

#if IL2CPP_TARGET_WINDOWS

	static LPTOP_LEVEL_EXCEPTION_FILTER s_previousExceptionFilter = nullptr;

	long __stdcall DefaultUnhandledExceptionFilter(_EXCEPTION_POINTERS* excp)
	{
		// windows SEH is not reentrant, so we need to handle this case
		if (s_handlerState == CrashHandlerState::NONE)
		{
			s_handlerState = CrashHandlerState::EXIT;
			if (excp->ExceptionRecord->ExceptionCode == EXCEPTION_ACCESS_VIOLATION)
			{
				if (s_crashReportCallback)
				{
					s_crashReportCallback(excp);
				}
			}
			long ret = EXCEPTION_CONTINUE_SEARCH;
			if (s_previousExceptionFilter)
			{
				ret = s_previousExceptionFilter(excp);
			}
			return ret;
		}
		else
		{
			return EXCEPTION_CONTINUE_SEARCH;
		}
	}

	void CrashReport::InstallCrashReportCallback(CrashReportCallback callback)
	{
		s_crashReportCallback = callback;
		s_previousExceptionFilter = SetUnhandledExceptionFilter(DefaultUnhandledExceptionFilter);
	}
#elif HYBRIDCLR_CRASH_POSIX_SIGNAL

		struct sigaction s_oldHandlerSegv;
		struct sigaction s_oldHandlerAbrt;
//		struct sigaction s_oldHandlerFpe;
//		struct sigaction s_oldHandlerIll;
		struct sigaction s_oldHandlerBus;

	static void SignalHandler(int sig, siginfo_t* info, void* context)
	{
        if (s_handlerState == CrashHandlerState::NONE)
        {
			s_handlerState = CrashHandlerState::EXIT;
            if (s_crashReportCallback)
            {
                s_crashReportCallback(info);
            }
            if (sig == SIGSEGV)
            {
                if (s_oldHandlerSegv.sa_sigaction)
                {
                    s_oldHandlerSegv.sa_sigaction(sig, info, context);
                }
            }
            else if (sig == SIGABRT)
            {
                if (s_oldHandlerAbrt.sa_sigaction)
                {
                    s_oldHandlerAbrt.sa_sigaction(sig, info, context);
                }
            }
            else if (sig == SIGBUS)
            {
                if (s_oldHandlerBus.sa_sigaction)
                {
                    s_oldHandlerBus.sa_sigaction(sig, info, context);
                }
            }
        }
	}


	static void RegisterSignalHandlers()
	{
		struct sigaction sa = {};
		sa.sa_flags = SA_SIGINFO | SA_RESETHAND;
		sa.sa_sigaction = SignalHandler;
		sigemptyset(&sa.sa_mask);

		sigaction(SIGSEGV, &sa, &s_oldHandlerSegv);
		sigaction(SIGABRT, &sa, &s_oldHandlerAbrt);
//		sigaction(SIGFPE, &sa, &s_oldHandlerFpe);
//		sigaction(SIGILL, &sa, &s_oldHandlerIll);
		sigaction(SIGBUS, &sa, &s_oldHandlerBus);
	}

	void CrashReport::InstallCrashReportCallback(CrashReportCallback callback)
	{
		s_crashReportCallback = callback;
		RegisterSignalHandlers();
	}
#else
	void CrashReport::InstallCrashReportCallback(CrashReportCallback callback)
	{
		s_crashReportCallback = callback;
	}
#endif
}
}

