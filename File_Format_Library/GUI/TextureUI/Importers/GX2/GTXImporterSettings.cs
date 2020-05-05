using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using Syroot.NintenTools.Bfres.GX2;
using Bfres.Structs;

namespace FirstPlugin
{
    public class GTXImporterSettings
    {
        public bool FlipY = false;

        public string TexName;
        public uint TexWidth;
        public uint TexHeight;

        private uint mipCount;
        public uint MipCount
        {
            get
            {
                if (UseOriginalMips && MipCountOriginal >= 0) return (uint)MipCountOriginal;
                else return mipCount;
            }
            set { mipCount = value; }
        }

        public int MipCountOriginal = -1;
        public uint Depth = 1;
        public uint arrayLength = 1;
        public List<byte[]> DataBlockOutput = new List<byte[]>();
        public List<byte[]> DecompressedData = new List<byte[]>();
        public GX2.GX2SurfaceFormat Format = (GX2.GX2SurfaceFormat)FTEX.ConvertToGx2Format(Runtime.PreferredTexFormat);
        public bool GenerateMipmaps;
        public bool IsSRGB;
        public bool UseOriginalMips = false;
        public uint tileMode = 4;

        public bool UseBc4Alpha;

        public uint SwizzlePattern = 0;
        public uint Swizzle = 0;
        public uint MipSwizzle = 0;

        public STChannelType RedComp = STChannelType.Red;
        public STChannelType GreenComp = STChannelType.Green;
        public STChannelType BlueComp = STChannelType.Blue;
        public STChannelType AlphaComp = STChannelType.Alpha;

        public GX2SurfaceDim SurfaceDim = GX2SurfaceDim.Dim2D;
        public GX2AAMode AAMode = GX2AAMode.Mode1X;
        public float alphaRef = 0.5f;

        public bool IsFinishedCompressing = false;

        public void LoadDDS(string FileName, byte[] FileData = null)
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
            arrayLength = 1;
            if (dds.header.caps2 == (uint)DDS.DDSCAPS2.CUBEMAP_ALLFACES)
            {
                arrayLength = 6;
            }
            DataBlockOutput.Add(dds.bdata);

            RedComp = dds.RedChannel;
            GreenComp = dds.GreenChannel;
            BlueComp = dds.BlueChannel;
            AlphaComp = dds.AlphaChannel;

            Format = (GX2.GX2SurfaceFormat)FTEX.ConvertToGx2Format(dds.Format);;
        }

        public void LoadBitMap(Image Image, string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);

            GenerateMipmaps = true;
            LoadImage(new Bitmap(Image));
        }

        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);

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

        public List<byte[]> GenerateMipList(int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);
            if (FlipY)
                Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (UseBc4Alpha && (Format == GX2.GX2SurfaceFormat.T_BC4_UNORM || Format == GX2.GX2SurfaceFormat.T_BC4_SNORM)) {
                Image = BitmapExtension.SetChannel(Image, STChannelType.Alpha, STChannelType.Alpha, STChannelType.Alpha, STChannelType.One);
            }

            Console.WriteLine($"FlipY {FlipY}");

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int MipWidth = Math.Max(1, (int)TexWidth >> mipLevel);
                int MipHeight = Math.Max(1, (int)TexHeight >> mipLevel);

                if (mipLevel != 0)
                    Image = BitmapExtension.Resize(Image, MipWidth, MipHeight);

                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                   Image.Width, Image.Height, FTEX.ConvertFromGx2Format((GX2SurfaceFormat)Format), alphaRef));
            }
            Image.Dispose();

            return mipmaps;
        }

        public byte[] GenerateMips(int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);

            if (FlipY)
                Image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int MipWidth = Math.Max(1, (int)TexWidth >> mipLevel);
                int MipHeight = Math.Max(1, (int)TexHeight >> mipLevel);

                if (mipLevel != 0)
                    Image = BitmapExtension.Resize(Image, MipWidth, MipHeight);

                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                       Image.Width, Image.Height, FTEX.ConvertFromGx2Format((GX2SurfaceFormat)Format), alphaRef));
            }
            Image.Dispose();

            return Utils.CombineByteArray(mipmaps.ToArray());
        }

        public void Compress()
        {
            if (IsFinishedCompressing)
                return;

            DataBlockOutput.Clear();
            foreach (var surface in DecompressedData)
            {
                DataBlockOutput.Add(FTEX.CompressBlock(surface, (int)TexWidth, (int)TexHeight,
                    FTEX.ConvertFromGx2Format((GX2SurfaceFormat)Format), alphaRef));
            }
        }
    }
}
