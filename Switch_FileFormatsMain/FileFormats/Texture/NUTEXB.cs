using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using System.ComponentModel;
using Syroot.NintenTools.NSW.Bntx;
using Syroot.NintenTools.NSW.Bntx.GFX;

namespace FirstPlugin
{
    public class NUTEXB : STGenericTexture, IFileFormat
    {
        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.BC1_UNORM,
                    TEX_FORMAT.BC1_UNORM_SRGB,
                    TEX_FORMAT.BC3_UNORM,
                    TEX_FORMAT.BC3_UNORM_SRGB,
                    TEX_FORMAT.BC6H_UF16,
                    TEX_FORMAT.BC7_UNORM,
                    TEX_FORMAT.BC7_UNORM_SRGB,
                    TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                    TEX_FORMAT.R8G8B8A8_UNORM,
                };
            }
        }

        public override bool CanEdit { get; set; } = true;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Namco Texture" };
        public string[] Extension { get; set; } = new string[] { "*.nutexb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(3, "XET", reader.BaseStream.Length - 7);
            }
        }

        public bool LimitFileSize { get; set; } = true;

        public enum NUTEXImageFormat : byte
        {
            R8G8B8A8_UNORM = 0x00,
            R8G8B8A8_SRGB = 0x05,
            R32G32B32A32_FLOAT = 0x34,
            B8G8R8A8_UNORM = 0x50,
            B8G8R8A8_SRGB = 0x55,
            BC1_UNORM = 0x80,
            BC1_SRGB = 0x85,
            BC2_UNORM = 0x90,
            BC2_SRGB = 0x95,
            BC3_UNORM = 0xa0,
            BC3_SRGB = 0xa5,
            BC4_UNORM = 0xb0,
            BC4_SNORM = 0xb5,
            BC5_UNORM = 0xc0,
            BC5_SNORM = 0xc5,
            BC6_UFLOAT = 0xd7,
            BC7_UNORM = 0xe0,
            BC7_SRGB = 0xe5,
        };

        public static uint blk_dims(byte format)
        {
            switch (format)
            {
                case (byte)NUTEXImageFormat.BC1_UNORM:
                case (byte)NUTEXImageFormat.BC1_SRGB:
                case (byte)NUTEXImageFormat.BC2_UNORM:
                case (byte)NUTEXImageFormat.BC2_SRGB:
                case (byte)NUTEXImageFormat.BC3_UNORM:
                case (byte)NUTEXImageFormat.BC3_SRGB:
                case (byte)NUTEXImageFormat.BC4_UNORM:
                case (byte)NUTEXImageFormat.BC4_SNORM:
                case (byte)NUTEXImageFormat.BC5_UNORM:
                case (byte)NUTEXImageFormat.BC5_SNORM:
                case (byte)NUTEXImageFormat.BC6_UFLOAT:
                case (byte)NUTEXImageFormat.BC7_UNORM:
                case (byte)NUTEXImageFormat.BC7_SRGB:
                    return 0x44;

                default: return 0x11;
            }
        }

        public static uint bpps(byte format)
        {
            switch (format)
            {
                case (byte)NUTEXImageFormat.B8G8R8A8_UNORM:
                case (byte)NUTEXImageFormat.B8G8R8A8_SRGB:
                case (byte)NUTEXImageFormat.R8G8B8A8_UNORM:
                case (byte)NUTEXImageFormat.R8G8B8A8_SRGB:
                    return 4;

                case (byte)NUTEXImageFormat.BC1_UNORM:
                case (byte)NUTEXImageFormat.BC1_SRGB:
                case (byte)NUTEXImageFormat.BC4_UNORM:
                case (byte)NUTEXImageFormat.BC4_SNORM:
                    return 8;

                case (byte)NUTEXImageFormat.R32G32B32A32_FLOAT:
                case (byte)NUTEXImageFormat.BC2_UNORM:
                case (byte)NUTEXImageFormat.BC2_SRGB:
                case (byte)NUTEXImageFormat.BC3_UNORM:
                case (byte)NUTEXImageFormat.BC3_SRGB:
                case (byte)NUTEXImageFormat.BC5_UNORM:
                case (byte)NUTEXImageFormat.BC5_SNORM:
                case (byte)NUTEXImageFormat.BC6_UFLOAT:
                case (byte)NUTEXImageFormat.BC7_UNORM:
                case (byte)NUTEXImageFormat.BC7_SRGB:
                    return 16;
                default: return 0x00;
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
            prop.ImageSize = (uint)ImageData.Length;
            prop.Format = NutFormat;

            editor.Text = Text;
            editor.LoadProperties(prop);
            editor.LoadImage(this);
        }

        private void UseSizeRestrictions(object sender, EventArgs args)
        {
            if (sender is STToolStripItem)
            {
                if (((STToolStripItem)sender).Checked)
                {
                    ((STToolStripItem)sender).Checked = false;
                }
                else
                {
                    ((STToolStripItem)sender).Checked = true;
                }

                LimitFileSize = ((STToolStripItem)sender).Checked;
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
            public NUTEXImageFormat Format { get; set; }

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

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => null;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => newFileExt;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;

            STToolStripItem[] newFileExt = new STToolStripItem[1];
            public MenuExt()
            {
                newFileExt[0] = new STToolStripItem("Batch Export NUTEXB");
                newFileExt[0].Click += Export;
            }
            private void Export(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in ofd.FileNames)
                    {
                        NUTEXB texture = new NUTEXB();
                        texture.Read(new FileReader(file));

                        Console.WriteLine(texture.Format.ToString("x") + " " + file + " " + texture.Text);
                        try
                        {
                            Bitmap bitmap = texture.GetBitmap();

                            if (bitmap != null)
                                bitmap.Save(System.IO.Path.GetFullPath(file) + texture.ArcOffset + texture.Text + ".png");
                            else
                                Console.WriteLine(" Not supported Format! " + texture.Format);

                            if (bitmap != null)
                                bitmap.Dispose();
                        }
                        catch
                        {
                            Console.WriteLine("Something went wrong??");
                        }

                        texture = null;
                        GC.Collect();
                    }
                }
            }
        }
        public uint unk;
        public int unk2;

        public NUTEXImageFormat NutFormat;
        public List<uint[]> mipSizes = new List<uint[]>();
        public int Alignment;
        public byte[] ImageData;
        public string ArcOffset; //Temp for exporting in batch 

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
                var bntxFile = new BNTX();
                var tex = new TextureData();
                tex.Replace(ofd.FileName, MipCount, Format);

                //If it's null, the operation is cancelled
                if (tex.Texture == null)
                    return;

                var surfacesNew = tex.GetSurfaces();
                var surfaces = GetSurfaces();

                if (LimitFileSize)
                {
                    if (surfaces[0].mipmaps[0].Length != surfacesNew[0].mipmaps[0].Length)
                        throw new Exception($"Image must be the same size! {surfaces[0].mipmaps[0].Length}");

                    if (mipSizes[0].Length != surfacesNew[0].mipmaps.Count)
                        throw new Exception($"Mip map count must be the same! {mipSizes[0].Length}");

                    if (Width != tex.Texture.Width || Height != tex.Texture.Height)
                        throw new Exception("Image size must be the same!");

                    ImageData = tex.Texture.TextureData[0][0];

                    Width = tex.Texture.Width;
                    Height = tex.Texture.Height;
                    MipCount = tex.Texture.MipCount;
                }
                else
                {
                    ImageData = tex.Texture.TextureData[0][0];

                    Width = tex.Texture.Width;
                    Height = tex.Texture.Height;
                    MipCount = tex.Texture.MipCount;

                    Format = tex.Format;
                    NutFormat = ConvertGenericToNutFormat(tex.Format);

                    mipSizes = GenerateMipSizes();
                }
                
                surfacesNew.Clear();
                surfaces.Clear();

                UpdateEditor();
            }
        }

        private List<uint[]> GenerateMipSizes(uint SurfaceCount = 1)
        {
            List<uint[]> MipMapSizes = new List<uint[]>();

            uint bpp = GetBytesPerPixel(Format);
            uint blkWidth = GetBlockWidth(Format);
            uint blkHeight = GetBlockHeight(Format);
            uint blkDepth = GetBlockDepth(Format);

            uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;

            uint Pitch = 0;
            uint DataAlignment = 512;

            int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

            uint ArrayCount = (uint)mipSizes.Count;

            uint ArrayOffset = 0;
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                uint SurfaceSize = 0;
                int blockHeightShift = 0;

                uint[] MipOffsets = new uint[MipCount];

                for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                {
                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);
                    uint depth = (uint)Math.Max(1, Depth >> mipLevel);

                    uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                    if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                        blockHeightShift += 1;

                    uint width__ = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth);
                    uint height__ = TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight);

                    //Calculate the mip size instead
                    byte[] AlignedData = new byte[(TegraX1Swizzle.round_up(SurfaceSize, DataAlignment) - SurfaceSize)];
                    SurfaceSize += (uint)AlignedData.Length;
                    MipOffsets[mipLevel] = (SurfaceSize);

                    //Get the first mip offset and current one and the total image size
                    int msize = (int)((MipOffsets[0] + ImageData.Length - MipOffsets[mipLevel]) / ArrayCount);

                    Pitch = TegraX1Swizzle.round_up(width__ * bpp, 64);
                    SurfaceSize += Pitch * TegraX1Swizzle.round_up(height__, Math.Max(1, blockHeight >> blockHeightShift) * 8);
                }
                ArrayOffset += (uint)(ImageData.Length / ArrayCount);

                MipMapSizes.Add(MipOffsets);
            }

            return MipMapSizes;
        }

        private void Export(object sender, EventArgs args)
        {
            ExportImage();
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
        public void Read(FileReader reader)
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            long pos = reader.BaseStream.Length;
            string magic = reader.ReadMagic((int)pos - 7, 3);//Check magic first!

            if (magic != "XET")
                throw new Exception($"Invalid magic! Expected XET but got {magic}");

            reader.Seek(pos - 112, System.IO.SeekOrigin.Begin); //Subtract size where the name occurs
            byte padding = reader.ReadByte();
            Text = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);

            reader.Seek(pos - 48, System.IO.SeekOrigin.Begin); //Subtract size of header
            uint padding2 = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            Depth = reader.ReadUInt32(); //3d textures
            NutFormat = reader.ReadEnum<NUTEXImageFormat>(true);
            unk = reader.ReadByte(); //Related to pixel type?? 
            ushort padding3 = reader.ReadUInt16();
            unk2 = reader.ReadInt32();
            MipCount = reader.ReadUInt32();
            Alignment = reader.ReadInt32();
            ArrayCount = reader.ReadUInt32(); //6 for cubemaps
            int imagesize = reader.ReadInt32();

            if (Alignment == 0)
                IsSwizzled = false;

            Format = ConvertFormat(NutFormat);

            reader.Seek(imagesize, System.IO.SeekOrigin.Begin); //Get mipmap sizes
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                long mipPos = reader.Position;
                uint[] mips = reader.ReadUInt32s((int)MipCount);
                mipSizes.Add(mips);

                //Each mip section is 0x40 size for each array
                //Seek to next one
                reader.Seek(mipPos + 0x40, System.IO.SeekOrigin.Begin);
            }
            reader.Seek(0, System.IO.SeekOrigin.Begin);
            ImageData = reader.ReadBytes(imagesize);

            reader.Dispose();
            reader.Close();
            reader = null;
        }

        public static NUTEXImageFormat ConvertGenericToNutFormat(TEX_FORMAT nutFormat)
        {
            switch (nutFormat)
            {
                case TEX_FORMAT.B8G8R8A8_UNORM_SRGB: return NUTEXImageFormat.B8G8R8A8_SRGB;
                case TEX_FORMAT.B8G8R8A8_UNORM: return NUTEXImageFormat.B8G8R8A8_UNORM;
                case TEX_FORMAT.BC1_UNORM_SRGB: return NUTEXImageFormat.BC1_SRGB;
                case TEX_FORMAT.BC1_UNORM: return NUTEXImageFormat.BC1_UNORM;
                case TEX_FORMAT.BC2_UNORM: return NUTEXImageFormat.BC2_UNORM;
                case TEX_FORMAT.BC3_UNORM: return NUTEXImageFormat.BC3_UNORM;
                case TEX_FORMAT.BC3_UNORM_SRGB: return NUTEXImageFormat.BC3_SRGB;
                case TEX_FORMAT.BC4_UNORM: return NUTEXImageFormat.BC4_UNORM;
                case TEX_FORMAT.BC4_SNORM: return NUTEXImageFormat.BC4_SNORM;
                case TEX_FORMAT.BC5_UNORM: return NUTEXImageFormat.BC5_UNORM;
                case TEX_FORMAT.BC5_SNORM: return NUTEXImageFormat.BC5_SNORM;
                case TEX_FORMAT.BC6H_UF16: return NUTEXImageFormat.BC6_UFLOAT;
                case TEX_FORMAT.BC7_UNORM: return NUTEXImageFormat.BC7_UNORM;
                case TEX_FORMAT.BC7_UNORM_SRGB: return NUTEXImageFormat.BC7_SRGB;
                case TEX_FORMAT.R32G32B32A32_FLOAT: return NUTEXImageFormat.R32G32B32A32_FLOAT;
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return NUTEXImageFormat.R8G8B8A8_SRGB;
                case TEX_FORMAT.R8G8B8A8_UNORM: return NUTEXImageFormat.R8G8B8A8_UNORM;
                default:
                    throw new Exception($"Cannot convert format {nutFormat}");
            }
        }

        public static TEX_FORMAT ConvertFormat(NUTEXImageFormat nutFormat)
        {
            switch (nutFormat)
            {
                case NUTEXImageFormat.B8G8R8A8_SRGB: return TEX_FORMAT.B8G8R8A8_UNORM_SRGB;
                case NUTEXImageFormat.B8G8R8A8_UNORM: return TEX_FORMAT.B8G8R8A8_UNORM;
                case NUTEXImageFormat.BC1_SRGB: return TEX_FORMAT.BC1_UNORM_SRGB;
                case NUTEXImageFormat.BC1_UNORM: return TEX_FORMAT.BC1_UNORM;
                case NUTEXImageFormat.BC2_UNORM: return TEX_FORMAT.BC2_UNORM;
                case NUTEXImageFormat.BC3_UNORM: return TEX_FORMAT.BC3_UNORM;
                case NUTEXImageFormat.BC3_SRGB: return TEX_FORMAT.BC3_UNORM_SRGB;
                case NUTEXImageFormat.BC4_UNORM: return TEX_FORMAT.BC4_UNORM;
                case NUTEXImageFormat.BC4_SNORM: return TEX_FORMAT.BC4_SNORM;
                case NUTEXImageFormat.BC5_UNORM: return TEX_FORMAT.BC5_UNORM;
                case NUTEXImageFormat.BC5_SNORM: return TEX_FORMAT.BC5_SNORM;
                case NUTEXImageFormat.BC6_UFLOAT: return TEX_FORMAT.BC6H_UF16;
                case NUTEXImageFormat.BC7_UNORM: return TEX_FORMAT.BC7_UNORM;
                case NUTEXImageFormat.BC7_SRGB: return TEX_FORMAT.BC7_UNORM_SRGB;
                case NUTEXImageFormat.R32G32B32A32_FLOAT: return TEX_FORMAT.R32G32B32A32_FLOAT;
                case NUTEXImageFormat.R8G8B8A8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                case NUTEXImageFormat.R8G8B8A8_UNORM: return TEX_FORMAT.R8G8B8A8_UNORM;
                default:
                    throw new Exception($"Cannot convert format {nutFormat}");
            }
        }

        public void Write(FileWriter writer)
        {
            //MipSizes stores mip sizes for multile arrays
            int arrayCount = mipSizes.Count;

            //Mip sizes for the first array
            int mipCount = mipSizes[0].Length;

            writer.Write(ImageData); //Write textue block first

            long headerStart = writer.Position;
            foreach (var mips in mipSizes)
            {
                long MipStart = writer.Position;
                writer.Write(mips); //Write textue block first

                writer.Seek(MipStart + 0x40, System.IO.SeekOrigin.Begin);
            }
            long stringPos = writer.Position;
            writer.Write((byte)0x20);
            writer.WriteString(Text);
            writer.Seek(stringPos + 0x40, System.IO.SeekOrigin.Begin);
            writer.Seek(4); //padding
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Depth);
            writer.Write((byte)NutFormat);
            writer.Write((byte)unk);
            writer.Seek(2); //padding
            writer.Write(unk2);
            writer.Write(mipCount);
            writer.Write(Alignment);
            writer.Write(arrayCount);
            writer.Write(ImageData.Length);
            writer.WriteSignature(" XET");
            writer.Write(131073);

        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            if (!IsSwizzled)
            {
             //   MipCount = GenerateMipCount(bitmap.Width, bitmap.Height);

              //  ImageData = GenerateMipsAndCompress(bitmap, Format);
                return;
            }

            Texture tex = new Texture();
            tex.Height = (uint)bitmap.Height;
            tex.Width = (uint)bitmap.Width;
            tex.Format = TextureData.GenericToBntxSurfaceFormat(Format);
            tex.Name = Text;
            tex.Path = "";
            tex.TextureData = new List<List<byte[]>>();

            STChannelType[] channels = SetChannelsByFormat(Format);
            tex.sparseBinding  = 0; //false
            tex.sparseResidency = 0; //false
            tex.Flags = 0;
            tex.Swizzle = 0;
            tex.textureLayout = 0;
            tex.Regs = new uint[0];
            tex.AccessFlags = 0x20;
            tex.ArrayLength = (uint)ArrayLevel;
            tex.MipCount = MipCount;
            tex.Depth = Depth;
            tex.Dim = Dim.Dim2D;
            tex.TileMode = TileMode.Default;
            tex.textureLayout2 = 0x010007;
            tex.SurfaceDim = SurfaceDim.Dim2D;
            tex.SampleCount = 1;
            tex.Pitch = 32;
            tex.MipOffsets = new long[tex.MipCount];

            MipCount = GenerateMipCount(bitmap.Width, bitmap.Height);

            var mipmaps = TextureImporterSettings.SwizzleSurfaceMipMaps(tex,
                GenerateMipsAndCompress(bitmap, Format), MipCount);

            ImageData = Utils.CombineByteArray(mipmaps.ToArray());
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            if (!IsSwizzled)
                return ImageData;

            int target = 1;
            uint bpp = GetBytesPerPixel(Format);
            uint blkWidth = GetBlockWidth(Format);
            uint blkHeight = GetBlockHeight(Format);
            uint blkDepth = GetBlockDepth(Format);

            uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;

            uint Pitch = 0;
            uint DataAlignment = 512;
            uint TileMode = 0;

            int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

            uint ArrayCount = (uint)mipSizes.Count;

            uint ArrayOffset = 0;
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                uint SurfaceSize = 0;
                int blockHeightShift = 0;

                List<uint> MipOffsets = new List<uint>();

                for (int mipLevel = 0; mipLevel < mipSizes[arrayLevel].Length; mipLevel++)
                {
                    uint width  = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);
                    uint depth  = (uint)Math.Max(1, Depth >> mipLevel);

                    uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                    if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                        blockHeightShift += 1;


                    uint width__ = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth);
                    uint height__ = TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight);

                    //Calculate the mip size instead
                    byte[] AlignedData = new byte[(TegraX1Swizzle.round_up(SurfaceSize, DataAlignment) - SurfaceSize)];
                    SurfaceSize += (uint)AlignedData.Length;
                    MipOffsets.Add(SurfaceSize);

                    //Get the first mip offset and current one and the total image size
                    int msize = (int)((MipOffsets[0] + ImageData.Length - MipOffsets[mipLevel]) / ArrayCount);

                    byte[] data_ = Utils.SubArray(ImageData, ArrayOffset + MipOffsets[mipLevel], (uint)msize);

                    try
                    {
                        Pitch = TegraX1Swizzle.round_up(width__ * bpp, 64);
                        SurfaceSize += Pitch * TegraX1Swizzle.round_up(height__, Math.Max(1, blockHeight >> blockHeightShift) * 8);

                        Console.WriteLine($"{width} {height} {blkWidth} {blkHeight} {target} {bpp} {TileMode} {(int)Math.Max(0, BlockHeightLog2 - blockHeightShift)} {data_.Length}");
                        byte[] result = TegraX1Swizzle.deswizzle(width, height, depth, blkWidth, blkHeight, blkDepth, target, bpp, TileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), data_);
                        //Create a copy and use that to remove uneeded data
                        byte[] result_ = new byte[size];
                        Array.Copy(result, 0, result_, 0, size);
                        result = null;

                        if (ArrayLevel == arrayLevel && MipLevel == mipLevel)
                            return result_;
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show($"Failed to swizzle texture {Text}!");
                        Console.WriteLine(e);

                        return new byte[0];
                    }
                }

                ArrayOffset += (uint)(ImageData.Length / ArrayCount);
            }
            return new byte[0];
        }
        MenuItem save = new MenuItem("Save");
        MenuItem export = new MenuItem("Export");
        MenuItem replace = new MenuItem("Replace");
        MenuItem useSizeRestrictions = new MenuItem("UseSizeRestrictions");

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = true;

            Read(new FileReader(stream));

            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            useSizeRestrictions.Checked = true;

            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(save);
            ContextMenu.MenuItems.Add(useSizeRestrictions);
            ContextMenu.MenuItems.Add(export);
            ContextMenu.MenuItems.Add(replace);

            save.Click += Save;
            replace.Click += Replace;
            export.Click += Export;
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            Write(new FileWriter(mem));
            return mem.ToArray();
        }
    }
}