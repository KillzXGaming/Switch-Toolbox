using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using FirstPlugin.Forms;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class MSBT : IEditor<MSBTEditor>, IFileFormat, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Message;

        public bool CanSave { get; set; } = true;
        public string[] Description { get; set; } = new string[] { "Message Studio Binary Text" };
        public string[] Extension { get; set; } = new string[] { "*.msbt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(8, "MsgStdBn");
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


        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Xml;
        public bool CanConvertBack => false;

        public string ConvertToString() {
            return MSYT.ToYaml(this);
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        public MSBTEditor OpenForm()
        {
            MSBTEditor editor = new MSBTEditor();
            editor.Text = FileName;
            editor.Dock = DockStyle.Fill;
            return editor;
        }

        public void FillEditor(UserControl control)
        {
            ((MSBTEditor)control).LoadMSBT(this);
        }

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            header = new Header();
            header.Read(new FileReader(stream));
        }
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            header.Write(new FileWriter(stream));
        }

        public bool HasLabels
        {
            get { return header.Label1.Labels.Count > 0; }
        }

        public class Header
        {
            public ushort ByteOrderMark;
            public ushort Padding;
            public ushort Unknown;
            public Encoding StringEncoding = Encoding.Unicode;

            public byte Version;
            public List<MSBTEntry> entries = new List<MSBTEntry>();

            byte[] Reserved = new byte[10];

            public LBL1 Label1;
            public NLI1 NLI1;
            public TXT2 Text2;

            public bool IsBigEndian = false;

            public void Read(FileReader reader)
            {
                Label1 = new LBL1();
                NLI1 = new NLI1();
                Text2 = new TXT2();
                 
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(8, "MsgStdBn");
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                IsBigEndian = reader.IsBigEndian;
                Padding = reader.ReadUInt16();
                byte encoding = reader.ReadByte();
                Version = reader.ReadByte();
                ushort SectionCount = reader.ReadUInt16();
                Unknown = reader.ReadUInt16();
                uint FileSize = reader.ReadUInt32();
                Reserved = reader.ReadBytes(10);


                if (encoding == 0x00)
                    StringEncoding = Encoding.UTF8;
                else if (reader.IsBigEndian)
                    StringEncoding = Encoding.BigEndianUnicode;
                else
                    StringEncoding = Encoding.Unicode;

                for (int i = 0; i < SectionCount; i++)
                {
                    if (reader.EndOfStream)
                        break;

                    long pos = reader.Position;

                    string Signature = reader.ReadString(4, Encoding.ASCII);
                    uint SectionSize = reader.ReadUInt32();

                    Console.WriteLine("Signature " + Signature);

                    switch (Signature)
                    {
                        case "NLI1":
                            NLI1 = new NLI1();
                            NLI1.Signature = Signature;
                            NLI1.Read(reader, this);
                            entries.Add(NLI1);
                            break;
                        case "TXT2":
                        case "TXTW":
                            Text2 = new TXT2();
                            Text2.Signature = Signature;
                            Text2.Read(reader, this);
                            entries.Add(Text2);
                            break;
                        case "LBL1":
                            Label1 = new LBL1();
                            Label1.Signature = Signature;
                            Label1.Read(reader, this);
                            entries.Add(Label1);
                            break;
                        case "ATR1":
                        case "ATO1":
                        case "TSY1":
                        default:
                            MSBTEntry entry = new MSBTEntry();
                            entry.Signature = Signature;
                            entry.Padding = reader.ReadBytes(8);
                            entry.Data = reader.ReadBytes((int)SectionSize);
                            entries.Add(entry);
                            break;
                    }

                    reader.SeekBegin(pos + SectionSize + 0x10);
                    reader.Align(16);
                }

                //Setup labels to text properly
                if (Label1 != null && Text2 != null)
                {
                    foreach (var label in Label1.Labels)
                        label.String = Text2.TextData[(int)label.Index];
                }
            }

            public void Write(FileWriter writer)
            {
                writer.SetByteOrder(true);
       
                writer.WriteSignature("MsgStdBn");
                if (!IsBigEndian)
                    writer.Write((ushort)0xFFFE);
                else
                    writer.Write((ushort)0xFEFF);
                writer.SetByteOrder(IsBigEndian);
                writer.Write(Padding);
                writer.Write(StringEncoding == Encoding.UTF8 ? (byte)0 : (byte)1);
                writer.Write(Version);
                writer.Write((ushort)entries.Count);
                writer.Write(Unknown);
                 
                long _ofsFileSize = writer.Position;
                writer.Write(0); //FileSize reserved for later
                writer.Write(Reserved);

                foreach (var entry in entries)
                    WriteSection(writer, this, entry.Signature, entry);

                //Write file size
                using (writer.TemporarySeek(_ofsFileSize, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }

            public override string ToString()
            {
                var builder = new StringBuilder();
                using (var textWriter = new StringWriter(builder))
                {
                    textWriter.Write($"");
                }
                return builder.ToString();
            }
        }

        public class LabelGroup
        {
            public uint NumberOfLabels;
            public uint Offset;
        }

        public class LabelEntry : MSBTEntry
        {
            private uint _index;

            public uint Length;
            public string Name;
            public uint Checksum;
            public StringEntry String;

            public uint Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public byte[] Value
            {
                get { return String.Data; }
                set { String.Data = value; }
            }
        }

        public class StringEntry : MSBTEntry
        {
            private uint _index;

            public byte[] OriginalDataCached = new byte[0];

            public StringEntry(byte[] data) {
                Data = data;
                OriginalDataCached = Data;
            }

            public StringEntry(byte[] data, Encoding encoding) {
                Data = data;
                OriginalDataCached = Data;
            }

            public StringEntry(string text, Encoding encoding) {
                Data = encoding.GetBytes(text);
                OriginalDataCached = encoding.GetBytes(text);
            }

            public uint Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public string GetTextLabel(bool ShowText, Encoding encoding)
            {
                if (ShowText)
                    return $"{_index + 1} {GetText(encoding)}";
                else
                    return $"{_index + 1}";
            }

            public string GetText(Encoding encoding)
            {
                return encoding.GetString(Data);
            }

            public string GetOriginalText(Encoding encoding) {
                return encoding.GetString(OriginalDataCached);
            }

            public void SetText(string text, Encoding encoding)
            {
                Data = encoding.GetBytes(text);
            }

            public byte[] ToBytes(Encoding encoding, bool isBigEndian)
            {
                return Data;

                var mem = new MemoryStream();
                var text = GetText(encoding);
                using (var writer = new FileWriter(mem, encoding)) {
                    writer.SetByteOrder(isBigEndian);
                    for (int i = 0; i < text.Length; i++)
                    {
                        var c = text[i];
                        writer.Write(c);
                        if (c == 0xE)
                        {
                            writer.Write((short)text[++i]);
                            writer.Write((short)text[++i]);
                            int count = text[++i];
                            writer.Write((short)count);
                            for (var j = 0; j < count; j++)
                            {
                                writer.Write((byte)text[++i]);
                            }
                        }
                        if (c == 0xF)
                        {
                            //end tag
                            writer.Write((short)text[++i]);
                            writer.Write((short)text[++i]);
                        }
                    }
                    writer.Write('\0');
                }
                return mem.ToArray();
            }
        }

        public class TXT2 : MSBTEntry
        {
            public uint[] Offsets;
            public List<StringEntry> TextData = new List<StringEntry>();
            public List<StringEntry> OriginalTextData = new List<StringEntry>();

            public override void Read(FileReader reader, Header header)
            {
                reader.Seek(-4);
                uint sectionSize = reader.ReadUInt32();

                Padding = reader.ReadBytes(8);

                long pos = reader.Position;
                EntryCount = reader.ReadUInt32();
                Offsets = reader.ReadUInt32s((int)EntryCount);

                for (int i = 0; i < EntryCount; i++)
                {
                    //Get the start and end position
                    uint startPos = Offsets[i] + (uint)pos;
                    uint endPos = i + 1 < EntryCount ? (uint)pos + Offsets[i + 1] :
                                                       (uint)pos + sectionSize;

                    reader.SeekBegin(startPos);
                    ReadMessageString(reader, header, (uint)i, endPos - startPos);
                }
            }

            private void ReadMessageString(FileReader reader, Header header, uint index, uint size)
            {
                byte[] textData = reader.ReadBytes((int)size);

                TextData.Add(new StringEntry(textData, header.StringEncoding) { Index = index, });
                OriginalTextData.Add(new StringEntry(textData, header.StringEncoding) { Index = index, });
            }

            public override void Write(FileWriter writer, Header header)
            {
                writer.Seek(8);

                long pos = writer.Position;
                writer.Write(TextData.Count);
                writer.Write(new uint[TextData.Count]);

                for (int i = 0; i < EntryCount; i++) {
                    writer.WriteUint32Offset(pos + 4 + (i * 4), pos);
                    writer.Write(TextData[i].ToBytes(header.StringEncoding, header.IsBigEndian));
                }
            }

            private char[] GetControlCode(FileReader reader)
            {
                //Get char controls
                //Code from https://github.com/Sage-of-Mirrors/WildText/blob/master/WildText/src/MessageManager.cs
                List<char> controlCode = new List<char>();
                controlCode.Add('<');

                short primaryType = reader.ReadInt16();
                short secondaryType = reader.ReadInt16();
                short dataSize = reader.ReadInt16();

                switch (primaryType)
                {
                    case 0:
                        controlCode.AddRange(GetTextModifier(reader, secondaryType));
                        break;
                    case 1:
                        controlCode.AddRange(GetPlayerInput(reader, secondaryType));
                        break;
                    case 2:
                        break;
                    case 3:
                        controlCode.AddRange(GetAnimationIndex(reader, secondaryType));
                        break;
                    case 4:
                        controlCode.AddRange(GetSoundIndex(reader, secondaryType));
                        break;
                    case 5:
                        controlCode.AddRange(GetPause(reader, secondaryType));
                        break;
                    default:
                        reader.BaseStream.Position += dataSize;
                        break;
                }

                controlCode.Add('>');

                return controlCode.ToArray();
            }

            private char[] GetTextModifier(FileReader reader, short secondaryType)
            {
                List<char> result = new List<char>();

                switch (secondaryType)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        result.AddRange($"size:{ reader.ReadInt16() }");
                        break;
                    case 3:
                        result.AddRange($"Color:{ reader.ReadInt16() }");
                        break;
                }

                return result.ToArray();
            }

            private char[] GetPause(FileReader reader, short secondaryType)
            {
                List<char> result = new List<char>();

                switch (secondaryType)
                {
                    case 0:
                        result.AddRange("pause:short");
                        break;
                    case 1:
                        result.AddRange("pause:medium");
                        break;
                    case 2:
                        result.AddRange("pause:long");
                        break;
                }

                return result.ToArray();
            }

            private char[] GetAnimationIndex(FileReader reader, short secondaryType)
            {
                List<char> result = new List<char>();

                switch (secondaryType)
                {
                    case 0:
                        throw new FormatException();
                    case 1:
                        result.AddRange($"Anim:{ reader.ReadUInt16() }");
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }

                return result.ToArray();
            }

            private char[] GetSoundIndex(FileReader reader, short secondaryType)
            {
                List<char> result = new List<char>();

                switch (secondaryType)
                {
                    case 1:
                        break;
                    case 2:
                        short stringIDSize = (short)(reader.ReadInt16() / 2);
                        result.AddRange("Sound:");
                        for (int i = 0; i < stringIDSize; i++)
                            result.Add((char)reader.ReadInt16());
                        break;
                }

                return result.ToArray();
            }

            private char[] GetPlayerInput(FileReader reader, short secondaryType)
            {
                List<char> result = new List<char>();

                switch (secondaryType)
                {
                    case 4:
                    case 5:
                    case 6:
                        result.AddRange("choice:");
                        reader.BaseStream.Position -= 2;
                        short numChoices = (short)(reader.ReadInt16() / 2);
                        for (int i = 0; i < numChoices; i++)
                        {
                            if (i != 0)
                                result.Add(',');
                            result.AddRange($"{ reader.ReadInt16() }");
                        }
                        break;
                    default:
                        break;
                }

                return result.ToArray();
            }
        }

        public class NLI1 : MSBTEntry
        {
            public List<Tuple<uint, int>> Entries = null;

            public override void Read(FileReader reader, Header header)
            {
                Entries = new List<Tuple<uint, int>>();

                Padding = reader.ReadBytes(8);
                EntryCount = reader.ReadUInt32();

                for (int i = 0; i < EntryCount; i++)
                {
                    uint MessageID = reader.ReadUInt32();
                    int MessageIndex = reader.ReadInt32();

                    Entries.Add(Tuple.Create(MessageID, MessageIndex));
                }
            }

            public override void Write(FileWriter writer, Header header)
            {
                writer.Write(Padding);
                writer.Write(Entries.Count);
                for (int i = 0; i < Entries.Count; i++)
                {
                   writer.Write(Entries[i].Item1); //MessageID
                   writer.Write(Entries[i].Item2); //MessageIndex
                }
            }
        }

        public class LBL1 : MSBTEntry
        {
            public List<LabelGroup> Groups = new List<LabelGroup>();
            public List<LabelEntry> Labels = new List<LabelEntry>();

            public override void Read(FileReader reader, Header header)
            {
                Padding = reader.ReadBytes(8);
                long pos = reader.Position;
                EntryCount = reader.ReadUInt32();

                for (int i = 0; i < EntryCount; i++)
                {
                    LabelGroup group = new LabelGroup();
                    group.NumberOfLabels = reader.ReadUInt32();
                    group.Offset = reader.ReadUInt32();
                    Groups.Add(group);
                }

                foreach (LabelGroup group in Groups)
                {
                    reader.Seek(pos + group.Offset, SeekOrigin.Begin);
                    for (int i = 0; i < group.NumberOfLabels; i++)
                    {
                        LabelEntry entry = new LabelEntry();
                        entry.Length = reader.ReadByte();
                        entry.Name = reader.ReadString((int)entry.Length);
                        entry.Index = reader.ReadUInt32();
                        entry.Checksum = (uint)Groups.IndexOf(group);
                        Labels.Add(entry);
                    }
                }

                reader.Align(8);
            }

            public override void Write(FileWriter writer, Header header)
            {
                writer.Seek(8);

                long pos = writer.Position;
                writer.Write(Groups.Count);
                for (int i = 0; i < Groups.Count; i++) {
                    writer.Write(Groups[i].NumberOfLabels);
                    writer.Write(uint.MaxValue);
                }

                int index = 0;
                for (int g = 0; g < Groups.Count; g++) {
                    writer.WriteUint32Offset(pos + 8 + (g * 8), pos);
                    for (int i = 0; i < Groups[g].NumberOfLabels; i++)
                    {
                        writer.Write((byte)Labels[index].Name.Length);
                        writer.WriteString(Labels[index].Name, Labels[index].Length);
                        writer.Write(Labels[index].Index);

                        index++;
                    }
                }
            }
        }

        public static void WriteSection(FileWriter writer, Header header, string magic, MSBTEntry section)
        {
            long startPos = writer.Position;
            writer.WriteSignature(magic);
            writer.Write(uint.MaxValue);
            section.Write(writer, header);
            long endPos = writer.Position;

            writer.AlignBytes(16, 0xAB);
            //Skip 20 bytes from the header
            writer.WriteSectionSizeU32(startPos + 4, startPos + 0x10, endPos);
        }

        public class MSBTEntry
        {
            public byte[] Data;
            public string Signature;
            public byte[] Padding = new byte[8];
            public uint EntryCount;

            public virtual void Read(FileReader reader, Header header)
            {

            }
            public virtual void Write(FileWriter writer, Header header)
            {
                writer.Write(Padding);
                writer.Write(Data);
            }
        }
    }
}
