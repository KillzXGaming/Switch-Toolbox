using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class BFSAR : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Audio;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Sound Archive" };
        public string[] Extension { get; set; } = new string[] { "*.bfsar" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FSAR");
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

        public void Load(System.IO.Stream stream)
        {
            FSAR bfsar = new FSAR();
            bfsar.Read(new FileReader(stream));

            Text = FileName;
            Nodes.Add("Audio List");
            Nodes.Add("Audio Set List");
            Nodes.Add("Bank List");
            Nodes.Add("Group List");
            Nodes.Add("Players List");
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        private static uint FileSizeOffset;
        public class FSAR
        {
            public uint Size;
            private ushort BOM;
            public ushort HeaderSize;
            public uint Version;
            public ushort SectionCount;
            public STRG STRG;
            public INFO INFO;
            public FILE FILE;

            public void Read(FileReader reader)
            {
                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != "FSAR")
                    throw new Exception($"Invalid signature {Signature}! Expected FSAR.");
                BOM = reader.ReadUInt16();
                HeaderSize = reader.ReadUInt16();
                Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                SectionCount = reader.ReadUInt16();
                ushort padding = reader.ReadUInt16();

                for (int i = 0; i < SectionCount; i++)
                {
                    ushort Identifier = reader.ReadUInt16();
                    ushort padding2 = reader.ReadUInt16();
                    uint Offset = reader.ReadUInt32();
                    uint Size = reader.ReadUInt32();

                    if (Identifier == 8192)
                    {
                        reader.Seek(Offset, SeekOrigin.Begin);
                        STRG = new STRG();
                        STRG.Read(reader);
                    }
                    if (Identifier == 8193)
                    {
                        reader.Seek(Offset, SeekOrigin.Begin);
                        INFO = new INFO();
                        INFO.Read(reader);
                    }
                    if (Identifier == 8194)
                    {
                        reader.Seek(Offset, SeekOrigin.Begin);
                        FILE = new FILE();
                        FILE.Read(reader, INFO);
                    }
                }
            }
            public void Write(FileWriter writer)
            {
                writer.WriteSignature("FSAR");
                writer.Write(BOM);
                writer.Write(HeaderSize);
                writer.Write(Version);
                FileSizeOffset = (uint)writer.Position;
                writer.Write(SectionCount);
                writer.Write((ushort)0);
                for (int i = 0; i < SectionCount; i++)
                {

                }
            }
        }
        public class INFO
        {
            public uint SectionSize;
            public AudioList audioList;

            public void Read(FileReader reader)
            {
                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != "INFO")
                    throw new Exception($"Invalid signature {Signature}! Expected INFO.");

                SectionSize = reader.ReadUInt32();
                long Pos = reader.Position;
                uint AudioListOffset = reader.ReadUInt32();

                if (AudioListOffset != 0)
                {
                    using (reader.TemporarySeek(AudioListOffset + Pos, SeekOrigin.Begin))
                    {
                        audioList = new AudioList();
                        audioList.Read(reader);
                    }
                }
            }
        }
        public class AudioList
        {
            public void Read(FileReader reader)
            {

            }
        }
        public class FILE
        {
            public uint SectionSize;
            public void Read(FileReader reader, INFO INFO)
            {
                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != "FILE")
                    throw new Exception($"Invalid signature {Signature}! Expected FILE.");

                SectionSize = reader.ReadUInt32();
                byte[] padding = reader.ReadBytes(0x18);


            }
        }
        public class STRG
        {
            public uint Size;
            private uint BOM;
            public uint SectionSize;
            public uint Version;
            public ushort SectionCount;
            public StringTable stringTable;
            public LookupTable lookupTable;

            public void Read(FileReader reader)
            {
                string Signature = reader.ReadString(4, Encoding.ASCII);
                if (Signature != "STRG")
                    throw new Exception($"Invalid signature {Signature}! Expected STRG.");

                SectionSize = reader.ReadUInt32();
                long pos = reader.Position;

                uint unk = reader.ReadUInt32();
                uint Stringtableoffset = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint LookupTableOffset = reader.ReadUInt32();

                if (Stringtableoffset != 0)
                {
                    reader.Seek(Stringtableoffset + pos, SeekOrigin.Begin);
                    stringTable = new StringTable();
                    stringTable.Read(reader);
                }
                if (LookupTableOffset != 0)
                {
                    reader.Seek(LookupTableOffset + pos, SeekOrigin.Begin);
                    lookupTable = new LookupTable();
                    lookupTable.Read(reader);
                }
            }
            public class StringTable
            {
                public List<string> Names = new List<string>();

                public void Read(FileReader reader)
                {
                    long pos = reader.Position;
                    uint Count = reader.ReadUInt32();

                    for (int i = 0; i < Count; i++)
                    {
                        uint unk = reader.ReadUInt32();
                        uint Offset = reader.ReadUInt32();
                        uint Size = reader.ReadUInt32();

                        using (reader.TemporarySeek(Offset + pos, SeekOrigin.Begin))
                        {
                            Names.Add(reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated));
                        }
                    }
                    foreach (string name in Names)
                        Console.WriteLine(name);
                }
            }
            public class LookupTable
            {
                public void Read(FileReader reader)
                {

                }
            }
        }
    }
}
