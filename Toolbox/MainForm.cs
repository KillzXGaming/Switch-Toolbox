using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using OpenTK.Graphics.OpenGL;

namespace Toolbox
{
    public partial class MainForm : Form, IMdiContainer
    {
        private static MainForm _instance;
        public static MainForm Instance { get { return _instance == null ? _instance = new MainForm() : _instance; } }

        IFileFormat[] SupportedFormats;
        IFileMenuExtension[] FileMenuExtensions;

        public void AddChildContainer(Form form)
        {
            TabDupeIndex = 0;
            form.Text = CheckTabDupes(form.Text);
            form.MdiParent = this;
            form.Show();

            IFileFormat activeFile;

            if (form is ObjectEditor)
                activeFile = ((ObjectEditor)form).GetActiveFile();
            else
                activeFile = GetActiveIFileFormat();

            if (activeFile != null)
                SetFormatSettings(activeFile);
        }

        public MainForm()
        {
            FormThemes.ActivePreset = FormThemes.Preset.Dark;

            Runtime.MainForm = this;
            LoadConfig();

            InitializeComponent();
            compressionToolStripMenuItem.DropDownItems.AddRange(CompressionMenus.GetMenuItems().ToArray());

            //Redo setting this since designer keeps resetting this
            tabForms.myBackColor = FormThemes.BaseTheme.FormBackColor;

            OnMdiWindowClosed();
            ResetMenus();
        }

        public void Reload()
        {
            SupportedFormats = FileManager.GetFileFormats();
            FileMenuExtensions = FileManager.GetMenuExtensions();
        }

        //Use for files opened with program
        public List<string> openedFiles = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            VersionCheck version = new VersionCheck();
            Runtime.ProgramVersion = version.ProgramVersion;
            Runtime.CommitInfo = version.CommitInfo;
            Runtime.CompileDate = version.CompileDate;

            ThreadStart t = new ThreadStart(UpdateProgram.CheckLatest);
            Thread thread = new Thread(t);
            thread.Start();

            Application.Idle += Application_Idle;

            if (Runtime.UseOpenGL)
            {
                //Create an instance of this to help load open gl data easier and quicker after boot
                var viewport = new Viewport(false);

                if (OpenTK.Graphics.GraphicsContext.CurrentContext != null)
                {
                    Runtime.OpenTKInitialized = true;

                    Runtime.renderer = GL.GetString(StringName.Renderer);
                    Runtime.openGLVersion = GL.GetString(StringName.Version);
                    Runtime.GLSLVersion = GL.GetString(StringName.ShadingLanguageVersion);
                    ParseGLVersion();
                }
            }

            LoadPLugins();
            UpdateToolbar();
            Reload();
            LoadMDITheme();
            LoadRecentList();
            LoadPluginFileContextMenus();

            foreach (string file in openedFiles)
            {
                if (File.Exists(file))
                    OpenFile(file);
            }

            openedFiles.Clear();

