namespace Switch_Toolbox.Library.Forms
{
    partial class ImageReEncodeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageReEncodeDialog));
            this.pictureBoxCustom1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.btnOk = new Switch_Toolbox.Library.Forms.STButton();
            this.btnCancel = new Switch_Toolbox.Library.Forms.STButton();
            this.mipcountUD = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel4 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel5 = new Switch_Toolbox.Library.Forms.STLabel();
            this.formatCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mipcountUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel5);
            this.contentContainer.Controls.Add(this.formatCB);
            this.contentContainer.Controls.Add(this.mipcountUD);
            this.contentContainer.Controls.Add(this.stLabel4);
            this.contentContainer.Controls.Add(this.btnCancel);
            this.contentContainer.Controls.Add(this.btnOk);
            this.contentContainer.Controls.Add(this.pictureBoxCustom1);
            this.contentContainer.Size = new System.Drawing.Size(729, 456);
            this.contentContainer.Controls.SetChildIndex(this.pictureBoxCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOk, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnCancel, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel4, 0);
            this.contentContainer.Controls.SetChildIndex(this.mipcountUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.formatCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel5, 0);
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
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
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
            // mipcountUD
            // 
            this.mipcountUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mipcountUD.Location = new System.Drawing.Point(626, 38);
            this.mipcountUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.mipcountUD.Name = "mipcountUD";
            this.mipcountUD.Size = new System.Drawing.Size(94, 20);
            this.mipcountUD.TabIndex = 22;
            this.mipcountUD.ValueChanged += new System.EventHandler(this.mipcountUD_ValueChanged);
            // 
            // stLabel4
            // 
            this.stLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(564, 40);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(58, 13);
            this.stLabel4.TabIndex = 21;
            this.stLabel4.Text = "Mip Count:";
            // 
            // stLabel5
            // 
            this.stLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(564, 73);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(42, 13);
            this.stLabel5.TabIndex = 24;
            this.stLabel5.Text = "Format:";
            // 
            // formatCB
            // 
            this.formatCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.formatCB.FormattingEnabled = true;
            this.formatCB.Location = new System.Drawing.Point(564, 89);
            this.formatCB.Name = "formatCB";
            this.formatCB.Size = new System.Drawing.Size(149, 21);
            this.formatCB.TabIndex = 23;
            this.formatCB.SelectedIndexChanged += new System.EventHandler(this.formatCB_SelectedIndexChanged);
            // 
            // ImageReEncodeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 461);
            this.Name = "ImageReEncodeDialog";
            this.Text = "ImageResizeDialog";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mipcountUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBoxCustom pictureBoxCustom1;
        private STButton btnCancel;
        private STButton btnOk;
        private STLabel stLabel5;
        private STComboBox formatCB;
        private NumericUpDownUint mipcountUD;
        private STLabel stLabel4;
    }
}