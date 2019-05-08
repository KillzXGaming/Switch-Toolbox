using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library
{
    public enum FileType
    {
        Default,
        Image,
        Archive,
        Layout,
        Model,
        Effect,
        Font,
        Audio,
        Message,
        Resource,
        Shader,
        Collision,
        Parameter,
        Graphic,
        Rom,
        Spreadsheet,
    }

    public interface IFileFormat
    {
        FileType FileType { get; set; }

        bool CanSave { get; set; }

        string[] Description { get; set; }

        string[] Extension { get; set; }

        Type[] Types { get; } //Types hold menu extensions

        string FileName { get; set; }
        string FilePath { get; set; }

        bool Identify(System.IO.Stream stream);
        void Load(System.IO.Stream stream);

        void Unload();
        byte[] Save();
        IFileInfo IFileInfo { get; set; }
    }
    public class IFileInfo
    {
        public CompressionType CompressionType { get; set; }
        public bool FileIsCompressed { get; set; }
        public bool FileIsEdited { get; set; }
        public bool UseEditMenu { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool InArchive { get; set; }
        public virtual int Alignment { get; set; } //Alignment to save the file back. Also used for Yaz0 comp sometimes
        public virtual uint DecompressedSize { get; set; }
        public virtual uint CompressedSize { get; set; }
    }
}
