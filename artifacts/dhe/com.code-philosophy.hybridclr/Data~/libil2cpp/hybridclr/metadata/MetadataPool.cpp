#include "MetadataPool.h"

#include "utils/MemoryPool.h"
#include "vm/MetadataAlloc.h"
#include "vm/MetadataLock.h"
#include "vm/GlobalMetadata.h"
#include "os/Mutex.h"

#include "MetadataModule.h"

namespace hybridclr
{
namespace metadata
{

	using il2cpp::utils::HashUtils;

	class Il2CppArrayTypeHash
	{
	public:
		size_t operator()(const Il2CppArrayType* t1) const
		{
			return Hash(t1);
		}

		static size_t Hash(const Il2CppArrayType* t1);
	};

	class Il2CppTypeFullHash
	{
	public:
		size_t operator()(const Il2CppType* t1) const
		{
			return Hash(t1);
		}

		static size_t Hash(const Il2CppType* t1)
		{
			size_t hash = t1->type;

			hash = HashUtils::Combine(hash, t1->byref);

			hash = il2cpp::utils::HashUtils::Combine(hash, t1->attrs);
			hash = il2cpp::utils::HashUtils::Combine(hash, t1->num_mods);
			hash = il2cpp::utils::HashUtils::Combine(hash, t1->pinned);
#if HYBRIDCLR_UNITY_2021_OR_NEW
			hash = il2cpp::utils::HashUtils::Combine(hash, t1->valuetype);
#endif

			switch (t1->type)
			{
			case IL2CPP_TYPE_VALUETYPE:
			case IL2CPP_TYPE_CLASS:
			{
				return HashUtils::Combine(hash, reinterpret_cast<size_t>(t1->data.typeHandle));
			}
			case IL2CPP_TYPE_SZARRAY:
			case IL2CPP_TYPE_PTR:
			{
				return HashUtils::Combine(hash, Hash(t1->data.type));
			}
			case IL2CPP_TYPE_ARRAY:
			{
				const Il2CppArrayType* a = t1->data.array;
				// dont' compute sizes and lobounds
				hash = HashUtils::Combine(hash, Il2CppArrayTypeHash::Hash(a));
				return hash;
			}
			case IL2CPP_TYPE_GENERICINST:
			{
				return HashUtils::Combine(hash, (size_t)t1->data.generic_class);
			}
			case IL2CPP_TYPE_VAR:
			case IL2CPP_TYPE_MVAR:
				return HashUtils::Combine(hash, reinterpret_cast<size_t>(t1->data.genericParameterHandle));
			default:
				return hash;
			}
			return hash;
		}
	};

	size_t Il2CppArrayTypeHash::Hash(const Il2CppArrayType* t1)
	{
		size_t hash = t1->rank;
		hash = HashUtils::Combine(hash, t1->numsizes);
		hash = HashUtils::Combine(hash, t1->numlobounds);
		hash = HashUtils::Combine(hash, Il2CppTypeFullHash::Hash(t1->etype));

		for (uint8_t i = 0; i < t1->numsizes; ++i)
		{
			hash = HashUtils::Combine(hash, t1->sizes[i]);
		}
		for (uint8_t i = 0; i < t1->numlobounds; ++i)
		{
			hash = HashUtils::Combine(hash, t1->lobounds[i]);
		}
		return hash;
	}

	class Il2CppArrayTypeEqualityComparer
	{
	public:
		bool operator()(const Il2CppArrayType* t1, const Il2CppArrayType* t2) const
		{
			return AreEqual(t1, t2);
		}

		static bool AreEqual(const Il2CppArrayType* t1, const Il2CppArrayType* t2);
	};

	class Il2CppTypeFullEqualityComparer
	{
	public:
		bool operator()(const Il2CppType* t1, const Il2CppType* t2) const
		{
			return AreEqual(t1, t2);
		}

