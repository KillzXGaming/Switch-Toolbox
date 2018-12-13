using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Smash_Forge.Rendering;

namespace Switch_Toolbox.Library.Rendering
{
    public class RenderableTex
    {
        public int width, height;
        public int display;
        public PixelInternalFormat pixelInternalFormat;
        public PixelFormat pixelFormat;
        public PixelType pixelType = PixelType.UnsignedByte;
        public int mipMapCount;
        public List<List<byte[]>> mipmaps = new List<List<byte[]>>();
        public byte[] data;
        public bool GLInitialized = false;

        public void LoadOpenGLTexture(STGenericTexture GenericTexture)
        {
            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;

            bool IsSNORM = (GenericTexture.FormatType == TEX_FORMAT_TYPE.SNORM);
            bool IsSRGB = (GenericTexture.FormatType == TEX_FORMAT_TYPE.SRGB);
            bool IsFLOAT = (GenericTexture.FormatType == TEX_FORMAT_TYPE.FLOAT);
            bool IsSINT = (GenericTexture.FormatType == TEX_FORMAT_TYPE.SINT);

            if (GenericTexture.Surfaces.Count <= 0)
            {
                throw new Exception($"No texture data found with texture {GenericTexture.Text}");
            }

            data = GenericTexture.Surfaces[0].mipmaps[0];
            width = (int)GenericTexture.Width;
            height = (int)GenericTexture.Height;

            switch (GenericTexture.Format)
            {
                case TEX_FORMAT.BC1:
                    if (GenericTexture.FormatType == TEX_FORMAT_TYPE.SRGB)
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    else
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case TEX_FORMAT.BC2:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case TEX_FORMAT.BC3:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case TEX_FORMAT.BC4:
                    pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case TEX_FORMAT.BC5:
                    if (IsSNORM)
                    {
                        pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;

                        data = DDSCompressor.DecompressBC5(GenericTexture.Surfaces[0].mipmaps[0],
                            (int)GenericTexture.Width, (int)GenericTexture.Height, true, true);
                        pixelInternalFormat = PixelInternalFormat.Rgba;
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    }
                    else
                        pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case TEX_FORMAT.BC6:
                    if (IsFLOAT)
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbBptcSignedFloat;
                    else
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbBptcUnsignedFloat;
                    break;
                case TEX_FORMAT.BC7:
                    if (IsSRGB)
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbaBptcUnorm;
                    else
                        pixelInternalFormat = PixelInternalFormat.CompressedSrgbAlphaBptcUnorm;
                    break;
                case TEX_FORMAT.R8_G8_B8_A8:
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
            }
            display = loadImage(this);

            GLInitialized = true;
        }
        public static int loadImage(RenderableTex t)
        {
            if (!t.GLInitialized)
                return -1;

            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);

            if (t.pixelInternalFormat != PixelInternalFormat.Rgba)
            {
                GL.CompressedTexImage2D<byte>(TextureTarget.Texture2D, 0, (InternalFormat)t.pixelFormat,
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
                    return (t.width * t.height);
                case PixelInternalFormat.Rgba:
                    return t.data.Length;
                default:
                    return t.data.Length;
            }
        }
        public static unsafe Bitmap GLTextureToBitmap(RenderableTex t, int id)
        {
            if (Viewport.Instance.gL_ControlModern1 == null)
                return null;

            Bitmap bitmap = new Bitmap(t.width, t.height);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, t.width, t.height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }

}
