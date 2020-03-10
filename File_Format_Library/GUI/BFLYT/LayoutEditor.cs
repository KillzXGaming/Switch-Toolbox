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
using LayoutBXLYT.Cafe;

namespace LayoutBXLYT
{
    public partial class LayoutEditor : Form, IArchiveEditor
    {
        public EventHandler UpdateArchiveFile { get; set; } = null;

        /// <summary>
        /// Enables or disables legacy opengl support
        /// Modern support is not quite finished yet so keep enabled!
        /// </summary>
        public static bool UseLegacyGL = true;

        public LayoutViewer GamePreviewWindow;

        private LayoutCustomPaneMapper CustomMapper;

        private Dictionary<string, STGenericTexture> Textures;

        public List<BxlytHeader> LayoutFiles = new List<BxlytHeader>();
        public List<BxlanHeader> AnimationFiles = new List<BxlanHeader>();
        public List<BxlanHeader> SelectedAnimations = new List<BxlanHeader>();

        public List<BasePane> SelectedPanes
        {
            get { return ActiveViewport?.SelectedPanes; }
        }

        public BxlytHeader ActiveLayout;
        private BxlanHeader ActiveAnimation;

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

            Text = $"Switch Toolbox Layout Editor";

            chkAutoKey.Enabled = false;
            chkAutoKey.ForeColor = FormThemes.BaseTheme.FormForeColor;

            CustomMapper = new LayoutCustomPaneMapper();

            Textures = new Dictionary<string, STGenericTexture>();

            ThemeBase theme = new VS2015DarkTheme();
            if (FormThemes.ActivePreset == FormThemes.Preset.White)
                theme = new VS2015LightTheme();

            this.dockPanel1.Theme = theme;
            this.dockPanel1.BackColor = FormThemes.BaseTheme.FormBackColor;
            this.BackColor = FormThemes.BaseTheme.FormBackColor;

            redoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = false;

            viewportBackColorCB.Items.Add("Back Color : Default");
            viewportBackColorCB.Items.Add("Back Color : Custom");
            orthographicViewToolStripMenuItem.Checked = true;

            editorModeCB.Items.Add("Normal");
            editorModeCB.Items.Add("Animation");
            editorModeCB.SelectedIndex = 0;

            foreach (var type in Enum.GetValues(typeof(Runtime.LayoutEditor.DebugShading)).Cast<Runtime.LayoutEditor.DebugShading>())
                debugShading.Items.Add(type);

            debugShading.SelectedItem = Runtime.LayoutEditor.Shading;
            displayNullPanesToolStripMenuItem.Checked = Runtime.LayoutEditor.DisplayNullPane;
            displayyBoundryPanesToolStripMenuItem.Checked = Runtime.LayoutEditor.DisplayBoundryPane;
            displayPicturePanesToolStripMenuItem.Checked = Runtime.LayoutEditor.DisplayPicturePane;
            displayWindowPanesToolStripMenuItem.Checked = Runtime.LayoutEditor.DisplayWindowPane;
            renderInGamePreviewToolStripMenuItem.Checked = Runtime.LayoutEditor.IsGamePreview;
            displayGridToolStripMenuItem.Checked = Runtime.LayoutEditor.DisplayGrid;
            displayTextPanesToolStripMenuItem.Checked = Runtime.LayoutEditor.DisplayTextPane;
            transformChildrenToolStripMenuItem.Checked = Runtime.LayoutEditor.TransformChidlren;
            viewPartsAsNullPanesToolStripMenuItem.Checked = Runtime.LayoutEditor.PartsAsNullPanes;

            showAnimationListToolStripMenuItem.Checked = true;
            showPropertiesToolStripMenuItem.Checked = true;
            showAnimationListToolStripMenuItem.Checked = true;
            showPanelHiearchyToolStripMenuItem.Checked = true;
            showTextureListToolStripMenuItem.Checked = true;

            ObjectSelected += OnObjectSelected;
            ObjectChanged += OnObjectChanged;

            if (Runtime.LayoutEditor.BackgroundColor != Color.FromArgb(130, 130, 130))
                viewportBackColorCB.SelectedIndex = 1;
            else
                viewportBackColorCB.SelectedIndex = 0;
        }

        private List<LayoutViewer> Viewports = new List<LayoutViewer>();
        private LayoutAnimEditor LayoutAnimEditor;
        private LayoutViewer ActiveViewport;
        private LayoutHierarchy LayoutHierarchy;
        private LayoutAnimList LayoutAnimList;
        private LayoutTextureList LayoutTextureList;
        private LayoutProperties LayoutProperties;
        private LayoutTextDocked TextConverter;
        private LayoutPartsEditor LayoutPartsEditor;
        private STAnimationPanel AnimationPanel;
        private PaneEditor LayoutPaneEditor;
        private LytAnimationWindow AnimationWindow;

