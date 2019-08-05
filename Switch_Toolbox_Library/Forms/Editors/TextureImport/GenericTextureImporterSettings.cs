using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class GenericTextureImporterSettings
    {
        public GenericTextureImporterSettings()
        {
        }

        public string TexName;

        public uint MipCount;

        public uint Depth = 1;

        public uint TexWidth;

        public uint TexHeight;

        public TEX_FORMAT Format = TEX_FORMAT.BC1_UNORM_SRGB;

        public STChannelType RedComp = STChannelType.Red;
        public STChannelType GreenComp = STChannelType.Green;
        public STChannelType BlueComp = STChannelType.Blue;
        public STChannelType AlphaComp = STChannelType.Alpha;

        public List<byte[]> DataBlockOutput = new List<byte[]>();
        public List<byte[]> DecompressedData = new List<byte[]>();

        public bool GenerateMipmaps = false; //If bitmap and count more that 1 then generate
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

            var surfaces = DDS.GetArrayFaces(dds, dds.ArrayCount);

            RedComp = dds.RedChannel;
            GreenComp = dds.GreenChannel;
            BlueComp = dds.BlueChannel;
            AlphaComp = dds.AlphaChannel;

            foreach (var surface in surfaces)
                DataBlockOutput.Add(Utils.CombineByteArray(surface.mipmaps.ToArray()));

            Format = dds.Format;
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

            Format = astc.Format;
        }

        public void LoadBitMap(Image Image, string Name)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(Name);

            Format = Runtime.PreferredTexFormat;

            GenerateMipmaps = true;
            LoadImage(new Bitmap(Image));
        }

        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);
            Format = Runtime.PreferredTexFormat;

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


        public List<byte[]> GenerateMipList(STCompressionMode CompressionMode, int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int MipWidth = Math.Max(1, (int)TexWidth >> mipLevel);
                int MipHeight = Math.Max(1, (int)TexHeight >> mipLevel);

                if (mipLevel != 0)
                    Image = BitmapExtension.Resize(Image, MipWidth, MipHeight);

                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, Format, alphaRef, CompressionMode));
            }
            Image.Dispose();

            return mipmaps;
        }

        public byte[] GenerateMips(STCompressionMode CompressionMode, int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int MipWidth = Math.Max(1, (int)TexWidth >> mipLevel);
                int MipHeight = Math.Max(1, (int)TexHeight >> mipLevel);

                if (mipLevel != 0)
                    Image = BitmapExtension.Resize(Image, MipWidth, MipHeight);

                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, Format, alphaRef, CompressionMode));
            }
            Image.Dispose();

            return Utils.CombineByteArray(mipmaps.ToArray());
        }

        public void Compress(STCompressionMode CompressionMode)
        {
            DataBlockOutput.Clear();
            foreach (var surface in DecompressedData)
            {
                DataBlockOutput.Add(STGenericTexture.CompressBlock(surface,
                    (int)TexWidth, (int)TexHeight, Format, alphaRef, CompressionMode));
            }
        }
    }
}
