using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using LibHac;
using LibHac.IO;

namespace FirstPlugin
{
    public class XCI : IFileFormat, IArchiveFile, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Rom;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "XCI" };
        public string[] Extension { get; set; } = new string[] { "*.xci" };
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

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".xci");
        }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public List<NSP.FileEntry> files = new List<NSP.FileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        Nca Control { get; set; }

        public void Load(System.IO.Stream stream)
        {
            var Keys = Forms.SwitchKeySelectionForm.ShowKeySelector();
            if (Keys == null)
                throw new Exception("Failed to get keys. Please select valid paths!");

            Xci xci = new Xci(Keys, stream.AsStorage());
            var CnmtNca = new Nca(Keys, xci.SecurePartition.OpenFile(
                xci.SecurePartition.Files.FirstOrDefault(s => s.Name.Contains(".cnmt.nca"))), false);
            var CnmtPfs = new Pfs(CnmtNca.OpenSection(0, false, IntegrityCheckLevel.None, true));
            var Cnmt = new Cnmt(CnmtPfs.OpenFile(CnmtPfs.Files[0]).AsStream());
            var Program = Cnmt.ContentEntries.FirstOrDefault(c => c.Type == CnmtContentType.Program);
            var CtrlEntry = Cnmt.ContentEntries.FirstOrDefault(c => c.Type == CnmtContentType.Control);
            if (CtrlEntry != null)
                Control = new Nca(Keys, xci.SecurePartition.OpenFile($"{CtrlEntry.NcaId.ToHexString().ToLower()}.nca"), false);
            var Input = xci.SecurePartition.OpenFile($"{Program.NcaId.ToHexString().ToLower()}.nca").AsStream();

            var Nca = new Nca(Keys, Input.AsStorage(), true);

            if (Nca.CanOpenSection((int)ProgramPartitionType.Code))
            {
                var exefs = new Pfs(Nca.OpenSection((int)ProgramPartitionType.Code,
                        false, IntegrityCheckLevel.None, true));

                foreach (var file in exefs.Files)
                    files.Add(new NSP.ExefsEntry(exefs, file));
            }

            Romfs romfs = new Romfs(
                 Nca.OpenSection(Nca.Sections.FirstOrDefault
                        (s => s?.Type == SectionType.Romfs || s?.Type == SectionType.Bktr)
                        .SectionNum, false, IntegrityCheckLevel.None, true));

            for (int i = 0; i < romfs.Files.Count; i++)
                files.Add(new NSP.FileEntry(romfs, romfs.Files[i]));
        }
        public void Unload()
        {
            Control?.Dispose();
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
