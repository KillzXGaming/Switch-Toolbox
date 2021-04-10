namespace Toolbox.Library.Forms
{
    partial class CubeMapFaceViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CubeMapFaceViewer));
            this.pbTopFace = new Toolbox.Library.Forms.PictureBoxCustom();
            this.pbFrontFace = new Toolbox.Library.Forms.PictureBoxCustom();
            this.pbLeftFace = new Toolbox.Library.Forms.PictureBoxCustom();
            this.pbBottomFace = new Toolbox.Library.Forms.PictureBoxCustom();
            this.pbBackFace = new Toolbox.Library.Forms.PictureBoxCustom();
            this.arrayLevelCounterLabel = new Toolbox.Library.Forms.STLabel();
            this.btnRightArray = new Toolbox.Library.Forms.STButton();
            this.btnLeftArray = new Toolbox.Library.Forms.STButton();
            this.pbRightFace = new Toolbox.Library.Forms.PictureBoxCustom();
            this.chkDisplayAlpha = new Toolbox.Library.Forms.STCheckBox();
            this.displayEncodedHDRAlphaChk = new Toolbox.Library.Forms.STCheckBox();
            this.gammaUD = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTopFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrontFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBottomFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gammaUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.BackColor = System.Drawing.Color.White;
            this.contentContainer.Controls.Add(this.gammaUD);
            this.contentContainer.Controls.Add(this.displayEncodedHDRAlphaChk);
            this.contentContainer.Controls.Add(this.chkDisplayAlpha);
            this.contentContainer.Controls.Add(this.arrayLevelCounterLabel);
            this.contentContainer.Controls.Add(this.btnRightArray);
            this.contentContainer.Controls.Add(this.btnLeftArray);
            this.contentContainer.Controls.Add(this.pbRightFace);
            this.contentContainer.Controls.Add(this.pbBackFace);
            this.contentContainer.Controls.Add(this.pbBottomFace);
            this.contentContainer.Controls.Add(this.pbLeftFace);
            this.contentContainer.Controls.Add(this.pbFrontFace);
            this.contentContainer.Controls.Add(this.pbTopFace);
            this.contentContainer.Size = new System.Drawing.Size(830, 650);
            this.contentContainer.Controls.SetChildIndex(this.pbTopFace, 0);
            this.contentContainer.Controls.SetChildIndex(this.pbFrontFace, 0);
            this.contentContainer.Controls.SetChildIndex(this.pbLeftFace, 0);
            this.contentContainer.Controls.SetChildIndex(this.pbBottomFace, 0);
            this.contentContainer.Controls.SetChildIndex(this.pbBackFace, 0);
            this.contentContainer.Controls.SetChildIndex(this.pbRightFace, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnLeftArray, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnRightArray, 0);
            this.contentContainer.Controls.SetChildIndex(this.arrayLevelCounterLabel, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkDisplayAlpha, 0);
            this.contentContainer.Controls.SetChildIndex(this.displayEncodedHDRAlphaChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.gammaUD, 0);
            // 
            // pbTopFace
            // 
            this.pbTopFace.BackColor = System.Drawing.Color.Transparent;
            this.pbTopFace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbTopFace.BackgroundImage")));
            this.pbTopFace.Location = new System.Drawing.Point(211, 31);
            this.pbTopFace.Name = "pbTopFace";
            this.pbTopFace.Size = new System.Drawing.Size(200, 200);
            this.pbTopFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbTopFace.TabIndex = 5;
            this.pbTopFace.TabStop = false;
            // 
            // pbFrontFace
            // 
            this.pbFrontFace.BackColor = System.Drawing.Color.Transparent;
            this.pbFrontFace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbFrontFace.BackgroundImage")));
            this.pbFrontFace.Location = new System.Drawing.Point(211, 237);
            this.pbFrontFace.Name = "pbFrontFace";
            this.pbFrontFace.Size = new System.Drawing.Size(200, 200);
            this.pbFrontFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFrontFace.TabIndex = 6;
            this.pbFrontFace.TabStop = false;
            // 
            // pbLeftFace
            // 
            this.pbLeftFace.BackColor = System.Drawing.Color.Transparent;
            this.pbLeftFace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbLeftFace.BackgroundImage")));
            this.pbLeftFace.Location = new System.Drawing.Point(5, 237);
            this.pbLeftFace.Name = "pbLeftFace";
            this.pbLeftFace.Size = new System.Drawing.Size(200, 200);
            this.pbLeftFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLeftFace.TabIndex = 7;
            this.pbLeftFace.TabStop = false;
            // 
            // pbBottomFace
            // 
            this.pbBottomFace.BackColor = System.Drawing.Color.Transparent;
            this.pbBottomFace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbBottomFace.BackgroundImage")));
            this.pbBottomFace.Location = new System.Drawing.Point(211, 443);
            this.pbBottomFace.Name = "pbBottomFace";
            this.pbBottomFace.Size = new System.Drawing.Size(200, 200);
            this.pbBottomFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBottomFace.TabIndex = 8;
            this.pbBottomFace.TabStop = false;
            // 
            // pbBackFace
            // 
            this.pbBackFace.BackColor = System.Drawing.Color.Transparent;
            this.pbBackFace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbBackFace.BackgroundImage")));
            this.pbBackFace.Location = new System.Drawing.Point(623, 237);
            this.pbBackFace.Name = "pbBackFace";
            this.pbBackFace.Size = new System.Drawing.Size(200, 200);
            this.pbBackFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBackFace.TabIndex = 10;
            this.pbBackFace.TabStop = false;
            // 
            // arrayLevelCounterLabel
            // 
            this.arrayLevelCounterLabel.AutoSize = true;
            this.arrayLevelCounterLabel.Location = new System.Drawing.Point(418, 55);
            this.arrayLevelCounterLabel.Name = "arrayLevelCounterLabel";
            this.arrayLevelCounterLabel.Size = new System.Drawing.Size(101, 13);
            this.arrayLevelCounterLabel.TabIndex = 19;
            this.arrayLevelCounterLabel.Text = "Array Level: 00 / 00";
            // 
            // btnRightArray
            // 
            this.btnRightArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRightArray.Location = new System.Drawing.Point(552, 51);
            this.btnRightArray.Name = "btnRightArray";
            this.btnRightArray.Size = new System.Drawing.Size(21, 21);
            this.btnRightArray.TabIndex = 18;
            this.btnRightArray.Text = ">";
            this.btnRightArray.UseVisualStyleBackColor = true;
            this.btnRightArray.Click += new System.EventHandler(this.btnRightArray_Click);
            // 
            // btnLeftArray
            // 
            this.btnLeftArray.Enabled = false;
            this.btnLeftArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeftArray.Location = new System.Drawing.Point(525, 51);
            this.btnLeftArray.Name = "btnLeftArray";
            this.btnLeftArray.Size = new System.Drawing.Size(21, 21);
            this.btnLeftArray.TabIndex = 17;
            this.btnLeftArray.Text = "<";
            this.btnLeftArray.UseVisualStyleBackColor = true;
            this.btnLeftArray.Click += new System.EventHandler(this.btnLeftArray_Click);
            // 
            // pbRightFace
            // 
            this.pbRightFace.BackColor = System.Drawing.Color.Transparent;
            this.pbRightFace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbRightFace.BackgroundImage")));
            this.pbRightFace.Location = new System.Drawing.Point(421, 237);
            this.pbRightFace.Name = "pbRightFace";
            this.pbRightFace.Size = new System.Drawing.Size(200, 200);
            this.pbRightFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbRightFace.TabIndex = 20;
            this.pbRightFace.TabStop = false;
            // 
            // chkDisplayAlpha
            // 
            this.chkDisplayAlpha.AutoSize = true;
            this.chkDisplayAlpha.Location = new System.Drawing.Point(421, 94);
            this.chkDisplayAlpha.Name = "chkDisplayAlpha";
            this.chkDisplayAlpha.Size = new System.Drawing.Size(90, 17);
            this.chkDisplayAlpha.TabIndex = 21;
            this.chkDisplayAlpha.Text = "Display Alpha";
            this.chkDisplayAlpha.UseVisualStyleBackColor = true;
            this.chkDisplayAlpha.CheckedChanged += new System.EventHandler(this.chkDisplayAlpha_CheckedChanged);
            // 
            // displayEncodedHDRAlphaChk
            // 
            this.displayEncodedHDRAlphaChk.AutoSize = true;
            this.displayEncodedHDRAlphaChk.Location = new System.Drawing.Point(421, 117);
            this.displayEncodedHDRAlphaChk.Name = "displayEncodedHDRAlphaChk";
            this.displayEncodedHDRAlphaChk.Size = new System.Drawing.Size(163, 17);
            this.displayEncodedHDRAlphaChk.TabIndex = 22;
            this.displayEncodedHDRAlphaChk.Text = "Display Encoded HDR Alpha";
            this.displayEncodedHDRAlphaChk.UseVisualStyleBackColor = true;
            this.displayEncodedHDRAlphaChk.CheckedChanged += new System.EventHandler(this.displayEncodedHDRAlphaChk_CheckedChanged);
            // 
            // gammaUD
            // 
            this.gammaUD.DecimalPlaces = 5;
            this.gammaUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.gammaUD.Location = new System.Drawing.Point(421, 149);
            this.gammaUD.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.gammaUD.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.gammaUD.Name = "gammaUD";
            this.gammaUD.Size = new System.Drawing.Size(120, 20);
            this.gammaUD.TabIndex = 23;
            this.gammaUD.ValueChanged += new System.EventHandler(this.gammaUD_ValueChanged);
            // 
            // CubeMapFaceViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(836, 655);
            this.Name = "CubeMapFaceViewer";
            this.Text = "CubeMap Face View";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTopFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrontFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeftFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBottomFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRightFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gammaUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Forms.PictureBoxCustom pbTopFace;
        private Forms.PictureBoxCustom pbFrontFace;
        private Forms.PictureBoxCustom pbLeftFace;
        private Forms.PictureBoxCustom pbBottomFace;
        private Forms.PictureBoxCustom pbBackFace;
        private STLabel arrayLevelCounterLabel;
        private STButton btnRightArray;
        private STButton btnLeftArray;
        private PictureBoxCustom pbRightFace;
        private STCheckBox chkDisplayAlpha;
        private STCheckBox displayEncodedHDRAlphaChk;
        private NumericUpDownFloat gammaUD;
    }
}