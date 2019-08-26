using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.ComponentModel;
using Bfres.Structs;

namespace FirstPlugin
{
    public class BFLIM : STGenericTexture, IEditor<ImageEditorBase>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

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
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                        TEX_FORMAT.A8_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                        TEX_FORMAT.R8G8_SNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.R10G10B10A2_UNORM,
                        TEX_FORMAT.B4G4R4A4_UNORM,
                };
            }
        }

        public override bool CanEdit { get; set; } = true;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Layout Image" };
        public string[] Extension { get; set; } = new string[] { "*.bflim" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FLIM", reader.BaseStream.Length - 0x28);
            }
        }


        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        public ImageEditorBase OpenForm()
        {
            bool IsDialog = IFileInfo != null && IFileInfo.InArchive;

            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)ImageData.Length;
            prop.Format = Format;
            prop.TileMode = image.TileMode;
            prop.Swizzle = image.Swizzle;

            form = new ImageEditorBase();
            form.Text = Text;
            form.Dock = DockStyle.Fill;
            form.AddFileContextEvent("Save", Save);
            form.AddFileContextEvent("Replace", Replace);
            form.LoadProperties(prop);
            form.LoadImage(this);

            return form;
        }

        private ImageEditorBase form;

        public void UpdateForm()
        {
            UpdateForm(form);
        }

        public void FillEditor(UserControl control)
        {
            form = (ImageEditorBase)control;
            UpdateForm();
        }

        private void UpdateForm(ImageEditorBase form)
        {
            if (image != null)
            {
                Properties prop = new Properties();
                prop.Width = Width;
                prop.Height = Height;
                prop.Depth = Depth;
                prop.MipCount = MipCount;
                prop.ArrayCount = ArrayCount;
                prop.ImageSize = (uint)ImageData.Length;
                prop.Format = Format;
                prop.TileMode = image.TileMode;
                prop.Swizzle = image.Swizzle;
           
                form.LoadProperties(prop);
                form.LoadImage(this);
            }
        }

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => null;
            public STToolStripItem[] NewFromFileMenuExtensions => newFileExt;
            public STToolStripItem[] ToolsMenuExtensions => toolExt;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] newFileExt = new STToolStripItem[1];
            STToolStripItem[] toolExt = new STToolStripItem[1];

            public MenuExt()
            {
                toolExt[0] = new STToolStripItem("Textures");
                toolExt[0].DropDownItems.Add(new STToolStripItem("Batch Export (Wii U Textures)", Export));
                newFileExt[0] = new STToolStripItem("BFLIM From Image", CreateNew);
            }
            private void Export(object sender, EventArgs args)
            {
                string formats = FileFilters.GTX;

                string[] forms = formats.Split('|');

                List<string> Formats = new List<string>();
                for (int i = 0; i < forms.Length; i++)
                {
                    if (i > 1 || i == (forms.Length - 1)) //Skip lines with all extensions
                    {
                        if (!forms[i].StartsWith("*"))
                            Formats.Add(forms[i]);
                    }
                }

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string Extension = form.GetSelectedExtension();

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Multiselect = true;
                    ofd.Filter = Utils.GetAllFilters(new Type[] { typeof(BFLIM), typeof(BFFNT), typeof(BFRES), typeof(PTCL), typeof(SARC) });

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        FolderSelectDialog folderDialog = new FolderSelectDialog();
                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string file in ofd.FileNames)
                            {
                                var FileFormat = STFileLoader.OpenFileFormat(file, new Type[] { typeof(BFLIM), typeof(PTCL), typeof(BFRES), typeof(BFFNT), typeof(SARC) });
                                if (FileFormat == null)
                                    continue;


                                if (FileFormat is SARC)
                                {
                                    string ArchiveFilePath = Path.Combine(folderDialog.SelectedPath, Path.GetFileNameWithoutExtension(file));
                                    ArchiveFilePath = Path.GetDirectoryName(ArchiveFilePath);

                                    if (!Directory.Exists(ArchiveFilePath))
                                        Directory.CreateDirectory(ArchiveFilePath);

                                    SearchBinary(FileFormat, ArchiveFilePath, Extension);
                                }
                                else
                                    SearchBinary(FileFormat, folderDialog.SelectedPath, Extension);
                            }
                        }
                    }
                }
            }

            private void SearchBinary(IFileFormat FileFormat, string Folder, string Extension)
            {
                if (FileFormat is SARC)
                {
                    string ArchiveFilePath = Path.Combine(Folder, Path.GetFileNameWithoutExtension(FileFormat.FileName));
                    if (!Directory.Exists(ArchiveFilePath))
                        Directory.CreateDirectory(ArchiveFilePath);

                    foreach (var file in ((SARC)FileFormat).Files)
                    {
                        var archiveFile = STFileLoader.OpenFileFormat(file.FileName, new Type[] { typeof(BFLIM), typeof(BFFNT), typeof(PTCL), typeof(BFRES), typeof(SARC) }, file.FileData);
                        if (archiveFile == null)
                            continue;

                        SearchBinary(archiveFile, ArchiveFilePath, Extension);
                    }
                }
                if (FileFormat is BFFNT)
                {
                    foreach (STGenericTexture texture in ((BFFNT)FileFormat).bffnt.FontSection.TextureGlyph.Gx2Textures)
                        texture.Export(Path.Combine(Folder, $"{texture.Text}{Extension}"));
                }
                if (FileFormat is BFRES)
                {
                    var FtexContainer = ((BFRES)FileFormat).GetFTEXContainer;
                    if (FtexContainer != null)
                    {
                        foreach (var texture in FtexContainer.ResourceNodes.Values)
                            ((FTEX)texture).Export(Path.Combine(Folder, $"{texture.Text}{Extension}"));
                    }
                }

                if (FileFormat is PTCL)
                {
                    if (((PTCL)FileFormat).headerU != null)
                    {
                        foreach (STGenericTexture texture in ((PTCL)FileFormat).headerU.Textures)
                            texture.Export(Path.Combine(Folder, $"{texture.Text}{Extension}"));
                    }
                }

                if (FileFormat is BFLIM)
                {
                    ((BFLIM)FileFormat).Export(Path.Combine(Folder, $"{FileFormat.FileName}{Extension}"));
                }

                FileFormat.Unload();
            }

            public void CreateNew(object sender, EventArgs args)
            {
                BFLIM bflim = new BFLIM();
                bflim.CanSave = true;
                bflim.IFileInfo = new IFileInfo();
                bflim.header = new Header();

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.Filter = FileFilters.GTX;
                if (ofd.ShowDialog() != DialogResult.OK) return;

                FTEX ftex = new FTEX();
                ftex.ReplaceTexture(ofd.FileName, TEX_FORMAT.BC3_UNORM_SRGB, 1, 0, bflim.SupportedFormats, false, true, false);
                if (ftex.texture != null)
                {
                    bflim.Text = ftex.texture.Name;
                    bflim.image = new Image();
                    bflim.image.Swizzle = (byte)ftex.texture.Swizzle;
                    bflim.image.BflimFormat = FormatsWiiU.FirstOrDefault(x => x.Value == ftex.Format).Key;
                    bflim.image.Height = (ushort)ftex.texture.Height;
                    bflim.image.Width = (ushort)ftex.texture.Width;

                    bflim.Format = FormatsWiiU[bflim.image.BflimFormat];
                    bflim.Width = bflim.image.Width;
                    bflim.Height = bflim.image.Height;

                    bflim.ImageData = ftex.texture.Data;
                    var form = new GenericEditorForm(false, bflim.OpenForm());
                    LibraryGUI.CreateMdiWindow(form);

                    bflim.UpdateForm();
                }
            }
        }

        public override string ExportFilter => FileFilters.GTX;
        public override string ReplaceFilter => FileFilters.GTX;

        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ReplaceFilter;
            ofd.FileName = Text;

            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Replace(ofd.FileName);
            }
        }

        public override void Replace(string FileName)
        {
            uint swizzle = (image.Swizzle >> 8) & 7;

            FTEX ftex = new FTEX();
            ftex.ReplaceTexture(FileName, Format, 1, swizzle, SupportedFormats, true, true, false);
            if (ftex.texture != null)
            {
                image.Swizzle = ftex.texture.Swizzle;
                image.BflimFormat = FormatsWiiU.FirstOrDefault(x => x.Value == ftex.Format).Key;
                image.Height = (ushort)ftex.texture.Height;
                image.Width = (ushort)ftex.texture.Width;

                Format = FormatsWiiU[image.BflimFormat];
                Width = image.Width;
                Height = image.Height;

                ImageData = ftex.texture.Data;

                UpdateForm();
            }
        }


        private void Save(object sender, EventArgs args)
        {
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public class Properties
        {
            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Height of the image.")]
            [Category("Image Info")]
            public uint Height { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Width of the image.")]
            [Category("Image Info")]
            public uint Width { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Format of the image.")]
            [Category("Image Info")]
            public TEX_FORMAT Format { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Depth of the image (3D type).")]
            [Category("Image Info")]
            public uint Depth { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Mip map count of the image.")]
            [Category("Image Info")]
            public uint MipCount { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Array count of the image for multiple surfaces.")]
            [Category("Image Info")]
            public uint ArrayCount { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("The image size in bytes.")]
            [Category("Image Info")]
            public uint ImageSize { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("The image tilemode.")]
            [Category("Image Info")]
            public GX2.GX2TileMode TileMode { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Category("Image Info")]
            public uint Swizzle { get; set; }
        }

        Header header;
        Image image;
        byte[] ImageData;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                uint FileSize = (uint)reader.BaseStream.Length;
                reader.Seek(FileSize - 0x28, SeekOrigin.Begin);

                header = new Header();
                header.Read(reader);

                bool Is3DS = reader.ByteOrder == Syroot.BinaryData.ByteOrder.LittleEndian;

                reader.Seek(header.HeaderSize + FileSize - 0x28, SeekOrigin.Begin);
                image = new Image(Is3DS);
                image.Read(reader);

                if (Is3DS)
                    Format = Formats3DS[image.BflimFormat];
                else
                    Format = FormatsWiiU[image.BflimFormat];

                Width = image.Width;
                Height = image.Height;

                LoadComponents(Format);

                uint ImageSize = reader.ReadUInt32();

                reader.Position = 0;
                ImageData = reader.ReadBytes((int)ImageSize);

                if (!PluginRuntime.bflimTextures.ContainsKey(Text))
                    PluginRuntime.bflimTextures.Add(Text, this);
            }
        }

        private void LoadComponents(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.BC5_SNORM:
                case TEX_FORMAT.BC5_UNORM:
                    RedChannel = STChannelType.Red;
                    GreenChannel = STChannelType.Red;
                    BlueChannel = STChannelType.Red;
                    AlphaChannel = STChannelType.Green;
                    break;
                case TEX_FORMAT.BC4_SNORM:
                case TEX_FORMAT.BC4_UNORM:
                    RedChannel = STChannelType.Red;
                    GreenChannel = STChannelType.Red;
                    BlueChannel = STChannelType.Red;
                    AlphaChannel = STChannelType.Red;
                    break;
            }
        }

        public static Dictionary<byte, TEX_FORMAT> Formats3DS = new Dictionary<byte, TEX_FORMAT>()
        {
            [0] = TEX_FORMAT.L8,
            [1] = TEX_FORMAT.A8_UNORM,
            [2] = TEX_FORMAT.A4,
            [3] = TEX_FORMAT.LA8,
            [4] = TEX_FORMAT.HIL08,
            [5] = TEX_FORMAT.B5G6R5_UNORM,
            [6] = TEX_FORMAT.R8G8B8A8_UNORM,
            [7] = TEX_FORMAT.B5G5R5A1_UNORM,
            [8] = TEX_FORMAT.B4G4R4A4_UNORM,
            [9] = TEX_FORMAT.R8G8B8A8_UNORM,
            [10] = TEX_FORMAT.ETC1_UNORM,
            [11] = TEX_FORMAT.ETC1_A4,
            [12] = TEX_FORMAT.BC1_UNORM,
            [13] = TEX_FORMAT.BC2_UNORM,
            [14] = TEX_FORMAT.BC3_UNORM,
            [15] = TEX_FORMAT.BC4_UNORM, //BC4L_UNORM
            [16] = TEX_FORMAT.BC4_UNORM, //BC4A_UNORM
            [17] = TEX_FORMAT.BC5_UNORM,
            [18] = TEX_FORMAT.L4,
            [19] = TEX_FORMAT.A4,
        };

        public static Dictionary<byte, TEX_FORMAT> FormatsWiiU = new Dictionary<byte, TEX_FORMAT>()
        {
            [0] = TEX_FORMAT.L8,
            [1] = TEX_FORMAT.A8_UNORM,
            [2] = TEX_FORMAT.A4,
            [3] = TEX_FORMAT.LA8,
            [4] = TEX_FORMAT.R8G8_UNORM, //HILO8
            [5] = TEX_FORMAT.B5G6R5_UNORM,
            [6] = TEX_FORMAT.R8G8B8A8_UNORM,
            [7] = TEX_FORMAT.B5G5R5A1_UNORM,
            [8] = TEX_FORMAT.B4G4R4A4_UNORM,
            [9] = TEX_FORMAT.R8G8B8A8_UNORM,
            [10] = TEX_FORMAT.ETC1_UNORM,
            [11] = TEX_FORMAT.ETC1_A4,
            [12] = TEX_FORMAT.BC1_UNORM,
            [13] = TEX_FORMAT.BC2_UNORM,
            [14] = TEX_FORMAT.BC3_UNORM,
            [15] = TEX_FORMAT.BC4_UNORM, //BC4L_UNORM
            [16] = TEX_FORMAT.BC4_UNORM, //BC4A_UNORM
            [17] = TEX_FORMAT.BC5_UNORM,
            [18] = TEX_FORMAT.L4,
            [19] = TEX_FORMAT.A4,
            [20] = TEX_FORMAT.R8G8B8A8_UNORM,
            [21] = TEX_FORMAT.BC1_UNORM_SRGB,
            [22] = TEX_FORMAT.BC2_UNORM_SRGB,
            [23] = TEX_FORMAT.BC3_UNORM_SRGB,
            [24] = TEX_FORMAT.R10G10B10A2_UNORM,
            [25] = TEX_FORMAT.R5G5B5_UNORM,
        };

        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {
            if (bitmap == null || image == null)
                return; //Image is likely disposed and not needed to be applied

            MipCount = 1;
            var Gx2Format = FTEX.ConvertToGx2Format(Format);

            uint swizzle = (image.Swizzle >> 8) & 7;

            try
            {
                //Create image block from bitmap first
                var data = GenerateMipsAndCompress(bitmap, MipCount, Format);

                //Swizzle and create surface
                var surface = GX2.CreateGx2Texture(data, Text,
                    (uint)image.TileMode,
                    (uint)0,
                    (uint)image.Width,
                    (uint)image.Height,
                    (uint)1,
                    (uint)Gx2Format,
                    (uint)swizzle,
                    (uint)1,
                    (uint)MipCount
                    );

                image.Swizzle = (byte)surface.swizzle;
                image.BflimFormat = FormatsWiiU.FirstOrDefault(x => x.Value == Format).Key;
                image.Height = (ushort)surface.height;
                image.Width = (ushort)surface.width;

                Width = image.Width;
                Height = image.Height;

                ImageData = surface.data;

                IsEdited = true;
                LoadOpenGLTexture();
                LibraryGUI.UpdateViewport();
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to swizzle and compress image " + Text, "Error", ex.ToString());
            }
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            if (image.Is3DS)
            {
                PlatformSwizzle = PlatformSwizzle.Platform_3DS;
                return ImageData;
            }
            else
            {
                uint bpp = GetBytesPerPixel(Format);

                GX2.GX2Surface surf = new GX2.GX2Surface();
                surf.bpp = bpp;
                surf.height = image.Height;
                surf.width = image.Width;
                surf.aa = (uint)GX2.GX2AAMode.GX2_AA_MODE_1X;
                surf.alignment = image.Alignment;
                surf.depth = 1;
                surf.dim = (uint)GX2.GX2SurfaceDimension.DIM_2D;
                surf.format = (uint)FTEX.ConvertToGx2Format(Format);
                surf.use = (uint)GX2.GX2SurfaceUse.USE_COLOR_BUFFER;
                surf.pitch = 0;
                surf.data = ImageData;
                surf.numMips = 1;
                surf.mipOffset = new uint[0];
                surf.mipData = ImageData;
                surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;
                surf.swizzle = image.Swizzle;
                surf.numArray = 1;

                return GX2.Decode(surf, ArrayLevel, MipLevel);
            }
        }

        public void Unload()
        {
            form.Dispose();
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream, true))
            {
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                writer.Write(ImageData);

                long headerPos = writer.Position;

                header.Write(writer);
                image.Write(writer);
                writer.Write(ImageData.Length);

                writer.Seek(headerPos + 0x0C, SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
            }
        }

        public class Header
        {
            public ushort ByteOrderMark;
            public ushort HeaderSize;
            public uint Version;
            public ushort blockount;
            public ushort padding;

            public Header()
            {
                ByteOrderMark = 65279;
                HeaderSize = 20;
                blockount = 1;
                Version = 33685504;
            }

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                reader.ReadSignature(4, "FLIM");
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                HeaderSize = reader.ReadUInt16();
                Version = reader.ReadUInt32();
                uint fileSize = reader.ReadUInt32();
                blockount = reader.ReadUInt16();
                padding = reader.ReadUInt16();
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("FLIM");
                writer.Write(ByteOrderMark);
                writer.Write(HeaderSize);
                writer.Write(Version);
                writer.Write(uint.MaxValue);
                writer.Write(blockount);
                writer.Write(padding);
            }
        }
        public class Image
        {
            public uint Size;
            public ushort Width;
            public ushort Height;
            public ushort Alignment;
            public byte BflimFormat;
            public byte Flags;

            public bool Is3DS = false;

            public Image(bool is3DS)
            {
                Is3DS = is3DS;
            }

            public Image()
            {
                Alignment = 8192;
                Flags = 0xC4;
                Size = 16;
            }

            public GX2.GX2TileMode TileMode
            {
                get
                {
                    return (GX2.GX2TileMode) ((int)Flags & 31);
                }
                set
                {
                    Flags = (byte)((int)Flags & 224 | (int)(byte)value & 31);
                }
            }

            public uint Swizzle
            {
                get
                {
                    return (uint)(((int)((uint)Flags >> 5) & 7) << 8);
                }
                set
                {
                    Flags = (byte)((int)Flags & 31 | (int)(byte)(value >> 8) << 5);
                }
            }

            public void Read(FileReader reader)
            {
                reader.ReadSignature(4, "imag");
                Size = reader.ReadUInt32();
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                Alignment = reader.ReadUInt16();
                BflimFormat = reader.ReadByte();
                Flags = reader.ReadByte();
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("imag");
                writer.Write(Size);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Alignment);
                writer.Write(BflimFormat);
                writer.Write(Flags);
            }
        }
    }
}
