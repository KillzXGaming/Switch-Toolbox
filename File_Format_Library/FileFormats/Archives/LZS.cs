using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class LZS : IFileFormat, IArchiveFile
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "LZS" };
        public string[] Extension { get; set; } = new string[] { "*.lzs" };
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
                reader.SetByteOrder(true);
                return Utils.GetExtension(FileName) == ".lzs";
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
                reader.SetByteOrder(false);
                //First get the first file offset
                reader.SeekBegin(4);
                uint sectionSize = reader.ReadUInt32();

                reader.SeekBegin(0);

                //Now go through each file
                while (reader.Position < sectionSize)
                {
                    var file = new FileEntry();
                    files.Add(file);

                    long pos = reader.Position;

                    ushort unk = reader.ReadUInt16(); //4 or 0
                    ushort unk2 = reader.ReadUInt16(); //40
                    uint dataOffset = reader.ReadUInt32();
                    uint dataSize = reader.ReadUInt32();
                    reader.ReadUInt32(); //padding
                    file.FileName = reader.ReadZeroTerminatedString();
                    //Aligned with 0xFF bytes. The size is always 64 bytes

                    reader.SeekBegin(pos + 64);

                    using (reader.TemporarySeek(dataOffset, System.IO.SeekOrigin.Begin)) {
                        file.FileData = reader.ReadBytes((int)dataSize);
                    }
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
          
        }
    }
}
