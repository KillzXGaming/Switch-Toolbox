using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class HSF : IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Resource;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Mario Party " };
        public string[] Extension { get; set; } = new string[] { "*.hsf" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "HSFV");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public string Version;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);
                reader.ReadSignature(3, "HSF");
                Version = reader.ReadString(4, Encoding.ASCII);
                    
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
