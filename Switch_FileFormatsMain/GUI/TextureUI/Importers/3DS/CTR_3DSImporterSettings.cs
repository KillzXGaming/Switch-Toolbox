using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Syroot.NintenTools.Bfres.GX2;
using Bfres.Structs;

namespace FirstPlugin
{
    public class CTR_3DSImporterSettings
    {
        public string TexName;
        public uint TexWidth;
        public uint TexHeight;
        public uint MipCount;
        public uint Depth = 1;
        public uint arrayLength = 1;
        public List<byte[]> DataBlockOutput = new List<byte[]>();
        public List<byte[]> DecompressedData = new List<byte[]>();
        public CTR_3DS.PICASurfaceFormat Format;
        public bool GenerateMipmaps;
        public bool IsSRGB;
        public uint tileMode = 4;

        public uint SwizzlePattern = 0;
        public uint Swizzle = 0;
        public uint MipSwizzle = 0;

        public float alphaRef = 0.5f;

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

            Format = (CTR_3DS.PICASurfaceFormat)CTR_3DS.ConvertToPICAFormat(dds.Format);
        }

        public void LoadBitMap(Image Image, string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);

            Format = CTR_3DS.ConvertToPICAFormat(Runtime.PreferredTexFormat);

            GenerateMipmaps = true;
            LoadImage(new Bitmap(Image));
        }

        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);

            Format = CTR_3DS.ConvertToPICAFormat(Runtime.PreferredTexFormat);
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
                (int)TexWidth, (int)TexHeight, FTEX.ConvertFromGx2Format((GX2SurfaceFormat)Format), alphaRef));

            //while (Image.Width / 2 > 0 && Image.Height / 2 > 0)
            //      for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                Image = BitmapExtension.Resize(Image, Image.Width / 2, Image.Height / 2);
                mipmaps.Add(STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, FTEX.ConvertFromGx2Format((GX2SurfaceFormat)Format), alphaRef));
            }
            Image.Dispose();

            return Utils.CombineByteArray(mipmaps.ToArray());
        }
        public void Compress()
        {
            DataBlockOutput.Clear();
            foreach (var surface in DecompressedData)
            {

            }
        }
    }
}
