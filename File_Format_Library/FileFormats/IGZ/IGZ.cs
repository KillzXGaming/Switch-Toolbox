using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class IGZ : IFileFormat
    {
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
            { 0xDE, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },
            { 0x08, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },
            { 0x46, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },
            { 0x99, new FormatInfo(TEX_FORMAT.R8G8B8A8_UNORM) },

            { 0xCD, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0x06, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0x3B, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0x9D, new FormatInfo(TEX_FORMAT.BC1_UNORM) },

            { 0x51, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x28, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x28, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },
            { 0x1B, new FormatInfo(TEX_FORMAT.BC1_UNORM, PlatformSwizzle.Platform_Switch) },

            { 0x39, new FormatInfo(TEX_FORMAT.BC3_UNORM) },
            { 0x88, new FormatInfo(TEX_FORMAT.BC3_UNORM) },
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

                //Now go to the first block. This is a list of section headers
                //Each header follows the same structure, a signature, count, and size
                for (int s = 0; s < SectionCount; s++)
                {
                    long pos = reader.Position;

                    string signature = reader.ReadString(4);
                    uint count = reader.ReadUInt32();
                    uint sectioSize = reader.ReadUInt32();
                    uint dataOffset = reader.ReadUInt32(); //0x10. Relative to start of section

                    uint textureCount = 0;
                    List<string> TextureNames = new List<string>();
                    FormatInfo TextureFormat;

                    reader.Seek(dataOffset);
                    switch (signature)
                    {
                        case "TSTR":
                            textureCount = count;
                            for (int i = 0; i < count; i++)
                                TextureNames.Add(reader.ReadZeroTerminatedString());
                            break;
                        case "TMET":
                            break;
                        case "MTSZ":
                            break;
                        case "EXID": //Texture Formats and maybe other formats
                            reader.ReadUInt32(); //padding?

                            //Seems only one code is used but why?
                            uint FormatCode = reader.ReadUInt32();
                            if (Formats.ContainsKey(FormatCode))
                                TextureFormat = Formats[FormatCode];
                            else
                                throw new Exception("Unsupported format code!" + FormatCode.ToString("X"));
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
            }
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
