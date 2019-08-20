namespace FirstPlugin.Forms
{
    partial class EmitterTexturePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmitterTexturePanel));
            this.pictureBoxCustom1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.pictureBoxCustom2 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.pictureBoxCustom3 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.showAlphaChk = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(3, 26);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // pictureBoxCustom2
            // 
            this.pictureBoxCustom2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom2.BackgroundImage")));
            this.pictureBoxCustom2.Location = new System.Drawing.Point(79, 26);
            this.pictureBoxCustom2.Name = "pictureBoxCustom2";
            this.pictureBoxCustom2.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxCustom2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom2.TabIndex = 1;
            this.pictureBoxCustom2.TabStop = false;
            // 
            // pictureBoxCustom3
            // 
            this.pictureBoxCustom3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom3.BackgroundImage")));
            this.pictureBoxCustom3.Location = new System.Drawing.Point(155, 26);
            this.pictureBoxCustom3.Name = "pictureBoxCustom3";
            this.pictureBoxCustom3.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxCustom3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom3.TabIndex = 2;
            this.pictureBoxCustom3.TabStop = false;
            // 
            // showAlphaChk
            // 
            this.showAlphaChk.AutoSize = true;
            this.showAlphaChk.Location = new System.Drawing.Point(142, 3);
            this.showAlphaChk.Name = "showAlphaChk";
            this.showAlphaChk.Size = new System.Drawing.Size(83, 17);
            this.showAlphaChk.TabIndex = 3;
            this.showAlphaChk.Text = "Show Alpha";
            this.showAlphaChk.UseVisualStyleBackColor = true;
            this.showAlphaChk.CheckedChanged += new System.EventHandler(this.showAlphaChk_CheckedChanged);
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(3, 4);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(48, 13);
            this.stLabel6.TabIndex = 48;
            this.stLabel6.Text = "Textures";
            // 
            // EmitterTexturePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stLabel6);
            this.Controls.Add(this.showAlphaChk);
            this.Controls.Add(this.pictureBoxCustom3);
            this.Controls.Add(this.pictureBoxCustom2);
            this.Controls.Add(this.pictureBoxCustom1);
            this.Name = "EmitterTexturePanel";
            this.Size = new System.Drawing.Size(233, 102);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom2;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom3;
        private Toolbox.Library.Forms.STCheckBox showAlphaChk;
        private Toolbox.Library.Forms.STLabel stLabel6;
    }
}
