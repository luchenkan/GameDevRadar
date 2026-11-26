#pragma once

#include "../CommonDef.h"

#include "utils/Il2CppHashMap.h"
#include "utils/HashUtils.h"

#include "../interpreter/Instruction.h"
#include "../transform/TransformContext.h"

#include "Optimization.h"
#include "CaseSearchingTree.h"

namespace hybridclr
{
namespace optimization
{

	struct PeepholeOptimizationContext
	{
		transform::TransformContext* ctx;
	};

	class PeepholeOptimizationGroup : public IOptimizationPass
	{
	public:
		static void Initialize();
		PeepholeOptimizationGroup() {}

		virtual ~PeepholeOptimizationGroup()
		{

		}
		virtual void Process(const OptimizationContext& ctx) override;
	private:
		std::vector<PeepholeOptimizationFunc> _cases;
	};
}
}