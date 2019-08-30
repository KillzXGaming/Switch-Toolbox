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
using FirstPlugin;

namespace LayoutBXLYT
{
    public partial class LayoutEditor : Form
    {
        private Dictionary<string, STGenericTexture> Textures;

        public List<BxlytHeader> LayoutFiles = new List<BxlytHeader>();

        private BxlytHeader ActiveLayout;

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

            Textures = new Dictionary<string, STGenericTexture>();

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

        private List<LayoutViewer> Viewports = new List<LayoutViewer>();
        private LayoutViewer ActiveViewport;
        private LayoutHierarchy LayoutHierarchy;
        private LayoutTextureList LayoutTextureList;
        private LayoutProperties LayoutProperties;
        private LayoutTextDocked TextConverter;

        private bool isLoaded = false;
        public void LoadBxlyt(BxlytHeader header, string fileName)
        {
            LayoutFiles.Add(header);
            ActiveLayout = header;

            LayoutViewer Viewport = new LayoutViewer(header, Textures);
            Viewport.Dock = DockStyle.Fill;
            Viewport.Show(dockPanel1, DockState.Document);
            Viewport.DockHandler.AllowEndUserDocking = false;
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
            ShowPropertiesPanel();
            UpdateBackColor();
        }

        private void ResetEditors()
        {
            if (LayoutHierarchy != null)
                LayoutHierarchy.Reset();
            if (LayoutTextureList != null)
                LayoutTextureList.Reset();
            if (LayoutProperties != null)
                LayoutProperties.Reset();
            if (TextConverter != null)
                TextConverter.Reset();
        }

        private void ReloadEditors(BxlytHeader activeLayout)
        {
            if (!isLoaded) return;

            if (LayoutProperties != null)
                LayoutProperties.Reset();
            if (LayoutHierarchy != null)
                LayoutHierarchy.LoadLayout(activeLayout, ObjectSelected);
            if (LayoutTextureList != null)
                LayoutTextureList.LoadTextures(activeLayout);
            if (TextConverter != null)
            {
                if (ActiveLayout.FileInfo is BFLYT)
                    TextConverter.LoadLayout((BFLYT)ActiveLayout.FileInfo);
            }
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

            if (LayoutProperties != null && (string)sender == "Select")
            {
                if (e is TreeViewEventArgs) {
                    var node = ((TreeViewEventArgs)e).Node;
                    var pane = (BasePane)node.Tag;

                    LayoutProperties.LoadProperties(pane, OnProperyChanged);

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

        private void ShowPropertiesPanel()
        {
            LayoutProperties = new LayoutProperties();
            LayoutProperties.Text = "Properties";
            LayoutProperties.Show(dockPanel1, DockState.DockRight);
        }

        private void ShowPaneHierarchy()
        {
            LayoutHierarchy = new LayoutHierarchy();
            LayoutHierarchy.Text = "Hierarchy";
            LayoutHierarchy.LoadLayout(ActiveLayout, ObjectSelected);
            LayoutHierarchy.Show(dockPanel1, DockState.DockLeft);
        }

        private void ShowTextureList()
        {
            LayoutTextureList = new LayoutTextureList();
            LayoutTextureList.Text = "Texture List";
            LayoutTextureList.LoadTextures(ActiveLayout);
            LayoutTextureList.Show(dockPanel1, DockState.DockRight);
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

        private void dockPanel1_ActiveDocumentChanged(object sender, EventArgs e)
        {
            var dockContent = dockPanel1.ActiveDocument as LayoutViewer;
            if (dockContent != null)
            {
                var file = (dockContent).LayoutFile;
                ReloadEditors(file);

                dockContent.UpdateViewport();
            }
        }

        private void LayoutEditor_DragDrop(object sender, DragEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
                OpenFile(filename);

            Cursor.Current = Cursors.Default;
        }

        private void OpenFile(string fileName)
        {
            //Todo if an image is dropped, we should make a picture pane if a viewer is active

            var file = STFileLoader.OpenFileFormat(fileName);
            if (file == null) return;

            if (file is BFLYT)
                LoadBxlyt(((BFLYT)file).header, file.FileName);
            else if (file is BCLYT)
                LoadBxlyt(((BCLYT)file).header, file.FileName);
            else if (file is IArchiveFile)
            {
                var layouts = SearchLayoutFiles((IArchiveFile)file);
                if (layouts.Count > 1)
                {
                    var form = new FileSelector();
                    form.LoadLayoutFiles(layouts);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var layout in form.SelectedLayouts())
                        {
                            if (layout is BFLYT)
                                LoadBxlyt(((BFLYT)layout).header, file.FileName);
                            if (layout is BCLYT)
                                LoadBxlyt(((BCLYT)layout).header, file.FileName);
                        }
                    }                    
                }
                else if (layouts.Count > 0)
                {
                    if (layouts[0] is BFLYT)
                        LoadBxlyt(((BFLYT)layouts[0]).header, file.FileName);
                    if (layouts[0] is BCLYT)
                        LoadBxlyt(((BCLYT)layouts[0]).header, file.FileName);
                }
            }
            else if (file is BFLAN)
            {

            }
            else if (file is BNTX)
            {

            }
        }

        private List<IFileFormat> SearchLayoutFiles(IArchiveFile archiveFile)
        {
            List<IFileFormat> layouts = new List<IFileFormat>();

            foreach (var file in archiveFile.Files)
            {
                var fileFormat = STFileLoader.OpenFileFormat(file.FileName,
                    new Type[] { typeof(BFLYT), typeof(BCLYT), typeof(SARC) }, file.FileData);

                if (fileFormat is BFLYT)
                {
                    fileFormat.IFileInfo.ArchiveParent = archiveFile;
                    layouts.Add(fileFormat);
                }
                else if (fileFormat is BCLYT)
                {
                    fileFormat.IFileInfo.ArchiveParent = archiveFile;
                    layouts.Add(fileFormat);
                }
                else if (Utils.GetExtension(file.FileName) == ".bntx")
                {
                   
                }
                else if (Utils.GetExtension(file.FileName) == ".bflim")
                {

                }
                else if (fileFormat is SARC)
                {
                    fileFormat.IFileInfo.ArchiveParent = archiveFile;

                    if (fileFormat is IArchiveFile)
                        return SearchLayoutFiles((IArchiveFile)fileFormat);
                }
            }

            return layouts;
        }

        private void LayoutEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
            {
                String[] strGetFormats = e.Data.GetFormats();
                e.Effect = DragDropEffects.None;
            }
        }

        private void clearWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var docs = dockPanel1.DocumentsToArray();
            for (int i = 0; i < docs.Length; i++)
                docs[i].DockHandler.DockPanel = null;

            ResetEditors();

            GC.Collect();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in ofd.FileNames)
                    OpenFile(filename);
            }
        }

        private void textConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveLayout.FileInfo is BFLYT)
            {
                TextConverter = new LayoutTextDocked();
                TextConverter.LoadLayout((BFLYT)ActiveLayout.FileInfo);
                TextConverter.Show(dockPanel1, DockState.DockLeft);
            }
        }
    }
}
