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
    //Based on this, very good color slider
    //https://github.com/libertyernie/brawltools/blob/40d7431b1a01ef4a0411cd69e51411bd581e93e2/BrawlLib/System/Windows/Controls/GoodColorControl.cs#L394
    //Slimmed down a bit to be more simple

    /// <summary>
    /// A panel which can select and pick colors
    /// </summary>
    public class ColorSelector : STUserControl
    {
        private Color _color;

        private HSVPixel _hsv = new HSVPixel(0, 100, 100);

        public Color NewColor
        {
            get
            {
                return Color.FromArgb(Alpha, Color);
            }
        }

        public STColor8 Color8
        {
            get { return new STColor8(NewColor); }
        }

        public STColor16 Color16
        {
            get { return new STColor16(NewColor); }
        }

        public STColor Color32
        {
            get { return new STColor(NewColor); }
        }


        /// <summary>
        /// The color the dialog gets and sets.
        /// </summary>
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                OnColorChanged(false);
            }
        }

        public Color AlphaColor
        {
            get
            {
                return Color.FromArgb(Alpha, Alpha, Alpha);
            }
        }

        private int _alpha;
        public int Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                alphaPanel.Invalidate();
                OnColorChanged(false);
            }
        }

        private bool displayAlpha = true;
        public bool DisplayAlpha
        {
            get { return displayAlpha; }
            set
            {
                displayAlpha = value;
                alphaPanel.Visible = displayAlpha;
            }
        }

        private bool displayColor = true;
        public bool DisplayColor
        {
            get { return displayColor; }
            set
            {
                displayColor = value;
                huePanel.Enabled = displayColor;
            }
        }

        private LinearGradientBrush _hueBrush;
        private STPanel colorSquare;
        private STPanel huePanel;
        private STPanel alphaPanel;
        private PathGradientBrush _mainBrush;

        public ColorSelector()
        {
            InitializeComponent();

            this.SetStyle(
               ControlStyles.AllPaintingInWmPaint |
               ControlStyles.UserPaint |
               ControlStyles.DoubleBuffer,
               true);

            colorSquare.SetDoubleBuffer();
            huePanel.SetDoubleBuffer();
            alphaPanel.SetDoubleBuffer();
        }

        private Point CursorPoint = new Point(0,0);

        public event EventHandler ColorChanged;

        private void InitializeComponent()
        {
            this.colorSquare = new Toolbox.Library.Forms.STPanel();
            this.huePanel = new Toolbox.Library.Forms.STPanel();
            this.alphaPanel = new Toolbox.Library.Forms.STPanel();
            this.SuspendLayout();
            // 
            // colorSquare
            // 
            this.colorSquare.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.colorSquare.Location = new System.Drawing.Point(3, 3);
            this.colorSquare.Name = "colorSquare";
            this.colorSquare.Size = new System.Drawing.Size(180, 235);
            this.colorSquare.TabIndex = 0;
            this.colorSquare.Paint += new System.Windows.Forms.PaintEventHandler(this.colorSquare_Paint);
            this.colorSquare.MouseDown += new System.Windows.Forms.MouseEventHandler(this.colorSquare_MouseDown);
            this.colorSquare.MouseMove += new System.Windows.Forms.MouseEventHandler(this.colorSquare_MouseMove);
            this.colorSquare.MouseUp += new System.Windows.Forms.MouseEventHandler(this.colorSquare_MouseUp);
            // 
            // huePanel
            // 
            this.huePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.huePanel.Location = new System.Drawing.Point(189, 3);
            this.huePanel.Name = "huePanel";
            this.huePanel.Size = new System.Drawing.Size(24, 235);
            this.huePanel.TabIndex = 1;
            this.huePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.huePanel_Paint);
            this.huePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.huePanel_MouseDown);
            this.huePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.huePanel_MouseMove);
            this.huePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.huePanel_MouseUp);
            // 
            // alphaPanel
            // 
            this.alphaPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.alphaPanel.Location = new System.Drawing.Point(219, 3);
            this.alphaPanel.Name = "alphaPanel";
            this.alphaPanel.Size = new System.Drawing.Size(24, 235);
            this.alphaPanel.TabIndex = 2;
            this.alphaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.alphaPanel_Paint);
            this.alphaPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alphaPanel_MouseDown);
            this.alphaPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.alphaPanel_MouseMove);
            this.alphaPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.alphaPanel_MouseUp);
            // 
            // ColorSelector
            // 
            this.Controls.Add(this.alphaPanel);
            this.Controls.Add(this.huePanel);
            this.Controls.Add(this.colorSquare);
            this.Name = "ColorSelector";
            this.Size = new System.Drawing.Size(245, 243);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorSelector_MouseMove);
            this.ResumeLayout(false);

        }

        private void ColorSelector_MouseMove(object sender, MouseEventArgs e)
        {
            CursorPoint = e.Location;
            this.Invalidate();
        }


        #region HueBar

        private bool _hueSelected = false;
        private void huePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _hueSelected = true;
                huePanel_MouseMove(sender, e);
            }
        }

        private void huePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _hueSelected = false;
        }

        private int hueY;
        private void huePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_hueSelected)
            {
                int y = Math.Max(Math.Min(e.Y, (huePanel.Height - 1)), 0);
                if (y != hueY)
                {
                    hueY = y;

                    _hsv.H = (ushort)((float)y / (huePanel.Height - 1) * 360);

                    OnColorChanged(true);
                }
            }
        }

        private void huePanel_Paint(object sender, PaintEventArgs e)
        {
            const float numHueColors = 6.0f;

            Rectangle r = colorSquare.ClientRectangle;

            //Draw the hue slider
            var g = e.Graphics;

            //Split the hue colors
            float p = r.Height / numHueColors / r.Height;

            _hueBrush = new LinearGradientBrush(new Rectangle(0, 0, r.Width, r.Height), Color.Red, Color.Red, LinearGradientMode.Vertical);

            //Create the hue list
            ColorBlend blend = new ColorBlend();
            blend.Colors = new Color[]
            {
                Color.Red,
                Color.Yellow,
                Color.Lime,
                Color.Cyan,
                Color.Blue,
                Color.Magenta,
                Color.Red
            };

            if (!huePanel.Enabled)
            {
                for (int i = 0; i < blend.Colors.Length; i++)
                    blend.Colors[i] = blend.Colors[i].Darken(190);
            }

            blend.Positions = new float[] { 0, p, p * 2, p * 3, p * 4, p * 5, 1.0f };
            _hueBrush.InterpolationColors = blend;



            g.FillRectangle(_hueBrush, r);

            Color pixel = new HSVPixel(_hsv.H, 100, 100).ToRGBA().Inverse();

            int y = (int)(_hsv.H / 360.0f * (huePanel.Height - 1));
            Rectangle c = new Rectangle(-1, y - 2, huePanel.Width + 1, 4);

            using (Pen pen = new Pen(pixel))
                g.DrawRectangle(pen, c);

            c.Y += 1;
            c.Height -= 2;
            pixel = pixel.Lighten(64);

            using (Pen pen = new Pen(pixel))
                g.DrawRectangle(pen, c);
        }

        #endregion

        private Color[] _boxColors = new Color[] { Color.Black, Color.White, Color.Black, Color.Black, Color.Black };
        private void colorSquare_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            Rectangle r = colorSquare.ClientRectangle;
            float p = r.Height / 6.0f / r.Height;

            _mainBrush = new PathGradientBrush(new PointF[] {
                new PointF(r.Width, 0),
                new PointF(r.Width, r.Height),
                new PointF(0, r.Height),
                new PointF(0,0),
                new PointF(r.Width, 0)});

            _boxColors[0] = _boxColors[4] = new HSVPixel(_hsv.H, 100, 100).ToRGBA();
            _mainBrush.SurroundColors = _boxColors;
            _mainBrush.CenterColor = new HSVPixel(_hsv.H, 50, 50).ToRGBA(); 
            _mainBrush.CenterPoint = new PointF(r.Width / 2, r.Height / 2);

            if (!huePanel.Enabled)
            {
                _boxColors[0] = _boxColors[4] = new HSVPixel(_hsv.H, 0, 50).ToRGBA();
                _mainBrush.SurroundColors = _boxColors;
                _mainBrush.CenterColor = new HSVPixel(_hsv.H, 0, 50).ToRGBA();
            }

            g.FillRectangle(_mainBrush, r);

            //Draw indicator
            int x = (int)(_hsv.V / 100.0f * colorSquare.Width);
            int y = (int)((100 - _hsv.S) / 100.0f * colorSquare.Height);

            Rectangle c = new Rectangle(x - 3, y - 3, 6, 6);

            Color pixel = _color.Inverse();
            pixel.WhiteAlpha();

            using (Pen pen = new Pen(pixel))
                g.DrawEllipse(pen, c);

            c.X -= 1;
            c.Y -= 1;
            c.Width += 2;
            c.Height += 2;

            pixel = pixel.Lighten(64);

            using (Pen pen = new Pen(pixel))
                g.DrawEllipse(pen, c);
        }

        protected virtual void OnColorChanged(bool hsvToRgb)
        {
            colorSquare.Invalidate();
            huePanel.Invalidate();

            if (hsvToRgb)
                _color = _hsv.ToRGBA();
            else
                _hsv = HSVPixel.FromRGBA(_color);

            if (ColorChanged != null)
                ColorChanged(this, null);
        }

        private int _squareX, _squareY;
        private bool _squareSelected = false;
        private void colorSquare_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _squareSelected = true;
                colorSquare_MouseMove(sender, e);
            }
        }

        private void colorSquare_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _squareSelected = false;
        }

        private bool _alphaBarSelected = false;
        private int alphaY;

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
                int y = Math.Max(Math.Min(e.Y, (alphaPanel.Height - 1)), 0);
                if (y != alphaY)
                {
                    alphaY = y;
                    Alpha = (byte)(255 - ((float)y / (alphaPanel.Height - 1) * 255));

                    if (!huePanel.Enabled)
                        _color = Color.FromArgb(Alpha, Alpha, Alpha); 

                    if (ColorChanged != null)
                        ColorChanged(this, null);
                }
            }
        }

        private void alphaPanel_Paint(object sender, PaintEventArgs e)
        {
            var alphaBrush = new LinearGradientBrush(new Rectangle(0, 0, alphaPanel.Width, alphaPanel.Height), Color.White, Color.Black, LinearGradientMode.Vertical);

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
            int y = (int)(col / 255.0f * (alphaPanel.Height - 1));
            Rectangle r = new Rectangle(-1, y - 2, alphaPanel.Width + 1, 4);

            using (Pen pen = new Pen(p))
                g.DrawRectangle(pen, r);

            p.Lighten(64);

            r.Y += 1;
            r.Height -= 2;

            using (Pen pen = new Pen(p))
                g.DrawRectangle(pen, r);
        }

        private void colorSquare_MouseMove(object sender, MouseEventArgs e)
        {
            if (_squareSelected)
            {
                int x = Math.Min(Math.Max(e.X, 0), colorSquare.Width);
                int y = Math.Min(Math.Max(e.Y, 0), colorSquare.Height);
                if (!huePanel.Enabled)
                    y = colorSquare.Height;

                if ((x != _squareX) || (y != _squareY))
                {
                    _hsv.V = (byte)((float)x / colorSquare.Width * 100);
                    _hsv.S = (byte)((float)(colorSquare.Height - y) / colorSquare.Height * 100);

                    OnColorChanged(true);

                    if (!huePanel.Enabled) {
                        Alpha = _color.R;
                        alphaPanel.Invalidate();
                    }
                }
            }
        }
    }
}
