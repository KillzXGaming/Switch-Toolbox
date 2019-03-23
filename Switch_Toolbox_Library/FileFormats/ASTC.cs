using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using System.Drawing;

namespace Switch_Toolbox.Library
{
    public class ASTC : STGenericTexture, IEditor<ImageEditorForm>, IFileFormat
    {
        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
                    TEX_FORMAT.ASTC_10x10_SRGB,
                    TEX_FORMAT.ASTC_10x10_UNORM,
                    TEX_FORMAT.ASTC_10x5_SRGB,
                    TEX_FORMAT.ASTC_10x5_UNORM,
                    TEX_FORMAT.ASTC_10x6_SRGB,
                    TEX_FORMAT.ASTC_10x6_UNORM,
                    TEX_FORMAT.ASTC_10x8_SRGB,
                    TEX_FORMAT.ASTC_10x8_UNORM,
                    TEX_FORMAT.ASTC_12x10_SRGB,
                    TEX_FORMAT.ASTC_12x10_UNORM,
                    TEX_FORMAT.ASTC_12x12_SRGB,
                    TEX_FORMAT.ASTC_12x12_UNORM,
                    TEX_FORMAT.ASTC_4x4_SRGB,
                    TEX_FORMAT.ASTC_4x4_UNORM,
                    TEX_FORMAT.ASTC_5x4_SRGB,
                    TEX_FORMAT.ASTC_5x4_UNORM,
                    TEX_FORMAT.ASTC_5x5_SRGB,
                    TEX_FORMAT.ASTC_5x5_UNORM,
                    TEX_FORMAT.ASTC_6x5_SRGB,
                    TEX_FORMAT.ASTC_6x5_UNORM,
                    TEX_FORMAT.ASTC_6x6_SRGB,
                    TEX_FORMAT.ASTC_6x6_UNORM,
                    TEX_FORMAT.ASTC_8x5_SRGB,
                    TEX_FORMAT.ASTC_8x5_UNORM,
                    TEX_FORMAT.ASTC_8x6_SRGB,
                    TEX_FORMAT.ASTC_8x6_UNORM,
                    TEX_FORMAT.ASTC_8x8_SRGB,
                    TEX_FORMAT.ASTC_8x8_UNORM,
                };
            }
        }

        const int MagicFileConstant = 0x5CA1AB13;

        public override bool CanEdit { get; set; } = false;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Adaptive Scalable Texture" };
        public string[] Extension { get; set; } = new string[] { "*.astc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.ReadInt32() == MagicFileConstant;
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public ImageEditorForm OpenForm()
        {
            ImageEditorForm form = new ImageEditorForm();
            form.editorBase.Text = Text;
            form.editorBase.Dock = DockStyle.Fill;
            form.editorBase.LoadImage(this);
            form.editorBase.LoadProperties(GenericProperties);
            return form;
        }

        //https://github.com/ARM-software/astc-encoder/blob/a47b80f081f10c43d96bd10bcb713c71708041b9/Source/astc_toplevel.cpp
        public byte[] magic;
        public byte BlockDimX;
        public byte BlockDimY;
        public byte BlockDimZ;
        public byte[] xsize;
        public byte[] ysize;
        public byte[] zsize;
        public byte[] DataBlock;

        public void Load(System.IO.Stream stream)
        {
            using (FileReader reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                magic = reader.ReadBytes(4);

                uint magicval = magic[0] + 256 * (uint)(magic[1]) + 65536 * (uint)(magic[2]) + 16777216 * (uint)(magic[3]);

                if (magicval != MagicFileConstant)
                    throw new Exception("Invalid identifier");

                BlockDimX = reader.ReadByte();
                BlockDimY = reader.ReadByte();
                BlockDimZ = reader.ReadByte();
                xsize = reader.ReadBytes(3);
                ysize = reader.ReadBytes(3);
                zsize = reader.ReadBytes(3);

                Width = (uint)(xsize[0] + 256 * xsize[1] + 65536 * xsize[2]);
                Height = (uint)(ysize[0] + 256 * ysize[1] + 65536 * ysize[2]);
                Depth = (uint)(zsize[0] + 256 * zsize[1] + 65536 * zsize[2]);

                reader.Seek(0x10, System.IO.SeekOrigin.Begin);
                DataBlock = reader.ReadBytes((int)(reader.BaseStream.Length - reader.Position));

                Console.WriteLine(Width);
                Console.WriteLine(Height);
                Console.WriteLine(Depth);

                if (BlockDimX == 4 && BlockDimY == 4)
                    Format = TEX_FORMAT.ASTC_4x4_UNORM;
                if (BlockDimX == 5 && BlockDimY == 4)
                    Format = TEX_FORMAT.ASTC_5x4_UNORM;
                if (BlockDimX == 5 && BlockDimY == 5)
                    Format = TEX_FORMAT.ASTC_5x5_UNORM;
                if (BlockDimX == 6 && BlockDimY == 5)
                    Format = TEX_FORMAT.ASTC_6x5_UNORM;
                if (BlockDimX == 6 && BlockDimY == 6)
                    Format = TEX_FORMAT.ASTC_6x6_UNORM;
                if (BlockDimX == 8 && BlockDimY == 5)
                    Format = TEX_FORMAT.ASTC_8x5_UNORM;
                if (BlockDimX == 8 && BlockDimY == 6)
                    Format = TEX_FORMAT.ASTC_8x6_UNORM;
                if (BlockDimX == 8 && BlockDimY == 8)
                    Format = TEX_FORMAT.ASTC_8x8_UNORM;
            }

            stream.Dispose();
            stream.Close();
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            using (FileWriter writer = new FileWriter(mem))
            {
                int sizeX = (int)Width;
                int sizeY = (int)Height;
                int sizeZ = (int)Depth;

                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                magic[0] = MagicFileConstant & 0xFF;
                magic[1] = (MagicFileConstant >> 8) & 0xFF;
                magic[2] = (MagicFileConstant >> 16) & 0xFF;
                magic[3] = (byte)(MagicFileConstant >> 24) & 0xFF;

                writer.Write(magic);
                writer.Write(BlockDimX);
                writer.Write(BlockDimY);
                writer.Write(BlockDimZ);

                xsize[0] = (byte)(sizeX & 0xFF);
                xsize[1] = (byte)((sizeX >> 8) & 0xFF);
                xsize[2] = (byte)((sizeX >> 16) & 0xFF);
                ysize[0] = (byte)(sizeY & 0xFF);
                ysize[1] = (byte)((sizeY >> 8) & 0xFF);
                ysize[2] = (byte)((sizeY >> 16) & 0xFF);
                zsize[0] = (byte)(sizeZ & 0xFF);
                zsize[1] = (byte)((sizeZ >> 8) & 0xFF);
                zsize[2] = (byte)((sizeZ >> 16) & 0xFF);

                writer.Write(xsize);
                writer.Write(ysize);
                writer.Write(zsize);
                writer.Write(DataBlock);
            }

            return mem.ToArray();
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException("Cannot set image data! Operation not implemented!");
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            return DataBlock;
        }
    }
}
