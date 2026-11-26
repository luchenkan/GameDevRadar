#pragma once

#include "../CommonDef.h"

#include "metadata/GenericMetadata.h"
#include "vm/MetadataAlloc.h"

namespace hybridclr
{
namespace metadata
{

	class MetadataPool
	{
	public:
		static void Initialize();
		static void PreserveIl2CppTypeCount(size_t count);
		static void RegisterIl2CppType(const Il2CppType* type);
		static const Il2CppType* LockGetPooledIl2CppType(const Il2CppType& type);
		static const Il2CppType* GetPooledIl2CppType(const Il2CppType& type);
		static const Il2CppArrayType* GetPooledIl2CppArrayType(const Il2CppType* elementType, uint8_t rank);
		static const Il2CppArrayType* GetPooledIl2CppArrayType(const Il2CppType* elementType, uint8_t rank, uint8_t numsizes, uint8_t numlobounds, int* sizes, int* lobounds);

		static const Il2CppType* CreateClassOrValueTypeType(const Il2CppType& originalType, Il2CppMetadataTypeHandle newTypeHandle);
		static const Il2CppType* CreateArrayType(const Il2CppType& originalType, const Il2CppType* newEleType);
		static const Il2CppType* CreateGenericInstType(const Il2CppType& originalType, Il2CppGenericClass* newGenericClass);
	};
}
}