using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class TimeLine : UserControl
    {
        public TimeLine()
        {
            InitializeComponent();
            timer.Interval = 10;
            timer.Tick += Timer_Tick;
        }

        public void Play() { }
        public void Stop() { }

        protected int margin = 0;

        private int startTime = 0;
        public int StartTime
        {
            set {
                if (value == FrameCount)
                    startTime = value - 1;
                else
                    startTime = value;
            }
        }

        public event EventHandler FrameChanged;

        private Timer timer = new Timer();

        public bool Locked { get; private set; } = false;

        private void ResolveCollision()
        {
            if (frameLeft < startTime)
            {
                frameRight += startTime - frameLeft;
                frameLeft = startTime;
            }
            else if (frameRight > lastFrame)
            {
                frameLeft += lastFrame - frameRight;
                frameRight = lastFrame;
            }
        }

        private void ResolveFitting()
        {
            if (frameLeft < startTime)
            {
                frameRight += startTime - frameLeft;
                if (frameRight > lastFrame)
                    frameRight = lastFrame;
                frameLeft = startTime;
            }
            else if (frameRight > lastFrame)
            {
                frameLeft += lastFrame - frameRight;
                if (frameLeft < startTime)
                    frameLeft = startTime;
                frameRight = lastFrame;
            }
        }

        public float CurrentFrame
        {
            get { return currentFrame; }
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

        public float FrameCount
        {
            get { return lastFrame + 1; }
            set
            {
                if (value == startTime)
                    lastFrame = value + 1;
                else
                    lastFrame = value;

                if (value == 1)
                {
                    frameLeft = startTime;
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

        protected float currentFrame = 0;
        protected float lastFrame = 1000;

        protected double frameLeft = 0;
        protected double frameRight = 200;

        protected Point lastMousePos;


        protected static Brush brush1 = new SolidBrush(FormThemes.BaseTheme.TimelineNumberColor);
        protected static Brush brush2 = new SolidBrush(FormThemes.BaseTheme.TimelineOverlayColor);
        protected static Brush brush3 = new SolidBrush(FormThemes.BaseTheme.TimelineBackColor);
        protected static Brush brush4 = new SolidBrush(Color.FromArgb(90, 90, 90));
        protected static Brush brush5 = new SolidBrush(Color.FromArgb(150, 150, 150));

        protected static Pen pen1 = new Pen(new SolidBrush(FormThemes.BaseTheme.TimelineLineColor), 2);
        protected static Pen pen2 = new Pen(new SolidBrush(FormThemes.BaseTheme.TimelineLine2Color), 2);
        protected static Pen pen3 = new Pen(new SolidBrush(Color.FromArgb(150, 150, 150)), 2);

        protected static Font font = new Font(new FontFamily("arial"), 10);

        protected static int barHeight = TextRenderer.MeasureText("0", font).Height;


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SetClip(new Rectangle(margin, 0, Width - margin, Height));

            double currentFrameX = 20 + margin + (currentFrame - frameLeft) * (Width - 40 - margin) / (frameRight - frameLeft);

            e.Graphics.FillRectangle(brush2, new Rectangle(0, 0, Width, Height));
            e.Graphics.FillRectangle(brush3, new Rectangle(0, 0, Width, barHeight));

            double step = 50 * (frameRight - frameLeft) / Width;
            if (step > 10)
                step = Math.Round(step / 10.0) * 10;
            else
            {
                step = Math.Round(Math.Max(1, step));
            }

            if (frameRight != frameLeft)
            {
                double max;
                if (frameRight < lastFrame)
                    max = Math.Min(frameRight + step, lastFrame);
                else
                    max = frameRight - step;

                for (double frame = Math.Floor(frameLeft / step) * step; frame <= max; frame += step)
                {
                    double frameX = 20 + margin + (frame - frameLeft) * (Width - 40 - margin) / (frameRight - frameLeft);

                    e.Graphics.DrawLine(pen1, new Point((int)frameX, barHeight), new Point((int)frameX, Height));

                    e.Graphics.DrawString("" + frame, font, brush4, new Point((int)frameX - TextRenderer.MeasureText("" + frame, font).Width / 2, 0));
                }
            }

            if (frameRight == lastFrame)
            {
                //draw last frame regardless of the steps
                double x = Width - 20;

                e.Graphics.DrawLine(pen1, new Point((int)x, barHeight), new Point((int)x, Height));

                e.Graphics.DrawString("" + lastFrame, font, brush4, new Point((int)x - TextRenderer.MeasureText("" + lastFrame, font).Width / 2, 0));
            }
            /*
			int lastY = value((int)(Math.Round(- 20+margin * (frameRight - frameLeft) / (Width - 40-margin) + frameLeft) + lastFrame + 1) % (lastFrame + 1));
			for (int x = 5; x < Width+10; x+=5)
			{
				e.Graphics.DrawLine(pen3, x - 5, lastY, x, lastY =
					value((int)(Math.Round((x - 20+margin) * (frameRight - frameLeft) / (Width - 40-margin) + frameLeft) + lastFrame + 1) % (lastFrame + 1))
					);
			}
			*/
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SetClip(new Rectangle(margin, 0, Width - margin, Height));

            double currentFrameX = 20 + margin + (currentFrame - frameLeft) * (Width - 40 - margin) / (frameRight - frameLeft);

            base.OnPaint(e);
            if (frameLeft != frameRight)
                e.Graphics.DrawLine(pen2, new Point((int)currentFrameX, barHeight), new Point((int)currentFrameX, Height));

            e.Graphics.DrawString("" + currentFrame, font, brush1, new Point((int)currentFrameX - TextRenderer.MeasureText("" + currentFrame, font).Width / 2, 0));
        }

        private int value(int frame)
        {
            return frame + 30;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double step;
            if (lastMousePos.X < 20 + margin)
            {
                step = (20 + margin - lastMousePos.X) * (frameRight - frameLeft) / Width;
                frameLeft -= step;
                frameRight -= step;

                #region resolve collsions
                if (frameLeft < startTime)
                {
                    frameRight -= frameLeft;
                    frameLeft = startTime;
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

            currentFrame = Math.Min(Math.Max(startTime, (int)Math.Round(((lastMousePos.X - 20 - margin) * (frameRight - frameLeft) / (Width - 40 - margin) + frameLeft))), lastFrame);
            FrameChanged?.Invoke(this, new EventArgs());
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (lastFrame == startTime)
                return;

            if (e.Button == MouseButtons.Left)
            {

                timer.Enabled = (e.X < 20 + margin || e.X > Width - 20);
                Locked = true;
                currentFrame = Math.Min(Math.Max(startTime, (int)Math.Round(((e.X - 20 - margin) * (frameRight - frameLeft) / (Width - 40 - margin) + frameLeft))), lastFrame);
                FrameChanged?.Invoke(this, new EventArgs());
                Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                double delta = (e.X - lastMousePos.X) * (frameRight - frameLeft) / (Width - 40 - margin);
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
            if (lastFrame == startTime)
                return;

            if (frameRight - frameLeft <= 2 && e.Delta > 0)
                return;

            double delta = 1 + Math.Min(Math.Max(-0.5, -e.Delta * 0.00390625), 0.5);

            double frameOrigin = Math.Min(Math.Max(startTime, ((e.X - 20 - margin) * (frameRight - frameLeft) / (Width - 40 - margin) + frameLeft)), lastFrame);

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
