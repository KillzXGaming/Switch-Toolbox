using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;

namespace FirstPlugin
{
    public class EFF : TreeNodeFile,IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Namco Effect" };
        public string[] Extension { get; set; } = new string[] { "*.eff" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "EFFN");
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
            Text = FileName;

            FileReader reader = new FileReader(stream);
            reader.Seek(4096, System.IO.SeekOrigin.Begin);

            PTCL pctl = new PTCL();
            pctl.Text = "Output.pctl";
            Nodes.Add(pctl);

            PTCL.Header Header = new PTCL.Header();
            Header.Read(reader, pctl);
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
