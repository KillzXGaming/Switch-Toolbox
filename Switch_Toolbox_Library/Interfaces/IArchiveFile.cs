using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public enum ArchiveFileState
    {
        Empty = 0,
        Archived = 1,
        Added = 2,
        Replaced = 4,
        Renamed = 8,
        Deleted = 16
    }

    public interface IArchiveFile
    {
        bool CanAddFiles { get; } 
        bool CanRenameFiles { get; } 
        bool CanReplaceFiles { get; } 
        bool CanDeleteFiles { get; }

        IEnumerable<ArchiveFileInfo> Files { get; }

        bool AddFile(ArchiveFileInfo archiveFileInfo);
        bool DeleteFile(ArchiveFileInfo archiveFileInfo);
    }
    public class ArchiveFileInfo
    {
        public FileType FileDataType = FileType.Default;

        //Will be used for list categories
        public enum FileType
        {
            Default,
            Images,
            Archives,
            Graphics,
            Models,
            Shaders,
            Collision,
            Byaml,
            Parameters,
        }

        public string GetSize()
        {
            return STMath.GetFileSize(FileData.Length, 4);
        }

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

        public ArchiveFileState State { get; set; } = ArchiveFileState.Empty;
    }
}
