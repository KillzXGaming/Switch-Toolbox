using ByamlExt.Byaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.BinaryData;
using EditorCore;
using Toolbox.Library.Forms;
using Toolbox.Library;
using ByamlExt;
using FirstPlugin.Forms;
using ByamlExt.Byaml;

namespace FirstPlugin
{
    //Editor from https://github.com/exelix11/EditorCore/blob/872d210f85ec0409f8a6ac3a12fc162aaf4cd90c/FileFormatPlugins/ByamlLib/Byaml/ByamlViewer.cs
    //Added as an editor form for saving data back via other plugins
    public partial class ByamlEditor : UserControl, IFIleEditor
    {
        public BYAML FileFormat;

        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { FileFormat };
        }

        public void BeforeFileSaved() { }

        public ByteOrder byteOrder;
        public dynamic byml
        {
            get { return FileFormat.BymlData.RootNode; }
            set { FileFormat.BymlData.RootNode = value; }
        }

        public string FileName = "";
        bool pathSupport;
        ushort bymlVer;

        bool useMuunt = true;

        bool IsXML => xmlToolstrip.Checked;
        bool IsOldXML => xmlOldToolstrip.Checked;

        private TextEditor textEditor;
        private STToolStipMenuItem xmlToolstrip;
        private STToolStipMenuItem xmlOldToolstrip;
        private STToolStipMenuItem yamlToolstrip;

        public ByamlEditor()
        {
            InitializeComponent();
            Reload();

            xmlOldToolstrip = new STToolStipMenuItem("XML (Toolbox/Editorcore)", null, OnFormatChanged);
            xmlToolstrip = new STToolStipMenuItem("XML (YamlConv)", null, OnFormatChanged);
            yamlToolstrip = new STToolStipMenuItem("YAML", null, OnFormatChanged);

            if (Runtime.ByamlEditor.TextFormat == Runtime.ByamlTextFormat.YAML)
                yamlToolstrip.Checked = true;
            else if (Runtime.ByamlEditor.TextFormat == Runtime.ByamlTextFormat.XML_YamlConv)
                xmlToolstrip.Checked = true;
            else if (Runtime.ByamlEditor.TextFormat == Runtime.ByamlTextFormat.XML_EditorCore)
                xmlOldToolstrip.Checked = true;
        }

        public ByamlEditor(System.Collections.IEnumerable by, bool _pathSupport, ushort _ver, ByteOrder defaultOrder = ByteOrder.LittleEndian, bool IsSaveDialog = false, BYAML byaml = null)
        {
            InitializeComponent();
            Reload();

            UpdateByaml(by, _pathSupport, _ver, defaultOrder, IsSaveDialog, byaml);
        }

        private void Reload()
        {
            chkShiftJISEncoding.Checked = BymlFileData.Encoding == Encoding.GetEncoding("shift_jis");
            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
            treeView1.Nodes.Clear();
        }

        public void UpdateByaml(System.Collections.IEnumerable by, bool _pathSupport, ushort _ver, ByteOrder defaultOrder = ByteOrder.LittleEndian, bool IsSaveDialog = false, BYAML byaml = null)
        {
            FileFormat = byaml;

            treeView1.Nodes.Clear();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            if (byaml.FileName == "course_muunt_debug.byaml" && useMuunt)
            {
                pathSupport = true;

                stPanel1.Controls.Remove(splitContainer1);

                TurboMunntEditor editor = new TurboMunntEditor();
                editor.Dock = DockStyle.Fill;
                editor.LoadCourseInfo(by, byaml.FilePath);
                stPanel1.Controls.Add(editor);
                return;
            }


            byteOrder = defaultOrder;
            FileName = byaml.FileName;
            byml = by;
            pathSupport = _pathSupport;
            bymlVer = _ver;

            if (byml == null) return;
            ParseBymlFirstNode();

            stPanel4.Controls.Clear();

            textEditor = new TextEditor();
            textEditor.ClearContextMenus(new string[] { "Search" });
            textEditor.AddContextMenu("Decompile", TextEditorToYaml);
            textEditor.AddContextMenu("Compile", TextEditorFromYaml);

            var formatMenu = new STToolStripItem("Change Formatting");
            formatMenu.DropDownItems.Add(xmlOldToolstrip);
            formatMenu.DropDownItems.Add(xmlToolstrip);
            formatMenu.DropDownItems.Add(yamlToolstrip);

            textEditor.AddContextMenu(formatMenu);

            stPanel4.Controls.Add(textEditor);
        }

