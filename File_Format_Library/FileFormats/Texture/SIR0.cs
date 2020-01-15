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
    public class SIR0 : STGenericTexture, IFileFormat, IContextMenuNode
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
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4,"SIR0");
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
            PlatformSwizzle = PlatformSwizzle.Platform_3DS;
            CanSave = true;
            CanReplace = true;

            using (var reader = new FileReader(stream))
            {
                Width = reader.ReadUInt32();
                Height = reader.ReadUInt32();
                byte FormatCtr = reader.ReadByte();
                MipCount = reader.ReadByte();
                ushort padding = reader.ReadUInt16();
                Text = reader.ReadZeroTerminatedString();
                Format = CTR_3DS.ConvertPICAToGenericFormat((CTR_3DS.PICASurfaceFormat)FormatCtr);

                reader.Position = 0x80;
                ImageData = reader.ReadBytes((int)reader.BaseStream.Length - 0x80);
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream, true))
            {
                writer.Write(Width);
                writer.Write(Height);
                writer.Write((byte)CTR_3DS.ConvertToPICAFormat(Format));
                writer.Write((byte)MipCount);
                writer.Write((ushort)0); //Padding
                writer.WriteString(Text);

                writer.Position = 0x80;
                writer.Write(ImageData);
            }
        }

        public override void Replace(string FileName)
        {
            CTR_3DSTextureImporter importer = new CTR_3DSTextureImporter();
            CTR_3DSImporterSettings settings = new CTR_3DSImporterSettings();

            if (Utils.GetExtension(FileName) == ".dds" ||
                Utils.GetExtension(FileName) == ".dds2")
            {
                settings.LoadDDS(FileName);
                importer.LoadSettings(new List<CTR_3DSImporterSettings>() { settings, });

                ApplySettings(settings);
                UpdateEditor();
            }
            else
            {
                settings.LoadBitMap(FileName);
                importer.LoadSettings(new List<CTR_3DSImporterSettings>() { settings, });

                if (importer.ShowDialog() == DialogResult.OK)
                {
                    if (settings.GenerateMipmaps && !settings.IsFinishedCompressing)
                    {
                        settings.DataBlockOutput.Clear();
                        settings.DataBlockOutput.Add(settings.GenerateMips());
                    }

                    ApplySettings(settings);
                    UpdateEditor();
                }
            }
        }

        private void ApplySettings(CTR_3DSImporterSettings settings)
        {
            this.ImageData = settings.DataBlockOutput[0];
            this.Width = settings.TexWidth;
            this.Height = settings.TexHeight;
            this.Format = settings.GenericFormat;
            this.MipCount = settings.MipCount;
            this.Depth = settings.Depth;
            this.ArrayCount = (uint)settings.DataBlockOutput.Count;
        }

        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {

        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return ImageData;
        }
    }
}
