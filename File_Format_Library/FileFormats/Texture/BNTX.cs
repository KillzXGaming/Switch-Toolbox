using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Syroot.NintenTools.NSW.Bntx;
using Syroot.NintenTools.NSW.Bntx.GFX;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using Toolbox.Library.Animations;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class BNTX : TreeNodeFile, IFileFormat, IContextMenuNode, ITextureContainer
    {
        public bool LoadIcons = false;

        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "BNTX" };
        public string[] Extension { get; set; } = new string[] { "*.bntx" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }
        
        public BNTX()
        {
            ImageKey = "bntx";
            SelectedImageKey = "bntx";
        }

        private long FilePosition = 0;
        private byte[] ExtraHeader;
        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                bool isBntx = reader.CheckSignature(4, "BNTX");
                bool isKillLaKill = false;
                if (stream.Length > 84)
                {
                    isKillLaKill = reader.CheckSignature(4, "BNTX", 80);
                    if (isKillLaKill) {
                        ExtraHeader = reader.ReadBytes(80);
                        FilePosition = 80;
                    }

                    reader.Position = 0;
                }

                return isBntx || isKillLaKill;
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        public bool DisplayIcons => LoadIcons;

        public List<STGenericTexture> TextureList
        {
            get
            {
                List<STGenericTexture> textures = new List<STGenericTexture>();
                foreach (STGenericTexture node in Nodes)
                    textures.Add(node);

                return textures;
            }
            set { }
        }

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => newFileExt;
            public STToolStripItem[] NewFromFileMenuExtensions => newFromFileExt;
            public STToolStripItem[] ToolsMenuExtensions => toolExt;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] toolExt = new STToolStripItem[1];
            STToolStripItem[] newFileExt = new STToolStripItem[1];
            STToolStripItem[] newFromFileExt = new STToolStripItem[2];

            public MenuExt()
            {
                toolExt[0] = new STToolStripItem("Textures");
                toolExt[0].DropDownItems.Add(new STToolStripItem("Batch All (BNTX)", Export));

                newFileExt[0] = new STToolStripItem("BNTX", New);
                newFromFileExt[0] = new STToolStripItem("BNTX From Image", NewFromFile);
            }

            private void New(object sender, EventArgs args)
            {
                BNTX bntx = new BNTX();
                bntx.IFileInfo = new IFileInfo();
                bntx.FileName = "textures";
                bntx.Load(new MemoryStream(CreateNewBNTX("textures")));

                ObjectEditor editor = new ObjectEditor(bntx);
                editor.Text = "Untitled-" + 0;
                LibraryGUI.CreateMdiWindow(editor);
            }

            private void NewFromFile(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.Filter = FileFilters.BNTX_TEX;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //Create a quick one to load all the textures imported in
                    BNTX tempBntx = new BNTX();
                    tempBntx.IFileInfo = new IFileInfo();
                    tempBntx.Load(new MemoryStream(CreateNewBNTX("textures.bntx")));
                    tempBntx.ImportTexture(ofd.FileNames);

                    string folder = Path.GetDirectoryName(ofd.FileNames[0]);

                    FolderSelectDialog fod = new FolderSelectDialog();
                    if (fod.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var tex in tempBntx.Textures.Values)
                        {
                            //Now make one for each texture and import each
                            var bntx = new BntxFile();
                            bntx.Target = new char[] { 'N', 'X', ' ', ' ' };
                            bntx.Name = tex.Text;
                            bntx.Alignment = 0xC;
                            bntx.TargetAddressSize = 0x40;
                            bntx.VersionMajor = 0;
                            bntx.VersionMajor2 = 4;
                            bntx.VersionMinor = 0;
                            bntx.VersionMinor2 = 0;
                            bntx.Textures = new List<Texture>();
                            bntx.TextureDict = new ResDict();
                            bntx.RelocationTable = new RelocationTable();
                            bntx.Flag = 0;
                            bntx.Textures.Add(tex.Texture);
                            bntx.Save($"{Path.Combine(fod.SelectedPath, tex.Text)}.bntx");
                        }
                    }

                    tempBntx.Unload();
                }
            }

            private void Export(object sender, EventArgs args)
            {
                string formats = FileFilters.BNTX_TEX;

                string[] forms = formats.Split('|');

                List<string> Formats = new List<string>();
                for (int i = 0; i < forms.Length; i++)
                {
                    if (i > 1 || i == (forms.Length - 1)) //Skip lines with all extensions
                    {
                        if (!forms[i].StartsWith("*"))
                            Formats.Add(forms[i]);
                    }
                }

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string Extension = form.GetSelectedExtension();

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Multiselect = true;
                    ofd.Filter = Utils.GetAllFilters(new Type[] { typeof(BNTX), typeof(BXFNT), typeof(BFRES), typeof(PTCL), typeof(SARC) });

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        FolderSelectDialog folderDialog = new FolderSelectDialog();
                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string file in ofd.FileNames)
                            {
                                var FileFormat = STFileLoader.OpenFileFormat(file, new Type[] { typeof(BNTX), typeof(BXFNT), typeof(BFRES), typeof(PTCL), typeof(SARC) });
                                if (FileFormat == null)
                                    continue;

                                if (FileFormat is SARC)
                                {
                                    string ArchiveFilePath = Path.Combine(folderDialog.SelectedPath, Path.GetFileNameWithoutExtension(file));
                                    ArchiveFilePath = Path.GetDirectoryName(ArchiveFilePath);
                                    if (!Directory.Exists(ArchiveFilePath))
                                        Directory.CreateDirectory(ArchiveFilePath);

                                    SearchBinary(FileFormat, ArchiveFilePath, Extension);
                                }
                                else
                                    SearchBinary(FileFormat, folderDialog.SelectedPath, Extension);
                            }
                        }
                    }
                }
            }

            private void SearchBinary(IFileFormat FileFormat, string Folder, string Extension)
            {
                if (FileFormat is SARC)
                {
                    string ArchiveFilePath = Path.Combine(Folder, Path.GetFileNameWithoutExtension(FileFormat.FileName));
                    if (!Directory.Exists(ArchiveFilePath))
                        Directory.CreateDirectory(ArchiveFilePath);

                    foreach (var file in ((SARC)FileFormat).Files)
                    {
                        var archiveFile = STFileLoader.OpenFileFormat(file.FileName, new Type[] { typeof(BNTX), typeof(BXFNT), typeof(BFRES), typeof(PTCL), typeof(SARC) }, file.FileData);
                        if (archiveFile == null)
                            continue;

                        SearchBinary(archiveFile, ArchiveFilePath, Extension);
                    }
                }
                if (FileFormat is BNTX)
                {
                    foreach (var texture in ((BNTX)FileFormat).Textures.Values)
                        texture.Export(Path.Combine(Folder, $"{texture.Text}{Extension}"));
                }
                if (FileFormat is BFRES)
                {
                    var bntx = ((BFRES)FileFormat).GetBNTX;
                    if(bntx != null)
                    {
                        foreach (var texture in bntx.Textures.Values)
                            texture.Export(Path.Combine(Folder, $"{texture.Text}{Extension}"));

                        bntx.Unload();
                    }
                }
                if (FileFormat is PTCL)
                {
                    var bntx = ((PTCL)FileFormat).header.BinaryTextureFile;
                    if (bntx != null)
                    {
                        foreach (var texture in bntx.Textures.Values)
                            texture.Export(Path.Combine(Folder, $"{texture.Text}{Extension}"));

                        bntx.Unload();
                    }
                }
                if (FileFormat is BXFNT)
                {
                    var bntx = ((BXFNT)FileFormat).bffnt.FontSection.TextureGlyph.BinaryTextureFile;
                    if (bntx != null)
                    {
                        foreach (var texture in bntx.Textures.Values)
                            texture.Export(Path.Combine(Folder, $"{texture.Text}{Extension}"));

                        bntx.Unload();
                    }
                }

                FileFormat.Unload();
                GC.Collect();
            }
        }

        public Dictionary<string, TextureData> Textures;

        public PropertieGridData prop;
        public BntxFile BinaryTexFile;

        public bool CanReplace;
        public bool AllGLInitialized
        {
            get
            {
                if (Textures.Any(item => item.Value.RenderableTex == null || item.Value.RenderableTex.GLInitialized == false))
                    return false;
                else
                    return true;
            }
        }

        public void Load(Stream stream)
        {
            if (IFileInfo == null)
                IFileInfo = new IFileInfo();

            IFileInfo.UseEditMenu = true;
            CanSave = true;

            Text = FileName;

            if (!IsLoadingArray)
                LoadBNTXArray(ContainerArray, stream, this);

            if (ContainerArray.Count == 0)
                LoadFile(stream, Name);

            if (!PluginRuntime.bntxContainers.Contains(this))
                PluginRuntime.bntxContainers.Add(this);
        }

        static bool IsLoadingArray = false;
        static bool IsSavingArray = false;
        List<BNTX> ContainerArray = new List<BNTX>();
        private static void LoadBNTXArray(List<BNTX> Containers, Stream stream, BNTX bntx)
        {
            IsLoadingArray = true;

            int Alignment = 4096;
            using (var reader = new FileReader(stream, true))
            {
                if (HasArray(reader, Alignment))
                {
                    reader.Position = 0;
                    SearchForBinaryContainerFile(reader, Alignment, Containers);
                }

                reader.Position = 0;
            }

            foreach (var container in Containers)
            {
                foreach (var texture in container.Textures.Values)
                    bntx.Nodes.Add(texture);
            }


            IsLoadingArray = false;
        }

        private static void SaveBNTXArray(Stream stream, List<BNTX> Containers)
        {
            IsSavingArray = true;

            int Alignment = 4096;
            using (var saver = new FileWriter(stream, true))
            {
                foreach (var container in Containers)
                {
                    var mem = new System.IO.MemoryStream();
                    container.Save(mem);
                    saver.Write(mem.ToArray());
                    saver.Align(Alignment);
                }
            }

            IsSavingArray = false;
        }

        private static bool HasArray(FileReader reader, int Alignment)
        {
            long StartPos = reader.Position;
            uint TotalSize = (uint)reader.BaseStream.Length;

            //Get file size in header
            reader.SeekBegin(StartPos + 28);
            uint FileSize = reader.ReadUInt32();

            reader.SeekBegin(StartPos + FileSize);
            if (TotalSize > FileSize)
            {
                //Align the reader and check for another bntx
                reader.Align(Alignment);
                if (reader.Position < TotalSize - 4)
                {
                    string Magic = reader.ReadString(4);
                    reader.Position = StartPos;

                    return (Magic == "BNTX");
                }
            }

            return false;
        }

        private static void SearchForBinaryContainerFile(FileReader reader, int Alignment, List<BNTX> Containers)
        {
            long StartPos = reader.Position;

            Console.WriteLine("StartPos " + StartPos);

            uint TotalSize = (uint)reader.BaseStream.Length;

            Console.WriteLine("TotalSize " + TotalSize);

            //Get file size in header
            reader.SeekBegin(StartPos + 28);
            uint FileSize = reader.ReadUInt32();

            Console.WriteLine("FileSize " + FileSize);

            //Create bntx for array
            BNTX bntx = new BNTX();

            bntx.IFileInfo = new IFileInfo();
            bntx.Text = "Sheet " + Containers.Count;

            //Get total bntx in bytes
            reader.SeekBegin(StartPos);
            byte[] Data = reader.ReadBytes((int)FileSize);
            bntx.Load(new MemoryStream(Data));
            Containers.Add(bntx);

            Console.WriteLine("File Container " + bntx.Text + " " + StartPos);

            reader.SeekBegin(StartPos + FileSize);
            if (TotalSize > FileSize)
            {
                //Align the reader and check for another bntx
                reader.Align(Alignment);
                if (reader.Position < TotalSize - 4)
                {
                    long NextPos = reader.Position;
                    string Magic = reader.ReadString(4);
                    if (Magic == "BNTX")
                    {
                        reader.Position = NextPos;

                        SearchForBinaryContainerFile(reader, Alignment, Containers);
                    }
                }
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem("Export", null, Save, Keys.Control | Keys.E),
                new ToolStripMenuItem("Replace", null, Import, Keys.Control | Keys.R) { Enabled = Parent != null, },
                new ToolStripSeparator(),
                new ToolStripMenuItem("Import Texture", null, ImportTextureAction, Keys.Control | Keys.I),
                new ToolStripMenuItem("Replace Textures (From Folder)", null, ReplaceAll, Keys.Control | Keys.T),
                new ToolStripMenuItem("Export All Textures", null, ExportAll, Keys.Control | Keys.A),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Rename", null, Rename, Keys.Control | Keys.N),
                new ToolStripMenuItem("Sort", null, SortTextures, Keys.Control | Keys.S),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Clear", null, Clear, Keys.Control | Keys.C),
            };
        }

        public void Unload()
        {
            if (ContainerArray.Count > 1)
            {
                for (int i = 0; i < ContainerArray.Count; i++)
                {
                    foreach (var tex in ContainerArray[i].Textures.Values)
                    {
                        tex.Texture.TextureData.Clear();
                        tex.Texture = null;
                        tex.DisposeRenderable();
                    }

                    ContainerArray[i].Textures.Clear();

                    if (PluginRuntime.bntxContainers.Contains(ContainerArray[i]))
                        PluginRuntime.bntxContainers.Remove(ContainerArray[i]);
                }

                GC.SuppressFinalize(this);
            }
            else
            {
                foreach (var tex in Textures.Values)
                {
                    tex.Texture.TextureData.Clear();
                    tex.Texture = null;
                    tex.DisposeRenderable();
                }

                Textures.Clear();
                if (this.IsVisible)
                    Nodes.Clear();

                this.BinaryTexFile = null;

                if (PluginRuntime.bntxContainers.Contains(this))
                    PluginRuntime.bntxContainers.Remove(this);

                GC.SuppressFinalize(this);
            }
        }

        public static BNTX CreateBNTXFromTexture(string filePath)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);

            BNTX bntx = new BNTX();
            bntx.IFileInfo = new IFileInfo();
            bntx.Load(new MemoryStream(CreateNewBNTX(name)));
            bntx.ImportTexture(new string[] { filePath });
            return bntx;
        }

        public static byte[] CreateNewBNTX(string Name)
        {
            MemoryStream mem = new MemoryStream();

            BntxFile bntx = new BntxFile();
            bntx.Target = new char[] { 'N', 'X', ' ', ' ' };
            bntx.Name = Name;
            bntx.Alignment = 0xC;
            bntx.TargetAddressSize = 0x40;
            bntx.VersionMajor = 0;
            bntx.VersionMajor2 = 4;
            bntx.VersionMinor = 0;
            bntx.VersionMinor2 = 0;
            bntx.Textures = new List<Texture>();
            bntx.TextureDict = new ResDict();
            bntx.RelocationTable = new RelocationTable();
            bntx.Flag = 0;
            bntx.Save(mem);
            var data = mem.ToArray();

            mem.Close();
            mem.Dispose();
            return data;
        }
        public void RemoveTexture(TextureData textureData)
        {
            Nodes.Remove(textureData);
            Textures.Remove(textureData.Text);
            LibraryGUI.UpdateViewport();
        }

        public override UserControl GetEditor()
        {
            STPropertyGrid editor = new STPropertyGrid();
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            return editor;
        }

        public override void FillEditor(UserControl control)
        {
            ((STPropertyGrid)control).LoadProperty(BinaryTexFile, OnPropertyChanged);
        }

        public override void OnClick(TreeView treeView) {
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (Parent != null && Parent is BFRES)
            {
                ((BFRES)Parent).LoadEditors(this);
                return;
            }

            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;

            if (ContainerArray.Count > 0)
                editor.LoadProperty(ContainerArray[0].BinaryTexFile, OnPropertyChanged);
            else
                editor.LoadProperty(BinaryTexFile, OnPropertyChanged);
        }

        public void OnPropertyChanged(){ Text = BinaryTexFile.Name; }

        public void LoadFile(Stream stream, string Name = "")
        {
            Textures = new Dictionary<string, TextureData>(StringComparer.InvariantCultureIgnoreCase);
            TextureList = new List<STGenericTexture>();

            if (FilePosition != 0)
                stream = new SubStream(stream, FilePosition, stream.Length - FilePosition);

            BinaryTexFile = new BntxFile(stream);
            Text = BinaryTexFile.Name;

            //Dispose previous entries if bntx is being replaced or reloaded
            foreach (var tex in Textures.Values)
            {
                tex.Texture.TextureData.Clear();
                tex.Texture = null;
                tex.DisposeRenderable();
            }

            Nodes.Clear();
            Textures.Clear();

            foreach (Texture tex in BinaryTexFile.Textures)
            {
                TextureData texData = new TextureData(tex, BinaryTexFile);
                texData.ParentBNTX = this;

                Nodes.Add(texData);
                if (!Textures.ContainsKey(tex.Name))
                    Textures.Add(tex.Name, texData);
            }
            BinaryTexFile.Textures.Clear(); //We don't need these in memeory anymore
            BinaryTexFile.TextureDict.Clear();

        }
        private void ImportTextureAction(object sender, EventArgs args) {
            ImportTexture();
        }
        public List<TextureData> ImportTexture()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileFilters.GetFilter(typeof(TextureData));

            ofd.DefaultExt = "png";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK) {
               return ImportTexture(ofd.FileNames);
            }
            return new List<TextureData>();
        }

        public List<TextureData> ImportTexture(string[] FileNames)
        {
            List<TextureData> textures = new List<TextureData>();

            BinaryTextureImporterList importer = new BinaryTextureImporterList();
            List<TextureImporterSettings> settings = new List<TextureImporterSettings>();

            foreach (string name in FileNames)
            {
                string ext = Path.GetExtension(name);
                ext = ext.ToLower();

                if (ext == ".dds" || ext == ".bftex" || ext == ".astc")
                {
                    textures.Add(AddTexture(name));
                }
                else
                {
                    settings.Add(LoadSettings(name));
                }
            }
            if (settings.Count == 0)
            {
                importer.Dispose();
                return textures;
            }

            importer.LoadSettings(settings);
            if (importer.ShowDialog() == DialogResult.OK) {
                textures.AddRange(ImportTexture(settings, importer.CompressionMode));
            }
            settings.Clear();
            GC.Collect();
            Cursor.Current = Cursors.Default;

            return textures;
        }

        public void ImportTexture(ImageKeyFrame[] Keys, string TextureName)
        {
            BinaryTextureImporterList importer = new BinaryTextureImporterList();
            List<TextureImporterSettings> settings = new List<TextureImporterSettings>();

            foreach (ImageKeyFrame key in Keys) {
                settings.Add(LoadSettings(key.Image, $"{TextureName}{key.Frame}"));
            }

            if (settings.Count == 0) {
                importer.Dispose();
                return;
            }

            importer.LoadSettings(settings);
            if (importer.ShowDialog() == DialogResult.OK)
            {
                ImportTexture(settings, importer.CompressionMode);
            }
            settings.Clear();
            GC.Collect();
            Cursor.Current = Cursors.Default;
        }

        private List<TextureData> ImportTexture(List<TextureImporterSettings> settings, STCompressionMode CompressionMode)
        {
            List<TextureData> textures = new List<TextureData>();

            Cursor.Current = Cursors.WaitCursor;
            foreach (var setting in settings)
            {
                if (setting.GenerateMipmaps && !setting.IsFinishedCompressing)
                {
                    setting.DataBlockOutput.Clear();
                    setting.DataBlockOutput.Add(setting.GenerateMips(CompressionMode, false));
                }
                if (setting.DataBlockOutput.Count <= 0)
                    throw new Exception("Data block is empty! Failed to compress!");

                if (setting.DataBlockOutput != null)
                {
                    Texture tex = setting.FromBitMap(setting.DataBlockOutput, setting);
                    if (setting.textureData == null)
                        setting.textureData = new TextureData(tex, BinaryTexFile);

                    int i = 0;
                    if (Textures.ContainsKey(setting.textureData.Text))
                    {
                        setting.textureData.Text = setting.textureData.Text + i++;
                    }

                    setting.textureData.ParentBNTX = this;

                    Nodes.Add(setting.textureData);
                    Textures.Add(setting.textureData.Text, setting.textureData);
                    textures.Add(setting.textureData);

                    setting.textureData.LoadOpenGLTexture();
                    LibraryGUI.UpdateViewport();
                }
                else
                {
                    MessageBox.Show("Something went wrong???");
                }
            }
            return textures;
        }

        //This function is an optional feature that will import a dummy texture if one is missing in the materials
        public void ImportPlaceholderTexture(string TextureName)
        {
            if (Textures.ContainsKey(TextureName))
                return;

            if (TextureName == "Basic_Alb")
                ImportBasicTextures("Basic_Alb");
            else if (TextureName == "Basic_Nrm")
                ImportBasicTextures("Basic_Nrm");
            else if (TextureName == "Basic_Spm")
                ImportBasicTextures("Basic_Spm");
            else if (TextureName == "Basic_Sphere")
                ImportBasicTextures("Basic_Sphere");
            else if (TextureName == "Basic_Mtl")
                ImportBasicTextures("Basic_Mtl");
            else if (TextureName == "Basic_Rgh")
                ImportBasicTextures("Basic_Rgh");
            else if (TextureName == "Basic_MRA")
                ImportBasicTextures("Basic_MRA");
            else if (TextureName == "Basic_Bake_st0")
                ImportBasicTextures("Basic_Bake_st0");
            else if (TextureName == "Basic_Bake_st1")
                ImportBasicTextures("Basic_Bake_st1");
            else if (TextureName == "Basic_Emm")
                ImportBasicTextures("Basic_Emm");
            else
            {
                ImportPlaceholderTexture(Properties.Resources.InjectTexErrored, TextureName);
            }
        }
        private void ImportPlaceholderTexture(byte[] data, string TextureName)
        {
            TextureImporterSettings importDDS = new TextureImporterSettings();
            importDDS.LoadDDS(TextureName, data);

            Texture tex = importDDS.FromBitMap(importDDS.DataBlockOutput, importDDS);
            TextureData texData = new TextureData(tex, BinaryTexFile);

            texData.Text = TextureName;


            texData.ParentBNTX = this;

            Nodes.Add(texData);
            Textures.Add(TextureName, texData);
            texData.LoadOpenGLTexture();
        }
        public void ImportBasicTextures(string TextureName, bool BC5Nrm = true)
        {
            if (Textures.ContainsKey(TextureName))
                return;

            if (TextureName == "Basic_Alb")
                ImportPlaceholderTexture(Properties.Resources.InjectTexErrored, TextureName);
            if (TextureName == "Basic_Nrm" && BC5Nrm)
                ImportPlaceholderTexture(Properties.Resources.Basic_NrmBC5, TextureName);
            if (TextureName == "Basic_Nrm" && BC5Nrm == false)
                ImportPlaceholderTexture(Properties.Resources.Basic_Nrm, TextureName);
            if (TextureName == "Basic_Spm")
                ImportPlaceholderTexture(Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Sphere")
                ImportPlaceholderTexture(Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Mtl")
                ImportPlaceholderTexture(Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Rgh")
                ImportPlaceholderTexture(Properties.Resources.White, TextureName);
            if (TextureName == "Basic_MRA")
                ImportPlaceholderTexture(Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Bake_st0")
                ImportPlaceholderTexture(Properties.Resources.Basic_Bake_st0, TextureName);
            if (TextureName == "Basic_Bake_st1")
                ImportPlaceholderTexture(Properties.Resources.Basic_Bake_st1, TextureName);
        }

        public TextureImporterSettings LoadSettings(Image image, string Name)
        {
            var importer = new TextureImporterSettings();
            importer.LoadBitMap(image, Name);
            return importer;
        }

        public TextureImporterSettings LoadSettings(string name)
        {
            var importer = new TextureImporterSettings();

            string ext = Path.GetExtension(name);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bftex":
                    Texture tex = new Texture();
                    tex.Import(name);
                    break;
                case ".dds":
                    importer.LoadDDS(name);
                    break;
                default:
                    importer.LoadBitMap(name);
                    break;
            }

            return importer;
        }
        public TextureData AddTexture(string name)
        {
            var importer = new TextureImporterSettings();

            Texture tex = null;
            TextureData texData = null;
            string ext = Path.GetExtension(name);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bftex":
                    tex = new Texture();
                    tex.Import(name);
                    texData = new TextureData(tex, BinaryTexFile);
                    break;
                case ".dds":
                case ".dds2":
                    importer.LoadDDS(name);
                    tex = importer.FromBitMap(importer.DataBlockOutput, importer);
                    texData = new TextureData(tex, BinaryTexFile);
                    break;
                case ".astc":
                    importer.LoadASTC(name);
                    tex = importer.FromBitMap(importer.DataBlockOutput, importer);
                    texData = new TextureData(tex, BinaryTexFile);
                    break;
                default:
                    break;
            }
            if (texData != null)
            {
                List<string> keyList = new List<string>(Textures.Keys);
                texData.Text = Utils.RenameDuplicateString(keyList, texData.Text);
                texData.Load(texData.Texture);
                Nodes.Add(texData);
                texData.ParentBNTX = this;
                Textures.Add(texData.Text, texData);
            }
            return texData;
        }
        private void Clear(object sender, EventArgs args)
        {
            var result = MessageBox.Show("Are you sure you want to clear this section? This cannot be undone!",
                          "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (var tex in Textures.Values)
                {
                    tex.DisposeRenderable();
                    tex.Texture.TextureData.Clear();
                    tex.Texture = null;
                }

                Textures.Clear();
                Nodes.Clear();

                UpdateEditor();

                GC.Collect();
            }
        }


        private void ReplaceAll(object sender, EventArgs args) {
            ReplaceAll(new BNTX[] { this });
        }

        public static void ReplaceAll(BNTX[] bntxFiles)
        {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                BinaryTextureImporterList importer = new BinaryTextureImporterList();
                List<TextureImporterSettings> settings = new List<TextureImporterSettings>();
                List<TextureData> TexturesForImportSettings = new List<TextureData>();

                foreach (string file in System.IO.Directory.GetFiles(sfd.SelectedPath))
                {
                    TextureImporterSettings setting = new TextureImporterSettings();

                    string FileName = System.IO.Path.GetFileNameWithoutExtension(file);

                    string ext = Utils.GetExtension(file);

                    foreach (var bntx in bntxFiles)
                    {
                        foreach (TextureData node in bntx.Textures.Values)
                        {
                            var DefaultFormat = node.Format;

                            if (FileName == node.Text)
                            {
                                switch (ext)
                                {
                                    case ".bftex":
                                        node.Texture.Import(file);
                                        node.LoadOpenGLTexture();
                                        break;
                                    case ".dds":
                                    case ".dds2":
                                        setting.LoadDDS(file, null, node);
                                        node.ApplyImportSettings(setting, STCompressionMode.Normal, false);
                                        break;
                                    case ".astc":
                                        setting.LoadASTC(file);
                                        node.ApplyImportSettings(setting, STCompressionMode.Normal, false);
                                        break;
                                    case ".png":
                                    case ".tga":
                                    case ".tiff":
                                    case ".gif":
                                    case ".jpg":
                                    case ".jpeg":
                                        TexturesForImportSettings.Add(node);
                                        setting.LoadBitMap(file);
                                        importer.LoadSetting(setting);
                                        if (!STGenericTexture.IsAtscFormat(DefaultFormat))
                                            setting.Format = TextureData.GenericToBntxSurfaceFormat(DefaultFormat);

                                        settings.Add(setting);
                                        break;
                                    default:
                                        return;
                                }
                            }
                        }
                    }
                }

                if (settings.Count == 0)
                {
                    importer.Close();
                    importer.Dispose();
                    return;
                }

                int settingsIndex = 0;
                if (importer.ShowDialog() == DialogResult.OK)
                {
                    foreach (TextureData node in TexturesForImportSettings)
                        node.ApplyImportSettings(settings[settingsIndex++], importer.CompressionMode, importer.MultiThreading);
                }

                TexturesForImportSettings.Clear();
            }
        }

        private void ExportAll(object sender, EventArgs args)
        {
            List<string> Formats = new List<string>();
            Formats.Add("Cafe Binary Textures (.bftex)");
            Formats.Add("Microsoft DDS (.dds)");
            Formats.Add("Portable Graphics Network (.png)");
            Formats.Add("Joint Photographic Experts Group (.jpg)");
            Formats.Add("Bitmap Image (.bmp)");
            Formats.Add("Tagged Image File Format (.tiff)");

            FolderSelectDialog sfd = new FolderSelectDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (TextureData tex in Nodes)
                    {
                        if (form.Index == 0)
                            tex.SaveBinaryTexture(folderPath + '\\' + tex.Text + ".bftex");
                        else if (form.Index == 1)
                            tex.SaveDDS(folderPath + '\\' + tex.Text + ".dds");
                        else if (form.Index == 2)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".png");
                        else if (form.Index == 3)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".jpg");
                        else if (form.Index == 4)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".bmp");
                        else if (form.Index == 5)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".tiff");
                    }
                }
            }
        }

        bool SortedAscending;
        private void SortTextures(object sender, EventArgs args)
        {
        }

        public void Save(System.IO.Stream stream)
        {
            if (ContainerArray.Count > 0 && !IsSavingArray)
            {
                SaveBNTXArray(stream, ContainerArray);
            }
            else
            {
                BinaryTexFile.Textures.Clear();
                BinaryTexFile.TextureDict.Clear();

                foreach (TextureData tex in Textures.Values)
                {
                    if (tex.IsEdited)
                    {
                        for (int i = 0; i < tex.EditedImages.Length; i++)
                        {
                            tex.SetImageData(tex.EditedImages[i].bitmap, tex.EditedImages[i].ArrayLevel);
                            tex.EditedImages[i].bitmap.Dispose();
                        }
                        tex.EditedImages = new EditedBitmap[0];
                    }

                    tex.Texture.Name = tex.Text;


                    BinaryTexFile.Textures.Add(tex.Texture);
                    BinaryTexFile.TextureDict.Add(tex.Text);
                }

                if (ExtraHeader != null)
                {
                    var mem = new MemoryStream();
                    BinaryTexFile.Save(mem);
                    using (var writer = new FileWriter(stream, true))
                    {
                        writer.Write(ExtraHeader);
                        writer.Write(mem.ToArray());
                    }
                }
                else
                    BinaryTexFile.Save(stream);
            }
        }

        public class PropertieGridData
        {
            [Browsable(true)]
            [Category("BNTX")]
            [DisplayName("Name")]
            public string Name { get; set; }

            [Browsable(true)]
            [Category("BNTX")]
            [DisplayName("Original Path")]
            public string Path { get; set; }

            [Browsable(true)]
            [Category("BNTX")]
            [DisplayName("Target")]
            public string Target { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Category("Versions")]
            [DisplayName("Full Version")]
            public string VersionFull { get; set; }

            [Browsable(true)]
            [Category("Versions")]
            [DisplayName("Version Major 1")]
            public uint VersionMajor { get; set; }

            [Browsable(true)]
            [Category("Versions")]
            [DisplayName("Version Major 2")]
            public uint VersionMajor2 { get; set; }

            [Browsable(true)]
            [Category("Versions")]
            [DisplayName("Version Minor 1")]
            public uint VersionMinor { get; set; }

            [Browsable(true)]
            [Category("Versions")]
            [DisplayName("Version Minor 2")]
            public uint VersionMinor2 { get; set; }
        }

        private void Import(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "BNTX |*.bntx;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadFile(new MemoryStream(File.ReadAllBytes(ofd.FileName)));
            }
        }

        private void Rename(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Text = dialog.textBox1.Text;
                if (BinaryTexFile != null)
                    BinaryTexFile.Name = Text;
            }
        }
        private void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "bntx";
            sfd.Filter = "Supported Formats|*.bntx;";
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }
    }

    public class TextureData : STGenericTexture
    {
        public BNTX ParentBNTX { get; set; }

        public EventHandler OnTextureDeleted;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.BC1_UNORM,
                    TEX_FORMAT.BC1_UNORM_SRGB,
                    TEX_FORMAT.BC2_UNORM,
                    TEX_FORMAT.BC2_UNORM_SRGB,
                    TEX_FORMAT.BC3_UNORM,
                    TEX_FORMAT.BC3_UNORM_SRGB,
                    TEX_FORMAT.BC4_UNORM,
                    TEX_FORMAT.BC4_SNORM,
                    TEX_FORMAT.BC5_UNORM,
                    TEX_FORMAT.BC5_SNORM,
                    TEX_FORMAT.BC6H_UF16,
                    TEX_FORMAT.BC6H_SF16,
                    TEX_FORMAT.BC7_UNORM,
                    TEX_FORMAT.BC7_UNORM_SRGB,
                    TEX_FORMAT.D32_FLOAT_S8X24_UINT,
                    TEX_FORMAT.B5G5R5A1_UNORM,
                    TEX_FORMAT.B5G6R5_UNORM,
                    TEX_FORMAT.B8G8R8A8_UNORM_SRGB,
                    TEX_FORMAT.B8G8R8A8_UNORM,
                    TEX_FORMAT.R10G10B10A2_UNORM,
                    TEX_FORMAT.R16_UNORM,
                    TEX_FORMAT.B4G4R4A4_UNORM,
                    TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                    TEX_FORMAT.R8G8B8A8_UNORM,
                    TEX_FORMAT.R8_UNORM,
                    TEX_FORMAT.R16_UINT,
                    TEX_FORMAT.R8G8_UNORM,
                    TEX_FORMAT.R32G8X24_FLOAT,
                    TEX_FORMAT.ASTC_10x10_SRGB,
                    TEX_FORMAT.ASTC_10x10_UNORM,
                    TEX_FORMAT.ASTC_10x5_SRGB,
                    TEX_FORMAT.ASTC_10x5_UNORM,
                    TEX_FORMAT.ASTC_10x6_SRGB,
                    TEX_FORMAT.ASTC_10x6_UNORM,
                    TEX_FORMAT.ASTC_10x8_SRGB,
                    TEX_FORMAT.ASTC_10x8_UNORM,
                    TEX_FORMAT.ASTC_12x10_SRGB,
                    TEX_FORMAT.ASTC_12x10_UNORM,
                    TEX_FORMAT.ASTC_12x12_SRGB,
                    TEX_FORMAT.ASTC_12x12_UNORM,
                    TEX_FORMAT.ASTC_4x4_SRGB,
                    TEX_FORMAT.ASTC_4x4_UNORM,
                    TEX_FORMAT.ASTC_5x4_SRGB,
                    TEX_FORMAT.ASTC_5x4_UNORM,
                    TEX_FORMAT.ASTC_5x5_SRGB,
                    TEX_FORMAT.ASTC_5x5_UNORM,
                    TEX_FORMAT.ASTC_6x5_SRGB,
                    TEX_FORMAT.ASTC_6x5_UNORM,
                    TEX_FORMAT.ASTC_6x6_SRGB,
                    TEX_FORMAT.ASTC_6x6_UNORM,
                    TEX_FORMAT.ASTC_8x5_SRGB,
                    TEX_FORMAT.ASTC_8x5_UNORM,
                    TEX_FORMAT.ASTC_8x6_SRGB,
                    TEX_FORMAT.ASTC_8x6_UNORM,
                    TEX_FORMAT.ASTC_8x8_SRGB,
                    TEX_FORMAT.ASTC_8x8_UNORM,
                };
            }
        }

        public override bool CanEdit { get; set; } = true;

        public Texture Texture;

        public TextureData()
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }
        public TextureData(Texture tex, BntxFile bntx)
        {
            Load(tex);
        }
        public void Load(Texture tex, int target = 1)
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";
            CanExport = true;
            CanRename = true;
            CanReplace = true;
            CanDelete = true;

            Texture = tex;

            Text = tex.Name;
            Width = tex.Width;
            Height = tex.Height;
            Depth = tex.Depth;
            ArrayCount = (uint)tex.TextureData.Count;
            MipCount = (uint)tex.TextureData[0].Count;
            Format = ConvertFormat(tex.Format);

            RedChannel = SetChannel(tex.ChannelRed);
            GreenChannel = SetChannel(tex.ChannelGreen);
            BlueChannel = SetChannel(tex.ChannelBlue);
            AlphaChannel = SetChannel(tex.ChannelAlpha);
        }

        private STChannelType SetChannel(ChannelType channelType)
        {
            if (channelType == ChannelType.Red) return STChannelType.Red;
            else if (channelType == ChannelType.Green) return STChannelType.Green;
            else if (channelType == ChannelType.Blue) return STChannelType.Blue;
            else if (channelType == ChannelType.Alpha) return STChannelType.Alpha;
            else if (channelType == ChannelType.Zero) return STChannelType.Zero;
            else return STChannelType.One;
        }

        public static SurfaceFormat GenericToBntxSurfaceFormat(TEX_FORMAT texFormat)
        {
            switch (texFormat)
            {
                case TEX_FORMAT.BC1_UNORM: return SurfaceFormat.BC1_UNORM;
                case TEX_FORMAT.BC1_UNORM_SRGB: return SurfaceFormat.BC1_SRGB;
                case TEX_FORMAT.BC2_UNORM: return SurfaceFormat.BC2_UNORM;
                case TEX_FORMAT.BC2_UNORM_SRGB: return SurfaceFormat.BC2_SRGB;
                case TEX_FORMAT.BC3_UNORM: return SurfaceFormat.BC3_UNORM;
                case TEX_FORMAT.BC3_UNORM_SRGB: return SurfaceFormat.BC3_SRGB;
                case TEX_FORMAT.BC4_UNORM: return SurfaceFormat.BC4_UNORM;
                case TEX_FORMAT.BC4_SNORM: return SurfaceFormat.BC4_SNORM;
                case TEX_FORMAT.BC5_UNORM: return SurfaceFormat.BC5_UNORM;
                case TEX_FORMAT.BC5_SNORM: return SurfaceFormat.BC5_SNORM;
                case TEX_FORMAT.BC6H_UF16: return SurfaceFormat.BC6_UFLOAT;
                case TEX_FORMAT.BC6H_SF16: return SurfaceFormat.BC6_FLOAT;
                case TEX_FORMAT.BC7_UNORM: return SurfaceFormat.BC7_UNORM;
                case TEX_FORMAT.BC7_UNORM_SRGB: return SurfaceFormat.BC7_SRGB;
                case TEX_FORMAT.R5G5B5A1_UNORM: return SurfaceFormat.R5_G5_B5_A1_UNORM;
                case TEX_FORMAT.B5G5R5A1_UNORM: return SurfaceFormat.B5_G5_R5_A1_UNORM; 
                case TEX_FORMAT.B5G6R5_UNORM: return SurfaceFormat.B5_G6_R5_UNORM; 
                case TEX_FORMAT.B8G8R8A8_UNORM_SRGB: return SurfaceFormat.B8_G8_R8_A8_SRGB; 
                case TEX_FORMAT.B8G8R8A8_UNORM: return SurfaceFormat.B8_G8_R8_A8_UNORM; 
                case TEX_FORMAT.R10G10B10A2_UNORM: return SurfaceFormat.R10_G10_B10_A2_UNORM;
                case TEX_FORMAT.R16_UNORM: return SurfaceFormat.R16_UNORM; 
                case TEX_FORMAT.B4G4R4A4_UNORM: return SurfaceFormat.R4_G4_B4_A4_UNORM;
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return SurfaceFormat.R8_G8_B8_A8_SRGB; 
                case TEX_FORMAT.R8G8B8A8_UNORM: return SurfaceFormat.R8_G8_B8_A8_UNORM;
                case TEX_FORMAT.R8G8B8A8_SNORM: return SurfaceFormat.R8_G8_B8_A8_SNORM;
                case TEX_FORMAT.R8_UNORM: return SurfaceFormat.R8_UNORM;
                case TEX_FORMAT.R16_UINT: return SurfaceFormat.R16_UINT;
                case TEX_FORMAT.R8G8_UNORM: return SurfaceFormat.R8_G8_UNORM;
                case TEX_FORMAT.R8G8_SNORM: return SurfaceFormat.R8_G8_SNORM;
                case TEX_FORMAT.D32_FLOAT_S8X24_UINT: return SurfaceFormat.D32_FLOAT_S8X24_UINT;
                case TEX_FORMAT.B8G8R8X8_UNORM: return SurfaceFormat.B8_G8_R8_A8_UNORM; //Todo
                case TEX_FORMAT.ETC1_UNORM: return SurfaceFormat.ETC1_UNORM;
                case TEX_FORMAT.ETC1_SRGB: return SurfaceFormat.ETC1_SRGB;
                case TEX_FORMAT.ASTC_10x10_SRGB: return SurfaceFormat.ASTC_10x10_SRGB;
                case TEX_FORMAT.ASTC_10x10_UNORM: return SurfaceFormat.ASTC_10x10_UNORM;
                case TEX_FORMAT.ASTC_10x5_SRGB: return SurfaceFormat.ASTC_10x5_SRGB;
                case TEX_FORMAT.ASTC_10x5_UNORM: return SurfaceFormat.ASTC_10x5_UNORM;
                case TEX_FORMAT.ASTC_10x6_SRGB: return SurfaceFormat.ASTC_10x6_SRGB;
                case TEX_FORMAT.ASTC_10x6_UNORM: return SurfaceFormat.ASTC_10x6_UNORM;
                case TEX_FORMAT.ASTC_10x8_SRGB: return SurfaceFormat.ASTC_10x8_SRGB;
                case TEX_FORMAT.ASTC_10x8_UNORM: return SurfaceFormat.ASTC_10x8_UNORM;
                case TEX_FORMAT.ASTC_12x10_SRGB: return SurfaceFormat.ASTC_12x10_SRGB;
                case TEX_FORMAT.ASTC_12x10_UNORM: return SurfaceFormat.ASTC_12x10_UNORM;
                case TEX_FORMAT.ASTC_12x12_SRGB: return SurfaceFormat.ASTC_12x12_SRGB;
                case TEX_FORMAT.ASTC_12x12_UNORM: return SurfaceFormat.ASTC_12x12_UNORM;
                case TEX_FORMAT.ASTC_4x4_SRGB: return SurfaceFormat.ASTC_4x4_SRGB;
                case TEX_FORMAT.ASTC_4x4_UNORM: return SurfaceFormat.ASTC_4x4_UNORM;
                case TEX_FORMAT.ASTC_5x4_SRGB: return SurfaceFormat.ASTC_5x4_SRGB;
                case TEX_FORMAT.ASTC_5x4_UNORM: return SurfaceFormat.ASTC_5x4_UNORM;
                case TEX_FORMAT.ASTC_5x5_SRGB: return SurfaceFormat.ASTC_5x5_SRGB;
                case TEX_FORMAT.ASTC_5x5_UNORM: return SurfaceFormat.ASTC_5x5_UNORM;
                case TEX_FORMAT.ASTC_6x5_SRGB: return SurfaceFormat.ASTC_6x5_SRGB;
                case TEX_FORMAT.ASTC_6x5_UNORM: return SurfaceFormat.ASTC_6x5_UNORM;
                case TEX_FORMAT.ASTC_6x6_SRGB: return SurfaceFormat.ASTC_6x6_SRGB;
                case TEX_FORMAT.ASTC_6x6_UNORM: return SurfaceFormat.ASTC_6x6_UNORM;
                case TEX_FORMAT.ASTC_8x5_SRGB: return SurfaceFormat.ASTC_8x5_SRGB;
                case TEX_FORMAT.ASTC_8x5_UNORM: return SurfaceFormat.ASTC_8x5_UNORM;
                case TEX_FORMAT.ASTC_8x6_SRGB: return SurfaceFormat.ASTC_8x6_SRGB;
                case TEX_FORMAT.ASTC_8x6_UNORM: return SurfaceFormat.ASTC_8x6_UNORM;
                case TEX_FORMAT.ASTC_8x8_SRGB: return SurfaceFormat.ASTC_8x8_SRGB;
                case TEX_FORMAT.ASTC_8x8_UNORM: return SurfaceFormat.ASTC_8x8_UNORM;
                default:
                    throw new Exception($"Cannot convert format {texFormat}");
            }
        }
        public static TEX_FORMAT ConvertFormat(uint surfaceFormat)
        {
            return ConvertFormat((SurfaceFormat)surfaceFormat);
        }

        public static TEX_FORMAT ConvertFormat(SurfaceFormat surfaceFormat)
        {
            switch (surfaceFormat)
            {
                case SurfaceFormat.BC1_UNORM: return TEX_FORMAT.BC1_UNORM;
                case SurfaceFormat.BC1_SRGB: return TEX_FORMAT.BC1_UNORM_SRGB;
                case SurfaceFormat.BC2_UNORM: return TEX_FORMAT.BC2_UNORM;
                case SurfaceFormat.BC2_SRGB: return TEX_FORMAT.BC2_UNORM_SRGB;
                case SurfaceFormat.BC3_UNORM: return TEX_FORMAT.BC3_UNORM;
                case SurfaceFormat.BC3_SRGB: return TEX_FORMAT.BC3_UNORM_SRGB;
                case SurfaceFormat.BC4_UNORM: return TEX_FORMAT.BC4_UNORM;
                case SurfaceFormat.BC4_SNORM: return TEX_FORMAT.BC4_SNORM;
                case SurfaceFormat.BC5_UNORM: return TEX_FORMAT.BC5_UNORM;
                case SurfaceFormat.BC5_SNORM: return TEX_FORMAT.BC5_SNORM;
                case SurfaceFormat.BC6_UFLOAT: return TEX_FORMAT.BC6H_UF16;
                case SurfaceFormat.BC6_FLOAT: return TEX_FORMAT.BC6H_SF16;
                case SurfaceFormat.BC7_UNORM: return TEX_FORMAT.BC7_UNORM;
                case SurfaceFormat.BC7_SRGB: return TEX_FORMAT.BC7_UNORM_SRGB;
                case SurfaceFormat.A1_B5_G5_R5_UNORM: return TEX_FORMAT.B5G5R5A1_UNORM;
                case SurfaceFormat.A4_B4_G4_R4_UNORM: return TEX_FORMAT.B4G4R4A4_UNORM;
                case SurfaceFormat.B5_G5_R5_A1_UNORM: return TEX_FORMAT.B5G5R5A1_UNORM;
                case SurfaceFormat.B5_G6_R5_UNORM: return TEX_FORMAT.B5G6R5_UNORM;
                case SurfaceFormat.B8_G8_R8_A8_SRGB: return TEX_FORMAT.B8G8R8A8_UNORM_SRGB;
                case SurfaceFormat.B8_G8_R8_A8_UNORM: return TEX_FORMAT.B8G8R8A8_UNORM;
                case SurfaceFormat.R10_G10_B10_A2_UNORM: return TEX_FORMAT.R10G10B10A2_UNORM;
                case SurfaceFormat.R11_G11_B10_UINT: return TEX_FORMAT.R11G11B10_FLOAT;
                case SurfaceFormat.R4_G4_B4_A4_UNORM: return TEX_FORMAT.B4G4R4A4_UNORM;
                case SurfaceFormat.R4_G4_UNORM: return TEX_FORMAT.B4G4R4A4_UNORM;
                case SurfaceFormat.R5_G5_B5_A1_UNORM: return TEX_FORMAT.R5G5B5A1_UNORM;
                case SurfaceFormat.R5_G6_B5_UNORM: return TEX_FORMAT.B5G6R5_UNORM;
                case SurfaceFormat.R8_G8_B8_A8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                case SurfaceFormat.R8_G8_B8_A8_UNORM: return TEX_FORMAT.R8G8B8A8_UNORM;
                case SurfaceFormat.R8_G8_B8_A8_SNORM: return TEX_FORMAT.R8G8B8A8_SNORM;
                case SurfaceFormat.R8_UNORM: return TEX_FORMAT.R8_UNORM;
                case SurfaceFormat.R16_UNORM: return TEX_FORMAT.R16_UNORM;
                case SurfaceFormat.R16_SNORM: return TEX_FORMAT.R16_SNORM;
                case SurfaceFormat.R16_UINT: return TEX_FORMAT.R16_UINT;
                case SurfaceFormat.R16_SINT: return TEX_FORMAT.R16_SINT;
                case SurfaceFormat.R16_FLOAT: return TEX_FORMAT.R16_FLOAT;
                case SurfaceFormat.R8_G8_UNORM: return TEX_FORMAT.R8G8_UNORM;
                case SurfaceFormat.R8_G8_SNORM: return TEX_FORMAT.R8G8_SNORM;
                case SurfaceFormat.ETC1_UNORM: return TEX_FORMAT.ETC1_UNORM;
                case SurfaceFormat.ETC1_SRGB: return TEX_FORMAT.ETC1_SRGB;
                case SurfaceFormat.D32_FLOAT_S8X24_UINT: return TEX_FORMAT.D32_FLOAT_S8X24_UINT;
                case SurfaceFormat.ASTC_10x10_SRGB: return TEX_FORMAT.ASTC_10x10_SRGB;
                case SurfaceFormat.ASTC_10x10_UNORM: return TEX_FORMAT.ASTC_10x10_UNORM;
                case SurfaceFormat.ASTC_10x5_SRGB: return TEX_FORMAT.ASTC_10x5_SRGB;
                case SurfaceFormat.ASTC_10x5_UNORM: return TEX_FORMAT.ASTC_10x5_UNORM;
                case SurfaceFormat.ASTC_10x6_SRGB: return TEX_FORMAT.ASTC_10x6_SRGB;
                case SurfaceFormat.ASTC_10x6_UNORM: return TEX_FORMAT.ASTC_10x6_UNORM;
                case SurfaceFormat.ASTC_10x8_SRGB: return TEX_FORMAT.ASTC_10x8_SRGB;
                case SurfaceFormat.ASTC_10x8_UNORM: return TEX_FORMAT.ASTC_10x8_UNORM;
                case SurfaceFormat.ASTC_12x10_SRGB: return TEX_FORMAT.ASTC_12x10_SRGB;
                case SurfaceFormat.ASTC_12x10_UNORM: return TEX_FORMAT.ASTC_12x10_UNORM;
                case SurfaceFormat.ASTC_12x12_SRGB: return TEX_FORMAT.ASTC_12x12_SRGB;
                case SurfaceFormat.ASTC_12x12_UNORM: return TEX_FORMAT.ASTC_12x12_UNORM;
                case SurfaceFormat.ASTC_4x4_SRGB: return TEX_FORMAT.ASTC_4x4_SRGB;
                case SurfaceFormat.ASTC_4x4_UNORM: return TEX_FORMAT.ASTC_4x4_UNORM;
                case SurfaceFormat.ASTC_5x4_SRGB: return TEX_FORMAT.ASTC_5x4_SRGB;
                case SurfaceFormat.ASTC_5x4_UNORM: return TEX_FORMAT.ASTC_5x4_UNORM;
                case SurfaceFormat.ASTC_5x5_SRGB: return TEX_FORMAT.ASTC_5x5_SRGB;
                case SurfaceFormat.ASTC_5x5_UNORM: return TEX_FORMAT.ASTC_5x5_UNORM;
                case SurfaceFormat.ASTC_6x5_SRGB: return TEX_FORMAT.ASTC_6x5_SRGB;
                case SurfaceFormat.ASTC_6x5_UNORM: return TEX_FORMAT.ASTC_6x5_UNORM;
                case SurfaceFormat.ASTC_6x6_SRGB: return TEX_FORMAT.ASTC_6x6_SRGB;
                case SurfaceFormat.ASTC_6x6_UNORM: return TEX_FORMAT.ASTC_6x6_UNORM;
                case SurfaceFormat.ASTC_8x5_SRGB: return TEX_FORMAT.ASTC_8x5_SRGB;
                case SurfaceFormat.ASTC_8x5_UNORM: return TEX_FORMAT.ASTC_8x5_UNORM;
                case SurfaceFormat.ASTC_8x6_SRGB: return TEX_FORMAT.ASTC_8x6_SRGB;
                case SurfaceFormat.ASTC_8x6_UNORM: return TEX_FORMAT.ASTC_8x6_UNORM;
                case SurfaceFormat.ASTC_8x8_SRGB: return TEX_FORMAT.ASTC_8x8_SRGB;
                case SurfaceFormat.ASTC_8x8_UNORM: return TEX_FORMAT.ASTC_8x8_UNORM;

                case SurfaceFormat.Invalid: throw new Exception("Invalid Format");
                default:
                //Unsupported, skip for now
                     return TEX_FORMAT.R8G8B8A8_UNORM;

                    //    throw new Exception($"Cannot convert format {surfaceFormat}");
            }
        }
        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }

        private bool IsEditorActive()
        {
            BfresEditor bfresEditor = (BfresEditor)LibraryGUI.GetActiveContent(typeof(BfresEditor));
            if (bfresEditor != null)
            {
                var imageEditor = bfresEditor.GetActiveEditor(typeof(ImageEditorBase));
                return imageEditor != null;
            }
            else
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                return editor != null;
            }
        }


        public void UpdateEditor()
        {
            if (Parent != null && Parent.Parent != null && Parent.Parent is BFRES)
            {
                ((BFRES)Parent.Parent).LoadEditors(this);
                return;
            }

            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }
            if (Texture.UserData != null)
            {
                UserDataEditor userEditor = (UserDataEditor)editor.GetActiveTabEditor(typeof(UserDataEditor));
                if (userEditor == null)
                {
                    userEditor = new UserDataEditor();
                    userEditor.Name = "User Data";
                    editor.AddCustomControl(userEditor, typeof(UserDataEditor));
                }
                userEditor.LoadUserData(Texture.UserData.ToList());
            }

            editor.Text = Text;

            editor.LoadProperties(Texture);
            editor.LoadImage(this);
        }

        private void OnPropertyChanged()
        {
            Text = Texture.Name;

            RedChannel = SetChannel(Texture.ChannelRed);
            GreenChannel = SetChannel(Texture.ChannelGreen);
            BlueChannel = SetChannel(Texture.ChannelBlue);
            AlphaChannel = SetChannel(Texture.ChannelAlpha);
        }

        public override void Delete()
        {
            ParentBNTX.RemoveTexture(this);
            OnTextureDeleted?.Invoke(this, EventArgs.Empty);
        }

        public override void Rename()
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ParentBNTX.Textures.Remove(Text);
                Text = dialog.textBox1.Text;

                ParentBNTX.Textures.Add(Text, this);
            }
        }

        public override string ExportFilter => FileFilters.BNTX_TEX;
        public override string ReplaceFilter => FileFilters.BNTX_TEX;

        public override void Replace(string FileName) {
            Replace(FileName, MipCount, 0, Format,  Texture.SurfaceDim);
        }

        public override void Export(string FileName) {
            Export(FileName);
        }

        //Max mip level will be set automatically unless overwritten
        //The tex format can be adjusted in the function if necessary. Will normally be set to format in settings
        public void Replace(string FileName, uint MaxMipLevel = 0, uint ArrayIndex = 0,
            TEX_FORMAT DefaultFormat = TEX_FORMAT.BC1_UNORM_SRGB, SurfaceDim surfaceDim = SurfaceDim.Dim2D,
            int alignment = 512) 
        {
            Console.WriteLine("surfaceDim" + surfaceDim);

            string ext = Path.GetExtension(FileName);
            ext = ext.ToLower();

            TextureImporterSettings setting = new TextureImporterSettings();
            BinaryTextureImporterList importer = new BinaryTextureImporterList();

            setting.SurfaceDim = surfaceDim;
            setting.Alignment = alignment;
            //Check if alignment is required for combining individual levels or not
            setting.CombineMipLevel = alignment != 0 ? true : false;

            var ImageDataCached = new List<List<byte[]>>();
            if (Texture != null && Texture.TextureData != null)
            {
                foreach (var sliceData in Texture.TextureData)
                    ImageDataCached.Add(sliceData);
            }

            //Check if current texture is SRGB
            bool srgb = Texture != null && Texture.UseSRGB; 

            switch (ext)
            {
                case ".bftex":
                    Texture.Import(FileName);
                    Texture.Name = Text;
                    LoadOpenGLTexture();
                    break;
                case ".dds":
                    setting.LoadDDS(FileName, null, this);
                    ApplyImportSettings(setting, STCompressionMode.Normal, false);
                    break;
                case ".astc":
                    setting.LoadASTC(FileName);
                    //Keep original SRGB setting as .astc has no SRGB flags
                    ApplyImportSettings(setting, STCompressionMode.Normal, false, srgb);
                    break;
                default:
                    setting.LoadBitMap(FileName);
                    importer.LoadSetting(setting);
                    if (!IsAtscFormat(DefaultFormat))
                        setting.Format = GenericToBntxSurfaceFormat(DefaultFormat);

                    if (MaxMipLevel != 0)
                    {
                        importer.ForceMipCount = true;
                        setting.MipCount = MaxMipLevel;
                        importer.SelectedMipCount = MaxMipLevel;
                    }

                    if (importer.ShowDialog() == DialogResult.OK)
                    {
                        ApplyImportSettings(setting, importer.CompressionMode, importer.MultiThreading);
                    }
                    break;
            }

            if (ImageDataCached.Count > 0)
            {
                //After image data is replaced, use original and swap the specific array index
                ImageDataCached[(int)ArrayIndex] = Texture.TextureData[0];

                Texture.TextureData.Clear();

                for (int i = 0; i < ImageDataCached.Count; i++)
                    Texture.TextureData.Add(ImageDataCached[i]);
            }
        }
        public void ApplyImportSettings(TextureImporterSettings setting,STCompressionMode CompressionMode, bool multiThread, bool force_srgb = false)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (setting.GenerateMipmaps && !setting.IsFinishedCompressing)
            {
                setting.DataBlockOutput.Clear();
                setting.DataBlockOutput.Add(setting.GenerateMips(CompressionMode, multiThread));
            }

            Texture tex = setting.FromBitMap(setting.DataBlockOutput, setting);
            if (Texture != null && Texture.UserData != null) {
                tex.UserData = Texture.UserData;
                tex.UserDataDict = Texture.UserDataDict;
            }

            tex.Name = Text;
            if (force_srgb)
                tex.UseSRGB = true;

            Texture = tex;
            Load(tex);

            LoadOpenGLTexture();

            UpdateTextureMapping();

            if (IsEditorActive())
                UpdateEditor();
        }

        private void UpdateTextureMapping()
        {
            var viewport = LibraryGUI.GetActiveViewport();
            if (viewport == null)
                return;

            foreach (var drawable in viewport.scene.staticObjects)
            {
                if (drawable is BFRESRender)
                {
                    ((BFRESRender)drawable).UpdateTextureMaps();
                }
            }

            viewport.UpdateViewport();
        }

        public void Export(string FileName, bool ExportSurfaceLevel = false,
            bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {
            string ext = Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bftex":
                    SaveBinaryTexture(FileName);
                    break;
                case ".dds":
                    SaveDDS(FileName);
                    break;
                case ".astc":
                    SaveASTC(FileName);
                    break;
                default:
                    SaveBitMap(FileName);
                    break;
            }
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            if (bitmap == null)
                return; //Image is likely disposed and not needed to be applied

            Texture.Format = GenericToBntxSurfaceFormat(Format);
            Texture.Width = (uint)bitmap.Width;
            Texture.Height = (uint)bitmap.Height;

            if (MipCount != 1)
            {
                MipCount = GenerateMipCount(bitmap.Width, bitmap.Height);
                if (MipCount == 0)
                    MipCount = 1;
            }

            Texture.MipCount = MipCount;
            Texture.MipOffsets = new long[MipCount];

            try
            {
                Texture.TextureData[ArrayLevel].Clear(); //Clear previous mip maps

                var mipmaps = TextureImporterSettings.SwizzleSurfaceMipMaps(Texture,
                      GenerateMipsAndCompress(bitmap, MipCount, Format), MipCount);

                Texture.TextureData[ArrayLevel] = mipmaps;

                //Combine mip map data
                byte[] combinedMips = Utils.CombineByteArray(mipmaps.ToArray());
                Texture.TextureData[ArrayLevel][0] = combinedMips;

                LoadOpenGLTexture();
                LibraryGUI.UpdateViewport();
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to swizzle and compress image " + Text, "Error", ex.ToString());
            }
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            int target = 1;
            if (ParentBNTX != null && ParentBNTX is BNTX && ParentBNTX.BinaryTexFile != null)
            {
                bool IsNX = ParentBNTX.BinaryTexFile.PlatformTarget == "NX  ";
                if (!IsNX)
                    target = 0;
            }

            try
            {
                RedChannel = SetChannel(Texture.ChannelRed);
                GreenChannel = SetChannel(Texture.ChannelGreen);
                BlueChannel = SetChannel(Texture.ChannelBlue);
                AlphaChannel = SetChannel(Texture.ChannelAlpha);

                uint blkWidth = GetBlockWidth(Format);
                uint blkHeight = GetBlockHeight(Format);
                uint blkDepth = GetBlockDepth(Format);

                int linesPerBlockHeight = (1 << (int)Texture.BlockHeightLog2) * 8;
                uint bpp = GetBytesPerPixel(Format);

                uint numDepth = 1;
                if (Depth > 1)
                    numDepth = Depth;

                for (int depthLevel = 0; depthLevel < numDepth; depthLevel++)
                {
                    for (int arrayLevel = 0; arrayLevel < Texture.TextureData.Count; arrayLevel++)
                    {
                        int blockHeightShift = 0;

                        for (int mipLevel = 0; mipLevel < Texture.TextureData[arrayLevel].Count; mipLevel++)
                        {
                            uint width = (uint)Math.Max(1, Texture.Width >> mipLevel);
                            uint height = (uint)Math.Max(1, Texture.Height >> mipLevel);
                            uint depth = (uint)Math.Max(1, Texture.Depth >> mipLevel);

                            uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                            if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                                blockHeightShift += 1;

                            Console.WriteLine($"{width} {height} {depth} {blkWidth} {blkHeight} {blkDepth} {target} {bpp} {Texture.TileMode} {(int)Math.Max(0, Texture.BlockHeightLog2 - blockHeightShift)} {Texture.TextureData[arrayLevel][mipLevel].Length}");
                            byte[] result = TegraX1Swizzle.deswizzle(width, height, depth, blkWidth, blkHeight, blkDepth, target, bpp, (uint)Texture.TileMode, (int)Math.Max(0, Texture.BlockHeightLog2 - blockHeightShift), Texture.TextureData[arrayLevel][mipLevel]);
                            //Create a copy and use that to remove uneeded data
                            byte[] result_ = new byte[size];
                            Array.Copy(result, 0, result_, 0, size);

                            result = null;

                            if (ArrayLevel == arrayLevel && MipLevel == mipLevel && DepthLevel == depthLevel)
                                return result_;
                        }
                    }
                }
              
                return new byte[0];
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to swizzle texture {Text}! Exception: {e}");
                return new byte[0];
            }
        }
        internal void SaveBinaryTexture(string FileName)
        {
            Texture.Export(FileName, ParentBNTX.BinaryTexFile);
        }
    }
}