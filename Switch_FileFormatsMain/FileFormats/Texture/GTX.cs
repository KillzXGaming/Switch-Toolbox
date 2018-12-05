using System;
using System.Collections.Generic;

namespace FirstPlugin
{
    public class GTX
    {
        //Some enums and parts from https://github.com/jam1garner/Smash-Forge/blob/master/Smash%20Forge/Filetypes/Textures/GTX.cs
        public class GX2Surface
        {
            public uint dim;
            public uint width;
            public uint height;
            public uint depth;
            public uint numMips;
            public uint firstSlice;
            public uint numSlices;
            public uint format;
            public uint aa;
            public uint use;
            public int resourceFlags;
            public uint imageSize;
            public uint imagePtr;
            public int MemPtr;
            public uint mipSize;
            public uint mipPtr;
            public uint tileMode;
            public uint swizzle;
            public uint alignment;
            public uint pitch;
            public uint bpp;
            public uint imageCount;
            public uint firstMip;

            public byte[] data;
            public byte[] mipData;

            public uint[] mipOffset;
            public byte[] compSel;
            public uint[] texRegs;
        };

        public static int m_configFlags = 4;

        public static uint expPitch = 0;
        public static uint expHeight = 0;
        public static uint expNumSlices = 0;

        public class surfaceIn
        {
            public uint size = 0;
            public uint tileMode = 0;
            public uint format = 0;
            public uint bpp = 0;
            public uint numSamples = 0;
            public uint width = 0;
            public uint height = 0;
            public uint numSlices = 0;
            public uint slice = 0;
            public uint mipLevel = 0;
            public Flags flags = new Flags();
            public uint numFrags = 0;
            public uint tileType = 0;
            public TileInfo pTileInfo = new TileInfo();
            public int tileIndex = 0;
        }
        public class surfaceOut
        {
            public uint size = 0;
            public uint pitch = 0;
            public uint height = 0;
            public uint depth = 0;
            public long surfSize = 0;
            public uint tileMode = 0;
            public uint baseAlign = 0;
            public uint pitchAlign = 0;
            public uint heightAlign = 0;
            public uint depthAlign = 0;
            public uint bpp = 0;
            public uint pixelPitch = 0;
            public uint pixelHeight = 0;
            public uint pixelBits = 0;
            public uint sliceSize = 0;
            public uint pitchTileMax = 0;
            public uint heightTileMax = 0;
            public uint sliceTileMax = 0;
            public uint tileType = 0;
            public TileInfo pTileInfo = new TileInfo();
            public int tileIndex = 0;
        }

        public class Flags
        {
            public uint value = 0;
        }

        public class TileInfo
        {
            public int banks = 0;
            public int bankWidth = 0;
            public int bankHeight = 0;
            public int macroAspectRatio = 0;
            public int tileSplitBytes = 0;
            public int pipeConfig = 0;
        }

        static surfaceIn pIn = new surfaceIn();
        static surfaceOut pOut = new surfaceOut();

        public enum GX2SurfaceDimension
        {
            GX2_SURFACE_DIM_1D = 0x0,
            GX2_SURFACE_DIM_2D = 0x1,
            GX2_SURFACE_DIM_3D = 0x2,
            GX2_SURFACE_DIM_CUBE = 0x3,
            GX2_SURFACE_DIM_1D_ARRAY = 0x4,
            GX2_SURFACE_DIM_2D_ARRAY = 0x5,
            GX2_SURFACE_DIM_2D_MSAA = 0x6,
            GX2_SURFACE_DIM_2D_MSAA_ARRAY = 0x7,
            GX2_SURFACE_DIM_FIRST = 0x0,
            GX2_SURFACE_DIM_LAST = 0x7,
        };
        public enum GX2SurfaceFormat
        {
            GX2_SURFACE_FORMAT_INVALID = 0x0,
            GX2_SURFACE_FORMAT_TC_R8_UNORM = 0x1,
            GX2_SURFACE_FORMAT_TC_R8_UINT = 0x101,
            GX2_SURFACE_FORMAT_TC_R8_SNORM = 0x201,
            GX2_SURFACE_FORMAT_TC_R8_SINT = 0x301,
            GX2_SURFACE_FORMAT_T_R4_G4_UNORM = 0x2,
            GX2_SURFACE_FORMAT_TCD_R16_UNORM = 0x5,
            GX2_SURFACE_FORMAT_TC_R16_UINT = 0x105,
            GX2_SURFACE_FORMAT_TC_R16_SNORM = 0x205,
            GX2_SURFACE_FORMAT_TC_R16_SINT = 0x305,
            GX2_SURFACE_FORMAT_TC_R16_FLOAT = 0x806,
            GX2_SURFACE_FORMAT_TC_R8_G8_UNORM = 0x7,
            GX2_SURFACE_FORMAT_TC_R8_G8_UINT = 0x107,
            GX2_SURFACE_FORMAT_TC_R8_G8_SNORM = 0x207,
            GX2_SURFACE_FORMAT_TC_R8_G8_SINT = 0x307,
            GX2_SURFACE_FORMAT_TCS_R5_G6_B5_UNORM = 0x8,
            GX2_SURFACE_FORMAT_TC_R5_G5_B5_A1_UNORM = 0xA,
            GX2_SURFACE_FORMAT_TC_R4_G4_B4_A4_UNORM = 0xB,
            GX2_SURFACE_FORMAT_TC_A1_B5_G5_R5_UNORM = 0xC,
            GX2_SURFACE_FORMAT_TC_R32_UINT = 0x10D,
            GX2_SURFACE_FORMAT_TC_R32_SINT = 0x30D,
            GX2_SURFACE_FORMAT_TCD_R32_FLOAT = 0x80E,
            GX2_SURFACE_FORMAT_TC_R16_G16_UNORM = 0xF,
            GX2_SURFACE_FORMAT_TC_R16_G16_UINT = 0x10F,
            GX2_SURFACE_FORMAT_TC_R16_G16_SNORM = 0x20F,
            GX2_SURFACE_FORMAT_TC_R16_G16_SINT = 0x30F,
            GX2_SURFACE_FORMAT_TC_R16_G16_FLOAT = 0x810,
            GX2_SURFACE_FORMAT_D_D24_S8_UNORM = 0x11,
            GX2_SURFACE_FORMAT_T_R24_UNORM_X8 = 0x11,
            GX2_SURFACE_FORMAT_T_X24_G8_UINT = 0x111,
            GX2_SURFACE_FORMAT_D_D24_S8_FLOAT = 0x811,
            GX2_SURFACE_FORMAT_TC_R11_G11_B10_FLOAT = 0x816,
            GX2_SURFACE_FORMAT_TCS_R10_G10_B10_A2_UNORM = 0x19,
            GX2_SURFACE_FORMAT_TC_R10_G10_B10_A2_UINT = 0x119,
            GX2_SURFACE_FORMAT_T_R10_G10_B10_A2_SNORM = 0x219,
            GX2_SURFACE_FORMAT_TC_R10_G10_B10_A2_SNORM = 0x219,
            GX2_SURFACE_FORMAT_TC_R10_G10_B10_A2_SINT = 0x319,
            GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_UNORM = 0x1A,
            GX2_SURFACE_FORMAT_TC_R8_G8_B8_A8_UINT = 0x11A,
            GX2_SURFACE_FORMAT_TC_R8_G8_B8_A8_SNORM = 0x21A,
            GX2_SURFACE_FORMAT_TC_R8_G8_B8_A8_SINT = 0x31A,
            GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_SRGB = 0x41A,
            GX2_SURFACE_FORMAT_TCS_A2_B10_G10_R10_UNORM = 0x1B,
            GX2_SURFACE_FORMAT_TC_A2_B10_G10_R10_UINT = 0x11B,
            GX2_SURFACE_FORMAT_D_D32_FLOAT_S8_UINT_X24 = 0x81C,
            GX2_SURFACE_FORMAT_T_R32_FLOAT_X8_X24 = 0x81C,
            GX2_SURFACE_FORMAT_T_X32_G8_UINT_X24 = 0x11C,
            GX2_SURFACE_FORMAT_TC_R32_G32_UINT = 0x11D,
            GX2_SURFACE_FORMAT_TC_R32_G32_SINT = 0x31D,
            GX2_SURFACE_FORMAT_TC_R32_G32_FLOAT = 0x81E,
            GX2_SURFACE_FORMAT_TC_R16_G16_B16_A16_UNORM = 0x1F,
            GX2_SURFACE_FORMAT_TC_R16_G16_B16_A16_UINT = 0x11F,
            GX2_SURFACE_FORMAT_TC_R16_G16_B16_A16_SNORM = 0x21F,
            GX2_SURFACE_FORMAT_TC_R16_G16_B16_A16_SINT = 0x31F,
            GX2_SURFACE_FORMAT_TC_R16_G16_B16_A16_FLOAT = 0x820,
            GX2_SURFACE_FORMAT_TC_R32_G32_B32_A32_UINT = 0x122,
            GX2_SURFACE_FORMAT_TC_R32_G32_B32_A32_SINT = 0x322,
            GX2_SURFACE_FORMAT_TC_R32_G32_B32_A32_FLOAT = 0x823,
            GX2_SURFACE_FORMAT_T_BC1_UNORM = 0x31,
            GX2_SURFACE_FORMAT_T_BC1_SRGB = 0x431,
            GX2_SURFACE_FORMAT_T_BC2_UNORM = 0x32,
            GX2_SURFACE_FORMAT_T_BC2_SRGB = 0x432,
            GX2_SURFACE_FORMAT_T_BC3_UNORM = 0x33,
            GX2_SURFACE_FORMAT_T_BC3_SRGB = 0x433,
            GX2_SURFACE_FORMAT_T_BC4_UNORM = 0x34,
            GX2_SURFACE_FORMAT_T_BC4_SNORM = 0x234,
            GX2_SURFACE_FORMAT_T_BC5_UNORM = 0x35,
            GX2_SURFACE_FORMAT_T_BC5_SNORM = 0x235,
            GX2_SURFACE_FORMAT_T_NV12_UNORM = 0x81,
            GX2_SURFACE_FORMAT_FIRST = 0x1,
            GX2_SURFACE_FORMAT_LAST = 0x83F,
        };
        public enum GX2AAMode
        {
            GX2_AA_MODE_1X = 0x0,
            GX2_AA_MODE_2X = 0x1,
            GX2_AA_MODE_4X = 0x2,
            GX2_AA_MODE_8X = 0x3,
            GX2_AA_MODE_FIRST = 0x0,
            GX2_AA_MODE_LAST = 0x3,
        };
        public enum GX2SurfaceUse : uint
        {
            GX2_SURFACE_USE_TEXTURE = 0x1,
            GX2_SURFACE_USE_COLOR_BUFFER = 0x2,
            GX2_SURFACE_USE_DEPTH_BUFFER = 0x4,
            GX2_SURFACE_USE_SCAN_BUFFER = 0x8,
            GX2_SURFACE_USE_FTV = 0x80000000,
            GX2_SURFACE_USE_COLOR_BUFFER_TEXTURE = 0x3,
            GX2_SURFACE_USE_DEPTH_BUFFER_TEXTURE = 0x5,
            GX2_SURFACE_USE_COLOR_BUFFER_FTV = 0x80000002,
            GX2_SURFACE_USE_COLOR_BUFFER_TEXTURE_FTV = 0x80000003,
            GX2_SURFACE_USE_FIRST = 0x1,
            GX2_SURFACE_USE_LAST = 0x8,
        };
        public enum GX2RResourceFlags
        {
            GX2R_RESOURCE_FLAGS_NONE = 0x0,
            GX2R_BIND_NONE = 0x0,
            GX2R_BIND_TEXTURE = 0x1,
            GX2R_BIND_COLOR_BUFFER = 0x2,
            GX2R_BIND_DEPTH_BUFFER = 0x4,
            GX2R_BIND_SCAN_BUFFER = 0x8,
            GX2R_BIND_VERTEX_BUFFER = 0x10,
            GX2R_BIND_INDEX_BUFFER = 0x20,
            GX2R_BIND_UNIFORM_BLOCK = 0x40,
            GX2R_BIND_SHADER_PROGRAM = 0x80,
            GX2R_BIND_STREAM_OUTPUT = 0x100,
            GX2R_BIND_DISPLAY_LIST = 0x200,
            GX2R_BIND_GS_RING = 0x400,
            GX2R_USAGE_NONE = 0x0,
            GX2R_USAGE_CPU_READ = 0x800,
            GX2R_USAGE_CPU_WRITE = 0x1000,
            GX2R_USAGE_GPU_READ = 0x2000,
            GX2R_USAGE_GPU_WRITE = 0x4000,
            GX2R_USAGE_DMA_READ = 0x8000,
            GX2R_USAGE_DMA_WRITE = 0x10000,
            GX2R_USAGE_FORCE_MEM1 = 0x20000,
            GX2R_USAGE_FORCE_MEM2 = 0x40000,
            GX2R_USAGE_MEM_DEFAULT = 0x0,
            GX2R_USAGE_CPU_READWRITE = 0x1800,
            GX2R_USAGE_GPU_READWRITE = 0x6000,
            GX2R_USAGE_NON_CPU_WRITE = 0x14000,
            GX2R_OPTION_NONE = 0x0,
            GX2R_OPTION_IGNORE_IN_USE = 0x80000,
            GX2R_OPTION_FIRST = 0x80000,
            GX2R_OPTION_NO_CPU_INVALIDATE = 0x100000,
            GX2R_OPTION_NO_GPU_INVALIDATE = 0x200000,
            GX2R_OPTION_LOCK_READONLY = 0x400000,
            GX2R_OPTION_NO_TOUCH_DESTROY = 0x800000,
            GX2R_OPTION_LAST = 0x800000,
            GX2R_OPTION_NO_INVALIDATE = 0x300000,
            GX2R_OPTION_MASK = 0xF80000,
            GX2R_RESOURCE_FLAG_RESERVED2 = 0x10000000,
            GX2R_RESOURCE_FLAG_RESERVED1 = 0x20000000,
            GX2R_RESOURCE_FLAG_RESERVED0 = 0x40000000,
        };
        public enum GX2TileMode
        {
            GX2_TILE_MODE_DEFAULT = 0x0,
            GX2_TILE_MODE_LINEAR_SPECIAL = 0x10,
            GX2_TILE_MODE_DEFAULT_FIX2197 = 0x20,
            GX2_TILE_MODE_LINEAR_ALIGNED = 0x1,
            GX2_TILE_MODE_1D_TILED_THIN1 = 0x2,
            GX2_TILE_MODE_1D_TILED_THICK = 0x3,
            GX2_TILE_MODE_2D_TILED_THIN1 = 0x4,
            GX2_TILE_MODE_2D_TILED_THIN2 = 0x5,
            GX2_TILE_MODE_2D_TILED_THIN4 = 0x6,
            GX2_TILE_MODE_2D_TILED_THICK = 0x7,
            GX2_TILE_MODE_2B_TILED_THIN1 = 0x8,
            GX2_TILE_MODE_2B_TILED_THIN2 = 0x9,
            GX2_TILE_MODE_2B_TILED_THIN4 = 0xA,
            GX2_TILE_MODE_2B_TILED_THICK = 0xB,
            GX2_TILE_MODE_3D_TILED_THIN1 = 0xC,
            GX2_TILE_MODE_3D_TILED_THICK = 0xD,
            GX2_TILE_MODE_3B_TILED_THIN1 = 0xE,
            GX2_TILE_MODE_3B_TILED_THICK = 0xF,
            GX2_TILE_MODE_FIRST = 0x0,
            GX2_TILE_MODE_LAST = 0x20,
        };

