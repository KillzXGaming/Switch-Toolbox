using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using WeifenLuo.WinFormsUI.Docking;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
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
        public void Import(object sender, EventArgs args)
        {

        }
        public void ExportAll(object sender, EventArgs args)
        {

        }
        public void Clear(object sender, EventArgs args)
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
