namespace Toolbox.Library.Forms
{
    partial class ExportModelSettings
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
            this.exportTexturesChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.chkFlipUvsVertical = new Toolbox.Library.Forms.STCheckBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.chkFlipUvsVertical);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.exportTexturesChkBox);
            this.contentContainer.Size = new System.Drawing.Size(329, 173);
            this.contentContainer.Controls.SetChildIndex(this.exportTexturesChkBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkFlipUvsVertical, 0);
            // 
            // exportTexturesChkBox
            // 
            this.exportTexturesChkBox.AutoSize = true;
            this.exportTexturesChkBox.Checked = true;
            this.exportTexturesChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportTexturesChkBox.Location = new System.Drawing.Point(23, 47);
            this.exportTexturesChkBox.Name = "exportTexturesChkBox";
            this.exportTexturesChkBox.Size = new System.Drawing.Size(100, 17);
            this.exportTexturesChkBox.TabIndex = 11;
            this.exportTexturesChkBox.Text = "Export Textures";
            this.exportTexturesChkBox.UseVisualStyleBackColor = true;
            this.exportTexturesChkBox.CheckedChanged += new System.EventHandler(this.exportTexturesChkBox_CheckedChanged);
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(244, 141);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 12;
            this.stButton1.Text = "Cancel";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(163, 141);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 13;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // chkFlipUvsVertical
            // 
            this.chkFlipUvsVertical.AutoSize = true;
            this.chkFlipUvsVertical.Location = new System.Drawing.Point(23, 70);
            this.chkFlipUvsVertical.Name = "chkFlipUvsVertical";
            this.chkFlipUvsVertical.Size = new System.Drawing.Size(101, 17);
            this.chkFlipUvsVertical.TabIndex = 14;
            this.chkFlipUvsVertical.Text = "Flp UVs Vertical";
            this.chkFlipUvsVertical.UseVisualStyleBackColor = true;
            this.chkFlipUvsVertical.CheckedChanged += new System.EventHandler(this.chkFlipUvsVertical_CheckedChanged);
            // 
            // ExportModelSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 178);
            this.Name = "ExportModelSettings";
            this.Text = "Export Settings";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STCheckBox exportTexturesChkBox;
        private STButton stButton2;
        private STButton stButton1;
        private STCheckBox chkFlipUvsVertical;
    }
}