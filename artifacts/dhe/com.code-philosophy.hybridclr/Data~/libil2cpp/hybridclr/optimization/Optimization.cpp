#include "Optimization.h"

#include "PeepholeOptimization.h"

namespace hybridclr
{
namespace optimization
{
	OptimizationPassGroup::OptimizationPassGroup()
	{
		AddPass(new PeepholeOptimizationGroup());
	}
}
}