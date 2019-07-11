using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using SuperBMDLib;
using System.Drawing;

namespace FirstPlugin
{
    public class BMD : TreeNodeFile, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Gamecube/Wii Binary Model (BMD/BDL)" };
        public string[] Extension { get; set; } = new string[] { "*.bmd", "*.bdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.SetByteOrder(true);
                bool IsBMD = reader.ReadUInt32() == 0x4A334432;
                reader.Position = 0;

                return IsBMD;
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
        public Model BMDFile;
        private TreeNode TextureFolder;
        private TreeNode ShapeFolder;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = true;

            ShapeFolder = new TreeNode("Shapes"); 
            TextureFolder = new TreeNode("Textures");
            Nodes.Add(ShapeFolder);
            Nodes.Add(TextureFolder);

            BMDFile = Model.Load(stream);

            for (int i = 0; i < BMDFile.Shapes.Shapes.Count; i++)
            {
                var shpWrapper = new BMDShapeWrapper(BMDFile.Shapes.Shapes[i]);
                shpWrapper.Text = $"Shape {i}";
                ShapeFolder.Nodes.Add(shpWrapper);
            }

            for (int i = 0; i < BMDFile.Textures.Textures.Count; i++)
            {
                var texWrapper = new BMDTextureWrapper(BMDFile.Textures.Textures[i]);
                TextureFolder.Nodes.Add(texWrapper);
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new STToolStipMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            BMDFile.ExportBMD(mem);
            return mem.ToArray();
        }
    }
}
