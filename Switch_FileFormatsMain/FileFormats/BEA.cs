using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using System.IO;
using BezelEngineArchive_Lib;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class BEA : TreeNode, IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "Bevel Engine Archive" };
        public string[] Extension { get; set; } = new string[] { "*.bea" };
        public string Magic { get; set; } = "SCNE";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }
        class MenuExt : IFileMenuExtension
        {
            public ToolStripItemDark[] NewFileMenuExtensions => null;
            public ToolStripItemDark[] ToolsMenuExtensions => null;
            public ToolStripItemDark[] TitleBarExtensions => null;
            public ToolStripItemDark[] CompressionMenuExtensions => null;
            public ToolStripItemDark[] ExperimentalMenuExtensions => experimentalMenu;

            ToolStripItemDark[] experimentalMenu = new ToolStripItemDark[1];
            public MenuExt()
            {
                experimentalMenu[0] = new ToolStripItemDark("BEA");

                ToolStripItemDark batchLUA = new ToolStripItemDark("Batch Extract LUA");
                batchLUA.Click += BatchExtractLUA;


                experimentalMenu[0].DropDownItems.Add(batchLUA);
            }

            private void BatchExtractLUA(object sender, EventArgs e)
            {
                FolderSelectDialog ofd = new FolderSelectDialog();

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = ofd.SelectedPath;

                    foreach (string file in Directory.GetFiles(folderPath))
                    {
                        Console.WriteLine(file);

                        if (Path.GetExtension(file) == ".bea")
                        {
                            BEA bea = new BEA();
                            bea.Data = File.ReadAllBytes(file);
                            bea.FileName = file;
                            bea.Load();

                            foreach (FileEntry asset in bea.Nodes)
                            {
                                if (Path.GetExtension(asset.FullName) == ".lua")
                                {
                                    try
                                    {
                                        if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName($"{folderPath}/{bea.Name}/{asset.FullName}")))
                                        {
                                            if (!File.Exists(asset.FullName))
                                            {
                                                if (!Directory.Exists($"{folderPath}/{bea.Name}/{asset.FullName}"))
                                                {
                                                    Directory.CreateDirectory(Path.GetDirectoryName($"{folderPath}/{bea.Name}/{asset.FullName}"));
                                                }
                                            }
                                        }
                                        File.WriteAllBytes($"{folderPath}/{bea.Name}/{asset.FullName}", GetASSTData(asset));
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static BezelEngineArchive beaFile;
        static ProgressBarWindow progressBar;

        public void Load()
        {
            Text = FileName;

            IsActive = true;
            CanSave = true;

            beaFile = new BezelEngineArchive(new MemoryStream(Data));
            FillTreeNodes(this, beaFile.FileList);

            ContextMenu = new ContextMenu();
            MenuItem save = new MenuItem("Save");
            ContextMenu.MenuItems.Add(save);
            save.Click += Save;
            MenuItem previewFiles = new MenuItem("Preview Window");
            ContextMenu.MenuItems.Add(previewFiles);
            previewFiles.Click += PreviewWindow;
            MenuItem exportAll = new MenuItem("Export All");
            ContextMenu.MenuItems.Add(exportAll);
            exportAll.Click += ExportAll;
        }
        public void Unload()
        {

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
        private void Save(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
            GC.Collect();
        }
        public byte[] Save()
        {
            beaFile.FileList.Clear();
            beaFile.FileDictionary.Clear();

            foreach (TreeNode node in Collect(Nodes))
            {
                if (node is TreeNodeFile && node != this)
                {
                    IFileFormat fileFormat = (IFileFormat)node;
                    if (fileFormat != null)
                    {
                        byte[] uncomrompressedData = fileFormat.Data;

                        //Save any active files in the editor if supported
                        if (fileFormat.CanSave)
                            uncomrompressedData = fileFormat.Save();

                        //Create a new asset entry
                        ASST asset = new ASST();
                        asset.unk = 2;
                        asset.unk2 = 2;
                        asset.UncompressedSize = uncomrompressedData.LongLength;

                        if (fileFormat.FileIsCompressed)
                            asset.FileData = STLibraryCompression.ZSTD.Compress(uncomrompressedData);
                        else
                            asset.FileData = fileFormat.Data;

                        asset.FileName = fileFormat.FilePath;
                        beaFile.FileList.Add(fileFormat.FilePath, asset);
                        beaFile.FileDictionary.Add(fileFormat.FilePath);
                    }
                }
                else if (node is FileEntry)
                {
                    ASST asset = new ASST();
                    asset.unk = ((FileEntry)node).unk1;
                    asset.unk2 = ((FileEntry)node).unk2;
                    asset.FileName = ((FileEntry)node).FullName;
                    asset.FileData = ((FileEntry)node).data;
                    byte[] uncomp = GetASSTData((FileEntry)node);
                    asset.UncompressedSize = uncomp.Length;
                    beaFile.FileList.Add(asset.FileName, asset);
                    beaFile.FileDictionary.Add(asset.FileName);
                }
            }

            MemoryStream mem = new MemoryStream();
            beaFile.Save(mem);
            return mem.ToArray();
        }
        private void ExportAll(object sender, EventArgs args)
        {
            FolderSelectDialog fsd = new FolderSelectDialog();

            if (fsd.ShowDialog() == DialogResult.OK)
            {
                progressBar = new ProgressBarWindow();
                progressBar.Task = "Extracing Files...";
                progressBar.Refresh();
                progressBar.Value = 0;
                progressBar.StartPosition = FormStartPosition.CenterScreen;
                progressBar.Show();

                ExportAll(fsd.SelectedPath, progressBar);
            }
        }
        private void ExportAll(string Folder, ProgressBarWindow progressBar)
        {
            int Curfile = 0;
            foreach (FileEntry asst in Nodes)
            {
                int value = (Curfile * 100) / beaFile.FileList.Count;
                progressBar.Value = value;
                progressBar.Refresh();

                try
                {
                    if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName($"{Folder}/{beaFile.Name}/{asst.FullName}")))
                    {
                        if (!File.Exists(asst.FullName))
                        {
                            if (!Directory.Exists($"{Folder}/{beaFile.Name}/{asst.FullName}"))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName($"{Folder}/{beaFile.Name}/{asst.FullName}"));
                            }
                        }
                    }
                    File.WriteAllBytes($"{Folder}/{beaFile.Name}/{asst.FullName}", GetASSTData(asst));
                }
                catch
                {

                }

                Curfile++;
                if (value == 99)
                    value = 100;
                progressBar.Value = value;
                progressBar.Refresh();
            }
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
            // Print each node recursively.  
            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn);
            }
        }

        public void PreviewWindow(object sender, EventArgs args)
        {
            PreviewFormatList previewFormatList = new PreviewFormatList();

            if (previewFormatList.ShowDialog() == DialogResult.OK)
            {
                CallRecursive(TreeView);
                Console.WriteLine("Loaded files");
                Console.WriteLine(PluginRuntime.bntxContainers.Count);
                PreviewEditor previewWindow = new PreviewEditor();
                previewWindow.Show();
            }
        }
        public bool Compressed;
        public class FolderEntry : TreeNode
        {
            public FolderEntry()
            {
                ImageKey = "folder";
                SelectedImageKey = "folder";
            }

            public FolderEntry(string Name)
            {
                Text = Name;
            }
        }
        public class FileEntry : TreeNodeCustom
        {
            public FileEntry()
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
            }

            public string FullName;
            public IFileFormat FileHandle; //Load file instance to save later if possible
            public byte[] data;
            public ushort unk1;
            public ushort unk2;
            public bool IsCompressed;

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.DefaultExt = Path.GetExtension(Text);
                sfd.Filter = "All files(*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, GetASSTData(this));
                }
            }

            private void Replace(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = Text;
                ofd.DefaultExt = Path.GetExtension(Text);
                ofd.Filter = "All files(*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                     SetASST(this, File.ReadAllBytes(ofd.FileName));
                }
            }

            public override void OnDoubleMouseClick(TreeView treeview)
            {
                if (GetASSTData(this) != null)
                {
                    TreeNode node = STFileLoader.GetNodeFileFormat(FullName, GetASSTData(this), true, "", this, IsCompressed, CompressionType.Zstb);

                    if (node != null)
                        ReplaceNode(this.Parent, this, node);
                }
            }
        }

        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        public static byte[] GetASSTData(FileEntry entry)
        {
            if (entry.IsCompressed)
                return STLibraryCompression.ZSTD.Decompress(entry.data);
            else
                return entry.data;
        }
        public static void SetASST(FileEntry fileEntry, byte[] data)
        {
            if (fileEntry.IsCompressed)
                fileEntry.data = STLibraryCompression.ZSTD.Compress(data);
            else
                fileEntry.data = data;
        }

        void FillTreeNodes(TreeNode root, Dictionary<string, ASST> files)
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
                TreeNode parentNode = root;
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
                            temp = SetupFileEntry(node.Value,parentName, node.Value.FileName, node.Value.IsCompressed);
                        else
                            temp = SetupFolderEntry(temp);

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

        public FolderEntry SetupFolderEntry(TreeNode node)
        {
            FolderEntry folder = new FolderEntry();
            folder.Text = node.Text;

            return folder;
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

        public FileEntry SetupFileEntry(ASST asset,string name, string fullName, bool IsCompressed)
        {
            FileEntry fileEntry = new FileEntry();
            fileEntry.FullName = fullName;
            fileEntry.Name = name;
            fileEntry.Text = name;
            fileEntry.unk1 = asset.unk;
            fileEntry.unk2 = asset.unk2;
            fileEntry.IsCompressed = IsCompressed;
            fileEntry.data = asset.FileData;

            //Now check magic
            //Todo clean this part up
            byte[] data = asset.FileData;
            if (IsCompressed)
            {
                try
                {
                    data = STLibraryCompression.ZSTD.Decompress(asset.FileData);
                }
                catch
                {
                    Console.WriteLine("Unkwon compression for file " + fileEntry.Name);
                }
            }


            string ext = Path.GetExtension(name);
            string SarcEx = SARCExt.SARC.GuessFileExtension(data);
            if (name.EndsWith("bfres") || name.EndsWith("fmdb") || name.EndsWith("fskb") ||
                       name.EndsWith("ftsb") || name.EndsWith("fvmb") || name.EndsWith("fvbb") ||
                       name.EndsWith("fspb") || name.EndsWith("fsnb"))
            {
                fileEntry.ImageKey = "bfres";
                fileEntry.SelectedImageKey = "bfres";
            }
            if (SarcEx == ".bntx")
            {
                fileEntry.ImageKey = "bntx";
                fileEntry.SelectedImageKey = "bntx";
            }
            if (SarcEx == ".byaml")
            {
                fileEntry.ImageKey = "byaml";
                fileEntry.SelectedImageKey = "byaml";
            }
            if (SarcEx == ".aamp")
            {
                fileEntry.ImageKey = "aamp";
                fileEntry.SelectedImageKey = "aamp";
            }
            if (ext == ".lua")
            {

            }
            data = null;

            return fileEntry;
        }
    }
}
