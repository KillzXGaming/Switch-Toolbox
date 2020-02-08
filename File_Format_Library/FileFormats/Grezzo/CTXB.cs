using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class CTXB : TreeNodeFile, IFileFormat, ITextureContainer
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CTXB" };
        public string[] Extension { get; set; } = new string[] { "*.ctxb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "ctxb");
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

        public bool DisplayIcons => true;

        public List<STGenericTexture> TextureList
        {
            get
            {
                List<STGenericTexture> textures = new List<STGenericTexture>();
                foreach (STGenericTexture node in Nodes)
                    textures.Add(node);

                return textures;
            }
            set { }
        }

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream), this);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream) {
            header.Write(new FileWriter(stream));
        }

        public class Header
        {
            public List<Chunk> Chunks = new List<Chunk>();

            public void Read(FileReader reader, CTXB ctxb)
            {
                string Magic = reader.ReadSignature(4, "ctxb");
                uint FileSize = reader.ReadUInt32();
                uint ChunkCount = reader.ReadUInt32();
                reader.ReadUInt32(); //padding
                uint ChunkOffset = reader.ReadUInt32();
                uint TextureDataOffset = reader.ReadUInt32();

                for (int i = 0; i < ChunkCount; i++)
                    Chunks.Add(new Chunk(reader));

                for (int i = 0; i < ChunkCount; i++)
                {
                    for (int t = 0; t < Chunks[i].Textures.Count; t++)
                    {
                        var texWrapper = new TextureWrapper();
                        texWrapper.Text = $"Texture_{t}";
                        texWrapper.ImageKey = "texture";
                        texWrapper.SelectedImageKey = texWrapper.ImageKey;
                        texWrapper.TextureInfo = Chunks[i].Textures[t];

                        if (Chunks[i].Textures[t].Name != string.Empty)
                            texWrapper.Text = Chunks[i].Textures[t].Name;

                        texWrapper.Width = Chunks[i].Textures[t].Width;
                        texWrapper.Height = Chunks[i].Textures[t].Height;
                        texWrapper.Format = CTR_3DS.ConvertPICAToGenericFormat(Chunks[i].Textures[t].PicaFormat);

                        reader.SeekBegin(TextureDataOffset + Chunks[i].Textures[t].DataOffset);
                        Chunks[i].Textures[t].ImageData = reader.ReadBytes((int)Chunks[i].Textures[t].ImageSize);
                        ctxb.Nodes.Add(texWrapper);
                    }
                }
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("ctxb");
                writer.Write(uint.MaxValue);
                writer.Write(Chunks.Count);
                writer.Write(0);
                writer.Write(24);
                writer.Write(24 + Chunks.Count * 8 + (Chunks.Sum(x => x.Textures.Count) * 40));

                foreach (var chunk in Chunks) {
                    long pos = writer.Position;
                    writer.WriteSignature(chunk.Magic);
                    writer.Write(uint.MaxValue);
                    writer.Write(chunk.Textures.Count);
                    foreach (var tex in chunk.Textures)
                        tex.Write(writer);

                    writer.WriteSectionSizeU32(pos + 4, pos, writer.Position);
                }

                uint dataOffset = 0;
                foreach (var chunk in Chunks)
                {
                    foreach (var tex in chunk.Textures) {
                        tex.DataOffset = dataOffset;
                        writer.Write(tex.ImageData);

                        dataOffset += (uint)tex.ImageData.Length;
                    }
                }

                using (writer.TemporarySeek(4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }

            public class Chunk
            {
                public readonly string Magic = "tex ";

                public List<Texture> Textures = new List<Texture>();

                public Chunk(FileReader reader)
                {
                    reader.ReadSignature(4, Magic);
                    uint SectionSize = reader.ReadUInt32();
                    uint TextureCount = reader.ReadUInt32();
                    for (int i = 0; i < TextureCount; i++)
                        Textures.Add(new Texture(reader));
                }
            }
        }

        public class Texture
        {
            public ushort MaxLevel { get; set; }
            public ushort Unknown { get; set; }
            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public string Name { get; set; }

            public uint ImageSize { get; set; }
            public uint DataOffset { get; set; }

            public CTR_3DS.PICASurfaceFormat PicaFormat;

            public byte[] ImageData;

            public enum TextureFormat : uint
            {
                ETC1 = 0x0000675A,
                ETC1A4 = 0x0000675B,
                RGBA8 = 0x14016752,
                RGBA4444 = 0x80336752,
                RGBA5551 = 0x80346752,
                RGB565 = 0x83636754,
                RGB8 = 0x14016754,
                A8 = 0x14016756,
                L8 = 0x14016757,
                L4 = 0x67616757,
                LA8 = 0x14016758,
            }

            public Texture(FileReader reader)
            {
                ImageSize = reader.ReadUInt32();
                MaxLevel = reader.ReadUInt16();
                Unknown = reader.ReadUInt16();
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                TextureFormat Format = reader.ReadEnum<TextureFormat>(true);
                DataOffset = reader.ReadUInt32();
                Name = reader.ReadString(16).TrimEnd('\0');

                PicaFormat = FormatList[Format];
            }

            public void Write(FileWriter writer)
            {
                TextureFormat format = FormatList.FirstOrDefault(x => x.Value == PicaFormat).Key;

                writer.Write(ImageData.Length);
                writer.Write(MaxLevel);
                writer.Write(Unknown);
                writer.Write((ushort)Width);
                writer.Write((ushort)Height);
                writer.Write(format, true);
                writer.Write(DataOffset);
                writer.WriteString(Name, 16);
            }

            private static Dictionary<TextureFormat, CTR_3DS.PICASurfaceFormat> FormatList =
                new Dictionary<TextureFormat, CTR_3DS.PICASurfaceFormat>()
                {
                    { TextureFormat.A8, CTR_3DS.PICASurfaceFormat.A8 },
                    { TextureFormat.ETC1, CTR_3DS.PICASurfaceFormat.ETC1 },
                    { TextureFormat.ETC1A4, CTR_3DS.PICASurfaceFormat.ETC1A4 },
                    { TextureFormat.L4, CTR_3DS.PICASurfaceFormat.L4 },
                    { TextureFormat.L8, CTR_3DS.PICASurfaceFormat.L8 },
                    { TextureFormat.LA8, CTR_3DS.PICASurfaceFormat.LA8 },
                    { TextureFormat.RGB565, CTR_3DS.PICASurfaceFormat.RGB565 },
                    { TextureFormat.RGBA4444, CTR_3DS.PICASurfaceFormat.RGBA4 },
                    { TextureFormat.RGBA5551, CTR_3DS.PICASurfaceFormat.RGBA5551 },
                    { TextureFormat.RGBA8, CTR_3DS.PICASurfaceFormat.RGBA8 },
                    { TextureFormat.RGB8, CTR_3DS.PICASurfaceFormat.RGB8 },
                };
        }

        public class TextureWrapper : STGenericTexture
        {
            public Texture TextureInfo;

            public byte[] ImageData
            {
                get { return TextureInfo.ImageData; }
                set { TextureInfo.ImageData = value; }
            }

            public override bool CanEdit { get; set; } = true;

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                    TEX_FORMAT.B5G6R5_UNORM,
                    TEX_FORMAT.R8G8_UNORM,
                    TEX_FORMAT.B5G5R5A1_UNORM,
                    TEX_FORMAT.B4G4R4A4_UNORM,
                    TEX_FORMAT.LA8,
                    TEX_FORMAT.HIL08,
                    TEX_FORMAT.L8,
                    TEX_FORMAT.A8_UNORM,
                    TEX_FORMAT.LA4,
                    TEX_FORMAT.A4,
                    TEX_FORMAT.ETC1_UNORM,
                    TEX_FORMAT.ETC1_A4,
                };
                }
            }

            public TextureWrapper()
            {
                PlatformSwizzle = PlatformSwizzle.Platform_3DS;

                CanReplace = true;
                CanRename = true;
                CanDelete = true;
            }

            public override void OnClick(TreeView treeview)
            {
                UpdateEditor();
            }

            private void UpdateEditor()
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

            public override void Replace(string FileName)
            {
                CTR_3DSTextureImporter importer = new CTR_3DSTextureImporter();
                CTR_3DSImporterSettings settings = new CTR_3DSImporterSettings();

                if (Utils.GetExtension(FileName) == ".dds" ||
                    Utils.GetExtension(FileName) == ".dds2")
                {
                    settings.LoadDDS(FileName);
                    importer.LoadSettings(new List<CTR_3DSImporterSettings>() { settings, });

                    ApplySettings(settings);
                    UpdateEditor();
                }
                else
                {
                    settings.LoadBitMap(FileName);
                    importer.LoadSettings(new List<CTR_3DSImporterSettings>() { settings, });

                    if (importer.ShowDialog() == DialogResult.OK)
                    {
                        if (settings.GenerateMipmaps && !settings.IsFinishedCompressing)
                        {
                            settings.DataBlockOutput.Clear();
                            settings.DataBlockOutput.Add(settings.GenerateMips());
                        }

                        Console.WriteLine($"ImageSize {this.ImageData.Length} {settings.DataBlockOutput[0]}");

                        ApplySettings(settings);
                        UpdateEditor();
                    }
                }
            }

            private void ApplySettings(CTR_3DSImporterSettings settings)
            {
                this.ImageData = settings.DataBlockOutput[0];
                this.Width = settings.TexWidth;
                this.Height = settings.TexHeight;
                this.Format = settings.GenericFormat;
                this.MipCount = settings.MipCount;
                this.Depth = settings.Depth;
                this.ArrayCount = (uint)settings.DataBlockOutput.Count;
            }

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
            {
                return ImageData;
            }
        }
    }
}
