namespace Toolbox.Library.Forms
{
    partial class CubeMapFaceViewer3D
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
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.encodeHDRAlphaChk = new Toolbox.Library.Forms.STCheckBox();
            this.gammaUD = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gammaUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.gammaUD);
            this.contentContainer.Controls.Add(this.encodeHDRAlphaChk);
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Size = new System.Drawing.Size(583, 455);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.encodeHDRAlphaChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.gammaUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(0, 51);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(580, 404);
            this.stPanel1.TabIndex = 11;
            // 
            // encodeHDRAlphaChk
            // 
            this.encodeHDRAlphaChk.AutoSize = true;
            this.encodeHDRAlphaChk.Location = new System.Drawing.Point(10, 32);
            this.encodeHDRAlphaChk.Name = "encodeHDRAlphaChk";
            this.encodeHDRAlphaChk.Size = new System.Drawing.Size(125, 17);
            this.encodeHDRAlphaChk.TabIndex = 12;
            this.encodeHDRAlphaChk.Text = "View Encoded Alpha";
            this.encodeHDRAlphaChk.UseVisualStyleBackColor = true;
            this.encodeHDRAlphaChk.CheckedChanged += new System.EventHandler(this.encodeHDRAlphaChk_CheckedChanged);
            // 
            // gammaUD
            // 
            this.gammaUD.DecimalPlaces = 5;
            this.gammaUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.gammaUD.Location = new System.Drawing.Point(193, 29);
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
            this.gammaUD.TabIndex = 13;
            this.gammaUD.ValueChanged += new System.EventHandler(this.gammaUD_ValueChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(141, 31);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(46, 13);
            this.stLabel1.TabIndex = 14;
            this.stLabel1.Text = "Gamma:";
            // 
            // CubeMapFaceViewer3D
            // 
            this.ClientSize = new System.Drawing.Size(589, 460);
            this.Name = "CubeMapFaceViewer3D";
            this.Text = "CubeMapFaceViewer3D";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gammaUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private STPanel stPanel1;
        private STLabel stLabel1;
        private NumericUpDownFloat gammaUD;
        private STCheckBox encodeHDRAlphaChk;
    }
}