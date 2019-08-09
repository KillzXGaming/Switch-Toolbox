using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using CTR.NCCH;

namespace FirstPlugin
{
    public class NCCH : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        private System.IO.Stream _stream = null;

        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Nintendo Content Container" };
        public string[] Extension { get; set; } = new string[] { "*.cxi" };
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
                return reader.CheckSignature(4, "NCCH", 0x100);
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

        public List<RomfsFileEntry> files = new List<RomfsFileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;
        public void ClearFiles() { files.Clear(); }

        private Header header;
        private RomFS RomFS;
        const int MediaUnitSize = 0x200;

        public void Load(System.IO.Stream stream)
        {
            _stream = stream;
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                header = reader.ReadStruct<Header>();

                Console.WriteLine("RomfsOffset " + header.RomfsOffset);
                Console.WriteLine("RomfsSize " + header.RomfsSize);

                if (header.RomfsOffset != 0 && header.RomfsSize != 0)
                {
                    uint offset = header.RomfsOffset * MediaUnitSize;
                    uint size = header.RomfsSize * MediaUnitSize;

                    RomFS = new RomFS();
                    RomFS.FileName = "RomFS.bin";
                    RomFS.IFileInfo = new IFileInfo();
                    RomFS.IFileInfo.InArchive = true;
                    RomFS.Load(new SubStream(stream, offset, size));
                    files.AddRange(RomFS.files);
                }
            }
        }

        public void Unload()
        {
            _stream?.Dispose();
            _stream?.Close();
            _stream = null;
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
    }
}