        private void OnFormatChanged(object sender, EventArgs e)
        {
            yamlToolstrip.Checked = false;
            xmlToolstrip.Checked = false;
            xmlOldToolstrip.Checked = false;

            var menu = sender as STToolStipMenuItem;
            menu.Checked = true;

            if (yamlToolstrip.Checked)
                Runtime.ByamlEditor.TextFormat = Runtime.ByamlTextFormat.YAML;
            if (xmlToolstrip.Checked)
                Runtime.ByamlEditor.TextFormat = Runtime.ByamlTextFormat.XML_YamlConv;
            if (xmlOldToolstrip.Checked)
                Runtime.ByamlEditor.TextFormat = Runtime.ByamlTextFormat.XML_EditorCore;

            Toolbox.Library.Config.Save();

            if (textEditor.GetText() != string.Empty) {
                UpdateTextEditor();
            }
        }

        void ParseBymlFirstNode()
        {
            TreeNode root = new TreeNode(FileName);
            root.Tag = byml;
            treeView1.Nodes.Add(root);
            treeView1.SelectedNode = root;

            //the first node should always be a dictionary node
            if (byml is IDictionary<string, dynamic>)
            {
                parseDictNode(byml, root.Nodes);
            }
            else if (byml is List<dynamic>)
            {
                parseArrayNode(byml, root.Nodes);
            }
            else if (byml is List<ByamlPathPoint>)
            {
                MessageBox.Show("Unsupported root node");
            }
            else throw new Exception($"Unsupported root node type {byml.GetType()}");
        }

        Stream saveStream = null;
        public ByamlEditor(System.Collections.IEnumerable by, bool _pathSupport, Stream saveTo, ushort _ver, ByteOrder defaultOrder = ByteOrder.LittleEndian, bool IsSaveDialog = false, BYAML byaml = null) : this(by, _pathSupport, _ver, defaultOrder, IsSaveDialog, byaml)
        {

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            if (!IsSaveDialog)
            {
                stPanel1.Dock = DockStyle.Fill;
            }

            saveStream = saveTo;
            saveToolStripMenuItem.Visible = saveTo != null && saveStream.CanWrite;
        }

        //get a reference to the value to change
        class EditableNode
        {
            public Type type { get { return Node[Index].GetType(); } }
            dynamic Node;
            dynamic Index;

            public dynamic Get() { return Node[Index]; }
            public void Set(dynamic value) { Node[Index] = value; }

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

        void parseDictNode(IDictionary<string, dynamic> node)
        {
            foreach (string k in node.Keys)
            {
                if ((node[k] is IDictionary<string, dynamic>) ||
                (node[k] is List<dynamic>) ||
                (node[k] is List<ByamlPathPoint>))
                {
                    continue;
                }


                string ValueText = (node[k] == null ? "<NULL>" : node[k].ToString());
                string NameText = k;
                string TypeText = "";

                if (node[k] == null)
                    TypeText = "NULL";
                else
                    TypeText = node[k].GetType().ToString();

                string TypeString = TypeText.Replace("System.", "");

                ListViewItem item = new ListViewItem(NameText);
                item.SubItems.Add(TypeString);
                item.SubItems.Add(ValueText);
                if (node[k] != null) item.Tag = new EditableNode(node, k);

                if (NameText == "name_en")
                    Console.WriteLine("{ " + '"' + $"{ValueText}" + '"' + "," + '"' + $"{ValueText}" + '"' + " },");

                listViewCustom1.Items.Add(item);
            }
        }


