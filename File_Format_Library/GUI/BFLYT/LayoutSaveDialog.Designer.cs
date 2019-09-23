namespace LayoutBXLYT
{
    partial class LayoutSaveDialog
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
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.saveArchiveRadioBtn = new System.Windows.Forms.RadioButton();
            this.saveNewArchiveRadioBtn = new System.Windows.Forms.RadioButton();
            this.saveFolderRadioBtn = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.radioButton6);
            this.contentContainer.Controls.Add(this.radioButton5);
            this.contentContainer.Controls.Add(this.radioButton4);
            this.contentContainer.Controls.Add(this.saveFolderRadioBtn);
            this.contentContainer.Controls.Add(this.saveNewArchiveRadioBtn);
            this.contentContainer.Controls.Add(this.saveArchiveRadioBtn);
            this.contentContainer.Controls.Add(this.treeView1);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Size = new System.Drawing.Size(495, 427);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.treeView1, 0);
            this.contentContainer.Controls.SetChildIndex(this.saveArchiveRadioBtn, 0);
            this.contentContainer.Controls.SetChildIndex(this.saveNewArchiveRadioBtn, 0);
            this.contentContainer.Controls.SetChildIndex(this.saveFolderRadioBtn, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton4, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton5, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton6, 0);
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(411, 395);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 0;
            this.stButton1.Text = "Save";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(3, 31);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(354, 361);
            this.treeView1.TabIndex = 1;
            // 
            // saveArchiveRadioBtn
            // 
            this.saveArchiveRadioBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveArchiveRadioBtn.AutoSize = true;
            this.saveArchiveRadioBtn.Checked = true;
            this.saveArchiveRadioBtn.Location = new System.Drawing.Point(363, 37);
            this.saveArchiveRadioBtn.Name = "saveArchiveRadioBtn";
            this.saveArchiveRadioBtn.Size = new System.Drawing.Size(105, 17);
            this.saveArchiveRadioBtn.TabIndex = 2;
            this.saveArchiveRadioBtn.TabStop = true;
            this.saveArchiveRadioBtn.Text = "Save To Archive";
            this.saveArchiveRadioBtn.UseVisualStyleBackColor = true;
            // 
            // saveNewArchiveRadioBtn
            // 
            this.saveNewArchiveRadioBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveNewArchiveRadioBtn.AutoSize = true;
            this.saveNewArchiveRadioBtn.Location = new System.Drawing.Point(363, 60);
            this.saveNewArchiveRadioBtn.Name = "saveNewArchiveRadioBtn";
            this.saveNewArchiveRadioBtn.Size = new System.Drawing.Size(129, 17);
            this.saveNewArchiveRadioBtn.TabIndex = 3;
            this.saveNewArchiveRadioBtn.Text = "Save As New Archive";
            this.saveNewArchiveRadioBtn.UseVisualStyleBackColor = true;
            // 
            // saveFolderRadioBtn
            // 
            this.saveFolderRadioBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveFolderRadioBtn.AutoSize = true;
            this.saveFolderRadioBtn.Location = new System.Drawing.Point(363, 83);
            this.saveFolderRadioBtn.Name = "saveFolderRadioBtn";
            this.saveFolderRadioBtn.Size = new System.Drawing.Size(119, 17);
            this.saveFolderRadioBtn.TabIndex = 4;
            this.saveFolderRadioBtn.Text = "Save To File/Folder";
            this.saveFolderRadioBtn.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton4.AutoSize = true;
            this.radioButton4.Enabled = false;
            this.radioButton4.Location = new System.Drawing.Point(363, 203);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(90, 17);
            this.radioButton4.TabIndex = 5;
            this.radioButton4.Text = "Save As XML";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            this.radioButton5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton5.AutoSize = true;
            this.radioButton5.Enabled = false;
            this.radioButton5.Location = new System.Drawing.Point(363, 226);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(97, 17);
            this.radioButton5.TabIndex = 6;
            this.radioButton5.Text = "Save As YAML";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            this.radioButton6.AllowDrop = true;
            this.radioButton6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButton6.AutoCheck = false;
            this.radioButton6.AutoSize = true;
            this.radioButton6.Checked = true;
            this.radioButton6.Location = new System.Drawing.Point(363, 180);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(97, 17);
            this.radioButton6.TabIndex = 11;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "Save As Binary";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // LayoutSaveDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 432);
            this.Name = "LayoutSaveDialog";
            this.Text = "Layout Save Dialog";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STButton stButton1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.RadioButton saveArchiveRadioBtn;
        private System.Windows.Forms.RadioButton saveNewArchiveRadioBtn;
        private System.Windows.Forms.RadioButton saveFolderRadioBtn;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton6;
    }
}