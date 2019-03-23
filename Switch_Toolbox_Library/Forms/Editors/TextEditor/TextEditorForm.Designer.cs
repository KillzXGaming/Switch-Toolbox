namespace Switch_Toolbox.Library.Forms
{
    partial class TextEditorForm
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
            this.textEditor1 = new Switch_Toolbox.Library.Forms.TextEditor();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.textEditor1);
            this.contentContainer.Controls.SetChildIndex(this.textEditor1, 0);
            // 
            // textEditor1
            // 
            this.textEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textEditor1.Location = new System.Drawing.Point(0, 24);
            this.textEditor1.Name = "textEditor1";
            this.textEditor1.Size = new System.Drawing.Size(543, 369);
            this.textEditor1.TabIndex = 0;
            // 
            // TextEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "TextEditorForm";
            this.Text = "TextEditorForm";
            this.contentContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TextEditor textEditor1;
    }
}