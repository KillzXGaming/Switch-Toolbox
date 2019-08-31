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
using System.Runtime.InteropServices;

namespace FirstPlugin
{
    public class WTB : TreeNodeFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "WTA Texture" };
        public string[] Extension { get; set; } = new string[] { "*.wta" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(3, "WTB");
            }
        }

        public List<STGenericTexture> IconTextureList
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

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(false);

                uint magic = reader.ReadUInt32();
                uint version = reader.ReadUInt32();
                uint numTextures = reader.ReadUInt32();

                uint dataOffsetTable = reader.ReadUInt32();
                uint dataSizeTable = reader.ReadUInt32();
                uint unkTable = reader.ReadUInt32();
                uint unk2Table = reader.ReadUInt32();
                uint textureInfoTable = reader.ReadUInt32();

                List<TextureEntry> entries = new List<TextureEntry>();
                for (int i = 0; i < numTextures; i++)
                {
                    TextureEntry tex = new TextureEntry();

                    //Get data offset
                    reader.SeekBegin(dataOffsetTable + (i * 4));
                    tex.DataOffset = reader.ReadUInt32();

                    //Get data size
                    reader.SeekBegin(dataSizeTable + (i * 4));
                    tex.DataSize = reader.ReadUInt32();

                    //Get tex info
                    reader.SeekBegin(textureInfoTable + (i * 56));
                    tex.Info = reader.ReadStruct<TextureInfo>();
                    entries.Add(tex);
                    Nodes.Add(new TextureWrapper(tex) { Text = $"Texture {i}" });
                }

                string fileData = FilePath.Replace("wta", "wtp");
                if (System.IO.File.Exists(fileData))
                {
                    using (var readerData = new FileReader(fileData))
                    {
                        for (int i = 0; i < numTextures; i++)
                        {
                            readerData.SeekBegin(entries[i].DataOffset);
                            entries[i].ImageData = readerData.ReadBytes((int)entries[i].DataSize);
                        }
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class TextureInfo
        {
            public Magic Magic = "XT1 ";
            public uint unknown;
            public uint ImageSize;
            public uint Padding;
            public uint Format;
            public uint MipCount;
            public uint unknown2;
            public uint unknown3;
            public uint Width;
            public uint Height;
            public uint Depth;
            public uint unknown4;
            public uint unknown5;
            public uint unknown6;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class TextureEntry
        {
            public uint DataOffset;
            public uint DataSize;

            public TextureInfo Info;

            public string SourceFile;

            public byte[] ImageData;
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public class TextureWrapper : STGenericTexture
        {
            public TextureEntry Texture;

            public TextureWrapper(TextureEntry tex)
            {
                Texture = tex;
                Width = tex.Info.Width;
                Height = tex.Info.Height;
                Depth = tex.Info.Depth;
                ArrayCount = 1; 
                MipCount = tex.Info.MipCount;
                if (Formats.ContainsKey(tex.Info.Format))
                    Format = Formats[tex.Info.Format];
            }

            private Dictionary<uint, TEX_FORMAT> Formats = new Dictionary<uint, TEX_FORMAT>()
            {
                {0x01, TEX_FORMAT.R8G8_UNORM},
                {0x0d, TEX_FORMAT.R9G9B9E5_SHAREDEXP},
                {0x1a, TEX_FORMAT.BC1_UNORM},
                {0x1b, TEX_FORMAT.BC2_UNORM},
                {0x1c, TEX_FORMAT.BC3_UNORM},
                {0x1d, TEX_FORMAT.BC4_UNORM},
                {0x1e, TEX_FORMAT.BC5_UNORM},
                {0x2d, TEX_FORMAT.ASTC_4x4_UNORM},
                {0x2e, TEX_FORMAT.ASTC_5x4_UNORM},
                {0x2f, TEX_FORMAT.ASTC_5x5_UNORM},
                {0x30, TEX_FORMAT.ASTC_6x5_UNORM},
                {0x31, TEX_FORMAT.ASTC_6x6_UNORM},
                {0x32, TEX_FORMAT.ASTC_8x5_UNORM},
                {0x33, TEX_FORMAT.ASTC_8x6_UNORM},
                {0x34, TEX_FORMAT.ASTC_8x8_UNORM},
                {0x35, TEX_FORMAT.ASTC_10x5_UNORM},
                {0x36, TEX_FORMAT.ASTC_10x6_UNORM},
                {0x37, TEX_FORMAT.ASTC_10x8_UNORM},
                {0x38, TEX_FORMAT.ASTC_10x10_UNORM},
                {0x39, TEX_FORMAT.ASTC_12x10_UNORM},
                {0x3a, TEX_FORMAT.ASTC_12x12_UNORM},
                {0x3b, TEX_FORMAT.B5G5R5A1_UNORM},

            };

            public override bool CanEdit { get; set; } = false;

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

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return TegraX1Swizzle.GetImageData(this, Texture.ImageData, ArrayLevel, MipLevel, 1);
            }


            public override void OnClick(TreeView treeView)
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

                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }
        }
    }
}
