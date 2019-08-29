namespace LayoutBXLYT
{
    partial class LayoutEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutEditor));
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.backColorDisplay = new System.Windows.Forms.PictureBox();
            this.viewportBackColorCB = new Toolbox.Library.Forms.STComboBox();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.stMenuStrip1 = new Toolbox.Library.Forms.STMenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.backColorDisplay)).BeginInit();
            this.stToolStrip1.SuspendLayout();
            this.stMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.Location = new System.Drawing.Point(0, 49);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(549, 349);
            this.dockPanel1.TabIndex = 6;
            // 
            // backColorDisplay
            // 
            this.backColorDisplay.Location = new System.Drawing.Point(319, 25);
            this.backColorDisplay.Name = "backColorDisplay";
            this.backColorDisplay.Size = new System.Drawing.Size(21, 21);
            this.backColorDisplay.TabIndex = 10;
            this.backColorDisplay.TabStop = false;
            this.backColorDisplay.Click += new System.EventHandler(this.backColorDisplay_Click);
            // 
            // viewportBackColorCB
            // 
            this.viewportBackColorCB.BorderColor = System.Drawing.Color.Empty;
            this.viewportBackColorCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.viewportBackColorCB.ButtonColor = System.Drawing.Color.Empty;
            this.viewportBackColorCB.FormattingEnabled = true;
            this.viewportBackColorCB.IsReadOnly = false;
            this.viewportBackColorCB.Location = new System.Drawing.Point(167, 25);
            this.viewportBackColorCB.Name = "viewportBackColorCB";
            this.viewportBackColorCB.Size = new System.Drawing.Size(146, 21);
            this.viewportBackColorCB.TabIndex = 9;
            this.viewportBackColorCB.SelectedIndexChanged += new System.EventHandler(this.viewportBackColorCB_SelectedIndexChanged);
            this.viewportBackColorCB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.stComboBox1_MouseDoubleClick);
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.stToolStrip1.Location = new System.Drawing.Point(0, 24);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(549, 25);
            this.stToolStrip1.TabIndex = 3;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // stMenuStrip1
            // 
            this.stMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.stMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stMenuStrip1.Name = "stMenuStrip1";
            this.stMenuStrip1.Size = new System.Drawing.Size(549, 24);
            this.stMenuStrip1.TabIndex = 0;
            this.stMenuStrip1.Text = "stMenuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textureListToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // textureListToolStripMenuItem
            // 
            this.textureListToolStripMenuItem.Name = "textureListToolStripMenuItem";
            this.textureListToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.textureListToolStripMenuItem.Text = "Texture List";
            this.textureListToolStripMenuItem.Click += new System.EventHandler(this.textureListToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // LayoutEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Controls.Add(this.backColorDisplay);
            this.Controls.Add(this.viewportBackColorCB);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.stToolStrip1);
            this.Controls.Add(this.stMenuStrip1);
            this.IsMdiContainer = true;
            this.Name = "LayoutEditor";
            this.ParentChanged += new System.EventHandler(this.LayoutEditor_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this.backColorDisplay)).EndInit();
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            this.stMenuStrip1.ResumeLayout(false);
            this.stMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STMenuStrip stMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureListToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private Toolbox.Library.Forms.STComboBox viewportBackColorCB;
        private System.Windows.Forms.PictureBox backColorDisplay;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    }
}
