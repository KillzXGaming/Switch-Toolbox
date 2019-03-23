namespace Toolbox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileAssociationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.tabForms = new Switch_Toolbox.Library.Forms.STTabControl();
            this.tabControlContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stToolStrip1 = new Switch_Toolbox.Library.Forms.STToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.tabControlContextMenuStrip.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.stToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.experimentalToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.newFromFileToolStripMenuItem,
            this.openToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // newFromFileToolStripMenuItem
            // 
            this.newFromFileToolStripMenuItem.Name = "newFromFileToolStripMenuItem";
            this.newFromFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newFromFileToolStripMenuItem.Text = "New From File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.recentToolStripMenuItem.Text = "Recent";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compressionToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // compressionToolStripMenuItem
            // 
            this.compressionToolStripMenuItem.Name = "compressionToolStripMenuItem";
            this.compressionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.compressionToolStripMenuItem.Text = "Compression";
            // 
            // experimentalToolStripMenuItem
            // 
            this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
            this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(87, 21);
            this.experimentalToolStripMenuItem.Text = "Experimental";
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cascadeToolStripMenuItem,
            this.minimizeToolStripMenuItem,
            this.maximizeToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.closeAllToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cascadeToolStripMenuItem.Text = "Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.cascadeToolStripMenuItem_Click);
            // 
            // minimizeToolStripMenuItem
            // 
            this.minimizeToolStripMenuItem.Name = "minimizeToolStripMenuItem";
            this.minimizeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.minimizeToolStripMenuItem.Text = "Minimize";
            this.minimizeToolStripMenuItem.Click += new System.EventHandler(this.minimizeToolStripMenuItem_Click);
            // 
            // maximizeToolStripMenuItem
            // 
            this.maximizeToolStripMenuItem.Name = "maximizeToolStripMenuItem";
            this.maximizeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.maximizeToolStripMenuItem.Text = "Maximize";
            this.maximizeToolStripMenuItem.Click += new System.EventHandler(this.maximizeToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeAllToolStripMenuItem.Text = "Close All";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.closeAllToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainSettingsToolStripMenuItem,
            this.fileAssociationsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 21);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // mainSettingsToolStripMenuItem
            // 
            this.mainSettingsToolStripMenuItem.Name = "mainSettingsToolStripMenuItem";
            this.mainSettingsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.mainSettingsToolStripMenuItem.Text = "Main Settings";
            this.mainSettingsToolStripMenuItem.Click += new System.EventHandler(this.mainSettingsToolStripMenuItem_Click);
            // 
            // fileAssociationsToolStripMenuItem
            // 
            this.fileAssociationsToolStripMenuItem.Enabled = false;
            this.fileAssociationsToolStripMenuItem.Name = "fileAssociationsToolStripMenuItem";
            this.fileAssociationsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.fileAssociationsToolStripMenuItem.Text = "File Associations";
            this.fileAssociationsToolStripMenuItem.Click += new System.EventHandler(this.fileAssociationsToolStripMenuItem_Click);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.menuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(800, 25);
            this.stPanel1.TabIndex = 5;
            // 
            // tabForms
            // 
            this.tabForms.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabForms.Location = new System.Drawing.Point(0, 55);
            this.tabForms.myBackColor = System.Drawing.Color.Empty;
            this.tabForms.Name = "tabForms";
            this.tabForms.SelectedIndex = 0;
            this.tabForms.Size = new System.Drawing.Size(800, 24);
            this.tabForms.TabIndex = 9;
            this.tabForms.Visible = false;
            this.tabForms.SelectedIndexChanged += new System.EventHandler(this.tabForms_SelectedIndexChanged);
            // 
            // tabControlContextMenuStrip
            // 
            this.tabControlContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem1});
            this.tabControlContextMenuStrip.Name = "contextMenuStrip1";
            this.tabControlContextMenuStrip.Size = new System.Drawing.Size(104, 26);
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem1.Text = "Close";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.CloseTab);
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stToolStrip1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel2.Location = new System.Drawing.Point(0, 25);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(800, 30);
            this.stPanel2.TabIndex = 7;
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.stToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(800, 30);
            this.stToolStrip1.TabIndex = 0;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Enabled = false;
            this.toolStripButton1.Image = global::Toolbox.Properties.Resources.Save;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 27);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Save File";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabForms);
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.stPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MdiChildActivate += new System.EventHandler(this.MainForm_MdiChildActivate);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.tabControlContextMenuStrip.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STMenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem experimentalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maximizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
        private Switch_Toolbox.Library.Forms.STTabControl tabForms;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip tabControlContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileAssociationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mainSettingsToolStripMenuItem;
        private Switch_Toolbox.Library.Forms.STPanel stPanel2;
        private Switch_Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem newFromFileToolStripMenuItem;
    }
}

