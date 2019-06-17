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
    public class RARC : IArchiveFile, IFileFormat
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

        public IEnumerable<ArchiveFileInfo> Files => files;

        private DirectoryEntry[] Directories;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
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
                uint NodeOffset = reader.ReadUInt32();
                uint StringTableSize = reader.ReadUInt32();
                uint StringTablOffset = reader.ReadUInt32();
                ushort NodeCount = reader.ReadUInt16();
                ushort Unknown = reader.ReadUInt16();
                byte[] Padding2 = reader.ReadBytes(4);

                Directories = new DirectoryEntry[DirectoryCount];

                reader.SeekBegin(DirectoryOffset + pos);
                for (int dir = 0; dir < DirectoryCount; dir++)
                {
                    Directories[dir] = new DirectoryEntry(this);
                    Directories[dir].Read(reader);
                    uint NamePointer = StringTablOffset + (uint)pos + Directories[dir].NameOffset;
                    Directories[dir].Name = ReadStringAtTable(reader, NamePointer);
                }



                reader.SeekBegin(NodeOffset + pos);
                for (int n = 0; n < TotalNodeCount; n++)
                {
                    files.Add(new FileEntry());
                    {

                    }
                }
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

        public class DirectoryEntry
        {
            public RARC ParentArchive { get; }

            public string Name { get; set; }

            private uint Identifier;

            internal uint NameOffset; //Relative to string table

            public ushort Hash { get; set; }

            private ushort NodeCount;

            public uint FirstNodeIndex { get; set; }

            public DirectoryEntry(RARC rarc) { ParentArchive = rarc; }

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
            public uint Unknown { get; set; }

            internal uint Size;
            internal uint Offset;
            internal uint NameOffset;

            public void Read(FileReader reader)
            {
                uint Unknown = reader.ReadUInt32();
                NameOffset = reader.ReadUInt32();
                Offset = reader.ReadUInt32();
                Size = reader.ReadUInt32();
            }
        }
    }
}
