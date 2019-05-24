using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;

namespace Switch_Toolbox.Library.NodeWrappers
{
    //Generic wrapper based on https://github.com/libertyernie/brawltools/blob/40d7431b1a01ef4a0411cd69e51411bd581e93e2/BrawlBox/NodeWrappers/GenericWrapper
    //Store useful generic functions in this wrapper for tree nodes
    public class STGenericWrapper : TreeNodeCustom
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

        private bool canExport;
        public bool CanExport
        {
            get { return canExport; }
            set
            {
                canExport = value;
                EnableContextMenu(ContextMenuStrip.Items, "Export", canExport);
            }
        }

        private bool canReplace;
        public bool CanReplace
        {
            get { return canReplace; }
            set
            {
                canReplace = value;
                EnableContextMenu(ContextMenuStrip.Items, "Replace", canReplace);
            }
        }

        private bool canRename;
        public bool CanRename
        {
            get { return canRename; }
            set
            {
                canRename = value;
                EnableContextMenu(ContextMenuStrip.Items, "Rename", canRename);

            }
        }

        private bool canDelete;
        public bool CanDelete
        {
            get { return canDelete; }
            set
            {
                canDelete = value;
                EnableContextMenu(ContextMenuStrip.Items,"Delete", canDelete);
            }
        }

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
            if (LoadMenus)
                LoadContextMenus();
            else
                ContextMenuStrip = new STContextMenuStrip();

            CanExport = true;
            CanReplace = false;
            CanRename = true;
            CanDelete = false;
        }

        public virtual void LoadContextMenus()
        {
            LoadFileMenus();
        }

        public void LoadFileMenus(bool Reset = true)
        {
            if (Reset)
                ContextMenuStrip = new STContextMenuStrip();

            //File Operations
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace", null, ReplaceAction, Keys.Control | Keys.R));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.Control | Keys.N));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Delete", null, DeleteAction, Keys.Control | Keys.Delete));
        }

        public void LoadFolderMenus()
        {
            ContextMenuStrip = new STContextMenuStrip();

            CanExport = false;
            CanReplace = false;
            CanRename = false;
            CanDelete = false;

            //Folder Operations
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Import", null, ImportAction, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace All", null, ReplaceAllAction, Keys.Control | Keys.R));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Sort", null, SortAction, Keys.Control | Keys.N));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Clear", null, ClearAction, Keys.Control | Keys.C));
        }

        protected void ReplaceAllAction(object sender, EventArgs e)
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
                string folderPath = sfd.SelectedPath;

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string extension = form.GetSelectedExtension();
                    extension.Replace(" ", string.Empty);

                    foreach (TreeNode node in Nodes)
                    {
                        if (node is STGenericWrapper)
                        {
                            ((STGenericWrapper)node).Export($"{folderPath}\\{node.Text}{extension}");
                        }
                    }
                }
            }
        }

        protected void ExportAction(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = ExportFilter;
            sfd.FileName = Text;

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
        protected void DeleteAction(object sender, EventArgs e) { Delete(); Unload(); }
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
