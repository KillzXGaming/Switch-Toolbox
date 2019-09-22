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
    public class NCA : IFileFormat, IArchiveFile, ILeaveOpenOnLoad
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
            var Keys = Forms.SwitchKeySelectionForm.ShowKeySelector();
            if (Keys == null)
                throw new Exception("Failed to get keys. Please select valid paths!");

            var Nca = new Nca(Keys, stream.AsStorage(), true);

            Romfs romfs = new Romfs(
                     Nca.OpenSection(Nca.Sections.FirstOrDefault
                            (s => s?.Type == SectionType.Romfs || s?.Type == SectionType.Bktr)
                            .SectionNum, false, IntegrityCheckLevel.None, true));

            if (Nca.CanOpenSection((int)ProgramPartitionType.Code))
            {
                var exefs = new Pfs(Nca.OpenSection((int)ProgramPartitionType.Code,
                        false, IntegrityCheckLevel.None, true));

                foreach (var file in exefs.Files)
                    files.Add(new NSP.ExefsEntry(exefs, file));
            }

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
