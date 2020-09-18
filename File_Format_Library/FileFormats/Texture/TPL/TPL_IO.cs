using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class TPL_IO
    {
        public class Header
        {
            public List<ImageEntry> Images = new List<ImageEntry>();

            public bool IsV1 = false;
            public bool IsBigEndian = true;

            public void Read(FileReader reader)
            {
                reader.SetByteOrder(true);
                uint Identifier = reader.ReadUInt32();
                if (Identifier != 0x0020AF30 && Identifier != 0x30AF2000) {
                    IsV1 = true;

                    reader.Position = 0;
                    uint ImageCount = reader.ReadUInt32();
                    for (int i = 0; i < ImageCount; i++) {
                        reader.SeekBegin(4 + (i * 0x10));
                        ImageEntry image = new ImageEntry();
                        image.Header = new ImageHeaderV1(reader);
                        image.ImageData = GetImageData(reader, image.Header);
                        Images.Add(image);
                    }
                }
                else {
                    IsBigEndian = Identifier == 0x0020AF30;
                    reader.SetByteOrder(IsBigEndian);
                    uint ImageCount = reader.ReadUInt32();
                    uint ImageOffsetTable = reader.ReadUInt32();
                    for (int i = 0; i < ImageCount; i++) {
                        reader.SeekBegin(ImageOffsetTable + (i * 8));

                        ImageEntry image = new ImageEntry();

                        uint ImageHeaderOffset = reader.ReadUInt32();
                        uint PaletteHeaderOffset = reader.ReadUInt32();

                        reader.SeekBegin(ImageHeaderOffset);
                        image.Header = new ImageHeaderV2(reader);
                        image.ImageData = GetImageData(reader, image.Header);
                        Images.Add(image);

                        if (PaletteHeaderOffset != 0) {
                            reader.SeekBegin(PaletteHeaderOffset);
                            image.PaletteHeader = new PaletteHeader(reader);
                        }
                    }
                }
            }

            public void Write(FileWriter writer)
            {
                writer.SetByteOrder(IsBigEndian);
                if (IsV1) {
                    writer.Write(Images.Count);
                    for (int i = 0; i < Images.Count; i++)
                        ((ImageHeaderV1)Images[i].Header).Write(writer);

                    AlignBytesIncrement(writer, 32);
                    for (int i = 0; i < Images.Count; i++) {
                        writer.WriteUint32Offset(8 + (i * 0x10));
                        writer.Write(Images[i].ImageData);
                    }
                }
                else
                {
                    writer.Write(0x0020AF30);
                    writer.Write(Images.Count);
                    writer.Write(0x0C); //Offset always 12

                    //Reserve space for palette and header offsets
                    writer.Write(new byte[Images.Count * 8]);

                    //Then write the palettes first
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (Images[i].PaletteHeader != null) {
                            writer.WriteUint32Offset(16 + (i * 8));
                            Images[i].PaletteHeader.Write(writer);
                        }
                    }

                    //Then write the headers
                    long imageHeaderPos = writer.Position;
                    for (int i = 0; i < Images.Count; i++)
                    {
                        writer.WriteUint32Offset(12 + (i * 8));
                        ((ImageHeaderV2)Images[i].Header).Write(writer);
                    }

                    //Then write the data
                    for (int i = 0; i < Images.Count; i++)
                    {
                        writer.Align(64);
                        writer.WriteUint32Offset(imageHeaderPos + 8 + (i * 0x24));
                        writer.Write(Images[i].ImageData);
                    }
                }
            }
        }

        private static void AlignBytesIncrement(FileWriter writer, int alignment)
        {
            var startPos = writer.Position;
            long position = writer.Seek((-writer.Position % alignment + alignment) % alignment, SeekOrigin.Current);

            byte value = 0;

            writer.Seek(startPos, System.IO.SeekOrigin.Begin);
            while (writer.Position != position) {
                writer.Write(value++);
            }
        }

        private static byte[] GetImageData(FileReader reader, TPLImageHeader image)
        {
           return reader.getSection(image.ImageOffset,
                (uint)Decode_Gamecube.GetDataSizeWithMips(image.Format, image.Width, image.Height, image.MaxLOD));
        }

        public class ImageEntry
        {
            public PaletteHeader PaletteHeader;
            public TPLImageHeader Header;
            public byte[] ImageData;
        }

        public interface TPLImageHeader
        {
            Decode_Gamecube.TextureFormats Format { get; set; }
            uint ImageOffset { get; set; }
            ushort Width { get; set; }
            ushort Height { get; set; }
            byte MinLOD { get; set; }
            byte MaxLOD { get; set; }
        }

        //TPL has 2 versions
        //V1 is used for some gc games

        public class ImageHeaderV1 : TPLImageHeader
        {
            [ReadOnly(true)]
            public Decode_Gamecube.TextureFormats Format { get; set; }
            [Browsable(false)]
            public uint ImageOffset { get; set; }
            [ReadOnly(true)]
            public ushort Width { get; set; }
            [ReadOnly(true)]
            public ushort Height { get; set; }
            [ReadOnly(true)]
            public byte MinLOD { get; set; }
            [ReadOnly(true)]
            public byte MaxLOD { get; set; }
            [ReadOnly(true)]
            public ushort Unknown { get; set; }

            public ImageHeaderV1() { }

            public ImageHeaderV1(FileReader reader)
            {
                Format = (Decode_Gamecube.TextureFormats)reader.ReadUInt32();
                ImageOffset = reader.ReadUInt32();
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                MinLOD = reader.ReadByte();
                MaxLOD = reader.ReadByte();
                Unknown = reader.ReadUInt16();
            }

            public void Write(FileWriter writer)
            {
                writer.Write((uint)Format);
                writer.Write(ImageOffset);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(MinLOD);
                writer.Write(MaxLOD);
                writer.Write(Unknown);
            }
        }

        public class ImageHeaderV2 : TPLImageHeader
        {
            [ReadOnly(true)]
            public ushort Width { get; set; }
            [ReadOnly(true)]
            public ushort Height { get; set; }
            [ReadOnly(true)]
            public Decode_Gamecube.TextureFormats Format { get; set; }
            [Browsable(false)]
            public uint ImageOffset { get; set; }

            public WrapMode WrapS { get; set; }
            public WrapMode WrapT { get; set; }
            public FilterMode MinFilter { get; set; }
            public FilterMode MagFilter { get; set; }

            public float LODBias { get; set; }
            public bool EdgeLODEnable { get; set; }
            [ReadOnly(true)]
            public byte MinLOD { get; set; }
            [ReadOnly(true)]
            public byte MaxLOD { get; set; }
            [ReadOnly(true)]
            public byte Unpacked { get; set; }

            public ImageHeaderV2() { }

            public ImageHeaderV2(FileReader reader)
            {
                Height = reader.ReadUInt16();
                Width = reader.ReadUInt16();
                Format = (Decode_Gamecube.TextureFormats)reader.ReadUInt32();
                ImageOffset = reader.ReadUInt32();
                WrapS = (WrapMode)reader.ReadUInt32();
                WrapT = (WrapMode)reader.ReadUInt32();
                MinFilter = (FilterMode)reader.ReadUInt32();
                MagFilter = (FilterMode)reader.ReadUInt32();
                LODBias = reader.ReadSingle();
                EdgeLODEnable = reader.ReadBoolean();
                MinLOD = reader.ReadByte();
                MaxLOD = reader.ReadByte();
                Unpacked = reader.ReadByte();
            }

            public void Write(FileWriter writer)
            {
                writer.Write(Height);
                writer.Write(Width);
                writer.Write((uint)Format);
                writer.Write(ImageOffset);
                writer.Write((uint)WrapS);
                writer.Write((uint)WrapT);
                writer.Write((uint)MinFilter);
                writer.Write((uint)MagFilter);
                writer.Write(LODBias);
                writer.Write(EdgeLODEnable);
                writer.Write(MinLOD);
                writer.Write(MaxLOD);
                writer.Write(Unpacked);
            }
        }

        public class PaletteHeader
        {
            public ushort EntryCount { get; set; }
            public byte Unpacked { get; set; }
            public uint PaletteFormat { get; set; }
            public uint PaletteDataOffset { get; set; }

            public byte[] Data;

            public PaletteHeader(FileReader reader) {
                EntryCount = reader.ReadUInt16();
                Unpacked = reader.ReadByte();
                reader.ReadByte();
                PaletteFormat = reader.ReadUInt32();
                PaletteDataOffset = reader.ReadUInt32();

                using (reader.TemporarySeek(PaletteDataOffset, SeekOrigin.Begin)) {
                    Data = reader.ReadBytes(EntryCount * 2);
                }
            }

            public void Write(FileWriter writer) {
                writer.Write(EntryCount);
                writer.Write(Unpacked);
                writer.Write((byte)0);
                writer.Write(PaletteFormat);
                writer.Write(PaletteDataOffset);
            }
        }

        public enum WrapMode : uint
        {
            Clamp,
            Repeat,
            Mirror
        }

        public enum FilterMode : uint
        {
            Nearest,
            Linear,
        }
    }
}
