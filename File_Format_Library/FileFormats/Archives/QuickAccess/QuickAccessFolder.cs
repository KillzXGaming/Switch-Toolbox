using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using Toolbox.Library;
using System.Windows.Forms;
using System.IO;

namespace FirstPlugin.FileFormats.Archives.QuickAccess
{
    public class QuickAccessFolder : TreeNodeCustom, IContextMenuNode
    {
        private IArchiveFile ArchiveFile;
        public QuickAccessFolder(IArchiveFile archiveFile, string text)
        {
            Text = text;
            ArchiveFile = archiveFile;
        }

        public virtual ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
            Items.Add(new ToolStripMenuItem("Replace (From Folder)", null, ReplaceAllAction, Keys.Control | Keys.R));
            return Items.ToArray();
        }

        private void ExportAllAction(object sender, EventArgs args)
        {
            FolderSelectDialog folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (TreeNode folder in Nodes)
            {
                string path = Path.Combine(folderDialog.SelectedPath, Text, folder.Text);

                foreach (TreeNode file in folder.Nodes)
                {
                    if (file.Tag is ArchiveFileInfo)
                    {
                        var fileInfo = (ArchiveFileInfo)file.Tag;
                        var filePath = Path.Combine(path, file.Text);

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        File.WriteAllBytes(filePath, fileInfo.FileData);
                    }
                }
            }
        }

        private void ReplaceAllAction(object sender, EventArgs args)
        {
            FolderSelectDialog folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (var folder in Directory.GetDirectories(folderDialog.SelectedPath))
            {
                foreach (var file in Directory.GetFiles(folder))
                {
                    var fileInfo = ArchiveFile.Files.FirstOrDefault(x =>
                         Path.GetFileName(x.FileName).Contains(Path.GetFileName(file)));

                    if (fileInfo != null)
                        fileInfo.FileData = File.ReadAllBytes(file);
                }
            }
        }
    }

}
