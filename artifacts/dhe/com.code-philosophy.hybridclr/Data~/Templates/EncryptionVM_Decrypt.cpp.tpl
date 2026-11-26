#include "../encryption/EncryptionVM.h"

#include "vm/Exception.h"

namespace hybridclr
{
namespace encryption
{
	void EncryptionVM::Decrypt(const void* ops, size_t opLength, const byte* key, byte* data, uint32_t dataLength)
	{
		if (dataLength == 0)
		{
			return;
		}
		for (size_t i = 0; i < opLength; i++)
		{
			byte op = ((byte*)ops)[i];
			switch (op)
			{
				//!!!{{INSTRUCTIONS

				//!!!}}INSTRUCTIONS
			}
		}
	}

}
}

