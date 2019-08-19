namespace FirstPlugin.Forms
{
    partial class ColorConstantPanel
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
            this.color0PB = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // color0PB
            // 
            this.color0PB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.color0PB.Location = new System.Drawing.Point(3, 4);
            this.color0PB.Name = "color0PB";
            this.color0PB.Size = new System.Drawing.Size(30, 30);
            this.color0PB.TabIndex = 1;
            this.color0PB.Click += new System.EventHandler(this.color0PB_Click);
            // 
            // ColorConstantPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.color0PB);
            this.Name = "ColorConstantPanel";
            this.Size = new System.Drawing.Size(291, 37);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel color0PB;
    }
}
