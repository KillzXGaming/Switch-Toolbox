using System;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using FirstPlugin;
using Syroot.NintenTools.NSW.Bfres;

namespace Bfres.Structs
{
    public class FmaaFolder : TreeNodeCustom
    {
        public FmaaFolder()
        {
            Text = "Material Animations";
            Name = "FMAA";

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
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;
                foreach (FMAA fmaa in Nodes)
                {
                    string FileName = folderPath + '\\' + fmaa.Text + ".bfmaa";
                    ((FMAA)fmaa).MaterialAnim.Export(FileName, fmaa.BFRESRender.resFile);
                }
            }


        }
        private void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all material animations? This cannot be undone!", "", MessageBoxButtons.YesNo);
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

    public class FMAA : TreeNodeCustom
    {
        public BFRESRender BFRESRender;
        public MaterialAnim MaterialAnim;

        public void Read(MaterialAnim anim)
        {
            MaterialAnim = anim;
        }
    }
}
