using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public interface IArchiveFile
    {
        string[] Description { get; set; }
        string[] Extension { get; set; }
        string Magic { get; set; }
        string CompressionType { get; set; }
        bool FileIsCompressed { get; set; }
        bool FileIsEdited { get; set; }
        bool CanSave { get; set; }
        bool IsActive { get; set; }
        bool UseEditMenu { get; set; }
        byte[] Data { get; set; }
        string FileName { get; set; }
        TreeNode EditorRoot { get; set; }
        void Load();
        void Save();
    }
}
