using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.IO;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library.Rendering;
using Ryujinx.Graphics.Gal.Texture; //For ASTC
using Toolbox.Library.NodeWrappers;
using Toolbox.Library.Forms;

namespace Toolbox.Library
{
    public enum STCompressionMode
    {
        Slow,
        Normal,
        Fast
    }

    public enum STChannelType
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Alpha = 3,
        One = 4,
        Zero = 5,
    }

    public enum PlatformSwizzle
    {
        None = 0,
        Platform_3DS = 1,
        Platform_Wii = 2,
        Platform_Gamecube = 3,
        Platform_WiiU = 4,
        Platform_Switch = 5,
        Platform_Ps4 = 6,
        Platform_Ps3 = 7,
        Platform_Ps2 = 8,
        Platform_Ps1 = 9,
    }

    public enum STSurfaceType
    {
        Texture1D,
        Texture2D,
        Texture3D,
        TextureCube,
        Texture1D_Array,
        Texture2D_Array,
        Texture2D_Mulitsample,
        Texture2D_Multisample_Array,
        TextureCube_Array,
    }


    public class EditedBitmap
    {
        public int ArrayLevel = 0;
        public Bitmap bitmap;
    }

    public abstract class STGenericTexture : STGenericWrapper
    {
        public STGenericTexture()
        {
            RenderableTex = new RenderableTex();
            RenderableTex.GLInitialized = false;

            RedChannel = STChannelType.Red;
            GreenChannel = STChannelType.Green;
            BlueChannel = STChannelType.Blue;
            AlphaChannel = STChannelType.Alpha;
        }

        public virtual UserControl GetEditor() { return new UserControl(); }

        public virtual void FillEditor(UserControl control) { }

        //A class that configs how the image should output (on display, and on export/replace)
        public ImageParameters Parameters = new ImageParameters();

        public bool IsCubemap
        {
            get
            {
                return ArrayCount == 6 || ArrayCount % 6 == 0;
            }
        }

        public STSurfaceType SurfaceType = STSurfaceType.Texture2D;

        /// <summary>
        /// The swizzle method to use when decoding or encoding back a texture.
        /// </summary>
        public PlatformSwizzle PlatformSwizzle;

        public bool IsSwizzled { get; set; } = true;

        /// <summary>
        /// Is the texture edited or not. Used for the image editor for saving changes.
        /// </summary>
        public bool IsEdited { get; set; } = false;

        /// <summary>
        /// An array of <see cref="EditedBitmap"/> from the image editor to be saved back.
        /// </summary>
        public EditedBitmap[] EditedImages { get; set; }

        //If the texture can be edited or not. Disables some functions in image editor if false
        //If true, the editors will call "SetImageData" for setting data back to the original data.
        public abstract bool CanEdit { get; set; }

        public STChannelType RedChannel = STChannelType.Red;
        public STChannelType GreenChannel = STChannelType.Green;
        public STChannelType BlueChannel = STChannelType.Blue;
        public STChannelType AlphaChannel = STChannelType.Alpha;

        /// <summary>
        /// The total length of all the bytes given from GetImageData.
        /// </summary>
        public long DataSizeInBytes
        {
            get
            {
                long totalSize = 0;

                if (PlatformSwizzle == PlatformSwizzle.Platform_3DS)
                {
                    for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
                    {
                        for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                        {
                            uint width = (uint)Math.Max(1, Width >> mipLevel);
                            uint height = (uint)Math.Max(1, Height >> mipLevel);

                            totalSize += CTR_3DS.CalculateLength((int)width, (int)height, CTR_3DS.ConvertToPICAFormat(Format));
                        }
                    }

                    return totalSize;
                }

                if (PlatformSwizzle == PlatformSwizzle.Platform_Gamecube)
                {
                    for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
                    {
                        for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                        {
                            uint width = (uint)Math.Max(1, Width >> mipLevel);
                            uint height = (uint)Math.Max(1, Height >> mipLevel);

                            totalSize += Decode_Gamecube.GetDataSize((uint)Decode_Gamecube.FromGenericFormat(Format), width, height);
                        }
                    }

                    return totalSize;
                }

                if (FormatTable.ContainsKey(Format))
                {
                    uint bpp = GetBytesPerPixel(Format); 

                    for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
                    {
                        for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                        {
                            uint width = (uint)Math.Max(1, Width >> mipLevel);
                            uint height = (uint)Math.Max(1, Height >> mipLevel);

                            uint size = width * height * bpp;
                            if (IsCompressed(Format))
                            {
                                size = ((width + 3) >> 2) * ((Height + 3) >> 2) * bpp;
                                if (size < bpp)
                                    size = bpp;
                            }

                            totalSize += size;
                        }
                    }
                }

                return totalSize;
            }
        }

        public string DataSize { get { return STMath.GetFileSize(DataSizeInBytes, 5); } }

        public abstract byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0);

        private byte[] paletteData = new byte[0];

        public virtual byte[] GetPaletteData() { return paletteData; }

        public virtual void SetPaletteData(byte[] data, PALETTE_FORMAT format)
        {
            paletteData = data;
            PaletteFormat = format;
        }

        public List<Surface> Get3DSurfaces(int IndexStart = 0, bool GetAllSurfaces = true, int GetSurfaceAmount = 1)
        {
            if (GetAllSurfaces)
                GetSurfaceAmount = (int)Depth;

            var surfaces = new List<Surface>();
            for (int depthLevel = 0; depthLevel < Depth; depthLevel++)
            {
                bool IsLower = depthLevel < IndexStart;
                bool IsHigher = depthLevel >= (IndexStart + GetSurfaceAmount);
                if (!IsLower && !IsHigher)
                {
                    List<byte[]> mips = new List<byte[]>();
                    for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                    {
                        mips.Add(GetImageData(0, mipLevel, depthLevel));
                    }

                    surfaces.Add(new Surface() { mipmaps = mips });
                }
            }

            return surfaces;
        }

        //
        //Gets a list of surfaces given the start index of the array and the amount of arrays to obtain
        //
        public List<Surface> GetSurfaces(int ArrayIndexStart = 0, bool GetAllSurfaces = true, int GetSurfaceAmount = 1)
        {
            if (GetAllSurfaces)
                GetSurfaceAmount = (int)ArrayCount;

            var surfaces = new List<Surface>();
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                bool IsLower = arrayLevel < ArrayIndexStart;
                bool IsHigher = arrayLevel >= (ArrayIndexStart + GetSurfaceAmount);
                if (!IsLower && !IsHigher)
                {
                    List<byte[]> mips = new List<byte[]>();
                    for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                    {
                        mips.Add(GetImageData(arrayLevel, mipLevel));
                    }

                    surfaces.Add(new Surface() { mipmaps = mips });
                }
            }

            return surfaces;
        }

        public abstract void SetImageData(Bitmap bitmap, int ArrayLevel);

        /// <summary>
        /// The total amount of surfaces for the texture.
        /// </summary>
        public uint ArrayCount
        {
            get { return arrayCount; }
            set { arrayCount = value; }
        }
        private uint arrayCount = 1;

        /// <summary>
        /// The total amount of mipmaps for the texture.
        /// </summary>
        public uint MipCount
        {
            get { return mipCount; }
            set
            {
                if (value == 0)
                    mipCount = 1;
                else if (value > 17)
                    throw new Exception($"Invalid mip map count! Texture: {Text} Value: {value}");
                else
                    mipCount = value;
            }
        }
        private uint mipCount = 1;

        /// <summary>
        /// The width of the image in pixels.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// The height of the image in pixels.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// The depth of the image in pixels. Used for 3D types.
        /// </summary>
        public uint Depth { get; set; }

        /// <summary>
        /// The <see cref="TEX_FORMAT"/> Format of the image. 
        /// </summary>
        public TEX_FORMAT Format { get; set; } = TEX_FORMAT.R8G8B8A8_UNORM;

        /// <summary>
        /// The <see cref="PALETTE_FORMAT"/> Format of the image. 
        /// </summary>
        public PALETTE_FORMAT PaletteFormat { get; set; } = PALETTE_FORMAT.None;

        public RenderableTex RenderableTex { get; set; }

        public abstract TEX_FORMAT[] SupportedFormats { get; }

        public static uint GetBytesPerPixel(TEX_FORMAT Format)
        {
            return FormatTable[Format].BytesPerPixel;
        }

        public static uint GetBlockHeight(TEX_FORMAT Format)
        {
            return FormatTable[Format].BlockHeight;
        }

        public static uint GetBlockWidth(TEX_FORMAT Format)
        {
            return FormatTable[Format].BlockWidth;
        }

        public static uint GetBlockDepth(TEX_FORMAT Format)
        {
            return FormatTable[Format].BlockDepth;
        }

        // Based on Ryujinx's image table 
        // https://github.com/Ryujinx/Ryujinx/blob/c86aacde76b5f8e503e2b412385c8491ecc86b3b/Ryujinx.Graphics/Graphics3d/Texture/ImageUtils.cs
        // A nice way to get bpp, block data, and buffer types for formats

        private static readonly Dictionary<TEX_FORMAT, FormatInfo> FormatTable =
                         new Dictionary<TEX_FORMAT, FormatInfo>()
        {
            { TEX_FORMAT.R32G32B32A32_FLOAT,   new FormatInfo(16, 1,  1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.R32G32B32A32_SINT,    new FormatInfo(16, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32G32B32A32_UINT,    new FormatInfo(16, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32G32B32_FLOAT,      new FormatInfo(8, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16B16A16_FLOAT,   new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16B16A16_SINT,    new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16B16A16_SNORM,   new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32G32_FLOAT,         new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32G32_SINT,          new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32G32_UINT,          new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8B8A8_SINT,        new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8B8A8_SNORM,       new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8B8A8_UINT,        new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8B8A8_UNORM,       new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8B8A8_UNORM_SRGB,  new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32G8X24_FLOAT,       new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8_B8G8_UNORM,      new FormatInfo(4, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.B8G8R8X8_UNORM,       new FormatInfo(4, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.B5G5R5A1_UNORM,       new FormatInfo(2, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R5G5B5A1_UNORM,       new FormatInfo(2, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.B8G8R8A8_UNORM,       new FormatInfo(4, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.B8G8R8A8_UNORM_SRGB,  new FormatInfo(4, 1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R5G5B5_UNORM,         new FormatInfo(2, 1,  1, 1,  TargetBuffer.Color) },


            { TEX_FORMAT.R10G10B10A2_UINT,      new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R10G10B10A2_UNORM,     new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32_SINT,              new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32_UINT,              new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32_FLOAT,             new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.B4G4R4A4_UNORM,        new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16_FLOAT,          new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16_SINT,           new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16_SNORM,          new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16_UINT,           new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16G16_UNORM,          new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8_SINT,             new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8_SNORM,            new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8_UINT,             new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8_UNORM,            new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16_SINT,              new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16_SNORM,             new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16_UINT,              new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16_UNORM,             new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8_SINT,               new FormatInfo(1,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8_SNORM,              new FormatInfo(1,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R4G4_UNORM,            new FormatInfo(1,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8_UINT,               new FormatInfo(1,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8_UNORM,              new FormatInfo(1,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.R11G11B10_FLOAT,       new FormatInfo(4,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.B5G6R5_UNORM,          new FormatInfo(2,  1,  1, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC1_UNORM,             new FormatInfo(8,  4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC1_UNORM_SRGB,        new FormatInfo(8,  4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC2_UNORM,             new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC2_UNORM_SRGB,        new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC3_UNORM,             new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC3_UNORM_SRGB,        new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC4_UNORM,             new FormatInfo(8,  4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC4_SNORM,             new FormatInfo(8,  4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC5_UNORM,             new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC5_SNORM,             new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC6H_SF16,             new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC6H_UF16,             new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC7_UNORM,             new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC7_UNORM_SRGB,        new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },

            { TEX_FORMAT.ASTC_4x4_UNORM,        new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_4x4_SRGB,         new FormatInfo(16, 4,  4, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_5x4_UNORM,        new FormatInfo(16, 5,  4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_5x4_SRGB,         new FormatInfo(16, 5,  4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_5x5_UNORM,        new FormatInfo(16, 5,  5, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_5x5_SRGB,         new FormatInfo(16, 5,  5, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_6x5_UNORM,        new FormatInfo(16, 6,  5, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_6x5_SRGB,         new FormatInfo(16, 6,  5, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_6x6_UNORM,        new FormatInfo(16, 6,  6, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_6x6_SRGB,         new FormatInfo(16, 6,  6, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_8x5_UNORM,        new FormatInfo(16, 8,  5,  1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_8x5_SRGB,         new FormatInfo(16, 8,  5, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_8x6_UNORM,        new FormatInfo(16, 8,  6, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_8x6_SRGB,         new FormatInfo(16, 8,  6, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_8x8_UNORM,        new FormatInfo(16, 8,  8, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_8x8_SRGB,         new FormatInfo(16, 8,  8, 1,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x5_UNORM,       new FormatInfo(16, 10, 5, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x5_SRGB,        new FormatInfo(16, 10, 5, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x6_UNORM,       new FormatInfo(16, 10, 6, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x6_SRGB,        new FormatInfo(16, 10, 6, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x8_UNORM,       new FormatInfo(16, 10, 8, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x8_SRGB,        new FormatInfo(16, 10, 8, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x10_UNORM,      new FormatInfo(16, 10, 10, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_10x10_SRGB,       new FormatInfo(16, 10, 10, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_12x10_UNORM,      new FormatInfo(16, 12, 10, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_12x10_SRGB,       new FormatInfo(16, 12, 10, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_12x12_UNORM,      new FormatInfo(16, 12, 12, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC_12x12_SRGB,       new FormatInfo(16, 12, 12, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ETC1_UNORM,            new FormatInfo(4, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ETC1_SRGB,             new FormatInfo(4, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.ETC1_A4,               new FormatInfo(8, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.HIL08,                 new FormatInfo(16, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.L4,                    new FormatInfo(4, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.LA4,                   new FormatInfo(4, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.L8,                    new FormatInfo(8, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.LA8,                   new FormatInfo(16, 1, 1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.A4,                    new FormatInfo(4, 1,  1, 1, TargetBuffer.Color) },
            { TEX_FORMAT.A8_UNORM,              new FormatInfo(8,  1,  1, 1,  TargetBuffer.Color) },

            { TEX_FORMAT.D16_UNORM,            new FormatInfo(2, 1, 1, 1, TargetBuffer.Depth)       },
            { TEX_FORMAT.D24_UNORM_S8_UINT,    new FormatInfo(4, 1, 1, 1, TargetBuffer.Depth)       },
            { TEX_FORMAT.D32_FLOAT,            new FormatInfo(4, 1, 1, 1, TargetBuffer.Depth)       },
            { TEX_FORMAT.D32_FLOAT_S8X24_UINT, new FormatInfo(8, 1, 1, 1, TargetBuffer.DepthStencil)},

            { TEX_FORMAT.I4,                   new FormatInfo(4,  8, 8, 1, TargetBuffer.Color) },
            { TEX_FORMAT.I8,                   new FormatInfo(8,  8, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.IA4,                  new FormatInfo(8,  8, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.IA8,                  new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.RGB565,               new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.RGB5A3,               new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.RGBA32,               new FormatInfo(32, 4, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.C4,                   new FormatInfo(4,  8, 8, 1, TargetBuffer.Color) },
            { TEX_FORMAT.C8,                   new FormatInfo(8,  8, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.C14X2,                new FormatInfo(16, 4, 4, 1, TargetBuffer.Color) },
            { TEX_FORMAT.CMPR,                 new FormatInfo(4,  8, 8, 1, TargetBuffer.Color) }

        };

        /// <summary>
        /// A Surface contains mip levels of compressed/uncompressed texture data
        /// </summary>
        public class Surface
        {
            public List<byte[]> mipmaps = new List<byte[]>();
        }

        public void CreateGenericTexture(uint width, uint height, List<Surface> surfaces, TEX_FORMAT format)
        {
            Width = width;
            Height = height;
            Format = format;
        }
        private enum TargetBuffer
        {
            Color = 1,
            Depth = 2,
            Stencil = 3,
            DepthStencil = 4,
        }

        public void DisposeRenderable()
        {
            if (RenderableTex != null && Runtime.UseOpenGL)
            {
                RenderableTex.Dispose();
                RenderableTex = null;
            }
        }

        private class FormatInfo
        {
            public uint BytesPerPixel { get; private set; }
            public uint BlockWidth { get; private set; }
            public uint BlockHeight { get; private set; }
            public uint BlockDepth { get; private set; }

            public TargetBuffer TargetBuffer;

            public FormatInfo(uint bytesPerPixel, uint blockWidth, uint blockHeight, uint blockDepth, TargetBuffer targetBuffer)
            {
                BytesPerPixel = bytesPerPixel;
                BlockWidth = blockWidth;
                BlockHeight = blockHeight;
                BlockDepth = blockDepth;
                TargetBuffer = targetBuffer;
            }
        }

        public Bitmap GetBitmap3D(int ArrayLevel = 0, int MipLevel = 0)
        {
            List<Bitmap> images = new List<Bitmap>();
            for (int i = 0; i < Depth; i++)
                images.Add(GetBitmap(ArrayLevel, MipLevel, i));

            //Combine images in a horizontal pattern
            uint width = Width * Depth;
            var newBitmap = new Bitmap((int)width, (int)Height);
            using (Graphics grfx = Graphics.FromImage(newBitmap))
            {
                int x = 0;
                foreach (var image in images) {
                    grfx.DrawImage(image, x, 0);
                    x += image.Width;
                }

                return newBitmap;
            }
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> given an array and mip index.
        /// </summary>
        /// <param name="ArrayIndex">The index of the surface/array. Cubemaps will have 6</param>
        /// <param name="MipLevel">The index of the mip level.</param>
        /// <returns></returns>
        public Bitmap GetBitmap(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            uint width = Math.Max(1, Width >> MipLevel);
            uint height = Math.Max(1, Height >> MipLevel);
            byte[] data = GetImageData(ArrayLevel, MipLevel, DepthLevel);
            byte[] paletteData = GetPaletteData();
            if (Format == TEX_FORMAT.R8G8B8A8_UNORM && PlatformSwizzle == PlatformSwizzle.None) 
                return BitmapExtension.GetBitmap(ConvertBgraToRgba(data), (int)width, (int)height);

            try
            {
                if (data == null)
                    throw new Exception("Data is null!");

                if (PlatformSwizzle == PlatformSwizzle.Platform_3DS)
                {
                    var Image = BitmapExtension.GetBitmap(ConvertBgraToRgba(CTR_3DS.DecodeBlock(data, (int)width, (int)height, Format)),
                      (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    return Image;
                }

                if (PlatformSwizzle == PlatformSwizzle.Platform_Gamecube)
                {
                    return BitmapExtension.GetBitmap(Decode_Gamecube.DecodeData(data, paletteData, width, height, Format, PaletteFormat),
                          (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }

                switch (Format)
                {
                    case TEX_FORMAT.R4G4_UNORM:
                        return BitmapExtension.GetBitmap(R4G4.Decompress(data, (int)width, (int)height, false),
                        (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    case TEX_FORMAT.BC5_SNORM:
                        return DDSCompressor.DecompressBC5(data, (int)width, (int)height, true);
                    case TEX_FORMAT.ETC1_UNORM:
                        return BitmapExtension.GetBitmap(ETC1.ETC1Decompress(data, (int)width, (int)height, false),
                               (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    case TEX_FORMAT.ETC1_A4:
                        return BitmapExtension.GetBitmap(ETC1.ETC1Decompress(data, (int)width, (int)height, true),
                              (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    case TEX_FORMAT.R5G5B5A1_UNORM:
                    case TEX_FORMAT.LA8:
                    case TEX_FORMAT.L8:
                        return BitmapExtension.GetBitmap(RGBAPixelDecoder.Decode(data, (int)width, (int)height, Format),
                              (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }

                if (Runtime.UseDirectXTexDecoder)
                {
                    return BitmapExtension.GetBitmap(DecodeBlock(data, width, height, Format, new byte[0], Parameters),
                      (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }
                else
                    return DecodeNotDirectXTex(data, width, height, Format);

            }
            catch (Exception ex)
            {
         /*       Forms.STErrorDialog.Show($"Texture failed to load!", "Texture [GetBitmap({MipLevel},{ArrayLevel})]", DebugInfo() + " \n" + ex);

                try
                {
                    return DecodeNotDirectXTex(data, width, height, Format);
                }
                catch
                {
                    Forms.STErrorDialog.Show($"Texture failed to load!", "Texture [GetBitmap({MipLevel},{ArrayLevel})]", DebugInfo() + " \n" + ex);
                }*/

                return null;
            }
        }

        private Bitmap DecodeNotDirectXTex(byte[] data, uint Width, uint Height, TEX_FORMAT Format)
        {
            if (Format == TEX_FORMAT.R8G8B8A8_UNORM)
                return BitmapExtension.GetBitmap(ConvertBgraToRgba(data), (int)Width, (int)Height);
            else if (Format == TEX_FORMAT.R8G8B8A8_UNORM_SRGB)
                return BitmapExtension.GetBitmap(ConvertBgraToRgba(data), (int)Width, (int)Height);
            else if (Format == TEX_FORMAT.BC1_UNORM)
                return DDSCompressor.DecompressBC1(data, (int)Width, (int)Height, false);
            else if (Format == TEX_FORMAT.BC1_UNORM_SRGB)
                return DDSCompressor.DecompressBC1(data, (int)Width, (int)Height, true);
            else if (Format == TEX_FORMAT.BC3_UNORM_SRGB)
                return DDSCompressor.DecompressBC3(data, (int)Width, (int)Height, false);
            else if (Format == TEX_FORMAT.BC3_UNORM)
                return DDSCompressor.DecompressBC3(data, (int)Width, (int)Height, true);
            else if (Format == TEX_FORMAT.BC4_UNORM)
                return DDSCompressor.DecompressBC4(data, (int)Width, (int)Height, false);
            else if (Format == TEX_FORMAT.BC4_SNORM)
                return DDSCompressor.DecompressBC4(data, (int)Width, (int)Height, true);
            else if (Format == TEX_FORMAT.BC5_UNORM)
                return DDSCompressor.DecompressBC5(data, (int)Width, (int)Height, false);
            else if (Format == TEX_FORMAT.BC7_UNORM)
                return BitmapExtension.GetBitmap(CSharpImageLibrary.DDS.Dxt.DecompressBc7(data, (int)Width, (int)Height), (int)Width, (int)Height);
            else if (Format == TEX_FORMAT.BC7_UNORM_SRGB)
                return BitmapExtension.GetBitmap(CSharpImageLibrary.DDS.Dxt.DecompressBc7(data, (int)Width, (int)Height), (int)Width, (int)Height);
            else
            {

                if (Runtime.UseOpenGL)
                {
                    Runtime.OpenTKInitialized = true;
                    if (RenderableTex == null || !RenderableTex.GLInitialized)
                        LoadOpenGLTexture();

                    return RenderableTex.ToBitmap();
                }
            }
            return null;
        }

        public static Bitmap DecodeBlockGetBitmap(byte[] data, uint Width, uint Height, TEX_FORMAT Format, byte[] paletteData, PALETTE_FORMAT PaletteFormat = PALETTE_FORMAT.None)
        {
            Bitmap bitmap = BitmapExtension.GetBitmap(DecodeBlock(data, Width, Height, Format, paletteData, new ImageParameters(), PaletteFormat),
               (int)Width, (int)Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return bitmap;
        }

        /// <summary>
        /// Decodes a byte array of image data given the source image in bytes, width, height, and DXGI format.
        /// </summary>
        /// <param name="byte[]">The byte array of the image</param>
        /// <param name="Width">The width of the image in pixels.</param>
        /// <param name="Height">The height of the image in pixels.</param>
        /// <param name=" DDS.DXGI_FORMAT">The image format.</param>
        /// <returns>Returns a byte array of decoded data. </returns>
        public static byte[] DecodeBlock(byte[] data, uint Width, uint Height, TEX_FORMAT Format, byte[] paletteData, ImageParameters parameters, PALETTE_FORMAT PaletteFormat = PALETTE_FORMAT.None, PlatformSwizzle PlatformSwizzle = PlatformSwizzle.None)
        {
            if (data == null) throw new Exception($"Data is null!");
            if (Format <= 0) throw new Exception($"Invalid Format!");
            if (data.Length <= 0) throw new Exception($"Data is empty!");
            if (Width <= 0) throw new Exception($"Invalid width size {Width}!");
            if (Height <= 0) throw new Exception($"Invalid height size {Height}!");

            byte[] imageData = new byte[0];
            bool DontSwapRG = false;

            if (PlatformSwizzle == PlatformSwizzle.Platform_3DS)
            {
                imageData = CTR_3DS.DecodeBlock(data, (int)Width, (int)Height, Format);
                DontSwapRG = true;
            }
            else if (PlatformSwizzle == PlatformSwizzle.Platform_Gamecube)
                imageData = Decode_Gamecube.DecodeData(data, paletteData, Width, Height, Format, PaletteFormat);
            else
            {
                if (Format == TEX_FORMAT.R32G8X24_FLOAT)
                    imageData = DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, DDS.DXGI_FORMAT.DXGI_FORMAT_R32G8X24_TYPELESS);

                if (Format == TEX_FORMAT.BC5_SNORM)
                    imageData = DDSCompressor.DecompressBC5(data, (int)Width, (int)Height, true, true);

                if (Format == TEX_FORMAT.L8)
                    return RGBAPixelDecoder.Decode(data, (int)Width, (int)Height, Format);
                if (Format == TEX_FORMAT.LA8)
                    return RGBAPixelDecoder.Decode(data, (int)Width, (int)Height, Format);
                if (Format == TEX_FORMAT.R5G5B5A1_UNORM)
                    return RGBAPixelDecoder.Decode(data, (int)Width, (int)Height, Format);

                if (IsCompressed(Format))
                    imageData = DDSCompressor.DecompressBlock(data, (int)Width, (int)Height, (DDS.DXGI_FORMAT)Format);
                else
                {
                    if (IsAtscFormat(Format))
                        imageData = ASTCDecoder.DecodeToRGBA8888(data, (int)GetBlockWidth(Format), (int)GetBlockHeight(Format), 1, (int)Width, (int)Height, 1);
                    else
                        imageData = DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, (DDS.DXGI_FORMAT)Format);

                    //    imageData = RGBAPixelDecoder.Decode(data, (int)Width, (int)Height, Format);
                }
            }

            if (parameters.DontSwapRG || DontSwapRG)
                return imageData;
            else
                return ConvertBgraToRgba(imageData);
        }

        public string DebugInfo()
        {
            return $"Texture Info:\n" +
                   $"Name:               {Text}\n" +
                   $"Format:             {Format}\n" +
                   $"Height:             {Height}\n" +
                   $"Width:              {Width}\n" +
                   $"Block Height:       {GetBlockHeight(Format)}\n" +
                   $"Block Width:        {GetBlockWidth(Format)}\n" +
                   $"Bytes Per Pixel:    {GetBytesPerPixel(Format)}\n" +
                   $"Array Count:        {ArrayCount}\n" +
                   $"Mip Map Count:      {MipCount}\n" +
                    "";
        }

        public uint GenerateMipCount(int Width, int Height)
        {
            return GenerateMipCount((uint)Width, (uint)Height);
        }

        public uint GenerateMipCount(uint Width, uint Height)
        {
            uint MipmapNum = 0;
            uint num = Math.Max(Width, Height);

            int width = (int)Width;
            int height = (int)Height;

            while (true)
            {
                num >>= 1;

                width = width / 2;
                height = height / 2;
                if (width <= 0 || height <= 0)
                    break;

                if (num > 0)
                    ++MipmapNum;
                else
                    break;
            }

            return MipmapNum;
        }

        public static byte[] GenerateMipsAndCompress(Bitmap bitmap, uint MipCount, TEX_FORMAT Format, bool multiThread = false, float alphaRef = 0.5f, STCompressionMode CompressionMode = STCompressionMode.Fast)
        {
            byte[] DecompressedData = BitmapExtension.ImageToByte(bitmap);
            DecompressedData = ConvertBgraToRgba(DecompressedData);

            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData, bitmap.Width, bitmap.Height);

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int width = Math.Max(1, bitmap.Width >> mipLevel);
                int height = Math.Max(1, bitmap.Height >> mipLevel);

                Image = BitmapExtension.Resize(Image, width, height);
                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, Format, alphaRef, multiThread, CompressionMode));
            }
            Image.Dispose();

            return Utils.CombineByteArray(mipmaps.ToArray());
        }

        public static byte[] CompressBlock(byte[] data, int width, int height, TEX_FORMAT format, float alphaRef, bool multiThread = false, STCompressionMode CompressionMode = STCompressionMode.Fast)
        {
            if (!Runtime.UseDirectXTexDecoder)
                return data;

            //Channels will be swapped
            if (format == TEX_FORMAT.R5G5B5A1_UNORM) {
                format = TEX_FORMAT.B5G5R5A1_UNORM;
                data = ConvertBgraToRgba(data);
            }

            if (IsCompressed(format))
                return DDSCompressor.CompressBlock(data, width, height, (DDS.DXGI_FORMAT)format, multiThread, alphaRef, CompressionMode);
            else if (IsAtscFormat(format))
                return null;
            else
                return DDSCompressor.EncodePixelBlock(data, width, height, (DDS.DXGI_FORMAT)format);
        }
        public void LoadDDS(string path)
        {
            Text = SetNameFromPath(path);

            DDS dds = new DDS();
            LoadDDS(path);

            Width = dds.header.width;
            Height = dds.header.height;
            Format = dds.GetFormat();

            MipCount = dds.header.mipmapCount;
        }
        public void LoadTGA(string path)
        {
            Text = SetNameFromPath(path);
            Bitmap tga = Paloma.TargaImage.LoadTargaImage(path);
        }
        public void LoadBitmap(string path)
        {
            Text = SetNameFromPath(path);

        }
        public void LoadASTC(string path)
        {
            ASTC astc = new ASTC();
            astc.Load(new FileStream(path, FileMode.Open));
        }

        public override string ExportFilter
        {
            get
            {
                if (IsAtscFormat(Format))
                {
                    return "Supported Formats|*.dds; *.astc; *.png;*.tga;*.jpg;*.tiff|" +
                                "ASTC |*.astc|" +
                                "Portable Network Graphics |*.png|" +
                                "Joint Photographic Experts Group |*.jpg|" +
                                "Bitmap Image |*.bmp|" +
                                "Tagged Image File Format |*.tiff|" +
                                "All files(*.*)|*.*";
                }
                else
                {
                    return "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                                 "Microsoft DDS |*.dds|" +
                                 "Portable Network Graphics |*.png|" +
                                 "Joint Photographic Experts Group |*.jpg|" +
                                 "Bitmap Image |*.bmp|" +
                                 "Tagged Image File Format |*.tiff|" +
                                 "All files(*.*)|*.*";
                }
            }
        }

        public override void Export(string FileName)
        {
            Export(FileName, false, false, GetViewedArrayLevel(), GetViewedMipLevel());
        }

        public void ExportArrayImage(int ArrayIndex = 0)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Text;
            sfd.DefaultExt = "dds";
            sfd.Filter = ExportFilter;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export(sfd.FileName, true, false, ArrayIndex, 0);
            }
        }

        private int GetViewedArrayLevel()
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor != null)
               return editor.GetArrayDisplayLevel();
            return 0;
        }

        private int GetViewedMipLevel()
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor != null)
                return editor.GetMipmapDisplayLevel();
            return 0;
        }

        public void ExportImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Text;
            sfd.DefaultExt = "dds";
            sfd.Filter = ExportFilter;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export(sfd.FileName, false, false, GetViewedArrayLevel(), GetViewedMipLevel());
            }
        }

        public void Export(string FileName, bool ExportSurfaceLevel = false,
            bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {
            string ext = Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".dds":
                    SaveDDS(FileName, ExportSurfaceLevel, ExportMipMapLevel, SurfaceLevel, MipLevel);
                    break;
                case ".astc":
                    SaveASTC(FileName, ExportSurfaceLevel, ExportMipMapLevel, SurfaceLevel, MipLevel);
                    break;
                default:
                    SaveBitMap(FileName, ExportSurfaceLevel, ExportMipMapLevel, SurfaceLevel, MipLevel);
                    break;
            }
        }
        public void SaveASTC(string FileName, bool ExportSurfaceLevel = true,
            bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {
            List<Surface> surfaces = GetSurfaces(SurfaceLevel, false);


            ASTC atsc = new ASTC();
            atsc.Width = Width;
            atsc.Height = Height;
            atsc.Depth = Depth;
            atsc.BlockDimX = (byte)GetBlockWidth(Format);
            atsc.BlockDimY = (byte)GetBlockHeight(Format);
            atsc.BlockDimZ = (byte)GetBlockDepth(Format);
            atsc.DataBlock = Utils.CombineByteArray(surfaces[0].mipmaps.ToArray());

            Console.WriteLine("DataBlock " + atsc.DataBlock.Length);

            using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite))
            {
                atsc.Save(fs);
            }
        }
        public void SaveTGA(string FileName, bool ExportSurfaceLevel = false,
            bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {

        }
        public void SaveBitMap(string FileName, bool ExportSurfaceLevel = false,
            bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {
            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Exporting Image Data...";
            progressBar.Value = 0;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            if (ArrayCount > 1 && !ExportSurfaceLevel)
            {
                string ext = Path.GetExtension(FileName);

                int index = FileName.LastIndexOf('.');
                string name = index == -1 ? FileName : FileName.Substring(0, index);

                for (int i = 0; i < ArrayCount; i++)
                {
                    progressBar.Task = $"Decoding Surface [{i}] for image {Text}... ";
                    progressBar.Value = (i * 100) / (int)ArrayCount;
                    progressBar.Refresh();

                    Bitmap arrayBitMap = GetBitmap(i, 0);
                    arrayBitMap.Save($"{name}_Slice_{i}_{ext}");
                    arrayBitMap.Dispose();
                }

                progressBar.Value = 100;
                progressBar.Close();
                return;
            }

            progressBar.Task = $"Decoding image {Text}... ";
            progressBar.Value = 20;
            progressBar.Refresh();

            Bitmap bitMap = GetBitmap(SurfaceLevel, MipLevel);
            if (Runtime.ImageEditor.UseComponetSelector)
                bitMap = BitmapExtension.SetChannel(bitMap, RedChannel, GreenChannel, BlueChannel, AlphaChannel);
            if (Runtime.ImageEditor.PreviewGammaFix)
                bitMap = BitmapExtension.AdjustGamma(bitMap, 1.0f / 2.2f);

            if (Parameters.FlipY)
                bitMap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            bitMap.Save(FileName);
            bitMap.Dispose();

            progressBar.Value = 100;
            progressBar.Close();
        }
        public void SaveDDS(string FileName, bool ExportSurfaceLevel = false,
            bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {
            List<Surface> surfaces = null;
            if (ExportSurfaceLevel)
                surfaces = GetSurfaces(SurfaceLevel, false, 1);
            else if (Depth > 1)
                surfaces = Get3DSurfaces();
            else
                surfaces = GetSurfaces();

            if (Depth == 0)
                Depth = 1;

            DDS dds = new DDS();
            dds.header = new DDS.Header();
            dds.header.width = Width;
            dds.header.height = Height;
            dds.header.depth = Depth;
            dds.header.mipmapCount = (uint)MipCount;
            dds.header.pitchOrLinearSize = (uint)surfaces[0].mipmaps[0].Length;

            //Check for components to be different. Then set our channel flags
            if (RedChannel != STChannelType.Red || GreenChannel != STChannelType.Green || 
               BlueChannel != STChannelType.Blue || AlphaChannel != STChannelType.Alpha)
            {
                //R G B A 1 0
                uint[] components = new uint[6] {
                    0x000000ff, 0x0000ff00,
                    0x00ff0000, 0xff000000, 0x00008000, 0};

            /*    dds.header.ddspf.RGBBitCount = 4;
                dds.header.ddspf.RBitMask = components[(int)RedChannel];
                dds.header.ddspf.GBitMask = components[(int)GreenChannel];
                dds.header.ddspf.BBitMask = components[(int)BlueChannel];
                dds.header.ddspf.ABitMask = components[(int)AlphaChannel];*/
            }

         /*  if (Runtime.ImageEditor.PreviewGammaFix)
            {
                foreach (var surface in surfaces)
                {
                    Bitmap bitMap = GetBitmap(surfaces.IndexOf(surface), 0);
                    bitMap = BitmapExtension.AdjustGamma(bitMap, 1.0f / 2.2f);
                    if (Runtime.ImageEditor.UseComponetSelector)
                        bitMap = BitmapExtension.SetChannel(bitMap, RedChannel, GreenChannel, BlueChannel, AlphaChannel);

                    var reEncoded = GenerateMipsAndCompress(bitMap, MipCount, Format);
                    //surface.mipmaps = reEncoded;
                }
            }*/

            bool isCubeMap = ArrayCount == 6;

            if (surfaces.Count > 1) //Use DX10 format for array surfaces as it can do custom amounts
                dds.SetFlags((DDS.DXGI_FORMAT)Format, true, isCubeMap);
            else
                dds.SetFlags((DDS.DXGI_FORMAT)Format, false, isCubeMap);

            if (dds.IsDX10)
            {
                if (dds.DX10header == null)
                    dds.DX10header = new DDS.DX10Header();

                dds.DX10header.ResourceDim = 3;

                if (isCubeMap)
                    dds.DX10header.arrayFlag = (uint)(ArrayCount / 6);
                else
                    dds.DX10header.arrayFlag = (uint)ArrayCount;
            }


            dds.Save(dds, FileName, surfaces);
        }
        public void LoadOpenGLTexture()
        {
            if (!Runtime.UseOpenGL)
                return;

            if (RenderableTex == null)
                RenderableTex = new RenderableTex();

            RenderableTex.GLInitialized = false;
            RenderableTex.LoadOpenGLTexture(this);
        }
        public static bool IsAtscFormat(TEX_FORMAT Format)
        {
            if (Format.ToString().Contains("ASTC"))
                return true;
            else
                return false;
        }

        public static bool IsCompressed(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.BC1_UNORM:
                case TEX_FORMAT.BC1_UNORM_SRGB:
                case TEX_FORMAT.BC1_TYPELESS:
                case TEX_FORMAT.BC2_UNORM_SRGB:
                case TEX_FORMAT.BC2_UNORM:
                case TEX_FORMAT.BC2_TYPELESS:
                case TEX_FORMAT.BC3_UNORM_SRGB:
                case TEX_FORMAT.BC3_UNORM:
                case TEX_FORMAT.BC3_TYPELESS:
                case TEX_FORMAT.BC4_UNORM:
                case TEX_FORMAT.BC4_TYPELESS:
                case TEX_FORMAT.BC4_SNORM:
                case TEX_FORMAT.BC5_UNORM:
                case TEX_FORMAT.BC5_TYPELESS:
                case TEX_FORMAT.BC5_SNORM:
                case TEX_FORMAT.BC6H_UF16:
                case TEX_FORMAT.BC6H_SF16:
                case TEX_FORMAT.BC7_UNORM:
                case TEX_FORMAT.BC7_UNORM_SRGB:
                    return true;
                default:
                    return false;
            }
        }
        public static STChannelType[] SetChannelsByFormat(TEX_FORMAT Format)
        {
            STChannelType[] channels = new STChannelType[4];

            switch (Format)
            {
                case TEX_FORMAT.BC5_UNORM:
                case TEX_FORMAT.BC5_SNORM:
                    channels[0] = STChannelType.Red;
                    channels[1] = STChannelType.Green;
                    channels[2] = STChannelType.One;
                    channels[3] = STChannelType.One;
                    break;
                case TEX_FORMAT.BC4_UNORM:
                case TEX_FORMAT.BC4_SNORM:
                    channels[0] = STChannelType.Red;
                    channels[1] = STChannelType.Red;
                    channels[2] = STChannelType.Red;
                    channels[3] = STChannelType.Red;
                    break;
                default:
                    channels[0] = STChannelType.Red;
                    channels[1] = STChannelType.Green;
                    channels[2] = STChannelType.Blue;
                    channels[3] = STChannelType.Alpha;
                    break;
            }
            return channels;
        }

        public static int GenerateTotalMipCount(uint Width, uint Height)
        {
            int mipCount = 1;

            int width = (int)Width;
            int height = (int)Height;

            while (width > 1 || height > 1)
            {
                ++mipCount;

                if (width > 1)
                    width /= 2;

                if (height > 1)
                    height /= 2;
            }

            return mipCount;
        }

        public static string SetNameFromPath(string path)
        {
            string FileName = Path.GetFileName(path);
            string extension = System.IO.Path.GetExtension(FileName);
            return FileName.Substring(0, FileName.Length - extension.Length);
        }

        public Bitmap GetComponentBitmap(Bitmap image, bool ShowAlpha = true)
        {
            //Check if it's RGBA to save time editing the image
            if (UseRGBA())
                return image;
            else if (ShowAlpha)
                return BitmapExtension.SetChannel(image, RedChannel, GreenChannel, BlueChannel, AlphaChannel);
            else
                return BitmapExtension.SetChannel(image, RedChannel, GreenChannel, BlueChannel, STChannelType.One);
        }

        private bool UseRGBA()
        {
            if (RedChannel == STChannelType.Red &&
                GreenChannel == STChannelType.Green &&
                BlueChannel == STChannelType.Blue &&
                AlphaChannel == STChannelType.Alpha)
                return true;
            else
                return false;
        }

        public static byte[] ConvertBgraToRgba(byte[] bytes)
        {
            if (bytes == null)
                throw new Exception("Data block returned null. Make sure the parameters and image properties are correct!");

            for (int i = 0; i < bytes.Length; i += 4)
            {
                var temp = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = temp;
            }
            return bytes;
        }


        private static byte[] ConvertBgraToRgba(byte[] bytes, string Format, int bpp, int width, int height, byte[] compSel)
        {
            if (bytes == null)
                throw new Exception("Data block returned null. Make sure the parameters and image properties are correct!");

            int size = width * height * 4;
            byte[] NewImageData = new byte[size];

            byte[] comp = new byte[6] { 0, 0xFF, 0, 0, 0, 0xFF };

            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    var pos = (y * width + x) * bpp;
                    var pos_ = (y * width + x) * 4;

                    int pixel = 0;
                    for (int i = 0; i < bpp; i += 1)
                        pixel |= bytes[pos + i] << (8 * i);

                    comp = GetComponentsFromPixel(Format, pixel, comp);

                    NewImageData[pos_ + 3] = comp[compSel[3]];
                    NewImageData[pos_ + 2] = comp[compSel[2]];
                    NewImageData[pos_ + 1] = comp[compSel[1]];
                    NewImageData[pos_ + 0] = comp[compSel[0]];
                }
            }
            return NewImageData;
        }

        private static byte[] GetComponentsFromPixel(string Format, int pixel, byte[] comp)
        {
            switch (Format)
            {
                case "RGBX8":
                    comp[2] = (byte)(pixel & 0xFF);
                    comp[3] = (byte)((pixel & 0xFF00) >> 8);
                    comp[4] = (byte)((pixel & 0xFF0000) >> 16);
                    comp[5] = (byte)((pixel & 0xFF000000) >> 24);
                    break;
                case "RGBA8":
                    comp[2] = (byte)(pixel & 0xFF);
                    comp[3] = (byte)((pixel & 0xFF00) >> 8);
                    comp[4] = (byte)((pixel & 0xFF0000) >> 16);
                    comp[5] = (byte)((pixel & 0xFF000000) >> 24);
                    break;
                case "RGBA4":
                    comp[2] = (byte)((pixel & 0xF) * 17);
                    comp[3] = (byte)(((pixel & 0xF0) >> 4) * 17);
                    comp[4] = (byte)(((pixel & 0xF00) >> 8) * 17);
                    comp[5] = (byte)(((pixel & 0xF000) >> 12) * 17);
                    break;
                case "RGBA5":
                    comp[2] = (byte)(((pixel & 0xF800) >> 11) / 0x1F * 0xFF);
                    comp[3] = (byte)(((pixel & 0x7E0) >> 5) / 0x3F * 0xFF);
                    comp[4] = (byte)((pixel & 0x1F) / 0x1F * 0xFF);
                    break;
            }

            return comp;
        }

        public Properties GenericProperties
        {
            get
            {
                Properties prop = new Properties();
                prop.Height = Height;
                prop.Width = Width;
                prop.Format = Format;
                prop.Depth = Depth;
                prop.MipCount = MipCount;
                prop.ArrayCount = ArrayCount;
                prop.ImageSize = (uint)GetImageData().Length;

                return prop;
            }
        }

        public class Properties
        {
            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Height of the image.")]
            [Category("Image Info")]
            public uint Height { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Width of the image.")]
            [Category("Image Info")]
            public uint Width { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Depth of the image (3D type).")]
            [Category("Image Info")]
            public uint Depth { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Format of the image.")]
            [Category("Image Info")]
            public TEX_FORMAT Format { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Mip map count of the image.")]
            [Category("Image Info")]
            public uint MipCount { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Array count of the image for multiple surfaces.")]
            [Category("Image Info")]
            public uint ArrayCount { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("The image size in bytes.")]
            [Category("Image Info")]
            public uint ImageSize { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("The swizzle value.")]
            [Category("Image Info")]
            public uint Swizzle { get; set; }
        }
    }
}
