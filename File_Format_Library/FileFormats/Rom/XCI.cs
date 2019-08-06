using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class XCI : IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Rom;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "XCI" };
        public string[] Extension { get; set; } = new string[] { "*.xci" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".xci");
        }

        public void Load(System.IO.Stream stream)
        {
        }
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
