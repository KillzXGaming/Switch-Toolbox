using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Drawing;

namespace Toolbox.Library
{
    public class ASTC : STGenericTexture, IEditor<ImageEditorBase>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

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
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
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

        public ImageEditorBase OpenForm()
        {
            bool IsDialog = IFileInfo != null && IFileInfo.InArchive;

            ImageEditorBase form = new ImageEditorBase();
            form.Text = Text;
            form.Dock = DockStyle.Fill;
            form.LoadImage(this);
            form.LoadProperties(GenericProperties);
            return form;
        }

        public void FillEditor(UserControl control)
        {
            ((ImageEditorBase)control).LoadImage(this);
            ((ImageEditorBase)control).LoadProperties(GenericProperties);
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
                else if (BlockDimX == 5 && BlockDimY == 4)
                    Format = TEX_FORMAT.ASTC_5x4_UNORM;
                else if (BlockDimX == 5 && BlockDimY == 5)
                    Format = TEX_FORMAT.ASTC_5x5_UNORM;
                else if (BlockDimX == 6 && BlockDimY == 5)
                    Format = TEX_FORMAT.ASTC_6x5_UNORM;
                else if (BlockDimX == 6 && BlockDimY == 6)
                    Format = TEX_FORMAT.ASTC_6x6_UNORM;
                else if (BlockDimX == 8 && BlockDimY == 5)
                    Format = TEX_FORMAT.ASTC_8x5_UNORM;
                else if (BlockDimX == 8 && BlockDimY == 6)
                    Format = TEX_FORMAT.ASTC_8x6_UNORM;
                else if (BlockDimX == 8 && BlockDimY == 8)
                    Format = TEX_FORMAT.ASTC_8x8_UNORM;
                else if (BlockDimX == 10 && BlockDimY == 10)
                    Format = TEX_FORMAT.ASTC_10x10_UNORM;
                else if (BlockDimX == 10 && BlockDimY == 5)
                    Format = TEX_FORMAT.ASTC_10x5_UNORM;
                else if (BlockDimX == 10 && BlockDimY == 6)
                    Format = TEX_FORMAT.ASTC_10x6_UNORM;
                else if (BlockDimX == 10 && BlockDimY == 8)
                    Format = TEX_FORMAT.ASTC_10x8_UNORM;
                else
                    throw new Exception($"Unsupported block dims! ({BlockDimX} x {BlockDimY})");
            }

            stream.Dispose();
            stream.Close();
        }
        public void Unload()
        {

        }
        public void Save(System.IO.Stream stream)
        {
            using (FileWriter writer = new FileWriter(stream, true))
            {
                if (Depth == 0)
                    Depth = 1;

                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                writer.Write(MagicFileConstant);
                writer.Write(BlockDimX);
                writer.Write(BlockDimY);
                writer.Write(BlockDimZ);
                writer.Write(IntTo3Bytes((int)Width));
                writer.Write(IntTo3Bytes((int)Height));
                writer.Write(IntTo3Bytes((int)Depth));
                writer.Write(DataBlock);
            }
        }

        private static byte[] IntTo3Bytes(int value)
        {
            byte[] newValue = new byte[3];
            newValue[0] = (byte)(value & 0xFF);
            newValue[1] = (byte)((value >> 8) & 0xFF);
            newValue[2] = (byte)((value >> 16) & 0xFF);
            return newValue;
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
