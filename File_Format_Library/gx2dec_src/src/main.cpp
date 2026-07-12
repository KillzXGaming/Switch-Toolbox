// Standalone GX2 (Wii U Latte) shader -> GLSL decompiler.
//
// usage: gx2dec <vs|ps> <program.bin> <regs.bin> [fetch.bin]
//   program.bin  raw shader microcode bytes
//   regs.bin     raw context-register bytes, loaded into a uint32[LATTE_MAX_REGISTER] (zero-padded)
//   fetch.bin    (vs only) raw fetch-shader microcode bytes

#include "Cafe/HW/Latte/Core/LatteConst.h"
#include "Cafe/HW/Latte/ISA/LatteReg.h"
#include "Cafe/HW/Latte/Core/Latte.h"
#include "Cafe/HW/Latte/Renderer/Renderer.h"
#include "Cafe/HW/Latte/LegacyShaderDecompiler/LatteDecompiler.h"
#include "Cafe/HW/Latte/Core/FetchShader.h"
#include "Cafe/HW/Latte/Core/LatteShader.h"
#include "util/helpers/StringBuf.h"

#include <cstdio>
#include <vector>
#include <string>

// register count the typed LatteContextRegister overlay spans, plus padding (see task spec)
static constexpr size_t kRegArrayCount = LATTE_MAX_REGISTER + 9; // 0x10009

std::unique_ptr<Renderer> CreateStubRenderer();

static bool readFile(const char* path, std::vector<uint8>& out)
{
	FILE* f = fopen(path, "rb");
	if (!f)
		return false;
	fseek(f, 0, SEEK_END);
	long sz = ftell(f);
	fseek(f, 0, SEEK_SET);
	if (sz < 0)
	{
		fclose(f);
		return false;
	}
	out.resize((size_t)sz);
	size_t rd = sz > 0 ? fread(out.data(), 1, (size_t)sz, f) : 0;
	fclose(f);
	return rd == (size_t)sz;
}

static void printUsage()
{
	fprintf(stderr,
		"gx2dec: GX2 (Wii U Latte) shader to GLSL decompiler\n"
		"usage: gx2dec <vs|ps> <program.bin> <regs.bin> [fetch.bin]\n"
		"  vs : decompile a vertex shader (requires fetch.bin)\n"
		"  ps : decompile a pixel shader\n");
}

// Dump the host binding/uniform contract to stderr (stdout stays pure GLSL). This is what the GL host
// must provide: which textures/UBOs bind where, and which uniform-buffer vec4 each uf_remapped slot reads.
static void dumpMapping(LatteDecompilerOutput_t& out)
{
	LatteDecompilerShader* s = out.shader;
	if (!s)
		return;
	auto& rm = s->resourceMapping;
	fprintf(stderr, "=== MAPPING ===\n");
	fprintf(stderr, "uniformMode=%u pixelColorOutputMask=0x%x\n", s->uniformMode, s->pixelColorOutputMask);
	fprintf(stderr, "textures (%u):\n", (unsigned)s->textureUnitListCount);
	for (uint32 i = 0; i < s->textureUnitListCount; i++)
	{
		uint8 unit = s->textureUnitList[i];
		fprintf(stderr, "  texUnit=%u dim=%d binding=%d\n", (unsigned)unit, (int)s->textureUnitDim[unit], (int)rm.textureUnitToBindingPoint[unit]);
	}
	fprintf(stderr, "uniformVarsBufferBindingPoint=%d\n", (int)rm.uniformVarsBufferBindingPoint);
	for (int i = 0; i < LATTE_NUM_MAX_UNIFORM_BUFFERS; i++)
		if (rm.uniformBuffersBindingPoint[i] >= 0)
			fprintf(stderr, "  uniformBuffer[%d] -> binding %d\n", i, (int)rm.uniformBuffersBindingPoint[i]);
	fprintf(stderr, "remappedUniformEntries (%zu)  [uf_remapped slot] <- src:\n", s->list_remappedUniformEntries.size());
	for (auto& e : s->list_remappedUniformEntries)
		fprintf(stderr, "  uf_remapped[%u] <- %s bank=%u vec4Index=%u\n",
			e.mappedIndex, e.isRegister ? "REGISTER" : "BUFFER", (unsigned)e.kcacheBankId, e.index);
	auto& uo = out.uniformOffsetsGL;
	fprintf(stderr, "uniformOffsetsGL: remapped=%d fragCoordScale=%d alphaTestRef=%d uniformRegister=%d countReg=%d endOfBlock=%d\n",
		uo.offset_remapped, uo.offset_fragCoordScale, uo.offset_alphaTestRef, uo.offset_uniformRegister, uo.count_uniformRegister, uo.offset_endOfBlock);
	bool anyAttr = false;
	for (int i = 0; i < LATTE_NUM_MAX_ATTRIBUTE_LOCATIONS; i++)
		if (rm.attributeMapping[i] >= 0) { if (!anyAttr) { fprintf(stderr, "attributeMapping (semanticId -> hostLoc):\n"); anyAttr = true; } fprintf(stderr, "  sem=%d -> loc %d\n", i, (int)rm.attributeMapping[i]); }
	fprintf(stderr, "=== END MAPPING ===\n");
}

