namespace FirstPlugin.Forms
{
    partial class ShaderParamEditor
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
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.btnExport = new Toolbox.Library.Forms.STButton();
            this.btnImport = new Toolbox.Library.Forms.STButton();
            this.btnScrolDown = new Toolbox.Library.Forms.STButton();
            this.btnScrollUp = new Toolbox.Library.Forms.STButton();
            this.shaderParamListView = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(3, 3);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(57, 23);
            this.stButton1.TabIndex = 1;
            this.stButton1.Text = "Add";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(66, 3);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(62, 23);
            this.stButton2.TabIndex = 2;
            this.stButton2.Text = "Remove";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // btnExport
            // 
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Location = new System.Drawing.Point(183, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Location = new System.Drawing.Point(264, 3);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnScrolDown
            // 
            this.btnScrolDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrolDown.Enabled = false;
            this.btnScrolDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrolDown.Location = new System.Drawing.Point(345, 64);
            this.btnScrolDown.Name = "btnScrolDown";
            this.btnScrolDown.Size = new System.Drawing.Size(32, 24);
            this.btnScrolDown.TabIndex = 32;
            this.btnScrolDown.Text = "▼";
            this.btnScrolDown.UseVisualStyleBackColor = true;
            // 
            // btnScrollUp
            // 
            this.btnScrollUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrollUp.Enabled = false;
            this.btnScrollUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrollUp.Location = new System.Drawing.Point(345, 35);
            this.btnScrollUp.Name = "btnScrollUp";
            this.btnScrollUp.Size = new System.Drawing.Size(32, 24);
            this.btnScrollUp.TabIndex = 31;
            this.btnScrollUp.Text = "▲";
            this.btnScrollUp.UseVisualStyleBackColor = true;
            // 
            // shaderParamListView
            // 
            this.shaderParamListView.AllowColumnReorder = true;
            this.shaderParamListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shaderParamListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.shaderParamListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader1,
            this.columnHeader2});
            this.shaderParamListView.Location = new System.Drawing.Point(3, 32);
            this.shaderParamListView.MultiSelect = false;
            this.shaderParamListView.Name = "shaderParamListView";
            this.shaderParamListView.OwnerDraw = true;
            this.shaderParamListView.Size = new System.Drawing.Size(336, 582);
            this.shaderParamListView.TabIndex = 30;
            this.shaderParamListView.UseCompatibleStateImageBehavior = false;
            this.shaderParamListView.View = System.Windows.Forms.View.Details;
            this.shaderParamListView.SelectedIndexChanged += new System.EventHandler(this.shaderParamListView_SelectedIndexChanged);
            this.shaderParamListView.Click += new System.EventHandler(this.shaderParamListView_Click);
            this.shaderParamListView.DoubleClick += new System.EventHandler(this.shaderParamListView_DoubleClick);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Name";
            this.columnHeader10.Width = 94;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Value";
            this.columnHeader11.Width = 122;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Color (If Used)";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "";
            this.columnHeader2.Width = 25;
            // 
            // ShaderParamEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnScrolDown);
            this.Controls.Add(this.btnScrollUp);
            this.Controls.Add(this.shaderParamListView);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.stButton2);
            this.Controls.Add(this.stButton1);
            this.Name = "ShaderParamEditor";
            this.Size = new System.Drawing.Size(380, 620);
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STButton stButton2;
        private Toolbox.Library.Forms.STButton btnExport;
        private Toolbox.Library.Forms.STButton btnImport;
        private Toolbox.Library.Forms.STButton btnScrolDown;
        private Toolbox.Library.Forms.STButton btnScrollUp;
        private Toolbox.Library.Forms.ListViewCustom shaderParamListView;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
