namespace FirstPlugin
{
    partial class TextureOpenEditor
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
            this.bntxEditor1 = new BNTXEditor();
            this.SuspendLayout();
            // 
            // bntxEditor1
            // 
            this.bntxEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bntxEditor1.Location = new System.Drawing.Point(0, 0);
            this.bntxEditor1.Name = "bntxEditor1";
            this.bntxEditor1.Size = new System.Drawing.Size(522, 567);
            this.bntxEditor1.TabIndex = 0;
            // 
            // TextureOpenEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 567);
            this.Controls.Add(this.bntxEditor1);
            this.Name = "TextureOpenEditor";
            this.Text = "TextureOpenEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private BNTXEditor bntxEditor1;
    }
}