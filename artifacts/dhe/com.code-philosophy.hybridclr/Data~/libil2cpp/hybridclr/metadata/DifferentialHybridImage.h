#pragma once
#include <unordered_set>
#include <unordered_map>

#include "InterpreterImage.h"

namespace hybridclr
{
namespace metadata
{

	struct TypeMapping
	{
		const Il2CppTypeDefinition* aotType;
		bool initedMethods;
		const Il2CppMethodDefinition** aotMethodOfInterpMethods;
		const Il2CppMethodDefinition** interpMethodOfAotMethods;
	};

	struct MethodMapping
	{
		const Il2CppMethodDefinition* aotMethod;
	};

	class DifferentialHybridImage : public InterpreterImage
	{
	public:
		static const Il2CppType* TranslateIl2CppTypeToDHE(const Il2CppType* type);
		static const Il2CppGenericInst* TranslateGenericInstToDHE(const Il2CppGenericInst* gi);
		static const Il2CppMethodDefinition* TranslateMethodDefinitionToDHE(const Il2CppMethodDefinition* methodDef);
#if HYBRIDCLR_UNITY_6000_OR_NEW
		static const Il2CppGenericMethod TranslateGenericMethodToDHE(const Il2CppGenericMethod& methodDef);
#else
		static const Il2CppGenericMethod* TranslateGenericMethodToDHE(const Il2CppGenericMethod* methodDef);
#endif
		static const Il2CppType* TranslateReverseGenericShareIl2CppTypeFromDHE(const Il2CppType* type);
		static const Il2CppGenericInst* TranslateReverseGenericShareGenericInstFromDHE(const Il2CppGenericInst* gi);
		static const MethodInfo* TranslateReverseMethodInfoFromDHE(const MethodInfo* method);
		static const MethodInfo* TranslateReverseNormalOrGenericMethodInfoFromDHE(const MethodInfo* method);

	public:
		DifferentialHybridImage(uint32_t imageIndex, const il2cpp::utils::dynamic_array<uint32_t>& changedClassTokens, const il2cpp::utils::dynamic_array<uint32_t>& changedMethodTokens) : InterpreterImage(imageIndex)
		{
			_changedClassTokens.insert(changedClassTokens.begin(), changedClassTokens.end());
			_changedMethodTokens.insert(changedMethodTokens.begin(), changedMethodTokens.end());
		}

		uint32_t GetRawTypeIndex(const TypeMapping& type) const
		{
			return (uint32_t)(&type - _typeMappings.data());
		}

		const Il2CppTypeDefinition* GetTypeDefinitionByTypeMapping(const TypeMapping& type) const
		{
			return &_typesDefines[GetRawTypeIndex(type)];
		}

		uint32_t GetOriginTokenByInterpreterToken(uint32_t token)
		{
			TableType tableType = DecodeTokenTableType(token);
			uint32_t rawIndex = DecodeTokenRowIndex(token) - 1;
			switch (tableType)
			{
			case TableType::TYPEDEF:
			{
				IL2CPP_ASSERT(rawIndex < (uint32_t)_typeMappings.size());
				const Il2CppTypeDefinition* aotTypeDef = _typeMappings[rawIndex].aotType;
				return aotTypeDef ? aotTypeDef->token : 0;
			}
			case TableType::METHOD:
			{
				IL2CPP_ASSERT(rawIndex < (uint32_t)_methodDefines.size());
				//const Il2CppMethodDefinition* aotMethod = _methodMappings[rawIndex].aotMethod;
				const Il2CppMethodDefinition* aotMethod = GetUnchangedOriginMethodDefintionByInterpreterMethod(&_methodDefines[rawIndex]);
				return aotMethod ? aotMethod->token : 0;
			}
			default:
			{
				IL2CPP_ASSERT(false);
				return 0;
			}
			}
		}

		const Il2CppTypeDefinition* GetOriginType(const Il2CppTypeDefinition* type)
		{
			uint32_t index = GetTypeRawIndex(type);
			IL2CPP_ASSERT(index < (uint32_t)_typeMappings.size());
			return _typeMappings[index].aotType;
		}

		const Il2CppTypeDefinition* GetGenericShareOriginType(const Il2CppTypeDefinition* type)
		{
			uint32_t index = GetTypeRawIndex(type);
			IL2CPP_ASSERT(index < (uint32_t)_typeMappings.size());
			TypeMapping& tm = _typeMappings[index];
			if (!tm.aotType)
			{
				return nullptr;
			}
			const Il2CppTypeDefinition* interpType = &_typesDefines[index];
			return IsValueType(tm.aotType) && _changedClassTokens.find(interpType->token) == _changedClassTokens.end() ? tm.aotType : nullptr;
		}

		const Il2CppTypeDefinition* GetInterpreterTypeByOriginType(const Il2CppTypeDefinition* type)
		{
			uint32_t typeIndex = DecodeTokenRowIndex(type->token) - 1;
			return typeIndex < (uint32_t)_originType2InterpreterTypes.size() ? _originType2InterpreterTypes[typeIndex] : nullptr;
		}

		const Il2CppMethodDefinition* GetInterpreterMethodDefinitionByOriginMethodDefinition(const Il2CppMethodDefinition* methodDef);

		const Il2CppMethodDefinition* GetUnchangedOriginMethodDefintionByInterpreterMethod(const Il2CppMethodDefinition* methodDef);

		FieldIndex GetInterpreterFieldIndexByOriginFieldIndex(const Il2CppTypeDefinition* interpType, FieldIndex index);

		Il2CppMethodPointer TryGetMethodPointer(const Il2CppMethodDefinition* method);
		Il2CppMethodPointer TryGetMethodPointer(const MethodInfo* method);
		Il2CppMethodPointer TryGetAdjustThunkMethodPointer(const Il2CppMethodDefinition* method);
		Il2CppMethodPointer TryGetAdjustThunkMethodPointer(const MethodInfo* method);
		InvokerMethod TryGetMethodInvoker(const Il2CppMethodDefinition* method);
		InvokerMethod TryGetMethodInvoker(const MethodInfo* method);

		void BuildIl2CppImage(Il2CppImage* image, Il2CppAssembly* originAssembly);
	protected:
		virtual void PostInitPhase1() override;
	private:
		void InitTypeMapping(TypeMapping& type, il2cpp::utils::dynamic_array<bool>& typeInitFlags);
		void InitTypeMappings();
		void InitTypeMethodsMapping(TypeMapping& tm);

		const Il2CppAssembly* _originAssembly;
		const Il2CppImage* _originImage;


		typedef Il2CppHashSet<uint32_t, il2cpp::utils::PassThroughHash<uint32_t>> Uint32HashSet;
		Uint32HashSet _changedMethodTokens;
		Uint32HashSet _changedClassTokens;
		il2cpp::utils::dynamic_array<TypeMapping> _typeMappings;
		il2cpp::utils::dynamic_array<const Il2CppTypeDefinition*> _originType2InterpreterTypes;
	};
}
}
