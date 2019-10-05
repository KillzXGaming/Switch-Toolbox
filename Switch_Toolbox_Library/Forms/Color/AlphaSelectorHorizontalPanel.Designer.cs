namespace Toolbox.Library.Forms
{
    partial class AlphaSelectorHorizontalPanel
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
            this.alphaPanel = new Toolbox.Library.Forms.STPanel();
            this.alphaSolidBP = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.alphaSolidBP)).BeginInit();
            this.SuspendLayout();
            // 
            // alphaPanel
            // 
            this.alphaPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaPanel.Location = new System.Drawing.Point(0, 0);
            this.alphaPanel.Name = "alphaPanel";
            this.alphaPanel.Size = new System.Drawing.Size(264, 35);
            this.alphaPanel.TabIndex = 3;
            this.alphaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.alphaPanel_Paint);
            this.alphaPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alphaPanel_MouseDown);
            this.alphaPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.alphaPanel_MouseMove);
            this.alphaPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.alphaPanel_MouseUp);
            // 
            // alphaSolidBP
            // 
            this.alphaSolidBP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaSolidBP.Location = new System.Drawing.Point(270, 0);
            this.alphaSolidBP.Name = "alphaSolidBP";
            this.alphaSolidBP.Size = new System.Drawing.Size(30, 35);
            this.alphaSolidBP.TabIndex = 4;
            this.alphaSolidBP.TabStop = false;
            this.alphaSolidBP.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // AlphaSelectorHorizontalPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.alphaSolidBP);
            this.Controls.Add(this.alphaPanel);
            this.Name = "AlphaSelectorHorizontalPanel";
            this.Size = new System.Drawing.Size(303, 38);
            ((System.ComponentModel.ISupportInitialize)(this.alphaSolidBP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private STPanel alphaPanel;
        private System.Windows.Forms.PictureBox alphaSolidBP;
    }
}
