#pragma once

#include "CommonDef.h"

namespace hybridclr
{

	class Runtime
	{
	public:
		static void EarlyInitialize();
		static void Initialize();
	};
}