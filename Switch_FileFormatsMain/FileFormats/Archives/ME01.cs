using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class ME01 : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "ME01" };
        public string[] Extension { get; set; } = new string[] { "*.bin" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "ME01");
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

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "ME01");
                uint FileCount = reader.ReadUInt32();
                uint Alignment = reader.ReadUInt32();
                uint[] DataOffsets = reader.ReadUInt32s((int)FileCount);
                uint[] Sizes = reader.ReadUInt32s((int)FileCount);

                string[] FileNames = new string[FileCount];
                for (int i = 0; i < FileCount; i++)
                {
                    FileNames[i] = reader.ReadZeroTerminatedString();
                    while (true)
                    {
                        if (reader.ReadByte() != 0x30)
                        {
                            reader.Seek(-1);
                            break;
                        }
                    }
                }

                long DataPosition = reader.Position;
                for (int i = 0; i < FileCount; i++)
                {
                   //reader.SeekBegin(DataPosition + DataOffsets[i]);
                    var file = new FileEntry();
                    file.FileName = FileNames[i];
                    file.FileData = reader.ReadBytes((int)Sizes[i]);
                    files.Add(file);

                    while (true)
                    {
                        if (reader.EndOfStream)
                            break;

                        if (reader.ReadByte() != 0x30)
                        {
                            reader.Seek(-1);
                            break;
                        }
                    }
                }
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
