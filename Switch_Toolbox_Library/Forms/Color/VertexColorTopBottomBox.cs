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
    public class VertexColorTopBottomBox : STPanel
    {
        public EventHandler OnColorChanged;

        private Point mouseLoc = Point.Empty;
        private Color TColor;
        private Color BColor;

        private Color AllColor
        {
            get
            {
                Color[] colors = new Color[2]
                { TColor, BColor };

                return Color.FromArgb(
                    (int)colors.Average(a => a.A),
                    (int)colors.Average(a => a.R),
                    (int)colors.Average(a => a.G),
                    (int)colors.Average(a => a.B));
            }
        }

        private bool SuppressChangeEvent = false;

        public Color TopColor
        {
            get { return TColor; }
            set
            {
                TColor = value;
                ColorChanged();
            }
        }

        public Color BottomColor
        {
            get { return BColor; }
            set
            {
                BColor = value;
                ColorChanged();
            }
        }

        public void ColorChanged()
        {
            this.Invalidate();
            OnColorChanged?.Invoke(this, new EventArgs());
        }

        public VertexColorTopBottomBox()
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

        public void DisposeControl()
        {
            mouseLoc = Point.Empty;
            if (colorDlg != null)
            {
                colorDlg.Close();
                colorDlg = null;
            }
        }

        private bool dialogActive;
        private STColorDialog colorDlg;
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (TopHit == null) return;

            if (e.Button == MouseButtons.Left)
            {
                if (AllHit.IsHit(e.Location))
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

                        TopColor = colorDlg.NewColor;
                        BottomColor = colorDlg.NewColor;

                        SuppressChangeEvent = false;
                        ColorChanged();
                    };
                }
                else if (TopHit.IsHit(e.Location))
                    LoadColorDialog(TopColor, 0);
                else if (BottomHit.IsHit(e.Location))
                    LoadColorDialog(BottomColor, 1);
            }

            this.Invalidate();
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
                    TopColor = colorDlg.NewColor;
                if (index == 1)
                    BottomColor = colorDlg.NewColor;
            };
        }

        private Rectangle TopHit;
        private Rectangle BottomHit;
        private Rectangle AllHit;

        private bool DisplayHitboxes = true;

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode.Bilinear;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            Rectangle r = ClientRectangle;

            int halfWidth = r.Width / 2;
            int halfHeight = r.Height / 2;

            var linearGradientBrush = new LinearGradientBrush(r, 
                TopColor, BottomColor, LinearGradientMode.Vertical);

            pe.Graphics.FillRectangle(linearGradientBrush, r);

            int topY = 10;
            int BottomY = r.Height - 25;

            var font = new Font(this.Font, FontStyle.Bold);

            using (Brush br = new SolidBrush(AllColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("ALL", font, br, new Point(halfWidth - 12, halfHeight - 10));

            using (Brush br = new SolidBrush(TopColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("T", font, br, new Point(halfWidth - 7, topY));

            using (Brush br = new SolidBrush(BottomColor.GrayScale(true).Inverse()))
                pe.Graphics.DrawString("B", font, br, new Point(halfWidth - 7, BottomY));

            topY = 0;
            BottomY = r.Height - 32;

            const int hitSize = 32;
            TopHit = new Rectangle(r.X, topY, r.Width, hitSize);
            BottomHit = new Rectangle(r.X, BottomY, r.Width, hitSize);
            AllHit = new Rectangle(r.X, halfHeight - 16, r.Width, hitSize);

            if (mouseLoc != Point.Empty)
            {
                if (AllHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, AllHit, AllColor);
                if (TopHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, TopHit, TopColor);
                if (BottomHit.IsHit(mouseLoc))
                    DrawSelectionOutline(pe, BottomHit, BottomColor);
            }

            //  pe.Graphics.FillRectangle(linearGradientBrush, ClientRectangle);

            base.OnPaint(pe);
        }

        private void DrawSelectionOutline(PaintEventArgs pe, Rectangle rect, Color color)
        {
            //Select entire regions

            var colorAmount = color.GrayScale(true).Inverse();

            int lineThickness = 2;
            Rectangle selection = rect;
            if (rect == TopHit || rect == BottomHit)
            {
                selection = new Rectangle(rect.X, rect.Y, pe.ClipRectangle.Width, rect.Height);
            }
            if (rect == AllHit)
            {
                selection = ClientRectangle;
                lineThickness = 2;
            }

            pe.Graphics.DrawRectangle(new Pen(new SolidBrush(colorAmount), lineThickness), new Rectangle(
                selection.X + (lineThickness / 2), selection.Y + (lineThickness / 2),
                selection.Width - lineThickness, selection.Height - lineThickness));
        }
    }
}
