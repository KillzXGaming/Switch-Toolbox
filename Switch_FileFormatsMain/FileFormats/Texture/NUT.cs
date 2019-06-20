using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using System.Drawing;
using Syroot.NintenTools.Bfres.GX2;

namespace FirstPlugin
{
    public class NUT : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Namco Universal Texture Container" };
        public string[] Extension { get; set; } = new string[] { "*.nut" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                if (reader.CheckSignature(4, "NTWU") || 
                    reader.CheckSignature(4, "NTP3") ||
                    reader.CheckSignature(4, "NTWD"))
                    return true;
                else
                    return false;
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

        Header NutHeader;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            NutHeader = new Header();
            NutHeader.Read(new FileReader(stream));

            foreach (var image in NutHeader.Images)
                Nodes.Add(image);
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            NutHeader.Write(new FileWriter(mem));
            return mem.ToArray();
        }

        public class Header
        {
            public string Magic;
            public ushort ByteOrderMark;
            public ushort Version;
            public List<NutImage> Images = new List<NutImage>();
            byte[] Reserved;

            public bool IsNTP3;

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                Magic = reader.ReadString(4, Encoding.ASCII);
                Version = reader.ReadUInt16();

                IsNTP3 = Magic == "NTP3";

                if (Magic == "NTWD")
                {
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                    IsNTP3 = true;
                }

                ushort ImageCount = reader.ReadUInt16();
                Reserved = reader.ReadBytes(8);

                for (int i = 0; i < ImageCount; i++)
                {
                    NutImage image = new NutImage();
                    image.Read(reader, this);
                    Images.Add(image);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature(Magic);
                writer.Write(Version);
                writer.Write((ushort)Images.Count);
                writer.Write(Reserved);

                for (int i = 0; i < Images.Count; i++)
                {
                    Images[i].Write(writer, IsNTP3);
                }
            }
        }

