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
using ZstdNet;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class BEA : IFileFormat
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

                            foreach (var asset in beaFile.FileList.Values)
                            {
                                if (Path.GetExtension(asset.FileName) == ".lua")
                                {
                                    try
                                    {
                                        if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName($"{folderPath}/{beaFile.Name}/{asset.FileName}")))
                                        {
                                            if (!File.Exists(asset.FileName))
                                            {
                                                if (!Directory.Exists($"{folderPath}/{beaFile.Name}/{asset.FileName}"))
                                                {
                                                    Directory.CreateDirectory(Path.GetDirectoryName($"{folderPath}/{beaFile.Name}/{asset.FileName}"));
                                                }
                                            }
                                        }
                                        File.WriteAllBytes($"{folderPath}/{beaFile.Name}/{asset.FileName}", GetASSTData(asset.FileName));
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
            IsActive = true;
            CanSave = true;

            beaFile = new BezelEngineArchive(new MemoryStream(Data));
            EditorRoot = new RootNode(Path.GetFileName(FileName));
            TreeNode root = EditorRoot;

            FillTreeNodes(root, beaFile.FileList);
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
        public byte[] Save()
        {
            foreach (TreeNode node in Collect(EditorRoot.Nodes))
            {
                if (node is FileEntry)
                {
                    Console.WriteLine(node);
                    if (((FileEntry)node).FileHandle != null)
                    {
                        Console.WriteLine("Saving FileHandle");
                        SaveFileEntryData((FileEntry)node);
                    }
                }
            }

            MemoryStream mem = new MemoryStream();
            beaFile.Save(mem);
            return mem.ToArray();
        }


        public class RootNode : TreeNodeCustom
        {
            public RootNode(string n)
            {
                Text = n;

                ContextMenu = new ContextMenu();
                MenuItem previewFiles = new MenuItem("Preview Window");
                ContextMenu.MenuItems.Add(previewFiles);
                previewFiles.Click += PreviewWindow;
                MenuItem exportAll = new MenuItem("Export All");
                ContextMenu.MenuItems.Add(exportAll);
                exportAll.Click += ExportAll;
            }
            public override void OnClick(TreeView treeview)
            {

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
                foreach (ASST asst in beaFile.FileList.Values)
                {
                    int value = (Curfile * 100) / beaFile.FileList.Count;
                    progressBar.Value = value;
                    progressBar.Refresh();

                    try
                    {
                        if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName($"{Folder}/{beaFile.Name}/{asst.FileName}")))
                        {
                            if (!File.Exists(asst.FileName))
                            {
                                if (!Directory.Exists($"{Folder}/{beaFile.Name}/{asst.FileName}"))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName($"{Folder}/{beaFile.Name}/{asst.FileName}"));
                                }
                            }
                        }
                        File.WriteAllBytes($"{Folder}/{beaFile.Name}/{asst.FileName}", GetASSTData(asst.FileName));
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
                if (treeNode is FileEntry)
                {
                    FileEntry file = (FileEntry)treeNode;

                    if (file.ImageKey == "bntx")
                        OpenFile(file.Name, GetASSTData(file.FullName), TreeView);

                    if (file.ImageKey == "bntx")
                        Console.WriteLine(file.Name);
                  //  if (file.ImageKey == "bfres")
                  //   OpenFile(file.Name, GetASSTData(file.FullName), TreeView);
                }

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

                SupportedFormats = null;
                data = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
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

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.DefaultExt = Path.GetExtension(Text);
                sfd.Filter = "All files(*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, GetASSTData(FullName));
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
                    SetASST(File.ReadAllBytes(ofd.FileName), FullName);
                }
            }

            public override void OnClick(TreeView treeView)
            {
                if (beaFile != null)
                {
                    if (GetASSTData(FullName) != null)
                        OpenFile(Name, GetASSTData(FullName), treeView);
                }
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

                        //     ReplaceNode(this.Parent, this, format.EditorRoot);
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
        }

        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        public static byte[] GetASSTData(string path)
        {
            if (beaFile.FileList.ContainsKey(path))
            {
                if (beaFile.FileList[path].UncompressedSize == beaFile.FileList[path].FileData.Length)
                {
                    return beaFile.FileList[path].FileData;
                }
                else
                {
                    using (var decompressor = new Decompressor())
                    {
                        return decompressor.Unwrap(beaFile.FileList[path].FileData);
                    }
                }

            }
            return null;
        }
        public static void SetASST(byte[] data, string path)
        {
            if (beaFile.FileList.ContainsKey(path))
            {
                ASST asst = beaFile.FileList[path];
                Console.WriteLine(path + " A match!");

                asst.UncompressedSize = data.Length;

                if (asst.IsCompressed)
                {
                    using (var compressor = new Compressor())
                    {
                        asst.FileData = compressor.Wrap(data);
                    }
                }
                else
                {
                    asst.FileData = data;
                }
            }
        }

        private void SaveFileEntryData(FileEntry entry)
        {
            IFileFormat file = entry.FileHandle;
            if (beaFile.FileList.ContainsKey(entry.FullName))
            {
                if (file.CanSave)
                {
                    SetASST(file.Save(), entry.FullName);
                }
            }
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
                            temp = SetupFileEntry(node.Value.FileData, parentName, node.Value.FileName, node.Value.IsCompressed);
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

        public FileEntry SetupFileEntry(byte[] data, string name, string fullName, bool IsCompressed)
        {
            FileEntry fileEntry = new FileEntry();
            fileEntry.FullName = fullName;
            fileEntry.Name = name;
            fileEntry.Text = name;

            if (IsCompressed)
            {
                try
                {
                    using (var decompressor = new Decompressor())
                    {
                        data = decompressor.Unwrap(data);
                    }
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
