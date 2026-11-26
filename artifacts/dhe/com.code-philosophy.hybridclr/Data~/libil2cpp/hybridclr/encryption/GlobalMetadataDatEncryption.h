#pragma once

#include "../CommonDef.h"

namespace hybridclr
{
namespace encryption
{
	class GlobalMetadataDatEncryption
	{
	public:
		static bool TryDecrypt(void* globalMetadata, void*& realGlobalMetadataPtr);
	};
}
}