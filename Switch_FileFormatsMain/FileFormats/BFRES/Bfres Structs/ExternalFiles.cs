using System;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;

namespace Bfres.Structs
{
    public class EmbeddedFilesFolder : TreeNodeCustom
    {
        public EmbeddedFilesFolder()
        {
            Text = "Embedded Files";
            Name = "EXT";

            ContextMenu = new ContextMenu();
            MenuItem import = new MenuItem("Import");
            ContextMenu.MenuItems.Add(import);
            import.Click += Import;
            MenuItem exportAll = new MenuItem("Export All");
            ContextMenu.MenuItems.Add(exportAll);
            exportAll.Click += ExportAll;
            MenuItem clear = new MenuItem("Clear");
            ContextMenu.MenuItems.Add(clear);
            clear.Click += Clear;
        }
        public void Import(object sender, EventArgs args)
        {

        }
        public void ExportAll(object sender, EventArgs args)
        {

        }
        private void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all external files? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                foreach (var ext in Nodes)
                {
                    if (ext is BinaryTextureContainer)
                    {
                        PluginRuntime.bntxContainers.Remove((BinaryTextureContainer)ext);
                    }
                }

                Nodes.Clear();
                Viewport.Instance.Refresh();
            }
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class ExternalFileData : TreeNode
    {
        public byte[] Data;
        public ExternalFileData(byte[] data, string Name)
        {
            Text = Name;
            ImageKey = "folder";
            Data = data;

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Import;
        }


        private void Import(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Data = System.IO.File.ReadAllBytes(ofd.FileName);
            }
        }

        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files(*.*)|*.*";

            sfd.DefaultExt = System.IO.Path.GetExtension(Text);
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(sfd.FileName, Data);

            }
        }
    }
}
