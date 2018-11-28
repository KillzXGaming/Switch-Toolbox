using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Switch_Toolbox.Library.IO;
using System.Text;

namespace FirstPlugin
{
    public enum BlockType : uint
    {
        Invalid = 0x00,
        EndOfFile = 0x01,
        AlignData = 0x02,
        VertexShaderHeader = 0x03,
        VertexShaderProgram = 0x05,
        PixelShaderHeader = 0x06,
        PixelShaderProgram = 0x07,
        GeometryShaderHeader = 0x08,
        GeometryShaderProgram = 0x09,
    }

    public class GTXHeader
    {
        public uint MajorVersion;
        public uint MinorVersion;
        public uint GpuVersion;
        public uint AlignMode;

        public void Read(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "Gfx2")
                throw new Exception($"Invalid signature {Signature}! Expected Gfx2.");

            uint HeaderSize = reader.ReadUInt32();
            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt32();
            GpuVersion = reader.ReadUInt32();
            AlignMode = reader.ReadUInt32();
            uint Reserved = reader.ReadUInt32();
            uint Reserved2 = reader.ReadUInt32();
        }
        public void Write(FileWriter reader)
        {
        }
    }
    public class GTXDataBlock
    {
        public uint MajorVersion;
        public uint MinorVersion;
        public BlockType BlockType;
        public uint Identifier;
        public uint index;

        public void Read(FileReader reader, GTXHeader header)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "BLK")
                throw new Exception($"Invalid signature {Signature}! Expected BLK.");
            uint HeaderSize = reader.ReadUInt32();

            MajorVersion = reader.ReadUInt32(); //Must be 0x01 for 6.x.x
            MinorVersion = reader.ReadUInt32(); //Must be 0x00 for 6.x.x
            BlockType = reader.ReadEnum<BlockType>(false);
            uint DataSize = reader.ReadUInt32();
            Identifier = reader.ReadUInt32();
            index = reader.ReadUInt32();
        }
    }
    public class GTX
    {
        //From https://github.com/jam1garner/Smash-Forge/blob/master/Smash%20Forge/Filetypes/Textures/GTX.cs
        //Todo. Add swizzling back
        public struct GX2Surface
        {
            public int dim;
            public int width;
            public int height;
            public int depth;
            public int numMips;
            public int format;
            public int aa;
            public int use;
            public int resourceFlags;
            public int imageSize;
            public int imagePtr;
            public int pMem;
            public int mipSize;
            public int mipPtr;
            public int tileMode;
            public int swizzle;
            public int alignment;
            public int pitch;
            public uint bpp;

            public byte[] data;

            public int[] mipOffset;
        };

        public static int m_configFlags = 4;
        public static int ADDR_OK = 0;

        public static int expPitch = 0;
        public static int expHeight = 0;
        public static int expNumSlices = 0;

        public class surfaceIn
        {
            public int size = 0;
            public int tileMode = 0;
            public int format = 0;
            public int bpp = 0;
            public int numSamples = 0;
            public int width = 0;
            public int height = 0;
            public int numSlices = 0;
            public int slice = 0;
            public int mipLevel = 0;
            public Flags flags = new Flags();
            public int numFrags = 0;
            public TileInfo tileType = new TileInfo();
            public int tileIndex = 0;
        }
        public class surfaceOut
        {
            public int size = 0;
            public int pitch = 0;
            public int height = 0;
            public int depth = 0;
            public int surfSize = 0;
            public int tileMode = 0;
            public int baseAlign = 0;
            public int pitchAlign = 0;
            public int heightAlign = 0;
            public int depthAlign = 0;
            public int bpp = 0;
            public int pixelPitch = 0;
            public int pixelHeight = 0;
            public int pixelBits = 0;
            public int sliceSize = 0;
            public int pitchTileMax = 0;
            public int heightTileMax = 0;
            public int sliceTileMax = 0;
            public int pTileInfo = 0;
            public TileInfo tileType = new TileInfo();
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

        static surfaceIn  pIn = new surfaceIn();
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

        public static List<byte[]> Decode(GX2Surface tex, byte[] MipData)
        {
            var surfInfo = getSurfaceInfo((GX2SurfaceFormat)tex.format, tex.width, tex.height, tex.depth, (uint)tex.dim, (uint)tex.tileMode, (uint)tex.aa, 0);
            if (surfInfo.depth != 1)
                throw new Exception("Unsupported Depth!");

            List<byte[]> result = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < tex.numMips; mipLevel++)
            {
                int size;
                if (IsFormatBCN((GX2SurfaceFormat)tex.format))
                    size = ((Math.Max(1, tex.width >> mipLevel) + 3) >> 2) * ((Math.Max(1, tex.height >> mipLevel) + 3) >> 2) * (int)tex.bpp;
                else
                    size = Math.Max(1, tex.width >> mipLevel) * Math.Max(1, tex.height >> mipLevel) * (int)tex.bpp;

                byte[] data;
                if (mipLevel == 0)
                {
                    data = new byte[surfInfo.surfSize];
                }
                else
                {
                    int mipOffset;
                    if (mipLevel == 1)
                        mipOffset = tex.mipOffset[mipLevel - 1] - surfInfo.surfSize;
                    else
                        mipOffset = tex.mipOffset[mipLevel - 1];

                    surfInfo = getSurfaceInfo((GX2SurfaceFormat)tex.format, tex.width, tex.height, tex.depth, (uint)tex.dim, (uint)tex.tileMode, (uint)tex.aa, mipLevel);

                    data = new byte[surfInfo.surfSize - mipOffset];
                    Array.Copy(MipData, mipOffset, data, 0, surfInfo.surfSize);
                }
                byte[] deswizzled = deswizzle(Math.Max(1, (uint)tex.width >> mipLevel), Math.Max(1, (uint)tex.height >> mipLevel), (uint)surfInfo.height, (uint)tex.format,
                (uint)surfInfo.tileMode, (uint)tex.swizzle, (uint)surfInfo.pitch, (uint)surfInfo.bpp, data);
                //Create a copy and use that to remove uneeded data
                byte[] result_ = new byte[size];
                Array.Copy(deswizzled, 0, result_, 0, size);
            }
            return result;
        }




        /*---------------------------------------
         * 
         * Code ported from AboodXD's GTX Extractor https://github.com/aboood40091/GTX-Extractor/blob/master/gtx_extract.py
         * 
         * With help by Aelan!
         * 
         *---------------------------------------*/

        /*var s_textureFormats[] = {
        // internalFormat,  gxFormat,                                 glFormat,                         fourCC,    nutFormat, name, bpp, compressed
        { FORMAT_RGBA_8888, GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_UNORM, GL_RGBA8,                         0x00000000, 0x11, "RGBA_8888", 0x20, 0 },
        { FORMAT_ABGR_8888, GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_UNORM, GL_RGBA8,                         0x00000000, 0x0E, "ABGR_8888 (WIP)", 0x20, 0 },
        { FORMAT_DXT1,      GX2_SURFACE_FORMAT_T_BC1_UNORM,           GL_COMPRESSED_RGBA_S3TC_DXT1_EXT, 0x31545844, 0x00, "DXT1",  0x40,     1 },
        { FORMAT_DXT3,      GX2_SURFACE_FORMAT_T_BC2_UNORM,           GL_COMPRESSED_RGBA_S3TC_DXT3_EXT, 0x33545844, 0x01, "DXT3",  0x80,     1 },
        { FORMAT_DXT5,      GX2_SURFACE_FORMAT_T_BC3_UNORM,           GL_COMPRESSED_RGBA_S3TC_DXT5_EXT, 0x35545844, 0x02, "DXT5",  0x80,     1 },
        { FORMAT_ATI1,      GX2_SURFACE_FORMAT_T_BC4_UNORM,           GL_COMPRESSED_RED_RGTC1,          0x31495441, 0x15, "ATI1",  0x40,     1 },
        { FORMAT_ATI2,      GX2_SURFACE_FORMAT_T_BC5_UNORM,           GL_COMPRESSED_RG_RGTC2,           0x32495441, 0x16, "ATI2",  0x80,     1 },
        { FORMAT_INVALID,   GX2_SURFACE_FORMAT_INVALID,               0,                                0xFFFFFFFF, 0x00, nullptr, 0x00,     0 }
    };*/

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

        static uint m_banks = 4;
        static uint m_banksBitcount = 2;
        static uint m_pipes = 2;
        static uint m_pipesBitcount = 1;
        static uint m_pipeInterleaveBytes = 256;
        static uint m_pipeInterleaveBytesBitcount = 8;
        static uint m_rowSize = 2048;
        static uint m_swapSize = 256;
        static uint m_splitSize = 2048;

        static uint m_chipFamily = 2;

        static uint MicroTilePixels = 64;


        private static byte[] swizzleSurf(uint width, uint height, uint height_, uint format, uint tileMode, uint swizzle_,
                uint pitch, uint bitsPerPixel, byte[] data, int swizzle)
        {
            byte[] result = new byte[data.Length];

            uint bytesPerPixel = bitsPerPixel / 8;

            uint pipeSwizzle, bankSwizzle, pos_;
            uint pos;

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
                    var tileGrp1 = new List<uint> { 0, 1 };
                    var tileGrp2 = new List<uint> { 2, 3 };


                    if (tileGrp1.Contains(tileMode))
                        pos = (uint)(y * pitch + x) * bytesPerPixel;
                    else if (tileGrp2.Contains(tileMode))
                    {
                        pos = computeSurfaceAddrFromCoordMicroTiled(x, y, bitsPerPixel, pitch, tileMode);
                    }
                    else
                    {
                        pos = computeSurfaceAddrFromCoordMacroTiled(x, y, bitsPerPixel, (int)pitch, (int)height_, (int)tileMode, (int)pipeSwizzle, (int)bankSwizzle);
                    }
                    pos = 0;

                     pos_ = (uint)(y * width + x) * bytesPerPixel;

                    if (pos_ + bytesPerPixel <= data.Length && pos + bytesPerPixel <= data.Length)
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
            return formatHwInfo[(surfaceFormat & 0x35) + 4];
        }

        private static uint computePipeFromCoordWoRotation(uint x, uint y)
        {
            return ((y >> 3) ^ (x >> 3)) & 1;
        }

        private static uint computeBankFromCoordWoRotation(uint x, uint y)
        {
            return ((y >> 5) ^ (x >> 3)) & 1 | 2 * (((y >> 4) ^ (x >> 4)) & 1);
        }

        private static bool isThickMacroTiled(uint tileMode)
        {
            var Grp1 = new List<uint> { 7, 11, 13, 15 };
            if (Grp1.Contains(tileMode))
                return true;

            return false;
        }

        private static bool isBankSwapped(uint tileMode)
        {
            var Grp1 = new List<uint> { 8, 9, 10, 11, 14, 15 };
            if (Grp1.Contains(tileMode))
                return true;

            return false;
        }

        private static uint computeMacroTileAspectRatio(uint tileMode)
        {
            var Grp1 = new List<uint> { 5, 9 };
            var Grp2 = new List<uint> { 6, 10 };

            if (Grp1.Contains(tileMode))
                return 2;

            if (Grp2.Contains(tileMode))
                return 4;

            return 1;
        }

        private static uint computeSurfaceBankSwappedWidth(uint tileMode, uint bpp, uint pitch, uint numSamplers)
        {
            if (isBankSwapped(tileMode) == true)
                return 0;

            return 0;
        }

        private static uint computeSurfaceAddrFromCoordMicroTiled(int x, int y, uint bpp, uint pitch, uint tileMode)
        {
            return 0; //Todo

 /*           uint microTileThickness = 1;

            if (tileMode == 3)
                microTileThickness = 4;

            uint microTileBytes = (MicroTilePixels * microTileThickness * bpp + 7) / 8;
            uint microTilesPerRow = pitch >> 3;
            uint microTileIndexX = (uint)x >> 3;
            uint microTileIndexY = (uint)y >> 3;

            uint microTileOffset = microTileBytes * (microTileIndexX + microTileIndexY * microTilesPerRow);

            uint pixelIndex = computePixelIndexWithinMicroTile(x, y, bpp, tileMode);

            uint pixelOffset = bpp * pixelIndex;
            pixelOffset >>= 3;

            return pixelOffset + microTileOffset;*/
        }



        private static int getBPP(int i)
        {
            switch ((GX2SurfaceFormat)i)
            {
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_TC_R5_G5_B5_A1_UNORM:
                    return 0x10;
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_UNORM:
                    return 0x20;
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_SRGB:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_SNORM:
                    return 0x40;
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_UNORM:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_SRGB:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_SRGB:
                case GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_SNORM:
                    return 0x80;
            }
            return -1;
        }



        private static uint computeSurfaceAddrFromCoordMacroTiled(int x, int y, uint bpp, int pitch,
            int height, int tileMode, int pipeSwizzle, int bankSwizzle)
        {
            return 0; //Todo
        }

        private static int computePixelIndexWithinMicroTile(int x, int y, uint bpp)
        {
            int bits = ((x & 4) << 1) | ((y & 2) << 3) | ((y & 4) << 3);

            if (bpp == 0x20 || bpp == 0x60)
            {
                bits |= (x & 1) | (x & 2) | ((y & 1) << 2);
            }
            else if (bpp == 0x40)
            {
                bits |= (x & 1) | ((y & 1) << 1) | ((x & 2) << 1);
            }
            else if (bpp == 0x80)
            {
                bits |= (y & 1) | ((x & 1) << 1) | ((x & 2) << 1);
            }

            return bits;
        }

        private static int getFormatBpp(int format)
        {
            int hwFormat = format & 0x3F;
            return formatHwInfo[hwFormat * 4];
        }

        private static int computeSurfaceThickness(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_1D_TILED_THICK:
                case AddrTileMode.ADDR_TM_2D_TILED_THICK:
                case AddrTileMode.ADDR_TM_2B_TILED_THICK:
                case AddrTileMode.ADDR_TM_3D_TILED_THICK:
                case AddrTileMode.ADDR_TM_3B_TILED_THICK:
                    {
                        return 4;
                    }
                case AddrTileMode.ADDR_TM_2D_TILED_XTHICK:
                case AddrTileMode.ADDR_TM_3D_TILED_XTHICK:
                    {
                        return 8;
                    }
            }

            return 1;
        }

        private static int isThickMacroTiled(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2D_TILED_THICK:
                case AddrTileMode.ADDR_TM_2B_TILED_THICK:
                case AddrTileMode.ADDR_TM_3D_TILED_THICK:
                case AddrTileMode.ADDR_TM_3B_TILED_THICK:
                    {
                        return 1;
                    }
            }

            return 0;
        }

        private static int isBankSwappedTileMode(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2B_TILED_THIN1:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN2:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN4:
                case AddrTileMode.ADDR_TM_2B_TILED_THICK:
                case AddrTileMode.ADDR_TM_3B_TILED_THIN1:
                case AddrTileMode.ADDR_TM_3B_TILED_THICK:
                    {
                        return 1;
                    }
            }
            return 0;
        }

        private static int computeSurfaceRotationFromTileMode(AddrTileMode tileMode)
        {
            switch ((int)tileMode)
            {
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    {
                        return 2;
                    }
                case 12:
                case 13:
                case 14:
                case 15:
                    {
                        return 1;
                    }
            }

            return 0;
        }

        private static int computePipeFromCoordWoRotation(int x, int y)
        {
            int pipe = ((y >> 3) ^ (x >> 3)) & 1;
            return pipe;
        }

        private static int computeBankFromCoordWoRotation(int x, int y)
        {
            int bankBit0 = ((y / (16 * 2)) ^ (x >> 3)) & 1;
            int bank = bankBit0 | 2 * (((y / (8 * 2)) ^ (x >> 4)) & 1);

            return bank;
        }

        private static int computeMacroTileAspectRatio(AddrTileMode tileMode)
        {
            switch (tileMode)
            {
                case AddrTileMode.ADDR_TM_2B_TILED_THIN1:
                case AddrTileMode.ADDR_TM_3D_TILED_THIN1:
                case AddrTileMode.ADDR_TM_3B_TILED_THIN1:
                    {
                        return 1;
                    }
                case AddrTileMode.ADDR_TM_2D_TILED_THIN2:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN2:
                    {
                        return 2;
                    }
                case AddrTileMode.ADDR_TM_2D_TILED_THIN4:
                case AddrTileMode.ADDR_TM_2B_TILED_THIN4:
                    {
                        return 4;
                    }
            }

            return 1;
        }

        private static int computeSurfaceBankSwappedWidth(AddrTileMode tileMode, int bpp, int numSamples, int pitch, int pSlicesPerTile)
        {
            int bankSwapWidth = 0;
            int numBanks = 4;
            int numPipes = 2;
            int swapSize = 256;
            int rowSize = 2048;
            int splitSize = 2048;
            int groupSize = 256;
            int slicesPerTile = 1;
            int bytesPerSample = 8 * bpp & 0x1FFFFFFF;
            int samplesPerTile = splitSize / bytesPerSample;

            if ((splitSize / bytesPerSample) != 0)
            {
                slicesPerTile = numSamples / samplesPerTile;
                if ((numSamples / samplesPerTile) == 0)
                {
                    slicesPerTile = 1;
                }
            }

            if (pSlicesPerTile != 0)
            {
                pSlicesPerTile = slicesPerTile;
            }

            if (isThickMacroTiled(tileMode) == 1)
            {
                numSamples = 4;
            }

            int bytesPerTileSlice = numSamples * bytesPerSample / slicesPerTile;

            if (isBankSwappedTileMode(tileMode) == 1)
            {
                int v7;
                int v8;
                int v9;

                int factor = computeMacroTileAspectRatio(tileMode);
                int swapTiles = (swapSize >> 1) / bpp;

                if (swapTiles != 0)
                {
                    v9 = swapTiles;
                }
                else
                {
                    v9 = 1;
                }

                int swapWidth = v9 * 8 * numBanks;
                int heightBytes = numSamples * factor * numPipes * bpp / slicesPerTile;
                int swapMax = numPipes * numBanks * rowSize / heightBytes;
                int swapMin = groupSize * 8 * numBanks / bytesPerTileSlice;

                if (swapMax >= swapWidth)
                {
                    if (swapMin <= swapWidth)
                    {
                        v7 = swapWidth;
                    }
                    else
                    {
                        v7 = swapMin;
                    }

                    v8 = v7;
                }
                else
                {
                    v8 = swapMax;
                }

                bankSwapWidth = v8;

                while (bankSwapWidth >= (2 * pitch))
                {
                    bankSwapWidth >>= 1;
                }
            }

            return bankSwapWidth;
        }

        private static int computePixelIndexWithinMicroTile(int x, int y, int z, int bpp, AddrTileMode tileMode, int microTileType)
        {
            int pixelBit0 = 0;
            int pixelBit1 = 0;
            int pixelBit2 = 0;
            int pixelBit3 = 0;
            int pixelBit4 = 0;
            int pixelBit5 = 0;
            int pixelBit6 = 0;
            int pixelBit7 = 0;
            int pixelBit8 = 0;
            int thickness = computeSurfaceThickness(tileMode);

            if (microTileType == 3)
            {
                pixelBit0 = x & 1;
                pixelBit1 = y & 1;
                pixelBit2 = z & 1;
                pixelBit3 = (x & 2) >> 1;
                pixelBit4 = (y & 2) >> 1;
                pixelBit5 = (z & 2) >> 1;
                pixelBit6 = (x & 4) >> 2;
                pixelBit7 = (y & 4) >> 2;
            }
            else
            {
                if (microTileType != 0)
                {
                    pixelBit0 = x & 1;
                    pixelBit1 = y & 1;
                    pixelBit2 = (x & 2) >> 1;
                    pixelBit3 = (y & 2) >> 1;
                    pixelBit4 = (x & 4) >> 2;
                    pixelBit5 = (y & 4) >> 2;
                }
                else
                {
                    if (bpp == 0x08)
                    {
                        pixelBit0 = x & 1;
                        pixelBit1 = (x & 2) >> 1;
                        pixelBit2 = (x & 4) >> 2;
                        pixelBit3 = (y & 2) >> 1;
                        pixelBit4 = y & 1;
                        pixelBit5 = (y & 4) >> 2;
                    }
                    else if (bpp == 0x10)
                    {
                        pixelBit0 = x & 1;
                        pixelBit1 = (x & 2) >> 1;
                        pixelBit2 = (x & 4) >> 2;
                        pixelBit3 = y & 1;
                        pixelBit4 = (y & 2) >> 1;
                        pixelBit5 = (y & 4) >> 2;
                    }
                    else if (bpp == 0x20 || bpp == 0x60)
                    {
                        pixelBit0 = x & 1;
                        pixelBit1 = (x & 2) >> 1;
                        pixelBit2 = y & 1;
                        pixelBit3 = (x & 4) >> 2;
                        pixelBit4 = (y & 2) >> 1;
                        pixelBit5 = (y & 4) >> 2;
                    }
                    else if (bpp == 0x40)
                    {
                        pixelBit0 = x & 1;
                        pixelBit1 = y & 1;
                        pixelBit2 = (x & 2) >> 1;
                        pixelBit3 = (x & 4) >> 2;
                        pixelBit4 = (y & 2) >> 1;
                        pixelBit5 = (y & 4) >> 2;
                    }
                    else if (bpp == 0x80)
                    {
                        pixelBit0 = y & 1;
                        pixelBit1 = x & 1;
                        pixelBit2 = (x & 2) >> 1;
                        pixelBit3 = (x & 4) >> 2;
                        pixelBit4 = (y & 2) >> 1;
                        pixelBit5 = (y & 4) >> 2;
                    }
                    else
                    {
                        pixelBit0 = x & 1;
                        pixelBit1 = (x & 2) >> 1;
                        pixelBit2 = y & 1;
                        pixelBit3 = (x & 4) >> 2;
                        pixelBit4 = (y & 2) >> 1;
                        pixelBit5 = (y & 4) >> 2;
                    }
                }

                if (thickness > 1)
                {
                    pixelBit6 = z & 1;
                    pixelBit7 = (z & 2) >> 1;
                }
            }

            if (thickness == 8)
            {
                pixelBit8 = (z & 4) >> 2;
            }

            return pixelBit0 |
                (pixelBit8 << 8) |
                (pixelBit7 << 7) |
                (pixelBit6 << 6) |
                (pixelBit5 << 5) |
                (pixelBit4 << 4) |
                (pixelBit3 << 3) |
                (pixelBit2 << 2) |
                (pixelBit1 << 1);
        }

        private static int surfaceAddrFromCoordMacroTiled(
            int x, int y, int slice, int sample, int bpp,
            int pitch, int height, int numSamples, AddrTileMode tileMode,
            int isDepth, int tileBase, int compBits,
            int pipeSwizzle, int bankSwizzle
        )
        {
            const int numPipes = 2;
            const int numBanks = 4;
            const int numGroupBits = 8;
            const int numPipeBits = 1;
            const int numBankBits = 2;

            int microTileThickness = computeSurfaceThickness(tileMode);
            int microTileBits = numSamples * bpp * (microTileThickness * (8 * 8));
            int microTileBytes = microTileBits >> 3;
            int microTileType = (isDepth == 1) ? 1 : 0;
            int pixelIndex = computePixelIndexWithinMicroTile(x, y, slice, bpp, tileMode, microTileType);

            int sampleOffset;
            int pixelOffset;
            if (isDepth == 1)
            {
                if (compBits != 0 && compBits != bpp)
                {
                    sampleOffset = tileBase + compBits * sample;
                    pixelOffset = numSamples * compBits * pixelIndex;
                }
                else
                {
                    sampleOffset = bpp * sample;
                    pixelOffset = numSamples * compBits * pixelIndex;
                }
            }
            else
            {
                sampleOffset = sample * (microTileBits / numSamples);
                pixelOffset = bpp * pixelIndex;
            }

            int elemOffset = pixelOffset + sampleOffset;
            int bytesPerSample = microTileBytes / numSamples;

            int samplesPerSlice;
            int numSampleSplits;
            int sampleSlice;

            if (numSamples <= 1 || microTileBytes <= 2048)
            {
                samplesPerSlice = numSamples;
                numSampleSplits = 1;
                sampleSlice = 0;
            }
            else
            {
                samplesPerSlice = 2048 / bytesPerSample;
                numSampleSplits = numSamples / samplesPerSlice;
                numSamples = samplesPerSlice;
                sampleSlice = elemOffset / (microTileBits / numSampleSplits);
                elemOffset %= microTileBits / numSampleSplits;
            }

            elemOffset >>= 3;

            int pipe = computePipeFromCoordWoRotation(x, y);
            int bank = computeBankFromCoordWoRotation(x, y);
            int bankPipe = pipe + numPipes * bank;
            int rotation = computeSurfaceRotationFromTileMode(tileMode);
            int swizzle = pipeSwizzle + numPipes * bankSwizzle;
            int sliceIn = slice;

            if (isThickMacroTiled(tileMode) == 1)
            {
                sliceIn >>= 2;
            }

            bankPipe ^= numPipes * sampleSlice * ((numBanks >> 1) + 1) ^ (swizzle + sliceIn * rotation);
            bankPipe %= numPipes * numBanks;
            pipe = bankPipe % numPipes;
            bank = bankPipe / numPipes;

            int sliceBytes = (height * pitch * microTileThickness * bpp * numSamples + 7) / 8;
            int sliceOffset = sliceBytes * ((sampleSlice + numSampleSplits * slice) / microTileThickness);
            int macroTilePitch = 8 * 4; // m_banks
            int macroTileHeight = 8 * 2; // m_pipes
            int v18 = (int)tileMode - 5;

            if ((int)tileMode == 5 || (int)tileMode == 9)
            {
                macroTilePitch >>= 1;
                macroTileHeight *= 2;
            }
            else if ((int)tileMode == 6 || (int)tileMode == 10)
            {
                macroTilePitch >>= 2;
                macroTileHeight *= 4;
            }

            int macroTilesPerRow = pitch / macroTilePitch;
            int macroTileBytes = (numSamples * microTileThickness * bpp * macroTileHeight * macroTilePitch + 7) >> 3;
            int macroTileIndexX = x / macroTilePitch;
            int macroTileIndexY = y / macroTileHeight;
            int macroTileOffset = (x / macroTilePitch + pitch / macroTilePitch * (y / macroTileHeight)) * macroTileBytes;

            int bankSwapWidth;
            int swapIndex;
            int bankMask;

            byte[] bankSwapOrder = { 0, 1, 3, 2 };
            switch ((int)tileMode)
            {
                case 8:
                case 9:
                case 10:
                case 11:
                case 14:
                case 15:
                    {
                        bankSwapWidth = computeSurfaceBankSwappedWidth(tileMode, bpp, numSamples, pitch, 0);
                        swapIndex = macroTilePitch * macroTileIndexX / bankSwapWidth;
                        bankMask = 3; // m_banks-1
                        bank ^= bankSwapOrder[swapIndex & bankMask];
                    }
                    break;
            }

            int p4 = pipe << numGroupBits;
            int p5 = bank << (numPipeBits + numGroupBits);
            int numSwizzleBits = numBankBits + numPipeBits;
            int unk1 = (macroTileOffset + sliceOffset) >> numSwizzleBits;
            int unk2 = ~((1 << numGroupBits) - 1);
            int unk3 = (elemOffset + unk1) & unk2;
            int groupMask = (1 << numGroupBits) - 1;
            int offset1 = macroTileOffset + sliceOffset;
            int unk4 = elemOffset + (offset1 >> numSwizzleBits);

            int subOffset1 = unk3 << numSwizzleBits;
            int subOffset2 = groupMask & unk4;

            return subOffset1 | subOffset2 | p4 | p5;
        }

        private static int getFillSizeFieldsFlags()
        {
            return (m_configFlags >> 6) & 1;
        }

        private static void computeSurfaceInfo(surfaceIn aSurfIn, surfaceOut aSurfOut)
        {
            //Todo!
            surfaceIn pIn = aSurfIn;
            surfaceOut pOut = aSurfOut;

            int v4 = 0;
            int v6 = 0;
            int v7 = 0;
            int v8 = 0;
            int v10 = 0;
            int v11 = 0;
            int v12 = 0;
            int v18 = 0;

            TileInfo tileInfoNull = new TileInfo();
            int sliceFlags = 0;
            int returnCode = 0;

            if (getFillSizeFieldsFlags() == 1 && (pIn.size != 60 || pOut.size != 96))
                returnCode = 6;

            if (pIn.bpp > 0x80)
                returnCode = 3;

            if (returnCode == ADDR_OK)
            {
                v18 = 0;


                computeMipLevel();

                int width = pIn.width;
                int height = pIn.height;
                int bpp = pIn.bpp;
                int expandX = 1;
                int expandY = 1;

            }
        }
        private static void computeMipLevel()
        {
            uint slices = 0;
            uint height = 0;
            uint width = 0;
            uint hwlHandled = 0;

            if ((49 <= pIn.format && 49 <= 55 && pIn.mipLevel != 49) || 49 <= ((pIn.flags.value >> 12) & 1))
            {
                pIn.width = powTwoAlign(pIn.width, 4);
                pIn.height = powTwoAlign(pIn.height, 4);
            }
            hwlHandled = hwlComputeMipLevel();

        }
        private static uint hwlComputeMipLevel()
        {
            //This part might be wrong?
            uint handled = 0;

            if (49 <= pIn.format && 49 <= 55)
            {
                if (pIn.mipLevel != 0)
                {
                    int width = pIn.width;
                    int height = pIn.height;
                    int slices = pIn.numSlices;

                    if (((pIn.flags.value >> 12) & 1) != 0)
                    {
                        int widtha = width >> pIn.mipLevel;
                        int heighta = height >> pIn.mipLevel;

                        if (((pIn.flags.value >> 4) & 1) == 0)
                            slices >>= pIn.mipLevel;

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
        private static int nextPow2(int dim)
        {
            uint newDim = 1;
            if (newDim <= 0x7FFFFFFF)
            {
                while (newDim < dim)
                    newDim *= 2;
            }
            else
                newDim = 2147483648;

            return (int)newDim;
        }
        private static int powTwoAlign(int x, int align)
        {
            return (int)(align - 1) & (x + align - 1);
        }

        public static surfaceOut getSurfaceInfo(GX2SurfaceFormat surfaceFormat, int surfaceWidth, int surfaceHeight, int surfaceDepth, uint surfaceDim, uint surfaceTileMode, uint surfaceAA, int level)
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

                width = (uint)((blockSize - 1) & ((surfaceWidth >> level) + blockSize - 1));

                if (hwFormat == 0x35)
                    return pSurfOut;

                pSurfOut.bpp = formatHwInfo[hwFormat * 4];
                pSurfOut.size = 96;
                pSurfOut.pitch = (int)(width / blockSize);
                pSurfOut.pixelBits = formatHwInfo[hwFormat * 4];
                pSurfOut.baseAlign = 1;
                pSurfOut.pitchAlign = 1;
                pSurfOut.heightAlign = 1;
                pSurfOut.depthAlign = 1;

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

                pSurfOut.height = (int)(((blockSize - 1) & (pSurfOut.height + blockSize - 1)) / blockSize);
                pSurfOut.pixelPitch = (int)((blockSize - 1) & ((surfaceWidth >> level) + blockSize - 1));
                pSurfOut.pixelPitch = (int)Math.Max(blockSize, pSurfOut.pixelPitch);
                pSurfOut.pixelHeight = (int)((blockSize - 1) & ((surfaceHeight >> level) + blockSize - 1));
                pSurfOut.pixelHeight = (int)Math.Max(blockSize, pSurfOut.pixelHeight);
                pSurfOut.pitch = Math.Max(1, pSurfOut.pitch);
                pSurfOut.height = Math.Max(1, pSurfOut.height);
                pSurfOut.surfSize = pSurfOut.bpp * numSamples * pSurfOut.depth * pSurfOut.height * pSurfOut.pitch >> 3;

                if (surfaceDim == 2)
                    pSurfOut.sliceSize = pSurfOut.surfSize;

                else
                    pSurfOut.sliceSize = pSurfOut.surfSize / pSurfOut.depth;

                pSurfOut.pitchTileMax = (pSurfOut.pitch >> 3) - 1;
                pSurfOut.heightTileMax = (pSurfOut.height >> 3) - 1;
                pSurfOut.sliceTileMax = (pSurfOut.height * pSurfOut.pitch >> 6) - 1;
            }
            else
            {
                aSurfIn.size = 60;
                aSurfIn.tileMode = (int)surfaceTileMode & 0x0F;
                aSurfIn.format = hwFormat;
                aSurfIn.bpp = formatHwInfo[hwFormat * 4];
                aSurfIn.numSamples = 1 << (int)surfaceAA;
                aSurfIn.numFrags = aSurfIn.numSamples;
                aSurfIn.width = Math.Max(1, surfaceWidth >> level);

                if (dim == 0)
                {
                    aSurfIn.height = 1;
                    aSurfIn.numSlices = 1;
                }
                else if (dim == 1)
                {
                    aSurfIn.height = Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = 1;
                }
                else if (dim == 2)
                {
                    aSurfIn.height = Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = Math.Max(1, surfaceDepth >> level);
                }
                else if (dim == 3)
                {
                    aSurfIn.height = Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = Math.Max(6, surfaceDepth);
                    aSurfIn.flags.value |= 0x10;
                }
                else if (dim == 4)
                {
                    aSurfIn.height = 1;
                    aSurfIn.numSlices = surfaceDepth;
                }
                else if (dim == 5)
                {
                    aSurfIn.height = Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = surfaceDepth;
                }
                else if (dim == 6)
                {
                    aSurfIn.height = Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = 1;
                }
                else if (dim == 7)
                {
                    aSurfIn.height = Math.Max(1, surfaceHeight >> level);
                    aSurfIn.numSlices = surfaceDepth;
                }
            }

            pSurfOut.size = 96;
            computeSurfaceInfo(aSurfIn, pSurfOut);

            pSurfOut = pOut;

            return pSurfOut;
        }
    }
}
