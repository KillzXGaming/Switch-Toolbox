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
    public class IGA_PAK : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Crash Team Racing Archive" };
        public string[] Extension { get; set; } = new string[] { "*.pak" };
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
                return reader.CheckSignature(3, "IGA");
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

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                uint Singnature = reader.ReadUInt32(); //IGAx01
                uint Version = reader.ReadUInt32(); //13
                uint FileSectionSize = reader.ReadUInt32(); //Total size of file entires that point to file data
                uint FileCount = reader.ReadUInt32();
                uint ChunkSize = reader.ReadUInt32();

                //Skip some unknowns for now
                reader.Seek(20, System.IO.SeekOrigin.Current);
                uint NameTableOffset = reader.ReadUInt32();
                uint Padding = reader.ReadUInt32();
                uint NameTableSize = reader.ReadUInt32();
                uint Unknown2 = reader.ReadUInt32(); //Always 1?
                uint[] Hashes = reader.ReadUInt32s((int)FileCount);

                //Read the filenames first
                long pos = reader.Position;

                string[] FileNames = new string[FileCount];
                for (int i = 0; i < FileCount; i++)
                {
                    reader.SeekBegin(NameTableOffset + (i * sizeof(uint)));
                    uint NameOffset = reader.ReadUInt32();

                    reader.SeekBegin(NameTableOffset + NameOffset);
                    FileNames[i] = reader.ReadZeroTerminatedString();
                }

                //Go back for file entries
                reader.Position = pos;

                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry(FilePath);
                    file.FileName = FileNames[i];
                    file.Read(reader);
                    files.Add(file);
                }
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
            //The archive file used to open the file
            public string SourceFile { get; set; }

            public CompressionType FileCompressionType;

            public enum CompressionType : int
            {
                None = -1,
                LZMA = 536870957,
                Deflate,
            }

            public FileEntry(string ArchivePath)
            {
                SourceFile = ArchivePath;
            }

            private uint FileOffset;
            private uint DecompressedFileSize;

            public override byte[] FileData
            {
                get
                {
                    using (var reader = new FileReader(SourceFile))
                    {
                        reader.Position = FileOffset;
                        if (FileCompressionType == CompressionType.None)
                        {
                            return reader.ReadBytes((int)DecompressedFileSize);
                        }
                        else
                        {
                            uint FileSize = reader.ReadUInt32();

                            reader.Position = FileOffset;
                            return reader.ReadBytes((int)FileSize + 0x10);
                        }
                    }
                }
            }

            public void Read(FileReader reader)
            {
                FileOffset = reader.ReadUInt32();
                byte[] Unknown = reader.ReadBytes(4);
                DecompressedFileSize = reader.ReadUInt32();
                FileCompressionType = reader.ReadEnum<CompressionType>(false);

              
            }
        }
    }
}
