namespace FirstPlugin.Forms
{
    partial class BfresAnimationCopy
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
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.chkUserData = new Toolbox.Library.Forms.STCheckBox();
            this.chkBoneAnims = new Toolbox.Library.Forms.STCheckBox();
            this.chkAnimSettings = new Toolbox.Library.Forms.STCheckBox();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.btnOk = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.btnOk);
            this.contentContainer.Controls.Add(this.stPanel3);
            this.contentContainer.Size = new System.Drawing.Size(543, 388);
            this.contentContainer.Controls.SetChildIndex(this.stPanel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOk, 0);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.chkUserData);
            this.stPanel1.Controls.Add(this.chkBoneAnims);
            this.stPanel1.Controls.Add(this.chkAnimSettings);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(180, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(349, 319);
            this.stPanel1.TabIndex = 1;
            // 
            // chkUserData
            // 
            this.chkUserData.AutoSize = true;
            this.chkUserData.Location = new System.Drawing.Point(20, 58);
            this.chkUserData.Name = "chkUserData";
            this.chkUserData.Size = new System.Drawing.Size(74, 17);
            this.chkUserData.TabIndex = 2;
            this.chkUserData.Text = "User Data";
            this.chkUserData.UseVisualStyleBackColor = true;
            // 
            // chkBoneAnims
            // 
            this.chkBoneAnims.AutoSize = true;
            this.chkBoneAnims.Location = new System.Drawing.Point(20, 35);
            this.chkBoneAnims.Name = "chkBoneAnims";
            this.chkBoneAnims.Size = new System.Drawing.Size(105, 17);
            this.chkBoneAnims.TabIndex = 1;
            this.chkBoneAnims.Text = "Bone Animations";
            this.chkBoneAnims.UseVisualStyleBackColor = true;
            // 
            // chkAnimSettings
            // 
            this.chkAnimSettings.AutoSize = true;
            this.chkAnimSettings.Location = new System.Drawing.Point(20, 12);
            this.chkAnimSettings.Name = "chkAnimSettings";
            this.chkAnimSettings.Size = new System.Drawing.Size(113, 17);
            this.chkAnimSettings.TabIndex = 0;
            this.chkAnimSettings.Text = "Animation Settings";
            this.chkAnimSettings.UseVisualStyleBackColor = true;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.listViewCustom1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(180, 319);
            this.stPanel2.TabIndex = 2;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.CheckBoxes = true;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCustom1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom1.HideSelection = false;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(180, 319);
            this.listViewCustom1.TabIndex = 0;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 179;
            // 
            // stPanel3
            // 
            this.stPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel3.Controls.Add(this.stPanel1);
            this.stPanel3.Controls.Add(this.stPanel2);
            this.stPanel3.Location = new System.Drawing.Point(5, 31);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(529, 319);
            this.stPanel3.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(459, 361);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // BfresAnimationCopy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 393);
            this.Name = "BfresAnimationCopy";
            this.Text = "Animation Copy";
            this.contentContainer.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stPanel2.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Toolbox.Library.Forms.STCheckBox chkUserData;
        private Toolbox.Library.Forms.STCheckBox chkBoneAnims;
        private Toolbox.Library.Forms.STCheckBox chkAnimSettings;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private Toolbox.Library.Forms.STButton btnOk;
    }
}