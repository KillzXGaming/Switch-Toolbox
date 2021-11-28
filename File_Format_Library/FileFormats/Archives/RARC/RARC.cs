using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using System.IO;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class RARC : IArchiveFile, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "RARC" };
        public string[] Extension { get; set; } = new string[] { "*.rarc", "*.arc", "*.crar", "*.yaz0" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "RARC") || reader.CheckSignature(4, "CRAR");
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

        RARC_Parser RarcFileData;

        public IEnumerable<ArchiveFileInfo> Files => RarcFileData.Files;

        public void ClearFiles() { RarcFileData.Files.Clear(); }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            Items.Add(new ToolStripMenuItem("Batch Rename Galaxy (Mario Galaxy)", null, BatchRenameGalaxy, Keys.Control | Keys.S));
            return Items.ToArray();
        }

        private void BatchRenameGalaxy(object sender, EventArgs args)
        {
            string ActorName = Path.GetFileNameWithoutExtension(FileName);

            RenameDialog dialog = new RenameDialog();
            dialog.SetString(ActorName);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string NewActorName = dialog.textBox1.Text;
                FileName = NewActorName + ".arc";

                foreach (var file in RarcFileData.Files)
                {
                    file.FileName = file.FileName.Replace(ActorName, NewActorName);
                    file.UpdateWrapper();
                }
            }
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public bool IsLittleEndian { get; set; } = false;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            CanRenameFiles = true;
            CanReplaceFiles = true;

            RarcFileData= new RARC_Parser(stream);
        }

        public void Save(System.IO.Stream stream)
        {
            RarcFileData.Save(stream);
        }

        public void Unload() { }

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