            if (Runtime.UseDebugDomainExceptionHandler)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            }
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("MyHandler caught : " + e.Message);
            MessageBox.Show("Runtime terminating: {0}", args.IsTerminating.ToString());

           
        }

        private void ParseGLVersion()
        {
            char[] Version = Runtime.openGLVersion.ToCharArray();

            char Major = Version[0];
            char Minor = Version[2];
            char Minor2 = Version[4];

            int major;

            if (int.TryParse(Version[0].ToString(), out major))
            {
                if (major <= 2)
                {
                    Runtime.UseLegacyGL = true;
                }
            }
        }

        #region Updater
        bool UsePrompt = true;
        private void Application_Idle(object sender, EventArgs e)
        {
            if (UpdateProgram.CanUpdate && Runtime.EnableVersionCheck && UsePrompt)
            {
                updateToolstrip.Enabled = true;
            }
        }

        private void UpdateNotifcationClick()
        {
            //Prompt once for the user to update the tool. 
            DialogResult result;
            using (DialogCenteringService centeringService = new DialogCenteringService(this)) // center message box
            {
                result = MessageBox.Show($"A new update is available {UpdateProgram.LatestRelease.TagName} \n\n{UpdateProgram.LatestRelease.Body}!" +
               $" Would you like to install it?", "Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            }
            if (result == DialogResult.Yes)
            {
                UpdateApplication();
            }
        }

        private void UpdateApplication()
        {
            //Start updating while program is closed
            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(Runtime.ExecutableDir, "Updater.exe");
            proc.StartInfo.WorkingDirectory = Runtime.ExecutableDir;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.Arguments = "-d -i -b";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            proc.Start();
            Application.Exit();
        }
        #endregion

        #region OpenFile

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Utils.GetAllFilters(SupportedFormats);
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (string file in ofd.FileNames)
                    OpenFile(file);

                Cursor.Current = Cursors.Default;
            }
        }

        private void openActiveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Utils.GetAllFilters(SupportedFormats);
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (string file in ofd.FileNames)
                    OpenFile(file, true);

                Cursor.Current = Cursors.Default;
            }
        }

        public void OpenFile(string FileName, bool InActiveEditor = false)
        {
            Reload();

            if (File.Exists(FileName))
                SaveRecentFile(FileName);


            object file = STFileLoader.OpenFileFormat(FileName);


            if (file == null) //File might not be supported so return
                return;

            Type objectType = file.GetType();

            bool HasEditorActive = false;
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    MethodInfo method = objectType.GetMethod("OpenForm");
                    var form = (STForm)method.Invoke(file, new object[0]);
                    TabDupeIndex = 0;
                    form.Text = CheckTabDupes(((IFileFormat)file).FileName);
                    form.MdiParent = this;
                    form.Show();

                    HasEditorActive = true;
                }
            }

            bool IsTreeNode = file is TreeNode;


            if (!IsTreeNode || HasEditorActive)
            {
                SetFormatSettings(GetActiveIFileFormat());
                return;
            }


            var node = (TreeNode)file;

            //ObjectEditor is for treenode types. Editors will be on the right side, treenodes on the left
            SetFormatSettings((IFileFormat)node);

            //Check for active object editors
            Form editor = (Form)LibraryGUI.Instance.GetActiveForm();

            bool useActiveEditor = false;

            if (editor != null && editor is ObjectEditor)
            {
                //If any are active and we want it to be a new tab then create an instance of one
                if (InActiveEditor || ((ObjectEditor)editor).AddFilesToActiveEditor)
                {
                    useActiveEditor = true;
                }
            }

            bool IsEditorActive = editor != null;

            //Create one if none are active
            if (!IsEditorActive)
            {
                editor = new ObjectEditor();
            }

            if (!useActiveEditor || !IsEditorActive)
            {
                editor = new ObjectEditor();
                AddObjectEditorFile(node, (ObjectEditor)editor, true);

                editor.Text = CheckTabDupes(node.Text);
                editor.Show();
            }
            else
            {
                AddObjectEditorFile(node, (ObjectEditor)editor, false);
            }

            SetFormatSettings(GetActiveIFileFormat());
        }

        private void AddObjectEditorFile(TreeNode file, ObjectEditor editor, bool ClearFiles)
        {
            TabDupeIndex = 0;
            editor.MdiParent = this;

            // Invoke the treeview to add the nodes
            editor.treeViewCustom1.Invoke((Action)delegate ()
            {
                editor.treeViewCustom1.BeginUpdate(); // No visual updates until we say 
                if (ClearFiles)
                    editor.treeViewCustom1.Nodes.Clear(); // Remove existing nodes
                editor.treeViewCustom1.Nodes.Add(file); // Add the new nodes
                editor.treeViewCustom1.EndUpdate(); // Allow the treeview to update visually
            });

            if (file is TreeNodeFile)
            {
                ((TreeNodeFile)file).OnAfterAdded();
            }
        }

        #endregion

        #region SaveFile

        public void SaveActiveFile(bool UseSaveDialog = true, bool UseCompressDialog = true)
        {
            if (ActiveMdiChild != null)
            {
                if (ActiveMdiChild is ObjectEditor)
                    SaveNodeFormats((ObjectEditor)ActiveMdiChild, UseSaveDialog, UseCompressDialog);
                else
                {
                    var format = GetActiveIFileFormat();

                    if (!format.CanSave)
                        return;

                    string FileName = format.FilePath;
                    if (!File.Exists(FileName))
                        UseSaveDialog = true;

                    if (UseSaveDialog)
                    {
                        if (format is VGAdudioFile)
                        {
                            SaveFileDialog sfd = new SaveFileDialog();

                            List<IFileFormat> formats = new List<IFileFormat>();
                            foreach (VGAdudioFile fileFormat in FileManager.GetVGAudioFileFormats())
                            {
                                formats.Add((IFileFormat)fileFormat);
                            }
                            sfd.Filter = Utils.GetAllFilters(formats);

                            if (sfd.ShowDialog() != DialogResult.OK)
                                return;

                            foreach (var fileFormat in formats)
                            {
                                foreach (var ext in format.Extension)
                                {
                                    if (Utils.HasExtension(sfd.FileName, ext))
                                    {
                                        ((VGAdudioFile)format).Format = fileFormat;
                                    }
                                }
                            }

                            FileName = sfd.FileName;
                        }
                        else
                        {
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Filter = Utils.GetAllFilters(format);
                            sfd.FileName = format.FileName;

                            if (sfd.ShowDialog() != DialogResult.OK)
                                return;

                            FileName = sfd.FileName;
                        }
                    }
                    Cursor.Current = Cursors.WaitCursor;

                    STFileSaver.SaveFileFormat(format, FileName, UseCompressDialog);

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void SaveNodeFormats(ObjectEditor editor, bool UseSaveDialog, bool UseCompressDialog)
        {
            foreach (var node in editor.treeViewCustom1.Nodes)
            {
                if (node is IFileFormat)
                {
                    var format = ((IFileFormat)node);

                    if (!format.CanSave)
                        return;

                    string FileName = format.FilePath;
                    if (!File.Exists(FileName))
                        UseSaveDialog = true;

                    if (UseSaveDialog)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = Utils.GetAllFilters(format);
                        sfd.FileName = format.FileName;

                        if (sfd.ShowDialog() != DialogResult.OK)
                            return;

                        FileName = sfd.FileName;
                    }
                    Cursor.Current = Cursors.WaitCursor;

                    STFileSaver.SaveFileFormat(((IFileFormat)node), FileName, UseCompressDialog);

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        #endregion

        #region Recent Files

        List<string> RecentFiles = new List<string>();

        int TabDupeIndex = 0;
        private string CheckTabDupes(string Name)
        {
            foreach (TabPage tab in tabForms.TabPages)
            {
                if (tab.Text == Name)
                {
                    Name = Name + TabDupeIndex++;
                    return CheckTabDupes(Name);
                }
            }
            return Name;
        }

        const int MRUnumber = 6;
        private void SaveRecentFile(string path)
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            LoadRecentList(); //load list from file
            if (!(RecentFiles.Contains(path))) //prevent duplication on recent list
                RecentFiles.Insert(0, path); //insert given path into list

            //keep list number not exceeded the given value
            while (RecentFiles.Count > MRUnumber)
            {
                RecentFiles.RemoveAt(MRUnumber);
            }
            foreach (string item in RecentFiles)
            {
                //create new menu for each item in list
                STToolStripItem fileRecent = new STToolStripItem();
                fileRecent.Click += RecentFile_click;
                fileRecent.Text = item;
                fileRecent.Size = new System.Drawing.Size(170, 40);
                fileRecent.AutoSize = true;
                fileRecent.Image = null;

                //add the menu to "recent" menu
                recentToolStripMenuItem.DropDownItems.Add(fileRecent);
            }
            //writing menu list to file
            //create file called "Recent.txt" located on app folder
            StreamWriter stringToWrite =
            new StreamWriter(Runtime.ExecutableDir + "\\Recent.txt");
            foreach (string item in RecentFiles)
            {
                stringToWrite.WriteLine(item); //write list to stream
            }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }
        private void LoadRecentList()
        {//try to load file. If file isn't found, do nothing
            RecentFiles.Clear();

            if (File.Exists(Runtime.ExecutableDir + "\\Recent.txt"))
            {
                StreamReader listToRead = new StreamReader(Runtime.ExecutableDir + "\\Recent.txt"); //read file stream
                string line;
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                {
                    if (File.Exists(line))
                        RecentFiles.Add(line); //insert to list
                }
                listToRead.Close(); //close the stream
            }
            foreach (string item in RecentFiles)
            {
                STToolStripItem fileRecent = new STToolStripItem();
                fileRecent.Click += RecentFile_click;
                fileRecent.Text = item;
                fileRecent.Size = new System.Drawing.Size(170, 40);
                fileRecent.AutoSize = true;
                fileRecent.Image = null;
                recentToolStripMenuItem.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            }
        }

        private void RecentFile_click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            OpenFile(sender.ToString());
            Cursor.Current = Cursors.Default;
        }

        #endregion

        #region Form Settings and plugin menus

        private void LoadConfig()
        {
            try
            {
                Config.StartupFromFile(Runtime.ExecutableDir + "\\config.xml");
                Config.GamePathsFromFile(Runtime.ExecutableDir + "\\config_paths.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load config file! {ex}");
            }
        }

        private void UpdateToolbar()
        {
            string commit = $"Commit: {Runtime.CommitInfo}";

            Text = $"Switch Toolbox | Version: {Runtime.ProgramVersion} | {commit} | Compile Date: {Runtime.CompileDate}";
        }

        private void LoadPLugins()
        {
            GenericPluginLoader.LoadPlugin();
            foreach (var plugin in GenericPluginLoader._Plugins)
            {
                plugin.Value.Load();
                LoadPluginContextMenus(plugin.Value.Types);
            }
        }

        private List<IMenuExtension> menuExtentions = new List<IMenuExtension>();
        private void LoadPluginContextMenus(Type[] types)
        {
            foreach (Type T in types)
            {
                Type[] interfaces_array = T.GetInterfaces();
                for (int i = 0; i < interfaces_array.Length; i++)
                {
                    if (interfaces_array[i] == typeof(IMenuExtension))
                    {
                        menuExtentions.Add((IMenuExtension)Activator.CreateInstance(T));
                    }
                }
            }
            foreach (IMenuExtension ext in menuExtentions)
            {
                if (ext.FileMenuExtensions != null)
                    RegisterMenuExtIndex(fileToolStripMenuItem, ext.FileMenuExtensions, fileToolStripMenuItem.DropDownItems.Count); //last items are separator and settings
                if (ext.ToolsMenuExtensions != null)
                    RegisterMenuExtIndex(toolsToolStripMenuItem, ext.ToolsMenuExtensions);
                if (ext.TitleBarExtensions != null)
                    RegisterMenuExtIndex(menuStrip1, ext.TitleBarExtensions, menuStrip1.Items.Count);
            }
        }

        private void LoadPluginFileContextMenus()
        {
            foreach (IFileMenuExtension ext in FileMenuExtensions)
            {
                if (ext.NewFileMenuExtensions != null)
                    RegisterMenuExtIndex(newToolStripMenuItem, ext.NewFileMenuExtensions, newToolStripMenuItem.DropDownItems.Count);
                if (ext.NewFromFileMenuExtensions != null)
                    RegisterMenuExtIndex(newFromFileToolStripMenuItem, ext.NewFromFileMenuExtensions, newFromFileToolStripMenuItem.DropDownItems.Count);
                if (ext.ToolsMenuExtensions != null)
                    RegisterMenuExtIndex(toolsToolStripMenuItem, ext.ToolsMenuExtensions);
                if (ext.TitleBarExtensions != null)
                    RegisterMenuExtIndex(menuStrip1, ext.TitleBarExtensions, menuStrip1.Items.Count);
                if (ext.ExperimentalMenuExtensions != null)
                    RegisterMenuExtIndex(experimentalToolStripMenuItem, ext.ExperimentalMenuExtensions, experimentalToolStripMenuItem.DropDownItems.Count);
                if (ext.CompressionMenuExtensions != null)
                    RegisterMenuExtIndex(compressionToolStripMenuItem, ext.CompressionMenuExtensions, compressionToolStripMenuItem.DropDownItems.Count);
            }
        }
        void RegisterMenuExtIndex(ToolStrip target, ToolStripButton[] list, int index = 0)
        {
            foreach (var i in list)
                target.Items.Insert(index++, i);
        }
        void RegisterMenuExtIndex(ToolStripMenuItem target, STToolStripItem[] list, int index = 0)
        {
            foreach (var i in list)
                target.DropDownItems.Insert(index++, i);
        }
        void RegisterMenuExtIndex(ToolStrip target, STToolStripItem[] list, int index = 0)
        {
            foreach (var i in list)
                target.Items.Insert(index++, i);
        }

        private void LoadMDITheme()
        {
            MDIController.MDIClientSupport.SetBevel(this, false);
            MdiClient ctlMDI;

            // Loop through all of the form's controls looking
            // for the control of type MdiClient.
            foreach (Control ctl in this.Controls)
            {
                try
                {
                    // Attempt to cast the control to type MdiClient.
                    ctlMDI = (MdiClient)ctl;

                    // Set the BackColor of the MdiClient control.
                    ctlMDI.BackColor = FormThemes.BaseTheme.MDIParentBackColor;
                }
                catch (InvalidCastException exc)
                {
                    // Catch and ignore the error if casting failed.
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        private IFileFormat GetActiveIFileFormat()
        {
            if (ActiveMdiChild is ObjectEditor)
            {
                return ((ObjectEditor)ActiveMdiChild).GetActiveFile();
            }
            if (ActiveMdiChild is IFIleEditor)
            {
                if (((IFIleEditor)ActiveMdiChild).GetFileFormats().Count > 0)
                    return ((IFIleEditor)ActiveMdiChild).GetFileFormats()[0];
            }
            else if (ActiveMdiChild is ImageEditorForm)
            {
                return ((ImageEditorForm)ActiveMdiChild).GetActiveFile();
            }
            else if (ActiveMdiChild is AudioPlayer)
            {
                if (((AudioPlayer)ActiveMdiChild).AudioFileFormats.Count > 0)
                    return ((AudioPlayer)ActiveMdiChild).AudioFileFormats[0];
            }

            return null;
        }
        private void SetFormatSettings(IFileFormat format)
        {
            ResetMenus();
            if (format == null)
                return;

            if (format.CanSave)
            {
                saveAsToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                saveToolStripButton.Enabled = true;
            }
            else
            {
                saveAsToolStripMenuItem.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                saveToolStripButton.Enabled = false;
            }

            var menuExtensions = FileManager.GetMenuExtensions(format);

            editToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < stToolStrip1.Items.Count; i++)
            {
                if (i > 1)
                    stToolStrip1.Items.RemoveAt(i);
            }

            if (menuExtensions != null)
            {
                if (menuExtensions.EditMenuExtensions != null)
                {
                    RegisterMenuExtIndex(editToolStripMenuItem, menuExtensions.EditMenuExtensions, editToolStripMenuItem.DropDownItems.Count);

                    if (editToolStripMenuItem.DropDownItems.Count > 0)
                        editToolStripMenuItem.Enabled = true;
                }
                if (menuExtensions.IconButtonMenuExtensions != null)
                {
                    RegisterMenuExtIndex(stToolStrip1, menuExtensions.IconButtonMenuExtensions, stToolStrip1.Items.Count);
                }
            }
        }
        private void ResetMenus()
        {
            editToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = false;
        }


        #region Events
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S) // Ctrl + S Save
            {
                e.SuppressKeyPress = true;

                SaveActiveFile(false);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveActiveFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveActiveFile(false);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e) {
            SaveActiveFile(false);
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);

            foreach (STForm frm in this.MdiChildren) frm.MDIWindowed();
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.SuspendLayout();

            foreach (Form frm in this.MdiChildren) frm.FormBorderStyle = FormBorderStyle.FixedSingle;

            LayoutMdi(MdiLayout.TileHorizontal);

            foreach (Form frm in this.MdiChildren) frm.FormBorderStyle = FormBorderStyle.None;

            this.ResumeLayout();
        }

        private void tileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void arrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();

            OnMdiWindowClosed();

            //Force garbage collection.
            GC.Collect();

            // Wait for all finalizers to complete before continuing.
            GC.WaitForPendingFinalizers();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiChildren) frm.Close();

            OnMdiWindowClosed();

            RenderTools.DisposeTextures();

            //Force garbage collection.
            GC.Collect();

            // Wait for all finalizers to complete before continuing.
            GC.WaitForPendingFinalizers();
        }

        private void OnMdiWindowClosed()
        {
            BtnMdiClose.Visible = false;
            BtnMdiMinMax.Visible = false;
            BtnMdiMinimize.Visible = false;
        }

        private void OnMdiWindowActived()
        {
            BtnMdiClose.Visible = true;
            BtnMdiMinMax.Visible = true;
            BtnMdiMinimize.Visible = true;
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.WindowState = FormWindowState.Minimized;
        }

        private void maximizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.WindowState = FormWindowState.Maximized;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
            {
                String[] strGetFormats = e.Data.GetFormats();
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
            {
                OpenFile(filename);
            }

            Cursor.Current = Cursors.Default;
        }
        #endregion

        #region TabMdiWindows

        private void TabControlMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tabForms.SelectedTab != null)
            {
                tabControlContextMenuStrip.Show(Cursor.Position);
            }
        }

        private void CloseTab(object sender, EventArgs e)
        {
            foreach (var child in this.MdiChildren)
            {
                if (child == tabForms.SelectedTab.Tag)
                {
                    OnMdiWindowClosed();
                    child.Close();
                    return;
                }
            }
        }

        private void ResetAnimPanel()
        {
            if (LibraryGUI.Instance.GetAnimationPanel() != null)
            {
                LibraryGUI.Instance.GetAnimationPanel().CurrentAnimation = null;
            }
        }

        bool IsChanged = false;
        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
            {
                tabForms.Visible = false;
                ResetMenus();
            }
            // If no any child form, hide tabControl 
            else
            {
                if (IsChanged)
                {
                    ResetAnimPanel();
                    IsChanged = false;
                }

                // If child form is new and no has tabPage, 
                // create new tabPage 
                if (this.ActiveMdiChild.Tag == null)
                {
                    if (Runtime.MaximizeMdiWindow && ActiveMdiChild.WindowState != FormWindowState.Maximized)
                        ((STForm)this.ActiveMdiChild).Maximize();

                    int tpIndex = 0;
                    foreach (TabPage tpCheck in tabForms.TabPages)
                    {
                        if (tpCheck.Text == this.ActiveMdiChild.Text)
                        {
                            tabForms.SelectedIndex = tpIndex;
                            return;
                        }
                        tpIndex++;
                    }

                    // Add a tabPage to tabControl with child 
                    // form caption 
                    TabPage tp = new TabPage(this.ActiveMdiChild.Text);
                    tp.BackColor = FormThemes.BaseTheme.TabPageInactive;
                    tp.ForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
                    tp.Tag = this.ActiveMdiChild;
                    tp.Parent = tabForms;

                    tabForms.SelectedTab = tp;
                    tabForms.SelectedTab.BackColor = FormThemes.BaseTheme.TabPageActive;

                    this.ActiveMdiChild.Tag = tp;
                    this.ActiveMdiChild.FormClosed +=
                        new FormClosedEventHandler(ActiveMdiChild_FormClosed);
                    this.ActiveMdiChild.SizeChanged +=
                        new EventHandler(ActiveMdiChild_StateChanged);
                }
                else
                {
                    //Select a tab if it has a tag
                    int tpIndex = 0;
                    foreach (TabPage tpCheck in tabForms.TabPages)
                    {
                        if (tpCheck.Tag == this.ActiveMdiChild)
                        {
                            tabForms.SelectedIndex = tpIndex;

                            if (ActiveMdiChild is STForm) {
                                if (ActiveMdiChild.WindowState == FormWindowState.Maximized) {
                                    ((STForm)ActiveMdiChild).MDIMaximized();
                                }
                            }

                            return;
                        }
                        tpIndex++;
                    }
                }

                if (!tabForms.Visible) tabForms.Visible = true;

            }
        }

        private void ActiveMdiChild_StateChanged(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
                return;

            if (ActiveMdiChild.WindowState == FormWindowState.Maximized)
                OnMdiWindowActived();
            else
                OnMdiWindowClosed();
        }

        private const int WM_SETREDRAW = 11;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private void ActiveMdiChild_FormClosed(object sender,
                                    FormClosedEventArgs e)
        {
            ((sender as Form).Tag as TabPage).Dispose();
        }

        private void tabForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (TabPage tpCheck in tabForms.TabPages)
                tpCheck.BackColor = FormThemes.BaseTheme.TabPageInactive;

            SetFormatSettings(GetActiveIFileFormat());

            IsChanged = true;

            if (tabForms.SelectedTab != null)
            {
                tabForms.SelectedTab.BackColor = FormThemes.BaseTheme.TabPageActive;
            }

            if ((tabForms.SelectedTab != null) &&
                (tabForms.SelectedTab.Tag != null))
            {
                SendMessage(this.Handle, WM_SETREDRAW, false, 0);
                (tabForms.SelectedTab.Tag as Form).Select();
                SendMessage(this.Handle, WM_SETREDRAW, true, 0);
                this.Refresh();
            }
        }

        #endregion

        private void mainSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show(this);
        }

        private void fileAssociationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileAssociationForm form = new FileAssociationForm();
            form.ShowDialog();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreditsWindow window = new CreditsWindow();
            window.Show();
        }

        private void BtnMinMax_Click(object sender, EventArgs e)
        {

        }

        private void BtnMinMax_MouseEnter(object sender, EventArgs e)
        {

        }

        private void BtnMinMax_MouseLeave(object sender, EventArgs e)
        {

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {

        }

        private void BtnClose_MouseEnter(object sender, EventArgs e)
        {

        }

        private void BtnClose_MouseLeave(object sender, EventArgs e)
        {

        }

        private void BtnMdiClose_MouseEnter(object sender, System.EventArgs e)
        {
            BtnMdiClose.Image = Switch_Toolbox.Library.Properties.Resources.Close_Hover;
        }

        private void BtnMdiClose_MouseLeave(object sender, System.EventArgs e)
        {
            BtnMdiClose.Image = Switch_Toolbox.Library.Properties.Resources.Close;
        }

        private void BtnMdiMinMax_MouseEnter(object sender, EventArgs e)
        {
            BtnMdiMinMax.Image = Switch_Toolbox.Library.Properties.Resources.maximize_sele;
        }

        private void BtnMdiMinMax_MouseLeave(object sender, EventArgs e)
        {
            BtnMdiMinMax.Image = Switch_Toolbox.Library.Properties.Resources.maximize;
        }

        private void BtnMdiMinimize_MouseEnter(object sender, EventArgs e)
        {
            BtnMdiMinimize.Image = Switch_Toolbox.Library.Properties.Resources.minimize_sele;
        }

        private void BtnMdiMinimize_MouseLeave(object sender, EventArgs e)
        {
            BtnMdiMinimize.Image = Switch_Toolbox.Library.Properties.Resources.minimize;
        }

        private void BtnMdiClose_Click(object sender, EventArgs e)
        {
            foreach (var child in this.MdiChildren)
            {
                if (child == tabForms.SelectedTab.Tag)
                {
                    OnMdiWindowClosed();
                    child.Close();
                    return;
                }
            }
        }

        private void BtnMdiMinMax_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);

            foreach (STForm frm in this.MdiChildren) frm.MDIWindowed();
        }

        private void BtnMdiMinimize_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.WindowState = FormWindowState.Minimized;
        }

        private void updateToolstrip_Click(object sender, EventArgs e) {
            UpdateNotifcationClick();
        }

        private void tabForms_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.tabControlContextMenuStrip.Show(this.tabForms, e.Location);
            }
        }

        STConsoleForm form;

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (form == null)
            {
                form = new STConsoleForm();
                form.Show(this);
            }
            form.Focus();
        }
    }
}
