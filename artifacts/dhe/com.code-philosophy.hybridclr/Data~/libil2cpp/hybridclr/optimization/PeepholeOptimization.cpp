#include "PeepholeOptimization.h"

#include "vm/MetadataLock.h"
#include "vm/MetadataAlloc.h"

namespace hybridclr
{
namespace optimization
{
	using namespace hybridclr::interpreter;
	CaseSearchingTree* g_searchingTree = nullptr;

	struct POCInfo
	{
		std::vector<std::vector<interpreter::HiOpcodeEnum>> patterns;
		PeepholeOptimizationFunc func;
	};

#define POCreateAddIR(varName, typeName) IR##typeName* varName = ctx.ctx->GetPool().AllocIR<IR##typeName>(); varName->type = HiOpcodeEnum::typeName; resultIrs[resultCount++] = varName;

	static bool POF_LdcStloc(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		POCreateAddIR(stc, StcVarConst);
		IRStlocVarVar_size_8* stloc = (IRStlocVarVar_size_8*)irs[1];
		switch (irs[0]->type)
		{
		case HiOpcodeEnum::LdcVarConst_1:
		{
			IRLdcVarConst_1* ldc = (IRLdcVarConst_1*)irs[0];
			if (ldc->dst != stloc->src)
			{
				return false;
			}
			stc->src = ldc->src;
			break;
		}
		case HiOpcodeEnum::LdcVarConst_2:
		{
			IRLdcVarConst_2* ldc = (IRLdcVarConst_2*)irs[0];
			if (ldc->dst != stloc->src)
			{
				return false;
			}
			stc->src = ldc->src;
			break;
		}
		case HiOpcodeEnum::LdcVarConst_4:
		{
			IRLdcVarConst_4* ldc = (IRLdcVarConst_4*)irs[0];
			if (ldc->dst != stloc->src)
			{
				return false;
			}
			stc->src = ldc->src;
			break;
		}
		case HiOpcodeEnum::LdcVarConst_8:
		{
			IRLdcVarConst_8* ldc = (IRLdcVarConst_8*)irs[0];
			if (ldc->dst != stloc->src)
			{
				return false;
			}
			stc->src = ldc->src;
			break;
		}
		default: return false;
		}
		stc->dst = stloc->dst;
		return true;
	}

	static bool POF_LoadthisLdfld(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// (ldarg|ldloc) + ldfld -> ldfld
		IRLdlocVarVar_size_8* loadThis = (IRLdlocVarVar_size_8*)irs[0];
		IRLdfldVarVar_i1* ldfld = (IRLdfldVarVar_i1*)irs[1];
		if (loadThis->dst != ldfld->obj)
		{
			return false;
		}
		POCreateAddIR(reLdfld, LdfldVarVar_i1);
		*reLdfld = *ldfld;
		reLdfld->obj = loadThis->src;
		return true;
	}

	static bool POF_LoadBinOp(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// ldloc + binop -> binop
		IRLdlocVarVar_size_8* loadIn2 = (IRLdlocVarVar_size_8*)irs[0];
		IRBinOpVarVarVar_Add_i4* binOp = (IRBinOpVarVarVar_Add_i4*)irs[1];
		if (loadIn2->dst != binOp->op2)
		{
			return false;
		}
		POCreateAddIR(rebinOp, BinOpVarVarVar_Add_i4);
		*rebinOp = *binOp;
		rebinOp->op2 = loadIn2->src;
		return true;
	}

	static bool POF_LoadLoadBinOp(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// ldloc + ldloc + binop -> binop
		IRLdlocVarVar_size_8* loadIn1 = (IRLdlocVarVar_size_8*)irs[0];
		IRLdlocVarVar_size_8* loadIn2 = (IRLdlocVarVar_size_8*)irs[1];
		IRBinOpVarVarVar_Add_i4* binOp = (IRBinOpVarVarVar_Add_i4*)irs[2];
		if (loadIn1->dst != binOp->op1)
		{
			return false;
		}
		if (loadIn2->dst != binOp->op2)
		{
			return false;
		}
		POCreateAddIR(rebinOp, BinOpVarVarVar_Add_i4);
		*rebinOp = *binOp;
		rebinOp->op1 = loadIn1->src;
		rebinOp->op2 = loadIn2->src;
		return true;
	}

