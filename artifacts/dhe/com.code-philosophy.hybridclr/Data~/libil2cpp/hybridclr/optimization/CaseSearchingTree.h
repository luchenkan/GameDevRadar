#pragma once

#include "../CommonDef.h"

#include "utils/Il2CppHashMap.h"
#include "utils/HashUtils.h"

#include "../interpreter/Instruction.h"

namespace hybridclr
{
namespace optimization
{

	struct PeepholeOptimizationContext;

	typedef bool (*PeepholeOptimizationFunc)(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount);


	struct OptimizationFuncNode
	{
		OptimizationFuncNode* next;
		PeepholeOptimizationFunc func;
	};

	struct CaseSearchingNode
	{
		CaseSearchingNode(CaseSearchingNode* parent, interpreter::HiOpcodeEnum code, int32_t depth) : parent(parent), code(code), depth(depth), cases{}
		{

		}
		CaseSearchingNode* const parent;
		OptimizationFuncNode cases;
		const int32_t depth;
		const interpreter::HiOpcodeEnum code;
	};

	struct CaseSearchingKey
	{
		CaseSearchingNode* parent;
		interpreter::HiOpcodeEnum code;
	};

	struct CaseSearchingKeyHash
	{
		size_t operator()(const CaseSearchingKey& key) const
		{
			return il2cpp::utils::HashUtils::Combine((size_t)(key.parent), (size_t)key.code);
		}
	};

	struct CaseSearchingKeyEquals
	{
		size_t operator()(const CaseSearchingKey& k1, const CaseSearchingKey& k2) const
		{
			return k1.parent == k2.parent && k1.code == k2.code;
		}
	};

	class CaseSearchingTree
	{
	public:
		CaseSearchingTree()
		{
			std::memset(_root, 0, sizeof(_root));
		}

		void AddCase(PeepholeOptimizationFunc c, CaseSearchingNode* cur, const std::vector<std::vector<interpreter::HiOpcodeEnum>>& patterns);
		CaseSearchingNode* FindCases(std::vector<interpreter::IRCommon*>& insts, size_t startIndex)
		{
			interpreter::IRCommon* firstIr = insts[startIndex];
			CaseSearchingNode* rootNode = _root[(int32_t)firstIr->type];
			if (!rootNode)
			{
				return nullptr;
			}
			return FindChildCases(rootNode, insts, startIndex + 1);
		}
	private:
		CaseSearchingNode* FindChildNode(CaseSearchingNode* parentNode, interpreter::HiOpcodeEnum op)
		{
			if (parentNode == nullptr)
			{
				return _root[(int)op];
			}
			else
			{
				CaseSearchingKey key = { parentNode, op };
				auto it = _hash2Node.find(key);
				return it != _hash2Node.end() ? it->second : nullptr;
			}
		}

		CaseSearchingNode* FindChildCases(CaseSearchingNode* rootNode, std::vector<interpreter::IRCommon*>& insts, size_t index);
		CaseSearchingNode* FindOrCreateChildNode(CaseSearchingNode* cur, interpreter::HiOpcodeEnum op);
		CaseSearchingNode* _root[(int32_t)interpreter::HiOpcodeEnum::INSTRUMENT_COUNT];
		Il2CppNotDefaultKeyHashMap<CaseSearchingKey, CaseSearchingNode*, CaseSearchingKeyHash, CaseSearchingKeyEquals> _hash2Node;
	};
}
}