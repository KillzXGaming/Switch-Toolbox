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
        private ObjectEditorTree ObjectTree;
        private ObjectEditorList ObjectList; //Optionally usable for archives

        private TreeView _fieldsTreeCache;

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

            ObjectTree = new ObjectEditorTree();
            ObjectTree.Dock = DockStyle.Fill;
            stPanel1.Controls.Add(ObjectTree);
        }

        public ObjectEditor(IFileFormat FileFormat)
        {
            InitializeComponent();

            if (UseListView && FileFormat is IArchiveFile)
            {
                /* ObjectList = new ObjectEditorList();
                 ObjectList.Dock = DockStyle.Fill;
                 stPanel1.Controls.Add(ObjectList);
                 ObjectList.FillList((IArchiveFile)FileFormat);*/

                ObjectTree = new ObjectEditorTree();
                ObjectTree.Dock = DockStyle.Fill;
                stPanel1.Controls.Add(ObjectTree);

                TreeNode FileRoot = new TreeNode(FileFormat.FileName);
                foreach (var archive in ((IArchiveFile)FileFormat).Files)
                {
                    ArchiveNodeWrapper node = new ArchiveNodeWrapper(archive.FileName);
                    node.ArchiveFileInfo = archive;
                    FileRoot.Nodes.Add(node);
                }

                AddNode(FileRoot);
            }
            else
            {
                ObjectTree = new ObjectEditorTree();
                ObjectTree.Dock = DockStyle.Fill;
                stPanel1.Controls.Add(ObjectTree);
                AddNode((TreeNode)FileFormat);
            }
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
            Viewport viewport = LibraryGUI.Instance.GetActiveViewport();

            if (viewport != null)
                viewport.FormClosing();

            if (ObjectTree != null)
                ObjectTree.FormClosing();
        }

        public void RemoveFile(TreeNode File)
        {
            if (File is IFileFormat) {
                ((IFileFormat)File).Unload();
            }

            ObjectTree.RemoveFile(File);
        }

        public void ResetControls()
        {
            ObjectTree.ResetControls();
            Text = "";
        }
    }
}
