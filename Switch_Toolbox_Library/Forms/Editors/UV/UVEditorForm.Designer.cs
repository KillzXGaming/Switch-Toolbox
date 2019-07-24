namespace Toolbox.Library.Forms
{
    partial class UVEditorForm
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
            this.uvEditor1 = new Toolbox.Library.Forms.UVEditor();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.contentContainer.Controls.Add(this.uvEditor1);
            this.contentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentContainer.Location = new System.Drawing.Point(0, 0);
            this.contentContainer.Size = new System.Drawing.Size(595, 468);
            this.contentContainer.Controls.SetChildIndex(this.uvEditor1, 0);
            // 
            // uvEditor1
            // 
            this.uvEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uvEditor1.Location = new System.Drawing.Point(0, 25);
            this.uvEditor1.Name = "uvEditor1";
            this.uvEditor1.Size = new System.Drawing.Size(595, 443);
            this.uvEditor1.TabIndex = 11;
            // 
            // UVEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 468);
            this.Name = "UVEditorForm";
            this.Text = "UV Editor";
            this.contentContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UVEditor uvEditor1;
    }
}