namespace FirstPlugin
{
    partial class EmitterEditorNX
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmitterEditorNX));
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.hexTB = new Toolbox.Library.Forms.STTextBox();
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPageColors = new System.Windows.Forms.TabPage();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.timeTB = new Toolbox.Library.Forms.STTextBox();
            this.colorSelector1 = new Toolbox.Library.Forms.ColorSelector();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stPanel6 = new Toolbox.Library.Forms.STPanel();
            this.stPanel5 = new Toolbox.Library.Forms.STPanel();
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.alpha1TypeCB = new Toolbox.Library.Forms.STComboBox();
            this.alpha0TypeCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.color1TypeCB = new Toolbox.Library.Forms.STComboBox();
            this.color0TypeCB = new Toolbox.Library.Forms.STComboBox();
            this.tabPageTextures = new System.Windows.Forms.TabPage();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.TBTexture0 = new Toolbox.Library.Forms.STTextBox();
            this.TBTexture2 = new Toolbox.Library.Forms.STTextBox();
            this.pictureBox3 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.TBTexture1 = new Toolbox.Library.Forms.STTextBox();
            this.pictureBox2 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.pictureBox1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.stTabControl1.SuspendLayout();
            this.tabPageColors.SuspendLayout();
            this.stPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.tabPageTextures.SuspendLayout();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(3, 18);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(40, 13);
            this.stLabel1.TabIndex = 0;
            this.stLabel1.Text = "Color 0";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(3, 86);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(40, 13);
            this.stLabel2.TabIndex = 2;
            this.stLabel2.Text = "Color 1";
            // 
            // hexTB
            // 
            this.hexTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hexTB.Location = new System.Drawing.Point(316, 255);
            this.hexTB.Name = "hexTB";
            this.hexTB.Size = new System.Drawing.Size(153, 20);
            this.hexTB.TabIndex = 27;
            this.hexTB.TextChanged += new System.EventHandler(this.hexTB_TextChanged);
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPageColors);
            this.stTabControl1.Controls.Add(this.tabPageTextures);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl1.Location = new System.Drawing.Point(0, 0);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(555, 561);
            this.stTabControl1.TabIndex = 38;
            // 
            // tabPageColors
            // 
            this.tabPageColors.Controls.Add(this.stPanel2);
            this.tabPageColors.Location = new System.Drawing.Point(4, 25);
            this.tabPageColors.Name = "tabPageColors";
            this.tabPageColors.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageColors.Size = new System.Drawing.Size(547, 532);
            this.tabPageColors.TabIndex = 0;
            this.tabPageColors.Text = "Colors";
            this.tabPageColors.UseVisualStyleBackColor = true;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.pictureBox4);
            this.stPanel2.Controls.Add(this.timeTB);
            this.stPanel2.Controls.Add(this.colorSelector1);
            this.stPanel2.Controls.Add(this.stLabel5);
            this.stPanel2.Controls.Add(this.stPanel6);
            this.stPanel2.Controls.Add(this.stPanel5);
            this.stPanel2.Controls.Add(this.stPanel4);
            this.stPanel2.Controls.Add(this.stPanel3);
            this.stPanel2.Controls.Add(this.alpha1TypeCB);
            this.stPanel2.Controls.Add(this.alpha0TypeCB);
            this.stPanel2.Controls.Add(this.stLabel3);
            this.stPanel2.Controls.Add(this.stLabel4);
            this.stPanel2.Controls.Add(this.color1TypeCB);
            this.stPanel2.Controls.Add(this.color0TypeCB);
            this.stPanel2.Controls.Add(this.stLabel1);
            this.stPanel2.Controls.Add(this.stLabel2);
            this.stPanel2.Controls.Add(this.hexTB);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(3, 3);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(541, 526);
            this.stPanel2.TabIndex = 0;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(488, 235);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(40, 40);
            this.pictureBox4.TabIndex = 45;
            this.pictureBox4.TabStop = false;
            // 
            // timeTB
            // 
            this.timeTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeTB.Location = new System.Drawing.Point(352, 18);
            this.timeTB.Name = "timeTB";
            this.timeTB.Size = new System.Drawing.Size(100, 20);
            this.timeTB.TabIndex = 0;
            // 
            // colorSelector1
            // 
            this.colorSelector1.Color = System.Drawing.Color.Empty;
            this.colorSelector1.Location = new System.Drawing.Point(316, 44);
            this.colorSelector1.MaximumSize = new System.Drawing.Size(212, 188);
            this.colorSelector1.MinimumSize = new System.Drawing.Size(212, 188);
            this.colorSelector1.Name = "colorSelector1";
            this.colorSelector1.Size = new System.Drawing.Size(212, 188);
            this.colorSelector1.TabIndex = 44;
            this.colorSelector1.ColorChanged += new System.EventHandler(this.colorSelector1_ColorChanged);
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(313, 18);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(33, 13);
            this.stLabel5.TabIndex = 43;
            this.stLabel5.Text = "Time:";
            // 
            // stPanel6
            // 
            this.stPanel6.Location = new System.Drawing.Point(6, 242);
            this.stPanel6.Name = "stPanel6";
            this.stPanel6.Size = new System.Drawing.Size(304, 33);
            this.stPanel6.TabIndex = 41;
            // 
            // stPanel5
            // 
            this.stPanel5.Location = new System.Drawing.Point(6, 176);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(304, 33);
            this.stPanel5.TabIndex = 40;
            // 
            // stPanel4
            // 
            this.stPanel4.Location = new System.Drawing.Point(6, 110);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(304, 33);
            this.stPanel4.TabIndex = 39;
            // 
            // stPanel3
            // 
            this.stPanel3.Location = new System.Drawing.Point(6, 44);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(304, 33);
            this.stPanel3.TabIndex = 38;
            // 
            // alpha1TypeCB
            // 
            this.alpha1TypeCB.BorderColor = System.Drawing.Color.Empty;
            this.alpha1TypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.alpha1TypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.alpha1TypeCB.FormattingEnabled = true;
            this.alpha1TypeCB.Location = new System.Drawing.Point(175, 215);
            this.alpha1TypeCB.Name = "alpha1TypeCB";
            this.alpha1TypeCB.Size = new System.Drawing.Size(135, 21);
            this.alpha1TypeCB.TabIndex = 37;
            // 
            // alpha0TypeCB
            // 
            this.alpha0TypeCB.BorderColor = System.Drawing.Color.Empty;
            this.alpha0TypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.alpha0TypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.alpha0TypeCB.FormattingEnabled = true;
            this.alpha0TypeCB.Location = new System.Drawing.Point(175, 149);
            this.alpha0TypeCB.Name = "alpha0TypeCB";
            this.alpha0TypeCB.Size = new System.Drawing.Size(135, 21);
            this.alpha0TypeCB.TabIndex = 35;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(3, 150);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(43, 13);
            this.stLabel3.TabIndex = 32;
            this.stLabel3.Text = "Alpha 0";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(3, 218);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(43, 13);
            this.stLabel4.TabIndex = 33;
            this.stLabel4.Text = "Alpha 1";
            // 
            // color1TypeCB
            // 
            this.color1TypeCB.BorderColor = System.Drawing.Color.Empty;
            this.color1TypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.color1TypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.color1TypeCB.FormattingEnabled = true;
            this.color1TypeCB.Location = new System.Drawing.Point(175, 83);
            this.color1TypeCB.Name = "color1TypeCB";
            this.color1TypeCB.Size = new System.Drawing.Size(135, 21);
            this.color1TypeCB.TabIndex = 31;
            // 
            // color0TypeCB
            // 
            this.color0TypeCB.BorderColor = System.Drawing.Color.Empty;
            this.color0TypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.color0TypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.color0TypeCB.FormattingEnabled = true;
            this.color0TypeCB.Location = new System.Drawing.Point(175, 17);
            this.color0TypeCB.Name = "color0TypeCB";
            this.color0TypeCB.Size = new System.Drawing.Size(135, 21);
            this.color0TypeCB.TabIndex = 29;
            // 
            // tabPageTextures
            // 
            this.tabPageTextures.Controls.Add(this.stPanel1);
            this.tabPageTextures.Location = new System.Drawing.Point(4, 25);
            this.tabPageTextures.Name = "tabPageTextures";
            this.tabPageTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTextures.Size = new System.Drawing.Size(547, 532);
            this.tabPageTextures.TabIndex = 1;
            this.tabPageTextures.Text = "Textures";
            this.tabPageTextures.UseVisualStyleBackColor = true;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.TBTexture0);
            this.stPanel1.Controls.Add(this.TBTexture2);
            this.stPanel1.Controls.Add(this.pictureBox3);
            this.stPanel1.Controls.Add(this.TBTexture1);
            this.stPanel1.Controls.Add(this.pictureBox2);
            this.stPanel1.Controls.Add(this.pictureBox1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(3, 3);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(541, 526);
            this.stPanel1.TabIndex = 41;
            // 
            // TBTexture0
            // 
            this.TBTexture0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TBTexture0.Location = new System.Drawing.Point(14, 15);
            this.TBTexture0.Name = "TBTexture0";
            this.TBTexture0.Size = new System.Drawing.Size(150, 20);
            this.TBTexture0.TabIndex = 38;
            // 
            // TBTexture2
            // 
            this.TBTexture2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TBTexture2.Location = new System.Drawing.Point(14, 335);
            this.TBTexture2.Name = "TBTexture2";
            this.TBTexture2.Size = new System.Drawing.Size(150, 20);
            this.TBTexture2.TabIndex = 40;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox3.BackgroundImage")));
            this.pictureBox3.Location = new System.Drawing.Point(14, 361);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(150, 134);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 37;
            this.pictureBox3.TabStop = false;
            // 
            // TBTexture1
            // 
            this.TBTexture1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TBTexture1.Location = new System.Drawing.Point(14, 166);
            this.TBTexture1.Name = "TBTexture1";
            this.TBTexture1.Size = new System.Drawing.Size(150, 20);
            this.TBTexture1.TabIndex = 39;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.Location = new System.Drawing.Point(14, 192);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(150, 134);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 36;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(14, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 120);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // EmitterEditorNX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTabControl1);
            this.Name = "EmitterEditorNX";
            this.Size = new System.Drawing.Size(555, 561);
            this.stTabControl1.ResumeLayout(false);
            this.tabPageColors.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.tabPageTextures.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STTextBox hexTB;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBox2;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBox3;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBox1;
        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPageColors;
        private System.Windows.Forms.TabPage tabPageTextures;
        private Toolbox.Library.Forms.STTextBox TBTexture2;
        private Toolbox.Library.Forms.STTextBox TBTexture1;
        private Toolbox.Library.Forms.STTextBox TBTexture0;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STComboBox color1TypeCB;
        private Toolbox.Library.Forms.STComboBox color0TypeCB;
        private Toolbox.Library.Forms.STComboBox alpha1TypeCB;
        private Toolbox.Library.Forms.STComboBox alpha0TypeCB;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STPanel stPanel6;
        private Toolbox.Library.Forms.STPanel stPanel5;
        private Toolbox.Library.Forms.STPanel stPanel4;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.ColorSelector colorSelector1;
        private Toolbox.Library.Forms.STTextBox timeTB;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}
