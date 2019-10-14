using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using Toolbox.Library.IO;

namespace Toolbox.Library.Forms
{
    public partial class AlphaSelectorHorizontalPanel : UserControl
    {
        public AlphaSelectorHorizontalPanel()
        {
            InitializeComponent();

            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer,
            true);
        }

        public event EventHandler AlphaChanged;

        private int _alpha;
        public int Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                OnAlphaChanged(false);
                alphaPanel.Invalidate();
            }
        }

        protected virtual void OnAlphaChanged(bool hsvToRgb)
        {
            alphaSolidBP.BackColor = Color.FromArgb(255, Alpha, Alpha, Alpha);
        }

        private bool _alphaBarSelected = false;
        private int alphaX;

        private void alphaPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _alphaBarSelected = true;
                alphaPanel_MouseMove(sender, e);
            }
        }

        private void alphaPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _alphaBarSelected = false;
        }

        private void alphaPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_alphaBarSelected)
            {
                int x = Math.Max(Math.Min(e.X, (alphaPanel.Width - 1)), 0);
                if (x != alphaX)
                {
                    alphaX = x;
                    Alpha = (byte)(255 - ((float)x / (alphaPanel.Width - 1) * 255));

                    if (AlphaChanged != null)
                        AlphaChanged(this, null);
                }
            }
        }

        private void alphaPanel_Paint(object sender, PaintEventArgs e)
        {
            var alphaBrush = new LinearGradientBrush(new Rectangle(0, 0, alphaPanel.Width, alphaPanel.Height), Color.White, Color.Black, LinearGradientMode.Horizontal);

            Graphics g = e.Graphics;

            if (!alphaPanel.Enabled)
            {
                for (int i = 0; i < alphaBrush.LinearColors.Length; i++)
                    alphaBrush.LinearColors[i] = alphaBrush.LinearColors[i].Darken(190);
            }

            //Draw bar
            g.FillRectangle(alphaBrush, alphaPanel.ClientRectangle);

            //Draw indicator
            byte col = (byte)(255 - Alpha);
            Color p = Color.FromArgb(255, col, col, col);
            int x = (int)(col / 255.0f * (alphaPanel.Width - 1));
            Rectangle r = new Rectangle(x, -1, 4, alphaPanel.Height + 1);

            using (Pen pen = new Pen(p))
                g.DrawRectangle(pen, r);

            p.Lighten(64);

            r.X += 1;
            r.Width -= 2;

            using (Pen pen = new Pen(p))
                g.DrawRectangle(pen, r);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
