using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace FirstPlugin.NLG
{
    public class StrikersRLT : TreeNodeFile, IFileFormat, ITextureContainer
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }

        public string[] Description { get; set; } = new string[] { "Strikers Texture Image" };
        public string[] Extension { get; set; } = new string[] { "*.glt",  "*.rlt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true)) {
                return reader.CheckSignature(4, "PTLG");
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

        public Dictionary<byte, Decode_Gamecube.TextureFormats> FormatList = new Dictionary<byte, Decode_Gamecube.TextureFormats>
        {
            { 0x8, Decode_Gamecube.TextureFormats.RGBA32 },
            { 0x7, Decode_Gamecube.TextureFormats.RGB565 },
            { 0x6, Decode_Gamecube.TextureFormats.CMPR },
            { 0x5, Decode_Gamecube.TextureFormats.RGB5A3 },
            { 0x4, Decode_Gamecube.TextureFormats.IA4 },
            { 0x3, Decode_Gamecube.TextureFormats.I8 },
            { 0x2, Decode_Gamecube.TextureFormats.I4 },
        };

        private bool IsGamecube = false;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                uint magic = reader.ReadUInt32();
                uint numTextures = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();

                if (unk == 0)
                    IsGamecube = true;

                if (reader.ReadUInt32() == 0)
                    reader.Seek(12);
                else
                    reader.Seek(-4);

                if (NLG_Common.HashNames.ContainsKey(unk))
                    Text = "HASH: " + NLG_Common.HashNames[unk];

                var textures = reader.ReadMultipleStructs<TextureHashEntry>(numTextures);

                long startPos = reader.Position;
                for (int i = 0; i < numTextures; i++)
                {
                    TextureEntry entry = new TextureEntry();
                    Nodes.Add(entry);
                    PluginRuntime.stikersTextures.Add(entry);

                    reader.SeekBegin(startPos + textures[i].ImageOffset);

                    long pos = reader.Position;

                    //Texture Info
                    uint MipCount = reader.ReadUInt32();
                    uint unknown2 = reader.ReadUInt32(); //1
                    byte unknown4 = reader.ReadByte(); //5
                    byte Format = reader.ReadByte();
                    byte unknown5 = reader.ReadByte(); //5
                    byte unknown6 = reader.ReadByte(); //3
                    ushort Width = reader.ReadUInt16();

                    //Dumb hack. Idk why there's sometimes padding. Aligning doesn't always fix it.
                    if (Width == 0)
                        Width = reader.ReadUInt16();
                    ushort Height = reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    uint headerSize = (uint)(reader.Position - pos);
                    uint imageSize = textures[i].SectionSize - headerSize;

                    reader.SeekBegin(pos + headerSize);
                    entry.ImageData = reader.ReadBytes((int)imageSize);


                    uint hash = textures[i].Hash;
                    entry.Text = hash.ToString("X");
                    entry.ImageKey = "texture";
                    entry.SelectedImageKey = "texture";

                    if (NLG_Common.HashNames.ContainsKey(hash))
                        entry.Text = NLG_Common.HashNames[hash];

                   // entry.Text += $" {textureInfo.Format} {textureInfo.unknown2} {textureInfo.unknown4} {textureInfo.unknown5} {textureInfo.unknown6} ";

                    var formatGC = Decode_Gamecube.TextureFormats.CMPR;
                    if (FormatList.ContainsKey(Format))
                        formatGC = FormatList[Format];

                    entry.MipCount = MipCount;
                    entry.Format = Decode_Gamecube.ToGenericFormat(formatGC);
                    entry.Width = Width;
                    entry.Height = Height;
                    entry.PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class TextureHashEntry
        {
            public uint Hash;
            public uint ImageOffset;
            public uint SectionSize;
            public uint Unknown;
        }

        public class TextureEntry : STGenericTexture
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

                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {

            }
        }

        public void Unload()
        {
            foreach (TextureEntry tex in Nodes)
                if (PluginRuntime.stikersTextures.Contains(tex))
                    PluginRuntime.stikersTextures.Remove(tex);
        }
    }
}
