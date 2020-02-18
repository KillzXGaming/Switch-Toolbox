using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Toolbox.Library.IO;

namespace Toolbox.Library.Forms
{
    public class VertexColorBox : STPanel
    {
        public EventHandler OnColorChanged;

        private Color TLColor;
        private Color TRColor;
        private Color BLColor;
        private Color BRColor;

        private Color AllColor
        {
            get
            {
                Color[] colors = new Color[4] 
                { TLColor, TRColor, BLColor, BRColor };

                return Color.FromArgb(
                    (int)colors.Average(a => a.A),
                    (int)colors.Average(a => a.R),
                    (int)colors.Average(a => a.G),
                    (int)colors.Average(a => a.B));
            }
        }

        private bool SuppressChangeEvent = false;

        public Color TopLeftColor
        {
            get { return TLColor; }
            set
            {
                TLColor = value;
                ColorChanged();
            }
        }

        public Color TopRightColor
        {
            get { return TRColor; }
            set
            {
                TRColor = value;
                ColorChanged();
            }
        }

        public Color BottomLeftColor
        {
            get { return BLColor; }
            set
            {
                BLColor = value;
                ColorChanged();
            }
        }

        public Color BottomRightColor
        {
            get { return BRColor; }
            set
            {
                BRColor = value;
                ColorChanged();
            }
        }

        public void ColorChanged()
        {
            this.Invalidate();
            OnColorChanged?.Invoke(this, new EventArgs());
        }

        public VertexColorBox()
        {
            this.BackgroundImage = Properties.Resources.CheckerBackground;
            this.BackColor = Color.Transparent;
            this.MouseClick += OnMouseClick;
            this.MouseMove += OnMouseMove;
            this.MouseLeave += OnMouseLeave;
            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer,
            true);
        }

        public void DisposeControl()
        {
            if (colorDlg != null)
            {
                colorDlg.Close();
                colorDlg = null;
            }
        }


        private void OnMouseMove(object sender, MouseEventArgs e) {
            if (dialogActive) return;

            mouseLoc = e.Location;
            Invalidate();
        }

        private void OnMouseLeave(object sender, EventArgs e) {
            if (dialogActive) return;

            mouseLoc = Point.Empty;
            Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e) {
        }

        private bool dialogActive;
        private STColorDialog colorDlg;
        private Point mouseLoc = Point.Empty;
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (TopLeftHit == null) return;