        void parseArrayNode(IList<dynamic> list)
        {
            int index = 0;
            foreach (dynamic k in list)
            {
                if ((k is IDictionary<string, dynamic>) ||
                (k is List<dynamic>) ||
                (k is List<ByamlPathPoint>))
                {
                    continue;
                }

                string ValueText = (k == null ? "<NULL>" : k.ToString());
                string ValueTypeString = "";

                if (k == null)
                    ValueTypeString = "NULL";
                else
                {
                    Type ValueType = k.GetType();
                    ValueTypeString = ValueType.ToString();
                }


                ListViewItem item = new ListViewItem(ValueText);
                item.SubItems.Add(ValueTypeString);
                item.SubItems.Add(ValueText);
                if (k != null) item.Tag = new EditableNode(list, index);

                listViewCustom1.Items.Add(item);

                index++;
            }
        }

        void parseDictNode(IDictionary<string, dynamic> node, TreeNodeCollection addto)
        {
            int dictionaryIndex = 0;
            int arrayIndex = 0;
            int pathPointIndex = 0;

            foreach (string k in node.Keys)
            {
                if (node[k] is IDictionary<string, dynamic> ||
                    node[k] is IList<dynamic> ||
                    node[k] is IList<ByamlPathPoint>)
                {
                    TreeNode current = addto.Add(k);

                    if (node[k] is IDictionary<string, dynamic>)
                    {
                        current.Text += $" : <Dictionary> {dictionaryIndex++}";
                        current.Tag = node[k];

                        if (HasDynamicListChildren(current))
                            current.Nodes.Add("✯✯dummy✯✯"); //a text that can't be in a byml
                    }
                    else if (node[k] is IList<dynamic>)
                    {
                        current.Text += $" : <Array> {arrayIndex++}";
                        current.Tag = ((IList<dynamic>)node[k]);

                        if (HasDynamicListChildren(current))
                            current.Nodes.Add("✯✯dummy✯✯");
                    }
                    else if (node[k] is IList<ByamlPathPoint>)
                    {
                        current.Text += $" : <PathPointArray> {pathPointIndex++}";
                        current.Tag = ((IList<ByamlPathPoint>)node[k]);
                        parsePathPointArray(node[k], current.Nodes);
                    }
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

        void parseArrayNode(IList<dynamic> list, TreeNodeCollection addto)
        {
            int dictionaryIndex = 0;
            int arrayIndex = 0;
            int pathPointIndex = 0;


            int index = 0;
            foreach (dynamic k in list)
            {
                if (k is IDictionary<string, dynamic> ||
                    k is IList<dynamic> ||
                    k is IList<ByamlPathPoint>)
                {
                    if (k is IDictionary<string, dynamic>)
                    {
                        TreeNode current = addto.Add($"<Dictionary> {dictionaryIndex++}");
                        current.Tag = ((IDictionary<string, dynamic>)k);

                        if (HasDynamicListChildren(current))
                            current.Nodes.Add("✯✯dummy✯✯");
                    }
                    else if (k is IList<dynamic>)
                    {
                        TreeNode current = addto.Add($"<Array> {arrayIndex++}");
                        current.Tag = ((IList<dynamic>)k);
                    }
                    else if (k is IList<ByamlPathPoint>)
                    {
                        TreeNode current = addto.Add($"<PathPointArray> {pathPointIndex++}");
                        current.Tag = ((IList<ByamlPathPoint>)k);
                        parsePathPointArray(k, current.Nodes);
                    }
                }

                index++;
            }
        }

        //Search through the properties of a dictionary or list and see if it contains a list/dictionary
        //Then use this information to add tree nodes.
        //This is so nodes can be added on click but visually have children
        private bool HasDynamicListChildren(TreeNode Node)
        {
            if (Node.Tag != null)
            {
                if (((dynamic)Node.Tag).Count > 0)
                {
                    if (Node.Tag is IList<dynamic>)
                        return ListHasListChild((IList<dynamic>)Node.Tag);
                    if (Node.Tag is IDictionary<string, dynamic>)
                        return DictionaryHasListChild((IDictionary<string, dynamic>)Node.Tag);
                }
            }

            return false;
        }

        private bool ListHasListChild(IList<dynamic> list)
        {
            foreach (dynamic k in list)
            {
                if (k is IDictionary<string, dynamic>)
                    return true;
                else if (k is IList<dynamic>)
                    return true;
            }
            return false;
        }

        private bool DictionaryHasListChild(IDictionary<string, dynamic> node)
        {
            foreach (string k in node.Keys)
            {
                if (node[k] is IDictionary<string, dynamic>)
                    return true;
                else if (node[k] is IList<dynamic>)
                    return true;
            }
            return false;
        }

        private void BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "✯✯dummy✯✯")
            {
                e.Node.Nodes.Clear();
                if (((dynamic)e.Node.Tag).Count == 0)
                {
                    e.Node.Nodes.Add("<Empty>");
                    return;
                }
                if (e.Node.Tag is IList<dynamic>) parseArrayNode((IList<dynamic>)e.Node.Tag, e.Node.Nodes);
                else if (e.Node.Tag is IDictionary<string, dynamic>) parseDictNode((IDictionary<string, dynamic>)e.Node.Tag, e.Node.Nodes);
                else throw new Exception("WTF");
            }
        }

        private void ContextMenuOpening(object sender, CancelEventArgs e)
        {
            CopyNode.Enabled = treeView1.SelectedNode != null;
            editValueNodeMenuItem.Enabled = listViewCustom1.SelectedItems.Count > 0 && listViewCustom1.SelectedItems[0].Tag is EditableNode;
        }

        private void CopyNode_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(treeView1.SelectedNode.Text);
        }

