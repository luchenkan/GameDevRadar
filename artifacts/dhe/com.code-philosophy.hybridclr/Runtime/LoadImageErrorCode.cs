
namespace HybridCLR
{
    public enum LoadImageErrorCode
	{
		OK = 0,
		BAD_IMAGE, // invalid dll file
		NOT_IMPLEMENT, // not implement feature
		AOT_ASSEMBLY_NOT_FIND, // AOT assembly not found
		HOMOLOGOUS_ONLY_SUPPORT_AOT_ASSEMBLY, // can not load supplementary metadata assembly for non-AOT assembly
		HOMOLOGOUS_ASSEMBLY_HAS_LOADED, // can not load supplementary metadata assembly for the same assembly
		INVALID_HOMOLOGOUS_MODE, // invalid homologous image mode
		PDB_BAD_FILE, // invalid pdb file
		UNKNOWN_IMAGE_FORMAT,
		UNSUPPORT_FORMAT_VERSION,
        UNMATCH_FORMAT_VARIANT,
		UNSUPPORT_ENCRYPTION_ALGORHITHM,
		DHE_NOT_DIFFERENTIAL_HYBRID_ASSEMBLY,
		DHE_HAS_BEEN_LOADED,
        DHE_BAD_DHAO_DATA,
        DHE_BAD_META_VERSION_FILE,
	};
}

