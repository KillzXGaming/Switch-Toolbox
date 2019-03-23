namespace FirstPlugin.Forms
{
    partial class RenderInfoEditor
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
            this.renderInfoListView = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnEdit = new Switch_Toolbox.Library.Forms.STButton();
            this.btnScrolDown = new Switch_Toolbox.Library.Forms.STButton();
            this.btnScrollUp = new Switch_Toolbox.Library.Forms.STButton();
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
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemoveRenderInfo_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 25;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAddRenderInfo_Click);
            // 
            // renderInfoListView
            // 
            this.renderInfoListView.AllowColumnReorder = true;
            this.renderInfoListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.renderInfoListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11});
            this.renderInfoListView.Location = new System.Drawing.Point(3, 32);
            this.renderInfoListView.MultiSelect = false;
            this.renderInfoListView.Name = "renderInfoListView";
            this.renderInfoListView.OwnerDraw = true;
            this.renderInfoListView.Size = new System.Drawing.Size(452, 400);
            this.renderInfoListView.TabIndex = 22;
            this.renderInfoListView.UseCompatibleStateImageBehavior = false;
            this.renderInfoListView.View = System.Windows.Forms.View.Details;
            this.renderInfoListView.SelectedIndexChanged += new System.EventHandler(this.renderInfoListView_SelectedIndexChanged_1);
            this.renderInfoListView.DoubleClick += new System.EventHandler(this.renderInfoListView_DoubleClick);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Name";
            this.columnHeader10.Width = 267;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Value";
            this.columnHeader11.Width = 149;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Location = new System.Drawing.Point(377, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 27;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnScrolDown
            // 
            this.btnScrolDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrolDown.Enabled = false;
            this.btnScrolDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrolDown.Location = new System.Drawing.Point(461, 61);
            this.btnScrolDown.Name = "btnScrolDown";
            this.btnScrolDown.Size = new System.Drawing.Size(32, 24);
            this.btnScrolDown.TabIndex = 29;
            this.btnScrolDown.Text = "▼";
            this.btnScrolDown.UseVisualStyleBackColor = true;
            // 
            // btnScrollUp
            // 
            this.btnScrollUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrollUp.Enabled = false;
            this.btnScrollUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrollUp.Location = new System.Drawing.Point(461, 32);
            this.btnScrollUp.Name = "btnScrollUp";
            this.btnScrollUp.Size = new System.Drawing.Size(32, 24);
            this.btnScrollUp.TabIndex = 28;
            this.btnScrollUp.Text = "▲";
            this.btnScrollUp.UseVisualStyleBackColor = true;
            // 
            // RenderInfoEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnScrolDown);
            this.Controls.Add(this.btnScrollUp);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.renderInfoListView);
            this.Name = "RenderInfoEditor";
            this.Size = new System.Drawing.Size(496, 435);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STButton btnRemove;
        private Switch_Toolbox.Library.Forms.STButton btnAdd;
        private Switch_Toolbox.Library.Forms.ListViewCustom renderInfoListView;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private Switch_Toolbox.Library.Forms.STButton btnEdit;
        private Switch_Toolbox.Library.Forms.STButton btnScrolDown;
        private Switch_Toolbox.Library.Forms.STButton btnScrollUp;
    }
}
