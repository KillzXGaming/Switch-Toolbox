namespace FirstPlugin.Forms
{
    partial class BfresEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.toggleViewportToolStripBtn = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stTabControl2 = new Toolbox.Library.Forms.STTabControl();
            this.timelineTabPage = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.textureLoader1 = new Forms.TextureLoader();
            this.stPanel5 = new Toolbox.Library.Forms.STPanel();
            this.stPanel2.SuspendLayout();
            this.stToolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stTabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stToolStrip1);
            this.stPanel2.Controls.Add(this.splitContainer1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(712, 543);
            this.stPanel2.TabIndex = 4;
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleViewportToolStripBtn});
            this.stToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(712, 25);
            this.stToolStrip1.TabIndex = 2;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // toggleViewportToolStripBtn
            // 
            this.toggleViewportToolStripBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleViewportToolStripBtn.Image = global::FirstPlugin.Properties.Resources.ViewportIcon;
            this.toggleViewportToolStripBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleViewportToolStripBtn.Name = "toggleViewportToolStripBtn";
            this.toggleViewportToolStripBtn.Size = new System.Drawing.Size(23, 22);
            this.toggleViewportToolStripBtn.Text = "toolStripButton1";
            this.toggleViewportToolStripBtn.Click += new System.EventHandler(this.toggleViewportToolStripBtn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.stPanel3);
            this.splitContainer1.Size = new System.Drawing.Size(712, 515);
            this.splitContainer1.SplitterDistance = 440;
            this.splitContainer1.TabIndex = 1;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.splitter1);
            this.stPanel3.Controls.Add(this.stPanel1);
            this.stPanel3.Controls.Add(this.stPanel5);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(0, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(440, 515);
            this.stPanel3.TabIndex = 4;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 366);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(440, 3);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stTabControl2);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel1.Location = new System.Drawing.Point(0, 369);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(440, 146);
            this.stPanel1.TabIndex = 3;
            this.stPanel1.DoubleClick += new System.EventHandler(this.stPanel1_DoubleClick);
            // 
            // stTabControl2
            // 
            this.stTabControl2.Controls.Add(this.timelineTabPage);
            this.stTabControl2.Controls.Add(this.tabPage5);
            this.stTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl2.Location = new System.Drawing.Point(0, 0);
            this.stTabControl2.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl2.Name = "stTabControl2";
            this.stTabControl2.SelectedIndex = 0;
            this.stTabControl2.Size = new System.Drawing.Size(440, 146);
            this.stTabControl2.TabIndex = 0;
            // 
            // timelineTabPage
            // 
            this.timelineTabPage.Location = new System.Drawing.Point(4, 25);
            this.timelineTabPage.Name = "timelineTabPage";
            this.timelineTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.timelineTabPage.Size = new System.Drawing.Size(432, 117);
            this.timelineTabPage.TabIndex = 3;
            this.timelineTabPage.Text = "Timeline";
            this.timelineTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.textureLoader1);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(432, 117);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Textures";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // textureLoader1
            // 
            this.textureLoader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureLoader1.Location = new System.Drawing.Point(3, 3);
            this.textureLoader1.Name = "textureLoader1";
            this.textureLoader1.Size = new System.Drawing.Size(426, 111);
            this.textureLoader1.TabIndex = 0;
            // 
            // stPanel5
            // 
            this.stPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel5.Location = new System.Drawing.Point(0, 0);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(440, 515);
            this.stPanel5.TabIndex = 2;
            // 
            // BfresEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel2);
            this.Name = "BfresEditor";
            this.Size = new System.Drawing.Size(712, 543);
            this.Enter += new System.EventHandler(this.BfresEditor_Enter);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stTabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private System.Windows.Forms.Splitter splitter1;
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STTabControl stTabControl2;
        private System.Windows.Forms.TabPage timelineTabPage;
        private System.Windows.Forms.TabPage tabPage5;
        private Forms.TextureLoader textureLoader1;
        private Toolbox.Library.Forms.STPanel stPanel5;
        private Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton toggleViewportToolStripBtn;
    }
}
