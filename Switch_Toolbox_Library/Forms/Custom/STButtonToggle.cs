using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Toolbox.Library.Forms
{
    public class STButtonToggle : STPanel
    {
        private bool check;
        public bool Checked
        { 
            get { return check; }
            set {
                check = value;
                this.Refresh();
            }
        }

        public string PropertyText { get; set; }

        public Color BtnBackColor
        {
            get
            {
                if (Checked)
                    return FormThemes.BaseTheme.CheckBoxEnabledBackColor;
                else
                    return FormThemes.BaseTheme.CheckBoxBackColor;
            }
        }

        public Color BtnForeColor
        {
            get
            {
                if (Checked)
                    return FormThemes.BaseTheme.FormForeColor;
                else
                    return FormThemes.BaseTheme.DisabledItemColor;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle r = e.ClipRectangle;
            if (r.Width <= 0 || r.Height <= 0)
                return;

            Color c1 = BtnBackColor;
            Color c2 = BtnBackColor.Darken(20);
            LinearGradientBrush br = new LinearGradientBrush(r, c1, c2, 90, true);

            ColorBlend cb = new ColorBlend();
            cb.Positions = new[] { 0.0f, 1.0f };
            cb.Colors = new[] { c1, c2 };
            br.InterpolationColors = cb;

            SolidBrush br2 = new SolidBrush(BtnForeColor);

            int tx2 = ClientSize.Width / 2;
            int ty2 = ClientSize.Height / 2 - 6;

            e.Graphics.FillRectangle(br, r);

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(PropertyText, this.Font, br2, ClientRectangle, sf);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (Checked)
                Checked = false;
            else
                Checked = true;
        }
    }
}
