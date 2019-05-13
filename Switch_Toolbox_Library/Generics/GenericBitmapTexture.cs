using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Toolbox.Library
{
    //
    // A class which can create generic texture instances from bitmaps for usage in opengl and image editors.
    //
    public class GenericBitmapTexture : STGenericTexture
    {
        public override bool CanEdit { get; set; } = true;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] { TEX_FORMAT.R8G8B8A8_UNORM };
            }
        }

        public byte[] ImageData;

        public GenericBitmapTexture(byte[] FileData, int width, int height)
        {
            Format = TEX_FORMAT.R8G8B8A8_UNORM;
            Width = (uint)width;
            Height = (uint)height;

            ImageData = DDSCompressor.CompressBlock(FileData, width, height,
                DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB);
        }

        public GenericBitmapTexture(string FileName) {
            LoadBitmap(new Bitmap(FileName));
        }

        public GenericBitmapTexture(Bitmap Image) {
            LoadBitmap(Image);
        }

        private void LoadBitmap(Bitmap Image)
        {
            Image = BitmapExtension.SwapBlueRedChannels(Image);

            Format = TEX_FORMAT.R8G8B8A8_UNORM;
            Width = (uint)Image.Width;
            Height = (uint)Image.Height;
            MipCount = 1;

            ImageData = GenerateMipsAndCompress(Image, Format);

            if (ImageData == null || ImageData.Length <= 0)
                throw new Exception("Image failed to encode!");
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            byte[] Data = BitmapExtension.ImageToByte(bitmap);
            Width = (uint)bitmap.Width;
            Height = (uint)bitmap.Height;

            ImageData = DDSCompressor.EncodePixelBlock(Data, bitmap.Width, bitmap.Width, DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB);
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            return ImageData;
        }
    }
}
