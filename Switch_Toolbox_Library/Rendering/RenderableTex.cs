using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Toolbox.Library.Rendering
{
    //Parts roughly based on this helpful gl wrapper https://github.com/ScanMountGoat/SFGraphics/blob/89f96b754e17078153315a259baef3859ef5984d/Projects/SFGraphics/GLObjects/Textures/Texture.cs
    public class RenderableTex
    {
        public int width, height;
        public int TexID = -1;
        public PixelInternalFormat pixelInternalFormat;
        public OpenTK.Graphics.OpenGL.PixelFormat pixelFormat;
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
            if (TexID != -1)
                GL.DeleteTexture(TexID);
        }

        public void SetTextureParameter(TextureParameterName param, int value)
        {
            Bind();
            GL.TexParameter(TextureTarget, param, value);
        }

        public static RenderableTex FromBitmap(Bitmap bitmap)
        {
            RenderableTex tex = new RenderableTex();
            tex.TextureTarget = TextureTarget.Texture2D;
            tex.TextureWrapS = TextureWrapMode.Repeat;
            tex.TextureWrapT = TextureWrapMode.Repeat;
            tex.TextureMinFilter = TextureMinFilter.Linear;
            tex.TextureMagFilter = TextureMagFilter.Linear;
            tex.width = bitmap.Width;
            tex.height = bitmap.Height;
            tex.pixelInternalFormat = PixelInternalFormat.Rgb8;
            tex.pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
            tex.GLInitialized = true;
            tex.TexID = GenerateOpenGLTexture(tex, bitmap);
            return tex;
        }

        public void UpdateFromBitmap(Bitmap bitmap)
        {
            width = bitmap.Width;
            height = bitmap.Height;
            TexID = GenerateOpenGLTexture(this, bitmap);
        }

        private bool IsFailedState = false;
        private bool UseMipmaps = false;
        private bool UseOpenGLDecoder = true;
        public void LoadOpenGLTexture(STGenericTexture GenericTexture, int ArrayStartIndex = 0, bool LoadArrayLevels = false)
        {
            if (!Runtime.OpenTKInitialized || GLInitialized || Runtime.UseLegacyGL || IsFailedState)
                return;

            width = (int)GenericTexture.Width;
            height = (int)GenericTexture.Height;

            if (Runtime.DisableLoadingGLHighResTextures)
            {
                if (width >= 3000 || height >= 3000)
                    return;
            }

            switch (GenericTexture.SurfaceType)
            {
                case STSurfaceType.Texture1D:
                    TextureTarget = TextureTarget.Texture1D;
                    break;
                case STSurfaceType.Texture2D:
                    TextureTarget = TextureTarget.Texture2D;
                    break;
                case STSurfaceType.Texture2D_Array:
                    TextureTarget = TextureTarget.Texture2DArray;
                    break;
                case STSurfaceType.Texture2D_Mulitsample:
                    TextureTarget = TextureTarget.Texture2DMultisample;
                    break;
                case STSurfaceType.Texture2D_Multisample_Array:
                    TextureTarget = TextureTarget.Texture2DMultisampleArray;
                    break;
                case STSurfaceType.Texture3D:
                    TextureTarget = TextureTarget.Texture3D;
                    break;
                case STSurfaceType.TextureCube:
                    TextureTarget = TextureTarget.TextureCubeMap;
                    break;
                case STSurfaceType.TextureCube_Array:
                    TextureTarget = TextureTarget.TextureCubeMapArray;
                    break;
            }

            if (GenericTexture.ArrayCount == 0)
                GenericTexture.ArrayCount = 1;

            List<STGenericTexture.Surface> Surfaces = new List<STGenericTexture.Surface>();
            try
            {
                if (UseMipmaps && GenericTexture.ArrayCount <= 1)
                {
                    //Load surfaces with mip maps
                    Surfaces = GenericTexture.GetSurfaces(ArrayStartIndex, false, 6);
                }
                else
                {
                    //Only load first mip level. Will be generated after
                    for (int i = 0; i < GenericTexture.ArrayCount; i++)
                    {
                        if (i >= ArrayStartIndex && i <= ArrayStartIndex + 6) //Only load up to 6 faces
                        {
                            Surfaces.Add(new STGenericTexture.Surface()
                            {
                                mipmaps = new List<byte[]>() { GenericTexture.GetImageData(i, 0) }
                            });
                        }
                    }
                }

            if (Surfaces.Count == 0 || Surfaces[0].mipmaps[0].Length == 0)
                return;

            IsCubeMap = Surfaces.Count == 6;
            ImageSize = Surfaces[0].mipmaps[0].Length;

            if (IsCubeMap)
                TextureTarget = TextureTarget.TextureCubeMap;

            //Force RGBA and use ST for decoding for weird width/heights
            //Open GL decoder has issues with certain width/heights

            Console.WriteLine($"width pow {width} {IsPow2(width)}");
            Console.WriteLine($"height pow {height} {IsPow2(height)}");

            if (!IsPow2(width) || !IsPow2(height))
                UseOpenGLDecoder = false;

            pixelInternalFormat = PixelInternalFormat.Rgba;
            pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;

            if (GenericTexture.PlatformSwizzle == PlatformSwizzle.Platform_3DS ||
                GenericTexture.PlatformSwizzle == PlatformSwizzle.Platform_Gamecube)
            {
                UseOpenGLDecoder = false;
                pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
            }


            if (UseOpenGLDecoder)
                  SetPixelFormats(GenericTexture.Format);
 
            GLInitialized = true;
            for (int i = 0; i < Surfaces.Count; i++)
            {
                for (int MipLevel = 0; MipLevel < Surfaces[i].mipmaps.Count; MipLevel++)
                {
                    uint width = Math.Max(1, GenericTexture.Width >> MipLevel);
                    uint height = Math.Max(1, GenericTexture.Height >> MipLevel);

                    Surfaces[i].mipmaps[MipLevel] = DecodeWithoutOpenGLDecoder(Surfaces[i].mipmaps[MipLevel], width, height, GenericTexture);
                }
            }

            TexID = GenerateOpenGLTexture(this, Surfaces);

            if (IsCubeMap)
            {
                TextureWrapS = TextureWrapMode.Clamp;
                TextureWrapT = TextureWrapMode.Clamp;
                TextureWrapR = TextureWrapMode.Clamp;
                TextureMinFilter = TextureMinFilter.LinearMipmapLinear;
                TextureMagFilter = TextureMagFilter.Linear;
            }

            Surfaces.Clear();

            }
            catch
            {
                IsFailedState = true;
                GLInitialized = false;
                return;
            }
        }

        static bool IsPow2(int Value)
        {
            return Value != 0 && (Value & (Value - 1)) == 0;
        }

        private void SetPixelFormats(TEX_FORMAT Format)
        {
            switch (Format)
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
                        pixelInternalFormat = PixelInternalFormat.Rgba;
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    }
                    break;
            }
        }

        private byte[] DecodeWithoutOpenGLDecoder(byte[] ImageData, uint width, uint height, STGenericTexture GenericTexture)
        {
            if (!UseOpenGLDecoder)
            {
                return STGenericTexture.ConvertBgraToRgba(
                     STGenericTexture.DecodeBlock(ImageData,
                 width,
                 height,
                 GenericTexture.Format,
                 GenericTexture.GetPaletteData(),
                 GenericTexture.Parameters,
                 GenericTexture.PaletteFormat,
                 GenericTexture.PlatformSwizzle));
            }

            switch (GenericTexture.Format)
            {
                case TEX_FORMAT.BC1_UNORM:
                case TEX_FORMAT.BC1_UNORM_SRGB:
                case TEX_FORMAT.BC2_UNORM:
                case TEX_FORMAT.BC2_UNORM_SRGB:
                case TEX_FORMAT.BC3_UNORM:
                case TEX_FORMAT.BC3_UNORM_SRGB:
                case TEX_FORMAT.BC5_UNORM:
                case TEX_FORMAT.BC6H_SF16:
                case TEX_FORMAT.BC6H_UF16:
                case TEX_FORMAT.BC7_UNORM:
                case TEX_FORMAT.BC7_UNORM_SRGB:
                case TEX_FORMAT.R8G8B8A8_UNORM:
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB:
                    return ImageData;
                case TEX_FORMAT.BC5_SNORM:
                  return  (DDSCompressor.DecompressBC5(ImageData,
                        (int)width, (int)height, true, true));
                default:
                    if (Runtime.UseDirectXTexDecoder)
                    {
                        return STGenericTexture.ConvertBgraToRgba(
                            STGenericTexture.DecodeBlock(ImageData,
                        width,
                        height,
                        GenericTexture.Format,
                        GenericTexture.GetPaletteData(),
                        GenericTexture.Parameters,
                        GenericTexture.PaletteFormat,
                        GenericTexture.PlatformSwizzle));
                    }
                    else
                        return ImageData;
            }
        }

        public static int GenerateOpenGLTexture(RenderableTex t, Bitmap bitmap, bool generateMips = false)
        {
            if (!t.GLInitialized)
                return -1;

            int texID = GL.GenTexture();
            GL.BindTexture(t.TextureTarget, texID);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            if (generateMips)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        public static int GenerateOpenGLTexture(RenderableTex t, List<STGenericTexture.Surface> ImageData)
        {
            if (!t.GLInitialized)
                return -1;

            int texID = GL.GenTexture();
            GL.BindTexture(t.TextureTarget, texID);
            if (t.IsCubeMap)
            {
                if (t.pixelInternalFormat != PixelInternalFormat.Rgba)
                {
                    for (int mipLevel = 0; mipLevel < ImageData[0].mipmaps.Count; mipLevel++)
                    {
                        int width = Math.Max(1, t.width >> mipLevel);
                        int height = Math.Max(1, t.height >> mipLevel);

                        for (int i = 0; i < 6; i++)
                            t.LoadCompressedMips(TextureTarget.TextureCubeMapPositiveX + i, ImageData[i], width, height, mipLevel);

                        break;
                    }
                }
                else
                {
                    for (int mipLevel = 0; mipLevel < ImageData[0].mipmaps.Count; mipLevel++)
                    {
                        int width = Math.Max(1, t.width >> mipLevel);
                        int height = Math.Max(1, t.height >> mipLevel);

                        for (int i = 0; i < 6; i++)
                            t.LoadUncompressedMips(TextureTarget.TextureCubeMapPositiveX + i, ImageData[i], width, height, mipLevel);

                        break;
                    }
                }

                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                //   if (ImageData[0].mipmaps.Count == 1)
            }
            else
            {
                if (t.pixelInternalFormat != PixelInternalFormat.Rgba)
                {
                    for (int mipLevel = 0; mipLevel < ImageData[0].mipmaps.Count; mipLevel++)
                        t.LoadCompressedMips(t.TextureTarget, ImageData[0], t.width, t.height, mipLevel);
                }
                else
                {
                    for (int mipLevel = 0; mipLevel < ImageData[0].mipmaps.Count; mipLevel++)
                        t.LoadUncompressedMips(t.TextureTarget, ImageData[0], t.width, t.height, mipLevel);
                }

                if (ImageData[0].mipmaps.Count == 1)
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            return texID;
        }

        public void LoadUncompressedMips(TextureTarget textureTarget, STGenericTexture.Surface ImageData,int mipwidth, int mipheight, int MipLevel = 0)
        {
            GL.TexImage2D<byte>(textureTarget, MipLevel, pixelInternalFormat, mipwidth, mipheight, 0,
                     pixelFormat, PixelType.UnsignedByte, ImageData.mipmaps[MipLevel]);

         //   GL.GenerateMipmap((GenerateMipmapTarget)textureTarget);
        }

        public void LoadCompressedMips(TextureTarget textureTarget, STGenericTexture.Surface ImageData, int mipwidth, int mipheight, int MipLevel = 0)
        {
            GL.CompressedTexImage2D<byte>(textureTarget, MipLevel, (InternalFormat)pixelInternalFormat,
                  mipwidth, mipheight, 0, getImageSize(this), ImageData.mipmaps[MipLevel]);

         //  GL.GenerateMipmap((GenerateMipmapTarget)textureTarget);
        }

        public void LoadParameters()
        {

        }

        public void Bind()
        {
            if (TexID == -1) return;

            GL.BindTexture(TextureTarget, TexID);

            Console.WriteLine($"binding {TextureTarget} {TexID}");
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
