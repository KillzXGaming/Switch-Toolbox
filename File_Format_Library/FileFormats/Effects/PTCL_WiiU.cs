using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using Toolbox.Library.IO;
using Toolbox.Library;
using System.IO;
using Syroot.BinaryData;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Bfres.Structs;

namespace FirstPlugin
{
    public class PTCL_WiiU
    {
        public class Header
        {
            public List<EmitterSet> emitterSets = new List<EmitterSet>();
            public List<TextureInfo> Textures = new List<TextureInfo>();

            public bool IsSPBD = false;

            public uint EffectNameTableOffset;
            public uint TextureBlockTableOffset;
            public uint TextureBlockTableSize;
            public uint ShaderGtxTableOffset;
            public uint ShaderGtxTableSize;

            public void Read(FileReader reader, PTCL pctl)
            {
                uint Position = (uint)reader.Position; //Offsets are relative to this

                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                string Signature = reader.ReadString(4, Encoding.ASCII);

                if (Signature == "SPBD")
                    IsSPBD = true;

                uint Version = reader.ReadUInt32();
                uint EmitterCount = reader.ReadUInt32();
                uint Padding = reader.ReadUInt32();
                EffectNameTableOffset = reader.ReadUInt32();
                TextureBlockTableOffset = reader.ReadUInt32();
                TextureBlockTableSize = reader.ReadUInt32();
                ShaderGtxTableOffset = reader.ReadUInt32();
                ShaderGtxTableSize = reader.ReadUInt32();
                uint KeyAnimationTableOffset = reader.ReadUInt32();
                uint KeyAnimationTableSize = reader.ReadUInt32();
                uint PrimativeTableOffset = reader.ReadUInt32();
                uint PrimativeTableSize = reader.ReadUInt32();
                uint ShaderParamTableOffset = reader.ReadUInt32();
                uint ShaderParamTableSize = reader.ReadUInt32();

                var groupEmitterSets = new TreeNode("Emitter Sets");
                var textureFolder = new PTCL.Header.TextureFolder("Textures");
                pctl.Nodes.Add(textureFolder);
                pctl.Nodes.Add(groupEmitterSets);

                if (IsSPBD)
                    reader.Seek(0x40, SeekOrigin.Begin);
                else
                    reader.Seek(72, SeekOrigin.Begin);
                for (int i = 0; i < EmitterCount; i++)
                {
                    EmitterSet emitterSet = new EmitterSet();
                    emitterSet.Read(reader, this);
                    emitterSets.Add(emitterSet);
                    groupEmitterSets.Nodes.Add(emitterSet);
                }

                int index = 0;
                foreach (var tex in Textures)
                {
                    tex.Text = $"Texture{index++}";
                    textureFolder.Nodes.Add(tex);
                }
            }
            public void Write(FileWriter writer, PTCL ptcl)
            {
                writer.ByteOrder = ByteOrder.BigEndian;
                writer.Write(ptcl.data.ToArray());

                foreach (TextureInfo tex in ptcl.headerU.Textures)
                {
                    //write texture blocks. Some slots are empty so check if it exists
                    tex.Write(writer, this);
                }

                foreach (EmitterSet emitterSets in emitterSets)
                {
                    foreach (EmitterU emitter in emitterSets.Nodes)
                    {
                        writer.Seek(((EmitterU)emitter).ColorPosition, SeekOrigin.Begin);
                        for (int i = 0; i < emitter.Color0Array.Length; i++)
                        {
                            writer.Write(emitter.Color0Array[i].R);
                            writer.Write(emitter.Color0Array[i].G);
                            writer.Write(emitter.Color0Array[i].B);
                            writer.Write(emitter.Color0AlphaArray[i].R);
                        }
                        for (int i = 0; i < emitter.Color1Array.Length; i++)
                        {
                            writer.Write(emitter.Color1Array[i].R);
                            writer.Write(emitter.Color1Array[i].G);
                            writer.Write(emitter.Color1Array[i].B);
                            writer.Write(emitter.Color1AlphaArray[i].R);
                        }
                    }
                }

                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        public class EmitterSet : TreeNodeCustom
        {
            public void Read(FileReader reader, Header header)
            {
                uint NameOffset = 0;
                uint EmitterCount = 0;
                uint EmitterTableOffset = 0;

                if (header.IsSPBD)
                {
                    uint Description = reader.ReadUInt32();
                    uint Unknown = reader.ReadUInt32();
                    NameOffset = reader.ReadUInt32();
                    uint NamePointer = reader.ReadUInt32();
                    EmitterCount = reader.ReadUInt32();
                    EmitterTableOffset = reader.ReadUInt32();
                    uint Unknown2 = reader.ReadUInt32();
                }
                else
                {
                    uint padding = reader.ReadUInt32();
                    uint padding2 = reader.ReadUInt32();
                    NameOffset = reader.ReadUInt32();
                    uint padding3 = reader.ReadUInt32();
                    EmitterCount = reader.ReadUInt32();
                    EmitterTableOffset = reader.ReadUInt32();
                    uint padding4 = reader.ReadUInt32();
                }

                using (reader.TemporarySeek(header.EffectNameTableOffset + NameOffset, SeekOrigin.Begin))
                {
                    Text = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                }

                long pos = reader.Position;

                reader.Seek(EmitterTableOffset, SeekOrigin.Begin);
                for (int i = 0; i < EmitterCount; i++)
                {
                    EmitterTable table = new EmitterTable();
                    table.Read(reader, header);
                    Nodes.Add(table.emitter);
                }
                reader.Seek(pos, SeekOrigin.Begin);
            }
            public void Write(FileWriter writer)
            {

            }
        }
        public class EmitterTable
        {
            public EmitterU emitter;
            public uint EmitterOffset;

            public void Read(FileReader reader, Header header)
            {
                if (header.IsSPBD)
                {
                    EmitterOffset = reader.ReadUInt32();
                    uint padding = reader.ReadUInt32();
                }
                else
                {
                    EmitterOffset = reader.ReadUInt32();
                    uint padding = reader.ReadUInt32();
                    uint padding2 = reader.ReadUInt32();
                    uint padding3 = reader.ReadUInt32();
                }

                long pos = reader.Position;

                reader.Seek(EmitterOffset, SeekOrigin.Begin);
                emitter = new EmitterU();
                emitter.Read(reader, header);

                reader.Seek(pos, SeekOrigin.Begin);
            }
            public void Write(FileWriter writer)
            {

            }
        }
        public class EmitterU : PTCL.Emitter
        {
            public long ColorPosition;

            public override void OnClick(TreeView treeview)
            {
                EmitterEditorNX editor = (EmitterEditorNX)LibraryGUI.GetActiveContent(typeof(EmitterEditorNX));
                if (editor == null)
                {
                    editor = new EmitterEditorNX();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadEmitter(this);
            }

            public void Read(FileReader reader, Header header)
            {
                Color0Array = new STColor[8];
                Color1Array = new STColor[8];
                Color0AlphaArray = new STColor[8];
                Color1AlphaArray = new STColor[8];
                ConstantColor0 = new STColor();
                ConstantColor1 = new STColor();

                Color0KeyCount = 8;
                Alpha0KeyCount = 8;
                Color1KeyCount = 8;
                Alpha1KeyCount = 8;

                Color0Type = ColorType.Random;
                Color1Type = ColorType.Random;
                Alpha0Type = ColorType.Random;
                Alpha1Type = ColorType.Random;

                long pos = reader.Position;

                reader.Seek(56);
                uint NameOffset = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();

                if (NameOffset != PTCL.NullOffset)
                {
                    using (reader.TemporarySeek(header.EffectNameTableOffset + NameOffset, SeekOrigin.Begin))
                    {
                        Text = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                    }
                }

                uint TextureAmount = 3;
                if (header.IsSPBD)
                    TextureAmount = 2;

                for (int i = 0; i < TextureAmount; i++) //Max of 3 or 2 textures depending on version. Seems to fill in texture info even if unused
                {
                    TextureInfo textureInfo = new TextureInfo();
                    textureInfo.Read(reader, header, Text);

                    if (!textureInfo.IsEmpty())
                    {
                        DrawableTex.Add(textureInfo);

                        bool HasImage = header.Textures.Any(item => item.DataPos == textureInfo.DataPos);
                        if (!HasImage)
                        {
                            header.Textures.Add(textureInfo);
                        }
                    }
                }
                if (header.IsSPBD)
                    reader.Seek(pos + 1358, SeekOrigin.Begin);
                else
                    reader.Seek(pos + 1616, SeekOrigin.Begin);
                ColorPosition = reader.Position;
                for (int i = 0; i < 8; i++)
                {
                    STColor clr = new STColor();
                    clr.R = reader.ReadSingle();
                    clr.G = reader.ReadSingle();
                    clr.B = reader.ReadSingle();
                    float A = reader.ReadSingle();
                    STColor alpha = new STColor();
                    alpha.R = A;
                    alpha.G = A;
                    alpha.B = A;
                    Color0Array[i] = clr;
                    Color0AlphaArray[i] = alpha;
                }
                for (int i = 0; i < 8; i++)
                {
                    STColor clr = new STColor();
                    clr.R = reader.ReadSingle();
                    clr.G = reader.ReadSingle();
                    clr.B = reader.ReadSingle();
                    float A = reader.ReadSingle();

                    STColor alpha = new STColor();
                    alpha.R = A;
                    alpha.G = A;
                    alpha.B = A;

                    Color1Array[i] = clr;
                    Color1AlphaArray[i] = alpha;
                }
            }

            public void Write(FileWriter writer)
            {

            }
        }
        public class TextureInfo : STGenericTexture
        {
            public const uint Alignment = 8192;

            public TextureInfo()
            {
                ImageKey = "Texture";
                SelectedImageKey = "Texture";
            }

            public override void OnClick(TreeView treeView)
            {
                UpdateEditor();
            }

            public override string ExportFilter => FileFilters.GTX;
            public override string ReplaceFilter => FileFilters.GTX;

            public override void Export(string FileName)
            {
                Export(FileName, false, false, 0, 0);
            }

            public override void Replace(string FileName)
            {
                int size = data.Length;

                FTEX ftex = new FTEX();
                ftex.ReplaceTexture(FileName, Format, MipCount,0, SupportedFormats, true, true, true);
                if (ftex.texture != null)
                {
                    byte[] ImageData = ftex.texture.Data;

                    if (ftex.texture.MipData != null)
                        ImageData = Utils.CombineByteArray(ftex.texture.Data, ftex.texture.MipData);

                    if (ImageData.Length != size)
                        MessageBox.Show($"Image size does not match! Make sure mip map count, format, height and width are all the same! Original Size {size} Import {ImageData.Length}");

                    Swizzle = (byte)ftex.texture.Swizzle;

                    byte[] NewData = new byte[size];
                    Array.Copy(ImageData, 0, NewData, 0, size);

                    data = NewData;

                    UpdateEditor();
                }
            }

            public byte[] AlignData(byte[] data)
            {
                using (var mem = new MemoryStream(data))
                {
                    using (var writer = new FileWriter(mem))
                    {
                        writer.Write(data);
                        writer.Align((int)Alignment);

                        return mem.ToArray();
                    }
                }
            }

            public void UpdateEditor()
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }

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
                            TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                            TEX_FORMAT.R8G8B8A8_UNORM,
                            TEX_FORMAT.R8_UNORM,
                            TEX_FORMAT.R8G8_UNORM,
                            TEX_FORMAT.R32G8X24_FLOAT,
                    };
                }
            }

