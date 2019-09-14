namespace FirstPlugin.Forms
{
    partial class EmitterTextureEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmitterTextureEditor));
            this.pictureBoxCustom1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.textureIdTextBox = new Toolbox.Library.Forms.STTextBox();
            this.enabledCheckBox = new Toolbox.Library.Forms.STCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(3, 52);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // textureIdTextBox
            // 
            this.textureIdTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textureIdTextBox.Location = new System.Drawing.Point(3, 26);
            this.textureIdTextBox.MaxLength = 8;
            this.textureIdTextBox.Name = "textureIdTextBox";
            this.textureIdTextBox.Size = new System.Drawing.Size(70, 20);
            this.textureIdTextBox.TabIndex = 1;
            this.textureIdTextBox.TextChanged += new System.EventHandler(this.TextureIdTextBox_TextChanged);
            // 
            // enabledCheckBox
            // 
            this.enabledCheckBox.AutoSize = true;
            this.enabledCheckBox.Location = new System.Drawing.Point(3, 3);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.Size = new System.Drawing.Size(65, 17);
            this.enabledCheckBox.TabIndex = 2;
            this.enabledCheckBox.Text = "Enabled";
            this.enabledCheckBox.UseVisualStyleBackColor = true;
            this.enabledCheckBox.CheckedChanged += new System.EventHandler(this.EnabledCheckBox_CheckedChanged);
            // 
            // EmitterTextureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.enabledCheckBox);
            this.Controls.Add(this.textureIdTextBox);
            this.Controls.Add(this.pictureBoxCustom1);
            this.Name = "EmitterTextureEditor";
            this.Size = new System.Drawing.Size(76, 126);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private Toolbox.Library.Forms.STTextBox textureIdTextBox;
        private Toolbox.Library.Forms.STCheckBox enabledCheckBox;
    }
}
