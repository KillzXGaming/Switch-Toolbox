using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Syroot.NintenTools.NSW.Bntx;
using Syroot.NintenTools.NSW.Bntx.GFX;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Switch_Toolbox.Library;
using WeifenLuo.WinFormsUI.Docking;
using Smash_Forge.Rendering;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class Formats
    {
        public enum BNTXImageFormat
        {
            IMAGE_FORMAT_INVALID = 0x0,
            IMAGE_FORMAT_R5_G6_B5 = 0x07,
            IMAGE_FORMAT_R8 = 0x02,
            IMAGE_FORMAT_R8_G8 = 0x09,
            IMAGE_FORMAT_R16 = 0x0a,
            IMAGE_FORMAT_R8_G8_B8_A8 = 0x0b,
            IMAGE_FORMAT_R11_G11_B11_A10 = 0x0f,
            IMAGE_FORMAT_BC1 = 0x1a,
            IMAGE_FORMAT_BC2 = 0x1b,
            IMAGE_FORMAT_BC3 = 0x1c,
            IMAGE_FORMAT_BC4 = 0x1d,
            IMAGE_FORMAT_BC5 = 0x1e,
            IMAGE_FORMAT_BC6 = 0x1f,
            IMAGE_FORMAT_BC7 = 0x20,
            IMAGE_FORMAT_ASTC4x4 = 0x2d,
            IMAGE_FORMAT_ASTC5x4 = 0x2e,
            IMAGE_FORMAT_ASTC5x5 = 0x2f,
            IMAGE_FORMAT_ASTC6x5 = 0x30,
            IMAGE_FORMAT_ASTC6x6 = 0x31,
            IMAGE_FORMAT_ASTC8x5 = 0x32,
            IMAGE_FORMAT_ASTC8x6 = 0x33,
            IMAGE_FORMAT_ASTC8x8 = 0x34,
            IMAGE_FORMAT_ASTC10x5 = 0x35,
            IMAGE_FORMAT_ASTC10x6 = 0x36,
            IMAGE_FORMAT_ASTC10x8 = 0x37,
            IMAGE_FORMAT_ASTC10x10 = 0x38,
            IMAGE_FORMAT_ASTC12x10 = 0x39,
            IMAGE_FORMAT_ASTC12x12 = 0x3a
        };

        public enum BNTXImageTypes
        {
            UNORM = 0x01,
            SNORM = 0x02,
            SRGB = 0x06,
        };

        public static uint blk_dims(uint format)
        {
            switch (format)
            {
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC1:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC2:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC3:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC4:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC5:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC6:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC7:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC4x4:
                    return 0x44;

                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC5x4: return 0x54;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC5x5: return 0x55;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC6x5: return 0x65;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC6x6: return 0x66;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC8x5: return 0x85;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC8x6: return 0x86;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC8x8: return 0x88;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x5: return 0xa5;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x6: return 0xa6;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x8: return 0xa8;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x10: return 0xaa;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC12x10: return 0xca;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC12x12: return 0xcc;

                default: return 0x11;
            }
        }

        public static uint bpps(uint format)
        {
            switch (format)
            {
                case (uint)BNTXImageFormat.IMAGE_FORMAT_R8_G8_B8_A8: return 4;
                case (uint)BNTXImageFormat.IMAGE_FORMAT_R8: return 1;

                case (uint)BNTXImageFormat.IMAGE_FORMAT_R5_G6_B5:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_R8_G8:
                    return 2;

                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC1:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC4:
                    return 8;

                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC2:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC3:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC5:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC6:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_BC7:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC5x4:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC5x5: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC6x5: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC6x6: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC8x5: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC8x6:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC8x8: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x5:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x6:
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x8: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC10x10: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC12x10: 
                case (uint)BNTXImageFormat.IMAGE_FORMAT_ASTC12x12:
                    return 16;
                default: return 0x00;
            }
        }
    }

    public class BNTX : TreeNodeFile, IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "*BNTX"};
        public string[] Extension { get; set; } = new string[] { "*.bntx"};
        public string Magic { get; set; } = "BNTX";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }
        class MenuExt : IFileMenuExtension
        {
            public ToolStripItemDark[] NewFileMenuExtensions => newFileExt;
            public ToolStripItemDark[] ToolsMenuExtensions => toolExt;
            public ToolStripItemDark[] TitleBarExtensions => null;
            public ToolStripItemDark[] CompressionMenuExtensions => null;
            public ToolStripItemDark[] ExperimentalMenuExtensions => null;

            ToolStripItemDark[] toolExt = new ToolStripItemDark[1];
            ToolStripItemDark[] newFileExt = new ToolStripItemDark[1];

            public MenuExt()
            {
                toolExt[0] = new ToolStripItemDark("Extract BNTX");
                toolExt[0].Click += Export;

                newFileExt[0] = new ToolStripItemDark("BNTX");
                newFileExt[0].Click += New;
            }
            private void New(object sender, EventArgs args)
            {
                BNTX bntx = new BNTX();
                bntx.FileName = "textures.bntx";
                bntx.Data = CreateNewBNTX("textures.bntx");
                bntx.Load();

                ObjectList.Instance.treeView1.Nodes.Add(bntx);
            }
            private void Export(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in ofd.FileNames)
                    {
                        FileReader reader = new FileReader(ofd.FileName);
                        reader.Seek(16, SeekOrigin.Begin);
                        int offsetName = reader.ReadInt32();

                        reader.Seek(offsetName, SeekOrigin.Begin);
                        string Name = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);

                        Console.WriteLine(file + " " + Name);

                        reader.Close();
                        reader.Dispose();

                   //     System.IO.File.Move(file, Name);
                    }
                }
            }
        }

        public Dictionary<string, TextureData> Textures;

        public PropertieGridData prop;
        public BntxFile BinaryTexFile;
        public string FileNameText;

        MenuItem save = new MenuItem("Save");
        MenuItem replace = new MenuItem("Replace");
        MenuItem rename = new MenuItem("Rename");
        MenuItem importTex = new MenuItem("Import Texture");
        MenuItem exportAll = new MenuItem("Export All Textures");
        MenuItem sort = new MenuItem("Sort");
        MenuItem clear = new MenuItem("Clear");

        private bool hasParent;
        public bool HasParent
        {
            get
            {
                hasParent = Parent != null;
                replace.Enabled = hasParent;
                rename.Enabled = hasParent;
                return hasParent;
            }
        }
        public bool CanReplace;
        public bool AllGLInitialized
        {
            get
            {
                if (Textures.Any(item => item.Value.RenderableTex.GLInitialized == false))
                    return false;
                else
                    return true;
            }
        }

        public void Load()
        {
            IFileInfo = new IFileInfo();

            IsActive = true;
            UseEditMenu = true;
            CanSave = true;

            ImageKey = "bntx";
            SelectedImageKey = "bntx";

            FileNameText = FileName;

            LoadFile(Data, Name);

            PluginRuntime.bntxContainers.Add(this);

            //Check if bntx is parented to determine if an archive is used
            bool checkParent = HasParent;

            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(save);
            ContextMenu.MenuItems.Add(replace);
            ContextMenu.MenuItems.Add(rename);
            ContextMenu.MenuItems.Add(importTex);
            ContextMenu.MenuItems.Add(exportAll);
            ContextMenu.MenuItems.Add(sort);
            ContextMenu.MenuItems.Add(clear);

            save.Click += Save;
            replace.Click += Import;
            rename.Click += Rename;
            importTex.Click += ImportTexture;
            exportAll.Click += ExportAll;
            sort.Click += SortTextures;
            clear.Click += Clear;
        }
        public void Unload()
        {
            foreach (TextureData tex in Textures.Values)
            {
                tex.Surfaces.Clear();
            }

            Textures.Clear();
            Nodes.Clear();
        }

        private static byte[] CreateNewBNTX(string Name)
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

            return mem.ToArray();
        }
        public void RemoveTexture(TextureData textureData)
        {
            Nodes.Remove(textureData);
            Textures.Remove(textureData.Text);
            Viewport.Instance.UpdateViewport();
        }
        public override void OnClick(TreeView treeView)
        {

        }
        //Check right click to enable/disable certain context menus
        public override void OnMouseRightClick(TreeView treeview)
        {
            bool checkParent = HasParent;
        }

        public void LoadFile(byte[] data, string Name = "")
        {
            Textures = new Dictionary<string, TextureData>();

            Data = data;
            BinaryTexFile = new BntxFile(new MemoryStream(Data));
            Text = BinaryTexFile.Name;

            prop = new PropertieGridData();
            prop.Target = new string(BinaryTexFile.Target);
            prop.VersionMajor = BinaryTexFile.VersionMajor;
            prop.VersionMajor2 = BinaryTexFile.VersionMajor2;
            prop.VersionMinor = BinaryTexFile.VersionMinor;
            prop.VersionMinor2 = BinaryTexFile.VersionMinor2;
            prop.VersionFull = $"{BinaryTexFile.VersionMajor}.{BinaryTexFile.VersionMajor2}.{BinaryTexFile.VersionMinor}.{BinaryTexFile.VersionMinor2}";

            foreach (Texture tex in BinaryTexFile.Textures)
            {
                TextureData texData = new TextureData(tex, BinaryTexFile);
                //      texData.LoadOpenGLTexture();

                Nodes.Add(texData);
                Textures.Add(tex.Name, texData);
            }
            BinaryTexFile.Textures.Clear(); //We don't need these in memeory anymore
            BinaryTexFile.TextureDict.Clear();

        }
        private void ImportTexture(object sender, EventArgs args)
        {
            ImportTexture();
        }
        public void ImportTexture()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                                     "Binary Texture |*.bftex|" +
                                     "Microsoft DDS |*.dds|" +
                                     "TGA |*.tga|" +
                                     "Portable Network Graphics |*.png|" +
                                     "Joint Photographic Experts Group |*.jpg|" +
                                     "Bitmap Image |*.bmp|" +
                                     "Tagged Image File Format |*.tiff|" +
                                     "All files(*.*)|*.*";

            ofd.DefaultExt = "bftex";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                BinaryTextureImporterList importer = new BinaryTextureImporterList();

                bool UseDialog = false;
                foreach (string name in ofd.FileNames)
                {
                    TextureImporterSettings setting = new TextureImporterSettings();

                }
                GC.Collect();
                Cursor.Current = Cursors.Default;
            }
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
            importDDS.LoadDDS(TextureName, BinaryTexFile, data);

            TextureData texData = importDDS.textureData;
            texData.Text = TextureName;

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
                    importer.LoadDDS(name, BinaryTexFile);
                    break;
                case ".tga":
                    importer.LoadTGA(name, BinaryTexFile);
                    break;
                default:
                    importer.LoadBitMap(name, BinaryTexFile);
                    break;
            }

            return importer;
        }
        public TextureData AddTexture(string name)
        {
            var importer = new TextureImporterSettings();

            TextureData texData = null;
            string ext = Path.GetExtension(name);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bftex":
                    Texture tex = new Texture();
                    tex.Import(name);
                    texData = new TextureData(tex, BinaryTexFile);
                    break;
                case ".dds":
                    importer.LoadDDS(name, BinaryTexFile);
                    texData = importer.textureData;
                    break;
                default:
                    importer.LoadBitMap(name, BinaryTexFile);
                    texData = importer.textureData;
                    break;
            }
            if (texData != null)
            {
                List<string> keyList = new List<string>(Textures.Keys);
                texData.Text = Utils.RenameDuplicateString(keyList, texData.Text);

                Nodes.Add(texData);
                Textures.Add(texData.Text, texData);
            }
            return texData;
        }
        private void Clear(object sender, EventArgs args)
        {
            Nodes.Clear();
            Textures.Clear();
            GC.Collect();
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

                TextureFormatExport form = new TextureFormatExport(Formats);
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
            SortNodes(this);
        }
        public byte[] Save()
        {
            BinaryTexFile.Textures.Clear();
            BinaryTexFile.TextureDict.Clear();

            foreach (TextureData tex in Textures.Values)
            {
                tex.Texture.Name = tex.Text;

                BinaryTexFile.Textures.Add(tex.Texture);
                BinaryTexFile.TextureDict.Add(tex.Text);
            }

            MemoryStream mem = new MemoryStream();
            BinaryTexFile.Save(mem);

            return mem.ToArray();
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

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Data = File.ReadAllBytes(ofd.FileName);
                LoadFile(Data);
            }
        }
        private void Rename(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Text = dialog.textBox1.Text;
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
        public Texture Texture;
        public BntxFile bntxFile;
        public bool GLInitialized = false;

        public TextureData()
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }
        public TextureData(Texture tex, BntxFile bntx)
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            Texture = tex;
            bntxFile = bntx;

            Text = tex.Name;
            Width = tex.Width;
            Height = tex.Height;
            MipmapCount = tex.MipCount;
            var formats = ConvertFormat(tex.Format, tex.FormatType);
            Format = formats.Item1;
            FormatType = formats.Item2;

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            MenuItem replace = new MenuItem("Replace");
            MenuItem remove = new MenuItem("Remove");
            MenuItem rename = new MenuItem("Rename");

            ContextMenu.MenuItems.Add(export);
            ContextMenu.MenuItems.Add(replace);
            ContextMenu.MenuItems.Add(remove);
            ContextMenu.MenuItems.Add(rename);

            export.Click += Export;
            replace.Click += Replace;
            remove.Click += Remove;
            rename.Click += Rename;
            string TargetString = new string(bntx.Target);

            int target = 0;
            if (TargetString == "NX  ")
                target = 1;


            LoadTexture(Texture);
        }
        public static Tuple<SurfaceFormat, SurfaceFormatType> GetSurfaceFormat(TEX_FORMAT format, TEX_FORMAT_TYPE type)
        {
            var surfaceFormat = SurfaceFormat.Invalid;
            var surfaceType = SurfaceFormatType.UNORM;

            Enum.TryParse(format.ToString(), out surfaceFormat);
            Enum.TryParse(type.ToString(), out surfaceType);
            
            return Tuple.Create(surfaceFormat, surfaceType);
        }
        public static Tuple<TEX_FORMAT, TEX_FORMAT_TYPE> ConvertFormat(SurfaceFormat surfaceFormat, SurfaceFormatType surfaceType)
        {
            var format = TEX_FORMAT.UNKNOWN;
            var type = TEX_FORMAT_TYPE.UNORM;

            Enum.TryParse(surfaceFormat.ToString(), out format);
            Enum.TryParse(surfaceType.ToString(), out type);

            return Tuple.Create(format, type);
        }
        public override void OnClick(TreeView treeView)
        {
            UpdateBNTXEditor();
        }
        public void UpdateBNTXEditor()
        {
            if (Viewport.Instance.gL_ControlModern1 == null || Viewport.Instance.gL_ControlModern1.Visible == false)
                PluginRuntime.FSHPDockState = WeifenLuo.WinFormsUI.Docking.DockState.Document;

            BNTXEditor docked = (BNTXEditor)LibraryGUI.Instance.GetContentDocked(new BNTXEditor());
            if (docked == null)
            {
                docked = new BNTXEditor();
                LibraryGUI.Instance.LoadDockContent(docked, PluginRuntime.FSHPDockState);
            }
            docked.Text = Text;
            docked.Dock = DockStyle.Fill;
            docked.LoadProperty(this);
        }
        private void Remove(object sender, EventArgs args)
        {
            ((BNTX)Parent).RemoveTexture(this);
        }
        private void Rename(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ((BNTX)Parent).Textures.Remove(Text);
                Text = dialog.textBox1.Text;

                ((BNTX)Parent).Textures.Add(Text, this);
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Binary Texture |*.bftex|" +
                         "Microsoft DDS |*.dds|" +
                         "TGA |*.tga|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Replace(ofd.FileName);
            }
        }
        public void Replace(string FileName)
        {
            string ext = Path.GetExtension(FileName);
            ext = ext.ToLower();

            TextureImporterSettings setting = new TextureImporterSettings();
            BinaryTextureImporterList importer = new BinaryTextureImporterList();

            switch (ext)
            {
                case ".bftex":
                    Texture.Import(FileName);
                    break;
                case ".dds":
                    setting.LoadDDS(FileName, bntxFile, null, this);
                    ApplyImportSettings(setting);
                    break;
                default:
                    setting.LoadBitMap(FileName, bntxFile);
                    importer.LoadSetting(setting);

                    if (importer.ShowDialog() == DialogResult.OK)
                    {
                        ApplyImportSettings(setting);
                    }
                    break;
            }
 
        }
        private void ApplyImportSettings(TextureImporterSettings setting)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (setting.GenerateMipmaps)
            {
                setting.DataBlockOutput.Clear();
                setting.DataBlockOutput.Add(setting.GenerateMips());
            }

            if (setting.DataBlockOutput != null)
            {
                Texture = setting.FromBitMap(setting.DataBlockOutput[0], setting);
                LoadOpenGLTexture();
            }
            else
            {
                MessageBox.Show("Something went wrong???");
            }
            Texture.Name = Text;
            UpdateBfresTextureMapping();
            UpdateBNTXEditor();
        }
        private void UpdateBfresTextureMapping()
        {
            foreach (GL_Core.Interfaces.AbstractGlDrawable draw in Runtime.abstractGlDrawables)
            {
                if (draw is BFRESRender)
                {
                    ((BFRESRender)draw).UpdateTextureMaps();
                }
            }
        }
        private  void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Texture.Name;
            sfd.DefaultExt = "bftex";
            sfd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Binary Texture |*.bftex|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export(sfd.FileName);
            }
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
        public void LoadTexture(Texture tex, int target = 1)
        {
            Surfaces.Clear();

            try
            {
                uint blk_dim = Formats.blk_dims((uint)((int)tex.Format >> 8));
                uint blkWidth = blk_dim >> 4;
                uint blkHeight = blk_dim & 0xF;

                int linesPerBlockHeight = (1 << (int)tex.BlockHeightLog2) * 8;

                uint bpp = Formats.bpps((uint)((int)tex.Format >> 8));
                for (int arrayLevel = 0; arrayLevel < tex.ArrayLength; arrayLevel++)
                {
                    int blockHeightShift = 0;
                    List<byte[]> mips = new List<byte[]>();
                    for (int mipLevel = 0; mipLevel < tex.TextureData[arrayLevel].Count; mipLevel++)
                    {
                        uint width = (uint)Math.Max(1, tex.Width >> mipLevel);
                        uint height = (uint)Math.Max(1, tex.Height >> mipLevel);

                        uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                        if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                            blockHeightShift += 1;

                        byte[] result = TegraX1Swizzle.deswizzle(width, height, blkWidth, blkHeight, target, bpp, (uint)tex.TileMode, (int)Math.Max(0, tex.BlockHeightLog2 - blockHeightShift), tex.TextureData[arrayLevel][mipLevel]);
                        //Create a copy and use that to remove uneeded data
                        byte[] result_ = new byte[size];
                        Array.Copy(result, 0, result_, 0, size);
                        mips.Add(result_);

                    }
                    Surfaces.Add(new Surface() { mipmaps = mips });
                }

                Texture = tex;

            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to swizzle texture {Text}! Exception: {e}");
            }
        }
        internal void SaveBinaryTexture(string FileName)
        {
            Console.WriteLine("Test");
            Texture.Export(FileName, bntxFile);
        }
    }
}
