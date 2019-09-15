using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class G1TFile
    {
        public GT1Platform Platform;

        public bool IsBigEndian = false;

        public List<GITextureWrapper> Textures = new List<GITextureWrapper>();

        public enum GT1Platform
        {
            PC,
            WiiU,
            Switch,
        }

        public void Read(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            long StartPos = reader.Position;
            string Magic = reader.ReadString(4);

            if (Magic == "GT1G")
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            else
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            IsBigEndian = reader.IsBigEndian;

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

                GITextureWrapper tex = new GITextureWrapper(this);
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
                Textures.Add(tex);
            }
        }

        public void Write(FileWriter writer)
        {

        }

        private static byte SwapEndianByte(byte x)
        {
            return (byte)(((x & 0x0F) << 4) | ((x & 0xF0) >> 4));
        }


        public class GITextureWrapper : STGenericTexture
        {
            public G1TFile ContainerParent;

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

            public GITextureWrapper(G1TFile GT1)
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
                if (ContainerParent.Platform == GT1Platform.WiiU)
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
                else
                {
                    return ImageData;
                }
            }

            private void GetMipmaps(STGenericTexture tex, byte[] ImageData)
            {
                uint width = 0;
                uint height = 0;

                for (int a = 0; a < tex.ArrayCount; a++)
                {
                    for (int m = 0; m < tex.MipCount; m++)
                    {
                        width = (uint)Math.Max(1, tex.Width >> m);
                        Height = (uint)Math.Max(1, tex.Height >> m);

                    }
                }
            }
        }

    }
}
