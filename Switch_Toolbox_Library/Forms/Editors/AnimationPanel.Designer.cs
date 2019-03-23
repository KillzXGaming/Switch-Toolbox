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
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.loopChkBox = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.totalFrame = new System.Windows.Forms.NumericUpDown();
            this.currentFrameUpDown = new System.Windows.Forms.NumericUpDown();
            this.animationPlayBtn = new System.Windows.Forms.Button();
            this.animationTrackBar = new ColorSlider.ColorSlider();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.arrowL;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.Location = new System.Drawing.Point(17, 7);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 21);
            this.button2.TabIndex = 2;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
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
            // loopChkBox
            // 
            this.loopChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loopChkBox.AutoSize = true;
            this.loopChkBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.loopChkBox.Checked = true;
            this.loopChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopChkBox.Location = new System.Drawing.Point(318, 10);
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
            this.totalFrame.Location = new System.Drawing.Point(508, 12);
            this.totalFrame.Name = "totalFrame";
            this.totalFrame.Size = new System.Drawing.Size(98, 16);
            this.totalFrame.TabIndex = 4;
            // 
            // currentFrameUpDown
            // 
            this.currentFrameUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.currentFrameUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.currentFrameUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.currentFrameUpDown.ForeColor = System.Drawing.Color.White;
            this.currentFrameUpDown.Location = new System.Drawing.Point(390, 12);
            this.currentFrameUpDown.Name = "currentFrameUpDown";
            this.currentFrameUpDown.Size = new System.Drawing.Size(98, 16);
            this.currentFrameUpDown.TabIndex = 3;
            this.currentFrameUpDown.ValueChanged += new System.EventHandler(this.currentFrameUpDown_ValueChanged);
            // 
            // animationPlayBtn
            // 
            this.animationPlayBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationPlayBtn.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.arrowR;
            this.animationPlayBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.animationPlayBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.animationPlayBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationPlayBtn.Location = new System.Drawing.Point(73, 7);
            this.animationPlayBtn.Name = "animationPlayBtn";
            this.animationPlayBtn.Size = new System.Drawing.Size(50, 21);
            this.animationPlayBtn.TabIndex = 1;
            this.animationPlayBtn.UseVisualStyleBackColor = false;
            this.animationPlayBtn.Click += new System.EventHandler(this.animationPlayBtn_Click);
            // 
            // animationTrackBar
            // 
            this.animationTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.animationTrackBar.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.animationTrackBar.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.animationTrackBar.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.animationTrackBar.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.animationTrackBar.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.animationTrackBar.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.animationTrackBar.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.animationTrackBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.animationTrackBar.ForeColor = System.Drawing.Color.Silver;
            this.animationTrackBar.LargeChange = ((uint)(5u));
            this.animationTrackBar.Location = new System.Drawing.Point(0, 16);
            this.animationTrackBar.MouseEffects = false;
            this.animationTrackBar.Name = "animationTrackBar";
            this.animationTrackBar.ScaleDivisions = 10;
            this.animationTrackBar.ScaleSubDivisions = 5;
            this.animationTrackBar.ShowDivisionsText = true;
            this.animationTrackBar.ShowSmallScale = true;
            this.animationTrackBar.Size = new System.Drawing.Size(631, 147);
            this.animationTrackBar.SmallChange = ((uint)(0u));
            this.animationTrackBar.TabIndex = 3;
            this.animationTrackBar.Text = "colorSlider1";
            this.animationTrackBar.ThumbInnerColor = System.Drawing.Color.Olive;
            this.animationTrackBar.ThumbOuterColor = System.Drawing.Color.Olive;
            this.animationTrackBar.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.animationTrackBar.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.animationTrackBar.ThumbSize = new System.Drawing.Size(5, 128);
            this.animationTrackBar.TickAdd = 0F;
            this.animationTrackBar.TickColor = System.Drawing.Color.Gray;
            this.animationTrackBar.TickDivide = 1F;
            this.animationTrackBar.Value = 0;
            this.animationTrackBar.ValueChanged += new System.EventHandler(this.animationTrackBar_ValueChanged);
            this.animationTrackBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider1_Scroll);
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
            // AnimationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.animationTrackBar);
            this.Controls.Add(this.panel1);
            this.Name = "AnimationPanel";
            this.Size = new System.Drawing.Size(631, 194);
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

        private ColorSlider.ColorSlider animationTrackBar;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button animationPlayBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown totalFrame;
        private System.Windows.Forms.NumericUpDown currentFrameUpDown;
        private Forms.STPanel stPanel1;
        private Forms.STCheckBox loopChkBox;
    }
}