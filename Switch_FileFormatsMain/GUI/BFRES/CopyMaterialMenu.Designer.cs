namespace FirstPlugin
{
    partial class CopyMaterialMenu
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
            this.chkBoxShaderParams = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkBoxShaderOptions = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.chkBoxTextures = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.chkBoxRenderInfo = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.materialTreeView = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkBoxShaderParams
            // 
            this.chkBoxShaderParams.AutoSize = true;
            this.chkBoxShaderParams.Location = new System.Drawing.Point(26, 42);
            this.chkBoxShaderParams.Name = "chkBoxShaderParams";
            this.chkBoxShaderParams.Size = new System.Drawing.Size(98, 17);
            this.chkBoxShaderParams.TabIndex = 1;
            this.chkBoxShaderParams.Text = "Shader Params";
            this.chkBoxShaderParams.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Copy Over:";
            // 
            // chkBoxShaderOptions
            // 
            this.chkBoxShaderOptions.AutoSize = true;
            this.chkBoxShaderOptions.Location = new System.Drawing.Point(26, 65);
            this.chkBoxShaderOptions.Name = "chkBoxShaderOptions";
            this.chkBoxShaderOptions.Size = new System.Drawing.Size(99, 17);
            this.chkBoxShaderOptions.TabIndex = 3;
            this.chkBoxShaderOptions.Text = "Shader Options";
            this.chkBoxShaderOptions.UseVisualStyleBackColor = true;
            // 
            // chkBoxTextures
            // 
            this.chkBoxTextures.AutoSize = true;
            this.chkBoxTextures.Location = new System.Drawing.Point(25, 88);
            this.chkBoxTextures.Name = "chkBoxTextures";
            this.chkBoxTextures.Size = new System.Drawing.Size(139, 17);
            this.chkBoxTextures.TabIndex = 4;
            this.chkBoxTextures.Text = "Textures/Sampler Maps";
            this.chkBoxTextures.UseVisualStyleBackColor = true;
            // 
            // chkBoxRenderInfo
            // 
            this.chkBoxRenderInfo.AutoSize = true;
            this.chkBoxRenderInfo.Location = new System.Drawing.Point(25, 111);
            this.chkBoxRenderInfo.Name = "chkBoxRenderInfo";
            this.chkBoxRenderInfo.Size = new System.Drawing.Size(82, 17);
            this.chkBoxRenderInfo.TabIndex = 5;
            this.chkBoxRenderInfo.Text = "Render Info";
            this.chkBoxRenderInfo.UseVisualStyleBackColor = true;
            // 
            // materialTreeView
            // 
            this.materialTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.materialTreeView.CheckBoxes = true;
            this.materialTreeView.ForeColor = System.Drawing.Color.White;
            this.materialTreeView.FullRowSelect = true;
            this.materialTreeView.Location = new System.Drawing.Point(170, 13);
            this.materialTreeView.Name = "materialTreeView";
            this.materialTreeView.Size = new System.Drawing.Size(313, 343);
            this.materialTreeView.TabIndex = 6;
            this.materialTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(408, 364);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // CopyMaterialMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(495, 399);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.materialTreeView);
            this.Controls.Add(this.chkBoxRenderInfo);
            this.Controls.Add(this.chkBoxTextures);
            this.Controls.Add(this.chkBoxShaderOptions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkBoxShaderParams);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "CopyMaterialMenu";
            this.Text = "Copy Materials";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public Switch_Toolbox.Library.Forms.STCheckBox chkBoxShaderParams;
        public Switch_Toolbox.Library.Forms.STCheckBox chkBoxShaderOptions;
        public Switch_Toolbox.Library.Forms.STCheckBox chkBoxTextures;
        public Switch_Toolbox.Library.Forms.STCheckBox chkBoxRenderInfo;
        public System.Windows.Forms.TreeView materialTreeView;
        private System.Windows.Forms.Button button1;
    }
}