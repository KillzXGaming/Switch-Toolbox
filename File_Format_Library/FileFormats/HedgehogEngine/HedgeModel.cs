using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace HedgehogLibrary
{
    public class HedgeModel : IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Hedgehog Engine Model" };
        public string[] Extension { get; set; } = new string[] { "*.model" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.SetByteOrder(true);

                reader.Seek(4);
                bool IsValid = reader.ReadInt32() == 0x133054A;
                reader.Position = 0;

                return IsValid;
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
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
