#include "GlobalMetadataDatEncryption.h"

#include "EncryptionVM.h"

namespace hybridclr
{
namespace encryption
{
    constexpr int32_t ENCRYPTION_SANITY = 0x1357FEDA;
    constexpr int32_t ENCRYPTION_OP_LENGTH = 64;
    constexpr int32_t ENCRYPTION_DATA_LENGTH = 0x1000;
    constexpr int32_t ENCRYPTION_SEGMENT_SIZE = 64;

    struct GlobalMetdataHeaderLite
    {
        int32_t sanity;
        int32_t encryptedDataLength;
        byte key[256];
        byte encryptionOpertions[64];
        char signature[8];
        byte realData;
    };


    bool GlobalMetadataDatEncryption::TryDecrypt(void* globalMetadata, void*& realGlobalMetadataPtr)
    {
        realGlobalMetadataPtr = nullptr;

		GlobalMetdataHeaderLite* header = (GlobalMetdataHeaderLite*)globalMetadata;
        if (header->sanity != ENCRYPTION_SANITY)
        {
			return true;
		}

        EncryptionVM::DecryptBySegment(header->encryptionOpertions, ENCRYPTION_OP_LENGTH, header->key, (byte*)header->signature, header->encryptedDataLength, ENCRYPTION_SEGMENT_SIZE);

        if (strncmp(header->signature, "CODEPHIL", 8))
        {
			return false;
		}

        realGlobalMetadataPtr = &header->realData;
        return true;
	}
}
}

