using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Switch_Toolbox.Library;
using Smash_Forge.Rendering;
using Switch_Toolbox.Library.IO;
using System.Net;


namespace Switch_Toolbox
{
    public partial class MainForm : Form
    {
        public static string executableDir = null;
        public DockContentST dockContent;
        public string LatestUpdateUrl = "";

        List<string> RecentFiles = new List<string>();

        public ObjectList objectList = null;
        IFileFormat[] SupportedFormats;
        IFileMenuExtension[] FileMenuExtensions;

        private static MainForm _instance;
        public static MainForm Instance { get { return _instance == null ? _instance = new MainForm() : _instance; } }

        public MainForm()
        {
            InitializeComponent();

            ShaderTools.executableDir = executableDir;

            Config.StartupFromFile(MainForm.executableDir + "\\config.xml");

            GenericPluginLoader.LoadPlugin();
            foreach (var plugin in GenericPluginLoader._Plugins)
            {
                plugin.Value.Load();
                LoadPluginContextMenus(plugin.Value.Types);
            }
            Settings settings = new Settings(this);
            settings.Close();
            Reload();
            LoadPluginFileContextMenus();
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
        void RegisterMenuExtIndex(ToolStripMenuItem target, ToolStripItemDark[] list, int index = 0)
        {
            foreach (var i in list)
                target.DropDownItems.Insert(index++, i);
        }
        void RegisterMenuExtIndex(ToolStrip target, ToolStripItemDark[] list, int index = 0)
        {
            foreach (var i in list)
                target.Items.Insert(index++, i);
        }

        public void Reload()
        {
            SupportedFormats = FileManager.GetFileFormats();
            FileMenuExtensions = FileManager.GetMenuExtensions();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadObjectList();
            LoadRecentList();
            foreach (string item in RecentFiles)
            {
                ToolStripMenuItem fileRecent = new ToolStripMenuItem();
                fileRecent.Click += RecentFile_click;
                fileRecent.Text = item;
                fileRecent.Size = new System.Drawing.Size(170, 40);
                fileRecent.AutoSize = true;
                fileRecent.Image = null;
                fileRecent.ForeColor = Color.White;
                recentToolStripMenuItem.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            }
            LibraryGUI.Instance.dockPanel = dockPanel1;

            if (OpenTK.Graphics.GraphicsContext.CurrentContext != null)
            {
                OpenTKSharedResources.InitializeSharedResources();
            }

            if (Runtime.OpenStartupWindow)
            {
                Startup_Window window = new Startup_Window();
                window.TopMost = true;
                window.Show();
            }
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
                ToolStripMenuItem fileRecent = new ToolStripMenuItem();
                fileRecent.Click += RecentFile_click;
                fileRecent.Text = item;
                fileRecent.Size = new System.Drawing.Size(170, 40);
                fileRecent.AutoSize = true;
                fileRecent.Image = null;
                fileRecent.ForeColor = Color.White;

                //add the menu to "recent" menu
                recentToolStripMenuItem.DropDownItems.Add(fileRecent);
            }
            //writing menu list to file
            //create file called "Recent.txt" located on app folder
            StreamWriter stringToWrite =
            new StreamWriter(System.Environment.CurrentDirectory + "\\Recent.txt");
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
            try
            {
                StreamReader listToRead = new StreamReader(System.Environment.CurrentDirectory + "\\Recent.txt"); //read file stream
                string line;
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                {
                    if (File.Exists(line))
                        RecentFiles.Add(line); //insert to list
                }
                listToRead.Close(); //close the stream
            }
            catch (Exception)
            {

                //throw;
            }

        }
        private void LoadObjectList()
        {
            objectList = new ObjectList();
            objectList.Show(dockPanel1, Runtime.objectListDockState);
        }
        public void SaveFile(IFileFormat format, string FileName)
        {
            byte[] data = format.Save();
            int Alignment = 0;

            if (format.IFileInfo != null)
               Alignment = format.IFileInfo.Alignment;

            SaveCompressFile(data, FileName, Alignment);
        }
        private void SaveCompressFile(byte[] data, string FileName, int Alignment = 0, bool EnableDialog = true)
        {
            if (EnableDialog)
            {
                DialogResult save = MessageBox.Show("Compress file?", "File Save", MessageBoxButtons.YesNo);

                if (save == DialogResult.Yes)
                    data = EveryFileExplorer.YAZ0.Compress(data, 3, (uint)Alignment);
            }
            File.WriteAllBytes(FileName, data);
            MessageBox.Show($"File has been saved to {FileName}");
            Cursor.Current = Cursors.Default;
        }
        public void OpenFile(string FileName, byte[] data = null, bool Compressed = false,
            CompressionType CompType = CompressionType.None)
        {
            Reload();
            if (data == null)
                data = File.ReadAllBytes(FileName);

            if (File.Exists(FileName))
                SaveRecentFile(FileName);

            FileReader f = new FileReader(data);
            string Magic = f.ReadMagic(0, 4);
            string Magic2 = f.ReadMagic(0, 2);

            //Determine if the file is compressed or not
            if (Magic == "Yaz0")
            {
                data = EveryFileExplorer.YAZ0.Decompress(data).ToArray();
                OpenFile(FileName, data, true, CompressionType.Yaz0);
                return;
            }
            if (Magic == "ZLIB")
            {
                data = FileReader.InflateZLIB(f.getSection(64, data.Length - 64));
                OpenFile(FileName, data, true, CompressionType.Zlib);
                return;
            }
            if (Path.GetExtension(FileName) == ".cmp" && CompType == CompressionType.None)
            {
                f.Position = 0;
                int OuSize = f.ReadInt32();
                int InSize = data.Length - 4;
                data = STLibraryCompression.Type_LZ4F.Decompress(f.getSection(4, InSize));
                OpenFile(FileName, data, true, CompressionType.Lz4f);
                return;
            }

            f.Dispose();
            f.Close();

             //Check magic first regardless of extension
            foreach (IFileFormat format in SupportedFormats)
            {
                if (format.Magic == Magic || format.Magic == Magic2 || format.Magic.Reverse() == Magic2)
                {
                    format.CompressionType = CompType;
                    format.FileIsCompressed = Compressed;
                    format.Data = data;
                    format.FileName = Path.GetFileName(FileName);
                    format.Load();
                    format.FilePath = FileName;

                    if (format.EditorRoot != null)
                    {
                        objectList.treeView1.Nodes.Add(format.EditorRoot);
                    }

                    if (format.CanSave)
                    {
                        saveAsToolStripMenuItem.Enabled = true;
                        saveToolStripMenuItem.Enabled = true;
                    }
                    if (format.UseEditMenu)
                        editToolStripMenuItem.Enabled = true;

                    return;
                }
            }
            //If magic fails, then check extensions
            foreach (IFileFormat format in SupportedFormats)
            {
                foreach (string ext in format.Extension)
                {
                    if (ext.Remove(0, 1) == Path.GetExtension(FileName))
                    {
                        format.CompressionType = CompType;
                        format.FileIsCompressed = Compressed;
                        format.Data = data;
                        format.FileName = Path.GetFileName(FileName);
                        format.FilePath = FileName;
                        format.Load();

                        if (format.EditorRoot != null)
                        {
                            objectList.treeView1.Nodes.Add(format.EditorRoot);
                        }

                        if (format.CanSave)
                        {
                            saveAsToolStripMenuItem.Enabled = true;
                            saveToolStripMenuItem.Enabled = true;
                        }
                        if (format.UseEditMenu)
                            editToolStripMenuItem.Enabled = true;

                        return;
                    }
                }
            }
        }
        private void DisposeControls()
        {

        }
        private void RecentFile_click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            OpenFile(sender.ToString());
            Cursor.Current = Cursors.Default;
        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginManager pluginManager = new PluginManager();
            pluginManager.Show();
        }

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

