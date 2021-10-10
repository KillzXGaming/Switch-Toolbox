using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class PKG : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "PKG" };
        public string[] Extension { get; set; } = new string[] { "*.pkg" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return FileName.EndsWith(".pkg");
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

        private System.IO.Stream _stream;
        public void Load(System.IO.Stream stream)
        {
            _stream = stream;
            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(false);
                uint headerSize = reader.ReadUInt32();
                uint fileSize = reader.ReadUInt32();
                uint numFiles = reader.ReadUInt32();

                for (int i = 0; i < numFiles; i++)
                {
                    var file = new FileEntry();
                    ulong nameHash = reader.ReadUInt64();
                    uint fileStartOffset = reader.ReadUInt32();
                    uint fileEndOffset = reader.ReadUInt32();

                    uint size = fileEndOffset - fileStartOffset;

                    file.FileName = nameHash.ToString();
                    file.FileDataStream = new SubStream(reader.BaseStream,
                        fileStartOffset, size);

                    string ext = ".bin";
                    if (size > 4)
                    {
                        using (reader.TemporarySeek(fileStartOffset, SeekOrigin.Begin)) {
                            string magic = reader.ReadString(4);
                            if (magic == "FWAV") ext = ".bfwav";
                            if (magic == "MTXT") ext = ".bctext";
                            if (magic == "MCAN") ext = ".mcamera";
                            if (magic == "MANM") ext = ".manim";
                            if (magic == "MSAS") ext = ".msas";
                            if (magic == "MMDL") ext = ".mmodel";
                            if (magic == "MSUR") ext = ".mmaterial";
                            if (magic == "MNAV") ext = ".mnavigation";
                        }
                    }

                    file.FileName += ext;

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
        }
    }
}
