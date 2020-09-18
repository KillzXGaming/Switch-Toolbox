using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using System.ComponentModel;

namespace FirstPlugin
{
    public class TPL : TreeNodeFile, IFileFormat,  ITextureContainer
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TPL" };
        public string[] Extension { get; set; } = new string[] { "*.tpl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                uint magic = reader.ReadUInt32();
                return magic == 0x0020AF30 || magic == 0x30AF2000 || Utils.GetExtension(FileName) == ".tpl";
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public bool DisplayIcons => true;

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


        public override void OnAfterAdded()
        {
            if (Nodes.Count > 0 && this.TreeView != null)
               this.TreeView.SelectedNode = Nodes[0];
        }

        public static TPL CreateNewFromImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = FileFilters.REV_TEX;
            if (ofd.ShowDialog() != DialogResult.OK)
                return null;

            TPL tpl = new TPL();
            tpl.IFileInfo = new IFileInfo();
            tpl.FileName = Path.GetFileNameWithoutExtension(ofd.FileName) + ".tpl";
            tpl.Header = new TPL_IO.Header();
            var image = TplTextureWrapper.Create(tpl, tpl.Header, ofd.FileName);
            if (image == null) //Operation cancelled
                return null;

            tpl.Nodes.Add(image);
            return tpl;
        }

        private TPL_IO.Header Header;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = true;

            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                Header = new TPL_IO.Header();
                Header.Read(reader);

                foreach (var tex in Header.Images) {
                    var texWrapper = new TplTextureWrapper(this, tex.Header);
                    texWrapper.PaletteHeader = tex.PaletteHeader;
                    texWrapper.ImageKey = "Texture";
                    texWrapper.SelectedImageKey = "Texture";
                    texWrapper.Format = Decode_Gamecube.ToGenericFormat(tex.Header.Format);
                    texWrapper.Width = tex.Header.Width;
                    texWrapper.Height = tex.Header.Height;
                    texWrapper.ImageData = tex.ImageData;
                    texWrapper.MipCount = tex.Header.MaxLOD;
                    texWrapper.PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;
                    Nodes.Add(texWrapper);

                    if (tex.PaletteHeader != null)
                    {
                        var GXPaletteFormat = (Decode_Gamecube.PaletteFormats)tex.PaletteHeader.PaletteFormat;
                        texWrapper.SetPaletteData(tex.PaletteHeader.Data, Decode_Gamecube.ToGenericPaletteFormat(GXPaletteFormat));
                    }
                }

                UpdateDisplayedNames();
            }
        }

        private void UpdateDisplayedNames() {
            string name = System.IO.Path.GetFileNameWithoutExtension(FileName);

            int i = 0;
            foreach (TplTextureWrapper tex in Nodes)
            {
                if (Header.Images.Count == 1)
                    tex.Text = name;
                else
                    tex.Text = $"{name}_{i++}";
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream)) {
                writer.SetByteOrder(true);
                Header.Write(writer);
            }
        }

        public class TplTextureWrapper : STGenericTexture
        {
            public TPL TPLParent { get; set; }

            public byte[] ImageData { get; set; }
            public TPL_IO.TPLImageHeader ImageHeader;
            public TPL_IO.PaletteHeader PaletteHeader;

            public TplTextureWrapper() { }

            public TplTextureWrapper(TPL tpl) {
                TPLParent = tpl;
            }

            public TplTextureWrapper(TPL tpl, TPL_IO.TPLImageHeader header) {
                TPLParent = tpl;
                ImageHeader = header;

                CanReplace = true;
            }

            public override string ExportFilter => FileFilters.REV_TEX;
            public override string ReplaceFilter => FileFilters.REV_TEX;

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                    TEX_FORMAT.I4,
                    TEX_FORMAT.I8,
                    TEX_FORMAT.IA4,
                    TEX_FORMAT.IA8,
                    TEX_FORMAT.RGB565,
                    TEX_FORMAT.RGB5A3,
                    TEX_FORMAT.RGBA32,
                    TEX_FORMAT.C4,
                    TEX_FORMAT.C8,
                    TEX_FORMAT.C14X2,
                    TEX_FORMAT.CMPR,
                    };
                }
            }


            public override bool CanEdit { get; set; } = true;

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
            {
                return Decode_Gamecube.GetMipLevel(ImageData, Width, Height, MipCount, (uint)MipLevel, Format);
            }

            public override void OnClick(TreeView treeView)
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                editor.LoadProperties(ImageHeader);
                editor.LoadImage(this);
            }

            public void UpdateEditor()
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }

            public static TplTextureWrapper Create(TPL tpl, TPL_IO.Header header, string FileName)
            {
                TplTextureWrapper tex = new TplTextureWrapper(tpl);
                tex.Replace(FileName);
                if (tex.ImageData == null) //Dialog cancelled if image data not set
                    return null;
                tex.ImageHeader.Width = (ushort)tex.Width;
                tex.ImageHeader.Height = (ushort)tex.Height;
                tex.ImageHeader.Format =  Decode_Gamecube.FromGenericFormat(tex.Format);
                tex.ImageHeader.MaxLOD = (byte)tex.MipCount;
                header.Images.Add(new TPL_IO.ImageEntry()
                {
                    Header= tex.ImageHeader,
                    ImageData = tex.ImageData,
                    PaletteHeader = tex.PaletteHeader,
                });
                return tex;
            }

            public override void Replace(string FileName)
            {
                GamecubeTextureImporterList importer = new GamecubeTextureImporterList(SupportedFormats);
                GameCubeTextureImporterSettings settings = new GameCubeTextureImporterSettings();

                importer.ForceMipCount = true;
                if (MipCount == 1)
                    importer.SelectedMipCount = 1;

                if (Utils.GetExtension(FileName) == ".dds" ||
                    Utils.GetExtension(FileName) == ".dds2")
                {
                    settings.LoadDDS(FileName);
                    importer.LoadSettings(new List<GameCubeTextureImporterSettings>() { settings, });

                    ApplySettings(settings);
                    UpdateEditor();
                }
                else
                {
                    settings.LoadBitMap(FileName);
                    importer.LoadSettings(new List<GameCubeTextureImporterSettings>() { settings, });

                    if (importer.ShowDialog() == DialogResult.OK)
                    {
                        if (settings.GenerateMipmaps && !settings.IsFinishedCompressing)
                            settings.Compress();

                        ApplySettings(settings);
                        UpdateEditor();
                    }
                }
            }

            private void ApplySettings(GameCubeTextureImporterSettings settings)
            {
                if (ImageHeader == null)
                    ImageHeader = new TPL_IO.ImageHeaderV2();

                this.ImageData = settings.DataBlockOutput[0];
                this.Width = settings.TexWidth;
                this.Height = settings.TexHeight;
                this.Format = settings.GenericFormat;
                this.MipCount = settings.MipCount; 
                this.Depth = 1; //Always 1
                this.ArrayCount = 1;
                UpdateHeader();

                if (this.RenderableTex != null)
                    this.LoadOpenGLTexture();
            }

            private void UpdateHeader() {
                ImageHeader.Width = (ushort)Width;
                ImageHeader.Height = (ushort)Height;
                ImageHeader.MaxLOD = MipCount == 1 ? (byte)0 : (byte)MipCount;
                ImageHeader.Format = Decode_Gamecube.FromGenericFormat(Format);
            }
        }

        public void Unload()
        {

        }
    }
}
