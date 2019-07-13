using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System;

namespace Switch_Toolbox.Library.Forms
{
    public class STForm : Form
    {
        private bool canResize = true;
        public bool CanResize
        {
            get
            {
                return canResize;
            }
            set
            {
                canResize = value;

                if (BtnMinMax != null)
                    BtnMinMax.Visible = value;
            }
        }

        public void AddControl(Control control) {
            contentContainer.Controls.Add(control);
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;

                if (LblTitle != null)
                    LblTitle.Text = value;
            }
        }

        private int BorderSize = 5;

        private Color BorderColor;

        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        private const int HTCLIENT = 0x1;
        private const int HT_CAPTION = 0x2;
        private const int WM_SETREDRAW = 0xb;
        private const int WM_NCHITTEST = 0x84;
        protected STPanel contentContainer;
        private const int WM_NCLBUTTONDOWN = 0xa1;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private Panel TitleBar;
        private Label LblTitle;
        private PictureBox PicIcon;
        private PictureBox BtnMinimize;
        private PictureBox BtnMinMax;
        private PictureBox BtnClose;

        public STForm()
        {
            InitializeComponent();

            mLastState = this.WindowState;

            SetStyle(ControlStyles.ResizeRedraw, true);

            ForeColor = FormThemes.BaseTheme.FormForeColor;
            BackColor = FormThemes.BaseTheme.FormBackColor;
            BorderColor = FormThemes.BaseTheme.MDIChildBorderColor;
            TitleBar.BackColor = BorderColor;

            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        }

        protected override void OnCreateControl()
        {
            this.ControlBox = false;
            this.DoubleBuffered = true;

            base.OnCreateControl();
        }

