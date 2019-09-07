﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstPlugin;
using Toolbox.Library.Forms;
using LayoutBXLYT.Cafe;

namespace LayoutBXLYT
{
    public partial class LayoutHierarchy : LayoutDocked
    {
        private STContextMenuStrip ContexMenu;

        public LayoutHierarchy()
        {
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
        public void LoadLayout(BxlytHeader bxlyt, EventHandler onPropertySelected)
        {
            isLoaded = false;
            OnProperySelected = onPropertySelected;

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

        public void Reset()
        {
            treeView1.Nodes.Clear();
            isLoaded = false;
        }

        private void LoadTextures(List<string> textures)
        {
            TreeNode node = new TreeNode("Textures");
            treeView1.Nodes.Add(node);
            for (int i = 0; i < textures.Count; i++)
            {
                TreeNode matNode = new TreeNode(textures[i]);
                matNode.ImageKey = "texture";
                matNode.SelectedImageKey = "texture";
                node.Nodes.Add(matNode);
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
    }
}
