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
    public class MSBT : IEditor<MSBTEditor>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Message;

        public bool CanSave { get; set; }
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

        public MSBTEditor OpenForm()
        {
            MSBTEditor editor = new MSBTEditor();
            editor.LoadMSBT(this);
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
            CanSave = false;

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

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(8, "MsgStdBn");
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                Padding = reader.ReadUInt16();
                byte encoding = reader.ReadByte();
                Version = reader.ReadByte();
                ushort SectionCount = reader.ReadUInt16();
                Unknown = reader.ReadUInt16();
                uint FileSize = reader.ReadUInt32();
                Reserved = reader.ReadBytes(10);

                StringEncoding = (encoding == 0x01 ? Encoding.BigEndianUnicode : Encoding.UTF8);

                for (int i = 0; i < SectionCount; i++)
                {
                    long pos = reader.Position;

                    string Signature = reader.ReadString(4, Encoding.ASCII);
                    uint SectionSize = reader.ReadUInt32();

                    Console.WriteLine("Signature " + Signature);

                    switch (Signature)
                    {
                        case "NLI1":
                            NLI1 = new NLI1();
                            NLI1.Read(reader, this);
                            entries.Add(NLI1);
                            break;
                        case "TXT2":
                            Text2 = new TXT2();
                            Text2.Read(reader, this);
                            entries.Add(Text2);
                            break;
                        case "LBL1":
                            Label1 = new LBL1();
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
                            entry.EntryCount = reader.ReadUInt32();
                            entry.Data = reader.ReadBytes((int)SectionSize);
                            entries.Add(entry);
                            break;
                    }

                    reader.SeekBegin(pos + SectionSize + 0x10);

                    while (reader.BaseStream.Position % 16 != 0 && reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        reader.ReadByte();
                    }
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
                if (ByteOrderMark == 0xFEFF)
                    writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                else
                    writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                writer.WriteSignature("MsgStdBn");
                writer.Write(ByteOrderMark);
                writer.Write(Padding);
                writer.Write(StringEncoding == Encoding.UTF8 ? (byte)0 : (byte)1);
                writer.Write(Version);
                writer.Write((ushort)entries.Count);
                writer.Write(Unknown);
                 
                long _ofsFileSize = writer.Position;
                writer.Write(0); //FileSize reserved for later
                writer.Write(Reserved);

                foreach (var entry in entries)
                    entry.Write(writer, this);

                //Write file size
                using (writer.TemporarySeek(_ofsFileSize, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
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

            public StringEntry(byte[] data)
            {
                Data = data;
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
        }

        public class TXT2 : MSBTEntry
        {
            public uint[] Offsets;
            public List<StringEntry> TextData = new List<StringEntry>();
            public List<StringEntry> OriginalTextData = new List<StringEntry>();

            public override void Read(FileReader reader, Header header)
            {
                Padding = reader.ReadBytes(8);

                long Position = reader.Position;
                EntryCount = reader.ReadUInt32();
                Offsets = reader.ReadUInt32s((int)EntryCount);

                for (int i = 0; i < EntryCount; i++)
                {
                    reader.Position = Offsets[i] + Position;
                    ReadMessageString(reader, (uint)i);
                }
            }

            private void ReadMessageString(FileReader reader, uint index)
            {
                List<byte> chars = new List<byte>();

                short charCheck = reader.ReadInt16();
                while (charCheck != 0)
                {
                    chars.Add((byte)(charCheck >> 8));
                    chars.Add((byte)(charCheck & 255));

                    charCheck = reader.ReadInt16();
                }

                TextData.Add(new StringEntry(chars.ToArray()) { Index = index, });
                OriginalTextData.Add(new StringEntry(chars.ToArray()) { Index = index, });
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
                writer.WriteSignature(Signature);
                writer.Write(Data.Length);
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

                        Console.WriteLine("label entry " + entry.Name);
                    }
                }

                reader.Align(8);
            }

            public override void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Signature);
                writer.Write(Data.Length);
                writer.Write(Padding);

                for (int i = 0; i < Groups.Count; i++)
                {

                }
            }
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
                writer.WriteSignature(Signature);
                writer.Write(Data.Length);
                writer.Write(Padding);
                writer.Write(EntryCount);
                writer.Write(Data);
                writer.Align(16);
            }
        }
    }
}
