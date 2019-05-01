using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Toolbox.Library
{
    public class GenericArchive
    {
        public List<ArchiveFileInfo> Files = new List<ArchiveFileInfo>();

    }

    public class ArchiveFileInfo
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
