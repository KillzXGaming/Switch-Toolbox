namespace FirstPlugin.Forms
{
    partial class TexturePatternAnimEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TexturePatternAnimEditor));
            this.trackbarKeyEditor1 = new TrackbarKeyEditor.TrackbarKeyEditor();
            this.stListView1 = new Toolbox.Library.Forms.STListView();
            this.samplerTreeView = new Toolbox.Library.TreeViewCustom();
            this.stPropertyGrid1 = new Toolbox.Library.Forms.STPropertyGrid();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.btnStop = new Toolbox.Library.Forms.STButton();
            this.btnForward1 = new Toolbox.Library.Forms.STButton();
            this.btnPlay = new Toolbox.Library.Forms.STButton();
            this.btnBackward1 = new Toolbox.Library.Forms.STButton();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.animCurrentFrameUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.animMaxFrameUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.pictureBoxCustom1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            ((System.ComponentModel.ISupportInitialize)(this.stListView1)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.stPanel4.SuspendLayout();
            this.stPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.animCurrentFrameUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animMaxFrameUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.stPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackbarKeyEditor1
            // 
            this.trackbarKeyEditor1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.trackbarKeyEditor1.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.trackbarKeyEditor1.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.trackbarKeyEditor1.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.trackbarKeyEditor1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.trackbarKeyEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackbarKeyEditor1.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.trackbarKeyEditor1.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.trackbarKeyEditor1.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.trackbarKeyEditor1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.trackbarKeyEditor1.ForeColor = System.Drawing.Color.Silver;
            this.trackbarKeyEditor1.LargeChange = ((uint)(5u));
            this.trackbarKeyEditor1.Location = new System.Drawing.Point(0, 0);
            this.trackbarKeyEditor1.MouseEffects = false;
            this.trackbarKeyEditor1.Name = "trackbarKeyEditor1";
            this.trackbarKeyEditor1.ScaleDivisions = 10;
            this.trackbarKeyEditor1.ScaleSubDivisions = 5;
            this.trackbarKeyEditor1.ShowDivisionsText = true;
            this.trackbarKeyEditor1.ShowSmallScale = true;
            this.trackbarKeyEditor1.Size = new System.Drawing.Size(679, 252);
            this.trackbarKeyEditor1.SmallChange = ((uint)(0u));
            this.trackbarKeyEditor1.TabIndex = 0;
            this.trackbarKeyEditor1.Text = "trackbarKeyEditor1";
            this.trackbarKeyEditor1.ThumbInnerColor = System.Drawing.Color.Olive;
            this.trackbarKeyEditor1.ThumbOuterColor = System.Drawing.Color.Olive;
            this.trackbarKeyEditor1.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackbarKeyEditor1.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.trackbarKeyEditor1.ThumbSize = new System.Drawing.Size(5, 128);
            this.trackbarKeyEditor1.TickAdd = 0F;
            this.trackbarKeyEditor1.TickColor = System.Drawing.Color.Gray;
            this.trackbarKeyEditor1.TickDivide = 1F;
            this.trackbarKeyEditor1.Value = 0;
            this.trackbarKeyEditor1.ValueChanged += new System.EventHandler(this.trackbarKeyEditor1_ValueChanged);
            // 
            // stListView1
            // 
            this.stListView1.CellEditUseWholeCell = false;
            this.stListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stListView1.Location = new System.Drawing.Point(0, 0);
            this.stListView1.Name = "stListView1";
            this.stListView1.ShowGroups = false;
            this.stListView1.Size = new System.Drawing.Size(679, 485);
            this.stListView1.TabIndex = 0;
            this.stListView1.UseCompatibleStateImageBehavior = false;
            this.stListView1.View = System.Windows.Forms.View.Details;
            this.stListView1.VirtualMode = true;
            // 
            // samplerTreeView
            // 
            this.samplerTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.samplerTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.samplerTreeView.ImageIndex = 0;
            this.samplerTreeView.Location = new System.Drawing.Point(0, 0);
            this.samplerTreeView.Name = "samplerTreeView";
            this.samplerTreeView.SelectedImageIndex = 0;
            this.samplerTreeView.Size = new System.Drawing.Size(161, 230);
            this.samplerTreeView.TabIndex = 2;
            this.samplerTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCustom1_AfterSelect);
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
            this.stPropertyGrid1.Location = new System.Drawing.Point(514, 0);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.Size = new System.Drawing.Size(165, 230);
            this.stPropertyGrid1.TabIndex = 3;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stPanel4);
            this.stPanel1.Controls.Add(this.stPanel3);
            this.stPanel1.Controls.Add(this.samplerTreeView);
            this.stPanel1.Controls.Add(this.stPropertyGrid1);
            this.stPanel1.Controls.Add(this.pictureBoxCustom1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(679, 230);
            this.stPanel1.TabIndex = 4;
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.BackColor = System.Drawing.Color.Empty;
            this.stPanel4.Controls.Add(this.btnStop);
            this.stPanel4.Controls.Add(this.btnForward1);
            this.stPanel4.Controls.Add(this.btnPlay);
            this.stPanel4.Controls.Add(this.btnBackward1);
            this.stPanel4.Location = new System.Drawing.Point(243, 185);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(195, 45);
            this.stPanel4.TabIndex = 12;
            this.stPanel4.Paint += new System.Windows.Forms.PaintEventHandler(this.stPanel4_Paint);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStop.BackgroundImage")));
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(145, 7);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(35, 35);
            this.btnStop.TabIndex = 3;
            this.btnStop.UseVisualStyleBackColor = false;
            // 
            // btnForward1
            // 
            this.btnForward1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnForward1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBackward1.BackgroundImage")));
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
            this.btnPlay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPlay.BackgroundImage")));
            this.btnPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Location = new System.Drawing.Point(50, 8);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(35, 35);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.UseVisualStyleBackColor = false;
            // 
            // btnBackward1
            // 
            this.btnBackward1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBackward1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnForward1.BackgroundImage")));
            this.btnBackward1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBackward1.FlatAppearance.BorderSize = 0;
            this.btnBackward1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackward1.Location = new System.Drawing.Point(11, 15);
            this.btnBackward1.Name = "btnBackward1";
            this.btnBackward1.Size = new System.Drawing.Size(20, 20);
            this.btnBackward1.TabIndex = 1;
            this.btnBackward1.UseVisualStyleBackColor = false;
            // 
            // stPanel3
            // 
            this.stPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel3.Controls.Add(this.animCurrentFrameUD);
            this.stPanel3.Controls.Add(this.animMaxFrameUD);
            this.stPanel3.Location = new System.Drawing.Point(167, 185);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(341, 45);
            this.stPanel3.TabIndex = 4;
            // 
            // animCurrentFrameUD
            // 
            this.animCurrentFrameUD.Location = new System.Drawing.Point(5, 9);
            this.animCurrentFrameUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.animCurrentFrameUD.Name = "animCurrentFrameUD";
            this.animCurrentFrameUD.Size = new System.Drawing.Size(65, 20);
            this.animCurrentFrameUD.TabIndex = 1;
            // 
            // animMaxFrameUD
            // 
            this.animMaxFrameUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.animMaxFrameUD.Location = new System.Drawing.Point(273, 9);
            this.animMaxFrameUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.animMaxFrameUD.Name = "animMaxFrameUD";
            this.animMaxFrameUD.Size = new System.Drawing.Size(65, 20);
            this.animMaxFrameUD.TabIndex = 0;
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Empty;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(167, 3);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(341, 183);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 1;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 230);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(679, 3);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.trackbarKeyEditor1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 233);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(679, 252);
            this.stPanel2.TabIndex = 6;
            // 
            // TexturePatternAnimEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.stListView1);
            this.Name = "TexturePatternAnimEditor";
            this.Size = new System.Drawing.Size(679, 485);
            ((System.ComponentModel.ISupportInitialize)(this.stListView1)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.stPanel4.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.animCurrentFrameUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animMaxFrameUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.stPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STListView stListView1;
        private TrackbarKeyEditor.TrackbarKeyEditor trackbarKeyEditor1;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private Toolbox.Library.TreeViewCustom samplerTreeView;
        private Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private Toolbox.Library.Forms.STPanel stPanel1;
        private System.Windows.Forms.Splitter splitter1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private Toolbox.Library.Forms.NumericUpDownUint animCurrentFrameUD;
        private Toolbox.Library.Forms.NumericUpDownUint animMaxFrameUD;
        private Toolbox.Library.Forms.STPanel stPanel4;
        private Toolbox.Library.Forms.STButton btnStop;
        private Toolbox.Library.Forms.STButton btnForward1;
        private Toolbox.Library.Forms.STButton btnPlay;
        private Toolbox.Library.Forms.STButton btnBackward1;
    }
}
