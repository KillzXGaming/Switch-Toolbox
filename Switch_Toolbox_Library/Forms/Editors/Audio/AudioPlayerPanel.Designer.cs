namespace Switch_Toolbox.Library.Forms
{
    partial class AudioPlayerPanel
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
            this.audioBarPanel = new Switch_Toolbox.Library.Forms.STPanel();
            this.trackbarVolume = new ColorSlider.ColorSlider();
            this.stPanel4 = new Switch_Toolbox.Library.Forms.STPanel();
            this.btnStop = new Switch_Toolbox.Library.Forms.STButton();
            this.btnForward1 = new Switch_Toolbox.Library.Forms.STButton();
            this.btnPlay = new Switch_Toolbox.Library.Forms.STButton();
            this.btnBackward1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.colorSlider1 = new ColorSlider.ColorSlider();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            this.audioListView = new Switch_Toolbox.Library.Forms.STListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.audioDevice = new Switch_Toolbox.Library.Forms.STComboBox();
            this.channelCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stContextMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loopingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chkLoopPlayer = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.audioBarPanel.SuspendLayout();
            this.stPanel4.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.audioListView)).BeginInit();
            this.stPanel2.SuspendLayout();
            this.stContextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.audioBarPanel);
            this.Size = new System.Drawing.Size(531, 441);
            this.Controls.SetChildIndex(this.audioBarPanel, 0);
            this.Controls.SetChildIndex(this.stPanel1, 0);
            // 
            // audioBarPanel
            // 
            this.audioBarPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.audioBarPanel.Controls.Add(this.chkLoopPlayer);
            this.audioBarPanel.Controls.Add(this.trackbarVolume);
            this.audioBarPanel.Controls.Add(this.stPanel4);
            this.audioBarPanel.Controls.Add(this.stLabel2);
            this.audioBarPanel.Controls.Add(this.stLabel1);
            this.audioBarPanel.Controls.Add(this.colorSlider1);
            this.audioBarPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.audioBarPanel.Location = new System.Drawing.Point(0, 373);
            this.audioBarPanel.Name = "audioBarPanel";
            this.audioBarPanel.Size = new System.Drawing.Size(531, 68);
            this.audioBarPanel.TabIndex = 11;
            // 
            // trackbarVolume
            // 
            this.trackbarVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trackbarVolume.BackColor = System.Drawing.Color.Transparent;
            this.trackbarVolume.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackbarVolume.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackbarVolume.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackbarVolume.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.trackbarVolume.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackbarVolume.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackbarVolume.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackbarVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.trackbarVolume.ForeColor = System.Drawing.Color.White;
            this.trackbarVolume.LargeChange = ((uint)(5u));
            this.trackbarVolume.Location = new System.Drawing.Point(437, 34);
            this.trackbarVolume.Maximum = 1000;
            this.trackbarVolume.MouseEffects = false;
            this.trackbarVolume.Name = "trackbarVolume";
            this.trackbarVolume.ScaleDivisions = 10;
            this.trackbarVolume.ScaleSubDivisions = 5;
            this.trackbarVolume.ShowDivisionsText = true;
            this.trackbarVolume.ShowSmallScale = false;
            this.trackbarVolume.Size = new System.Drawing.Size(85, 37);
            this.trackbarVolume.SmallChange = ((uint)(1u));
            this.trackbarVolume.TabIndex = 12;
            this.trackbarVolume.Text = "colorSlider2";
            this.trackbarVolume.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackbarVolume.ThumbPenColor = System.Drawing.Color.Silver;
            this.trackbarVolume.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.trackbarVolume.ThumbSize = new System.Drawing.Size(8, 8);
            this.trackbarVolume.TickAdd = 0F;
            this.trackbarVolume.TickColor = System.Drawing.Color.White;
            this.trackbarVolume.TickDivide = 0F;
            this.trackbarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackbarVolume.ValueChanged += new System.EventHandler(this.trackbarVolume_ValueChanged);
            this.trackbarVolume.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trackbarVolume_Scroll);
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.stPanel4.BackColor = System.Drawing.Color.Transparent;
            this.stPanel4.Controls.Add(this.btnStop);
            this.stPanel4.Controls.Add(this.btnForward1);
            this.stPanel4.Controls.Add(this.btnPlay);
            this.stPanel4.Controls.Add(this.btnBackward1);
            this.stPanel4.Location = new System.Drawing.Point(121, 3);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(195, 45);
            this.stPanel4.TabIndex = 11;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnStop.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.StopBtn;
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(145, 7);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(35, 35);
            this.btnStop.TabIndex = 3;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnForward1
            // 
            this.btnForward1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnForward1.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.RewindArrows1R;
            this.btnForward1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnForward1.FlatAppearance.BorderSize = 0;
            this.btnForward1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForward1.Location = new System.Drawing.Point(102, 15);
            this.btnForward1.Name = "btnForward1";
            this.btnForward1.Size = new System.Drawing.Size(23, 20);
            this.btnForward1.TabIndex = 2;
            this.btnForward1.UseVisualStyleBackColor = false;
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnPlay.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.PlayArrowR;
            this.btnPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Location = new System.Drawing.Point(50, 8);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(35, 35);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnBackward1
            // 
            this.btnBackward1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBackward1.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.RewindArrows1L;
            this.btnBackward1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBackward1.FlatAppearance.BorderSize = 0;
            this.btnBackward1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackward1.Location = new System.Drawing.Point(11, 15);
            this.btnBackward1.Name = "btnBackward1";
            this.btnBackward1.Size = new System.Drawing.Size(20, 20);
            this.btnBackward1.TabIndex = 1;
            this.btnBackward1.UseVisualStyleBackColor = false;
            // 
            // stLabel2
            // 
            this.stLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(397, 46);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(34, 13);
            this.stLabel2.TabIndex = 10;
            this.stLabel2.Text = "0 : 00";
            // 
            // stLabel1
            // 
            this.stLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(5, 46);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(34, 13);
            this.stLabel1.TabIndex = 9;
            this.stLabel1.Text = "0 : 00";
            // 
            // colorSlider1
            // 
            this.colorSlider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorSlider1.BackColor = System.Drawing.Color.Transparent;
            this.colorSlider1.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorSlider1.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorSlider1.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colorSlider1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.colorSlider1.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.colorSlider1.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.colorSlider1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.colorSlider1.ForeColor = System.Drawing.Color.White;
            this.colorSlider1.LargeChange = ((uint)(5u));
            this.colorSlider1.Location = new System.Drawing.Point(47, 34);
            this.colorSlider1.Maximum = 1000;
            this.colorSlider1.MouseEffects = false;
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.ScaleDivisions = 10;
            this.colorSlider1.ScaleSubDivisions = 5;
            this.colorSlider1.ShowDivisionsText = true;
            this.colorSlider1.ShowSmallScale = false;
            this.colorSlider1.Size = new System.Drawing.Size(333, 37);
            this.colorSlider1.SmallChange = ((uint)(1u));
            this.colorSlider1.TabIndex = 8;
            this.colorSlider1.Text = "colorSlider1";
            this.colorSlider1.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.colorSlider1.ThumbPenColor = System.Drawing.Color.Silver;
            this.colorSlider1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.ThumbSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.TickAdd = 0F;
            this.colorSlider1.TickColor = System.Drawing.Color.White;
            this.colorSlider1.TickDivide = 0F;
            this.colorSlider1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.colorSlider1.ValueChanged += new System.EventHandler(this.colorSlider1_ValueChanged);
            this.colorSlider1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.colorSlider1_MouseDown);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stPanel3);
            this.stPanel1.Controls.Add(this.splitter1);
            this.stPanel1.Controls.Add(this.stPanel2);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 25);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(531, 348);
            this.stPanel1.TabIndex = 12;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.audioListView);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(0, 117);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(531, 231);
            this.stPanel3.TabIndex = 3;
            // 
            // audioListView
            // 
            this.audioListView.AllColumns.Add(this.olvColumn1);
            this.audioListView.AllColumns.Add(this.olvColumn2);
            this.audioListView.AllColumns.Add(this.olvColumn3);
            this.audioListView.AllColumns.Add(this.olvColumn4);
            this.audioListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.audioListView.CellEditUseWholeCell = false;
            this.audioListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4});
            this.audioListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.audioListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audioListView.Location = new System.Drawing.Point(0, 0);
            this.audioListView.Name = "audioListView";
            this.audioListView.Scrollable = false;
            this.audioListView.ShowGroups = false;
            this.audioListView.Size = new System.Drawing.Size(531, 231);
            this.audioListView.TabIndex = 0;
            this.audioListView.UseCompatibleStateImageBehavior = false;
            this.audioListView.View = System.Windows.Forms.View.Details;
            this.audioListView.SelectedIndexChanged += new System.EventHandler(this.audioListView_SelectedIndexChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Status";
            this.olvColumn1.Text = "Status";
            this.olvColumn1.Width = 71;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Title";
            this.olvColumn2.Text = "Title";
            this.olvColumn2.Width = 122;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Artist";
            this.olvColumn3.Text = "Artist";
            this.olvColumn3.Width = 132;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "Duration";
            this.olvColumn4.Text = "Duration";
            this.olvColumn4.Width = 206;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 114);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(531, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.audioDevice);
            this.stPanel2.Controls.Add(this.channelCB);
            this.stPanel2.Controls.Add(this.stContextMenuStrip1);
            this.stPanel2.Controls.Add(this.pictureBox1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(531, 114);
            this.stPanel2.TabIndex = 1;
            // 
            // audioDevice
            // 
            this.audioDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.audioDevice.FormattingEnabled = true;
            this.audioDevice.Location = new System.Drawing.Point(214, 3);
            this.audioDevice.Name = "audioDevice";
            this.audioDevice.Size = new System.Drawing.Size(151, 21);
            this.audioDevice.TabIndex = 20;
            this.audioDevice.SelectedIndexChanged += new System.EventHandler(this.audioDevice_SelectedIndexChanged);
            // 
            // channelCB
            // 
            this.channelCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.channelCB.FormattingEnabled = true;
            this.channelCB.Location = new System.Drawing.Point(371, 3);
            this.channelCB.Name = "channelCB";
            this.channelCB.Size = new System.Drawing.Size(151, 21);
            this.channelCB.TabIndex = 19;
            this.channelCB.SelectedIndexChanged += new System.EventHandler(this.channelCB_SelectedIndexChanged);
            // 
            // stContextMenuStrip1
            // 
            this.stContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.stContextMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stContextMenuStrip1.Name = "stContextMenuStrip1";
            this.stContextMenuStrip1.Size = new System.Drawing.Size(531, 24);
            this.stContextMenuStrip1.TabIndex = 18;
            this.stContextMenuStrip1.Text = "stContextMenuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loopingToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // loopingToolStripMenuItem
            // 
            this.loopingToolStripMenuItem.Name = "loopingToolStripMenuItem";
            this.loopingToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.loopingToolStripMenuItem.Text = "Loop Editor";
            this.loopingToolStripMenuItem.Click += new System.EventHandler(this.loopingToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(0, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(531, 84);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 40;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chkLoopPlayer
            // 
            this.chkLoopPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLoopPlayer.AutoSize = true;
            this.chkLoopPlayer.Checked = true;
            this.chkLoopPlayer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLoopPlayer.Location = new System.Drawing.Point(400, 18);
            this.chkLoopPlayer.Name = "chkLoopPlayer";
            this.chkLoopPlayer.Size = new System.Drawing.Size(82, 17);
            this.chkLoopPlayer.TabIndex = 13;
            this.chkLoopPlayer.Text = "Loop Player";
            this.chkLoopPlayer.UseVisualStyleBackColor = true;
            // 
            // AudioPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 446);
            this.Name = "AudioPlayer";
            this.Text = "AudioPlayer";
            this.audioBarPanel.ResumeLayout(false);
            this.audioBarPanel.PerformLayout();
            this.stPanel4.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.audioListView)).EndInit();
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.stContextMenuStrip1.ResumeLayout(false);
            this.stContextMenuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private STPanel audioBarPanel;
        private STPanel stPanel1;
        private STListView audioListView;
        private System.Windows.Forms.Splitter splitter1;
        private STPanel stPanel2;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private STPanel stPanel3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ColorSlider.ColorSlider trackbarVolume;
        private STPanel stPanel4;
        private STButton btnForward1;
        private STButton btnPlay;
        private STButton btnBackward1;
        private STLabel stLabel2;
        private STLabel stLabel1;
        private ColorSlider.ColorSlider colorSlider1;
        private STComboBox audioDevice;
        private STComboBox channelCB;
        private STMenuStrip stContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loopingToolStripMenuItem;
        private STButton btnStop;
        private Switch_Toolbox.Library.Forms.STCheckBox chkLoopPlayer;
    }
}