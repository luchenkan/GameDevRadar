#include "EncryptionVM.h"

#include "vm/Exception.h"

namespace hybridclr
{
namespace encryption
{
	void EncryptionVM::Initialize()
	{
	}

	void EncryptionVM::DecryptBySegment(const void* ops, size_t opLength, const byte* key, byte* data, uint32_t dataLength, size_t segmentSize)
	{
		if (opLength == 0)
		{
			return;
		}
		for (size_t i = 0; i < dataLength; i += segmentSize)
		{
			size_t segmentLength = std::min(segmentSize, dataLength - i);
			Decrypt(ops, opLength, key, data + i, (uint32_t)segmentLength);
		}
	}
}
}

