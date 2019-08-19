namespace FirstPlugin.Forms
{
    partial class Color8KeySlider
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
            this.SuspendLayout();
            // 
            // Color8KeySlider
            // 
            this.Size = new System.Drawing.Size(396, 42);
            this.Click += new System.EventHandler(this.Color8KeySlider_Click);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Color8KeySlider_MouseDown);
            this.MouseHover += new System.EventHandler(this.Color8KeySlider_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Color8KeySlider_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Color8KeySlider_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
