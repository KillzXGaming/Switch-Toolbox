namespace Toolbox.Library
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
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toggleFrameRateBtn = new System.Windows.Forms.Button();
            this.loopChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.totalFrame = new System.Windows.Forms.NumericUpDown();
            this.currentFrameUpDown = new System.Windows.Forms.NumericUpDown();
            this.animationPlayBtn = new System.Windows.Forms.Button();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.animationTrackBar = new Toolbox.Library.Forms.TimeLine();
            this.panel1.SuspendLayout();
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
            this.button2.Location = new System.Drawing.Point(26, 10);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(51, 29);
            this.button2.TabIndex = 2;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.GoToPreviousFrame);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panel1.Controls.Add(this.toggleFrameRateBtn);
            this.panel1.Controls.Add(this.loopChkBox);
            this.panel1.Controls.Add(this.totalFrame);
            this.panel1.Controls.Add(this.currentFrameUpDown);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.animationPlayBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 225);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(946, 44);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // toggleFrameRateBtn
            // 
            this.toggleFrameRateBtn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toggleFrameRateBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toggleFrameRateBtn.Location = new System.Drawing.Point(139, 11);
            this.toggleFrameRateBtn.Name = "toggleFrameRateBtn";
            this.toggleFrameRateBtn.Size = new System.Drawing.Size(78, 30);
            this.toggleFrameRateBtn.TabIndex = 6;
            this.toggleFrameRateBtn.Text = "30 FPS";
            this.toggleFrameRateBtn.UseVisualStyleBackColor = true;
            this.toggleFrameRateBtn.Click += new System.EventHandler(this.toggleFrameRateBtn_Click);
            // 
            // loopChkBox
            // 
            this.loopChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loopChkBox.AutoSize = true;
            this.loopChkBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.loopChkBox.Checked = true;
            this.loopChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopChkBox.Location = new System.Drawing.Point(683, 11);
            this.loopChkBox.Margin = new System.Windows.Forms.Padding(4);
            this.loopChkBox.Name = "loopChkBox";
            this.loopChkBox.Size = new System.Drawing.Size(70, 22);
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
            this.totalFrame.Location = new System.Drawing.Point(856, 14);
            this.totalFrame.Margin = new System.Windows.Forms.Padding(4);
            this.totalFrame.Name = "totalFrame";
            this.totalFrame.Size = new System.Drawing.Size(86, 24);
            this.totalFrame.TabIndex = 4;
            // 
            // currentFrameUpDown
            // 
            this.currentFrameUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.currentFrameUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.currentFrameUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.currentFrameUpDown.ForeColor = System.Drawing.Color.White;
            this.currentFrameUpDown.Location = new System.Drawing.Point(762, 14);
            this.currentFrameUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.currentFrameUpDown.Name = "currentFrameUpDown";
            this.currentFrameUpDown.Size = new System.Drawing.Size(86, 24);
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
            this.animationPlayBtn.Location = new System.Drawing.Point(86, 10);
            this.animationPlayBtn.Margin = new System.Windows.Forms.Padding(4);
            this.animationPlayBtn.Name = "animationPlayBtn";
            this.animationPlayBtn.Size = new System.Drawing.Size(46, 29);
            this.animationPlayBtn.TabIndex = 1;
            this.animationPlayBtn.UseVisualStyleBackColor = false;
            this.animationPlayBtn.Click += new System.EventHandler(this.animationPlayBtn_Click);
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(946, 22);
            this.stPanel1.TabIndex = 5;
            // 
            // animationTrackBar
            // 
            this.animationTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrackBar.CurrentFrame = 0F;
            this.animationTrackBar.FrameCount = 1014F;
            this.animationTrackBar.Location = new System.Drawing.Point(0, 19);
            this.animationTrackBar.Margin = new System.Windows.Forms.Padding(6);
            this.animationTrackBar.Name = "animationTrackBar";
            this.animationTrackBar.Size = new System.Drawing.Size(946, 206);
            this.animationTrackBar.TabIndex = 6;
            // 
            // AnimationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.animationTrackBar);
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AnimationPanel";
            this.Size = new System.Drawing.Size(946, 269);
            this.Load += new System.EventHandler(this.AnimationPanel_Load);
            this.Click += new System.EventHandler(this.AnimationPanel_Click);
            this.Enter += new System.EventHandler(this.AnimationPanel_Enter);
            this.Leave += new System.EventHandler(this.AnimationPanel_Leave);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private Forms.TimeLine animationTrackBar;
        private System.Windows.Forms.Button toggleFrameRateBtn;
    }
}