using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Switch_Toolbox.Library.Forms;
using Bfres.Structs;
using System.IO;

namespace FirstPlugin
{
    public enum BlockType : uint
    {
        Invalid = 0x00,
        EndOfFile = 0x01,
        AlignData = 0x02,
        VertexShaderHeader = 0x03,
        VertexShaderProgram = 0x05,
        PixelShaderHeader = 0x06,
        PixelShaderProgram = 0x07,
        GeometryShaderHeader = 0x08,
        GeometryShaderProgram = 0x09,
        GeometryShaderProgram2 = 0x10,
        ImageInfo = 0x11,
        ImageData = 0x12,
        MipData = 0x13,
        ComputeShaderHeader = 0x14,
        ComputeShader = 0x15,
        UserBlock = 0x16,
    }

    public class GTXFile : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GTX" };
        public string[] Extension { get; set; } = new string[] { "*.gtx" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "Gfx2");
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
        private GTXHeader header;

        public List<byte[]> data = new List<byte[]>();
        public List<byte[]> mipMaps = new List<byte[]>();
        public List<TextureData> textures = new List<TextureData>();

        public List<GTXDataBlock> blocks = new List<GTXDataBlock>();

        public override void OnClick(TreeView treeview)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.Instance.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.Instance.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(header, null);
        }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            Text = FileName;

            ReadGx2(new FileReader(stream));

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.S));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
        }
        private void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "gtx";
            sfd.Filter = "Supported Formats|*.gtx;";
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

            string formats = FileFilters.GTX;

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
        public byte[] Save()
        {
            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            using (FileWriter writer = new FileWriter(mem))
            {
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                header.Write(writer);

                uint surfBlockType;
                uint dataBlockType;
                uint mipBlockType;

                if (header.MajorVersion == 6 && header.MinorVersion == 0)
                {
                    surfBlockType = 0x0A;
                    dataBlockType = 0x0B;
                    mipBlockType = 0x0C;
                }
                else if (header.MajorVersion == 6 || header.MajorVersion == 7)
                {
                    surfBlockType = 0x0B;
                    dataBlockType = 0x0C;
                    mipBlockType = 0x0D;
                }
                else
                    throw new Exception($"Unsupported GTX version {header.MajorVersion}");

                int imageInfoIndex = 0;
                int imageBlockIndex = 0;
                int imageMipBlockIndex = 0;

                writer.Seek(header.HeaderSize, System.IO.SeekOrigin.Begin);
                foreach (var block in blocks)
                {
                    if ((uint)block.BlockType == surfBlockType)
                    {
                        block.data = textures[imageInfoIndex++].surface.Write();
                        block.Write(writer);
                    }
                    else if ((uint)block.BlockType == dataBlockType)
                    {
                        block.data = textures[imageBlockIndex++].surface.data;
                        block.Write(writer);
                    }
                    else if ((uint)block.BlockType == mipBlockType)
                    {
                        block.data = textures[imageMipBlockIndex++].surface.mipData;
                        block.Write(writer);
                    }
                    else
                    {
                        block.Write(writer);
                    }
                }
            }
            return mem.ToArray();
        }
        private void ReadGx2(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            header = new GTXHeader();
            header.Read(reader);

            Console.WriteLine("header size " + header.HeaderSize);

            uint surfBlockType;
            uint dataBlockType;
            uint mipBlockType;

            if (header.MajorVersion == 6 && header.MinorVersion == 0)
            {
                surfBlockType = 0x0A;
                dataBlockType = 0x0B;
                mipBlockType = 0x0C;
            }
            else if (header.MajorVersion == 6 || header.MajorVersion == 7)
            {
                surfBlockType = 0x0B;
                dataBlockType = 0x0C;
                mipBlockType = 0x0D;
            }
            else
                throw new Exception($"Unsupported GTX version {header.MajorVersion}");

            if (header.GpuVersion != 2)
                throw new Exception($"Unsupported GPU version {header.GpuVersion}");

            reader.Position = header.HeaderSize;

            bool blockB = false;
            bool blockC = false;

            uint ImageInfo = 0;
            uint images = 0;

            while (reader.Position < reader.BaseStream.Length)
            {
                Console.WriteLine("BLOCK POS " + reader.Position + " " + reader.BaseStream.Length);
                GTXDataBlock block = new GTXDataBlock();
                block.Read(reader);
                blocks.Add(block);

                //Here we use "if" instead of "case" statements as types vary between versions
                if ((uint)block.BlockType == surfBlockType)
                {
                    ImageInfo += 1;
                    blockB = true;

                    var surface = new SurfaceInfoParse();
                    surface.Read(new FileReader(block.data));

                    if (surface.tileMode == 0 || surface.tileMode > 16)
                        throw new Exception($"Invalid tileMode {surface.tileMode}!");

                    if (surface.numMips > 14)
                        throw new Exception($"Invalid number of mip maps {surface.numMips}!");

                    TextureData textureData = new TextureData();
                    textureData.surface = surface;
                    textureData.MipCount = surface.numMips;
                    textureData.ArrayCount = surface.numArray;
                    textureData.Text = "Texture" + ImageInfo;
                    Nodes.Add(textureData);
                    textures.Add(textureData);
                }
                else if ((uint)block.BlockType == dataBlockType)
                {
                    images += 1;
                    blockC = true;

                    data.Add(block.data);
                }
                else if ((uint)block.BlockType == mipBlockType)
                {
                    mipMaps.Add(block.data);
                }
            }
            if (textures.Count != data.Count)
                throw new Exception($"Bad size! {textures.Count} {data.Count}");

            int curTex = 0;
            int curMip = 0;
            foreach (var node in Nodes)
            {
                TextureData tex = (TextureData)node;

                tex.surface.data = data[curTex];
                tex.surface.bpp = GX2.surfaceGetBitsPerPixel(tex.surface.format) >> 3;
                tex.Format = FTEX.ConvertFromGx2Format((Syroot.NintenTools.Bfres.GX2.GX2SurfaceFormat)tex.surface.format);
                tex.Width = tex.surface.width;
                tex.Height = tex.surface.height;

                if (tex.surface.numMips > 1)
                    tex.surface.mipData = mipMaps[curMip++];
                else
                    tex.surface.mipData = new byte[0];

                if (tex.surface.mipData == null)
                    tex.surface.numMips = 1;

                curTex++;
            }
            reader.Close();
            reader.Dispose();
        }
        public class GTXHeader
        {
            readonly string Magic = "Gfx2";
            public uint HeaderSize;
            public uint MajorVersion;
            public uint MinorVersion;
            public uint GpuVersion;
            public uint AlignMode;

            public void Read(FileReader reader)
            {
                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != Magic)
                    throw new Exception($"Invalid signature {Signature}! Expected Gfx2.");

                HeaderSize = reader.ReadUInt32();
                MajorVersion = reader.ReadUInt32();
                MinorVersion = reader.ReadUInt32();
                GpuVersion = reader.ReadUInt32(); //Ignored in 6.0
                AlignMode = reader.ReadUInt32();
            }
            public void Write(FileWriter writer)
            {
                writer.WriteSignature(Magic);
                writer.Write(HeaderSize);
                writer.Write(MajorVersion);
                writer.Write(MinorVersion);
                writer.Write(GpuVersion);
                writer.Write(AlignMode);
            }
        }
        public class GTXDataBlock
        {
            readonly string Magic = "BLK{";
            public uint HeaderSize;
            public uint MajorVersion;
            public uint MinorVersion;
            public BlockType BlockType;
            public uint Identifier;
            public uint index;
            public uint DataSize;
            public byte[] data;

            public void Read(FileReader reader)
            {
                long blockStart = reader.Position;

                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != Magic)
                    throw new Exception($"Invalid signature {Signature}! Expected BLK.");

                HeaderSize = reader.ReadUInt32();
                MajorVersion = reader.ReadUInt32(); //Must be 0x01 for 6.x
                MinorVersion = reader.ReadUInt32(); //Must be 0x00 for 6.x
                BlockType = reader.ReadEnum<BlockType>(false);
                DataSize = reader.ReadUInt32();
                Identifier = reader.ReadUInt32();
                index = reader.ReadUInt32();

                reader.Seek(blockStart + HeaderSize, System.IO.SeekOrigin.Begin);
                data = reader.ReadBytes((int)DataSize);
            }
            public void Write(FileWriter writer)
            {
                long blockStart = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(HeaderSize);
                writer.Write(MajorVersion);
                writer.Write(MinorVersion);
                writer.Write(BlockType, false);
                writer.Write(data.Length);
                writer.Write(Identifier);
                writer.Write(index);
                writer.Seek(blockStart + HeaderSize, System.IO.SeekOrigin.Begin);

                writer.Write(data);
            }
        }
        public class TextureData : STGenericTexture
        {
            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
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
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.B8G8R8A8_UNORM_SRGB,
                        TEX_FORMAT.B8G8R8A8_UNORM,
                        TEX_FORMAT.R10G10B10A2_UNORM,
                        TEX_FORMAT.R16_UNORM,
                        TEX_FORMAT.B4G4R4A4_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                        TEX_FORMAT.R32G8X24_FLOAT,
                    };
                }
            }

            public override bool CanEdit { get; set; } = true;

            public SurfaceInfoParse surface;

            public TextureData()
            {
                ImageKey = "Texture";
                SelectedImageKey = "Texture";

                LoadContextMenus();

                CanDelete = false;
                CanRename = false;
            }

            public override string ExportFilter => FileFilters.GTX;
            public override string ReplaceFilter => FileFilters.GTX;

            private void ApplySurface(GX2.GX2Surface NewSurface)
            {
                surface.aa = NewSurface.aa;
                surface.alignment = NewSurface.alignment;
                surface.bpp = NewSurface.bpp;
                surface.compSel = NewSurface.compSel;
                surface.data = NewSurface.data;
                surface.depth = NewSurface.depth;
                surface.dim = NewSurface.dim;
                surface.firstMip = NewSurface.firstMip;
                surface.firstSlice = NewSurface.firstSlice;
                surface.format = NewSurface.format;
                surface.height = NewSurface.height;
                surface.imageCount = NewSurface.imageCount;
                surface.imageSize = NewSurface.imageSize;
                surface.mipData = NewSurface.mipData;
                surface.mipOffset = NewSurface.mipOffset;
                surface.numArray = NewSurface.numArray;
                surface.numMips = NewSurface.numMips;
                surface.pitch = NewSurface.pitch;
                surface.resourceFlags = NewSurface.resourceFlags;
                surface.swizzle = NewSurface.swizzle;
                surface.texRegs = NewSurface.texRegs;
                surface.tileMode = NewSurface.tileMode;
                surface.use = NewSurface.use;
                surface.width = NewSurface.width;

                SetChannelComponents();
            }

            private STChannelType SetChannel(byte compSel)
            {
                if (compSel == 0) return STChannelType.Red;
                else if (compSel == 1) return STChannelType.Green;
                else if (compSel == 2) return STChannelType.Blue;
                else if (compSel == 3) return STChannelType.Alpha;
                else if (compSel == 4) return STChannelType.Zero;
                else return STChannelType.One;
            }

            private void SetChannelComponents()
            {
                surface.compSel = new byte[4] { 0, 1, 2, 3 };
            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                if (bitmap == null)
                    return; //Image is likely disposed and not needed to be applied

                RedChannel = SetChannel(surface.compSel[0]);
                GreenChannel = SetChannel(surface.compSel[1]);
                BlueChannel = SetChannel(surface.compSel[2]);
                AlphaChannel = SetChannel(surface.compSel[3]);

                surface.format = (uint)FTEX.ConvertToGx2Format(Format);
                surface.width = (uint)bitmap.Width;
                surface.height = (uint)bitmap.Height;

                if (MipCount != 1)
                {
                    MipCount = GenerateMipCount(bitmap.Width, bitmap.Height);
                    if (MipCount == 0)
                        MipCount = 1;
                }

                surface.numMips = MipCount;
                surface.mipOffset = new uint[MipCount];

                try
                {
                    //Create image block from bitmap first
                    var data = GenerateMipsAndCompress(bitmap, MipCount, Format);

                    //Swizzle and create surface
                    var NewSurface = GX2.CreateGx2Texture(data, Text,
                        (uint)surface.tileMode,
                        (uint)surface.aa,
                        (uint)surface.width,
                        (uint)surface.height,
                        (uint)surface.depth,
                        (uint)surface.format,
                        (uint)surface.swizzle,
                        (uint)surface.dim,
                        (uint)surface.numMips
                        );

                    ApplySurface(NewSurface);
                    IsEdited = true;
                    LoadOpenGLTexture();
                    LibraryGUI.Instance.UpdateViewport();
                }
                catch (Exception ex)
                {
                    STErrorDialog.Show("Failed to swizzle and compress image " + Text, "Error", ex.ToString());
                }
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                Console.WriteLine("");
                Console.WriteLine("// ----- GX2Surface Info ----- ");
                Console.WriteLine("  dim             = " + surface.dim);
                Console.WriteLine("  width           = " + surface.width);
                Console.WriteLine("  height          = " + surface.height);
                Console.WriteLine("  depth           = " + surface.depth);
                Console.WriteLine("  numMips         = " + surface.numMips);
                Console.WriteLine("  format          = " + surface.format);
                Console.WriteLine("  aa              = " + surface.aa);
                Console.WriteLine("  use             = " + surface.use);
                Console.WriteLine("  imageSize       = " + surface.imageSize);
                Console.WriteLine("  mipSize         = " + surface.mipSize);
                Console.WriteLine("  tileMode        = " + surface.tileMode);
                Console.WriteLine("  swizzle         = " + surface.swizzle);
                Console.WriteLine("  alignment       = " + surface.alignment);
                Console.WriteLine("  pitch           = " + surface.pitch);
                Console.WriteLine("  bits per pixel  = " + (surface.bpp << 3));
                Console.WriteLine("  bytes per pixel = " + surface.bpp);
                Console.WriteLine("  data size       = " + surface.data.Length);
                Console.WriteLine("  mip size        = " + surface.mipData.Length);
                Console.WriteLine("  realSize        = " + surface.imageSize);

                var surfaces = GX2.Decode(surface);

                return surfaces[ArrayLevel][MipLevel];
            }
            private void Remove(object sender, EventArgs args) {
                ((GTXFile)Parent).Nodes.Remove(this);
            }

            public override void Export(string FileName) {
                Export(FileName);
            }

            public override void Replace(string FileName)
            {
                FTEX ftex = new FTEX();
                ftex.ReplaceTexture(FileName, Format, 1, SupportedFormats, true, true);
                if (ftex.texture != null)
                {
                    surface.swizzle = ftex.texture.Swizzle;
                    surface.format = (uint)ftex.texture.Format;
                    surface.aa = (uint)ftex.texture.AAMode;
                    surface.alignment = (uint)ftex.texture.Alignment;
                    surface.dim = (uint)ftex.texture.Dim;
                    surface.width = (uint)ftex.texture.Width;
                    surface.height = (uint)ftex.texture.Height;
                    surface.depth = (uint)ftex.texture.Depth;
                    surface.numMips = (uint)ftex.texture.MipCount;
                    surface.imageSize = (uint)ftex.texture.Data.Length;
                    surface.mipSize = (uint)ftex.texture.MipData.Length;
                    surface.data = ftex.texture.Data;
                    surface.mipData = ftex.texture.MipData;
                    surface.mipOffset = ftex.texture.MipOffsets;
                    surface.firstSlice = ftex.texture.ViewSliceFirst;
                    surface.numSlices = ftex.texture.ViewSliceCount;
                    surface.imageCount = ftex.texture.ArrayLength;
                    surface.pitch = ftex.texture.Pitch;
                    SetChannelComponents();

                    Format = FTEX.ConvertFromGx2Format((Syroot.NintenTools.Bfres.GX2.GX2SurfaceFormat)surface.format);
                    Width = surface.width;
                    Height = surface.height;

                    ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));

                    if (editor != null)
                        UpdateEditor();
                }
            }
            public override void OnClick(TreeView treeView)
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
                var tex = FTEX.FromGx2Surface(surface, Text);
                tex.MipCount = MipCount;
                editor.LoadProperties(tex);
                editor.LoadImage(this);
            }
        }
        public class SurfaceInfoParse : GX2.GX2Surface
        {
            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                dim = reader.ReadUInt32();
                width = reader.ReadUInt32();
                height = reader.ReadUInt32();
                depth = reader.ReadUInt32();
                numMips = reader.ReadUInt32();
                format = reader.ReadUInt32();
                aa = reader.ReadUInt32();
                use = reader.ReadUInt32();
                imageSize = reader.ReadUInt32();
                imagePtr = reader.ReadUInt32();
                mipSize = reader.ReadUInt32();
                mipPtr = reader.ReadUInt32();
                tileMode = reader.ReadUInt32();
                swizzle = reader.ReadUInt32();
                alignment = reader.ReadUInt32();
                pitch = reader.ReadUInt32();
                mipOffset = reader.ReadUInt32s(13);
                firstMip = reader.ReadUInt32();
                imageCount = reader.ReadUInt32();
                firstSlice = reader.ReadUInt32();
                numSlices = reader.ReadUInt32();
                compSel = reader.ReadBytes(4);
                texRegs = reader.ReadUInt32s(5);
            }

            public byte[] Write()
            {
                MemoryStream mem = new MemoryStream();

                FileWriter writer = new FileWriter(mem);
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                writer.Write(dim);
                writer.Write(width);
                writer.Write(height);
                writer.Write(depth);
                writer.Write(numMips);
                writer.Write(format);
                writer.Write(aa);
                writer.Write(use);
                writer.Write(imageSize);
                writer.Write(imagePtr);
                writer.Write(mipSize);
                writer.Write(mipPtr);
                writer.Write(tileMode);
                writer.Write(swizzle);
                writer.Write(alignment);
                writer.Write(pitch);

                for (int i = 0; i < 13; i++)
                {
                    if (mipOffset.Length > i)
                        writer.Write(mipOffset[i]);
                    else
                        writer.Write(0);
                }

                writer.Write(firstMip);
                writer.Write(imageCount);
                writer.Write(firstSlice);
                writer.Write(numSlices);

                for (int i = 0; i < 4; i++)
                {
                    if (compSel != null && compSel.Length > i)
                        writer.Write(compSel[i]);
                    else
                        writer.Write((byte)0);
                }

                for (int i = 0; i < 5; i++)
                {
                    if (texRegs != null && texRegs.Length > i)
                        writer.Write(texRegs[i]);
                    else
                        writer.Write(0);
                }

                return mem.ToArray();
            }
        }
    }


}
