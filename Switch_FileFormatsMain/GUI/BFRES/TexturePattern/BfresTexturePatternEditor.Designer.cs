namespace FirstPlugin.Forms
{
    partial class BfresTexturePatternEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BfresTexturePatternEditor));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPanel5 = new Switch_Toolbox.Library.Forms.STPanel();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.stPanel6 = new Switch_Toolbox.Library.Forms.STPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.stPanel8 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stToolStrip2 = new Switch_Toolbox.Library.Forms.STToolStrip();
            this.addKeyFrameToolstrip = new System.Windows.Forms.ToolStripButton();
            this.removeKeyFrameToolstrip = new System.Windows.Forms.ToolStripButton();
            this.toolstripShiftUp = new System.Windows.Forms.ToolStripButton();
            this.toolstripShiftDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Switch_Toolbox.Library.Forms.STLabel();
            this.textureFrameUD = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stPanel7 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stLabel5 = new Switch_Toolbox.Library.Forms.STLabel();
            this.activeAnimCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.frameCountUD = new Switch_Toolbox.Library.Forms.STNumbericUpDown();
            this.backgroundCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stToolStrip1 = new Switch_Toolbox.Library.Forms.STToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.pictureBoxCustom1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.loopChkBox = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.animationTrackBar = new ColorSlider.ColorSlider();
            this.maxFrameCounterUD = new Switch_Toolbox.Library.Forms.STNumbericUpDown();
            this.currentFrameCounterUD = new Switch_Toolbox.Library.Forms.STNumbericUpDown();
            this.stPanel4 = new Switch_Toolbox.Library.Forms.STPanel();
            this.btnStop = new Switch_Toolbox.Library.Forms.STButton();
            this.btnForward1 = new Switch_Toolbox.Library.Forms.STButton();
            this.btnPlay = new Switch_Toolbox.Library.Forms.STButton();
            this.btnBackward1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adjustmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.contentContainer.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.stPanel5.SuspendLayout();
            this.stPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel8.SuspendLayout();
            this.stToolStrip2.SuspendLayout();
            this.stPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureFrameUD)).BeginInit();
            this.stPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frameCountUD)).BeginInit();
            this.stToolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxFrameCounterUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameCounterUD)).BeginInit();
            this.stPanel4.SuspendLayout();
            this.stMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stPanel3);
            this.contentContainer.Controls.Add(this.splitter1);
            this.contentContainer.Size = new System.Drawing.Size(1000, 674);
            this.contentContainer.Controls.SetChildIndex(this.splitter1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stPanel3, 0);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 649);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.stPanel5);
            this.stPanel3.Controls.Add(this.splitter2);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(3, 25);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(997, 649);
            this.stPanel3.TabIndex = 8;
            // 
            // stPanel5
            // 
            this.stPanel5.Controls.Add(this.splitter3);
            this.stPanel5.Controls.Add(this.stPanel6);
            this.stPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel5.Location = new System.Drawing.Point(0, 0);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(997, 646);
            this.stPanel5.TabIndex = 9;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(0, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(997, 3);
            this.splitter3.TabIndex = 17;
            this.splitter3.TabStop = false;
            // 
            // stPanel6
            // 
            this.stPanel6.Controls.Add(this.splitContainer1);
            this.stPanel6.Controls.Add(this.splitter4);
            this.stPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel6.Location = new System.Drawing.Point(0, 0);
            this.stPanel6.Name = "stPanel6";
            this.stPanel6.Size = new System.Drawing.Size(997, 646);
            this.stPanel6.TabIndex = 16;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.stPanel8);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.stPanel7);
            this.splitContainer1.Size = new System.Drawing.Size(994, 646);
            this.splitContainer1.SplitterDistance = 201;
            this.splitContainer1.TabIndex = 17;
            // 
            // stPanel8
            // 
            this.stPanel8.Controls.Add(this.stToolStrip2);
            this.stPanel8.Controls.Add(this.stPanel2);
            this.stPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel8.Location = new System.Drawing.Point(0, 0);
            this.stPanel8.Name = "stPanel8";
            this.stPanel8.Size = new System.Drawing.Size(201, 646);
            this.stPanel8.TabIndex = 15;
            // 
            // stToolStrip2
            // 
            this.stToolStrip2.Dock = System.Windows.Forms.DockStyle.Left;
            this.stToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addKeyFrameToolstrip,
            this.removeKeyFrameToolstrip,
            this.toolstripShiftUp,
            this.toolstripShiftDown,
            this.toolStripButton2});
            this.stToolStrip2.Location = new System.Drawing.Point(0, 0);
            this.stToolStrip2.Name = "stToolStrip2";
            this.stToolStrip2.Size = new System.Drawing.Size(24, 646);
            this.stToolStrip2.TabIndex = 19;
            this.stToolStrip2.Text = "stToolStrip2";
            // 
            // addKeyFrameToolstrip
            // 
            this.addKeyFrameToolstrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addKeyFrameToolstrip.Image = global::FirstPlugin.Properties.Resources.AddIcon;
            this.addKeyFrameToolstrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addKeyFrameToolstrip.Name = "addKeyFrameToolstrip";
            this.addKeyFrameToolstrip.Size = new System.Drawing.Size(29, 20);
            this.addKeyFrameToolstrip.Text = "Add Frame";
            this.addKeyFrameToolstrip.Click += new System.EventHandler(this.addKeyFrameToolstrip_Click);
            // 
            // removeKeyFrameToolstrip
            // 
            this.removeKeyFrameToolstrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeKeyFrameToolstrip.Image = global::FirstPlugin.Properties.Resources.RemoveIcon;
            this.removeKeyFrameToolstrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeKeyFrameToolstrip.Name = "removeKeyFrameToolstrip";
            this.removeKeyFrameToolstrip.Size = new System.Drawing.Size(29, 20);
            this.removeKeyFrameToolstrip.Text = "Remove Frame";
            this.removeKeyFrameToolstrip.Click += new System.EventHandler(this.removeKeyFrameToolstrip_Click);
            // 
            // toolstripShiftUp
            // 
            this.toolstripShiftUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstripShiftUp.Image = global::FirstPlugin.Properties.Resources.ArrowIcon;
            this.toolstripShiftUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripShiftUp.Name = "toolstripShiftUp";
            this.toolstripShiftUp.Size = new System.Drawing.Size(29, 20);
            this.toolstripShiftUp.Text = "Move Up";
            this.toolstripShiftUp.Click += new System.EventHandler(this.toolstripShiftUp_Click);
            // 
            // toolstripShiftDown
            // 
            this.toolstripShiftDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstripShiftDown.Image = global::FirstPlugin.Properties.Resources.ArrowIcon;
            this.toolstripShiftDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripShiftDown.Name = "toolstripShiftDown";
            this.toolstripShiftDown.Size = new System.Drawing.Size(29, 20);
            this.toolstripShiftDown.Text = "Move Down";
            this.toolstripShiftDown.Click += new System.EventHandler(this.toolstripShiftDown_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(21, 20);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // stPanel2
            // 
            this.stPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel2.Controls.Add(this.stLabel2);
            this.stPanel2.Controls.Add(this.stLabel4);
            this.stPanel2.Controls.Add(this.textureFrameUD);
            this.stPanel2.Controls.Add(this.stLabel1);
            this.stPanel2.Controls.Add(this.treeView1);
            this.stPanel2.Controls.Add(this.listViewCustom1);
            this.stPanel2.Location = new System.Drawing.Point(21, 6);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(178, 634);
            this.stPanel2.TabIndex = 18;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(6, 5);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(48, 13);
            this.stLabel2.TabIndex = 18;
            this.stLabel2.Text = "Textures";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(6, 447);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(39, 13);
            this.stLabel4.TabIndex = 15;
            this.stLabel4.Text = "Frame:";
            // 
            // textureFrameUD
            // 
            this.textureFrameUD.Location = new System.Drawing.Point(77, 445);
            this.textureFrameUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.textureFrameUD.Name = "textureFrameUD";
            this.textureFrameUD.Size = new System.Drawing.Size(93, 20);
            this.textureFrameUD.TabIndex = 10;
            this.textureFrameUD.ValueChanged += new System.EventHandler(this.textureFrameUD_ValueChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(6, 478);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(56, 13);
            this.stLabel1.TabIndex = 17;
            this.stLabel1.Text = "Data View";
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(6, 494);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(164, 140);
            this.treeView1.TabIndex = 16;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.FullRowSelect = true;
            this.listViewCustom1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom1.Location = new System.Drawing.Point(9, 21);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(172, 418);
            this.listViewCustom1.TabIndex = 7;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            this.listViewCustom1.DoubleClick += new System.EventHandler(this.listViewCustom1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 339;
            // 
            // stPanel7
            // 
            this.stPanel7.Controls.Add(this.stLabel5);
            this.stPanel7.Controls.Add(this.activeAnimCB);
            this.stPanel7.Controls.Add(this.stLabel3);
            this.stPanel7.Controls.Add(this.frameCountUD);
            this.stPanel7.Controls.Add(this.backgroundCB);
            this.stPanel7.Controls.Add(this.stToolStrip1);
            this.stPanel7.Controls.Add(this.pictureBoxCustom1);
            this.stPanel7.Controls.Add(this.stPanel1);
            this.stPanel7.Controls.Add(this.stMenuStrip1);
            this.stPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel7.Location = new System.Drawing.Point(0, 0);
            this.stPanel7.Name = "stPanel7";
            this.stPanel7.Size = new System.Drawing.Size(789, 646);
            this.stPanel7.TabIndex = 14;
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(141, 34);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(89, 13);
            this.stLabel5.TabIndex = 8;
            this.stLabel5.Text = "Active Animation:";
            // 
            // activeAnimCB
            // 
            this.activeAnimCB.BorderColor = System.Drawing.Color.Empty;
            this.activeAnimCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.activeAnimCB.ButtonColor = System.Drawing.Color.Empty;
            this.activeAnimCB.FormattingEnabled = true;
            this.activeAnimCB.Location = new System.Drawing.Point(233, 31);
            this.activeAnimCB.Name = "activeAnimCB";
            this.activeAnimCB.ReadOnly = true;
            this.activeAnimCB.Size = new System.Drawing.Size(220, 21);
            this.activeAnimCB.TabIndex = 7;
            this.activeAnimCB.SelectedIndexChanged += new System.EventHandler(this.activeAnimCB_SelectedIndexChanged);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(383, 6);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(70, 13);
            this.stLabel3.TabIndex = 6;
            this.stLabel3.Text = "Frame Count:";
            // 
            // frameCountUD
            // 
            this.frameCountUD.Location = new System.Drawing.Point(459, 4);
            this.frameCountUD.Name = "frameCountUD";
            this.frameCountUD.Size = new System.Drawing.Size(169, 20);
            this.frameCountUD.TabIndex = 5;
            // 
            // backgroundCB
            // 
            this.backgroundCB.BorderColor = System.Drawing.Color.Empty;
            this.backgroundCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.backgroundCB.ButtonColor = System.Drawing.Color.Empty;
            this.backgroundCB.FormattingEnabled = true;
            this.backgroundCB.Location = new System.Drawing.Point(233, 3);
            this.backgroundCB.Name = "backgroundCB";
            this.backgroundCB.ReadOnly = true;
            this.backgroundCB.Size = new System.Drawing.Size(144, 21);
            this.backgroundCB.TabIndex = 4;
            this.backgroundCB.SelectedIndexChanged += new System.EventHandler(this.backgroundCB_SelectedIndexChanged);
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton4});
            this.stToolStrip1.Location = new System.Drawing.Point(0, 24);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(789, 25);
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
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(16, 58);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(776, 514);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.loopChkBox);
            this.stPanel1.Controls.Add(this.animationTrackBar);
            this.stPanel1.Controls.Add(this.maxFrameCounterUD);
            this.stPanel1.Controls.Add(this.currentFrameCounterUD);
            this.stPanel1.Controls.Add(this.stPanel4);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel1.Location = new System.Drawing.Point(0, 578);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(789, 68);
            this.stPanel1.TabIndex = 1;
            // 
            // loopChkBox
            // 
            this.loopChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loopChkBox.AutoSize = true;
            this.loopChkBox.Checked = true;
            this.loopChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopChkBox.Location = new System.Drawing.Point(662, 5);
            this.loopChkBox.Name = "loopChkBox";
            this.loopChkBox.Size = new System.Drawing.Size(50, 17);
            this.loopChkBox.TabIndex = 17;
            this.loopChkBox.Text = "Loop";
            this.loopChkBox.UseVisualStyleBackColor = true;
            // 
            // animationTrackBar
            // 
            this.animationTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrackBar.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationTrackBar.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationTrackBar.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationTrackBar.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.animationTrackBar.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.animationTrackBar.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.animationTrackBar.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.animationTrackBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.animationTrackBar.ForeColor = System.Drawing.Color.White;
            this.animationTrackBar.LargeChange = ((uint)(5u));
            this.animationTrackBar.Location = new System.Drawing.Point(6, 46);
            this.animationTrackBar.Maximum = 1000;
            this.animationTrackBar.MouseEffects = false;
            this.animationTrackBar.Name = "animationTrackBar";
            this.animationTrackBar.ScaleDivisions = 10;
            this.animationTrackBar.ScaleSubDivisions = 5;
            this.animationTrackBar.ShowDivisionsText = true;
            this.animationTrackBar.ShowSmallScale = false;
            this.animationTrackBar.Size = new System.Drawing.Size(765, 19);
            this.animationTrackBar.SmallChange = ((uint)(1u));
            this.animationTrackBar.TabIndex = 16;
            this.animationTrackBar.Text = "colorSlider1";
            this.animationTrackBar.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.animationTrackBar.ThumbPenColor = System.Drawing.Color.Silver;
            this.animationTrackBar.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.animationTrackBar.ThumbSize = new System.Drawing.Size(8, 8);
            this.animationTrackBar.TickAdd = 0F;
            this.animationTrackBar.TickColor = System.Drawing.Color.White;
            this.animationTrackBar.TickDivide = 0F;
            this.animationTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.animationTrackBar.ValueChanged += new System.EventHandler(this.animationTrackBar_ValueChanged);
            this.animationTrackBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.animationTrackBar_Scroll);
            // 
            // maxFrameCounterUD
            // 
            this.maxFrameCounterUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.maxFrameCounterUD.Location = new System.Drawing.Point(662, 25);
            this.maxFrameCounterUD.Name = "maxFrameCounterUD";
            this.maxFrameCounterUD.Size = new System.Drawing.Size(109, 20);
            this.maxFrameCounterUD.TabIndex = 15;
            this.maxFrameCounterUD.ValueChanged += new System.EventHandler(this.maxFrameCounterUD_ValueChanged);
            // 
            // currentFrameCounterUD
            // 
            this.currentFrameCounterUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.currentFrameCounterUD.Location = new System.Drawing.Point(8, 25);
            this.currentFrameCounterUD.Name = "currentFrameCounterUD";
            this.currentFrameCounterUD.Size = new System.Drawing.Size(109, 20);
            this.currentFrameCounterUD.TabIndex = 14;
            this.currentFrameCounterUD.ValueChanged += new System.EventHandler(this.currentFrameCounterUD_ValueChanged);
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.stPanel4.Controls.Add(this.btnStop);
            this.stPanel4.Controls.Add(this.btnForward1);
            this.stPanel4.Controls.Add(this.btnPlay);
            this.stPanel4.Controls.Add(this.btnBackward1);
            this.stPanel4.Location = new System.Drawing.Point(123, 7);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(534, 41);
            this.stPanel4.TabIndex = 13;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStop.BackgroundImage")));
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(314, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(35, 27);
            this.btnStop.TabIndex = 3;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnForward1
            // 
            this.btnForward1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnForward1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnForward1.BackgroundImage")));
            this.btnForward1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnForward1.FlatAppearance.BorderSize = 0;
            this.btnForward1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForward1.Location = new System.Drawing.Point(271, 10);
            this.btnForward1.Name = "btnForward1";
            this.btnForward1.Size = new System.Drawing.Size(23, 20);
            this.btnForward1.TabIndex = 2;
            this.btnForward1.UseVisualStyleBackColor = false;
            this.btnForward1.Click += new System.EventHandler(this.btnForward1_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnPlay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPlay.BackgroundImage")));
            this.btnPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Location = new System.Drawing.Point(219, 6);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(35, 28);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnBackward1
            // 
            this.btnBackward1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBackward1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBackward1.BackgroundImage")));
            this.btnBackward1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBackward1.FlatAppearance.BorderSize = 0;
            this.btnBackward1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackward1.Location = new System.Drawing.Point(180, 10);
            this.btnBackward1.Name = "btnBackward1";
            this.btnBackward1.Size = new System.Drawing.Size(20, 20);
            this.btnBackward1.TabIndex = 1;
            this.btnBackward1.UseVisualStyleBackColor = false;
            this.btnBackward1.Click += new System.EventHandler(this.btnBackward1_Click);
            // 
            // stMenuStrip1
            // 
            this.stMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.adjustmentsToolStripMenuItem});
            this.stMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stMenuStrip1.Name = "stMenuStrip1";
            this.stMenuStrip1.Size = new System.Drawing.Size(789, 24);
            this.stMenuStrip1.TabIndex = 2;
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
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.imageToolStripMenuItem.Text = "Image";
            // 
            // adjustmentsToolStripMenuItem
            // 
            this.adjustmentsToolStripMenuItem.Name = "adjustmentsToolStripMenuItem";
            this.adjustmentsToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.adjustmentsToolStripMenuItem.Text = "Adjustments";
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter4.Location = new System.Drawing.Point(994, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 646);
            this.splitter4.TabIndex = 16;
            this.splitter4.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 646);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(997, 3);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // BfresTexturePatternEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 679);
            this.MainMenuStrip = this.stMenuStrip1;
            this.Name = "BfresTexturePatternEditor";
            this.contentContainer.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.stPanel5.ResumeLayout(false);
            this.stPanel6.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel8.ResumeLayout(false);
            this.stPanel8.PerformLayout();
            this.stToolStrip2.ResumeLayout(false);
            this.stToolStrip2.PerformLayout();
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureFrameUD)).EndInit();
            this.stPanel7.ResumeLayout(false);
            this.stPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frameCountUD)).EndInit();
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxFrameCounterUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameCounterUD)).EndInit();
            this.stPanel4.ResumeLayout(false);
            this.stMenuStrip1.ResumeLayout(false);
            this.stMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel3;
        private System.Windows.Forms.Splitter splitter2;
        private Switch_Toolbox.Library.Forms.STPanel stPanel5;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
        private ColorSlider.ColorSlider animationTrackBar;
        private Switch_Toolbox.Library.Forms.STNumbericUpDown maxFrameCounterUD;
        private Switch_Toolbox.Library.Forms.STNumbericUpDown currentFrameCounterUD;
        private Switch_Toolbox.Library.Forms.STPanel stPanel4;
        private Switch_Toolbox.Library.Forms.STButton btnStop;
        private Switch_Toolbox.Library.Forms.STButton btnForward1;
        private Switch_Toolbox.Library.Forms.STButton btnPlay;
        private Switch_Toolbox.Library.Forms.STButton btnBackward1;
        private Switch_Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Splitter splitter3;
        private Switch_Toolbox.Library.Forms.STPanel stPanel6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Splitter splitter4;
        private Switch_Toolbox.Library.Forms.STPanel stPanel8;
        private Switch_Toolbox.Library.Forms.STPanel stPanel7;
        private Switch_Toolbox.Library.Forms.STCheckBox loopChkBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Switch_Toolbox.Library.Forms.NumericUpDownUint textureFrameUD;
        private Switch_Toolbox.Library.Forms.STLabel stLabel4;
        private Switch_Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private Switch_Toolbox.Library.Forms.STMenuStrip stMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel2;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STToolStrip stToolStrip2;
        private System.Windows.Forms.ToolStripButton addKeyFrameToolstrip;
        private System.Windows.Forms.ToolStripButton removeKeyFrameToolstrip;
        private System.Windows.Forms.ToolStripButton toolstripShiftUp;
        private System.Windows.Forms.ToolStripButton toolstripShiftDown;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private Switch_Toolbox.Library.Forms.STNumbericUpDown frameCountUD;
        private Switch_Toolbox.Library.Forms.STComboBox backgroundCB;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adjustmentsToolStripMenuItem;
        private Switch_Toolbox.Library.Forms.STLabel stLabel5;
        private Switch_Toolbox.Library.Forms.STComboBox activeAnimCB;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}
