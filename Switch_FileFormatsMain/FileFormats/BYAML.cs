using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using ByamlExt.Byaml;
using ByamlExt;
using WeifenLuo.WinFormsUI.Docking;

namespace FirstPlugin
{
    public class BYAML : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "BYAML" };
        public string[] Extension { get; set; } = new string[] { "*.byaml", "*.byml", "*.bprm", "*.sbyml" };
        public string Magic { get; set; } = "YB";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public TreeNodeFile EditorRoot { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        class EditableNode
        {
            public Type type { get => Node[Index].GetType(); }
            dynamic Node;
            dynamic Index;

            public dynamic Get() => Node[Index];
            public void Set(dynamic value) => Node[Index] = value;
            public string GetTreeViewString()
            {
                if (Index is int)
                    return Node[Index].ToString();
                else
                    return Index + " : " + Node[Index].ToString();
            }

            public EditableNode(dynamic _node, dynamic _index)
            {
                Node = _node;
                Index = _index;
            }
        }

        public void Load()
        {
            IsActive = false;
            CanSave = false;

        //    ByamlViewer.OpenByml(new System.IO.MemoryStream(Data), FileName);


            //    BymlFileData byamlFile = ByamlFile.LoadN(new System.IO.MemoryStream(Data), false, Syroot.BinaryData.ByteOrder.LittleEndian);
            //          EditorRoot = LoadByamlNodes(byamlFile.RootNode);

            //    LoadDockedEditor(byamlFile);
        }
        public void Unload()
        {

        }

        ByamlEditor ByamlEditor;

        public void LoadDockedEditor(BymlFileData byamlFile)
        {
            foreach (Control control in FirstPlugin.MainF.Controls)
            {
                if (control is DockPanel)
                {
                    ByamlEditor = new ByamlEditor();
                    ByamlEditor.Dock = DockStyle.Fill;
                    ByamlEditor.Show(((DockPanel)control), DockState.Document);
                    ByamlEditor.LoadByaml(byamlFile);
                }
            }
        }

        public TreeNode LoadByamlNodes(dynamic root)
        {
            TreeNode node = new TreeNode();

            if (root == null)
                return node;
            if (root is Dictionary<string, dynamic>)
            {
                parseDictNode(root, node.Nodes);
            }
            else if (root is List<dynamic>)
            {
                if (((List<dynamic>)root).Count == 0)
                {
                    MessageBox.Show("This byml is empty");
                }
                parseArrayNode(root, node.Nodes);
            }
            else if (root is List<ByamlPathPoint>)
            {

            }

            return node;
        }

        void parseArrayNode(IList<dynamic> list, TreeNodeCollection addto)
        {
            int index = 0;
            foreach (dynamic k in list)
            {
                if (k is IDictionary<string, dynamic>)
                {
                    TreeNode current = addto.Add("<Dictionary>");
                    current.Tag = ((IDictionary<string, dynamic>)k);
                    current.Nodes.Add("✯✯dummy✯✯");
                }
                else if (k is IList<dynamic>)
                {
                    TreeNode current = addto.Add("<Array>");
                    current.Tag = ((IList<dynamic>)k);
                    current.Nodes.Add("✯✯dummy✯✯");
                }
                else if (k is IList<ByamlPathPoint>)
                {
                    TreeNode current = addto.Add("<PathPointArray>");
                    current.Tag = ((IList<ByamlPathPoint>)k);
                    parsePathPointArray(k, current.Nodes);
                }
                else
                {
                    var n = addto.Add(k == null ? "<NULL>" : k.ToString());
                    if (k != null) n.Tag = new EditableNode(list, index);
                }
                index++;
            }
        }

        void parseDictNode(IDictionary<string, dynamic> node, TreeNodeCollection addto)
        {
            foreach (string k in node.Keys)
            {
                TreeNode current = addto.Add(k);
                if (node[k] is IDictionary<string, dynamic>)
                {
                    current.Text += " : <Dictionary>";
                    current.Tag = node[k];
                    current.Nodes.Add("✯✯dummy✯✯"); //a text that can't be in a byml
                }
                else if (node[k] is IList<dynamic>)
                {
                    current.Text += " : <Array>";
                    current.Tag = ((IList<dynamic>)node[k]);
                    current.Nodes.Add("✯✯dummy✯✯");
                }
                else if (node[k] is IList<ByamlPathPoint>)
                {
                    current.Text += " : <PathPointArray>";
                    current.Tag = ((IList<ByamlPathPoint>)node[k]);
                    parsePathPointArray(node[k], current.Nodes);
                }
                else
                {
                    current.Text = current.Text + " : " + (node[k] == null ? "<NULL>" : node[k].ToString());
                    if (node[k] != null) current.Tag = new EditableNode(node, k);
                }
            }
        }
        void parsePathPointArray(IList<ByamlPathPoint> list, TreeNodeCollection addto)
        {
            int index = 0;
            foreach (var k in list)
            {
                index++;
                var n = addto.Add(k == null ? "<NULL>" : k.ToString());
                if (k != null) n.Tag = new EditableNode(list, index);
            }
        }

        public byte[] Save()
        {
            return null;
        }
    }
}
