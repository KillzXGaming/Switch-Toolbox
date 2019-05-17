using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using System.IO;
using Syroot.BinaryData;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
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
                var textureFolder = new TreeNode("Textures");
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

                reader.Dispose();
                reader.Close();
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
                        foreach (var color0 in emitter.Color0Array)
                        {
                            writer.Write(color0.R);
                            writer.Write(color0.G);
                            writer.Write(color0.B);
                            writer.Write(color0.A);
                        }
                        foreach (var color1 in emitter.Color1Array)
                        {
                            writer.Write(color1.R);
                            writer.Write(color1.G);
                            writer.Write(color1.B);
                            writer.Write(color1.A);
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
                EmitterEditor editor = (EmitterEditor)LibraryGUI.Instance.GetActiveContent(typeof(EmitterEditor));
                if (editor == null)
                {
                    editor = new EmitterEditor();
                    LibraryGUI.Instance.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadEmitter(this);
            }

            public void Read(FileReader reader, Header header)
            {
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
                    ColorData clr = new ColorData();
                    clr.R = reader.ReadSingle();
                    clr.G = reader.ReadSingle();
                    clr.B = reader.ReadSingle();
                    clr.A = reader.ReadSingle();
                    Color0Array[i] = clr;

                    int red = Utils.FloatToIntClamp(clr.R);
                    int green = Utils.FloatToIntClamp(clr.G);
                    int blue = Utils.FloatToIntClamp(clr.B);
                    int alpha = Utils.FloatToIntClamp(clr.A);

                    //          Console.WriteLine($"Color0 {i} R {R} G {G} B {B} A {A}");
                    //         Console.WriteLine($"Color0 {i} R {red} G {green} B {blue} A {alpha}");

                    Color0s[i] = Color.FromArgb(alpha, red, green, blue);
                }
                for (int i = 0; i < 8; i++)
                {
                    ColorData clr = new ColorData();
                    clr.R = reader.ReadSingle();
                    clr.G = reader.ReadSingle();
                    clr.B = reader.ReadSingle();
                    clr.A = reader.ReadSingle();
                    Color1Array[i] = clr;

                    int red = Utils.FloatToIntClamp(clr.R);
                    int green = Utils.FloatToIntClamp(clr.G);
                    int blue = Utils.FloatToIntClamp(clr.B);
                    int alpha = Utils.FloatToIntClamp(clr.A);

                    //      Console.WriteLine($"Color1 {i} R {R} G {G} B {B} A {A}");
                    //    Console.WriteLine($"Color1 {i} R {red} G {green} B {blue} A {alpha}");

                    Color1s[i] = Color.FromArgb(alpha, red, green, blue);
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
                ftex.ReplaceTexture(FileName, Format, MipCount, SupportedFormats, true, true, true);
                if (ftex.texture != null)
                {
                    byte[] ImageData = ftex.texture.Data;


                    if (ftex.texture.MipData != null)
                        ImageData = Utils.CombineByteArray(ftex.texture.Data, ftex.texture.MipData);

                    //  if (ImageData.Length != size)
                    // MessageBox.Show($"Image size does not match! Make sure mip map count, format, height and width are all the same! Original Size {size} Import {ImageData.Length}", );

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
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.Instance.LoadEditor(editor);
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
                            TEX_FORMAT.B5_G5_R5_A1_UNORM,
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

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
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
                surf.numMips = MipCount;
                surf.mipOffset = new uint[MipCount];
                surf.mipData = null;
                surf.tileMode = TileMode;
                surf.swizzle = Swizzle;
                surf.imageSize = ImageSize;

                // GX2.GenerateMipSurfaceData(surf);

                foreach (var mipoffset in surf.mipOffset)
                {
                    Console.WriteLine($"mipoffset {mipoffset}");
                }

                var surfaces = GX2.Decode(surf);
                return surfaces[ArrayLevel][MipLevel];
            }

            public void Write(FileWriter writer)
            {

            }
        }
    }
}
