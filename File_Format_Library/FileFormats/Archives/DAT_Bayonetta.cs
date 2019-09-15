using System;
using System.Collections.Generic;
using System.IO;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class DAT_Bayonetta : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Platinum Games Archive" };
        public string[] Extension { get; set; } = new string[] { "*.pkz" };
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
                return reader.CheckSignature(3, "DAT");
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

        public void ClearFiles() { files.Clear(); }

        private System.IO.Stream _stream;
        public void Load(System.IO.Stream stream)
        {
            _stream = stream;
            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(false);
                uint magic = reader.ReadUInt32();
                uint fileCount = reader.ReadUInt32();
                uint offsetFileOffsetTbl= reader.ReadUInt32();
                uint offsetFileExtTbl = reader.ReadUInt32();
                uint offsetFileNameTbl = reader.ReadUInt32();
                uint offsetFileSizeTbl = reader.ReadUInt32();

                reader.SeekBegin(offsetFileOffsetTbl);
                var offsets = reader.ReadUInt32s((int)fileCount);

                reader.SeekBegin(offsetFileExtTbl);
                string[] extensions = new string[fileCount];
                for (int i = 0; i < fileCount; i++)
                    extensions[i] = reader.ReadString(4, true);

                reader.SeekBegin(offsetFileNameTbl);
                uint strSize = reader.ReadUInt32();

                string[] names = new string[fileCount];
                for (int i = 0; i < fileCount; i++)
                    names[i] = reader.ReadString((int)strSize, true);

                reader.SeekBegin(offsetFileSizeTbl);
                var sizes = reader.ReadUInt32s((int)fileCount);

                for (int i = 0; i < fileCount; i++)
                {
                    var file = new FileEntry();
                    file.FileName = $"{names[i]}";
                    file.FileDataStream = new SubStream(reader.BaseStream, (long)offsets[i], (long)sizes[i]);
                    files.Add(file);
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
            public ulong fileSize;
            public ulong fileOffset;
            public ulong compressedSize;

            protected Stream stream;
            public override Stream FileDataStream
            {
                get
                {
                    if (compressedSize != fileSize)
                        return new MemoryStream(Zstb.SDecompress(stream.ToBytes()));
                    else
                        return stream;
                }
                set
                {
                    stream = value;
                }
            }
        }
    }
}
