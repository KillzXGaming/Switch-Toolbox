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

namespace FirstPlugin
{
    public class MT_TEX : STGenericTexture, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TEX (3DS)" };
        public string[] Extension { get; set; } = new string[] { "*.tex" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true)) {
                return reader.CheckSignature(3, "TEX");
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

        public override bool CanEdit { get; set; } = false;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.B5G6R5_UNORM,
                    TEX_FORMAT.R8G8_UNORM,
                    TEX_FORMAT.B5G5R5A1_UNORM,
                    TEX_FORMAT.B4G4R4A4_UNORM,
                    TEX_FORMAT.LA8,
                    TEX_FORMAT.HIL08,
                    TEX_FORMAT.L8,
                    TEX_FORMAT.A8_UNORM,
                    TEX_FORMAT.LA4,
                    TEX_FORMAT.A4,
                    TEX_FORMAT.ETC1_UNORM,
                    TEX_FORMAT.ETC1_A4,
            };
            }
        }

        public byte[] ImageData;


        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }

        private void UpdateEditor()
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

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            Items.AddRange(base.GetContextMenuItems());
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public void FillEditor(UserControl control)
        {
            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)ImageData.Length;
            prop.Format = Format;

            ((ImageEditorBase)control).LoadImage(this);
            ((ImageEditorBase)control).LoadProperties(prop);
        }

        public void Load(System.IO.Stream stream)
        {
            PlatformSwizzle = PlatformSwizzle.Platform_Switch;
            CanSave = true;
            CanReplace = true;
            Text = FileName;
            this.ImageKey = "texture";
            this.SelectedImageKey = "texture";

            using (var reader = new FileReader(stream))
            {
                reader.ReadInt32(); //magic
                int block1 = reader.ReadInt32();
                uint block2 = reader.ReadUInt32();
                uint block3 = reader.ReadUInt32();

                int Version = (block1 >> 0) & 0xfff;
                int Shift = (block1 >> 24) & 0xf;
                Width = (block2 >> 6) & 0x1fff;
                Height = (block2 >> 19) & 0x1fff;
                uint format = (block3 >> 8) & 0xff;
                uint Aspect = (block3 >> 16) & 0x1fff;
                MipCount = (block2 & 0x3F);
                uint imageSize = reader.ReadUInt32();
                reader.ReadUInt32s((int)MipCount); //mip offsets

                ImageData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

                Format = FormatList[format];
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream, true))
            {
            }
        }

        public override void Replace(string FileName)
        {
     
        }


        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {
         
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, DepthLevel);
        }

        Dictionary<uint, TEX_FORMAT> FormatList = new Dictionary<uint, TEX_FORMAT>()
            {
                {  7, TEX_FORMAT.R8G8B8A8_UNORM_SRGB },

                {  19, TEX_FORMAT.BC1_UNORM_SRGB },
                {  20, TEX_FORMAT.BC2_UNORM_SRGB },
                {  23, TEX_FORMAT.BC3_UNORM_SRGB },
                {  25, TEX_FORMAT.BC4_UNORM },
                {  31, TEX_FORMAT.BC5_UNORM },

                {  33, TEX_FORMAT.BC1_UNORM },
                {  39, TEX_FORMAT.BC3_UNORM },
                {  42, TEX_FORMAT.BC3_UNORM },

                {  48, TEX_FORMAT.BC7_UNORM },
            };
    }
}
