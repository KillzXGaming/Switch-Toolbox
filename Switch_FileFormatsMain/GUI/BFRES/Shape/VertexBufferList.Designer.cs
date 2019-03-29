namespace FirstPlugin.Forms
{
    partial class VertexBufferList
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
            this.btnRemove = new Switch_Toolbox.Library.Forms.STButton();
            this.btnAdd = new Switch_Toolbox.Library.Forms.STButton();
            this.btnScrolDown = new Switch_Toolbox.Library.Forms.STButton();
            this.btnScrollUp = new Switch_Toolbox.Library.Forms.STButton();
            this.attributeListView = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnView = new Switch_Toolbox.Library.Forms.STButton();
            this.btnEdit = new Switch_Toolbox.Library.Forms.STButton();
            this.SuspendLayout();
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(84, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 26;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 25;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnScrolDown
            // 
            this.btnScrolDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrolDown.Enabled = false;
            this.btnScrolDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrolDown.Location = new System.Drawing.Point(360, 62);
            this.btnScrolDown.Name = "btnScrolDown";
            this.btnScrolDown.Size = new System.Drawing.Size(32, 24);
            this.btnScrolDown.TabIndex = 24;
            this.btnScrolDown.Text = "▼";
            this.btnScrolDown.UseVisualStyleBackColor = true;
            // 
            // btnScrollUp
            // 
            this.btnScrollUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrollUp.Enabled = false;
            this.btnScrollUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrollUp.Location = new System.Drawing.Point(360, 32);
            this.btnScrollUp.Name = "btnScrollUp";
            this.btnScrollUp.Size = new System.Drawing.Size(32, 24);
            this.btnScrollUp.TabIndex = 23;
            this.btnScrollUp.Text = "▲";
            this.btnScrollUp.UseVisualStyleBackColor = true;
            // 
            // attributeListView
            // 
            this.attributeListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.attributeListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.attributeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.attributeListView.Location = new System.Drawing.Point(3, 32);
            this.attributeListView.Name = "attributeListView";
            this.attributeListView.OwnerDraw = true;
            this.attributeListView.Size = new System.Drawing.Size(351, 305);
            this.attributeListView.TabIndex = 22;
            this.attributeListView.UseCompatibleStateImageBehavior = false;
            this.attributeListView.View = System.Windows.Forms.View.Details;
            this.attributeListView.SelectedIndexChanged += new System.EventHandler(this.attributeListView_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 85;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Hint";
            this.columnHeader3.Width = 89;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Format";
            this.columnHeader4.Width = 177;
            // 
            // btnView
            // 
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView.Location = new System.Drawing.Point(165, 3);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 27;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.stbtnView_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Location = new System.Drawing.Point(246, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 28;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // VertexBufferList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnScrolDown);
            this.Controls.Add(this.btnScrollUp);
            this.Controls.Add(this.attributeListView);
            this.Name = "VertexBufferList";
            this.Size = new System.Drawing.Size(400, 346);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STButton btnRemove;
        private Switch_Toolbox.Library.Forms.STButton btnAdd;
        private Switch_Toolbox.Library.Forms.STButton btnScrolDown;
        private Switch_Toolbox.Library.Forms.STButton btnScrollUp;
        private Switch_Toolbox.Library.Forms.ListViewCustom attributeListView;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private Switch_Toolbox.Library.Forms.STButton btnView;
        private Switch_Toolbox.Library.Forms.STButton btnEdit;
    }
}
