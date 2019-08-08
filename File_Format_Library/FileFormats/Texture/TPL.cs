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
    public class TPL : TreeNodeFile, IFileFormat, ITextureIconLoader
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

        public List<STGenericTexture> IconTextureList
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
                    Console.WriteLine("ImageCount" + ImageCount);
                    for (int i = 0; i < ImageCount; i++)
                    {
                        reader.SeekBegin(4 + (i * 0x10));

                        uint format = reader.ReadUInt32();
                        uint offset = reader.ReadUInt32();
                        uint width = reader.ReadUInt16();
                        uint height = reader.ReadUInt16();
                        uint mipCount = reader.ReadUInt16();
                        ushort unknown = reader.ReadUInt16();

                        if (format == 256)
                            break;

                        Console.WriteLine(offset);
                        Console.WriteLine(format);

                        var GXFormat = (Decode_Gamecube.TextureFormats)format;

                        //Now create a wrapper
                        var texWrapper = new TplTextureWrapper();
                        texWrapper.Text = $"Texture {i}";
                        texWrapper.ImageKey = "Texture";
                        texWrapper.SelectedImageKey = "Texture";
                        texWrapper.Format = Decode_Gamecube.ToGenericFormat(GXFormat);
                        texWrapper.Width = width;
                        texWrapper.Height = height;
                        texWrapper.MipCount = mipCount;
                        texWrapper.ImageData = reader.getSection(offset, (uint)Decode_Gamecube.GetDataSize(GXFormat, (int)width, (int)height));
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

                        var GXFormat = (Decode_Gamecube.TextureFormats)image.Format;

                        Console.WriteLine($"ImageOffset {image.ImageOffset}");

                        //Now create a wrapper
                        var texWrapper = new TplTextureWrapper();
                        texWrapper.Text = $"Texture {i}";
                        texWrapper.ImageKey = "Texture";
                        texWrapper.SelectedImageKey = "Texture";
                        texWrapper.Format = Decode_Gamecube.ToGenericFormat(GXFormat);
                        texWrapper.Width = image.Width;
                        texWrapper.Height = image.Height;
                        texWrapper.MipCount = 1;
                        texWrapper.PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;
                        texWrapper.ImageData = reader.getSection(image.ImageOffset, 
                            (uint)Decode_Gamecube.GetDataSize(GXFormat, (int)image.Width, (int)image.Height));

                        //Palette is sometimes unused to check
                        if (PaletteHeaderOffset != 0)
                        {
                            reader.SeekBegin(PaletteHeaderOffset);
                            var palette = new PaletteHeader();
                            palette.Read(reader);
                            PaletteHeaders.Add(palette);

                            var GXPaletteFormat = (Decode_Gamecube.PaletteFormats)image.Format;

                            texWrapper.SetPaletteData(palette.Data, Decode_Gamecube.ToGenericPaletteFormat(GXPaletteFormat));
                        }

                        Nodes.Add(texWrapper);
                    }
                }
            }
        }

        public class TplTextureWrapper : STGenericTexture
        {
            public byte[] ImageData { get; set; }

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

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
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

                editor.LoadProperties(GenericProperties);
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
                Height = reader.ReadUInt16();
                Width = reader.ReadUInt16();
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

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
