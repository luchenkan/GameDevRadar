#pragma once

#include "../CommonDef.h"

namespace hybridclr
{
	namespace encryption
	{
#define ENCRYPTION_KEY_LENGTH 256
		class EncryptionVM
		{
		public:
			static void Initialize();
			static void Decrypt(const void* ops, size_t opLength, const byte* key, byte* data, uint32_t dataLength);
			static void DecryptBySegment(const void* ops, size_t opLength, const byte* key, byte* data, uint32_t dataLength, size_t segmentSize);
		};
	}
}