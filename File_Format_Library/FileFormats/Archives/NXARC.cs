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
    public class NXARC : IArchiveFile,  IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "NX Archive" };
        public string[] Extension { get; set; } = new string[] { "*.nxarc" };
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
                return reader.CheckSignature(4, "RAXN");
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

        public IEnumerable<ArchiveFileInfo> Files
        {
            get { return files; }
            set { }
        }

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "RAXN");
                uint unk = reader.ReadUInt32();
                char[] Type = reader.ReadChars(4);
                uint OffsetBlock = reader.ReadUInt32();
                uint HeaderSize = reader.ReadUInt32();
                uint FileCount = reader.ReadUInt32();
                uint BlockSize = reader.ReadUInt32();

                List<string> FileNames = new List<string>();
                reader.Seek(OffsetBlock, System.IO.SeekOrigin.Begin);
                for (int i = 0; i < FileCount; i++)
                {
                    FileNames.Add(reader.ReadZeroTerminatedString());
                }

                reader.Seek(HeaderSize, System.IO.SeekOrigin.Begin);
                for (int i = 0; i < FileCount; i++)
                {
                    if (i == 0)
                    {
                        //Skip string table
                        reader.Seek(32);
                    }
                    else
                    {
                        var file = new FileEntry();
                        file.FileName = FileNames[i];
                        file.Read(reader);
                        files.Add(file);
                    }
                }

                reader.Seek(HeaderSize, System.IO.SeekOrigin.Begin);
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
            public void Read(FileReader reader)
            {
                ulong Size = reader.ReadUInt64();
                ulong Offset = reader.ReadUInt64();
                ulong Flag = reader.ReadUInt64();
                ulong Unknown = reader.ReadUInt64();

                using (reader.TemporarySeek((long)Offset, System.IO.SeekOrigin.Begin))
                {
                    FileData = reader.ReadBytes((int)Size);
                    if (Flag == 1)
                    {
                        FileData = STLibraryCompression.ZLIB.Decompress(FileData);
                    }
                }
            }

            public void Write(FileWriter writer)
            {

            }
        }
    }
}
