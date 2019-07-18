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
    public class ARC : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Mario Kart Arcade GP DX Layout Archive (PAC)" };
        public string[] Extension { get; set; } = new string[] { "*.arc" };
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
                return reader.ReadInt64() == 0x000000000000;
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
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                long Identifier = reader.ReadInt64();
                ulong ChunkSection1 = reader.ReadUInt64();
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
          
        }
    }
}
