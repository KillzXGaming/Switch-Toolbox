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

                bool UseExternalBinary = true;

                List<TextureEntry> entries = new List<TextureEntry>();
                for (int i = 0; i < numTextures; i++)
                {
                    TextureEntry tex = new TextureEntry();
                    tex.ParentFile = this;

                    //Get data offset
                    reader.SeekBegin(dataOffsetTable + (i * 4));
                    tex.DataOffset = reader.ReadUInt32();

                    if (i == 0 && tex.DataOffset != 0)
                        UseExternalBinary = false;

                    //Get data size
                    reader.SeekBegin(dataSizeTable + (i * 4));
                    tex.DataSize = reader.ReadUInt32();

                    //Get tex info
                    reader.SeekBegin(textureInfoTable + (i * 56));
                    tex.Info = reader.ReadStruct<TextureInfo>();
                    entries.Add(tex);
                    Nodes.Add(new TextureWrapper(tex) { Text = $"Texture {i}" });
                }

                if (UseExternalBinary)
                {
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
                else
                {
                    for (int i = 0; i < numTextures; i++)
                    {
                        reader.SeekBegin(entries[i].DataOffset);
                        entries[i].ImageData = reader.ReadBytes((int)entries[i].DataSize);
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class TextureInfo
        {
            public Magic Magic = "XT1 ";
            public uint unknown;
            public ulong ImageSize;
            public uint HeaderSize;
            public uint MipCount;
            public uint Type;
            public uint Format;
            public uint Width;
            public uint Height;
            public uint Depth;
            public uint unknown4;
            public uint textureLayout;
            public uint textureLayout2;
        }

        public enum SurfaceType
        {
            T_1D,
            T_2D,
            T_3D,
            T_Cube,
            T_1D_Array,
            T_2D_Array,
            T_2D_Mulitsample,
            T_2D_Multisample_Array,
            T_Cube_Array,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class TextureEntry
        {
            public WTB ParentFile;

            public uint DataOffset;
            public uint DataSize;

            public TextureInfo Info;

            public string SourceFile;

            private byte[] imageData;
            public byte[] ImageData
            {
                get
                {
                    if (imageData == null)
                    {
                        //Search in archives for data
                        if (ParentFile.IFileInfo.ArchiveParent != null)
                        {
                            foreach (var file in ParentFile.IFileInfo.ArchiveParent.Files)
                            {
                                if (file.FileName == ParentFile.FileName.Replace("wta", "wtp"))
                                {
                                    using (var readerData = new FileReader(file.FileDataStream)) {
                                        readerData.SeekBegin(DataOffset);
                                        imageData = readerData.ReadBytes((int)DataSize);
                                    }
                                }
                            }
                        }
                    }

                    return imageData;
                }
                set
                {
                    imageData = value;
                }
            }
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

            public SurfaceType SurfaceType;
            public uint HeaderSize;
            public ulong ImageSize;

            public TextureWrapper(TextureEntry tex)
            {
                Texture = tex;
                Width = tex.Info.Width;
                Height = tex.Info.Height;
                Depth = tex.Info.Depth;
                ArrayCount = 1; 
                MipCount = tex.Info.MipCount;

               // Format = TextureData.ConvertFormat(tex.Info.Format);
                if (Formats.ContainsKey(tex.Info.Format))
                    Format = Formats[tex.Info.Format];

                SurfaceType = (SurfaceType)tex.Info.Type;
                ImageSize = tex.Info.ImageSize;
                HeaderSize = tex.Info.HeaderSize;

                if (SurfaceType == SurfaceType.T_Cube || SurfaceType == SurfaceType.T_Cube_Array)
                    ArrayCount = 6;
            }

            private Dictionary<uint, TEX_FORMAT> Formats = new Dictionary<uint, TEX_FORMAT>()
            {
                {0x25 , TEX_FORMAT.R8G8B8A8_UNORM},
                {0x42 , TEX_FORMAT.BC1_UNORM},
                {0x43, TEX_FORMAT.BC2_UNORM},
                {0x44, TEX_FORMAT.BC3_UNORM},
                {0x45, TEX_FORMAT.BC4_UNORM},
                {0x46 , TEX_FORMAT.BC1_UNORM_SRGB},
                {0x47 , TEX_FORMAT.BC2_UNORM_SRGB},
                {0x48 , TEX_FORMAT.BC3_UNORM_SRGB},
                {0x49, TEX_FORMAT.BC4_SNORM},
                {0x50, TEX_FORMAT.BC6H_UF16},
                {0x79, TEX_FORMAT.ASTC_4x4_UNORM},
                {0x80, TEX_FORMAT.ASTC_8x8_UNORM},
                {0x87, TEX_FORMAT.ASTC_4x4_SRGB},
                {0x8E, TEX_FORMAT.ASTC_8x8_SRGB},

             /*   {0x01, TEX_FORMAT.R8G8_UNORM},
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
                {0x38, TEX_FORMAT.ASTC_8x8_UNORM},
                {0x39, TEX_FORMAT.ASTC_12x10_UNORM},
                {0x3a, TEX_FORMAT.ASTC_12x12_UNORM},
                {0x3b, TEX_FORMAT.B5G5R5A1_UNORM},*/

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

            private bool hasShownDialog = false;
            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                if (Texture.ImageData == null)
                {
                    if (!hasShownDialog)
                    {
                        MessageBox.Show("No image data found! Put this next to it's data file (.wtp)");
                        hasShownDialog = true;
                    }
                    return new byte[Texture.Info.ImageSize];
                }

                Console.WriteLine($" Texture.ImageData " + Texture.ImageData.Length);

                var BlockHeightLog2 = Texture.Info.textureLayout & 7;

                 return TegraX1Swizzle.GetImageData(this, Texture.ImageData, ArrayLevel, MipLevel, BlockHeightLog2, 1);
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

                editor.LoadProperties(new WtaProperties(Format,
                    Width, Height, Depth, SurfaceType, ImageSize, HeaderSize));
                editor.LoadImage(this);
            }
        }

        public class WtaProperties
        {
            public TEX_FORMAT Format { get; private set; }
            public uint Width { get; private set; }
            public uint Height { get; private set; }
            public uint Depth { get; private set; }
            public SurfaceType SurfaceType { get; private set; }
            public ulong ImageSize { get; private set; }
            public uint HeaderSize { get; private set; }

            public WtaProperties(TEX_FORMAT format, uint width, uint height,
                uint depth, SurfaceType type, ulong imageSize, uint headerSize)
            {
                Format = format;
                Width = width;
                Height = height;
                Depth = depth;
                SurfaceType = type;
                ImageSize = imageSize;
                HeaderSize = headerSize;
            }
        }
    }
}
