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
    public class RARC : IArchiveFile, IFileFormat, IDirectoryContainer, IContextMenuNode
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
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
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

        public void ClearFiles() { files.Clear(); }

        public string Name
        {
            get { return FileName; }
            set { FileName = value; }
        }

        private DirectoryEntry[] Directories;

        private uint HeaderSize = 32;
        private uint Unknown = 256;

        public RamAllocation RamType = RamAllocation.MRAM;

        public enum RamAllocation
        {
            None,
            ARAM,
            MRAM,
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            CanRenameFiles = true;
            CanReplaceFiles = true;

            files.Clear();

            using (var reader = new FileReader(stream))
            {
                _savedDirectories.Clear();

                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(4, "RARC");
                uint FileSize = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt32();
                uint DataOffset = reader.ReadUInt32();
                uint FileDataSize = reader.ReadUInt32();
                uint MRamSize = reader.ReadUInt32();
                uint ARamSize = reader.ReadUInt32();
                byte[] Padding = reader.ReadBytes(4);

                if (MRamSize != 0)
                    RamType |= RamAllocation.MRAM;
                else if (ARamSize != 0) 
                    RamType |= RamAllocation.ARAM;

                //Info Block
                long pos = reader.Position;

                uint DirectoryCount = reader.ReadUInt32();
                uint DirectoryOffset = reader.ReadUInt32() + (uint)pos;
                uint TotalNodeCount = reader.ReadUInt32();
                uint NodeOffset = reader.ReadUInt32() + (uint)pos;
                uint StringTableSize = reader.ReadUInt32();
                uint StringTablOffset = reader.ReadUInt32() + (uint)pos;
                ushort NodeCount = reader.ReadUInt16();
                Unknown = reader.ReadUInt16();
                byte[] Padding2 = reader.ReadBytes(4);

                Directories = new DirectoryEntry[DirectoryCount];
                for (int dir = 0; dir < DirectoryCount; dir++)
                    Directories[dir] = new DirectoryEntry(this);

                Console.WriteLine($"DirectoryCount {DirectoryCount}");
                Console.WriteLine($"StringTablOffset {StringTablOffset}");

                reader.SeekBegin(DirectoryOffset);
                for (int dir = 0; dir < DirectoryCount; dir++)
                {
                    Directories[dir].Read(reader);
                }

                for (int dir = 0; dir < DirectoryCount; dir++)
                {
                    uint NamePointer = StringTablOffset + Directories[dir].NameOffset;
                    Directories[dir].Name = ReadStringAtTable(reader, NamePointer);

                    Console.WriteLine($"{  Directories[dir].Name } {dir}");

                    for (int n = 0; n < Directories[dir].NodeCount; n++)
                    {
                        reader.SeekBegin(NodeOffset + ((n + Directories[dir].FirstNodeIndex) * 0x14));
                        FileEntry entry = new FileEntry();
                        entry.Read(reader);
                        NamePointer = StringTablOffset + entry.NameOffset;
                        entry.Name = ReadStringAtTable(reader, NamePointer);

                        //Root and parent strings. Skip these unecessary nodes.
                        if (entry.Name == "." || entry.Name == "..")
                            continue;

                        if (entry.IsDirectory)
                        {
                         //   Directories[entry.Offset].ID = entry.FileId; //0xFFFF or 0
                            Directories[dir].AddNode(Directories[entry.Offset]);
                            _savedDirectories.Add(entry);
                        }
                        else
                        {
                            using (reader.TemporarySeek(pos + DataOffset + entry.Offset, System.IO.SeekOrigin.Begin))
                            {
                                entry.FileData = reader.ReadBytes((int)entry.Size);
                            }
                            entry.FileName = entry.Name;
                            files.Add(entry);

                            Directories[dir].AddNode(entry);
                        }
                    }
                }

                this.Name = Directories[0].Name;
                nodes.AddRange(Directories[0].Nodes);
            }
        }

        private void LoadAllDirectoriesAndFiles(DirectoryEntry parentDir)
        {
            for (int i = 0; i < parentDir.Children.Count; i++)
            {
                if (parentDir.Children[i] is DirectoryEntry)
                {
                   //_savedDirectories.Add((DirectoryEntry)parentDir.nodes[i]);
                }
                else
                    _savedFiles.Add((FileEntry)parentDir.Children[i]);
            }

            for (int i = 0; i < parentDir.Children.Count; i++)
            {
                if (parentDir.Children[i] is DirectoryEntry)
                    LoadAllDirectoriesAndFiles((DirectoryEntry)parentDir.Children[i]);
            }
        }

        //Save a string table to a char array like WArchive 
        //It's a good idea, and i don't want to deal with offset linking
        private List<byte[]> _savedFileData;
        private List<FileEntry> _savedFiles = new List<FileEntry>();
        private List<FileEntry> _savedDirectories = new List<FileEntry>();
        private List<DirectoryEntry> _savedNodes = new List<DirectoryEntry>();

        private void SaveFile(FileWriter writer)
        {
            _savedFiles.Clear();
            _savedFileData = new List<byte[]>();
            _savedNodes = Directories.ToList();

            Console.WriteLine("LoadAllDirectoriesAndFiles");

            LoadAllDirectoriesAndFiles(Directories[0]);
            var stringTable = CreateStringTable(Directories[0]);

            long pos = writer.Position;

            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            writer.WriteSignature("RARC");
            writer.Write(uint.MaxValue); //FileSize
            writer.Write(HeaderSize);
            writer.Write(uint.MaxValue); //DataOffset
            writer.Write(uint.MaxValue); //File Size
            writer.Write(0); //MRAM (saved later if used)
            writer.Write(0); //ARAM (saved later if used)
            writer.Seek(4); //padding

            writer.SeekBegin(HeaderSize);
            long InfoPos = writer.Position;

            writer.Write(_savedNodes.Count);
            writer.Write(uint.MaxValue); //DirectoryOffset
            writer.Write(_savedNodes.Sum(x => (x.Children.Count + 2)));
            writer.Write(uint.MaxValue); //File Node Offset
            writer.Write(uint.MaxValue); //String pool size
            writer.Write(uint.MaxValue); //String pool offset
            writer.Write((ushort)(_savedNodes.Sum(x => (x.Children.Count + 2))));
            writer.Write((ushort)Unknown);
            writer.Write(0); //padding

            int startIndex = 0;

            //Write nodes
            WriteOffset(writer, 4, InfoPos);
            for (int dir = 0; dir < _savedNodes.Count; dir++)
            {
                writer.Write(_savedNodes[dir].Identifier);
                writer.Write(_savedNodes[dir].NameOffset);
                writer.Write(_savedNodes[dir].Hash);
                writer.Write((ushort)(_savedNodes[dir].Children.Count + 2));
                writer.Write(startIndex);

                startIndex += _savedNodes[dir].Children.Count + 2;
            }

            foreach (FileEntry entry in Files)
                entry.SaveFileFormat();

            writer.Align(32);

            Console.WriteLine("Directories");

            //Write the directories and files
            WriteOffset(writer, 12, InfoPos);
            for (int dir = 0; dir < _savedNodes.Count; dir++)
            {
                for (int n = 0; n < _savedNodes[dir].Children.Count; n++)
                {
                    var entry = _savedNodes[dir].Children[n];
                    if (entry is DirectoryEntry)
                    {
                        var dirEntry = (DirectoryEntry)_savedNodes[dir].Children[n];
                        int index = Directories.ToList().IndexOf(dirEntry);

                        writer.Write(dirEntry.ID);
                        writer.Write(dirEntry.Hash);
                        writer.Write((byte)0x2);
                        writer.Write((byte)0);
                        writer.Write((ushort)dirEntry.NameOffset);
                        writer.Write((int)index);
                        writer.Write((int)16);
                        writer.Write((int)0);
                    }
                    else
                    {
                        var fileEntry = (FileEntry)entry;
                        writer.Write(fileEntry.FileId);
                        writer.Write(fileEntry.Hash);
                        writer.Write(fileEntry.Flags);
                        writer.Write((byte)0); //Padding
                        writer.Write((ushort)fileEntry.NameOffset);

                        fileEntry._dataOffsetPos = writer.Position;
                        writer.Write(0);
                        writer.Write((uint)fileEntry.FileData.Length);
                        _savedFileData.Add(fileEntry.FileData);
                        writer.Write(0);
                    }
                }

                //Save parent and root nodes at end
                writer.Write((ushort)0);
                writer.Write((ushort)46);
                writer.Write((byte)2);
                writer.Write((byte)0);
                writer.Write((ushort)0);
                writer.Write(dir);
                writer.Write(16);
                writer.Write(0);

                int rootIndex = Directories.ToList().IndexOf((DirectoryEntry)_savedNodes[dir].Parent);

                writer.Write((ushort)0);
                writer.Write((ushort)184);
                writer.Write((byte)2);
                writer.Write((byte)0);
                writer.Write((ushort)2);
                writer.Write(rootIndex);
                writer.Write(16);
                writer.Write(0);
            }
            writer.Align(32);


            long stringTablePos = writer.Position;
            WriteOffset(writer, 20, InfoPos);
            foreach (var str in stringTable)
            {
                writer.WriteString(str);
            }
            writer.Align(32);
            long stringTableSize = writer.Position - stringTablePos;

            var dataPos = writer.Position;
            using (writer.TemporarySeek(pos + 12, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)(dataPos - InfoPos));
            }

            uint dataOffset = 0;
            SaveFileData(writer, ref dataOffset, Directories[0]);
            uint DataSize = (uint)(writer.Position - dataPos);

            writer.Align(32);

            uint EndFileSize = (uint)(writer.Position - dataPos);

            //Write data size
            using (writer.TemporarySeek(pos + 16, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)DataSize);
            }

            //Write ram size
            if (RamType != RamAllocation.None)
            {
                using (writer.TemporarySeek(pos + (RamType.HasFlag(RamAllocation.MRAM) ? 20 : 24), System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)EndFileSize);
                }
            }

            //Write file size
            using (writer.TemporarySeek(pos + 0x4, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)writer.BaseStream.Length);
            }

            //Write string table size
            using (writer.TemporarySeek(InfoPos + 16, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)stringTableSize);
            }
        }

        //Generates a string table 
        private List<string> CreateStringTable(DirectoryEntry parentDir)
        {
            //Strings are ordered recusively through directories
            List<string> stringList = new List<string>();
            //Add a root string
            stringList.Add(".");
            //Then add a parent string after it
            stringList.Add("..");

            uint stringPos = 5;
            CreateStringTable(stringList, ref stringPos, parentDir);
            return stringList;
        }

        private void CreateStringTable(List<string> stringList,
                   ref uint stringPos, DirectoryEntry parentDir)
        {
            if (!stringList.Contains(parentDir.Name))
            {
                parentDir.NameOffset = stringPos;
                stringList.Add(parentDir.Name);
                stringPos += (uint)parentDir.Name.Length + 1;
            }
            for (int i = 0; i < parentDir.Children.Count; i++)
            {
                if (parentDir.Children[i] is FileEntry)
                    ((FileEntry)parentDir.Children[i]).NameOffset = (ushort)stringPos;
                else
                    ((DirectoryEntry)parentDir.Children[i]).NameOffset = (ushort)stringPos;

                Console.WriteLine($"{parentDir.Children[i].Name} {stringPos}");

                stringList.Add(parentDir.Children[i].Name);
                stringPos += (uint)parentDir.Children[i].Name.Length + 1;

                if (parentDir.Children[i] is DirectoryEntry)
                    CreateStringTable(stringList, ref stringPos, (DirectoryEntry)parentDir.Children[i]);
            }
        }

        //Saves data in recusive order
        private void SaveFileData(FileWriter writer, ref uint dataPos, DirectoryEntry parentDir)
        {
            for (int i = 0; i < parentDir.Children.Count; i++)
            {
                if (parentDir.Children[i] is FileEntry)
                {
                    var entry = (FileEntry)parentDir.Children[i];
                    long pos = writer.Position;
                    using (writer.TemporarySeek(entry._dataOffsetPos, System.IO.SeekOrigin.Begin))
                    {
                        writer.Write((uint)dataPos);
                    }
                    writer.Write(entry.FileData);
                    writer.AlignBytes(32);

                    dataPos += (uint)(writer.Position - pos);
                }
                if (parentDir.Children[i] is DirectoryEntry)
                    SaveFileData(writer, ref dataPos, (DirectoryEntry)parentDir.Children[i]);
            }
        }

        private void WriteOffset(FileWriter writer, long Target, long RelativePosition)
        {
            long Position = writer.Position;
            using (writer.TemporarySeek(RelativePosition + Target, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)(Position - RelativePosition));
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
            public uint Identifier;
            internal uint NameOffset; //Relative to string table
            public ushort Hash { get; set; }
            public ushort NodeCount;
            public uint FirstNodeIndex { get; set; }

            public ushort ID { get; set; } = 0xFFFF;

            public DirectoryEntry(RARC rarc) { ParentArchive = rarc; }

            public IEnumerable<INode> Nodes { get { return Children; } }
            public List<INode> Children = new List<INode>();

            public INode Parent { get; set; }

            public void AddNode(INode node)
            {
                if (node is FileEntry)
                    ((FileEntry)node).Parent = this;
                else
                    ((DirectoryEntry)node).Parent = this;

                Children.Add(node);
            }

            public void Read(FileReader reader)
            {
                Identifier = reader.ReadUInt32();
                NameOffset = reader.ReadUInt32();
                Hash = reader.ReadUInt16();
                NodeCount = reader.ReadUInt16();
                FirstNodeIndex = reader.ReadUInt32();
            }

            public void UpdateHash() {
                Hash = CalculateHash(Name);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            SaveFile(new FileWriter(stream));
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
            //According to this to determine directory or not 
            //https://github.com/LordNed/WArchive-Tools/blob/3c7fdefe54b4c7634a042847b7455de61705033f/ArchiveToolsLib/Archive/Archive.cs#L44
            public bool IsDirectory { get { return (Flags & 2) >> 1 == 1; } }

            public ushort FileId { get; set; }
            public ushort Hash { get; set; }
            public byte Flags { get; set; }

            internal uint Size;
            internal uint Offset;
            internal ushort NameOffset;

            internal long _dataOffsetPos;

            public INode Parent { get; set; }

            public void Read(FileReader reader)
            {
                FileId = reader.ReadUInt16();
                Hash = reader.ReadUInt16();
                Flags = reader.ReadByte();
                reader.Seek(1); //Padding
                NameOffset = reader.ReadUInt16();
                Offset = reader.ReadUInt32();
                Size = reader.ReadUInt32();
            }
        }
    }
}
