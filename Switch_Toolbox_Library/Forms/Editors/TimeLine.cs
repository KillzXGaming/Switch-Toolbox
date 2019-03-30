using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class TimeLine : UserControl
    {
        public TimeLine()
        {
            InitializeComponent();
        }

        public event EventHandler FrameChanged;

        public int CurrentFrame
        {
            get { return currentFrame; }
            set
            {
                if (FollowCurrentFrame)
                {
                    double delta = value - (frameRight + frameLeft) * 0.5;
                    frameLeft += delta;
                    frameRight += delta;

                    #region resolve collsions
                    if (frameLeft < 0)
                    {
                        frameRight -= frameLeft;
                        frameLeft = 0;
                    }
                    else if (frameRight > frameCount)
                    {
                        frameLeft += frameCount - frameRight;
                        frameRight = frameCount;
                    }
                    #endregion

                }

                currentFrame = value;

                FrameChanged?.Invoke(this, new EventArgs());

                Refresh();
            }
        }

        public int FrameCount
        {
            get { return frameCount; }
            set
            {
                frameCount = value;

                if (value == 1)
                {
                    frameLeft = 0;
                    frameRight = 1;
                }
                else
                {
                    ResolveCollision();
                }

                Refresh();
            }
        }

        public bool FollowCurrentFrame = true;

        private int currentFrame = 0;
        private int frameCount = 1000;

        private double frameLeft = 0;
        private double frameRight = 200;

        private Point lastMousePos;


        private static Brush brush1 = new SolidBrush(Color.FromArgb(255, 255, 20));
        private static Brush brush2 = new SolidBrush(Color.FromArgb(20, 20, 20));
        private static Brush brush3 = new SolidBrush(Color.FromArgb(50, 50, 50));
        private static Brush brush4 = new SolidBrush(Color.FromArgb(90, 90, 90));
        private static Pen pen1 = new Pen(new SolidBrush(Color.FromArgb(30, 30, 30)), 2);
        private static Pen pen2 = new Pen(new SolidBrush(Color.FromArgb(100, 100, 20)), 2);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            double currentFrameX = 20 + (currentFrame - frameLeft) * (Width - 40.0) / (frameRight - frameLeft);

            e.Graphics.FillRectangle(brush2, new Rectangle(0, 0, Width, Height));
            e.Graphics.FillRectangle(brush3, new Rectangle(0, 0, Width, TextRenderer.MeasureText("" + currentFrame, Font).Height));

            double step = 50 * (frameRight - frameLeft) / Width;
            if (step > 10)
                step = Math.Round(step / 10.0) * 10;
            else
            {
                step = Math.Round(Math.Max(1, step));
            }

            for (double frame = Math.Round(frameLeft / step) * step; frame <= frameRight; frame += step)
            {
                double frameX = 20 + (frame - frameLeft) * (Width - 40.0) / (frameRight - frameLeft);

                e.Graphics.DrawLine(pen1, new Point((int)frameX, TextRenderer.MeasureText("" + frame, Font).Height), new Point((int)frameX, Height));

                e.Graphics.DrawString("" + frame, Font, brush4, new Point((int)frameX - TextRenderer.MeasureText("" + frame, Font).Width / 2, 0));
            }

            e.Graphics.DrawLine(pen2, new Point((int)currentFrameX, TextRenderer.MeasureText("" + currentFrame, Font).Height), new Point((int)currentFrameX, Height));


            e.Graphics.DrawString("" + currentFrame, Font, brush1, new Point((int)currentFrameX - TextRenderer.MeasureText("" + currentFrame, Font).Width / 2, 0));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                currentFrame = Math.Min(Math.Max(0, (int)Math.Round(((e.Location.X - 20) * (frameRight - frameLeft) / (Width - 40.0) + frameLeft))), frameCount);
                FrameChanged?.Invoke(this, new EventArgs());

                double delta = e.Location.X * (frameRight - frameLeft) / (Width - 40.0);
                frameLeft -= delta;
                frameRight -= delta;

                ResolveCollision();
                Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                double delta = (e.Location.X - lastMousePos.X) * (frameRight - frameLeft) / (Width - 40.0);
                frameLeft -= delta;
                frameRight -= delta;

                ResolveCollision();
                Refresh();
            }

            lastMousePos = e.Location;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            double delta = 1 + Math.Min(Math.Max(-0.5, -e.Delta * 0.00390625), 0.5);

            double frameOrigin = Math.Min(Math.Max(0, ((e.Location.X - 20) * (frameRight - frameLeft) / (Width - 40.0) + frameLeft)), frameCount);

            frameLeft = Math.Min(-1, (frameLeft - frameOrigin)) * delta + frameOrigin;
            frameRight = Math.Max(1, (frameRight - frameOrigin)) * delta + frameOrigin;

            ResolveCollision();

            Refresh();
        }

        #region resolve collsions
        private void ResolveCollision()
        {
            if (frameLeft < 0)
            {
                frameRight -= frameLeft;
                if (frameRight > frameCount)
                    frameRight = frameCount;
                frameLeft = 0;
            }
            else if (frameRight > frameCount)
            {
                frameLeft += frameCount - frameRight;
                if (frameLeft < 0)
                    frameLeft = 0;
                frameRight = frameCount;
            }
        }
        #endregion

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void TimeLine_Resize(object sender, EventArgs e) {
            ResolveCollision();
        }
    }
}
