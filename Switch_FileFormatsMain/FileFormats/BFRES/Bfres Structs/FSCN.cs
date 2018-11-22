using System;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using FirstPlugin;

namespace Bfres.Structs
{
    public class FscnFolder : TreeNodeCustom
    {
        public FscnFolder()
        {
            Text = "Scene Animations";
            Name = "FSCN";

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
        private void Import(object sender, EventArgs args)
        {

        }
        private void ExportAll(object sender, EventArgs args)
        {

        }
        private void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all objects? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Nodes.Clear();
            }
        }

        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }

    public class FSCN
    {

    }
}