        public enum AddrTileMode
        {
            ADDR_TM_LINEAR_GENERAL = 0x0,
            ADDR_TM_LINEAR_ALIGNED = 0x1,
            ADDR_TM_1D_TILED_THIN1 = 0x2,
            ADDR_TM_1D_TILED_THICK = 0x3,
            ADDR_TM_2D_TILED_THIN1 = 0x4,
            ADDR_TM_2D_TILED_THIN2 = 0x5,
            ADDR_TM_2D_TILED_THIN4 = 0x6,
            ADDR_TM_2D_TILED_THICK = 0x7,
            ADDR_TM_2B_TILED_THIN1 = 0x8,
            ADDR_TM_2B_TILED_THIN2 = 0x9,
            ADDR_TM_2B_TILED_THIN4 = 0x0A,
            ADDR_TM_2B_TILED_THICK = 0x0B,
            ADDR_TM_3D_TILED_THIN1 = 0x0C,
            ADDR_TM_3D_TILED_THICK = 0x0D,
            ADDR_TM_3B_TILED_THIN1 = 0x0E,
            ADDR_TM_3B_TILED_THICK = 0x0F,
            ADDR_TM_2D_TILED_XTHICK = 0x10,
            ADDR_TM_3D_TILED_XTHICK = 0x11,
            ADDR_TM_POWER_SAVE = 0x12,
            ADDR_TM_COUNT = 0x13,
        }
        public enum AddrTileType
        {
            ADDR_DISPLAYABLE = 0,
            ADDR_NON_DISPLAYABLE = 1,
            ADDR_DEPTH_SAMPLE_ORDER = 2,
            ADDR_THICK_TILING = 3,
        }
        public enum AddrPipeCfg
        {
            ADDR_PIPECFG_INVALID = 0x0,
            ADDR_PIPECFG_P2 = 0x1,
            ADDR_PIPECFG_P4_8x16 = 0x5,
            ADDR_PIPECFG_P4_16x16 = 0x6,
            ADDR_PIPECFG_P4_16x32 = 0x7,
            ADDR_PIPECFG_P4_32x32 = 0x8,
            ADDR_PIPECFG_P8_16x16_8x16 = 0x9,
            ADDR_PIPECFG_P8_16x32_8x16 = 0xA,
            ADDR_PIPECFG_P8_32x32_8x16 = 0xB,
            ADDR_PIPECFG_P8_16x32_16x16 = 0xC,
            ADDR_PIPECFG_P8_32x32_16x16 = 0xD,
            ADDR_PIPECFG_P8_32x32_16x32 = 0xE,
            ADDR_PIPECFG_P8_32x64_32x32 = 0xF,
            ADDR_PIPECFG_MAX = 0x10,
        }
        public enum AddrFormat
        {
            ADDR_FMT_INVALID = 0x0,
            ADDR_FMT_8 = 0x1,
            ADDR_FMT_4_4 = 0x2,
            ADDR_FMT_3_3_2 = 0x3,
            ADDR_FMT_RESERVED_4 = 0x4,
            ADDR_FMT_16 = 0x5,
            ADDR_FMT_16_FLOAT = 0x6,
            ADDR_FMT_8_8 = 0x7,
            ADDR_FMT_5_6_5 = 0x8,
            ADDR_FMT_6_5_5 = 0x9,
            ADDR_FMT_1_5_5_5 = 0xA,
            ADDR_FMT_4_4_4_4 = 0xB,
            ADDR_FMT_5_5_5_1 = 0xC,
            ADDR_FMT_32 = 0xD,
            ADDR_FMT_32_FLOAT = 0xE,
            ADDR_FMT_16_16 = 0xF,
            ADDR_FMT_16_16_FLOAT = 0x10,
            ADDR_FMT_8_24 = 0x11,
            ADDR_FMT_8_24_FLOAT = 0x12,
            ADDR_FMT_24_8 = 0x13,
            ADDR_FMT_24_8_FLOAT = 0x14,
            ADDR_FMT_10_11_11 = 0x15,
            ADDR_FMT_10_11_11_FLOAT = 0x16,
            ADDR_FMT_11_11_10 = 0x17,
            ADDR_FMT_11_11_10_FLOAT = 0x18,
            ADDR_FMT_2_10_10_10 = 0x19,
            ADDR_FMT_8_8_8_8 = 0x1A,
            ADDR_FMT_10_10_10_2 = 0x1B,
            ADDR_FMT_X24_8_32_FLOAT = 0x1C,
            ADDR_FMT_32_32 = 0x1D,
            ADDR_FMT_32_32_FLOAT = 0x1E,
            ADDR_FMT_16_16_16_16 = 0x1F,
            ADDR_FMT_16_16_16_16_FLOAT = 0x20,
            ADDR_FMT_RESERVED_33 = 0x21,
            ADDR_FMT_32_32_32_32 = 0x22,
            ADDR_FMT_32_32_32_32_FLOAT = 0x23,
            ADDR_FMT_RESERVED_36 = 0x24,
            ADDR_FMT_1 = 0x25,
            ADDR_FMT_1_REVERSED = 0x26,
            ADDR_FMT_GB_GR = 0x27,
            ADDR_FMT_BG_RG = 0x28,
            ADDR_FMT_32_AS_8 = 0x29,
            ADDR_FMT_32_AS_8_8 = 0x2A,
            ADDR_FMT_5_9_9_9_SHAREDEXP = 0x2B,
            ADDR_FMT_8_8_8 = 0x2C,
            ADDR_FMT_16_16_16 = 0x2D,
            ADDR_FMT_16_16_16_FLOAT = 0x2E,
            ADDR_FMT_32_32_32 = 0x2F,
            ADDR_FMT_32_32_32_FLOAT = 0x30,
            ADDR_FMT_BC1 = 0x31,
            ADDR_FMT_BC2 = 0x32,
            ADDR_FMT_BC3 = 0x33,
            ADDR_FMT_BC4 = 0x34,
            ADDR_FMT_BC5 = 0x35,
            ADDR_FMT_BC6 = 0x36,
            ADDR_FMT_BC7 = 0x37,
            ADDR_FMT_32_AS_32_32_32_32 = 0x38,
            ADDR_FMT_APC3 = 0x39,
            ADDR_FMT_APC4 = 0x3A,
            ADDR_FMT_APC5 = 0x3B,
            ADDR_FMT_APC6 = 0x3C,
            ADDR_FMT_APC7 = 0x3D,
            ADDR_FMT_CTX1 = 0x3E,
            ADDR_FMT_RESERVED_63 = 0x3F,
        };

