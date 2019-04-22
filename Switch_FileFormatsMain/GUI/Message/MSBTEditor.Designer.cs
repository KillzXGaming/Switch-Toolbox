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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textEditor1 = new Switch_Toolbox.Library.Forms.TextEditor();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.splitContainer1);
            this.contentContainer.Controls.SetChildIndex(this.splitContainer1, 0);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewCustom1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textEditor1);
            this.splitContainer1.Size = new System.Drawing.Size(543, 368);
            this.splitContainer1.SplitterDistance = 181;
            this.splitContainer1.TabIndex = 11;
            // 
            // textEditor1
            // 
            this.textEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor1.IsXML = false;
            this.textEditor1.Location = new System.Drawing.Point(0, 0);
            this.textEditor1.Name = "textEditor1";
            this.textEditor1.Size = new System.Drawing.Size(358, 368);
            this.textEditor1.TabIndex = 0;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(181, 368);
            this.listViewCustom1.TabIndex = 0;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 181;
            // 
            // MSBTEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "MSBTEditor";
            this.Text = "MSBTEditor";
            this.contentContainer.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Switch_Toolbox.Library.Forms.TextEditor textEditor1;
    }
}