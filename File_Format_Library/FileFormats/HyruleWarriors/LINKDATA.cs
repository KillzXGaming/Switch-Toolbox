using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.IO;

namespace FirstPlugin
{
    public class LINKDATA : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Fire Emblem Three Houses Archive" };
        public string[] Extension { get; set; } = new string[] { "*.bin" , "*.idx" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return (FileName == "LINKDATA.IDX" || FileName == "DATA0.bin");
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
                    FileEntry entry = new FileEntry(FilePath, this);
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
            private LINKDATA ParentFile;

            public string SourcePath { get; set; }

            public ulong Offset;
            public ulong Size;
            public ulong CompSize;
            public ulong CompFlags;

            public FileEntry(string path, LINKDATA data) {
                SourcePath = path;
                ParentFile = data;
            }

            public void Read(FileReader reader)
            {
                Offset = reader.ReadUInt64();
                Size = reader.ReadUInt64();
                CompSize = reader.ReadUInt64();
                CompFlags = reader.ReadUInt64();
            }

            public override Stream FileDataStream
            {
                get
                {
                    if (ParentFile.IFileInfo.ArchiveParent != null)
                    {
                        foreach (var file in ParentFile.IFileInfo.ArchiveParent.Files)
                        {
                            if (file.FileName.Contains("DATA1.bin"))
                            {
                                return GetData(new FileReader(file.FileDataStream, true));
                            }
                        }

                        return new MemoryStream();
                    }
                    else
                    {
                        string path = "";
                        if (SourcePath.Contains("LINKDATA.IDX"))
                            path = SourcePath.Replace("IDX", "BIN");
                        if (SourcePath.Contains("DATA1.bin"))
                            path = SourcePath.Replace("DATA0", "DATA1");

                        if (!System.IO.File.Exists(path))
                            return new MemoryStream();

                        using (var reader = new FileReader(path, true))
                        {
                            return GetData(reader);
                        }
                    }
                }
            }

            private Stream GetData(FileReader reader)
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
                    return new SubStream(reader.BaseStream, (long)Offset, (long)Size);
                }
                else
                    return new SubStream(reader.BaseStream, (long)Offset, (long)Size);
            }
        }
    }
}