        public static void Debug(surfaceOut surf)
        {
            if (surf == null)
                surf = getSurfaceInfo((GX2SurfaceFormat)0x33, 701, 77, 1, 0, 13, 1, 0);

            Console.WriteLine($"size          {surf.size}");
            Console.WriteLine($"pitch         {surf.pitch}");
            Console.WriteLine($"height        {surf.height}");
            Console.WriteLine($"depth         {surf.depth}");
            Console.WriteLine($"surfSize      {surf.surfSize}");
            Console.WriteLine($"tileMode      {surf.tileMode}");
            Console.WriteLine($"baseAlign     {surf.baseAlign}");
            Console.WriteLine($"pitchAlign    {surf.pitchAlign}");
            Console.WriteLine($"heightAlign   {surf.heightAlign}");
            Console.WriteLine($"depthAlign    {surf.depthAlign}");
            Console.WriteLine($"bpp           {surf.bpp}");
            Console.WriteLine($"pixelPitch    {surf.pixelPitch}");
            Console.WriteLine($"pixelHeight   {surf.pixelHeight}");
            Console.WriteLine($"pixelBits     {surf.pixelBits}");
            Console.WriteLine($"sliceSize     {surf.sliceSize}");
            Console.WriteLine($"pitchTileMax  {surf.pitchTileMax}");
            Console.WriteLine($"heightTileMax {surf.heightTileMax}");
            Console.WriteLine($"sliceTileMax  {surf.sliceTileMax}");
            Console.WriteLine($"tileType      {surf.tileType}");
            Console.WriteLine($"tileIndex     {surf.tileIndex}");
        }
        static bool DebugSurface = false;

        public static List<byte[]> Decode(GX2Surface tex)
        {
            Console.WriteLine("");
            Console.WriteLine("// ----- GX2Surface Decode Info ----- ");
            Console.WriteLine("  dim             = " + tex.dim);
            Console.WriteLine("  width           = " + tex.width);
            Console.WriteLine("  height          = " + tex.height);
            Console.WriteLine("  depth           = " + tex.depth);
            Console.WriteLine("  numMips         = " + tex.numMips);
            Console.WriteLine("  format          = " + tex.format);
            Console.WriteLine("  aa              = " + tex.aa);
            Console.WriteLine("  use             = " + tex.use);
            Console.WriteLine("  imageSize       = " + tex.imageSize);
            Console.WriteLine("  mipSize         = " + tex.mipSize);
            Console.WriteLine("  tileMode        = " + tex.tileMode);
            Console.WriteLine("  swizzle         = " + tex.swizzle);
            Console.WriteLine("  alignment       = " + tex.alignment);
            Console.WriteLine("  pitch           = " + tex.pitch);
            Console.WriteLine("  bits per pixel  = " + (tex.bpp << 3));
            Console.WriteLine("  bytes per pixel = " + tex.bpp);
            Console.WriteLine("  data size       = " + tex.data.Length);
            Console.WriteLine("  realSize        = " + tex.imageSize);

            uint blkWidth, blkHeight;
            if (IsFormatBCN((GX2SurfaceFormat)tex.format))
            {
                blkWidth = 4;
                blkHeight = 4;
            }
            else
            {
                blkWidth = 1;
                blkHeight = 1;
            }

            byte[] data = tex.data;

            var surfInfo = getSurfaceInfo((GX2SurfaceFormat)tex.format, tex.width, tex.height, tex.depth, (uint)tex.dim, (uint)tex.tileMode, (uint)tex.aa, 0);
            uint bpp = TegraX1Swizzle.DIV_ROUND_UP(surfInfo.bpp, 8);

            if (surfInfo.depth != 1)
                throw new Exception($"Unsupported Depth {surfInfo.depth}!");

            List<byte[]> result = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < tex.numMips; mipLevel++)
            {
                uint width_ = (uint)Math.Max(1, tex.width >> mipLevel);
                uint height_ = (uint)Math.Max(1, tex.height >> mipLevel);

                uint size = TegraX1Swizzle.DIV_ROUND_UP(width_, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height_, blkHeight) * bpp;

                uint mipOffset;
                if (mipLevel != 0)
                {
                    mipOffset = (tex.mipOffset[mipLevel - 1]);
                    if (mipLevel == 1)
                        mipOffset -= (uint)surfInfo.surfSize;

                    surfInfo = getSurfaceInfo((GX2SurfaceFormat)tex.format, tex.width, tex.height, tex.depth, (uint)tex.dim, (uint)tex.tileMode, (uint)tex.aa, mipLevel);
                    data = new byte[surfInfo.surfSize];
                    Array.Copy(tex.mipData, mipOffset, data, 0, surfInfo.surfSize);
                }
                byte[] deswizzled = deswizzle(width_, height_, surfInfo.height, (uint)tex.format,
                surfInfo.tileMode, (uint)tex.swizzle, surfInfo.pitch, surfInfo.bpp, data);
                //Create a copy and use that to remove uneeded data
                byte[] result_ = new byte[size];
                Array.Copy(deswizzled, 0, result_, 0, size);
                result.Add(result_);
            }
            return result;
        }




        /*---------------------------------------
         * 
         * Code ported from AboodXD's GTX Extractorhttps://github.com/aboood40091/GTX-Extractor/blob/f586dde90bd4a262421a4a565c1556d0079a748e/addrlib/addrlib_cy.pyx
         * 
         *---------------------------------------*/

        public static bool IsFormatBCN(GX2SurfaceFormat Format)
        {
            switch (Format)
            {
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_SRGB:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_SRGB:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_SRGB:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_SNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_SNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_UNORM:
                    return true;
                default:
                    return false;
            }
        }

        public static byte[] deswizzle(uint width, uint height, uint height_, uint format_, uint tileMode, uint swizzle_,
             uint pitch, uint bpp, byte[] data)
        {
            return swizzleSurf(width, height, height_, format_, tileMode, swizzle_, pitch, bpp, data, 0);
        }
        public static byte[] swizzle(uint width, uint height, uint height_, uint format_, uint tileMode, uint swizzle_,
     uint pitch, uint bpp, byte[] data)
        {
            return swizzleSurf(width, height, height_, format_, tileMode, swizzle_, pitch, bpp, data, 1);
        }

        private static byte[] swizzleSurf(uint width, uint height, uint height_, uint format, uint tileMode, uint swizzle_,
                uint pitch, uint bitsPerPixel, byte[] data, int swizzle)
        {
            uint bytesPerPixel = bitsPerPixel / 8;
            byte[] result = new byte[data.Length];

            uint pipeSwizzle, bankSwizzle, pos_;
            ulong pos;

            if (IsFormatBCN((GX2SurfaceFormat)format))
            {
                width = (width + 3) / 4;
                height = (height + 3) / 4;
            }
            pipeSwizzle = (swizzle_ >> 8) & 1;
            bankSwizzle = (swizzle_ >> 9) & 3;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tileMode == 0 || tileMode == 1)
                        pos = (uint)(y * pitch + x) * bytesPerPixel;
                    else if (tileMode == 2 || tileMode == 3)
                    {
                        pos = computeSurfaceAddrFromCoordMicroTiled((uint)x, (uint)y, bitsPerPixel, pitch, (AddrTileMode)tileMode);
                    }
                    else
                    {
                        pos = computeSurfaceAddrFromCoordMacroTiled((uint)x, (uint)y, bitsPerPixel, pitch, height_, (AddrTileMode)tileMode, pipeSwizzle, bankSwizzle);
                    }


                    pos_ = (uint)(y * width + x) * bytesPerPixel;