        private void InitializeComponent()
        {
            this.contentContainer = new Switch_Toolbox.Library.Forms.STPanel();
            this.TitleBar = new System.Windows.Forms.Panel();
            this.LblTitle = new System.Windows.Forms.Label();
            this.PicIcon = new System.Windows.Forms.PictureBox();
            this.BtnMinimize = new System.Windows.Forms.PictureBox();
            this.BtnMinMax = new System.Windows.Forms.PictureBox();
            this.BtnClose = new System.Windows.Forms.PictureBox();
            this.contentContainer.SuspendLayout();
            this.TitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMinMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentContainer.Controls.Add(this.TitleBar);
            this.contentContainer.Location = new System.Drawing.Point(3, 2);
            this.contentContainer.Name = "contentContainer";
            this.contentContainer.Size = new System.Drawing.Size(543, 393);
            this.contentContainer.TabIndex = 11;
            // 
            // TitleBar
            // 
            this.TitleBar.BackColor = System.Drawing.Color.White;
            this.TitleBar.Controls.Add(this.LblTitle);
            this.TitleBar.Controls.Add(this.PicIcon);
            this.TitleBar.Controls.Add(this.BtnMinimize);
            this.TitleBar.Controls.Add(this.BtnMinMax);
            this.TitleBar.Controls.Add(this.BtnClose);
            this.TitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitleBar.Location = new System.Drawing.Point(0, 0);
            this.TitleBar.Name = "TitleBar";
            this.TitleBar.Size = new System.Drawing.Size(543, 25);
            this.TitleBar.TabIndex = 10;
            this.TitleBar.DoubleClick += new System.EventHandler(this.TitleBar_DoubleClick);
            this.TitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            // 
            // LblTitle
            // 
            this.LblTitle.AutoSize = true;
            this.LblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblTitle.Location = new System.Drawing.Point(20, 3);
            this.LblTitle.Name = "LblTitle";
            this.LblTitle.Size = new System.Drawing.Size(33, 17);
            this.LblTitle.TabIndex = 5;
            this.LblTitle.Text = "Title";
            this.LblTitle.DoubleClick += new System.EventHandler(this.LblTitle_DoubleClick);
            this.LblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            // 
            // PicIcon
            // 
            this.PicIcon.Location = new System.Drawing.Point(5, 5);
            this.PicIcon.Name = "PicIcon";
            this.PicIcon.Size = new System.Drawing.Size(16, 16);
            this.PicIcon.TabIndex = 4;
            this.PicIcon.TabStop = false;
            this.PicIcon.DoubleClick += new System.EventHandler(this.PicIcon_DoubleClick);
            // 
            // BtnMinimize
            // 
            this.BtnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.BtnMinimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnMinimize.Image = global::Switch_Toolbox.Library.Properties.Resources.minimize;
            this.BtnMinimize.Location = new System.Drawing.Point(429, 0);
            this.BtnMinimize.Name = "BtnMinimize";
            this.BtnMinimize.Size = new System.Drawing.Size(38, 25);
            this.BtnMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.BtnMinimize.TabIndex = 3;
            this.BtnMinimize.TabStop = false;
            this.BtnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            this.BtnMinimize.MouseEnter += new System.EventHandler(this.BtnMinimize_MouseEnter);
            this.BtnMinimize.MouseLeave += new System.EventHandler(this.BtnMinimize_MouseLeave);
            // 
            // BtnMinMax
            // 
            this.BtnMinMax.BackColor = System.Drawing.Color.Transparent;
            this.BtnMinMax.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnMinMax.Image = global::Switch_Toolbox.Library.Properties.Resources.maximize;
            this.BtnMinMax.Location = new System.Drawing.Point(467, 0);
            this.BtnMinMax.Name = "BtnMinMax";
            this.BtnMinMax.Size = new System.Drawing.Size(38, 25);
            this.BtnMinMax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.BtnMinMax.TabIndex = 2;
            this.BtnMinMax.TabStop = false;
            this.BtnMinMax.Click += new System.EventHandler(this.BtnMinMax_Click);
            this.BtnMinMax.MouseEnter += new System.EventHandler(this.BtnMinMax_MouseEnter);
            this.BtnMinMax.MouseLeave += new System.EventHandler(this.BtnMinMax_MouseLeave);
            // 
            // BtnClose
            // 
            this.BtnClose.BackColor = System.Drawing.Color.Transparent;
            this.BtnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnClose.Image = global::Switch_Toolbox.Library.Properties.Resources.Close;
            this.BtnClose.Location = new System.Drawing.Point(505, 0);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(38, 25);
            this.BtnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.BtnClose.TabIndex = 1;
            this.BtnClose.TabStop = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.BtnClose.MouseEnter += new System.EventHandler(this.BtnClose_MouseEnter);
            this.BtnClose.MouseLeave += new System.EventHandler(this.BtnClose_MouseLeave);
            // 
            // STForm
            // 
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.ControlBox = false;
            this.Controls.Add(this.contentContainer);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "STForm";
            this.Shown += new System.EventHandler(this.STForm_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.STForm_Paint);
            this.Enter += new System.EventHandler(this.STForm_Enter);
            this.Leave += new System.EventHandler(this.STForm_Leave);
            this.contentContainer.ResumeLayout(false);
            this.TitleBar.ResumeLayout(false);
            this.TitleBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMinMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #region Events

        private void BtnMinimize_Click(object sender, System.EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BtnMinMax_Click(object sender, System.EventArgs e)
        {
            CheckWindowState();
        }
        public void Maximize()
        {
            if (IsMdiChild)
                MDIMaximized();

    //        MaximumSize = Screen.FromControl(this).WorkingArea.Size;
            WindowState = FormWindowState.Maximized;
        }
        private void CheckWindowState()
        {
            if (WindowState == FormWindowState.Maximized)
            {
                BtnMinMax.Image = Properties.Resources.maximize;
                WindowState = FormWindowState.Normal;

                if (IsMdiChild)
                    MDIWindowed();
            }
            else
            {
                BtnMinMax.Image = Properties.Resources.maximized;

                Maximize();
            }
            Refresh();
        }

        public void MDIWindowed()
        {
            TitleBar.Show();

            this.ClientSize = new System.Drawing.Size(549, 398);
        }
        public void MDIMaximized()
        {
             TitleBar.Hide();
        }

        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void BtnClose_MouseEnter(object sender, System.EventArgs e)
        {
            BtnClose.Image = Properties.Resources.Close_Hover;
        }

        private void BtnClose_MouseLeave(object sender, System.EventArgs e)
        {
            BtnClose.Image = Properties.Resources.Close;
        }

        private void BtnMinMax_MouseEnter(object sender, EventArgs e)
        {
            BtnMinMax.Image = Properties.Resources.maximize_sele;
        }

        private void BtnMinMax_MouseLeave(object sender, EventArgs e)
        {
            BtnMinMax.Image = Properties.Resources.maximize;
        }

        private void BtnMinimize_MouseEnter(object sender, EventArgs e)
        {
            BtnMinimize.Image = Properties.Resources.minimize_sele;
        }

        private void BtnMinimize_MouseLeave(object sender, EventArgs e)
        {
            BtnMinimize.Image = Properties.Resources.minimize;
        }


        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Y < 28 && e.Clicks == 1)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void STForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(BorderColor, BorderSize),
                         this.DisplayRectangle);

