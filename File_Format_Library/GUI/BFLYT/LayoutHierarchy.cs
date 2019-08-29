using System;
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

namespace LayoutBXLYT
{
    public partial class LayoutHierarchy : UserControl
    {
        public LayoutHierarchy()
        {
            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            var imgList = new ImageList();
            imgList.Images.Add("AlignmentPane", FirstPlugin.Properties.Resources.AlignmentPane);
            imgList.Images.Add("WindowPane", FirstPlugin.Properties.Resources.WindowPane);
            imgList.Images.Add("ScissorPane", FirstPlugin.Properties.Resources.ScissorPane);
            imgList.Images.Add("BoundryPane", FirstPlugin.Properties.Resources.BoundryPane);
            imgList.Images.Add("NullPane", FirstPlugin.Properties.Resources.NullPane);
            imgList.Images.Add("PicturePane", FirstPlugin.Properties.Resources.PicturePane);
            imgList.ImageSize = new Size(22,22);
            treeView1.ImageList = imgList;
        }

        private bool isLoaded = false;
        private EventHandler OnProperySelected;
        public void LoadLayout(BxlytHeader bflyt, EventHandler onPropertySelected)
        {
            isLoaded = false;
            OnProperySelected = onPropertySelected;

            treeView1.Nodes.Clear();

            LoadPane(bflyt.RootGroup);
            LoadPane(bflyt.RootPane);

            isLoaded = true;
        }

        private void LoadPane(BasePane pane, TreeNode parent = null)
        {
            PaneTreeWrapper paneNode = new PaneTreeWrapper();
            paneNode.Checked = true;
            paneNode.Text = pane.Name;
            paneNode.Tag = pane;

            string imageKey = "";
            if (pane is BFLYT.WND1) imageKey = "WindowPane";
            else if (pane is BFLYT.PIC1) imageKey = "PicturePane";
            else if (pane is BFLYT.BND1) imageKey = "BoundryPane";
            else imageKey = "NullPane";

            paneNode.ImageKey = imageKey;
            paneNode.SelectedImageKey = imageKey;

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
                OnProperySelected.Invoke("Checked", e);
        }
    }
}
