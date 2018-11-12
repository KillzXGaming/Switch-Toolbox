namespace Switch_Toolbox
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new Switch_Toolbox.Library.Forms.ContextMenuStripDark();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportShaderErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yaz0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yaz0DecompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yaz0CompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gzipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gzipCompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gzipDecompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showObjectlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockPanel1 = new Switch_Toolbox.Library.Forms.DockPanelCustom();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(779, 38);
            this.panel1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.pluginsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.experimentalToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(779, 38);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked_1);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.exportShaderErrorsToolStripMenuItem,
            this.saveConfigToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.clearWorkspaceToolStripMenuItem});
            this.fileToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.fileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 34);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.recentToolStripMenuItem.Text = "Recent";
            // 
            // exportShaderErrorsToolStripMenuItem
            // 
            this.exportShaderErrorsToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exportShaderErrorsToolStripMenuItem.Name = "exportShaderErrorsToolStripMenuItem";
            this.exportShaderErrorsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.exportShaderErrorsToolStripMenuItem.Text = "Export Shader Errors";
            this.exportShaderErrorsToolStripMenuItem.Click += new System.EventHandler(this.exportShaderErrorsToolStripMenuItem_Click);
            // 
            // saveConfigToolStripMenuItem
            // 
            this.saveConfigToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.saveConfigToolStripMenuItem.Name = "saveConfigToolStripMenuItem";
            this.saveConfigToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveConfigToolStripMenuItem.Text = "Save Config";
            this.saveConfigToolStripMenuItem.Click += new System.EventHandler(this.saveConfigToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click_1);
            // 
            // clearWorkspaceToolStripMenuItem
            // 
            this.clearWorkspaceToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.clearWorkspaceToolStripMenuItem.Name = "clearWorkspaceToolStripMenuItem";
            this.clearWorkspaceToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.clearWorkspaceToolStripMenuItem.Text = "Clear Workspace";
            this.clearWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.clearWorkspaceToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Checked = true;
            this.editToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.editToolStripMenuItem.Enabled = false;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 34);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 34);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compressionToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 34);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // compressionToolStripMenuItem
            // 
            this.compressionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yaz0ToolStripMenuItem,
            this.gzipToolStripMenuItem});
            this.compressionToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.compressionToolStripMenuItem.Name = "compressionToolStripMenuItem";
            this.compressionToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.compressionToolStripMenuItem.Text = "Compression";
            // 
            // yaz0ToolStripMenuItem
            // 
            this.yaz0ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yaz0DecompressToolStripMenuItem,
            this.yaz0CompressToolStripMenuItem});
            this.yaz0ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.yaz0ToolStripMenuItem.Name = "yaz0ToolStripMenuItem";
            this.yaz0ToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.yaz0ToolStripMenuItem.Text = "Yaz0";
            // 
            // yaz0DecompressToolStripMenuItem
            // 
            this.yaz0DecompressToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.yaz0DecompressToolStripMenuItem.Name = "yaz0DecompressToolStripMenuItem";
            this.yaz0DecompressToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.yaz0DecompressToolStripMenuItem.Text = "Decompress";
            this.yaz0DecompressToolStripMenuItem.Click += new System.EventHandler(this.yaz0DecompressToolStripMenuItem_Click);
            // 
            // yaz0CompressToolStripMenuItem
            // 
            this.yaz0CompressToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.yaz0CompressToolStripMenuItem.Name = "yaz0CompressToolStripMenuItem";
            this.yaz0CompressToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.yaz0CompressToolStripMenuItem.Text = "Compress";
            this.yaz0CompressToolStripMenuItem.Click += new System.EventHandler(this.yaz0CompressToolStripMenuItem_Click);
            // 
            // gzipToolStripMenuItem
            // 
            this.gzipToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gzipCompressToolStripMenuItem,
            this.gzipDecompressToolStripMenuItem});
            this.gzipToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.gzipToolStripMenuItem.Name = "gzipToolStripMenuItem";
            this.gzipToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.gzipToolStripMenuItem.Text = "GZIP";
            // 
            // gzipCompressToolStripMenuItem
            // 
            this.gzipCompressToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.gzipCompressToolStripMenuItem.Name = "gzipCompressToolStripMenuItem";
            this.gzipCompressToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.gzipCompressToolStripMenuItem.Text = "Compress";
            this.gzipCompressToolStripMenuItem.Click += new System.EventHandler(this.gzipCompressToolStripMenuItem_Click);
            // 
            // gzipDecompressToolStripMenuItem
            // 
            this.gzipDecompressToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.gzipDecompressToolStripMenuItem.Name = "gzipDecompressToolStripMenuItem";
            this.gzipDecompressToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.gzipDecompressToolStripMenuItem.Text = "Decompress";
            this.gzipDecompressToolStripMenuItem.Click += new System.EventHandler(this.gzipDecompressToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(58, 34);
            this.pluginsToolStripMenuItem.Text = "Plugins";
            this.pluginsToolStripMenuItem.Click += new System.EventHandler(this.pluginsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 34);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 34);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // experimentalToolStripMenuItem
            // 
            this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
            this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(87, 34);
            this.experimentalToolStripMenuItem.Text = "Experimental";
            // 
            // showObjectlistToolStripMenuItem
            // 
            this.showObjectlistToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.showObjectlistToolStripMenuItem.Name = "showObjectlistToolStripMenuItem";
            this.showObjectlistToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showObjectlistToolStripMenuItem.Text = "Show Objectlist";
            this.showObjectlistToolStripMenuItem.Click += new System.EventHandler(this.showObjectlistToolStripMenuItem_Click);
            // 
            // dockPanel1
            // 
            this.dockPanel1.AllowDrop = true;
            this.dockPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dockPanel1.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.dockPanel1.Location = new System.Drawing.Point(0, 38);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel1.ShowAutoHideContentOnHover = false;
            this.dockPanel1.Size = new System.Drawing.Size(780, 422);
            this.dockPanel1.TabIndex = 1;
            this.dockPanel1.DockChanged += new System.EventHandler(this.dockPanel1_DockChanged);
            this.dockPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.dockPanel1_DragDrop);
            this.dockPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.dockPanel1_DragEnter);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(779, 460);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Switch Toolbox";
            this.Load += new System.EventHandler(this.Form4_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Switch_Toolbox.Library.Forms.ContextMenuStripDark menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private Switch_Toolbox.Library.Forms.DockPanelCustom dockPanel1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showObjectlistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportShaderErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearWorkspaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yaz0ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yaz0DecompressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yaz0CompressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gzipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gzipCompressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gzipDecompressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem experimentalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}