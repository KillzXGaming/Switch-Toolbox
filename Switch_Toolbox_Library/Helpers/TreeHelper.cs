using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using System.IO;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class TreeHelper
    {
        public static void CreateFileDirectory(TreeNode root)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();
            foreach (TreeNode n in root.Nodes)
                treeNodes.Add(n);

            var rootText = root.Text;
            var rootTextLength = rootText.Length;
            root.Nodes.Clear();

            int I = 0;
            foreach (TreeNode node in treeNodes)
            {
                string nodeString = node.Text;

                var roots = nodeString.Split(new char[] { '/' },
                    StringSplitOptions.RemoveEmptyEntries);

                // The initial parent is the root node
                var parentNode = root;
                var sb = new System.Text.StringBuilder(rootText, nodeString.Length + rootTextLength);
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
                        var folder = new TreeNode(parentName);
                        if (rootIndex == roots.Length - 1)
                        {
                            node.Text = parentName;
                            parentNode.Nodes.Add(node);
                            parentNode = node;
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


        public static string[] ExtractAllFiles(string ParentPath, TreeNodeCollection Nodes, string overridePath = "")
        {
            List<string> filesExtracted = new List<string>();

            if (overridePath == string.Empty)
            {
                FolderSelectDialog folderDialog = new FolderSelectDialog();
                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return new string[0];

                overridePath = folderDialog.SelectedPath;
            }

            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Extracing Files...";
            progressBar.Refresh();
            progressBar.Value = 0;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                var Collection = TreeViewExtensions.Collect(Nodes).ToList();
                Console.WriteLine($"Collection {Collection.Count}");

                int Curfile = 0;
                foreach (TreeNode node in Collection)
                {
                    if (progressBar.IsDisposed || progressBar.Disposing) {
                        break;
                    }

                    ArchiveFileInfo file = null;

                    if (node.Tag != null && node.Tag is ArchiveFileInfo)
                        file = (ArchiveFileInfo)node.Tag;
                    else if (node is ArchiveFileWrapper)
                        file = ((ArchiveFileWrapper)node).ArchiveFileInfo;

                    if (file != null)
                    {
                        string FilePath = file.FileName;
                        string FolderPath = Path.GetDirectoryName(FilePath.RemoveIllegaleFolderNameCharacters());
                        string FolderPathDir = Path.Combine(overridePath, FolderPath);

                        if (!Directory.Exists(FolderPathDir))
                            Directory.CreateDirectory(FolderPathDir);

                        string FileName = Path.GetFileName(file.FileName).RemoveIllegaleFileNameCharacters();

                        FilePath = Path.Combine(FolderPath, FileName);

                        if (ParentPath != string.Empty)
                            FilePath = FilePath.Replace(ParentPath, string.Empty);

                        var path = $"{overridePath}/{FilePath}";

                        if (progressBar.InvokeRequired)
                        {
                            progressBar.Invoke((MethodInvoker)delegate {
                                // Running on the UI thread
                                progressBar.Task = $"Extracting File {FileName}";
                                progressBar.Value = (Curfile++ * 100) / Collection.Count;
                                progressBar.Refresh();
                            });
                        }

                        CreateDirectoryIfExists($"{path}");

                        filesExtracted.Add($"{path}");

                        if (file.FileFormat != null && file.FileFormat.CanSave)
                            file.SaveFileFormat();

                        if (file.FileDataStream != null)
                            file.FileDataStream.ExportToFile(path);
                        else
                            File.WriteAllBytes($"{path}", file.FileData);
                    }
                }

                if (progressBar.InvokeRequired)
                {
                    progressBar.Invoke((MethodInvoker)delegate {
                        progressBar.Value = 100;
                        progressBar.Refresh();
                        progressBar.Close();
                    });
                }
                else
                {
                    progressBar.Value = 100;
                    progressBar.Refresh();
                    progressBar.Close();
                }

            }));
            Thread.Start();

            return filesExtracted.ToArray();
        }

        /// <summary>
        /// S
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static List<Tuple<string, string>> ReadFiles(string directory)
        {
            var Files = new List<Tuple<string, string>>();
            ProcessDirectory(directory, "", Files);
            return Files;
        }

        private static void ProcessDirectory(string targetDirectory, string directory,
            List<Tuple<string, string>> FileList, string seperator = "/")
        {
            //Combine the target and current directory to get the full path for searching
            string folder = Path.Combine(targetDirectory, directory);

            //Search sub directories. Remove target directory from path
            foreach (string dir in Directory.GetDirectories(folder))
                ProcessDirectory(targetDirectory, dir.Replace($"{targetDirectory}\\", string.Empty), FileList);

            //Search files. Remove target directory from path
            foreach (string file in Directory.GetFiles(folder))
                FileList.Add(Tuple.Create($"{file.Replace($"{targetDirectory}\\", string.Empty)}".Replace("\\", seperator), file));
        }

        private static void CreateDirectoryIfExists(string Dir)
        {
            if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName(Dir)))
            {
                //Make sure no file names use the same name to prevent errors
                if (!File.Exists(Dir))
                {
                    if (!Directory.Exists(Dir))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Dir));
                    }
                }
            }
        }

        public static string GetFolderAbsoultePath(TreeNode node, ArchiveRootNodeWrapper rootNode)
        {
            string nodePath = node.FullPath;
            int startIndex = nodePath.IndexOf(rootNode.Text);
            if (startIndex > 0)
                nodePath = nodePath.Substring(startIndex);

            string slash = Path.DirectorySeparatorChar.ToString();
            string slashAlt = Path.AltDirectorySeparatorChar.ToString();
            return nodePath.Replace(rootNode.Text + slash, string.Empty).Replace(slash ?? "", slashAlt);
        }

        public static void AddFiles(TreeNode parentNode, ArchiveRootNodeWrapper rootNode, string[] Files)
        {
            var archiveFile = rootNode.ArchiveFile;

            if (Files == null || Files.Length <= 0 || !archiveFile.CanAddFiles) return;

            for (int i = 0; i < Files.Length; i++)
            {
                var File = ArchiveFileWrapper.FromPath(Files[i], rootNode);
                string FileName = Path.GetFileName(Files[i]);

                //Don't add the root file name
                if (parentNode.FullPath != string.Empty && !(parentNode is ArchiveRootNodeWrapper))
                {
                    string nodePath = parentNode.FullPath;
                    int startIndex = nodePath.IndexOf(rootNode.Text);
                    if (startIndex > 0)
                        nodePath = nodePath.Substring(startIndex);

                    string slash = Path.DirectorySeparatorChar.ToString();
                    string slashAlt = Path.AltDirectorySeparatorChar.ToString();
                    string SetPath = nodePath.Replace(rootNode.Text + slash, string.Empty).Replace(slash ?? "", slashAlt);

                    string FullPath = Path.Combine(SetPath, FileName).Replace(slash ?? "", slashAlt);
                    File.ArchiveFileInfo.FileName = FullPath;
                }
                else
                    File.ArchiveFileInfo.FileName = FileName;


                bool HasAddedFile = archiveFile.AddFile(File.ArchiveFileInfo);
                if (HasAddedFile)
                {
                    //Re apply the newly added archive info
                    File.ArchiveFileInfo = archiveFile.Files.LastOrDefault();

                    parentNode.Nodes.Add(File);

                    if (parentNode is ArchiveRootNodeWrapper)
                        ((ArchiveRootNodeWrapper)parentNode).AddFileNode(File);
                    if (parentNode is ArchiveFolderNodeWrapper)
                        ((ArchiveFolderNodeWrapper)parentNode).RootNode.AddFileNode(File);
                }
            }
        }

        public static void RemoveFile(ArchiveFileWrapper fileNode, IArchiveFile archiveFile)
        {
            if (!archiveFile.CanDeleteFiles) return;

            var parentNode = fileNode.Parent;

            bool HasRemovedFile = archiveFile.DeleteFile(fileNode.ArchiveFileInfo);

            if (HasRemovedFile)
                parentNode.Nodes.Remove(fileNode);
        }

        public static void RemoveFolder(TreeNode folderNode, IArchiveFile archiveFile)
        {
            if (!archiveFile.CanDeleteFiles) return;

            foreach (var node in TreeViewExtensions.Collect(folderNode.Nodes))
            {
                if (node is ArchiveFileWrapper)
                    archiveFile.DeleteFile(((ArchiveFileWrapper)node).ArchiveFileInfo);
            }

            var parentNode = folderNode.Parent;
            parentNode.Nodes.Remove(folderNode);
        }
    }
}
