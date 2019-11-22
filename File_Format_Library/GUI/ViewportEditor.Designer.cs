namespace FirstPlugin.Forms
{
    partial class ViewportEditor
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
            this.timelineTabPage = new Toolbox.Library.Forms.STPanel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.stPanel2.SuspendLayout();
            this.stToolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel3.SuspendLayout();
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
            this.stToolStrip1.HighlightSelectedTab = false;
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
            this.splitContainer1.Panel1.Controls.Add(this.splitter2);
            this.splitContainer1.Panel1.Controls.Add(this.timelineTabPage);
            this.splitContainer1.Panel1.Controls.Add(this.stPanel3);
            this.splitContainer1.Size = new System.Drawing.Size(712, 515);
            this.splitContainer1.SplitterDistance = 440;
            this.splitContainer1.TabIndex = 1;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.splitter1);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(0, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(440, 515);
            this.stPanel3.TabIndex = 4;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 512);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(440, 3);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // timelineTabPage
            // 
            this.timelineTabPage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.timelineTabPage.Location = new System.Drawing.Point(0, 380);
            this.timelineTabPage.Name = "timelineTabPage";
            this.timelineTabPage.Size = new System.Drawing.Size(440, 135);
            this.timelineTabPage.TabIndex = 5;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 377);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(440, 3);
            this.splitter2.TabIndex = 6;
            this.splitter2.TabStop = false;
            // 
            // ViewportEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel2);
            this.Name = "ViewportEditor";
            this.Size = new System.Drawing.Size(712, 543);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private System.Windows.Forms.Splitter splitter1;
        private Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton toggleViewportToolStripBtn;
        private System.Windows.Forms.Splitter splitter2;
        private Toolbox.Library.Forms.STPanel timelineTabPage;
    }
}
