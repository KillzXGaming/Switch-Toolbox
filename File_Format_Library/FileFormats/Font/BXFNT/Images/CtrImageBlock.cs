using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Toolbox.Library;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class CtrImageBlock : STGenericTexture
    {
        public TGLP TextureTGLP;

        public int SheetIndex = 0;

        public void Load(TGLP texture, int Index)
        {
            CanReplace = true;

            SheetIndex = Index;
            TextureTGLP = texture;
            Height = TextureTGLP.SheetHeight;
            Width = TextureTGLP.SheetWidth;
            var BFNTFormat = (CTR_3DS.PICASurfaceFormat)TextureTGLP.Format;
            Format = CTR_3DS.ConvertPICAToGenericFormat(BFNTFormat);

            if (Format == TEX_FORMAT.A4) {
                RedChannel = STChannelType.Alpha;
                GreenChannel = STChannelType.Alpha;
                BlueChannel = STChannelType.Alpha;
                AlphaChannel = STChannelType.Alpha;
            }

            PlatformSwizzle = PlatformSwizzle.Platform_3DS;

            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }

        public override bool CanEdit { get; set; } = true;
        public override string ExportFilter => FileFilters.CTR_TEX;
        public override string ReplaceFilter => FileFilters.CTR_TEX;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
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

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {

        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return TextureTGLP.SheetDataList[SheetIndex];
        }

        public override void OnClick(TreeView treeview)
        {
            UpdateEditor();
        }

        public override void Replace(string FileName)
        {
            CTR_3DSTextureImporter importer = new CTR_3DSTextureImporter();
            CTR_3DSImporterSettings settings = new CTR_3DSImporterSettings();

            settings.LoadBitMap(FileName);
            importer.LoadSettings(new List<CTR_3DSImporterSettings>() { settings, });
            settings.MipCount = 1;
            settings.Format = CTR_3DS.ConvertToPICAFormat(Format);

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

        private void ApplySettings(CTR_3DSImporterSettings settings)
        {
            if (this.Width != settings.TexWidth)
                throw new Exception("The image should be the same width as the original!");
            if (this.Height != settings.TexHeight)
                throw new Exception("The image should be the same height as the original!");

            this.TextureTGLP.SheetDataList[SheetIndex] = settings.DataBlockOutput[0];
            this.TextureTGLP.Format = (ushort)CTR_3DS.ConvertToPICAFormat(settings.GenericFormat);
            this.Format = settings.GenericFormat;
            this.MipCount = settings.MipCount;
            this.Depth = settings.Depth;
            this.ArrayCount = (uint)settings.DataBlockOutput.Count;
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

            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)TextureTGLP.SheetDataList[SheetIndex].Length;
            prop.Format = Format;

            editor.Text = Text;
            editor.LoadProperties(prop);
            editor.LoadImage(this);
        }
    }

}
