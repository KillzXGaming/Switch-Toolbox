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
    public class PTCL_3DS
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

            public uint Version;

            public void Read(FileReader reader, PTCL pctl)
            {
                uint Position = (uint)reader.Position; //Offsets are relative to this

                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                string Signature = reader.ReadString(4, Encoding.ASCII);

                if (Signature == "SPBD")
                    IsSPBD = true;

                Version = reader.ReadUInt32();
                uint EmitterCount = reader.ReadUInt32();
                uint Padding = reader.ReadUInt32();
                EffectNameTableOffset = reader.ReadUInt32();
                TextureBlockTableOffset = reader.ReadUInt32();
                TextureBlockTableSize = reader.ReadUInt32();

                if (Version > 11)
                {
                    ShaderGtxTableOffset = reader.ReadUInt32();
                    ShaderGtxTableSize = reader.ReadUInt32();
                    uint KeyAnimationTableOffset = reader.ReadUInt32();
                    uint KeyAnimationTableSize = reader.ReadUInt32();
                    uint PrimativeTableOffset = reader.ReadUInt32();
                    uint PrimativeTableSize = reader.ReadUInt32();
                    uint ShaderParamTableOffset = reader.ReadUInt32();
                    uint ShaderParamTableSize = reader.ReadUInt32();
                    uint TotalTextureTableSize = reader.ReadUInt32();
                    uint Unknown1 = reader.ReadUInt32();
                    uint Unknown2 = reader.ReadUInt32();
                }


                var groupEmitterSets = new TreeNode("Emitter Sets");
                var textureFolder = new TreeNode("Textures");
                pctl.Nodes.Add(textureFolder);
                pctl.Nodes.Add(groupEmitterSets);

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

                foreach (TextureInfo tex in ptcl.header3DS.Textures)
                {
                    //write texture blocks. Some slots are empty so check if it exists
                    tex.Write(writer, this);
                }

                foreach (EmitterSet emitterSets in emitterSets)
                {
                    foreach (Emitter3DS emitter in emitterSets.Nodes)
                    {
                        writer.Seek(((Emitter3DS)emitter).ColorPosition, SeekOrigin.Begin);
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
            public Emitter3DS emitter;
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
                emitter = new Emitter3DS();
                emitter.Read(reader, header);

                reader.Seek(pos, SeekOrigin.Begin);
            }
            public void Write(FileWriter writer)
            {

            }
        }
        public class Emitter3DS : PTCL.Emitter
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

                if (header.Version <= 11)
                {
                    reader.Seek(12);
                }
                else
                {
                    reader.Seek(56);
                }

                uint NameOffset = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();

                if (NameOffset != PTCL.NullOffset)
                {
                    using (reader.TemporarySeek(header.EffectNameTableOffset + NameOffset, SeekOrigin.Begin))
                    {
                        Text = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                    }
                }

                int TextureCount = 2;
                if (header.Version <= 11)
                    TextureCount = 1;

                for (int i = 0; i < TextureCount; i++) //Max of 2 textures. Any more and it'll overlap some data
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
                reader.Seek(pos + 1072, SeekOrigin.Begin);
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
                ftex.ReplaceTexture(FileName, MipCount, SupportedFormats, true, true, true, Format);
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
                            TEX_FORMAT.R8_UNORM,
                            TEX_FORMAT.R8G8_UNORM,
                            TEX_FORMAT.R8G8B8A8_UNORM,
                            TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                            TEX_FORMAT.R8G8_SNORM,
                            TEX_FORMAT.B4G4R4A4_UNORM,
                            TEX_FORMAT.ETC1,
                            TEX_FORMAT.ETC1_A4,
                            TEX_FORMAT.HIL08,
                            TEX_FORMAT.L4,
                            TEX_FORMAT.A4,
                            TEX_FORMAT.L8,
                            TEX_FORMAT.A8_UNORM,
                            TEX_FORMAT.LA8,
                            TEX_FORMAT.LA4,
                            TEX_FORMAT.B5G5R5A1_UNORM,
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
                TC_R8_G8_B8_A8_SRGB = 15,
                TC_R8_SNORM = 16,
                TC_R4_R4_SNORM = 17,
                ETC1_A4 = 18,
                ETC1 = 19,
                HIL08 = 20,
                L4 = 21,
                A4 = 22,
                L8 = 23,
                A8 = 24,
                LA4 = 25,
                LA8 = 26,
                TCS_R5_G5_B5_A1_UNORM = 27,
                TC_R4_G4_B4_UNORM = 28,
                TC_R8_G8_B8_A8_UNORM = 29,
                TC_R8_G8_B8_UNORM = 30,
                TCS_R5_G6_B5_UNORM = 31,
            };

            public uint TileMode;
            public uint Swizzle;
            public byte WrapMode;
            public uint CompSel;
            public uint ImageSize;
            public uint ImageOffset;
            public SurfaceFormat SurfFormat;
            public CTR_3DS.PICASurfaceFormat SurfFormatOld;

            public byte[] data;
            public uint DataPos;

            public bool IsEmpty()
            {
                if (Width == 0 || Height == 0)
                    return true;
                else
                    return false;
            }

            public void Read(FileReader reader, Header header, string EmitterName)
            {
                CanReplace = true;
                CanRename = false;
                PlatformSwizzle = PlatformSwizzle.Platform_3DS;

                if (header.Version <= 11)
                {
                    Width = reader.ReadUInt16();
                    Height = reader.ReadUInt16();
                    ushort Format = reader.ReadUInt16();
                    ushort unk4 = reader.ReadUInt16();
                    SurfFormatOld = (CTR_3DS.PICASurfaceFormat)Format;
                    uint unk = reader.ReadUInt32();
                    ImageSize = reader.ReadUInt32();
                    DataPos = reader.ReadUInt32();
                    uint unk2 = reader.ReadUInt32();
                    uint unk3 = reader.ReadUInt32();
                }
                else
                {
                    Width = reader.ReadUInt16();
                    Height = reader.ReadUInt16();
                    Swizzle = reader.ReadUInt32();
                    uint Alignment = reader.ReadUInt32();
                    uint Pitch = reader.ReadUInt32();
                    WrapMode = reader.ReadByte(); //11 = repeat, 22 = mirror
                    byte unk = reader.ReadByte();
                    byte unk2 = reader.ReadByte();
                    byte unk3 = reader.ReadByte();
                    uint mipCount = reader.ReadUInt32();
                    CompSel = reader.ReadUInt32();
                    uint[] MipOffsets = reader.ReadUInt32s(13);
                    uint[] unk4 = reader.ReadUInt32s(4);

                    uint originalDataFormat = reader.ReadUInt32();
                    uint originalDataPos = reader.ReadUInt32();
                    uint originalDataSize = reader.ReadUInt32();
                    SurfFormat = reader.ReadEnum<SurfaceFormat>(false);
                    ImageSize = reader.ReadUInt32();
                    DataPos = reader.ReadUInt32();
                    uint handle = reader.ReadUInt32();
                }


                ArrayCount = 1;

                if (Width != 0 && Height != 0 && SurfFormat != 0)
                {
                    using (reader.TemporarySeek(header.TextureBlockTableOffset + DataPos, SeekOrigin.Begin))
                    {
                        data = reader.ReadBytes((int)ImageSize);
                    }
                }

                if (data != null && data.Length > 0)
                {
                    ConvertFormat(header.Version);
                }
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

            private void ConvertFormat(uint Version)
            {
                if (Version <= 11)
                {
                    Format = CTR_3DS.ConvertPICAToGenericFormat(SurfFormatOld);
                }
                else
                {

                    switch (SurfFormat)
                    {
                        case SurfaceFormat.T_BC1_UNORM:
                            Format = TEX_FORMAT.BC1_UNORM;
                            break;
                        case SurfaceFormat.T_BC1_SRGB:
                            Format = TEX_FORMAT.BC1_UNORM_SRGB;
                            break;
                        case SurfaceFormat.T_BC2_UNORM:
                            Format = TEX_FORMAT.BC2_UNORM;
                            break;
                        case SurfaceFormat.T_BC2_SRGB:
                            Format = TEX_FORMAT.BC2_UNORM_SRGB;
                            break;
                        case SurfaceFormat.T_BC3_UNORM:
                            Format = TEX_FORMAT.BC3_UNORM;
                            break;
                        case SurfaceFormat.T_BC3_SRGB:
                            Format = TEX_FORMAT.BC3_UNORM_SRGB;
                            break;
                        case SurfaceFormat.T_BC4_UNORM:
                            Format = TEX_FORMAT.BC4_UNORM;
                            break;
                        case SurfaceFormat.T_BC4_SNORM:
                            Format = TEX_FORMAT.BC4_SNORM;
                            break;
                        case SurfaceFormat.T_BC5_UNORM:
                            Format = TEX_FORMAT.BC5_UNORM;
                            break;
                        case SurfaceFormat.T_BC5_SNORM:
                            Format = TEX_FORMAT.BC5_SNORM;
                            break;
                        case SurfaceFormat.TC_R8_G8_UNORM:
                            Format = TEX_FORMAT.R8G8_UNORM;
                            break;
                        case SurfaceFormat.TC_R8_G8_B8_A8_SRGB:
                            Format = TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                            break;
                        case SurfaceFormat.TCS_R8_G8_B8_A8:
                            Format = TEX_FORMAT.R8G8B8A8_UNORM;
                            break;
                        case SurfaceFormat.TC_R8_UNORM:
                            Format = TEX_FORMAT.R8_UNORM;
                            break;
                        case SurfaceFormat.TCS_R5_G6_B5_UNORM:
                            Format = TEX_FORMAT.B5G6R5_UNORM;
                            break;
                        case SurfaceFormat.ETC1:
                            Format = TEX_FORMAT.ETC1;
                            break;
                        case SurfaceFormat.ETC1_A4:
                            Format = TEX_FORMAT.ETC1_A4;
                            break;
                        case SurfaceFormat.L4:
                            Format = TEX_FORMAT.L4;
                            break;
                        case SurfaceFormat.L8:
                            Format = TEX_FORMAT.L8;
                            break;
                        case SurfaceFormat.LA4:
                            Format = TEX_FORMAT.LA4;
                            break;
                        case SurfaceFormat.LA8:
                            Format = TEX_FORMAT.LA8;
                            break;
                        case SurfaceFormat.HIL08:
                            Format = TEX_FORMAT.HIL08;
                            break;
                        case SurfaceFormat.TC_R8_G8_B8_A8_UNORM:
                            Format = TEX_FORMAT.R8G8B8A8_UNORM;
                            break;
                        case SurfaceFormat.TC_R4_G4_B4_UNORM:
                            Format = TEX_FORMAT.B4G4R4A4_UNORM;
                            break;
                        case SurfaceFormat.TC_R8_G8_B8_UNORM:
                            Format = TEX_FORMAT.R8G8B8A8_UNORM;
                            break;
                        case SurfaceFormat.TC_R4_R4_SNORM:
                            Format = TEX_FORMAT.R4G4_UNORM;
                            break;
                        case SurfaceFormat.A4:
                            Format = TEX_FORMAT.A4;
                            break;
                        default:
                            throw new Exception("Format unsupported! " + SurfFormat);
                    }
                }
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return data;
            }

            public void Write(FileWriter writer)
            {

            }
        }
    }
}
