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
    public class BNR : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Wii Opening Banner" };
        public string[] Extension { get; set; } = new string[] { "*.bnr" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            if (stream.Length < 68) return false;

            using (var reader = new Toolbox.Library.IO.FileReader(stream, true)) {
                return reader.CheckSignature(4, "IMET", 64);
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

        public List<U8.FileEntry> files = new List<U8.FileEntry>();

        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.SeekBegin(64);
                reader.ReadSignature(4, "IMET");

                reader.SeekBegin(1536);
                U8 u8 = new U8();
                u8.Load(new SubStream(reader.BaseStream, reader.Position, reader.BaseStream.Length - 1536));
                foreach (var file in u8.files) {
                    file.FileData = ParseFileData(file.FileData);
                    files.Add(file);
                }
            }
        }

        private byte[] ParseFileData(byte[] data) {
            using (var reader = new FileReader(data))
            {
                reader.SetByteOrder(true);
                string magic = reader.ReadString(4, Encoding.ASCII);
                if (magic == "IMD5")
                {
                    uint fileSize = reader.ReadUInt32();
                    reader.Seek(8); //padding
                    byte[] md5Hash = reader.ReadBytes(16);
                    string compMagic = reader.ReadString(4, Encoding.ASCII);
                    reader.Position -= 4;

                    if (compMagic == "LZ77")
                        return LZ77_WII.Decompress(reader.ReadBytes((int)fileSize));
                    else
                        reader.ReadBytes((int)fileSize);
                }
                if (magic == "IMET")
                {
                    
                }
                if (magic == "BNS")
                {

                }
            }
            return data;
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
    }
}
