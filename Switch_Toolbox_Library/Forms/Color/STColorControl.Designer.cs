namespace Toolbox.Library.Forms
{
    partial class STColorControl
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
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.alphaUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.blueUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.greenUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.redUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.alphaPB = new System.Windows.Forms.PictureBox();
            this.colorPB = new System.Windows.Forms.PictureBox();
            this.colorSelector1 = new Toolbox.Library.Forms.ColorSelector();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            this.stLabel7 = new Toolbox.Library.Forms.STLabel();
            this.hexTB = new Toolbox.Library.Forms.STTextBox();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alphaUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).BeginInit();
            this.SuspendLayout();
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.hexTB);
            this.stPanel1.Controls.Add(this.stLabel7);
            this.stPanel1.Controls.Add(this.stLabel6);
            this.stPanel1.Controls.Add(this.stLabel5);
            this.stPanel1.Controls.Add(this.stLabel4);
            this.stPanel1.Controls.Add(this.stLabel3);
            this.stPanel1.Controls.Add(this.stLabel2);
            this.stPanel1.Controls.Add(this.alphaUD);
            this.stPanel1.Controls.Add(this.stLabel1);
            this.stPanel1.Controls.Add(this.blueUD);
            this.stPanel1.Controls.Add(this.greenUD);
            this.stPanel1.Controls.Add(this.redUD);
            this.stPanel1.Controls.Add(this.alphaPB);
            this.stPanel1.Controls.Add(this.colorPB);
            this.stPanel1.Controls.Add(this.colorSelector1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(389, 279);
            this.stPanel1.TabIndex = 1;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(263, 187);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(14, 13);
            this.stLabel4.TabIndex = 11;
            this.stLabel4.Text = "A";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(263, 161);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(14, 13);
            this.stLabel3.TabIndex = 10;
            this.stLabel3.Text = "B";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(263, 135);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(15, 13);
            this.stLabel2.TabIndex = 9;
            this.stLabel2.Text = "G";
            // 
            // alphaUD
            // 
            this.alphaUD.Location = new System.Drawing.Point(284, 185);
            this.alphaUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.alphaUD.Name = "alphaUD";
            this.alphaUD.Size = new System.Drawing.Size(93, 20);
            this.alphaUD.TabIndex = 8;
            this.alphaUD.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(263, 109);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(15, 13);
            this.stLabel1.TabIndex = 7;
            this.stLabel1.Text = "R";
            // 
            // blueUD
            // 
            this.blueUD.Location = new System.Drawing.Point(284, 159);
            this.blueUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blueUD.Name = "blueUD";
            this.blueUD.Size = new System.Drawing.Size(93, 20);
            this.blueUD.TabIndex = 6;
            this.blueUD.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // greenUD
            // 
            this.greenUD.Location = new System.Drawing.Point(284, 133);
            this.greenUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.greenUD.Name = "greenUD";
            this.greenUD.Size = new System.Drawing.Size(93, 20);
            this.greenUD.TabIndex = 5;
            this.greenUD.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // redUD
            // 
            this.redUD.Location = new System.Drawing.Point(284, 107);
            this.redUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.redUD.Name = "redUD";
            this.redUD.Size = new System.Drawing.Size(93, 20);
            this.redUD.TabIndex = 4;
            this.redUD.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // alphaPB
            // 
            this.alphaPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaPB.Location = new System.Drawing.Point(338, 48);
            this.alphaPB.Name = "alphaPB";
            this.alphaPB.Size = new System.Drawing.Size(39, 39);
            this.alphaPB.TabIndex = 2;
            this.alphaPB.TabStop = false;
            // 
            // colorPB
            // 
            this.colorPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorPB.Location = new System.Drawing.Point(338, 3);
            this.colorPB.Name = "colorPB";
            this.colorPB.Size = new System.Drawing.Size(39, 39);
            this.colorPB.TabIndex = 1;
            this.colorPB.TabStop = false;
            // 
            // colorSelector1
            // 
            this.colorSelector1.Alpha = 0;
            this.colorSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorSelector1.Color = System.Drawing.Color.Empty;
            this.colorSelector1.DisplayAlpha = true;
            this.colorSelector1.DisplayColor = true;
            this.colorSelector1.Location = new System.Drawing.Point(3, 3);
            this.colorSelector1.Name = "colorSelector1";
            this.colorSelector1.Size = new System.Drawing.Size(254, 276);
            this.colorSelector1.TabIndex = 0;
            this.colorSelector1.ColorChanged += new System.EventHandler(this.colorSelector1_ColorChanged);
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(252, 226);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(26, 13);
            this.stLabel5.TabIndex = 13;
            this.stLabel5.Text = "Hex";
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(281, 20);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(31, 13);
            this.stLabel6.TabIndex = 14;
            this.stLabel6.Text = "Color";
            // 
            // stLabel7
            // 
            this.stLabel7.AutoSize = true;
            this.stLabel7.Location = new System.Drawing.Point(281, 61);
            this.stLabel7.Name = "stLabel7";
            this.stLabel7.Size = new System.Drawing.Size(34, 13);
            this.stLabel7.TabIndex = 15;
            this.stLabel7.Text = "Alpha";
            // 
            // hexTB
            // 
            this.hexTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hexTB.Location = new System.Drawing.Point(277, 224);
            this.hexTB.Name = "hexTB";
            this.hexTB.Size = new System.Drawing.Size(100, 20);
            this.hexTB.TabIndex = 16;
            this.hexTB.TextChanged += new System.EventHandler(this.stTextBox1_TextChanged);
            // 
            // STColorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 279);
            this.Controls.Add(this.stPanel1);
            this.Name = "STColorDialog";
            this.Load += new System.EventHandler(this.STColorDialog_Load);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alphaUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ColorSelector colorSelector1;
        private STPanel stPanel1;
        private System.Windows.Forms.PictureBox colorPB;
        private System.Windows.Forms.PictureBox alphaPB;
        private NumericUpDownUint blueUD;
        private NumericUpDownUint greenUD;
        private NumericUpDownUint redUD;
        private STLabel stLabel4;
        private STLabel stLabel3;
        private STLabel stLabel2;
        private NumericUpDownUint alphaUD;
        private STLabel stLabel1;
        private STLabel stLabel7;
        private STLabel stLabel6;
        private STLabel stLabel5;
        private STTextBox hexTB;
    }
}
