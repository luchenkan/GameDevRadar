#pragma once

#include "RawImageBase.h"

namespace hybridclr
{
namespace metadata
{
	enum class AlgorithmId : uint32_t
	{
		NONE = 0,
		CUSTOM = 1,
	};


	const size_t kSmallSegmentSize = 0x10;
	const size_t kBigSegmentSize = 0x100;
	const size_t kSignatureSize = 0x10;

	class EncryptedRawImage : public RawImageBase
	{
	public:
		EncryptedRawImage() : RawImageBase()
		{

		}

		LoadImageErrorCode LoadCLIHeader(uint32_t& entryPointToken, uint32_t& metadataRva, uint32_t& metadataSize) override;

		LoadImageErrorCode PostLoadStreams() override;
		LoadImageErrorCode PostLoadTables() override;

		Il2CppString* GetUserStringBlogByIndex(uint32_t index) const override;

		void DecryptMethodBodyIfNeed(const byte* methodBody, uint32_t methodBodySize) override;

		TbTypeDef ReadTypeDef(uint32_t rawIndex) override;

	private:
		struct EncryptionInfo
		{
			AlgorithmId algorithm;
			uint32_t version;
			byte param[256];
		};

		typedef il2cpp::utils::dynamic_array<byte> EncryptionMethod;

		bool ReadEncryptionMethod(size_t& offset, EncryptionMethod& encMethod)
		{
			IL2CPP_ASSERT(offset % 4 == 0);
			uint32_t length = *(uint32_t*)(_imageData + offset);
			offset += 4;
			size_t oldOffset = offset;
			offset += length;
			if (offset > _imageLength)
			{
				return false;
			}
			encMethod.assign(_imageData + oldOffset, _imageData + offset);
			offset = (offset + 3) & ~3;
			return true;
		}

		const EncryptionInfo* _encryptionInfo;

		EncryptionMethod _headerEncCodes;

		EncryptionMethod _stringEncCodes;
		EncryptionMethod _blobEncCodes;
		EncryptionMethod _userStringEncCodes;
		EncryptionMethod _lazyUserStringEncCodes;
		EncryptionMethod _tableEncCodes;
		EncryptionMethod _lazyTableEncCodes;

		EncryptionMethod _methodBodyEnCodes;

		Il2CppNotDefaultKeyHashSet<void*, il2cpp::utils::PassThroughHash<void*>> _decryptedMethodBodies;
		il2cpp::utils::dynamic_array<byte> _decryptedTypeDefRids;
	};
}
}
