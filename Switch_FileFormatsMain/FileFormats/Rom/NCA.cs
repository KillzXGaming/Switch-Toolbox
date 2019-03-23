using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;

namespace FirstPlugin
{
    public class NCA : IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "NCA" };
        public string[] Extension { get; set; } = new string[] { "*.nca" };
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
            return Utils.HasExtension(FileName, ".nca");
        }

        public void Load(System.IO.Stream stream)
        {
      
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }
    }
}
