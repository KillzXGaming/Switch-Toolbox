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

namespace FirstPlugin
{
    public class Formats
    {
        public enum BNTXImageFormat
        {
            IMAGE_FORMAT_INVALID = 0x0,
            IMAGE_FORMAT_R8_G8_B8_A8 = 0x0b,
            IMAGE_FORMAT_R5_G6_B5 = 0x07,
            IMAGE_FORMAT_R8 = 0x02,
            IMAGE_FORMAT_R8_G8 = 0x09,
            IMAGE_FORMAT_BC1 = 0x1a,
            IMAGE_FORMAT_BC2 = 0x1b,
            IMAGE_FORMAT_BC3 = 0x1c,
            IMAGE_FORMAT_BC4 = 0x1d,
            IMAGE_FORMAT_BC5 = 0x1e,
            IMAGE_FORMAT_BC6 = 0x1f,
            IMAGE_FORMAT_BC7 = 0x20,
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
                case 0x2d:
                    return 0x44;

                case 0x2e: return 0x54;
                case 0x2f: return 0x55;
                case 0x30: return 0x65;
                case 0x31: return 0x66;
                case 0x32: return 0x85;
                case 0x33: return 0x86;
                case 0x34: return 0x88;
                case 0x35: return 0xa5;
                case 0x36: return 0xa6;
                case 0x37: return 0xa8;
                case 0x38: return 0xaa;
                case 0x39: return 0xca;
                case 0x3a: return 0xcc;

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
                case 0x2e:
                case 0x2f:
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                case 0x38:
                case 0x39:
                case 0x3a:
                    return 16;
                default: return 0x00;
            }
        }
    }

    public class BNTX : IFileFormat
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
        public TreeNodeFile EditorRoot { get; set; }
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
            public ToolStripItemDark[] NewFileMenuExtensions => null;
            public ToolStripItemDark[] ToolsMenuExtensions => null;
            public ToolStripItemDark[] TitleBarExtensions => null;
            public ToolStripItemDark[] CompressionMenuExtensions => null;
            public ToolStripItemDark[] ExperimentalMenuExtensions => null;

            ToolStripItemDark[] newFileExt = new ToolStripItemDark[1];
            public MenuExt()
            {
                newFileExt[0] = new ToolStripItemDark("BNTX ");
            }
        }   

        BinaryTextureContainer bntx;

        public void Load()
        {
            IFileInfo = new IFileInfo();

            IsActive = true;
            UseEditMenu = true;
            CanSave = true;
            bntx = new BinaryTextureContainer(Data, FileName, "", this);
            EditorRoot = bntx;
        }
        public void Unload()
        {
            foreach (TextureData tex in bntx.Textures.Values)
            {
                tex.mipmaps.Clear();
                tex.renderedGLTex = null;
            }

            bntx.Textures.Clear();
            bntx.Nodes.Clear();
        }
        public byte[] Save()
        {
            return bntx.Save();
        }
    }

    public class BinaryTextureContainer : TreeNodeFile
    {
        public Dictionary<string, TextureData> Textures;

        public byte[] Data;
        public PropertieGridData prop;
        public BntxFile BinaryTexFile;
        public string FileNameText;

        MenuItem save = new MenuItem("Save");
        MenuItem replace = new MenuItem("Replace");
        MenuItem rename = new MenuItem("Rename");
        MenuItem importTex = new MenuItem("Import Texture");
        MenuItem exportAll = new MenuItem("Export All Textures");
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
                if (Textures.Any(item => item.Value.GLInitialized == false))
                    return false;
                else
                    return true;
            }
        }

        public BinaryTextureContainer()
        {
            ImageKey = "bntx";
            SelectedImageKey = "bntx";
        }
        public BinaryTextureContainer(byte[] data, string Name = "", string FileName = "", IFileFormat handler = null)
        {
            if (data.Length == 0)
                data = CreateNewBNTX(Name);

            ImageKey = "bntx";
            SelectedImageKey = "bntx";

            FileNameText = FileName;
            LoadFile(data, Name);

            PluginRuntime.bntxContainers.Add(this);
            FileHandler = handler;

            //Check if bntx is parented to determine if an archive is used
            bool checkParent = HasParent;

            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(save);
            ContextMenu.MenuItems.Add(replace);
            ContextMenu.MenuItems.Add(rename);
            ContextMenu.MenuItems.Add(importTex);
            ContextMenu.MenuItems.Add(exportAll);
            ContextMenu.MenuItems.Add(clear);

            save.Click      += Save;
            replace.Click   += Import;
            rename.Click   += Rename;
            importTex.Click += ImportTexture;
            exportAll.Click += ExportAll;
            clear.Click     += Clear;
        }
        private byte[] CreateNewBNTX(string Name)
        {
            MemoryStream mem = new MemoryStream();

            BntxFile bntx = new BntxFile();
            bntx.Target = new char[] { 'N', 'X', ' ', ' '};
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

                List<TextureImporterSettings> settings = new List<TextureImporterSettings>();
                foreach (string name in ofd.FileNames)
                {
                    string ext = Path.GetExtension(name);
                    ext = ext.ToLower();

                    if (ext == ".dds" || ext == ".bftex")
                    {
                        AddTexture(name);
                    }
                    else
                    {
                        settings.Add(LoadSettings(name));
                    }
                }
                if (settings.Count == 0)
                {
                    importer.Dispose();
                    return;
                }

                importer.LoadSettings(settings, this);
                if (importer.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    foreach (var setting in settings)
                    {
                        if (setting.GenerateMipmaps)
                        {
                            setting.DataBlockOutput.Clear();
                            setting.DataBlockOutput.Add(setting.GenerateMips());
                        }

                        if (setting.DataBlockOutput != null)
                        {
                            Texture tex = setting.FromBitMap(setting.DataBlockOutput[0], setting);
                            if (setting.textureData != null)
                            {
                                setting.textureData.LoadTexture(tex, 1);
                            }
                            else
                            {
                                setting.textureData = new TextureData(tex, setting.bntx);
                            }

                            int i = 0;
                            if (Textures.ContainsKey(setting.textureData.Text))
                            {
                                setting.textureData.Text = setting.textureData.Text + i++;
                            }

                            Nodes.Add(setting.textureData);
                            Textures.Add(setting.textureData.Text, setting.textureData);
                            setting.textureData.LoadOpenGLTexture();
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong???");
                        }
                    }
                }
                settings.Clear();
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
                texData.LoadOpenGLTexture();
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
            sfd.FileName = FileHandler.FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, FileHandler.Save());
            }
        }
    }

    public class TextureData : TreeNodeCustom
    {
        public Texture Texture;
        public BntxFile bntxFile;
        public List<List<byte[]>> mipmaps = new List<List<byte[]>>();
        public BRTI_Texture renderedGLTex = new BRTI_Texture();
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

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
            MenuItem remove = new MenuItem("Remove");
            ContextMenu.MenuItems.Add(remove);
            remove.Click += Remove;
            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;

            string TargetString = new string(bntx.Target);

            int target = 0;
            if (TargetString == "NX  ")
                target = 1;
        }
        public override void OnClick(TreeView treeView)
        {
            if (LibraryGUI.Instance.dockContent != null && !EditorIsActive(LibraryGUI.Instance.dockContent))
            {
                BNTXEditor BNTXEditor = new BNTXEditor();
                BNTXEditor.Text = Text;
                BNTXEditor.Dock = DockStyle.Fill;
                BNTXEditor.LoadProperty(this);
                LibraryGUI.Instance.LoadDockContent(BNTXEditor, PluginRuntime.FSHPDockState);
            }
        }
        public bool EditorIsActive(DockContent dock)
        {
            foreach (Control ctrl in dock.Controls)
            {
                if (ctrl is BNTXEditor)
                {
                    dock.Text = Text;
                    ((BNTXEditor)ctrl).LoadProperty(this);
                    return true;
                }
            }

            return false;
        }

        public BRTI_Texture LoadOpenGLTexture()
        {
            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return null;

            LoadTexture(Texture);

            if (mipmaps.Count <= 0)
            {
                throw new Exception("No texture data found");
            }

            renderedGLTex.data = mipmaps[0][0];
            renderedGLTex.width = (int)Texture.Width;
            renderedGLTex.height = (int)Texture.Height;

            switch (Texture.Format)
            {
                case SurfaceFormat.BC1_UNORM:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case SurfaceFormat.BC1_SRGB:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case SurfaceFormat.BC2_UNORM:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case SurfaceFormat.BC2_SRGB:
                    renderedGLTex.type = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext;
                    break;
                case SurfaceFormat.BC3_UNORM:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case SurfaceFormat.BC3_SRGB:
                    renderedGLTex.type = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
                    break;
                case SurfaceFormat.BC4_UNORM:
                    renderedGLTex.type = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case SurfaceFormat.BC4_SNORM:
                    renderedGLTex.type = PixelInternalFormat.CompressedSignedRedRgtc1;
                    break;
                case SurfaceFormat.BC5_UNORM:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case SurfaceFormat.BC5_SNORM:
                    renderedGLTex.data = DDSCompressor.DecompressBC5(mipmaps[0][0], (int)Texture.Width, (int)Texture.Height, true, true);
                    renderedGLTex.type = PixelInternalFormat.Rgba;
                    renderedGLTex.utype = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case SurfaceFormat.BC6_FLOAT:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgbBptcSignedFloat;
                    break;
                case SurfaceFormat.BC6_UFLOAT:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgbBptcUnsignedFloat;
                    break;
                case SurfaceFormat.BC7_SRGB:
                    renderedGLTex.type = PixelInternalFormat.CompressedSrgbAlphaBptcUnorm;
                    break;
                case SurfaceFormat.BC7_UNORM:
                    renderedGLTex.type = PixelInternalFormat.CompressedRgbaBptcUnorm;
                    break;
                case SurfaceFormat.R8_G8_B8_A8_SRGB:
                    renderedGLTex.type = PixelInternalFormat.Rgba;
                    renderedGLTex.utype = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
            }
            renderedGLTex.display = loadImage(renderedGLTex);
            GLInitialized = true;

            return renderedGLTex;
        }

        //Gets the decompressed byte[]
        public static Bitmap DecodeBlock(byte[] data, uint Width, uint Height, SurfaceFormat Format)
        {
            Bitmap decomp;

            if (Format == SurfaceFormat.BC5_SNORM)
                return DDSCompressor.DecompressBC5(data, (int)Width, (int)Height, true);

            byte[] d = null;
            if (IsCompressedFormat(Format))
                d = DDSCompressor.DecompressBlock(data, (int)Width, (int)Height, GetCompressedDXGI_FORMAT(Format));
            else if (IsAtscFormat(Format))
                d = null;
            else
                d = DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, GetUncompressedDXGI_FORMAT(Format));
            
            if (d != null)
            {
                decomp = BitmapExtension.GetBitmap(d, (int)Width, (int)Height);
                return SwapBlueRedChannels(decomp);
            }
            return null;
        }
        private static DDS.DXGI_FORMAT GetUncompressedDXGI_FORMAT(SurfaceFormat Format)
        {
            switch (Format)
            {
                case SurfaceFormat.A1_B5_G5_R5_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM;
                case SurfaceFormat.A4_B4_G4_R4_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM;
                case SurfaceFormat.B5_G5_R5_A1_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM;
                case SurfaceFormat.B5_G6_R5_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM;
                case SurfaceFormat.B8_G8_R8_A8_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
                case SurfaceFormat.B8_G8_R8_A8_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                case SurfaceFormat.R10_G10_B10_A2_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UNORM;
                case SurfaceFormat.R11_G11_B10_FLOAT: return DDS.DXGI_FORMAT.DXGI_FORMAT_R11G11B10_FLOAT;
                case SurfaceFormat.R16_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_R16_UNORM;
                case SurfaceFormat.R32_FLOAT: return DDS.DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT;
                case SurfaceFormat.R4_G4_B4_A4_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM;
                case SurfaceFormat.R4_G4_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM;
                case SurfaceFormat.R5_G5_B5_A1_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM;
                case SurfaceFormat.R5_G6_B5_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM;
                case SurfaceFormat.R8_G8_B8_A8_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
                case SurfaceFormat.R8_G8_B8_A8_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                case SurfaceFormat.R8_G8_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM;
                case SurfaceFormat.R8_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8_UNORM;
                case SurfaceFormat.Invalid: throw new Exception("Invalid Format");
                default:
                    throw new Exception($"Cannot convert format {Format}");
            }
        }
        private static DDS.DXGI_FORMAT GetCompressedDXGI_FORMAT(SurfaceFormat Format)
        {
            switch (Format)
            {
                case SurfaceFormat.BC1_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                case SurfaceFormat.BC1_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB;
                case SurfaceFormat.BC2_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
                case SurfaceFormat.BC2_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB;
                case SurfaceFormat.BC3_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                case SurfaceFormat.BC3_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB;
                case SurfaceFormat.BC4_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
                case SurfaceFormat.BC4_SNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM;
                case SurfaceFormat.BC5_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
                case SurfaceFormat.BC5_SNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM;
                case SurfaceFormat.BC6_UFLOAT: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16;
                case SurfaceFormat.BC6_FLOAT: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16;
                case SurfaceFormat.BC7_UNORM: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
                case SurfaceFormat.BC7_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB;
                case SurfaceFormat.Invalid: throw new Exception("Invalid Format");
                default:
                    throw new Exception($"Cannot convert format {Format}");
            }
        }
        private static bool IsCompressedFormat(SurfaceFormat Format)
        {
            switch (Format)
            {
                case SurfaceFormat.BC1_UNORM: 
                case SurfaceFormat.BC1_SRGB: 
                case SurfaceFormat.BC2_UNORM: 
                case SurfaceFormat.BC2_SRGB:
                case SurfaceFormat.BC3_UNORM:
                case SurfaceFormat.BC3_SRGB:
                case SurfaceFormat.BC4_UNORM:
                case SurfaceFormat.BC4_SNORM: 
                case SurfaceFormat.BC5_UNORM: 
                case SurfaceFormat.BC5_SNORM:
                case SurfaceFormat.BC6_UFLOAT:
                case SurfaceFormat.BC6_FLOAT: 
                case SurfaceFormat.BC7_UNORM: 
                case SurfaceFormat.BC7_SRGB:
                    return true;
                default:
                    return false;
            }
        }
        private static bool IsAtscFormat(SurfaceFormat Format)
        {
            switch (Format)
            {
                case SurfaceFormat.ASTC_10x10_SRGB:
                case SurfaceFormat.ASTC_10x10_UNORM:
                case SurfaceFormat.ASTC_10x5_SRGB:
                case SurfaceFormat.ASTC_10x5_UNORM:
                case SurfaceFormat.ASTC_10x6_SRGB:
                case SurfaceFormat.ASTC_10x6_UNORM:
                case SurfaceFormat.ASTC_10x8_SRGB:
                case SurfaceFormat.ASTC_10x8_UNORM:
                case SurfaceFormat.ASTC_12x10_SRGB:
                case SurfaceFormat.ASTC_12x10_UNORM:
                case SurfaceFormat.ASTC_12x12_SRGB:
                case SurfaceFormat.ASTC_12x12_UNORM:
                case SurfaceFormat.ASTC_4x4_SRGB:
                case SurfaceFormat.ASTC_5x4_SRGB:
                case SurfaceFormat.ASTC_5x4_UNORM:
                case SurfaceFormat.ASTC_5x5_SRGB:
                case SurfaceFormat.ASTC_5x5_UNORM:
                case SurfaceFormat.ASTC_6x5_SRGB:
                case SurfaceFormat.ASTC_6x5_UNORM:
                case SurfaceFormat.ASTC_6x6_SRGB:
                case SurfaceFormat.ASTC_6x6_UNORM:
                case SurfaceFormat.ASTC_8x5_SRGB:
                case SurfaceFormat.ASTC_8x5_UNORM:
                case SurfaceFormat.ASTC_8x6_SRGB:
                case SurfaceFormat.ASTC_8x6_UNORM:
                case SurfaceFormat.ASTC_8x8_SRGB:
                case SurfaceFormat.ASTC_8x8_UNORM:
                    return true;
                default:
                    return false;
            }
        }
        public static Bitmap SwapBlueRedChannels(Bitmap bitmap)
        {
            return ColorComponentSelector(bitmap, ChannelType.Blue, ChannelType.Green, ChannelType.Red, ChannelType.Alpha);
        }
        public static byte[] CompressBlock(byte[] data, int width, int height, SurfaceFormat format)
        {
            if (IsCompressedFormat(format))
                return DDSCompressor.CompressBlock(data, width, height, GetCompressedDXGI_FORMAT(format));
            else if (IsAtscFormat(format))
                return null;
            else
                return DDSCompressor.EncodePixelBlock(data, width, height, GetUncompressedDXGI_FORMAT(format));
        }
        public unsafe Bitmap GLTextureToBitmap(BRTI_Texture t, int id)
        {
            Bitmap bitmap = new Bitmap(t.width, t.height);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, t.width, t.height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
        public unsafe void ExportAsImage(BRTI_Texture t, int id, string path)
        {
            Bitmap bitmap = new Bitmap(t.width, t.height);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, t.width, t.height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);
            bitmap.Save(path);
        }
        public class BRTI_Texture
        {
            public List<byte[]> mipmaps = new List<byte[]>();
            public byte[] data;
            public int width, height;
            public int display = 0;
            public PixelInternalFormat type;
            public OpenTK.Graphics.OpenGL.PixelFormat utype;
        }
        public static int getImageSize(BRTI_Texture t)
        {
            switch (t.type)
            {
                case PixelInternalFormat.CompressedRgbaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedRedRgtc1:
                case PixelInternalFormat.CompressedSignedRedRgtc1:
                    return (t.width * t.height / 2);
                case PixelInternalFormat.CompressedRgbaS3tcDxt3Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext:
                case PixelInternalFormat.CompressedRgbaS3tcDxt5Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext:
                case PixelInternalFormat.CompressedSignedRgRgtc2:
                case PixelInternalFormat.CompressedRgRgtc2:
                    return (t.width * t.height);
                case PixelInternalFormat.Rgba:
                    return t.data.Length;
                default:
                    return t.data.Length;
            }
        }
        public static int loadImage(BRTI_Texture t)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);

            if (t.type != PixelInternalFormat.Rgba)
            {
                GL.CompressedTexImage2D<byte>(TextureTarget.Texture2D, 0, (InternalFormat)t.type,
                    t.width, t.height, 0, getImageSize(t), t.data);
                //Debug.WriteLine(GL.GetError());
            }
            else
            {
                GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, t.type, t.width, t.height, 0,
                    t.utype, PixelType.UnsignedByte, t.data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }
        private void Remove(object sender, EventArgs args)
        {
            ((BinaryTextureContainer)Parent).RemoveTexture(this);
        }
        private void Rename(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ((BinaryTextureContainer)Parent).Textures.Remove(Text);
                Text = dialog.textBox1.Text;

                ((BinaryTextureContainer)Parent).Textures.Add(Text, this);
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Binary Texture |*.bftex|" +
                         "Microsoft DDS |*.dds|" +
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
                    break;
                default:
                    setting.LoadBitMap(FileName, bntxFile);
                    importer.LoadSetting(setting, (BinaryTextureContainer)Parent);
                    break;
            }

            if (importer.ShowDialog() == DialogResult.OK)
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
                    LoadTexture(Texture, 1);
                    LoadOpenGLTexture();
                }
                else
                {
                    MessageBox.Show("Something went wrong???");
                }
                Texture.Name = Text;
                UpdateBfresTextureMapping();

                //LibraryGUI.Instance.LoadDockContent(BNTXEditor);
            }
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
        private void Export(object sender, EventArgs args)
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
                default:
                    SaveBitMap(FileName);
                    break;
            }
        }
        internal void SaveBitMap(string FileName, int SurfaceLevel = 0, int MipLevel = 0)
        {
            Bitmap bitMap = DisplayTexture(MipLevel, SurfaceLevel);

            bitMap.Save(FileName);
        }
        internal void SaveBinaryTexture(string FileName)
        {
            Console.WriteLine("Test");
            Texture.Export(FileName, bntxFile);
        }
        internal void SaveDDS(string FileName)
        {
            DDS dds = new DDS();
            dds.header = new DDS.Header();
            dds.header.width = Texture.Width;
            dds.header.height = Texture.Height;
            dds.header.mipmapCount = (uint)mipmaps.Count;

            bool IsDX10 = false;

            switch (Texture.Format)
            {
                case SurfaceFormat.BC1_UNORM:
                case SurfaceFormat.BC1_SRGB:
                    dds.header.ddspf.fourCC = "DXT1";
                    break;
                case SurfaceFormat.BC2_UNORM:
                case SurfaceFormat.BC2_SRGB:
                    dds.header.ddspf.fourCC = "DXT3";
                    break;
                case SurfaceFormat.BC3_UNORM:
                case SurfaceFormat.BC3_SRGB:
                    dds.header.ddspf.fourCC = "DXT5";
                    break;
                case SurfaceFormat.BC4_UNORM:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
                    break;
                case SurfaceFormat.BC4_SNORM:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM;
                    break;
                case SurfaceFormat.BC5_UNORM:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
                    break;
                case SurfaceFormat.BC5_SNORM:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM;
                    break;
                case SurfaceFormat.BC6_FLOAT:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16;
                    break;
                case SurfaceFormat.BC6_UFLOAT:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16;
                    break;
                case SurfaceFormat.BC7_UNORM:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB;
                    break;
                case SurfaceFormat.BC7_SRGB:
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
                    break;
                default:
                    throw new Exception($"Format {Texture.Format} not supported!");
            }
     
            if (IsDX10)
                dds.header.ddspf.fourCC = "DX10";

            dds.Save(dds, FileName, IsDX10, mipmaps);
        }
        public void LoadTexture(Texture tex, int target = 1)
        {
            mipmaps.Clear();

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
                mipmaps.Add(mips);
            }

            Texture = tex;
        }

        public Bitmap DisplayTexture(int DisplayMipIndex = 0, int ArrayIndex = 0)
        {
            LoadTexture(Texture);

            if (mipmaps.Count <= 0)
            {
                throw new Exception("No texture data found");
            }

            uint width = (uint)Math.Max(1, Texture.Width >> DisplayMipIndex);
            uint height = (uint)Math.Max(1, Texture.Height >> DisplayMipIndex);

            byte[] data = mipmaps[ArrayIndex][DisplayMipIndex];

            return DecodeBlock(data, width, height, Texture.Format);
        }

        public Bitmap ToBitmap()
        {
            return new Bitmap("");
        }
        public Bitmap UpdateBitmap(Bitmap image)
        {
            return ColorComponentSelector(image, Texture.ChannelRed, Texture.ChannelGreen, Texture.ChannelBlue, Texture.ChannelAlpha);
        }
        public static ChannelType[] SetChannelsByFormat(SurfaceFormat Format)
        {
            ChannelType[] channels = new ChannelType[4];

            switch (Format)
            {
                case SurfaceFormat.BC5_UNORM:
                case SurfaceFormat.BC5_SNORM:
                    channels[0] = ChannelType.Red;
                    channels[1] = ChannelType.Green;
                    channels[2] = ChannelType.Zero;
                    channels[3] = ChannelType.One;
                    break;
                case SurfaceFormat.BC4_SNORM:
                case SurfaceFormat.BC4_UNORM:
                    channels[0] = ChannelType.Red;
                    channels[1] = ChannelType.Red;
                    channels[2] = ChannelType.Red;
                    channels[3] = ChannelType.Red;
                    break;
                default:
                    channels[0] = ChannelType.Red;
                    channels[1] = ChannelType.Green;
                    channels[2] = ChannelType.Blue;
                    channels[3] = ChannelType.Alpha;
                    break;
            }
            return channels;
        }
        public static Bitmap ColorComponentSelector(Bitmap image, ChannelType R, ChannelType G, ChannelType B, ChannelType A)
        {
            BitmapExtension.ColorSwapFilter color = new BitmapExtension.ColorSwapFilter();
            if (R == ChannelType.Red)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Red;
            if (R == ChannelType.Green)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Green;
            if (R == ChannelType.Blue)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Blue;
            if (R == ChannelType.Alpha)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Alpha;
            if (R == ChannelType.One)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.One;
            if (R == ChannelType.Zero)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Zero;

            if (G == ChannelType.Red)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Red;
            if (G == ChannelType.Green)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Green;
            if (G == ChannelType.Blue)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Blue;
            if (G == ChannelType.Alpha)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Alpha;
            if (G == ChannelType.One)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.One;
            if (G == ChannelType.Zero)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Zero;

            if (B == ChannelType.Red)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Red;
            if (B == ChannelType.Green)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Green;
            if (B == ChannelType.Blue)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Blue;
            if (B == ChannelType.Alpha)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Alpha;
            if (B == ChannelType.One)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.One;
            if (B == ChannelType.Zero)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Zero;

            if (A == ChannelType.Red)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Red;
            if (A == ChannelType.Green)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Green;
            if (A == ChannelType.Blue)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Blue;
            if (A == ChannelType.Alpha)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Alpha;
            if (A == ChannelType.One)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.One;
            if (A == ChannelType.Zero)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Zero;

            return BitmapExtension.SwapRGB(image, color);
        }

        private void SwapChannels(Bitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bitmap.GetPixel(x, y);
                }
            }
        }
    }
}
