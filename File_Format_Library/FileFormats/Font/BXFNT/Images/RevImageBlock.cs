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
    public class RevImageBlock : STGenericTexture
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
            var BFNTFormat = (Decode_Gamecube.TextureFormats)TextureTGLP.Format;
            Format = Decode_Gamecube.ToGenericFormat(BFNTFormat);

            if (Format == TEX_FORMAT.A4)
            {
                AlphaChannel = STChannelType.Red;
            }

            PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;

            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }

        public override bool CanEdit { get; set; } = true;
        public override string ExportFilter => FileFilters.REV_TEX;
        public override string ReplaceFilter => FileFilters.REV_TEX;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
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

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
          
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return TextureTGLP.SheetDataList[SheetIndex];
        }

        public override void Replace(string FileName)
        {
            GamecubeTextureImporterList importer = new GamecubeTextureImporterList(SupportedFormats);
            GameCubeTextureImporterSettings settings = new GameCubeTextureImporterSettings();

            importer.ForceMipCount = true;
            importer.SelectedMipCount = 1;

            settings.LoadBitMap(FileName);
            importer.LoadSettings(new List<GameCubeTextureImporterSettings>() { settings, });
            settings.MipCount = 1;
            settings.Format = Decode_Gamecube.FromGenericFormat(Format);

            if (importer.ShowDialog() == DialogResult.OK)
            {
                if (settings.GenerateMipmaps && !settings.IsFinishedCompressing)
                    settings.Compress();

                ApplySettings(settings);
                UpdateEditor();
            }
        }

        private void ApplySettings(GameCubeTextureImporterSettings settings)
        {
            if (this.Width != settings.TexWidth)
                throw new Exception("The image should be the same width as the original!");
            if (this.Height != settings.TexHeight)
                throw new Exception("The image should be the same height as the original!");

            this.TextureTGLP.SheetDataList[SheetIndex] = settings.DataBlockOutput[0];
            this.TextureTGLP.Format = (ushort)Decode_Gamecube.FromGenericFormat(settings.GenericFormat);
            this.Format = settings.GenericFormat;
            this.MipCount = 1; //Always 1
            this.Depth = 1;
            this.ArrayCount = (uint)settings.DataBlockOutput.Count;
        }

        public override void OnClick(TreeView treeview)
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