	static bool POF_LoadLoadBinOpStore(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// ldloc + ldloc + binop + stloc -> binop
		IRLdlocVarVar_size_8* loadIn1 = (IRLdlocVarVar_size_8*)irs[0];
		IRLdlocVarVar_size_8* loadIn2 = (IRLdlocVarVar_size_8*)irs[1];
		IRBinOpVarVarVar_Add_i4* binOp = (IRBinOpVarVarVar_Add_i4*)irs[2];
		IRStlocVarVar_size_8* ret = (IRStlocVarVar_size_8*)irs[3];
		if (loadIn1->dst != binOp->op1)
		{
			return false;
		}
		if (loadIn2->dst != binOp->op2)
		{
			return false;
		}
		if (binOp->ret != ret->src)
		{
			return false;
		}
		POCreateAddIR(rebinOp, BinOpVarVarVar_Add_i4);
		*rebinOp = *binOp;
		rebinOp->op1 = loadIn1->src;
		rebinOp->op2 = loadIn2->src;
		rebinOp->ret = ret->dst;
		return true;
	}

	static bool POF_BinOpStore(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// binop + stloc -> binop
		IRBinOpVarVarVar_Add_i4* binOp = (IRBinOpVarVarVar_Add_i4*)irs[0];
		IRStlocVarVar_size_8* ret = (IRStlocVarVar_size_8*)irs[1];
		if (binOp->ret != ret->src)
		{
			return false;
		}
		POCreateAddIR(rebinOp, BinOpVarVarVar_Add_i4);
		*rebinOp = *binOp;
		rebinOp->ret = ret->dst;
		return true;
	}

	// varvarconst

