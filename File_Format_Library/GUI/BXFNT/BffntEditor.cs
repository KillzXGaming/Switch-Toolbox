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
using Toolbox.Library;
using LibEveryFileExplorer.GFX;
using Toolbox.Library.IO;

namespace FirstPlugin.Forms
{
    public partial class BffntEditor : STUserControl, IFIleEditor
    {
        public EventHandler OnFontEdited = null;

        public BffntEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            pictureBoxCustom1.BackColor = Color.Black;
            pictureBoxCustom1.BackgroundImage = null;
        }

        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { FileFormat };
        }

        public void BeforeFileSaved() { }

        private Image PanelImage { get; set; }
        private BitmapFont bitmapFont;

        private FFNT ActiveFile;
        private BXFNT FileFormat;

        public void LoadFontFile(BXFNT fontFile)
        {
            FileFormat = fontFile;
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

            try {
                bitmapFont = fontFile.bffnt.GetBitmapFont();
            }
            catch
            {

            }

            textPreviewTB.Text = "This is a preview!";

            if (bitmapFont != null)
                pictureBoxCustom1.Image = bitmapFont.PrintToBitmap(textPreviewTB.Text, new BitmapFont.FontRenderSettings());
        }

        public override void OnControlClosing()
        {
            bitmapFont?.Dispose();
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

        private void imagePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (ActiveFile == null) return;

            int ImageIndex = imagesCB.SelectedIndex;

                if (e.Button == MouseButtons.Right && ImageIndex != -1)
                {
                    var image = ActiveFile.FontSection.TextureGlyph.GetImageSheet(ImageIndex);

                imageMenuStrip.Items.Clear();
                imageMenuStrip.Items.Add(new ToolStripMenuItem("Export", null, ExportImageAction, Keys.Control | Keys.E));
                imageMenuStrip.Items.Add(new ToolStripMenuItem("Replace", null, ReplaceImageAction, Keys.Control | Keys.R));
                imageMenuStrip.Items.Add(new ToolStripMenuItem("Copy", null, CopyImageAction, Keys.Control | Keys.C));
                imageMenuStrip.Items.Add(new ToolStripMenuItem("Open Image Editor", null, ImageEditorAction, Keys.Control | Keys.E));
                imageMenuStrip.Show(Cursor.Position);
            }
        }

        private void imagesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ImageIndex = imagesCB.SelectedIndex;
            if (ImageIndex != -1)
                UpdateImagePanel(ImageIndex);
        }

        private void UpdateImagePanel(int ImageIndex)
        {
            var image = ActiveFile.FontSection.TextureGlyph.GetImageSheet(ImageIndex);
            bool IsBntx = ActiveFile.FontSection.TextureGlyph.BinaryTextureFile != null;

            if (IsBntx)
                PanelImage = image.GetComponentBitmap(image.GetBitmap(ImageIndex));
            else
                PanelImage = image.GetComponentBitmap(image.GetBitmap());

            if (PanelImage != null && ActiveFile.Platform >= FFNT.PlatformType.Cafe)
                PanelImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            FillCells();

            imagePanel.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle ee = new Rectangle(10, 10, 30, 30);
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, ee);
            }
        }

        private void ImageEditorAction(object sender, EventArgs e)
        {
            int ImageIndex = imagesCB.SelectedIndex;
            if (ImageIndex != -1)
            {
                var image = ActiveFile.FontSection.TextureGlyph.GetImageSheet(ImageIndex);
                bool IsBntx = ActiveFile.FontSection.TextureGlyph.BinaryTextureFile != null;

                ImageEditorForm form = new ImageEditorForm(true);
                form.editorBase.Text = Text;
                form.editorBase.Dock = DockStyle.Fill;

                if (ActiveFile.Platform >= FFNT.PlatformType.Cafe)
                    image.Parameters.FlipY = true;

                if (IsBntx)
                {
                    form.editorBase.LoadProperties(((TextureData)image).Texture);
                    form.editorBase.LoadImage(image, ImageIndex);
                }
                else
                {
                    form.editorBase.LoadProperties(image.GenericProperties);
                    form.editorBase.LoadImage(image);
                }

                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateImagePanel(ImageIndex);
                }
            }
        }
        
        private void ReplaceImageAction(object sender, EventArgs e)
        {
            int ImageIndex = imagesCB.SelectedIndex;
            if (ImageIndex != -1)
            {
                var image = ActiveFile.FontSection.TextureGlyph.GetImageSheet(ImageIndex);
                bool IsBntx = ActiveFile.FontSection.TextureGlyph.BinaryTextureFile != null;

                image.Parameters.FlipY = true;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = image.ReplaceFilter;
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (IsBntx)
                        ((TextureData)image).Replace(ofd.FileName, 1, (uint)ImageIndex, image.Format, ((TextureData)image).Texture.SurfaceDim);
                    else
                        image.Replace(ofd.FileName);
                }

                UpdateImagePanel(ImageIndex);
                OnFontEdited?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ExportImageAction(object sender, EventArgs e)
        {
            int ImageIndex = imagesCB.SelectedIndex;
            if (ImageIndex != -1)
            {
                var image = ActiveFile.FontSection.TextureGlyph.GetImageSheet(ImageIndex);
                bool IsBntx = ActiveFile.FontSection.TextureGlyph.BinaryTextureFile != null;

                if (ActiveFile.Platform >= FFNT.PlatformType.Cafe)
                    image.Parameters.FlipY = true;

                if (IsBntx)
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

            public bool IsHit(int X, int Y)
            {
                if (DrawnRectangle == null) return false;

                if ((X > DrawnRectangle.X) && (X < DrawnRectangle.X + DrawnRectangle.Width) &&
                    (Y > DrawnRectangle.Y) && (Y < DrawnRectangle.Y + DrawnRectangle.Height))
                    return true;
                else
                    return false;
            }

            public bool IsSelected { get; private set; }

            public void Select()
            {
                Color = Color.Blue;
                IsSelected = true;
            }

            public void Unselect()
            {
                Color = Color.Cyan;
                IsSelected = false;
            }
        }

        private FontCell[] FontCells;

        public GlyphImage[] GlyphImages;

        public class GlyphImage
        {
            public Image Image { get; set; }
        }

        private void FillCells()
        {
            List<GlyphImage> images = new List<GlyphImage>();
            List<FontCell> Cells = new List<FontCell>();

            var textureGlyph = ActiveFile.FontSection.TextureGlyph;
            var fontSection = ActiveFile.FontSection;

            PanelImage = BitmapExtension.Resize(PanelImage, textureGlyph.SheetWidth, textureGlyph.SheetHeight);

            for (int c = 0; c < (int)textureGlyph.ColumnCount; c++)
            {
                for (int r = 0; r < (int)textureGlyph.RowCount; r++)
                {
                    int x = r * (textureGlyph.CellWidth + 1);
                    int y = c * (textureGlyph.CellHeight + 1);

                    var rect = new Rectangle(x, y, textureGlyph.CellWidth, textureGlyph.CellHeight);

                    Cells.Add(new FontCell()
                    {
                        DrawnRectangle = rect,
                    });

                 /*   var glyphImage = new GlyphImage();
                    glyphImage.Image = CopyRegionIntoImage(bitmap, rect);
                    glyphImage.Image.Save($"Glpyh{c} {r}.png");
                    images.Add(glyphImage);*/

                }
            }

            GlyphImages = images.ToArray();
            FontCells = Cells.ToArray();
        }

        private static Bitmap CopyRegionIntoImage(Image srcBitmap, Rectangle srcRegion)
        {
            Bitmap destBitmap = new Bitmap(srcRegion.Width, srcRegion.Height);
            using (Graphics grD = Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, new Rectangle(0,0,destBitmap.Width, destBitmap.Height), srcRegion, GraphicsUnit.Pixel);
            }
            return destBitmap;
        }

        private void imagePanel_Paint(object sender, PaintEventArgs e)
        {
            if (PanelImage == null)
                return;

            Graphics graphics = e.Graphics;

            graphics.Clear(Color.FromArgb(30,30,30));
            graphics.DrawImage(PanelImage, 0.0f, 0.0f);

            if (ActiveFile == null)
                return;

            var textureGlyph = ActiveFile.FontSection.TextureGlyph;

             if (FontCells == null)
                return;

            for (int i = 0; i < FontCells.Length; i++) {
                if (FontCells[i].IsSelected)
                {
                    SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(70, 0, 255, 255));

                    graphics.DrawRectangle(new Pen(FontCells[i].Color, 1), FontCells[i].DrawnRectangle);
                    graphics.FillRectangle(semiTransBrush, FontCells[i].DrawnRectangle);
                }
            }

            graphics.ScaleTransform(textureGlyph.SheetWidth, textureGlyph.SheetHeight);
        }

        private void CopyImageAction(object sender, EventArgs e)
        {
            if (PanelImage != null)
                Clipboard.SetImage(PanelImage);
        }

        bool isMouseDown = false;
        private void imagePanel_MouseDown(object sender, MouseEventArgs e) {
            isMouseDown = true;
        }

        private void imagePanel_MouseUp(object sender, MouseEventArgs e) {
            isMouseDown = false;
        }

        private void imagePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (FontCells != null)
            {
                for (int i = 0; i < FontCells.Length; i++)
                {
                    if (FontCells[i] == null) continue;

                    if (FontCells[i].IsHit(e.X, e.Y))
                        FontCells[i].Select();
                    else 
                        FontCells[i].Unselect();
                }

                imagePanel.Refresh();
            }
        }

        private void imagePanel_MouseLeave(object sender, EventArgs e)
        {
            isMouseDown = false;

            if (FontCells != null)
            {
                for (int i = 0; i < FontCells.Length; i++)
                    FontCells[i].Unselect();

                imagePanel.Refresh();
            }
        }

        private void textPreviewTB_TextChanged(object sender, EventArgs e)
        {
            if (bitmapFont != null)
                pictureBoxCustom1.Image = bitmapFont.PrintToBitmap(textPreviewTB.Text, new BitmapFont.FontRenderSettings());
        }
    }

    public class ImagePaenl : STPanel
    {
        public ImagePaenl()
        {
            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer,
            true);
        }
    }

}
