namespace Switch_Toolbox.Library.Forms
{
    partial class ImageResizeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageResizeDialog));
            this.pictureBoxCustom1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.btnOk = new Switch_Toolbox.Library.Forms.STButton();
            this.btnCancel = new Switch_Toolbox.Library.Forms.STButton();
            this.widthUD = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.chkKeepAspectRatio = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.resampleCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.heightUD = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.heightUD);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.resampleCB);
            this.contentContainer.Controls.Add(this.chkKeepAspectRatio);
            this.contentContainer.Controls.Add(this.widthUD);
            this.contentContainer.Controls.Add(this.btnCancel);
            this.contentContainer.Controls.Add(this.btnOk);
            this.contentContainer.Controls.Add(this.pictureBoxCustom1);
            this.contentContainer.Size = new System.Drawing.Size(729, 456);
            this.contentContainer.Controls.SetChildIndex(this.pictureBoxCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOk, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnCancel, 0);
            this.contentContainer.Controls.SetChildIndex(this.widthUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkKeepAspectRatio, 0);
            this.contentContainer.Controls.SetChildIndex(this.resampleCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.heightUD, 0);
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(0, 31);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(558, 416);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 11;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(564, 424);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(645, 424);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // widthUD
            // 
            this.widthUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.widthUD.Location = new System.Drawing.Point(604, 112);
            this.widthUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.widthUD.Name = "widthUD";
            this.widthUD.Size = new System.Drawing.Size(116, 20);
            this.widthUD.TabIndex = 14;
            this.widthUD.ValueChanged += new System.EventHandler(this.widthUD_ValueChanged);
            // 
            // chkKeepAspectRatio
            // 
            this.chkKeepAspectRatio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkKeepAspectRatio.AutoSize = true;
            this.chkKeepAspectRatio.Location = new System.Drawing.Point(564, 89);
            this.chkKeepAspectRatio.Name = "chkKeepAspectRatio";
            this.chkKeepAspectRatio.Size = new System.Drawing.Size(109, 17);
            this.chkKeepAspectRatio.TabIndex = 15;
            this.chkKeepAspectRatio.Text = "Keep aspect ratio";
            this.chkKeepAspectRatio.UseVisualStyleBackColor = true;
            // 
            // resampleCB
            // 
            this.resampleCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resampleCB.FormattingEnabled = true;
            this.resampleCB.Location = new System.Drawing.Point(564, 52);
            this.resampleCB.Name = "resampleCB";
            this.resampleCB.Size = new System.Drawing.Size(149, 21);
            this.resampleCB.TabIndex = 16;
            this.resampleCB.SelectedIndexChanged += new System.EventHandler(this.resampleCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(564, 36);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(65, 13);
            this.stLabel1.TabIndex = 17;
            this.stLabel1.Text = "Resampling:";
            // 
            // stLabel2
            // 
            this.stLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(564, 114);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(38, 13);
            this.stLabel2.TabIndex = 18;
            this.stLabel2.Text = "Width:";
            // 
            // stLabel3
            // 
            this.stLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(564, 140);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(41, 13);
            this.stLabel3.TabIndex = 19;
            this.stLabel3.Text = "Height:";
            // 
            // heightUD
            // 
            this.heightUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.heightUD.Location = new System.Drawing.Point(604, 138);
            this.heightUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.heightUD.Name = "heightUD";
            this.heightUD.Size = new System.Drawing.Size(116, 20);
            this.heightUD.TabIndex = 20;
            this.heightUD.ValueChanged += new System.EventHandler(this.heightUD_ValueChanged);
            // 
            // ImageResizeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 461);
            this.Name = "ImageResizeDialog";
            this.Text = "ImageResizeDialog";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBoxCustom pictureBoxCustom1;
        private NumericUpDownUint heightUD;
        private STLabel stLabel3;
        private STLabel stLabel2;
        private STLabel stLabel1;
        private STComboBox resampleCB;
        private Switch_Toolbox.Library.Forms.STCheckBox chkKeepAspectRatio;
        private NumericUpDownUint widthUD;
        private STButton btnCancel;
        private STButton btnOk;
    }
}