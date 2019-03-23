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
            this.stLabel4 = new Switch_Toolbox.Library.Forms.STLabel();
            this.textureFrameUD = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.btnRemove = new Switch_Toolbox.Library.Forms.STButton();
            this.btnAdd = new Switch_Toolbox.Library.Forms.STButton();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stPanel7 = new Switch_Toolbox.Library.Forms.STPanel();
            this.btnEditMaterial = new Switch_Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.pictureBoxCustom1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.materialCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.samplerCB = new Switch_Toolbox.Library.Forms.STComboBox();
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
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stTextBox1 = new Switch_Toolbox.Library.Forms.STTextBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.btnEditSamplers = new Switch_Toolbox.Library.Forms.STButton();
            this.stPanel3.SuspendLayout();
            this.stPanel5.SuspendLayout();
            this.stPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureFrameUD)).BeginInit();
            this.stPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxFrameCounterUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameCounterUD)).BeginInit();
            this.stPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 837);
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
            this.stPanel3.Location = new System.Drawing.Point(3, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(729, 837);
            this.stPanel3.TabIndex = 8;
            // 
            // stPanel5
            // 
            this.stPanel5.Controls.Add(this.splitter3);
            this.stPanel5.Controls.Add(this.stPanel6);
            this.stPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel5.Location = new System.Drawing.Point(0, 0);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(729, 834);
            this.stPanel5.TabIndex = 9;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(0, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(729, 3);
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
            this.stPanel6.Size = new System.Drawing.Size(729, 834);
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
            this.splitContainer1.Size = new System.Drawing.Size(726, 834);
            this.splitContainer1.SplitterDistance = 242;
            this.splitContainer1.TabIndex = 17;
            // 
            // stPanel8
            // 
            this.stPanel8.Controls.Add(this.stLabel4);
            this.stPanel8.Controls.Add(this.textureFrameUD);
            this.stPanel8.Controls.Add(this.btnRemove);
            this.stPanel8.Controls.Add(this.btnAdd);
            this.stPanel8.Controls.Add(this.listViewCustom1);
            this.stPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel8.Location = new System.Drawing.Point(0, 0);
            this.stPanel8.Name = "stPanel8";
            this.stPanel8.Size = new System.Drawing.Size(242, 834);
            this.stPanel8.TabIndex = 15;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(6, 34);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(39, 13);
            this.stLabel4.TabIndex = 15;
            this.stLabel4.Text = "Frame:";
            // 
            // textureFrameUD
            // 
            this.textureFrameUD.Location = new System.Drawing.Point(59, 32);
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
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(77, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            this.listViewCustom1.Location = new System.Drawing.Point(3, 58);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(230, 779);
            this.listViewCustom1.TabIndex = 7;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            this.listViewCustom1.DoubleClick += new System.EventHandler(this.listViewCustom1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 230;
            // 
            // stPanel7
            // 
            this.stPanel7.Controls.Add(this.btnEditSamplers);
            this.stPanel7.Controls.Add(this.btnEditMaterial);
            this.stPanel7.Controls.Add(this.stLabel2);
            this.stPanel7.Controls.Add(this.pictureBoxCustom1);
            this.stPanel7.Controls.Add(this.materialCB);
            this.stPanel7.Controls.Add(this.samplerCB);
            this.stPanel7.Controls.Add(this.stPanel1);
            this.stPanel7.Controls.Add(this.stLabel3);
            this.stPanel7.Controls.Add(this.stLabel1);
            this.stPanel7.Controls.Add(this.stTextBox1);
            this.stPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel7.Location = new System.Drawing.Point(0, 0);
            this.stPanel7.Name = "stPanel7";
            this.stPanel7.Size = new System.Drawing.Size(480, 834);
            this.stPanel7.TabIndex = 14;
            // 
            // btnEditMaterial
            // 
            this.btnEditMaterial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditMaterial.Location = new System.Drawing.Point(299, 12);
            this.btnEditMaterial.Name = "btnEditMaterial";
            this.btnEditMaterial.Size = new System.Drawing.Size(47, 23);
            this.btnEditMaterial.TabIndex = 14;
            this.btnEditMaterial.Text = "Edit";
            this.btnEditMaterial.UseVisualStyleBackColor = false;
            this.btnEditMaterial.Click += new System.EventHandler(this.btnEditMaterial_Click);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(3, 14);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(81, 13);
            this.stLabel2.TabIndex = 11;
            this.stLabel2.Text = "Material Target:";
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(6, 102);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(467, 506);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // materialCB
            // 
            this.materialCB.BorderColor = System.Drawing.Color.Empty;
            this.materialCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.materialCB.ButtonColor = System.Drawing.Color.Empty;
            this.materialCB.FormattingEnabled = true;
            this.materialCB.Location = new System.Drawing.Point(90, 14);
            this.materialCB.Name = "materialCB";
            this.materialCB.ReadOnly = true;
            this.materialCB.Size = new System.Drawing.Size(203, 21);
            this.materialCB.TabIndex = 10;
            this.materialCB.SelectedIndexChanged += new System.EventHandler(this.materialCB_SelectedIndexChanged);
            // 
            // samplerCB
            // 
            this.samplerCB.BorderColor = System.Drawing.Color.Empty;
            this.samplerCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.samplerCB.ButtonColor = System.Drawing.Color.Empty;
            this.samplerCB.FormattingEnabled = true;
            this.samplerCB.Location = new System.Drawing.Point(90, 43);
            this.samplerCB.Name = "samplerCB";
            this.samplerCB.ReadOnly = true;
            this.samplerCB.Size = new System.Drawing.Size(203, 21);
            this.samplerCB.TabIndex = 5;
            this.samplerCB.SelectedIndexChanged += new System.EventHandler(this.samplerCB_SelectedIndexChanged);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.loopChkBox);
            this.stPanel1.Controls.Add(this.animationTrackBar);
            this.stPanel1.Controls.Add(this.maxFrameCounterUD);
            this.stPanel1.Controls.Add(this.currentFrameCounterUD);
            this.stPanel1.Controls.Add(this.stPanel4);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel1.Location = new System.Drawing.Point(0, 614);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(480, 220);
            this.stPanel1.TabIndex = 1;
            // 
            // loopChkBox
            // 
            this.loopChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loopChkBox.AutoSize = true;
            this.loopChkBox.Checked = true;
            this.loopChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopChkBox.Location = new System.Drawing.Point(367, 10);
            this.loopChkBox.Name = "loopChkBox";
            this.loopChkBox.Size = new System.Drawing.Size(50, 17);
            this.loopChkBox.TabIndex = 17;
            this.loopChkBox.Text = "Loop";
            this.loopChkBox.UseVisualStyleBackColor = true;
            // 
            // animationTrackBar
            // 
            this.animationTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrackBar.BackColor = System.Drawing.Color.Transparent;
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
            this.animationTrackBar.Location = new System.Drawing.Point(16, 65);
            this.animationTrackBar.Maximum = 1000;
            this.animationTrackBar.MouseEffects = false;
            this.animationTrackBar.Name = "animationTrackBar";
            this.animationTrackBar.ScaleDivisions = 10;
            this.animationTrackBar.ScaleSubDivisions = 5;
            this.animationTrackBar.ShowDivisionsText = true;
            this.animationTrackBar.ShowSmallScale = false;
            this.animationTrackBar.Size = new System.Drawing.Size(459, 37);
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
            this.maxFrameCounterUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxFrameCounterUD.Location = new System.Drawing.Point(364, 33);
            this.maxFrameCounterUD.Name = "maxFrameCounterUD";
            this.maxFrameCounterUD.Size = new System.Drawing.Size(109, 20);
            this.maxFrameCounterUD.TabIndex = 15;
            this.maxFrameCounterUD.ValueChanged += new System.EventHandler(this.maxFrameCounterUD_ValueChanged);
            // 
            // currentFrameCounterUD
            // 
            this.currentFrameCounterUD.Location = new System.Drawing.Point(16, 33);
            this.currentFrameCounterUD.Name = "currentFrameCounterUD";
            this.currentFrameCounterUD.Size = new System.Drawing.Size(109, 20);
            this.currentFrameCounterUD.TabIndex = 14;
            this.currentFrameCounterUD.ValueChanged += new System.EventHandler(this.currentFrameCounterUD_ValueChanged);
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.stPanel4.Controls.Add(this.btnStop);
            this.stPanel4.Controls.Add(this.btnForward1);
            this.stPanel4.Controls.Add(this.btnPlay);
            this.stPanel4.Controls.Add(this.btnBackward1);
            this.stPanel4.Location = new System.Drawing.Point(135, 16);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(225, 45);
            this.stPanel4.TabIndex = 13;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStop.BackgroundImage")));
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(160, 7);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(35, 35);
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
            this.btnForward1.Location = new System.Drawing.Point(117, 15);
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
            this.btnPlay.Location = new System.Drawing.Point(65, 8);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(35, 35);
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
            this.btnBackward1.Location = new System.Drawing.Point(26, 15);
            this.btnBackward1.Name = "btnBackward1";
            this.btnBackward1.Size = new System.Drawing.Size(20, 20);
            this.btnBackward1.TabIndex = 1;
            this.btnBackward1.UseVisualStyleBackColor = false;
            this.btnBackward1.Click += new System.EventHandler(this.btnBackward1_Click);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(3, 72);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(70, 13);
            this.stLabel3.TabIndex = 13;
            this.stLabel3.Text = "Sampler Hint:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(6, 43);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(82, 13);
            this.stLabel1.TabIndex = 6;
            this.stLabel1.Text = "Sampler Target:";
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(90, 70);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.ReadOnly = true;
            this.stTextBox1.Size = new System.Drawing.Size(203, 20);
            this.stTextBox1.TabIndex = 12;
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter4.Location = new System.Drawing.Point(726, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 834);
            this.splitter4.TabIndex = 16;
            this.splitter4.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 834);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(729, 3);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // btnEditSamplers
            // 
            this.btnEditSamplers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditSamplers.Location = new System.Drawing.Point(299, 41);
            this.btnEditSamplers.Name = "btnEditSamplers";
            this.btnEditSamplers.Size = new System.Drawing.Size(47, 23);
            this.btnEditSamplers.TabIndex = 15;
            this.btnEditSamplers.Text = "Edit";
            this.btnEditSamplers.UseVisualStyleBackColor = false;
            this.btnEditSamplers.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // BfresTexturePatternEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel3);
            this.Controls.Add(this.splitter1);
            this.Name = "BfresTexturePatternEditor";
            this.Size = new System.Drawing.Size(732, 837);
            this.stPanel3.ResumeLayout(false);
            this.stPanel5.ResumeLayout(false);
            this.stPanel6.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel8.ResumeLayout(false);
            this.stPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureFrameUD)).EndInit();
            this.stPanel7.ResumeLayout(false);
            this.stPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxFrameCounterUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameCounterUD)).EndInit();
            this.stPanel4.ResumeLayout(false);
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
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STComboBox samplerCB;
        private Switch_Toolbox.Library.Forms.STButton btnAdd;
        private Switch_Toolbox.Library.Forms.STButton btnRemove;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private Switch_Toolbox.Library.Forms.STTextBox stTextBox1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STComboBox materialCB;
        private System.Windows.Forms.Splitter splitter3;
        private Switch_Toolbox.Library.Forms.STPanel stPanel6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Splitter splitter4;
        private Switch_Toolbox.Library.Forms.STPanel stPanel8;
        private Switch_Toolbox.Library.Forms.STPanel stPanel7;
        private Switch_Toolbox.Library.Forms.STButton btnEditMaterial;
        private Switch_Toolbox.Library.Forms.STCheckBox loopChkBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Switch_Toolbox.Library.Forms.NumericUpDownUint textureFrameUD;
        private Switch_Toolbox.Library.Forms.STLabel stLabel4;
        private Switch_Toolbox.Library.Forms.STButton btnEditSamplers;
    }
}