int main(int argc, char** argv)
{
	if (argc < 4)
	{
		printUsage();
		return 1;
	}

	std::string mode = argv[1];
	const char* programPath = argv[2];
	const char* regsPath = argv[3];
	const char* fetchPath = (argc >= 5) ? argv[4] : nullptr;

	if (mode != "vs" && mode != "ps")
	{
		fprintf(stderr, "error: mode must be 'vs' or 'ps'\n");
		printUsage();
		return 1;
	}

	std::vector<uint8> programData;
	if (!readFile(programPath, programData))
	{
		fprintf(stderr, "error: cannot read program file '%s'\n", programPath);
		return 1;
	}

	std::vector<uint8> regsBytes;
	if (!readFile(regsPath, regsBytes))
	{
		fprintf(stderr, "error: cannot read regs file '%s'\n", regsPath);
		return 1;
	}

	// load registers into a zero-padded uint32 array of the size the decompiler expects
	std::vector<uint32> contextRegisters(kRegArrayCount, 0u);
	{
		size_t copyBytes = std::min(regsBytes.size(), kRegArrayCount * sizeof(uint32));
		memcpy(contextRegisters.data(), regsBytes.data(), copyBytes);
	}

	// minimal global state
	g_renderer = CreateStubRenderer();
	LatteGPUState.glVendor = GLVENDOR_NVIDIA;
	// mirror the registers into LatteGPUState (FetchShader reads from LatteGPUState.contextRegister)
	memcpy(LatteGPUState.contextRegister, contextRegisters.data(), LATTE_MAX_REGISTER * sizeof(uint32));

	// Populate the PS input table from the SPI_PS registers. The decompiler reads it via
	// LatteSHRC_GetPSInputTable() to emit the PS input varyings + their loads at the top of main()
	// (and, for VS, to know which outputs the paired PS consumes). Cemu fills this during shader bind;
	// standalone we must do it, or the table stays empty and every PS input register reads 0.
	LatteShader_UpdatePSInputs(contextRegisters.data());

	LatteDecompilerOptions opt{};
	opt.strictMul = true;
	opt.usesGeometryShader = false;

	LatteDecompilerOutput_t out{};

	if (mode == "ps")
	{
		LatteDecompiler_DecompilePixelShader(0x1234, contextRegisters.data(), programData.data(), (uint32)programData.size(), opt, &out);
	}
	else // vs
	{
		if (!fetchPath)
		{
			fprintf(stderr, "error: vertex shader mode requires a fetch shader (fetch.bin)\n");
			return 1;
		}
		std::vector<uint8> fetchData;
		if (!readFile(fetchPath, fetchData))
		{
			fprintf(stderr, "error: cannot read fetch file '%s'\n", fetchPath);
			return 1;
		}
		LatteFetchShader* fetchShader = LatteShaderRecompiler_createFetchShader(
			0xF7C0,
			contextRegisters.data(),
			(uint32*)fetchData.data(),
			(uint32)fetchData.size());
		LatteDecompiler_DecompileVertexShader(0x1234, contextRegisters.data(), programData.data(), (uint32)programData.size(), fetchShader, opt, &out);
	}

	dumpMapping(out);

	if (out.shader && out.shader->strBuf_shaderSource)
	{
		fputs(out.shader->strBuf_shaderSource->c_str(), stdout);
		fputc('\n', stdout);
		return 0;
	}

	fprintf(stderr, "<null output>\n");
	return 2;
}
