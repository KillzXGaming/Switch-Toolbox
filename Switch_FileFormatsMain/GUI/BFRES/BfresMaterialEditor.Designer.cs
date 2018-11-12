namespace FirstPlugin
{
    partial class FMATEditor
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
            this.visualStudioTabControl1 = new VisualStudioTabControl.VisualStudioTabControl();
            this.generalTab = new System.Windows.Forms.TabPage();
            this.chkboxVisible = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxShaderModel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxShaderArchive = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMaterialName = new System.Windows.Forms.TextBox();
            this.textureMapTab = new System.Windows.Forms.TabPage();
            this.btnSamplerEditor = new System.Windows.Forms.Button();
            this.textureRefListView = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.materialParamTab = new System.Windows.Forms.TabPage();
            this.btnReplaceParams = new System.Windows.Forms.Button();
            this.btnExportParams = new System.Windows.Forms.Button();
            this.listView1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.renderInfoTab = new System.Windows.Forms.TabPage();
            this.renderInfoListView = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.shaderAssignTab = new System.Windows.Forms.TabPage();
            this.shaderOptionsListView = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.visualStudioTabControl1.SuspendLayout();
            this.generalTab.SuspendLayout();
            this.textureMapTab.SuspendLayout();
            this.materialParamTab.SuspendLayout();
            this.renderInfoTab.SuspendLayout();
            this.shaderAssignTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // visualStudioTabControl1
            // 
            this.visualStudioTabControl1.ActiveColor = System.Drawing.Color.Gray;
            this.visualStudioTabControl1.AllowDrop = true;
            this.visualStudioTabControl1.BackTabColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.visualStudioTabControl1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.visualStudioTabControl1.ClosingButtonColor = System.Drawing.Color.WhiteSmoke;
            this.visualStudioTabControl1.ClosingMessage = null;
            this.visualStudioTabControl1.Controls.Add(this.generalTab);
            this.visualStudioTabControl1.Controls.Add(this.textureMapTab);
            this.visualStudioTabControl1.Controls.Add(this.materialParamTab);
            this.visualStudioTabControl1.Controls.Add(this.renderInfoTab);
            this.visualStudioTabControl1.Controls.Add(this.shaderAssignTab);
            this.visualStudioTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visualStudioTabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.visualStudioTabControl1.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.visualStudioTabControl1.HorizontalLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.visualStudioTabControl1.ItemSize = new System.Drawing.Size(240, 14);
            this.visualStudioTabControl1.Location = new System.Drawing.Point(0, 0);
            this.visualStudioTabControl1.Name = "visualStudioTabControl1";
            this.visualStudioTabControl1.Padding = new System.Drawing.Point(2, 20);
            this.visualStudioTabControl1.SelectedIndex = 0;
            this.visualStudioTabControl1.SelectedTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.visualStudioTabControl1.ShowClosingButton = false;
            this.visualStudioTabControl1.ShowClosingMessage = false;
            this.visualStudioTabControl1.Size = new System.Drawing.Size(610, 548);
            this.visualStudioTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.visualStudioTabControl1.TabIndex = 1;
            this.visualStudioTabControl1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // generalTab
            // 
            this.generalTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.generalTab.Controls.Add(this.chkboxVisible);
            this.generalTab.Controls.Add(this.label3);
            this.generalTab.Controls.Add(this.textBoxShaderModel);
            this.generalTab.Controls.Add(this.label2);
            this.generalTab.Controls.Add(this.textBoxShaderArchive);
            this.generalTab.Controls.Add(this.label1);
            this.generalTab.Controls.Add(this.textBoxMaterialName);
            this.generalTab.Location = new System.Drawing.Point(4, 18);
            this.generalTab.Name = "generalTab";
            this.generalTab.Padding = new System.Windows.Forms.Padding(3);
            this.generalTab.Size = new System.Drawing.Size(602, 526);
            this.generalTab.TabIndex = 0;
            this.generalTab.Text = "General";
            // 
            // chkboxVisible
            // 
            this.chkboxVisible.AutoSize = true;
            this.chkboxVisible.Checked = true;
            this.chkboxVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkboxVisible.Location = new System.Drawing.Point(19, 19);
            this.chkboxVisible.Name = "chkboxVisible";
            this.chkboxVisible.Size = new System.Drawing.Size(65, 19);
            this.chkboxVisible.TabIndex = 36;
            this.chkboxVisible.Text = "Enable";
            this.chkboxVisible.UseVisualStyleBackColor = true;
            this.chkboxVisible.CheckedChanged += new System.EventHandler(this.chkboxVisible_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Shader Model";
            // 
            // textBoxShaderModel
            // 
            this.textBoxShaderModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxShaderModel.ForeColor = System.Drawing.Color.White;
            this.textBoxShaderModel.Location = new System.Drawing.Point(107, 120);
            this.textBoxShaderModel.Name = "textBoxShaderModel";
            this.textBoxShaderModel.Size = new System.Drawing.Size(257, 21);
            this.textBoxShaderModel.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Shader Archive";
            // 
            // textBoxShaderArchive
            // 
            this.textBoxShaderArchive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxShaderArchive.ForeColor = System.Drawing.Color.White;
            this.textBoxShaderArchive.Location = new System.Drawing.Point(107, 86);
            this.textBoxShaderArchive.Name = "textBoxShaderArchive";
            this.textBoxShaderArchive.Size = new System.Drawing.Size(257, 21);
            this.textBoxShaderArchive.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // textBoxMaterialName
            // 
            this.textBoxMaterialName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBoxMaterialName.ForeColor = System.Drawing.Color.White;
            this.textBoxMaterialName.Location = new System.Drawing.Point(107, 52);
            this.textBoxMaterialName.Name = "textBoxMaterialName";
            this.textBoxMaterialName.Size = new System.Drawing.Size(257, 21);
            this.textBoxMaterialName.TabIndex = 0;
            // 
            // textureMapTab
            // 
            this.textureMapTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.textureMapTab.Controls.Add(this.btnSamplerEditor);
            this.textureMapTab.Controls.Add(this.textureRefListView);
            this.textureMapTab.Location = new System.Drawing.Point(4, 18);
            this.textureMapTab.Name = "textureMapTab";
            this.textureMapTab.Padding = new System.Windows.Forms.Padding(3);
            this.textureMapTab.Size = new System.Drawing.Size(602, 526);
            this.textureMapTab.TabIndex = 1;
            this.textureMapTab.Text = "Texture Mapping";
            this.textureMapTab.DoubleClick += new System.EventHandler(this.textureMapTab_DoubleClick);
            // 
            // btnSamplerEditor
            // 
            this.btnSamplerEditor.Enabled = false;
            this.btnSamplerEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSamplerEditor.Location = new System.Drawing.Point(6, 7);
            this.btnSamplerEditor.Name = "btnSamplerEditor";
            this.btnSamplerEditor.Size = new System.Drawing.Size(157, 23);
            this.btnSamplerEditor.TabIndex = 2;
            this.btnSamplerEditor.Text = "Sampler Editor";
            this.btnSamplerEditor.UseVisualStyleBackColor = true;
            this.btnSamplerEditor.Click += new System.EventHandler(this.btnSamplerEditor_Click);
            // 
            // textureRefListView
            // 
            this.textureRefListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textureRefListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textureRefListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.textureRefListView.ForeColor = System.Drawing.Color.White;
            this.textureRefListView.Location = new System.Drawing.Point(3, 36);
            this.textureRefListView.Name = "textureRefListView";
            this.textureRefListView.OwnerDraw = true;
            this.textureRefListView.Size = new System.Drawing.Size(596, 487);
            this.textureRefListView.TabIndex = 1;
            this.textureRefListView.UseCompatibleStateImageBehavior = false;
            this.textureRefListView.View = System.Windows.Forms.View.Details;
            this.textureRefListView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.textureRefListView_DrawColumnHeader);
            this.textureRefListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.textureRefListView_DrawItem);
            this.textureRefListView.SelectedIndexChanged += new System.EventHandler(this.textureRefListView_SelectedIndexChanged);
            this.textureRefListView.DoubleClick += new System.EventHandler(this.textureRefListView_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Texture";
            this.columnHeader1.Width = 410;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Sampler";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Frag Sampler";
            this.columnHeader3.Width = 155;
            // 
            // materialParamTab
            // 
            this.materialParamTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.materialParamTab.Controls.Add(this.btnReplaceParams);
            this.materialParamTab.Controls.Add(this.btnExportParams);
            this.materialParamTab.Controls.Add(this.listView1);
            this.materialParamTab.Location = new System.Drawing.Point(4, 18);
            this.materialParamTab.Name = "materialParamTab";
            this.materialParamTab.Padding = new System.Windows.Forms.Padding(3);
            this.materialParamTab.Size = new System.Drawing.Size(602, 526);
            this.materialParamTab.TabIndex = 2;
            this.materialParamTab.Text = "Material Params";
            // 
            // btnReplaceParams
            // 
            this.btnReplaceParams.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReplaceParams.Location = new System.Drawing.Point(87, 6);
            this.btnReplaceParams.Name = "btnReplaceParams";
            this.btnReplaceParams.Size = new System.Drawing.Size(75, 23);
            this.btnReplaceParams.TabIndex = 7;
            this.btnReplaceParams.Text = "Replace";
            this.btnReplaceParams.UseVisualStyleBackColor = true;
            this.btnReplaceParams.Click += new System.EventHandler(this.btnReplaceParams_Click);
            // 
            // btnExportParams
            // 
            this.btnExportParams.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportParams.Location = new System.Drawing.Point(6, 6);
            this.btnExportParams.Name = "btnExportParams";
            this.btnExportParams.Size = new System.Drawing.Size(75, 23);
            this.btnExportParams.TabIndex = 6;
            this.btnExportParams.Text = "Export";
            this.btnExportParams.UseVisualStyleBackColor = true;
            this.btnExportParams.Click += new System.EventHandler(this.btnExportParams_Click);
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader6,
            this.columnHeader5});
            this.listView1.ForeColor = System.Drawing.Color.White;
            this.listView1.Location = new System.Drawing.Point(3, 34);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(596, 489);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 140;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Value";
            this.columnHeader6.Width = 90;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Color";
            this.columnHeader5.Width = 75;
            // 
            // renderInfoTab
            // 
            this.renderInfoTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.renderInfoTab.Controls.Add(this.renderInfoListView);
            this.renderInfoTab.Location = new System.Drawing.Point(4, 18);
            this.renderInfoTab.Name = "renderInfoTab";
            this.renderInfoTab.Padding = new System.Windows.Forms.Padding(3);
            this.renderInfoTab.Size = new System.Drawing.Size(602, 526);
            this.renderInfoTab.TabIndex = 3;
            this.renderInfoTab.Text = "Render Info";
            // 
            // renderInfoListView
            // 
            this.renderInfoListView.AllowColumnReorder = true;
            this.renderInfoListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.renderInfoListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.renderInfoListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.renderInfoListView.ForeColor = System.Drawing.Color.White;
            this.renderInfoListView.Location = new System.Drawing.Point(6, 16);
            this.renderInfoListView.MultiSelect = false;
            this.renderInfoListView.Name = "renderInfoListView";
            this.renderInfoListView.Size = new System.Drawing.Size(596, 486);
            this.renderInfoListView.TabIndex = 5;
            this.renderInfoListView.UseCompatibleStateImageBehavior = false;
            this.renderInfoListView.View = System.Windows.Forms.View.Details;
            this.renderInfoListView.DoubleClick += new System.EventHandler(this.renderInfoListView_DoubleClick);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Name";
            this.columnHeader7.Width = 267;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Value";
            this.columnHeader8.Width = 169;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Format";
            this.columnHeader9.Width = 75;
            // 
            // shaderAssignTab
            // 
            this.shaderAssignTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.shaderAssignTab.Controls.Add(this.shaderOptionsListView);
            this.shaderAssignTab.Location = new System.Drawing.Point(4, 18);
            this.shaderAssignTab.Name = "shaderAssignTab";
            this.shaderAssignTab.Padding = new System.Windows.Forms.Padding(3);
            this.shaderAssignTab.Size = new System.Drawing.Size(602, 526);
            this.shaderAssignTab.TabIndex = 4;
            this.shaderAssignTab.Text = "Shader Options";
            // 
            // shaderOptionsListView
            // 
            this.shaderOptionsListView.AllowColumnReorder = true;
            this.shaderOptionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shaderOptionsListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.shaderOptionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11});
            this.shaderOptionsListView.ForeColor = System.Drawing.Color.White;
            this.shaderOptionsListView.Location = new System.Drawing.Point(3, 19);
            this.shaderOptionsListView.MultiSelect = false;
            this.shaderOptionsListView.Name = "shaderOptionsListView";
            this.shaderOptionsListView.Size = new System.Drawing.Size(596, 486);
            this.shaderOptionsListView.TabIndex = 6;
            this.shaderOptionsListView.UseCompatibleStateImageBehavior = false;
            this.shaderOptionsListView.View = System.Windows.Forms.View.Details;
            this.shaderOptionsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.shaderOptionsListView_ColumnClick);
            this.shaderOptionsListView.DoubleClick += new System.EventHandler(this.shaderOptionsListView_DoubleClick);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Name";
            this.columnHeader10.Width = 267;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Value";
            this.columnHeader11.Width = 169;
            // 
            // FMATEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.visualStudioTabControl1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FMATEditor";
            this.Size = new System.Drawing.Size(610, 548);
            this.visualStudioTabControl1.ResumeLayout(false);
            this.generalTab.ResumeLayout(false);
            this.generalTab.PerformLayout();
            this.textureMapTab.ResumeLayout(false);
            this.materialParamTab.ResumeLayout(false);
            this.renderInfoTab.ResumeLayout(false);
            this.shaderAssignTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private VisualStudioTabControl.VisualStudioTabControl visualStudioTabControl1;
        private System.Windows.Forms.TabPage generalTab;
        private System.Windows.Forms.TabPage textureMapTab;
        private Switch_Toolbox.Library.Forms.ListViewCustom textureRefListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TabPage materialParamTab;
        private System.Windows.Forms.TabPage renderInfoTab;
        private System.Windows.Forms.TabPage shaderAssignTab;
        private Switch_Toolbox.Library.Forms.ListViewCustom listView1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private Switch_Toolbox.Library.Forms.ListViewCustom renderInfoListView;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Button btnReplaceParams;
        private System.Windows.Forms.Button btnExportParams;
        private Switch_Toolbox.Library.Forms.ListViewCustom shaderOptionsListView;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxShaderModel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxShaderArchive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMaterialName;
        private System.Windows.Forms.CheckBox chkboxVisible;
        private System.Windows.Forms.Button btnSamplerEditor;
    }
}