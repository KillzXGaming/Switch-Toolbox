using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class RARC : IArchiveFile, IFileFormat, IDirectoryContainer
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "RARC" };
        public string[] Extension { get; set; } = new string[] { "*.rarc", "*.arc", "*.yaz0" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "RARC");
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

        public List<FileEntry> files = new List<FileEntry>();
        public List<INode> nodes = new List<INode>();

        public IEnumerable<ArchiveFileInfo> Files => files;
        public IEnumerable<INode> Nodes => nodes;

        public string Name
        {
            get { return FileName; }
            set { FileName = value; }
        }

        private DirectoryEntry[] Directories;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(4, "RARC");
                uint FileSize = reader.ReadUInt32();
                uint HeaderSize = reader.ReadUInt32();
                uint DataOffset = reader.ReadUInt32();
                uint FileDataSize = reader.ReadUInt32();
                uint EndOfFileOffset = reader.ReadUInt32();
                byte[] Padding = reader.ReadBytes(8);


                //Info Block
                long pos = reader.Position;

                uint DirectoryCount = reader.ReadUInt32();
                uint DirectoryOffset = reader.ReadUInt32();
                uint TotalNodeCount = reader.ReadUInt32();
                uint NodeOffset = reader.ReadUInt32() + (uint)pos;
                uint StringTableSize = reader.ReadUInt32();
                uint StringTablOffset = reader.ReadUInt32() + (uint)pos;
                ushort NodeCount = reader.ReadUInt16();
                ushort Unknown = reader.ReadUInt16();
                byte[] Padding2 = reader.ReadBytes(4);

                Directories = new DirectoryEntry[DirectoryCount];
                for (int dir = 0; dir < DirectoryCount; dir++)
                    Directories[dir] = new DirectoryEntry(this);

                Console.WriteLine($"DirectoryCount {DirectoryCount}");

                reader.SeekBegin(DirectoryOffset + pos);
                for (int dir = 0; dir < DirectoryCount; dir++)
                {
                    Directories[dir].Read(reader);
                    uint NamePointer = StringTablOffset + Directories[dir].NameOffset;
                    Directories[dir].Name = ReadStringAtTable(reader, NamePointer);

                    Console.WriteLine($"DirectoryCount {DirectoryCount}");

                    long EndDirectoryPos = reader.Position;
                    for (int n = 0; n < Directories[dir].NodeCount; n++)
                    {
                        reader.SeekBegin(NodeOffset + ((n + Directories[dir].FirstNodeIndex) * 0x14));
                        FileEntry entry = new FileEntry();
                        entry.Read(reader);
                        NamePointer = StringTablOffset + entry.NameOffset;
                        entry.Name = ReadStringAtTable(reader, NamePointer);

                        if (entry.FileId != 0xFFFF)
                        {
                            using (reader.TemporarySeek(pos + DataOffset + entry.Offset, System.IO.SeekOrigin.Begin))
                            {
                                entry.FileData = reader.ReadBytes((int)entry.Size);
                            }
                            entry.FileName = entry.Name;

                            Directories[dir].AddNode(entry);
                        }
                        else
                            Directories[dir].AddNode(Directories[entry.Offset]);
                    }
                    reader.SeekBegin(EndDirectoryPos);
                }

                this.Name = Directories[0].Name;
                nodes.AddRange(Directories[0].Nodes);
            }
        }

        private string ReadStringAtTable(FileReader reader,  uint NameOffset)
        {
            using (reader.TemporarySeek(NameOffset, System.IO.SeekOrigin.Begin))
            {
                return reader.ReadZeroTerminatedString();
            }
        }

        private static ushort CalculateHash(string Name)
        {
            ushort Hash = 0;
            for (int i = 0; i < Name.Length; i++)
            {
                Hash *= 3;
                Hash += Name[i];
            }
            return Hash;
        }

        private void CreateDirectoryEntry()
        {
            
        }

        public class DirectoryEntry : IDirectoryContainer
        {
            public RARC ParentArchive { get; }

            public string Name { get; set; }

            private uint Identifier;

            internal uint NameOffset; //Relative to string table

            public ushort Hash { get; set; }

            public ushort NodeCount;

            public uint FirstNodeIndex { get; set; }

            public DirectoryEntry(RARC rarc) { ParentArchive = rarc; }

            public IEnumerable<INode> Nodes { get { return nodes; } }
            public List<INode> nodes = new List<INode>();

            public void AddNode(INode node)
            {
                nodes.Add(node);
            }

            public void Read(FileReader reader)
            {
                Identifier = reader.ReadUInt32();
                NameOffset = reader.ReadUInt32();
                Hash = reader.ReadUInt16();
                NodeCount = reader.ReadUInt16();
                FirstNodeIndex = reader.ReadUInt32();
            }

            public void Write(FileWriter writer)
            {
                Hash = CalculateHash(Name);

                writer.Write(Identifier);
                writer.Write(NameOffset);
                writer.Write(Hash);
                writer.Write(NodeCount);
                writer.Write(FirstNodeIndex);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public class FileEntry : ArchiveFileInfo
        {
            public ushort FileId { get; set; }
            public uint Unknown { get; set; }

            internal uint Size;
            internal uint Offset;
            internal ushort NameOffset;

            public void Read(FileReader reader)
            {
                FileId = reader.ReadUInt16();
                uint Unknown = reader.ReadUInt32();
                NameOffset = reader.ReadUInt16();
                Offset = reader.ReadUInt32();
                Size = reader.ReadUInt32();
            }
        }
    }
}
