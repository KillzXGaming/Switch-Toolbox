using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Syroot.NintenTools.Bfres.GX2;

namespace FirstPlugin
{
    public class GTXImporterSettings
    {
        public string TexName;
        public uint TexWidth;
        public uint TexHeight;
        public uint MipCount;
        public uint Depth = 1;
        public uint arrayLength = 1;
        public List<byte[]> DataBlockOutput = new List<byte[]>();
        public List<byte[]> DecompressedData = new List<byte[]>();
        public GTX.GX2SurfaceFormat Format;
        public bool GenerateMipmaps;
        public bool IsSRGB;
        public uint tileMode = 4;
        public uint swizzle = 4;
        public GX2CompSel[] compSel = new GX2CompSel[4];
        public GX2SurfaceDim SurfaceDim = GX2SurfaceDim.Dim2D;
        public GX2AAMode AAMode = GX2AAMode.Mode1X;

        private GTX.GX2SurfaceFormat LoadDDSFormat(uint fourCC, DDS dds = null, bool IsSRGB = false)
        {
            bool IsDX10 = false;

            switch (fourCC)
            {
                case DDS.FOURCC_DXT1:
                    if (IsSRGB)
                        return GTX.GX2SurfaceFormat.T_BC1_SRGB;
                    else
                        return GTX.GX2SurfaceFormat.T_BC1_UNORM;
                case DDS.FOURCC_DXT3:
                    if (IsSRGB)
                        return GTX.GX2SurfaceFormat.T_BC2_SRGB;
                    else
                        return GTX.GX2SurfaceFormat.T_BC2_UNORM;
                case DDS.FOURCC_DXT5:
                    if (IsSRGB)
                        return GTX.GX2SurfaceFormat.T_BC3_SRGB;
                    else
                        return GTX.GX2SurfaceFormat.T_BC3_UNORM;
                case DDS.FOURCC_BC4U:
                    return GTX.GX2SurfaceFormat.T_BC4_UNORM;
                case DDS.FOURCC_ATI1:
                    return GTX.GX2SurfaceFormat.T_BC4_UNORM;
                case DDS.FOURCC_ATI2:
                    return GTX.GX2SurfaceFormat.T_BC5_UNORM;
                case DDS.FOURCC_BC5U:
                    return GTX.GX2SurfaceFormat.T_BC4_UNORM;
                case DDS.FOURCC_DX10:
                    IsDX10 = true;
                    break;
                default:
                    return GTX.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
            }
            if (IsDX10)
            {
                switch (dds.DX10header.DXGI_Format)
                {
                    case DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
                        return GTX.GX2SurfaceFormat.T_BC4_UNORM;
                    case DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                        return GTX.GX2SurfaceFormat.T_BC4_SNORM;
                    case DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
                        return GTX.GX2SurfaceFormat.T_BC4_UNORM;
                    case DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
                        return GTX.GX2SurfaceFormat.T_BC5_UNORM;
                    case DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
                        return GTX.GX2SurfaceFormat.T_BC5_SNORM;
                    case DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
                        return GTX.GX2SurfaceFormat.T_BC5_UNORM;
                    default:
                        throw new Exception($"Format {dds.DX10header.DXGI_Format} not supported!");
                }
            }
            throw new Exception($"This shouldn't happen :(");
        }
        public void LoadDDS(string FileName, byte[] FileData = null)
        {
            TexName = Path.GetFileNameWithoutExtension(FileName);

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
            DataBlockOutput.Add(dds.bdata);

            Format = LoadDDSFormat(dds.header.ddspf.fourCC, dds, IsSRGB);

            GTX.GX2Surface tex = CreateGx2Texture(DataBlockOutput[0]);
        }
        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = Path.GetFileNameWithoutExtension(FileName);
            Format = GTX.GX2SurfaceFormat.T_BC1_SRGB;
            GenerateMipmaps = true;

            Bitmap Image = new Bitmap(FileName);
            Image = TextureData.SwapBlueRedChannels(Image);

            TexWidth = (uint)Image.Width;
            TexHeight = (uint)Image.Height;
            MipCount = (uint)GetTotalMipCount() + 1;

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

                if (width == 1)
                {
                    ++MipmapNum;
                    break;
                }
                if (height == 1)
                {
                    ++MipmapNum;
                    break;
                }

                width = width / 2;
                height = height / 2;
                if (width <= 0 || height <= 0)
                    break;

                ++MipmapNum;

            }

