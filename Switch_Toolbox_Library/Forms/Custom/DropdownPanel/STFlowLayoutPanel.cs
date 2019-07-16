using System;
using System.Drawing;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class STFlowLayoutPanel : FlowLayoutPanel
    {
        public bool FixedWidth { get; set; } = true;
        public bool FixedHeight { get; set; }

        public STFlowLayoutPanel()
        {
            InitializeComponent();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (FixedWidth)
            {
                foreach (Control ctrl in Controls)
                    FillWidth(ctrl);
            }
            else if (FixedHeight)
            {
                foreach (Control ctrl in Controls)
                    FillHeight(ctrl);
            }

            base.OnSizeChanged(e);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();
            base.OnScroll(se);
        }

        protected void FillHeight(Control ctrl)
        {
            ctrl.Size = new Size(ClientSize.Height - ctrl.Margin.Bottom - ctrl.Margin.Top, ctrl.Width);
        }

        protected void FillWidth(Control ctrl)
        {
            ctrl.Size = new Size(ClientSize.Width - ctrl.Margin.Left - ctrl.Margin.Right, ctrl.Height);
        }

        private void InitializeComponent()
        {
            this.Margin = Padding.Empty;
            this.Padding = Padding.Empty;
            this.DoubleBuffered = true;
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
