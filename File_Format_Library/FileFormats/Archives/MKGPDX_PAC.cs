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
    public class MKGPDX_PAC : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Mario Kart Arcade GP DX Layout Archive (PAC)" };
        public string[] Extension { get; set; } = new string[] { "*.pac" };
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
                return reader.CheckSignature(4, "pack");
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
                reader.ReadSignature(4, "pack");
                uint FileCount = reader.ReadUInt32();
                uint OffsetStringPool = reader.ReadUInt32();
                uint Alignment = reader.ReadUInt32();
                uint FileType = reader.ReadUInt32();

                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry();
                    file.Read(reader);
                    files.Add(file);
                }

                //Use the header alignment and set the block data
                reader.Align((int)Alignment);
                var DataBlockPosition = reader.Position;
                for (int i = 0; i < files.Count; i++)
                {
                    reader.Seek(DataBlockPosition + files[i].Offset, System.IO.SeekOrigin.Begin);
                    files[i].FileData = reader.ReadBytes((int)files[i].Size);

                    //Get the string data
                    reader.Seek(DataBlockPosition + OffsetStringPool + files[i].NameOffset, System.IO.SeekOrigin.Begin);
                    files[i].FileName = reader.ReadZeroTerminatedString();
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

            internal uint Size;
            internal uint Offset;
            internal uint NameOffset;

            public void Read(FileReader reader)
            {
                uint Unknown = reader.ReadUInt32();
                NameOffset = reader.ReadUInt32();
                Offset = reader.ReadUInt32();
                Size = reader.ReadUInt32();
            }
        }
    }
}
