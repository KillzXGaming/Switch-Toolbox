// External functions called by the Latte shader decompiler that normally live in
// Cemu source files we do not compile (LatteShader.cpp, LatteRenderTarget.cpp,
// LatteTextureLegacy.cpp). The bodies here are pulled verbatim (or near-verbatim,
// see notes) from Cemu so the decompiler's actual GLSL generation is unchanged.

#include "Cafe/HW/Latte/Core/LatteConst.h"
#include "Cafe/HW/Latte/ISA/RegDefines.h"
#include "Cafe/HW/Latte/ISA/LatteReg.h"
#include "Cafe/HW/Latte/Core/Latte.h"
#include "Cafe/HW/Latte/LegacyShaderDecompiler/LatteDecompiler.h"
#include "Cafe/HW/Latte/Core/LatteShader.h"
#include "Cafe/HW/Latte/Core/LatteCachedFBO.h"

// ----------------------------------------------------------------------------
// PS input table (verbatim from Cemu/src/Cafe/HW/Latte/Core/LatteShader.cpp)
// Reads only SPI_PS_IN_CONTROL_0 and SPI_PS_INPUT_CNTL_0..N registers.
// ----------------------------------------------------------------------------

static LatteShaderPSInputTable _activePSImportTable;

void LatteShader_CreatePSInputTable(LatteShaderPSInputTable* psInputTable, uint32* contextRegisters)
{
	// PS control
	uint32 psControl0 = contextRegisters[mmSPI_PS_IN_CONTROL_0];
	uint32 spi0_positionEnable = (psControl0 >> 8) & 1;
	uint32 spi0_positionCentroid = (psControl0 >> 9) & 1;
	cemu_assert_debug(spi0_positionCentroid == 0); // controls gl_FragCoord
	uint32 spi0_positionAddr = (psControl0 >> 10) & 0x1F; // controls gl_FragCoord
	uint32 spi0_paramGen = (psControl0 >> 15) & 0xF; // used for gl_PointCoords
	uint32 spi0_paramGenAddr = (psControl0 >> 19) & 0x7F;
	sint32 importIndex = 0;

	// VS/GS parameters
	uint32 numPSInputs = contextRegisters[mmSPI_PS_IN_CONTROL_0] & 0x3F;
	uint64 key = 0;

	if (spi0_positionEnable)
	{
		key += (uint64)spi0_positionAddr + 1;
	}

	// parameter gen
	if (spi0_paramGen != 0)
	{
		key += std::rotr<uint64>(spi0_paramGen, 7);
		key += std::rotr<uint64>(spi0_paramGenAddr, 3);
		psInputTable->paramGen = spi0_paramGen;
		psInputTable->paramGenGPR = spi0_paramGenAddr;
	}
	else
	{
		psInputTable->paramGen = 0;
	}

	// semantic imports from vertex shader
	cemu_assert_debug(numPSInputs <= GPU7_PS_MAX_INPUTS);
	numPSInputs = std::min<uint32>(numPSInputs, GPU7_PS_MAX_INPUTS);

	for (uint32 f = 0; f < numPSInputs; f++)
	{
		uint32 psInputControl = contextRegisters[mmSPI_PS_INPUT_CNTL_0 + f];
		uint32 psSemanticId = (psInputControl & 0xFF);

		uint8 defaultValue = (psInputControl >> 8) & 3;
		cemu_assert_debug(defaultValue <= 1);

		uint32 uknBits = psInputControl & ~((0xFF) | (0x3 << 8) | (1 << 10) | (1 << 12));
		uknBits &= ~0x800; // FLAT_SHADE
		cemu_assert_debug(psSemanticId != 0xFF);

		key += (uint64)psInputControl;
		key = std::rotl<uint64>(key, 7);
		if (spi0_positionEnable && f == spi0_positionAddr)
		{
			psInputTable->import[f].semanticId = LATTE_ANALYZER_IMPORT_INDEX_SPIPOSITION;
			psInputTable->import[f].isFlat = false;
			psInputTable->import[f].isNoPerspective = false;
			key += (uint64)0x33;
		}
		else
		{
			psInputTable->import[f].semanticId = psSemanticId;
			psInputTable->import[f].isFlat = (psInputControl & (1 << 10)) != 0;
			psInputTable->import[f].isNoPerspective = (psInputControl & (1 << 12)) != 0;
		}
	}
	psInputTable->key = key;
	psInputTable->count = numPSInputs;
}

LatteShaderPSInputTable* LatteSHRC_GetPSInputTable()
{
	return &_activePSImportTable;
}

// ----------------------------------------------------------------------------
// LatteMRT helpers (from Cemu/src/Cafe/HW/Latte/Core/LatteRenderTarget.cpp)
// Pure register-bit reads. GetActiveColorBufferMask omits the render-target
// scissor-size culling step (see NOTE) which depends on runtime FBO dimensions
// not available in a standalone decompile and does not affect generated GLSL.
// ----------------------------------------------------------------------------

