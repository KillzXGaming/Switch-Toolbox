namespace Toolbox
{
    partial class FileAssociationForm
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
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkAll = new Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.chkAll);
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.contentContainer.Size = new System.Drawing.Size(289, 349);
            this.contentContainer.Controls.SetChildIndex(this.listViewCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkAll, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewCustom1.CheckBoxes = true;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewCustom1.Location = new System.Drawing.Point(9, 52);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(272, 264);
            this.listViewCustom1.TabIndex = 11;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Defeault Extensions";
            this.columnHeader1.Width = 246;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(210, 31);
            this.chkAll.Name = "chkAll";
            this.chkAll.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkAll.Size = new System.Drawing.Size(71, 17);
            this.chkAll.TabIndex = 12;
            this.chkAll.Text = "Check All";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(125, 322);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 13;
            this.stButton1.Text = "Save";
            this.stButton1.UseVisualStyleBackColor = false;
            this.stButton1.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(206, 322);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 14;
            this.stButton2.Text = "Cancel";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "";
            // 
            // FileAssociationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 354);
            this.Name = "FileAssociationForm";
            this.Text = "File Association";
            this.Load += new System.EventHandler(this.FileAssociationForm_Load);
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STButton stButton2;
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STCheckBox chkAll;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}