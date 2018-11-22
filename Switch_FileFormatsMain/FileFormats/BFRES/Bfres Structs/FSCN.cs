using System;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using FirstPlugin;
using Syroot.NintenTools.NSW.Bfres;

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
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all scene animations? This cannot be undone!", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Nodes.Clear();
            }
        }
        public void LoadAnimations(ResFile resFile, BFRESRender BFRESRender)
        {
            foreach (var scn in resFile.SceneAnims)
            {
                FSCN sceneAnim = new FSCN();
                sceneAnim.Text = scn.Name;
                sceneAnim.BFRESRender = BFRESRender;
                sceneAnim.Read(scn);
                Nodes.Add(sceneAnim);
            }
        }

        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }

    public class FSCN : TreeNodeCustom
    {
        public SceneAnim SceneAnim;
        public BFRESRender BFRESRender;

        public FSCN()
        {
            ImageKey = "skeletonAnimation";
            SelectedImageKey = "skeletonAnimation";

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
        }

        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfska;";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfska";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SceneAnim.Export(sfd.FileName, BFRESRender.resFile);
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfska;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SceneAnim.Import(ofd.FileName);
            }
            SceneAnim.Name = Text;
        }
        public void Read(SceneAnim scn)
        {
            SceneAnim = scn;
        }
    }
}
