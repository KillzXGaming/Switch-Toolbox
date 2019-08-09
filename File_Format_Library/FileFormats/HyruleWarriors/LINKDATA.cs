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
    public class LINKDATA : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Fire Emblem Three Houses Archive" };
        public string[] Extension { get; set; } = new string[] { "*.idx" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return (FileName == "LINKDATA.IDX");
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

        public void ClearFiles() { files.Clear(); }

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                const uint SizeOfEntry = 32;
                uint FileCount = (uint)reader.BaseStream.Length / SizeOfEntry;

                reader.SetByteOrder(false);

                for (int i = 0; i < FileCount; i++)
                {
                    FileEntry entry = new FileEntry(FilePath);
                    entry.FileName = $"file {i}";
                    entry.Read(reader);

                    if (entry.Size != 0 && entry.CompFlags == 0)
                        files.Add(entry);
                }
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
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
            public string SourcePath { get; set; }

            public ulong Offset;
            public ulong Size;
            public ulong CompSize;
            public ulong CompFlags;

            public FileEntry(string path) {
                SourcePath = path;
            }

            public void Read(FileReader reader)
            {
                Offset = reader.ReadUInt64();
                Size = reader.ReadUInt64();
                CompSize = reader.ReadUInt64();
                CompFlags = reader.ReadUInt64();
            }

            public override byte[] FileData
            {
                get
                {
                    string path = SourcePath.Replace("IDX", "BIN");
                    if (!System.IO.File.Exists(path))
                        return new byte[0];

                    using (var reader = new FileReader(path))
                    {
                        reader.SeekBegin((long)Offset);

                        if (CompFlags != 0)
                        {
                            uint chunkSize = reader.ReadUInt32();
                            uint numChunks = reader.ReadUInt32();
                            uint size = reader.ReadUInt32();
                            if (chunkSize == 0xffffffff)
                            {

                            }

                            reader.SeekBegin((long)Offset);
                            return reader.ReadBytes((int)Size);
                        }
                        else
                            return reader.ReadBytes((int)Size);
                    }
                }
            }
        }
    }
}
