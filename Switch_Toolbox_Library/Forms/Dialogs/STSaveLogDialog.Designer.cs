namespace Switch_Toolbox.Library.Forms
{
    partial class STSaveLogDialog
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
            this.btnDetails = new Switch_Toolbox.Library.Forms.STButton();
            this.lblMessage = new Switch_Toolbox.Library.Forms.STLabel();
            this.tbDetails = new System.Windows.Forms.RichTextBox();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.SuspendLayout();
            // 
            // btnDetails
            // 
            this.btnDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetails.Enabled = false;
            this.btnDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetails.Location = new System.Drawing.Point(12, 78);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(75, 23);
            this.btnDetails.TabIndex = 1;
            this.btnDetails.Text = "Details";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(12, 19);
            this.lblMessage.MaximumSize = new System.Drawing.Size(310, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(35, 13);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "label1";
            // 
            // tbDetails
            // 
            this.tbDetails.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tbDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDetails.Location = new System.Drawing.Point(-42, 107);
            this.tbDetails.MaximumSize = new System.Drawing.Size(328, 100);
            this.tbDetails.Name = "tbDetails";
            this.tbDetails.ReadOnly = true;
            this.tbDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.tbDetails.Size = new System.Drawing.Size(328, 100);
            this.tbDetails.TabIndex = 6;
            this.tbDetails.Text = "";
            this.tbDetails.Visible = false;
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(160, 78);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 8;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = true;
            // 
            // STSaveLogDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(247, 111);
            this.Controls.Add(this.stButton1);
            this.Controls.Add(this.tbDetails);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "STSaveLogDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Save Notification";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Switch_Toolbox.Library.Forms.STButton btnDetails;
        private Switch_Toolbox.Library.Forms.STLabel lblMessage;
        private System.Windows.Forms.RichTextBox tbDetails;
        private STButton stButton1;
    }
}