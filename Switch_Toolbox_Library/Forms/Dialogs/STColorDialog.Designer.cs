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
            this.colorSelector1 = new Toolbox.Library.Forms.ColorSelector();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
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
            this.colorSelector1.Size = new System.Drawing.Size(278, 258);
            this.colorSelector1.TabIndex = 0;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.colorSelector1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(284, 261);
            this.stPanel1.TabIndex = 1;
            // 
            // STColorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.stPanel1);
            this.Name = "STColorDialog";
            this.stPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ColorSelector colorSelector1;
        private STPanel stPanel1;
    }
}
