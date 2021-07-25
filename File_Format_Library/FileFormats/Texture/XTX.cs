using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Linq;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Syroot.NintenTools.NSW.Bntx.GFX;

namespace FirstPlugin
{
    public class XTX : TreeNodeFile, IFileFormat, IContextMenuNode, ITextureContainer
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

        public bool DisplayIcons => false; //Astc decoding is slow and the UI has loading issues atm

        public List<STGenericTexture> TextureList
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
            for (int i = 0; i < TextureInfos.Count; i++) {
                string name = Path.GetFileNameWithoutExtension(FileName);
                TextureInfos[i].Text = TextureInfos.Count == 1 ? name : $"{name}_image{i}";
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.S),
                new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E),
            };
        }

        private void Save(object sender, EventArgs args)
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

        protected void ExportAllAction(object sender, EventArgs e)
        {
            if (Nodes.Count <= 0)
                return;

            string formats = FileFilters.XTX;

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

            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string extension = form.GetSelectedExtension();
                    extension.Replace(" ", string.Empty);

                    foreach (STGenericTexture node in Nodes)
                    {
                        ((STGenericTexture)node).Export($"{folderPath}\\{node.Text}{extension}");
                    }
                }
            }
        }

        public void Unload()
        {

        }

        public override void OnAfterAdded()
        {
            if (Nodes.Count > 0 && this.TreeView != null)
                this.TreeView.SelectedNode = Nodes[0];
        }

        public void Save(System.IO.Stream stream)
        {
            SaveFile(new FileWriter(stream, true));
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
                BC7U = 0x0000004d,

                //Same order as this https://github.com/aboood40091/BNTX-Editor/blob/master/globals.py
                //However SRGB goes after unorm
                ASTC_4x4_UNORM = 0x00000079,
                ASTC_5x4_UNORM = 0x0000007A,
                ASTC_5x5_UNORM = 0x0000007B,
                ASTC_6x5_UNORM = 0x0000007C,
                ASTC_6x6_UNORM = 0x0000007D,
                ASTC_8x5_UNORM = 0x0000007E,
                ASTC_8x6_UNORM = 0x0000007F,
                ASTC_8x8_UNORM = 0x00000080,
                ASTC_10x5_UNORM = 0x00000081,
                ASTC_10x6_UNORM = 0x00000082,
                ASTC_10x8_UNORM = 0x00000083,
                ASTC_10x10_UNORM = 0x00000084,
                ASTC_12x10_UNORM = 0x00000085,
                ASTC_12x12_UNORM = 0x00000086,
                ASTC_4x4_SRGB = 0x00000087,
                ASTC_5x4_SRGB = 0x00000088,
                ASTC_5x5_SRGB = 0x00000089,
                ASTC_6x5_SRGB = 0x0000008A,
                ASTC_6x6_SRGB = 0x0000008B,
                ASTC_8x5_SRGB = 0x0000008C,
                ASTC_8x6_SRGB = 0x0000008D,
                ASTC_8x8_SRGB = 0x0000008E,
                ASTC_10x5_SRGB = 0x0000008F,
                ASTC_10x6_SRGB = 0x00000090,
                ASTC_10x8_SRGB = 0x00000091,
                ASTC_10x10_SRGB = 0x00000092,
                ASTC_12x10_SRGB = 0x00000093,
                ASTC_12x12_SRGB = 0x00000094,
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
                    block.Data = TextureInfos[curTexImage].ImageData;
                    block.WriteHeader(writer);
                    block.WriteBlock(writer, 512);
                    curTexImage++;
                }
                else if (block.BlockType == TextureInfoType)
                {
                    block.Data = TextureInfos[curTexImage].Write();
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

            public uint TextureLayout1;
            public uint TextureLayout2;

            public uint BlockHeightLog2;

            public uint Boolean;

            public override string ExportFilter => FileFilters.XTX;
            public override string ReplaceFilter => FileFilters.XTX;

            public TextureInfo()
            {
                CanExport = true;
                CanReplace = true;
            }

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
                MipOffsets = reader.ReadUInt32s(17);
                TextureLayout1 = reader.ReadUInt32();
                TextureLayout2 = reader.ReadUInt32();
                Boolean = reader.ReadUInt32();

                BlockHeightLog2 = TextureLayout1 & 7;
                Format = ConvertFormat(XTXFormat);
                ArrayCount = 1;
            }

            public byte[] Write()
            {
                TextureLayout1 = (uint)BlockHeightLog2;

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
                writer.Write(TextureLayout1);
                writer.Write(TextureLayout2);
                writer.Write(Boolean);

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
                var tex = new TextureData();
                tex.Replace(FileName, MipCount, 0, Format);

                //If it's null, the operation is cancelled
                if (tex.Texture == null)
                    return;

                var surfacesNew = tex.GetSurfaces();
                var surfaces = GetSurfaces();

                ImageData = tex.Texture.TextureData[0][0];

                Width = tex.Texture.Width;
                Height = tex.Texture.Height;
                MipCount = tex.Texture.MipCount;
                TextureLayout1 = tex.Texture.textureLayout;
                TextureLayout2 = tex.Texture.textureLayout2;
                BlockHeightLog2 = tex.Texture.BlockHeightLog2;

                Format = tex.Format;
                XTXFormat = ConvertFromGenericFormat(tex.Format);

                uint[] mips = TegraX1Swizzle.GenerateMipSizes(tex.Format, tex.Width, tex.Height, tex.Depth, tex.ArrayCount, tex.MipCount, (uint)ImageData.Length)[0];
                MipOffsets = new uint[17];

                for (int i = 0; i < mips.Length; i++)
                    MipOffsets[i] = mips[i];

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

            public static XTXFormats.XTXImageFormat ConvertFromGenericFormat(TEX_FORMAT Format)
            {
                switch (Format)
                {
                    case TEX_FORMAT.BC1_UNORM: return XTXFormats.XTXImageFormat.DXT1;
                    case TEX_FORMAT.BC2_UNORM: return XTXFormats.XTXImageFormat.DXT3;
                    case TEX_FORMAT.BC3_UNORM: return XTXFormats.XTXImageFormat.DXT5;
                    case TEX_FORMAT.BC4_UNORM: return XTXFormats.XTXImageFormat.BC4U;
                    case TEX_FORMAT.BC4_SNORM: return XTXFormats.XTXImageFormat.BC4S;
                    case TEX_FORMAT.BC5_UNORM: return XTXFormats.XTXImageFormat.BC5U;
                    case TEX_FORMAT.BC7_UNORM: return XTXFormats.XTXImageFormat.BC7U;
                    case TEX_FORMAT.R8_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_R8;
                    case TEX_FORMAT.R8G8_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RG8;
                    case TEX_FORMAT.R10G10B10A2_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGB10A2;
                    case TEX_FORMAT.B5G6R5_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGB565;
                    case TEX_FORMAT.B5G5R5A1_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGB5A1;
                    case TEX_FORMAT.B4G4R4A4_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA4;
                    case TEX_FORMAT.R8G8B8A8_UNORM: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8;
                    case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8_SRGB;
                    case TEX_FORMAT.ASTC_4x4_UNORM: return XTXFormats.XTXImageFormat.ASTC_4x4_UNORM;
                    case TEX_FORMAT.ASTC_5x4_UNORM: return XTXFormats.XTXImageFormat.ASTC_5x4_UNORM;
                    case TEX_FORMAT.ASTC_5x5_UNORM: return XTXFormats.XTXImageFormat.ASTC_5x5_UNORM;
                    case TEX_FORMAT.ASTC_6x5_UNORM: return XTXFormats.XTXImageFormat.ASTC_6x5_UNORM;
                    case TEX_FORMAT.ASTC_6x6_UNORM: return XTXFormats.XTXImageFormat.ASTC_6x6_UNORM;
                    case TEX_FORMAT.ASTC_8x5_UNORM: return XTXFormats.XTXImageFormat.ASTC_8x5_UNORM;
                    case TEX_FORMAT.ASTC_8x6_UNORM: return XTXFormats.XTXImageFormat.ASTC_8x6_UNORM;
                    case TEX_FORMAT.ASTC_8x8_UNORM: return XTXFormats.XTXImageFormat.ASTC_8x8_UNORM;
                    case TEX_FORMAT.ASTC_10x5_UNORM: return XTXFormats.XTXImageFormat.ASTC_10x5_UNORM;
                    case TEX_FORMAT.ASTC_10x6_UNORM: return XTXFormats.XTXImageFormat.ASTC_10x6_UNORM;
                    case TEX_FORMAT.ASTC_10x10_UNORM: return XTXFormats.XTXImageFormat.ASTC_10x10_UNORM;
                    case TEX_FORMAT.ASTC_12x10_UNORM: return XTXFormats.XTXImageFormat.ASTC_12x10_UNORM;
                    case TEX_FORMAT.ASTC_12x12_UNORM: return XTXFormats.XTXImageFormat.ASTC_12x12_UNORM;
                    case TEX_FORMAT.ASTC_4x4_SRGB: return XTXFormats.XTXImageFormat.ASTC_4x4_SRGB;
                    case TEX_FORMAT.ASTC_5x4_SRGB: return XTXFormats.XTXImageFormat.ASTC_5x4_SRGB;
                    case TEX_FORMAT.ASTC_5x5_SRGB: return XTXFormats.XTXImageFormat.ASTC_5x5_SRGB;
                    case TEX_FORMAT.ASTC_6x5_SRGB: return XTXFormats.XTXImageFormat.ASTC_6x5_SRGB;
                    case TEX_FORMAT.ASTC_6x6_SRGB: return XTXFormats.XTXImageFormat.ASTC_6x6_SRGB;
                    case TEX_FORMAT.ASTC_8x5_SRGB: return XTXFormats.XTXImageFormat.ASTC_8x5_SRGB;
                    case TEX_FORMAT.ASTC_8x6_SRGB: return XTXFormats.XTXImageFormat.ASTC_8x6_SRGB;
                    case TEX_FORMAT.ASTC_8x8_SRGB: return XTXFormats.XTXImageFormat.ASTC_8x8_SRGB;
                    case TEX_FORMAT.ASTC_10x5_SRGB: return XTXFormats.XTXImageFormat.ASTC_10x5_SRGB;
                    case TEX_FORMAT.ASTC_10x6_SRGB: return XTXFormats.XTXImageFormat.ASTC_10x6_SRGB;
                    case TEX_FORMAT.ASTC_10x10_SRGB: return XTXFormats.XTXImageFormat.ASTC_10x10_SRGB;
                    case TEX_FORMAT.ASTC_12x10_SRGB: return XTXFormats.XTXImageFormat.ASTC_12x10_SRGB;
                    case TEX_FORMAT.ASTC_12x12_SRGB: return XTXFormats.XTXImageFormat.ASTC_12x12_SRGB;
                    default:
                        throw new Exception($"Cannot convert format {Format}");
                }
            }

            public static TEX_FORMAT ConvertFormat(uint Format)
            {
                return ConvertFormat((XTXFormats.XTXImageFormat)Format);
            }

            public static TEX_FORMAT ConvertFormat(XTXFormats.XTXImageFormat Format)
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
                    case XTXFormats.XTXImageFormat.BC7U: return TEX_FORMAT.BC7_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_R8: return TEX_FORMAT.R8_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RG8: return TEX_FORMAT.R8G8_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGB10A2: return TEX_FORMAT.R10G10B10A2_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGB565: return TEX_FORMAT.B5G6R5_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGB5A1: return TEX_FORMAT.B5G5R5A1_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA4: return TEX_FORMAT.B4G4R4A4_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8: return TEX_FORMAT.R8G8B8A8_UNORM;
                    case XTXFormats.XTXImageFormat.NVN_FORMAT_RGBA8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_4x4_UNORM: return TEX_FORMAT.ASTC_4x4_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_5x4_UNORM: return TEX_FORMAT.ASTC_5x4_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_5x5_UNORM: return TEX_FORMAT.ASTC_5x5_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_6x5_UNORM: return TEX_FORMAT.ASTC_6x5_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_6x6_UNORM: return TEX_FORMAT.ASTC_6x6_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_8x5_UNORM: return TEX_FORMAT.ASTC_8x5_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_8x6_UNORM: return TEX_FORMAT.ASTC_8x6_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_8x8_UNORM: return TEX_FORMAT.ASTC_8x8_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_10x5_UNORM: return TEX_FORMAT.ASTC_10x5_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_10x6_UNORM: return TEX_FORMAT.ASTC_10x6_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_10x10_UNORM: return TEX_FORMAT.ASTC_10x10_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_12x10_UNORM: return TEX_FORMAT.ASTC_12x10_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_12x12_UNORM: return TEX_FORMAT.ASTC_12x12_UNORM;
                    case XTXFormats.XTXImageFormat.ASTC_4x4_SRGB: return TEX_FORMAT.ASTC_4x4_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_5x4_SRGB: return TEX_FORMAT.ASTC_5x4_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_5x5_SRGB: return TEX_FORMAT.ASTC_5x5_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_6x5_SRGB: return TEX_FORMAT.ASTC_6x5_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_6x6_SRGB: return TEX_FORMAT.ASTC_6x6_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_8x5_SRGB: return TEX_FORMAT.ASTC_8x5_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_8x6_SRGB: return TEX_FORMAT.ASTC_8x6_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_8x8_SRGB: return TEX_FORMAT.ASTC_8x8_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_10x5_SRGB: return TEX_FORMAT.ASTC_10x5_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_10x6_SRGB: return TEX_FORMAT.ASTC_10x6_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_10x10_SRGB: return TEX_FORMAT.ASTC_10x10_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_12x10_SRGB: return TEX_FORMAT.ASTC_12x10_SRGB;
                    case XTXFormats.XTXImageFormat.ASTC_12x12_SRGB: return TEX_FORMAT.ASTC_12x12_SRGB;
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
                tex.AccessFlags = AccessFlags.Texture;
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

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
            {
                return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, DepthLevel, BlockHeightLog2, (int)Target);
            }
        }
    }
}
