using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;

namespace FirstPlugin
{
    public class MSBT : IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Message Binary Text" };
        public string[] Extension { get; set; } = new string[] { "*.msbt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(8, "MsgStdBn");
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
        public byte[] Save()
        {
            return null;
        }
    }
}
