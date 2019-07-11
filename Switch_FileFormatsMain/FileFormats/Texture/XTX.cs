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
        public FileType FileType { get; set; } = FileType.Image;

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
            CanSave = true;
            Text = FileName;
            LoadFile(stream);

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "xtx";
            sfd.Filter = "Supported Formats|*.xtx;";
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            SaveFile(new FileWriter(mem));
            return mem.ToArray();
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
        public List<BlockHeader> Blocks { get; set; }
        public List<TextureInfo> TextureInfos { get; set; }
        public List<byte[]> TextureBlocks { get; set; }

        private const int texHeadBlkType = 2;
        private const int dataBlkType = 3;

        public void LoadFile(Stream data)
        {
            Blocks = new List<BlockHeader>();
            TextureInfos = new List<TextureInfo>();
            TextureBlocks = new List<byte[]>();

            FileReader reader = new FileReader(data);
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "DFvN")
                throw new Exception($"Invalid signature {Signature}! Expected DFvN.");

            HeaderSize = reader.ReadUInt32();
            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt32();

            uint TextureBlockType = 2;
            uint DataBlockType = 3;

            reader.Seek(HeaderSize, SeekOrigin.Begin);

            bool blockB = false;
            bool blockC = false;

            uint ImageInfo = 0;
            uint images = 0;

            while (reader.Position < reader.BaseStream.Length)
            {
                Console.WriteLine("BLOCK POS " + reader.Position);
                BlockHeader blockHeader = new BlockHeader();
                blockHeader.Read(reader);
                Blocks.Add(blockHeader);

                if ((uint)blockHeader.BlockType == TextureBlockType)
                {
                    ImageInfo += 1;
                    blockB = true;
                
                    TextureInfo textureInfo = new TextureInfo();
                    textureInfo.Read(new FileReader(blockHeader.Data));
                    textureInfo.Text = "Texture " + ImageInfo;
                    TextureInfos.Add(textureInfo);
                }
                if ((uint)blockHeader.BlockType == DataBlockType)
                {
                    images += 1;
                    blockC = true;

                    TextureBlocks.Add(blockHeader.Data);
                }
            }

            reader.Close();
            reader.Dispose();

            int curTex = 0;

            foreach (var tex in TextureInfos) {
                tex.ImageData = TextureBlocks[curTex++];
                Nodes.Add(tex);
            }
        }

        public void SaveFile(FileWriter writer)
        {
            int Alignment = 512;
            uint TextureInfoType = 2;
            uint TextureBlockType = 3;

            SetupBlockSaving(TextureBlockType);

            writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            writer.WriteSignature("DFvN");
            writer.Write(HeaderSize);
            writer.Write(MajorVersion);
            writer.Write(MinorVersion);
            writer.Seek(HeaderSize, SeekOrigin.Begin);

            int curTexBlock = 0;
            int curTexImage = 0;

            foreach (var block in Blocks)
            {
                if (block.BlockType == TextureBlockType)
                {
                    block.WriteHeader(writer);
                    block.WriteBlock(writer, 512);
                }
                else if (block.BlockType == TextureInfoType)
                {
                    block.Data = TextureInfos[curTexImage++].Write();
                    block.WriteHeader(writer);
                    block.WriteBlock(writer);
                }
                else
                {
                    block.WriteHeader(writer);
                    block.WriteBlock(writer);
                }
            }

            writer.Close();
            writer.Dispose();
        }

        private void SetupBlockSaving(uint TextureBlockType)
        {
            int curTex = 0;
            foreach (var block in Blocks)
            {
                if (block.BlockType == TextureBlockType)
                {
                    block.Data = TextureInfos[curTex].ImageData;
                }
            }
        }

        public class BlockHeader
        {
            public uint BlockSize { get; set; }
            public UInt64 DataSize { get; set; }
            public uint BlockType { get; set; }
            public uint GlobalBlockIndex { get; set; }
            public uint IncBlockTypeIndex { get; set; }
            public long DataOffset;
            public byte[] Data;

            public void Read(FileReader reader)
            {
                var pos = reader.Position;

                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != "HBvN")
                    throw new Exception($"Invalid signature {Signature}! Expected HBvN.");

                BlockSize = reader.ReadUInt32();
                DataSize = reader.ReadUInt64();
                DataOffset = reader.ReadInt64();
                BlockType = reader.ReadUInt32();
                GlobalBlockIndex = reader.ReadUInt32();
                IncBlockTypeIndex = reader.ReadUInt32();

                reader.Seek((int)pos + DataOffset, SeekOrigin.Begin);
                Data = reader.ReadBytes((int)DataSize);
            }

            private long headerPos = 0;
            public void WriteHeader(FileWriter writer)
            {
                headerPos = writer.Position;

                writer.WriteSignature("HBvN");
                writer.Write(BlockSize);
                writer.Write((ulong)Data.Length);
                writer.Write((ulong)DataOffset);
                writer.Write(BlockType);
                writer.Write(GlobalBlockIndex);
                writer.Write(IncBlockTypeIndex);
            }

            public void WriteBlock(FileWriter writer, int DataAlignment = 0)
            {
                //From the block size goto where the data will start
                writer.Seek((int)(BlockSize + headerPos), SeekOrigin.Begin);

                //Add alignment if necessary
                if (DataAlignment != 0)
                    writer.Align(DataAlignment);

                var blockPos = writer.Position;

                //Satisfy the offset
                using (writer.TemporarySeek(headerPos + 16, SeekOrigin.Begin))
                {
                    //Offset starts from the block header
                    writer.Write(blockPos - headerPos);
                }

                writer.Write(Data);

                //Set the total size
                uint TotalDataSize =(uint)(writer.Position - blockPos);
                using (writer.TemporarySeek(headerPos + 8, SeekOrigin.Begin)) {
                    writer.Write((ulong)TotalDataSize);
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

            public override bool CanEdit { get; set; } = true;

            public UInt64 DataSize { get; set; }
            public uint Alignment { get; set; }
            public uint Target { get; set; }
            public XTXFormats.XTXImageFormat XTXFormat { get; set; }
            public uint SliceSize { get; set; }
            public uint[] MipOffsets { get; set; }
            public byte[] ImageData;

            public override string ExportFilter => FileFilters.XTX;
            public override string ReplaceFilter => FileFilters.XTX;

            private byte[] unknownData;

            public void Read(FileReader reader)
            {
                DataSize = reader.ReadUInt64();
                Alignment = reader.ReadUInt32();
                Width = reader.ReadUInt32();
                Height = reader.ReadUInt32();
                Depth = reader.ReadUInt32();
                Target = reader.ReadUInt32();
                XTXFormat = reader.ReadEnum<XTXFormats.XTXImageFormat>(true);
                MipCount = reader.ReadUInt32();
                SliceSize = reader.ReadUInt32();
                MipOffsets = reader.ReadUInt32s((int)MipCount);
                unknownData = reader.ReadBytes(0x38);

                Format = ConvertFormat(XTXFormat);
                ArrayCount = 1;

                ContextMenuStrip = new STContextMenuStrip();
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace", null, ReplaceAction, Keys.Control | Keys.R));
            }

            public byte[] Write()
            {
                MemoryStream mem = new MemoryStream();

                FileWriter writer = new FileWriter(mem);
                writer.Write((ulong)DataSize);
                writer.Write(Alignment);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Depth);
                writer.Write(Target);
                writer.Write(XTXFormat, true);
                writer.Write(MipCount);
                writer.Write(SliceSize);
                writer.Write(MipOffsets);
                writer.Write(unknownData);
                writer.Close();
                writer.Dispose();

                return mem.ToArray();
            }

            public override void Export(string FileName)
            {
                Export(FileName);
            }

            public override void Replace(string FileName)
            {
                var bntxFile = new BNTX();
                var tex = new TextureData();
                tex.Replace(FileName, MipCount, Format);

                //If it's null, the operation is cancelled
                if (tex.Texture == null)
                    return;

                var surfacesNew = tex.GetSurfaces();
                var surfaces = GetSurfaces();

                ImageData = tex.Texture.TextureData[0][0];

                Width = tex.Texture.Width;
                Height = tex.Texture.Height;
                MipCount = tex.Texture.MipCount;

                Format = tex.Format;
                XTXFormat = ConvertFromGenericFormat(tex.Format);

                MipOffsets = TegraX1Swizzle.GenerateMipSizes(tex.Format, tex.Width, tex.Height, tex.Depth, tex.ArrayCount, tex.MipCount, (uint)ImageData.Length)[0];

                surfacesNew.Clear();
                surfaces.Clear();

                UpdateEditor();
            }

            public override void OnClick(TreeView treeview)
            {
                UpdateEditor();
            }
            public void UpdateEditor()
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;

                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.LoadImage(this);
                editor.LoadProperties(GenericProperties);
            }

            private static XTXFormats.XTXImageFormat ConvertFromGenericFormat(TEX_FORMAT Format)
            {
                switch (Format)
                {
                    case TEX_FORMAT.BC1_UNORM: return XTXFormats.XTXImageFormat.DXT1;
                    case TEX_FORMAT.BC2_UNORM: return XTXFormats.XTXImageFormat.DXT3;
                    case TEX_FORMAT.BC3_UNORM: return XTXFormats.XTXImageFormat.DXT5;
                    case TEX_FORMAT.BC4_UNORM: return XTXFormats.XTXImageFormat.BC4U;
                    case TEX_FORMAT.BC4_SNORM: return XTXFormats.XTXImageFormat.BC4S;
                    case TEX_FORMAT.BC5_UNORM: return XTXFormats.XTXImageFormat.BC5U;
                    case TEX_FORMAT.R8_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_R8;
                    case TEX_FORMAT.R8G8_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RG8;
                    case TEX_FORMAT.R10G10B10A2_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGB10A2;
                    case TEX_FORMAT.B5G6R5_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGB565;
                    case TEX_FORMAT.B5G5R5A1_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGB5A1;
                    case TEX_FORMAT.B4G4R4A4_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA4;
                    case TEX_FORMAT.R8G8B8A8_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8;
                    case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8_SRGB;
                    default:
                        throw new Exception($"Cannot convert format {Format}");
                }
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
                if (bitmap == null)
                    return; //Image is likely disposed and not needed to be applied

                MipCount = GenerateMipCount(bitmap.Width, bitmap.Height);

                XTXFormat = ConvertFromGenericFormat(Format);

                var tex = new Syroot.NintenTools.NSW.Bntx.Texture();
                tex.Height = (uint)bitmap.Height;
                tex.Width = (uint)bitmap.Width;
                tex.Format = TextureData.GenericToBntxSurfaceFormat(Format);
                tex.Name = Text;
                tex.Path = "";
                tex.TextureData = new List<List<byte[]>>();

                STChannelType[] channels = SetChannelsByFormat(Format);
                tex.sparseBinding = 0; //false
                tex.sparseResidency = 0; //false
                tex.Flags = 0;
                tex.Swizzle = 0;
                tex.textureLayout = 0;
                tex.Regs = new uint[0];
                tex.AccessFlags = 0x20;
                tex.ArrayLength = (uint)ArrayLevel;
                tex.MipCount = MipCount;
                tex.Depth = Depth;
                tex.Dim = Syroot.NintenTools.NSW.Bntx.GFX.Dim.Dim2D;
                tex.TileMode = Syroot.NintenTools.NSW.Bntx.GFX.TileMode.Default;
                tex.textureLayout2 = 0x010007;
                tex.SurfaceDim = Syroot.NintenTools.NSW.Bntx.GFX.SurfaceDim.Dim2D;
                tex.SampleCount = 1;
                tex.Pitch = 32;

                tex.MipOffsets = new long[tex.MipCount];

                var mipmaps = TextureImporterSettings.SwizzleSurfaceMipMaps(tex,
                    GenerateMipsAndCompress(bitmap, MipCount, Format), MipCount);

                ImageData = Utils.CombineByteArray(mipmaps.ToArray());
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, (int)Target);
            }
        }
    }
}
