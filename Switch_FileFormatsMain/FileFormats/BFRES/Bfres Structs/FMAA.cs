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
                    ((FMAA)fmaa).MaterialAnim.Export(FileName, fmaa.GetResFile());
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
        public FMAA()
        {
            ImageKey = "materialAnim";
            SelectedImageKey = "materialAnim";

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
        }

        public ResFile GetResFile()
        {
            return ((BFRES)Parent.Parent).resFile;
        }
        public void Read(MaterialAnim anim)
        {
            MaterialAnim = anim;
        }
        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfmaa;";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfska";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MaterialAnim.Export(sfd.FileName, GetResFile());
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfmaa;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                MaterialAnim.Import(ofd.FileName);
            }
            MaterialAnim.Name = Text;
        }
    }
}
