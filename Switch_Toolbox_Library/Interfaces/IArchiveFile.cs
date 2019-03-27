using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public interface IArchiveFile : IFileFormat
    {
        bool CanAddFiles { get; } 
        bool CanRenameFiles { get; } 
        bool CanReplaceFiles { get; } 
        bool CanDeleteFiles { get; }

        IEnumerable<ArchiveFileInfo> Files { get; }
        bool AddFile(ArchiveFileInfo archiveFileInfo);
        bool DeleteFile(ArchiveFileInfo archiveFileInfo);
    }

    //This abstract class can be more advanced than the interface
    public abstract class ArchiveFile : TreeNodeCustom
    {
        public bool CanAddFiles { get; }
        public bool CanRenameFiles { get; }
        public bool CanReplaceFiles { get; }
        public bool CanDeleteFiles { get; }

        List<ArchiveFileInfo> Files { get; }

        public virtual void FillTreeNodes()
        {

        }

    /*    private void ExportAll(string Folder, STProgressBar progressBar)
        {
            int Curfile = 0;
            foreach (ArchiveFileInfo file in Files)
            {
                int value = (Curfile * 100) / Files.Count;
                progressBar.Value = value;
                progressBar.Refresh();

                try
                {
                    if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName($"{Folder}/{file.FullPath}")))
                    {
                        if (!File.Exists(file.FullPath))
                        {
                            if (!Directory.Exists($"{Folder}/{file.FullPath}"))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName($"{Folder}/{file.FullPath}"));
                            }
                        }
                    }
                    File.WriteAllBytes($"{Folder}/{file.FullPath}", file.FileData);
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
        }*/

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }
    }

    public abstract class ArchiveFileInfo : TreeNodeCustom
    {
        IFileFormat FileFormat = null; //Format attached for saving

        protected Stream _fileData = null;

        public string FileName { get; set; } = string.Empty;  //Full File Name
        public string Name { get; set; } = string.Empty; //File Name (No Path)
        public virtual Stream FileData 
        {
            get
            {
                _fileData.Position = 0;
                return _fileData;
            }
            set { _fileData = value; }
        }
    }
}
