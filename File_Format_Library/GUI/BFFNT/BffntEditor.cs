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

        private Image PanelImage { get; set; }

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

            ReloadCharacterCodes();
            ReloadTextures();
        }

        private void ReloadCharacterCodes()
        {
            foreach (char entry in ActiveFile.FontSection.CodeMapDictionary.Keys)
                characterCodeCB.Items.Add(entry);

            if (ActiveFile.FontSection.CodeMapDictionary.Count > 0)
                characterCodeCB.SelectedIndex = 0;
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
                    PanelImage = image.GetBitmap(ImageIndex);
                }
                else
                {
                    PanelImage = image.GetBitmap();
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle ee = new Rectangle(10, 10, 30, 30);
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, ee);
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

        public class FontCell
        {
            public Rectangle DrawnRectangle;

            public Color Color { get; set; }

            public FontCell()
            {
                Color = Color.Cyan;
            }
        }

        private FontCell[] FontCells;

        private void imagePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawImage(PanelImage, 0.0f, 0.0f);

            if (ActiveFile == null)
                return;

            var textureGlyph = ActiveFile.FontSection.TextureGlyph;

            FontCells = new FontCell[textureGlyph.ColumnCount * textureGlyph.RowCount];

            int CellPosY = 0;
            for (int c = 0; c < (int)textureGlyph.ColumnCount; c++)
            {
                int CellPosX = 0;
                for (int r = 0; r < (int)textureGlyph.RowCount; r++)
                {
                    int Index = c + r;

                    FontCells[Index] = new FontCell();
                    FontCells[Index].DrawnRectangle = new Rectangle()
                    {
                        X = CellPosX,
                        Y = CellPosY,
                        Width = (int)textureGlyph.CellWidth,
                        Height = (int)textureGlyph.CellHeight,
                    };

                    graphics.DrawRectangle(new Pen(FontCells[Index].Color), FontCells[Index].DrawnRectangle);
                    CellPosX += (int)textureGlyph.CellWidth;
                }
                CellPosY += (int)textureGlyph.CellHeight;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PanelImage != null)
                Clipboard.SetImage(PanelImage);
        }
    }
}
