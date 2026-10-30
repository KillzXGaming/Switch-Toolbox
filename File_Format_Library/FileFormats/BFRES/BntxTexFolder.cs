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
        private class BntxEntry
        {
            public BNTX Bntx;

            public List<TextureData> Textures;

            //Texture names at load/last save, used to notice renamed, added or removed textures.
            public HashSet<string> Names;
        }

        private readonly List<BntxEntry> _tracked = new List<BntxEntry>();

        public BntxTexFolder(List<BNTX> textures) : base("External Textures")
        {
            foreach (var tex in textures)
            {
                var entry = AddEntry(tex);

                foreach (var n in entry.Textures)
                    Nodes.Add(n);
            }
        }

        public override ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save Edited Textures", null, (o, e) => SaveEditedAction(), Keys.Control | Keys.E));
            Items.Add(new ToolStripSeparator());
            Items.AddRange(base.GetContextMenuItems());
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("Import BNTX", null, (o, e) => AddTexture(), Keys.Control | Keys.E));

            return Items.ToArray();
        }

        public void SaveEditedAction()
        {
            SaveEditedTextures(true);
        }

        public int SaveEditedTextures(bool showMessage = false)
        {
            int saved = 0;
            List<string> failed = new List<string>();

            foreach (var entry in _tracked)
            {
                if (!HasChanges(entry))
                    continue;

                var bntx = entry.Bntx;
                if (string.IsNullOrEmpty(bntx.FilePath))
                {
                    failed.Add($"{bntx.Text}: no file path to save to.");
                    continue;
                }

                try
                {
                    STFileSaver.SaveFileFormat(bntx, bntx.FilePath, false);

                    var current = GetTextures(entry);
                    foreach (var tex in current)
                        tex.IsEdited = false;
                    entry.Names = GetNames(current);
                    saved++;
                }
                catch (Exception ex)
                {
                    failed.Add($"{bntx.FilePath}: {ex.Message}");
                }
            }

            if (failed.Count > 0)
                MessageBox.Show("Some external textures could not be saved:\n\n" + string.Join("\n", failed), "Save Textures");
            else if (showMessage)
                MessageBox.Show(saved == 0 ? "No edited textures to save." : $"Saved {saved} texture file(s).", "Save Textures");

            return saved;
        }

        private BntxEntry AddEntry(BNTX bntx)
        {
            var entry = new BntxEntry();
            entry.Bntx = bntx;
            entry.Textures = bntx.Nodes.OfType<TextureData>().ToList();
            entry.Names = GetNames(entry.Textures);
            _tracked.Add(entry);
            return entry;
        }

        // Track names for edit checks
        private static HashSet<string> GetNames(IEnumerable<TextureData> textures)
        {
            return new HashSet<string>(textures.Select(x => x.Text), StringComparer.Ordinal);
        }

        // Track texture list for edit checks
        private static List<TextureData> GetTextures(BntxEntry entry)
        {
            if (entry.Bntx.Textures != null)
                return entry.Bntx.Textures.Values.ToList();

            return entry.Textures;
        }

        private static bool HasChanges(BntxEntry entry)
        {
            var current = GetTextures(entry);
            if (current.Any(x => x.IsEdited))
                return true;

            return !entry.Names.SetEquals(current.Select(x => x.Text));
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
            AddEntry(bntx);
            Nodes.Add(bntx);
        }
    }
}
