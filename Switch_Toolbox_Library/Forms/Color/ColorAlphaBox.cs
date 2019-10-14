using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Toolbox.Library.Forms
{
    public class ColorAlphaBox : STPanel
    {
        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                this.Invalidate();
            }
        }

        private bool displayAlphaSolid;
        public bool DisplayAlphaSolid
        {
            get { return displayAlphaSolid; }
            set
            {
                displayAlphaSolid = true;
                this.BackgroundImage = null;
                this.BackColor = Color.Black;
                this.Invalidate();
            }
        }

        public ColorAlphaBox()
        {
            this.BackgroundImage = Properties.Resources.CheckerBackground;
            this.BackColor = Color.Transparent;

            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer,
            true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            Brush RGBColor = new SolidBrush(Color.FromArgb(255, Color.R, Color.G, Color.B));
            Brush AlphaColor = new SolidBrush(Color.FromArgb(Color.A, Color.R, Color.G, Color.B));

            if (DisplayAlphaSolid)
                AlphaColor = new SolidBrush(Color.FromArgb(255, Color.A, Color.A, Color.A));


            Point rgbPos = new Point(ClientRectangle.X, ClientRectangle.Y);
            Point alphaPos = new Point(ClientRectangle.X + ClientRectangle.Width / 2, ClientRectangle.Y);

            pe.Graphics.FillRectangle(RGBColor, new RectangleF(rgbPos.X, rgbPos.Y, Width / 2, Height));
            pe.Graphics.FillRectangle(AlphaColor, new RectangleF(alphaPos.X, alphaPos.Y, Width / 2, Height));

            base.OnPaint(pe);
        }
    }
}
