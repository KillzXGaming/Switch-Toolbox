namespace FirstPlugin.Forms
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
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkboxVisible = new Toolbox.Library.Forms.STCheckBox();
            this.label3 = new Toolbox.Library.Forms.STLabel();
            this.textBoxShaderModel = new Toolbox.Library.Forms.STTextBox();
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.samplerEditor1 = new Forms.SamplerEditor();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.shaderParamEditor1 = new Forms.ShaderParamEditor();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.renderInfoEditor1 = new Forms.RenderInfoEditor();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.shaderOptionsEditor1 = new Forms.ShaderOptionsEditor();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.userDataEditor = new UserDataEditor();
            this.label2 = new Toolbox.Library.Forms.STLabel();
            this.textBoxShaderArchive = new Toolbox.Library.Forms.STTextBox();
            this.label1 = new Toolbox.Library.Forms.STLabel();
            this.textBoxMaterialName = new Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.btnViotileFlags = new Toolbox.Library.Forms.STButton();
            this.btnSamplerInputEditor = new Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.btnAttributeInputEditor = new Toolbox.Library.Forms.STButton();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stTabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
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
            this.columnHeader9.Width = 44;
            // 
            // chkboxVisible
            // 
            this.chkboxVisible.AutoSize = true;
            this.chkboxVisible.Checked = true;
            this.chkboxVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkboxVisible.Location = new System.Drawing.Point(361, 8);
            this.chkboxVisible.Name = "chkboxVisible";
            this.chkboxVisible.Size = new System.Drawing.Size(56, 17);
            this.chkboxVisible.TabIndex = 43;
            this.chkboxVisible.Text = "Visible";
            this.chkboxVisible.UseVisualStyleBackColor = true;
            this.chkboxVisible.CheckedChanged += new System.EventHandler(this.chkboxVisible_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "Shader Model";
            // 
            // textBoxShaderModel
            // 
            this.textBoxShaderModel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxShaderModel.Location = new System.Drawing.Point(85, 54);
            this.textBoxShaderModel.Name = "textBoxShaderModel";
            this.textBoxShaderModel.Size = new System.Drawing.Size(257, 20);
            this.textBoxShaderModel.TabIndex = 41;
            this.textBoxShaderModel.TextChanged += new System.EventHandler(this.textBoxShaderModel_TextChanged);
            // 
            // stTabControl1
            // 
            this.stTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Controls.Add(this.tabPage3);
            this.stTabControl1.Controls.Add(this.tabPage4);
            this.stTabControl1.Controls.Add(this.tabPage5);
            this.stTabControl1.Controls.Add(this.tabPage6);
            this.stTabControl1.Location = new System.Drawing.Point(0, 105);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(538, 539);
            this.stTabControl1.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.samplerEditor1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(530, 510);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Textures";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // samplerEditor1
            // 
            this.samplerEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.samplerEditor1.Location = new System.Drawing.Point(3, 3);
            this.samplerEditor1.Name = "samplerEditor1";
            this.samplerEditor1.Size = new System.Drawing.Size(524, 504);
            this.samplerEditor1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.shaderParamEditor1);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(530, 510);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Parameters";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // shaderParamEditor1
            // 
            this.shaderParamEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shaderParamEditor1.Location = new System.Drawing.Point(3, 3);
            this.shaderParamEditor1.Name = "shaderParamEditor1";
            this.shaderParamEditor1.Size = new System.Drawing.Size(524, 504);
            this.shaderParamEditor1.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.renderInfoEditor1);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(530, 510);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Render Info";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // renderInfoEditor1
            // 
            this.renderInfoEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderInfoEditor1.Location = new System.Drawing.Point(3, 3);
            this.renderInfoEditor1.Name = "renderInfoEditor1";
            this.renderInfoEditor1.Size = new System.Drawing.Size(524, 504);
            this.renderInfoEditor1.TabIndex = 9;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.shaderOptionsEditor1);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(530, 510);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Shader Options";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // shaderOptionsEditor1
            // 
            this.shaderOptionsEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shaderOptionsEditor1.Location = new System.Drawing.Point(3, 3);
            this.shaderOptionsEditor1.Name = "shaderOptionsEditor1";
            this.shaderOptionsEditor1.Size = new System.Drawing.Size(524, 504);
            this.shaderOptionsEditor1.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.userDataEditor);
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(530, 510);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "User Data";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // userDataEditor
            // 
            this.userDataEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userDataEditor.Location = new System.Drawing.Point(3, 3);
            this.userDataEditor.Name = "userDataEditor";
            this.userDataEditor.Size = new System.Drawing.Size(524, 504);
            this.userDataEditor.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "Shader Archive";
            // 
            // textBoxShaderArchive
            // 
            this.textBoxShaderArchive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxShaderArchive.Location = new System.Drawing.Point(85, 29);
            this.textBoxShaderArchive.Name = "textBoxShaderArchive";
            this.textBoxShaderArchive.Size = new System.Drawing.Size(257, 20);
            this.textBoxShaderArchive.TabIndex = 39;
            this.textBoxShaderArchive.TextChanged += new System.EventHandler(this.textBoxShaderArchive_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Name";
            // 
            // textBoxMaterialName
            // 
            this.textBoxMaterialName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMaterialName.Location = new System.Drawing.Point(85, 5);
            this.textBoxMaterialName.Name = "textBoxMaterialName";
            this.textBoxMaterialName.Size = new System.Drawing.Size(257, 20);
            this.textBoxMaterialName.TabIndex = 37;
            this.textBoxMaterialName.TextChanged += new System.EventHandler(this.textBoxMaterialName_TextChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(366, 56);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(67, 13);
            this.stLabel1.TabIndex = 44;
            this.stLabel1.Text = "Violate Flags";
            // 
            // btnViotileFlags
            // 
            this.btnViotileFlags.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViotileFlags.Location = new System.Drawing.Point(439, 53);
            this.btnViotileFlags.Name = "btnViotileFlags";
            this.btnViotileFlags.Size = new System.Drawing.Size(75, 23);
            this.btnViotileFlags.TabIndex = 45;
            this.btnViotileFlags.UseVisualStyleBackColor = false;
            this.btnViotileFlags.Click += new System.EventHandler(this.btnViotileFlags_Click);
            // 
            // btnSamplerInputEditor
            // 
            this.btnSamplerInputEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSamplerInputEditor.Location = new System.Drawing.Point(85, 79);
            this.btnSamplerInputEditor.Name = "btnSamplerInputEditor";
            this.btnSamplerInputEditor.Size = new System.Drawing.Size(75, 23);
            this.btnSamplerInputEditor.TabIndex = 47;
            this.btnSamplerInputEditor.UseVisualStyleBackColor = false;
            this.btnSamplerInputEditor.Click += new System.EventHandler(this.btnSamplerInputEditor_Click);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(4, 84);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(80, 13);
            this.stLabel2.TabIndex = 46;
            this.stLabel2.Text = "Sampler Inputs:";
            // 
            // btnAttributeInputEditor
            // 
            this.btnAttributeInputEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAttributeInputEditor.Location = new System.Drawing.Point(267, 79);
            this.btnAttributeInputEditor.Name = "btnAttributeInputEditor";
            this.btnAttributeInputEditor.Size = new System.Drawing.Size(75, 23);
            this.btnAttributeInputEditor.TabIndex = 49;
            this.btnAttributeInputEditor.UseVisualStyleBackColor = false;
            this.btnAttributeInputEditor.Click += new System.EventHandler(this.btnAttributeInputEditor_Click);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(186, 84);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(81, 13);
            this.stLabel3.TabIndex = 48;
            this.stLabel3.Text = "Attribute Inputs:";
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(439, 82);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 51;
            this.stButton1.UseVisualStyleBackColor = false;
            this.stButton1.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(366, 85);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(42, 13);
            this.stLabel4.TabIndex = 50;
            this.stLabel4.Text = "Presets";
            // 
            // FMATEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stButton1);
            this.Controls.Add(this.stLabel4);
            this.Controls.Add(this.btnAttributeInputEditor);
            this.Controls.Add(this.stLabel3);
            this.Controls.Add(this.btnSamplerInputEditor);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.btnViotileFlags);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxShaderModel);
            this.Controls.Add(this.chkboxVisible);
            this.Controls.Add(this.stTabControl1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxShaderArchive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMaterialName);
            this.Name = "FMATEditor";
            this.Size = new System.Drawing.Size(538, 644);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private Toolbox.Library.Forms.STCheckBox chkboxVisible;
        private Toolbox.Library.Forms.STLabel label3;
        private Toolbox.Library.Forms.STTextBox textBoxShaderModel;
        private Toolbox.Library.Forms.STLabel label2;
        private Toolbox.Library.Forms.STTextBox textBoxShaderArchive;
        private Toolbox.Library.Forms.STLabel label1;
        private Toolbox.Library.Forms.STTextBox textBoxMaterialName;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private SamplerEditor samplerEditor1;
        private UserDataEditor userDataEditor;
        private ShaderParamEditor shaderParamEditor1;
        private RenderInfoEditor renderInfoEditor1;
        private ShaderOptionsEditor shaderOptionsEditor1;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STButton btnViotileFlags;
        private Toolbox.Library.Forms.STButton btnSamplerInputEditor;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STButton btnAttributeInputEditor;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STLabel stLabel4;
    }
}