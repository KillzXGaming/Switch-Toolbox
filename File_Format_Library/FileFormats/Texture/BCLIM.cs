using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.ComponentModel;
using Bfres.Structs;

namespace FirstPlugin
{
    public class BCLIM : STGenericTexture, IEditor<ImageEditorBase>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                        TEX_FORMAT.ETC1_UNORM,
                        TEX_FORMAT.ETC1_A4,

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
                };
            }
        }

        public BCLIMFormat ConvertFormatGenericToBflim(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.A8_UNORM: return BCLIMFormat.L8_UNORM;
                case TEX_FORMAT.R8G8_UNORM: return BCLIMFormat.LA8;
                case TEX_FORMAT.B5G6R5_UNORM: return BCLIMFormat.RGB565;
                case TEX_FORMAT.R8G8B8A8_UNORM: return BCLIMFormat.RGBA8;
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return BCLIMFormat.RGBA8_SRGB;
                case TEX_FORMAT.R10G10B10A2_UNORM: return BCLIMFormat.RGB10A2_UNORM;
                case TEX_FORMAT.B4G4R4A4_UNORM: return BCLIMFormat.RGBA4;
                case TEX_FORMAT.BC1_UNORM: return BCLIMFormat.BC1_UNORM;
                case TEX_FORMAT.BC1_UNORM_SRGB: return BCLIMFormat.BC1_SRGB;
                case TEX_FORMAT.BC2_UNORM: return BCLIMFormat.BC2_UNORM;
                case TEX_FORMAT.BC2_UNORM_SRGB: return BCLIMFormat.BC2_SRGB;
                case TEX_FORMAT.BC3_UNORM: return BCLIMFormat.BC3_UNORM;
                case TEX_FORMAT.BC3_UNORM_SRGB: return BCLIMFormat.BC3_SRGB;
                case TEX_FORMAT.BC4_UNORM: return BCLIMFormat.BC4A_UNORM;
                case TEX_FORMAT.BC4_SNORM: return BCLIMFormat.BC4L_UNORM;
                case TEX_FORMAT.BC5_UNORM: return BCLIMFormat.BC5_UNORM;
                default:
                    throw new Exception("Unsupported format " + Format);
            }
        }

        public override bool CanEdit { get; set; } = true;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Layout Image" };
        public string[] Extension { get; set; } = new string[] { "*.bclim" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            if (stream.Length < 0x40)
                return false;

            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "CLIM", reader.BaseStream.Length - 0x28);
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

        ImageEditorBase form;
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

            form = new ImageEditorBase();
            form.Text = Text;
            form.Dock = DockStyle.Fill;
            form.AddFileContextEvent("Save", Save);
            form.AddFileContextEvent("Replace", Replace);
            form.LoadProperties(prop);
            form.LoadImage(this);

            return form;
        }

        public void FillEditor(UserControl control)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            if (form != null && image != null)
            {
                Properties prop = new Properties();
                prop.Width = Width;
                prop.Height = Height;
                prop.Depth = Depth;
                prop.MipCount = MipCount;
                prop.ArrayCount = ArrayCount;
                prop.ImageSize = (uint)ImageData.Length;
                prop.Format = Format;

                form.LoadProperties(prop);
                form.LoadImage(this);
            }
        }

        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FTEX ftex = new FTEX();
                ftex.ReplaceTexture(ofd.FileName, Format, 1, 0, SupportedFormats, true, true, false);
                if (ftex.texture != null)
                {
                    image.BCLIMFormat = ConvertFormatGenericToBflim(ftex.Format);
                    image.Height = (ushort)ftex.texture.Height;
                    image.Width = (ushort)ftex.texture.Width;

                    Format = GetFormat(image.BCLIMFormat);
                    Width = image.Width;
                    Height = image.Height;

                    ImageData = ftex.texture.Data;

                    UpdateForm();
                }
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
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                uint FileSize = (uint)reader.BaseStream.Length;
                reader.Seek(FileSize - 0x28, SeekOrigin.Begin);

                header = new Header();
                header.Read(reader);

                reader.Seek(header.HeaderSize + FileSize - 0x28, SeekOrigin.Begin);
                image = new Image();
                image.Read(reader);
                Format = GetFormat(image.BCLIMFormat);
                Width = image.Width;
                Height = image.Height;

                LoadComponents(Format);

                reader.Position = 0;
                ImageData = reader.ReadBytes((int)image.ImageSize);

                PlatformSwizzle = PlatformSwizzle.Platform_3DS;
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

        private TEX_FORMAT GetFormat(BCLIMFormat format)
        {
            switch (format)
            {
                case BCLIMFormat.L8_UNORM:
                    return TEX_FORMAT.L8;
                case BCLIMFormat.A8:
                    return TEX_FORMAT.A8_UNORM;
                case BCLIMFormat.LA4:
                    return TEX_FORMAT.LA4;
                case BCLIMFormat.LA8:
                    return TEX_FORMAT.LA8;
                case BCLIMFormat.HILO8:
                    return TEX_FORMAT.HIL08;
                case BCLIMFormat.ETC1:
                    return TEX_FORMAT.ETC1_UNORM;
                case BCLIMFormat.ETC1A4:
                    return TEX_FORMAT.ETC1_A4;
                case BCLIMFormat.RGB565:
                    return TEX_FORMAT.B5G6R5_UNORM;
                case BCLIMFormat.RGBX8:
                case BCLIMFormat.RGBA8:
                    return TEX_FORMAT.R8G8B8A8_UNORM;
                case BCLIMFormat.RGBA8_SRGB:
                    return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                case BCLIMFormat.RGB10A2_UNORM:
                    return TEX_FORMAT.R10G10B10A2_UNORM;
                case BCLIMFormat.RGB5A1:
                    return TEX_FORMAT.B5G5R5A1_UNORM;
                case BCLIMFormat.RGBA4:
                    return TEX_FORMAT.B4G4R4A4_UNORM;
                case BCLIMFormat.BC1_UNORM:
                    return TEX_FORMAT.BC1_UNORM;
                case BCLIMFormat.BC1_SRGB:
                    return TEX_FORMAT.BC1_UNORM_SRGB;
                case BCLIMFormat.BC2_UNORM:
                    return TEX_FORMAT.BC2_UNORM_SRGB;
                case BCLIMFormat.BC3_UNORM:
                    return TEX_FORMAT.BC3_UNORM;
                case BCLIMFormat.BC3_SRGB:
                    return TEX_FORMAT.BC3_UNORM_SRGB;
                case BCLIMFormat.BC4L_UNORM:
                case BCLIMFormat.BC4A_UNORM:
                    return TEX_FORMAT.BC4_UNORM;
                case BCLIMFormat.BC5_UNORM:
                    return TEX_FORMAT.BC5_UNORM;
                default:
                    throw new Exception("Unsupported format " + format);
            }
        }

        public enum BCLIMFormat : byte
        {
            L8_UNORM = 0,
            A8 = 1,
            LA4 = 2,
            LA8 = 3,
            HILO8 = 4,
            RGB565 = 5,
            RGBX8 = 6,
            RGB5A1 = 7,
            RGBA4 = 8,
            RGBA8 = 9,
            ETC1 = 10,
            ETC1A4 = 11,
            BC1_UNORM,
            BC2_UNORM,
            BC3_UNORM,
            BC4L_UNORM,
            BC4A_UNORM,
            BC5_UNORM,
            L4_UNORM,
            A4_UNORM,
            RGBA8_SRGB,
            BC1_SRGB,
            BC2_SRGB,
            BC3_SRGB,
            RGB10A2_UNORM,
            RGB565_Indirect_UNORM,
        }

        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {
            if (bitmap == null || image == null)
                return; //Image is likely disposed and not needed to be applied

            MipCount = 1;
            var CtrFormat = CTR_3DS.ConvertToPICAFormat(Format);

            try
            {
                //Create image block from bitmap first
                var data = GenerateMipsAndCompress(bitmap, MipCount, Format);

                //Swizzle and create surface
                /*    var surface = GX2.CreateGx2Texture(data, Text,
                        (uint)image.TileMode,
                        (uint)0,
                        (uint)image.Width,
                        (uint)image.Height,
                        (uint)1,
                        (uint)Gx2Format,
                        (uint)0,
                        (uint)1,
                        (uint)MipCount
                        );

                    image.Swizzle = (byte)surface.swizzle;
                    image.BCLIMFormat = ConvertFormatGenericToBflim(Format);
                    image.Height = (ushort)surface.height;
                    image.Width = (ushort)surface.width;*/

                Width = image.Width;
                Height = image.Height;

                // ImageData = surface.data;

                IsEdited = true;
                LoadOpenGLTexture();
                LibraryGUI.UpdateViewport();
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to swizzle and compress image " + Text, "Error", ex.ToString());
            }
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return ImageData;
        }

        public void Unload()
        {

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

            public void Read(FileReader reader)
            {
                reader.ReadSignature(4, "CLIM");
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
            public BCLIMFormat BCLIMFormat;
            public byte Flags;
            public uint ImageSize;

            public void Read(FileReader reader)
            {
                reader.ReadSignature(4, "imag");
                Size = reader.ReadUInt32();
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                BCLIMFormat = reader.ReadEnum<BCLIMFormat>(true);
                Flags = reader.ReadByte();
                Alignment = reader.ReadUInt16();
                ImageSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("imag");
                writer.Write(Size);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(BCLIMFormat, true);
                writer.Write(Flags);
                writer.Write(Alignment);
                writer.Write(ImageSize);
            }
        }
    }
}
