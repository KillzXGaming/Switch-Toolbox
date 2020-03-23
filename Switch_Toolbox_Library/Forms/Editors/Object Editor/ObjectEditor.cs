using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using System.Text.RegularExpressions;
using Toolbox.Library.Animations;
using Toolbox.Library.IO;

namespace Toolbox.Library.Forms
{
    public partial class ObjectEditor : STForm
    {
        private ObjectEditorTree ObjectTree;
        private ObjectEditorList ObjectList; //Optionally usable for archives

        public void BeginUpdate() { ObjectTree.BeginUpdate(); }
        public void EndUpdate() { ObjectTree.EndUpdate(); }

        public void AddNodeCollection (TreeNodeCollection nodes, bool ClearNodes)
        {
            if (ObjectTree != null) {
                ObjectTree.AddNodeCollection(nodes,ClearNodes);
            }
        }

        public TreeNodeCollection GetNodes() { return ObjectTree.GetNodes(); }

        public void AddNode(TreeNode node, bool ClearAllNodes = false)
        {
            if (ObjectTree != null) {
                ObjectTree.AddNode(node, ClearAllNodes);
            }
        }

        private void AddNodes(TreeNode node, bool ClearAllNodes = false)
        {
            if (ObjectTree != null) {
                ObjectTree.AddNode(node, ClearAllNodes);
            }
        }

        public void ClearNodes()
        {
            if (ObjectTree != null) {
                ObjectTree.ClearNodes();
            }
        }

        public bool AddFilesToActiveEditor
        {
            get
            {
                return ObjectTree.AddFilesToActiveEditor;
            }
            set
            {
                ObjectTree.AddFilesToActiveEditor = value;
            }
        }

        public bool UseListView = true;

        public ObjectEditor()
        {
            InitializeComponent();

            ObjectTree = new ObjectEditorTree(this);
            ObjectTree.Dock = DockStyle.Fill;
            stPanel1.Controls.Add(ObjectTree);
        }

        public ObjectEditor(IFileFormat FileFormat)
        {
            InitializeComponent();

            if (FileFormat is IArchiveFile)
            {
                /* ObjectList = new ObjectEditorList();
                 ObjectList.Dock = DockStyle.Fill;
                 stPanel1.Controls.Add(ObjectList);
                 ObjectList.FillList((IArchiveFile)FileFormat);*/

                ObjectTree = new ObjectEditorTree(this);
                ObjectTree.Dock = DockStyle.Fill;
                stPanel1.Controls.Add(ObjectTree);
                AddIArchiveFile(FileFormat);
            }
            else
            {
                ObjectTree = new ObjectEditorTree(this);
                ObjectTree.Dock = DockStyle.Fill;
                stPanel1.Controls.Add(ObjectTree);
                AddNode((TreeNode)FileFormat);
            }

            if (FileFormat is ITextureContainer) {
                ObjectTree.LoadGenericTextureIcons((ITextureContainer)FileFormat);
            }
        }

        public void SelectFirstNode() { if (ObjectTree != null) ObjectTree.SelectFirstNode(); }
        public void SelectNode(TreeNode node) { if (ObjectTree != null) ObjectTree.SelectNode(node); }

        public void SortTreeAscending() { if (ObjectTree != null) ObjectTree.SortTreeAscending(); }

        public void UpdateTextureIcon(ISingleTextureIconLoader texturIcon) {
            ObjectTree.LoadGenericTextureIcons(texturIcon);
        }

        public void UpdateTextureIcon(ISingleTextureIconLoader texturIcon, Image image) {
            ObjectTree.UpdateTextureIcon(texturIcon,image );
        }

        public void ReloadArchiveFile(IFileFormat FileFormat) {
            ObjectTree.ReloadArchiveFile(FileFormat);
        }

        public void AddIArchiveFile(IFileFormat FileFormat){
            ObjectTree.AddIArchiveFile(FileFormat);
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

        public List<DrawableContainer> DrawableContainers = new List<DrawableContainer>();

        public static List<DrawableContainer> GetDrawableContainers()
        {
            var editor = LibraryGUI.GetObjectEditor();
            if (editor == null)
                return new List<DrawableContainer>();

           return editor.DrawableContainers;
        }

        public static void AddContainer(DrawableContainer drawable)
        {
            var editor = LibraryGUI.GetObjectEditor();
            if (editor == null)
                return;

            editor.DrawableContainers.Add(drawable);
        }

        public static void RemoveContainer(DrawableContainer drawable)
        {
            var editor = LibraryGUI.GetObjectEditor();
            if (editor == null)
                return;

            editor.DrawableContainers.Remove(drawable);
        }

        public List<Control> GetEditors()
        {
            if (ObjectTree != null)
                return ObjectTree.GetEditors();
            else
                return new List<Control>();
        }

        public IFileFormat GetActiveFile()
        {
            if (ObjectTree != null)
                return ObjectTree.GetActiveFile();
            else
                return ObjectList.GetActiveFile();
        }

        public void LoadEditor(Control control)
        {
            ObjectTree.LoadEditor(control);
        }

        private void ObjectEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Viewport viewport = LibraryGUI.GetActiveViewport();

            if (viewport != null)
                viewport.FormClosing();

            if (ObjectTree != null)
                ObjectTree.FormClosing();
        }

        public void ResetControls()
        {
            ObjectTree.ResetControls();
            Text = "";
        }
    }
}
