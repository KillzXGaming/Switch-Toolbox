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

namespace FirstPlugin
{
    public class IGZ : STGenericTexture, IFileFormat, ISingleTextureIconLoader
    {
        public STGenericTexture IconTexture { get { return this; } }

        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Crash N.Sane Trilogy / CTR: Nitro-Fueled (IGZ)" };
        public string[] Extension { get; set; } = new string[] { "*.igz" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "\x01ZGI");
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

        public Dictionary<uint, FormatInfo> Formats = new Dictionary<uint, FormatInfo>()
        {
        /*    { 0xDE, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },
            { 0x08, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },
            { 0x46, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },
            { 0x99, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },

          //  { 0xCD, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0x06, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0x3B, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0x9D, new FormatInfo(TEX_FORMAT.BC1_UNORM) },

            { 0x51, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x28, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x1B, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },

            { 0x39, new FormatInfo(TEX_FORMAT.BC3_UNORM) },
            { 0x88, new FormatInfo(TEX_FORMAT.BC3_UNORM) },
            { 0xDA, new FormatInfo(TEX_FORMAT.BC3_UNORM) },

            { 0xCD, new FormatInfo(TEX_FORMAT.BC3_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x6E, new FormatInfo(TEX_FORMAT.BC3_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x45, new FormatInfo(TEX_FORMAT.BC3_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x37, new FormatInfo(TEX_FORMAT.BC3_UNORM, PlatformSwizzle.Platform_Switch) },

            { 0x18, new FormatInfo(TEX_FORMAT.BC5_UNORM) },
            { 0x47, new FormatInfo(TEX_FORMAT.BC5_UNORM) },
            { 0xB9, new FormatInfo(TEX_FORMAT.BC5_UNORM) },
            { 0x78, new FormatInfo(TEX_FORMAT.BC5_UNORM) },
            */
            { 0x9D3B06CD, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0xDA888839, new FormatInfo(TEX_FORMAT.BC3_UNORM) }, //PC
            { 0x78B94718, new FormatInfo(TEX_FORMAT.BC5_UNORM) }, //PC
            { 0x994608DE, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT) }, //PC
            { 0x1B282851, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x37456ECD, new FormatInfo(TEX_FORMAT.BC3_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0xD0124568, new FormatInfo(TEX_FORMAT.BC3_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x8EBE8CF2, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT, PlatformSwizzle.Platform_Switch) },
            { 0xF8313483, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Ps4) },
            { 0xF0B976CF, new FormatInfo(TEX_FORMAT.BC3_UNORM, PlatformSwizzle.Platform_Ps4) },
            { 0x7D081E6A, new FormatInfo(TEX_FORMAT.BC5_UNORM, PlatformSwizzle.Platform_Ps4) },
            { 0x9B54FB48, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT, PlatformSwizzle.Platform_Ps4) },
            { 0xEDF22608, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT, PlatformSwizzle.Platform_Ps4) }, //Todo BGR32
        };

        public class FormatInfo
        {
            public TEX_FORMAT Format;
            public PlatformSwizzle Platform;

            public FormatInfo(TEX_FORMAT format, PlatformSwizzle platform = PlatformSwizzle.None)
            {
                Format = format;
                Platform = platform;
            }
        }

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ReadSignature(4, "\x01ZGI");
                uint unknown = reader.ReadUInt32(); //Same as section count?
                uint unknown2 = reader.ReadUInt32(); //2171947023
                uint unknown3    = reader.ReadUInt32(); //2
                uint SectionCount = reader.ReadUInt32();
                reader.Seek(4); //padding

                //Always 3 of these used that point to certain areas
                const int BLOCK_COUNT = 3;

                uint[] offsets = new uint[BLOCK_COUNT];
                uint[] sizes = new uint[BLOCK_COUNT];
                for (int j = 0; j < BLOCK_COUNT; j++)
                {
                    offsets[j] = reader.ReadUInt32();
                    sizes[j] = reader.ReadUInt32();
                    reader.Seek(8);
                }

                List<string> TextureNames = new List<string>();
                FormatInfo TextureFormat = null;

                //Now go to the first block. This is a list of section headers
                //Each header follows the same structure, a signature, count, and size
                reader.SeekBegin(offsets[0]);
                for (int s = 0; s < SectionCount; s++)
                {
                    long pos = reader.Position;

                    string signature = reader.ReadString(4);
                    uint count = reader.ReadUInt32();
                    uint sectioSize = reader.ReadUInt32();
                    uint dataOffset = reader.ReadUInt32(); //0x10. Relative to start of section

                    Console.WriteLine($"{signature} {count} {sectioSize} {dataOffset}");

                    reader.SeekBegin(pos + dataOffset);
                    switch (signature)
                    {
                        case "TSTR":
                            for (int i = 0; i < count; i++)
                                TextureNames.Add(reader.ReadZeroTerminatedString());

                            if (count > 0)
                                Text = TextureNames[0];

                            foreach (string name in TextureNames)
                                Console.WriteLine("tex " + name);
                            break;
                        case "TMET":
                            break;
                        case "MTSZ":
                            break;
                        case "EXID": //Texture Formats and maybe other formats
                           // reader.ReadUInt32(); //padding?

                            //Seems only one code is used but why?
                            uint FormatCode = reader.ReadUInt32();
                            if (Formats.ContainsKey(FormatCode))
                                TextureFormat = Formats[FormatCode];
                            else
                                throw new Exception("Unsupported format code!" + FormatCode.ToString("X"));

                            Console.WriteLine($"TextureFormat {FormatCode.ToString("X")}");
                            break;
                        case "RVTB":
                            break;
                        case "RSTT":
                            break;
                        case "ROFS":
                            break;
                        case "REXT":
                            break;
                        case "ROOT":
                            break;
                        case "ONAM":
                            break;
                        default:
                            Console.WriteLine("Unexpected Magic " + signature);
                            break;
                    }

                    reader.SeekBegin(pos + sectioSize);
                }

                reader.SeekBegin(offsets[1] + 0x48);
                ushort width = reader.ReadUInt16();
                ushort height = reader.ReadUInt16();
                ushort depth = reader.ReadUInt16();
                ushort mipCount = reader.ReadUInt16();
                ushort arrayCount = reader.ReadUInt16();
                reader.Seek(10); //padding
                uint unk = reader.ReadUInt32();
                reader.Seek(8); //padding
                uint ImageSize = reader.ReadUInt32();

                //Seek to start of image block
                reader.Seek(84);
                long dataPos = reader.Position;
                long ImageSizeReal = reader.BaseStream.Length - dataPos;

                ImageData = reader.ReadBytes((int)ImageSizeReal);
                Width = width;
                Height = height;
                Depth = depth;
                ArrayCount = arrayCount;
                Format = TextureFormat.Format;
             //   PlatformSwizzle = TextureFormat.Platform;

                Console.WriteLine($"PlatformSwizzle " + PlatformSwizzle);
            }
        }

        public byte[] ImageData { get; set; }

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.BC1_UNORM,
                    TEX_FORMAT.BC3_UNORM,
                    TEX_FORMAT.BC5_UNORM,
                    TEX_FORMAT.R8G8B8A8_UNORM,
                    TEX_FORMAT.R32G32B32A32_FLOAT,
                };
            }
        }


        public override bool CanEdit { get; set; } = false;

        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {

        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            if (PlatformSwizzle == PlatformSwizzle.Platform_Switch)
                return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, 1);
            else
                return ImageData;
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

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public class TSTR
        {
            List<string> TextureNames = new List<string>();

            public void Read(FileReader reader)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, "TSTR");
                uint textureCount = reader.ReadUInt32();
                uint sectionSize = reader.ReadUInt32();
                uint stringOffset = reader.ReadUInt32();

                reader.SeekBegin(pos +stringOffset);
                for (int i = 0; i < textureCount; i++)
                {
                    TextureNames.Add(reader.ReadZeroTerminatedString());
                }
            }
        }
    }
}
