using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Switch_Toolbox;
using System.Windows.Forms;
using SARCExt;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class SARC : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "SARC", "SARC", "SARC", "SARC", "SARC", "SARC" };
        public string[] Extension { get; set; } = new string[] { "*.pack", "*.sarc", "*.bgenv", "*.sblarc", "*.sbactorpack", ".arc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "SARC");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public List<SarcEntry> Files = new List<SarcEntry>();

        public Dictionary<string, byte[]> OpenedFiles = new Dictionary<string, byte[]>();

        public SarcData sarcData;
        public string SarcHash;
        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            IFileInfo.UseEditMenu = true;

            var SzsFiles = SARCExt.SARC.UnpackRamN(stream);
            sarcData = new SarcData();
            sarcData.HashOnly = SzsFiles.HashOnly;
            sarcData.Files = SzsFiles.Files;
            sarcData.endianness = GetByteOrder(stream);
            SarcHash = Utils.GenerateUniqueHashID();

            FillTreeNodes(this, SzsFiles.Files, sarcData.HashOnly);

            Text = FileName;

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Save",null, Save, Keys.Control | Keys.S));
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Rename Actor Files (Odyssey)", null, RenameActors, Keys.Control | Keys.S));

            //  ContextMenuStrip.Items.Add(new STToolStipMenuItem("Unpack to Folder", null, UnpackToFolder, Keys.Control | Keys.E));
            //    ContextMenuStrip.Items.Add(new STToolStipMenuItem("Pack From Folder", null, PackFromFolder, Keys.Control | Keys.R));
            ContextMenuStrip.Items.Add(new STToolStripSeparator());
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Batch Texture Editor", null, PreviewTextures, Keys.Control | Keys.P));
            ContextMenuStrip.Items.Add(new STToolStripSeparator());
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Sort Childern", null, SortChildern, Keys.Control | Keys.E));
            CanDelete = true;

            sarcData.Files.Clear();
        }

        private void RenameActors(object sender, EventArgs args)
        {
            string ActorName = Path.GetFileNameWithoutExtension(Text);

            RenameDialog dialog = new RenameDialog();
            dialog.SetString(ActorName);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string NewActorName = dialog.textBox1.Text;
                Text = NewActorName + ".szs";

                foreach (TreeNode node in Nodes)
                {
                    string NodeName = Path.GetFileNameWithoutExtension(node.Text);
                    string ext = Utils.GetExtension(node.Text);
                    if (NodeName == ActorName)
                    {
                        node.Text = $"{NewActorName}{ext}";
                    }
                    else if (node.Text.Contains("Attribute.byml"))
                    {
                        node.Text = $"{NewActorName}Attribute.byml";
                    }
                }
            }
        }
        
        private void UnpackToFolder(object sender, EventArgs args)
        {

        }

        private void PackFromFolder(object sender, EventArgs args)
        {

        }

        private void Delete(object sender, EventArgs args) {
            Unload();
            var editor = LibraryGUI.Instance.GetObjectEditor();
            if (editor != null)
                editor.ResetControls();
        }

        private void SortChildern(object sender, EventArgs args)
        {
            TreeView.TreeViewNodeSorter = new TreeChildSorter();
            TreeView.Sort();
        }

        public class FolderEntry : TreeNode, IContextMenuNode
        {
            public FolderEntry(string text, int imageIndex, int selectedImageIndex)
            {
                Text = text;
                ImageIndex = imageIndex;
                SelectedImageIndex = selectedImageIndex;
            }

            public ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new ToolStripMenuItem("Sort Childern", null, SortChildern, Keys.Control | Keys.W));
                return Items.ToArray();
            }

            private void SortChildern(object sender, EventArgs args)
            {
                TreeView.TreeViewNodeSorter = new TreeChildSorter();
                TreeView.Sort();
            }
        }

        public Syroot.BinaryData.ByteOrder GetByteOrder(System.IO.Stream data)
        {
            using (FileReader reader = new FileReader(data))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.Seek(6);
                ushort bom = reader.ReadUInt16();
                reader.Close();
                reader.Dispose();

                if (bom == 0xFFFE)
                    return Syroot.BinaryData.ByteOrder.LittleEndian;
                else
                    return Syroot.BinaryData.ByteOrder.BigEndian;
            }
        }

        public void Unload()
        {
            Nodes.Clear();
        }

        IEnumerable<TreeNode> Collect(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;

                bool IsNodeFile = node is IFileFormat;

                if (!IsNodeFile)
                {
                    //We don't need to save the child of IFIleFormats
                    //If opened the file should save it's own children
                    foreach (var child in Collect(node.Nodes))
                        yield return child;
                }
            }
        }
        public byte[] Save()
        {
            Console.WriteLine("Saving sarc");

            sarcData.Files.Clear();
            foreach (TreeNode node in Collect(Nodes))
            {
                if (node is SarcEntry)
                {
                    Console.WriteLine("Saving " + node);
                    SaveFileEntryData((SarcEntry)node);
                }
                else if (node is IFileFormat && node != this)
                {
                    IFileFormat fileFormat = (IFileFormat)node;
                    if (fileFormat != null && ((IFileFormat)node).CanSave)
                    {
                        sarcData.Files.Add(SetSarcPath(node, this),
                            STLibraryCompression.CompressFile(fileFormat.Save(), fileFormat));
                    }
                    else
                    {
                        sarcData.Files.Add(SetSarcPath(node, this),
                            STLibraryCompression.CompressFile(OpenedFiles[node.FullPath], fileFormat));
                    }
                }
            }

            Tuple<int, byte[]> sarc = SARCExt.SARC.PackN(sarcData);

            IFileInfo.Alignment = sarc.Item1;
            return sarc.Item2;
        }

        public static string SetSarcPath(TreeNode node, TreeNode sarcNode)
        {
            string nodePath = node.FullPath;
            int startIndex = nodePath.IndexOf(sarcNode.Text);
            if (startIndex > 0)
                nodePath = nodePath.Substring(startIndex);

            string slash = Path.DirectorySeparatorChar.ToString();
            string slashAlt = Path.AltDirectorySeparatorChar.ToString();

            string SetPath = nodePath.Replace(sarcNode.Text + slash, string.Empty).Replace(slash ?? "", slashAlt);
            return !(SetPath == string.Empty) ? SetPath : node.Text;
        }

        private void SaveFileEntryData(SarcEntry sarc)
        {
            string dir = Path.GetDirectoryName(sarc.FullName);

            if (!sarcData.HashOnly)
            {
                if (dir == string.Empty)
                    sarc.FullName = sarc.Text;
                else
                    sarc.FullName = Path.Combine(dir, sarc.Text);

                sarc.FullName = sarc.FullName.Replace(@"\", "/");
            }

            sarcData.Files.Add(sarc.FullName, sarc.Data);
        }
        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);


            if (NewNode is TreeNodeFile)
                ((TreeNodeFile)NewNode).OnAfterAdded();
        }

        private void Save(object sender, EventArgs args)
        {
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public static bool SuppressFormDialog = false;
        private void PreviewTextures(object sender, EventArgs args)
        {
            SuppressFormDialog = true;

            List<IFileFormat> Formats = new List<IFileFormat>();

            try
            {
                CallRecursive(TreeView, Formats);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            ArchiveListPreviewForm editor = new ArchiveListPreviewForm();
            editor.LoadArchive(Formats);
            editor.Show();

            SuppressFormDialog = false;
        }

        private void CallRecursive(TreeView treeView, List<IFileFormat> Formats)
        {
            // Print each node recursively.  
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                GetNodeFileFormat(n, Formats);
            }
        }
        private void GetNodeFileFormat(TreeNode treeNode, List<IFileFormat> Formats)
        {
            // Print the node. 

            if (treeNode is SarcEntry)
            {
                var format = ((SarcEntry)treeNode).OpenFile();
                if (format != null)
                    Formats.Add(format);
            }

            // Print each node recursively.  
            foreach (TreeNode tn in treeNode.Nodes)
            {
                GetNodeFileFormat(tn, Formats);
            }
        }

        public class SarcEntry : TreeNodeCustom, IContextMenuNode
        {
            public SARC sarc; //Sarc file the entry is located in
            public byte[] Data;
            public string sarcHash;

            public SarcEntry()
            {
                ImageKey = "fileBlank";
                SelectedImageKey = "fileBlank";
            }

            public ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new STToolStipMenuItem("Export Raw Data", null, Export, Keys.Control | Keys.E));
                Items.Add(new STToolStipMenuItem("Export Raw Data to File Location", null, ExportToFileLoc, Keys.Control | Keys.F));
                Items.Add(new STToolStipMenuItem("Replace Raw Data", null, Replace, Keys.Control | Keys.R));
                Items.Add(new STToolStripSeparator());
                Items.Add(new STToolStipMenuItem("Open With Text Editor", null, OpenTextEditor, Keys.Control | Keys.T));
                Items.Add(new STToolStripSeparator());
                Items.Add(new STToolStipMenuItem("Remove", null, Remove, Keys.Control | Keys.Delete));
                Items.Add(new STToolStipMenuItem("Rename", null, Rename, Keys.Control | Keys.N));
                return Items.ToArray();
            }

            public override void OnClick(TreeView treeView)
            {
                UpdateHexView();
            }

            private void UpdateHexView()
            {
                HexEditor editor = (HexEditor)LibraryGUI.Instance.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.Instance.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(Data);
            }

            public IFileFormat OpenFile()
            {
                return STFileLoader.OpenFileFormat(FullName, Data, false, true, this);
            }

            public override void OnDoubleMouseClick(TreeView treeView)
            {
                if (Data.Length <= 0)
                    return;

                IFileFormat file = OpenFile();
                if (file == null) //File returns null if no supported format is found
                    return; 

                if (Utils.HasInterface(file.GetType(), typeof(IEditor<>)) && !SuppressFormDialog)
                {
                    OpenFormDialog(file);
                }
                else if (file != null)
                {
                    sarc.OpenedFiles.Add(FullPath, Data);
                    ReplaceNode(this.Parent, this, (TreeNode)file);
                }
            }

            private void OpenFormDialog(IFileFormat fileFormat)
            {
                UserControl form = GetEditorForm(fileFormat);
                form.Text = (((IFileFormat)fileFormat).FileName);

                var parentForm = LibraryGUI.Instance.GetActiveForm();

                GenericEditorForm editorForm = new GenericEditorForm(true, form);
                editorForm.FormClosing += (sender, e) => FormClosing(sender, e, fileFormat);
                if (editorForm.ShowDialog() == DialogResult.OK)
                {
                    if (fileFormat.CanSave)
                    {
                        Data = fileFormat.Save();
                        UpdateHexView();
                    }
                }
            }

            private void FormClosing(object sender, EventArgs args, IFileFormat fileFormat)
            {
                if (((Form)sender).DialogResult != DialogResult.OK)
                    return;

                if (fileFormat.CanSave)
                {
                    Data = fileFormat.Save();
                    UpdateHexView();
                }
            }

            private UserControl GetEditorForm(IFileFormat fileFormat)
            {
                Type objectType = fileFormat.GetType();
                foreach (var inter in objectType.GetInterfaces())
                {
                    if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                    {
                        System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                        return (UserControl)method.Invoke(fileFormat, new object[0]);
                    }
                }
                return null;
            }

            private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
            {
                TreeNode node = TreeView.SelectedNode;

                // Determine by checking the Text property.  
            }

            public string FullName;
            private void Replace(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = Text;
                ofd.DefaultExt = Path.GetExtension(Text);
                ofd.Filter = "Raw Data (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Data = File.ReadAllBytes(ofd.FileName);
                }
            }
            private void ExportToFileLoc(object sender, EventArgs args)
            {
                Cursor.Current = Cursors.WaitCursor;
                File.WriteAllBytes($"{Path.GetDirectoryName(sarc.FilePath)}/{Text}", Data);
                Cursor.Current = Cursors.Default;
            }
            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.DefaultExt = Path.GetExtension(Text);
                sfd.Filter = "Raw Data (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, Data);
                }
            }

            private void OpenTextEditor(object sender, EventArgs args)
            {
                TextEditor editor = (TextEditor)LibraryGUI.Instance.GetActiveContent(typeof(TextEditor));
                if (editor == null)
                {
                    editor = new TextEditor();
                    LibraryGUI.Instance.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.FillEditor(Data);
            }

            private void Remove(object sender, EventArgs args)
            {
                DialogResult result = MessageBox.Show($"Are your sure you want to remove {Text}? This cannot be undone!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Parent.Nodes.Remove(this);
                }
            }
            private void Rename(object sender, EventArgs args)
            {
                RenameDialog dialog = new RenameDialog();
                dialog.SetString(Text);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Text = dialog.textBox1.Text;
                }
            }
        }
        void FillTreeNodes(TreeNode root, Dictionary<string, byte[]> files, bool HashOnly)
        {
            var rootText = root.Text;
            var rootTextLength = rootText.Length;
            var nodeStrings = files;
            foreach (var node in nodeStrings)
            {
                string nodeString = node.Key;

                if (HashOnly)
                    nodeString = SARCExt.SARC.TryGetNameFromHashTable(nodeString);

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

                        var folder = new FolderEntry(parentName, 0, 0);
                        if (rootIndex == roots.Length - 1)
                        {
                            var file = SetupFileEntry(node.Value, parentName, node.Key);
                            file.Name = nodeName;
                            parentNode.Nodes.Add(file);
                            parentNode = file;
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

        List<string> BuildFinalList(List<string> paths)
        {
            var finalList = new List<string>();
            foreach (var path in paths)
            {
                bool found = false;
                foreach (var item in finalList)
                {
                    if (item.StartsWith(path, StringComparison.Ordinal))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    finalList.Add(path);
                }
            }
            return finalList;
        }

        public SarcEntry SetupFileEntry(byte[] data, string name, string fullName)
        {
            SarcEntry sarcEntry = new SarcEntry();
            sarcEntry.FullName = fullName;
            sarcEntry.Name = name;
            sarcEntry.Text = name;
            sarcEntry.sarc = this;
            sarcEntry.Data = data;

            Files.Add(sarcEntry);

            string ext = Path.GetExtension(name);
            string SarcEx = SARCExt.SARC.GuessFileExtension(data);
            if (SarcEx == ".bfres" || ext == ".sbfres")
            {
                sarcEntry.ImageKey = "bfres";
                sarcEntry.SelectedImageKey = "bfres";
            }
            if (SarcEx == ".bntx")
            {
                sarcEntry.ImageKey = "bntx";
                sarcEntry.SelectedImageKey = "bntx";
            }
            if (SarcEx == ".byaml")
            {
                sarcEntry.ImageKey = "byaml";
                sarcEntry.SelectedImageKey = "byaml";
            }
            if (SarcEx == ".aamp")
            {
                sarcEntry.ImageKey = "aamp";
                sarcEntry.SelectedImageKey = "aamp";
            }
            return sarcEntry;
        }
    }
}
