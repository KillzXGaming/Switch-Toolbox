using System;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;


namespace Bfres.Structs
{
    public class FbnvFolder : TreeNodeCustom
    {
        public FbnvFolder()
        {
            Text = "Bone Visabilty Animations";
            Name = "FBNV";

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
        public void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all visibility animations? This cannot be undone!", "", MessageBoxButtons.YesNo);

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
}
