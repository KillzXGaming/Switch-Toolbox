using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using FirstPlugin.Forms;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class MSBT : IEditor<MSBTEditor>, IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Message Binary Text" };
        public string[] Extension { get; set; } = new string[] { "*.msbt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
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
        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            header.Write(new FileWriter(mem));
            return mem.ToArray();
        }

        public class Header
        {
            public ushort ByteOrderMark;
            public ushort Padding;
            public ushort Unknown;

            public EncodingType StringEncoding;
            public byte Version;
            public List<MSBTEntry> entries = new List<MSBTEntry>();

            byte[] Reserved = new byte[10];

            public enum EncodingType : byte
            {
                UTF8 = 0,
                UTF16 = 1,
            }

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
                StringEncoding = reader.ReadEnum<EncodingType>(true);
                Version = reader.ReadByte();
                ushort SectionCount = reader.ReadUInt16();
                ushort Unknown = reader.ReadUInt16();
                uint FileSize = reader.ReadUInt32();
                Reserved = reader.ReadBytes(10);

                for (int i = 0; i < SectionCount; i++)
                {
                    long pos = reader.Position;

                    string Signature = reader.ReadString(4, Encoding.ASCII);
                    uint SectionSize = reader.ReadUInt32();

                    Console.WriteLine("Signature " + Signature);

                    switch (Signature)
                    {
                        //   Label1 = new LBL1();
                        //  Label1.Read(reader, this);
                        //  entries.Add(Label1);

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
                        case "ATR1":
                        case "ATO1":
                        case "TSY1":
                        case "LBL1":
                        default:
                            MSBTEntry entry = new MSBTEntry();
                            entry.Signature = Signature;
                            entry.Padding = reader.ReadBytes(8);
                            entry.EntryCount = reader.ReadUInt32();
                            entry.Data = reader.ReadBytes((int)SectionSize);
                            entries.Add(entry);
                            break;
                    }

                    while (reader.BaseStream.Position % 16 != 0 && reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        reader.ReadByte();
                    }
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
                writer.Write(StringEncoding, true);
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

        public class TXT2 : MSBTEntry
        {
            public uint[] Offsets;
            public List<string> TextData = new List<string>(); 

            public override void Read(FileReader reader, Header header)
            {
                Padding = reader.ReadBytes(8);

                long Position = reader.Position;
                EntryCount = reader.ReadUInt32();
                Offsets = reader.ReadUInt32s((int)EntryCount);

                for (int i = 0; i < EntryCount; i++)
                {
                    if (i == 0)

                    reader.Position = Offsets[i] + Position;
                    ReadMessageString(reader);
                }
            }

            private void ReadMessageString(FileReader reader)
            {
                List<char> chars = new List<char>();

                short charCheck = reader.ReadInt16(); 
                while (charCheck != 0)
                {
                    if (charCheck == 0x0E) //Control code
                    {
                        chars.AddRange(GetControlCode(reader));
                    }
                    else
                        chars.Add((char)charCheck);

                    charCheck = reader.ReadInt16();
                }

                TextData.Add(new string(chars.ToArray()));
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
            public List<Tuple<string, int>> Labels = null;

            public override void Read(FileReader reader, Header header)
            {
                Labels = new List<Tuple<string, int>>();

                Padding = reader.ReadBytes(8);
                EntryCount = reader.ReadUInt32();

                for (int i = 0; i < EntryCount; i++)
                {
                    uint LabelCount = reader.ReadUInt32();
                    uint Offset = reader.ReadUInt32();

                    using (reader.TemporarySeek(Offset))
                    {
                        for (int l = 0; l < LabelCount; l++)
                        {
                            byte stringSize = reader.ReadByte();
                            string Text = reader.ReadString(stringSize, Encoding.ASCII);
                            int index = reader.ReadInt32();
                            Labels.Add(Tuple.Create(Text, index));
                        }
                    }
                }
            }

            public override void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Signature);
                writer.Write(Data.Length);
                writer.Write(Padding);
            }
        }

        public class LabelEntry
        {

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