            using (SolidBrush brush = new SolidBrush(BorderColor))
            {
                e.Graphics.FillRectangle(brush, Top);
                e.Graphics.FillRectangle(brush, Left);
                e.Graphics.FillRectangle(brush, Right);
                e.Graphics.FillRectangle(brush, Bottom);
            }
        }

        private void STForm_Leave(object sender, EventArgs e)
        {
            return;

            BorderColor = FormThemes.BaseTheme.DisabledBorderColor;
            TitleBar.BackColor = BorderColor;
            LblTitle.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            Refresh();
        }

        private void STForm_Enter(object sender, EventArgs e)
        {
            return;

            BorderColor = FormThemes.BaseTheme.MDIChildBorderColor;
            TitleBar.BackColor = BorderColor;
            LblTitle.ForeColor = FormThemes.BaseTheme.TextForeColor;
            Refresh();
        }


        #endregion

        #region Shadow

        private void STForm_Shown(object sender, EventArgs e)
        {

        }

        private FormWindowState mLastState;

        protected override void OnClientSizeChanged(EventArgs e)
        {
       /*     if (this.WindowState != mLastState)
            {
                mLastState = this.WindowState;
                OnWindowStateChanged(e);
            }
            base.OnClientSizeChanged(e);*/
        }
        protected void OnWindowStateChanged(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                if (IsMdiChild)
                    MDIWindowed();
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                if (IsMdiChild)
                    MDIMaximized();
            }
        }

        #endregion

        #region Border
        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, BorderSize); } }
        Rectangle Left { get { return new Rectangle(0, 0, BorderSize, this.ClientSize.Height); } }

        private void TitleBar_DoubleClick(object sender, EventArgs e)
        {
            if (CanResize)
            {
                Maximize();
            }
        }

        private void LblTitle_DoubleClick(object sender, EventArgs e)
        {
            if (CanResize)
            {
                Maximize();
            }
        }

        private void PicIcon_DoubleClick(object sender, EventArgs e)
        {
            if (CanResize)
            {
                Maximize();
            }
        }

        Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - BorderSize, this.ClientSize.Width, BorderSize); } }
        Rectangle Right { get { return new Rectangle(this.ClientSize.Width - BorderSize, 0, BorderSize, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, BorderSize, BorderSize); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - BorderSize, 0, BorderSize, BorderSize); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - BorderSize, BorderSize, BorderSize); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - BorderSize, this.ClientSize.Height - BorderSize, BorderSize, BorderSize); } }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && CanResize && WindowState == FormWindowState.Normal)
            {
                int x = message.LParam.ToInt32() & 0xffff;
                int y = (int)((message.LParam.ToInt32() & 0xffff0000) >> 16);

                Point cursor = PointToClient(new Point(x, y));

                //  var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }
    }
    #endregion

}