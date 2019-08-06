using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Drawing;

namespace FirstPlugin
{ 
    public class G1T : TreeNodeFile, IFileFormat, IContextMenuNode, ITextureIconLoader
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "G1T Textre" };
        public string[] Extension { get; set; } = new string[] { "*.g1t" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "G1TG") || reader.CheckSignature(4, "GT1G");
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

        public void Load(Stream stream)
        {
            Read(new FileReader(stream));
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
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

        public GT1Platform Platform;

        public enum GT1Platform
        {
            PC,
            WiiU,
            Switch,
        }

        public void Read(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            Text = FileName;

            long StartPos = reader.Position;
            string Magic = reader.ReadString(4);

            if (Magic == "GT1G")
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            else
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            if (Magic == "GT1G")
                Platform = GT1Platform.Switch;
            else
                Platform = GT1Platform.WiiU;

            string Version = reader.ReadString(4);
            uint FileSize = reader.ReadUInt32();
            uint DataOffset = reader.ReadUInt32();
            uint TextureCount = reader.ReadUInt32();
            uint unk = reader.ReadUInt32();
            uint unk2 = reader.ReadUInt32();
            uint[] unk3s = reader.ReadUInt32s((int)TextureCount);

            for (int i = 0; i < TextureCount; i++)
            {
                reader.SeekBegin(StartPos + DataOffset + (i * 4));

                uint InfoOffset = reader.ReadUInt32();

                reader.SeekBegin(DataOffset + InfoOffset);

                byte numMips = reader.ReadByte();
                byte format = reader.ReadByte();
                byte texDims = reader.ReadByte();
                byte unknown3 = reader.ReadByte(); //1
                byte unknown4 = reader.ReadByte(); //0
                byte unknown5 = reader.ReadByte(); //1
                byte unknown6 = reader.ReadByte(); //12
                byte extraHeader = reader.ReadByte();

                if (reader.ByteOrder == Syroot.BinaryData.ByteOrder.LittleEndian)
                {
                    numMips = SwapEndianByte(numMips);
                    texDims = SwapEndianByte(texDims);
                    extraHeader = SwapEndianByte(extraHeader);
                }

                if (extraHeader == 1)
                {
                    var extSize = reader.ReadInt32();
                    var ext = reader.ReadBytes(extSize - 4);
                }

                uint Width = (uint)Math.Pow(2, texDims / 16);
                uint Height = (uint)Math.Pow(2, texDims % 16);

                GITexture tex = new GITexture(this);
                tex.ImageKey = "texture";
                tex.SelectedImageKey = tex.ImageKey;
                tex.Text = $"Texture {i}  {format.ToString("x")}";
                tex.Width = Width;
                tex.Height = Height;
                tex.MipCount = numMips;

                switch (format)
                {
                    case 0x00: //ABGR
                    case 0x01: //BGRA 32 bit (no mip maps)
                    case 0x02: //RGBA 32 bit
                    case 0x09:
                        tex.Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        break;
                    case 0x06:
                    case 0x10: //PC and xbox (swizzled)
                    case 0x59: 
                    case 0x60: //Swizzled
                        tex.Format = TEX_FORMAT.BC1_UNORM;
                        break;
                    case 0x7:
                    case 0x8:
                    case 0x12: //PC and xbox (swizzled)
                    case 0x5B: 
                    case 0x62: //bc1 swizzled
                        tex.Format = TEX_FORMAT.BC3_UNORM;
                        break;
                    case 0x5C:
                        tex.Format = TEX_FORMAT.BC4_UNORM;
                        break;
                    case 0x5D: //DXT5 swizzled or ATI2
                        tex.Format = TEX_FORMAT.BC5_UNORM;
                        break;
                    case 0x5E:
                        tex.Format = TEX_FORMAT.BC6H_UF16; //Uses cubemaps
                        break;
                    case 0x5F:
                        tex.Format = TEX_FORMAT.BC7_UNORM;
                        break;
                    default:
                        throw new Exception("Unsupported format! " + format.ToString("x"));
                }

                uint textureSize = (Width * Height * STGenericTexture.GetBytesPerPixel(tex.Format)) / 8;
                if (format == 0x09)
                    textureSize = (Width * Height * 64) / 8;
                if (format == 0x01)
                {
                    textureSize = (Width * Height * 32) / 8;
                    tex.Parameters.DontSwapRG = true;
                }

                tex.ImageData = reader.ReadBytes((int)textureSize);
                Nodes.Add(tex);
            }
        }

        private static byte SwapEndianByte(byte x)
        {
            return (byte)(((x & 0x0F) << 4) | ((x & 0xF0) >> 4));
        }

        public virtual ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportAllAction(object sender, EventArgs args)
        {
            ExportAll();
        }

        public virtual void ExportAll()
        {
            List<string> Formats = new List<string>();
            Formats.Add("Microsoft DDS (.dds)");
            Formats.Add("Portable Graphics Network (.png)");
            Formats.Add("Joint Photographic Experts Group (.jpg)");
            Formats.Add("Bitmap Image (.bmp)");
            Formats.Add("Tagged Image File Format (.tiff)");

            FolderSelectDialog sfd = new FolderSelectDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (STGenericTexture tex in Nodes)
                    {
                        if (form.Index == 0)
                            tex.SaveDDS(folderPath + '\\' + tex.Text + ".dds");
                        else if (form.Index == 1)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".png");
                        else if (form.Index == 2)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".jpg");
                        else if (form.Index == 3)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".bmp");
                        else if (form.Index == 4)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".tiff");
                    }
                }
            }
        }
    }

    public class GITexture : STGenericTexture
    {
        public G1T ContainerParent;

        public override bool CanEdit { get; set; } = false;

        public byte[] ImageData;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
                    TEX_FORMAT.R8G8B8A8_UNORM,
                };
            }
        }

        public GITexture(G1T GT1)
        {
            ContainerParent = GT1;
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

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            if (ContainerParent.Platform == G1T.GT1Platform.WiiU)
            {
                uint bpp = GetBytesPerPixel(Format);

                GX2.GX2Surface surf = new GX2.GX2Surface();
                surf.bpp = bpp;
                surf.height = Height;
                surf.width = Width;
                surf.aa = (uint)GX2.GX2AAMode.GX2_AA_MODE_1X;
                surf.alignment = 0;
                surf.depth = 1;
                surf.dim = (uint)GX2.GX2SurfaceDimension.DIM_2D;
                surf.format = (uint)Bfres.Structs.FTEX.ConvertToGx2Format(Format);
                surf.use = (uint)GX2.GX2SurfaceUse.USE_COLOR_BUFFER;
                surf.pitch = 0;
                surf.data = ImageData;
                surf.numMips = MipCount;
                surf.mipOffset = new uint[0];
                surf.mipData = ImageData;
                surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;

                //  surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;
                surf.swizzle = 0;
                surf.numArray = 1;

                return GX2.Decode(surf, ArrayLevel, MipLevel);
            }
            else if (ContainerParent.Platform == G1T.GT1Platform.Switch)
            {
                return ImageData;
             //   return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, 1);
            }
            else
            {
                return ImageData;
            }
        }
    }

}