        private bool isLoaded = false;
        public void LoadBxlyt(BxlytHeader header)
        {
            Text = $"Switch Toolbox Layout Editor [{header.FileName}]";
            /*   if (PluginRuntime.BxfntFiles.Count > 0)
               {
                   var result = MessageBox.Show("Found font files opened. Would you like to save character images to disk? " +
                       "(Allows for faster loading and won't need to be reopened)", "Layout Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                   if (result == DialogResult.Yes)
                   {
                       var cacheDir = $"{Runtime.ExecutableDir}/Cached/Font";
                       if (!System.IO.Directory.Exists(cacheDir))
                           System.IO.Directory.CreateDirectory(cacheDir);

                       foreach (var bffnt in PluginRuntime.BxfntFiles)
                       {
                           if (!System.IO.Directory.Exists($"{cacheDir}/{bffnt.Name}"))
                               System.IO.Directory.CreateDirectory($"{cacheDir}/{bffnt.Name}");

                           var fontBitmap = bffnt.GetBitmapFont(true);
                           foreach (var character in fontBitmap.Characters)
                           {
                               var charaMap = character.Value;
                               charaMap.CharBitmap.Save($"{cacheDir}/{bffnt.Name}/cmap_{Convert.ToUInt16(character.Key).ToString("x2")}_{charaMap.CharWidth}_{charaMap.GlyphWidth}_{charaMap.LeftOffset}.png");
                           }

                           fontBitmap.Dispose();
                       }
                   }
               }*/

            LayoutFiles.Add(header);
            ActiveLayout = header;

            //Update the saving for the layout
            ActiveLayout.FileInfo.CanSave = true;

            LayoutViewer Viewport = new LayoutViewer(this, header, Textures);
            Viewport.DockContent = new DockContent();
            Viewport.DockContent.Text = header.FileName;
            Viewport.DockContent.Controls.Add(Viewport);
            Viewport.Dock = DockStyle.Fill;
            Viewport.DockContent.Show(dockPanel1, DockState.Document);
            Viewport.DockContent.DockHandler.AllowEndUserDocking = false;
            Viewports.Add(Viewport);
            ActiveViewport = Viewport;

            UpdateUndo();
            /*    if (ActiveViewport == null)
                {
                    LayoutViewer Viewport = new LayoutViewer(this,header, Textures);
                    Viewport.DockContent = new DockContent();
                    Viewport.DockContent.Controls.Add(Viewport);
                    Viewport.Dock = DockStyle.Fill;
                    Viewport.DockContent.Show(dockPanel1, DockState.Document);
                    Viewport.DockContent.DockHandler.AllowEndUserDocking = false;
                    Viewports.Add(Viewport);
                    ActiveViewport = Viewport;
                }
                else
                    ActiveViewport.LoadLayout(header);*/

            orthographicViewToolStripMenuItem.Checked = Runtime.LayoutEditor.UseOrthographicView;

            if (!isLoaded)
                InitializeDockPanels();

            LayoutAnimList?.SearchAnimations(header);

            CustomMapper.LoadMK8DCharaSelect(Textures, header);

            isLoaded = true;
        }

        public Dictionary<string, STGenericTexture> GetTextures()
        {
            return Textures;
        }

        public void LoadBxlan(BxlanHeader header)
        {
            AnimationFiles.Add(header);
            ActiveAnimation = header;

            ShowAnimationHierarchy();
            ShowPropertiesPanel();
            LayoutAnimList.LoadAnimation(ActiveAnimation);

            isLoaded = true;
        }

        private void InitializeDockPanels(bool isAnimation = false)
        {
            ShowAnimationHierarchy();
            ShowTextureList();
            ShowPartsEditor();
            ShowPaneHierarchy();
            ShowPropertiesPanel();
            UpdateBackColor();
            ShowAnimationPanel();
            UpdateMenuBar();
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
            if (LayoutPaneEditor != null)
                LayoutPaneEditor.Reset();
        }

        private void ReloadEditors(BxlytHeader activeLayout)
        {
            if (!isLoaded) return;

            Text = $"Switch Toolbox Layout Editor [{activeLayout.FileName}]";

            if (LayoutProperties != null)
                LayoutProperties.Reset();
            if (LayoutHierarchy != null)
                LayoutHierarchy.LoadLayout(activeLayout, ObjectSelected);
            if (LayoutTextureList != null)
                LayoutTextureList.LoadTextures(this, activeLayout, Textures);
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
            Console.WriteLine("UpdateProperties");

            if (LayoutProperties != null)
                LayoutProperties.UpdateProperties();

            if (ActiveViewport != null)
                ActiveViewport.UpdateViewport();
        }