            public override bool CanEdit { get; set; } = false;

            public enum SurfaceFormat
            {
                INVALID = 0x0,
                TCS_R8_G8_B8_A8 = 2,
                T_BC1_UNORM = 3,
                T_BC1_SRGB = 4,
                T_BC2_UNORM = 5,
                T_BC2_SRGB = 6,
                T_BC3_UNORM = 7,
                T_BC3_SRGB = 8,
                T_BC4_UNORM = 9,
                T_BC4_SNORM = 10,
                T_BC5_UNORM = 11,
                T_BC5_SNORM = 12,
                TC_R8_UNORM = 13,
                TC_R8_G8_UNORM = 14,
                TCS_R8_G8_B8_A8_UNORM = 15,
                TCS_R5_G6_B5_UNORM = 25,
            };

            public uint TileMode;
            public uint Swizzle;
            public byte WrapMode;
            public byte[] CompSel;
            public uint ImageSize;
            public uint ImageOffset;
            public SurfaceFormat SurfFormat;
            public byte[] data;
            public uint DataPos;

            public bool IsEmpty()
            {
                if (Width == 0 || Height == 0 || SurfFormat == 0)
                    return true;
                else
                    return false;
            }

            public void Read(FileReader reader, Header header, string EmitterName)
            {
                CanReplace = true;
                CanRename = false;


                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                TileMode = reader.ReadUInt32();
                Swizzle = reader.ReadUInt32();

                if (header.IsSPBD)
                {
                    uint Alignment = reader.ReadUInt32();
                    uint Pitch = reader.ReadUInt32();
                    WrapMode = reader.ReadByte(); //11 = repeat, 22 = mirror
                    byte unk = reader.ReadByte();
                    byte unk2 = reader.ReadByte();
                    byte unk3 = reader.ReadByte();
                    uint mipCount = reader.ReadUInt32();
                    CompSel = reader.ReadBytes(4);
                    uint[] MipOffsets = reader.ReadUInt32s(13);
                    uint enableMipLevel = reader.ReadUInt32();
                    uint mipBias = reader.ReadUInt32();
                    uint originalDataFormat = reader.ReadUInt32();
                    uint originalDataPos = reader.ReadUInt32();
                    uint originalDataSize = reader.ReadUInt32();
                    SurfFormat = reader.ReadEnum<SurfaceFormat>(false);
                    ImageSize = reader.ReadUInt32();
                    DataPos = reader.ReadUInt32();
                }
                else
                {
                    WrapMode = reader.ReadByte(); //11 = repeat, 22 = mirror
                    byte unk = reader.ReadByte();
                    Depth = reader.ReadByte();
                    byte unk1 = reader.ReadByte();
                    uint mipCount = reader.ReadUInt32();
                    if (mipCount < 14)
                        MipCount = mipCount;

                    CompSel = reader.ReadBytes(4);
                    uint enableMipLevel = reader.ReadUInt32();
                    uint mipBias = reader.ReadUInt32();
                    uint originalDataFormat = reader.ReadUInt32();
                    uint originalDataPos = reader.ReadUInt32();
                    uint originalDataSize = reader.ReadUInt32();
                    SurfFormat = reader.ReadEnum<SurfaceFormat>(false);
                    ImageSize = reader.ReadUInt32();
                    DataPos = reader.ReadUInt32();
                }

                RedChannel = SetChannel(CompSel[0]);
                GreenChannel = SetChannel(CompSel[1]);
                BlueChannel = SetChannel(CompSel[2]);
                AlphaChannel = SetChannel(CompSel[3]);

                ArrayCount = 1;
                Depth = 1;

                if (Width != 0 && Height != 0 && SurfFormat != 0)
                {
                    using (reader.TemporarySeek(header.TextureBlockTableOffset + DataPos, SeekOrigin.Begin))
                    {
                        data = reader.ReadBytes((int)ImageSize);
                    }
                }

                if (data != null && data.Length > 0)
                {
                    ConvertFormat();
                }

                reader.Seek(164, SeekOrigin.Current);
            }

