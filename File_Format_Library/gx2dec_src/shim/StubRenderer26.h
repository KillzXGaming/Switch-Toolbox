#pragma once
// auto-generated no-op overrides for Cemu v2.6 Renderer pure virtuals
class StubRenderer : public Renderer
{
public:
	RendererAPI GetType()  override { return {}; }
	bool IsPadWindowActive()  override { return {}; }
	void ClearColorbuffer(bool padView)  override {}
	void DrawEmptyFrame(bool mainWindow)  override {}
	void SwapBuffers(bool swapTV, bool swapDRC)  override {}
	void DrawBackbufferQuad(LatteTextureView* texView, RendererOutputShader* shader, bool useLinearTexFilter, sint32 imageX, sint32 imageY, sint32 imageWidth, sint32 imageHeight, bool padView, bool clearBackground)  override {}
	bool BeginFrame(bool mainWindow)  override { return {}; }
	void Flush(bool waitIdle = false)  override {}
	void NotifyLatteCommandProcessorIdle()  override {}
	void ImguiEnd()  override {}
	ImTextureID GenerateTexture(const std::vector<uint8>& data, const Vector2i& size)  override { return {}; }
	void DeleteTexture(ImTextureID id)  override {}
	void DeleteFontTextures()  override {}
	void AppendOverlayDebugInfo()  override {}
	void renderTarget_setViewport(float x, float y, float width, float height, float nearZ, float farZ, bool halfZ = false)  override {}
	void renderTarget_setScissor(sint32 scissorX, sint32 scissorY, sint32 scissorWidth, sint32 scissorHeight)  override {}
	LatteCachedFBO* rendertarget_createCachedFBO(uint64 key)  override { return {}; }
	void rendertarget_deleteCachedFBO(LatteCachedFBO* fbo)  override {}
	void rendertarget_bindFramebufferObject(LatteCachedFBO* cfbo)  override {}
	void* texture_acquireTextureUploadBuffer(uint32 size)  override { return {}; }
	void texture_releaseTextureUploadBuffer(uint8* mem)  override {}
	TextureDecoder* texture_chooseDecodedFormat(Latte::E_GX2SURFFMT format, bool isDepth, Latte::E_DIM dim, uint32 width, uint32 height)  override { return {}; }
	void texture_clearSlice(LatteTexture* hostTexture, sint32 sliceIndex, sint32 mipIndex)  override {}
	void texture_loadSlice(LatteTexture* hostTexture, sint32 width, sint32 height, sint32 depth, void* pixelData, sint32 sliceIndex, sint32 mipIndex, uint32 compressedImageSize)  override {}
	void texture_clearColorSlice(LatteTexture* hostTexture, sint32 sliceIndex, sint32 mipIndex, float r, float g, float b, float a)  override {}
	void texture_clearDepthSlice(LatteTexture* hostTexture, uint32 sliceIndex, sint32 mipIndex, bool clearDepth, bool clearStencil, float depthValue, uint32 stencilValue)  override {}
	LatteTexture* texture_createTextureEx(Latte::E_DIM dim, MPTR physAddress, MPTR physMipAddress, Latte::E_GX2SURFFMT format, uint32 width, uint32 height, uint32 depth, uint32 pitch, uint32 mipLevels, uint32 swizzle, Latte::E_HWTILEMODE tileMode, bool isDepth)  override { return {}; }
	void texture_setLatteTexture(LatteTextureView* textureView, uint32 textureUnit)  override {}
	void texture_copyImageSubData(LatteTexture* src, sint32 srcMip, sint32 effectiveSrcX, sint32 effectiveSrcY, sint32 srcSlice, LatteTexture* dst, sint32 dstMip, sint32 effectiveDstX, sint32 effectiveDstY, sint32 dstSlice, sint32 effectiveCopyWidth, sint32 effectiveCopyHeight, sint32 srcDepth)  override {}
	LatteTextureReadbackInfo* texture_createReadback(LatteTextureView* textureView)  override { return {}; }
	void surfaceCopy_copySurfaceWithFormatConversion(LatteTexture* sourceTexture, sint32 srcMip, sint32 srcSlice, LatteTexture* destinationTexture, sint32 dstMip, sint32 dstSlice, sint32 width, sint32 height)  override {}
	void bufferCache_init(const sint32 bufferSize)  override {}
	void bufferCache_upload(uint8* buffer, sint32 size, uint32 bufferOffset)  override {}
	void bufferCache_copy(uint32 srcOffset, uint32 dstOffset, uint32 size)  override {}
	void bufferCache_copyStreamoutToMainBuffer(uint32 srcOffset, uint32 dstOffset, uint32 size)  override {}
	void buffer_bindVertexBuffer(uint32 bufferIndex, uint32 offset, uint32 size)  override {}
	void buffer_bindUniformBuffer(LatteConst::ShaderType shaderType, uint32 bufferIndex, uint32 offset, uint32 size)  override {}
	RendererShader* shader_create(RendererShader::ShaderType type, uint64 baseHash, uint64 auxHash, const std::string& source, bool compileAsync, bool isGfxPackSource)  override { return {}; }
	void streamout_setupXfbBuffer(uint32 bufferIndex, sint32 ringBufferOffset, uint32 rangeAddr, uint32 rangeSize)  override {}
	void streamout_begin()  override {}
	void streamout_rendererFinishDrawcall()  override {}
	void draw_beginSequence()  override {}
	void draw_execute(uint32 baseVertex, uint32 baseInstance, uint32 instanceCount, uint32 count, MPTR indexDataMPTR, Latte::LATTE_VGT_DMA_INDEX_TYPE::E_INDEX_TYPE indexType, bool isFirst)  override {}
	void draw_endSequence()  override {}
	IndexAllocation indexData_reserveIndexMemory(uint32 size)  override { return {}; }
	void indexData_releaseIndexMemory(IndexAllocation& allocation)  override {}
	void indexData_uploadIndexMemory(IndexAllocation& allocation)  override {}
	LatteQueryObject* occlusionQuery_create()  override { return {}; }
	void occlusionQuery_destroy(LatteQueryObject* queryObj)  override {}
	void occlusionQuery_flush()  override {}
	void occlusionQuery_updateState()  override {}
};
