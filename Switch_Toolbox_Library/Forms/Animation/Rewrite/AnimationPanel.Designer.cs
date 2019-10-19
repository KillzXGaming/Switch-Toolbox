namespace Toolbox.Library
{
    partial class STAnimationPanel
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
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.frameSpeedUD = new System.Windows.Forms.NumericUpDown();
            this.loopChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.totalFrame = new System.Windows.Forms.NumericUpDown();
            this.currentFrameUpDown = new System.Windows.Forms.NumericUpDown();
            this.animationPlayBtn = new System.Windows.Forms.Button();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.animationTrackBar = new Toolbox.Library.Forms.KeyedAnimTimeline();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frameSpeedUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.BackgroundImage = global::Toolbox.Library.Properties.Resources.arrowL;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.Location = new System.Drawing.Point(17, 7);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(34, 21);
            this.button2.TabIndex = 2;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panel1.Controls.Add(this.stLabel1);
            this.panel1.Controls.Add(this.frameSpeedUD);
            this.panel1.Controls.Add(this.loopChkBox);
            this.panel1.Controls.Add(this.totalFrame);
            this.panel1.Controls.Add(this.currentFrameUpDown);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.animationPlayBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 162);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(631, 32);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(114, 11);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(27, 13);
            this.stLabel1.TabIndex = 7;
            this.stLabel1.Text = "FPS";
            // 
            // frameSpeedUD
            // 
            this.frameSpeedUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.frameSpeedUD.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.frameSpeedUD.ForeColor = System.Drawing.Color.White;
            this.frameSpeedUD.Location = new System.Drawing.Point(158, 9);
            this.frameSpeedUD.Name = "frameSpeedUD";
            this.frameSpeedUD.Size = new System.Drawing.Size(87, 16);
            this.frameSpeedUD.TabIndex = 6;
            this.frameSpeedUD.ValueChanged += new System.EventHandler(this.frameSpeedUD_ValueChanged);
            // 
            // loopChkBox
            // 
            this.loopChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loopChkBox.AutoSize = true;
            this.loopChkBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.loopChkBox.Checked = true;
            this.loopChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopChkBox.Location = new System.Drawing.Point(452, 8);
            this.loopChkBox.Name = "loopChkBox";
            this.loopChkBox.Size = new System.Drawing.Size(50, 17);
            this.loopChkBox.TabIndex = 5;
            this.loopChkBox.Text = "Loop";
            this.loopChkBox.UseVisualStyleBackColor = true;
            // 
            // totalFrame
            // 
            this.totalFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.totalFrame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.totalFrame.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.totalFrame.ForeColor = System.Drawing.Color.White;
            this.totalFrame.Location = new System.Drawing.Point(571, 10);
            this.totalFrame.Name = "totalFrame";
            this.totalFrame.Size = new System.Drawing.Size(57, 16);
            this.totalFrame.TabIndex = 4;
            // 
            // currentFrameUpDown
            // 
            this.currentFrameUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.currentFrameUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.currentFrameUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.currentFrameUpDown.ForeColor = System.Drawing.Color.White;
            this.currentFrameUpDown.Location = new System.Drawing.Point(508, 10);
            this.currentFrameUpDown.Name = "currentFrameUpDown";
            this.currentFrameUpDown.Size = new System.Drawing.Size(57, 16);
            this.currentFrameUpDown.TabIndex = 3;
            this.currentFrameUpDown.ValueChanged += new System.EventHandler(this.currentFrameUpDown_ValueChanged);
            // 
            // animationPlayBtn
            // 
            this.animationPlayBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationPlayBtn.BackgroundImage = global::Toolbox.Library.Properties.Resources.arrowR;
            this.animationPlayBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.animationPlayBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.animationPlayBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationPlayBtn.Location = new System.Drawing.Point(57, 7);
            this.animationPlayBtn.Name = "animationPlayBtn";
            this.animationPlayBtn.Size = new System.Drawing.Size(31, 21);
            this.animationPlayBtn.TabIndex = 1;
            this.animationPlayBtn.UseVisualStyleBackColor = false;
            this.animationPlayBtn.Click += new System.EventHandler(this.animationPlayBtn_Click);
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(631, 16);
            this.stPanel1.TabIndex = 5;
            // 
            // animationTrackBar
            // 
            this.animationTrackBar.ActiveAnimation = null;
            this.animationTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrackBar.CurrentFrame = 0F;
            this.animationTrackBar.FrameCount = 1002F;
            this.animationTrackBar.Location = new System.Drawing.Point(0, 14);
            this.animationTrackBar.Name = "animationTrackBar";
            this.animationTrackBar.Size = new System.Drawing.Size(631, 149);
            this.animationTrackBar.TabIndex = 6;
            // 
            // STAnimationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.animationTrackBar);
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "STAnimationPanel";
            this.Size = new System.Drawing.Size(631, 194);
            this.Load += new System.EventHandler(this.AnimationPanel_Load);
            this.Click += new System.EventHandler(this.AnimationPanel_Click);
            this.Enter += new System.EventHandler(this.AnimationPanel_Enter);
            this.Leave += new System.EventHandler(this.AnimationPanel_Leave);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frameSpeedUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button animationPlayBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown totalFrame;
        private System.Windows.Forms.NumericUpDown currentFrameUpDown;
        private Forms.STPanel stPanel1;
        private Forms.STCheckBox loopChkBox;
        private Forms.KeyedAnimTimeline animationTrackBar;
        private Forms.STLabel stLabel1;
        private System.Windows.Forms.NumericUpDown frameSpeedUD;
    }
}