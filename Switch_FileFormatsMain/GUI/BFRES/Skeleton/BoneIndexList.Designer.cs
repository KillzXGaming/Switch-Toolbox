namespace FirstPlugin.Forms
{
    partial class BoneIndexList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.contentContainer.Location = new System.Drawing.Point(3, 1);
            this.contentContainer.Size = new System.Drawing.Size(351, 395);
            this.contentContainer.Controls.SetChildIndex(this.listViewCustom1, 0);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3});
            this.listViewCustom1.FullRowSelect = true;
            this.listViewCustom1.Location = new System.Drawing.Point(6, 31);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(342, 361);
            this.listViewCustom1.TabIndex = 7;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 198;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Index";
            this.columnHeader3.Width = 144;
            // 
            // BoneIndexList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 398);
            this.Name = "BoneIndexList";
            this.contentContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
