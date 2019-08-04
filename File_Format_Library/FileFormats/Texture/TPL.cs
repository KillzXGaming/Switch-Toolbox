using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class TPL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TPL" };
        public string[] Extension { get; set; } = new string[] { "*.tpl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                return reader.ReadUInt32() == 0x0020AF30;
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

        public List<ImageHeader> ImageHeaders = new List<ImageHeader>();
        public List<PaletteHeader> PaletteHeaders = new List<PaletteHeader>();

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                uint Identifier = reader.ReadUInt32();
                uint ImageCount = reader.ReadUInt32();
                uint ImageOffsetTable = reader.ReadUInt32();

                for (int i = 0; i < ImageCount; i++)
                {
                    reader.SeekBegin(ImageOffsetTable + (i * 8));

                    uint ImageHeaderOffset = reader.ReadUInt32();
                    uint PaletteHeaderOffset = reader.ReadUInt32();

                    if (ImageHeaderOffset != 0)
                    {
                        reader.SeekBegin(ImageHeaderOffset);
                        var image = new ImageHeader();
                        image.Read(reader);
                        ImageHeaders.Add(image);
                    }

                    if (PaletteHeaderOffset != 0)
                    {
                        reader.SeekBegin(PaletteHeaderOffset);
                        var palette = new PaletteHeader();
                        palette.Read(reader);
                        PaletteHeaders.Add(palette);
                    }
                }
            }
        }

        public class PaletteHeader
        {
            public ushort EntryCount { get; set; }
            public byte Unpacked { get; set; }
            public uint PaletteFormat { get; set; }
            public uint PaletteDataOffset { get; set; }

            public void Read(FileReader reader)
            {
                EntryCount = reader.ReadUInt16();
                Unpacked = reader.ReadByte();
                PaletteFormat = reader.ReadUInt32();
                PaletteDataOffset = reader.ReadUInt32();
            }

            public void Write(FileWriter writer)
            {
                writer.Write(EntryCount);
                writer.Write(Unpacked);
                writer.Write(PaletteFormat);
                writer.Write(PaletteDataOffset);
            }
        }

        public class ImageHeader
        {
            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public uint Format { get; set; }
            public uint ImageOffset { get; set; }
            public uint WrapS { get; set; }
            public uint WrapT { get; set; }
            public uint MinFilter { get; set; }
            public uint MagFilter { get; set; }
            public float LODBias { get; set; }
            public bool EdgeLODEnable { get; set; }
            public byte MinLOD { get; set; }
            public byte MaxLOD { get; set; }
            public byte Unpacked { get; set; }

            public void Read(FileReader reader)
            {
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                Format = reader.ReadUInt32();
                ImageOffset = reader.ReadUInt32();
                WrapS = reader.ReadUInt32();
                WrapT = reader.ReadUInt32();
                MinFilter = reader.ReadUInt32();
                MagFilter = reader.ReadUInt32();
                LODBias = reader.ReadSingle();
                EdgeLODEnable = reader.ReadBoolean();
                MinLOD = reader.ReadByte();
                MaxLOD = reader.ReadByte();
                Unpacked = reader.ReadByte();
            }

            public void Write(FileWriter writer)
            {
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Format);
                writer.Write(ImageOffset);
                writer.Write(WrapS);
                writer.Write(WrapT);
                writer.Write(MinFilter);
                writer.Write(MagFilter);
                writer.Write(LODBias);
                writer.Write(EdgeLODEnable);
                writer.Write(MinLOD);
                writer.Write(MaxLOD);
                writer.Write(Unpacked);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            return mem.ToArray();
        }

        public class TPL_Texture : STGenericTexture
        {
            private libWiiSharp.TPL ParentTPL;
            private int TextureIndex { get; set; }


            private TextureProperties properties;
            public class TextureProperties 
            {
                private libWiiSharp.TPL ParentTPL;
                private int TextureIndex { get; set; }

                public libWiiSharp.TPL_TextureFormat TextureFormat
                {
                    get { return ParentTPL.GetTextureFormat(TextureIndex); }
                }

                public libWiiSharp.TPL_PaletteFormat PaletteFormat
                {
                    get { return ParentTPL.GetPaletteFormat(TextureIndex); }
                }

                public int Width
                {
                    get { return ParentTPL.GetImageWidth(TextureIndex); }
                }

                public int Height
                {
                    get { return ParentTPL.GetImageHeight(TextureIndex); }
                }

                public TextureProperties(libWiiSharp.TPL tpl, int index)
                {
                    ParentTPL = tpl;
                    TextureIndex = index;
                }
            }



            public TPL_Texture(libWiiSharp.TPL tpl, int index)
            {
                ParentTPL = tpl;
                TextureIndex = index;

                Width = tpl.GetImageWidth(index);
                Height = tpl.GetImageHeight(index);
                MipCount = 1;

                Text = $"Image {index}";

                Format = TEX_FORMAT.R8G8B8A8_UNORM;

                SelectedImageKey = "texture";
                ImageKey = "texture";

                properties = new TextureProperties(tpl, index);
            }

            public override bool CanEdit { get; set; } = false;

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                         TEX_FORMAT.R8G8B8A8_UNORM,
                };
                }
            }

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return ImageUtilty.ConvertBgraToRgba(ParentTPL.ExtractTextureByteArray(TextureIndex));
            }

            public override void OnClick(TreeView treeView)
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
                editor.LoadProperties(properties);
                editor.LoadImage(this);
            }
        }
    }
}
