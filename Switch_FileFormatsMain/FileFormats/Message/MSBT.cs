using System;
using System.Collections.Generic;
using System.Linq;
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
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream));
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public class Header
        {
            public ushort ByteOrderMark;
            public ushort Padding;
            public EncodingType StringEncoding;
            public byte Version;
            public List<MSBTEntry> entries = new List<MSBTEntry>();

            byte[] Reserved;

            public enum EncodingType : byte
            {
                UTF8 = 0,
                UTF16 = 1,
            }

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
                    byte[] Padding = reader.ReadBytes(8);
                    uint EntryCount = reader.ReadUInt32();

                    Console.WriteLine("Signature " + Signature);

                    switch (Signature)
                    {
                        case "LBL1":
                        case "NLI1":
                        case "ATR1":
                        case "TXT2":
                        case "ATO1":
                        case "TSY1":
                        default:
                            MSBTEntry entry = new MSBTEntry();
                            entry.Signature = Signature;
                            entry.Data = reader.ReadBytes((int)SectionSize + 8);
                            entries.Add(entry);
                            break;
                    }

                    reader.Seek(pos + SectionSize + 0x1C, System.IO.SeekOrigin.Begin);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("MsgStdBn");
                writer.Write(ByteOrderMark);
                writer.Write(Padding);
                writer.Write(StringEncoding, true);
                writer.Write(Version);

                long _ofsFileSize = writer.Position;
                writer.Write(0); //FileSize reserved for later
                writer.Write(Reserved);

                foreach (var entry in entries)
                    entry.Write(writer, this);

                //Write file size
                using (writer.TemporarySeek(_ofsFileSize, System.IO.SeekOrigin.Begin))
                {
                    writer.Write(writer.BaseStream.Length);
                }
            }
        }

        public class MSBTEntry
        {
            public byte[] Data;
            public string Signature;

            public virtual void Read(FileReader reader, Header header)
            {

            }
            public virtual void Write(FileWriter writer, Header header)
            {
                writer.Write(Signature);
                writer.Write(Data.Length);
                writer.Write(Data);
            }
        }
    }
}
