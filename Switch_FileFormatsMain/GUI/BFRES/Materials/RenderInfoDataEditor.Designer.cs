namespace FirstPlugin
{
    partial class RenderInfoDataEditor
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
            this.nameTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.typeCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.valueTB = new System.Windows.Forms.RichTextBox();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.btnCancel = new Switch_Toolbox.Library.Forms.STButton();
            this.btnOk = new Switch_Toolbox.Library.Forms.STButton();
            this.stLabel4 = new Switch_Toolbox.Library.Forms.STLabel();
            this.presetCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel4);
            this.contentContainer.Controls.Add(this.presetCB);
            this.contentContainer.Controls.Add(this.btnOk);
            this.contentContainer.Controls.Add(this.btnCancel);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.valueTB);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.typeCB);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.nameTB);
            this.contentContainer.Size = new System.Drawing.Size(314, 411);
            this.contentContainer.Controls.SetChildIndex(this.nameTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.typeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.valueTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnCancel, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOk, 0);
            this.contentContainer.Controls.SetChildIndex(this.presetCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel4, 0);
            // 
            // nameTB
            // 
            this.nameTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTB.Location = new System.Drawing.Point(62, 36);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(243, 20);
            this.nameTB.TabIndex = 11;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 39);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(38, 13);
            this.stLabel1.TabIndex = 12;
            this.stLabel1.Text = "Name:";
            // 
            // typeCB
            // 
            this.typeCB.DropDownStyle = Switch_Toolbox.Library.Forms.STComboBox.STDropDownStyle;
            this.typeCB.FormattingEnabled = true;
            this.typeCB.Location = new System.Drawing.Point(62, 88);
            this.typeCB.Name = "typeCB";
            this.typeCB.Size = new System.Drawing.Size(136, 21);
            this.typeCB.TabIndex = 13;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(9, 91);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(34, 13);
            this.stLabel2.TabIndex = 14;
            this.stLabel2.Text = "Type:";
            // 
            // valueTB
            // 
            this.valueTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTB.BackColor = System.Drawing.Color.White;
            this.valueTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueTB.Location = new System.Drawing.Point(12, 132);
            this.valueTB.Name = "valueTB";
            this.valueTB.Size = new System.Drawing.Size(293, 241);
            this.valueTB.TabIndex = 15;
            this.valueTB.Text = "";
            this.valueTB.TextChanged += new System.EventHandler(this.valueTB_TextChanged);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(9, 116);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(134, 13);
            this.stLabel3.TabIndex = 16;
            this.stLabel3.Text = "Values: (Enter one per line)";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(229, 379);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(148, 379);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 18;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(9, 65);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(40, 13);
            this.stLabel4.TabIndex = 20;
            this.stLabel4.Text = "Preset:";
            // 
            // presetCB
            // 
            this.presetCB.DropDownStyle = Switch_Toolbox.Library.Forms.STComboBox.STDropDownStyle;;
            this.presetCB.FormattingEnabled = true;
            this.presetCB.Location = new System.Drawing.Point(62, 62);
            this.presetCB.Name = "presetCB";
            this.presetCB.Size = new System.Drawing.Size(136, 21);
            this.presetCB.TabIndex = 19;
            this.presetCB.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // RenderInfoDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 416);
            this.Name = "RenderInfoDataEditor";
            this.Text = "Render Info";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STTextBox nameTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private System.Windows.Forms.RichTextBox valueTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STComboBox typeCB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STButton btnOk;
        private Switch_Toolbox.Library.Forms.STButton btnCancel;
        private Switch_Toolbox.Library.Forms.STLabel stLabel4;
        private Switch_Toolbox.Library.Forms.STComboBox presetCB;
    }
}