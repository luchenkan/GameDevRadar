#include "EncryptedRawImage.h"

#include <memory>

#include "../encryption/EncryptionVM.h"

namespace hybridclr
{
namespace metadata
{
	template<typename T>
	T getValueAtOffset(const byte* data, size_t offset)
	{
		return *(T*)(data + offset);
	}

	struct RawSectionHeader
	{
		uint32_t fileOffset;
		uint32_t fileLength;
		uint32_t rva;
		uint32_t virtualSize;
	};

	static void DecryptSignature(const byte* key, const byte* encryptedSig, byte originalSig[16])
	{
		std::memcpy(originalSig, encryptedSig, 16);
#define SIG_OP_SIZE 256
		byte opCodes[SIG_OP_SIZE];
		for (int i = 0; i < SIG_OP_SIZE; i++)
		{
			opCodes[i] = (byte)(SIG_OP_SIZE - 1 - i);
		}
		hybridclr::encryption::EncryptionVM::Decrypt(opCodes, sizeof(opCodes), key, originalSig, 16);
	}

	LoadImageErrorCode EncryptedRawImage::LoadCLIHeader(uint32_t& entryPointToken, uint32_t& metadataRva, uint32_t& metadataSize)
	{
		if (_imageLength < 0x100)
		{
			return LoadImageErrorCode::BAD_IMAGE;
		}
		const char* sig = (const char*)_imageData;
		if (std::strncmp(sig, "CDPH", 4))
		{
			return LoadImageErrorCode::BAD_IMAGE;
		}
		uint32_t formatVer = *(uint32_t*)(_imageData + 4);
		if (formatVer != 1)
		{
			return LoadImageErrorCode::UNSUPPORT_FORMAT_VERSION;
		}
		IL2CPP_ASSERT(sizeof(EncryptionInfo) == 264);
		_encryptionInfo = (EncryptionInfo*)(_imageData + 8);

		if (_encryptionInfo->algorithm > AlgorithmId::CUSTOM)
		{
			return LoadImageErrorCode::UNSUPPORT_ENCRYPTION_ALGORHITHM;
		}

		size_t encMethodOffset = 8 + sizeof(EncryptionInfo);
		EncryptionMethod* methods[] = {&_headerEncCodes, &_stringEncCodes, &_blobEncCodes, &_userStringEncCodes,
			&_lazyUserStringEncCodes, &_tableEncCodes, &_lazyTableEncCodes, &_methodBodyEnCodes };
		for (EncryptionMethod* method : methods)
		{
			if (!ReadEncryptionMethod(encMethodOffset, *method))
			{
				return LoadImageErrorCode::BAD_IMAGE;
			}
		}

		const byte* encryptedSignature = _imageData + encMethodOffset;
		byte originalSignature[16] = {  };
		DecryptSignature(_encryptionInfo->param, encryptedSignature, originalSignature);

		if (std::strncmp((const char*)originalSignature, "Hello, HybridCLR", kSignatureSize))
		{
			return LoadImageErrorCode::BAD_IMAGE;
		}

		const size_t kEntryPointTokenOffset = encMethodOffset + kSignatureSize;
		entryPointToken = getValueAtOffset<uint32_t>(_imageData, kEntryPointTokenOffset);
		metadataRva = getValueAtOffset<uint32_t>(_imageData, kEntryPointTokenOffset + 4);
		metadataSize = getValueAtOffset<uint32_t>(_imageData, kEntryPointTokenOffset + 8);

		uint32_t sectionCount = getValueAtOffset<uint32_t>(_imageData, kEntryPointTokenOffset + 12);
		RawSectionHeader* sectionHeaders = (RawSectionHeader*)(_imageData + kEntryPointTokenOffset + 16);
		for (uint32_t i = 0; i < sectionCount; i++)
		{
			RawSectionHeader& curSection = sectionHeaders[i];
			_sections.push_back({ curSection.rva, curSection.rva + curSection.virtualSize, curSection.fileOffset - curSection.rva });
		}
		return LoadImageErrorCode::OK;
	}

