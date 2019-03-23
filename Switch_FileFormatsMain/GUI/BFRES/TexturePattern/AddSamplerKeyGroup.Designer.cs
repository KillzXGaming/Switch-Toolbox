namespace FirstPlugin.Forms
{
    partial class AddSamplerKeyGroup
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
            this.samplerNameTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.btnEdit = new Switch_Toolbox.Library.Forms.STButton();
            this.btnCancel = new Switch_Toolbox.Library.Forms.STButton();
            this.btnOK = new Switch_Toolbox.Library.Forms.STButton();
            this.constantChkBox = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.constantChkBox);
            this.contentContainer.Controls.Add(this.btnOK);
            this.contentContainer.Controls.Add(this.btnCancel);
            this.contentContainer.Controls.Add(this.btnEdit);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.samplerNameTB);
            this.contentContainer.Size = new System.Drawing.Size(354, 124);
            this.contentContainer.Controls.SetChildIndex(this.samplerNameTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnEdit, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnCancel, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOK, 0);
            this.contentContainer.Controls.SetChildIndex(this.constantChkBox, 0);
            // 
            // samplerNameTB
            // 
            this.samplerNameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.samplerNameTB.Location = new System.Drawing.Point(62, 39);
            this.samplerNameTB.Name = "samplerNameTB";
            this.samplerNameTB.ReadOnly = true;
            this.samplerNameTB.Size = new System.Drawing.Size(205, 20);
            this.samplerNameTB.TabIndex = 0;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 41);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(48, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Sampler:";
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
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(273, 94);
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
            this.btnOK.Location = new System.Drawing.Point(187, 94);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // constantChkBox
            // 
            this.constantChkBox.AutoSize = true;
            this.constantChkBox.Location = new System.Drawing.Point(12, 77);
            this.constantChkBox.Name = "constantChkBox";
            this.constantChkBox.Size = new System.Drawing.Size(79, 17);
            this.constantChkBox.TabIndex = 11;
            this.constantChkBox.Text = "Is Constant";
            this.constantChkBox.UseVisualStyleBackColor = true;
            // 
            // AddSamplerKeyGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 129);
            this.Name = "AddSamplerKeyGroup";
            this.Text = "Add Texture Key Frame";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STTextBox samplerNameTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STButton btnEdit;
        private Switch_Toolbox.Library.Forms.STButton btnCancel;
        private Switch_Toolbox.Library.Forms.STButton btnOK;
        private Switch_Toolbox.Library.Forms.STCheckBox constantChkBox;
    }
}