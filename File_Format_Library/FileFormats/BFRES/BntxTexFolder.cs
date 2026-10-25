using GL_EditorFramework.EditorDrawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    internal class BntxTexFolder : STTextureFolder
    {
        private List<BNTX> _cache = new List<BNTX>();

        public BntxTexFolder(List<BNTX> textures) : base("Textures")
        {
            foreach (var tex in textures)
            {
                foreach (TextureData n in tex.Nodes)
                    Nodes.Add(n);
            }
        }

        public override ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save Edited Textures", null, (o, e) => SaveEdited(), Keys.Control | Keys.E));
            Items.Add(new ToolStripSeparator());
            Items.AddRange(base.GetContextMenuItems());
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("Import TXTG", null, (o, e) => AddTexture(), Keys.Control | Keys.E));

            return Items.ToArray();
        }

        public void SaveEdited()
        {
            if (_cache.Count == 0)
                return;

            var folder = System.IO.Path.GetDirectoryName(_cache[0].FilePath);
            FolderSelectDialog dlg = new FolderSelectDialog(folder);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // TODO
               // foreach (var tex in _cache.TextureList)
                ///    if (tex.IsEdited)
                  //      STFileSaver.SaveFileFormat(tex, System.IO.Path.Combine(dlg.SelectedPath, $"{tex.Text}.txtg"), false);
            }
        }

        private void AddTexture()
        {
            Dictionary<string, string> extensions = new Dictionary<string, string>();
            extensions.Add(".bntx", "Texture");

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileFilters.GetCompleteFilter(extensions);

            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddTexture(ofd.FileName);
            }
        }

        private void AddTexture(string filePath)
        {
            BNTX bntx = STFileLoader.OpenFileFormat(filePath) as BNTX;
            if (bntx == null) 
            {
                MessageBox.Show($"File {filePath} not a valid BNTX file!");
                return;
            }
            _cache.Add(bntx);
            Nodes.Add(bntx);
        }
    }
}
