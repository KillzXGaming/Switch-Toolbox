using System;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;
using Syroot.NintenTools.NSW.Bfres;

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
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;
                foreach (FBNV fbnv in Nodes)
                {
                    string FileName = folderPath + '\\' + fbnv.Text + ".bfska";
                    ((FBNV)fbnv).VisibilityAnim.Export(FileName, fbnv.GetResFile());
                }
            }
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
    public class FBNV : TreeNodeCustom
    {
        public VisibilityAnim VisibilityAnim;
        public FBNV()
        {
            ImageKey = "visibilityAnim";
            SelectedImageKey = "visibilityAnim";

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
            return ((ResourceFile)Parent.Parent).resFile;
        }

        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfvis;";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfvis";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                VisibilityAnim.Export(sfd.FileName, GetResFile());
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfvis;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                VisibilityAnim.Import(ofd.FileName);
            }
            VisibilityAnim.Name = Text;
        }
        public void Read(VisibilityAnim vis)
        {
            VisibilityAnim = vis;
        }
    }
}
