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
            this.menuStrip1 = new Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hashCalculatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchExportTexturesAllSupportedFormatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchExportAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchExportModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchReplaceFTPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchRenameBNTXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchReplaceTXTGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.consoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.requestFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tutorialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportBugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.requestFeatureToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.githubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.tabForms = new Toolbox.Library.Forms.STTabControl();
            this.tabControlContextMenuStrip = new Toolbox.Library.Forms.STContextMenuStrip(this.components);
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.BtnMdiMinimize = new System.Windows.Forms.PictureBox();
            this.BtnMdiMinMax = new System.Windows.Forms.PictureBox();
            this.BtnMdiClose = new System.Windows.Forms.PictureBox();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.updateToolstrip = new System.Windows.Forms.ToolStripButton();
            this.openUserFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.tabControlContextMenuStrip.SuspendLayout();
            this.stPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMdiMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMdiMinMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMdiClose)).BeginInit();
            this.stToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.HighlightSelectedTab = false;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.experimentalToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.consoleToolStripMenuItem,
            this.requestFeatureToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1108, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.newFromFileToolStripMenuItem,
            this.openToolStripMenuItem,
            this.openFolderToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.openUserFolderToolStripMenuItem,
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
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openFolderToolStripMenuItem.Text = "Open (Folder)";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
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
            this.compressionToolStripMenuItem,
            this.batchToolStripMenuItem,
            this.hashCalculatorToolStripMenuItem,
            this.batchExportTexturesAllSupportedFormatsToolStripMenuItem,
            this.batchExportAnimationsToolStripMenuItem,
            this.batchExportModelsToolStripMenuItem,
            this.batchReplaceFTPToolStripMenuItem,
            this.batchReplaceTXTGToolStripMenuItem,
            this.batchRenameBNTXToolStripMenuItem,});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 21);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // compressionToolStripMenuItem
            // 
            this.compressionToolStripMenuItem.Name = "compressionToolStripMenuItem";
            this.compressionToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.compressionToolStripMenuItem.Text = "Compression";
            // 
            // batchToolStripMenuItem
            // 
            this.batchToolStripMenuItem.Name = "batchToolStripMenuItem";
            this.batchToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.batchToolStripMenuItem.Text = "Batch Set File Table";
            this.batchToolStripMenuItem.Click += new System.EventHandler(this.batchToolStripMenuItem_Click);
            // 
            // hashCalculatorToolStripMenuItem
            // 
            this.hashCalculatorToolStripMenuItem.Name = "hashCalculatorToolStripMenuItem";
            this.hashCalculatorToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.hashCalculatorToolStripMenuItem.Text = "Hash Calculator";
            this.hashCalculatorToolStripMenuItem.Click += new System.EventHandler(this.hashCalculatorToolStripMenuItem_Click);
            // 
            // batchExportTexturesAllSupportedFormatsToolStripMenuItem
            // 
            this.batchExportTexturesAllSupportedFormatsToolStripMenuItem.Name = "batchExportTexturesAllSupportedFormatsToolStripMenuItem";
            this.batchExportTexturesAllSupportedFormatsToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.batchExportTexturesAllSupportedFormatsToolStripMenuItem.Text = "Batch Export Textures";
            this.batchExportTexturesAllSupportedFormatsToolStripMenuItem.Click += new System.EventHandler(this.batchExportTexturesAllSupportedFormatsToolStripMenuItem_Click);
            // 

            // batchExportAnimationsToolStripMenuItem
            // 
            this.batchExportAnimationsToolStripMenuItem.Name = "batchExportAnimationsToolStripMenuItem";
            this.batchExportAnimationsToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.batchExportAnimationsToolStripMenuItem.Text = "Batch Export Animations";
            this.batchExportAnimationsToolStripMenuItem.Click += new System.EventHandler(this.batchExportAnimationsToolStripMenuItem_Click);
            // 

            // batchExportModelsToolStripMenuItem
            // 
            this.batchExportModelsToolStripMenuItem.Name = "batchExportModelsToolStripMenuItem";
            this.batchExportModelsToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.batchExportModelsToolStripMenuItem.Text = "Batch Export Models";
            this.batchExportModelsToolStripMenuItem.Click += new System.EventHandler(this.batchExportModelsToolStripMenuItem_Click);
            // 
            // batchReplaceFTPToolStripMenuItem
            // 
            this.batchReplaceFTPToolStripMenuItem.Name = "batchReplaceFTPToolStripMenuItem";
            this.batchReplaceFTPToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.batchReplaceFTPToolStripMenuItem.Text = "Batch Replace FTP";
            this.batchReplaceFTPToolStripMenuItem.Click += new System.EventHandler(this.batchReplaceFTPToolStripMenuItem_Click);
            // 
            // batchReplaceTXTGToolStripMenuItem
            // 
            this.batchReplaceTXTGToolStripMenuItem.Name = "batchReplaceTXTGToolStripMenuItem";
            this.batchReplaceTXTGToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.batchReplaceTXTGToolStripMenuItem.Text = "Batch Replace TXTG";
            this.batchReplaceTXTGToolStripMenuItem.Click += new System.EventHandler(this.batchReplaceTXTGToolStripMenuItem_Click);
            // 
            // batchRenameBNTXToolStripMenuItem
            // 
            this.batchRenameBNTXToolStripMenuItem.Name = "batchRenameBNTXToolStripMenuItem";
            this.batchRenameBNTXToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.batchRenameBNTXToolStripMenuItem.Text = "Batch Rename BNTX from Filename";
            this.batchRenameBNTXToolStripMenuItem.Click += new System.EventHandler(this.batchRenameBNTXToolStripMenuItem_Click);
            // 
            // experimentalToolStripMenuItem
            // 
            this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
            this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(88, 21);
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
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.cascadeToolStripMenuItem.Text = "Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.cascadeToolStripMenuItem_Click);
            // 
            // minimizeToolStripMenuItem
            // 
            this.minimizeToolStripMenuItem.Name = "minimizeToolStripMenuItem";
            this.minimizeToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.minimizeToolStripMenuItem.Text = "Minimize";
            this.minimizeToolStripMenuItem.Click += new System.EventHandler(this.minimizeToolStripMenuItem_Click);
            // 
            // maximizeToolStripMenuItem
            // 
            this.maximizeToolStripMenuItem.Name = "maximizeToolStripMenuItem";
            this.maximizeToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.maximizeToolStripMenuItem.Text = "Maximize";
            this.maximizeToolStripMenuItem.Click += new System.EventHandler(this.maximizeToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
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
            // consoleToolStripMenuItem
            // 
            this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
            this.consoleToolStripMenuItem.Size = new System.Drawing.Size(62, 21);
            this.consoleToolStripMenuItem.Text = "Console";
            this.consoleToolStripMenuItem.Click += new System.EventHandler(this.consoleToolStripMenuItem_Click);
            // 
            // requestFeatureToolStripMenuItem
            // 
            this.requestFeatureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1,
            this.tutorialToolStripMenuItem,
            this.reportBugToolStripMenuItem,
            this.requestFeatureToolStripMenuItem1,
            this.githubToolStripMenuItem,
            this.donateToolStripMenuItem});
            this.requestFeatureToolStripMenuItem.Name = "requestFeatureToolStripMenuItem";
            this.requestFeatureToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.requestFeatureToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // tutorialToolStripMenuItem
            // 
            this.tutorialToolStripMenuItem.Name = "tutorialToolStripMenuItem";
            this.tutorialToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.tutorialToolStripMenuItem.Text = "Tutorials";
            this.tutorialToolStripMenuItem.Click += new System.EventHandler(this.tutorialToolStripMenuItem_Click);
            // 
            // reportBugToolStripMenuItem
            // 
            this.reportBugToolStripMenuItem.Name = "reportBugToolStripMenuItem";
            this.reportBugToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.reportBugToolStripMenuItem.Text = "Report Bug";
            this.reportBugToolStripMenuItem.Click += new System.EventHandler(this.reportBugToolStripMenuItem_Click);
            // 
            // requestFeatureToolStripMenuItem1
            // 
            this.requestFeatureToolStripMenuItem1.Name = "requestFeatureToolStripMenuItem1";
            this.requestFeatureToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.requestFeatureToolStripMenuItem1.Text = "Request Feature";
            this.requestFeatureToolStripMenuItem1.Click += new System.EventHandler(this.requestFeatureToolStripMenuItem1_Click);
            // 
            // githubToolStripMenuItem
            // 
            this.githubToolStripMenuItem.Name = "githubToolStripMenuItem";
            this.githubToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.githubToolStripMenuItem.Text = "Github";
            this.githubToolStripMenuItem.Click += new System.EventHandler(this.githubToolStripMenuItem_Click);
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.donateToolStripMenuItem.Text = "Donate";
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.donateToolStripMenuItem_Click);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.menuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(1108, 25);
            this.stPanel1.TabIndex = 5;
            // 
            // tabForms
            // 
            this.tabForms.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabForms.Location = new System.Drawing.Point(0, 49);
            this.tabForms.myBackColor = System.Drawing.Color.Empty;
            this.tabForms.Name = "tabForms";
            this.tabForms.SelectedIndex = 0;
            this.tabForms.Size = new System.Drawing.Size(1108, 24);
            this.tabForms.TabIndex = 9;
            this.tabForms.Visible = false;
            this.tabForms.SelectedIndexChanged += new System.EventHandler(this.tabForms_SelectedIndexChanged);
            this.tabForms.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabForms_MouseClick);
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
            this.stPanel2.Controls.Add(this.BtnMdiMinimize);
            this.stPanel2.Controls.Add(this.BtnMdiMinMax);
            this.stPanel2.Controls.Add(this.BtnMdiClose);
            this.stPanel2.Controls.Add(this.stToolStrip1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel2.Location = new System.Drawing.Point(0, 25);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(1108, 24);
            this.stPanel2.TabIndex = 7;
            // 
            // BtnMdiMinimize
            // 
            this.BtnMdiMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMdiMinimize.BackColor = System.Drawing.Color.Transparent;
            this.BtnMdiMinimize.Image = ((System.Drawing.Image)(resources.GetObject("BtnMdiMinimize.Image")));
            this.BtnMdiMinimize.Location = new System.Drawing.Point(991, 2);
            this.BtnMdiMinimize.Name = "BtnMdiMinimize";
            this.BtnMdiMinimize.Size = new System.Drawing.Size(38, 22);
            this.BtnMdiMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.BtnMdiMinimize.TabIndex = 6;
            this.BtnMdiMinimize.TabStop = false;
            this.BtnMdiMinimize.Click += new System.EventHandler(this.BtnMdiMinimize_Click);
            this.BtnMdiMinimize.MouseEnter += new System.EventHandler(this.BtnMdiMinimize_MouseEnter);
            this.BtnMdiMinimize.MouseLeave += new System.EventHandler(this.BtnMdiMinimize_MouseLeave);
            // 
            // BtnMdiMinMax
            // 
            this.BtnMdiMinMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMdiMinMax.BackColor = System.Drawing.Color.Transparent;
            this.BtnMdiMinMax.Image = ((System.Drawing.Image)(resources.GetObject("BtnMdiMinMax.Image")));
            this.BtnMdiMinMax.Location = new System.Drawing.Point(1029, 2);
            this.BtnMdiMinMax.Name = "BtnMdiMinMax";
            this.BtnMdiMinMax.Size = new System.Drawing.Size(38, 22);
            this.BtnMdiMinMax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.BtnMdiMinMax.TabIndex = 5;
            this.BtnMdiMinMax.TabStop = false;
            this.BtnMdiMinMax.Click += new System.EventHandler(this.BtnMdiMinMax_Click);
            this.BtnMdiMinMax.MouseEnter += new System.EventHandler(this.BtnMdiMinMax_MouseEnter);
            this.BtnMdiMinMax.MouseLeave += new System.EventHandler(this.BtnMdiMinMax_MouseLeave);
            // 
            // BtnMdiClose
            // 
            this.BtnMdiClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMdiClose.BackColor = System.Drawing.Color.Transparent;
            this.BtnMdiClose.Image = ((System.Drawing.Image)(resources.GetObject("BtnMdiClose.Image")));
            this.BtnMdiClose.Location = new System.Drawing.Point(1067, 2);
            this.BtnMdiClose.Name = "BtnMdiClose";
            this.BtnMdiClose.Size = new System.Drawing.Size(38, 22);
            this.BtnMdiClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.BtnMdiClose.TabIndex = 4;
            this.BtnMdiClose.TabStop = false;
            this.BtnMdiClose.Click += new System.EventHandler(this.BtnMdiClose_Click);
            this.BtnMdiClose.MouseEnter += new System.EventHandler(this.BtnMdiClose_MouseEnter);
            this.BtnMdiClose.MouseLeave += new System.EventHandler(this.BtnMdiClose_MouseLeave);
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stToolStrip1.HighlightSelectedTab = false;
            this.stToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripButton,
            this.updateToolstrip});
            this.stToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(1108, 24);
            this.stToolStrip1.TabIndex = 0;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Enabled = false;
            this.saveToolStripButton.Image = global::Toolbox.Properties.Resources.Save;
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 21);
            this.saveToolStripButton.Text = "saveToolStripButton1";
            this.saveToolStripButton.ToolTipText = "Save File";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // updateToolstrip
            // 
            this.updateToolstrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.updateToolstrip.Enabled = false;
            this.updateToolstrip.Image = global::Toolbox.Properties.Resources.UpdateIcon;
            this.updateToolstrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.updateToolstrip.Name = "updateToolstrip";
            this.updateToolstrip.Size = new System.Drawing.Size(23, 21);
            this.updateToolstrip.Text = "toolStripButton1";
            this.updateToolstrip.ToolTipText = "Update Tool";
            this.updateToolstrip.Click += new System.EventHandler(this.updateToolstrip_Click);
            // 
            // openUserFolderToolStripMenuItem
            // 
            this.openUserFolderToolStripMenuItem.Name = "openUserFolderToolStripMenuItem";
            this.openUserFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openUserFolderToolStripMenuItem.Text = "Open User Folder";
            this.openUserFolderToolStripMenuItem.Click += new System.EventHandler(this.openUserFolderToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.ClientSize = new System.Drawing.Size(1108, 621);
            this.Controls.Add(this.tabForms);
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.stPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
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
            ((System.ComponentModel.ISupportInitialize)(this.BtnMdiMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMdiMinMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnMdiClose)).EndInit();
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STMenuStrip menuStrip1;
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
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STTabControl tabForms;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private Toolbox.Library.Forms.STContextMenuStrip tabControlContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileAssociationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mainSettingsToolStripMenuItem;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton updateToolstrip;
        private System.Windows.Forms.ToolStripMenuItem newFromFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox BtnMdiMinimize;
        private System.Windows.Forms.PictureBox BtnMdiMinMax;
        private System.Windows.Forms.PictureBox BtnMdiClose;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem consoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem requestFeatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reportBugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem requestFeatureToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem githubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tutorialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hashCalculatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchExportTexturesAllSupportedFormatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchExportAnimationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchExportModelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchReplaceFTPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchReplaceTXTGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchRenameBNTXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openUserFolderToolStripMenuItem;
    }
}
