namespace FirstPlugin.Forms
{
    partial class ShaderOptionsEditor
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
            this.chkFilterDefaults = new Toolbox.Library.Forms.STCheckBox();
            this.shaderOptionsListView = new Toolbox.Library.Forms.STListView();
            this.ovlColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ovlColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnScrolDown = new Toolbox.Library.Forms.STButton();
            this.btnScrollUp = new Toolbox.Library.Forms.STButton();
            this.btnEdit = new Toolbox.Library.Forms.STButton();
            this.btnRemove = new Toolbox.Library.Forms.STButton();
            this.btnAdd = new Toolbox.Library.Forms.STButton();
            ((System.ComponentModel.ISupportInitialize)(this.shaderOptionsListView)).BeginInit();
            this.SuspendLayout();
            // 
            // chkFilterDefaults
            // 
            this.chkFilterDefaults.AutoSize = true;
            this.chkFilterDefaults.Checked = true;
            this.chkFilterDefaults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFilterDefaults.Location = new System.Drawing.Point(165, 7);
            this.chkFilterDefaults.Name = "chkFilterDefaults";
            this.chkFilterDefaults.Size = new System.Drawing.Size(120, 17);
            this.chkFilterDefaults.TabIndex = 31;
            this.chkFilterDefaults.Text = "Filter Default Values";
            this.chkFilterDefaults.UseVisualStyleBackColor = true;
            this.chkFilterDefaults.CheckedChanged += new System.EventHandler(this.chkFilterDefaults_CheckedChanged);
            // 
            // shaderOptionsListView
            // 
            this.shaderOptionsListView.AllColumns.Add(this.ovlColumn1);
            this.shaderOptionsListView.AllColumns.Add(this.ovlColumn2);
            this.shaderOptionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shaderOptionsListView.CellEditUseWholeCell = false;
            this.shaderOptionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ovlColumn1,
            this.ovlColumn2});
            this.shaderOptionsListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.shaderOptionsListView.HideSelection = false;
            this.shaderOptionsListView.Location = new System.Drawing.Point(3, 32);
            this.shaderOptionsListView.Name = "shaderOptionsListView";
            this.shaderOptionsListView.ShowGroups = false;
            this.shaderOptionsListView.Size = new System.Drawing.Size(449, 400);
            this.shaderOptionsListView.TabIndex = 30;
            this.shaderOptionsListView.UseCompatibleStateImageBehavior = false;
            this.shaderOptionsListView.View = System.Windows.Forms.View.Details;
            this.shaderOptionsListView.SelectedIndexChanged += new System.EventHandler(this.shaderOptionsListView_SelectedIndexChanged);
            this.shaderOptionsListView.DoubleClick += new System.EventHandler(this.shaderOptionsListView_DoubleClick);
            // 
            // ovlColumn1
            // 
            this.ovlColumn1.AspectName = "Name";
            this.ovlColumn1.Text = "Name";
            this.ovlColumn1.Width = 170;
            // 
            // ovlColumn2
            // 
            this.ovlColumn2.AspectName = "Value";
            this.ovlColumn2.Text = "Value";
            this.ovlColumn2.Width = 385;
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
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(84, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 26;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
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
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // ShaderOptionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkFilterDefaults);
            this.Controls.Add(this.shaderOptionsListView);
            this.Controls.Add(this.btnScrolDown);
            this.Controls.Add(this.btnScrollUp);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Name = "ShaderOptionsEditor";
            this.Size = new System.Drawing.Size(496, 435);
            ((System.ComponentModel.ISupportInitialize)(this.shaderOptionsListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STButton btnRemove;
        private Toolbox.Library.Forms.STButton btnAdd;
        private Toolbox.Library.Forms.STButton btnEdit;
        private Toolbox.Library.Forms.STButton btnScrolDown;
        private Toolbox.Library.Forms.STButton btnScrollUp;
        private Toolbox.Library.Forms.STListView shaderOptionsListView;
        private BrightIdeasSoftware.OLVColumn ovlColumn1;
        private BrightIdeasSoftware.OLVColumn ovlColumn2;
        private Toolbox.Library.Forms.STCheckBox chkFilterDefaults;
    }
}
