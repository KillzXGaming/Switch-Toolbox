namespace Toolbox.Library.Forms
{
    partial class STColorDialog
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
            this.alphaPB = new System.Windows.Forms.PictureBox();
            this.colorPB = new System.Windows.Forms.PictureBox();
            this.colorSelector1 = new Toolbox.Library.Forms.ColorSelector();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.numericUpDownUint1 = new Toolbox.Library.Forms.NumericUpDownUint();
            this.numericUpDownUint2 = new Toolbox.Library.Forms.NumericUpDownUint();
            this.numericUpDownUint3 = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alphaPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint3)).BeginInit();
            this.SuspendLayout();
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.numericUpDownUint3);
            this.stPanel1.Controls.Add(this.numericUpDownUint2);
            this.stPanel1.Controls.Add(this.numericUpDownUint1);
            this.stPanel1.Controls.Add(this.stButton1);
            this.stPanel1.Controls.Add(this.alphaPB);
            this.stPanel1.Controls.Add(this.colorPB);
            this.stPanel1.Controls.Add(this.colorSelector1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(350, 259);
            this.stPanel1.TabIndex = 1;
            // 
            // alphaPB
            // 
            this.alphaPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaPB.Location = new System.Drawing.Point(257, 48);
            this.alphaPB.Name = "alphaPB";
            this.alphaPB.Size = new System.Drawing.Size(39, 39);
            this.alphaPB.TabIndex = 2;
            this.alphaPB.TabStop = false;
            // 
            // colorPB
            // 
            this.colorPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorPB.Location = new System.Drawing.Point(257, 3);
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
            this.colorSelector1.Size = new System.Drawing.Size(248, 256);
            this.colorSelector1.TabIndex = 0;
            this.colorSelector1.ColorChanged += new System.EventHandler(this.colorSelector1_ColorChanged);
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(272, 233);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 3;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // numericUpDownUint1
            // 
            this.numericUpDownUint1.Location = new System.Drawing.Point(257, 107);
            this.numericUpDownUint1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownUint1.Name = "numericUpDownUint1";
            this.numericUpDownUint1.Size = new System.Drawing.Size(93, 20);
            this.numericUpDownUint1.TabIndex = 4;
            // 
            // numericUpDownUint2
            // 
            this.numericUpDownUint2.Location = new System.Drawing.Point(257, 133);
            this.numericUpDownUint2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownUint2.Name = "numericUpDownUint2";
            this.numericUpDownUint2.Size = new System.Drawing.Size(93, 20);
            this.numericUpDownUint2.TabIndex = 5;
            // 
            // numericUpDownUint3
            // 
            this.numericUpDownUint3.Location = new System.Drawing.Point(257, 159);
            this.numericUpDownUint3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownUint3.Name = "numericUpDownUint3";
            this.numericUpDownUint3.Size = new System.Drawing.Size(93, 20);
            this.numericUpDownUint3.TabIndex = 6;
            // 
            // STColorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 259);
            this.Controls.Add(this.stPanel1);
            this.Name = "STColorDialog";
            this.stPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alphaPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ColorSelector colorSelector1;
        private STPanel stPanel1;
        private System.Windows.Forms.PictureBox colorPB;
        private System.Windows.Forms.PictureBox alphaPB;
        private STButton stButton1;
        private NumericUpDownUint numericUpDownUint3;
        private NumericUpDownUint numericUpDownUint2;
        private NumericUpDownUint numericUpDownUint1;
    }
}