            private STChannelType SetChannel(int comp)
            {
                if (comp == 0) return STChannelType.Red;
                else if (comp == 1) return STChannelType.Green;
                else if (comp == 2) return STChannelType.Blue;
                else if (comp == 3) return STChannelType.Alpha;
                else if (comp == 4) return STChannelType.Zero;
                else return STChannelType.One;
            }

            public void Write(FileWriter writer, Header header)
            {
                if (data != null && data.Length > 0)
                {
                    using (writer.TemporarySeek(header.TextureBlockTableOffset + DataPos, SeekOrigin.Begin))
                    {
                        writer.Write(data);
                    }
                }
            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                throw new NotImplementedException("Cannot set image data! Operation not implemented!");
            }

            uint GX2Format = 0;

            private void ConvertFormat()
            {
                switch (SurfFormat)
                {
                    case SurfaceFormat.T_BC1_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC1_UNORM;
                        Format = TEX_FORMAT.BC1_UNORM;
                        break;
                    case SurfaceFormat.T_BC1_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC1_SRGB;
                        Format = TEX_FORMAT.BC1_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC2_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC2_UNORM;
                        Format = TEX_FORMAT.BC2_UNORM;
                        break;
                    case SurfaceFormat.T_BC2_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC2_SRGB;
                        Format = TEX_FORMAT.BC2_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC3_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC3_UNORM;
                        Format = TEX_FORMAT.BC3_UNORM;
                        break;
                    case SurfaceFormat.T_BC3_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC3_SRGB;
                        Format = TEX_FORMAT.BC3_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC4_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC4_UNORM;
                        Format = TEX_FORMAT.BC4_UNORM;
                        break;
                    case SurfaceFormat.T_BC4_SNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC4_SNORM;
                        Format = TEX_FORMAT.BC4_SNORM;
                        break;
                    case SurfaceFormat.T_BC5_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_UNORM;
                        Format = TEX_FORMAT.BC5_UNORM;
                        break;
                    case SurfaceFormat.T_BC5_SNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_SNORM;
                        Format = TEX_FORMAT.BC5_SNORM;
                        break;
                    case SurfaceFormat.TC_R8_G8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TC_R8_G8_UNORM;
                        Format = TEX_FORMAT.R8G8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R8_G8_B8_A8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
                        Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R8_G8_B8_A8:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
                        Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        break;
                    case SurfaceFormat.TC_R8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TC_R8_UNORM;
                        Format = TEX_FORMAT.R8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R5_G6_B5_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R5_G6_B5_UNORM;
                        Format = TEX_FORMAT.B5G6R5_UNORM;
                        break;
                    default:
                        throw new Exception("Format unsupported! " + SurfFormat);
                }
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
            {
                int swizzle = (int)Swizzle;
                int pitch = (int)0;
                uint bpp = GX2.surfaceGetBitsPerPixel(GX2Format) >> 3;

                GX2.GX2Surface surf = new GX2.GX2Surface();
                surf.bpp = bpp;
                surf.height = Height;
                surf.width = Width;
                surf.aa = (uint)0;
                surf.alignment = Alignment;
                surf.depth = Depth;
                surf.dim = 0x1;
                surf.format = GX2Format;
                surf.use = 0x1;
                surf.pitch = 0;
                surf.data = data;
                surf.mipData = data;
                surf.numMips = MipCount;
                surf.tileMode = TileMode;
                surf.swizzle = Swizzle;
                surf.imageSize = ImageSize;

                // GX2.GenerateMipSurfaceData(surf);

                return GX2.Decode(surf, ArrayLevel, MipLevel);
            }

            public void Write(FileWriter writer)
            {

            }
        }
    }
}
