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
    public class LZARC : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "LZARC" };
        public string[] Extension { get; set; } = new string[] { "*.lzarc" };
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
                return Utils.GetExtension(FileName) == ".lzarc";
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

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                uint FileSize =  reader.ReadUInt32();
                uint Unknown = reader.ReadUInt32();
                uint FileCount = reader.ReadUInt32();

                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry();
                    file.Read(reader);
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
            public uint Unknown { get; set; }

            public uint CompressedSize;
            public uint DecompressedSize;

            public void Read(FileReader reader)
            {
                long pos = reader.Position;
                FileName = reader.ReadZeroTerminatedString();
                reader.SeekBegin(pos + 128);

                uint Offset = reader.ReadUInt32();
                CompressedSize = reader.ReadUInt32();
                uint Unknown = reader.ReadUInt32();
                uint Unknown2 = reader.ReadUInt32();
                DecompressedSize = reader.ReadUInt32();

                using (reader.TemporarySeek((int)Offset, System.IO.SeekOrigin.Begin))
                {
                    reader.Seek(8);
                    FileData = reader.ReadBytes((int)CompressedSize - 8);
                    FileData = LZ77_WII.Decompress11(FileData,(int)DecompressedSize);
                }
            }
        }
    }
}
