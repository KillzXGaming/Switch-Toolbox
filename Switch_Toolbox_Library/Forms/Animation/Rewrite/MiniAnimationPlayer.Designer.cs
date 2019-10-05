namespace Toolbox.Library.Forms
{
    partial class MiniAnimationPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MiniAnimationPlayer));
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.loopChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.animationTrackBar = new ColorSlider.ColorSlider();
            this.totalFrame = new Toolbox.Library.Forms.STNumbericUpDown();
            this.currentFrameUpDown = new Toolbox.Library.Forms.STNumbericUpDown();
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.btnStop = new Toolbox.Library.Forms.STButton();
            this.btnForward1 = new Toolbox.Library.Forms.STButton();
            this.btnPlay = new Toolbox.Library.Forms.STButton();
            this.btnBackward1 = new Toolbox.Library.Forms.STButton();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUpDown)).BeginInit();
            this.stPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel3
            // 
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel3.Location = new System.Drawing.Point(0, 3);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(467, 68);
            this.stPanel3.TabIndex = 5;
            this.stPanel3.Controls.Add(this.loopChkBox);
            this.stPanel3.Controls.Add(this.animationTrackBar);
            this.stPanel3.Controls.Add(this.totalFrame);
            this.stPanel3.Controls.Add(this.currentFrameUpDown);
            this.stPanel3.Controls.Add(this.stPanel4);
            // 
            // loopChkBox
            // 
            this.loopChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loopChkBox.AutoSize = true;
            this.loopChkBox.Checked = true;
            this.loopChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopChkBox.Location = new System.Drawing.Point(340, 5);
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
            this.animationTrackBar.Size = new System.Drawing.Size(443, 19);
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
            this.animationTrackBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.animationTrackBar_Scroll);
            // 
            // totalFrame
            // 
            this.totalFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.totalFrame.Location = new System.Drawing.Point(340, 25);
            this.totalFrame.Name = "totalFrame";
            this.totalFrame.Size = new System.Drawing.Size(109, 20);
            this.totalFrame.TabIndex = 15;
            this.totalFrame.ValueChanged += new System.EventHandler(this.totalFrame_ValueChanged);
            // 
            // currentFrameUpDown
            // 
            this.currentFrameUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.currentFrameUpDown.Location = new System.Drawing.Point(8, 25);
            this.currentFrameUpDown.Name = "currentFrameUpDown";
            this.currentFrameUpDown.Size = new System.Drawing.Size(109, 20);
            this.currentFrameUpDown.TabIndex = 14;
            this.currentFrameUpDown.ValueChanged += new System.EventHandler(this.currentFrameUpDown_ValueChanged);
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.Controls.Add(this.btnStop);
            this.stPanel4.Controls.Add(this.btnForward1);
            this.stPanel4.Controls.Add(this.btnPlay);
            this.stPanel4.Controls.Add(this.btnBackward1);
            this.stPanel4.Location = new System.Drawing.Point(123, 7);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(212, 41);
            this.stPanel4.TabIndex = 13;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStop.BackgroundImage")));
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(153, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(35, 27);
            this.btnStop.TabIndex = 3;
            this.btnStop.UseVisualStyleBackColor = false;
            // 
            // btnForward1
            // 
            this.btnForward1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnForward1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnForward1.BackgroundImage")));
            this.btnForward1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnForward1.FlatAppearance.BorderSize = 0;
            this.btnForward1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForward1.Location = new System.Drawing.Point(110, 10);
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
            this.btnPlay.Location = new System.Drawing.Point(58, 6);
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
            this.btnBackward1.Location = new System.Drawing.Point(19, 10);
            this.btnBackward1.Name = "btnBackward1";
            this.btnBackward1.Size = new System.Drawing.Size(20, 20);
            this.btnBackward1.TabIndex = 1;
            this.btnBackward1.UseVisualStyleBackColor = false;
            // 
            // MiniAnimationPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel3);
            this.Name = "MiniAnimationPlayer";
            this.Size = new System.Drawing.Size(467, 71);
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUpDown)).EndInit();
            this.stPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private STPanel stPanel3;
        private STCheckBox loopChkBox;
        private ColorSlider.ColorSlider animationTrackBar;
        private STNumbericUpDown totalFrame;
        private STNumbericUpDown currentFrameUpDown;
        private STPanel stPanel4;
        private STButton btnStop;
        private STButton btnForward1;
        private STButton btnPlay;
        private STButton btnBackward1;
    }
}