		static bool AreEqual(const Il2CppType* t1, const Il2CppType* t2)
		{
			if (!t1 || !t2)
			{
				return t1 == t2;
			}
			if (t1->type != t2->type)
			{
				return false;
			}

			if (t1->byref != t2->byref
				|| t1->attrs != t2->attrs
				|| t1->pinned != t2->pinned
#if HYBRIDCLR_UNITY_2021_OR_NEW
				|| t1->valuetype != t2->valuetype
#endif
				|| t1->num_mods != t2->num_mods)
			{
				return false;
			}

			switch (t1->type)
			{
			case IL2CPP_TYPE_VALUETYPE:
			case IL2CPP_TYPE_CLASS:
				return t1->data.typeHandle == t2->data.typeHandle;
			case IL2CPP_TYPE_PTR:
			case IL2CPP_TYPE_SZARRAY:
				return AreEqual(t1->data.type, t2->data.type);

			case IL2CPP_TYPE_ARRAY:
			{
				const Il2CppArrayType* a1 = t1->data.array;
				const Il2CppArrayType* a2 = t2->data.array;
				return Il2CppArrayTypeEqualityComparer::AreEqual(a1, a2);
			}
			case IL2CPP_TYPE_GENERICINST:
			{
				return t1->data.generic_class == t2->data.generic_class;
			}
			case IL2CPP_TYPE_VAR:
			case IL2CPP_TYPE_MVAR:
				return t1->data.genericParameterHandle == t2->data.genericParameterHandle;
			default:
				return true;
			}
			return true;
		}
	};

	bool Il2CppArrayTypeEqualityComparer::AreEqual(const Il2CppArrayType* a1, const Il2CppArrayType* a2)
	{
		if (!a1 || !a2)
		{
			return a1 == a2;
		}
		if (a1->rank != a2->rank || a1->numsizes != a2->numsizes || a1->numlobounds != a2->numlobounds)
		{
			return false;
		}
		for (uint8_t i = 0; i < a1->numsizes; ++i)
		{
			if (a1->sizes[i] != a2->sizes[i])
				return false;
		}
		for (uint8_t i = 0; i < a1->numlobounds; ++i)
		{
			if (a1->lobounds[i] != a2->lobounds[i])
				return false;
		}
		return Il2CppTypeFullEqualityComparer::AreEqual(a1->etype, a2->etype);
	}



	typedef Il2CppNotDefaultKeyHashSet<const Il2CppType*, Il2CppTypeFullHash, Il2CppTypeFullEqualityComparer> Il2CppTypeHashSet;
	static Il2CppTypeHashSet s_Il2CppTypePool;

	typedef Il2CppNotDefaultKeyHashSet<const Il2CppArrayType*, Il2CppArrayTypeHash, Il2CppArrayTypeEqualityComparer> Il2CppArrayTypeHashSet;
	Il2CppArrayTypeHashSet s_Il2CppArrayTypePool;


	void MetadataPool::Initialize()
	{

	}

	void MetadataPool::PreserveIl2CppTypeCount(size_t count)
	{
		s_Il2CppTypePool.resize(count);
	}

	void MetadataPool::RegisterIl2CppType(const Il2CppType* type)
	{
		auto ret = s_Il2CppTypePool.insert(type);
		IL2CPP_ASSERT(ret.second);
		if (type->type == IL2CPP_TYPE_ARRAY)
		{
			s_Il2CppArrayTypePool.insert(type->data.array);
		}
	}

	const Il2CppType* MetadataPool::LockGetPooledIl2CppType(const Il2CppType& type)
	{
		il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
		return GetPooledIl2CppType(type);
	}

	static Il2CppType* DeepCloneIl2CppType(const Il2CppType& type)
	{
		Il2CppType* newType = (Il2CppType*)HYBRIDCLR_METADATA_MALLOC(sizeof(Il2CppType));
		*newType = type;

		switch (type.type)
		{
		case IL2CPP_TYPE_VOID:
			break;
		case IL2CPP_TYPE_BOOLEAN:
		case IL2CPP_TYPE_I1:
		case IL2CPP_TYPE_U1:
		case IL2CPP_TYPE_CHAR:
		case IL2CPP_TYPE_I2:
		case IL2CPP_TYPE_U2:
		case IL2CPP_TYPE_I4:
		case IL2CPP_TYPE_U4:
		case IL2CPP_TYPE_R4:
		case IL2CPP_TYPE_I8:
		case IL2CPP_TYPE_U8:
		case IL2CPP_TYPE_R8:
		case IL2CPP_TYPE_I:
		case IL2CPP_TYPE_U:
		case IL2CPP_TYPE_TYPEDBYREF:
		case IL2CPP_TYPE_FNPTR:
		{
			break;
		}
		case IL2CPP_TYPE_STRING:
		case IL2CPP_TYPE_OBJECT:
		case IL2CPP_TYPE_BYREF:
		case IL2CPP_TYPE_VAR:
		case IL2CPP_TYPE_MVAR:
		{
#if HYBRIDCLR_UNITY_2021_OR_NEW
			IL2CPP_ASSERT(!type.valuetype);
#endif
			break;
		}
		case IL2CPP_TYPE_CLASS:
		case IL2CPP_TYPE_VALUETYPE:
		{
			break;
		}
		case IL2CPP_TYPE_PTR:
		{
			newType->data.type = MetadataPool::GetPooledIl2CppType(*type.data.type);
			break;
		}
		case IL2CPP_TYPE_SZARRAY:
		{
#if HYBRIDCLR_UNITY_2021_OR_NEW
			IL2CPP_ASSERT(!type.valuetype);
#endif
			newType->data.type = MetadataPool::GetPooledIl2CppType(*type.data.type);
			break;
		}
		case IL2CPP_TYPE_ARRAY:
		{
#if HYBRIDCLR_UNITY_2021_OR_NEW
			IL2CPP_ASSERT(!type.valuetype);
#endif
			const Il2CppArrayType* arrayType = type.data.array;
			newType->data.array = const_cast<Il2CppArrayType*>(MetadataPool::GetPooledIl2CppArrayType(arrayType->etype, arrayType->rank, arrayType->numsizes, arrayType->numlobounds, arrayType->sizes, arrayType->lobounds));
			break;
		}
		case IL2CPP_TYPE_GENERICINST:
		{
			newType->data.generic_class = const_cast<Il2CppGenericClass*>(type.data.generic_class);
			break;
		}
		default:;
		}

		return newType;
	}