        private bool isSelectedInViewer = false;
        private bool isChecked = false;
        private void OnObjectSelected(object sender, EventArgs e)
        {
            if (isChecked) return;

            if (AnimationPanel != null)
            {
                if (e is ListViewItemSelectionChangedEventArgs)
                {
                    var node = ((ListViewItemSelectionChangedEventArgs)e).Item;
                    if (node.Tag is ArchiveFileInfo)
                    {
                        UpdateAnimationNode(node);
                    }

                    if (node.Tag is BxlanHeader)
                    {
                        AnimationPanel?.Reset();
                        foreach (ListViewItem item in LayoutAnimList.GetSelectedAnimations)
                            if (item.Tag is BxlanHeader)
                                UpdateAnimationPlayer((BxlanHeader)item.Tag);
                    }
                }
            }
            if (LayoutPaneEditor != null && (string)sender == "Select")
            {
                if (e is TreeViewEventArgs) {
                    var node = ((TreeViewEventArgs)e).Node;
                    var mutliSelectedNodes = LayoutHierarchy.GetSelectedPanes();

                    if (Runtime.LayoutEditor.AnimationEditMode)
                        OnSelectedAnimationMode();

                    if (!isSelectedInViewer)
                        ActiveViewport?.SelectedPanes.Clear();

                    if (node.Tag is BasePane)
                        LoadPaneEditorOnSelect(mutliSelectedNodes);
                    else if (node.Tag is BxlytMaterial)
                    {
                        LayoutPaneEditor.Text = $"Properties [{((BxlytMaterial)node.Tag).Name}]";
                        LayoutPaneEditor.LoadMaterial((BxlytMaterial)node.Tag, this);
                    }
                    else
                    {
                        LayoutPaneEditor.Text = $"Properties";
                        LayoutPaneEditor.LoadProperties(node.Tag);
                    }
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

        private void OnSelectedAnimationMode()
        {
            if (AnimationPanel == null) return;


        }

        public void UpdateViewport() {
            ActiveViewport?.UpdateViewport();
        }

        public void LoadPaneEditorOnSelect(BasePane pane) {
            LoadPaneEditorOnSelect(new List<BasePane>() { pane });
        }

        public void LoadPaneEditorOnSelect(List<BasePane> panes)
        {
            var pane = panes.FirstOrDefault();

            if (pane is IPicturePane)
                LayoutPaneEditor.Text = $"Properties [{pane.Name}]    |   (Picture Pane)";
            else if (pane is ITextPane)
                LayoutPaneEditor.Text = $"Properties [{pane.Name}]    |    (Text Box Pane)";
            else if (pane is IWindowPane)
                LayoutPaneEditor.Text = $"Properties [{pane.Name}]    |    (Window Pane)";
            else if (pane is IBoundryPane)
                LayoutPaneEditor.Text = $"Properties [{pane.Name}]    |    (Boundry Pane)";
            else if (pane is IPartPane)
                LayoutPaneEditor.Text = $"Properties [{pane.Name}]    |    (Part Pane)";
            else
                LayoutPaneEditor.Text = $"Properties [{pane.Name}]    |    (Null Pane)";

            LayoutPaneEditor.LoadPane(pane, this);

            if (ActiveViewport == null) return;

            foreach (var pn in panes) {
                if (!ActiveViewport.SelectedPanes.Contains(pn))
                    ActiveViewport.SelectedPanes.Add(pn);
            }

            if (AnimationWindow != null && !AnimationWindow.Disposing && !AnimationWindow.IsDisposed)
                AnimationWindow.LoadPane(pane, this);
        }

        public void RefreshEditors()
        {
            LayoutPaneEditor?.RefreshEditor();
        }

        public void UpdateHiearchyNodeSelection(BasePane pane)
        {
            var nodeWrapper = pane.NodeWrapper;
            if (nodeWrapper == null) return;

            isSelectedInViewer = true;
            LayoutHierarchy?.SelectNode(nodeWrapper);
            isSelectedInViewer = false;
        }

        private void UpdateAnimationNode(ListViewItem node)
        {
            var archiveNode = node.Tag as ArchiveFileInfo;
            var fileFormat = archiveNode.OpenFile();
            if (archiveNode.FileWrapper != null)
                fileFormat.IFileInfo.ArchiveParent = archiveNode.FileWrapper.ArchiveFile;

            archiveNode.FileFormat = fileFormat;

            //Update the tag so this doesn't run again
            node.Tag = "Expanded";

            if (fileFormat != null && fileFormat is BXLAN)
            {
                node.Tag = ((BXLAN)fileFormat).BxlanHeader;
                AnimationFiles.Add(((BXLAN)fileFormat).BxlanHeader);
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

        private void UpdateAnimationPlayer(BxlanHeader animHeader)
        {
            if (AnimationPanel == null) return;

            if (ActiveLayout != null)
            {
                AnimationPanel.AddAnimation(animHeader.ToGenericAnimation(ActiveLayout), false);
                //   foreach (var anim in AnimationFiles)
                //     AnimationPanel.AddAnimation(anim.ToGenericAnimation(ActiveLayout), false);
            }
        }

        private void LayoutEditor_ParentChanged(object sender, EventArgs e)
        {
            if (this.ParentForm == null) return;
        }

        private void textureListToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowTextureList();
        }

        private void ShowPartsEditor()
        {
            if (LayoutPartsEditor != null)
                return;

            LayoutPartsEditor = new LayoutPartsEditor();
            LayoutPartsEditor.Text = "Parts Editor";
            LayoutPartsEditor.Show(dockPanel1, DockState.DockLeft);
        }

        public void ShowBxlanEditor(BxlanHeader bxlan)
        {
            LayoutDocked dock = new LayoutDocked();
            LayoutAnimEditorBasic editor = new LayoutAnimEditorBasic();
            editor.LoadAnim(bxlan, ActiveLayout);
            editor.OnPropertyChanged += AnimPropertyChanged;
            editor.Dock = DockStyle.Fill;
            dock.Controls.Add(editor);

            dock.Show(this);

            /*    if (LayoutAnimEditor != null) {
                    LayoutAnimEditor.LoadFile(bxlan.GetGenericAnimation());
                    return;
                }

                LayoutAnimEditor = new LayoutAnimEditor();
                AnimationPanel.OnNodeSelected = LayoutAnimEditor.OnNodeSelected;
                LayoutAnimEditor.LoadFile(bxlan.GetGenericAnimation());
                if (LayoutHierarchy != null)
                    LayoutAnimEditor.Show(LayoutHierarchy.Pane, DockAlignment.Bottom, 0.5);
                else
                    LayoutAnimEditor.Show(dockPanel1, DockState.DockRight);*/
        }

        private void AnimPropertyChanged(object sender, EventArgs e)
        {
            ActiveViewport?.UpdateViewport();
        }

        private void ShowPropertiesPanel()
        {
            if (LayoutPaneEditor == null)
                LayoutPaneEditor = new PaneEditor();
            else if (LayoutPaneEditor.IsDisposed)
                LayoutPaneEditor = new PaneEditor();

            LayoutPaneEditor.Text = "Properties";
            LayoutPaneEditor.PropertyChanged += OnPanePropertyChanged;

            if (LayoutHierarchy != null && !LayoutHierarchy.IsDisposed)
                LayoutPaneEditor.Show(LayoutHierarchy.Pane, DockAlignment.Bottom, 0.5);
            else
                LayoutPaneEditor.Show(dockPanel1, DockState.DockRight);

            /*     if (LayoutProperties != null)
                     return;

                 LayoutProperties = new LayoutProperties();
                 LayoutProperties.Text = "Properties";
                 if (LayoutHierarchy != null)
                     LayoutProperties.Show(LayoutHierarchy.Pane, DockAlignment.Bottom, 0.5);
                 else
                     LayoutProperties.Show(dockPanel1, DockState.DockRight);*/
        }

        public void ShowPaneEditor(BasePane pane)
        {
            /*     if (LayoutPaneEditor == null)
                     LayoutPaneEditor = new PaneEditor();

                 LayoutPaneEditor.PropertyChanged += OnPanePropertyChanged;
                 LayoutPaneEditor.LoadPane(pane);
                 LayoutPaneEditor.Show();*/
        }

        public void ShowAnimationHierarchy()
        {
            if (LayoutAnimList != null && !LayoutAnimList.IsDisposed)
                return;

            LayoutAnimList = new LayoutAnimList(this, ObjectSelected);
            LayoutAnimList.Text = "Animation Hierarchy";
            LayoutAnimList.Show(dockPanel1, DockState.DockLeft);
        }

        private void ShowPaneHierarchy()
        {
            if (LayoutHierarchy != null && !LayoutHierarchy.IsDisposed)
                return;

            LayoutHierarchy = new LayoutHierarchy(this);
            LayoutHierarchy.Text = "Hierarchy";
            LayoutHierarchy.LoadLayout(ActiveLayout, ObjectSelected);
            LayoutHierarchy.Show(dockPanel1, DockState.DockLeft);
        }

        private void ShowTextureList()
        {
            if (LayoutTextureList != null && !LayoutTextureList.IsDisposed)
                return;

            LayoutTextureList = new LayoutTextureList();
            LayoutTextureList.Text = "Texture List";
            LayoutTextureList.LoadTextures(this, ActiveLayout, Textures);
            LayoutTextureList.Show(dockPanel1, DockState.DockRight);
        }

        public void ShowAnimationPanel()
        {
            DockContent dockContent = new DockContent();
            AnimationPanel = new STAnimationPanel();
            AnimationPanel.Dock = DockStyle.Fill;
            AnimationPanel.AnimationPlaying += OnAnimationPlaying;
            AnimationPanel.SetViewport(ActiveViewport.GetGLControl());
            dockContent.Controls.Add(AnimationPanel);

            if (ActiveViewport != null)
                dockContent.Show(ActiveViewport.Pane, DockAlignment.Bottom, 0.2);
            else
                dockContent.Show(dockPanel1, DockState.DockBottom);
        }

        public void UpdateHiearchyTree()
        {
            LayoutHierarchy?.UpdateTree();
        }

        public void UpdateLayoutTextureList()
        {
            LayoutTextureList?.LoadTextures(this, ActiveLayout, Textures);
        }

        public void DeselectTextureList() {
            LayoutTextureList?.DeselectTextureList();
        }

        private void OnAnimationPlaying(object sender, EventArgs e)
        {
            if (LayoutAnimEditor != null)
                LayoutAnimEditor.OnAnimationPlaying();
            if (Runtime.LayoutEditor.AnimationEditMode && LayoutPaneEditor != null)
                LayoutPaneEditor.ReloadEditor();
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
            else if (!isLoaded)
            {
                ActiveViewport.UpdateBackgroundColor(Runtime.LayoutEditor.BackgroundColor);
                backColorDisplay.BackColor = Runtime.LayoutEditor.BackgroundColor;
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
            var dockContent = dockPanel1.ActiveDocument as DockContent;
            if (dockContent == null) return;

            LayoutViewer viewer = null;
            foreach (var control in dockContent.Controls)
                if (control is LayoutViewer)
                    viewer = control as LayoutViewer;

            if (viewer != null)
            {
                var file = viewer.LayoutFile;
                ActiveLayout = file;
                ReloadEditors(file);
                ActiveViewport = viewer;
                UpdateUndo();
                viewer.UpdateViewport();

                Console.WriteLine("changed " + ActiveLayout.FileName);
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
                LoadBxlyt(((BFLYT)file).header);
            else if (file is BCLYT)
                LoadBxlyt(((BCLYT)file).header);
            else if (file is BRLYT)
                LoadBxlyt(((BRLYT)file).header);
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
                                LoadBxlyt(((BFLYT)layout).header);
                            if (layout is BCLYT)
                                LoadBxlyt(((BCLYT)layout).header);
                            if (layout is BRLYT)
                                LoadBxlyt(((BRLYT)layout).header);
                        }
                    }
                }
                else if (layouts.Count > 0)
                {
                    if (layouts[0] is BFLYT)
                        LoadBxlyt(((BFLYT)layouts[0]).header);
                    if (layouts[0] is BCLYT)
                        LoadBxlyt(((BCLYT)layouts[0]).header);
                    if (layouts[0] is BRLYT)
                        LoadBxlyt(((BRLYT)layouts[0]).header);
                }
            }
            else if (file is BXLAN)
                LoadBxlan(((BXLAN)file).BxlanHeader);
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
                    new Type[] { typeof(BFLYT), typeof(BCLYT), typeof(BRLYT), typeof(SARC) }, file.FileData);

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
                else if (fileFormat is BRLYT)
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
            if (!textConverterToolStripMenuItem.Checked)
            {
                if (ActiveLayout.FileInfo is BFLYT)
                {
                    if (TextConverter == null)
                        TextConverter = new LayoutTextDocked();
                    TextConverter.Text = "Text Converter";
                    TextConverter.TextCompiled += OnTextCompiled;
                    TextConverter.LoadLayout((BFLYT)ActiveLayout.FileInfo);
                    if (ActiveViewport != null)
                        TextConverter.Show(ActiveViewport.Pane, DockAlignment.Bottom, 0.4);
                    else
                        TextConverter.Show(dockPanel1, DockState.DockLeft);
                }

                textConverterToolStripMenuItem.Checked = true;
            }
            else
            {
                textConverterToolStripMenuItem.Checked = false;
                TextConverter?.Hide();
            }
        }

        private void OnTextCompiled(object sender, EventArgs e)
        {
            var layout = TextConverter.GetLayout();
            ActiveLayout = layout.header;
            ReloadEditors(layout.header);

            if (ActiveViewport != null)
                ActiveViewport.ResetLayout(ActiveLayout);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveAnimation != null && ActiveAnimation.FileInfo.CanSave)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = Utils.GetAllFilters(ActiveAnimation.FileInfo);
                sfd.FileName = ActiveAnimation.FileInfo.FileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    STFileSaver.SaveFileFormat(ActiveAnimation.FileInfo, sfd.FileName);
                }
            }

            if (ActiveLayout != null && ActiveLayout.FileInfo.CanSave)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = Utils.GetAllFilters(ActiveLayout.FileInfo);
                sfd.FileName = ActiveLayout.FileInfo.FileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    STFileSaver.SaveFileFormat(ActiveLayout.FileInfo, sfd.FileName);
                }
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (ActiveLayout != null)
                SaveActiveFile(ActiveLayout.FileInfo);
        }

        private void saveAnimationToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (var anim in AnimationFiles)
                SaveActiveFile(anim.FileInfo);
        }

        private void SaveActiveFile(IFileFormat fileFormat, bool ForceDialog = false)
        {
            if (fileFormat.CanSave)
            {
                if (fileFormat.IFileInfo != null &&
                    fileFormat.IFileInfo.ArchiveParent != null && !ForceDialog)
                {
                    UpdateArchiveFile?.Invoke(fileFormat, new EventArgs());
                    MessageBox.Show($"Saved {fileFormat.FileName} to archive!");
                }
                else
                {
                    STFileSaver.SaveFileFormat(fileFormat, fileFormat.FilePath);
                }
            }
        }

        private void LayoutEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt && e.KeyCode == Keys.S) // Ctrl + Alt + S Save As
            {
                e.SuppressKeyPress = true;
                if (ActiveLayout != null)
                    SaveActiveFile(ActiveLayout.FileInfo, true);
                if (ActiveAnimation != null)
                    SaveActiveFile(ActiveAnimation.FileInfo, true);
            }
            else if (e.Control && e.KeyCode == Keys.S) // Ctrl + S Save
            {
                e.SuppressKeyPress = true;
                if (ActiveLayout != null)
                    SaveActiveFile(ActiveLayout.FileInfo, false);
                if (ActiveAnimation != null)
                    SaveActiveFile(ActiveAnimation.FileInfo, false);
            }
        }

        private void debugShading_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (debugShading.SelectedIndex < 0) return;

            Runtime.LayoutEditor.Shading = (Runtime.LayoutEditor.DebugShading)debugShading.SelectedItem;
            if (ActiveViewport != null)
                ActiveViewport.UpdateViewport();
        }

        private void orthographicViewToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleOrthMode();
        }

        private void toolstripOrthoBtn_Click(object sender, EventArgs e) {
            if (orthographicViewToolStripMenuItem.Checked)
                orthographicViewToolStripMenuItem.Checked = false;
            else
                orthographicViewToolStripMenuItem.Checked = true;

            ToggleOrthMode();
        }

        private void ToggleOrthMode()
        {
            if (ActiveViewport != null)
            {
                if (!orthographicViewToolStripMenuItem.Checked)
                    toolstripOrthoBtn.Image = BitmapExtension.GrayScale(FirstPlugin.Properties.Resources.OrthoView);
                else
                    toolstripOrthoBtn.Image = FirstPlugin.Properties.Resources.OrthoView;

                Runtime.LayoutEditor.UseOrthographicView = orthographicViewToolStripMenuItem.Checked;
                ActiveViewport.ResetCamera();
                ActiveViewport.UpdateViewport();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            if (ActiveLayout != null)
                SaveActiveFile(ActiveLayout.FileInfo, false);
            if (ActiveAnimation != null)
                SaveActiveFile(ActiveAnimation.FileInfo, false);
        }

        private void displayPanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Runtime.LayoutEditor.DisplayNullPane = displayNullPanesToolStripMenuItem.Checked;
            Runtime.LayoutEditor.DisplayBoundryPane = displayyBoundryPanesToolStripMenuItem.Checked;
            Runtime.LayoutEditor.DisplayPicturePane = displayPicturePanesToolStripMenuItem.Checked;
            Runtime.LayoutEditor.DisplayWindowPane = displayWindowPanesToolStripMenuItem.Checked;
            Runtime.LayoutEditor.DisplayTextPane = displayTextPanesToolStripMenuItem.Checked;

            ActiveViewport?.UpdateViewport();
        }

        private void renderInGamePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Runtime.LayoutEditor.IsGamePreview = renderInGamePreviewToolStripMenuItem.Checked;
            ActiveViewport?.UpdateViewport();
        }

        private void displayGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Runtime.LayoutEditor.DisplayGrid = displayGridToolStripMenuItem.Checked;
            ActiveViewport?.UpdateViewport();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnimationPanel?.Reset();

            if (ActiveLayout == null) return;

            foreach (var pane in ActiveLayout.PaneLookup.Values)
                pane.animController.ResetAnim();

            foreach (var mat in ActiveLayout.Materials)
                mat.animController.ResetAnim();

            ActiveViewport?.UpdateViewport();
        }

        private void showGameWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GamePreviewWindow == null || GamePreviewWindow.Disposing || GamePreviewWindow.IsDisposed)
            {
                GamePreviewWindow = new LayoutViewer(this, ActiveLayout, Textures);
                GamePreviewWindow.GameWindow = true;
                GamePreviewWindow.Dock = DockStyle.Fill;
                STForm form = new STForm();
                form.Text = "Game Preview";
                form.AddControl(GamePreviewWindow);
                form.Show(this);
            }
        }

        private void LayoutEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnimationPanel?.OnControlClosing();
            GamePreviewWindow?.OnControlClosing();
            GamePreviewWindow?.Dispose();
            LayoutPaneEditor?.Close();
            LayoutAnimEditor?.Close();
            LayoutHierarchy?.Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            ActiveViewport?.UndoManger.Undo();
            ActiveViewport?.UpdateViewport();

            UpdateUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
            ActiveViewport?.UndoManger.Redo();
            ActiveViewport?.UpdateViewport();

            UpdateUndo();
        }

        public void UpdateUndo()
        {
            if (ActiveViewport == null)
                return;

            RefreshEditors();

            redoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = false;

            if (ActiveViewport.UndoManger.HasUndo)
                undoToolStripMenuItem.Enabled = true;
            if (ActiveViewport.UndoManger.HasRedo)
                redoToolStripMenuItem.Enabled = true;
        }

        private void OnPanePropertyChanged(object sender, EventArgs e)
        {
            ActiveViewport?.UpdateViewport();
        }

        private void viewPartsAsNullPanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Runtime.LayoutEditor.PartsAsNullPanes = viewPartsAsNullPanesToolStripMenuItem.Checked;
            ActiveViewport?.UpdateViewport();
        }

        #region Pane Adding

        public BasePane AddNewTextPane()
        {
            NewTextboxDialog newTextDlg = new NewTextboxDialog();
            newTextDlg.LoadFonts(ActiveLayout.Fonts);
            if (newTextDlg.ShowDialog() == DialogResult.OK)
            {
                string font = newTextDlg.GetFont();
                int fontIndex = ActiveLayout.AddFont(font);
                string text = newTextDlg.GetText();

                BasePane pane = ActiveLayout.CreateNewTextPane(RenamePane("T_text"));
                if (pane != null)
                {
                    ((ITextPane)pane).FontIndex = (ushort)fontIndex;
                    ((ITextPane)pane).FontName = font;
                    ((ITextPane)pane).Text = text;

                    pane.NodeWrapper = LayoutHierarchy.CreatePaneWrapper(pane);
                    ((ITextPane)pane).MaterialIndex = (ushort)ActiveLayout.AddMaterial(((ITextPane)pane).Material);
                    ActiveLayout.AddPane(pane, ActiveLayout.RootPane);
                }

                return pane;
            }
            else return null;
        }

        public BasePane AddNewBoundryPane()
        {
            BasePane pane = ActiveLayout.CreateNewBoundryPane(RenamePane("B_bound"));
            if (pane != null)
            {
                pane.NodeWrapper = LayoutHierarchy.CreatePaneWrapper(pane);
                ActiveLayout.AddPane(pane, ActiveLayout.RootPane);
            }

            return pane;
        }

        public BasePane AddNewWindowPane()
        {
            BasePane pane = ActiveLayout.CreateNewWindowPane(RenamePane("W_window"));
            if (pane != null)
            {
                pane.NodeWrapper = LayoutHierarchy.CreatePaneWrapper(pane);
                ActiveLayout.AddPane(pane, ActiveLayout.RootPane);
            }

            return pane;
        }

        public BasePane AddNewPicturePane()
        {
            BasePane pane = ActiveLayout.CreateNewPicturePane(RenamePane("P_pict"));
            if (pane != null)
            {
                pane.NodeWrapper = LayoutHierarchy.CreatePaneWrapper(pane);
                ((IPicturePane)pane).MaterialIndex = (ushort)ActiveLayout.AddMaterial(((IPicturePane)pane).Material);
                ActiveLayout.AddPane(pane, ActiveLayout.RootPane);
            }

            return pane;
        }

        public BasePane AddNewPartPane()
        {
            BasePane pane = ActiveLayout.CreateNewPartPane(RenamePane("N_part")) as BasePane;
            if (pane != null)
            {
                pane.NodeWrapper = LayoutHierarchy.CreatePaneWrapper(pane);
                ActiveLayout.AddPane(pane, ActiveLayout.RootPane);
            }

            return pane;
        }

        public BasePane AddNewNullPane()
        {
            BasePane pane = ActiveLayout.CreateNewNullPane(RenamePane("N_null"));
            if (pane != null)
            {
                pane.NodeWrapper = LayoutHierarchy.CreatePaneWrapper(pane);
                ActiveLayout.AddPane(pane, ActiveLayout.RootPane);
            }

            return pane;
        }

        public void AddNewPastedPane(BasePane pane)
        {
            string name = pane.Name;
            string numberedEnd = pane.Name.Split('_').LastOrDefault().Replace("_", string.Empty);
            if (numberedEnd.All(char.IsDigit)) {
                if (name.Contains($"_{numberedEnd}"))
                    name = name.Replace($"_{numberedEnd}", string.Empty);
                else
                    name = name.Replace(numberedEnd, string.Empty);
            }

            name = RenamePane(name);
            if (ActiveLayout.PaneLookup.ContainsKey(name))
                throw new Exception("Pane name duplicate! " + name);


            pane.Name = name;
            pane.LayoutFile = ActiveLayout;

            if (pane is IPicturePane)
            {
                ((IPicturePane)pane).Material.Name = pane.Name;
                ((IPicturePane)pane).MaterialIndex = (ushort)ActiveLayout.AddMaterial(((IPicturePane)pane).Material);
            }
            if (pane is ITextPane)
            {
                ((ITextPane)pane).Material.Name = pane.Name;
                ((ITextPane)pane).MaterialIndex = (ushort)ActiveLayout.AddMaterial(((ITextPane)pane).Material);
            }
            if (pane is IWindowPane)
            {
            }

            pane.NodeWrapper = LayoutHierarchy.CreatePaneWrapper(pane);
            ActiveLayout.AddPane(pane, pane.Parent);
        }

        private string RenamePane(string name)
        {
            List<string> names = ActiveLayout.PaneLookup.Keys.ToList();
            return Utils.RenameDuplicateString(names, name, 0, 2);
        }

        #endregion

        private void transformChildrenToolStripMenuItem_Click(object sender, EventArgs e) {
            Runtime.LayoutEditor.TransformChidlren = transformChildrenToolStripMenuItem.Checked;
        }

        private void editorModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.LayoutEditor.AnimationEditMode = editorModeCB.SelectedIndex == 1;
            if (Runtime.LayoutEditor.AnimationEditMode)
            {
                chkAutoKey.Enabled = true;
            }
            else
                chkAutoKey.Enabled = false;

            if (LayoutPaneEditor != null)
                LayoutPaneEditor.ReloadEditor();
        }

        private void showAnimationWindowToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenAnimationEditor();
        }

        private void OpenAnimationEditor() {
            if (AnimationWindow == null || AnimationWindow.Disposing || AnimationWindow.IsDisposed) {
                AnimationWindow = new LytAnimationWindow();
                AnimationWindow.OnPropertyChanged += AnimPropertyChanged;
                if (SelectedPanes.Count > 0)
                    AnimationWindow.LoadPane(SelectedPanes[0], this);
                AnimationWindow.Show(this);
            }
        }

        private void showDockedPanel_Click(object sender, EventArgs e)
        {
            if (ActiveViewport == null || ActiveLayout == null)
                return;

            var menu = sender as ToolStripMenuItem;
            if (menu == null) return;

            Console.WriteLine($"menu {menu.Checked}");

            if (menu.Checked)
            {
                if (menu == showTimelineToolStripMenuItem) {
                    ShowAnimationPanel();
                    LayoutAnimList.LoadAnimation(ActiveAnimation);
                }
                if (menu == showPropertiesToolStripMenuItem)
                    ShowPropertiesPanel();
                if (menu == showPanelHiearchyToolStripMenuItem)
                    ShowPaneHierarchy();
                if (menu == showTextureListToolStripMenuItem)
                    ShowTextureList();
                if (menu == showAnimationListToolStripMenuItem)
                    ShowAnimationHierarchy();
            }
            else
            {
                if (menu == showTimelineToolStripMenuItem)
                    CloseAnimationPanel();
                if (menu == showPropertiesToolStripMenuItem)
                    LayoutProperties?.Close();
                if (menu == showPanelHiearchyToolStripMenuItem)
                    LayoutHierarchy?.Close();
                if (menu == showTextureListToolStripMenuItem)
                    LayoutTextureList?.Close();
                if (menu == showAnimationListToolStripMenuItem)
                    LayoutAnimList?.Close();
            }
        }

        private void UpdateMenuBar() {
            showTimelineToolStripMenuItem.Checked = ControlIsActive(AnimationPanel);
            showPropertiesToolStripMenuItem.Checked = ControlIsActive(LayoutProperties);
            showPanelHiearchyToolStripMenuItem.Checked = ControlIsActive(LayoutHierarchy);
            showTextureListToolStripMenuItem.Checked = ControlIsActive(LayoutTextureList);
            showAnimationListToolStripMenuItem.Checked = ControlIsActive(LayoutAnimList);
        }

        private bool ControlIsActive(Control control) {
            if (control == null) return false;
            return !control.IsDisposed && !control.Disposing;
        }

        private void CloseAnimationPanel()
        {
            if (AnimationPanel == null) return;

            var dock = AnimationPanel.Parent as DockContent;
            dock.Close();
        }

        private void dockPanel1_ContentRemoved(object sender, DockContentEventArgs e) {
            UpdateMenuBar();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
