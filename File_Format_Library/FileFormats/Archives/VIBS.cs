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
    public class VIBS : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Joycon Vibration Archive" };
        public string[] Extension { get; set; } = new string[] { "*.vibs" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".vibs";
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
                uint version = reader.ReadUInt32();
                uint numEntries = reader.ReadUInt32();

                for (int i = 0; i < numEntries; i++)
                {
                    var file = new FileEntry();
                    file.FileName = reader.ReadString(24, true);
                    uint unk = reader.ReadUInt32(); // 8
                    uint unk2 = reader.ReadUInt32(); // 12
                    uint dataLength = reader.ReadUInt32();
                    uint index = reader.ReadUInt32();
                    uint dataOffset = reader.ReadUInt32();
                    files.Add(file);

                    using (reader.TemporarySeek(dataOffset, System.IO.SeekOrigin.Begin)) {
                        file.FileData = reader.ReadBytes((int)dataLength);
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