            return MipmapNum;
        }
        public byte[] GenerateMips(int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);

            List<byte[]> mipmaps = new List<byte[]>();
            mipmaps.Add(FTEX.CompressBlock(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight, Format));

            //while (Image.Width / 2 > 0 && Image.Height / 2 > 0)
            //      for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            int width = Image.Width;
            int height = Image.Height;
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
               if (Image.Width != 1)
                    width = Image.Width / 2;
                if (Image.Height != 1)
                    height = Image.Height / 2;

                Image = BitmapExtension.Resize(Image, width, height);
                mipmaps.Add(FTEX.CompressBlock(BitmapExtension.ImageToByte(Image), Image.Width, Image.Height, Format));
            }
            Image.Dispose();

            return Utils.CombineByteArray(mipmaps.ToArray());
        }
        public void Compress()
        {
            DataBlockOutput.Clear();
            foreach (var surface in DecompressedData)
            {
                DataBlockOutput.Add(FTEX.CompressBlock(surface, (int)TexWidth, (int)TexHeight, Format));
            }
        }

        public GTX.GX2Surface CreateGx2Texture(byte[] imageData)
        {
            var surfOut = GTX.getSurfaceInfo(Format, TexWidth, TexHeight, 1, 1, tileMode, 0, 0);
            uint imageSize = (uint)surfOut.surfSize;
            uint alignment = surfOut.baseAlign;
            uint pitch = surfOut.pitch;
            uint mipSize = 0;
            uint dataSize = (uint)imageData.Length;
            uint bpp = GTX.surfaceGetBitsPerPixel((uint)Format) >> 3;

            if (dataSize <= 0)
                throw new Exception($"Image is empty!!");

            if (surfOut.depth != 1)
                throw new Exception($"Unsupported Depth {surfOut.depth}!");

            uint s = 0;
            switch (tileMode)
            {
                case 1:
                case 2:
                case 3:
                case 16:
                    s = 0;
                    break;
                default:
                    s = 0xd0000 | swizzle << 8;
                    break;
            }
            uint blkWidth, blkHeight;
            if (GTX.IsFormatBCN(Format))
            {
                blkWidth = 4;
                blkHeight = 4;
            }
            else
            {
                blkWidth = 1;
                blkHeight = 1;
            }
            List<uint> mipOffsets = new List<uint>();
            List<byte[]> Swizzled = new List<byte[]>();

            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                var result = TextureHelper.GetCurrentMipSize(TexWidth, TexHeight, blkWidth, blkHeight, bpp, mipLevel);

                uint offset = result.Item1;
                uint size = result.Item2;

                Console.WriteLine("Swizzle Size " + size);
                Console.WriteLine("Swizzle offset " + offset);
                Console.WriteLine("bpp " + bpp);
                Console.WriteLine("TexWidth " + TexWidth);
                Console.WriteLine("TexHeight " + TexHeight);
                Console.WriteLine("blkWidth " + blkWidth);
                Console.WriteLine("blkHeight " + blkHeight);
                Console.WriteLine("mipLevel " + mipLevel);

                byte[] data_ = new byte[size];
                Array.Copy(imageData, offset, data_,0, size);

                uint width_ = Math.Max(1, TexWidth >> mipLevel);
                uint height_ = Math.Max(1, TexHeight >> mipLevel);

                if (mipLevel != 0)
                {
                    surfOut = GTX.getSurfaceInfo(Format, TexWidth, TexHeight, 1, 1, tileMode, 0, mipLevel);

                    if (mipLevel == 1)
                        mipOffsets.Add(imageSize);
                    else
                        mipOffsets.Add(mipSize);
                }

                data_ = Utils.CombineByteArray(data_, new byte[surfOut.surfSize - size]);
                byte[] dataAlignBytes = new byte[RoundUp(mipSize, surfOut.baseAlign) - mipSize];
                    
                if (mipLevel != 0)
                    mipSize += (uint)(surfOut.surfSize + dataAlignBytes.Length);

                byte[] SwizzledData = GTX.swizzle(width_, height_, surfOut.height, (uint)Format, surfOut.tileMode, s,
                        surfOut.pitch, surfOut.bpp, data_);

                Swizzled.Add(dataAlignBytes.Concat(SwizzledData).ToArray());
            }

            compSel[0] = GX2CompSel.ChannelR;
            compSel[1] = GX2CompSel.ChannelG;
            compSel[2] = GX2CompSel.ChannelB;
            compSel[3] = GX2CompSel.ChannelA;

            GTX.GX2Surface surf = new GTX.GX2Surface();
            surf.depth = Depth;
            surf.width = TexWidth;
            surf.height = TexHeight;
            surf.depth = 1;
            surf.use = 1;
            surf.dim = (uint)SurfaceDim;
            surf.tileMode = tileMode;
            surf.swizzle = s;
            surf.resourceFlags = 0;
            surf.pitch = pitch;
            surf.bpp = bpp;
            surf.format = (uint)Format;
            surf.numMips = MipCount;
            surf.aa = (uint)AAMode;
            surf.mipOffset = mipOffsets.ToArray();
            surf.numMips = (uint)Swizzled.Count;
            surf.alignment = alignment;
            surf.mipSize = mipSize;
            surf.imageSize = imageSize;
            surf.data = Swizzled[0];

            List<byte[]> mips = new List<byte[]>();
            for (int mipLevel = 1; mipLevel < Swizzled.Count; mipLevel++)
            {
                mips.Add(Swizzled[mipLevel]);
                Console.WriteLine(Swizzled[mipLevel].Length);
            }
            surf.mipData = Utils.CombineByteArray(mips.ToArray());
            mips.Clear();


            Console.WriteLine("");
            Console.WriteLine("// ----- GX2Surface Info ----- ");
            Console.WriteLine("  dim             = 1");
            Console.WriteLine("  width           = " + surf.width);
            Console.WriteLine("  height          = " + surf.height);
            Console.WriteLine("  depth           = 1");
            Console.WriteLine("  numMips         = " + surf.numMips);
            Console.WriteLine("  format          = " + surf.format);
            Console.WriteLine("  aa              = 0");
            Console.WriteLine("  use             = 1");
            Console.WriteLine("  imageSize       = " + surf.imageSize);
            Console.WriteLine("  mipSize         = " + surf.mipSize);
            Console.WriteLine("  tileMode        = " + surf.tileMode);
            Console.WriteLine("  swizzle         = " + surf.swizzle);
            Console.WriteLine("  alignment       = " + surf.alignment);
            Console.WriteLine("  pitch           = " + surf.pitch);
            Console.WriteLine("");
            Console.WriteLine("  GX2 Component Selector:");
            Console.WriteLine("  Red Channel:    " + compSel[0]);
            Console.WriteLine("  Green Channel:  " + compSel[1]);
            Console.WriteLine("  Blue Channel:   " + compSel[2]);
            Console.WriteLine("  Alpha Channel:  " + compSel[3]);
            Console.WriteLine("");
            Console.WriteLine("  bits per pixel  = " + (surf.bpp << 3));
            Console.WriteLine("  bytes per pixel = " + surf.bpp);
            Console.WriteLine("  realSize        = " + imageData.Length);

            return surf;
        }
        private static Tuple<uint, uint> GetCurrentMipSize(uint width, uint height, uint bpp, int CurLevel, bool IsCompressed)
        {
            uint offset = 0;
            uint size = 0;

            for (int mipLevel = 0; mipLevel < CurLevel; mipLevel++)
            {
                int level = mipLevel + 1;
                if (IsCompressed)
                    offset += ((Math.Max(1, width >> level) + 3) >> 2) * ((Math.Max(1, height >> level) + 3) >> 2) * bpp;
                else
                    offset += Math.Max(1, width >> level) * Math.Max(1, height >> level) * bpp;
            }
            if (IsCompressed)
                size = ((Math.Max(1, width >> CurLevel) + 3) >> 2) * ((Math.Max(1, height >> CurLevel) + 3) >> 2) * bpp;
            else
                size = Math.Max(1, width >> CurLevel) * Math.Max(1, height >> CurLevel) * bpp;

            return Tuple.Create(offset, size);

        }
        private uint getAlignBlockSize(uint dataOffset, uint alignment)
        {
            uint alignSize = RoundUp(dataOffset, alignment) - dataOffset - 32;

            uint z = 1;
            while (alignSize <= 0)
            {
                alignSize = RoundUp(dataOffset + (alignment * z), alignment) - dataOffset - 32;
                z += 1;
            }
            return alignSize;
        }

        private int RoundUp(int X, int Y)
        {
            return ((X - 1) | (Y - 1)) + 1;
        }
        private uint RoundUp(uint X, uint Y)
        {
            return ((X - 1) | (Y - 1)) + 1;
        }
    }
}
