namespace LayoutBXLYT
{
    partial class AddGroupTargetDialog
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
            this.targetCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.curveTypeCB = new Toolbox.Library.Forms.STComboBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.curveTypeCB);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.targetCB);
            this.contentContainer.Size = new System.Drawing.Size(316, 143);
            this.contentContainer.Controls.SetChildIndex(this.targetCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.curveTypeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            // 
            // targetCB
            // 
            this.targetCB.BorderColor = System.Drawing.Color.Empty;
            this.targetCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.targetCB.ButtonColor = System.Drawing.Color.Empty;
            this.targetCB.FormattingEnabled = true;
            this.targetCB.IsReadOnly = false;
            this.targetCB.Location = new System.Drawing.Point(96, 35);
            this.targetCB.Name = "targetCB";
            this.targetCB.Size = new System.Drawing.Size(208, 21);
            this.targetCB.TabIndex = 11;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(20, 38);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(41, 13);
            this.stLabel1.TabIndex = 12;
            this.stLabel1.Text = "Target:";
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(232, 111);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 13;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(20, 65);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(68, 13);
            this.stLabel2.TabIndex = 15;
            this.stLabel2.Text = "Curve Type::";
            // 
            // curveTypeCB
            // 
            this.curveTypeCB.BorderColor = System.Drawing.Color.Empty;
            this.curveTypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.curveTypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.curveTypeCB.FormattingEnabled = true;
            this.curveTypeCB.IsReadOnly = false;
            this.curveTypeCB.Location = new System.Drawing.Point(96, 62);
            this.curveTypeCB.Name = "curveTypeCB";
            this.curveTypeCB.Size = new System.Drawing.Size(208, 21);
            this.curveTypeCB.TabIndex = 14;
            // 
            // AddGroupTargetDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 148);
            this.Name = "AddGroupTargetDialog";
            this.Text = "Add Target";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STComboBox targetCB;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox curveTypeCB;
        private Toolbox.Library.Forms.STButton stButton1;
    }
}