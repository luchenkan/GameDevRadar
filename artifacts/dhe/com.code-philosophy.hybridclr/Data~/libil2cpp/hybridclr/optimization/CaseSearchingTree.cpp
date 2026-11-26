#include "PeepholeOptimization.h"

#include "vm/MetadataAlloc.h"

namespace hybridclr
{
namespace optimization
{
	using namespace hybridclr::interpreter;

	CaseSearchingNode* CaseSearchingTree::FindOrCreateChildNode(CaseSearchingNode* parentNode, interpreter::HiOpcodeEnum op)
	{
		if (parentNode == nullptr)
		{
			CaseSearchingNode* childNode = _root[(int)op];
			if (!childNode)
			{
				_root[(int)op] = childNode = new (HYBRIDCLR_METADATA_MALLOC(sizeof(CaseSearchingNode))) CaseSearchingNode(nullptr, op, 0);
			}
			return childNode;
		}
		else
		{
			CaseSearchingKey key = { parentNode, op };
			auto it = _hash2Node.find(key);
			if (it != _hash2Node.end())
			{
				return it->second;
			}
			CaseSearchingNode* childNode = new (HYBRIDCLR_METADATA_MALLOC(sizeof(CaseSearchingNode))) CaseSearchingNode(parentNode, op, parentNode->depth + 1);
			_hash2Node.insert({ key, childNode });
			return childNode;
		}
	}

	void CaseSearchingTree::AddCase(PeepholeOptimizationFunc c, CaseSearchingNode* cur, const std::vector<std::vector<interpreter::HiOpcodeEnum>>& patterns)
	{
		size_t newDepth = cur ? cur->depth + 1 : 0;
		if (newDepth < patterns.size())
		{
			for (interpreter::HiOpcodeEnum op : patterns[newDepth])
			{
				CaseSearchingNode* node = FindOrCreateChildNode(cur, op);
				AddCase(c, node, patterns);
			}
		}
		else
		{
			OptimizationFuncNode& cases = cur->cases;
			if (cases.func == nullptr)
			{
				cases.next = nullptr;
			}
			else
			{
				OptimizationFuncNode* node = (OptimizationFuncNode*)HYBRIDCLR_METADATA_MALLOC(sizeof(OptimizationFuncNode));
				*node = cases;
				cases.next = node;
			}
			cases.func = c;
		}
	}

	CaseSearchingNode* CaseSearchingTree::FindChildCases(CaseSearchingNode* lastNode, std::vector<interpreter::IRCommon*>& insts, size_t startIndex)
	{
		CaseSearchingNode* result = lastNode->cases.func ? lastNode : nullptr;
		for (size_t curIndex = startIndex; curIndex < insts.size(); ++curIndex)
		{
			interpreter::IRCommon* curIr = insts[curIndex];
			lastNode = FindChildNode(lastNode, curIr->type);
			if (!lastNode)
			{
				break;
			}
			if (lastNode->cases.func)
			{
				result = lastNode;
			}
		}
		return result;
	}

}
}