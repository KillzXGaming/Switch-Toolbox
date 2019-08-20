using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class Color8KeySlider : STPanel, IColorPanelCommon
    {
        public bool IsAlpha { get; set; }

        private int SelectedIndex = 0;
        public Color GetColor()
        {
            return Keys[SelectedIndex].Color;
        }

        public float GetTime()
        {
            return Keys[SelectedIndex].STColor.Time;
        }

        public void SetColor(Color color)
        {
            Keys[SelectedIndex].STColor.Color = color;
            this.Invalidate();
        }

        public event EventHandler ColorSelected;

        public void SelectPanel() {}
        public void DeselectPanel() {}

        public Color8KeySlider()
        {
            InitializeComponent();

            this.SetStyle(
                  ControlStyles.AllPaintingInWmPaint |
                  ControlStyles.UserPaint |
                  ControlStyles.DoubleBuffer,
                  true);

            Paint += new PaintEventHandler(panel_Paint);
            BorderStyle = BorderStyle.FixedSingle;
            MouseHover += Color8KeySlider_MouseHover;
            MouseMove += Color8KeySlider_MouseMove;
            MouseDown += Color8KeySlider_MouseDown;
            MouseUp += Color8KeySlider_MouseUp;
        }

        private List<KeyFrame> Keys = new List<KeyFrame>();

        public void LoadColors(STColor[] keys, int keyCount)
        {
            Keys.Clear();
            for (int i = 0; i < keyCount; i++)
                Keys.Add(new KeyFrame(keys[i]));
        }

        private void HitDetect()
        {

        }

        private Point MouseCursor;
        private void panel_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            var g = e.Graphics;

            Color firstColor = Color.White;
            Color lastColor = Color.White;
            if (Keys.Count > 0)
            {
                firstColor = Keys[0].Color;
                lastColor = Keys[Keys.Count - 1].Color;
            }

            float keyMarigin = 10f;

            RectangleF r = new RectangleF(ClientRectangle.X, ClientRectangle.Y + keyMarigin, ClientRectangle.Width, ClientRectangle.Height - keyMarigin);

            //Start our gradient brush
            LinearGradientBrush br = new LinearGradientBrush(r, firstColor, lastColor, 0, true);

            List<Color> colors = new List<Color>();
            List<float> frames = new List<float>();

            frames.Add(0);
            colors.Add(firstColor);

            for (int i = 0; i < Keys.Count; i++)
            {
                var currentKey = Keys[i];
                Color c2 = currentKey.Color;
                c2 = Color.FromArgb(c2.R, c2.G, c2.B);

                float p2 = currentKey.STColor.Time;

                colors.Add(c2);
                frames.Add(p2);
            }
            colors.Add(lastColor);
            frames.Add(1);

            ColorBlend cb = new ColorBlend();
            cb.Positions = frames.ToArray(); ;
            cb.Colors = colors.ToArray();
            br.InterpolationColors = cb;

            // paint gradient
            g.FillRectangle(br, r);

            for (int i = 0; i < Keys.Count; i++)
            {
                //Create a box to reperesent a key frame
                int keyPos = (int)(ClientRectangle.Width * Keys[i].STColor.Time);
                if (i == Keys.Count - 1 && Keys[i].STColor.Time >= 0.09f)
                    keyPos -= 8;

                Rectangle keyBox = new Rectangle(keyPos, (int)r.Y, 7, (int)r.Height);
                Keys[i].DrawnRectangle = keyBox;

                // paint keys
                Color cursorColor = Color.White;
                if (Keys[i].IsHit(MouseCursor.X, MouseCursor.Y) || Keys[i].IsSelected) {
                    cursorColor = Color.Yellow;
                }

                using (Pen pen = new Pen(Color.Black,1))
                    g.DrawRectangle(pen, keyBox);

                keyBox.Y += 1;
                keyBox.X += 1;

                keyBox.Height -= 2;
                keyBox.Width -= 2;

                using (Pen pen = new Pen(cursorColor, 1))
                    g.DrawRectangle(pen, keyBox);

                keyBox.Y += 1;
                keyBox.X += 1;

                keyBox.Height -= 2;
                keyBox.Width -= 2;

                using (Pen pen = new Pen(Color.Black, 1))
                    g.DrawRectangle(pen, keyBox);


                //Draw key pointer at top

                int keyTopPos = (int)(r.Y - 10);
                keyPos += keyBox.Width;
                Point[] triPoints = { new Point(keyPos - 5, keyTopPos), new Point(keyPos, keyTopPos + 10), new Point(keyPos + 5, keyTopPos) };
                e.Graphics.FillPolygon(new SolidBrush(cursorColor), triPoints);
                e.Graphics.DrawPolygon(new Pen(Color.Black,0.5f), triPoints);
            }

            frames.Clear();
            colors.Clear();
        }

        private bool IsSelected = false;
        private void Color8KeySlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsSelected = true;
                Color8KeySlider_MouseMove(sender, e);
            }
        }

        private void Color8KeySlider_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsSelected = false;
            }
        }

        private void Color8KeySlider_MouseMove(object sender, MouseEventArgs e)
        {
            MouseCursor = e.Location;
            this.Invalidate();
        }

        private void OnKeySelected()
        {
            this.Invalidate();

            if (ColorSelected != null)
                ColorSelected(this, null);
        }

        private void Color8KeySlider_MouseHover(object sender, EventArgs e)
        {

        }

        private class KeyFrame
        {
            public bool IsSelected = false;

            public Color Color => STColor.Color;

            public STColor STColor;

            public Rectangle DrawnRectangle;

            public KeyFrame(STColor color)
            {
                STColor = color;
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
        }

        private void Color8KeySlider_Click(object sender, EventArgs e) {
            for (int i = 0; i < Keys.Count; i++)
                Keys[i].IsSelected = false;

            if (IsSelected)
            {
                for (int i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i].IsHit(MouseCursor.X, MouseCursor.Y))
                    {
                        Keys[i].IsSelected = true;
                        SelectedIndex = i;
                        OnKeySelected();
                    }
                }
            }
        }
    }
}
