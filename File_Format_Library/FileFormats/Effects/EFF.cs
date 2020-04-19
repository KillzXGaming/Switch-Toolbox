using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using System.Windows.Forms;

namespace FirstPlugin
{
    public class EFF : TreeNodeFile, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Effect;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Namco Effect" };
        public string[] Extension { get; set; } = new string[] { "*.eff" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
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

        private PTCL ptcl;
        private byte[] DataStart;
        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = true;

            FileReader reader = new FileReader(stream);
            int SectionSize = 0;
            while (!reader.EndOfStream)
            {
                string magicCheck = reader.ReadString(4, Encoding.ASCII);
                if (magicCheck == "VFXB")
                {
                    reader.Seek(-4);
                    break;
                }

                SectionSize += 4;
            }

            if (SectionSize == reader.BaseStream.Length)
                return;

            DataStart = reader.getSection(0, SectionSize);

            Text = FileName;
            ptcl = new PTCL();
            ptcl.IFileInfo = new IFileInfo();
            ptcl.FileName = "Output.pctl";
            Nodes.Add(ptcl);

            ptcl.Load(new SubStream(stream, reader.Position));
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                 new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S),
            };
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(typeof(EFF));
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
                STFileSaver.SaveFileFormat(this, sfd.FileName);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.Write(DataStart);
                var mem = new System.IO.MemoryStream();
                ptcl.Save(mem);
                writer.Write(mem.ToArray());
            }
        }
    }
}