        private void ByamlViewer_Load(object sender, EventArgs e)
        {

        }

        private void exportJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog() { Filter = TextFilter };
            sav.FileName = Path.GetFileNameWithoutExtension(FileName);
            sav.DefaultExt = ".yaml";
            if (sav.ShowDialog() != DialogResult.OK) return;

            string ext = Utils.GetExtension(sav.FileName);
            if (ext == ".xml") {
                File.WriteAllText(sav.FileName, XmlByamlConverter.ToXML(FileFormat.BymlData));
            }
            else {
                File.WriteAllText(sav.FileName, YamlByamlConverter.ToYaml(FileFormat.BymlData));
            }

            MessageBox.Show("Byaml converted successfully!");
        }

        public static void ImportFromJson()
        {

        }

        public static void OpenByml()
        {
            OpenFileDialog opn = new OpenFileDialog();
            opn.Filter = "byml file | *.byml";
            if (opn.ShowDialog() == DialogResult.OK)
            {
                OpenByml(opn.FileName, new BYAML());
            }
        }

        private static string TextFilter
        {
            get
            {
                return "Supported Formats|*.yaml;*.xml;|" +
                        "YAML |*.yaml|" +
                        "XML  |*.xml|" +
                        "All files(*.*)|*.*";
                ;
            }
        }

