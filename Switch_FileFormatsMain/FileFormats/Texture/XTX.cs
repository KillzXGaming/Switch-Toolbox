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
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class XTX : TreeNodeFile, IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "XTX" };
        public string[] Extension { get; set; } = new string[] { "*.xtx", "*.z" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "DFvN");
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

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            LoadFile(stream);
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

        public void LoadFile(Stream data)
        {
            FileReader reader = new FileReader(data);
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "DFvN")
                throw new Exception($"Invalid signature {Signature}! Expected DFvN.");

            HeaderSize = reader.ReadUInt32();
            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt32();

            blockHeader = new BlockHeader();
            blockHeader.Read(reader);
            Nodes.Add(blockHeader.textureInfo);

            reader.Close();
            reader.Dispose();
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

                int indx = 0;
                if (BlockType == texHeadBlkType)
                {
                    textureInfo = new TextureInfo();
                    textureInfo.Read(reader);
                    textureInfo.Text = "Texture " + indx++;
                }
                if (BlockType == dataBlkType)
                {

                }
            }
        }
        public class TextureInfo : STGenericTexture
        {
            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                      TEX_FORMAT.BC1_UNORM,
                      TEX_FORMAT.BC2_UNORM,
                      TEX_FORMAT.BC3_UNORM,
                      TEX_FORMAT.BC4_UNORM,
                      TEX_FORMAT.BC5_UNORM,
                      TEX_FORMAT.R8_UNORM,
                      TEX_FORMAT.R8G8_UNORM,
                      TEX_FORMAT.R8G8_UNORM,
                      TEX_FORMAT.R10G10B10A2_UNORM,
                      TEX_FORMAT.B5G6R5_UNORM,
                      TEX_FORMAT.B5G5R5A1_UNORM,
                      TEX_FORMAT.B4G4R4A4_UNORM,
                      TEX_FORMAT.R8G8B8A8_UNORM,
                      TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                    };
                }
            }

            public override bool CanEdit { get; set; } = false;

            public UInt64 DataSize { get; set; }
            public uint Alignment { get; set; }
            public uint Depth { get; set; }
            public uint Target { get; set; }
            public XTXFormats.XTXImageFormat XTXFormat { get; set; }
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
                XTXFormat = reader.ReadEnum<XTXFormats.XTXImageFormat>(true);
                Format = ConvertFormat(XTXFormat);

                MipCount = reader.ReadUInt32();
                ArrayCount = 1;

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
                EndBlockHeader.Read(reader);            }

            public override void OnClick(TreeView treeview)
            {
                UpdateEditor();
            }
            public void UpdateEditor()
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;

                    LibraryGUI.Instance.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.LoadImage(this);
            }

            private static TEX_FORMAT ConvertFormat(XTXFormats.XTXImageFormat Format)
            {
                switch (Format)
                {
                    case XTXFormats.XTXImageFormat.DXT1: return TEX_FORMAT.BC1_UNORM;
                    case XTXFormats.XTXImageFormat.DXT3: return TEX_FORMAT.BC2_UNORM;
                    case XTXFormats.XTXImageFormat.DXT5: return TEX_FORMAT.BC3_UNORM;
                    case XTXFormats.XTXImageFormat.BC4U: return TEX_FORMAT.BC4_UNORM;
                    case XTXFormats.XTXImageFormat.BC4S: return TEX_FORMAT.BC4_SNORM;
                    case XTXFormats.XTXImageFormat.BC5U: return TEX_FORMAT.BC5_UNORM;
                    case XTXFormats.XTXImageFormat.BC5S: return TEX_FORMAT.BC5_SNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_R8: return TEX_FORMAT.R8_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RG8: return TEX_FORMAT.R8G8_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGB10A2: return TEX_FORMAT.R10G10B10A2_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGB565: return TEX_FORMAT.B5G6R5_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGB5A1: return TEX_FORMAT.B5G5R5A1_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA4: return TEX_FORMAT.B4G4R4A4_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8: return TEX_FORMAT.R8G8B8A8_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                    default:
                        throw new Exception($"Cannot convert format {Format}");
                }
            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                throw new NotImplementedException("Cannot set image data! Operation not implemented!");
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                uint bpp = GetBytesPerPixel(Format);
                uint blkWidth = GetBlockHeight(Format);
                uint blkHeight = GetBlockWidth(Format);
                uint blkDepth = GetBlockDepth(Format);

                uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(Height, blkHeight));
                uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;

                int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

                int TileMode = 0;


                List<byte[]> mips = new List<byte[]>();

                int blockHeightShift = 0;
                for (int mipLevel = 0; mipLevel < MipOffsets.Length; mipLevel++)
                {

                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);
                    uint depth = (uint)Math.Max(1, Depth >> mipLevel);

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

                    byte[] result = TegraX1Swizzle.deswizzle(width, height, depth, blkWidth, blkHeight, blkDepth, (int)Target, bpp, (uint)TileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), Output);
                    //Create a copy and use that to remove uneeded data
                    byte[] result_ = new byte[size];
                    Array.Copy(result, 0, result_, 0, size);

                    result = null;

                    if (MipLevel == mipLevel)
                        return result_;

                    Console.WriteLine("bpp " + bpp);
                    Console.WriteLine("result_ " + size);
                    Console.WriteLine("width " + width);
                    Console.WriteLine("height " + height);

                    break;
                }
                return new byte[0];
            }
        }
    }
}
