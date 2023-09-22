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
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library.NodeWrappers;
using Toolbox.Library.Rendering;
using Bfres.Structs;
using Syroot.NintenTools.NSW.Bntx;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using FirstPlugin;

namespace Toolbox
{
    public partial class MainForm : Form, IMdiContainer, IUpdateForm, IMainForm
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
            InitializeComponent();
        }

        public void UpdateForm()
        {
            if (ActiveMdiChild is ObjectEditor)
            {
                var activeFile = ((ObjectEditor)ActiveMdiChild).GetActiveFile();
                SetFormatSettings(activeFile);
            }
        }

        //Use for files opened with program
        public List<string> OpenedFiles = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            Runtime.MainForm = this;
            compressionToolStripMenuItem.DropDownItems.AddRange(CompressionMenus.GetMenuItems().ToArray());

            //Redo setting this since designer keeps resetting this
            tabForms.myBackColor = FormThemes.BaseTheme.FormBackColor;

            OnMdiWindowClosed();
            ResetMenus();

            bool HasVersionFile = true;
            VersionCheck version = new VersionCheck(HasVersionFile);

            if (HasVersionFile)
            {
                Runtime.ProgramVersion = version.ProgramVersion;
                Runtime.CommitInfo = version.CommitInfo;
                Runtime.CompileDate = version.CompileDate;
            }
            else
            {
                version.SaveVersionInfo();
            }

            ThreadStart t = new ThreadStart(UpdateProgram.CheckLatest);
            Thread thread = new Thread(t);
            thread.Start();

            Application.Idle += Application_Idle;

            if (Runtime.UseOpenGL)
            {
                ShaderTools.executableDir = Runtime.ExecutableDir;

                OpenTKSharedResources.InitializeSharedResources();

                if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Initialized)
                {
                    Runtime.OpenTKInitialized = true;
                    Runtime.renderer = GL.GetString(StringName.Renderer);
                    Runtime.openGLVersion = GL.GetString(StringName.Version);
                    Runtime.GLSLVersion = GL.GetString(StringName.ShadingLanguageVersion);
                    ParseGLVersion();
                }
            }

            LoadPLugins();
            UpdateToolbar(HasVersionFile);
            LoadMDITheme();
            LoadRecentList();
            ReloadFiles();
            LoadPluginFileContextMenus();
            WindowsExplorer.ExplorerContextMenu.LoadMenus();

            foreach (string file in OpenedFiles)
            {
                if (File.Exists(file))
                    OpenFile(file);
            }

            OpenedFiles.Clear();

            if (Runtime.UseDebugDomainExceptionHandler)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            }
        }

        public void OpenFiles()
        {
            foreach (string file in OpenedFiles)
            {
                if (File.Exists(file))
                    OpenFile(file);
            }

            OpenedFiles.Clear();
        }

        private void ReloadFiles()
        {
            SupportedFormats = FileManager.GetFileFormats();
            FileMenuExtensions = FileManager.GetMenuExtensions(); 
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
                if (major <= 1)
                {
                    Runtime.UseOpenGL = false;
                }
            }
        }

        #region Updater
        bool UsePrompt = true;
        private void Application_Idle(object sender, EventArgs e)
        {
            if (UpdateProgram.CanUpdate && Runtime.EnableVersionCheck && UsePrompt &&
                UpdateProgram.CommitList.Count > 0)
            {
                updateToolstrip.Enabled = true;
                UsePrompt = false;
            }
        }


        private void UpdateNotifcationClick()
        {
            if (UpdateProgram.CommitList.Count <= 0)
                return;

            var dialog = new GithubUpdateDialog();
            dialog.LoadCommits(UpdateProgram.CommitList);
            if (dialog.ShowDialog() == DialogResult.Yes)
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
            Environment.Exit(0);
        }
        #endregion

        #region OpenFile

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileSelect();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            FolderSelectDialog dlg = new FolderSelectDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var root = new ExplorerFolder(dlg.SelectedPath);
                ObjectEditor editor = new ObjectEditor();
                editor.MdiParent = this;
                editor.Text = CheckTabDupes(root.Text);
                editor.AddNode(root);
                editor.Show();
            }
        }

        private void OpenFileSelect()
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
            if (!File.Exists(FileName))
                return;

            SaveRecentFile(FileName);

            object file = STFileLoader.OpenFileFormat(FileName);
            if (file == null) //File might not be supported so return
            {
                STConsole.WriteLine($"{FileName} not supported.");
                return;
            }

            Type objectType = file.GetType();

            bool HasEditorActive = false;
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    MethodInfo method = objectType.GetMethod("OpenForm");
                    MethodInfo methodFill = objectType.GetMethod("FillEditor");
                    var control = (UserControl)method.Invoke(file, new object[0]);
                    methodFill.Invoke(file, new object[1] { control });
                    var form = new GenericEditorForm(false, control);
                    TabDupeIndex = 0;
                    form.Text = CheckTabDupes(((IFileFormat)file).FileName);
                    form.MdiParent = this;
                    form.Show();

                    HasEditorActive = true;
                }
                else if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditorForm<>))
                {
                    MethodInfo method = objectType.GetMethod("OpenForm");
                    var form = (Form)method.Invoke(file, new object[0]);
                    TabDupeIndex = 0;
                    form.Text = CheckTabDupes(((IFileFormat)file).FileName);
                    form.Show();

                    HasEditorActive = true;
                }
            }

            bool IsTreeNode = file is TreeNode;
            bool IsArchiveFile = file is IArchiveFile;

            if (!IsTreeNode && !IsArchiveFile || HasEditorActive)
            {
                SetFormatSettings(GetActiveIFileFormat());
                return;
            }

            //ObjectEditor is for treenode or archive file types. Editors will be on the right side, treenodes on the left
            SetFormatSettings((IFileFormat)file);

            //Check for active object editors
            Form editor = (Form)LibraryGUI.GetActiveForm();

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

            if (!useActiveEditor || !IsEditorActive)
            {
                editor = new ObjectEditor(((IFileFormat)file));
                editor.MdiParent = this;
                editor.Text = CheckTabDupes(((IFileFormat)file).FileName);
                editor.Show();

                ((ObjectEditor)editor).SelectFirstNode();
            }
            else
            {
                if (IsArchiveFile)
                    ((ObjectEditor)editor).AddIArchiveFile((IFileFormat)file);
                else
                    AddObjectEditorFile(((TreeNode)file), (ObjectEditor)editor, false);
            }
            SetFormatSettings(GetActiveIFileFormat());

            if (file is TreeNodeFile)
                ((TreeNodeFile)file).OnAfterAdded();
        }

        private void AddObjectEditorFile(TreeNode file, ObjectEditor editor, bool ClearFiles)
        {
            TabDupeIndex = 0;
            editor.MdiParent = this;
            editor.AddNode(file, ClearFiles);
            editor.SelectNode(file);

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

                    if (ActiveMdiChild is IFIleEditor)
                    {
                       // if (((IFIleEditor)ActiveMdiChild).GetFileFormats().Count > 0)
                        //    ((IFIleEditor)ActiveMdiChild).BeforeFileSaved();
                    }

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
                            sfd.DefaultExt = Path.GetExtension(format.FilePath);

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
                            sfd.DefaultExt = Path.GetExtension(format.FilePath);

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
            foreach (var node in editor.GetNodes())
            {
                IFileFormat format = null;
                if (node is ArchiveBase)
                {
                    format = (IFileFormat)((ArchiveBase)node).ArchiveFile;

                    if (node is ArchiveRootNodeWrapper)
                        ((ArchiveRootNodeWrapper)node).UpdateFileNames();
                }
                else if (node is IFileFormat)
                {
                    format = ((IFileFormat)node);
                }
                if (format != null)
                {
                    if (!format.CanSave)
                    {
                        if (Runtime.AlwaysSaveAll)
                        {
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }

                    string FileName = format.FilePath;
                    if (!File.Exists(FileName))
                        UseSaveDialog = true;

                    if (UseSaveDialog)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = Utils.GetAllFilters(format);
                        sfd.FileName = format.FileName;

                        if (sfd.ShowDialog() != DialogResult.OK)
                        {
                            if (Runtime.AlwaysSaveAll)
                            {
                                continue;
                            }
                            else
                            {
                                return;
                            }
                        }

                        FileName = sfd.FileName;
                    }
                    Cursor.Current = Cursors.WaitCursor;

                    //Use the export method for particular formats like bfres for special save operations
                    if (format is STGenericWrapper && !(format is STGenericTexture))
                    {
                        ((STGenericWrapper)format).Export(FileName);
                        if (Runtime.AlwaysSaveAll)
                        {
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (node is ArchiveBase)
                        STFileSaver.SaveFileFormat(((IFileFormat)((ArchiveBase)node).ArchiveFile), FileName, UseCompressDialog);
                    else
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

        public static void LoadConfig()
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

        private void UpdateToolbar(bool DisplayVersion)
        {
            string commit = $"Commit: {Runtime.CommitInfo}";
            var asssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (DisplayVersion)
                Text = $"{Application.ProductName} | Version: {Runtime.ProgramVersion} | {commit} | Compile Date: {Runtime.CompileDate} Assembly {asssemblyVersion}";
            else
                Text = $"{Application.ProductName}";
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
             //   if (ext.MapEditorMenuExtensions != null)
               //     RegisterMenuExtIndex(mapEditorsToolStripMenuItem, ext.MapEditorMenuExtensions, mapEditorsToolStripMenuItem.DropDownItems.Count);
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
            {
                  target.Items.Insert(index++, i);
            }
        }
        void RegisterMenuExtIndex(ToolStripMenuItem target, STToolStripItem[] list, int index = 0)
        {
            foreach (STToolStripItem i in list)
            {
                if (i == null)
                    continue;

                if (!MenuHasItem(target, i))
                    target.DropDownItems.Insert(index++, i);
                else
                    MergeDuplicateMenus(i, GetDuplicateMenuItem(target, i));
            }
        }

        bool MenuHasItem(ToolStripMenuItem target, STToolStripItem item)
        {
            foreach (ToolStripItem i in target.DropDownItems)
                if (i.Text == item.Text)
                    return true;

            return false;
        }


        STToolStripItem GetDuplicateMenuItem(ToolStripMenuItem target, STToolStripItem item)
        {
            foreach (STToolStripItem i in target.DropDownItems)
                if (i.Text == item.Text)
                    return i;

            return null;
        }


        void MergeDuplicateMenus(STToolStripItem src, STToolStripItem dest )
        {
            dest.DropDownItems.AddRange(src.DropDownItems);
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
            else if (ActiveMdiChild is GenericEditorForm)
            {
                var control = ((GenericEditorForm)ActiveMdiChild).GetControl();
                if (control != null && control is IFIleEditor)
                {
                    if (((IFIleEditor)control).GetFileFormats().Count > 0)
                        return ((IFIleEditor)control).GetFileFormats()[0];
                }
                if (control != null && control is ImageEditorBase)
                {
                    return (IFileFormat)((ImageEditorBase)control).ActiveTexture;
                }
                if (control != null && control is AudioPlayerPanel)
                {
                    return ((AudioPlayerPanel)control).AudioFileFormats[0];
                }
            }
            else if (ActiveMdiChild is IFIleEditor)
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
            if (e.Control && e.KeyCode == Keys.O) // Ctrl + O Open
            {
                e.SuppressKeyPress = true;
                OpenFileSelect();
            }
            else if (e.Control && e.Alt && e.KeyCode == Keys.S) // Ctrl + Alt + S Save As
            {
                e.SuppressKeyPress = true;
                SaveActiveFile(true);
            }
            else if(e.Control && e.KeyCode == Keys.S) // Ctrl + S Save
            {
                e.SuppressKeyPress = true;
                SaveActiveFile(false);
            }
            else if (e.Control && e.KeyCode == Keys.W) // Ctrl + W Exit
            {
                e.SuppressKeyPress = true;
                Application.Exit();
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
            if (!Runtime.EnableDragDrop) return;

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
                        if (!Runtime.EnableDragDrop) return;

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
            if (LibraryGUI.GetAnimationPanel() != null)
            {
                LibraryGUI.GetAnimationPanel().CurrentAnimation = null;
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
                    {
                        ((STForm)this.ActiveMdiChild).Maximize();
                    }

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
                BtnMdiClose.Image = Toolbox.Library.Properties.Resources.Close_Hover;
            }

            private void BtnMdiClose_MouseLeave(object sender, System.EventArgs e)
            {
                BtnMdiClose.Image = Toolbox.Library.Properties.Resources.Close;
            }

            private void BtnMdiMinMax_MouseEnter(object sender, EventArgs e)
            {
                BtnMdiMinMax.Image = Toolbox.Library.Properties.Resources.maximize_sele;
            }

            private void BtnMdiMinMax_MouseLeave(object sender, EventArgs e)
            {
                BtnMdiMinMax.Image = Toolbox.Library.Properties.Resources.maximize;
            }

            private void BtnMdiMinimize_MouseEnter(object sender, EventArgs e)
            {
                BtnMdiMinimize.Image = Toolbox.Library.Properties.Resources.minimize_sele;
            }

            private void BtnMdiMinimize_MouseLeave(object sender, EventArgs e)
            {
                BtnMdiMinimize.Image = Toolbox.Library.Properties.Resources.minimize;
            }

        private void BtnMdiClose_Click(object sender, EventArgs e)
        {
            foreach (var child in this.MdiChildren)
            {
                if (child == tabForms.SelectedTab.Tag)
                {
                    OnMdiWindowClosed();
                    child.Close();
                    GC.Collect();
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
            if (form == null || form.IsDisposed)
            {
                form = new STConsoleForm();
                form.Show(this);
            }
            form.Focus();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CreditsWindow window = new CreditsWindow();
            window.Show();
        }

        private void githubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/KillzXGaming/Switch-Toolbox");
        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/KillzXGaming/Switch-Toolbox/issues");
        }

        private void requestFeatureToolStripMenuItem1_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/KillzXGaming/Switch-Toolbox/issues");
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/KillzXGaming/Switch-Toolbox/wiki");
        }

        private void checkShaderErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShaderTools.SaveErrorLogs();

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (tabForms.TabPages.Count > 0)
            {
                if (Runtime.ShowCloseDialog)
                {
                    var form = new ApplicationCloseDialog();
                    form.StartPosition = FormStartPosition.CenterParent;
                    if (form.ShowDialog() != DialogResult.OK)
                        e.Cancel = true;

                    Config.Save();
                }
            }
        }

        private void batchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectDialog folderDlg = new FolderSelectDialog();
            if (folderDlg.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.BatchFileTable(folderDlg.SelectedPath);
            }
        }

        private void hashCalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HashCalculatorForm form = new HashCalculatorForm();
            form.Show(this);
        }

        private void batchExportTexturesAllSupportedFormatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FolderSelectDialog folderDlg = new FolderSelectDialog();
                if (folderDlg.ShowDialog() == DialogResult.OK) {
                    BatchExportTextures(ofd.FileNames, folderDlg.SelectedPath);
                }
            }
        }


        private void batchExportModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FolderSelectDialog folderDlg = new FolderSelectDialog();
                if (folderDlg.ShowDialog() == DialogResult.OK) {
                    BatchExportModels(ofd.FileNames, folderDlg.SelectedPath);
                }
            }
        }

        private void batchReplaceTXTGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchReplaceTXTG();
        }

        private void BatchReplaceTXTG()
        {
            ObjectEditor ObjectEditor = (ObjectEditor)ActiveMdiChild;
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                foreach (TreeNode node in ObjectEditor.GetNodes())
                {
                    STGenericWrapper foundNode = (STGenericWrapper)node;
                    if (foundNode == null)
                    {
                        continue;
                    }

                    foreach (string file in System.IO.Directory.GetFiles(sfd.SelectedPath))
                    {
                        if (!file.Contains(foundNode.Text + "."))
                        {
                            continue;
                        }
                        foundNode.Replace(file);
                    }
                }
            }
        }
        private void batchReplaceFTPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchReplaceFTP();
        }

        private void BatchReplaceFTP()
        {
            ObjectEditor ObjectEditor = (ObjectEditor)ActiveMdiChild;
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                foreach (TreeNode node in ObjectEditor.GetNodes())
                {
                    TreeNode foundNode = FindNodeByText(node, "Texture Pattern Animations");

                    // Skip if no Texture Pattern Animation node
                    if (foundNode == null)
                    {
                        continue;
                    }

                    string parentName = foundNode.FullPath.Split('\\')[0];
                    string sourcePath = Path.Combine(sfd.SelectedPath, parentName + ".bfres");
                    
                    // Skip if no path found
                    if (!Directory.Exists(sourcePath))
                    {
                        continue;
                    }

                    BFRESGroupNode groupNode = (BFRESGroupNode)foundNode;
                    groupNode.ReplaceAll(sourcePath);
                }
            }
        }

        private TreeNode FindNodeByText(TreeNode treeNode, string searchText)
        {
            // Check if the current node matches the searchText.
            if (treeNode.Text == searchText)
            {
                return treeNode; // Found a match, return the current node.
            }

            // Recursively search in each child node.
            foreach (TreeNode tn in treeNode.Nodes)
            {
                TreeNode result = FindNodeByText(tn, searchText);
                if (result != null)
                {
                    return result; // If a match is found in the child nodes, return it.
                }
            }

            // If no match is found in this subtree, return null.
            return null;
        }

        private void batchRenameBNTXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectEditor ObjectEditor = (ObjectEditor)ActiveMdiChild;

            foreach (TreeNode node in ObjectEditor.GetNodes())
            {
                FirstPlugin.BNTX foundNode = (FirstPlugin.BNTX)node;

                // Skip if no BNTX
                if (foundNode == null)
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(foundNode.FilePath).Split('.')[0];

                // Rename file
                foundNode.Text = fileName;
                if (foundNode.BinaryTexFile != null)
                {
                    foundNode.BinaryTexFile.Name = fileName;
                }

                string textureKey = foundNode.Textures.Keys.FirstOrDefault();
                TextureData textureData = foundNode.Textures.Values.FirstOrDefault();
                if (textureData != null)
                {
                    textureData.Text = fileName;
                    textureData.Name = fileName;
                    textureData.Texture.Name = fileName;
                    foundNode.Textures.Remove(textureKey);
                    foundNode.Textures.Add(fileName, textureData);
                }
            }
            ObjectEditor.Update();
        }

        private List<string> failedFiles = new List<string>();
        private List<string> batchExportFileList = new List<string>();
        private void BatchExportModels(string[] files, string outputFolder)
        {
            List<string> Formats = new List<string>();
            Formats.Add("DAE (.dae)");

            failedFiles = new List<string>();

            BatchFormatExport form = new BatchFormatExport(Formats);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string extension = form.GetSelectedExtension();
                foreach (var file in files)
                {
                    IFileFormat fileFormat = null;
                    try
                    {
                        fileFormat = STFileLoader.OpenFileFormat(file);
                    }
                    catch (Exception ex)
                    {
                        failedFiles.Add($"{file} \n Error:\n {ex} \n");
                    }

                    SearchFileFormat(form.BatchSettings, fileFormat, extension, outputFolder, ExportMode.Models);
                }
                batchExportFileList.Clear();
            }
            else
                return;

            if (failedFiles.Count > 0)
            {
                string detailList = "";
                foreach (var file in failedFiles)
                    detailList += $"{file}\n";

                STErrorDialog.Show("Some files failed to export! See detail list of failed files.", "Switch Toolbox", detailList);
            }
            else
                MessageBox.Show("Files batched successfully!");
        }

        private void BatchExportTextures(string[] files, string outputFolder)
        {
            List<string> Formats = new List<string>();
            Formats.Add("Portable Graphics Network (.png)");
            Formats.Add("Microsoft DDS (.dds)");
            Formats.Add("Joint Photographic Experts Group (.jpg)");
            Formats.Add("Bitmap Image (.bmp)");
            Formats.Add("Tagged Image File Format (.tiff)");
            Formats.Add("ASTC (.astc)");

           failedFiles = new List<string>();

            BatchFormatExport form = new BatchFormatExport(Formats);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string extension = form.GetSelectedExtension();
                foreach (var file in files)
                {
                    IFileFormat fileFormat = null;
                    try
                    {
                         fileFormat = STFileLoader.OpenFileFormat(file);
                    }
                    catch (Exception ex)
                    {
                        failedFiles.Add($"{file} \n Error:\n {ex} \n");
                    }

                    SearchFileFormat(form.BatchSettings, fileFormat, extension, outputFolder, ExportMode.Textures);
                }
                batchExportFileList.Clear();
            }

            if (failedFiles.Count > 0)
            {
                string detailList = "";
                foreach (var file in failedFiles)
                    detailList += $"{file}\n";

                STErrorDialog.Show("Some files failed to export! See detail list of failed files.", "Switch Toolbox", detailList);
            }
            else
                MessageBox.Show("Files batched successfully!");
        }

        private void SearchFileFormat(BatchFormatExport.Settings settings, IFileFormat fileFormat, 
            string extension, string outputFolder, ExportMode exportMode)
        {
            if (fileFormat == null) return;

            if (fileFormat is STGenericTexture && exportMode == ExportMode.Textures) {
                string name = ((STGenericTexture)fileFormat).Text;
                ExportTexture(((STGenericTexture)fileFormat), settings, $"{outputFolder}/{name}", extension);
            }
            else if (fileFormat is IArchiveFile)
                SearchArchive(settings, (IArchiveFile)fileFormat, extension, outputFolder, exportMode);
            else if (fileFormat is ITextureContainer && exportMode == ExportMode.Textures)
            {
                string name = fileFormat.FileName.Split('.').FirstOrDefault();
                if (settings.SeperateTextureContainers)
                    outputFolder = Path.Combine(outputFolder, name);

                if (((ITextureContainer)fileFormat).TextureList.Count > 0)
                {
                    if (!Directory.Exists(outputFolder))
                        Directory.CreateDirectory(outputFolder);
                }

                foreach (STGenericTexture tex in ((ITextureContainer)fileFormat).TextureList) {
                    ExportTexture(tex, settings, $"{outputFolder}/{tex.Text}", extension);
                }
            }
            else if (fileFormat is IExportableModelContainer && exportMode == ExportMode.Models)
            {
                string name = fileFormat.FileName.Split('.').FirstOrDefault();
                if (settings.SeperateTextureContainers)
                    outputFolder = Path.Combine(outputFolder, name);

                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                DAE.ExportSettings daesettings = new DAE.ExportSettings();
                daesettings.SuppressConfirmDialog = true;
                daesettings.ExportTextures = settings.ExportTextures;

                var textures = ((IExportableModelContainer)fileFormat).ExportableTextures.ToList();
                foreach (var model in ((IExportableModelContainer)fileFormat).ExportableModels)
                {
                    string path = $"{outputFolder}/{model.Text}";
                    path = Utils.RenameDuplicateString(batchExportFileList, path, 0, 3);

                    DAE.Export($"{path}.{extension}", daesettings, model, textures, model.GenericSkeleton);
                    batchExportFileList.Add(path);
                }
            }
            else if (fileFormat is IExportableModel && exportMode == ExportMode.Models)
            {
                string name = fileFormat.FileName.Split('.').FirstOrDefault();
                if (settings.SeperateTextureContainers)
                    outputFolder = Path.Combine(outputFolder, name);

                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                DAE.ExportSettings daesettings = new DAE.ExportSettings();
                daesettings.SuppressConfirmDialog = true;
                daesettings.ExportTextures = settings.ExportTextures;

                var model = new STGenericModel();
                model.Materials = ((IExportableModel)fileFormat).ExportableMaterials;
                model.Objects = ((IExportableModel)fileFormat).ExportableMeshes;
                var textures = ((IExportableModel)fileFormat).ExportableTextures.ToList();
                var skeleton = ((IExportableModel)fileFormat).ExportableSkeleton;
                string modelname = Path.GetFileNameWithoutExtension(fileFormat.FileName);
                string path = $"{outputFolder}/{modelname}";
                path = Utils.RenameDuplicateString(batchExportFileList, path, 0, 3);
                batchExportFileList.Add(path);

                DAE.Export($"{path}.{extension}", daesettings, model, textures, skeleton);
            }

            fileFormat.Unload();
        }

        public enum ExportMode
        {
            Models,
            Textures,
        }

        private void ExportTexture(STGenericTexture tex, BatchFormatExport.Settings settings, string filePath, string ext) {
            filePath = Utils.RenameDuplicateString(batchExportFileList, filePath, 0, 3);
            batchExportFileList.Add(filePath);

            //Switch the runtime comp setting to the batch settings then switch back later
            bool compSetting = Runtime.ImageEditor.UseComponetSelector;

            Runtime.ImageEditor.UseComponetSelector = settings.UseTextureChannelComponents;
            tex.Export($"{filePath}.{ext}");
            Runtime.ImageEditor.UseComponetSelector = compSetting;
        }

        private void SearchArchive(BatchFormatExport.Settings settings, IArchiveFile archiveFile,
            string extension, string outputFolder, ExportMode exportMode)
        {
            string ArchiveFilePath = outputFolder;
            if (settings.SeperateArchiveFiles)
                ArchiveFilePath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(((IFileFormat)archiveFile).FileName));

            foreach (var file in archiveFile.Files)
            {
                try
                {
                    SearchFileFormat(settings, file.OpenFile(), extension, ArchiveFilePath, exportMode);
                }
                catch (Exception ex)
                {
                    failedFiles.Add($"{file} \n Error:\n {ex} \n");
                }
            }
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebUtil.OpenDonation();
        }

        private void openUserFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var userDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SwitchToolbox");
            if (!Directory.Exists(userDir))
                Directory.CreateDirectory(userDir);

            Process.Start("explorer.exe", userDir);
        }
    }
}
