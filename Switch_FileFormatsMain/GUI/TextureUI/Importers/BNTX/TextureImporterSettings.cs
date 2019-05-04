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
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class TextureImporterSettings
    {
        public TextureImporterSettings()
        {
        }

        public string TexName;
        public uint arrayLength = 1;
        public uint AccessFlags = 0x20;
        public uint MipCount;
        public uint Depth = 1;
        public uint TexWidth;
        public uint TexHeight;
        public uint Flags;
        public uint Swizzle;
        public uint SampleCount = 1;
        public uint Pitch = 32;
        public uint[] Regs;
        public SurfaceFormat Format;

        public SurfaceDim SurfaceDim = SurfaceDim.Dim2D;
        public TileMode TileMode = TileMode.Default;
        public Dim Dim = Dim.Dim2D;
        public ChannelType RedComp = ChannelType.Red;
        public ChannelType GreenComp = ChannelType.Green;
        public ChannelType BlueComp = ChannelType.Blue;
        public ChannelType AlphaComp = ChannelType.Alpha;
        public TextureData textureData;
        public uint TextureLayout;
        public uint TextureLayout2 = 0x010007;
        public uint bpp;
        public List<byte[]> DataBlockOutput = new List<byte[]>();
        public List<byte[]> DecompressedData = new List<byte[]>();
        public int sparseResidency = 0; //false
        public int sparseBinding = 0; //false
        public bool IsSRGB = true;
        public bool GenerateMipmaps = false; //If bitmap and count more that 1 then geenrate
        public float alphaRef = 0.5f;

        public void LoadDDS(string FileName, byte[] FileData = null, TextureData tree = null)
        {
            TexName = Path.GetFileNameWithoutExtension(FileName);

            Console.WriteLine(TexName);

            DDS dds = new DDS();

            if (FileData != null)
                dds.Load(new FileReader(new MemoryStream(FileData)));
            else
                dds.Load(new FileReader(FileName));
            MipCount = dds.header.mipmapCount;
            TexWidth = dds.header.width;
            TexHeight = dds.header.height;
            arrayLength = 1;
            if (dds.header.caps2 == (uint)DDS.DDSCAPS2.CUBEMAP_ALLFACES)
            {
                arrayLength = 6;
            }

            foreach (var array in DDS.GetArrayFacesBytes(dds.bdata, (int)arrayLength))
            {
                DataBlockOutput.Add(array);
            }

            Format = TextureData.GenericToBntxSurfaceFormat(dds.Format);
        }

        public void LoadASTC(string FileName)
        {
            DecompressedData.Clear();
            TexName = Path.GetFileNameWithoutExtension(FileName);

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

            TexName = Path.GetFileNameWithoutExtension(Name);
            Format = TextureData.GenericToBntxSurfaceFormat(Runtime.PreferredTexFormat);

            GenerateMipmaps = true;
            LoadImage(new Bitmap(Image));
        }

        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = Path.GetFileNameWithoutExtension(FileName);
            Format = TextureData.GenericToBntxSurfaceFormat(Runtime.PreferredTexFormat);

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
            MipCount = (uint)GetTotalMipCount();

            DecompressedData.Add(BitmapExtension.ImageToByte(Image));

            Image.Dispose();
            if (DecompressedData.Count == 0)
            {
                throw new Exception("Failed to load " + Format);
            }

        }

        public int GetTotalMipCount()
        {
            int MipmapNum = 0;
            uint num = Math.Max(TexHeight, TexWidth);

            int width = (int)TexWidth;
            int height = (int)TexHeight;

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
        public byte[] GenerateMips(int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);

            List<byte[]> mipmaps = new List<byte[]>();
            mipmaps.Add(STGenericTexture.CompressBlock(DecompressedData[SurfaceLevel],
                (int)TexWidth, (int)TexHeight, TextureData.ConvertFormat(Format), alphaRef));

            //while (Image.Width / 2 > 0 && Image.Height / 2 > 0)
            //      for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int width = Image.Width / 2;
                int height = Image.Height / 2;
                if (width <= 0)
                    width = 1;
                if (height <= 0)
                    height = 1;

                Image = BitmapExtension.Resize(Image, width, height);
                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, TextureData.ConvertFormat(Format), alphaRef));
            }
            Image.Dispose();

            return Utils.CombineByteArray(mipmaps.ToArray());
        }
        public void Compress()
        {
            DataBlockOutput.Clear();
            foreach (var surface in DecompressedData)
            {
                DataBlockOutput.Add(STGenericTexture.CompressBlock(surface,
                    (int)TexWidth, (int)TexHeight, TextureData.ConvertFormat(Format), alphaRef));
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
            tex.ArrayLength = settings.arrayLength;
            tex.MipCount = settings.MipCount;
            tex.Depth = settings.Depth;
            tex.Dim = settings.Dim;
            tex.Flags = settings.Flags;
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
                List<byte[]> mipmaps = SwizzleSurfaceMipMaps(tex, arrayFaces[i], tex.MipCount);
                tex.TextureData.Add(mipmaps);

                //Combine mip map data
                byte[] combinedMips = Utils.CombineByteArray(mipmaps.ToArray());
                tex.TextureData[i][0] = combinedMips;
            }

            return tex;
        }
        public static List<byte[]> SwizzleSurfaceMipMaps(Texture tex, byte[] data, uint MipCount)
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
                tex.Alignment = 512;
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

                byte[] SwizzledData = TegraX1Swizzle.swizzle(width_, height_, depth_, blkWidth, blkHeight, blkDepth, target, bpp, (uint)tex.TileMode, (int)Math.Max(0, tex.BlockHeightLog2 - blockHeightShift), data_);
                mipmaps.Add(AlignedData.Concat(SwizzledData).ToArray());
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
