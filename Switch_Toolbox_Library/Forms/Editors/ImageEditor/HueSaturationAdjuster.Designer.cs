namespace Toolbox.Library.Forms
{
    partial class HueSaturationAdjuster
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
            this.hueTrackBar = new System.Windows.Forms.TrackBar();
            this.saturationTrackBar = new System.Windows.Forms.TrackBar();
            this.brightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            ((System.ComponentModel.ISupportInitialize)(this.hueTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.saturationTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Size = new System.Drawing.Size(492, 191);
            // 
            // hueTrackBar
            // 
            this.hueTrackBar.Location = new System.Drawing.Point(56, 51);
            this.hueTrackBar.Maximum = 100;
            this.hueTrackBar.Name = "hueTrackBar";
            this.hueTrackBar.Size = new System.Drawing.Size(420, 45);
            this.hueTrackBar.TabIndex = 0;
            this.hueTrackBar.Scroll += new System.EventHandler(this.hueTrackBar_Scroll);
            this.hueTrackBar.ValueChanged += new System.EventHandler(this.hueTrackBar_ValueChanged);
            // 
            // saturationTrackBar
            // 
            this.saturationTrackBar.Location = new System.Drawing.Point(56, 102);
            this.saturationTrackBar.Maximum = 100;
            this.saturationTrackBar.Name = "saturationTrackBar";
            this.saturationTrackBar.Size = new System.Drawing.Size(420, 45);
            this.saturationTrackBar.TabIndex = 1;
            this.saturationTrackBar.Scroll += new System.EventHandler(this.saturationTrackBar_Scroll);
            this.saturationTrackBar.ValueChanged += new System.EventHandler(this.saturationTrackBar_ValueChanged);
            // 
            // brightnessTrackBar
            // 
            this.brightnessTrackBar.Location = new System.Drawing.Point(56, 153);
            this.brightnessTrackBar.Maximum = 100;
            this.brightnessTrackBar.Name = "brightnessTrackBar";
            this.brightnessTrackBar.Size = new System.Drawing.Size(420, 45);
            this.brightnessTrackBar.TabIndex = 2;
            this.brightnessTrackBar.Scroll += new System.EventHandler(this.brightnessTrackBar_Scroll);
            this.brightnessTrackBar.ValueChanged += new System.EventHandler(this.brightnessTrackBar_ValueChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(20, 31);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(27, 13);
            this.stLabel1.TabIndex = 3;
            this.stLabel1.Text = "Hue";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(20, 83);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(55, 13);
            this.stLabel2.TabIndex = 11;
            this.stLabel2.Text = "Saturation";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(20, 134);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(56, 13);
            this.stLabel3.TabIndex = 12;
            this.stLabel3.Text = "Brightness";
            // 
            // HueSaturationAdjuster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 196);
            this.Controls.Add(this.stLabel3);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.brightnessTrackBar);
            this.Controls.Add(this.saturationTrackBar);
            this.Controls.Add(this.hueTrackBar);
            this.Name = "HueSaturationAdjuster";
            this.Text = "HueSaturationAdjuster";
            this.Controls.SetChildIndex(this.contentContainer, 0);
            this.Controls.SetChildIndex(this.hueTrackBar, 0);
            this.Controls.SetChildIndex(this.saturationTrackBar, 0);
            this.Controls.SetChildIndex(this.brightnessTrackBar, 0);
            this.Controls.SetChildIndex(this.stLabel1, 0);
            this.Controls.SetChildIndex(this.stLabel2, 0);
            this.Controls.SetChildIndex(this.stLabel3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.hueTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.saturationTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar hueTrackBar;
        private System.Windows.Forms.TrackBar saturationTrackBar;
        private System.Windows.Forms.TrackBar brightnessTrackBar;
        private STLabel stLabel1;
        private STLabel stLabel2;
        private STLabel stLabel3;
    }
}