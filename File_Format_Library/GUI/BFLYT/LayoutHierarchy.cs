using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using LayoutBXLYT.Cafe;

namespace LayoutBXLYT
{
    public partial class LayoutHierarchy : LayoutDocked
    {
        private LayoutEditor ParentEditor;
        private STContextMenuStrip ContexMenu;

        public LayoutHierarchy(LayoutEditor layoutEditor)
        {
            ParentEditor = layoutEditor;

            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            var imgList = new ImageList();
            imgList.ColorDepth = ColorDepth.Depth32Bit;
            imgList.Images.Add("folder", Toolbox.Library.Properties.Resources.Folder);
            imgList.Images.Add("AlignmentPane", FirstPlugin.Properties.Resources.AlignmentPane);
            imgList.Images.Add("WindowPane", FirstPlugin.Properties.Resources.WindowPane);
            imgList.Images.Add("ScissorPane", FirstPlugin.Properties.Resources.ScissorPane);
            imgList.Images.Add("BoundryPane", FirstPlugin.Properties.Resources.BoundryPane);
            imgList.Images.Add("NullPane", FirstPlugin.Properties.Resources.NullPane);
            imgList.Images.Add("PicturePane", FirstPlugin.Properties.Resources.PicturePane);
            imgList.Images.Add("QuickAcess", FirstPlugin.Properties.Resources.QuickAccess);
            imgList.Images.Add("TextPane", FirstPlugin.Properties.Resources.TextPane);
            imgList.Images.Add("material", Toolbox.Library.Properties.Resources.materialSphere);
            imgList.Images.Add("texture", Toolbox.Library.Properties.Resources.Texture);
            imgList.Images.Add("font", Toolbox.Library.Properties.Resources.Font);

            imgList.ImageSize = new Size(22,22);
            treeView1.ImageList = imgList;

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            ContexMenu = new STContextMenuStrip();
        }

        private bool isLoaded = false;
        private EventHandler OnProperySelected;
        private BxlytHeader ActiveLayout;
        public void LoadLayout(BxlytHeader bxlyt, EventHandler onPropertySelected)
        {
            isLoaded = false;
            OnProperySelected = onPropertySelected;

            ActiveLayout = bxlyt;

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            CreateQuickAccess(bxlyt);
            treeView1.Nodes.Add(new TreeNode("File Settings") {Tag = bxlyt });
            LoadTextures(bxlyt.Textures);
            LoadFonts(bxlyt.Fonts);
            LoadMaterials(bxlyt.GetMaterials());
            LoadPane(bxlyt.RootGroup);
            LoadPane(bxlyt.RootPane);

            treeView1.EndUpdate();

            isLoaded = true;
        }

        public void SearchAnimations(BxlytHeader bxlyt, EventHandler onPropertySelected)
        {
            OnProperySelected = onPropertySelected;

            isLoaded = false;

            var layoutFile = bxlyt.FileInfo;
            var parentArchive = layoutFile.IFileInfo.ArchiveParent;
            if (parentArchive == null) return;

            treeView1.BeginUpdate();
            foreach (var file in parentArchive.Files)
            {
                if (Utils.GetExtension(file.FileName) == ".brlan" ||
                    Utils.GetExtension(file.FileName) == ".bclan" ||
                    Utils.GetExtension(file.FileName) == ".bflan") {
                    LoadAnimation(file);
                }
            }

            treeView1.EndUpdate();
            treeView1.Sort();

            isLoaded = true;
        }

        public void LoadAnimation(ArchiveFileInfo archiveEntry)
        {
            var animNode = new TreeNode(System.IO.Path.GetFileName(archiveEntry.FileName)) { Tag = archiveEntry };
            animNode.Nodes.Add("<dummy>");
            treeView1.Nodes.Add(animNode);
        }

        public void LoadAnimation(BxlanHeader bxlan, EventHandler onPropertySelected)
        {
            isLoaded = false;
            OnProperySelected = onPropertySelected;

            treeView1.BeginUpdate();
            LoadAnimations(bxlan,new TreeNode(bxlan.FileName) { Tag = bxlan });
            treeView1.EndUpdate();
            isLoaded = true;
        }

        public void Reset()
        {
            treeView1.Nodes.Clear();
            isLoaded = false;
        }

        public void LoadAnimations(BxlanHeader bxlan, TreeNode root, bool LoadRoot = true)
        {
            if (LoadRoot)
                treeView1.Nodes.Add(root);

            if (bxlan is BxlanHeader)
            {
                var header = bxlan as BxlanHeader;
                var pat1 = new TreeNode("Tag Info") { Tag = header.AnimationTag };
                var pai1 = new TreeNode("Animation Info") { Tag = header.AnimationInfo };

                for (int i = 0; i < header.AnimationInfo.Entries.Count; i++)
                    LoadAnimationEntry(header.AnimationInfo.Entries[i], pai1);

                root.Nodes.Add(pat1);
                root.Nodes.Add(pai1);
            }
        }

