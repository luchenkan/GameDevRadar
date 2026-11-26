#include "Runtime.h"

#include "vm/Exception.h"
#include "vm/String.h"
#include "vm/Assembly.h"
#include "vm/Class.h"
#include "vm/Object.h"
#include "vm/Reflection.h"
#include "icalls/mscorlib/System.Reflection/Assembly.h"

#include "RuntimeApi.h"
#include "interpreter/InterpreterModule.h"
#include "metadata/MetadataModule.h"
#include "metadata/MetadataPool.h"
#include "transform/TransformModule.h"
#include "optimization/OptimizationModule.h"
#include "encryption/EncryptionVM.h"


namespace hybridclr
{
	void Runtime::EarlyInitialize()
	{
		hybridclr::metadata::MetadataPool::Initialize();
	}

	void Runtime::Initialize()
	{
		RuntimeApi::RegisterInternalCalls();

		encryption::EncryptionVM::Initialize();
		metadata::MetadataModule::Initialize();
		interpreter::InterpreterModule::Initialize();
		transform::TransformModule::Initialize();
		optimization::OptimizationModule::Initialize();
	}
}