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
    public class ColorFullAlphaBox : STPanel
    {
        public Color Color { get; set; }

        public ColorFullAlphaBox()
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

            Brush RGBAColor = new SolidBrush(Color.FromArgb(Color.A, Color.R, Color.G, Color.B));
            pe.Graphics.FillRectangle(RGBAColor, ClientRectangle);

            base.OnPaint(pe);
        }
    }
}
