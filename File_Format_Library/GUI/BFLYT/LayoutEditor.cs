using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using WeifenLuo.WinFormsUI.Docking;
using WeifenLuo.WinFormsUI.ThemeVS2015;
using Toolbox.Library.IO;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public partial class LayoutEditor : Form
    {
        public List<BFLYT.Header> LayoutFiles = new List<BFLYT.Header>();

        private BFLYT.Header ActiveLayout;

        public enum DockLayout
        {
            Default,
            Animation,
        }

        public EventHandler ObjectSelected;
        public EventHandler ObjectChanged;

        public LayoutEditor()
        {
            InitializeComponent();

            var theme = new VS2015DarkTheme();
            this.dockPanel1.Theme = theme;
            this.dockPanel1.BackColor = FormThemes.BaseTheme.FormBackColor;
            this.BackColor = FormThemes.BaseTheme.FormBackColor;

            viewportBackColorCB.Items.Add("Back Color : Default");
            viewportBackColorCB.Items.Add("Back Color : Custom");
            viewportBackColorCB.SelectedIndex = 0;

            ObjectSelected += OnObjectSelected;
            ObjectChanged += OnObjectChanged;
        }

        private DockContent TextureListDock;
        private DockContent GroupTreeDock;
        private DockContent PaneTreeDock;
        private DockContent ColorDock;
        private DockContent PropertiesDock;

        private List<LayoutViewer> Viewports = new List<LayoutViewer>();
        private LayoutViewer ActiveViewport;

        private bool isLoaded = false;
        public void LoadBflyt(BFLYT.Header header, string fileName)
        {
            LayoutFiles.Add(header);
            ActiveLayout = header;

            LayoutViewer Viewport = new LayoutViewer(header);
            Viewport.Dock = DockStyle.Fill;
            DockShow(Viewport, fileName, DockState.Document);
            Viewports.Add(Viewport);
            ActiveViewport = Viewport;

            if (!isLoaded)
                InitializeDockPanels();

            isLoaded = true;
        }

        private void InitializeDockPanels()
        {
            ShowTextureList();
            ShowPaneHierarchy();
            ShowGroupsHierarchy();
            ShowPropertiesPanel();

            UpdateBackColor();

        }

        private void OnObjectChanged(object sender, EventArgs e)
        {

        }

        private void OnProperyChanged()
        {
            if (ActiveViewport != null)
                ActiveViewport.UpdateViewport();
        }

        private bool isChecked = false;
        private void OnObjectSelected(object sender, EventArgs e)
        {
            if (isChecked) return;

            ActiveViewport.SelectedPanes.Clear();

            if (PropertiesDock != null && (string)sender == "Select")
            {
                if (e is TreeViewEventArgs) {
                    var node = ((TreeViewEventArgs)e).Node;
                    var pane = (BasePane)node.Tag;

                    ((LayoutProperties)PropertiesDock.Controls[0]).LoadProperties(pane, OnProperyChanged);

                    ActiveViewport.SelectedPanes.Add(pane);
                }
            }
            if (ActiveViewport != null)
            {
                if (e is TreeViewEventArgs && (string)sender == "Checked" && !isChecked) {
                    isChecked = true;
                    var node = ((TreeViewEventArgs)e).Node;
                    ToggleChildern(node, node.Checked);
                    isChecked = false;
                }

                ActiveViewport.UpdateViewport();
            }
        }

        private void ToggleChildern(TreeNode node, bool isChecked)
        {
            if (node.Tag is BasePane)
                ((BasePane)node.Tag).DisplayInEditor = isChecked;

            node.Checked = isChecked;
            foreach (TreeNode child in node.Nodes)
                ToggleChildern(child, isChecked);
        }

        private DockContent DockShow(UserControl control, string text, DockAlignment dockState, DockContent dockSide = null, float Alignment = 0)
        {
            DockContent content = CreateContent(control, text);
            content.Show(dockSide.Pane, dockState, Alignment);
            return content;
        }

        private DockContent DockShow(UserControl control, string text, DockState dockState)
        {
            DockContent content = CreateContent(control, text);
            content.Show(dockPanel1, dockState);
            return content;
        }

        private DockContent CreateContent(UserControl control, string text)
        {
            DockContent content = new DockContent();
            content.Text = text;
            control.Dock = DockStyle.Fill;
            content.Controls.Add(control);
            return content;
        }

        public void LoadBflan()
        {

        }

        public void InitalizeEditors()
        {

        }

        private void LayoutEditor_ParentChanged(object sender, EventArgs e)
        {
            if (this.ParentForm == null) return;
        }

        private void textureListToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowTextureList();
        }

        private void ShowGroupsHierarchy()
        {
            dockPanel1.GetContainerControl();

            LayoutHierarchy hierarchyList = new LayoutHierarchy();
            hierarchyList.LoadLayout(ActiveLayout,ObjectSelected,  true);
            GroupTreeDock = DockShow(hierarchyList, "Groups", DockAlignment.Top, TextureListDock, 0.5f);

        }

        private void ShowPropertiesPanel()
        {
            LayoutProperties properties = new LayoutProperties();
            PropertiesDock = DockShow(properties, "Properties", DockAlignment.Top, TextureListDock, 0.5f);
        }

        private void ShowPaneHierarchy()
        {
            LayoutHierarchy hierarchyList = new LayoutHierarchy();
            hierarchyList.LoadLayout(ActiveLayout, ObjectSelected);
            PaneTreeDock = DockShow(hierarchyList, "Panes", DockAlignment.Top, TextureListDock, 0.5f);
        }

        private void ShowTextureList()
        {
            LayoutTextureList textureListForm = new LayoutTextureList();
            textureListForm.LoadTextures(ActiveLayout);
            TextureListDock = DockShow(textureListForm, "Texture List", DockState.DockRight);
        }

        private void stComboBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private bool isBGUpdating = false;
        private void viewportBackColorCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveViewport == null || isBGUpdating) return;
            UpdateBackColor();
        }

        private void UpdateBackColor()
        {
            if (viewportBackColorCB.SelectedIndex == 0)
            {
                ActiveViewport.UpdateBackgroundColor(Color.FromArgb(130, 130, 130));
                backColorDisplay.BackColor = Color.FromArgb(130, 130, 130);
            }
            else
            {
                ColorDialog dlg = new ColorDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ActiveViewport.UpdateBackgroundColor(dlg.Color);
                    backColorDisplay.BackColor = dlg.Color;
                }
                else
                    viewportBackColorCB.SelectedIndex = 0;
            }
        }

        private void backColorDisplay_Click(object sender, EventArgs e)
        {
            isBGUpdating = true;

            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ActiveViewport.UpdateBackgroundColor(dlg.Color);
                backColorDisplay.BackColor = dlg.Color;

                if (viewportBackColorCB.SelectedIndex == 0)
                    viewportBackColorCB.SelectedIndex = 1;
            }

            isBGUpdating = false;
        }
    }
}
