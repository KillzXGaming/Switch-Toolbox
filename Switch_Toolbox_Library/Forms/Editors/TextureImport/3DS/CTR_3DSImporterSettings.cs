using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace Toolbox.Library.Forms
{
    public class CTR_3DSImporterSettings
    {
        public string TexName;
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

        //Round 3DS width/height by power of 2
        private uint _width;
        public uint TexWidth
        {
            get { return _width; }
            set {
                //3DS Limitations
                _width = Pow2RoundDown(value);
                if (_width > 1024)
                    _width = 1024;
                if (_width < 8)
                    _width = 8;
            }
        }

        private uint _height;
        public uint TexHeight
        {
            get { return _height; }
            set { 
                _height = Pow2RoundDown(value);
                if (_height > 1024)
                    _height = 1024;
                if (_height < 8)
                    _height = 8;
            }
        }

        public TEX_FORMAT GenericFormat
        {
            get
            {
                return CTR_3DS.ConvertPICAToGenericFormat(Format);
            }
        }

        static uint Pow2RoundDown(uint Value) {
            return IsPow2(Value) ? Value : Pow2RoundUp(Value) >> 1;
        }

        static bool IsPow2(uint Value) {
            return Value != 0 && (Value & (Value - 1)) == 0;
        }

        static uint Pow2RoundUp(uint Value)
        {
            Value--;

            Value |= (Value >> 1);
            Value |= (Value >> 2);
            Value |= (Value >> 4);
            Value |= (Value >> 8);
            Value |= (Value >> 16);

            return ++Value;
        }

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

            Format = (CTR_3DS.PICASurfaceFormat)CTR_3DS.ConvertToPICAFormat(dds.Format);
        }

        public void LoadBitMap(Image Image, string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);

            Format = CTR_3DS.PICASurfaceFormat.RGBA8;

            GenerateMipmaps = true;
            LoadImage(new Bitmap(Image));
        }

        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);

            Format = CTR_3DS.PICASurfaceFormat.RGBA8;
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

            if (TexWidth != Image.Width || TexHeight != Image.Height)
                Image = BitmapExtension.Resize(Image, TexWidth, TexHeight);

            DecompressedData.Add(STGenericTexture.ConvertBgraToRgba(BitmapExtension.ImageToByte(Image)));

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

            uint width = TexWidth;
            uint height = TexHeight;

            while (true)
            {
                num >>= 1;

                width = width / 2;
                height = height / 2;

                width = Pow2RoundDown(width);
                height = Pow2RoundDown(height);

                if (Format == CTR_3DS.PICASurfaceFormat.ETC1 || Format == CTR_3DS.PICASurfaceFormat.ETC1A4)
                {
                    if (width < 16 || height < 16)
                        break;
                }
                else if (width < 8 || height < 8)
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
            return Utils.CombineByteArray(GenerateMipList().ToArray());
        }

        public List<byte[]> GenerateMipList(int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);

            List<byte[]> mipmaps = new List<byte[]>();
            mipmaps.Add(CTR_3DS.EncodeBlock(DecompressedData[SurfaceLevel],
                (int)TexWidth, (int)TexHeight, Format));

            //while (Image.Width / 2 > 0 && Image.Height / 2 > 0)
            //      for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int width = Image.Width / 2;
                int height = Image.Width / 2;
                if (Format == CTR_3DS.PICASurfaceFormat.ETC1 || Format == CTR_3DS.PICASurfaceFormat.ETC1A4) {
                    if (width < 16)
                        break;
                    if (height < 16)
                        break;
                }
                else
                {
                    if (width < 8)
                        break;
                    if (height < 8)
                        break;
                }

                Image = BitmapExtension.Resize(Image, width, height);
                mipmaps.Add(CTR_3DS.EncodeBlock(BitmapExtension.ImageToByte(Image),
                    Image.Width, Image.Height, Format));
            }   
            Image.Dispose();

            return mipmaps;
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
