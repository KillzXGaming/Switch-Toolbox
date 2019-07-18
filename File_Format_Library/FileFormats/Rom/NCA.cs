using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library;
using LibHac;
using LibHac.IO;

namespace FirstPlugin
{
    public class NCA : IFileFormat, IArchiveFile
    {
        public FileType FileType { get; set; } = FileType.Rom;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "NCA" };
        public string[] Extension { get; set; } = new string[] { "*.nca" };
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
            return Utils.HasExtension(FileName, ".nca");
        }

        public void Load(System.IO.Stream stream)
        {
            string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string KeyFile = Path.Combine(homeFolder, ".switch", "prod.keys");
            string TitleKeyFile = Path.Combine(homeFolder, ".switch", "title.keys");

            var Keys = ExternalKeys.ReadKeyFile(KeyFile, TitleKeyFile);

            var Nca = new Nca(Keys, stream.AsStorage(), true);

            Romfs romfs = new Romfs(
                     Nca.OpenSection(Nca.Sections.FirstOrDefault
                            (s => s?.Type == SectionType.Romfs || s?.Type == SectionType.Bktr)
                            .SectionNum, false, IntegrityCheckLevel.None, true));

            for (int i = 0; i < romfs.Files.Count; i++)
                files.Add(new NSP.FileEntry(romfs, romfs.Files[i]));
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
