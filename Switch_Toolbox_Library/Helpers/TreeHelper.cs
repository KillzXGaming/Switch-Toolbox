using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.Forms;
using System.Windows.Forms;
using System.IO;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library
{
    public class TreeHelper
    {
        public static void ExtractAllFiles(string ParentPath, TreeNodeCollection Nodes)
        {
            FolderSelectDialog folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
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

                        string FileName = file.Text.RemoveIllegaleFileNameCharacters();

                        FilePath = Path.Combine(FolderPath, FileName);

                        if (ParentPath != string.Empty)
                            FilePath = FilePath.Replace(ParentPath, string.Empty);

                        var path = Path.Combine(folderDialog.SelectedPath, FilePath);

                        progressBar.Task = $"Extracting File {file}";
                        progressBar.Value = (Curfile++ * 100) / Collection.Count();
                        progressBar.Refresh();
                        CreateDirectoryIfExists($"{path}");

                        if (file is ArchiveFileWrapper)
                        {
                            File.WriteAllBytes($"{path}",
                                ((ArchiveFileWrapper)file).ArchiveFileInfo.FileData);
                        }
                    }
                }

                progressBar.Value = 100;
                progressBar.Refresh();
                progressBar.Close();
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


        public static void AddFiles(TreeNode parentNode, IArchiveFile archiveFile, string[] Files)
        {
            if (Files == null || Files.Length <= 0 || !archiveFile.CanAddFiles) return;

            for (int i = 0; i < Files.Length; i++)
            {
                var File = ArchiveFileWrapper.FromPath(Files[i], archiveFile);
                string FileName = Path.GetFileName(Files[i]);

                //Don't add the root file name
                if (parentNode.FullPath != string.Empty || !(parentNode is ArchiveRootNodeWrapper))
                {
                    File.ArchiveFileInfo.FileName = Path.Combine(parentNode.FullPath, FileName);
                }
                else
                    File.ArchiveFileInfo.FileName = FileName;

                bool HasAddedFile = archiveFile.AddFile(File.ArchiveFileInfo);

                if (HasAddedFile)
                    parentNode.Nodes.Add(File);
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
                var parentNode = node.Parent;
                parentNode.Nodes.Remove(node);

                if (node is ArchiveFileWrapper)
                    archiveFile.DeleteFile(((ArchiveFileWrapper)node).ArchiveFileInfo);
            }
        }
    }
}
