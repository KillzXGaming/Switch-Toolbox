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
    public partial class ObjectEditor : STForm
    {
        public static ObjectEditor Instance
        {
            get { return _instance != null ? _instance : (_instance = new ObjectEditor()); }
        }
        private static ObjectEditor _instance;

        public ObjectEditor()
        {
            InitializeComponent();

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

        public List<EditableObject> editableGlDrawables = new List<EditableObject>();
        public List<AbstractGlDrawable> staticGlDrawables = new List<AbstractGlDrawable>();

        public static void AddObject(EditableObject drawable)
        {
            var editor = LibraryGUI.Instance.GetObjectEditor();
            if (editor == null)
                return;

            editor.editableGlDrawables.Add(drawable);
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
                if (RenderedObjectWasSelected == true)
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

            return false;
        }

        private void treeViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewCustom1.SelectedNode is TreeNodeCustom)
            {
                ((TreeNodeCustom)treeViewCustom1.SelectedNode).OnDoubleMouseClick(treeViewCustom1);
            }
        }

        private void ObjectEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Viewport viewport = LibraryGUI.Instance.GetActiveViewport();

            if (viewport != null)
                viewport.FormClosing();

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
            treeViewCustom1.Nodes.Clear();
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

        bool UpdateViewport = false;
        private void treeViewCustom1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            UpdateViewport = false;

            if (e.Node is STGenericModel)
            {
                CheckChildNodes(e.Node, e.Node.Checked);
            }

            if (UpdateViewport)
                LibraryGUI.Instance.UpdateViewport();
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

            bool IsCheckable = (e.Node is STGenericObject || e.Node is STGenericModel);

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
                treeViewCustom1.Nodes.Add(node);
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
    }
}
