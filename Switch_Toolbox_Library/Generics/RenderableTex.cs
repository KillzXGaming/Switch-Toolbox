using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Switch_Toolbox.Library.Rendering
{
    public class RenderableTex
    {
        public int width, height;
        public int TexID;
        public PixelInternalFormat pixelInternalFormat;
        public PixelFormat pixelFormat;
        public PixelType pixelType = PixelType.UnsignedByte;
        public byte[] data;
        public bool GLInitialized = false;

        public void Dispose()
        {
            GL.DeleteTexture(TexID);
            data = null;
        }

        public void LoadOpenGLTexture(STGenericTexture GenericTexture)
        {
            if (!Runtime.OpenTKInitialized || GLInitialized || Runtime.UseLegacyGL)
                return;

            if (GenericTexture.ArrayCount <= 0)
            {
                throw new Exception($"No texture data found with texture {GenericTexture.Text}");
            }

            width = (int)GenericTexture.Width;
            height = (int)GenericTexture.Height;

            data = GenericTexture.GetImageData(0, 0);

            if (data.Length <= 0)
                throw new Exception("Data is empty!");

            switch (GenericTexture.Format)
            {
                case TEX_FORMAT.BC1_UNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case TEX_FORMAT.BC1_UNORM_SRGB:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case TEX_FORMAT.BC2_UNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case TEX_FORMAT.BC2_UNORM_SRGB:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case TEX_FORMAT.BC3_UNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case TEX_FORMAT.BC3_UNORM_SRGB:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case TEX_FORMAT.BC4_UNORM:
                case TEX_FORMAT.BC4_SNORM:
                    //Convert to rgb to prevent red output
                    //While shaders could prevent this, converting is easier and works fine across all editors
                    data = STGenericTexture.DecodeBlock(data,
                        GenericTexture.Width,
                        GenericTexture.Height,
                        GenericTexture.Format,
                        GenericTexture.PlatformSwizzle);

                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;

                   // pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    //     pixelInternalFormat = PixelInternalFormat.CompressedSignedRedRgtc1;
                    break;
                case TEX_FORMAT.BC5_SNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;

                    data = DDSCompressor.DecompressBC5(GenericTexture.GetImageData(0,0),
                        (int)GenericTexture.Width, (int)GenericTexture.Height, true, true);
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case TEX_FORMAT.BC5_UNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case TEX_FORMAT.BC6H_UF16:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbBptcUnsignedFloat;
                    break;
                case TEX_FORMAT.BC6H_SF16:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbBptcSignedFloat;
                    break;
                case TEX_FORMAT.BC7_UNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaBptcUnorm;
                    break;
                case TEX_FORMAT.BC7_UNORM_SRGB:
                    pixelInternalFormat = PixelInternalFormat.CompressedSrgbAlphaBptcUnorm;
                    break;
                case TEX_FORMAT.R8G8B8A8_UNORM:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                default:
                    data = STGenericTexture.DecodeBlock(data,
                        GenericTexture.Width, 
                        GenericTexture.Height, 
                        GenericTexture.Format,
                        GenericTexture.PlatformSwizzle);

                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
            }
            GLInitialized = true;

            TexID = loadImage(this);
        }

        public static int loadImage(RenderableTex t)
        {
            if (!t.GLInitialized)
                return -1;

            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);

            if (t.pixelInternalFormat != PixelInternalFormat.Rgba)
            {
                GL.CompressedTexImage2D<byte>(TextureTarget.Texture2D, 0, (InternalFormat)t.pixelInternalFormat,
                    t.width, t.height, 0, getImageSize(t), t.data);
                //Debug.WriteLine(GL.GetError());
            }
            else
            {
                GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, t.pixelInternalFormat, t.width, t.height, 0,
                    t.pixelFormat, PixelType.UnsignedByte, t.data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, TexID);
        }

        private static int getImageSize(RenderableTex t)
        {
            switch (t.pixelInternalFormat)
            {
                case PixelInternalFormat.CompressedRgbaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedRedRgtc1:
                case PixelInternalFormat.CompressedSignedRedRgtc1:
                    return (t.width * t.height / 2);
                case PixelInternalFormat.CompressedRgbaS3tcDxt3Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext:
                case PixelInternalFormat.CompressedRgbaS3tcDxt5Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext:
                case PixelInternalFormat.CompressedSignedRgRgtc2:
                case PixelInternalFormat.CompressedRgRgtc2:
                case PixelInternalFormat.CompressedRgbaBptcUnorm:
                case PixelInternalFormat.CompressedSrgbAlphaBptcUnorm:
                    return (t.width * t.height);
                case PixelInternalFormat.Rgba:
                    return t.data.Length;
                default:
                    return t.data.Length;
            }
        }

        public unsafe Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(width, height);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, TexID);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public static unsafe Bitmap GLTextureToBitmap(RenderableTex t)
        {
            Bitmap bitmap = new Bitmap(t.width, t.height);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, t.width, t.height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, t.TexID);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }

}
