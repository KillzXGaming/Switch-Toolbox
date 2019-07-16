using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Toolbox.Library.Forms
{
    public class PictureBoxCustom : PictureBox
    {
        public PictureBoxCustom()
        {
            this.BackgroundImage = GetCheckerBackground();
            this.BackColor = Color.Transparent;
            this.SizeMode = PictureBoxSizeMode.Zoom;

        }
        public Image GetCheckerBackground()
        {
            return Properties.Resources.CheckerBackground;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(pe);
        }
    }
}
