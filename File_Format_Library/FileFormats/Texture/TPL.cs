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
using System.ComponentModel;

namespace FirstPlugin
{
    public class TPL : TreeNodeFile, IFileFormat,  ITextureContainer
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
                return reader.ReadUInt32() == 0x0020AF30 || Utils.GetExtension(FileName) == ".tpl";
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

        public List<ImageHeader> ImageHeaders = new List<ImageHeader>();
        public List<PaletteHeader> PaletteHeaders = new List<PaletteHeader>();

        public override void OnAfterAdded()
        {
            if (Nodes.Count > 0 && this.TreeView != null)
               this.TreeView.SelectedNode = Nodes[0];
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                uint Identifier = reader.ReadUInt32();

                //TPL has multiple versions i assume? Some games like F Zero use a custom one so try that
                if (Identifier != 0x0020AF30)
                {
                    reader.Position = 0;
                    uint ImageCount = reader.ReadUInt32();
                    for (int i = 0; i < ImageCount; i++)
                    {
                        reader.SeekBegin(4 + (i * 0x10));

                        var image = new ImageHeader();

                        image.Format = (Decode_Gamecube.TextureFormats)reader.ReadUInt32();
                        image.ImageOffset = reader.ReadUInt32();
                        image.Width = reader.ReadUInt16();
                        image.Height = reader.ReadUInt16();
                        image.MaxLOD = (byte)reader.ReadUInt16();
                        ushort unknown = reader.ReadUInt16();

                        if ((uint)image.Format == 256)
                            break;

                        Console.WriteLine(image.ImageOffset);
                        Console.WriteLine(image.Format);

                        //Now create a wrapper
                        var texWrapper = new TplTextureWrapper(this, image);
                        string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                        if (ImageCount == 1)
                            texWrapper.Text = name;
                        else
                            texWrapper.Text = $"{name}_{i}";
                        texWrapper.ImageKey = "Texture";
                        texWrapper.SelectedImageKey = "Texture";
                        texWrapper.Format = Decode_Gamecube.ToGenericFormat(image.Format);
                        texWrapper.Width = image.Width;
                        texWrapper.Height = image.Height;
                        texWrapper.MipCount = image.MaxLOD;
                        texWrapper.ImageData = reader.getSection(image.ImageOffset, (uint)Decode_Gamecube.GetDataSize(image.Format, (int)image.Width, (int)image.Height));
                        texWrapper.PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;
                        Nodes.Add(texWrapper);
                    }
                }
                else
                {
                    uint ImageCount = reader.ReadUInt32();
                    uint ImageOffsetTable = reader.ReadUInt32();

                    for (int i = 0; i < ImageCount; i++)
                    {
                        reader.SeekBegin(ImageOffsetTable + (i * 8));

                        uint ImageHeaderOffset = reader.ReadUInt32();
                        uint PaletteHeaderOffset = reader.ReadUInt32();

                        reader.SeekBegin(ImageHeaderOffset);
                        var image = new ImageHeader();
                        image.Read(reader);
                        ImageHeaders.Add(image);

                        Console.WriteLine($"ImageOffset {image.ImageOffset}");

                        //Now create a wrapper
                        var texWrapper = new TplTextureWrapper(this, image);
                        string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                        if (ImageCount == 1)
                            texWrapper.Text = name;
                        else
                            texWrapper.Text = $"{name}_{i}";

                        texWrapper.ImageKey = "Texture";
                        texWrapper.SelectedImageKey = "Texture";
                        texWrapper.Format = Decode_Gamecube.ToGenericFormat(image.Format);
                        texWrapper.Width = image.Width;
                        texWrapper.Height = image.Height;
                        texWrapper.MipCount = 1;
                        texWrapper.PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;
                        texWrapper.ImageData = reader.getSection(image.ImageOffset, 
                            (uint)Decode_Gamecube.GetDataSize(image.Format, (int)image.Width, (int)image.Height));

                        Console.WriteLine($"PaletteHeaderOffset {PaletteHeaderOffset}");
                        Console.WriteLine($"ImageData  {  texWrapper.ImageData.Length}");

                        //Palette is sometimes unused to check
                        if (PaletteHeaderOffset != 0)
                        {
                            reader.SeekBegin(PaletteHeaderOffset);
                            var palette = new PaletteHeader();
                            palette.Read(reader);
                            PaletteHeaders.Add(palette);

                            var GXPaletteFormat = (Decode_Gamecube.PaletteFormats)palette.PaletteFormat;
                            image.PaletteFormat = GXPaletteFormat;

                            Console.WriteLine($"GXPaletteFormat {GXPaletteFormat}");
                            texWrapper.SetPaletteData(palette.Data, Decode_Gamecube.ToGenericPaletteFormat(GXPaletteFormat));
                        }

                        Nodes.Add(texWrapper);
                    }
                }
            }
        }

        public class TplTextureWrapper : STGenericTexture
        {
            public TPL TPLParent { get; set; }

            public byte[] ImageData { get; set; }
            public ImageHeader ImageHeader;

            public TplTextureWrapper(TPL tpl, ImageHeader header) {
                TPLParent = tpl;
                ImageHeader = header;
            }

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                    TEX_FORMAT.I4,
                    TEX_FORMAT.I8,
                    TEX_FORMAT.IA4,
                    TEX_FORMAT.IA8,
                    TEX_FORMAT.RGB565,
                    TEX_FORMAT.RGB5A3,
                    TEX_FORMAT.RGBA32,
                    TEX_FORMAT.C4,
                    TEX_FORMAT.C8,
                    TEX_FORMAT.C14X2,
                    TEX_FORMAT.CMPR,
                    };
                }
            }


            public override bool CanEdit { get; set; } = false;

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
            {
                return Decode_Gamecube.GetMipLevel(ImageData, Width, Height, MipCount, (uint)MipLevel, Format);
            }

            public override void OnClick(TreeView treeView)
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                editor.LoadProperties(ImageHeader);
                editor.LoadImage(this);
            }
        }

        public class PaletteHeader
        {
            public ushort EntryCount { get; set; }
            public byte Unpacked { get; set; }
            public uint PaletteFormat { get; set; }
            public uint PaletteDataOffset { get; set; }

            public byte[] Data;

            public void Read(FileReader reader)
            {
                EntryCount = reader.ReadUInt16();
                Unpacked = reader.ReadByte();
                reader.ReadByte();
                PaletteFormat = reader.ReadUInt32();
                PaletteDataOffset = reader.ReadUInt32();

                using (reader.TemporarySeek(PaletteDataOffset, SeekOrigin.Begin))
                {
                    Data = reader.ReadBytes(EntryCount * 2);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.Write(EntryCount);
                writer.Write(Unpacked);
                writer.Write((byte)0);
                writer.Write(PaletteFormat);
                writer.Write(PaletteDataOffset);
            }
        }

        public class ImageHeader
        {
            [ReadOnly(true)]
            public Decode_Gamecube.TextureFormats Format { get; set; }
            [ReadOnly(true)]
            public Decode_Gamecube.PaletteFormats PaletteFormat { get; set; }

            [ReadOnly(true)]
            public ushort Width { get; set; }
            [ReadOnly(true)]
            public ushort Height { get; set; }
            [Browsable(false)]
            public uint ImageOffset { get; set; }

            public float LODBias { get; set; }
            public bool EdgeLODEnable { get; set; }

            public WrapMode WrapS { get; set; }
            public WrapMode WrapT { get; set; }
            public FilterMode MinFilter { get; set; }
            public FilterMode MagFilter { get; set; }

            [Browsable(false)]
            public byte MinLOD { get; set; }
            [Browsable(false)]
            public byte MaxLOD { get; set; }
            [Browsable(false)]
            public byte Unpacked { get; set; }

            public void Read(FileReader reader)
            {
                Height = reader.ReadUInt16();
                Width = reader.ReadUInt16();
                Format = (Decode_Gamecube.TextureFormats)reader.ReadUInt32();
                ImageOffset = reader.ReadUInt32();
                WrapS = (WrapMode)reader.ReadUInt32();
                WrapT = (WrapMode)reader.ReadUInt32();
                MinFilter = (FilterMode)reader.ReadUInt32();
                MagFilter = (FilterMode)reader.ReadUInt32();
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
                writer.Write((uint)Format);
                writer.Write(ImageOffset);
                writer.Write((uint)WrapS);
                writer.Write((uint)WrapT);
                writer.Write((uint)MinFilter);
                writer.Write((uint)MagFilter);
                writer.Write(LODBias);
                writer.Write(EdgeLODEnable);
                writer.Write(MinLOD);
                writer.Write(MaxLOD);
                writer.Write(Unpacked);
            }
        }

        public enum WrapMode : uint
        {
            Clamp,
            Repeat,
            Mirror
        }

        public enum FilterMode : uint
        {
            Nearest,
            Linear,
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