        static bool SupportPaths()
        {
            return MessageBox.Show("Does this game support paths ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        public static void OpenByml(string Filename, BYAML byaml)
        {
            OpenByml(new FileStream(Filename, FileMode.Open), byaml, Filename);
        }

        public static void OpenByml(Stream file, BYAML byaml, string FileName = "")
        {
            OpenByml(file, byaml, FileName, SupportPaths());
        }

        public static void OpenByml(Stream file, BYAML byaml, string FileName, bool paths)
        {
            OpenByml(file, byaml, FileName, paths, null, false);
        }

        public static void OpenByml(Stream file, BYAML byaml, string FileName, bool? paths, Stream saveStream, bool AsDialog)
        {
            bool _paths = paths == null ? SupportPaths() : paths.Value;
            var byml = ByamlFile.LoadN(file, _paths);
            OpenByml(byml, byaml, saveStream, AsDialog);
        }

        public static void OpenByml(BymlFileData data, BYAML byaml, Stream saveStream = null, bool AsDialog = false)
        {
            var form = new ByamlEditor(data.RootNode, data.SupportPaths, saveStream, data.Version, data.byteOrder, AsDialog, byaml);

            if (saveStream != null && saveStream.CanWrite)
            {
                saveStream.Position = 0;
                saveStream.SetLength(0);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog() { FileName = FileName, Filter = "byml file | *.byml" };
            if (sav.ShowDialog() == DialogResult.OK)
            {
                ByamlFile.SaveN(sav.FileName,
                    new BymlFileData { Version = bymlVer, byteOrder = byteOrder, SupportPaths = pathSupport, RootNode = byml });
            }
        }

        private void editValueNodeMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count <= 0)
                return;

            var node = listViewCustom1.SelectedItems[0].Tag as EditableNode;
            if (node == null) return;

            if (node.Get() is ByamlPathPoint)
            {
                new BymlPathPointEditor(node.Get()).ShowDialog(); //ByamlPathPoint is a reference type
            }
            else
            {
                string value = node.Get().ToString();
                var dRes = InputDialog.Show("Enter value", $"Enter new value for the node, the value must be of type {node.type}", ref value);
                if (dRes != DialogResult.OK) return;
                if (value.Trim() == "") return;
                node.Set(ByamlTypeHelper.ConvertValue(node.type, value));
            }

            ResetValues();
        }

        private void ResetValues()
        {
            if (treeView1.SelectedNode == null)
                return;

            listViewCustom1.Items.Clear();

            var targetNodeCollection = treeView1.SelectedNode.Nodes;

            dynamic target = treeView1.SelectedNode.Tag;

            if (target is IDictionary<string, dynamic>)
            {
                parseDictNode((IDictionary<string, dynamic>)target);
            }
            else if (target is IList<dynamic>)
            {
                parseArrayNode((IList<dynamic>)target);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ResetValues();
        }

        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dynamic target = treeView1.SelectedNode == null ? byml : treeView1.SelectedNode.Tag;

            var targetNodeCollection = treeView1.SelectedNode == null ? treeView1.Nodes : treeView1.SelectedNode.Nodes;

            if (target is EditableNode)
            {
                if (treeView1.SelectedNode.Parent == null)
                {
                    target = byml;
                    targetNodeCollection = treeView1.Nodes;
                }
                else
                {
                    target = treeView1.SelectedNode.Parent.Tag;
                    targetNodeCollection = treeView1.SelectedNode.Parent.Nodes;
                }
            }

            var newProp = AddBymlPropertyDialog.newProperty(!(target is IList<dynamic>));
            if (newProp == null) return;
            bool clone = newProp.Item2 is IDictionary<string, dynamic> || newProp.Item2 is IList<dynamic>; //reference types must be manually cloned
            var toAdd = clone ? DeepCloneDictArr.DeepClone(newProp.Item2) : newProp.Item2;

            targetNodeCollection.Clear();

            if (target is IList<dynamic>)
            {
                ((IList<dynamic>)target).Insert(((IList<dynamic>)target).Count, toAdd);
                parseArrayNode((IList<dynamic>)target, targetNodeCollection);
            }
            else if (target is IDictionary<string, dynamic>)
            {
                ((IDictionary<string, dynamic>)target).Add(newProp.Item1, toAdd);
                parseDictNode((IDictionary<string, dynamic>)target, targetNodeCollection);
            }
            else throw new Exception();

            ResetValues();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count <= 0 || treeView1.SelectedNode == null)
                return;

            dynamic targetNode = treeView1.SelectedNode.Tag;
            foreach (ListViewItem item in listViewCustom1.SelectedItems)
            {
                dynamic target = item.Tag;
                if (targetNode is IDictionary<string, dynamic>)
                    ((IDictionary<string, dynamic>)targetNode).Remove(item.Text);
                if (targetNode is IList<dynamic>)
                    ((IList<dynamic>)targetNode).Remove(target);

                int index = listViewCustom1.Items.IndexOf(item);
                listViewCustom1.Items.RemoveAt(index);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count <= 0)
                return;

            Clipboard.SetText(listViewCustom1.SelectedItems[0].Text);
        }

