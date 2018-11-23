using System;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using FirstPlugin;
using Syroot.NintenTools.NSW.Bfres;

namespace Bfres.Structs
{
    public class FshpaFolder : TreeNodeCustom
    {
        public FshpaFolder()
        {
            Text = "Shape Animations";
            Name = "FSHPA";

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
        public void ExportAll(object sender, EventArgs args)
        {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;
                foreach (FSHA fsha in Nodes)
                {
                    string FileName = folderPath + '\\' + fsha.Text + ".bfshpa";
                    ((FSHA)fsha).ShapeAnim.Export(FileName, fsha.GetResFile());
                }
            }
        }
        private void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all shape animations? This cannot be undone!", "", MessageBoxButtons.YesNo);

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
    public class FSHA : TreeNodeCustom
    {
        public ShapeAnim ShapeAnim;
        public FSHA()
        {
            ImageKey = "shapeAnimation";
            SelectedImageKey = "shapeAnimation";

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
            sfd.Filter = "Supported Formats|*.bfshpa;";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfshpa";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ShapeAnim.Export(sfd.FileName, GetResFile());
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfshpa;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ShapeAnim.Import(ofd.FileName);
            }
            ShapeAnim.Name = Text;
        }
        public void Read(ShapeAnim shapeAnim)
        {
            ShapeAnim = shapeAnim;
        }
    }
}
