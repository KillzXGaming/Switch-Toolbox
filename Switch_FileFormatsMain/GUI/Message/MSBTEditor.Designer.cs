namespace FirstPlugin.Forms
{
    partial class MSBTEditor
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
            this.textEditor1 = new Switch_Toolbox.Library.Forms.TextEditor();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.contentContainer.Controls.Add(this.textEditor1);
            this.contentContainer.Controls.SetChildIndex(this.textEditor1, 0);
            this.contentContainer.Controls.SetChildIndex(this.listViewCustom1, 0);
            // 
            // textEditor1
            // 
            this.textEditor1.IsXML = false;
            this.textEditor1.Location = new System.Drawing.Point(204, 31);
            this.textEditor1.Name = "textEditor1";
            this.textEditor1.Size = new System.Drawing.Size(336, 359);
            this.textEditor1.TabIndex = 11;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.Location = new System.Drawing.Point(3, 31);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(195, 359);
            this.listViewCustom1.TabIndex = 12;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 177;
            // 
            // MSBTEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "MSBTEditor";
            this.Text = "MSBTEditor";
            this.contentContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.TextEditor textEditor1;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}