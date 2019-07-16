namespace FirstPlugin.Forms
{
    partial class MaterialReplaceDialog
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
            this.textureChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.paramChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.renderInfosChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.optionsChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.UserDataChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.UserDataChkBox);
            this.contentContainer.Controls.Add(this.optionsChkBox);
            this.contentContainer.Controls.Add(this.renderInfosChkBox);
            this.contentContainer.Controls.Add(this.paramChkBox);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.textureChkBox);
            this.contentContainer.Size = new System.Drawing.Size(208, 243);
            this.contentContainer.Controls.SetChildIndex(this.textureChkBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.paramChkBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.renderInfosChkBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.optionsChkBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.UserDataChkBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            // 
            // textureChkBox
            // 
            this.textureChkBox.AutoSize = true;
            this.textureChkBox.Location = new System.Drawing.Point(12, 73);
            this.textureChkBox.Name = "textureChkBox";
            this.textureChkBox.Size = new System.Drawing.Size(91, 17);
            this.textureChkBox.TabIndex = 11;
            this.textureChkBox.Text = "Texture Maps";
            this.textureChkBox.UseVisualStyleBackColor = true;
            this.textureChkBox.CheckedChanged += new System.EventHandler(this.ChkBox_CheckedChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 41);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(192, 13);
            this.stLabel1.TabIndex = 12;
            this.stLabel1.Text = "Select the sectiions you want replaced:";
            // 
            // paramChkBox
            // 
            this.paramChkBox.AutoSize = true;
            this.paramChkBox.Location = new System.Drawing.Point(12, 96);
            this.paramChkBox.Name = "paramChkBox";
            this.paramChkBox.Size = new System.Drawing.Size(98, 17);
            this.paramChkBox.TabIndex = 13;
            this.paramChkBox.Text = "Shader Params";
            this.paramChkBox.UseVisualStyleBackColor = true;
            this.paramChkBox.CheckedChanged += new System.EventHandler(this.ChkBox_CheckedChanged);
            // 
            // renderInfosChkBox
            // 
            this.renderInfosChkBox.AutoSize = true;
            this.renderInfosChkBox.Location = new System.Drawing.Point(12, 142);
            this.renderInfosChkBox.Name = "renderInfosChkBox";
            this.renderInfosChkBox.Size = new System.Drawing.Size(82, 17);
            this.renderInfosChkBox.TabIndex = 14;
            this.renderInfosChkBox.Text = "Render Info";
            this.renderInfosChkBox.UseVisualStyleBackColor = true;
            this.renderInfosChkBox.CheckedChanged += new System.EventHandler(this.ChkBox_CheckedChanged);
            // 
            // optionsChkBox
            // 
            this.optionsChkBox.AutoSize = true;
            this.optionsChkBox.Location = new System.Drawing.Point(12, 119);
            this.optionsChkBox.Name = "optionsChkBox";
            this.optionsChkBox.Size = new System.Drawing.Size(99, 17);
            this.optionsChkBox.TabIndex = 15;
            this.optionsChkBox.Text = "Shader Options";
            this.optionsChkBox.UseVisualStyleBackColor = true;
            this.optionsChkBox.CheckedChanged += new System.EventHandler(this.ChkBox_CheckedChanged);
            // 
            // UserDataChkBox
            // 
            this.UserDataChkBox.AutoSize = true;
            this.UserDataChkBox.Location = new System.Drawing.Point(12, 165);
            this.UserDataChkBox.Name = "UserDataChkBox";
            this.UserDataChkBox.Size = new System.Drawing.Size(74, 17);
            this.UserDataChkBox.TabIndex = 16;
            this.UserDataChkBox.Text = "User Data";
            this.UserDataChkBox.UseVisualStyleBackColor = true;
            this.UserDataChkBox.CheckedChanged += new System.EventHandler(this.ChkBox_CheckedChanged);
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(124, 211);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 17;
            this.stButton1.Text = "Cancel";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(43, 211);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 18;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // MaterialReplaceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 248);
            this.Name = "MaterialReplaceDialog";
            this.Text = "Material Replace";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STCheckBox textureChkBox;
        private Toolbox.Library.Forms.STButton stButton2;
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STCheckBox UserDataChkBox;
        private Toolbox.Library.Forms.STCheckBox optionsChkBox;
        private Toolbox.Library.Forms.STCheckBox renderInfosChkBox;
        private Toolbox.Library.Forms.STCheckBox paramChkBox;
        private Toolbox.Library.Forms.STLabel stLabel1;
    }
}