        public class NutImage : STGenericTexture
        {
            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                        TEX_FORMAT.BC1_UNORM,
                        TEX_FORMAT.BC1_UNORM_SRGB,
                        TEX_FORMAT.BC2_UNORM,
                        TEX_FORMAT.BC2_UNORM_SRGB,
                        TEX_FORMAT.BC3_UNORM,
                        TEX_FORMAT.BC3_UNORM_SRGB,
                        TEX_FORMAT.BC4_UNORM,
                        TEX_FORMAT.BC4_SNORM,
                        TEX_FORMAT.BC5_UNORM,
                        TEX_FORMAT.BC5_SNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.B8G8R8A8_UNORM_SRGB,
                        TEX_FORMAT.B8G8R8A8_UNORM,
                        TEX_FORMAT.R10G10B10A2_UNORM,
                        TEX_FORMAT.R16_UNORM,
                        TEX_FORMAT.B4G4R4A4_UNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                        TEX_FORMAT.R32G8X24_FLOAT,
                    };
                }
            }

            public override bool CanEdit { get; set; } = false;

            public uint TotalTextureSize;
            public uint PaletteSize;
            public uint ImageSize;
            public uint HeaderSize;
            public ushort PaletteCount;
            public byte NutFormat;
            public byte OldFormat;
            public byte PaletteFormat;

            public bool IsCubeMap = false;

            public EXT ExternalData;
            public GIDX GIDXHeaderData;
            public NutGX2Surface Gx2HeaderData;

            public byte[] Data;
            public byte[] MipData;

            public uint[] ImageSizes;

            public override void OnClick(TreeView treeview)
            {
                UpdateEditor();
            }

            private void UpdateEditor()
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.Instance.LoadEditor(editor);
                }

                Properties prop = new Properties();
                prop.Width = Width;
                prop.Height = Height;
                prop.Depth = Depth;
                prop.MipCount = MipCount;
                prop.ArrayCount = ArrayCount;
                prop.ImageSize = (uint)ImageSize;
                prop.Format = Format;

                editor.Text = Text;
                editor.LoadProperties(prop);
                editor.LoadImage(this);
            }

            private TEX_FORMAT SetFormat(byte format)
            {
                switch (format)
                {
                    case 0x0: return TEX_FORMAT.BC1_UNORM;
                    case 0x1: return TEX_FORMAT.BC2_UNORM;
                    case 0x2: return TEX_FORMAT.BC3_UNORM;
                    case 8: return TEX_FORMAT.B5G5R5A1_UNORM;
                    case 12: return TEX_FORMAT.R16G16B16A16_UNORM;
                    case 14: return TEX_FORMAT.R8G8B8A8_UNORM;
                    case 16: return TEX_FORMAT.R8G8B8A8_UNORM;
                    case 17: return TEX_FORMAT.R8G8B8A8_UNORM;
                    case 21: return TEX_FORMAT.BC4_UNORM;
                    case 22: return TEX_FORMAT.BC5_UNORM;
                    default:
                        throw new NotImplementedException($"Unsupported Nut Format {format}");
                }
            }

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                Console.WriteLine("pos " + pos);

                TotalTextureSize = reader.ReadUInt32(); //Including header
                PaletteSize = reader.ReadUInt32(); //Used in older versions
                ImageSize = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt16();
                PaletteCount = reader.ReadUInt16(); //Used in older versions
                OldFormat = reader.ReadByte(); //Used in older versions
                MipCount = reader.ReadByte();
                PaletteFormat = reader.ReadByte(); //Used in older versions
                NutFormat = reader.ReadByte();
                Format = SetFormat(NutFormat);

                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                uint Unknown = reader.ReadUInt32(); //Maybe related to 3D depth size
                uint Flags = reader.ReadUInt32();  //Determine when to use cube maps


                uint dataOffset = 0;

                if (header.IsNTP3)
                {
                    if (header.Version < 0x0200) {
                        dataOffset = HeaderSize;
                        uint padding2 = reader.ReadUInt32();
                    }
                    else if (header.Version >= 0x0200)
                        dataOffset = reader.ReadUInt32();
                }
                else
                    dataOffset = reader.ReadUInt32();
                

                uint mipOffset = reader.ReadUInt32();
                uint gtxHeaderOffset = reader.ReadUInt32();
                uint padding = reader.ReadUInt32(); //Could be an offset to an unused section?

                uint cubeMapSize1 = 0;
                uint cubeMapSize2 = 0;

                if ((Flags & (uint)DDS.DDSCAPS2.CUBEMAP) == (uint)DDS.DDSCAPS2.CUBEMAP)
                {
                    //Only supporting all six faces
                    if ((Flags & (uint)DDS.DDSCAPS2.CUBEMAP_ALLFACES) == (uint)DDS.DDSCAPS2.CUBEMAP_ALLFACES)
                    {
                        IsCubeMap = true;
                        ArrayCount = 6;
                    }
                    else
                    {
                        throw new NotImplementedException($"Unsupported cubemap face amount for texture.");
                    }
                }


                if (IsCubeMap)
                {
                    cubeMapSize1 = reader.ReadUInt32();
                    cubeMapSize2 = reader.ReadUInt32();
                    uint unk = reader.ReadUInt32();
                    uint unk2 = reader.ReadUInt32();
                }

               ImageSizes = new uint[MipCount];

                uint MipBlockSize = 0;

                if (MipCount == 1) {
                    if (IsCubeMap)
                        ImageSizes[0] = cubeMapSize1;
                    else
                        ImageSizes[0] = ImageSize;
                }
                else
                {
                    if (header.IsNTP3)
                    {
                        ImageSizes = reader.ReadUInt32s((int)MipCount );
                    }
                    else
                    {
                        ImageSizes[0] = reader.ReadUInt32();
                        MipBlockSize = reader.ReadUInt32();

                        reader.ReadUInt32s((int)MipCount - 2); //Padding / Unused
                    }

                    reader.Align(16);
                }

                ExternalData = new EXT();
                ExternalData.Read(reader, header.IsNTP3);

                GIDXHeaderData = new GIDX();
                GIDXHeaderData.Read(reader, header.IsNTP3);
                Text = GIDXHeaderData.HashID.ToString();

                if (dataOffset != 0)
                {
                    using (reader.TemporarySeek(dataOffset + pos, System.IO.SeekOrigin.Begin)) {
                        if (header.IsNTP3)
                            Data = reader.ReadBytes((int)ImageSize);
                        else
                            Data = reader.ReadBytes((int)ImageSizes[0]); //Mip maps are seperate for GX2
                    }
                }

                if (mipOffset != 0)
                {
                    using (reader.TemporarySeek(mipOffset + pos, System.IO.SeekOrigin.Begin)) {
                        MipData = reader.ReadBytes((int)MipBlockSize);
                    }
                }

                if (gtxHeaderOffset != 0)
                {
                    using (reader.TemporarySeek(gtxHeaderOffset + pos, System.IO.SeekOrigin.Begin))
                    {
                        //Now here is where the GX2 header starts
                        Gx2HeaderData = new NutGX2Surface();
                        Gx2HeaderData.Read(reader);
                        Gx2HeaderData.data = Data;
                        Gx2HeaderData.mipData = MipData;
                        RedChannel = GX2ChanneToGeneric((GX2CompSel)Gx2HeaderData.compSel[0]);
                        GreenChannel = GX2ChanneToGeneric((GX2CompSel)Gx2HeaderData.compSel[1]);
                        BlueChannel = GX2ChanneToGeneric((GX2CompSel)Gx2HeaderData.compSel[2]);
                        AlphaChannel = GX2ChanneToGeneric((GX2CompSel)Gx2HeaderData.compSel[3]);

                        Format = Bfres.Structs.FTEX.ConvertFromGx2Format((GX2SurfaceFormat)Gx2HeaderData.format);
                    }
                }

                //Seek back for next image
                reader.Seek(pos + HeaderSize, System.IO.SeekOrigin.Begin);
            }

            private STChannelType GX2ChanneToGeneric(GX2CompSel comp)
            {
                if (comp == GX2CompSel.ChannelR) return STChannelType.Red;
                else if (comp == GX2CompSel.ChannelG) return STChannelType.Green;
                else if (comp == GX2CompSel.ChannelB) return STChannelType.Blue;
                else if (comp == GX2CompSel.ChannelA) return STChannelType.Alpha;
                else if (comp == GX2CompSel.Always0) return STChannelType.Zero;
                else return STChannelType.One;
            }

            public void Write(FileWriter writer, bool IsNTP3)
            {

            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                throw new NotImplementedException();
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                if (Gx2HeaderData != null)
                {
                    var surfaces = GX2.Decode(Gx2HeaderData);
                    return surfaces[ArrayLevel][MipLevel];
                }
                else
                {
                    uint DataOffset = 0;
                    for (byte arrayLevel = 0; arrayLevel < ArrayCount; ++arrayLevel)
                    {
                        for (byte mipLevel = 0; mipLevel < MipCount; ++mipLevel)
                        {
                            if (ArrayLevel == arrayLevel && MipLevel == mipLevel)
                            {
                                return Utils.SubArray(Data, DataOffset, ImageSizes[mipLevel]);
                            }
                            else
                            {
                                DataOffset += ImageSizes[mipLevel];
                            }
                        }
                    }

                    return null;
                }
            }
        }

        public class GIDX
        {
            public uint HeaderSize;
            public uint HashID;
            public uint Padding;

            public void Read(FileReader reader, bool IsNTP3)
            {
                 var Magic = reader.ReadSignature(4, "GIDX");
                 HeaderSize = reader.ReadUInt32();
                 HashID = reader.ReadUInt32();
                 Padding = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, bool IsNTP3)
            {
                writer.WriteSignature("GIDX");
                writer.Write(HeaderSize);
                writer.Write(HashID);
                writer.Write(Padding);
            }
        }

        public class EXT
        {
            public uint Unknown = 32;
            public uint HeaderSize = 16;
            public uint Padding;

            public void Read(FileReader reader, bool IsNTP3)
            {
                var Magic = reader.ReadSignature(3, "eXt");
                byte padding = reader.ReadByte();
                Unknown = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt32();
                Padding = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, bool IsNTP3)
            {
                writer.WriteSignature("Ext");
                writer.Write((byte)0);
                writer.Write(Unknown);
                writer.Write(HeaderSize);
                writer.Write(Padding);
            }
        }

        public class NutGX2Surface : GX2.GX2Surface
        {
            public NutGX2Surface()
            {
                compSel = new byte[4] { 0,1,2,3,};
            }

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                dim = reader.ReadUInt32();
                width = reader.ReadUInt32();
                height = reader.ReadUInt32();
                depth = reader.ReadUInt32();
                numMips = reader.ReadUInt32();
                format = reader.ReadUInt32();
                aa = reader.ReadUInt32();
                use = reader.ReadUInt32();
                imageSize = reader.ReadUInt32();
                imagePtr = reader.ReadUInt32();
                mipSize = reader.ReadUInt32();
                mipPtr = reader.ReadUInt32();
                tileMode = reader.ReadUInt32();
                swizzle = reader.ReadUInt32();
                alignment = reader.ReadUInt32();
                pitch = reader.ReadUInt32();
                mipOffset = reader.ReadUInt32s(13);
                firstMip = reader.ReadUInt32();
                imageCount = reader.ReadUInt32();
                firstSlice = reader.ReadUInt32();
            }
            public void Write(FileWriter writer)
            {
                writer.Write(dim);
                writer.Write(width);
                writer.Write(height);
                writer.Write(depth);
                writer.Write(numMips);
                writer.Write(format);
                writer.Write(aa);
                writer.Write(use);
                writer.Write(imageSize);
                writer.Write(imagePtr);
                writer.Write(mipSize);
                writer.Write(mipPtr);
                writer.Write(tileMode);
                writer.Write(swizzle);
                writer.Write(alignment);
                writer.Write(pitch);
                writer.Write(mipOffset);
                writer.Write(firstMip);
                writer.Write(imageCount);
                writer.Write(firstSlice);
            }
        }
    }
}
