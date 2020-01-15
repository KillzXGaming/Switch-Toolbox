using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Runtime.InteropServices;

namespace FirstPlugin
{
    public class CTPK : TreeNodeFile, IFileFormat, ITextureContainer
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CTR Texture Package" };
        public string[] Extension { get; set; } = new string[] { "*.ctpk" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "CTPK");
            }
        }

        public bool DisplayIcons => true;

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

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            header = new Header();
            header.Read(new FileReader(stream));

            for (int i = 0; i < header.Textures.Count; i++)
                Nodes.Add(new CtpkTextureWrapper(header.Textures[i]));
        }
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public class CtpkTextureWrapper : STGenericTexture
        {
            public TextureEntry CtpkTexture;

            public CtpkTextureWrapper(TextureEntry tex) {
                ApplyCtpkTexture(tex);
            }

            private void ApplyCtpkTexture(TextureEntry tex)
            {
                CtpkTexture = tex;
                Width = tex.Width;
                Height = tex.Height;
                MipCount = tex.MipCount;
                ArrayCount = tex.FaceCount;
                Text = tex.Name;
                Format = CTR_3DS.ConvertPICAToGenericFormat(tex.PicaFormat);

                PlatformSwizzle = PlatformSwizzle.Platform_3DS;
            }

            public override bool CanEdit { get; set; } = false;

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                    TEX_FORMAT.B5G6R5_UNORM,
                    TEX_FORMAT.R8G8_UNORM,
                    TEX_FORMAT.B5G5R5A1_UNORM,
                    TEX_FORMAT.B4G4R4A4_UNORM,
                    TEX_FORMAT.LA8,
                    TEX_FORMAT.HIL08,
                    TEX_FORMAT.L8,
                    TEX_FORMAT.A8_UNORM,
                    TEX_FORMAT.LA4,
                    TEX_FORMAT.A4,
                    TEX_FORMAT.ETC1_UNORM,
                    TEX_FORMAT.ETC1_A4,
                };
                }
            }

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
            {
                return CtpkTexture.ImageData;
            }


            public override void OnClick(TreeView treeView)
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

                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }
        }

        public class Header
        {
            public const string MAGIC = "CTPK";

            public ushort Version { get; set; }

            public List<TextureEntry> Textures { get; set; }

            public List<ConversionInfo> ConversionInfos { get; set; }

            public void Read(FileReader reader)
            {
                Textures = new List<TextureEntry>();
                ConversionInfos = new List<ConversionInfo>();

                reader.ReadSignature(4, MAGIC);
                Version = reader.ReadUInt16();
                ushort numTextures = reader.ReadUInt16();
                uint textureDataOffset = reader.ReadUInt32();
                uint hashArrayOffset = reader.ReadUInt32();
                uint conversionInfoOffset = reader.ReadUInt32();

                reader.Position = 0x20;
                for (int i = 0; i < numTextures; i++)
                {
                    TextureEntry entry = new TextureEntry();
                    entry.Read(reader);
                    Textures.Add(entry);
                }

                reader.SeekBegin(conversionInfoOffset);
                for (int i = 0; i < numTextures; i++)
                    ConversionInfos.Add(reader.ReadStruct<ConversionInfo>());

                for (int i = 0; i < numTextures; i++)
                {
                    reader.SeekBegin(textureDataOffset + Textures[i].DataOffset);
                    Textures[i].ImageData = reader.ReadBytes((int)Textures[i].ImageSize);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.Write(MAGIC);
            }
        }

        public class TextureEntry
        {
            public CTR_3DS.PICASurfaceFormat PicaFormat { get; set; }

            public string Name { get; set; }
            public uint ImageSize { get; set; }
            public uint TextureFormat { get; set; }
            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public byte MipCount { get; set; }
            public byte Type { get; set; }
            public ushort FaceCount { get; set; }
            public uint UnixTimeStamp { get; set; }

            internal uint DataOffset { get; set; }
            internal uint BitmapSizeOffset { get; set; }

            public byte[] ImageData { get; set; }

            public void Read(FileReader reader)
            {
                Name = reader.LoadString(false, typeof(uint));
                ImageSize = reader.ReadUInt32();
                DataOffset = reader.ReadUInt32();
                TextureFormat = reader.ReadUInt32();
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                MipCount = reader.ReadByte();
                Type = reader.ReadByte();
                FaceCount = reader.ReadUInt16();
                BitmapSizeOffset = reader.ReadUInt32();
                UnixTimeStamp = reader.ReadUInt32();

                PicaFormat = (CTR_3DS.PICASurfaceFormat)TextureFormat;
            }

            public void Write(FileWriter writer)
            {
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class ConversionInfo
        {
            public byte TextureFormat { get; set; }
            public byte Unknown { get; set; }
            public bool Compressed { get; set; }
            public byte Etc1Quality { get; set; }
        }
    }
}