        private void LoadAnimationEntry(BxlanPaiEntry entry, TreeNode root)
        {
            var nodeEntry = new TreeNode(entry.Name) { Tag = entry };
            root.Nodes.Add(nodeEntry);

            for (int i = 0;i < entry.Tags.Count; i++)
            {
                var nodeTag = new TreeNode(entry.Tags[i].Type) { Tag = entry.Tags[i] };
                nodeEntry.Nodes.Add(nodeTag);
                for (int j = 0; j < entry.Tags[i].Entries.Count; j++)
                    LoadTagEntry(entry.Tags[i].Entries[j], nodeTag, j);
            }
        }

        private void LoadTagEntry(BxlanPaiTagEntry entry, TreeNode root, int index)
        {
            var nodeEntry = new TreeNode(entry.TargetName) { Tag = entry };
            root.Nodes.Add(nodeEntry);

            for (int i = 0; i < entry.KeyFrames.Count; i++)
            {
                var keyNode = new TreeNode($"Key Frame {i}") { Tag = entry.KeyFrames[i] };
                nodeEntry.Nodes.Add(keyNode);
            }
        }

        private void LoadTextures(List<string> textures)
        {
            TreeNode node = new TreeNode("Textures");
            treeView1.Nodes.Add(node);
            for (int i = 0; i < textures.Count; i++)
            {
                TreeNode matNode = new TreeNode(textures[i]);
                matNode.Tag = i;
                matNode.ContextMenuStrip = new ContextMenuStrip();
                var menu = new STToolStipMenuItem("Rename");
                menu.Click += RenameTextureAction;
                matNode.ContextMenuStrip.Items.Add(menu);
                matNode.ImageKey = "texture";
                matNode.SelectedImageKey = "texture";
                node.Nodes.Add(matNode);
            }
        }

        private void RenameTextureAction(object sender, EventArgs e)
        {
            var selectedNode = treeView1.SelectedNode;
            if (selectedNode == null) return;

            int index = (int)selectedNode.Tag;
            string activeTex = ActiveLayout.Textures[index];

            RenameDialog dlg = new RenameDialog();
            dlg.SetString(activeTex);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ActiveLayout.Textures[index] = dlg.textBox1.Text;
                selectedNode.Text = dlg.textBox1.Text;
            }
        }

        private void LoadFonts(List<string> fonts)
        {
            TreeNode node = new TreeNode("Fonts");
            treeView1.Nodes.Add(node);
            for (int i = 0; i < fonts.Count; i++)
            {
                TreeNode matNode = new TreeNode(fonts[i]);
                matNode.ImageKey = "font";
                matNode.SelectedImageKey = "font";
                node.Nodes.Add(matNode);
            }
        }

        private void LoadMaterials(List<BxlytMaterial> materials)
        {
            TreeNode node = new TreeNode("Materials");
            treeView1.Nodes.Add(node);
            for (int i = 0; i < materials.Count; i++)
            {
                TreeNode matNode = new TreeNode(materials[i].Name);
                matNode.Tag = materials[i];
                matNode.ImageKey = "material";
                matNode.SelectedImageKey = "material";
                node.Nodes.Add(matNode);
            }
        }

        private void CreateQuickAccess(BxlytHeader bxlyt)
        {
            var panes = new List<BasePane>();
            var groupPanes = new List<BasePane>();
            GetPanes(bxlyt.RootPane,ref panes);
            GetGroupPanes(bxlyt.RootGroup,ref groupPanes);

            TreeNode node = new TreeNode("Quick Access");
            node.ImageKey = "QuickAcess";
            node.SelectedImageKey = "QuickAcess";
            treeView1.Nodes.Add(node);

            TreeNode nullFolder = new TreeNode("Null Panes");
            TreeNode textFolder = new TreeNode("Text Boxes");
            TreeNode windowFolder = new TreeNode("Window Panes");
            TreeNode pictureFolder = new TreeNode("Picture Panes");
            TreeNode boundryFolder = new TreeNode("Boundry Panes");
            TreeNode partsFolder = new TreeNode("Part Panes");
            TreeNode groupFolder = new TreeNode("Groups");

            node.Nodes.Add(nullFolder);
            node.Nodes.Add(textFolder);
            node.Nodes.Add(windowFolder);
            node.Nodes.Add(pictureFolder);
            node.Nodes.Add(boundryFolder);
            node.Nodes.Add(partsFolder);
            node.Nodes.Add(groupFolder);

            for (int i = 0; i < panes.Count; i++)
            {
                var paneNode = CreatePaneWrapper(panes[i]);
                if (panes[i] is BFLYT.WND1) windowFolder.Nodes.Add(paneNode);
                else if (panes[i] is BCLYT.WND1) windowFolder.Nodes.Add(paneNode);
                else if (panes[i] is BRLYT.WND1) windowFolder.Nodes.Add(paneNode);
                else if (panes[i] is BFLYT.PIC1) pictureFolder.Nodes.Add(paneNode);
                else if (panes[i] is BCLYT.PIC1) pictureFolder.Nodes.Add(paneNode);
                else if (panes[i] is BRLYT.PIC1) pictureFolder.Nodes.Add(paneNode);
                else if (panes[i] is BFLYT.BND1) boundryFolder.Nodes.Add(paneNode);
                else if (panes[i] is BCLYT.BND1) boundryFolder.Nodes.Add(paneNode);
                else if (panes[i] is BRLYT.BND1) boundryFolder.Nodes.Add(paneNode);
                else if (panes[i] is BCLYT.PRT1) partsFolder.Nodes.Add(paneNode);
                else if (panes[i] is BFLYT.PRT1) partsFolder.Nodes.Add(paneNode);
                else if (panes[i] is BRLYT.PRT1) partsFolder.Nodes.Add(paneNode);
                else if (panes[i] is BRLYT.TXT1) textFolder.Nodes.Add(paneNode);
                else if (panes[i] is BCLYT.TXT1) textFolder.Nodes.Add(paneNode);
                else if (panes[i] is BFLYT.TXT1) textFolder.Nodes.Add(paneNode);
                else nullFolder.Nodes.Add(paneNode);
            }

            for (int i = 0; i < groupPanes.Count; i++)
            {
                var paneNode = CreatePaneWrapper(groupPanes[i]);
                groupFolder.Nodes.Add(paneNode);
            }
        }

