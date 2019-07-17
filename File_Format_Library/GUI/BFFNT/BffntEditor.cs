using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class BffntEditor : STUserControl
    {
        public BffntEditor()
        {
            InitializeComponent();
        }

        private FFNT ActiveFile;
        public void LoadFontFile(BFFNT fontFile)
        {
            ActiveFile = fontFile.bffnt;

            fontTypeCB.Bind(typeof(FINF.FontType), ActiveFile.FontSection, "Type");
            fontTypeCB.SelectedItem = ActiveFile.FontSection.Type;

            encodingTypeCB.Bind(typeof(FINF.CharacterCode), ActiveFile.FontSection, "CharEncoding");
            encodingTypeCB.SelectedItem = ActiveFile.FontSection.CharEncoding;

            lineFeedUD.Bind(ActiveFile.FontSection, "LineFeed");
            leftSpacingUD.Bind(ActiveFile.FontSection, "DefaultLeftWidth");
            charWidthUD.Bind(ActiveFile.FontSection, "DefaultCharWidth");
            glyphWidthCB.Bind(ActiveFile.FontSection, "DefaultGlyphWidth");
            ascentUD.Bind(ActiveFile.FontSection, "Ascent");
            fontWidthUD.Bind(ActiveFile.FontSection, "Width");
            fontHeightUD.Bind(ActiveFile.FontSection, "Height");

            ReloadTextures();
        }

        private void ReloadTextures()
        {
            imagesCB.Items.Clear();
            var textureGlyph = ActiveFile.FontSection.TextureGlyph;
            for (int i = 0; i < textureGlyph.SheetCount; i++)
                imagesCB.Items.Add($"Image {i}");

            if (textureGlyph.SheetCount > 0)
                imagesCB.SelectedIndex = 0;
        }

        private void imagesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ImageIndex = imagesCB.SelectedIndex;
            if (ImageIndex != -1)
            {
                var image = ActiveFile.FontSection.TextureGlyph.GetImageSheet(ImageIndex);
                bool IsBntx = ActiveFile.FontSection.TextureGlyph.BinaryTextureFile != null;

                if (IsBntx)
                {
                    pictureBoxCustom1.Image = image.GetBitmap(ImageIndex);
                }
                else
                {
                    pictureBoxCustom1.Image = image.GetBitmap();
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ImageIndex = imagesCB.SelectedIndex;
            if (ImageIndex != -1)
            {
                var image = ActiveFile.FontSection.TextureGlyph.GetImageSheet(ImageIndex);
                bool IsBntx = ActiveFile.FontSection.TextureGlyph.BinaryTextureFile != null;

                if(IsBntx)
                    image.ExportArrayImage(ImageIndex);
                else
                    image.ExportImage();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBoxCustom1.Image != null)
                Clipboard.SetImage(pictureBoxCustom1.Image);
        }
    }
}
