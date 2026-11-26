#include "OptimizationModule.h"

#include "PeepholeOptimization.h"

namespace hybridclr
{
namespace optimization
{
	void OptimizationModule::Initialize()
	{
		PeepholeOptimizationGroup::Initialize();
	}
}
}