	const Il2CppType* MetadataPool::GetPooledIl2CppType(const Il2CppType& type)
	{
		auto it = s_Il2CppTypePool.find(&type);
		if (it != s_Il2CppTypePool.end())
			return *it;
		Il2CppType* newType = DeepCloneIl2CppType(type);
		auto ret = s_Il2CppTypePool.insert(newType);
		IL2CPP_ASSERT(ret.second);
		return newType;
	}

	const Il2CppArrayType* MetadataPool::GetPooledIl2CppArrayType(const Il2CppType* elementType, uint8_t rank, uint8_t numsizes, uint8_t numlobounds, int* sizes, int* lobounds)
	{
		Il2CppArrayType type = { elementType, rank, numsizes, numlobounds, sizes, lobounds };

		auto it = s_Il2CppArrayTypePool.find(&type);
		if (it != s_Il2CppArrayTypePool.end())
			return *it;

		elementType = GetPooledIl2CppType(*elementType);

		Il2CppArrayType* newArrayType = (Il2CppArrayType*)HYBRIDCLR_METADATA_MALLOC(sizeof(Il2CppArrayType));
		newArrayType->etype = elementType;
		newArrayType->rank = rank;
		newArrayType->numsizes = numsizes;
		newArrayType->numlobounds = numlobounds;
		if (sizes)
		{
			newArrayType->sizes = (int*)HYBRIDCLR_METADATA_CALLOC(numsizes, sizeof(int));
			std::memcpy(newArrayType->sizes, sizes, sizeof(int) * numsizes);
		}
		if (lobounds)
		{
			newArrayType->lobounds = (int*)HYBRIDCLR_METADATA_CALLOC(numlobounds, sizeof(int));
			std::memcpy(newArrayType->lobounds, lobounds, sizeof(int) * numlobounds);
		}
		auto ret = s_Il2CppArrayTypePool.insert(newArrayType);
		IL2CPP_ASSERT(ret.second);
		return newArrayType;
	}

	const Il2CppArrayType* MetadataPool::GetPooledIl2CppArrayType(const Il2CppType* elementType, uint8_t rank)
	{
		return GetPooledIl2CppArrayType(elementType, rank, 0, 0, nullptr, nullptr);
	}

	const Il2CppType* MetadataPool::CreateClassOrValueTypeType(const Il2CppType& originalType, Il2CppMetadataTypeHandle newTypeHandle)
	{
		Il2CppType newType = originalType;
		newType.data.typeHandle = (Il2CppMetadataTypeHandle)newTypeHandle;
		return GetPooledIl2CppType(newType);
	}

	const Il2CppType* MetadataPool::CreateArrayType(const Il2CppType& type, const Il2CppType* newEleType)
	{
		Il2CppArrayType* oldArrayType = type.data.array;
		Il2CppType newType = type;
		newType.data.array = const_cast<Il2CppArrayType*>(GetPooledIl2CppArrayType(newEleType, oldArrayType->rank, oldArrayType->numsizes, oldArrayType->numlobounds, oldArrayType->sizes, oldArrayType->lobounds));
		return GetPooledIl2CppType(newType);
	}

	const Il2CppType* MetadataPool::CreateGenericInstType(const Il2CppType& originalType, Il2CppGenericClass* newGenericClass)
	{
		Il2CppType newType = originalType;
		newType.data.generic_class = newGenericClass;
		return GetPooledIl2CppType(newType);
	}
}
}