	LoadImageErrorCode EncryptedRawImage::PostLoadStreams()
	{
		hybridclr::encryption::EncryptionVM::DecryptBySegment(_stringEncCodes.begin(), _stringEncCodes.size(), _encryptionInfo->param, (byte*)_streamStringHeap.data, _streamStringHeap.size, kBigSegmentSize);
		hybridclr::encryption::EncryptionVM::DecryptBySegment(_blobEncCodes.begin(), _blobEncCodes.size(), _encryptionInfo->param, (byte*)_streamBlobHeap.data, _streamBlobHeap.size, kBigSegmentSize);
		hybridclr::encryption::EncryptionVM::DecryptBySegment(_userStringEncCodes.begin(), _userStringEncCodes.size(), _encryptionInfo->param, (byte*)_streamUS.data, _streamUS.size, kBigSegmentSize);
		hybridclr::encryption::EncryptionVM::DecryptBySegment(_tableEncCodes.begin(), _tableEncCodes.size(), _encryptionInfo->param, (byte*)_streamTables.data, _streamTables.size, kBigSegmentSize);

		return LoadImageErrorCode::OK;
	}

	LoadImageErrorCode EncryptedRawImage::PostLoadTables()
	{
		_decryptedTypeDefRids.resize_initialized(GetTableRowNum(TableType::TYPEDEF));
		return LoadImageErrorCode::OK;
	}

	TbTypeDef EncryptedRawImage::ReadTypeDef(uint32_t rawIndex)
	{
		if (!_decryptedTypeDefRids[rawIndex - 1])
		{
			_decryptedTypeDefRids[rawIndex - 1] = 1;
			auto& tb = _tables[(int)TableType::TYPEDEF];
			const byte* rowPtr = GetTableRowPtr(TableType::TYPEDEF, rawIndex);
			hybridclr::encryption::EncryptionVM::Decrypt(_lazyTableEncCodes.begin(), _lazyTableEncCodes.size(), _encryptionInfo->param, const_cast<byte*>(rowPtr), tb.rowMetaDataSize);
		}

		return RawImageBase::ReadTypeDef(rawIndex);
	}

	Il2CppString* EncryptedRawImage::GetUserStringBlogByIndex(uint32_t index) const
	{
		IL2CPP_ASSERT(index >= 0 && (uint32_t)index < _streamUS.size);
		const byte* str = _streamUS.data + index;
		uint32_t lengthSize;
		uint32_t stringLength = BlobReader::ReadCompressedUint32(str, lengthSize);
		if (stringLength == 0)
		{
			return CreateUserString("", 0);
		}
		str += lengthSize;
		if (stringLength < 0x1000)
		{
			byte buffer[0x1000];
			std::memcpy(buffer, str, stringLength);
			hybridclr::encryption::EncryptionVM::DecryptBySegment(_lazyUserStringEncCodes.begin(), _lazyUserStringEncCodes.size(), _encryptionInfo->param, buffer, stringLength, kSmallSegmentSize);
			return CreateUserString((const char*)buffer, stringLength);
		}
		std::unique_ptr<byte> buf(new byte[stringLength]);
		std::memcpy(buf.get(), str, stringLength);
		hybridclr::encryption::EncryptionVM::DecryptBySegment(_lazyUserStringEncCodes.begin(), _lazyUserStringEncCodes.size(), _encryptionInfo->param, buf.get(), stringLength, kSmallSegmentSize);
		return CreateUserString((const char*)buf.get(), stringLength);
	}

	void EncryptedRawImage::DecryptMethodBodyIfNeed(const byte* methodBody, uint32_t methodBodySize)
	{
		auto it = _decryptedMethodBodies.find((void*)methodBody);
		if (it != _decryptedMethodBodies.end())
		{
			return;
		}
		hybridclr::encryption::EncryptionVM::DecryptBySegment(_methodBodyEnCodes.begin(), _methodBodyEnCodes.size(), _encryptionInfo->param, const_cast<byte*>(methodBody), methodBodySize, kSmallSegmentSize);
		_decryptedMethodBodies.insert((void*)methodBody);
	}
}
}