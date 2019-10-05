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

        private Color AllColor = Color.White;

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
            OnColorChanged?.Invoke(null, new EventArgs());
        }

        public VertexColorBox()
        {
            this.BackgroundImage = Properties.Resources.CheckerBackground;
            this.BackColor = Color.Transparent;
            this.MouseClick += OnMouseClick;
            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer,
            true);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (TopLeftHit == null) return;

            if (e.Button == MouseButtons.Left)
            {
                if (TopLeftHit.IsHit(e.Location))
                    TopLeftColor = LoadColorDialog(TopLeftColor);
                if (TopRightHit.IsHit(e.Location))
                    TopRightColor = LoadColorDialog(TopRightColor);
                if (BottomLefttHit.IsHit(e.Location))
                    BottomLeftColor = LoadColorDialog(BottomLeftColor);
                if (BottomRightHit.IsHit(e.Location))
                    BottomRightColor = LoadColorDialog(BottomRightColor);
                if (AllHit.IsHit(e.Location))
                {
                    ColorDialog colorDlg = new ColorDialog();
                    colorDlg.Color = AllColor;
                    if (colorDlg.ShowDialog() == DialogResult.OK)
                    {
                        TopLeftColor = colorDlg.Color;
                        TopRightColor = colorDlg.Color;
                        BottomLeftColor = colorDlg.Color;
                        BottomRightColor = colorDlg.Color;
                    }
                }
            }

            this.Invalidate();
        }

        private Color LoadColorDialog(Color color)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = color;
            if (colorDlg.ShowDialog() == DialogResult.OK)
                return colorDlg.Color;
            else
                return color;
        }

        private Rectangle TopLeftHit;
        private Rectangle TopRightHit;
        private Rectangle BottomLefttHit;
        private Rectangle BottomRightHit;
        private Rectangle AllHit;

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
            float delta34R = 1f * (c4.R - c3.R) / r.Height;
            float delta34G = 1f * (c4.G - c3.G) / r.Height;
            float delta34B = 1f * (c4.B - c3.B) / r.Height;

            for (int y = 0; y < r.Height; y++)
            {
                Color c12 = Color.FromArgb(255, c1.R + (int)(y * delta12R),
                      c1.G + (int)(y * delta12G), c1.B + (int)(y * delta12B));
                Color c34 = Color.FromArgb(255, c3.R + (int)(y * delta34R),
                      c3.G + (int)(y * delta34G), c3.B + (int)(y * delta34B));
                using (LinearGradientBrush lgBrush = new LinearGradientBrush(
                      new Rectangle(0, y, r.Width, 1), c12, c34, 0f))
                {
                    pe.Graphics.FillRectangle(lgBrush, 0, y, r.Width, 1);
                }
            }

            int halfWidth = r.Width / 2;
            int halfHeight = r.Height / 2;

            int LeftX = 10;
            int RightX = r.Width - 30;
            int topY = 10;
            int BottomY = r.Height - 25;

            var font = new Font(this.Font, FontStyle.Bold);

            using (Brush br = new SolidBrush(c1.GrayScale().Inverse()))
                pe.Graphics.DrawString("ALL", font, br, new Point(halfWidth - 10, halfHeight - 10));

            using (Brush br = new SolidBrush(TopLeftColor.GrayScale().Inverse()))
                pe.Graphics.DrawString("TL", font, br, new Point(LeftX, topY));

            using (Brush br = new SolidBrush(TopRightColor.GrayScale().Inverse()))
                pe.Graphics.DrawString("TR", font, br, new Point(RightX, topY));

            using (Brush br = new SolidBrush(BottomLeftColor.GrayScale().Inverse()))
                pe.Graphics.DrawString("BL", font, br, new Point(LeftX, BottomY));

            using (Brush br = new SolidBrush(BottomRightColor.GrayScale().Inverse()))
                pe.Graphics.DrawString("BR", font, br, new Point(RightX, BottomY));
            //  pe.Graphics.FillRectangle(linearGradientBrush, ClientRectangle);

            const int hitSize = 40;
            TopLeftHit = new Rectangle(LeftX, topY, hitSize, hitSize);
            TopRightHit = new Rectangle(RightX, topY, hitSize, hitSize);
            BottomLefttHit = new Rectangle(LeftX, BottomY, hitSize, hitSize);
            BottomRightHit = new Rectangle(RightX, BottomY, hitSize, hitSize);
            AllHit = new Rectangle(halfWidth - 10, halfHeight - 10, hitSize, hitSize);

            base.OnPaint(pe);
        }
    }
}
