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

        public SarcData sarcData;
        public string SarcHash;
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
            SarcHash = Utils.GenerateUniqueHashID();

            IFileInfo = new IFileInfo();

            FillTreeNodes(EditorRoot, SzsFiles.Files, SarcHash);

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
                if (node is TreeNodeFile && node != EditorRoot)
                {
                    TreeNodeFile treeNodeFile = (TreeNodeFile)node;
                    if (treeNodeFile.FileHandler != null && treeNodeFile.FileHandler.IFileInfo.ArchiveHash == SarcHash)
                    {
                        sarcData.Files.Add(SARC.SetSarcPath(node, (TreeNode)this.EditorRoot), STLibraryCompression.CompressFile(treeNodeFile.FileHandler.Save(), treeNodeFile.FileHandler));
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

            if (dir == string.Empty)
                sarc.FullName = sarc.Text;
            else
                sarc.FullName = Path.Combine(dir, sarc.Text);

            sarcData.Files.Add(sarc.FullName, sarc.Data);
        }
        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        public class RootNode : TreeNodeFile
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
                    SaveCompressFile(sarc.Save(), sfd.FileName, sarc.IFileInfo.Alignment);
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
            public string sarcHash;

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
            }
            public override void OnMouseClick(TreeView treeView)
            {
                ReplaceNode(this.Parent, this, OpenFile(Name, Data, this));
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
        void FillTreeNodes(TreeNode root, Dictionary<string, byte[]> files, string SarcHash)
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
                            temp = SetupFileEntry(node.Value, parentName, node.Key, SarcHash);

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

        public static TreeNode OpenFile(string FileName, byte[] data, SarcEntry sarcEntry, bool Compressed = false, CompressionType CompType = 0)
        {
            Cursor.Current = Cursors.WaitCursor;
            FileReader fileReader = new FileReader(data);
            string Magic4 = fileReader.ReadMagic(0, 4);
            string Magic2 = fileReader.ReadMagic(0, 2);
            if (Magic4 == "Yaz0")
            {
                data = EveryFileExplorer.YAZ0.Decompress(data);
                return OpenFile(FileName, data, sarcEntry, true, (CompressionType)1);
            }
            if (Magic4 == "ZLIB")
            {
                data = FileReader.InflateZLIB(fileReader.getSection(64, data.Length - 64));
                return OpenFile(FileName, data, sarcEntry, true, (CompressionType)2);
            }
            fileReader.Dispose();
            fileReader.Close();
            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                if (fileFormat.Magic == Magic4 || fileFormat.Magic == Magic2)
                {
                    fileFormat.CompressionType = CompType;
                    fileFormat.FileIsCompressed = Compressed;
                    fileFormat.Data = data;
                    fileFormat.Load();
                    fileFormat.FileName = Path.GetFileName(FileName);
                    fileFormat.FilePath = FileName;
                    fileFormat.IFileInfo = new IFileInfo();
                    fileFormat.IFileInfo.ArchiveHash = sarcEntry.sarcHash;
                    fileFormat.IFileInfo.InArchive = true;
                    if (fileFormat.EditorRoot == null)
                        return null;
                    fileFormat.EditorRoot.ImageKey = sarcEntry.ImageKey;
                    fileFormat.EditorRoot.SelectedImageKey = sarcEntry.SelectedImageKey;
                    fileFormat.EditorRoot.Text = sarcEntry.Text;

                    return fileFormat.EditorRoot;
                }
                if (fileFormat.Magic == string.Empty)
                {
                    foreach (string str3 in fileFormat.Extension)
                    {
                        if (str3.Remove(0, 1) == Path.GetExtension(FileName))
                        {
                            fileFormat.Data = data;
                            fileFormat.Load();
                            fileFormat.FileName = Path.GetFileName(FileName);
                            fileFormat.FilePath = FileName;
                            fileFormat.IFileInfo = new IFileInfo();
                            fileFormat.IFileInfo.ArchiveHash = sarcEntry.sarcHash;
                            fileFormat.IFileInfo.InArchive = true;
                            if (fileFormat.EditorRoot == null)
                                return null;
                            fileFormat.EditorRoot.ImageKey = sarcEntry.ImageKey;
                            fileFormat.EditorRoot.SelectedImageKey = sarcEntry.SelectedImageKey;
                            fileFormat.EditorRoot.Text = sarcEntry.Text;
                            return fileFormat.EditorRoot;
                        }
                    }
                }
            }
            return (TreeNode)null;
        }


        public SarcEntry SetupFileEntry(byte[] data, string name, string fullName, string SarchHash)
        {
            SarcEntry sarcEntry = new SarcEntry();
            sarcEntry.FullName = fullName;
            sarcEntry.Name = name;
            sarcEntry.Text = name;
            sarcEntry.sarc = this;
            sarcEntry.Data = data;
            sarcEntry.sarcHash = SarcHash;

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
