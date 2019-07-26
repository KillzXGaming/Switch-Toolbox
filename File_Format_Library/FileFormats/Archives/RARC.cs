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

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(4, "RARC");
                uint FileSize = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt32();
                uint DataOffset = reader.ReadUInt32();
                uint FileDataSize = reader.ReadUInt32();
                uint EndOfFileOffset = reader.ReadUInt32();
                byte[] Padding = reader.ReadBytes(8);


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

                        //These make it crash so just skip them
                        //Unsure what purpose these have
                        if (entry.Name == "." || entry.Name == "..")
                            continue;

                        if (entry.IsDirectory)
                        {
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

                            Directories[dir].AddNode(entry);
                        }
                    }
                }

                this.Name = Directories[0].Name;
                nodes.AddRange(Directories[0].Nodes);
            }
        }

        private List<FileEntry> _savedFiles = new List<FileEntry>();
        private List<FileEntry> _savedDirectories = new List<FileEntry>();
        private List<DirectoryEntry> _savedNodes = new List<DirectoryEntry>();

        private void LoadAllDirectoriesAndFiles(DirectoryEntry parentDir)
        {
            for (int i = 0; i < parentDir.nodes.Count; i++)
            {
                if (parentDir.nodes[i] is DirectoryEntry)
                {
                   //_savedDirectories.Add((DirectoryEntry)parentDir.nodes[i]);
                }
                else
                    _savedFiles.Add((FileEntry)parentDir.nodes[i]);
            }

            for (int i = 0; i < parentDir.nodes.Count; i++)
            {
                if (parentDir.nodes[i] is DirectoryEntry)
                    LoadAllDirectoriesAndFiles((DirectoryEntry)parentDir.nodes[i]);
            }
        }

        //Save a string table to a char array like WArchive 
        //It's a good idea, and i don't want to deal with offset linking
        private List<char> _exportStringTable;
        private List<byte[]> _savedFileData;
        public void SaveFile(FileWriter writer)
        {
            _exportStringTable = new List<char>();
            _savedFileData = new List<byte[]>();
            _savedNodes = Directories.ToList();

            LoadAllDirectoriesAndFiles(Directories[0]);

            _exportStringTable = new List<char>();
            _exportStringTable.AddRange(".\0".ToCharArray());
            _exportStringTable.AddRange("..\0".ToCharArray());

            long pos = writer.Position;

            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            writer.WriteSignature("RARC");
            writer.Write(uint.MaxValue); //FileSize
            writer.Write(HeaderSize);
            writer.Write(uint.MaxValue); //DataOffset
            writer.Write(uint.MaxValue); //File Size
            writer.Write(uint.MaxValue); //End of file
            writer.Seek(8); //padding

            writer.SeekBegin(HeaderSize);
            long InfoPos = writer.Position;

            writer.Write(_savedNodes.Count); 
            writer.Write(uint.MaxValue); //DirectoryOffset
            writer.Write(_savedDirectories.Count + _savedFiles.Count);
            writer.Write(uint.MaxValue); //File Node Offset
            writer.Write(uint.MaxValue); //String pool size
            writer.Write(uint.MaxValue); //String pool offset
            writer.Write((ushort)0);
            writer.Write((ushort)Unknown);
            writer.Write(0); //padding

            List<FileEntry> TotalList = _savedDirectories.Concat(_savedFiles).ToList();

            //Write nodes
            WriteOffset(writer, 4, InfoPos);
            for (int dir = 0; dir < _savedNodes.Count; dir++)
            {
                writer.Write(_savedNodes[dir].Identifier);
                writer.Write((int)_exportStringTable.Count);
                writer.Write(_savedNodes[dir].Hash);
                writer.Write((ushort)(_savedNodes[dir].nodes.Count));
                writer.Write(TotalList.FindIndex(i => i.Name == _savedNodes[dir].nodes[0].Name));
                _exportStringTable.AddRange(_savedNodes[dir].Name.ToCharArray());
                _exportStringTable.Add('\0'); //Null terminated
            }

            writer.Align(32);

            //Write the directories and files
            WriteOffset(writer, 12, InfoPos);
            foreach (FileEntry entry in TotalList)
            {
                writer.Write(entry.FileId);
                writer.Write(entry.Hash);
                writer.Write(entry.Flags);
                writer.Seek(1); //Padding

                if ((entry.Name == "."))
                {
                    writer.Write((ushort)0);
                }
                else if ((entry.Name == ".."))
                {
                    writer.Write((ushort)2);
                }
                else
                {
                    // Offset of name in the string table
                    writer.Write((ushort)_exportStringTable.Count);

                    // Add name to string table
                    _exportStringTable.AddRange(entry.Name.ToCharArray());

                    // Strings must be null terminated
                    _exportStringTable.Add('\0');

             /*     if (_savedFiles.Find(i => i.Name == entry.Name) != null)
                    {
                        string test = new string(_exportStringTable.ToArray());

                        int testt = test.IndexOf(entry.Name);

                        writer.Write((ushort)test.IndexOf(entry.Name));
                    }

                    else
                    {
                
                    }*/
                }

                if (entry.Flags == 0x02)
                {
                    writer.Write((int)TotalList.IndexOf(entry) + 1);
                    writer.Write((int)0); 
                }
                else
                {
                    uint dataOffset = 0;
                    foreach (var data in _savedFileData)
                        dataOffset += (uint)data.Length;

                    writer.Write(dataOffset);
                    writer.Write(entry.FileData.Length);
                    _savedFileData.Add(entry.FileData);
                }

                writer.Write(0);
            }

            writer.Align(32);

            WriteOffset(writer, 20, InfoPos);
            writer.Write(_exportStringTable.ToArray());

            writer.Align(32);


            var dataPos = writer.Position;
            using (writer.TemporarySeek(pos + 12, System.IO.SeekOrigin.Begin)) {
                writer.Write((uint)(dataPos - InfoPos));
            }

            foreach (var data in _savedFileData)
                writer.Write(data);

            uint DataSize = (uint)(writer.Position - dataPos);

            writer.Align(32);

            uint EndFileSize = (uint)(writer.Position - dataPos);

            //Write data size
            using (writer.TemporarySeek(pos + 16, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)DataSize);
            }

            //Write end of file size
            using (writer.TemporarySeek(pos + 20, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)EndFileSize);
            }

            //Write string table size
            using (writer.TemporarySeek(pos + 0x4, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)writer.BaseStream.Length);
            }

            //Write file size
            using (writer.TemporarySeek(InfoPos + 16, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)_exportStringTable.Count);
            }
        }

        private void WriteDirectories()
        {

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

            public void UpdateHash() {
                Hash = CalculateHash(Name);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            SaveFile(new FileWriter(mem));
            return mem.ToArray();
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

            internal long _positionPtr;
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
