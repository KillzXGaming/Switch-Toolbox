/* This file is part of libWiiSharp
 * Copyright (C) 2009 Leathl
 * 
 * libWiiSharp is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * libWiiSharp is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

//TPL conversion based on Wii.py by Xuzz, SquidMan, megazig, Matt_P, Omega and The Lemon Man.
//Zetsubou by SquidMan was also a reference.
//Thanks to the authors!

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace libWiiSharp
{
    #region Enums
    public enum TPL_TextureFormat : int
    {
        /// <summary>
        /// Intensity, 4bpp
        /// </summary>
        I4 = 0,
        /// <summary>
        /// Intensity, 8bpp
        /// </summary>
        I8 = 1,
        /// <summary>
        /// Intensity, Alpha, 8bpp
        /// </summary>
        IA4 = 2,
        /// <summary>
        /// Intensity, Alpha, 16bpp
        /// </summary>
        IA8 = 3,
        /// <summary>
        /// RGB, 16bpp
        /// </summary>
        RGB565 = 4,
        /// <summary>
        /// RGB, Alpha, 16bpp
        /// </summary>
        RGB5A3 = 5,
        /// <summary>
        /// RGB, Alpha, 32bpp
        /// </summary>
        RGBA8 = 6,
        /// <summary>
        /// Indexed, 4bpp
        /// </summary>
        CI4 = 8,
        /// <summary>
        /// Indexed, 8bpp
        /// </summary>
        CI8 = 9,
        /// <summary>
        /// Indexed, 14bpp
        /// </summary>
        CI14X2 = 10,
        /// <summary>
        /// S3TC Compressed
        /// </summary>
        CMP = 14,
    }

    public enum TPL_PaletteFormat : int
    {
        /// <summary>
        /// No Palette
        /// </summary>
        None = 255,
        /// <summary>
        /// Intensity, Alpha, 16bpp
        /// </summary>
        IA8 = 0,
        /// <summary>
        /// RGB, 16bpp
        /// </summary>
        RGB565 = 1,
        /// <summary>
        /// RGB, Alpha, 16bpp
        /// </summary>
        RGB5A3 = 2,
    }
    #endregion

    public class TPL : IDisposable
    {
        private TPL_Header tplHeader = new TPL_Header();
        private List<TPL_TextureEntry> tplTextureEntries = new List<TPL_TextureEntry>();
        private List<TPL_TextureHeader> tplTextureHeaders = new List<TPL_TextureHeader>();
        private List<TPL_PaletteHeader> tplPaletteHeaders = new List<TPL_PaletteHeader>();
        private List<byte[]> textureData = new List<byte[]>();
        private List<byte[]> paletteData = new List<byte[]>();

        /// <summary>
        /// The Number of Textures in the TPL.
        /// </summary>
        public int NumOfTextures { get { return (int)tplHeader.NumOfTextures; } }

        #region IDisposable Members
        private bool isDisposed = false;

        ~TPL()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                tplHeader = null;

                tplTextureEntries.Clear();
                tplTextureEntries = null;

                tplTextureHeaders.Clear();
                tplTextureHeaders = null;

                tplPaletteHeaders.Clear();
                tplPaletteHeaders = null;

                textureData.Clear();
                textureData = null;

                paletteData.Clear();
                paletteData = null;
            }

            isDisposed = true;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Loads a TPL file.
        /// </summary>
        /// <param name="pathToTpl"></param>
        /// <returns></returns>
        public static TPL Load(string pathToTpl)
        {
            TPL tmpTpl = new TPL();

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(pathToTpl));

            try { tmpTpl.parseTpl(ms); }
            catch { ms.Dispose(); throw; }

            ms.Dispose();
            return tmpTpl;
        }

        /// <summary>
        /// Loads a TPL file.
        /// </summary>
        /// <param name="tplFile"></param>
        /// <returns></returns>
        public static TPL Load(byte[] tplFile)
        {
            TPL tmpTpl = new TPL();
            MemoryStream ms = new MemoryStream(tplFile);

            try { tmpTpl.parseTpl(ms); }
            catch { ms.Dispose(); throw; }

            ms.Dispose();
            return tmpTpl;
        }

        /// <summary>
        /// Loads a TPL file.
        /// </summary>
        /// <param name="tpl"></param>
        /// <returns></returns>
        public static TPL Load(Stream tpl)
        {
            TPL t = new TPL();
            t.parseTpl(tpl);
            return t;
        }

        public static Bitmap ToImage(byte[] imageData, int width, int height, TPL_TextureFormat format, byte[] paletteData, int count, TPL_PaletteFormat paletteFormat)
        {
            TPL t = new TPL();
            uint[] paletteRgba = new uint[0];
            byte[] rgba;

            if (paletteData != null)
            {
                var tmpHeader = new TPL_PaletteHeader
                {
                    NumberOfItems = (ushort)count,
                    PaletteFormat = (uint)paletteFormat
                };

                paletteRgba = t.paletteToRgba(tmpHeader, paletteData);
            }

            switch (format)
            {
                case TPL_TextureFormat.I4:
                    rgba = t.fromI4(imageData, width, height);
                    break;
                case TPL_TextureFormat.I8:
                    rgba = t.fromI8(imageData, width, height);
                    break;
                case TPL_TextureFormat.IA4:
                    rgba = t.fromIA4(imageData, width, height);
                    break;
                case TPL_TextureFormat.IA8:
                    rgba = t.fromIA8(imageData, width, height);
                    break;
                case TPL_TextureFormat.RGB565:
                    rgba = t.fromRGB565(imageData, width, height);
                    break;
                case TPL_TextureFormat.RGB5A3:
                    rgba = t.fromRGB5A3(imageData, width, height);
                    break;
                case TPL_TextureFormat.RGBA8:
                    rgba = t.fromRGBA8(imageData, width, height);
                    break;
                case TPL_TextureFormat.CI4:
                    rgba = new byte[0];
                    rgba = t.fromCI4(imageData, paletteRgba, width, height);
                    break;
                case TPL_TextureFormat.CI8:
                    rgba = new byte[0];
                    rgba = t.fromCI8(imageData, paletteRgba, width, height);
                    break;
                case TPL_TextureFormat.CI14X2:
                    rgba = new byte[0];
                    rgba = t.fromCI14X2(imageData, paletteRgba, width, height);
                    break;
                case TPL_TextureFormat.CMP:
                    rgba = t.fromCMP(imageData, width, height);
                    break;
                default:
                    rgba = new byte[0];
                    break;
            }

            return t.rgbaToImage(rgba, width, height);
        }

        /// <summary>
        /// Creates a TPL from an image.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// </summary>
        /// <param name="pathToImage"></param>
        /// <param name="tplFormat"></param>
        /// <returns></returns>
        public static TPL FromImage(string pathToImage, TPL_TextureFormat tplFormat, TPL_PaletteFormat paletteFormat = TPL_PaletteFormat.RGB5A3)
        {
            return FromImages(new string[] { pathToImage }, new TPL_TextureFormat[] { tplFormat }, new TPL_PaletteFormat[] { paletteFormat });
        }

        /// <summary>
        /// Creates a TPL from an image.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// </summary>
        /// <param name="img"></param>
        /// <param name="tplFormat"></param>
        /// <returns></returns>
        public static TPL FromImage(Image img, TPL_TextureFormat tplFormat, TPL_PaletteFormat paletteFormat = TPL_PaletteFormat.RGB5A3)
        {
            return FromImages(new Image[] { img }, new TPL_TextureFormat[] { tplFormat }, new TPL_PaletteFormat[] { paletteFormat });
        }

        /// <summary>
        /// Creates a TPL from multiple images.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// If you use a color indexed format, please provide at least one palette format.
        /// If you use multiple color indexed TPLs, the palette formats must have the same index as the image and tpl format!
        /// </summary>
        /// <param name="imagePaths"></param>
        /// <param name="tplFormats"></param>
        /// <returns></returns>
        public static TPL FromImages(string[] imagePaths, TPL_TextureFormat[] tplFormats, TPL_PaletteFormat[] paletteFormats)
        {
            if (tplFormats.Length < imagePaths.Length)
                throw new Exception("You must specify a format for each image!");

            List<Image> images = new List<Image>();
            foreach (string imagePath in imagePaths)
                images.Add(Image.FromFile(imagePath));

            TPL tmpTpl = new TPL();
            tmpTpl.createFromImages(images.ToArray(), tplFormats, paletteFormats);
            return tmpTpl;
        }

        /// <summary>
        /// Creates a TPL from multiple images.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// If you use a color indexed format, please provide at least one palette format.
        /// If you use multiple color indexed TPLs, the palette formats must have the same index as the image and tpl format!
        /// </summary>
        /// <param name="images"></param>
        /// <param name="tplFormats"></param>
        /// <returns></returns>
        public static TPL FromImages(Image[] images, TPL_TextureFormat[] tplFormats, TPL_PaletteFormat[] paletteFormats)
        {
            if (tplFormats.Length < images.Length)
                throw new Exception("You must specify a format for each image!");

            TPL tmpTpl = new TPL();
            tmpTpl.createFromImages(images, tplFormats, paletteFormats);
            return tmpTpl;
        }



        /// <summary>
        /// Loads a TPL file.
        /// </summary>
        /// <param name="pathToTpl"></param>
        public void LoadFile(string pathToTpl)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(pathToTpl));

            try { parseTpl(ms); }
            catch { ms.Dispose(); throw; }

            ms.Dispose();
        }

        /// <summary>
        /// Loads a TPL file.
        /// </summary>
        /// <param name="tplFile"></param>
        public void LoadFile(byte[] tplFile)
        {
            MemoryStream ms = new MemoryStream(tplFile);

            try { parseTpl(ms); }
            catch { ms.Dispose(); throw; }

            ms.Dispose();
        }

        /// <summary>
        /// Loads a TPL file.
        /// </summary>
        /// <param name="tpl"></param>
        public void LoadFile(Stream tpl)
        {
            parseTpl(tpl);
        }

        /// <summary>
        /// Creates a TPL from an image.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// </summary>
        /// <param name="pathToImage"></param>
        /// <param name="tplFormat"></param>
        public void CreateFromImage(string pathToImage, TPL_TextureFormat tplFormat, TPL_PaletteFormat paletteFormat = TPL_PaletteFormat.RGB5A3)
        {
            CreateFromImages(new string[] { pathToImage }, new TPL_TextureFormat[] { tplFormat }, new TPL_PaletteFormat[] { paletteFormat });
        }

        /// <summary>
        /// Creates a TPL from an image.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// </summary>
        /// <param name="img"></param>
        /// <param name="tplFormat"></param>
        public void CreateFromImage(Image img, TPL_TextureFormat tplFormat, TPL_PaletteFormat paletteFormat = TPL_PaletteFormat.RGB5A3)
        {
            CreateFromImages(new Image[] { img }, new TPL_TextureFormat[] { tplFormat }, new TPL_PaletteFormat[] { paletteFormat });
        }

        /// <summary>
        /// Creates a TPL from multiple images.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// If you use a color indexed format, please provide at least one palette format.
        /// If you use multiple color indexed TPLs, the palette formats must have the same index as the image and tpl format!
        /// </summary>
        /// <param name="imagePaths"></param>
        /// <param name="tplFormats"></param>
        public void CreateFromImages(string[] imagePaths, TPL_TextureFormat[] tplFormats, TPL_PaletteFormat[] paletteFormats)
        {
            if (tplFormats.Length < imagePaths.Length)
                throw new Exception("You must specify a format for each image!");

            List<Image> images = new List<Image>();
            foreach (string imagePath in imagePaths)
                images.Add(Image.FromFile(imagePath));

            createFromImages(images.ToArray(), tplFormats, paletteFormats);
        }

        /// <summary>
        /// Creates a TPL from multiple images.
        /// Palette formats are only required for color indexed TPL formats (CI4, CI8 and CI14X2)!
        /// If you use a color indexed format, please provide at least one palette format.
        /// If you use multiple color indexed TPLs, the palette formats must have the same index as the image and tpl format!
        /// </summary>
        /// <param name="images"></param>
        /// <param name="tplFormats"></param>
        public void CreateFromImages(Image[] images, TPL_TextureFormat[] tplFormats, TPL_PaletteFormat[] paletteFormats)
        {
            if (tplFormats.Length < images.Length)
                throw new Exception("You must specify a format for each image!");

            createFromImages(images, tplFormats, paletteFormats);
        }



        /// <summary>
        /// Saves the TPL.
        /// </summary>
        /// <param name="savePath"></param>
        public void Save(string savePath)
        {
            if (File.Exists(savePath)) File.Delete(savePath);
            FileStream fs = new FileStream(savePath, FileMode.Create);

            try { writeToStream(fs); }
            catch { fs.Dispose(); throw; }

            fs.Dispose();
        }

        /// <summary>
        /// Returns the TPL as a memory stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToMemoryStream()
        {
            MemoryStream ms = new MemoryStream();

            try { writeToStream(ms); }
            catch { ms.Dispose(); throw; }

            return ms;
        }

        /// <summary>
        /// Returns the TPL as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return ToMemoryStream().ToArray();
        }

        /// <summary>
        /// Extracts the first Texture of the TPL.
        /// </summary>
        /// <returns></returns>
        public Image ExtractTexture()
        {
            return ExtractTexture(0);
        }

        /// <summary>
        /// Extracts the Texture with the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Image ExtractTexture(int index)
        {
            byte[] rgbaData;

            switch ((TPL_TextureFormat)tplTextureHeaders[index].TextureFormat)
            {
                case TPL_TextureFormat.I4:
                    rgbaData = fromI4(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.I8:
                    rgbaData = fromI8(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.IA4:
                    rgbaData = fromIA4(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.IA8:
                    rgbaData = fromIA8(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.RGB565:
                    rgbaData = fromRGB565(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.RGB5A3:
                    rgbaData = fromRGB5A3(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.RGBA8:
                    rgbaData = fromRGBA8(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CI4:
                    rgbaData = fromCI4(textureData[index], paletteToRgba(index), tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CI8:
                    rgbaData = fromCI8(textureData[index], paletteToRgba(index), tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CI14X2:
                    rgbaData = fromCI14X2(textureData[index], paletteToRgba(index), tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CMP:
                    rgbaData = fromCMP(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                default:
                    throw new FormatException("Unsupported Texture Format!");
            }

            return rgbaToImage(rgbaData, tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
        }

        public ushort GetImageWidth(int index)
        {
           return tplTextureHeaders[index].TextureWidth;
        }

        public ushort GetImageHeight(int index)
        {
            return tplTextureHeaders[index].TextureHeight;
        }

        public byte[] ExtractTextureByteArray(int index)
        {
            byte[] rgbaData;

            switch ((TPL_TextureFormat)tplTextureHeaders[index].TextureFormat)
            {
                case TPL_TextureFormat.I4:
                    rgbaData = fromI4(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.I8:
                    rgbaData = fromI8(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.IA4:
                    rgbaData = fromIA4(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.IA8:
                    rgbaData = fromIA8(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.RGB565:
                    rgbaData = fromRGB565(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.RGB5A3:
                    rgbaData = fromRGB5A3(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.RGBA8:
                    rgbaData = fromRGBA8(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CI4:
                    rgbaData = fromCI4(textureData[index], paletteToRgba(index), tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CI8:
                    rgbaData = fromCI8(textureData[index], paletteToRgba(index), tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CI14X2:
                    rgbaData = fromCI14X2(textureData[index], paletteToRgba(index), tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                case TPL_TextureFormat.CMP:
                    rgbaData = fromCMP(textureData[index], tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
                    break;
                default:
                    throw new FormatException("Unsupported Texture Format!");
            }

            return rgbaData;
        }

        /// <summary>
        /// Extracts the first Texture of the TPL.
        /// </summary>
        /// <param name="savePath"></param>
        public void ExtractTexture(string savePath)
        {
            ExtractTexture(0, savePath);
        }

        /// <summary>
        /// Extracts the Texture with the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="savePath"></param>
        public void ExtractTexture(int index, string savePath)
        {
            if (File.Exists(savePath)) File.Delete(savePath);
            Image img = ExtractTexture(index);

            switch (Path.GetExtension(savePath).ToLower())
            {
                case ".tif":
                case ".tiff":
                    img.Save(savePath, System.Drawing.Imaging.ImageFormat.Tiff);
                    break;
                case ".bmp":
                    img.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case ".gif":
                    img.Save(savePath, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case ".jpg":
                case ".jpeg":
                    img.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                default:
                    img.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
                    break;
            }
        }

        /// <summary>
        /// Extracts all Textures of the TPL.
        /// </summary>
        /// <returns></returns>
        public Image[] ExtractAllTextures()
        {
            List<Image> imgList = new List<Image>();

            for (int i = 0; i < tplHeader.NumOfTextures; i++)
                imgList.Add(ExtractTexture(i));

            return imgList.ToArray();
        }

        /// <summary>
        /// Extracts all Textures of the TPL.
        /// </summary>
        /// <param name="saveDir"></param>
        public void ExtractAllTextures(string saveDir)
        {
            if (Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

            for (int i = 0; i < tplHeader.NumOfTextures; i++)
                ExtractTexture(i, saveDir + Path.DirectorySeparatorChar + "Texture_" + i.ToString("x2") + ".png");
        }

        /// <summary>
        /// Adds a Texture to the TPL.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="tplFormat"></param>
        public void AddTexture(string imagePath, TPL_TextureFormat tplFormat, TPL_PaletteFormat paletteFormat = TPL_PaletteFormat.RGB5A3)
        {
            Image img = Image.FromFile(imagePath);
            AddTexture(img, tplFormat, paletteFormat);
        }

        /// <summary>
        /// Adds a Texture to the TPL.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="tplFormat"></param>
        public void AddTexture(Image img, TPL_TextureFormat tplFormat, TPL_PaletteFormat paletteFormat = TPL_PaletteFormat.RGB5A3)
        {
            TPL_TextureEntry tempTexture = new TPL_TextureEntry();
            TPL_TextureHeader tempTextureHeader = new TPL_TextureHeader();
            TPL_PaletteHeader tempPaletteHeader = new TPL_PaletteHeader();
            byte[] tempTextureData = imageToTpl(img, tplFormat);
            byte[] tempPaletteData = new byte[0];

            tempTextureHeader.TextureHeight = (ushort)img.Height;
            tempTextureHeader.TextureWidth = (ushort)img.Width;
            tempTextureHeader.TextureFormat = (uint)tplFormat;

            if (tplFormat == TPL_TextureFormat.CI4 || tplFormat == TPL_TextureFormat.CI8 || tplFormat == TPL_TextureFormat.CI14X2)
            {
                ColorIndexConverter cic = new ColorIndexConverter(imageToRgba(img), img.Width, img.Height, tplFormat, paletteFormat);

                tempTextureData = cic.Data;
                tempPaletteData = cic.Palette;

                tempPaletteHeader.NumberOfItems = (ushort)(tempPaletteData.Length / 2);
                tempPaletteHeader.PaletteFormat = (uint)paletteFormat;
            }

            tplTextureEntries.Add(tempTexture);
            tplTextureHeaders.Add(tempTextureHeader);
            tplPaletteHeaders.Add(tempPaletteHeader);
            textureData.Add(tempTextureData);
            paletteData.Add(tempPaletteData);

            tplHeader.NumOfTextures++;
        }

        /// <summary>
        /// Removes a Texture from the TPL.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveTexture(int index)
        {
            if (tplHeader.NumOfTextures > index)
            {
                tplTextureEntries.RemoveAt(index);
                tplTextureHeaders.RemoveAt(index);
                tplPaletteHeaders.RemoveAt(index);
                textureData.RemoveAt(index);
                paletteData.RemoveAt(index);

                tplHeader.NumOfTextures--;
            }
        }

        /// <summary>
        /// Gets the format of the Texture with the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TPL_TextureFormat GetTextureFormat(int index)
        {
            return (TPL_TextureFormat)tplTextureHeaders[index].TextureFormat;
        }

        /// <summary>
        /// Gets the palette format of the Texture with the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TPL_PaletteFormat GetPaletteFormat(int index)
        {
            return (TPL_PaletteFormat)tplPaletteHeaders[index].PaletteFormat;
        }

        /// <summary>
        /// Gets the size of the Texture with the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Size GetTextureSize(int index)
        {
            return new Size(tplTextureHeaders[index].TextureWidth, tplTextureHeaders[index].TextureHeight);
        }
        #endregion

        #region Private Functions
        private void writeToStream(Stream writeStream)
        {
            fireDebug("Writing TPL...");

            writeStream.Seek(0, SeekOrigin.Begin);

            fireDebug("   Writing TPL Header... (Offset: 0x{0})", writeStream.Position);
            tplHeader.Write(writeStream);

            int textureEntriesPosition = (int)writeStream.Position;
            writeStream.Seek(tplHeader.NumOfTextures * 8, SeekOrigin.Current);

            //Get Number of Palettes
            int paletteCount = 0;
            for (int i = 0; i < tplHeader.NumOfTextures; i++)
                if ((TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI4 ||
                    (TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI8 ||
                    (TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI14X2)
                    paletteCount++;

            int paletteHeaderPosition = (int)writeStream.Position;
            writeStream.Seek(paletteCount * 12, SeekOrigin.Current);

            //Write Palette Data
            for (int i = 0; i < tplHeader.NumOfTextures; i++)
            {
                if ((TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI4 ||
                    (TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI8 ||
                    (TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI14X2)
                {
                    fireDebug("   Writing Palette of Texture #{1}... (Offset: 0x{0})", writeStream.Position, i + 1);

                    writeStream.Seek(Shared.AddPadding(writeStream.Position, 32), SeekOrigin.Begin);

                    tplPaletteHeaders[i].PaletteDataOffset = (uint)writeStream.Position;
                    writeStream.Write(paletteData[i], 0, paletteData[i].Length);
                }
            }

            int textureHeaderPosition = (int)writeStream.Position;
            writeStream.Seek(tplHeader.NumOfTextures * 36, SeekOrigin.Current);

            //Write textureData
            for (int i = 0; i < tplHeader.NumOfTextures; i++)
            {
                fireDebug("   Writing Texture #{1} of {2}... (Offset: 0x{0})", writeStream.Position, i + 1, tplHeader.NumOfTextures);

                writeStream.Seek(Shared.AddPadding((int)writeStream.Position, 32), SeekOrigin.Begin);

                tplTextureHeaders[i].TextureDataOffset = (uint)writeStream.Position;
                writeStream.Write(textureData[i], 0, textureData[i].Length);
            }

            while (writeStream.Position % 32 != 0)
                writeStream.WriteByte(0x00);

            writeStream.Seek(paletteHeaderPosition, SeekOrigin.Begin);

            //Write Palette Headers
            for (int i = 0; i < tplHeader.NumOfTextures; i++)
            {
                if ((TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI4 ||
                    (TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI8 ||
                    (TPL_TextureFormat)tplTextureHeaders[i].TextureFormat == TPL_TextureFormat.CI14X2)
                {
                    fireDebug("   Writing Palette Header of Texture #{1}... (Offset: 0x{0})", writeStream.Position, i + 1);

                    tplTextureEntries[i].PaletteHeaderOffset = (uint)writeStream.Position;
                    tplPaletteHeaders[i].Write(writeStream);
                }
            }

            writeStream.Seek(textureHeaderPosition, SeekOrigin.Begin);

            //Write Texture Headers
            for (int i = 0; i < tplHeader.NumOfTextures; i++)
            {
                fireDebug("   Writing Texture Header #{1} of {2}... (Offset: 0x{0})", writeStream.Position, i + 1, tplHeader.NumOfTextures);

                tplTextureEntries[i].TextureHeaderOffset = (uint)writeStream.Position;
                tplTextureHeaders[i].Write(writeStream);
            }

            writeStream.Seek(textureEntriesPosition, SeekOrigin.Begin);

            //Write Texture Entries
            for (int i = 0; i < tplHeader.NumOfTextures; i++)
            {
                fireDebug("   Writing Texture Entry #{1} of {2}... (Offset: 0x{0})", writeStream.Position, i + 1, tplHeader.NumOfTextures);

                tplTextureEntries[i].Write(writeStream);
            }

            fireDebug("Writing TPL Finished...");
        }

        private void parseTpl(Stream tplFile)
        {
            fireDebug("Parsing TPL...");

            tplHeader = new TPL_Header();
            tplTextureEntries = new List<TPL_TextureEntry>();
            tplTextureHeaders = new List<TPL_TextureHeader>();
            tplPaletteHeaders = new List<TPL_PaletteHeader>();
            textureData = new List<byte[]>();
            paletteData = new List<byte[]>();

            tplFile.Seek(0, SeekOrigin.Begin);
            byte[] temp = new byte[4];

            fireDebug("   Reading TPL Header: Magic... (Offset: 0x{0})", tplFile.Position);
            tplFile.Read(temp, 0, 4);
            if (Shared.Swap(BitConverter.ToUInt32(temp, 0)) != tplHeader.TplMagic)
            {
                fireDebug("    -> Invalid Magic: 0x{0}", Shared.Swap(BitConverter.ToUInt32(temp, 0)));
                throw new Exception("TPL Header: Invalid Magic!");
            }

            fireDebug("   Reading TPL Header: NumOfTextures... (Offset: 0x{0})", tplFile.Position);
            tplFile.Read(temp, 0, 4);
            tplHeader.NumOfTextures = Shared.Swap(BitConverter.ToUInt32(temp, 0));

            fireDebug("   Reading TPL Header: Headersize... (Offset: 0x{0})", tplFile.Position);
            tplFile.Read(temp, 0, 4);
            if (Shared.Swap(BitConverter.ToUInt32(temp, 0)) != tplHeader.HeaderSize)
            {
                fireDebug("    -> Invalid Headersize: 0x{0}", Shared.Swap(BitConverter.ToUInt32(temp, 0)));
                throw new Exception("TPL Header: Invalid Headersize!");
            }

            for (int i = 0; i < tplHeader.NumOfTextures; i++)
            {
                fireDebug("   Reading Texture Entry #{1} of {2}... (Offset: 0x{0})", tplFile.Position, i + 1, tplHeader.NumOfTextures);

                TPL_TextureEntry tempTexture = new TPL_TextureEntry();

                tplFile.Read(temp, 0, 4);
                tempTexture.TextureHeaderOffset = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTexture.PaletteHeaderOffset = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplTextureEntries.Add(tempTexture);
            }

            for (int i = 0; i < tplHeader.NumOfTextures; i++)
            {
                fireDebug("   Reading Texture Header #{1} of {2}... (Offset: 0x{0})", tplFile.Position, i + 1, tplHeader.NumOfTextures);

                TPL_TextureHeader tempTextureHeader = new TPL_TextureHeader();
                TPL_PaletteHeader tempPaletteHeader = new TPL_PaletteHeader();
                tplFile.Seek(tplTextureEntries[i].TextureHeaderOffset, SeekOrigin.Begin);

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.TextureHeight = Shared.Swap(BitConverter.ToUInt16(temp, 0));
                tempTextureHeader.TextureWidth = Shared.Swap(BitConverter.ToUInt16(temp, 2));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.TextureFormat = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.TextureDataOffset = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.WrapS = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.WrapT = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.MinFilter = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.MagFilter = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.LodBias = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                tplFile.Read(temp, 0, 4);
                tempTextureHeader.EdgeLod = temp[0];
                tempTextureHeader.MinLod = temp[1];
                tempTextureHeader.MaxLod = temp[2];
                tempTextureHeader.Unpacked = temp[3];

                if (tplTextureEntries[i].PaletteHeaderOffset != 0)
                {
                    fireDebug("   Reading Palette Header #{1} of {2}... (Offset: 0x{0})", tplFile.Position, i + 1, tplHeader.NumOfTextures);

                    tplFile.Seek(tplTextureEntries[i].PaletteHeaderOffset, SeekOrigin.Begin);

                    tplFile.Read(temp, 0, 4);
                    tempPaletteHeader.NumberOfItems = Shared.Swap(BitConverter.ToUInt16(temp, 0));
                    tempPaletteHeader.Unpacked = temp[2];
                    tempPaletteHeader.Pad = temp[3];

                    tplFile.Read(temp, 0, 4);
                    tempPaletteHeader.PaletteFormat = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                    tplFile.Read(temp, 0, 4);
                    tempPaletteHeader.PaletteDataOffset = Shared.Swap(BitConverter.ToUInt32(temp, 0));
                }

                tplFile.Seek(tempTextureHeader.TextureDataOffset, SeekOrigin.Begin);
                byte[] tempTextureData = new byte[textureByteSize((TPL_TextureFormat)tempTextureHeader.TextureFormat, tempTextureHeader.TextureWidth, tempTextureHeader.TextureHeight)];
                byte[] tempPaletteData = new byte[tempPaletteHeader.NumberOfItems * 2];

                fireDebug("   Reading Texture #{1} of {2}... (Offset: 0x{0})", tplFile.Position, i + 1, tplHeader.NumOfTextures);
                tplFile.Read(tempTextureData, 0, tempTextureData.Length);

                if (tplTextureEntries[i].PaletteHeaderOffset > 0)
                {
                    fireDebug("   Reading Palette #{1} of {2}... (Offset: 0x{0})", tplFile.Position, i + 1, tplHeader.NumOfTextures);

                    tplFile.Seek(tempPaletteHeader.PaletteDataOffset, SeekOrigin.Begin);
                    tplFile.Read(tempPaletteData, 0, tempPaletteData.Length);
                }
                else tempPaletteData = new byte[0];

                tplTextureHeaders.Add(tempTextureHeader);
                tplPaletteHeaders.Add(tempPaletteHeader);
                textureData.Add(tempTextureData);
                paletteData.Add(tempPaletteData);
            }
        }

        private int textureByteSize(TPL_TextureFormat tplFormat, int width, int height)
        {
            switch (tplFormat)
            {
                case TPL_TextureFormat.I4:
                    return Shared.AddPadding(width, 8) * Shared.AddPadding(height, 8) / 2;
                case TPL_TextureFormat.I8:
                case TPL_TextureFormat.IA4:
                    return Shared.AddPadding(width, 8) * Shared.AddPadding(height, 4);
                case TPL_TextureFormat.IA8:
                case TPL_TextureFormat.RGB565:
                case TPL_TextureFormat.RGB5A3:
                    return Shared.AddPadding(width, 4) * Shared.AddPadding(height, 4) * 2;
                case TPL_TextureFormat.RGBA8:
                    return Shared.AddPadding(width, 4) * Shared.AddPadding(height, 4) * 4;
                case TPL_TextureFormat.CI4:
                    return Shared.AddPadding(width, 8) * Shared.AddPadding(height, 8) / 2;
                case TPL_TextureFormat.CI8:
                    return Shared.AddPadding(width, 8) * Shared.AddPadding(height, 4);
                case TPL_TextureFormat.CI14X2:
                    return Shared.AddPadding(width, 4) * Shared.AddPadding(height, 4) * 2;
                case TPL_TextureFormat.CMP:
                    return width * height;
                default:
                    throw new FormatException("Unsupported Texture Format!");
            }
        }

        private void createFromImages(Image[] images, TPL_TextureFormat[] tplFormats, TPL_PaletteFormat[] paletteFormats)
        {
            tplHeader = new TPL_Header();
            tplTextureEntries = new List<TPL_TextureEntry>();
            tplTextureHeaders = new List<TPL_TextureHeader>();
            tplPaletteHeaders = new List<TPL_PaletteHeader>();
            textureData = new List<byte[]>();
            paletteData = new List<byte[]>();

            tplHeader.NumOfTextures = (uint)images.Length;

            for (int i = 0; i < images.Length; i++)
            {
                Image img = images[i];

                TPL_TextureEntry tempTexture = new TPL_TextureEntry();
                TPL_TextureHeader tempTextureHeader = new TPL_TextureHeader();
                TPL_PaletteHeader tempPaletteHeader = new TPL_PaletteHeader();
                byte[] tempTextureData = imageToTpl(img, tplFormats[i]);
                byte[] tempPaletteData = new byte[0];

                tempTextureHeader.TextureHeight = (ushort)img.Height;
                tempTextureHeader.TextureWidth = (ushort)img.Width;
                tempTextureHeader.TextureFormat = (uint)tplFormats[i];

                if (tplFormats[i] == TPL_TextureFormat.CI4 || tplFormats[i] == TPL_TextureFormat.CI8 || tplFormats[i] == TPL_TextureFormat.CI14X2)
                {
                    ColorIndexConverter cic = new ColorIndexConverter(imageToRgba(img), img.Width, img.Height, tplFormats[i], paletteFormats[i]);

                    tempTextureData = cic.Data;
                    tempPaletteData = cic.Palette;

                    tempPaletteHeader.NumberOfItems = (ushort)(tempPaletteData.Length / 2);
                    tempPaletteHeader.PaletteFormat = (uint)paletteFormats[i];
                }

                tplTextureEntries.Add(tempTexture);
                tplTextureHeaders.Add(tempTextureHeader);
                tplPaletteHeaders.Add(tempPaletteHeader);
                textureData.Add(tempTextureData);
                paletteData.Add(tempPaletteData);
            }
        }

        private byte[] imageToTpl(Image img, TPL_TextureFormat tplFormat)
        {
            switch (tplFormat)
            {
                case TPL_TextureFormat.I4:
                    return toI4((Bitmap)img);
                case TPL_TextureFormat.I8:
                    return toI8((Bitmap)img);
                case TPL_TextureFormat.IA4:
                    return toIA4((Bitmap)img);
                case TPL_TextureFormat.IA8:
                    return toIA8((Bitmap)img);
                case TPL_TextureFormat.RGB565:
                    return toRGB565((Bitmap)img);
                case TPL_TextureFormat.RGB5A3:
                    return toRGB5A3((Bitmap)img);
                case TPL_TextureFormat.RGBA8:
                    return toRGBA8((Bitmap)img);
                case TPL_TextureFormat.CI4:
                case TPL_TextureFormat.CI8:
                case TPL_TextureFormat.CI14X2:
                    return new byte[0];
                case TPL_TextureFormat.CMP:
                default:
                    throw new FormatException("Format not supported!\nCurrently, images can only be converted to the following formats:\nI4, I8, IA4, IA8, RGB565, RGB5A3, RGBA8, CI4, CI8 , CI14X2.");
            }
        }

        private uint[] imageToRgba(Image img)
        {
            Bitmap bmp = (Bitmap)img;
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] pixelData = new byte[bmpData.Height * (int)Math.Abs(bmpData.Stride)];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelData, 0, pixelData.Length);

            bmp.UnlockBits(bmpData);
            return Shared.ByteArrayToUIntArray(pixelData);
        }

        private Bitmap rgbaToImage(byte[] data, int width, int height)
        {
            if (width == 0) width = 1;
            if (height == 0) height = 1;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(
                                     new Rectangle(0, 0, bmp.Width, bmp.Height),
                                     System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);

                System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
                bmp.UnlockBits(bmpData);
            }
            catch { bmp.Dispose(); throw; }

            return bmp;
        }

        private uint[] paletteToRgba(int index)
        {
            TPL_PaletteFormat paletteformat = (TPL_PaletteFormat)tplPaletteHeaders[index].PaletteFormat;
            int itemcount = tplPaletteHeaders[index].NumberOfItems;
            int r, g, b, a;

            uint[] output = new uint[itemcount];
            for (int i = 0; i < itemcount; i++)
            {
                if (i >= itemcount) continue;

                ushort pixel = BitConverter.ToUInt16(new byte[] { paletteData[index][i * 2 + 1], paletteData[index][i * 2] }, 0);

                if (paletteformat == TPL_PaletteFormat.IA8) //IA8
                {
                    r = pixel & 0xff;
                    b = r;
                    g = r;
                    a = pixel >> 8;
                }
                else if (paletteformat == TPL_PaletteFormat.RGB565) //RGB565
                {
                    b = (((pixel >> 11) & 0x1F) << 3) & 0xff;
                    g = (((pixel >> 5) & 0x3F) << 2) & 0xff;
                    r = (((pixel >> 0) & 0x1F) << 3) & 0xff;
                    a = 255;
                }
                else //RGB5A3
                {
                    if ((pixel & (1 << 15)) != 0) //RGB555
                    {
                        a = 255;
                        b = (((pixel >> 10) & 0x1F) * 255) / 31;
                        g = (((pixel >> 5) & 0x1F) * 255) / 31;
                        r = (((pixel >> 0) & 0x1F) * 255) / 31;
                    }
                    else //RGB4A3
                    {
                        a = (((pixel >> 12) & 0x07) * 255) / 7;
                        b = (((pixel >> 8) & 0x0F) * 255) / 15;
                        g = (((pixel >> 4) & 0x0F) * 255) / 15;
                        r = (((pixel >> 0) & 0x0F) * 255) / 15;
                    }
                }

                output[i] = (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));
            }

            return output;
        }

        private uint[] paletteToRgba(TPL_PaletteHeader header, byte[] data)
        {
            TPL_PaletteFormat paletteformat = (TPL_PaletteFormat)header.PaletteFormat;
            int itemcount = header.NumberOfItems;
            int r, g, b, a;

            uint[] output = new uint[itemcount];
            for (int i = 0; i < itemcount; i++)
            {
                if (i >= itemcount) continue;

                ushort pixel = BitConverter.ToUInt16(new byte[] { data[i * 2 + 1], data[i * 2] }, 0);

                if (paletteformat == TPL_PaletteFormat.IA8) //IA8
                {
                    r = pixel & 0xff;
                    b = r;
                    g = r;
                    a = pixel >> 8;
                }
                else if (paletteformat == TPL_PaletteFormat.RGB565) //RGB565
                {
                    b = (((pixel >> 11) & 0x1F) << 3) & 0xff;
                    g = (((pixel >> 5) & 0x3F) << 2) & 0xff;
                    r = (((pixel >> 0) & 0x1F) << 3) & 0xff;
                    a = 255;
                }
                else //RGB5A3
                {
                    if ((pixel & (1 << 15)) != 0) //RGB555
                    {
                        a = 255;
                        b = (((pixel >> 10) & 0x1F) * 255) / 31;
                        g = (((pixel >> 5) & 0x1F) * 255) / 31;
                        r = (((pixel >> 0) & 0x1F) * 255) / 31;
                    }
                    else //RGB4A3
                    {
                        a = (((pixel >> 12) & 0x07) * 255) / 7;
                        b = (((pixel >> 8) & 0x0F) * 255) / 15;
                        g = (((pixel >> 4) & 0x0F) * 255) / 15;
                        r = (((pixel >> 0) & 0x0F) * 255) / 15;
                    }
                }

                output[i] = (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));
            }

            return output;
        }

        private int avg(int w0, int w1, int c0, int c1)
        {
            int a0 = c0 >> 11;
            int a1 = c1 >> 11;
            int a = (w0 * a0 + w1 * a1) / (w0 + w1);
            int c = (a << 11) & 0xffff;

            a0 = (c0 >> 5) & 63;
            a1 = (c1 >> 5) & 63;
            a = (w0 * a0 + w1 * a1) / (w0 + w1);
            c = c | ((a << 5) & 0xffff);

            a0 = c0 & 31;
            a1 = c1 & 31;
            a = (w0 * a0 + w1 * a1) / (w0 + w1);
            c = c | a;

            return c;
        }
        #endregion

        #region Conversions
        #region RGBA8
        private byte[] fromRGBA8(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int y1 = y; y1 < y + 4; y1++)
                        {
                            for (int x1 = x; x1 < x + 4; x1++)
                            {
                                ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                                if ((x1 >= width) || (y1 >= height))
                                    continue;

                                if (k == 0)
                                {
                                    int a = (pixel >> 8) & 0xff;
                                    int r = (pixel >> 0) & 0xff;
                                    output[x1 + (y1 * width)] |= (uint)((r << 16) | (a << 24));
                                }
                                else
                                {
                                    int g = (pixel >> 8) & 0xff;
                                    int b = (pixel >> 0) & 0xff;
                                    output[x1 + (y1 * width)] |= (uint)((g << 8) | (b << 0));
                                }
                            }
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private byte[] toRGBA8(Bitmap img)
        {
            uint[] pixeldata = imageToRgba(img);
            int w = img.Width;
            int h = img.Height;
            int z = 0, iv = 0;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 4];
            uint[] lr = new uint[32], lg = new uint[32], lb = new uint[32], la = new uint[32];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < (y1 + 4); y++)
                    {
                        for (int x = x1; x < (x1 + 4); x++)
                        {
                            uint rgba;

                            if (y >= h || x >= w)
                                rgba = 0;
                            else
                                rgba = pixeldata[x + (y * w)];

                            lr[z] = (uint)(rgba >> 16) & 0xff;
                            lg[z] = (uint)(rgba >> 8) & 0xff;
                            lb[z] = (uint)(rgba >> 0) & 0xff;
                            la[z] = (uint)(rgba >> 24) & 0xff;

                            z++;
                        }
                    }

                    if (z == 16)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            output[iv++] = (byte)(la[i]);
                            output[iv++] = (byte)(lr[i]);
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            output[iv++] = (byte)(lg[i]);
                            output[iv++] = (byte)(lb[i]);
                        }

                        z = 0;
                    }
                }
            }

            return output;
        }
        #endregion

        #region RGB5A3
        private byte[] fromRGB5A3(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;
            int r, g, b;
            int a = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            if ((pixel & (1 << 15)) != 0)
                            {
                                b = (((pixel >> 10) & 0x1F) * 255) / 31;
                                g = (((pixel >> 5) & 0x1F) * 255) / 31;
                                r = (((pixel >> 0) & 0x1F) * 255) / 31;
                                a = 255;
                            }
                            else
                            {
                                a = (((pixel >> 12) & 0x07) * 255) / 7;
                                b = (((pixel >> 8) & 0x0F) * 255) / 15;
                                g = (((pixel >> 4) & 0x0F) * 255) / 15;
                                r = (((pixel >> 0) & 0x0F) * 255) / 15;
                            }

                            output[(y1 * width) + x1] = (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private byte[] toRGB5A3(Bitmap img)
        {
            uint[] pixeldata = imageToRgba(img);
            int w = img.Width;
            int h = img.Height;
            int z = -1;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 2];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 4; x++)
                        {
                            int newpixel;

                            if (y >= h || x >= w)
                                newpixel = 0;
                            else
                            {
                                int rgba = (int)pixeldata[x + (y * w)];
                                newpixel = 0;

                                int r = (rgba >> 16) & 0xff;
                                int g = (rgba >> 8) & 0xff;
                                int b = (rgba >> 0) & 0xff;
                                int a = (rgba >> 24) & 0xff;

                                if (a <= 0xda) //RGB4A3
                                {
                                    newpixel &= ~(1 << 15);

                                    r = ((r * 15) / 255) & 0xf;
                                    g = ((g * 15) / 255) & 0xf;
                                    b = ((b * 15) / 255) & 0xf;
                                    a = ((a * 7) / 255) & 0x7;

                                    newpixel |= (a << 12) | (r << 8) | (g << 4) | b;
                                }
                                else //RGB5
                                {
                                    newpixel |= (1 << 15);

                                    r = ((r * 31) / 255) & 0x1f;
                                    g = ((g * 31) / 255) & 0x1f;
                                    b = ((b * 31) / 255) & 0x1f;

                                    newpixel |= (r << 10) | (g << 5) | b;
                                }
                            }

                            output[++z] = (byte)(newpixel >> 8);
                            output[++z] = (byte)(newpixel & 0xff);
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region RGB565
        private byte[] fromRGB565(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            int b = (((pixel >> 11) & 0x1F) << 3) & 0xff;
                            int g = (((pixel >> 5) & 0x3F) << 2) & 0xff;
                            int r = (((pixel >> 0) & 0x1F) << 3) & 0xff;

                            output[y1 * width + x1] = (uint)((r << 0) | (g << 8) | (b << 16) | (255 << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private byte[] toRGB565(Bitmap img)
        {
            uint[] pixeldata = imageToRgba(img);
            int w = img.Width;
            int h = img.Height;
            int z = -1;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 2];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 4; x++)
                        {
                            ushort newpixel;

                            if (y >= h || x >= w)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint b = (rgba >> 16) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint r = (rgba >> 0) & 0xff;

                                newpixel = (ushort)(((b >> 3) << 11) | ((g >> 2) << 5) | ((r >> 3) << 0));
                            }

                            output[++z] = (byte)(newpixel >> 8);
                            output[++z] = (byte)(newpixel & 0xff);
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region I4
        private byte[] fromI4(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 8; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1 += 2)
                        {
                            int pixel = tpl[inp++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            int i = (pixel >> 4) * 255 / 15;
                            output[y1 * width + x1] = (uint)((i << 0) | (i << 8) | (i << 16) | (255 << 24));

                            i = (pixel & 0x0F) * 255 / 15;
                            if (y1 * width + x1 + 1 < output.Length) output[y1 * width + x1 + 1] = (uint)((i << 0) | (i << 8) | (i << 16) | (255 << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private byte[] toI4(Bitmap img)
        {
            uint[] pixeldata = imageToRgba(img);
            int w = img.Width;
            int h = img.Height;
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 8) * Shared.AddPadding(h, 8) / 2];

            for (int y1 = 0; y1 < h; y1 += 8)
            {
                for (int x1 = 0; x1 < w; x1 += 8)
                {
                    for (int y = y1; y < y1 + 8; y++)
                    {
                        for (int x = x1; x < x1 + 8; x += 2)
                        {
                            byte newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                uint i1 = ((r + g + b) / 3) & 0xff;

                                if ((x + (y * w) + 1) >= pixeldata.Length) rgba = 0;
                                else rgba = pixeldata[x + (y * w) + 1];

                                r = (rgba >> 0) & 0xff;
                                g = (rgba >> 8) & 0xff;
                                b = (rgba >> 16) & 0xff;

                                uint i2 = ((r + g + b) / 3) & 0xff;

                                newpixel = (byte)((((i1 * 15) / 255) << 4) | (((i2 * 15) / 255) & 0xf));
                            }

                            output[inp++] = newpixel;
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region I8
        private byte[] fromI8(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            int pixel = tpl[inp++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            output[y1 * width + x1] = (uint)((pixel << 0) | (pixel << 8) | (pixel << 16) | (255 << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private byte[] toI8(Bitmap img)
        {
            uint[] pixeldata = imageToRgba(img);
            int w = img.Width;
            int h = img.Height;
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 8) * Shared.AddPadding(h, 4)];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 8)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 8; x++)
                        {
                            byte newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                newpixel = (byte)(((r + g + b) / 3) & 0xff);
                            }

                            output[inp++] = newpixel;
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region IA4
        private byte[] fromIA4(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            int pixel = tpl[inp++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            int i = ((pixel & 0x0F) * 255 / 15) & 0xff;
                            int a = (((pixel >> 4) * 255) / 15) & 0xff;

                            output[y1 * width + x1] = (uint)((i << 0) | (i << 8) | (i << 16) | (a << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private byte[] toIA4(Bitmap img)
        {
            uint[] pixeldata = imageToRgba(img);
            int w = img.Width;
            int h = img.Height;
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 8) * Shared.AddPadding(h, 4)];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 8)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 8; x++)
                        {
                            byte newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                uint i = ((r + g + b) / 3) & 0xff;
                                uint a = (rgba >> 24) & 0xff;

                                newpixel = (byte)((((i * 15) / 255) & 0xf) | (((a * 15) / 255) << 4));
                            }

                            output[inp++] = newpixel;
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region IA8
        private byte[] fromIA8(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            int pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            uint a = (uint)(pixel >> 8);
                            uint i = (uint)(pixel & 0xff);

                            output[y1 * width + x1] = (i << 0) | (i << 8) | (i << 16) | (a << 24);
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private byte[] toIA8(Bitmap img)
        {
            uint[] pixeldata = imageToRgba(img);
            int w = img.Width;
            int h = img.Height;
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 2];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 4; x++)
                        {
                            ushort newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                uint i = ((r + g + b) / 3) & 0xff;
                                uint a = (rgba >> 24) & 0xff;

                                newpixel = (ushort)((a << 8) | i);
                            }

                            byte[] temp = BitConverter.GetBytes(newpixel);
                            Array.Reverse(temp);

                            output[inp++] = (byte)(newpixel >> 8);
                            output[inp++] = (byte)(newpixel & 0xff);
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region CI4
        private byte[] fromCI4(byte[] tpl, uint[] paletteData, int width, int height)
        {
            uint[] output = new uint[width * height];
            int i = 0;

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 8; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1 += 2)
                        {
                            byte pixel = tpl[i++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            output[y1 * width + x1] = paletteData[pixel >> 4]; ;
                            if (y1 * width + x1 + 1 < output.Length) output[y1 * width + x1 + 1] = paletteData[pixel & 0x0F];
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        //toCI4 done in class ColorIndexConverter
        #endregion

        #region CI8
        private byte[] fromCI8(byte[] tpl, uint[] paletteData, int width, int height)
        {
            uint[] output = new uint[width * height];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            ushort pixel = tpl[i++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            output[y1 * width + x1] = paletteData[pixel];
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        //toCI8 done in class ColorIndexConverter
        #endregion

        #region CI14X2
        private byte[] fromCI14X2(byte[] tpl, uint[] paletteData, int width, int height)
        {
            uint[] output = new uint[width * height];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, i++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            output[y1 * width + x1] = paletteData[pixel & 0x3FFF];
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        //toCI14X2 done in class ColorIndexConverter
        #endregion

        #region CMP
        private byte[] fromCMP(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            ushort[] c = new ushort[4];
            int[] pix = new int[4];
            int inp = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int ww = Shared.AddPadding(width, 8);

                    int x0 = x & 0x03;
                    int x1 = (x >> 2) & 0x01;
                    int x2 = x >> 3;

                    int y0 = y & 0x03;
                    int y1 = (y >> 2) & 0x01;
                    int y2 = y >> 3;

                    int off = (8 * x1) + (16 * y1) + (32 * x2) + (4 * ww * y2);

                    c[0] = Shared.Swap(BitConverter.ToUInt16(tpl, off));
                    c[1] = Shared.Swap(BitConverter.ToUInt16(tpl, off + 2));

                    if (c[0] > c[1])
                    {
                        c[2] = (ushort)avg(2, 1, c[0], c[1]);
                        c[3] = (ushort)avg(1, 2, c[0], c[1]);
                    }
                    else
                    {
                        c[2] = (ushort)avg(1, 1, c[0], c[1]);
                        c[3] = 0;
                    }

                    uint pixel = Shared.Swap(BitConverter.ToUInt32(tpl, off + 4));

                    int ix = x0 + (4 * y0);
                    int raw = c[(pixel >> (30 - (2 * ix))) & 0x03];

                    pix[0] = (raw >> 8) & 0xf8;
                    pix[1] = (raw >> 3) & 0xf8;
                    pix[2] = (raw << 3) & 0xf8;
                    pix[3] = 0xff;
                    if (((pixel >> (30 - (2 * ix))) & 0x03) == 3 && c[0] <= c[1]) pix[3] = 0x00;

                    output[inp] = (uint)((pix[0] << 16) | (pix[1] << 8) | (pix[2] << 0) | (pix[3] << 24));
                    inp++;
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        //There's currently no conversion to CMP
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Fires debugging messages. You may write them into a log file or log textbox.
        /// </summary>
        private void fireDebug(string debugMessage, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"{debugMessage}");
        }
        #endregion
    }

    public class TPL_Header
    {
        private uint tplMagic = 0x0020AF30;
        private uint numOfTextures = 0;
        private uint headerSize = 0x0C;

        public uint TplMagic { get { return tplMagic; } }
        public uint NumOfTextures { get { return numOfTextures; } set { numOfTextures = value; } }
        public uint HeaderSize { get { return headerSize; } }

        public void Write(Stream writeStream)
        {
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(tplMagic)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(numOfTextures)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(headerSize)), 0, 4);
        }
    }

    public class TPL_TextureEntry
    {
        private uint textureHeaderOffset = 0x00000000;
        private uint paletteHeaderOffset = 0x00000000;

        public uint TextureHeaderOffset { get { return textureHeaderOffset; } set { textureHeaderOffset = value; } }
        public uint PaletteHeaderOffset { get { return paletteHeaderOffset; } set { paletteHeaderOffset = value; } }

        public void Write(Stream writeStream)
        {
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(textureHeaderOffset)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(paletteHeaderOffset)), 0, 4);
        }
    }

    public class TPL_TextureHeader
    {
        private ushort textureHeight;
        private ushort textureWidth;
        private uint textureFormat;
        private uint textureDataOffset;
        private uint wrapS = 0x00000000;
        private uint wrapT = 0x00000000;
        private uint minFilter = 0x00000001;
        private uint magFilter = 0x00000001;
        private uint lodBias = 0x00000000;
        private byte edgeLod = 0x00;
        private byte minLod = 0x00;
        private byte maxLod = 0x00;
        private byte unpacked = 0x00;

        public ushort TextureHeight { get { return textureHeight; } set { textureHeight = value; } }
        public ushort TextureWidth { get { return textureWidth; } set { textureWidth = value; } }
        public uint TextureFormat { get { return textureFormat; } set { textureFormat = value; } }
        public uint TextureDataOffset { get { return textureDataOffset; } set { textureDataOffset = value; } }
        public uint WrapS { get { return wrapS; } set { wrapS = value; } }
        public uint WrapT { get { return wrapT; } set { wrapT = value; } }
        public uint MinFilter { get { return minFilter; } set { minFilter = value; } }
        public uint MagFilter { get { return magFilter; } set { magFilter = value; } }
        public uint LodBias { get { return lodBias; } set { lodBias = value; } }
        public byte EdgeLod { get { return edgeLod; } set { edgeLod = value; } }
        public byte MinLod { get { return minLod; } set { minLod = value; } }
        public byte MaxLod { get { return maxLod; } set { maxLod = value; } }
        public byte Unpacked { get { return unpacked; } set { unpacked = value; } }

        public void Write(Stream writeStream)
        {
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(textureHeight)), 0, 2);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(textureWidth)), 0, 2);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(textureFormat)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(textureDataOffset)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(wrapS)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(wrapT)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(minFilter)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(magFilter)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(lodBias)), 0, 4);
            writeStream.WriteByte(edgeLod);
            writeStream.WriteByte(minLod);
            writeStream.WriteByte(maxLod);
            writeStream.WriteByte(unpacked);
        }
    }

    public class TPL_PaletteHeader
    {
        private ushort numberOfItems = 0x0000;
        private byte unpacked = 0x00;
        private byte pad = 0x00;
        private uint paletteFormat = 255;
        private uint paletteDataOffset;

        public ushort NumberOfItems { get { return numberOfItems; } set { numberOfItems = value; } }
        public byte Unpacked { get { return unpacked; } set { unpacked = value; } }
        public byte Pad { get { return pad; } set { pad = value; } }
        public uint PaletteFormat { get { return paletteFormat; } set { paletteFormat = value; } }
        public uint PaletteDataOffset { get { return paletteDataOffset; } set { paletteDataOffset = value; } }

        public void Write(Stream writeStream)
        {
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(numberOfItems)), 0, 2);
            writeStream.WriteByte(unpacked);
            writeStream.WriteByte(pad);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(paletteFormat)), 0, 4);
            writeStream.Write(BitConverter.GetBytes(Shared.Swap(paletteDataOffset)), 0, 4);
        }
    }

    internal class ColorIndexConverter
    {
        private uint[] rgbaPalette;
        private byte[] tplPalette;
        private uint[] rgbaData;
        private byte[] tplData;
        private TPL_TextureFormat tplFormat;
        private TPL_PaletteFormat paletteFormat;
        private int width;
        private int height;

        public byte[] Palette { get { return tplPalette; } }
        public byte[] Data { get { return tplData; } }

        public ColorIndexConverter(uint[] rgbaData, int width, int height, TPL_TextureFormat tplFormat, TPL_PaletteFormat paletteFormat)
        {
            if (tplFormat != TPL_TextureFormat.CI4 && tplFormat != TPL_TextureFormat.CI8) // && tplFormat != TPL_TextureFormat.CI14X2)
                throw new Exception("Texture format must be either CI4 or CI8"); // or CI14X2!");
            if (paletteFormat != TPL_PaletteFormat.IA8 && paletteFormat != TPL_PaletteFormat.RGB565 && paletteFormat != TPL_PaletteFormat.RGB5A3)
                throw new Exception("Palette format must be either IA8, RGB565 or RGB5A3!");

            this.rgbaData = rgbaData;
            this.width = width;
            this.height = height;
            this.tplFormat = tplFormat;
            this.paletteFormat = paletteFormat;

            buildPalette();

            if (tplFormat == TPL_TextureFormat.CI4) toCI4();
            else if (tplFormat == TPL_TextureFormat.CI8) toCI8();
            else toCI14X2();
        }

        #region Private Functions
        private void toCI4()
        {
            byte[] indexData = new byte[libWiiSharp.Shared.AddPadding(width, 8) * libWiiSharp.Shared.AddPadding(height, 8) / 2];
            int i = 0;

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 8; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1 += 2)
                        {
                            uint pixel;

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1];

                            uint index1 = getColorIndex(pixel);

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else if (y1 * width + x1 + 1 >= rgbaData.Length)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1 + 1];

                            uint index2 = getColorIndex(pixel);

                            indexData[i++] = (byte)(((byte)index1 << 4) | (byte)index2);
                        }
                    }
                }
            }

            this.tplData = indexData;
        }

        private void toCI8()
        {
            byte[] indexData = new byte[libWiiSharp.Shared.AddPadding(width, 8) * libWiiSharp.Shared.AddPadding(height, 4)];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            uint pixel;

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1];

                            indexData[i++] = (byte)getColorIndex(pixel);
                        }
                    }
                }
            }

            this.tplData = indexData;
        }

        private void toCI14X2()
        {
            byte[] indexData = new byte[libWiiSharp.Shared.AddPadding(width, 4) * libWiiSharp.Shared.AddPadding(height, 4) * 2];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            uint pixel;

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1];

                            byte[] temp = BitConverter.GetBytes((ushort)getColorIndex(pixel));
                            indexData[i++] = temp[1];
                            indexData[i++] = temp[0];
                        }
                    }
                }
            }

            this.tplData = indexData;
        }

        private void buildPalette()
        {
            int palLength = 256;
            if (tplFormat == TPL_TextureFormat.CI4) palLength = 16;
            else if (tplFormat == TPL_TextureFormat.CI14X2) palLength = 16384;

            List<uint> palette = new List<uint>();
            List<ushort> tPalette = new List<ushort>();

            palette.Add(0);
            tPalette.Add(0);

            for (int i = 1; i < rgbaData.Length; i++)
            {
                if (palette.Count == palLength) break;
                if (((rgbaData[i] >> 24) & 0xff) < ((tplFormat == TPL_TextureFormat.CI14X2) ? 1 : 25)) continue;

                ushort tplValue = libWiiSharp.Shared.Swap(convertToPaletteValue((int)rgbaData[i]));

                if (!palette.Contains(rgbaData[i]) && !tPalette.Contains(tplValue))
                {
                    palette.Add(rgbaData[i]);
                    tPalette.Add(tplValue);
                }
            }

            while (palette.Count % 16 != 0)
            { palette.Add(0xffffffff); tPalette.Add(0xffff); }

            tplPalette = libWiiSharp.Shared.UShortArrayToByteArray(tPalette.ToArray());
            rgbaPalette = palette.ToArray();
        }

        private ushort convertToPaletteValue(int rgba)
        {
            int newpixel = 0, r, g, b, a;

            if (paletteFormat == TPL_PaletteFormat.IA8)
            {
                int intensity = ((((rgba >> 0) & 0xff) + ((rgba >> 8) & 0xff) + ((rgba >> 16) & 0xff)) / 3) & 0xff;
                int alpha = (rgba >> 24) & 0xff;

                newpixel = (ushort)((alpha << 8) | intensity);
            }
            else if (paletteFormat == TPL_PaletteFormat.RGB565)
            {
                newpixel = (ushort)(((((rgba >> 16) & 0xff) >> 3) << 11) | ((((rgba >> 8) & 0xff) >> 2) << 5) | ((((rgba >> 0) & 0xff) >> 3) << 0));
            }
            else
            {
                r = (rgba >> 16) & 0xff;
                g = (rgba >> 8) & 0xff;
                b = (rgba >> 0) & 0xff;
                a = (rgba >> 24) & 0xff;

                if (a <= 0xda) //RGB4A3
                {
                    newpixel &= ~(1 << 15);

                    r = ((r * 15) / 255) & 0xf;
                    g = ((g * 15) / 255) & 0xf;
                    b = ((b * 15) / 255) & 0xf;
                    a = ((a * 7) / 255) & 0x7;

                    newpixel |= a << 12;
                    newpixel |= b << 0;
                    newpixel |= g << 4;
                    newpixel |= r << 8;
                }
                else //RGB5
                {
                    newpixel |= (1 << 15);

                    r = ((r * 31) / 255) & 0x1f;
                    g = ((g * 31) / 255) & 0x1f;
                    b = ((b * 31) / 255) & 0x1f;

                    newpixel |= b << 0;
                    newpixel |= g << 5;
                    newpixel |= r << 10;
                }
            }

            return (ushort)newpixel;
        }

        private uint getColorIndex(uint value)
        {
            uint minDistance = 0x7FFFFFFF;
            uint colorIndex = 0;

            if (((value >> 24) & 0xFF) < ((tplFormat == TPL_TextureFormat.CI14X2) ? 1 : 25)) return 0;
            ushort color = convertToPaletteValue((int)value);

            for (int i = 0; i < rgbaPalette.Length; i++)
            {
                ushort curPal = convertToPaletteValue((int)rgbaPalette[i]);

                if (color == curPal) return (uint)i;
                uint curDistance = getDistance(color, curPal); //(uint)Math.Abs(Math.Abs(color) - Math.Abs(curVal));

                if (curDistance < minDistance)
                {
                    minDistance = curDistance;
                    colorIndex = (uint)i;
                }
            }

            return colorIndex;
        }

        private uint getDistance(ushort color, ushort paletteColor)
        {
            uint curCol = convertToRgbaValue(color);
            uint palCol = convertToRgbaValue(paletteColor);

            uint curA = (curCol >> 24) & 0xFF;
            uint curR = (curCol >> 16) & 0xFF;
            uint curG = (curCol >> 8) & 0xFF;
            uint curB = (curCol >> 0) & 0xFF;

            uint palA = (palCol >> 24) & 0xFF;
            uint palR = (palCol >> 16) & 0xFF;
            uint palG = (palCol >> 8) & 0xFF;
            uint palB = (palCol >> 0) & 0xFF;

            uint distA = Math.Max(curA, palA) - Math.Min(curA, palA);
            uint distR = Math.Max(curR, palR) - Math.Min(curR, palR);
            uint distG = Math.Max(curG, palG) - Math.Min(curG, palG);
            uint distB = Math.Max(curB, palB) - Math.Min(curB, palB);

            return distA + distR + distG + distB;
        }

        private uint convertToRgbaValue(ushort pixel)
        {
            int rgba = 0, r, g, b, a;

            if (paletteFormat == TPL_PaletteFormat.IA8)
            {
                int i = (pixel >> 8);
                a = pixel & 0xff;

                rgba = (i << 0) | (i << 8) | (i << 16) | (a << 24);
            }
            else if (paletteFormat == TPL_PaletteFormat.RGB565)
            {
                b = (((pixel >> 11) & 0x1F) << 3) & 0xff;
                g = (((pixel >> 5) & 0x3F) << 2) & 0xff;
                r = (((pixel >> 0) & 0x1F) << 3) & 0xff;
                a = 255;

                rgba = (r << 0) | (g << 8) | (b << 16) | (a << 24);
            }
            else
            {
                if ((pixel & (1 << 15)) != 0)
                {
                    b = (((pixel >> 10) & 0x1F) * 255) / 31;
                    g = (((pixel >> 5) & 0x1F) * 255) / 31;
                    r = (((pixel >> 0) & 0x1F) * 255) / 31;
                    a = 255;
                }
                else
                {
                    a = (((pixel >> 12) & 0x07) * 255) / 7;
                    b = (((pixel >> 8) & 0x0F) * 255) / 15;
                    g = (((pixel >> 4) & 0x0F) * 255) / 15;
                    r = (((pixel >> 0) & 0x0F) * 255) / 15;
                }

                rgba = (r << 0) | (g << 8) | (b << 16) | (a << 24);
            }

            return (uint)rgba;
        }
        #endregion
    }



    public static class Shared
    {
        /// <summary>
        /// Merges two string arrays into one without double entries.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string[] MergeStringArrays(string[] a, string[] b)
        {
            List<string> sList = new List<string>(a);

            foreach (string currentString in b)
                if (!sList.Contains(currentString)) sList.Add(currentString);

            sList.Sort();
            return sList.ToArray();
        }

        /// <summary>
        /// Compares two byte arrays.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="firstIndex"></param>
        /// <param name="second"></param>
        /// <param name="secondIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] first, int firstIndex, byte[] second, int secondIndex, int length)
        {
            if (first.Length < length || second.Length < length) return false;

            for (int i = 0; i < length; i++)
                if (first[firstIndex + i] != second[secondIndex + i]) return false;

            return true;
        }

        /// <summary>
        /// Compares two byte arrays.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] first, byte[] second)
        {
            if (first.Length != second.Length) return false;
            else
                for (int i = 0; i < first.Length; i++)
                    if (first[i] != second[i]) return false;

            return true;
        }

        /// <summary>
        /// Turns a byte array into a string, default separator is a space.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] byteArray, char separator = ' ')
        {
            string res = string.Empty;

            foreach (byte b in byteArray)
                res += b.ToString("x2").ToUpper() + separator;

            return res.Remove(res.Length - 1);
        }

        /// <summary>
        /// Turns a hex string into a byte array.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] ba = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length / 2; i++)
                ba[i] = byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

            return ba;
        }

        /// <summary>
        /// Counts how often the given char exists in the given string.
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="theChar"></param>
        /// <returns></returns>
        public static int CountCharsInString(string theString, char theChar)
        {
            int count = 0;

            foreach (char thisChar in theString)
                if (thisChar == theChar)
                    count++;

            return count;
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long AddPadding(long value)
        {
            return AddPadding(value, 64);
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static long AddPadding(long value, int padding)
        {
            if (value % padding != 0)
            {
                value = value + (padding - (value % padding));
            }

            return value;
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int AddPadding(int value)
        {
            return AddPadding(value, 64);
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static int AddPadding(int value, int padding)
        {
            if (value % padding != 0)
            {
                value = value + (padding - (value % padding));
            }

            return value;
        }

        /// <summary>
        /// Swaps endianness.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort Swap(ushort value)
        {
            return (ushort)IPAddress.HostToNetworkOrder((short)value);
        }

        /// <summary>
        /// Swaps endianness.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint Swap(uint value)
        {
            return (uint)IPAddress.HostToNetworkOrder((int)value);
        }

        /// <summary>
        /// Swaps endianness
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong Swap(ulong value)
        {
            return (ulong)IPAddress.HostToNetworkOrder((long)value);
        }

        /// <summary>
        /// Turns a ushort array into a byte array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] UShortArrayToByteArray(ushort[] array)
        {
            List<byte> results = new List<byte>();
            foreach (ushort value in array)
            {
                byte[] converted = BitConverter.GetBytes(value);
                results.AddRange(converted);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Turns a uint array into a byte array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] UIntArrayToByteArray(uint[] array)
        {
            List<byte> results = new List<byte>();
            foreach (uint value in array)
            {
                byte[] converted = BitConverter.GetBytes(value);
                results.AddRange(converted);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Turns a byte array into a uint array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static uint[] ByteArrayToUIntArray(byte[] array)
        {
            UInt32[] converted = new UInt32[array.Length / 4];
            int j = 0;

            for (int i = 0; i < array.Length; i += 4)
                converted[j++] = BitConverter.ToUInt32(array, i);

            return converted;
        }

        /// <summary>
        /// Turns a byte array into a ushort array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ushort[] ByteArrayToUShortArray(byte[] array)
        {
            ushort[] converted = new ushort[array.Length / 2];
            int j = 0;

            for (int i = 0; i < array.Length; i += 2)
                converted[j++] = BitConverter.ToUInt16(array, i);

            return converted;
        }
    }
}