using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using LibHac.IO;
using LibHac;

namespace FirstPlugin
{
    public class IStorage : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Rom;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "IStorage" };
        public string[] Extension { get; set; } = new string[] { "*.istorage" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public List<NSP.FileEntry> files = new List<NSP.FileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".istorage");
        }

        public void Load(System.IO.Stream stream)
        {
            Romfs romfs = new Romfs(stream.AsStorage());

            for (int i = 0; i < romfs.Files.Count; i++)
                files.Add(new NSP.FileEntry(romfs, romfs.Files[i]));
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
