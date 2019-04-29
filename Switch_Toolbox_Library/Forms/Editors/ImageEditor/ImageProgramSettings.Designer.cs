namespace Switch_Toolbox.Library.Forms
{
    partial class ImageProgramSettings
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
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.textureImageFormatCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.textureFileFormatCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.textureFileFormatCB);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.textureImageFormatCB);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.textureImageFormatCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.textureFileFormatCB, 0);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(20, 67);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(121, 13);
            this.stLabel1.TabIndex = 11;
            this.stLabel1.Text = "Format to save back as:";
            // 
            // textureImageFormatCB
            // 
            this.textureImageFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.textureImageFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.textureImageFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.textureImageFormatCB.FormattingEnabled = true;
            this.textureImageFormatCB.Location = new System.Drawing.Point(148, 64);
            this.textureImageFormatCB.Name = "textureImageFormatCB";
            this.textureImageFormatCB.ReadOnly = true;
            this.textureImageFormatCB.Size = new System.Drawing.Size(236, 21);
            this.textureImageFormatCB.TabIndex = 12;
            // 
            // textureFileFormatCB
            // 
            this.textureFileFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.textureFileFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.textureFileFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.textureFileFormatCB.FormattingEnabled = true;
            this.textureFileFormatCB.Location = new System.Drawing.Point(147, 38);
            this.textureFileFormatCB.Name = "textureFileFormatCB";
            this.textureFileFormatCB.ReadOnly = true;
            this.textureFileFormatCB.Size = new System.Drawing.Size(236, 21);
            this.textureFileFormatCB.TabIndex = 14;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(20, 41);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(61, 13);
            this.stLabel2.TabIndex = 13;
            this.stLabel2.Text = "File Format:";
            // 
            // ImageProgramSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "ImageProgramSettings";
            this.Text = "ImageProgramSettings";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STLabel stLabel1;
        private STComboBox textureImageFormatCB;
        private STComboBox textureFileFormatCB;
        private STLabel stLabel2;
    }
}