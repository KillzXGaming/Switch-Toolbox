namespace LayoutBXLYT
{
    partial class PaneMatColorEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaneMatColorEditor));
            this.chkAlphaInterpolation = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.whiteColorPB = new Toolbox.Library.Forms.ColorAlphaBox();
            this.blackColorBP = new Toolbox.Library.Forms.ColorAlphaBox();
            this.SuspendLayout();
            // 
            // chkAlphaInterpolation
            // 
            this.chkAlphaInterpolation.AutoSize = true;
            this.chkAlphaInterpolation.Location = new System.Drawing.Point(16, 13);
            this.chkAlphaInterpolation.Name = "chkAlphaInterpolation";
            this.chkAlphaInterpolation.Size = new System.Drawing.Size(147, 17);
            this.chkAlphaInterpolation.TabIndex = 0;
            this.chkAlphaInterpolation.Text = "Threshold Alpha Blending";
            this.chkAlphaInterpolation.UseVisualStyleBackColor = true;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(131, 55);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(62, 13);
            this.stLabel1.TabIndex = 2;
            this.stLabel1.Text = "White Color";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(132, 103);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(61, 13);
            this.stLabel2.TabIndex = 4;
            this.stLabel2.Text = "Black Color";
            // 
            // whiteColorPB
            // 
            this.whiteColorPB.BackColor = System.Drawing.Color.Transparent;
            this.whiteColorPB.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("whiteColorPB.BackgroundImage")));
            this.whiteColorPB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.whiteColorPB.Color = System.Drawing.Color.Empty;
            this.whiteColorPB.DisplayAlphaSolid = false;
            this.whiteColorPB.Location = new System.Drawing.Point(16, 36);
            this.whiteColorPB.Name = "whiteColorPB";
            this.whiteColorPB.Size = new System.Drawing.Size(90, 45);
            this.whiteColorPB.TabIndex = 5;
            this.whiteColorPB.Click += new System.EventHandler(this.whiteColorPB_Click);
            // 
            // blackColorBP
            // 
            this.blackColorBP.BackColor = System.Drawing.Color.Transparent;
            this.blackColorBP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("blackColorBP.BackgroundImage")));
            this.blackColorBP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.blackColorBP.Color = System.Drawing.Color.Empty;
            this.blackColorBP.DisplayAlphaSolid = false;
            this.blackColorBP.Location = new System.Drawing.Point(16, 87);
            this.blackColorBP.Name = "blackColorBP";
            this.blackColorBP.Size = new System.Drawing.Size(90, 45);
            this.blackColorBP.TabIndex = 6;
            this.blackColorBP.Click += new System.EventHandler(this.blackColorBP_Click);
            // 
            // PaneMatColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.blackColorBP);
            this.Controls.Add(this.whiteColorPB);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.chkAlphaInterpolation);
            this.Name = "PaneMatColorEditor";
            this.Size = new System.Drawing.Size(303, 313);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STCheckBox chkAlphaInterpolation;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.ColorAlphaBox whiteColorPB;
        private Toolbox.Library.Forms.ColorAlphaBox blackColorBP;
    }
}
