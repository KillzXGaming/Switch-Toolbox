namespace FirstPlugin.Forms
{
    partial class FrameTextureThumbnail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameTextureThumbnail));
            this.pictureBoxCustom1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.textureNameLbl = new Toolbox.Library.Forms.STLabel();
            this.frameCounterLbl = new Toolbox.Library.Forms.STLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Empty;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(173, 133);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.TabStop = false;
            this.pictureBoxCustom1.Click += new System.EventHandler(this.pictureBoxCustom1_Click);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.textureNameLbl);
            this.stPanel1.Controls.Add(this.frameCounterLbl);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel1.Location = new System.Drawing.Point(0, 133);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(173, 40);
            this.stPanel1.TabIndex = 1;
            this.stPanel1.Click += new System.EventHandler(this.stPanel1_Click);
            this.stPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.stPanel1_Paint);
            // 
            // textureNameLbl
            // 
            this.textureNameLbl.AutoSize = true;
            this.textureNameLbl.Location = new System.Drawing.Point(14, 19);
            this.textureNameLbl.Name = "textureNameLbl";
            this.textureNameLbl.Size = new System.Drawing.Size(88, 13);
            this.textureNameLbl.TabIndex = 1;
            this.textureNameLbl.Text = "Name: Basic_Alb";
            // 
            // frameCounterLbl
            // 
            this.frameCounterLbl.AutoSize = true;
            this.frameCounterLbl.Location = new System.Drawing.Point(13, 3);
            this.frameCounterLbl.Name = "frameCounterLbl";
            this.frameCounterLbl.Size = new System.Drawing.Size(77, 13);
            this.frameCounterLbl.TabIndex = 0;
            this.frameCounterLbl.Text = "Frame: 00 / 00";
            this.frameCounterLbl.Resize += new System.EventHandler(this.frameCounterLbl_Resize);
            // 
            // FrameTextureThumbnail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.pictureBoxCustom1);
            this.Name = "FrameTextureThumbnail";
            this.Size = new System.Drawing.Size(173, 173);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STLabel textureNameLbl;
        private Toolbox.Library.Forms.STLabel frameCounterLbl;
    }
}
