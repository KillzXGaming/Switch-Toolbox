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
    public class MSBP : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Message;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Message Studio Binary Project" };
        public string[] Extension { get; set; } = new string[] { "*.msbp" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(8, "MsgPrjBn");
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

        public Header header;
        public void Load(System.IO.Stream stream)
        {
            CanSave = false;
            Text = FileName;

            header = new Header();
            header.Read(new FileReader(stream));

            TreeNode clr1Node = new TreeNode("Colors");
            TreeNode ati2Node = new TreeNode("Attributes");
            TreeNode tgg2Node = new TreeNode("Tag Groups");
            TreeNode syl3Node = new TreeNode("Styles");
            TreeNode cti1Node = new TreeNode("Project Contents");

            for (int i = 0; i < header.entries.Count; i++)
            {
                var node = new TreeNode(header.entries[i].Signature);

                if (header.entries[i] is CTI1)
                {
                    CTI1 cti1 = header.entries[i] as CTI1;

                    for (int t = 0; t < cti1.TextEntries.Count; t++)
                    {
                        cti1Node.Nodes.Add(cti1.TextEntries[t]);
                    }
                }
                else
                    Nodes.Add(node);
            }

            if (clr1Node.Nodes.Count > 0) Nodes.Add(clr1Node);
            if (ati2Node.Nodes.Count > 0) Nodes.Add(ati2Node);
            if (tgg2Node.Nodes.Count > 0) Nodes.Add(tgg2Node);
            if (syl3Node.Nodes.Count > 0) Nodes.Add(syl3Node);
            if (cti1Node.Nodes.Count > 0) Nodes.Add(cti1Node);
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
            public uint Version;

            public Encoding StringEncoding = Encoding.Unicode;

            public List<MSBPEntry> entries = new List<MSBPEntry>();

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(8, "MsgPrjBn");
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                reader.ReadUInt16(); //Padding
                byte encoding = reader.ReadByte();
                Version = reader.ReadByte();
                ushort SectionCount = reader.ReadUInt16();
                reader.ReadUInt16(); //Padding
                uint FileSize = reader.ReadUInt32();
                reader.ReadBytes(10);  //Reserved

                StringEncoding = (encoding == 0x01 ? Encoding.BigEndianUnicode : Encoding.UTF8);



                for (int i = 0; i < SectionCount; i++)
                {
                    long pos = reader.Position;

                    string Signature = reader.ReadString(4, Encoding.ASCII);
                    uint SectionSize = reader.ReadUInt32();
                    reader.ReadUInt32(); //padding
                    reader.ReadUInt32(); //padding

                    MSBPEntry entry = new MSBPEntry();

                    switch (Signature)
                    {
                        case "SLB1":
                            entry = new SLB1();
                            entry.Read(reader, this);
                            entries.Add(entry);
                            break;
                        case "CTI1":
                            entry = new CTI1();
                            entry.Read(reader, this);
                            entries.Add(entry);
                            break;
                        default:
                            entries.Add(entry);
                            break;
                    }

                    entry.Signature = Signature;
                    entry.Data = reader.getSection((int)pos, (int)SectionSize + 0x10);

                    reader.SeekBegin(pos + SectionSize + 0x10);

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

                writer.WriteSignature("MsgPrjBn");
                writer.Write(ByteOrderMark);
                writer.Write((ushort)0);
                writer.Write(StringEncoding == Encoding.UTF8 ? (byte)0 : (byte)1);
                writer.Write(Version);
                writer.Write((ushort)entries.Count);
                writer.Write((ushort)0);

                long _ofsFileSize = writer.Position;
                writer.Write(0); //FileSize reserved for later
                writer.Seek(10);
            }
        }

        public class SLB1 : MSBPEntry
        {
            public List<SLB1Entry> TextEntries = new List<SLB1Entry>();

            public override void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                uint entryCount = reader.ReadUInt32();

                uint[] Offsets = new uint[entryCount];
                uint[] TextCounts = new uint[entryCount];

                for (int i = 0; i < entryCount; i++)
                {
                    TextCounts[i] = reader.ReadUInt32();
                    Offsets[i] = reader.ReadUInt32();
                }

                for (int i = 0; i < entryCount; i++)
                {
                    reader.SeekBegin(pos + Offsets[i]);
                    for (int t = 0; t < TextCounts[i]; t++)
                        TextEntries.Add(new SLB1Entry(reader));
                }
            }
        }

        public class SLB1Entry 
        {
            public string Text { get; set; }
            public ushort Unknown { get; set; }

            public SLB1Entry(FileReader reader)
            {
                Text = reader.ReadZeroTerminatedString();
                Unknown = reader.ReadUInt16();
            }
        }

        public class CTI1 : MSBPEntry
        {
            public List<string> TextEntries = new List<string>();

            public override void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                uint entryCount = reader.ReadUInt32();

                uint[] Offsets = reader.ReadUInt32s((int)entryCount);
                for (int i =0; i < entryCount; i++)
                {
                    reader.SeekBegin(pos + Offsets[i]);
                    TextEntries.Add(reader.ReadZeroTerminatedString());
                }
            }
        }

        public class MSBPEntry
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
