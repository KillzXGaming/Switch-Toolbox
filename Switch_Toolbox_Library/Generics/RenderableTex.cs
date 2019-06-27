using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Switch_Toolbox.Library.Rendering
{
    //Parts roughly based on this helpful gl wrapper https://github.com/ScanMountGoat/SFGraphics/blob/89f96b754e17078153315a259baef3859ef5984d/Projects/SFGraphics/GLObjects/Textures/Texture.cs
    public class RenderableTex
    {
        public int width, height;
        public int TexID;
        public PixelInternalFormat pixelInternalFormat;
        public PixelFormat pixelFormat;
        public PixelType pixelType = PixelType.UnsignedByte;
        public TextureTarget TextureTarget = TextureTarget.Texture2D;
        public bool GLInitialized = false;
        public int ImageSize;
        public bool IsCubeMap = false;

        public TextureWrapMode TextureWrapS
        {
            get { return textureWrapS; }
            set
            {
                textureWrapS = value;
                SetTextureParameter(TextureParameterName.TextureWrapS, (int)value);
            }
        }

        public TextureWrapMode TextureWrapT
        {
            get { return textureWrapT; }
            set
            {
                textureWrapT = value;
                SetTextureParameter(TextureParameterName.TextureWrapT, (int)value);
            }
        }

        public TextureWrapMode TextureWrapR
        {
            get { return textureWrapR; }
            set
            {
                textureWrapR = value;
                SetTextureParameter(TextureParameterName.TextureWrapR, (int)value);
            }
        }

        public TextureMinFilter TextureMinFilter
        {
            get { return textureMinFilter; }
            set
            {
                textureMinFilter = value;
                SetTextureParameter(TextureParameterName.TextureMinFilter, (int)value);
            }
        }

        public TextureMagFilter TextureMagFilter
        {
            get { return textureMagFilter; }
            set
            {
                textureMagFilter = value;
                SetTextureParameter(TextureParameterName.TextureMinFilter, (int)value);
            }
        }

        private TextureWrapMode textureWrapS = TextureWrapMode.Repeat;
        private TextureWrapMode textureWrapT = TextureWrapMode.Repeat;
        private TextureWrapMode textureWrapR = TextureWrapMode.Clamp;
        private TextureMinFilter textureMinFilter = TextureMinFilter.Linear;
        private TextureMagFilter textureMagFilter = TextureMagFilter.Linear;

        public void Dispose()
        {
            GL.DeleteTexture(TexID);
        }

        public void SetTextureParameter(TextureParameterName param, int value)
        {
            Bind();
            GL.TexParameter(TextureTarget, param, value);
        }

        public void LoadOpenGLTexture(STGenericTexture GenericTexture, int ArrayStartIndex = 0)
        {
            if (!Runtime.OpenTKInitialized || GLInitialized || Runtime.UseLegacyGL)
                return;

            width = (int)GenericTexture.Width;
            height = (int)GenericTexture.Height;
            if (GenericTexture.ArrayCount == 0)
                GenericTexture.ArrayCount = 1;

            List<byte[]> ImageData = new List<byte[]>();
            for (int i = 0; i < GenericTexture.ArrayCount; i++)
            {
                if (i >= ArrayStartIndex && i <= ArrayStartIndex + 6) //Only load up to 6 faces
                    ImageData.Add(GenericTexture.GetImageData(i, 0));
            }

            if (ImageData.Count == 0 || ImageData[0].Length == 0)
                throw new Exception("Data is empty!");

            IsCubeMap = ImageData.Count == 6;
            ImageSize = ImageData[0].Length;

            if (IsCubeMap)
            {
                TextureTarget = TextureTarget.TextureCubeMap;
                TextureWrapS = TextureWrapMode.Clamp;
                TextureWrapT = TextureWrapMode.Clamp;
                TextureWrapR = TextureWrapMode.Clamp;
                TextureMinFilter = TextureMinFilter.LinearMipmapLinear;
                TextureMagFilter = TextureMagFilter.Linear;
            }

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
                    if (Runtime.UseDirectXTexDecoder)
                    {
                        ImageData.Add(STGenericTexture.DecodeBlock(ImageData[0],
                        GenericTexture.Width,
                        GenericTexture.Height,
                        GenericTexture.Format,
                        GenericTexture.PlatformSwizzle));
                        pixelInternalFormat = PixelInternalFormat.Rgba;
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    }
                    else
                    {
                        pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                        pixelInternalFormat = PixelInternalFormat.CompressedSignedRedRgtc1;
                    }
                    break;
                case TEX_FORMAT.BC5_SNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;

                    ImageData.Add(DDSCompressor.DecompressBC5(ImageData[0],
                        (int)GenericTexture.Width, (int)GenericTexture.Height, true, true));
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
                    if (Runtime.UseDirectXTexDecoder)
                    {
                        ImageData.Add(STGenericTexture.DecodeBlock(ImageData[0],
                        GenericTexture.Width,
                        GenericTexture.Height,
                        GenericTexture.Format,
                        GenericTexture.PlatformSwizzle));

                        pixelInternalFormat = PixelInternalFormat.Rgba;
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    }
                    break;
            }
            GLInitialized = true;

            TexID = GenerateOpenGLTexture(this, ImageData);

            ImageData.Clear();
        }

        public static int GenerateOpenGLTexture(RenderableTex t, List<byte[]> ImageData)
        {
            if (!t.GLInitialized)
                return -1;

            int texID = GL.GenTexture();

            GL.BindTexture(t.TextureTarget, texID);
            if (t.IsCubeMap)
            {
                if (t.pixelInternalFormat != PixelInternalFormat.Rgba)
                {
                    t.LoadCompressedMips(TextureTarget.TextureCubeMapPositiveX, ImageData[0]);
                    t.LoadCompressedMips(TextureTarget.TextureCubeMapNegativeX, ImageData[1]);

                    t.LoadCompressedMips(TextureTarget.TextureCubeMapPositiveY, ImageData[2]);
                    t.LoadCompressedMips(TextureTarget.TextureCubeMapNegativeY, ImageData[3]);

                    t.LoadCompressedMips(TextureTarget.TextureCubeMapPositiveZ, ImageData[4]);
                    t.LoadCompressedMips(TextureTarget.TextureCubeMapNegativeZ, ImageData[5]);
                }
                else
                {
                    t.LoadUncompressedMips(TextureTarget.TextureCubeMapPositiveX, ImageData[0]);
                    t.LoadUncompressedMips(TextureTarget.TextureCubeMapNegativeX, ImageData[1]);

                    t.LoadUncompressedMips(TextureTarget.TextureCubeMapPositiveY, ImageData[2]);
                    t.LoadUncompressedMips(TextureTarget.TextureCubeMapNegativeY, ImageData[3]);

                    t.LoadUncompressedMips(TextureTarget.TextureCubeMapPositiveZ, ImageData[4]);
                    t.LoadUncompressedMips(TextureTarget.TextureCubeMapNegativeZ, ImageData[5]);
                }
            }
            else
            {
                if (t.pixelInternalFormat != PixelInternalFormat.Rgba)
                {
                    GL.CompressedTexImage2D<byte>(TextureTarget.Texture2D, 0, (InternalFormat)t.pixelInternalFormat,
                        t.width, t.height, 0, getImageSize(t), ImageData[0]);
                    //Debug.WriteLine(GL.GetError());
                }
                else
                {
                    GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, t.pixelInternalFormat, t.width, t.height, 0,
                        t.pixelFormat, PixelType.UnsignedByte, ImageData[0]);
                }
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            return texID;
        }

        private void LoadUncompressedMips(TextureTarget textureTarget, byte[] ImageData, int MipLevel = 0)
        {
            GL.TexImage2D<byte>(textureTarget, MipLevel, pixelInternalFormat, width, height, 0,
                     pixelFormat, PixelType.UnsignedByte, ImageData);

            GL.GenerateMipmap((GenerateMipmapTarget)textureTarget);
        }

        private void LoadCompressedMips(TextureTarget textureTarget, byte[] ImageData, int MipLevel = 0)
        {
            GL.CompressedTexImage2D<byte>(textureTarget, MipLevel, (InternalFormat)pixelInternalFormat,
                  width, height, 0, getImageSize(this), ImageData);

            GL.GenerateMipmap((GenerateMipmapTarget)textureTarget);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget, TexID);
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
                    return t.ImageSize;
                default:
                    return t.ImageSize;
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
