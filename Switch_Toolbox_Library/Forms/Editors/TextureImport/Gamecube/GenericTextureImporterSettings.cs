using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace Toolbox.Library.Forms
{
    public class GameCubeTextureImporterSettings
    {
        public GameCubeTextureImporterSettings()
        {
        }

        public string TexName;

        public uint MipCount;

        public uint Depth = 1;

        public uint TexWidth;

        public uint TexHeight;

        public Decode_Gamecube.TextureFormats Format = Decode_Gamecube.TextureFormats.CMPR;
        public Decode_Gamecube.PaletteFormats PaletteFormat = Decode_Gamecube.PaletteFormats.RGB565;

        public TEX_FORMAT GenericFormat
        {
            get { return Decode_Gamecube.ToGenericFormat(Format); }
        }

        public PALETTE_FORMAT GenericPaletteFormat
        {
            get { return Decode_Gamecube.ToGenericPaletteFormat(PaletteFormat); }
        }

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

            Format = Decode_Gamecube.FromGenericFormat(dds.Format);
        }

        public void LoadBitMap(Image Image, string Name)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(Name);

            Format = Decode_Gamecube.TextureFormats.CMPR;

            GenerateMipmaps = true;
            LoadImage(new Bitmap(Image));
        }

        public void LoadBitMap(string FileName)
        {
            DecompressedData.Clear();

            TexName = STGenericTexture.SetNameFromPath(FileName);

            Format = Decode_Gamecube.TextureFormats.CMPR;

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


        public Tuple<List<byte[]>, ushort[]> GenerateMipList(int SurfaceLevel = 0)
        {
            Bitmap Image = BitmapExtension.GetBitmap(DecompressedData[SurfaceLevel], (int)TexWidth, (int)TexHeight);

            ushort[] paletteData = new ushort[0];

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                int MipWidth = Math.Max(1, (int)TexWidth >> mipLevel);
                int MipHeight = Math.Max(1, (int)TexHeight >> mipLevel);

                if (mipLevel != 0)
                    Image = BitmapExtension.Resize(Image, MipWidth, MipHeight);

                var EncodedData = Decode_Gamecube.EncodeData(BitmapExtension.ImageToByte(Image), Format, PaletteFormat, MipWidth, MipHeight);

                mipmaps.Add(EncodedData.Item1);

                if (mipLevel == 0) //Set palette data once
                    paletteData = EncodedData.Item2;
            }
            Image.Dispose();

            return Tuple.Create(mipmaps, paletteData);
        }

        public void Compress()
        {
            DataBlockOutput.Clear();
            foreach (var surface in DecompressedData)
            {
                var encodedData = GenerateMipList();
                DataBlockOutput.Add(Utils.CombineByteArray(encodedData.Item1.ToArray()));
            }
        }
    }
}
