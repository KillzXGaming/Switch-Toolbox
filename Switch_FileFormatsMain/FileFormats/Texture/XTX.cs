using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class XTX : TreeNodeFile, IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "XTX" };
        public string[] Extension { get; set; } = new string[] { "*.xtx", "*.z" };
        public string Magic { get; set; } = "DFvN ";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public void Load()
        {
            IsActive = true;
            Text = FileName;
            LoadFile(Data);
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public class XTXFormats
        {
            public enum XTXImageFormat : uint
            {
                NVN_FORMAT_RGBA8 = 0x00000025,
                NVN_FORMAT_RGBA8_SRGB = 0x00000038,
                NVN_FORMAT_RGB10A2 = 0x0000003d,
                NVN_FORMAT_RGB565 = 0x0000003c,
                NVN_FORMAT_RGB5A1 = 0x0000003b,
                NVN_FORMAT_RGBA4 = 0x00000039,
                NVN_FORMAT_R8 = 0x00000001,
                NVN_FORMAT_RG8 = 0x0000000d,
                DXT1 = 0x00000042,
                DXT3 = 0x00000043,
                DXT5 = 0x00000044,
                BC4U = 0x00000049,
                BC4S = 0x0000004a,
                BC5U = 0x0000004b,
                BC5S = 0x0000004c,
            };

            public static uint blk_dims(uint format)
            {
                switch (format)
                {
                    case (uint)XTXImageFormat.DXT1:
                    case (uint)XTXImageFormat.DXT3:
                    case (uint)XTXImageFormat.DXT5:
                    case (uint)XTXImageFormat.BC4U:
                    case (uint)XTXImageFormat.BC4S:
                    case (uint)XTXImageFormat.BC5U:
                    case (uint)XTXImageFormat.BC5S:
                    case 0x2d:
                        return 0x44;

                    default: return 0x11;
                }
            }

            public static uint bpps(uint format)
            {
                switch (format)
                {
                    case (uint)XTXImageFormat.NVN_FORMAT_R8:
                        return 1;

                    case (uint)XTXImageFormat.NVN_FORMAT_RGBA8:
                    case (uint)XTXImageFormat.NVN_FORMAT_RGBA8_SRGB:
                    case (uint)XTXImageFormat.NVN_FORMAT_RGB10A2:
                        return 4;

                    case (uint)XTXImageFormat.NVN_FORMAT_RGB565:
                    case (uint)XTXImageFormat.NVN_FORMAT_RGB5A1:
                    case (uint)XTXImageFormat.NVN_FORMAT_RGBA4:
                    case (uint)XTXImageFormat.NVN_FORMAT_RG8:
                        return 2;

                    case (uint)XTXImageFormat.DXT1:
                    case (uint)XTXImageFormat.BC4S:
                    case (uint)XTXImageFormat.BC4U:
                        return 8;

                    case (uint)XTXImageFormat.DXT3:
                    case (uint)XTXImageFormat.DXT5:
                    case (uint)XTXImageFormat.BC5U:
                    case (uint)XTXImageFormat.BC5S:
                        return 16;
                    default: return 0x00;
                }
            }
        }

        public uint HeaderSize { get; set; }
        public uint MajorVersion { get; set; }
        public uint MinorVersion { get; set; }
        public BlockHeader blockHeader { get; set; }
        private const int texHeadBlkType = 2;
        private const int dataBlkType = 3;

        public void LoadFile(byte[] data)
        {
            FileReader reader = new FileReader(new MemoryStream(data));
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "DFvN")
                throw new Exception($"Invalid signature {Signature}! Expected DFvN.");

            HeaderSize = reader.ReadUInt32();
            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt32();

            blockHeader = new BlockHeader();
            blockHeader.Read(reader);
        }

        public override void OnClick(TreeView treeview)
        {
            UpdateEditor();
        }

        public void UpdateEditor()
        {
            if (Viewport.Instance.gL_ControlModern1.Visible == false)
                PluginRuntime.FSHPDockState = WeifenLuo.WinFormsUI.Docking.DockState.Document;

            XTXEditor docked = (XTXEditor)LibraryGUI.Instance.GetContentDocked(new XTXEditor());
            if (docked == null)
            {
                docked = new XTXEditor();
                LibraryGUI.Instance.LoadDockContent(docked, PluginRuntime.FSHPDockState);
            }
            docked.Text = Text;
            docked.Dock = DockStyle.Fill;
            docked.LoadProperty(blockHeader.textureInfo);
        }

        public class BlockHeader
        {
            public uint BlockSize { get; set; }
            public UInt64 DataSize { get; set; }
            public uint BlockType { get; set; }
            public uint GlobalBlockIndex { get; set; }
            public uint IncBlockTypeIndex { get; set; }
            public TextureInfo textureInfo { get; set; }
            public long DataOffset;

            public void Read(FileReader reader)
            {
                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != "HBvN")
                    throw new Exception($"Invalid signature {Signature}! Expected HBvN.");

                BlockSize = reader.ReadUInt32();
                DataSize = reader.ReadUInt64();
                DataOffset = reader.ReadInt64();
                BlockType = reader.ReadUInt32();
                GlobalBlockIndex = reader.ReadUInt32();
                IncBlockTypeIndex = reader.ReadUInt32();

                if (BlockType == texHeadBlkType)
                {
                    textureInfo = new TextureInfo();
                    textureInfo.Read(reader);
                }
                if (BlockType == dataBlkType)
                {

                }
            }
        }
        public class TextureInfo : STGenericTexture
        {
            public UInt64 DataSize { get; set; }
            public uint Alignment { get; set; }
            public uint Depth { get; set; }
            public uint Target { get; set; }
            public XTXFormats.XTXImageFormat Format { get; set; }
            public uint MipCount { get; set; }
            public uint SliceSize { get; set; }
            public uint[] MipOffsets { get; set; }
            public BlockHeader DataBlockHeader { get; set; }
            public byte[] ImageData;


            public void Read(FileReader reader)
            {
                DataSize = reader.ReadUInt64();
                Alignment = reader.ReadUInt32();
                Width = reader.ReadUInt32();
                Height = reader.ReadUInt32();
                Depth = reader.ReadUInt32();
                Target = reader.ReadUInt32();
                Format = reader.ReadEnum<XTXFormats.XTXImageFormat>(true);
                MipCount = reader.ReadUInt32();
                SliceSize = reader.ReadUInt32();
                long offPos = reader.Position;
                MipOffsets = reader.ReadUInt32s((int)MipCount);

                reader.Seek(offPos + 68, SeekOrigin.Begin);
                byte[] Layout = reader.ReadBytes(8);
                byte Sparse = reader.ReadByte();
                reader.Seek(3);
                long DataBlockOff = reader.Position;

                DataBlockHeader = new BlockHeader();
                DataBlockHeader.Read(reader);

                reader.Seek(DataBlockOff + DataBlockHeader.DataOffset, SeekOrigin.Begin);
                long datastart = reader.Position;
                ImageData = reader.ReadBytes((int)DataSize);

                if (ImageData.Length == 0)
                    throw new System.Exception("Empty data size!");

                reader.Seek(DataBlockOff + DataBlockHeader.DataOffset + (long)DataBlockHeader.DataSize, SeekOrigin.Begin);
                BlockHeader EndBlockHeader = new BlockHeader();
                EndBlockHeader.Read(reader);
            }

            public void LoadTexture()
            {
                Surfaces.Clear();

                Console.WriteLine(Format);

                uint blk_dim = XTXFormats.blk_dims((uint)((int)Format >> 8));
                uint blkWidth = blk_dim >> 4;
                uint blkHeight = blk_dim & 0xF;

                uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(Height, blkHeight));
                uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;

                int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

                int TileMode = 0;

                uint bpp = XTXFormats.bpps((uint)Format);
                List<byte[]> mips = new List<byte[]>();

                int blockHeightShift = 0;
                for (int mipLevel = 0; mipLevel < MipOffsets.Length; mipLevel++)
                {

                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);

                    //  uint size = width * height * bpp;
                    uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                    byte[] Output = new byte[size];

                    uint mipOffset;
                    if (mipLevel != 0)
                    {
                        mipOffset = (MipOffsets[mipLevel - 1]);
                        if (mipLevel == 1)
                            mipOffset -= (uint)size;

                        Array.Copy(ImageData, mipOffset, Output, 0, size);
                    }
                    else
                        Output = ImageData;

                    byte[] output = new byte[size];
                    Console.WriteLine(mipLevel + " " + size);

                    if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                        blockHeightShift += 1;

                    byte[] result = TegraX1Swizzle.deswizzle(width, height, blkWidth, blkHeight, (int)Target, bpp, (uint)TileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), Output);
                    //Create a copy and use that to remove uneeded data
                    byte[] result_ = new byte[size];
                    Array.Copy(result, 0, result_, 0, size);

                    mips.Add(result_);
                }
                Surfaces.Add(new Surface() { mipmaps = mips });
            }
        }
    }
}