        private void GetPanes(BasePane pane, ref List<BasePane> panes)
        {
            panes.Add(pane);
            foreach (var childPane in pane.Childern)
                  GetPanes(childPane,ref panes);
        }

        private void GetGroupPanes(BasePane pane, ref List<BasePane> panes)
        {
            panes.Add(pane);
            foreach (var childPane in pane.Childern)
                GetPanes(childPane,ref panes);
        }

        private PaneTreeWrapper CreatePaneWrapper(BasePane pane)
        {
            PaneTreeWrapper paneNode = new PaneTreeWrapper();
            paneNode.Text = pane.Name;
            paneNode.Tag = pane;
            paneNode.Checked = true;

            string imageKey = "";
            if (pane is BFLYT.WND1) imageKey = "WindowPane";
            else if (pane is BCLYT.WND1) imageKey = "WindowPane";
            else if (pane is BRLYT.WND1) imageKey = "WindowPane";
            else if (pane is BFLYT.PIC1) imageKey = "PicturePane";
            else if (pane is BCLYT.PIC1) imageKey = "PicturePane";
            else if (pane is BRLYT.PIC1) imageKey = "PicturePane";
            else if (pane is BFLYT.BND1) imageKey = "BoundryPane";
            else if (pane is BCLYT.BND1) imageKey = "BoundryPane";
            else if (pane is BRLYT.BND1) imageKey = "BoundryPane";
            else if (pane is BFLYT.TXT1) imageKey = "TextPane";
            else if (pane is BCLYT.TXT1) imageKey = "TextPane";
            else if (pane is BRLYT.TXT1) imageKey = "TextPane";
            else imageKey = "NullPane";

            paneNode.ImageKey = imageKey;
            paneNode.SelectedImageKey = imageKey;

            return paneNode;
        }

        private void LoadPane(BasePane pane, TreeNode parent = null)
        {
            var paneNode = CreatePaneWrapper(pane);
            if (parent == null)
                treeView1.Nodes.Add(paneNode);
            else
                parent.Nodes.Add(paneNode);

            foreach (var childPane in pane.Childern)
                LoadPane(childPane, paneNode);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (isLoaded)
                OnProperySelected.Invoke("Select", e);
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (isLoaded)
            {
                if (!e.Node.Checked)
                    e.Node.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
                else
                    e.Node.ForeColor = treeView1.ForeColor;

                OnProperySelected.Invoke("Checked", e);
            }
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void TogglePane(object sender, EventArgs e)
        {
            TogglePane(treeView1.SelectedNode);
        }

        private void TogglePane(TreeNode node)
        {
            if (node == null)
                return;

            if (node.Checked)
                node.Checked = false;
            else
                node.Checked = true;
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node == null || node.Tag == null)
                return;

            if (e.KeyCode == Keys.H && e.Control)
            {
                if (node.Tag is BasePane)
                    TogglePane(node);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;

                if (e.Node.Tag is BasePane)
                {
                    ContexMenu.Items.Clear();
                    ContexMenu.Items.Add(new STToolStipMenuItem("Display Panes", null, TogglePane, Keys.Control | Keys.H));
                //    ContexMenu.Items.Add(new STToolStipMenuItem("Display Children Panes", null, TogglePane, Keys.Control | Keys.H));
                    ContexMenu.Show(Cursor.Position);
                }
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            //Create and expand a file format, then update the tag
            //Allows for faster loading
            if (e.Node.Tag is ArchiveFileInfo)
            {

            }
        }
    }
}
