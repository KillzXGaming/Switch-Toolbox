namespace FirstPlugin.Forms
{
    partial class AddTextureKey
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
            this.textureNameTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.btnEdit = new Switch_Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.currentFrameCountUD = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.maxFrameCountUD = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.btnCancel = new Switch_Toolbox.Library.Forms.STButton();
            this.btnOK = new Switch_Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameCountUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxFrameCountUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.btnOK);
            this.contentContainer.Controls.Add(this.btnCancel);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.maxFrameCountUD);
            this.contentContainer.Controls.Add(this.currentFrameCountUD);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.btnEdit);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.textureNameTB);
            this.contentContainer.Size = new System.Drawing.Size(354, 157);
            this.contentContainer.Controls.SetChildIndex(this.textureNameTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnEdit, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.currentFrameCountUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.maxFrameCountUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnCancel, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOK, 0);
            // 
            // textureNameTB
            // 
            this.textureNameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textureNameTB.Location = new System.Drawing.Point(62, 39);
            this.textureNameTB.Name = "textureNameTB";
            this.textureNameTB.ReadOnly = true;
            this.textureNameTB.Size = new System.Drawing.Size(205, 20);
            this.textureNameTB.TabIndex = 0;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 41);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(46, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Texture:";
            // 
            // btnEdit
            // 
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Location = new System.Drawing.Point(273, 36);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(9, 81);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(52, 13);
            this.stLabel2.TabIndex = 4;
            this.stLabel2.Text = "At Frame:";
            // 
            // currentFrameCountUD
            // 
            this.currentFrameCountUD.Location = new System.Drawing.Point(62, 79);
            this.currentFrameCountUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.currentFrameCountUD.Name = "currentFrameCountUD";
            this.currentFrameCountUD.Size = new System.Drawing.Size(88, 20);
            this.currentFrameCountUD.TabIndex = 5;
            // 
            // maxFrameCountUD
            // 
            this.maxFrameCountUD.Location = new System.Drawing.Point(174, 79);
            this.maxFrameCountUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.maxFrameCountUD.Name = "maxFrameCountUD";
            this.maxFrameCountUD.Size = new System.Drawing.Size(88, 20);
            this.maxFrameCountUD.TabIndex = 7;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(156, 81);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(12, 13);
            this.stLabel3.TabIndex = 8;
            this.stLabel3.Text = "/";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(273, 125);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(187, 125);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // AddTextureKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 162);
            this.Name = "AddTextureKey";
            this.Text = "Edit Texture Key Frame";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameCountUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxFrameCountUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STTextBox textureNameTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STButton btnEdit;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.NumericUpDownUint currentFrameCountUD;
        private Switch_Toolbox.Library.Forms.NumericUpDownUint maxFrameCountUD;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private Switch_Toolbox.Library.Forms.STButton btnCancel;
        private Switch_Toolbox.Library.Forms.STButton btnOK;
    }
}