uint8 LatteMRT::GetActiveColorBufferMask(const LatteDecompilerShader* pixelShader, const LatteContextRegister& lcr)
{
	const uint32* regView = lcr.GetRawView();

	uint8 colorBufferMask = 0;
	for (uint32 i = 0; i < 8; i++)
	{
		if (regView[mmCB_COLOR0_BASE + i] != MPTR_NULL)
			colorBufferMask |= (1 << i);
	}
	// check if color buffer output is active
	const Latte::LATTE_CB_COLOR_CONTROL& colorControlReg = lcr.CB_COLOR_CONTROL;
	uint32 colorBufferDisable = colorControlReg.get_SPECIAL_OP() == Latte::LATTE_CB_COLOR_CONTROL::E_SPECIALOP::DISABLE;
	if (colorBufferDisable)
		return 0;
	// combine color buffer mask with pixel output mask from pixel shader
	colorBufferMask &= (pixelShader ? pixelShader->pixelColorOutputMask : 0);
	// combine color buffer mask with color channel mask from mmCB_TARGET_MASK (disable render buffer if all colors are blocked)
	uint32 channelTargetMask = lcr.CB_TARGET_MASK.get_MASK();
	for (uint32 i = 0; i < 8; i++)
	{
		if (((channelTargetMask >> (i * 4)) & 0xF) == 0)
			colorBufferMask &= ~(1 << i);
	}
	// NOTE: Cemu additionally drops attachments whose color buffer is smaller than the
	// active scissor rect. That culling needs live FBO sizes and only affects which
	// render targets are bound at draw time, not the decompiled shader source, so it is
	// intentionally omitted in this standalone tool.
	return colorBufferMask;
}

bool LatteMRT::GetActiveDepthBufferMask(const LatteContextRegister& lcr)
{
	bool depthBufferMask = true;
	bool depthEnable = lcr.DB_DEPTH_CONTROL.get_Z_ENABLE();
	bool stencilTestEnable = lcr.DB_DEPTH_CONTROL.get_STENCIL_ENABLE();
	bool backStencilEnable = lcr.DB_DEPTH_CONTROL.get_BACK_STENCIL_ENABLE();

	if (!depthEnable && !stencilTestEnable && !backStencilEnable)
		depthBufferMask = false;

	return depthBufferMask;
}

static const uint32 _colorBufferFormatBits[] =
{
	0,      // 0
	0x200,  // 1
	0,      // 2
	0,      // 3
	0x100,  // 4
	0x300,  // 5
	0x400,  // 6
	0x800,  // 7
};

Latte::E_GX2SURFFMT LatteMRT::GetColorBufferFormat(const uint32 index, const LatteContextRegister& lcr)
{
	cemu_assert_debug(index < Latte::GPU_LIMITS::NUM_COLOR_ATTACHMENTS);
	uint32 regColorInfo = lcr.GetRawView()[mmCB_COLOR0_INFO + index];
	uint32 colorBufferFormat = (regColorInfo >> 2) & 0x3F; // base HW format
	uint32 numberType = (regColorInfo >> 12) & 7;
	colorBufferFormat |= _colorBufferFormatBits[numberType];
	return (Latte::E_GX2SURFFMT)colorBufferFormat;
}

// ----------------------------------------------------------------------------
// LatteTexture_ReconstructGX2Format (verbatim from Cemu/.../LatteTextureLegacy.cpp)
// Pure register-bit read of the texture resource words.
// ----------------------------------------------------------------------------

Latte::E_GX2SURFFMT LatteTexture_ReconstructGX2Format(const Latte::LATTE_SQ_TEX_RESOURCE_WORD1_N& texUnitWord1, const Latte::LATTE_SQ_TEX_RESOURCE_WORD4_N& texUnitWord4)
{
	Latte::E_GX2SURFFMT gx2Format = (Latte::E_GX2SURFFMT)texUnitWord1.get_DATA_FORMAT();
	auto nfa = texUnitWord4.get_NUM_FORM_ALL();
	if (nfa == Latte::LATTE_SQ_TEX_RESOURCE_WORD4_N::E_NUM_FORMAT_ALL::NUM_FORMAT_SCALED)
		gx2Format |= Latte::E_GX2SURFFMT::FMT_BIT_FLOAT;
	else if (nfa == Latte::LATTE_SQ_TEX_RESOURCE_WORD4_N::E_NUM_FORMAT_ALL::NUM_FORMAT_INT)
		gx2Format |= Latte::E_GX2SURFFMT::FMT_BIT_INT;

	if (texUnitWord4.get_FORCE_DEGAMMA())
		gx2Format |= Latte::E_GX2SURFFMT::FMT_BIT_SRGB;

	if (texUnitWord4.get_FORMAT_COMP_X() == Latte::LATTE_SQ_TEX_RESOURCE_WORD4_N::E_FORMAT_COMP::COMP_SIGNED)
		gx2Format |= Latte::E_GX2SURFFMT::FMT_BIT_SIGNED;

	return gx2Format;
}

// v2.6 renamed entry point: fills the active PS input table from context registers
void LatteShader_UpdatePSInputs(uint32* contextRegisters)
{
	LatteShader_CreatePSInputTable(&_activePSImportTable, contextRegisters);
}