            if (e.Button == MouseButtons.Left)
            {
                if (!dialogActive) {
                    mouseLoc = e.Location;
                    this.Invalidate();
                }
      

                if (TopLeftHit.IsHit(e.Location))
                     LoadColorDialog(TopLeftColor, 0);
                else if (TopRightHit.IsHit(e.Location))
                    LoadColorDialog(TopRightColor, 1);
                else if (BottomLeftHit.IsHit(e.Location))
                    LoadColorDialog(BottomLeftColor, 2);
                else if (BottomRightHit.IsHit(e.Location))
                    LoadColorDialog(BottomRightColor, 3);
                else if (TopHit.IsHit(e.Location))
                    LoadColorDialogSides(TopRightColor, TopLeftColor,  0);
                else if (BottomHit.IsHit(e.Location))
                    LoadColorDialogSides(BottomRightColor, BottomLeftColor, 1);
                else if (RightHit.IsHit(e.Location))
                    LoadColorDialogSides(TopRightColor, BottomRightColor, 2);
                else if (LeftHit.IsHit(e.Location))
                    LoadColorDialogSides(TopLeftColor, BottomLeftColor, 3);
                else if (AllHit.IsHit(e.Location))
                {
                    if (dialogActive)
                    {
                        colorDlg.Focus();
                        return;
                    }

                    dialogActive = true;
                    colorDlg = new STColorDialog(AllColor);
                    colorDlg.Show();
                    colorDlg.FormClosed += delegate
                    {
                        mouseLoc = Point.Empty;
                        dialogActive = false;
                        this.Invalidate();
                    };
                    colorDlg.ColorChanged += delegate
                    {
                        //Update only once!
                        SuppressChangeEvent = true;

                        TopLeftColor = colorDlg.NewColor;
                        TopRightColor = colorDlg.NewColor;
                        BottomLeftColor = colorDlg.NewColor;
                        BottomRightColor = colorDlg.NewColor;

                        SuppressChangeEvent = false;
                        ColorChanged();
                    };
                }
            }
        }

        private void LoadColorDialogSides(Color color1, Color color2, int index)
        {
            if (dialogActive)
            {
                colorDlg.Focus();
                return;
            }

            Color[] colors = new Color[2]
             { color1, color2 };

            var color = Color.FromArgb(
                    (int)colors.Average(a => a.A),
                    (int)colors.Average(a => a.R),
                    (int)colors.Average(a => a.G),
                    (int)colors.Average(a => a.B));

            dialogActive = true;
            colorDlg = new STColorDialog(color);
            colorDlg.FormClosed += delegate
            {
                mouseLoc = Point.Empty;
                dialogActive = false;
                this.Invalidate();
            };
            colorDlg.Show();
            colorDlg.ColorChanged += delegate
            {
                if (index == 0)
                {
                    TopRightColor = colorDlg.NewColor;
                    TopLeftColor = colorDlg.NewColor;
                }
                if (index == 1)
                {
                    BottomRightColor = colorDlg.NewColor;
                    BottomLeftColor = colorDlg.NewColor;
                }
                if (index == 2)
                {
                    TopRightColor = colorDlg.NewColor;
                    BottomRightColor = colorDlg.NewColor;
                }
                if (index == 3)
                {
                    TopLeftColor = colorDlg.NewColor;
                    BottomLeftColor = colorDlg.NewColor;
                }
            };
        }

        private void LoadColorDialog(Color color, int index)
        {
            if (dialogActive)
            {
                colorDlg.Focus();
                return;
            }

            dialogActive = true;
            colorDlg = new STColorDialog(color);
            colorDlg.FormClosed += delegate
            {
                mouseLoc = Point.Empty;
                dialogActive = false;
                this.Invalidate();
            };
            colorDlg.Show();
            colorDlg.ColorChanged += delegate
            {
                if (index == 0)
                    TopLeftColor = colorDlg.NewColor;
                if (index == 1)
                    TopRightColor = colorDlg.NewColor;
                if (index == 2)
                    BottomLeftColor = colorDlg.NewColor;
                if (index == 3)
                    BottomRightColor = colorDlg.NewColor;
            };
        }

        private Rectangle TopLeftHit;
        private Rectangle TopRightHit;
        private Rectangle BottomLeftHit;
        private Rectangle BottomRightHit;
        private Rectangle TopHit;
        private Rectangle RightHit;
        private Rectangle LeftHit;
        private Rectangle BottomHit;
        private Rectangle AllHit;

        private bool DisplayHitboxes = false;

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode.Bilinear;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            LinearGradientBrush linearGradientBrush =
                   new LinearGradientBrush(ClientRectangle, TopLeftColor, BottomRightColor, 0f, true);

            ColorBlend cblend = new ColorBlend(3);
            cblend.Colors = new Color[4] { TopLeftColor, TopRightColor, BottomLeftColor, BottomRightColor };
            cblend.Positions = new float[4] { 0f, 0.5f, 0.5f, 1f };
            linearGradientBrush.InterpolationColors = cblend;

            Color c1 = TopLeftColor;
            Color c2 = BottomLeftColor;
            Color c3 = TopRightColor;
            Color c4 = BottomRightColor;
            Rectangle r = ClientRectangle;

            float delta12R = 1f * (c2.R - c1.R) / r.Height;
            float delta12G = 1f * (c2.G - c1.G) / r.Height;
            float delta12B = 1f * (c2.B - c1.B) / r.Height;
            float delta12A = 1f * (c2.A - c1.A) / r.Height;
            float delta34R = 1f * (c4.R - c3.R) / r.Height;
            float delta34G = 1f * (c4.G - c3.G) / r.Height;
            float delta34B = 1f * (c4.B - c3.B) / r.Height;
            float delta34A = 1f * (c4.A - c3.A) / r.Height;

            var destRect = ClientRectangle;

            var image = Toolbox.Library.Properties.Resources.CheckerBackground;
            pe.Graphics.DrawImage(Toolbox.Library.Properties.Resources.CheckerBackground,
                ClientRectangle, destRect, GraphicsUnit.Pixel);

            for (int y = 0; y < r.Height; y++)
            {
                Color c12 = Color.FromArgb(
                      c1.A + (int)(y * delta12A),
                      c1.R + (int)(y * delta12R),
                      c1.G + (int)(y * delta12G), 
                      c1.B + (int)(y * delta12B));
                Color c34 = Color.FromArgb(
                      c3.A + (int)(y * delta34A),
                      c3.R + (int)(y * delta34R),
                      c3.G + (int)(y * delta34G), 
                      c3.B + (int)(y * delta34B));
                using (LinearGradientBrush lgBrush = new LinearGradientBrush(
                      new Rectangle(0, y, r.Width, 1), c12, c34, 0f))
                {
                    pe.Graphics.FillRectangle(lgBrush, 0, y, r.Width, 1);
                }
            }

            int halfWidth = r.Width / 2;
            int halfHeight = r.Height / 2;

            int LeftX = 0;
            int RightX = r.Width - 32;
            int topY = 0;
            int BottomY = r.Height - 32;

            var font = new Font(this.Font, FontStyle.Bold);

            const int hitSize = 32;
            TopLeftHit = new Rectangle(LeftX, topY, hitSize, hitSize);
            TopRightHit = new Rectangle(RightX, topY, hitSize, hitSize);
            BottomLeftHit = new Rectangle(LeftX, BottomY, hitSize, hitSize);
            BottomRightHit = new Rectangle(RightX, BottomY, hitSize, hitSize);
            TopHit = new Rectangle(halfWidth - 16, topY, hitSize, hitSize);
            BottomHit = new Rectangle(halfWidth - 16, BottomY, hitSize, hitSize);
            LeftHit = new Rectangle(LeftX, halfHeight - 16, hitSize, hitSize);
            RightHit = new Rectangle(RightX, halfHeight - 16, hitSize, hitSize);
            AllHit = new Rectangle(halfWidth - 16, halfHeight - 16, hitSize, hitSize);

            LeftX = 10;
            RightX = r.Width - 30;
            topY = 10;
            BottomY = r.Height - 25;

            if (DisplayHitboxes)
            {
                pe.Graphics.FillRectangle(new SolidBrush(Color.Blue), TopLeftHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Purple), TopRightHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Green), BottomLeftHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Yellow), BottomRightHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Red), TopHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Red), RightHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Red), LeftHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Red), BottomHit);
                pe.Graphics.FillRectangle(new SolidBrush(Color.Pink), AllHit);
            }

            using (Brush br = new SolidBrush(AllColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("ALL", font, br, new Point(halfWidth - 10, halfHeight - 10));

            using (Brush br = new SolidBrush(TopLeftColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("TL", font, br, new Point(LeftX, topY));

            using (Brush br = new SolidBrush(TopRightColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("TR", font, br, new Point(RightX, topY));

            using (Brush br = new SolidBrush(BottomLeftColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("BL", font, br, new Point(LeftX, BottomY));

            using (Brush br = new SolidBrush(BottomRightColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("BR", font, br, new Point(RightX, BottomY));

            if (mouseLoc != Point.Empty)
            {
                if (AllHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, AllHit, AllColor);
                if (TopLeftHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, TopLeftHit, TopLeftColor);
                if (TopRightHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, TopRightHit, TopRightColor);
                if (BottomLeftHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, BottomLeftHit, BottomLeftColor);
                if (BottomRightHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, BottomRightHit, BottomRightColor);
                if (TopHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, TopHit, ColorBlend(TopRightColor, TopLeftColor));
                if (BottomHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, BottomHit, ColorBlend(BottomRightColor, BottomLeftColor));
                if (RightHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, RightHit, ColorBlend(BottomRightColor, TopRightColor));
                if (LeftHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, LeftHit, ColorBlend(BottomLeftColor, TopLeftColor));
            }

            base.OnPaint(pe);
        }

        private static Color ColorBlend(Color A, Color B)
        {
            Color[] colors = new Color[2]
            { A, B };

            return Color.FromArgb(
                (int)colors.Average(a => a.A),
                (int)colors.Average(a => a.R),
                (int)colors.Average(a => a.G),
                (int)colors.Average(a => a.B));
        }

        private void DrawSelectionOutline(PaintEventArgs pe, Rectangle rect, Color color) {
            //Select entire regions

            var colorAmount = color.GrayScale(true).Inverse();

            int lineThickness = 2;
            Rectangle selection = rect;
            if (rect == TopHit || rect == BottomHit) {
                selection = new Rectangle(0, rect.Y, pe.ClipRectangle.Width, rect.Height);
            }
            if (rect == LeftHit || rect == RightHit) {
                selection = new Rectangle(rect.X,0, rect.Width, pe.ClipRectangle.Height);
            }
            if (rect == AllHit) {
                selection = ClientRectangle;
                lineThickness = 2;
            }

            pe.Graphics.DrawRectangle(new Pen(new SolidBrush(colorAmount), lineThickness), new Rectangle(
                selection.X + (lineThickness / 2), selection.Y + (lineThickness / 2), 
                selection.Width - lineThickness, selection.Height - lineThickness));
        }
    }
}
