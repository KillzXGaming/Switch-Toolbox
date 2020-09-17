using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using LibHac;
using LibHac.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;

namespace FirstPlugin
{
    public class NSP : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Rom;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "NSP" };
        public string[] Extension { get; set; } = new string[] { "*.nsp" };
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

        Nca Control { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public List<FileEntry> files = new List<FileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".nsp");
        }

        public void Load(System.IO.Stream stream)
        {
            var Keys = Forms.SwitchKeySelectionForm.ShowKeySelector();
            if (Keys == null)
                throw new Exception("Failed to get keys. Please select valid paths!");

            Stream Input;

            var Pfs = new Pfs(stream.AsStorage());
            var CnmtNca = new Nca(Keys, Pfs.OpenFile(Pfs.Files.FirstOrDefault(s => s.Name.Contains(".cnmt.nca"))), false);
            var CnmtPfs = new Pfs(CnmtNca.OpenSection(0, false, IntegrityCheckLevel.None, true));
            var Cnmt = new Cnmt(CnmtPfs.OpenFile(CnmtPfs.Files[0]).AsStream());
            foreach (var entry in Cnmt.ContentEntries)
            {
                if (entry.Type == CnmtContentType.Program) {
                    var Program = entry;

                    Input = Pfs.OpenFile($"{Program.NcaId.ToHexString().ToLower()}.nca").AsStream();
                    var Nca = new Nca(Keys, Input.AsStorage(), true);

                    Romfs romfs = new Romfs(
                     Nca.OpenSection(Nca.Sections.FirstOrDefault
                (s => s?.Type == SectionType.Romfs || s?.Type == SectionType.Bktr)
                        .SectionNum, false, IntegrityCheckLevel.None, true));

                    if (Nca.CanOpenSection((int)ProgramPartitionType.Code))
                    {
                        var exefs = new Pfs(Nca.OpenSection((int)ProgramPartitionType.Code,
                                false, IntegrityCheckLevel.None, true));

                        foreach (var file in exefs.Files)
                            files.Add(new ExefsEntry(exefs, file, Program.NcaId.ToHexString()));
                    }

                    for (int i = 0; i < romfs.Files.Count; i++)
                        files.Add(new FileEntry(romfs, romfs.Files[i], Program.NcaId.ToHexString()));
                }
            }
            var CtrlEntry = Cnmt.ContentEntries.FirstOrDefault(c => c.Type == CnmtContentType.Control);
            if (CtrlEntry != null)
                Control = new Nca(Keys, Pfs.OpenFile($"{CtrlEntry.NcaId.ToHexString().ToLower()}.nca"), false);
        }
 
        public void Unload()
        {
            files.Clear();

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

        public class ExefsEntry : PartEntry
        {
            public ExefsEntry(Pfs pfs, PfsFileEntry entry, string root = "") : base(pfs, entry)
            {
                if (root != string.Empty)
                    FileName = $"{root}/Exefs/{entry.Name}";
                else
                    FileName = $"Exefs/{entry.Name}";
            }
        }

        public class PartEntry : FileEntry
        {
            private Pfs ParentPfs;
            private PfsFileEntry fileEntry;

            public override Stream FileDataStream
            {
                get
                {
                    return ParentPfs.OpenFile(fileEntry).AsStream();
                }
            }

            public override void Export()
            {
                string fileName = Path.GetFileName(FileName.RemoveIllegaleFolderNameCharacters());

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = fileName;
                sfd.DefaultExt = Path.GetExtension(fileName);
                sfd.Filter = "Raw Data (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileDataStream.ExportToFile(sfd.FileName);
                }
            }

            public PartEntry(Pfs pfs, PfsFileEntry entry)
            {
                ParentPfs = pfs;
                fileEntry = entry;
                FileName = fileEntry.Name;
            }
        }

        public class FileEntry : ArchiveFileInfo
        {
            private Romfs ParentROMFS;
            private RomfsFile File;

            [Browsable(false)]
            public override object DisplayProperties => this;

            public override string FileSize => STMath.GetFileSize(File.DataLength, 4);

            public override Stream FileDataStream
            {
                get
                {
                    return ParentROMFS.OpenFile(File).AsStream(); 
                }
            }

            public override void Export()
            {
                string fileName = Path.GetFileName(FileName.RemoveIllegaleFolderNameCharacters());

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = fileName;
                sfd.DefaultExt = Path.GetExtension(fileName);
                sfd.Filter = "Raw Data (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ParentROMFS.OpenFile(File).WriteAllBytes($"{sfd.FileName}");
                }
            }

            public FileEntry()
            {

            }

            public FileEntry(Romfs romfs, RomfsFile romfsFile, string root = "")
            {
                ParentROMFS = romfs;
                File = romfsFile;
                if (root != string.Empty)
                    FileName = $"{root}/Romfs/{File.FullPath}";
                else
                    FileName = $"Romfs/{File.FullPath}";
            }
        }
    }
}