        private void copyDataAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count <= 0)
                return;

            if (listViewCustom1.SelectedItems.Count <= 0)
                return;

            var target = listViewCustom1.SelectedItems[0].Tag as EditableNode;
            var value = target.Get();
            if (value is string)
                Clipboard.SetText((string)value);
            else
            {
                try{
                    Clipboard.SetText(value.ToString());
                }
                catch
                {

                }
            }
        }

        private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("Select a node first");
                return;
            }

            dynamic target;
            TreeNodeCollection targetNode;
            if (treeView1.SelectedNode.Parent == null)
            {
                target = byml;
                targetNode = treeView1.Nodes;
            }
            else
            {
                target = treeView1.SelectedNode.Parent.Tag;
                targetNode = treeView1.SelectedNode.Parent.Nodes;
            }

            int index = targetNode.IndexOf(treeView1.SelectedNode);

            if (target is IDictionary<string, dynamic>)
                target.Remove(((Dictionary<string, dynamic>)target).ElementAt(index).Key);
            if (target is IList<dynamic>)
                target.RemoveAt(index);

            targetNode.RemoveAt(index);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveStream.Position = 0;
            saveStream.SetLength(0);
            ByamlFile.SaveN(saveStream, new BymlFileData { Version = bymlVer, byteOrder = byteOrder, SupportPaths = pathSupport, RootNode = byml });
        }

        private void importFromXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = TextFilter;
            if (openFile.ShowDialog() != DialogResult.OK) return;

            string ext = Utils.GetExtension(openFile.FileName);
            if (ext == ".xml") {
                StreamReader t = new StreamReader(new FileStream(openFile.FileName, FileMode.Open));
                FileFormat.BymlData = XmlByamlConverter.FromXML(t.ReadToEnd());
            }
            else {
                StreamReader t = new StreamReader(new FileStream(openFile.FileName, FileMode.Open));
                FileFormat.BymlData = YamlByamlConverter.FromYaml(t.ReadToEnd());
            }
            treeView1.Nodes.Clear();
            ParseBymlFirstNode();
        }

        private void contentContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listViewCustom1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = listViewCustom1.PointToScreen(e.Location);
                stContextMenuStrip1.Show(pt);
            }
        }

        private void TextEditorToYaml(object sender, EventArgs e) {
            UpdateTextEditor();
        }

        private void UpdateTextEditor() {
            textEditor.IsXML = false;
            textEditor.IsYAML = false;

            if (IsXML)
            {
                textEditor.FillEditor(XmlByamlConverter.ToXML(FileFormat.BymlData));
                textEditor.IsXML = true;
            }
            else if (IsOldXML)
            {
                textEditor.FillEditor(XmlConverter.ToXml(FileFormat.BymlData));
                textEditor.IsXML = true;
            }
            else
            {
                textEditor.FillEditor(YamlByamlConverter.ToYaml(FileFormat.BymlData));
                textEditor.IsYAML = true;
            }
        }

        private void TextEditorFromYaml(object sender, EventArgs e)
        {
            string editorText = textEditor.GetText();
            if (editorText == string.Empty)
                return;

            try
            {
                if (FileFormat != null) {
                    if (IsXML)
                        FileFormat.BymlData = XmlByamlConverter.FromXML(textEditor.GetText());
                    else if (IsOldXML)
                    {
                        byte[] TextData = Encoding.Unicode.GetBytes(textEditor.GetText());
                        StreamReader t = new StreamReader(new MemoryStream(TextData), Encoding.GetEncoding(932));
                        FileFormat.BymlData = XmlConverter.ToByml(t.ReadToEnd());
                    }
                    else
                        FileFormat.BymlData = YamlByamlConverter.FromYaml(textEditor.GetText());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Byaml failed to convert! " + ex.ToString());
                return;
            }

            treeView1.Nodes.Clear();
            ParseBymlFirstNode();

            MessageBox.Show("Byaml converted successfully!");
        }

        private void treeView1_MouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
            }
        }

        private void chkShiftJISEncoding_CheckedChanged(object sender, EventArgs e) {
            if (FileFormat == null) return;

            if (chkShiftJISEncoding.Checked)
                FileFormat.ReloadEncoding(Encoding.GetEncoding("shift_jis"));
            else
                FileFormat.ReloadEncoding(Encoding.UTF8);

            treeView1.Nodes.Clear();
            ParseBymlFirstNode();
        }
    }
}