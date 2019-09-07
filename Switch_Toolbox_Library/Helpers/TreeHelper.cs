﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var Collection = TreeViewExtensions.Collect(Nodes);

            int Curfile = 0;
            foreach (TreeNode file in Collection)
            {
                if (file is ArchiveFileWrapper)
                {
                    string FilePath = ((ArchiveFileWrapper)file).ArchiveFileInfo.FileName;
                    string FolderPath = Path.GetDirectoryName(FilePath.RemoveIllegaleFolderNameCharacters());
                    string FolderPathDir = Path.Combine(overridePath, FolderPath);

                    if (!Directory.Exists(FolderPathDir))
                        Directory.CreateDirectory(FolderPathDir);

                    string FileName = file.Text.RemoveIllegaleFileNameCharacters();

                    FilePath = Path.Combine(FolderPath, FileName);

                    if (ParentPath != string.Empty)
                        FilePath = FilePath.Replace(ParentPath, string.Empty);

                    var path = Path.Combine(overridePath, FilePath);

                    progressBar.Task = $"Extracting File {file}";
                    progressBar.Value = (Curfile++ * 100) / Collection.Count();
                    progressBar.Refresh();
                    CreateDirectoryIfExists($"{path}");

                    if (file is ArchiveFileWrapper)
                    {
                        filesExtracted.Add($"{path}");

                        File.WriteAllBytes($"{path}",
                            ((ArchiveFileWrapper)file).ArchiveFileInfo.FileData);
                    }
                }
            }

            progressBar.Value = 100;
            progressBar.Refresh();
            progressBar.Close();

            return filesExtracted.ToArray();
        }

        public static List<Tuple<string, string>> ReadFiles(string directory)
        {
            var Files = new List<Tuple<string, string>>();
            ProcessDirectory(directory, Files);
            return Files;
        }

        private static void ProcessDirectory(string targetDirectory, List<Tuple<string, string>> FileList)
        {
            var fileEntries = GetRelativePaths(targetDirectory);

            foreach (string fileName in fileEntries)
            {
                char[] sep = { '\\' };
                string[] fn = fileName.Split(sep);
                string tempf = "";
                for (int i = 0; i < fn.Length; i++)
                {
                    tempf += fn[i];
                    if (fn.Length > 2 && (i != fn.Length - 1))
                    {
                        tempf += "/";
                    }
                }

                Console.WriteLine($"{tempf} {targetDirectory} {fileName}");

                FileList.Add(Tuple.Create(fileName, $"{targetDirectory}/{fileName}"));
            }

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, FileList);
        }

        private static IEnumerable<string> GetRelativePaths(string root)
        {
            int rootLength = root.Length + (root[root.Length - 1] == '\\' ? 0 : 1);

            foreach (string path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            {
                yield return path.Remove(0, rootLength);
            }
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
