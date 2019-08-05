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
    //A structure for an IGZ file that contains multiple resources
    //This is to fully parse and get data from it from all resource types
    //This links to multiple seperate files
    public class IGZ_Structure
    {
        public static bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "\x01ZGI");
            }
        }

        public enum Platform
        {
            Ps4,
            Switch,
            PC,
        }

        //PC works perfectly, but Switch has issues with CTR
        //Also a note, switch swizzling is never used? It always seems to be raw data
        //Set the platform still for the sake of it. It won't swizzle or anything
        public Dictionary<uint, FormatInfo> Formats = new Dictionary<uint, FormatInfo>()
        {
            { 0x9D3B06CD, new FormatInfo(TEX_FORMAT.BC1_UNORM) },
            { 0xDA888839, new FormatInfo(TEX_FORMAT.BC3_UNORM) }, //PC
            { 0x78B94718, new FormatInfo(TEX_FORMAT.BC5_UNORM) }, //PC
            { 0x994608DE, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT) }, //PC
            { 0x1B282851, new FormatInfo(TEX_FORMAT.BC1_UNORM, Platform.Switch) },
            { 0x37456ECD, new FormatInfo(TEX_FORMAT.BC3_UNORM, Platform.Switch) },
            { 0xD0124568, new FormatInfo(TEX_FORMAT.BC3_UNORM, Platform.Switch) },
            { 0x8EBE8CF2, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT, Platform.Switch) },
            { 0xF8313483, new FormatInfo(TEX_FORMAT.BC1_UNORM, Platform.Ps4) },
            { 0xF0B976CF, new FormatInfo(TEX_FORMAT.BC3_UNORM, Platform.Ps4) },
            { 0x7D081E6A, new FormatInfo(TEX_FORMAT.BC5_UNORM, Platform.Ps4) },
            { 0x9B54FB48, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT, Platform.Ps4) },
            { 0xEDF22608, new FormatInfo(TEX_FORMAT.R32G32B32_FLOAT, Platform.Ps4) }, //Todo BGR32
        };

        public class FormatInfo
        {
            public TEX_FORMAT Format;
            public Platform Platform;

            public FormatInfo(TEX_FORMAT format, Platform platform = Platform.PC)
            {
                Format = format;
                Platform = platform;
            }
        }

        public uint Version { get; set; }
        public uint HeaderUnknown { get; set; }
        public uint HeaderUnknown2 { get; set; }

        public List<string> HeaderStrings = new List<string>();
        public List<string> DependenciesTable = new List<string>();
        public List<string> StringTable = new List<string>();
        public List<string> TMETStrings = new List<string>();
        public List<uint> MTSZTable = new List<uint>();
        public List<Tuple<uint, uint>> FormatCodeList = new List<Tuple<uint, uint>>();
        public List<uint> ONAMTable = new List<uint>();

        public List<CommonHeaderInfo> Headers = new List<CommonHeaderInfo>();

        public TextureBlockInfo TextureInfo;

        public byte[] UnknownData;

        public void Read(FileReader reader)
        {
            reader.ReadSignature(4, "\x01ZGI");
            Version = reader.ReadUInt32(); //10
            HeaderUnknown = reader.ReadUInt32(); //2171947023
            HeaderUnknown2 = reader.ReadUInt32(); //2
            uint SectionCount = reader.ReadUInt32();
            reader.Seek(4); //padding

            //A chunk/block offset list. 
            List<uint> offsets = new List<uint>();
            List<uint> sizes = new List<uint>();

            while (true)
            {
                uint offset = reader.ReadUInt32();
                uint size = reader.ReadUInt32();
                uint unknown = reader.ReadUInt32();
                uint unknown2 = reader.ReadUInt32();

                if (offset != 0)
                {
                    offsets.Add(offset);
                    sizes.Add(size);
                }
                else
                    break;
            }

            //After we parsed the offsets/size array
            //Save the rest of the data up to the next section
            //All of this is mostly padding and a string or 2

            reader.Seek(-16); //Seek back 16 due to trying to read the chunk info

            long headerEnd = reader.Position;
            UnknownData = reader.ReadBytes((int)(offsets[0] - headerEnd));

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

                CommonHeaderInfo header = new CommonHeaderInfo();
                header.Magic = signature;

                reader.SeekBegin(pos + dataOffset);
                switch (signature)
                {
                    case "TDEP": //Dependinces table
                        for (int i = 0; i < count; i++)
                            DependenciesTable.Add(reader.ReadZeroTerminatedString());
                        break;
                    case "TSTR": //string table i assume
                        for (int i = 0; i < count; i++)
                            StringTable.Add(reader.ReadZeroTerminatedString());
                        break;
                    case "TMET": //Another string list
                        for (int i = 0; i < count; i++)
                            TMETStrings.Add(reader.ReadZeroTerminatedString());
                        break;
                    case "MTSZ":
                        for (int i = 0; i < count; i++)
                            MTSZTable.Add(reader.ReadUInt32());
                        break;
                    case "EXID": //A format list for multiple data types
                        for (int i = 0; i < count; i++)
                            FormatCodeList.Add(Tuple.Create(reader.ReadUInt32(), reader.ReadUInt32()));
                        break;
                    case "EXNM": //Material data
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
                        for (int i = 0; i < count; i++)
                            ONAMTable.Add(reader.ReadUInt32());
                        break;
                    default:
                        Console.WriteLine("Unexpected Magic " + signature);
                        break;
                }

                //For saving back just save the raw bytes
                reader.SeekBegin(pos);
                header.data = reader.ReadBytes((int)sectioSize);
                Headers.Add(header);

                reader.SeekBegin(pos + sectioSize);
            }

            //Todo find a better check for how a block/chunk is determined
            if (sizes[1] == 192) //The size of an image info chunk/block
            {
                reader.SeekBegin(offsets[1]);
                TextureInfo = new TextureBlockInfo();
                TextureInfo.UnkownData1 = reader.ReadBytes(0x48);
                TextureInfo.Width = reader.ReadUInt16();
                TextureInfo.Height = reader.ReadUInt16();
                TextureInfo.Depth = reader.ReadUInt16();
                TextureInfo.MipCount = reader.ReadUInt16();
                TextureInfo.ArrayCount = reader.ReadUInt16();
                reader.Seek(10); //padding
                TextureInfo.UnknownValue = reader.ReadUInt32();
                reader.Seek(8); //padding
                uint ImageSize = reader.ReadUInt32();
                //Seek to start of image block

                TextureInfo.UnkownData2 = reader.ReadBytes((int)(offsets[2] - reader.Position));

                //Seek the second block/chunk info 
                reader.SeekBegin(offsets[2]);
                long dataPos = reader.Position;

                //The imagesize is weird so get from the end of file
                long ImageSizeReal = reader.BaseStream.Length - dataPos;
                TextureInfo.ImageData = reader.ReadBytes((int)ImageSizeReal);

                //Todo why is there 2 format codes even for textures?
                foreach (var format in FormatCodeList)
                {
                    if (Formats.ContainsKey(format.Item1))
                        TextureInfo.FormatInfo = Formats[format.Item1];
                    else
                        Console.WriteLine("Unsupported format code!" + format.Item1.ToString("X"));

                  /*  if (Formats.ContainsKey(format.Item2))
                        TextureInfo.FormatInfo = Formats[format.Item2];
                    else
                        Console.WriteLine("Unsupported format code!" + format.Item2.ToString("X"));*/
                }
            }
            else if (sizes[1] == 2568) //The size of a model info chunk/block
            {

            }
        }

        //Todo this has quite a bit of unknowns and needs more chunk support
        //Textures should save fine
        public void Write(FileWriter writer)
        {
            if (TextureInfo == null)
                throw new Exception("Only textures can be saved currently!");

            //Set the format code for the texture if found a proper pair match
            if (TextureInfo != null)
            {
                //Go through each value and find a pair with a matching format and matching platform
                foreach (var formatInfo in Formats)
                {
                    uint code = formatInfo.Key;
                    if (formatInfo.Value.Format == TextureInfo.FormatInfo.Format &&
                      formatInfo.Value.Platform == TextureInfo.FormatInfo.Platform)
                    {
                        //Set the first code
                        //unsure what the second code is for
                        uint code2 =  FormatCodeList[0].Item2;
                        FormatCodeList[0] = Tuple.Create(code, code2);
                    }
                }
            }

            writer.SetByteOrder(false);

            writer.WriteSignature("\x01ZGI");
            writer.Write(Version);
            writer.Write(HeaderUnknown);
            writer.Write(HeaderUnknown2);
            writer.Write(Headers.Count);
            writer.Seek(4); //padding

            long chunkInfoArray = writer.Position;

            //Reseve space for all the used chunks
            for (int i = 0; i < 3; i++)
            {
                writer.Write(uint.MaxValue);
                writer.Write(uint.MaxValue);
                writer.Write(uint.MaxValue);
                writer.Write(uint.MaxValue);
            }

            //Possibly aligned data, but there's sometimes strings in it
            writer.Write(UnknownData);

            //Now start saving the header chunk. 
            long headerArrayPos = writer.Position;
            foreach (var header in Headers)
            {
                if (header.Magic == "TSTR") //Save our string array chunk
                {
                    long posTSRT = writer.Position;

                    writer.WriteSignature("TSTR");
                    writer.Write(StringTable.Count);
                    long posStrSizeOfs = writer.Position;
                    writer.Write(0); //save size after
                    writer.Write(16); //offset

                    foreach (var item in StringTable)
                    {
                        writer.WriteString(item);
                    }

                    long posTSRTEnd = writer.Position;

                    //Note i will automate sizes later
                    writer.SeekBegin(posStrSizeOfs);
                    writer.Write((uint)(posTSRTEnd - posTSRT));

                    writer.SeekBegin(posTSRTEnd);
                }
                else if (header.Magic == "EXID") //Save our format chunk
                {
                    writer.WriteSignature("EXID");
                    writer.Write(FormatCodeList.Count);
                    writer.Write(20 + (FormatCodeList.Count * 8)); //Size
                    writer.Write(20); //Offset
                    writer.Write(0); //Padding
                    foreach (var item in FormatCodeList)
                    {
                        writer.Write(item.Item1);
                        writer.Write(item.Item2);
                    }
                }
                else
                    writer.Write(header.data);
            }
            long headerEndPos = writer.Position;

            long infoChunkPos = writer.Position;
            if (TextureInfo != null)
            {
                writer.Write(TextureInfo.UnkownData1);
                writer.Write(TextureInfo.Width);
                writer.Write(TextureInfo.Height);
                writer.Write(TextureInfo.Depth);
                writer.Write(TextureInfo.MipCount);
                writer.Write(TextureInfo.ArrayCount);
                writer.Seek(10); //padding
                writer.Write(TextureInfo.UnknownValue);
                writer.Seek(8); //padding
                writer.Write(TextureInfo.ImageData.Length);
                writer.Write(TextureInfo.UnkownData2);
            }

            long infoChunkEndPos = writer.Position;

            long dataChunkPos = writer.Position;
            if (TextureInfo != null)
            {
                writer.Write(TextureInfo.ImageData);
            }

            long dataChunkEndPos = writer.Position;

            //Go back and write all the used chunk data so far
            writer.SeekBegin(chunkInfoArray);

            //Write the header array chunk first
            writer.Write((uint)headerArrayPos); //Offset
            writer.Write((uint)(headerEndPos - headerArrayPos)); //Size
            writer.Write(2048); //Unknown
            writer.Write(0); //Unknown

            //Write the info chunk
            writer.Write((uint)infoChunkPos); //Offset
            writer.Write((uint)(infoChunkEndPos - infoChunkPos)); //Size
            writer.Write(8); //Unknown
            writer.Write(131072); //Unknown

            //Write the data chunk
            writer.Write((uint)dataChunkPos); //Offset
            writer.Write((uint)(dataChunkEndPos - dataChunkPos)); //Size
            writer.Write(4); //Unknown
            writer.Write(0); //Unknown
        }

        public class CommonHeaderInfo
        {
            public string Magic;
            public byte[] data;
        }

        public class TextureBlockInfo
        {
            public byte[] UnkownData1;
            public uint UnknownValue;
            public byte[] UnkownData2;

            public byte[] ImageData { get; set; }

            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public ushort Depth { get; set; }
            public ushort MipCount { get; set; }
            public ushort ArrayCount { get; set; }
            public FormatInfo FormatInfo { get; set; }
        }
    }
}
