using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class APAK : IFileFormat, IArchiveFile
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "APAK" };
        public string[] Extension { get; set; } = new string[] { "*.apak" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "APAK");
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

        public List<APAKFileInfo> files = new List<APAKFileInfo>();

        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                reader.ReadSignature(4, "APAK");
                reader.ReadUInt16();
                uint Version = reader.ReadUInt16();
                if (Version != 5)
                    reader.SetByteOrder(false);

                uint FileCount = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint DataTotalSize = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();

                for (int i = 0; i < FileCount; i++)
                    files.Add(new APAKFileInfo(reader));
            }
        }

        public void Save(System.IO.Stream stream)
        {

        }

        public class APAKFileInfo : ArchiveFileInfo
        {
            public APAKFileInfo(FileReader reader)
            {
                long pos = reader.Position;

                uint dataOffset = reader.ReadUInt32();
                uint uncompressedSize = reader.ReadUInt32();
                uint compressedSize = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();
                uint unk4 = reader.ReadUInt32();

                FileName = reader.ReadString(0x20, true);
                reader.ReadUInt32();

                long endpos = reader.Position;

                reader.Seek(dataOffset, System.IO.SeekOrigin.Begin);
                FileData = reader.ReadBytes((int)compressedSize);

                reader.Seek(endpos, System.IO.SeekOrigin.Begin);
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
    }
}
