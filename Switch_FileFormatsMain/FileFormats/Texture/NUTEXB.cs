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

namespace FirstPlugin
{
    public class NUTEXB : STGenericTexture, IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "TEX" };
        public string[] Extension { get; set; } = new string[] { "*.nutexb" };
        public string Magic { get; set; } = "XET";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

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
        class MenuExt : IFileMenuExtension
        {
            public ToolStripItemDark[] NewFileMenuExtensions => null;
            public ToolStripItemDark[] ToolsMenuExtensions => newFileExt;
            public ToolStripItemDark[] TitleBarExtensions => null;
            public ToolStripItemDark[] CompressionMenuExtensions => null;
            public ToolStripItemDark[] ExperimentalMenuExtensions => null;

            ToolStripItemDark[] newFileExt = new ToolStripItemDark[1];
            public MenuExt()
            {
                newFileExt[0] = new ToolStripItemDark("Batch Export NUTEXB");
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
                            Bitmap bitmap = texture.DisplayTexture();

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
                        texture.surfaces.Clear();


                        texture = null;
                        GC.Collect();
                    }
                }
            }
        }
        public bool BadSwizzle;
        public uint Width;
        public uint Height;
        public uint unk;
        public int unk2;
        public int unk3;

        public NUTEXImageFormat Format;
        public List<uint[]> mipSizes = new List<uint[]>();
        public int Alignment;
        public byte[] ImageData;
        bool IsSwizzled = true;
        public string ArcOffset; //Temp for exporting in batch 

        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            /*  ofd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                           "Microsoft DDS |*.dds|" +
                           "Portable Network Graphics |*.png|" +
                           "Joint Photographic Experts Group |*.jpg|" +
                           "Bitmap Image |*.bmp|" +
                           "Tagged Image File Format |*.tiff|" +
                           "All files(*.*)|*.*";*/
            ofd.Filter = "Supported Formats|*.dds;" +
                                 "Microsoft DDS |*.dds" +
                                 "All files(*.*)|*.*";

            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var bntxFile = new BNTX();
                var tex = new TextureData();
                tex.Replace(ofd.FileName);

                if (surfaces[0].mipmaps[0].Length != surfaces[0].mipmaps[0].Length)
                    throw new Exception("Image must be the same size!");

                if (surfaces[0].mipmaps.Count != tex.surfaces[0].mipmaps.Count)
                    throw new Exception("Map map count must be the same!");

                if (Width != tex.Texture.Width || Height != tex.Texture.Height)
                    throw new Exception("Image size must be the same!");

                ImageData = tex.Texture.TextureData[0][0];
                surfaces = tex.surfaces;

                Width = tex.Texture.Width;
                Height = tex.Texture.Height;

                UpdateEditor();
            }
        }
        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Text;
            sfd.DefaultExt = "png";
            sfd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export(sfd.FileName);
            }
        }
        private void Save(object sender, EventArgs args)
        {
            List<IFileFormat> formats = new List<IFileFormat>();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }
        public void Export(string FileName, bool ExportSurfaceLevel = false,
      bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();
            switch (ext)
            {
                case ".dds":
                    SaveDDS(FileName);
                    break;
                default:
                    SaveBitMap(FileName);
                    break;
            }
        }
        internal void SaveBitMap(string FileName, int SurfaceLevel = 0, int MipLevel = 0)
        {
            Bitmap bitMap = DisplayTexture(SurfaceLevel, MipLevel);

            bitMap.Save(FileName);
        }
        internal void SaveDDS(string FileName)
        {
            DDS dds = new DDS();
            dds.header = new DDS.Header();
            dds.header.width = Width;
            dds.header.height = Height;
            dds.header.mipmapCount = (uint)surfaces[0].mipmaps.Count;
            dds.header.pitchOrLinearSize = (uint)surfaces[0].mipmaps[0].Length;

            if (IsCompressedFormat((NUTEXImageFormat)Format))
                dds.SetFlags(GetCompressedDXGI_FORMAT((NUTEXImageFormat)Format));
            else
                dds.SetFlags(GetUncompressedDXGI_FORMAT((NUTEXImageFormat)Format));


            dds.Save(dds, FileName, surfaces);
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
            unk3 = reader.ReadInt32();
            Format = reader.ReadEnum<NUTEXImageFormat>(true);
            unk = reader.ReadByte(); //Related to pixel size?? 
            ushort padding3 = reader.ReadUInt16();
            unk2 = reader.ReadInt32();
            uint mipCount = reader.ReadUInt32();
            Alignment = reader.ReadInt32();
            uint ArrayCount = reader.ReadUInt32(); //6 for cubemaps
            int imagesize = reader.ReadInt32();

            reader.Seek(imagesize, System.IO.SeekOrigin.Begin); //Get mipmap sizes
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                long mipPos = reader.Position;
                uint[] mips = reader.ReadUInt32s((int)mipCount);
                mipSizes.Add(mips);

                //Each mip section is 0x40 size for each array
                //Seek to next one
                reader.Seek(mipPos + 0x40, System.IO.SeekOrigin.Begin);
            }
            reader.Seek(0, System.IO.SeekOrigin.Begin);
            ImageData = reader.ReadBytes(imagesize);

            LoadTexture();
        }
        public void Write(FileWriter writer)
        {
            int arrayCount = mipSizes.Count;

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
            writer.Write(unk3);
            writer.Write((byte)Format);
            writer.Write((byte)unk);
            writer.Seek(2); //padding
            writer.Write(unk2);
            writer.Write(mipSizes.Count);
            writer.Write(Alignment);
            writer.Write(arrayCount);
            writer.Write(ImageData.Length);
            writer.WriteSignature(" XET");
            writer.Write(131073);

        }
        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }

        public Bitmap DisplayTexture(int ArrayIndex = 0, int DisplayMipIndex = 0)
        {
            if (BadSwizzle)
                return BitmapExtension.GetBitmap(Properties.Resources.Black, 32, 32);

            if (IsSwizzled)
                LoadTexture();
            else
                surfaces.Add(new Surface() {mipmaps = new List<byte[]>() { ImageData } } );

            if (surfaces[0].mipmaps.Count <= 0)
            {
                return BitmapExtension.GetBitmap(Properties.Resources.Black, 32, 32);
            }

            Console.WriteLine(ArrayIndex);

            uint width = (uint)Math.Max(1, Width >> DisplayMipIndex);
            uint height = (uint)Math.Max(1, Height >> DisplayMipIndex);

            byte[] data = surfaces[ArrayIndex].mipmaps[DisplayMipIndex];

            return DecodeBlock(data, width, height, (NUTEXImageFormat)Format);
        }
        public static Bitmap DecodeBlock(byte[] data, uint Width, uint Height, NUTEXImageFormat Format)
        {
            Bitmap decomp;

            Console.WriteLine(Format);

            if (Format == NUTEXImageFormat.BC5_SNORM)
                return DDSCompressor.DecompressBC5(data, (int)Width, (int)Height, true);

            byte[] d = null;
            if (IsCompressedFormat(Format))
                d = DDSCompressor.DecompressBlock(data, (int)Width, (int)Height, GetCompressedDXGI_FORMAT(Format));
            else
                d = DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, GetUncompressedDXGI_FORMAT(Format));

            if (d != null)
            {
                decomp = BitmapExtension.GetBitmap(d, (int)Width, (int)Height);
                return TextureData.SwapBlueRedChannels(decomp);
            }
            return null;
        }
        private static DDS.DXGI_FORMAT GetUncompressedDXGI_FORMAT(NUTEXImageFormat Format)
        {
            switch (Format)
            {
                case NUTEXImageFormat.R32G32B32A32_FLOAT: return DDS.DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;
                case NUTEXImageFormat.R8G8B8A8_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                case NUTEXImageFormat.R8G8B8A8_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                case NUTEXImageFormat.B8G8R8A8_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                case NUTEXImageFormat.B8G8R8A8_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                default:
                    throw new Exception($"Cannot convert format {Format}");
            }
        }
        private static DDS.DXGI_FORMAT GetCompressedDXGI_FORMAT(NUTEXImageFormat Format)
        {
            //This uses UNORM instead of SRGB due to decod
            switch (Format)
            {
                case NUTEXImageFormat.BC1_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                case NUTEXImageFormat.BC1_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                case NUTEXImageFormat.BC2_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
                case NUTEXImageFormat.BC2_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
                case NUTEXImageFormat.BC3_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                case NUTEXImageFormat.BC3_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                case NUTEXImageFormat.BC4_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
                case NUTEXImageFormat.BC4_SNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM;
                case NUTEXImageFormat.BC5_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
                case NUTEXImageFormat.BC5_SNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM;
                case NUTEXImageFormat.BC6_UFLOAT: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16;
                case NUTEXImageFormat.BC7_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
                case NUTEXImageFormat.BC7_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
                default:
                    throw new Exception($"Cannot convert format {Format}");
            }
        }
        private static bool IsCompressedFormat(NUTEXImageFormat Format)
        {
            switch (Format)
            {
                case NUTEXImageFormat.BC1_UNORM:
                case NUTEXImageFormat.BC1_SRGB:
                case NUTEXImageFormat.BC2_UNORM:
                case NUTEXImageFormat.BC2_SRGB:
                case NUTEXImageFormat.BC3_UNORM:
                case NUTEXImageFormat.BC3_SRGB:
                case NUTEXImageFormat.BC4_UNORM:
                case NUTEXImageFormat.BC4_SNORM:
                case NUTEXImageFormat.BC5_UNORM:
                case NUTEXImageFormat.BC5_SNORM:
                case NUTEXImageFormat.BC6_UFLOAT:
                case NUTEXImageFormat.BC7_UNORM:
                case NUTEXImageFormat.BC7_SRGB:
                    return true;
                default:
                    return false;
            }
        }

        public void LoadTexture(int target = 1)
        {
            surfaces.Clear();

            uint blk_dim = blk_dims((byte)Format);
            uint blkWidth = blk_dim >> 4;
            uint blkHeight = blk_dim & 0xF;

            uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;
            uint tileMode = 0;

            int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

            uint bpp = bpps((byte)Format);

            for (int arrayLevel = 0; arrayLevel < mipSizes.Count; arrayLevel++)
            {
                int blockHeightShift = 0;
                uint mipOffset = 0;

                List<byte[]> mips = new List<byte[]>();
                for (int mipLevel = 0; mipLevel < mipSizes[arrayLevel].Length; mipLevel++)
                {
                    //Get the size from the size array
                    int MipSize = (int)mipSizes[arrayLevel][mipLevel];

                    //Align the size
                    if (mipLevel == 0)
                        if (MipSize % Alignment != 0) MipSize = MipSize + (Alignment - (MipSize % Alignment));

                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);

                    uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                    if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                        blockHeightShift += 1;

                    Console.WriteLine($"{blk_dim.ToString("x")} {bpp} {width} {height} {linesPerBlockHeight} {blkWidth} {blkHeight} {size} { ImageData.Length}");

                    try
                    {
                        byte[] result = TegraX1Swizzle.deswizzle(width, height, blkWidth, blkHeight, target, bpp, tileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), ImageData);
                        //Create a copy and use that to remove uneeded data
                        byte[] result_ = new byte[size];
                        Array.Copy(result, mipOffset, result_, 0, size);

                        mips.Add(result_);
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show($"Failed to swizzle texture {Text}!");
                        Console.WriteLine(e);
                        BadSwizzle = true;
                        break;
                    }
                    mipOffset += (uint)MipSize;
                    break;
                }
                surfaces.Add(new Surface() { mipmaps = mips });
            }
        }

        public void UpdateEditor()
        {
            if (Viewport.Instance.gL_ControlModern1.Visible == false)
                PluginRuntime.FSHPDockState = WeifenLuo.WinFormsUI.Docking.DockState.Document;

            NuTexEditor docked = (NuTexEditor)LibraryGUI.Instance.GetContentDocked(new NuTexEditor());
            if (docked == null)
            {
                docked = new NuTexEditor();
                LibraryGUI.Instance.LoadDockContent(docked, PluginRuntime.FSHPDockState);
            }
            docked.Text = Text;
            docked.Dock = DockStyle.Fill;
            docked.LoadProperty(this);
        }
        MenuItem save = new MenuItem("Save");
        MenuItem export = new MenuItem("Export");
        MenuItem replace = new MenuItem("Replace");

        public void Load()
        {
            IsActive = true;
            Text = FileName;
            Read(new FileReader(Data));

            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(save);
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
