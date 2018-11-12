namespace Switch_Toolbox.Library
{
    partial class AnimationPanel
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
            this.animationPlayBtn = new System.Windows.Forms.Button();
            this.currentFrameUpDown = new System.Windows.Forms.NumericUpDown();
            this.totalFrame = new System.Windows.Forms.NumericUpDown();
            this.animationTrackBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // animationPlayBtn
            // 
            this.animationPlayBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationPlayBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.animationPlayBtn.ForeColor = System.Drawing.Color.White;
            this.animationPlayBtn.Location = new System.Drawing.Point(12, 12);
            this.animationPlayBtn.Name = "animationPlayBtn";
            this.animationPlayBtn.Size = new System.Drawing.Size(231, 47);
            this.animationPlayBtn.TabIndex = 0;
            this.animationPlayBtn.Text = "Play";
            this.animationPlayBtn.UseVisualStyleBackColor = false;
            this.animationPlayBtn.Click += new System.EventHandler(this.animationPlayBtn_Click);
            // 
            // currentFrameUpDown
            // 
            this.currentFrameUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.currentFrameUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.currentFrameUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.currentFrameUpDown.ForeColor = System.Drawing.Color.White;
            this.currentFrameUpDown.Location = new System.Drawing.Point(452, 21);
            this.currentFrameUpDown.Maximum = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            0});
            this.currentFrameUpDown.Name = "currentFrameUpDown";
            this.currentFrameUpDown.Size = new System.Drawing.Size(60, 16);
            this.currentFrameUpDown.TabIndex = 1;
            this.currentFrameUpDown.ValueChanged += new System.EventHandler(this.currentFrameUpDown_ValueChanged);
            // 
            // totalFrame
            // 
            this.totalFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.totalFrame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.totalFrame.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.totalFrame.ForeColor = System.Drawing.Color.White;
            this.totalFrame.Location = new System.Drawing.Point(530, 21);
            this.totalFrame.Maximum = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            0});
            this.totalFrame.Name = "totalFrame";
            this.totalFrame.Size = new System.Drawing.Size(60, 16);
            this.totalFrame.TabIndex = 2;
            this.totalFrame.ValueChanged += new System.EventHandler(this.totalFrame_ValueChanged);
            // 
            // animationTrackBar
            // 
            this.animationTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.animationTrackBar.Location = new System.Drawing.Point(249, 12);
            this.animationTrackBar.Name = "animationTrackBar";
            this.animationTrackBar.Size = new System.Drawing.Size(197, 45);
            this.animationTrackBar.TabIndex = 3;
            this.animationTrackBar.Scroll += new System.EventHandler(this.animationTrackBar_Scroll);
            this.animationTrackBar.ValueChanged += new System.EventHandler(this.animationTrackBar_ValueChanged);
            // 
            // AnimationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ClientSize = new System.Drawing.Size(602, 69);
            this.Controls.Add(this.animationTrackBar);
            this.Controls.Add(this.totalFrame);
            this.Controls.Add(this.currentFrameUpDown);
            this.Controls.Add(this.animationPlayBtn);
            this.Name = "AnimationPanel";
            this.Text = "AnimationPanel";
            this.Click += new System.EventHandler(this.AnimationPanel_Click);
            this.Enter += new System.EventHandler(this.AnimationPanel_Enter);
            this.Leave += new System.EventHandler(this.AnimationPanel_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button animationPlayBtn;
        private System.Windows.Forms.NumericUpDown currentFrameUpDown;
        private System.Windows.Forms.NumericUpDown totalFrame;
        private System.Windows.Forms.TrackBar animationTrackBar;
    }
}