        private void showObjectlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectList == null)
                LoadObjectList();
        }

        private void dockPanel1_DragDrop(object sender, DragEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
            {
                OpenFile(filename);
            }

            Cursor.Current = Cursors.Default;
        }

        private void dockPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
            {
                String[] strGetFormats = e.Data.GetFormats();
                e.Effect = DragDropEffects.None;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(true);
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void SaveFile(bool UseSaveDialog)
        {
            foreach (IFileFormat format in SupportedFormats)
            {
                if (format.CanSave)
                {
                    List<IFileFormat> f = new List<IFileFormat>();
                    f.Add(format);

                    if (UseSaveDialog)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = Utils.GetAllFilters(f);
                        sfd.FileName = format.FileName;

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            SaveFile(format, sfd.FileName);
                        }
                    }
                    else
                    {
                        SaveFile(format, format.FilePath);
                    }
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)       // Ctrl-S Save
            {
                // Do what you want here
                SaveFile(true);

                e.SuppressKeyPress = true;  // Stops other controls on the form receiving event.

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        private void saveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Save();

            //    dockPanel1.SaveAsXml("DockStates/" + dockPanel1.Text + ".xml");
        }

        private void dockPanel1_DockChanged(object sender, EventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(this);
            settings.Show();
        }

        private void exportShaderErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShaderTools.SaveErrorLogs();
        }

        private void clearWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var plugin in GenericPluginLoader._Plugins)
            {
                plugin.Value.Unload();

                foreach (IFileFormat format in SupportedFormats)
                {
                    if (format.CanSave)
                    {
                        format.Unload();
                    }
                }
            }
            Viewport.Instance.Dispose();
            GC.Collect();
        }

        public void CompressData(CompressionType CompressionType, byte[] data)
        {
            switch (CompressionType)
            {
                case CompressionType.Yaz0:
                    SaveFileForCompression(EveryFileExplorer.YAZ0.Compress(data, Runtime.Yaz0CompressionLevel));
                    break;
                case CompressionType.Zlib:
                    break;
                case CompressionType.Gzip:
                    SaveFileForCompression(STLibraryCompression.GZIP.Compress(data));
                    break;
                case CompressionType.Zstb:
                    break;
                case CompressionType.Lz4f:
                    SaveFileForCompression(STLibraryCompression.Type_LZ4F.Compress(data));
                    break;
                case CompressionType.Lz4:
                    SaveFileForCompression(STLibraryCompression.Type_LZ4.Compress(data));
                    break;
            }
        }
        public void DecompressData(CompressionType CompressionType, byte[] data)
        {
            try
            {
                switch (CompressionType)
                {
                    case CompressionType.Yaz0:
                        SaveFileForCompression(EveryFileExplorer.YAZ0.Decompress(data));
                        break;
                    case CompressionType.Zlib:
                        break;
                    case CompressionType.Gzip:
                        SaveFileForCompression(STLibraryCompression.GZIP.Decompress(data));
                        break;
                    case CompressionType.Zstb:
                        break;  
                    case CompressionType.Lz4f:
                        SaveFileForCompression(STLibraryCompression.Type_LZ4F.Decompress(data));
                        break;
                    case CompressionType.Lz4:
                        SaveFileForCompression(STLibraryCompression.Type_LZ4.Decompress(data));
                        break;
                }
            }
            catch
            {
                MessageBox.Show($"File not compressed with {CompressionType} compression!");
            }
        }

        private void yaz0DecompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Yaz0, false);
        }
        private void yaz0CompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Yaz0, true);
        }
        private void gzipCompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Gzip, true);
        }
        private void lz4CompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Lz4, false);
        }
        private void lz4fCompressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Lz4f, true);
        }
        private void gzipDecompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Gzip, false);
        }
        private void lz4DecompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Lz4, false);
        }
        private void lz4fDeompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileForCompression(CompressionType.Lz4f, false);
        }

        private void SaveFileForCompression(byte[] data)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files(*.*)|*.*";

            Cursor.Current = Cursors.Default;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveCompressFile(data, sfd.FileName, 0, false);
            }
        }
        private void OpenFileForCompression(CompressionType compressionType, bool Compress)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files(*.*)|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (string file in ofd.FileNames)
                {
                    if (Compress)
                        CompressData(compressionType, File.ReadAllBytes(ofd.FileName));
                    else
                        DecompressData(compressionType, File.ReadAllBytes(ofd.FileName));
                }
            }
        }

        private void checkUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LatestUpdateUrl == "")
                return;

            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(LatestUpdateUrl, "update.zip");
                webClient.DownloadDataCompleted += UpdateDownloadCompleted;
            }
        }
        void UpdateDownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            MessageBox.Show("Update downloaded!");

            System.IO.Compression.ZipFile.ExtractToDirectory("update.zip", "update/");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreditsWindow credits = new CreditsWindow();
            credits.TopMost = true;
            credits.Show();
        }
    }
}
