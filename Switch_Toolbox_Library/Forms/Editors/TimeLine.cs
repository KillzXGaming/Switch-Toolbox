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
            timer.Interval = 10;
            timer.Tick += Timer_Tick;
        }

        public event EventHandler FrameChanged;

        private Timer timer = new Timer();

        public bool Locked { get; private set; } = false;

        public void Play()
        {
            Locked = true;
        }

        public void Stop()
        {
            Locked = false;
        }

        private void ResolveCollision()
        {
            if (frameLeft < 0)
            {
                frameRight -= frameLeft;
                frameLeft = 0;
            }
            else if (frameRight > lastFrame)
            {
                frameLeft += lastFrame - frameRight;
                frameRight = lastFrame;
            }
        }

        private void ResolveFitting()
        {
            if (frameLeft < 0)
            {
                frameRight -= frameLeft;
                if (frameRight > lastFrame)
                    frameRight = lastFrame;
                frameLeft = 0;
            }
            else if (frameRight > lastFrame)
            {
                frameLeft += lastFrame - frameRight;
                if (frameLeft < 0)
                    frameLeft = 0;
                frameRight = lastFrame;
            }
        }

        public int CurrentFrame
        {
            get => currentFrame;
            set
            {
                if (FollowCurrentFrame && !(Focused && MouseButtons == MouseButtons.Right))
                {
                    double delta = value - (frameRight + frameLeft) * 0.5;
                    frameLeft += delta;
                    frameRight += delta;

                    ResolveCollision();

                }

                currentFrame = value;

                Refresh();
            }
        }

        public int FrameCount
        {
            get => lastFrame + 1;
            set
            {
                lastFrame = value - 1;

                if (value == 1)
                {
                    frameLeft = 0;
                    frameRight = 1;
                }
                else
                {
                    ResolveFitting();
                }

                if (currentFrame > lastFrame)
                    currentFrame = lastFrame;

                Refresh();
            }
        }

        public bool FollowCurrentFrame = true;

        private int currentFrame = 0;
        private int lastFrame = 1000;

        private double frameLeft = 0;
        private double frameRight = 200;

        private Point lastMousePos;


        private static Brush brush1 = new SolidBrush(Color.FromArgb(255, 255, 20));
        private static Brush brush2 = new SolidBrush(Color.FromArgb(20, 20, 20));
        private static Brush brush3 = new SolidBrush(Color.FromArgb(50, 50, 50));
        private static Brush brush4 = new SolidBrush(Color.FromArgb(90, 90, 90));
        private static Pen pen1 = new Pen(new SolidBrush(Color.FromArgb(30, 30, 30)), 2);
        private static Pen pen2 = new Pen(new SolidBrush(Color.FromArgb(100, 100, 20)), 2);

        private static Font font = new Font(new FontFamily("arial"), 10);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            double currentFrameX = 20 + (currentFrame - frameLeft) * (Width - 40.0) / (frameRight - frameLeft);

            e.Graphics.FillRectangle(brush2, new Rectangle(0, 0, Width, Height));
            e.Graphics.FillRectangle(brush3, new Rectangle(0, 0, Width, TextRenderer.MeasureText("" + currentFrame, font).Height));

            double step = 50 * (frameRight - frameLeft) / Width;
            if (step > 10)
                step = Math.Round(step / 10.0) * 10;
            else
            {
                step = Math.Round(Math.Max(1, step));
            }

            if (lastFrame != 0)
            {
                double max;
                if (frameRight < lastFrame)
                    max = Math.Min(frameRight + step, lastFrame);
                else
                    max = frameRight - step;

                for (double frame = Math.Floor(frameLeft / step) * step; frame <= max; frame += step)
                {
                    double frameX = 20 + (frame - frameLeft) * (Width - 40.0) / (frameRight - frameLeft);

                    e.Graphics.DrawLine(pen1, new Point((int)frameX, TextRenderer.MeasureText("" + frame, font).Height), new Point((int)frameX, Height));

                    e.Graphics.DrawString("" + frame, font, brush4, new Point((int)frameX - TextRenderer.MeasureText("" + frame, font).Width / 2, 0));
                }
            }


            if (frameRight == lastFrame)
            {
                //draw last frame regardless of the steps
                double x = Width - 20;

                e.Graphics.DrawLine(pen1, new Point((int)x, TextRenderer.MeasureText("" + lastFrame, font).Height), new Point((int)x, Height));

                e.Graphics.DrawString("" + lastFrame, font, brush4, new Point((int)x - TextRenderer.MeasureText("" + lastFrame, font).Width / 2, 0));
            }

            e.Graphics.DrawLine(pen2, new Point((int)currentFrameX, TextRenderer.MeasureText("" + currentFrame, font).Height), new Point((int)currentFrameX, Height));

            e.Graphics.DrawString("" + currentFrame, font, brush1, new Point((int)currentFrameX - TextRenderer.MeasureText("" + currentFrame, font).Width / 2, 0));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double step;
            if (lastMousePos.X < 20)
            {
                step = (20 - lastMousePos.X) * (frameRight - frameLeft) / Width;
                frameLeft -= step;
                frameRight -= step;

                #region resolve collsions
                if (frameLeft < 0)
                {
                    frameRight -= frameLeft;
                    frameLeft = 0;
                }
                #endregion
            }
            else
            {
                step = (lastMousePos.X - Width + 20) * (frameRight - frameLeft) / Width;
                frameLeft += step;
                frameRight += step;

                #region resolve collsions
                if (frameRight > lastFrame)
                {
                    frameLeft += lastFrame - frameRight;
                    frameRight = lastFrame;
                }
                #endregion
            }

            currentFrame = Math.Min(Math.Max(0, (int)Math.Round(((lastMousePos.X - 20) * (frameRight - frameLeft) / (Width - 40.0) + frameLeft))), lastFrame);
            FrameChanged?.Invoke(this, new EventArgs());
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (lastFrame == 0)
                return;

            if (e.Button == MouseButtons.Left)
            {

                timer.Enabled = (e.X < 20 || e.X > Width - 20);
                Locked = true;
                currentFrame = Math.Min(Math.Max(0, (int)Math.Round(((e.X - 20) * (frameRight - frameLeft) / (Width - 40.0) + frameLeft))), lastFrame);
                FrameChanged?.Invoke(this, new EventArgs());
                Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                double delta = (e.X - lastMousePos.X) * (frameRight - frameLeft) / (Width - 40.0);
                frameLeft -= delta;
                frameRight -= delta;

                ResolveCollision();

                Refresh();
            }

            lastMousePos = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            timer.Stop();
            Locked = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (lastFrame == 0)
                return;

            if (frameRight - frameLeft <= 2 && e.Delta > 0)
                return;

            double delta = 1 + Math.Min(Math.Max(-0.5, -e.Delta * 0.00390625), 0.5);

            double frameOrigin = Math.Min(Math.Max(0, ((e.X - 20) * (frameRight - frameLeft) / (Width - 40.0) + frameLeft)), lastFrame);

            frameLeft = Math.Min(-1, (frameLeft - frameOrigin)) * delta + frameOrigin;
            frameRight = Math.Max(1, (frameRight - frameOrigin)) * delta + frameOrigin;

            ResolveFitting();

            Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            Refresh();
            base.OnResize(e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}
