using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using System.Text.RegularExpressions;
using Toolbox.Library.Animations;
using Toolbox.Library.IO;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Toolbox.Library.Forms
{
    public partial class ObjectEditorTree : UserControl
    {
        private bool SuppressAfterSelectEvent = false;

        private bool IsSearchPanelDocked
        {
            get
            {
                return dockSearchListToolStripMenuItem.Checked;
            }
            set
            {
                dockSearchListToolStripMenuItem.Checked = value;
            }
        }

        private enum TreeNodeSize
        {
            Small,
            Normal,
            Large,
            ExtraLarge,
        }

        public ObjectEditor ObjectEditor;

        public void BeginUpdate() { treeViewCustom1.BeginUpdate(); }
        public void EndUpdate() { treeViewCustom1.EndUpdate(); }

        public void ReloadArchiveFile(IFileFormat fileFormat)
        {
            this.treeViewCustom1.Nodes.Clear();
            AddIArchiveFile(fileFormat);
        }

        public void AddIArchiveFile(IFileFormat FileFormat)
        {
            var FileRoot = new ArchiveRootNodeWrapper(FileFormat.FileName, (IArchiveFile)FileFormat);
            FileRoot.FillTreeNodes();
            AddNode(FileRoot);

            if (FileFormat is TreeNode) //It can still be both, so add all it's nodes
            {
                foreach (TreeNode n in ((TreeNode)FileFormat).Nodes)
                    FileRoot.Nodes.Add(n);
            }

            if (FileFormat is IArchiveQuickAccess)
            {
                var lookup = ((IArchiveQuickAccess)FileFormat).CategoryLookup;

                TreeNode quickAcessNode = new TreeNode("Quick access");
                AddNode(quickAcessNode);

                Dictionary<string, TreeNode> folders = new Dictionary<string, TreeNode>();
                for (int i = 0; i < FileRoot.FileNodes.Count; i++)
                {
                    string fileName = FileRoot.FileNodes[i].Item1.FileName;
                    string name = System.IO.Path.GetFileName(fileName);
                    string ext = Utils.GetExtension(fileName);

                    var fileNode = FileRoot.FileNodes[i].Item2;

                    string folder = "Other";
                    if (lookup.ContainsKey(ext))
                        folder = lookup[ext];

                    if (!folders.ContainsKey(folder)) {
                        var dirNode = new TreeNode(folder);
                        folders.Add(folder, dirNode);
                        quickAcessNode.Nodes.Add(dirNode);
                    }

                    folders[folder].Nodes.Add(new TreeNode()
                    {
                        Tag = fileNode,
                        Text = name,
                        ImageKey = fileNode.ImageKey,
                        SelectedImageKey = fileNode.SelectedImageKey,
                    });
                    // dirNode.Nodes.Add(FileRoot.FileNodes[i].Item2);
                }
            }

            SelectNode(FileRoot);
            for (int i = 0; i < FileRoot.FileNodes.Count; i++)
            {
                if (FileRoot.FileNodes[i].Item1.OpenFileFormatOnLoad)
                {
                    if (FileRoot.FileNodes[i].Item2 is ArchiveFileWrapper)
                    {
                        try {
                            ((ArchiveFileWrapper)FileRoot.FileNodes[i].Item2).OpenFileFormat(treeViewCustom1);
                        }
                        catch
                        {

                        }
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
        }

        public void SelectFirstNode() { treeViewCustom1.SelectedNode = treeViewCustom1.Nodes[0]; }

        private void AddNodes(TreeNode node, bool ClearAllNodes = false)
        {
            treeViewCustom1.BeginUpdate(); // No visual updates until we say 
            if (ClearAllNodes)
                ClearNodes();
            treeViewCustom1.Nodes.Add(node); // Add the new nodes
            treeViewCustom1.EndUpdate(); // Allow the treeview to update visually

            if (node is ISingleTextureIconLoader) {
                LoadGenericTextureIcons((ISingleTextureIconLoader)node);
            }
        }

        public void ClearNodes()
        {
            treeViewCustom1.Nodes.Clear();
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

            btnPanelDisplay.ForeColor = FormThemes.BaseTheme.DisabledBorderColor;

            UpdateSearchPanelDockState();

            ObjectEditor = objectEditor;

            if (Runtime.ObjectEditor.ListPanelWidth > 0)
                splitContainer1.Panel1.Width = Runtime.ObjectEditor.ListPanelWidth;

            treeViewCustom1.BackColor = FormThemes.BaseTheme.ObjectEditorBackColor;

            AddFilesToActiveEditor = Runtime.AddFilesToActiveObjectEditor;

            foreach (TreeNodeSize nodeSize in (TreeNodeSize[])Enum.GetValues(typeof(TreeNodeSize)))
                nodeSizeCB.Items.Add(nodeSize);

            nodeSizeCB.SelectedIndex = 1;
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
            if (SuppressAfterSelectEvent)
                return;

            var node = treeViewCustom1.SelectedNode;

            if (node is Animation || node is IAnimationContainer) {
                OnAnimationSelected(node);
            }

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

            if (node.Tag != null && node.Tag is TreeNodeCustom)
                ((TreeNodeCustom)node.Tag).OnClick(treeViewCustom1);

            //Check if it is renderable for updating the viewport
            if (IsRenderable(node))
            {
                LibraryGUI.UpdateViewport();
                RenderedObjectWasSelected = true;
            }
            else
            {
                //Check if the object was previously selected
                //This will disable selection view and other things
                if (RenderedObjectWasSelected)
                {
                    LibraryGUI.UpdateViewport();
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
            if (treeViewCustom1.SelectedNode == null) return;

            if (treeViewCustom1.SelectedNode is TreeNodeCustom)
            {
                ((TreeNodeCustom)treeViewCustom1.SelectedNode).OnDoubleMouseClick(treeViewCustom1);
            }
            if (treeViewCustom1.SelectedNode.Tag != null && treeViewCustom1.SelectedNode.Tag is TreeNodeCustom)
            {
                ((TreeNodeCustom)treeViewCustom1.SelectedNode.Tag).OnDoubleMouseClick(treeViewCustom1);
            }
        }

        public void UpdateTextureIcon(ISingleTextureIconLoader texturIcon, Image image) {
            treeViewCustom1.ReloadTextureIcons(texturIcon, image);
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
            if (searchForm != null)
            {
                searchForm.OnControlClosing();
                searchForm.Dispose();
            }

            foreach (var control in stPanel2.Controls)
            {
                if (control is STUserControl)
                    ((STUserControl)control).OnControlClosing();
            }

            foreach (var node in TreeViewExtensions.Collect(treeViewCustom1.Nodes))
            {
                if (node is ArchiveRootNodeWrapper)
                {
                    var file = ((ArchiveRootNodeWrapper)node).ArchiveFile;
                    ((IFileFormat)file).Unload();
                }
                else if (node is IFileFormat)
                {
                    ((IFileFormat)node).Unload();
                }
            }
            ClearNodes();
        }

        private ToolStripItem[] GetArchiveMenus(TreeNode node, ArchiveFileInfo info)
        {
            return info.FileWrapper.GetContextMenuItems();
        }

        private void treeViewCustom1_MouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeNodeContextMenu.Items.Clear();

                var menuItems = GetMenuItems(e.Node);
                treeNodeContextMenu.Items.AddRange(menuItems);

                //Select the node without the evemt
                //We don't want editors displaying on only right clicking
                SuppressAfterSelectEvent = true;
                treeViewCustom1.SelectedNode = e.Node;
                SuppressAfterSelectEvent = false;

                if (treeNodeContextMenu.Items.Count > 0)
                    treeNodeContextMenu.Show(Cursor.Position);
            }
            else
            {
            }
        }

        private ToolStripItem[] GetMenuItems(TreeNode selectednode)
        {
            List<ToolStripItem> archiveMenus = new List<ToolStripItem>();
            List<ToolStripItem> menuItems = new List<ToolStripItem>();

            Console.WriteLine($"tag {selectednode.Tag }");
            if (selectednode.Tag != null && selectednode.Tag is ArchiveFileInfo)
            {
                //The tag gets set when an archive is replaced by a treenode
                //Todo store this in a better place as devs could possiblly replace this
                //Create menus when an archive node is replaced
                archiveMenus.AddRange(GetArchiveMenus(selectednode, (ArchiveFileInfo)selectednode.Tag));
                Console.WriteLine($"archiveMenus {archiveMenus.Count}");
            }

            if (selectednode is IExportableModel)
            {
                menuItems.Add(new ToolStripMenuItem("Export Model", null, ExportModelAction, Keys.Control | Keys.E));
            }

            bool IsRoot = selectednode.Parent == null;
            bool HasChildren = selectednode.Nodes.Count > 0;

            IContextMenuNode node = null;
            if (selectednode is IContextMenuNode)
            {
                node = (IContextMenuNode)selectednode;
            }
            else if (selectednode.Tag != null && selectednode.Tag is IContextMenuNode)
            {
                node = (IContextMenuNode)selectednode.Tag;
            }

            if (selectednode is IAnimationContainer)
            {
                var anim = ((IAnimationContainer)selectednode).AnimationController;
                if (anim is IContextMenuNode)
                    node = (IContextMenuNode)anim;
            }

            if (node != null)
            {
                if (IsRoot)
                {
                    foreach (var item in node.GetContextMenuItems())
                    {
                        if (item.Text != "Delete" && item.Text != "Remove")
                            menuItems.Add(item);
                    }
                    menuItems.Add(new ToolStripMenuItem("Delete", null, DeleteAction, Keys.Delete));
                }
                else
                {
                    menuItems.AddRange(node.GetContextMenuItems());
                }

                bool HasCollpase = false;
                bool HasExpand = false;
                foreach (var item in node.GetContextMenuItems())
                {
                    if (item.Text == "Collapse All")
                        HasCollpase = true;
                    if (item.Text == "Expand All")
                        HasExpand = true;
                }

                if (!HasCollpase && HasChildren)
                    menuItems.Add(new ToolStripMenuItem("Collapse All", null, CollapseAllAction, Keys.Control | Keys.Q));

                if (!HasExpand && HasChildren)
                    menuItems.Add(new ToolStripMenuItem("Expand All", null, ExpandAllAction, Keys.Control | Keys.P));
            }

            if (archiveMenus.Count > 0)
            {
                if (menuItems.Count > 0)
                {
                    STToolStipMenuItem archiveItem = new STToolStipMenuItem("Archive");
                    treeNodeContextMenu.Items.Add(archiveItem);

                    foreach (var item in archiveMenus)
                        archiveItem.DropDownItems.Add(item);
                }
                else
                {
                    if (archiveMenus.Count > 0)
                        treeNodeContextMenu.Items.AddRange(archiveMenus.ToArray());
                }
            }

            var fileFormat = TryGetActiveFile(selectednode);
            if (fileFormat != null)
            {
                string path = fileFormat.FilePath;
                if (File.Exists(path))
                    menuItems.Add(new ToolStripMenuItem("Open In Explorer", null, SelectFileInExplorer, Keys.Control | Keys.Q));
            }

            Keys currentKey = Keys.A;
            List<Keys> shortcuts = new List<Keys>();
            foreach (ToolStripItem item in menuItems)
            {
                if (item is ToolStripMenuItem) {
                    var menu = item as ToolStripMenuItem;
                    CheckDuplicateShortcuts(menu, currentKey, shortcuts);
                }
            }

            return menuItems.ToArray();
        }

        private void CheckDuplicateShortcuts(ToolStripMenuItem menu, Keys current, List<Keys> shortcuts)
        {
            if (menu.ShowShortcutKeys)
            {
                if (!shortcuts.Contains(menu.ShortcutKeys))
                    shortcuts.Add(menu.ShortcutKeys);
                else
                {
                    //Auto set the key
                    var controlKey = Keys.Control | current;
                    if (!shortcuts.Contains(controlKey))
                    {
                        shortcuts.Add(controlKey);
                        menu.ShortcutKeys = controlKey;
                    }
                    else
                    {
                        menu.ShortcutKeys = Keys.Control | current++;
                        CheckDuplicateShortcuts(menu, current, shortcuts);
                    }
                }
            }
        }

        private IFileFormat TryGetActiveFile(TreeNode node)
        {
            if (node.Tag != null && node.Tag is IFileFormat)
                return (IFileFormat)node.Tag;
            else if (node is IFileFormat)
                return (IFileFormat)node;
            else if (node is ArchiveRootNodeWrapper)
            {
                if (((ArchiveRootNodeWrapper)node).ArchiveFile is IFileFormat)
                    return ((ArchiveRootNodeWrapper)node).ArchiveFile as IFileFormat;
                else
                    return null;
            }
            else
                return null;
        }

        private void ExportModelAction(object sender, EventArgs args)
        {
            var node = treeViewCustom1.SelectedNode as IExportableModel;
            if (node != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Supported Formats|*.dae;";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportModelSettings exportDlg = new ExportModelSettings();
                    if (exportDlg.ShowDialog() == DialogResult.OK)
                        ExportModel(node, sfd.FileName, exportDlg.Settings);
                }
            }
        }

        public void ExportModel(IExportableModel exportableModel, string fileName, DAE.ExportSettings settings)
        {
            var model = new STGenericModel();
            model.Materials = exportableModel.ExportableMaterials;
            model.Objects = exportableModel.ExportableMeshes;
            var textures = new List<STGenericTexture>();
            foreach (var tex in exportableModel.ExportableTextures)
                textures.Add(tex);

            DAE.Export(fileName, settings, model, textures, exportableModel.ExportableSkeleton);
        }

        private void SelectFileInExplorer(object sender, EventArgs args)
        {
            var node = treeViewCustom1.SelectedNode;
            if (node != null) {
                var fileFormat = TryGetActiveFile(node);
                string argument = "/select, \"" + fileFormat.FilePath + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }

        private void ExpandAllAction(object sender, EventArgs args)
        {
            var node = treeViewCustom1.SelectedNode;
            if (node != null)
                node.ExpandAll();
        }

        private void CollapseAllAction(object sender, EventArgs args)
        {
            var node = treeViewCustom1.SelectedNode;
            if (node != null)
                node.Collapse();
        }

        private void DeleteAction(object sender, EventArgs args)
        {
            var node = treeViewCustom1.SelectedNode;
            if (node != null)
            {
                var result = MessageBox.Show("If you remove this file, any unsaved progress will be lost! Continue?",
                    "Remove Dialog", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (node is IFileFormat)
                    {
                        ((IFileFormat)node).Unload();
                    }

                    treeViewCustom1.Nodes.Remove(node);

                    if (treeViewCustom1.Nodes.Count == 0)
                        ResetEditor();

                    //Force garbage collection.
                    GC.Collect();

                    // Wait for all finalizers to complete before continuing.
                    GC.WaitForPendingFinalizers();

                    ((IUpdateForm)Runtime.MainForm).UpdateForm();
                }
            }
        }

        private void ResetEditor()
        {
            foreach (Control control in stPanel2.Controls)
            {
                if (control is STUserControl)
                    ((STUserControl)control).OnControlClosing();
            }

            stPanel2.Controls.Clear();
        }

        private void OnAnimationSelected(TreeNode Node)
        {
            if (Node is Animation)
            {
                Viewport viewport = LibraryGUI.GetActiveViewport();
                if (viewport == null)
                    return;

                if (((Animation)Node).Bones.Count <= 0)
                    ((Animation)Node).OpenAnimationData();

                string AnimName = Node.Text;
                AnimName = Regex.Match(AnimName, @"([A-Z][0-9][0-9])(.*)").Groups[0].ToString();
                if (AnimName.Length > 3)
                    AnimName = AnimName.Substring(3);

                Animation running = new Animation(AnimName);
                running.ReplaceMe((Animation)Node);
                running.Tag = Node;

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
                            if (n == Node)
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

                if (LibraryGUI.GetAnimationPanel() != null)
                {
                    LibraryGUI.GetAnimationPanel().CurrentAnimation = running;
                }
            }
            if (Node is IAnimationContainer)
            {
                Viewport viewport = LibraryGUI.GetActiveViewport();
                if (viewport == null)
                    return;

                var running = ((IAnimationContainer)Node).AnimationController;
                if (LibraryGUI.GetAnimationPanel() != null) {
                    Console.WriteLine($"running {running.Name}");

                    LibraryGUI.GetAnimationPanel().CurrentSTAnimation = running;
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
            Text = "";

            ResetEditor();
        }

        bool UpdateViewport = false;
        bool SupressUpdateEvent = false;
        private void treeViewCustom1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            UpdateViewport = false;

            if (e.Node is STGenericModel)
            {
                SupressUpdateEvent = true;
                CheckChildNodes(e.Node, e.Node.Checked);
                SupressUpdateEvent = false;
            }

            if (Control.ModifierKeys == Keys.Shift && !SupressUpdateEvent)
            {
                SupressUpdateEvent = true;
                CheckChildNodes(e.Node, e.Node.Checked);
                SupressUpdateEvent = false;
            }

            if (e.Node is STGenericObject && !SupressUpdateEvent)
            {
                UpdateViewport = true;
            }
            else if (e.Node is STBone && !SupressUpdateEvent)
            {
                UpdateViewport = true;
            }

            if (UpdateViewport)
            {
                LibraryGUI.UpdateViewport();
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            AddNewFile();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            AddNewFile();
        }

        private void AddNewFile()
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
            else if (file is IArchiveFile)
            {
                AddIArchiveFile((IFileFormat)file);
            }
            else
            {
                STErrorDialog.Show("Invalid file type. Cannot add file to object list.", "Object List", "");
            }

            ((IUpdateForm)Runtime.MainForm).UpdateForm();
        }

        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewCustom1.Sort();
        }

        public void SortTreeAscending()
        {
            treeViewCustom1.Sort();
        }

        public void SelectNode(TreeNode node)
        {
            treeViewCustom1.SelectedNode = node;
        }

        private void splitter1_Resize(object sender, EventArgs e)
        {
        }

        private void splitter1_LocationChanged(object sender, EventArgs e)
        {
        }

        private void activeEditorChkBox_CheckedChanged(object sender, EventArgs e)
        {
            AddFilesToActiveEditor = activeEditorChkBox.Checked;
            Console.WriteLine("AddFilesToActiveObjectEditor " + Runtime.AddFilesToActiveObjectEditor);
        }

        private void treeViewCustom1_DragEnter(object sender, DragEventArgs e)
        {
            if (!Runtime.EnableDragDrop) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
            {
                String[] strGetFormats = e.Data.GetFormats();
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = treeViewCustom1.SelectedNode;
            if (node == null) return;

            if (node is ArchiveFileWrapper)
            {
                treeViewCustom1.DoDragDrop("dummy", DragDropEffects.Copy);
            }
            else if (node is ArchiveFolderNodeWrapper || node is ArchiveRootNodeWrapper)
            {
                treeViewCustom1.DoDragDrop("dummy", DragDropEffects.Copy);
            }
        }

        private void DropFileOutsideApplication(TreeNode node)
        {
            if (node is ArchiveFileWrapper)
            {
                Runtime.EnableDragDrop = false;

                string fullPath = Write2TempAndGetFullPath(((ArchiveFileWrapper)node).ArchiveFileInfo);

                DataObject dragObj = new DataObject();
                dragObj.SetFileDropList(new System.Collections.Specialized.StringCollection() { fullPath });
                treeViewCustom1.DoDragDrop(dragObj, DragDropEffects.Copy);

                Runtime.EnableDragDrop = true;
            }
            else if (node is ArchiveFolderNodeWrapper || node is ArchiveRootNodeWrapper)
            {
                Runtime.EnableDragDrop = false;

                string[] fullPaths = Write2TempAndGetFullPath(node);

                DataObject dragObj = new DataObject();
                var collection = new System.Collections.Specialized.StringCollection();
                collection.AddRange(fullPaths);
                dragObj.SetFileDropList(collection);
                treeViewCustom1.DoDragDrop(dragObj, DragDropEffects.Copy);

                Runtime.EnableDragDrop = true;
            }
        }

        private string[] Write2TempAndGetFullPath(TreeNode folder)
        {
            var ParentPath = string.Empty;
            if (folder.Parent != null)
                ParentPath = folder.Parent.FullPath;

            return TreeHelper.ExtractAllFiles(ParentPath, folder.Nodes, System.IO.Path.GetTempPath());
        }

        private string Write2TempAndGetFullPath(ArchiveFileInfo file)
        {
            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), file.FileName);
            using (var writer = new FileStream(tempFilePath, 
                           FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                new MemoryStream(file.FileData).CopyTo(writer);
            }

            return tempFilePath;
        }


        private void treeViewCustom1_DragLeave(object sender, EventArgs e)
        {
        }

        private void treeViewCustom1_DragOver(object sender, DragEventArgs e)
        { 
        }

        private void treeViewCustom1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
     
        }

        private void treeViewCustom1_DragDrop(object sender, DragEventArgs e)
        {
            if (!Runtime.EnableDragDrop) return;

            Console.WriteLine("test");

            if (e.Effect == DragDropEffects.Copy)
            {
                Console.WriteLine("drop");

                DropFileOutsideApplication(treeViewCustom1.SelectedNode);
            }
            else if (e.Effect == DragDropEffects.All)
            {
                Point pt = treeViewCustom1.PointToClient(new Point(e.X, e.Y));
                var node = treeViewCustom1.GetNodeAt(pt.X, pt.Y);

                if (node != null)
                {
                    treeViewCustom1.SelectedNode = node;

                    bool IsRoot = node is ArchiveRootNodeWrapper;
                    bool IsFolder = node is ArchiveFolderNodeWrapper;
                    bool IsFile = node is ArchiveFileWrapper && node.Parent != null;

                    if (IsRoot || IsFolder || IsFile)
                    {
                        var archiveFile = GetActiveArchive();

                        //Use the parent folder for files if it has any
                        if (IsFile)
                            TreeHelper.AddFiles(treeViewCustom1.SelectedNode.Parent, archiveFile, e.Data.GetData(DataFormats.FileDrop) as string[]);
                        else
                            TreeHelper.AddFiles(treeViewCustom1.SelectedNode, archiveFile, e.Data.GetData(DataFormats.FileDrop) as string[]);
                    }
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;

                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (string filename in files)
                    {
                        ((IMainForm)Runtime.MainForm).OpenFile(filename, Runtime.ObjectEditor.OpenModelsOnOpen);
                    }

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private ArchiveRootNodeWrapper GetActiveArchive()
        {
            var node = treeViewCustom1.SelectedNode;
            if (node != null && node is ArchiveRootNodeWrapper)
                return (ArchiveRootNodeWrapper)node;
            if (node != null && node is ArchiveFileWrapper)
                return ((ArchiveFileWrapper)node).RootNode;
            if (node != null && node is ArchiveFolderNodeWrapper)
                return ((ArchiveFolderNodeWrapper)node).RootNode;

            return null;
        }

        private void treeViewCustom1_KeyPress(object sender, KeyEventArgs e)
        {
            if (treeViewCustom1.SelectedNode == null) return;

            var Items = GetMenuItems(treeViewCustom1.SelectedNode);
            foreach (ToolStripItem toolstrip in Items)
            {
                if (toolstrip is ToolStripMenuItem)
                {
                    if (((ToolStripMenuItem)toolstrip).ShortcutKeys == e.KeyData)
                        toolstrip.PerformClick();
                }
            }
        }

        private SearchNodePanel searchForm;
        private void searchFormToolStrip_Click(object sender, EventArgs e)
        {
            searchForm = new SearchNodePanel(treeViewCustom1);
            searchForm.Dock = DockStyle.Fill;
            STForm form = new STForm();

            var panel = new STPanel() { Dock = DockStyle.Fill };
            panel.Controls.Add(searchForm);
            form.AddControl(panel);
            form.Text = "Search Window";
            form.Show(this);
        }

        private void dockSearchListToolStripMenuItem_Click(object sender, EventArgs e) {
            UpdateSearchPanelDockState();
        }

        private void UpdateSearchPanelDockState()
        {
            if (IsSearchPanelDocked)
            {
                splitContainer1.Panel1Collapsed = false;
                splitContainer1.Panel1.Controls.Clear();

                searchForm = new SearchNodePanel(treeViewCustom1);
                searchForm.Dock = DockStyle.Fill;
                splitContainer1.Panel1.Controls.Add(searchForm);
            }
            else
            {
                splitContainer1.Panel1Collapsed = true;
            }
        }

        public void LoadGenericTextureIcons(TreeNodeCollection nodes) {
            List<ISingleTextureIconLoader> texIcons = new List<ISingleTextureIconLoader>();
            foreach (var node in nodes)
            {
                if (node is ISingleTextureIconLoader)
                {
                    treeViewCustom1.SingleTextureIcons.Add((ISingleTextureIconLoader)node);
                    texIcons.Add((ISingleTextureIconLoader)node);
                }
            }

            if (texIcons.Count > 0)
                treeViewCustom1.ReloadTextureIcons(texIcons, false);
        }

        public void LoadGenericTextureIcons(ITextureContainer iconList) {
            treeViewCustom1.TextureIcons.Add(iconList);
            treeViewCustom1.ReloadTextureIcons(iconList);
        }

        public void LoadGenericTextureIcons(ISingleTextureIconLoader iconTex) {
            treeViewCustom1.SingleTextureIcons.Add(iconTex);
            treeViewCustom1.ReloadTextureIcons(iconTex, false);
        }

        private void nodeSizeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            var nodeSize = nodeSizeCB.SelectedItem;
            if (nodeSize != null)
            {
                int Size = 22;

                switch ((TreeNodeSize)nodeSize)
                {
                    case TreeNodeSize.Small: Size = 18;
                        break;
                    case TreeNodeSize.Normal: Size = 22;
                        break;
                    case TreeNodeSize.Large: Size = 30;
                        break;
                    case TreeNodeSize.ExtraLarge: Size = 35;
                        break;
                }

                treeViewCustom1.BeginUpdate();
                treeViewCustom1.ItemHeight = Size;
                treeViewCustom1.ReloadImages(Size, Size);
                treeViewCustom1.ReloadTextureIcons(true);
                treeViewCustom1.EndUpdate();
            }
        }

        private void treeViewCustom1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null) return;

            else if (e.Node is ITextureContainer) {
                treeViewCustom1.BeginUpdate();
                LoadGenericTextureIcons((ITextureContainer)e.Node);
                treeViewCustom1.EndUpdate();
            }
            else if (e.Node is ISingleTextureIconLoader) {
                LoadGenericTextureIcons((ISingleTextureIconLoader)e.Node);
            }
            else if (e.Node is ArchiveFolderNodeWrapper) {
                LoadGenericTextureIcons(e.Node.Nodes);
            }
            else if (e.Node is ExplorerFolder)
            {
                treeViewCustom1.BeginUpdate();
                ((ExplorerFolder)e.Node).OnBeforeExpand();
                treeViewCustom1.EndUpdate();
            }
            else if (e.Node is TreeNodeCustom)
            {
                treeViewCustom1.BeginUpdate();
                ((TreeNodeCustom)e.Node).OnExpand();
                treeViewCustom1.EndUpdate();
            }
        }

        private void treeViewCustom1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            if (e.Node is ExplorerFolder)
            {
                treeViewCustom1.BeginUpdate();
                ((ExplorerFolder)e.Node).OnAfterCollapse();
                treeViewCustom1.EndUpdate();
            }
        }

        private bool DisplayEditor = true;
        private void btnPanelDisplay_Click(object sender, EventArgs e)
        {
            if (DisplayEditor) {
                splitContainer2.Panel1Collapsed = true;
                splitContainer2.Panel1.Hide();
                DisplayEditor = false;
                btnPanelDisplay.Text = ">";
            }
            else {
                splitContainer2.Panel1Collapsed = false;
                splitContainer2.Panel1.Show();
                DisplayEditor = true;
                btnPanelDisplay.Text = "<";
            }
        }

        private void splitContainer1_Panel1_Resize(object sender, EventArgs e) {
            Runtime.ObjectEditor.ListPanelWidth = splitContainer1.Panel1.Width;
        }
    }
}