                    if (pos_ + bytesPerPixel <= data.Length && pos + bytesPerPixel <= (ulong)data.Length)
                    {
                        if (swizzle == 0)
                        {
                            for (int n = 0; n < bytesPerPixel; n++)
                                result[pos_ + n] = data[(uint)pos + n];
                        }
                        else
                        {
                            for (int n = 0; n < bytesPerPixel; n++)
                                result[(uint)pos + n] = data[pos_ + n];
                        }
                    }
                }
            }
            return result;
        }

        private static byte[] formatHwInfo = {
    0x00, 0x00, 0x00, 0x01, 0x08, 0x03, 0x00, 0x01, 0x08, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
    0x00, 0x00, 0x00, 0x01, 0x10, 0x07, 0x00, 0x00, 0x10, 0x03, 0x00, 0x01, 0x10, 0x03, 0x00, 0x01,
    0x10, 0x0B, 0x00, 0x01, 0x10, 0x01, 0x00, 0x01, 0x10, 0x03, 0x00, 0x01, 0x10, 0x03, 0x00, 0x01,
    0x10, 0x03, 0x00, 0x01, 0x20, 0x03, 0x00, 0x00, 0x20, 0x07, 0x00, 0x00, 0x20, 0x03, 0x00, 0x00,
    0x20, 0x03, 0x00, 0x01, 0x20, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x03, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x20, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
    0x00, 0x00, 0x00, 0x01, 0x20, 0x0B, 0x00, 0x01, 0x20, 0x0B, 0x00, 0x01, 0x20, 0x0B, 0x00, 0x01,
    0x40, 0x05, 0x00, 0x00, 0x40, 0x03, 0x00, 0x00, 0x40, 0x03, 0x00, 0x00, 0x40, 0x03, 0x00, 0x00,
    0x40, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x80, 0x03, 0x00, 0x00, 0x80, 0x03, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x10, 0x01, 0x00, 0x00,
    0x10, 0x01, 0x00, 0x00, 0x20, 0x01, 0x00, 0x00, 0x20, 0x01, 0x00, 0x00, 0x20, 0x01, 0x00, 0x00,
    0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x60, 0x01, 0x00, 0x00,
    0x60, 0x01, 0x00, 0x00, 0x40, 0x01, 0x00, 0x01, 0x80, 0x01, 0x00, 0x01, 0x80, 0x01, 0x00, 0x01,
    0x40, 0x01, 0x00, 0x01, 0x80, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        private static byte[] formatExInfo = {
   0x00, 0x01, 0x01, 0x03, 0x08, 0x01, 0x01, 0x03, 0x08, 0x01, 0x01, 0x03, 0x08, 0x01, 0x01, 0x03,
    0x00, 0x01, 0x01, 0x03, 0x10, 0x01, 0x01, 0x03, 0x10, 0x01, 0x01, 0x03, 0x10, 0x01, 0x01, 0x03,
    0x10, 0x01, 0x01, 0x03, 0x10, 0x01, 0x01, 0x03, 0x10, 0x01, 0x01, 0x03, 0x10, 0x01, 0x01, 0x03,
    0x10, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03,
    0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03,
    0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03,
    0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03,
    0x40, 0x01, 0x01, 0x03, 0x40, 0x01, 0x01, 0x03, 0x40, 0x01, 0x01, 0x03, 0x40, 0x01, 0x01, 0x03,
    0x40, 0x01, 0x01, 0x03, 0x00, 0x01, 0x01, 0x03, 0x80, 0x01, 0x01, 0x03, 0x80, 0x01, 0x01, 0x03,
    0x00, 0x01, 0x01, 0x03, 0x01, 0x08, 0x01, 0x05, 0x01, 0x08, 0x01, 0x06, 0x10, 0x01, 0x01, 0x07,
    0x10, 0x01, 0x01, 0x08, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03, 0x20, 0x01, 0x01, 0x03,
    0x18, 0x03, 0x01, 0x04, 0x30, 0x03, 0x01, 0x04, 0x30, 0x03, 0x01, 0x04, 0x60, 0x03, 0x01, 0x04,
    0x60, 0x03, 0x01, 0x04, 0x40, 0x04, 0x04, 0x09, 0x80, 0x04, 0x04, 0x0A, 0x80, 0x04, 0x04, 0x0B,
    0x40, 0x04, 0x04, 0x0C, 0x40, 0x04, 0x04, 0x0D, 0x40, 0x04, 0x04, 0x0D, 0x40, 0x04, 0x04, 0x0D,
    0x00, 0x01, 0x01, 0x03, 0x00, 0x01, 0x01, 0x03, 0x00, 0x01, 0x01, 0x03, 0x00, 0x01, 0x01, 0x03,
    0x00, 0x01, 0x01, 0x03, 0x00, 0x01, 0x01, 0x03, 0x40, 0x01, 0x01, 0x03, 0x00, 0x01, 0x01, 0x03,
        };

        public static uint surfaceGetBitsPerPixel(uint surfaceFormat)
        {
            return formatHwInfo[(surfaceFormat & 0x3F) * 4];
        }

        public static uint nextPow2(uint dim)
        {
            uint newDim = 1;
            if (dim < 0x7FFFFFFF)
            {
                while (newDim < dim)
                    newDim *= 2;
            }
            else
                newDim = 0x80000000;

            return newDim;
        }

        private static uint computeSurfaceThickness(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_1D_TILED_THICK:
                case AddrTileMode.ADDR_TM_2D_TILED_THICK:
                case AddrTileMode.ADDR_TM_2B_TILED_THICK:
                case AddrTileMode.ADDR_TM_3D_TILED_THICK:
                case AddrTileMode.ADDR_TM_3B_TILED_THICK:
                    return 4;

                case AddrTileMode.ADDR_TM_2D_TILED_XTHICK:
                case AddrTileMode.ADDR_TM_3D_TILED_XTHICK:
                    return 8;

                default:
                    return 1;
            }
        }

        private static uint computePixelIndexWithinMicroTile(uint x, uint y, uint bpp)
        {
            switch (bpp)
            {
                case 0x08:
                    return (32 * ((y & 4) >> 2) | 16 * (y & 1) | 8 * ((y & 2) >> 1) |
                            4 * ((x & 4) >> 2) | 2 * ((x & 2) >> 1) | x & 1);

                case 0x10:
                    return (32 * ((y & 4) >> 2) | 16 * ((y & 2) >> 1) | 8 * (y & 1) |
                            4 * ((x & 4) >> 2) | 2 * ((x & 2) >> 1) | x & 1);

                case 0x20:
                case 0x60:
                    return (32 * ((y & 4) >> 2) | 16 * ((y & 2) >> 1) | 8 * ((x & 4) >> 2) |
                            4 * (y & 1) | 2 * ((x & 2) >> 1) | x & 1);

                case 0x40:
                    return (32 * ((y & 4) >> 2) | 16 * ((y & 2) >> 1) | 8 * ((x & 4) >> 2) |
                            4 * ((x & 2) >> 1) | 2 * (y & 1) | x & 1);

                case 0x80:
                    return (32 * ((y & 4) >> 2) | 16 * ((y & 2) >> 1) | 8 * ((x & 4) >> 2) |
                            4 * ((x & 2) >> 1) | 2 * (x & 1) | y & 1);

                default:
                    return (32 * ((y & 4) >> 2) | 16 * ((y & 2) >> 1) | 8 * ((x & 4) >> 2) |
                            4 * (y & 1) | 2 * ((x & 2) >> 1) | x & 1);
            }
        }

        private static uint computePipeFromCoordWoRotation(uint x, uint y)
        {
            return ((y >> 3) ^ (x >> 3)) & 1;
        }


        private static uint computeBankFromCoordWoRotation(uint x, uint y)
        {
            return ((y >> 5) ^ (x >> 3)) & 1 | 2 * (((y >> 4) ^ (x >> 4)) & 1);
        }

        private static uint isThickMacroTiled(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2D_TILED_THICK:
                case AddrTileMode.ADDR_TM_2B_TILED_THICK:
                case AddrTileMode.ADDR_TM_3D_TILED_THICK:
                case AddrTileMode.ADDR_TM_3B_TILED_THICK:
                    return 1;

                default:
                    return 0;
            }
        }

        private static uint isBankSwappedTileMode(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2B_TILED_THIN1:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN2:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN4:
                case AddrTileMode.ADDR_TM_2B_TILED_THICK:
                case AddrTileMode.ADDR_TM_3B_TILED_THIN1:
                case AddrTileMode.ADDR_TM_3B_TILED_THICK:
                    return 1;

                default:
                    return 0;
            }
        }

        private static uint computeMacroTileAspectRatio(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2D_TILED_THIN2:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN2:
                    return 2;

                case AddrTileMode.ADDR_TM_2D_TILED_THIN4:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN4:
                    return 4;

                default:
                    return 1;
            }
        }

        private static uint computeSurfaceBankSwappedWidth(AddrTileMode tileMode, uint bpp, uint pitch, uint numSamples)
        {
            if (isBankSwappedTileMode(tileMode) == 0)
                return 0;

            uint bytesPerSample = 8 * bpp;
            uint samplesPerTile, slicesPerTile;

            if (bytesPerSample != 0)
            {
                samplesPerTile = 2048 / bytesPerSample;
                slicesPerTile = Math.Max(1, numSamples / samplesPerTile);
            }

            else
                slicesPerTile = 1;

            if (isThickMacroTiled(tileMode) != 0)
                numSamples = 4;

            uint bytesPerTileSlice = numSamples * bytesPerSample / slicesPerTile;

            uint factor = computeMacroTileAspectRatio(tileMode);
            uint swapTiles = Math.Max(1, 128 / bpp);

            uint swapWidth = swapTiles * 32;
            uint heightBytes = numSamples * factor * bpp * 2 / slicesPerTile;
            uint swapMax = 0x4000 / heightBytes;
            uint swapMin = 256 / bytesPerTileSlice;

            uint bankSwapWidth = Math.Min(swapMax, Math.Max(swapMin, swapWidth));

            while (bankSwapWidth >= 2 * pitch)
                bankSwapWidth >>= 1;

            return bankSwapWidth;
        }


        private static ulong computeSurfaceAddrFromCoordMicroTiled(uint x, uint y, uint bpp, uint pitch, AddrTileMode tileMode)
        {
            int microTileThickness = 1;

            if (tileMode == AddrTileMode.ADDR_TM_1D_TILED_THICK)
                microTileThickness = 4;

            uint microTileBytes = (uint)(64 * microTileThickness * bpp + 7) / 8;
            uint microTilesPerRow = pitch >> 3;
            uint microTileIndexX = x >> 3;
            uint microTileIndexY = y >> 3;

            ulong microTileOffset = microTileBytes * (microTileIndexX + microTileIndexY * microTilesPerRow);
            uint pixelIndex = computePixelIndexWithinMicroTile(x, y, bpp);
            ulong pixelOffset = (bpp * pixelIndex) >> 3;

            return pixelOffset + microTileOffset;
        }

        private static byte[] bankSwapOrder = { 0, 1, 3, 2, 6, 7, 5, 4, 0, 0 };

        private static ulong computeSurfaceAddrFromCoordMacroTiled(uint x, uint y, uint bpp, uint pitch, uint height,
                                                           AddrTileMode tileMode, uint pipeSwizzle, uint bankSwizzle)
        {

            uint sampleSlice, numSamples, samplesPerSlice;
            uint numSampleSplits, bankSwapWidth, swapIndex;

            uint microTileThickness = computeSurfaceThickness(tileMode);

            uint microTileBits = bpp * (microTileThickness * 64);
            uint microTileBytes = (microTileBits + 7) / 8;

            uint pixelIndex = computePixelIndexWithinMicroTile(x, y, bpp);
            ulong elemOffset = bpp * pixelIndex;

            uint bytesPerSample = microTileBytes;

            if (microTileBytes <= 2048)
            {
                numSamples = 1;
                sampleSlice = 0;
            }

            else
            {
                samplesPerSlice = 2048 / bytesPerSample;
                numSampleSplits = 1;
                numSamples = samplesPerSlice;
                sampleSlice = (uint)(elemOffset / (microTileBits / numSampleSplits));
                elemOffset %= microTileBits / numSampleSplits;
            }

            elemOffset = (elemOffset + 7) / 8;

            uint pipe = computePipeFromCoordWoRotation(x, y);
            uint bank = computeBankFromCoordWoRotation(x, y);

            uint swizzle_ = pipeSwizzle + 2 * bankSwizzle;
            uint bankPipe = ((pipe + 2 * bank) ^ (6 * sampleSlice ^ swizzle_)) % 8;

            pipe = bankPipe % 2;
            bank = bankPipe / 2;

            uint sliceBytes = (height * pitch * microTileThickness * bpp * numSamples + 7) / 8;
            uint sliceOffset = sliceBytes * (sampleSlice / microTileThickness);

            uint macroTilePitch = 32;
            uint macroTileHeight = 16;

            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2D_TILED_THIN2:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN2:
                    {
                        macroTilePitch = 16;
                        macroTileHeight = 32;
                        break;
                    }

                case AddrTileMode.ADDR_TM_2D_TILED_THIN4:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN4:
                    {
                        macroTilePitch = 8;
                        macroTileHeight = 64;
                        break;
                    }
            }

            uint macroTilesPerRow = pitch / macroTilePitch;
            uint macroTileBytes = (numSamples * microTileThickness * bpp * macroTileHeight
                                  * macroTilePitch + 7) / 8;
            uint macroTileIndexX = x / macroTilePitch;
            uint macroTileIndexY = y / macroTileHeight;
            ulong macroTileOffset = (macroTileIndexX + macroTilesPerRow * macroTileIndexY) * macroTileBytes;

            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2B_TILED_THIN1:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN2:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN4:
                case AddrTileMode.ADDR_TM_2B_TILED_THICK:
                case AddrTileMode.ADDR_TM_3B_TILED_THIN1:
                case AddrTileMode.ADDR_TM_3B_TILED_THICK:
                    {
                        bankSwapWidth = computeSurfaceBankSwappedWidth(tileMode, bpp, pitch, 1);
                        swapIndex = macroTilePitch * macroTileIndexX / bankSwapWidth;
                        bank ^= bankSwapOrder[swapIndex & 3];
                        break;
                    }
            }

            ulong totalOffset = elemOffset + ((macroTileOffset + sliceOffset) >> 3);
            return bank << 9 | pipe << 8 | 255 & totalOffset | (ulong)((int)totalOffset & -256) << 3;
        }

        public static uint computeSurfaceMipLevelTileMode(uint baseTileMode, uint bpp, uint level, uint width, uint height,
            uint numSlices, uint numSamples, uint isDepth, uint noRecursive)
        {
            uint widthAlignFactor = 1;
            uint macroTileWidth = 32;
            uint macroTileHeight = 16;
            uint tileSlices = computeSurfaceTileSlices(baseTileMode, bpp, numSamples);


            uint expTileMode, widtha, heighta, numSlicesa, thickness, microTileBytes;
            expTileMode = baseTileMode;

            if (DebugSurface)
                Console.WriteLine("baseTileMode " + baseTileMode);
            if (baseTileMode == 7)
            {
                if (numSamples > 1 || tileSlices > 1 || isDepth != 0)
                    expTileMode = 4;
            }
            else if (baseTileMode == 13)
            {
                if (numSamples > 1 || tileSlices > 1 || isDepth != 0)
                    expTileMode = 12;
            }
            else if (baseTileMode == 11)
            {
                if (numSamples > 1 || tileSlices > 1 || isDepth != 0)
                    expTileMode = 8;
            }
            else if (baseTileMode == 15)
            {
                if (numSamples > 1 || tileSlices > 1 || isDepth != 0)
                    expTileMode = 14;
            }
            else if (baseTileMode == 2)
            {
                if (numSamples > 1 && ((4 >> 2) & 1) != 0)
                    expTileMode = 4;
            }
            else if (baseTileMode == 3)
            {
                if (numSamples > 1 || isDepth != 0)
                    expTileMode = 2;

                if (numSamples == 2 || numSamples == 4)
                {
                    expTileMode = 7;
                }
            }
            else
            {
                expTileMode = baseTileMode;
            }
            if (DebugSurface)
                Console.WriteLine("computeSurfaceMipLevelTileMode expTileMode " + expTileMode);
            if (expTileMode != 12 && expTileMode != 13 && expTileMode != 14 &&
                expTileMode != 15)
            {
                if (expTileMode == 12)
                    expTileMode = 4;
                else if (expTileMode == 13)
                    expTileMode = 7;
                else if (expTileMode == 14)
                    expTileMode = 8;
                else if (expTileMode == 15)
                    expTileMode = 11;
            }
            if (noRecursive == 0)
            {
                switch (bpp)
                {
                    case 24:
                    case 48:
                    case 96:
                        bpp /= 3;
                        break;
                }
                if (bpp == 24) ;

                widtha = nextPow2(width);
                heighta = nextPow2(height);
                numSlicesa = nextPow2(numSlices);

                if (level != 0)
                {
                    expTileMode = convertToNonBankSwappedMode((AddrTileMode)expTileMode);
                    thickness = computeSurfaceThickness((AddrTileMode)expTileMode);
                    microTileBytes = (numSamples * bpp * (thickness << 6) + 7) >> 3;

                    if (microTileBytes < 256)
                    {
                        widthAlignFactor = Math.Max(1, 256 / microTileBytes);
                    }
                    if (expTileMode == 4 || expTileMode == 12)
                    {
                        if ((widtha < widthAlignFactor * macroTileWidth) || heighta < macroTileHeight)
                            expTileMode = 2;
                    }
                    else if (expTileMode == 5)
                    {
                        macroTileWidth = 16;
                        macroTileHeight = 32;

                        if ((widtha < widthAlignFactor * macroTileWidth) || heighta < macroTileHeight)
                            expTileMode = 2;
                    }
                    else if (expTileMode == 6)
                    {
                        macroTileWidth = 8;
                        macroTileHeight = 64;

                        if ((widtha < widthAlignFactor * macroTileWidth) || heighta < macroTileHeight)
                            expTileMode = 2;
                    }
                    else if (expTileMode == 7 || expTileMode == 13)
                    {
                        if ((widtha < widthAlignFactor * macroTileWidth) || heighta < macroTileHeight)
                            expTileMode = 3;
                    }
                    else if (expTileMode == 3)
                    {
                        if (numSlicesa < 4)
                            expTileMode = 2;
                    }
                    else if (expTileMode == 7)
                    {
                        if (numSlicesa < 4)
                            expTileMode = 4;
                    }
                    else if (expTileMode == 13 && numSlicesa < 4)
                    {
                        expTileMode = 13;
                    }

                    return computeSurfaceMipLevelTileMode(
                    expTileMode,
                    bpp,
                    level,
                    widtha,
                    heighta,
                    numSlicesa,
                    numSamples,
                    isDepth,
                    1);
                }
            }

            return expTileMode;
        }
        private static uint computeSurfaceTileSlices(uint tileMode, uint bpp, uint numSamples)
        {
            uint bytePerSample = ((bpp << 6) + 7) >> 3;
            uint tileSlices = 1;
            uint samplePerTile;

            if (computeSurfaceThickness((AddrTileMode)tileMode) > 1)
                numSamples = 4;

            if (bytePerSample != 0)
            {
                samplePerTile = 2048 / bytePerSample;
                if (samplePerTile < numSamples)
                    tileSlices = Math.Max(1, numSamples / samplePerTile);
            }

            return tileSlices;
        }

        private static int getFillSizeFieldsFlags()
        {
            return (m_configFlags >> 6) & 1;
        }

        private static uint ComputeSurfaceInfoEx()
        {
            uint tileMode = pIn.tileMode;
            uint bpp = pIn.bpp;
            uint numSamples = Math.Max(1, pIn.numSamples);
            uint pitch = pIn.width;
            Console.WriteLine("ComputeSurfaceInfoEx pitch " + pitch);
            uint height = pIn.height;
            uint numSlices = pIn.numSlices;
            uint mipLevel = pIn.mipLevel;
            Flags flags = new Flags();
            uint pPitchOut = pOut.pitch;
            uint pHeightOut = pOut.height;
            uint pNumSlicesOut = pOut.depth;
            uint pTileModeOut = pOut.tileMode;
            long pSurfSize = pOut.surfSize;
            uint pBaseAlign = pOut.baseAlign;
            uint pPitchAlign = pOut.pitchAlign;
            uint pHeightAlign = pOut.heightAlign;
            uint pDepthAlign = pOut.depthAlign;
            uint padDims = 0;
            uint valid = 0;
            uint baseTileMode = tileMode;

            if (DebugSurface)
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine(tileMode);
                Console.WriteLine(bpp);
                Console.WriteLine(numSamples);
                Console.WriteLine(pitch);
                Console.WriteLine(height);
                Console.WriteLine(numSlices);
                Console.WriteLine(mipLevel);
                Console.WriteLine(flags);
                Console.WriteLine(pPitchOut);
                Console.WriteLine(pHeightOut);
                Console.WriteLine(pNumSlicesOut);
                Console.WriteLine(pTileModeOut);
                Console.WriteLine(pSurfSize);
                Console.WriteLine(pBaseAlign);
                Console.WriteLine(pPitchAlign);
                Console.WriteLine(pHeightAlign);
                Console.WriteLine(pDepthAlign);
                Console.WriteLine(padDims);
                Console.WriteLine(valid);
                Console.WriteLine(baseTileMode);
                Console.WriteLine("---------------------------");
            }


            flags.value = pIn.flags.value;

            Console.WriteLine("padDims " + padDims);

            if (((flags.value >> 4) & 1) != mipLevel)
                padDims = 2;

            Console.WriteLine("padDims " + padDims);

            if (((flags.value >> 6) & 1) != 0)
                tileMode = convertToNonBankSwappedMode((AddrTileMode)tileMode);
            else
            {
                if (DebugSurface)
                    Console.WriteLine(tileMode);

                tileMode = computeSurfaceMipLevelTileMode(
                tileMode,
                bpp,
                mipLevel,
                pitch,
                height,
                numSlices,
                numSamples,
                (flags.value >> 1) & 1, 0);

                if (DebugSurface)
                {
                    Console.WriteLine("---------------------------");
                    Console.WriteLine(tileMode);
                    Console.WriteLine("---------------------------");
                }
            }


            switch (tileMode)
            {
                case 0:
                case 1:
                    var compSurfInfoLinear = computeSurfaceInfoLinear(
                tileMode,
                bpp,
                numSamples,
                pitch,
                height,
                numSlices,
                mipLevel,
                padDims,
                flags);

                    valid = compSurfInfoLinear[0];
                    pPitchOut = compSurfInfoLinear[1];
                    pHeightOut = compSurfInfoLinear[2];
                    pNumSlicesOut = compSurfInfoLinear[3];
                    pSurfSize = compSurfInfoLinear[4];
                    pBaseAlign = compSurfInfoLinear[5];
                    pPitchAlign = compSurfInfoLinear[6];
                    pHeightAlign = compSurfInfoLinear[7];
                    pDepthAlign = compSurfInfoLinear[8];

                    pTileModeOut = tileMode;
                    break;
                case 2:
                case 3:
                    var compSurfInfoMicroTile = computeSurfaceInfoMicroTiled(
                tileMode,
                bpp,
                numSamples,
                pitch,
                height,
                numSlices,
                mipLevel,
                padDims,
                flags);

                    valid = compSurfInfoMicroTile[0];
                    pPitchOut = compSurfInfoMicroTile[1];
                    pHeightOut = compSurfInfoMicroTile[2];
                    pNumSlicesOut = compSurfInfoMicroTile[3];
                    pSurfSize = compSurfInfoMicroTile[4];
                    pTileModeOut = compSurfInfoMicroTile[5];
                    pBaseAlign = compSurfInfoMicroTile[6];
                    pPitchAlign = compSurfInfoMicroTile[7];
                    pHeightAlign = compSurfInfoMicroTile[8];
                    pDepthAlign = compSurfInfoMicroTile[9];

                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    var compSurfInfoMacoTile = computeSurfaceInfoMacroTiled(
                tileMode,
                baseTileMode,
                bpp,
                numSamples,
                pitch,
                height,
                numSlices,
                mipLevel,
                padDims,
                flags);

                    valid = compSurfInfoMacoTile[0];
                    pPitchOut = compSurfInfoMacoTile[1];
                    pHeightOut = compSurfInfoMacoTile[2];
                    pNumSlicesOut = compSurfInfoMacoTile[3];
                    pSurfSize = compSurfInfoMacoTile[4];
                    pTileModeOut = compSurfInfoMacoTile[5];
                    pBaseAlign = compSurfInfoMacoTile[6];
                    pPitchAlign = compSurfInfoMacoTile[7];
                    pHeightAlign = compSurfInfoMacoTile[8];
                    pDepthAlign = compSurfInfoMacoTile[9];
                    break;
            }

            pOut.pitch = pPitchOut;
            pOut.height = pHeightOut;
            pOut.depth = pNumSlicesOut;
            pOut.tileMode = pTileModeOut;
            pOut.surfSize = pSurfSize;
            pOut.baseAlign = pBaseAlign;
            pOut.pitchAlign = pPitchAlign;
            pOut.heightAlign = pHeightAlign;
            pOut.depthAlign = pDepthAlign;

            if (DebugSurface)
            {
                Console.WriteLine(pOut.pitch);
                Console.WriteLine(pOut.height);
                Console.WriteLine(pOut.depth);
                Console.WriteLine(pOut.tileMode);
                Console.WriteLine(pOut.surfSize);
                Console.WriteLine(pOut.baseAlign);
                Console.WriteLine(pOut.pitchAlign);
                Console.WriteLine(pOut.heightAlign);
                Console.WriteLine(pOut.depthAlign);
            }
      

            if (valid == 0)
                return 3;


            return 0;
        }
        private static uint[] computeSurfaceInfoLinear(uint tileMode, uint bpp, uint numSamples, uint pitch, uint height,
            uint numSlices, uint mipLevel, uint padDims, Flags flags)
        {
            expPitch = pitch;
            expHeight = height;
            expNumSlices = numSlices;

            uint valid = 1;
            uint microTileThickness = computeSurfaceThickness((AddrTileMode)tileMode);

            uint baseAlign, pitchAlign, heightAlign, slices;
            uint pPitchOut, pHeightOut, pNumSlicesOut, pSurfSize, pBaseAlign, pPitchAlign, pHeightAlign, pDepthAlign;

            var compAllignLinear = computeSurfaceAlignmentsLinear(tileMode, bpp, flags);
            baseAlign = compAllignLinear.Item1;
            pitchAlign = compAllignLinear.Item2;
            heightAlign = compAllignLinear.Item3;

            if ((flags.value >> 9 & 1) != mipLevel)
            {
                expPitch /= 3;
                expPitch = nextPow2(expPitch);
            }
            if (mipLevel != 0)
            {
                expPitch = nextPow2(expPitch);
                expHeight = nextPow2(expHeight);

                if (((flags.value >> 4) & 1) != 0)
                {
                    expNumSlices = numSlices;

                    if (numSlices <= 1)
                        padDims = 2;
                    else
                        padDims = 0;
                }
                else
                    expNumSlices = nextPow2(numSlices);
            }

            var padimens = padDimensions(
            tileMode,
            padDims,
            (flags.value >> 4) & 1,
            (flags.value >> 7) & 1,
            pitchAlign,
            heightAlign,
            microTileThickness);

            expPitch = padimens.Item1;
            expHeight = padimens.Item2;
            expNumSlices = padimens.Item3;

            if (((flags.value >> 9) & 1) != mipLevel)
                expPitch *= 3;


            slices = expNumSlices * numSamples / microTileThickness;
            pPitchOut = expPitch;
            pHeightOut = expHeight;
            pNumSlicesOut = expNumSlices;
            pSurfSize = (expHeight * expPitch * slices * bpp * numSamples + 7) / 8;
            pBaseAlign = baseAlign;
            pPitchAlign = pitchAlign;
            pHeightAlign = heightAlign;
            pDepthAlign = microTileThickness;

            return new uint[] { valid, pPitchOut, pHeightOut, pNumSlicesOut, pSurfSize, pBaseAlign, pPitchAlign, pHeightAlign, pDepthAlign };
        }
        private static Tuple<uint, uint, uint> computeSurfaceAlignmentsLinear(uint tileMode, uint bpp, Flags flags)
        {
            uint pixelsPerPipeInterleave;
            uint baseAlign, pitchAlign, heightAlign;

            if (tileMode != 0)
            {
                if (tileMode == 1)
                {
                    pixelsPerPipeInterleave = 2048 / bpp;
                    baseAlign = 256;
                    pitchAlign = Math.Max(0x40, pixelsPerPipeInterleave);
                    heightAlign = 1;
                }
                else
                {
                    baseAlign = 1;
                    pitchAlign = 1;
                    heightAlign = 1;
                }
            }
            else
            {
                baseAlign = 1;
                if (bpp != 1)
                    pitchAlign = 1;
                else
                    pitchAlign = 8;

                heightAlign = 1;
            }
            pitchAlign = adjustPitchAlignment(flags, pitchAlign);

            return new Tuple<uint, uint, uint>(baseAlign, pitchAlign, heightAlign);
        }
        private static uint convertToNonBankSwappedMode(AddrTileMode tileMode)
        {
            switch ((uint)tileMode)
            {
                case 8:
                    return 4;
                case 9:
                    return 5;
                case 10:
                    return 6;
                case 11:
                    return 7;
                case 14:
                    return 12;
                case 15:
                    return 13;
            }
            return (uint)tileMode;
        }

        private static Tuple<uint, uint, uint, uint> getBitsPerPixel(uint format_)
        {
            uint fmtIdx = format_ * 4;
            return new Tuple<uint, uint, uint, uint>
                   (formatExInfo[fmtIdx], formatExInfo[fmtIdx + 1],
                    formatExInfo[fmtIdx + 2], formatExInfo[fmtIdx + 3]);
        }

        private static void computeSurfaceInfo(surfaceIn aSurfIn, surfaceOut pSurfOut)
        {
            if (DebugSurface)
            {
                Console.WriteLine(" computeSurfaceInfo ------------------------------------ ");

            }

            pIn = aSurfIn;
            pOut = pSurfOut;

            TileInfo tileInfoNull = new TileInfo();
            uint sliceFlags = 0;
            uint returnCode = 0;

            uint width, height, bpp, elemMode;
            uint expandY, expandX;

            elemMode = 0;

            if (pIn.bpp > 0x80)
                returnCode = 3;

            if (DebugSurface)
                Console.WriteLine("returnCode " + returnCode);

            if (returnCode == 0)
            {
                if (DebugSurface)
                {
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine(" computeMipLevel");
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine(" pIn.width " + pIn.width);
                    Console.WriteLine(" pIn.height " + pIn.height);
                    Console.WriteLine(" pIn.numSlices " + pIn.numSlices);
                }

                computeMipLevel();

                width = pIn.width;
                height = pIn.height;
                bpp = pIn.bpp;
                expandX = 1;
                expandY = 1;

                if (DebugSurface)
                {
                    Console.WriteLine(pIn.width);
                    Console.WriteLine(pIn.height);
                    Console.WriteLine(pIn.numSlices);
                    Console.WriteLine("-------------------------------------------");
                }

                if (useTileIndex(pIn.tileIndex) == 0 && pIn.pTileInfo == null)
                {
                    if (pOut.pTileInfo != null)
                        pIn.pTileInfo = pOut.pTileInfo;

                    else
                    {
                        pOut.pTileInfo = tileInfoNull;
                        pIn.pTileInfo = tileInfoNull;
                    }
                }
                pOut.pixelBits = pIn.bpp;

                if (pIn.format != 0)
                {
                    bpp = formatExInfo[pIn.format * 4];
                    expandX = formatExInfo[pIn.format * 4 + 1];
                    expandY = formatExInfo[pIn.format * 4 + 2];
                    elemMode = formatExInfo[pIn.format * 4 + 3];

                    if (DebugSurface)
                    {
                        Console.WriteLine($"bpp {bpp}");
                        Console.WriteLine($"expandX {expandX}");
                        Console.WriteLine($"expandY {expandY}");
                        Console.WriteLine($"elemMode {elemMode}");
                    }


                    if (elemMode == 4 && expandX == 3 && pIn.tileMode == 1)
                        pIn.flags.value |= 0x200;

                    bpp = adjustSurfaceInfo(elemMode, expandX, expandY, bpp, width, height);

                    if (DebugSurface)
                    {
                        Console.WriteLine($"width {pIn.width}");
                        Console.WriteLine($"height {pIn.height}");
                        Console.WriteLine($"bpp {pIn.bpp}");
                    }
                }
                else if (pIn.bpp != 0)
                {
                    pIn.width = Math.Max(1, pIn.width);
                    pIn.height = Math.Max(1, pIn.height);
                }
                else
                    returnCode = 3;

                if (returnCode == 0)
                    returnCode = ComputeSurfaceInfoEx();

                if (returnCode == 0)
                {
                    pOut.bpp = pIn.bpp;
                    pOut.pixelPitch = pOut.pitch;
                    pOut.pixelHeight = pOut.height;

                    if (pIn.format != 0 && (((pIn.flags.value >> 9) & 1) == 0 || pIn.mipLevel == 0))
                        bpp = restoreSurfaceInfo(elemMode, expandX, expandY, bpp);

                    if (sliceFlags == 1)
                        pOut.sliceSize = (pOut.height * pOut.pitch * pOut.bpp * pIn.numSamples + 7) / 8;

                    else if (((pIn.flags.value >> 5) & 1) != 0)
                        pOut.sliceSize = (uint)pOut.surfSize;

                    else
                    {
                        pOut.sliceSize = (uint)pOut.surfSize / pOut.depth;

                        if (pIn.slice == (pIn.numSlices - 1) && pIn.numSlices > 1)
                            pOut.sliceSize += pOut.sliceSize * (pOut.depth - pIn.numSlices);
                    }

                    pOut.pitchTileMax = (pOut.pitch >> 3) - 1;
                    pOut.heightTileMax = (pOut.height >> 3) - 1;
                    pOut.sliceTileMax = (pOut.height * pOut.pitch >> 6) - 1;
                }
            }
        }
        private static uint[] computeSurfaceInfoMicroTiled(uint tileMode, uint bpp, uint numSamples, uint pitch, uint height, uint numSlices, uint mipLevel, uint padDims, Flags flags)
        {
            expPitch = pitch;
            expHeight = height;
            expNumSlices = numSlices;

            uint valid = 1;
            uint expTileMode = tileMode;
            uint microTileThickness = computeSurfaceThickness((AddrTileMode)tileMode);
            uint pPitchOut, pHeightOut, pNumSlicesOut, pSurfSize, pTileModeOut, pBaseAlign, pPitchAlign, pHeightAlign, pDepthAlign;

            if (mipLevel != 0)
            {
                expPitch = nextPow2(pitch);
                expHeight = nextPow2(height);
                if (((flags.value >> 4) & 1) != 0)
                {
                    expNumSlices = numSlices;

                    if (numSlices <= 1)
                        padDims = 2;

                    else
                        padDims = 0;
                }

                else
                    expNumSlices = nextPow2(numSlices);

                if (expTileMode == 3 && expNumSlices < 4)
                {
                    expTileMode = 2;
                    microTileThickness = 1;
                }
            }

            var surfMicroAlign = computeSurfaceAlignmentsMicroTiled(
                expTileMode,
                bpp,
                flags,
                numSamples);

            uint baseAlign = surfMicroAlign.Item1;
            uint pitchAlign = surfMicroAlign.Item2;
            uint heightAlign = surfMicroAlign.Item3;

            var padDimens = padDimensions(
                expTileMode,
                padDims,
                (flags.value >> 4) & 1,
                (flags.value >> 7) & 1,
                pitchAlign,
                heightAlign,
                microTileThickness);

            expPitch = padDimens.Item1;
            expHeight = padDimens.Item2;
            expNumSlices = padDimens.Item3;

            pPitchOut = expPitch;
            pHeightOut = expHeight;
            pNumSlicesOut = expNumSlices;
            pSurfSize = (expHeight * expPitch * expNumSlices * bpp * numSamples + 7) / 8;

            Console.WriteLine("pSurfSize " + pSurfSize);
            Console.WriteLine("expHeight " + expHeight);
            Console.WriteLine("expPitch " + expPitch);
            Console.WriteLine("expNumSlices " + expNumSlices);
            Console.WriteLine("numSamples " + numSamples);

            pTileModeOut = expTileMode;
            pBaseAlign = baseAlign;
            pPitchAlign = pitchAlign;
            pHeightAlign = heightAlign;
            pDepthAlign = microTileThickness;

            return new uint[] { valid, pPitchOut, pHeightOut, pNumSlicesOut, pSurfSize, pTileModeOut, pBaseAlign, pPitchAlign, pHeightAlign, pDepthAlign };
        }
        private static uint IsPow2(uint dim)
        {
            if ((dim & (dim - 1)) == 0)
                return 1;
            else
                return 0;
        }
        private static Tuple<uint, uint, uint> padDimensions(uint tileMode, uint padDims, uint isCube, uint cubeAsArray, uint pitchAlign, uint heightAlign, uint sliceAlign)
        {
            uint thickness = computeSurfaceThickness((AddrTileMode)tileMode);
            if (padDims == 0)
                padDims = 3;

            if ((pitchAlign & (pitchAlign - 1)) == 0)
                expPitch = powTwoAlign(expPitch, pitchAlign);
            else
            {
                expPitch = pitchAlign + expPitch - 1;
                expPitch /= pitchAlign;
                expPitch *= pitchAlign;
            }

            if (padDims > 1)
                expHeight = powTwoAlign(expHeight, heightAlign);

            if (padDims > 2 || thickness > 1)
            {
                if (isCube != 0)
                    expNumSlices = nextPow2(expNumSlices);

                if (thickness > 1)
                    expNumSlices = powTwoAlign(expNumSlices, sliceAlign);
            }
            return new Tuple<uint, uint, uint>(expPitch, expHeight, expNumSlices);
        }


        private static uint[] computeSurfaceInfoMacroTiled(uint tileMode, uint baseTileMode, uint bpp, uint numSamples,
            uint pitch, uint height, uint numSlices, uint mipLevel, uint padDims, Flags flags)
        {
            expPitch = pitch;

            expHeight = height;
            expNumSlices = numSlices;

            uint valid = 1;
            uint expTileMode = tileMode;
            uint microTileThickness = computeSurfaceThickness((AddrTileMode)tileMode);

            uint baseAlign, pitchAlign, heightAlign, macroWidth, macroHeight;
            uint bankSwappedWidth, pitchAlignFactor;
            uint result, pPitchOut, pHeightOut, pNumSlicesOut, pSurfSize, pTileModeOut, pBaseAlign, pPitchAlign, pHeightAlign, pDepthAlign;

            if (mipLevel != 0)
            {
                expPitch = nextPow2(pitch);
                expHeight = nextPow2(height);

                if (((flags.value >> 4) & 1) != 0)
                {
                    expNumSlices = numSlices;

                    if (numSlices <= 1)
                        padDims = 2;
                    else
                        padDims = 0;
                }
                else
                    expNumSlices = nextPow2(numSlices);

                if (expTileMode == 7 && expNumSlices < 4)
                {
                    expTileMode = 4;
                    microTileThickness = 1;
                }
            }
            if (tileMode == baseTileMode
                || mipLevel == 0
                || isThickMacroTiled((AddrTileMode)baseTileMode) == 0
                || isThickMacroTiled((AddrTileMode)tileMode) != 0)
            {
                var tup = computeSurfaceAlignmentsMacroTiled(
                    tileMode,
                    bpp,
                    flags,
                    numSamples);

                baseAlign = tup.Item1;
                pitchAlign = tup.Item2;
                heightAlign = tup.Item3;
                macroWidth = tup.Item4;
                macroHeight = tup.Item5;

                bankSwappedWidth = computeSurfaceBankSwappedWidth((AddrTileMode)tileMode, bpp, pitch, numSamples);

                if (bankSwappedWidth > pitchAlign)
                    pitchAlign = bankSwappedWidth;

                var padDimens = padDimensions(
                     tileMode,
                     padDims,
                     (flags.value >> 4) & 1,
                     (flags.value >> 7) & 1,
                     pitchAlign,
                     heightAlign,
                     microTileThickness);

                expPitch = padDimens.Item1;
                expHeight = padDimens.Item2;
                expNumSlices = padDimens.Item3;

                pPitchOut = expPitch;
                pHeightOut = expHeight;
                pNumSlicesOut = expNumSlices;
                pSurfSize = (expHeight * expPitch * expNumSlices * bpp * numSamples + 7) / 8;
                pTileModeOut = expTileMode;
                pBaseAlign = baseAlign;
                pPitchAlign = pitchAlign;
                pHeightAlign = heightAlign;
                pDepthAlign = microTileThickness;
                result = valid;
            }

            else
            {
                var tup = computeSurfaceAlignmentsMacroTiled(
                    baseTileMode,
                    bpp,
                    flags,
                    numSamples);

                baseAlign = tup.Item1;
                pitchAlign = tup.Item2;
                heightAlign = tup.Item3;
                macroWidth = tup.Item4;
                macroHeight = tup.Item5;

                pitchAlignFactor = Math.Max(1, 32 / bpp);

                if (expPitch < pitchAlign * pitchAlignFactor || expHeight < heightAlign)
                {
                    expTileMode = 2;

                    var microTileInfo = computeSurfaceInfoMicroTiled(
                        2,
                        bpp,
                        numSamples,
                        pitch,
                        height,
                        numSlices,
                        mipLevel,
                        padDims,
                        flags);

                    result = microTileInfo[0];
                    pPitchOut = microTileInfo[1];
                    pHeightOut = microTileInfo[2];
                    pNumSlicesOut = microTileInfo[3];
                    pSurfSize = microTileInfo[4];
                    pTileModeOut = microTileInfo[5];
                    pBaseAlign = microTileInfo[6];
                    pPitchAlign = microTileInfo[7];
                    pHeightAlign = microTileInfo[8];
                    pDepthAlign = microTileInfo[9];
                }

                else
                {
                    tup = computeSurfaceAlignmentsMacroTiled(
                        tileMode,
                        bpp,
                        flags,
                        numSamples);

                    baseAlign = tup.Item1;
                    pitchAlign = tup.Item2;
                    heightAlign = tup.Item3;
                    macroWidth = tup.Item4;
                    macroHeight = tup.Item5;

                    bankSwappedWidth = computeSurfaceBankSwappedWidth((AddrTileMode)tileMode, bpp, pitch, numSamples);
                    if (bankSwappedWidth > pitchAlign)
                        pitchAlign = bankSwappedWidth;

                    var padDimens = padDimensions(
                        tileMode,
                        padDims,
                        (flags.value >> 4) & 1,
                        (flags.value >> 7) & 1,
                        pitchAlign,
                        heightAlign,
                        microTileThickness);

                    expPitch = padDimens.Item1;
                    expHeight = padDimens.Item2;
                    expNumSlices = padDimens.Item3;

                    pPitchOut = expPitch;
                    pHeightOut = expHeight;
                    pNumSlicesOut = expNumSlices;
                    pSurfSize = (expHeight * expPitch * expNumSlices * bpp * numSamples + 7) / 8;

                    pTileModeOut = expTileMode;
                    pBaseAlign = baseAlign;
                    pPitchAlign = pitchAlign;
                    pHeightAlign = heightAlign;
                    pDepthAlign = microTileThickness;
                    result = valid;
                }
            }

            return new uint[] { result, pPitchOut, pHeightOut,
                pNumSlicesOut, pSurfSize, pTileModeOut, pBaseAlign, pitchAlign, heightAlign, pDepthAlign};
        }
        private static Tuple<uint, uint, uint> computeSurfaceAlignmentsMicroTiled(uint tileMode, uint bpp, Flags flags, uint numSamples)
        {
            switch (bpp)
            {
                case 24:
                case 48:
                case 96:
                    bpp /= 3;
                    break;
            }
            uint thickness = computeSurfaceThickness((AddrTileMode)tileMode);
            uint baseAlign = 256;
            uint pitchAlign = Math.Max(8, 256 / bpp / numSamples / thickness);
            uint heightAlign = 8;

            pitchAlign = adjustPitchAlignment(flags, pitchAlign);

            return new Tuple<uint, uint, uint>(baseAlign, pitchAlign, heightAlign);

        }
        private static Tuple<uint, uint, uint, uint, uint> computeSurfaceAlignmentsMacroTiled(uint tileMode, uint bpp, Flags flags, uint numSamples)
        {
            uint aspectRatio = computeMacroTileAspectRatio((AddrTileMode)tileMode);
            uint thickness = computeSurfaceThickness((AddrTileMode)tileMode);

            switch (bpp)
            {
                case 24:
                case 48:
                case 96:
                    bpp /= 3;
                    break;
                case 3:
                    bpp = 1;
                    break;
            }
            uint macroTileWidth = 32 / aspectRatio;
            uint macroTileHeight = aspectRatio * 16;

            uint pitchAlign = Math.Max(macroTileWidth, macroTileWidth * (256 / bpp / (8 * thickness) / numSamples));
            pitchAlign = adjustPitchAlignment(flags, pitchAlign);

            uint heightAlign = macroTileHeight;
            uint macroTileBytes = numSamples * ((bpp * macroTileHeight * macroTileWidth + 7) >> 3);

            uint baseAlign;

            if (thickness == 1)
                baseAlign = Math.Max(macroTileBytes, (numSamples * heightAlign * bpp * pitchAlign + 7) >> 3);
            else
                baseAlign = Math.Max(256, (4 * heightAlign * bpp * pitchAlign + 7) >> 3);

            uint microTileBytes = (thickness * numSamples * (bpp << 6) + 7) >> 3;
            uint numSlicesPerMicroTile;
            if (microTileBytes < 2048)
                numSlicesPerMicroTile = 1;
            else
                numSlicesPerMicroTile = microTileBytes / 2048;

            baseAlign /= numSlicesPerMicroTile;

            return new Tuple<uint, uint, uint, uint, uint>(baseAlign, pitchAlign, heightAlign, macroTileWidth, macroTileHeight);
        }
        private static uint adjustPitchAlignment(Flags flags, uint pitchAlign)
        {
            if (((flags.value >> 13) & 1) != 0)
                pitchAlign = powTwoAlign(pitchAlign, 0x20);

            return pitchAlign;
        }
        private static uint adjustSurfaceInfo(uint elemMode, uint expandX, uint expandY, uint bpp, uint width, uint height)
        {
            uint bBCnFormat = 0;
            uint widtha, heighta;

            switch (elemMode)
            {
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    if (bpp != 0)
                        bBCnFormat = 1;

                    break;
            }

            if (width != 0 && height != 0)
            {
                if (expandX > 1 || expandY > 1)
                {
                    if (elemMode == 4)
                    {
                        widtha = expandX * width;
                        heighta = expandY * height;
                    }
                    else if (bBCnFormat != 0)
                    {
                        widtha = width / expandX;
                        heighta = height / expandY;
                    }   
                    else
                    {
                        widtha = (width + expandX - 1) / expandX;
                        heighta = (height + expandY - 1) / expandY;
                    }

                    pIn.width = Math.Max(1, widtha);
                    pIn.height = Math.Max(1, heighta);
                }
            }

            if (bpp != 0)
            {
                switch (elemMode)
                {
                    case 4:
                        pIn.bpp = bpp / expandX / expandY;
                        break;

                    case 5:
                    case 6:
                        pIn.bpp = expandY * expandX * bpp;
                        break;

                    case 9:
                    case 12:
                        pIn.bpp = 64;
                        break;

                    case 10:
                    case 11:
                    case 13:
                        pIn.bpp = 128;
                        break;

                    default:
                        pIn.bpp = bpp;
                        break;
                }

                return pIn.bpp;
            }

            return 0;
        }
        private static uint useTileIndex(int index)
        {
            return 0;
        }
        private static void computeMipLevel()
        {
            uint slices = 0;
            uint height = 0;
            uint width = 0;
            uint hwlHandled = 0;

            if (49 <= pIn.format && pIn.format <= 55 && (pIn.mipLevel == 0 || ((pIn.flags.value >> 12) & 1) != 0))
            {
                pIn.width = powTwoAlign(pIn.width, 4);
                pIn.height = powTwoAlign(pIn.height, 4);
            }


            if (DebugSurface)
            {
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine(" hwlComputeMipLevel");
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine(" pIn.width " + pIn.width);
                Console.WriteLine(" pIn.height " + pIn.height);
                Console.WriteLine(" pIn.numSlices " + pIn.numSlices);
            }

            hwlHandled = hwlComputeMipLevel();
            if (DebugSurface)
            {
                Console.WriteLine(" Output:");
                Console.WriteLine(" pIn.width " + pIn.width);
                Console.WriteLine(" pIn.height " + pIn.height);
                Console.WriteLine(" pIn.numSlices " + pIn.numSlices);
                Console.WriteLine("-------------------------------------------");
            }

            if (hwlHandled == 0 && pIn.mipLevel != 0 && ((pIn.flags.value >> 12) & 1) != 0)
            {
                width = Math.Max(1, pIn.width >> (int)pIn.mipLevel);
                height = Math.Max(1, pIn.height >> (int)pIn.mipLevel);
                slices = Math.Max(1, pIn.numSlices);

                if (((pIn.flags.value >> 4) & 1) != 0)
                    slices = Math.Max(1, slices >> (int)pIn.mipLevel);

                if (pIn.format != 47 && pIn.format != 48)
                {
                    width = nextPow2(width);
                    height = nextPow2(height);
                    slices = nextPow2(slices);
                }
                width = nextPow2(width);
                height = nextPow2(height);
                slices = nextPow2(slices);
            }
        }

        private static uint restoreSurfaceInfo(uint elemMode, uint expandX, uint expandY, uint bpp)
        {
            uint width, height;

            if (pOut.pixelPitch != 0 && pOut.pixelHeight != 0)
            {
                width = pOut.pixelPitch;
                height = pOut.pixelHeight;

                if (expandX > 1 || expandY > 1)
                {
                    if (elemMode == 4)
                    {
                        width /= expandX;
                        height /= expandY;
                    }

                    else
                    {
                        width *= expandX;
                        height *= expandY;
                    }

                    pOut.pixelPitch = Math.Max(1, width);
                    pOut.pixelHeight = Math.Max(1, height);
                }
            }
            if (bpp != 0)
            {
                switch (elemMode)
                {
                    case 4:
                        return expandY * expandX * bpp;

                    case 5:
                    case 6:
                        return bpp / expandX / expandY;

                    case 9:
                    case 12:
                        return 64;

                    case 10:
                    case 11:
                    case 13:
                        return 128;

                    default:
                        return bpp;
                }
            }

            return 0;
        }

        private static uint hwlComputeMipLevel()
        {
            uint handled = 0;

            if (49 <= pIn.format && pIn.format <= 55)
            {
                if (pIn.mipLevel != 0)
                {
                    uint width = pIn.width;
                    uint height = pIn.height;
                    uint slices = pIn.numSlices;

                    if (((pIn.flags.value >> 12) & 1) != 0)
                    {
                        uint widtha = width >> (int)pIn.mipLevel;
                        uint heighta = height >> (int)pIn.mipLevel;

                        if (((pIn.flags.value >> 4) & 1) == 0)
                            slices >>= (int)pIn.mipLevel;

                        width = Math.Max(1, widtha);
                        height = Math.Max(1, heighta);
                        slices = Math.Max(1, slices);
                    }

                    pIn.width = nextPow2(width);
                    pIn.height = nextPow2(height);
                    pIn.numSlices = slices;

                    handled = 1;
                }
            }
            return handled;
        }
        private static uint powTwoAlign(uint x, uint align)
        {
            return ~(align - 1) & (x + align - 1);
        }

        public static surfaceOut getSurfaceInfo(GX2SurfaceFormat surfaceFormat, uint surfaceWidth, uint surfaceHeight, uint surfaceDepth, uint surfaceDim, uint surfaceTileMode, uint surfaceAA, int level)
        {
            GX2Surface surface = new GX2Surface();

            uint dim = 0;
            uint width = 0;
            uint blockSize = 0;
            int numSamples = 0;
            int hwFormat = 0;

            var aSurfIn = new surfaceIn();
            var pSurfOut = new surfaceOut();

            hwFormat = (int)surfaceFormat & 0x3F;


            if (surfaceTileMode == 16)
            {
                numSamples = 1 << (int)surfaceAA;

                if (hwFormat < 0x31 || hwFormat > 0x35)
                    blockSize = 1;
                else
                    blockSize = 4;

                width = (uint)(~(blockSize - 1) & ((surfaceWidth >> level) + blockSize - 1));

                if (hwFormat == 0x35)
                    return pSurfOut;

                pSurfOut.bpp = formatHwInfo[hwFormat * 4];
                pSurfOut.size = 96;
                pSurfOut.pitch = (uint)(width / blockSize);
                pSurfOut.pixelBits = formatHwInfo[hwFormat * 4];
                pSurfOut.baseAlign = 1;
                pSurfOut.pitchAlign = 1;
                pSurfOut.heightAlign = 1;
                pSurfOut.depthAlign = 1;
                dim = surfaceDim;

                if (dim == 0)
                {
                    pSurfOut.height = 1;
                    pSurfOut.depth = 1;
                }
                else if (dim == 1)
                {
                    pSurfOut.height = Math.Max(1, surfaceHeight >> level);
                    pSurfOut.depth = 1;
                }
                else if (dim == 2)
                {
                    pSurfOut.height = Math.Max(1, surfaceHeight >> level);
                    pSurfOut.depth = Math.Max(1, surfaceDepth >> level);
                }
                else if (dim == 3)
                {
                    pSurfOut.height = Math.Max(1, surfaceHeight >> level);
                    pSurfOut.depth = Math.Max(6, surfaceDepth);
                }
                else if (dim == 4)
                {
                    pSurfOut.height = 1;
                    pSurfOut.depth = surfaceDepth;
                }
                else if (dim == 5)
                {
                    pSurfOut.height = Math.Max(1, surfaceHeight >> level);
                    pSurfOut.depth = surfaceDepth;
                }

                pSurfOut.height = (~(blockSize - 1) & (pSurfOut.height + blockSize - 1)) / blockSize;
                pSurfOut.pixelPitch = (~(blockSize - 1) & ((surfaceWidth >> level) + blockSize - 1));
                pSurfOut.pixelPitch = Math.Max(blockSize, pSurfOut.pixelPitch);
                pSurfOut.pixelHeight = (~(blockSize - 1) & ((surfaceHeight >> level) + blockSize - 1));
                pSurfOut.pixelHeight = Math.Max(blockSize, pSurfOut.pixelHeight);
                pSurfOut.pitch = Math.Max(1, pSurfOut.pitch);
                pSurfOut.height = Math.Max(1, pSurfOut.height);
                pSurfOut.surfSize = pSurfOut.bpp * numSamples * pSurfOut.depth * pSurfOut.height * pSurfOut.pitch >> 3;

                if (surfaceDim == 2)
                    pSurfOut.sliceSize = (uint)pSurfOut.surfSize;
                else
                    pSurfOut.sliceSize = (uint)pSurfOut.surfSize / pSurfOut.depth;

                pSurfOut.pitchTileMax = (pSurfOut.pitch >> 3) - 1;
                pSurfOut.heightTileMax = (pSurfOut.height >> 3) - 1;
                pSurfOut.sliceTileMax = (pSurfOut.height * pSurfOut.pitch >> 6) - 1;
            }
            else
            {
                aSurfIn.size = 60;
                aSurfIn.tileMode = surfaceTileMode & 0x0F;
                aSurfIn.format = (uint)hwFormat;
                aSurfIn.bpp = formatHwInfo[hwFormat * 4];
                aSurfIn.numSamples = (uint)1 << (int)surfaceAA;
                aSurfIn.numFrags = aSurfIn.numSamples;
                aSurfIn.width = (uint)Math.Max(1, surfaceWidth >> level);
                dim = surfaceDim;

                if (dim == 0)
                {
                    aSurfIn.height = 1;
                    aSurfIn.numSlices = 1;
                }
                else if (dim == 1)
                {
                    aSurfIn.height = (uint)Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = 1;
                }
                else if (dim == 2)
                {
                    aSurfIn.height = (uint)Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = (uint)Math.Max(1, surfaceDepth >> level);
                }
                else if (dim == 3)
                {
                    aSurfIn.height = (uint)Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = (uint)Math.Max(6, surfaceDepth);
                    aSurfIn.flags.value |= 0x10;
                }
                else if (dim == 4)
                {
                    aSurfIn.height = 1;
                    aSurfIn.numSlices = (uint)surfaceDepth;
                }
                else if (dim == 5)
                {
                    aSurfIn.height = (uint)Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = (uint)surfaceDepth;
                }
                else if (dim == 6)
                {
                    aSurfIn.height = (uint)Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = 1;
                }
                else if (dim == 7)
                {
                    aSurfIn.height = (uint)Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = (uint)surfaceDepth;
                }

                aSurfIn.slice = 0;
                aSurfIn.mipLevel = (uint)level;

                if (surfaceDim == 2)
                    aSurfIn.flags.value |= 0x20;

                if (level == 0)
                    aSurfIn.flags.value = (1 << 12) | aSurfIn.flags.value & 0xFFFFEFFF;
                else
                    aSurfIn.flags.value = aSurfIn.flags.value & 0xFFFFEFFF;

                pSurfOut.size = 96;
                computeSurfaceInfo(aSurfIn, pSurfOut);

                pSurfOut = pOut;
            }
            return pSurfOut;
        }
    }
}
