using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bntx;
using Syroot.NintenTools.NSW.Bntx.GFX;
using System.Runtime.InteropServices;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class TextureImporterSettings
    {
        public TextureImporterSettings()
        {
        }

        public bool CombineMipLevel = true;
        public int Alignment = 512;
        public bool GammaFix = false;
        public string TexName;
        public AccessFlags AccessFlags = AccessFlags.Texture;
        public uint MipCount;
        public uint Depth = 1;
        public uint TexWidth;
        public uint TexHeight;
        public uint Flags;
        public uint Swizzle;
        public uint SampleCount = 1;
        public uint Pitch = 32;
        public uint[] Regs;

        public TEX_FORMAT DefaultFormat = TEX_FORMAT.BC1_UNORM_SRGB;

        public SurfaceFormat Format;

        public SurfaceDim SurfaceDim = SurfaceDim.Dim2D;
        public TileMode TileMode = TileMode.Default;
        public Dim Dim = Dim.Dim2D;
        public STChannelType RedComp = STChannelType.Red;
        public STChannelType GreenComp = STChannelType.Green;
        public STChannelType BlueComp = STChannelType.Blue;
        public STChannelType AlphaComp = STChannelType.Alpha;
        public TextureData textureData;
        public uint TextureLayout;
        public uint TextureLayout2 = 0x010007;
        public uint bpp;
        public List<byte[]> DataBlockOutput = new List<byte[]>();
        public List<byte[]> DecompressedData = new List<byte[]>();
        public int sparseResidency = 0; //false
        public int sparseBinding = 0; //false
        public bool IsSRGB = true;
        public bool GenerateMipmaps = false; //If bitmap and count more that 1 then generate
        public float alphaRef = 0.5f;

        public bool IsFinishedCompressing = false;

        public void LoadDDS(string FileName, byte[] FileData = null, TextureData tree = null)
        {
            TexName = STGenericTexture.SetNameFromPath(FileName);

            DDS dds = new DDS();

            if (FileData != null)
                dds.Load(new FileReader(new MemoryStream(FileData)));
            else
                dds.Load(new FileReader(FileName));
            MipCount = dds.header.mipmapCount;
            TexWidth = dds.header.width;
            TexHeight = dds.header.height;

            var surfaces = DDS.GetArrayFaces(dds, dds.ArrayCount);

            if (dds.IsCubemap)
                SurfaceDim = SurfaceDim.DimCube;

            RedComp = dds.RedChannel;
            GreenComp = dds.GreenChannel;
            BlueComp = dds.BlueChannel;
            AlphaComp = dds.AlphaChannel;

            foreach (var surface in surfaces)
                DataBlockOutput.Add(Utils.CombineByteArray(surface.mipmaps.ToArray()));

            Format = TextureData.GenericToBntxSurfaceFormat(dds.Format);
        }

        public void LoadASTC(string FileName)
        {
            DecompressedData.Clear();
            TexName = STGenericTexture.SetNameFromPath(FileName);

            ASTC astc = new ASTC();
            astc.Load(new FileStream(FileName, FileMode.Open));

            MipCount = 0;

            TexWidth = (uint)astc.Width;
            TexHeight = (uint)astc.Height;

            DataBlockOutput.Add(astc.DataBlock);

            Format = TextureData.GenericToBntxSurfaceFormat(astc.Format);
        }

        public void LoadBitMap(Image Image, string Name)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(Name);

            Format = TextureData.GenericToBntxSurfaceFormat(DefaultFormat);

            GenerateMipmaps = true;
            LoadImage(new Bitmap(Image));
        }

        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);
            Format = TextureData.GenericToBntxSurfaceFormat(DefaultFormat);

            GenerateMipmaps = true;

            //If a texture is .tga, we need to convert it
            Bitmap Image = null;
            if (Utils.GetExtension(FileName) == ".tga")
            {
                Image = Paloma.TargaImage.LoadTargaImage(FileName);
            }
            else
            {
                Image = new Bitmap(FileName);
            }

            LoadImage(Image);
        }

        private void LoadImage(Bitmap Image)
        {
            Image = BitmapExtension.SwapBlueRedChannels(Image);

            TexWidth = (uint)Image.Width;
            TexHeight = (uint)Image.Height;
            MipCount = (uint)STGenericTexture.GenerateTotalMipCount(TexWidth, TexHeight);

            DecompressedData.Add(BitmapExtension.ImageToByte(Image));

            Image.Dispose();
            if (DecompressedData.Count == 0)
            {
                throw new Exception("Failed to load " + Format);
            }
        }

        public List<byte[]> GenerateMipList(STCompressionMode CompressionMode, bool multiThread, bool bc4Alpha, int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);
            if (GammaFix)
                Image = BitmapExtension.AdjustGamma(Image, 2.2f);
            if (bc4Alpha)
                Image = BitmapExtension.SetChannel(Image, STChannelType.Alpha, STChannelType.Alpha, STChannelType.Alpha, STChannelType.Alpha);

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int MipWidth = Math.Max(1, (int)TexWidth >> mipLevel);
                int MipHeight = Math.Max(1, (int)TexHeight >> mipLevel);

                if (mipLevel != 0)
                    Image = BitmapExtension.Resize(Image, MipWidth, MipHeight);

                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, TextureData.ConvertFormat(Format), alphaRef, multiThread, CompressionMode));
            }
            Image.Dispose();

            return mipmaps;
        }

        public byte[] GenerateMips(STCompressionMode CompressionMode, bool multiThread, int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);
            if (GammaFix)
                Image = BitmapExtension.AdjustGamma(Image, 2.2f);

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int MipWidth = Math.Max(1, (int)TexWidth >> mipLevel);
                int MipHeight = Math.Max(1, (int)TexHeight >> mipLevel);

                if (mipLevel != 0)
                    Image = BitmapExtension.Resize(Image, MipWidth, MipHeight);

                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, TextureData.ConvertFormat(Format), alphaRef, multiThread, CompressionMode));
            }
            Image.Dispose();

            return Utils.CombineByteArray(mipmaps.ToArray());
        }
        public void Compress(STCompressionMode CompressionMode, bool multiThread)
        {
            DataBlockOutput.Clear();
            foreach (var surface in DecompressedData)
            {
                DataBlockOutput.Add(STGenericTexture.CompressBlock(surface,
                    (int)TexWidth, (int)TexHeight, TextureData.ConvertFormat(Format), alphaRef, multiThread, CompressionMode));
            }
        }
        public static uint DIV_ROUND_UP(uint value1, uint value2)
        {
            return TegraX1Swizzle.DIV_ROUND_UP(value1, value2);
        }
        private ChannelType ConvertChannel(STChannelType channel)
        {
            switch (channel)
            {
                case STChannelType.Red:
                    return ChannelType.Red;
                case STChannelType.Green:
                    return ChannelType.Green;
                case STChannelType.Blue:
                    return ChannelType.Blue;
                case STChannelType.Alpha:
                    return ChannelType.Alpha;
                case STChannelType.One:
                    return ChannelType.One;
                case STChannelType.Zero:
                    return ChannelType.Zero;
                default:
                    throw new Exception("Unsupported format! " + channel);
            }
        }
        public Texture FromBitMap(List<byte[]> arrayFaces, TextureImporterSettings settings)
        {
            Texture tex = new Texture();
            tex.Height = (uint)settings.TexHeight;
            tex.Width = (uint)settings.TexWidth;
            tex.Format = Format;

            tex.Name = settings.TexName;
            tex.Path = "";
            tex.TextureData = new List<List<byte[]>>();

            if (settings.MipCount == 0)
                settings.MipCount = 1;

            STChannelType[] channels = STGenericTexture.SetChannelsByFormat(TextureData.ConvertFormat(settings.Format));
            tex.ChannelRed = ConvertChannel(channels[0]);
            tex.ChannelGreen = ConvertChannel(channels[1]);
            tex.ChannelBlue = ConvertChannel(channels[2]);
            tex.ChannelAlpha = ConvertChannel(channels[3]);
            tex.sparseBinding = settings.sparseBinding;
            tex.sparseResidency = settings.sparseResidency;
            tex.AccessFlags = settings.AccessFlags;
            tex.MipCount = settings.MipCount;
            tex.Depth = settings.Depth;
            tex.Dim = settings.Dim;
            tex.Flags = (byte)settings.Flags;
            tex.TileMode = settings.TileMode;
            tex.textureLayout = settings.TextureLayout;
            tex.textureLayout2 = settings.TextureLayout2;
            tex.Swizzle = settings.Swizzle;
            tex.SurfaceDim = settings.SurfaceDim;
            tex.SampleCount = settings.SampleCount;
            tex.Regs = settings.Regs;
            tex.Pitch = settings.Pitch;

            tex.MipOffsets = new long[tex.MipCount];

            for (int i = 0; i < arrayFaces.Count; i++)
            {
                List<byte[]> mipmaps = SwizzleSurfaceMipMaps(tex, arrayFaces[i], tex.MipCount, settings.Alignment);
                tex.TextureData.Add(mipmaps);

                //Combine mip map data
                if (settings.Alignment != 1)
                {
                    byte[] combinedMips = Utils.CombineByteArray(mipmaps.ToArray());
                    tex.TextureData[i][0] = combinedMips;
                }
            }

            return tex;
        }
        public static List<byte[]> SwizzleSurfaceMipMaps(Texture tex, byte[] data, uint MipCount, int alignment = 512)
        {
            var TexFormat = TextureData.ConvertFormat(tex.Format);

            int blockHeightShift = 0;
            int target = 1;
            uint Pitch = 0;
            uint SurfaceSize = 0;
            uint blockHeight = 0;
            uint blkWidth = STGenericTexture.GetBlockWidth(TexFormat);
            uint blkHeight = STGenericTexture.GetBlockHeight(TexFormat);
            uint blkDepth = STGenericTexture.GetBlockDepth(TexFormat);

            uint bpp = STGenericTexture.GetBytesPerPixel(TexFormat);

            uint linesPerBlockHeight = 0;

            if (tex.TileMode == TileMode.LinearAligned)
            {
                blockHeight = 1;
                tex.BlockHeightLog2 = 0;
                tex.Alignment = 1;

                linesPerBlockHeight = 1;
                tex.ReadTextureLayout = 0;
            }
            else
            {
                blockHeight = TegraX1Swizzle.GetBlockHeight(DIV_ROUND_UP(tex.Height, blkHeight));
                tex.BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;
                tex.Alignment = alignment;
                tex.ReadTextureLayout = 1;

                linesPerBlockHeight = blockHeight * 8;

            }

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                var result = TextureHelper.GetCurrentMipSize(tex.Width, tex.Height, blkWidth, blkHeight, bpp, mipLevel);
                uint offset = result.Item1;
                uint size = result.Item2;


                byte[] data_ = Utils.SubArray(data, offset, size);

                uint width_ = Math.Max(1, tex.Width >> mipLevel);
                uint height_ = Math.Max(1, tex.Height >> mipLevel);
                uint depth_ = Math.Max(1, tex.Depth >> mipLevel);

                uint width__ = DIV_ROUND_UP(width_, blkWidth);
                uint height__ = DIV_ROUND_UP(height_, blkHeight);
                uint depth__ = DIV_ROUND_UP(depth_, blkDepth);


                byte[] AlignedData = new byte[(TegraX1Swizzle.round_up(SurfaceSize, (uint)tex.Alignment) - SurfaceSize)];
                if (tex.Alignment == 1)
                    AlignedData = new byte[0];

                SurfaceSize += (uint)AlignedData.Length;

              //  Console.WriteLine("SurfaceSize Aligned " + AlignedData);

                Console.WriteLine("MipOffsets " + SurfaceSize);
                Console.WriteLine("size " + size);

                tex.MipOffsets[mipLevel] = SurfaceSize;
                if (tex.TileMode == TileMode.LinearAligned)
                {
                    Pitch = width__ * bpp;

                    if (target == 1)
                        Pitch = TegraX1Swizzle.round_up(width__ * bpp, 32);
                   
                    SurfaceSize += Pitch * height__;
                }
                else
                {
                    if (TegraX1Swizzle.pow2_round_up(height__) < linesPerBlockHeight)
                        blockHeightShift += 1;

                    Pitch = TegraX1Swizzle.round_up(width__ * bpp, 64);
                    SurfaceSize += Pitch * TegraX1Swizzle.round_up(height__, Math.Max(1, blockHeight >> blockHeightShift) * 8);
                }

                byte[] SwizzledData = TegraX1Swizzle.swizzle(width_, height_, depth_, blkWidth, blkHeight, blkDepth, target, bpp, (uint)tex.TileMode, (int)Math.Max(0, tex.BlockHeightLog2 - blockHeightShift), data_, size);
                mipmaps.Add(AlignedData.Concat(SwizzledData).ToArray());

                Console.WriteLine("SwizzledData " + SwizzledData.Length);
            }
            tex.ImageSize = SurfaceSize;

            return mipmaps;
        }

        private byte[] Combine(List<byte[]> arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
