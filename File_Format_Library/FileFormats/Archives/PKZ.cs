using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class PKZ : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
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
                return reader.CheckSignature(3, "pkz");
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
                uint unknown = reader.ReadUInt32();
                ulong totalFileSize = reader.ReadUInt64();
                uint fileCount = reader.ReadUInt32();
                uint offsetFileInfo = reader.ReadUInt32();
                ulong stringTableLength = reader.ReadUInt64();

                uint InfoSize = fileCount * 0x20;
                uint StringTablePos = InfoSize + offsetFileInfo;

                reader.SeekBegin(offsetFileInfo);
                for (int i = 0; i < fileCount; i++)
                {
                    var file = new FileEntry();
                    ulong nameOffset = reader.ReadUInt64();
                    file.fileSize = reader.ReadUInt64();
                    file.fileOffset = reader.ReadUInt64();
                    file.compressedSize = reader.ReadUInt64();

                    using (reader.TemporarySeek((long)nameOffset + StringTablePos,
                        System.IO.SeekOrigin.Begin))
                    {
                        file.FileName = reader.ReadZeroTerminatedString();
                    }

                    file.FileDataStream = new SubStream(reader.BaseStream, (long)file.fileOffset, (long)file.compressedSize);

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
                    if (compressedSize != fileSize && fileSize != 0)
                        return new MemoryStream(Zstb.SDecompress(stream.ToBytes(),(int)fileSize));
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
