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
    public class GT1 : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GT1" };
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

        public byte[] Save()
        {
            return null;
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
            string Version = reader.ReadString(4);

            if (Magic == "GT1G")
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            else
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            if (Magic == "GT1G")
                Platform = GT1Platform.Switch;
            else
                Platform = GT1Platform.WiiU;

            Console.WriteLine(Platform);

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
                uint NextOffset = reader.ReadUInt32();

                if (i == TextureCount - 1)
                    NextOffset = (uint)reader.BaseStream.Length - DataOffset;

                reader.SeekBegin(DataOffset + InfoOffset);
                Console.WriteLine(reader.Position);

                byte numMips = reader.ReadByte();
                byte format = reader.ReadByte();
                byte texDims = reader.ReadByte();
                byte unknown3 = reader.ReadByte(); //1
                byte unknown4 = reader.ReadByte(); //0
                byte unknown5 = reader.ReadByte(); //1
                byte unknown6 = reader.ReadByte(); //12
                byte unknown7 = reader.ReadByte(); //1
                uint unknown8 = reader.ReadUInt32();
                uint padding4 = reader.ReadUInt32();
                uint unknown9 = reader.ReadUInt32(); //1

                if (reader.ByteOrder == Syroot.BinaryData.ByteOrder.LittleEndian)
                {
                    numMips = SwapEndianByte(numMips);
                    texDims = SwapEndianByte(texDims);
                }

                uint Width = (uint)(texDims & 0xF0) / 16;
                uint Height = (uint)(texDims & 0x0F);
                Width = (uint)Math.Pow(2, Width);
                Height = (uint)Math.Pow(2, Height);

                // uint Width = (uint)(1 << (texDims >> 4));
                //  uint Height = (uint)(1 << (texDims & 0x0F));

                const uint InfoHeaderSize = 20;

                uint ImageSize = NextOffset - InfoOffset - InfoHeaderSize;

                GITexture tex = new GITexture(this);
                tex.ImageKey = "texture";
                tex.SelectedImageKey = tex.ImageKey;
                tex.Text = $"Texture {i}  {format.ToString("x")}";
                tex.Width = Width;
                tex.Height = Height;
                tex.MipCount = numMips;

                switch (format)
                {
                    case 0x01: //PC?
                    case 0x09: //Wii U
                        tex.Format = TEX_FORMAT.R32G32B32A32_FLOAT;
                        break;
                    case 0x06:
                    case 0x59: //Switch
                    case 0x60: //Wii U
                        tex.Format = TEX_FORMAT.BC1_UNORM;
                        break;
                    case 0x12:
                    case 0x5B:  //Switch
                    case 0x62:  //Wii U
                        tex.Format = TEX_FORMAT.BC3_UNORM;
                        break;
                    default:
                        throw new Exception("Unsupported format! " + format.ToString("x"));
                }

                tex.ImageData = reader.ReadBytes((int)ImageSize);
                Nodes.Add(tex);
            }
        }

        private static byte SwapEndianByte(byte x)
        {
            return (byte)(((x & 0x0F) << 4) | ((x & 0xF0) >> 4));
        }
    }

    public class GITexture : STGenericTexture
    {
        public GT1 ContainerParent;

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

        public GITexture(GT1 GT1)
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
            if (ContainerParent.Platform == GT1.GT1Platform.WiiU)
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
            else if (ContainerParent.Platform == GT1.GT1Platform.Switch)
            {
                return ImageData;

                return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, 1);
            }
            else
            {
                return ImageData;
            }
        }
    }

}