	static bool POF_LdcBinOp(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		IL2CPP_ASSERT((int)HiOpcodeEnum::BinOpVarConstVar_Rem_f8 - (int)HiOpcodeEnum::BinOpVarConstVar_Add_i4 == (int)HiOpcodeEnum::BinOpVarVarVar_Rem_f8 - (int)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
		switch (irs[0]->type)
		{
		case HiOpcodeEnum::LdcVarConst_4:
		{
			IRLdcVarConst_4* loadIn2 = (IRLdcVarConst_4*)irs[0];
			IRBinOpVarVarVar_Add_i4* binOp = (IRBinOpVarVarVar_Add_i4*)irs[1];
			if (loadIn2->dst != binOp->op2)
			{
				return false;
			}
			POCreateAddIR(rebinOp, BinOpVarVarConst_Add_i4);
			rebinOp->type = (HiOpcodeEnum)((int32_t)binOp->type + (int32_t)HiOpcodeEnum::BinOpVarVarConst_Add_i4 - (int32_t)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
			rebinOp->op1 = binOp->op1;
			rebinOp->op2 = loadIn2->src;
			rebinOp->ret = binOp->ret;
			return true;
		}
		case HiOpcodeEnum::LdcVarConst_8:
		{
			IRLdcVarConst_8* loadIn2 = (IRLdcVarConst_8*)irs[0];
			IRBinOpVarVarVar_Add_i8* binOp = (IRBinOpVarVarVar_Add_i8*)irs[1];
			if (loadIn2->dst != binOp->op2)
			{
				return false;
			}
			POCreateAddIR(rebinOp, BinOpVarVarConst_Add_i8);
			rebinOp->type = (HiOpcodeEnum)((int32_t)binOp->type + (int32_t)HiOpcodeEnum::BinOpVarVarConst_Add_i4 - (int32_t)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
			rebinOp->op1 = binOp->op1;
			rebinOp->op2 = loadIn2->src;
			rebinOp->ret = binOp->ret;
			return true;
		}
		default: return false;
		}
	}

	static bool POF_LoadLdcBinOp(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		IL2CPP_ASSERT((int)HiOpcodeEnum::BinOpVarConstVar_Rem_f8 - (int)HiOpcodeEnum::BinOpVarConstVar_Add_i4 == (int)HiOpcodeEnum::BinOpVarVarVar_Rem_f8 - (int)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
		// ldloc + ldc + binop -> binop
		switch (irs[1]->type)
		{
		case HiOpcodeEnum::LdcVarConst_4:
		{
			IRLdlocVarVar_size_8* loadIn1 = (IRLdlocVarVar_size_8*)irs[0];
			IRLdcVarConst_4* loadIn2 = (IRLdcVarConst_4*)irs[1];
			IRBinOpVarVarVar_Add_i4* binOp = (IRBinOpVarVarVar_Add_i4*)irs[2];
			if (loadIn1->dst != binOp->op1)
			{
				return false;
			}
			if (loadIn2->dst != binOp->op2)
			{
				return false;
			}
			POCreateAddIR(rebinOp, BinOpVarVarConst_Add_i4);
			rebinOp->type = (HiOpcodeEnum)((int32_t)binOp->type + (int32_t)HiOpcodeEnum::BinOpVarVarConst_Add_i4 - (int32_t)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
			rebinOp->op1 = loadIn1->src;
			rebinOp->op2 = loadIn2->src;
			rebinOp->ret = binOp->ret;
			return true;
		}
		case HiOpcodeEnum::LdcVarConst_8:
		{
			IRLdlocVarVar_size_8* loadIn1 = (IRLdlocVarVar_size_8*)irs[0];
			IRLdcVarConst_8* loadIn2 = (IRLdcVarConst_8*)irs[1];
			IRBinOpVarVarVar_Add_i8* binOp = (IRBinOpVarVarVar_Add_i8*)irs[2];
			if (loadIn1->dst != binOp->op1)
			{
				return false;
			}
			if (loadIn2->dst != binOp->op2)
			{
				return false;
			}
			POCreateAddIR(rebinOp, BinOpVarVarConst_Add_i8);
			rebinOp->type = (HiOpcodeEnum)((int32_t)binOp->type + (int32_t)HiOpcodeEnum::BinOpVarVarConst_Add_i4 - (int32_t)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
			rebinOp->op1 = loadIn1->src;
			rebinOp->op2 = loadIn2->src;
			rebinOp->ret = binOp->ret;
			return true;
		}
		default: return false;
		}
	}

	static bool POF_LoadLdcBinOpStore(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		IL2CPP_ASSERT((int)HiOpcodeEnum::BinOpVarConstVar_Rem_f8 - (int)HiOpcodeEnum::BinOpVarConstVar_Add_i4 == (int)HiOpcodeEnum::BinOpVarVarVar_Rem_f8 - (int)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
		// ldloc + ldloc + binop + stloc -> binop
		switch (irs[1]->type)
		{
		case HiOpcodeEnum::LdcVarConst_4:
		{
			IRLdlocVarVar_size_8* loadIn1 = (IRLdlocVarVar_size_8*)irs[0];
			IRLdcVarConst_4* loadIn2 = (IRLdcVarConst_4*)irs[1];
			IRBinOpVarVarVar_Add_i4* binOp = (IRBinOpVarVarVar_Add_i4*)irs[2];
			IRStlocVarVar_size_8* ret = (IRStlocVarVar_size_8*)irs[3];
			if (loadIn1->dst != binOp->op1)
			{
				return false;
			}
			if (loadIn2->dst != binOp->op2)
			{
				return false;
			}
			if (binOp->ret != ret->src)
			{
				return false;
			}
			POCreateAddIR(rebinOp, BinOpVarVarConst_Add_i4);
			rebinOp->type = (HiOpcodeEnum)((int32_t)binOp->type + (int32_t)HiOpcodeEnum::BinOpVarVarConst_Add_i4 - (int32_t)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
			rebinOp->op1 = loadIn1->src;
			rebinOp->op2 = loadIn2->src;
			rebinOp->ret = ret->dst;
			return true;
		}
		case HiOpcodeEnum::LdcVarConst_8:
		{
			IRLdlocVarVar_size_8* loadIn1 = (IRLdlocVarVar_size_8*)irs[0];
			IRLdcVarConst_8* loadIn2 = (IRLdcVarConst_8*)irs[1];
			IRBinOpVarVarVar_Add_i8* binOp = (IRBinOpVarVarVar_Add_i8*)irs[2];
			IRStlocVarVar_size_8* ret = (IRStlocVarVar_size_8*)irs[3];
			if (loadIn1->dst != binOp->op1)
			{
				return false;
			}
			if (loadIn2->dst != binOp->op2)
			{
				return false;
			}
			if (binOp->ret != ret->src)
			{
				return false;
			}
			POCreateAddIR(rebinOp, BinOpVarVarConst_Add_i8);
			rebinOp->type = (HiOpcodeEnum)((int32_t)binOp->type + (int32_t)HiOpcodeEnum::BinOpVarVarConst_Add_i4 - (int32_t)HiOpcodeEnum::BinOpVarVarVar_Add_i4);
			rebinOp->op1 = loadIn1->src;
			rebinOp->op2 = loadIn2->src;
			rebinOp->ret = ret->dst;
			return true;
		}
		default: return false;
		}
	}

	static bool POF_BinOpLdcStore(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// binop + stloc -> binop
		IRBinOpVarVarConst_Add_i4* binOp = (IRBinOpVarVarConst_Add_i4*)irs[0];
		IRStlocVarVar_size_8* ret = (IRStlocVarVar_size_8*)irs[1];
		if (binOp->ret != ret->src)
		{
			return false;
		}
		POCreateAddIR(rebinOp, BinOpVarVarConst_Add_i4);
		*rebinOp = *binOp;
		rebinOp->ret = ret->dst;
		return true;
	}

	static bool POF_NotNullIrBrTrueFalse(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// not null ir + (brtrue|brfalse)
		// FIXME. the output of not null ir should be same to the input of br
		if (irs[0]->type == HiOpcodeEnum::BoxVarVar)
		{
			IRBoxVarVar* irBox = (IRBoxVarVar*)irs[0];
			Il2CppClass* klass = (Il2CppClass*)ctx.ctx->GetResolveData(irBox->klass);
			if (!IS_CLASS_VALUE_TYPE(klass) || il2cpp::vm::Class::IsNullable(klass))
			{
				return false;
			}
		}


		IRBranchTrueVar_i4* br = (IRBranchTrueVar_i4*)irs[1];
		switch (br->type)
		{
		case HiOpcodeEnum::BranchTrueVar_i4:
		case HiOpcodeEnum::BranchTrueVar_i8:
		{
			POCreateAddIR(ir, BranchUncondition_4);
			ir->offset = br->offset;
			ctx.ctx->PushOffset(&ir->offset);
			return true;
		}
		case HiOpcodeEnum::BranchFalseVar_i4:
		case HiOpcodeEnum::BranchFalseVar_i8:
		{
			return true;
		}
		default:
		{
			return false;
		}
		}
	}

	static bool POF_LdlocLdfld(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// ldloc + ldfld -> ldfld
		IRLdlocVarVar_size_8* ldloc = (IRLdlocVarVar_size_8*)irs[0];
		IRLdfldVarVar_i4* ldfld = (IRLdfldVarVar_i4*)irs[1];
		if (ldloc->dst != ldfld->obj)
		{
			return false;
		}
		POCreateAddIR(newLdfld, LdfldVarVar_i4);
		*newLdfld = *ldfld;
		newLdfld->obj = ldloc->src;
		return true;
	}

	static bool POF_LdfldLdloc(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		// ldfld + ldloc -> ldfld
		IRLdfldVarVar_i4* ldfld = (IRLdfldVarVar_i4*)irs[0];
		IRLdlocVarVar_size_8* ldloc = (IRLdlocVarVar_size_8*)irs[1];
		if (ldloc->src != ldfld->dst)
		{
			return false;
		}
		POCreateAddIR(newLdfld, LdfldVarVar_i4);
		*newLdfld = *ldfld;
		newLdfld->dst = ldloc->dst;
		return true;
	}

	inline bool IsLdfldClassFamily(HiOpcodeEnum op)
	{
		return HiOpcodeEnum::LdfldVarVar_i1 <= op && op <= HiOpcodeEnum::LdfldVarVar_n_4;
	}

	inline bool IsLdfldValueTypeFamily(HiOpcodeEnum op)
	{
		return HiOpcodeEnum::LdfldValueTypeVarVar_i1 <= op && op <= HiOpcodeEnum::LdfldValueTypeVarVar_n_4;
	}

	// 7 combination => 6 final combination => 3 new combination
	// ldfldClass + ldfldClass # (*a)[offset1][offset2] # LdfldClassLdfldClass @
	// ldfldClass + ldfldStruct => ldfldClass(offset1+offset2) # (*a)[offset1+offset2]
	// ldfldClass + ldflda => # (*a)[offset] + offset2 # LdfldClassLdflda @
	// ldflda + ldfldClass => ldfldClass(offset1+offset2) # (*a)[offset1+offset2]
	// ldflda + ldflda => ldflda(offset1+offset2) # (*a)+offset1+offset2
	// ldfldStruct + ldfldClass # a[offset1][offset] # LdfldStructLdfldClass @
	// ldfldStruct + ldfldStruct => ldfldStruct(offset1+offset2) # a[offset1+offset2]

	static bool POF_LdfldLdfld(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		IL2CPP_ASSERT((int)HiOpcodeEnum::LdfldValueTypeVarVar_size_64 - (int)HiOpcodeEnum::LdfldValueTypeVarVar_i1 == (int)HiOpcodeEnum::LdfldVarVar_size_64 - (int)HiOpcodeEnum::LdfldVarVar_i1);
		IL2CPP_ASSERT((int)HiOpcodeEnum::LdfldClassLdfldClassVarVar_size_64 - (int)HiOpcodeEnum::LdfldClassLdfldClassVarVar_i1 == (int)HiOpcodeEnum::LdfldVarVar_size_64 - (int)HiOpcodeEnum::LdfldVarVar_i1);
		//IL2CPP_ASSERT((int)HiOpcodeEnum::LdfldClassLdfldValueTypeVarVar_size_64 - (int)HiOpcodeEnum::LdfldClassLdfldValueTypeVarVar_i1 == (int)HiOpcodeEnum::LdfldVarVar_size_64 - (int)HiOpcodeEnum::LdfldVarVar_i1);
		IL2CPP_ASSERT((int)HiOpcodeEnum::LdfldValueTypeLdfldClassVarVar_size_64 - (int)HiOpcodeEnum::LdfldValueTypeLdfldClassVarVar_i1 == (int)HiOpcodeEnum::LdfldVarVar_size_64 - (int)HiOpcodeEnum::LdfldVarVar_i1);
		//IL2CPP_ASSERT((int)HiOpcodeEnum::LdfldValueTypeLdfldValueTypeVarVar_size_64 - (int)HiOpcodeEnum::LdfldValueTypeLdfldValueTypeVarVar_i1 == (int)HiOpcodeEnum::LdfldVarVar_size_64 - (int)HiOpcodeEnum::LdfldVarVar_i1);
		IRLdfldVarVar_i4* ldfld1 = (IRLdfldVarVar_i4*)irs[0];
		IRLdfldVarVar_i4* ldfld2 = (IRLdfldVarVar_i4*)irs[1];
		if (ldfld1->dst != ldfld2->obj)
		{
			return false;
		}

		HiOpcodeEnum op1 = ldfld1->type;
		HiOpcodeEnum op2 = ldfld2->type;
		if (IsLdfldClassFamily(op1))
		{
			if (IsLdfldClassFamily(op2))
			{
				// ldfldClass + ldfldClass # (*a)[offset1][offset2] # LdfldClassLdfldClass
				POCreateAddIR(newIr, LdfldClassLdfldClassVarVar_i1);
				newIr->type = (HiOpcodeEnum)((int32_t)ldfld2->type + (int32_t)HiOpcodeEnum::LdfldClassLdfldClassVarVar_i1 - (int32_t)HiOpcodeEnum::LdfldVarVar_i1);
				newIr->obj = ldfld1->obj;
				newIr->offset1 = ldfld1->offset;
				newIr->offset2 = ldfld2->offset;
				newIr->dst = ldfld2->dst;
				return true;
			}
			else if (IsLdfldValueTypeFamily(op2))
			{
				// ldfldClass + ldfldStruct => ldfldClass(offset1+offset2) # (*a)[offset1+offset2]
				POCreateAddIR(newIr, LdfldVarVar_i1);
				newIr->type = (HiOpcodeEnum)((int32_t)ldfld2->type + (int32_t)HiOpcodeEnum::LdfldVarVar_i1 - (int32_t)HiOpcodeEnum::LdfldValueTypeVarVar_i1);
				newIr->obj = ldfld1->obj;
				newIr->offset = ldfld1->offset + ldfld2->offset;
				newIr->dst = ldfld2->dst;
				return true;
			}
			else if (op2 == HiOpcodeEnum::LdfldaVarVar)
			{
				// ldfldClass + ldflda => # (*a)[offset] + offset2 # LdfldClassLdflda @
				POCreateAddIR(newIr, LdfldClassLdfldaVarVar);
				newIr->obj = ldfld1->obj;
				newIr->offset1 = ldfld1->offset;
				newIr->offset2 = ldfld2->offset;
				newIr->dst = ldfld2->dst;
				return true;
			}
			else
			{
				return false;
			}
		}
		else if (IsLdfldValueTypeFamily(op1))
		{
			if (IsLdfldClassFamily(op2))
			{
				// ldfldStruct + ldfldClass # a[offset1][offset] # LdfldStructLdfldClass @
				POCreateAddIR(newIr, LdfldValueTypeLdfldClassVarVar_i1);
				newIr->type = (HiOpcodeEnum)((int32_t)ldfld2->type + (int32_t)HiOpcodeEnum::LdfldValueTypeLdfldClassVarVar_i1 - (int32_t)HiOpcodeEnum::LdfldVarVar_i1);
				newIr->obj = ldfld1->obj;
				newIr->offset1 = ldfld1->offset;
				newIr->offset2 = ldfld2->offset;
				newIr->dst = ldfld2->dst;
				return true;
			}
			else if (IsLdfldValueTypeFamily(op2))
			{
				// ldfldStruct + ldfldStruct => ldfldStruct(offset1+offset2) # a[offset1+offset2]
				POCreateAddIR(newIr, LdfldValueTypeVarVar_i1);
				newIr->type = ldfld2->type;
				newIr->obj = ldfld1->obj;
				newIr->offset = ldfld1->offset + ldfld2->offset;
				newIr->dst = ldfld2->dst;
				return true;
			}
			else if (op2 == HiOpcodeEnum::LdfldaVarVar)
			{
				return false;
			}
			else
			{
				return false;
			}
		}
		else if (op1 == HiOpcodeEnum::LdfldaVarVar)
		{
			if (IsLdfldClassFamily(op2))
			{
				// ldflda + ldfldClass => ldfldClass(offset1+offset2) # (*a)[offset1+offset2]
				POCreateAddIR(newIr, LdfldVarVar_i1);
				newIr->type = ldfld2->type;
				newIr->obj = ldfld1->obj;
				newIr->offset = ldfld1->offset + ldfld2->offset;
				newIr->dst = ldfld2->dst;
				return true;
			}
			else if (IsLdfldValueTypeFamily(op2))
			{
				return false;
			}
			else if (op2 == HiOpcodeEnum::LdfldaVarVar)
			{
				// ldflda + ldflda => ldflda(offset1+offset2) # (*a)+offset1+offset2
				POCreateAddIR(newIr, LdfldaVarVar);
				newIr->type = ldfld2->type;
				newIr->obj = ldfld1->obj;
				newIr->offset = ldfld1->offset + ldfld2->offset;
				newIr->dst = ldfld2->dst;
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
		return false;
	}

	static bool POF_Typeof(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		IRLdtokenVar* ldtoken = (IRLdtokenVar*)irs[0];
		IRCallNativeStatic_ret* getTypeHandle = (IRCallNativeStatic_ret*)irs[1];
		const MethodInfo* method = (const MethodInfo*)ctx.ctx->GetResolveData(getTypeHandle->methodInfo);
		if (strcmp(method->name, "GetTypeFromHandle"))
		{
			return false;
		}
		void* typeHandle = il2cpp::icalls::mscorlib::System::Type::internal_from_handle((intptr_t)ctx.ctx->GetResolveData(ldtoken->token));

		POCreateAddIR(ret, LdcVarConst_8);
		ret->src = (uint64_t)typeHandle;
		ret->dst = getTypeHandle->ret;
		return true;
	}

	static bool POF_ReduceStloc(const PeepholeOptimizationContext& ctx, interpreter::IRCommon** irs, int32_t count, interpreter::IRCommon** resultIrs, size_t& resultCount)
	{
		IRLdlocVarVar_size_8* out = (IRLdlocVarVar_size_8*)irs[1];
		return false;
	}

	static std::vector<HiOpcodeEnum> MakeOpRange(HiOpcodeEnum begin, HiOpcodeEnum end)
	{
		IL2CPP_ASSERT((int32_t)begin <= (int32_t)end);
		std::vector<HiOpcodeEnum> opRange;
		opRange.reserve((size_t)end - (size_t)begin + 1);
		for (size_t i = (size_t)begin, e = (size_t)end; i <= e; i++)
		{
			opRange.push_back((HiOpcodeEnum)i);
		}
		return opRange;
	}

	void PeepholeOptimizationGroup::Initialize()
	{
		CaseSearchingTree* cst = g_searchingTree = new (HYBRIDCLR_METADATA_MALLOC(sizeof(CaseSearchingTree))) CaseSearchingTree();

		std::vector<HiOpcodeEnum> binVarVarVarOps = MakeOpRange(HiOpcodeEnum::BinOpVarVarVar_Add_i4, HiOpcodeEnum::BinOpVarVarVar_Rem_f8);
		std::vector<HiOpcodeEnum> binVarVarConstOps = MakeOpRange(HiOpcodeEnum::BinOpVarVarConst_Add_i4, HiOpcodeEnum::BinOpVarConstVar_Rem_f8);
		std::vector<HiOpcodeEnum> ldfldClassVarVarOps = MakeOpRange(HiOpcodeEnum::LdfldVarVar_i1, HiOpcodeEnum::LdfldVarVar_size_64);
		std::vector<HiOpcodeEnum> ldfldValueTypeVarVarOps = MakeOpRange(HiOpcodeEnum::LdfldValueTypeVarVar_i1, HiOpcodeEnum::LdfldValueTypeVarVar_size_64);
		std::vector<HiOpcodeEnum> ldfldVarVarOps = ldfldClassVarVarOps;
		ldfldVarVarOps.insert(ldfldVarVarOps.end(), ldfldValueTypeVarVarOps.begin(), ldfldValueTypeVarVarOps.end());
		
		std::vector<HiOpcodeEnum> ldfldVarVarAndLdfldaOps = ldfldVarVarOps;
		ldfldVarVarAndLdfldaOps.push_back(HiOpcodeEnum::LdfldaVarVar);

		std::vector<HiOpcodeEnum> ldlocVarVarOps = MakeOpRange(HiOpcodeEnum::LdlocVarVar_size_8, HiOpcodeEnum::LdlocVarVar_size_64);
		std::vector<HiOpcodeEnum> stlocVarVarOps = MakeOpRange(HiOpcodeEnum::StlocVarVar_size_8, HiOpcodeEnum::StlocVarVar_size_64);
		std::vector<HiOpcodeEnum> notNullOps = {
			HiOpcodeEnum::BoxVarVar, HiOpcodeEnum::NewSystemObjectVar,
			HiOpcodeEnum::LdstrVar, HiOpcodeEnum::NewString, HiOpcodeEnum::NewString_2, HiOpcodeEnum::NewString_3,
			HiOpcodeEnum::CtorDelegate, HiOpcodeEnum::NewDelegate,
			HiOpcodeEnum::NewMdArrVarVar_length, HiOpcodeEnum::NewMdArrVarVar_length_bound, HiOpcodeEnum::NewArrVarVar,
			HiOpcodeEnum::LdsfldaFromFieldDataVarVar, HiOpcodeEnum::LdsfldaVarVar, HiOpcodeEnum::LdthreadlocalaVarVar, HiOpcodeEnum::LdlocVarAddress,
		};

		POCInfo pocs[] =
		{
			{
				{
					{HiOpcodeEnum::LdcVarConst_1, HiOpcodeEnum::LdcVarConst_2, HiOpcodeEnum::LdcVarConst_4, HiOpcodeEnum::LdcVarConst_8},
					{HiOpcodeEnum::StlocVarVar_size_8},
				},
				POF_LdcStloc,
			},
			{
				{
					ldlocVarVarOps,
					ldfldClassVarVarOps,
				},
				POF_LoadthisLdfld,
			},
			{
				{
					ldlocVarVarOps,
					binVarVarVarOps,
				},
				POF_LoadBinOp,
			},
			{
				{
					ldlocVarVarOps,
					ldlocVarVarOps,
					binVarVarVarOps,
				},
				POF_LoadLoadBinOp,
			},
			//{
			//	{
			//		ldlocVarVarOps,
			//		ldlocVarVarOps,
			//		binVarVarVarOps,
			//		stlocVarVarOps,
			//	},
			//	POF_LoadLoadBinOpStore,
			//},
			{
				{
					binVarVarVarOps,
					stlocVarVarOps,
				},
				POF_BinOpStore,
			},
			// ldc

			{
				{
					{HiOpcodeEnum::LdcVarConst_4, HiOpcodeEnum::LdcVarConst_8},
					binVarVarVarOps,
				},
				POF_LdcBinOp,
			},
			{
				{
					ldlocVarVarOps,
					{HiOpcodeEnum::LdcVarConst_4, HiOpcodeEnum::LdcVarConst_8},
					binVarVarVarOps,
				},
				POF_LoadLdcBinOp,
			},
			//{
			//	{
			//		ldlocVarVarOps,
			//		{HiOpcodeEnum::LdcVarConst_4, HiOpcodeEnum::LdcVarConst_8},
			//		binVarVarVarOps,
			//		stlocVarVarOps,
			//	},
			//	POF_LoadLdcBinOpStore,
			//},
			{
				{
					binVarVarConstOps,
					stlocVarVarOps,
				},
				POF_BinOpLdcStore,
			},
			// notNullIr + (brtrue|brfalse)
			{
				{
					notNullOps,
					{ HiOpcodeEnum::BranchFalseVar_i4, HiOpcodeEnum::BranchFalseVar_i8, HiOpcodeEnum::BranchTrueVar_i4, HiOpcodeEnum::BranchTrueVar_i8 },
				},
				POF_NotNullIrBrTrueFalse,
			},
			{
				{
					ldlocVarVarOps,
					ldfldVarVarAndLdfldaOps,
				},
				POF_LdlocLdfld,
			},
			{
				{
					ldfldVarVarAndLdfldaOps,
					ldlocVarVarOps,
				},
				POF_LdfldLdloc,
			},
			{
				{
					ldfldVarVarAndLdfldaOps,
					ldfldVarVarAndLdfldaOps,
				},
				POF_LdfldLdfld,
			},
			{
				{
					{HiOpcodeEnum::LdtokenVar},
					{HiOpcodeEnum::CallNativeStatic_ret},
				},
				POF_Typeof,
			},
		};

		il2cpp::os::FastAutoLock lock(&il2cpp::vm::g_MetadataLock);
		for (POCInfo pi : pocs)
		{
			cst->AddCase(pi.func, nullptr, pi.patterns);
		}
	}

#define MAX_TRANSLATED_RESULT_INSTRUMENT_COUNT 10

	void PeepholeOptimizationGroup::Process(const OptimizationContext& ctx)
	{
		PeepholeOptimizationContext pctx = {ctx.ctx};
		interpreter::IRCommon* resultInsts[MAX_TRANSLATED_RESULT_INSTRUMENT_COUNT];
		size_t resultCount;
		for (transform::IRBasicBlock* bb : *ctx.blocks)
		{
			std::vector<interpreter::IRCommon*>& insts = bb->insts;
			size_t appendIndex = 0;
			for (int32_t i = 0, n = (int32_t)insts.size(); i < n ; )
			{
				CaseSearchingNode* matchNode = g_searchingTree->FindCases(insts, i);
				bool processed = false;
				for (; matchNode; matchNode = matchNode->parent)
				{
					if (matchNode->cases.func)
					{
						OptimizationFuncNode* funcNode = &matchNode->cases;
						do
						{
							resultCount = 0;
							if (funcNode->func(pctx, &insts[i], matchNode->depth + 1, resultInsts, resultCount))
							{
								processed = true;
								IL2CPP_ASSERT(resultCount <= matchNode->depth + 1);
								i += matchNode->depth + 1 - (int32_t)resultCount;
								for (size_t j = 0; j < resultCount; j++)
								{
									insts[j + i] = resultInsts[j];
								}
								break;
							}
							else
							{
								IL2CPP_ASSERT(resultCount == 0);
							}
							funcNode = funcNode->next;
						} while (funcNode);
						
						if (processed)
						{
							break;
						}
					}
				}
				if (!processed)
				{
					insts[appendIndex++] = insts[i++];
				}
			}
			insts.resize(appendIndex);
		}
		
	}
}
}