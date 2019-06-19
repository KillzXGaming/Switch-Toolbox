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
    public partial class ObjectEditorTree : UserControl
    {
        public ObjectEditor ObjectEditor;

        private TreeView _fieldsTreeCache;

        public void BeginUpdate() { treeViewCustom1.BeginUpdate(); }
        public void EndUpdate() { treeViewCustom1.EndUpdate(); }

        public readonly int MAX_TREENODE_VALUE = 1;

        public void AddIArchiveFile(IFileFormat FileFormat)
        {
            TreeNode FileRoot = new ArchiveRootNodeWrapper(FileFormat.FileName, (IArchiveFile)FileFormat);
            FillTreeNodes(FileRoot, (IArchiveFile)FileFormat);
            AddNode(FileRoot);
        }

        void FillTreeNodes(TreeNode root, IArchiveFile archiveFile)
        {
            var rootText = root.Text;
            var rootTextLength = rootText.Length;
            var nodeFiles = archiveFile.Files;

            int I = 0;
            foreach (var node in nodeFiles)
            {
                if (I++ == MAX_TREENODE_VALUE)
                    break;

                string nodeString = node.FileName;

                var roots = nodeString.Split(new char[] { '/' },
                    StringSplitOptions.RemoveEmptyEntries);

                // The initial parent is the root node
                var parentNode = root;
                var sb = new StringBuilder(rootText, nodeString.Length + rootTextLength);
                for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                {
                    // Build the node name
                    var parentName = roots[rootIndex];
                    sb.Append("/");
                    sb.Append(parentName);
                    var nodeName = sb.ToString();

                    // Search for the node
                    var index = parentNode.Nodes.IndexOfKey(nodeName);
                    if (index == -1)
                    {
                        // Node was not found, add it

                        var folder = new ArchiveFolderNodeWrapper(parentName, archiveFile);

                        if (rootIndex == roots.Length - 1)
                        {
                            ArchiveFileWrapper wrapperFile = new ArchiveFileWrapper(parentName, node, archiveFile);
                            wrapperFile.Name = nodeName;
                            parentNode.Nodes.Add(wrapperFile);
                            parentNode = wrapperFile;
                        }
                        else
                        {
                            folder.Name = nodeName;
                            parentNode.Nodes.Add(folder);
                            parentNode = folder;
                        }
                    }
                    else
                    {
                        // Node was found, set that as parent and continue
                        parentNode = parentNode.Nodes[index];
                    }
                }
            }
        }

        public void AddNodeCollection(TreeNodeCollection nodes, bool ClearNodes)
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

        public bool AddFilesToActiveEditor
        {
            get
            {
                return activeEditorChkBox.Checked;
            }
            set
            {
                activeEditorChkBox.Checked = value;
                Runtime.AddFilesToActiveObjectEditor = value;
            }
        }

        public ObjectEditorTree(ObjectEditor objectEditor)
        {
            InitializeComponent();

            ObjectEditor = objectEditor;

            _fieldsTreeCache = new TreeView();

            if (Runtime.ObjectEditor.ListPanelWidth > 0)
                stPanel1.Width = Runtime.ObjectEditor.ListPanelWidth;

            searchLbl.BackColor = stTextBox1.BackColor;

            treeViewCustom1.BackColor = FormThemes.BaseTheme.ObjectEditorBackColor;

            AddFilesToActiveEditor = Runtime.AddFilesToActiveObjectEditor;
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
            if (treeViewCustom1.Nodes[0] is ArchiveBase)
                return (IFileFormat)((ArchiveBase)treeViewCustom1.Nodes[0]).ArchiveFile;
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

        bool RenderedObjectWasSelected = false;
        private void treeViewCustom1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = treeViewCustom1.SelectedNode;

            //Set the current index used determine what bone is selected.
            //Update viewport for selection viewing
            if (node is STBone)
            {
                Runtime.SelectedBoneIndex = ((STBone)node).GetIndex();
            }
            else
                Runtime.SelectedBoneIndex = -1;

            //Set click events for custom nodes
            if (node is TreeNodeCustom)
            {
                ((TreeNodeCustom)node).OnClick(treeViewCustom1);
            }

            //Check if it is renderable for updating the viewport
            if (IsRenderable(node))
            {
                LibraryGUI.Instance.UpdateViewport();
                RenderedObjectWasSelected = true;
            }
            else
            {
                //Check if the object was previously selected
                //This will disable selection view and other things
                if (RenderedObjectWasSelected)
                {
                    LibraryGUI.Instance.UpdateViewport();
                    RenderedObjectWasSelected = false;
                }
            }
        }

        public bool IsRenderable(TreeNode obj)
        {
            if (obj is STGenericModel)
                return true;
            if (obj is STGenericObject)
                return true;
            if (obj is STBone)
                return true;
            if (obj is STSkeleton)
                return true;
            if (obj is STGenericMaterial)
                return true;

            return false;
        }

        private void treeViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewCustom1.SelectedNode is TreeNodeCustom)
            {
                ((TreeNodeCustom)treeViewCustom1.SelectedNode).OnDoubleMouseClick(treeViewCustom1);
            }
        }

        public List<Control> GetEditors()
        {
            List<Control> controls = new List<Control>();
            foreach (Control ctrl in stPanel2.Controls)
                controls.Add(ctrl);
            return controls;
        }

        public void FormClosing()
        {
            foreach (var control in stPanel2.Controls)
            {
                if (control is STUserControl)
                    ((STUserControl)control).OnControlClosing();
            }

            foreach (var node in TreeViewExtensions.Collect(treeViewCustom1.Nodes))
            {
                if (node is IFileFormat)
                {
                    ((IFileFormat)node).Unload();
                }
            }
            ClearNodes();
        }

        private void selectItem(object sender, TreeNodeMouseClickEventArgs e)
        {
            Viewport viewport = LibraryGUI.Instance.GetActiveViewport();

            if (viewport == null)
                return;

            if (e.Node is Animation)
            {
                if (((Animation)e.Node).Bones.Count <= 0)
                    ((Animation)e.Node).OpenAnimationData();

                string AnimName = e.Node.Text;
                AnimName = Regex.Match(AnimName, @"([A-Z][0-9][0-9])(.*)").Groups[0].ToString();
                if (AnimName.Length > 3)
                    AnimName = AnimName.Substring(3);

                Animation running = new Animation(AnimName);
                running.ReplaceMe((Animation)e.Node);
                running.Tag = e.Node;

                Queue<TreeNode> NodeQueue = new Queue<TreeNode>();
                foreach (TreeNode n in treeViewCustom1.Nodes)
                {
                    NodeQueue.Enqueue(n);
                }
                while (NodeQueue.Count > 0)
                {
                    try
                    {
                        TreeNode n = NodeQueue.Dequeue();
                        string NodeName = Regex.Match(n.Text, @"([A-Z][0-9][0-9])(.*)").Groups[0].ToString();
                        if (NodeName.Length <= 3)
                            Console.WriteLine(NodeName);
                        else
                            NodeName = NodeName.Substring(3);
                        if (n is Animation)
                        {
                            if (n == e.Node)
                                continue;
                            if (NodeName.Equals(AnimName))
                            {
                                running.Children.Add(n);
                            }
                        }
                        if (n is AnimationGroupNode)
                        {
                            foreach (TreeNode tn in n.Nodes)
                                NodeQueue.Enqueue(tn);
                        }
                    }
                    catch
                    {

                    }
                }

                if (LibraryGUI.Instance.GetAnimationPanel() != null)
                {
                    LibraryGUI.Instance.GetAnimationPanel().CurrentAnimation = running;
                }
            }
        }

        public void RemoveFile(TreeNode File)
        {
            if (File is IFileFormat)
            {
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

        bool UpdateViewport = false;
        bool IsModelChecked = false;
        private void treeViewCustom1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            UpdateViewport = false;

            if (e.Node is STGenericModel)
            {
                IsModelChecked = true;
                CheckChildNodes(e.Node, e.Node.Checked);
                IsModelChecked = false;
            }
            else if (e.Node is STGenericObject && !IsModelChecked)
            {
                UpdateViewport = true;
            }
            else if (e.Node is STBone && !IsModelChecked)
            {
                UpdateViewport = true;
            }

            if (UpdateViewport)
            {
                LibraryGUI.Instance.UpdateViewport();
            }
        }

        private void CheckChildNodes(TreeNode node, bool IsChecked)
        {
            foreach (TreeNode n in node.Nodes)
            {
                n.Checked = IsChecked;
                if (n.Nodes.Count > 0)
                {
                    CheckChildNodes(n, IsChecked);
                }
            }

            UpdateViewport = true; //Update viewport on the last node checked
        }

        private void treeViewCustom1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;

            bool IsCheckable = (e.Node is STGenericObject || e.Node is STGenericModel
                                                          || e.Node is STBone);

            if (!IsCheckable)
                TreeViewExtensions.HideCheckBox(e.Node);
        }

        private void treeViewCustom1_MouseClick(object sender, MouseEventArgs e)
        {
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
                STErrorDialog.Show("Invalid file type. Cannot add file to object list.", "Object List", "");
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

        private void activeEditorChkBox_CheckedChanged(object sender, EventArgs e)
        {
            AddFilesToActiveEditor = activeEditorChkBox.Checked;
        }


        private void treeViewCustom1_DragDrop(object sender, DragEventArgs e)
        {
            Point pt = treeViewCustom1.PointToClient(new Point(e.X, e.Y));
            treeViewCustom1.SelectedNode = treeViewCustom1.GetNodeAt(pt.X, pt.Y);
            bool IsFile = treeViewCustom1.SelectedNode is ArchiveFileWrapper && treeViewCustom1.SelectedNode.Parent != null;

            var archiveFile = GetActiveArchive();

            //Use the parent folder for files if it has any
            if (IsFile)
                TreeHelper.AddFiles(treeViewCustom1.SelectedNode.Parent, archiveFile, e.Data.GetData(DataFormats.FileDrop) as string[]);
            else
                TreeHelper.AddFiles(treeViewCustom1.SelectedNode, archiveFile, e.Data.GetData(DataFormats.FileDrop) as string[]);
        }

        private void treeViewCustom1_DragOver(object sender, DragEventArgs e)
        {
            var file = GetActiveArchive();
            if (file == null || !file.CanReplaceFiles)
                return;

            Point pt = treeViewCustom1.PointToClient(new Point(e.X, e.Y));
            TreeNode node = treeViewCustom1.GetNodeAt(pt.X, pt.Y);
            treeViewCustom1.SelectedNode = node;
            bool IsRoot = node is ArchiveRootNodeWrapper;
            bool IsFolder = node is ArchiveFolderNodeWrapper;
            bool IsFile = node is ArchiveFileWrapper && node.Parent != null;

            if (IsFolder || IsRoot || IsFile)
                e.Effect = DragDropEffects.Link;
         //   else
             //   e.Effect = DragDropEffects.None;
        }

        private IArchiveFile GetActiveArchive()
        {
            if (treeViewCustom1.SelectedNode != null && treeViewCustom1.SelectedNode is ArchiveBase)
                return ((ArchiveBase)treeViewCustom1.SelectedNode).ArchiveFile;

            return null;
        }
    }
}
