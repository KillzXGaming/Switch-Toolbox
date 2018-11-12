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
    public class SARC : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "*SARC", "*SARC", "*SARC" };
        public string[] Extension { get; set; } = new string[] { "*.szs", "*.pack", "*.sarc" };
        public string Magic { get; set; } = "SARC";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public TreeNode EditorRoot { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public SarcData sarcData;
        public void Load()
        {
            EditorRoot = new RootNode(Path.GetFileName(FileName), this);
            IsActive = true;
            CanSave = true;
            UseEditMenu = true;

            var SzsFiles = SARCExt.SARC.UnpackRamN(Data);

            sarcData = new SarcData();
            sarcData.HashOnly = false;
            sarcData.Files = SzsFiles.Files;
            sarcData.endianness = Syroot.BinaryData.ByteOrder.LittleEndian;

            FillTreeNodes(EditorRoot, SzsFiles.Files);

            sarcData.Files.Clear();
        }

        public void Unload()
        {
            EditorRoot.Nodes.Clear();
        }

        IEnumerable<TreeNode> Collect(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;

                foreach (var child in Collect(node.Nodes))
                    yield return child;
            }
        }
        public byte[] Save()
        {
            Console.WriteLine("Saving sarc");

            sarcData.Files.Clear();
            foreach (TreeNode node in Collect(EditorRoot.Nodes))
            {
                if (node is SarcEntry)
                {
                    Console.WriteLine("Saving " + node);
                    SaveFileEntryData((SarcEntry)node);
                }
            }

            Tuple<int, byte[]> sarc = SARCExt.SARC.PackN(sarcData);
            Alignment = sarc.Item1;
            return sarc.Item2;
        }

        private void SaveFileEntryData(SarcEntry sarc)
        {
            string dir = Path.GetDirectoryName(sarc.FullName);
            Console.WriteLine(dir);
            Console.WriteLine(sarc.FullName);

            if (dir == string.Empty)
                sarc.FullName = sarc.Text;
            else
                sarc.FullName = Path.Combine(dir, sarc.Text);

            Console.WriteLine(sarc.FullName);

            if (sarc.FileHandle != null)
            {
                IFileFormat file = sarc.FileHandle;

                if (file.CanSave)
                    sarcData.Files.Add(sarc.FullName, file.Save());
                else
                    sarcData.Files.Add(sarc.FullName, sarc.Data);
            }
            else
                sarcData.Files.Add(sarc.FullName, sarc.Data);
        }
        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        public class RootNode : TreeNodeCustom
        {
            SARC sarc;
            public RootNode(string n, SARC s)
            {
                Text = n;
                sarc = s;

                ContextMenu = new ContextMenu();
                MenuItem save = new MenuItem("Save");
                ContextMenu.MenuItems.Add(save);
                save.Click += Save;
            }
            private void Save(object sender, EventArgs args)
            {
                List<IFileFormat> formats = new List<IFileFormat>();
                formats.Add(sarc);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = Utils.GetAllFilters(formats);
                sfd.FileName = sarc.FileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    SaveCompressFile(sarc.Save(), sfd.FileName, sarc.Alignment);
                }
            }
            private void SaveCompressFile(byte[] data, string FileName, int Alignment = 0, bool EnableDialog = true)
            {
                if (EnableDialog)
                {
                    DialogResult save = MessageBox.Show("Compress file?", "File Save", MessageBoxButtons.YesNo);

                    if (save == DialogResult.Yes)
                        data = EveryFileExplorer.YAZ0.Compress(data, 3, (uint)Alignment);
                }
                File.WriteAllBytes(FileName, data);
                MessageBox.Show($"File has been saved to {FileName}");
                Cursor.Current = Cursors.Default;
            }

            public override void OnClick(TreeView treeview)
            {
            /*    CallRecursive(treeview);
                foreach (TreeNode n in Nodes)
                {
                    n.ExpandAll();
                }*/
            }
            private void CallRecursive(TreeView treeView)
            {
                // Print each node recursively.  
                TreeNodeCollection nodes = treeView.Nodes;
                foreach (TreeNode n in nodes)
                {
                    PrintRecursive(n);
                }
            }
            private void PrintRecursive(TreeNode treeNode)
            {
                // Print the node. 

                if (treeNode is SarcEntry)
                {
                    ((SarcEntry)treeNode).OnClick(treeNode.TreeView);
                }
                if (treeNode is IFileFormat)
                {

                }

                // Print each node recursively.  
                foreach (TreeNode tn in treeNode.Nodes)
                {
                    PrintRecursive(tn);
                }
            }
        }

        public class SarcEntry : TreeNodeCustom
        {
            public SARC sarc; //Sarc file the entry is located in
            public byte[] Data;

            public SarcEntry()
            {
                ImageKey = "fileBlank";
                SelectedImageKey = "fileBlank";

                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;

                MenuItem replace = new MenuItem("Replace");
                ContextMenu.MenuItems.Add(replace);
                replace.Click += Replace;

                MenuItem remove = new MenuItem("Remove");
                ContextMenu.MenuItems.Add(remove);
                remove.Click += Remove;

                MenuItem rename = new MenuItem("Rename");
                ContextMenu.MenuItems.Add(rename);
                rename.Click += Rename;
            }
            public override void OnClick(TreeView treeView)
            {
                OpenFile(Name, Data, treeView);
            }
            public void OpenFile(string FileName, byte[] data, TreeView treeView, bool Compressed = false, CompressionType CompType = CompressionType.None)
            { 
                FileReader f = new FileReader(data);
                string Magic = f.ReadMagic(0, 4);
                string Magic2 = f.ReadMagic(0, 2);

                //Determine if the file is compressed or not
                if (Magic == "Yaz0")
                {
                    data = EveryFileExplorer.YAZ0.Decompress(data);
                    OpenFile(FileName, data, treeView, true, CompressionType.Yaz0);
                    return;
                }
                if (Magic == "ZLIB")
                {
                    data = FileReader.InflateZLIB(f.getSection(64, data.Length - 64));
                    OpenFile(FileName, data, treeView, true, CompressionType.Zlib);
                    return;
                }

                f.Dispose();
                f.Close();

                IFileFormat[] SupportedFormats = FileManager.GetFileFormats();

                foreach (IFileFormat format in SupportedFormats)
                {
                    if (format.Magic == Magic || format.Magic == Magic2)
                    {
                        FileHandle = format;

                        format.CompressionType = CompType;
                        format.FileIsCompressed = Compressed;
                        format.Data = data;
                        format.FileName = Path.GetFileName(FileName);
                        format.Load();
                        format.FilePath = FileName;

                        if (format.EditorRoot != null)
                        {
                            format.EditorRoot.Text = Text;
                            format.EditorRoot.ImageKey = ImageKey;
                            format.EditorRoot.SelectedImageKey = SelectedImageKey;

                            Nodes.Add(format.EditorRoot);


                        //    ReplaceNode(this.Parent, this, format.EditorRoot);
                        }
                    }
                     if (format.Magic == String.Empty) //Load by extension if magic isn't defined
                    {
                        foreach (string ext in format.Extension)
                        {
                            if (ext.Remove(0, 1) == Path.GetExtension(FileName))
                            {
                                format.Load();
                            }
                        }
                    }
                }
            }

            private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
            {
                TreeNode node = TreeView.SelectedNode;

                // Determine by checking the Text property.  
                MessageBox.Show(node.Text);
            }

            public string FullName;
            public IFileFormat FileHandle; //Load file instance to save later if possible
            private void Replace(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = Text;
                ofd.DefaultExt = Path.GetExtension(Text);
                ofd.Filter = "All files(*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                        Data = File.ReadAllBytes(ofd.FileName);
                }
            }
            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.DefaultExt = Path.GetExtension(Text);
                sfd.Filter = "All files(*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, Data);
                }
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
        void FillTreeNodes(TreeNode root, Dictionary<string, byte[]> files)
        {
            var rootText = root.Text;
            var rootTextLength = rootText.Length;
            var nodeStrings = files;
            foreach (var node in nodeStrings)
            {
                string nodeString = node.Key;

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

                        var temp = new TreeNode(parentName, 0, 0);
                        if (rootIndex == roots.Length - 1)
                            temp = SetupFileEntry(node.Value, parentName, node.Key);

                        temp.Name = nodeName;
                        parentNode.Nodes.Add(temp);
                        parentNode = temp;
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

            Console.WriteLine(name);

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
