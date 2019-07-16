using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    class BoneAnimTimeline : TimeLine
    {
        int[] keyFrames = new int[] { 0, 5, 15, 20, 40, 100 };

        protected static int lineHeight = TextRenderer.MeasureText("§", font).Height;

        protected int scrollY = 0;

        protected int trackCount = 30;

        public BoneAnimTimeline()
        {
            margin = 100;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(brush3, new Rectangle(0, 0, margin, Height));

            e.Graphics.SetClip(new Rectangle(0, barHeight, Width, Height - barHeight));

            bool v = false;
            int y = -scrollY;
            for (int _i = 0; _i < trackCount; _i++)
            {
                e.Graphics.DrawString("bone" + _i, font, brush5, new Point(10, barHeight + y));
                for (int i = 1; i < keyFrames.Length; i++)
                {
                    int l = Math.Max(-20, (int)((
                        keyFrames[i - 1]
                        - frameLeft) * (Width - 40 - margin) / (frameRight - frameLeft)));
                    int r = (int)((
                        keyFrames[i]
                        - frameLeft) * (Width - 40 - margin) / (frameRight - frameLeft));

                    if (v = !v)
                        e.Graphics.FillRectangle(brush5, new Rectangle(l + margin + 20, barHeight + y, r - l, lineHeight));
                }
                y += lineHeight;
            }
            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.X < margin)
            {
                scrollY = Math.Max(Math.Min(trackCount * lineHeight + barHeight - Height, scrollY - e.Delta / 2), 0);
                Refresh();
            }
            else
                base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            scrollY = Math.Max(Math.Min(trackCount * lineHeight + barHeight - Height, scrollY), 0);
            base.OnResize(e);
        }
    }
}
