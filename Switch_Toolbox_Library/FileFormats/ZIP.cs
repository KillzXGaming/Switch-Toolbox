using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Toolbox.Library
{
    public class ZIP : TreeNodeFile, IFileFormat
    {
        const int MagicFileConstant = 0x504B0304;

        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "ZIP" };
        public string[] Extension { get; set; } = new string[] { "*.zip" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.SetByteOrder(true);
                return reader.ReadInt32() == MagicFileConstant;
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

        public void Load(System.IO.Stream stream)
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public void Unload()
        {

        }
    }
}
