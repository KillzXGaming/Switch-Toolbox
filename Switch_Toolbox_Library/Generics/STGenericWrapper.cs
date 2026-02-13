using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using System.IO;

namespace Toolbox.Library.NodeWrappers
{
    //Generic wrapper based on https://github.com/libertyernie/brawltools/blob/40d7431b1a01ef4a0411cd69e51411bd581e93e2/BrawlBox/NodeWrappers/GenericWrapper
    //Store useful generic functions in this wrapper for tree nodes
    public class STGenericWrapper : TreeNodeCustom, IContextMenuNode
    {
        public STGenericWrapper(string name) { Text = name; }

        int IndexStr = 0;
        public string SearchDuplicateNames(string Name)
        {
            bool IsDuped = false;
            foreach (TreeNode node in Nodes)
                if (node.Text == Name)
                    IsDuped = true;

            if (IsDuped)
                return SearchDuplicateNames($"{Name}{IndexStr++}");
            else
                return Name;
        }

        public bool IsFolder = false;

        public bool CanExport { get; set; }

        public bool CanReplace { get; set; }

        public bool CanRename { get; set; }

        public bool CanDelete { get; set; }

        private void EnableContextMenu(ToolStripItemCollection Items, string Key, bool Enabled)
        {
            foreach (ToolStripItem item in Items)
            {
                if (item.Text == Key)
                    item.Enabled = Enabled;
            }
        }

        public STGenericWrapper(bool LoadMenus = true)
        {
            CanExport = true;
            CanReplace = false;
            CanRename = true;
            CanDelete = false;
        }

        public virtual ToolStripItem[] GetContextMenuItems()
        {
            if (IsFolder)
            {
                return new ToolStripItem[]
                {
                    new ToolStripMenuItem("Import", null, ImportAction, Keys.Control | Keys.I) {Enabled = CanReplace },
                    new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E)  {Enabled = CanExport },
                    new ToolStripMenuItem("Replace All", null, ReplaceAllAction, Keys.Control | Keys.R) {Enabled = CanReplace },
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Sort", null, SortAction, Keys.Control | Keys.N),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Clear", null, ClearAction, Keys.Control | Keys.Delete) {Enabled = CanDelete } ,
                };
            }
            else
            {
                return new ToolStripItem[]
                {
                    new ToolStripMenuItem("Export", null, ExportAction, Keys.Control | Keys.E) {Enabled = CanExport },
                    new ToolStripMenuItem("Replace", null, ReplaceAction, Keys.Control | Keys.R) {Enabled = CanReplace },
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Rename", null, RenameAction, Keys.Control | Keys.N) {Enabled = CanRename },
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Delete", null, DeleteAction, Keys.Control | Keys.Delete) {Enabled = CanDelete },
                };
            }
        }

        public void LoadFolderMenus()
        {
            CanExport = false;
            CanReplace = false;
            CanRename = false;
            CanDelete = false;

            IsFolder = true;
        }

        protected void ReplaceAllAction(object sender, EventArgs e) {
            ReplaceAll();
        }

        public virtual void ReplaceAll(string ReplacePath = "")
        {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in System.IO.Directory.GetFiles(sfd.SelectedPath))
                {
                    string FileName = System.IO.Path.GetFileNameWithoutExtension(file);

                    foreach (TreeNode node in Nodes)
                    {
                        if (node is STGenericWrapper)
                        {
                            if (FileName == node.Text)
                            {
                                ((STGenericWrapper)node).Replace(file);
                            }
                        }
                    }
                }
            }
        }

        protected void ExportAllAction(object sender, EventArgs e)
        {
            if (Nodes.Count <= 0)
                return;

            string formats = ExportFilter;

            string[] forms = formats.Split('|');

            List<string> Formats = new List<string>();

            for (int i = 0; i < forms.Length; i++)
            {
                if (i > 1 || i == (forms.Length - 1)) //Skip lines with all extensions
                {
                    if (!forms[i].StartsWith("*"))
                        Formats.Add(forms[i]);
                }
            }

            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = $"{sfd.SelectedPath}\\{RootNode.Text}";
                    Directory.CreateDirectory(folderPath);

                    string extension = form.GetSelectedExtension();
                    extension.Replace(" ", string.Empty);

                    var failedExports = new List<string>();

                    foreach (TreeNode node in Nodes)
                    {
                        try
                        {
                            if (node is STGenericWrapper)
                            {
                                ((STGenericWrapper)node).Export($"{folderPath}\\{node.Text}{extension}");
                            }
                        } catch (Exception ex)
                        {
                            failedExports.Add(ex.Message);
                        }

                    }

                    if (failedExports.Count > 0)
                        STErrorDialog.Show("Files exported with warnings.", "Switch Toolbox", string.Join("\n", failedExports));

                }
            }
        }

        protected void ExportAction(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = ExportFilter;
            sfd.FileName = Text;
            if (this is IFileFormat) {
                string extension = System.IO.Path.GetExtension(((IFileFormat)this).FilePath);
                if (extension != string.Empty) {
                    sfd.DefaultExt = extension;
                }
            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export(sfd.FileName);
            }
        }

        protected void ImportAction(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ImportFilter;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Import(ofd.FileNames);
            }
        }

        protected void ReplaceAction(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ReplaceFilter;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Replace(ofd.FileName);
            }
        }
        protected void DeleteAction(object sender, EventArgs e) { Delete();}
        protected void RenameAction(object sender, EventArgs e) { Rename(); }
        protected void SortAction(object sender, EventArgs e) { Sort(); }
        protected void ClearAction(object sender, EventArgs e) { Clear(); }

        public virtual string ExportFilter { get { return "All files(*.*)|*.*"; } }
        public virtual string ImportFilter { get { return ExportFilter; } }
        public virtual string ReplaceFilter { get { return ImportFilter; } }

        public virtual void Sort()
        {
            TreeView.TreeViewNodeSorter = new TreeChildSorter();
            TreeView.Sort();
        }

        public virtual void Export(string FileName)
        {

        }

        public virtual void Import(string[] FileName)
        {

        }

        public virtual void Replace(string FileName)
        {

        }

        public virtual void Clear()
        {
            var result = MessageBox.Show("Are you sure you want to clear this section? This cannot be undone!",
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (var node in Nodes)
                    if (node is STGenericWrapper)
                    {
                        ((STGenericWrapper)node).Unload();
                    }

                Nodes.Clear();
            }
        }

        public virtual void Delete()
        {
            if (Parent != null)
            {
                Remove();
            }
        }

        public virtual void Unload()
        {
            foreach (var node in Nodes)
                if (node is STGenericWrapper)
                    ((STGenericWrapper)node).Unload();

            Nodes.Clear();
        }

        public virtual void Rename()
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK) { Text = dialog.textBox1.Text; }
        }
    }
}
