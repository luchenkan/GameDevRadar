#pragma once

#include "../CommonDef.h"

#include "utils/Il2CppHashMap.h"
#include "utils/HashUtils.h"

#include "../interpreter/Instruction.h"
#include "../transform/TransformContext.h"

namespace hybridclr
{
	namespace optimization
	{
		struct OptimizationContext
		{
			transform::TransformContext* ctx;
			std::vector<transform::IRBasicBlock*>* blocks;
		};

		class IOptimizationPass
		{
		public:
			virtual void Process(const OptimizationContext& ctx) = 0;
			virtual ~IOptimizationPass() {}
		};

		class OptimizationPassGroup
		{
		public:
			OptimizationPassGroup();

			void AddPass(IOptimizationPass* pass)
			{
				_passes.push_back(pass);
			}

			void Process(const OptimizationContext& ctx)
			{
				for (IOptimizationPass* pass : _passes)
				{
					pass->Process(ctx);
				}
			}

			~OptimizationPassGroup()
			{
				for (IOptimizationPass* pass : _passes)
				{
					delete pass;
				}
			}
		private:
			std::vector<IOptimizationPass*> _passes;
		};
	}
}