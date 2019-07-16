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
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Size = new System.Drawing.Size(314, 332);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            // 
            // stPanel1
            // 
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 25);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(314, 307);
            this.stPanel1.TabIndex = 11;
            // 
            // CubeMapFaceViewer3D
            // 
            this.ClientSize = new System.Drawing.Size(320, 337);
            this.Name = "CubeMapFaceViewer3D";
            this.Text = "CubeMapFaceViewer3D";
            this.contentContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private STPanel stPanel1;
    }
}