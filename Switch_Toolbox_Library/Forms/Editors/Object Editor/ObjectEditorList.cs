using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using System.Text.RegularExpressions;
using Switch_Toolbox.Library.Animations;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library.Forms
{
    public partial class ObjectEditorList : UserControl
    {
        private IArchiveFile archiveFile;

        private List<ArchiveFileInfo> Files;

        private TreeView _fieldsTreeCache;

        public void BeginUpdate() { treeViewCustom1.BeginUpdate(); }
        public void EndUpdate() { treeViewCustom1.EndUpdate(); }

        public void AddNodeCollection (TreeNodeCollection nodes, bool ClearNodes)
        {
            // Invoke the treeview to add the nodes
            treeViewCustom1.Invoke((Action)delegate ()
            {
                treeViewCustom1.BeginUpdate(); // No visual updates until we say 
                if (ClearNodes)
                    treeViewCustom1.Nodes.Clear(); // Remove existing nodes

                foreach (TreeNode node in nodes)
                    treeViewCustom1.Nodes.Add(node); // Add the new nodes

                treeViewCustom1.EndUpdate(); // Allow the treeview to update visually
            });

        }

        public TreeNodeCollection GetNodes() { return treeViewCustom1.Nodes; }

        public void AddNode(TreeNode node, bool ClearAllNodes = false)
        {
            if (treeViewCustom1.InvokeRequired)
            {
                // Invoke the treeview to add the nodes
                treeViewCustom1.Invoke((Action)delegate ()
                {
                    AddNodes(node, ClearAllNodes);
                });
            }
            else
            {
                AddNodes(node, ClearAllNodes);
            }
            //       _fieldsTreeCache.Nodes.Add(node);
        }

        private void AddNodes(TreeNode node, bool ClearAllNodes = false)
        {
            treeViewCustom1.BeginUpdate(); // No visual updates until we say 
            if (ClearAllNodes)
                ClearNodes();
            treeViewCustom1.Nodes.Add(node); // Add the new nodes
            treeViewCustom1.EndUpdate(); // Allow the treeview to update visually
        }

        public void ClearNodes()
        {
            treeViewCustom1.Nodes.Clear();
          //  _fieldsTreeCache.Nodes.Clear();
        }

        public ObjectEditorList()
        {
            InitializeComponent();

            listViewCustom1.SmallImageList = TreeViewCustom.imgList;

            _fieldsTreeCache = new TreeView();

            if (Runtime.ObjectEditor.ListPanelWidth > 0)
                stPanel1.Width = Runtime.ObjectEditor.ListPanelWidth;

            searchLbl.BackColor = stTextBox1.BackColor;

            treeViewCustom1.BackColor = FormThemes.BaseTheme.ObjectEditorBackColor;
        }

        public Viewport GetViewport() => viewport;

        //Attatch a viewport instance here if created.
        //If the editor gets switched, we can keep the previous viewed area when switched back
        Viewport viewport = null;

        bool IsLoaded = false;
        public void LoadViewport(Viewport Viewport)
        {
            viewport = Viewport;

            IsLoaded = true;
        }

        public IFileFormat GetActiveFile()
        {
            if (treeViewCustom1.Nodes.Count == 0)
                return null;

            if (treeViewCustom1.Nodes[0] is IFileFormat)
                return (IFileFormat)treeViewCustom1.Nodes[0];
            return null;
        }

        public void LoadEditor(Control control)
        {
            foreach (var ctrl in stPanel2.Controls)
            {
                if (ctrl is STUserControl)
                    ((STUserControl)ctrl).OnControlClosing();
            }

            stPanel2.Controls.Clear();
            stPanel2.Controls.Add(control);
        }


        private void UpdateFiles()
        {
            Files = archiveFile.Files.ToList();
        }

        public void FillList(IArchiveFile file)
        {
            archiveFile = file;
            UpdateFiles();

            treeViewCustom1.BeginUpdate();
            treeViewCustom1.Nodes.Clear();

            if (Files.Count > 0)
            {
                var lookup = Files.OrderBy(f => f.FileName.TrimStart('/', '\\')).ToLookup(f => System.IO.Path.GetDirectoryName(f.FileName.TrimStart('/', '\\')));

                // Build directory tree
                var root = treeViewCustom1.Nodes.Add("root", ((IFileFormat)file).FileName, "tree-archive-file", "tree-archive-file");
                foreach (var dir in lookup.Select(g => g.Key))
                {
                    dir.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                        .Aggregate(root, (node, part) => node.Nodes[part] ?? node.Nodes.Add(part, part))
                        .Tag = lookup[dir];
                }

                root.Expand();
                treeViewCustom1.SelectedNode = root;
            }

            treeViewCustom1.EndUpdate();
        }

        private void LoadFiles()
        {
            var group = new ListViewGroup("Image", HorizontalAlignment.Center);
            listViewCustom1.Groups.Add(group);

            listViewCustom1.BeginUpdate();
            listViewCustom1.Items.Clear();
            if (treeViewCustom1.SelectedNode.Tag is IEnumerable<ArchiveFileInfo>)
            {
                foreach (var file in (IEnumerable<ArchiveFileInfo>)treeViewCustom1.SelectedNode.Tag)
                {
                    ListViewItem item = new ListViewItem(System.IO.Path.GetFileName(file.FileName));
                    item.ImageKey = "texture";
                    item.Group = listViewCustom1.Groups[0];
                    item.SubItems.Add(file.GetSize());
                    item.SubItems.Add("");
                    item.SubItems.Add(file.State.ToString());
                    listViewCustom1.Items.Add(item);
                }
            }
           listViewCustom1.EndUpdate();
        }

        private void treeViewCustom1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadFiles();
        }

        public void FormClosing()
        {
            foreach (var node in TreeViewExtensions.Collect(treeViewCustom1.Nodes))
            {
                if (node is IFileFormat)
                {
                    ((IFileFormat)node).Unload();
                }
            }
            ClearNodes();
        }

        public void RemoveFile(TreeNode File)
        {
            if (File is IFileFormat) {
                ((IFileFormat)File).Unload();
            }

            treeViewCustom1.Nodes.Remove(File);
        }

        public void ResetControls()
        {
            treeViewCustom1.Nodes.Clear();
            stPanel2.Controls.Clear();
            Text = "";
        }

        private void stTextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateSearchBox();
        }

        private void stTextBox1_Click(object sender, EventArgs e)
        {
            searchImgPB.Visible = false;
            searchLbl.Visible = false;
        }

        private void stTextBox1_Leave(object sender, EventArgs e)
        {
            UpdateSearchBox();
        }

        private void UpdateSearchBox()
        {
            if (stTextBox1.Text != string.Empty)
            {
                searchImgPB.Visible = false;
                searchLbl.Visible = false;
            }
            else
            {
                searchLbl.Visible = true;
                searchImgPB.Visible = true;
            }
        }

        private void SearchText(string searchText)
        {
            //blocks repainting tree till all objects loaded
            this.treeViewCustom1.BeginUpdate();
            this.treeViewCustom1.Nodes.Clear();
            if (searchText != string.Empty)
            {
                foreach (TreeNode _parentNode in _fieldsTreeCache.Nodes)
                {
                    foreach (TreeNode _childNode in _parentNode.Nodes)
                    {
                        if (_childNode.Text.StartsWith(searchText))
                        {
                            this.treeViewCustom1.Nodes.Add((TreeNode)_childNode.Clone());
                        }
                    }
                }
            }
            else
            {
                foreach (TreeNode _node in this._fieldsTreeCache.Nodes)
                {
                    treeViewCustom1.Nodes.Add((TreeNode)_node.Clone());
                }
            }
            //enables redrawing tree after all objects have been added
            this.treeViewCustom1.EndUpdate();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Utils.GetAllFilters(FileManager.GetFileFormats());
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (string file in ofd.FileNames)
                    OpenFile(file);

                Cursor.Current = Cursors.Default;
            }
        }

        private void OpenFile(string FileName)
        {
            object file = STFileLoader.OpenFileFormat(FileName);

            if (file is TreeNode)
            {
                var node = (TreeNode)file;
                AddNode(node);
            }
            else
            {
                STErrorDialog.Show("Invalid file type. Cannot add file to object list.", "Object List","");
            }
        }

        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewCustom1.Sort();
        }

        private void splitter1_Resize(object sender, EventArgs e)
        {
        }

        private void splitter1_LocationChanged(object sender, EventArgs e)
        {
        }

        private void stPanel1_Resize(object sender, EventArgs e)
        {
            Runtime.ObjectEditor.ListPanelWidth = stPanel1.Width;
        }

        private void treeViewCustom1_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void treeViewCustom1_DragOver(object sender, DragEventArgs e)
        {

        }
    }
}
