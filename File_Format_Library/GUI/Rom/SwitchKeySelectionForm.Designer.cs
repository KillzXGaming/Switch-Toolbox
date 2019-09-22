namespace FirstPlugin.Forms
{
    partial class SwitchKeySelectionForm
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
            this.setProdKeyPath = new Toolbox.Library.Forms.STButton();
            this.setTitleKeyPath = new Toolbox.Library.Forms.STButton();
            this.btnOk = new Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.TextBoxTitleKey = new Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.TextBoxProdKeyPath = new Toolbox.Library.Forms.STTextBox();
            this.stCheckBox1 = new Toolbox.Library.Forms.STCheckBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stCheckBox1);
            this.contentContainer.Controls.Add(this.setProdKeyPath);
            this.contentContainer.Controls.Add(this.setTitleKeyPath);
            this.contentContainer.Controls.Add(this.btnOk);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.TextBoxTitleKey);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.TextBoxProdKeyPath);
            this.contentContainer.Size = new System.Drawing.Size(430, 188);
            this.contentContainer.Controls.SetChildIndex(this.TextBoxProdKeyPath, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.TextBoxTitleKey, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOk, 0);
            this.contentContainer.Controls.SetChildIndex(this.setTitleKeyPath, 0);
            this.contentContainer.Controls.SetChildIndex(this.setProdKeyPath, 0);
            this.contentContainer.Controls.SetChildIndex(this.stCheckBox1, 0);
            // 
            // setProdKeyPath
            // 
            this.setProdKeyPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setProdKeyPath.Location = new System.Drawing.Point(9, 50);
            this.setProdKeyPath.Name = "setProdKeyPath";
            this.setProdKeyPath.Size = new System.Drawing.Size(53, 23);
            this.setProdKeyPath.TabIndex = 6;
            this.setProdKeyPath.Text = "Set";
            this.setProdKeyPath.UseVisualStyleBackColor = false;
            this.setProdKeyPath.Click += new System.EventHandler(this.setProdKeyPath_Click);
            // 
            // setTitleKeyPath
            // 
            this.setTitleKeyPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setTitleKeyPath.Location = new System.Drawing.Point(9, 113);
            this.setTitleKeyPath.Name = "setTitleKeyPath";
            this.setTitleKeyPath.Size = new System.Drawing.Size(53, 23);
            this.setTitleKeyPath.TabIndex = 5;
            this.setTitleKeyPath.Text = "Set";
            this.setTitleKeyPath.UseVisualStyleBackColor = false;
            this.setTitleKeyPath.Click += new System.EventHandler(this.setTitleKeyPath_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Enabled = false;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(332, 149);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(9, 88);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(53, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Title Keys";
            // 
            // TextBoxTitleKey
            // 
            this.TextBoxTitleKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TextBoxTitleKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBoxTitleKey.Location = new System.Drawing.Point(68, 115);
            this.TextBoxTitleKey.Name = "TextBoxTitleKey";
            this.TextBoxTitleKey.ReadOnly = true;
            this.TextBoxTitleKey.Size = new System.Drawing.Size(339, 20);
            this.TextBoxTitleKey.TabIndex = 2;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 28);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(55, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Prod Keys";
            // 
            // TextBoxProdKeyPath
            // 
            this.TextBoxProdKeyPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TextBoxProdKeyPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBoxProdKeyPath.Location = new System.Drawing.Point(68, 53);
            this.TextBoxProdKeyPath.Name = "TextBoxProdKeyPath";
            this.TextBoxProdKeyPath.ReadOnly = true;
            this.TextBoxProdKeyPath.Size = new System.Drawing.Size(339, 20);
            this.TextBoxProdKeyPath.TabIndex = 0;
            // 
            // stCheckBox1
            // 
            this.stCheckBox1.AutoSize = true;
            this.stCheckBox1.Location = new System.Drawing.Point(3, 153);
            this.stCheckBox1.Name = "stCheckBox1";
            this.stCheckBox1.Size = new System.Drawing.Size(170, 17);
            this.stCheckBox1.TabIndex = 11;
            this.stCheckBox1.Text = "Use \"UserName/.switch path\"";
            this.stCheckBox1.UseVisualStyleBackColor = true;
            this.stCheckBox1.CheckedChanged += new System.EventHandler(this.stCheckBox1_CheckedChanged);
            // 
            // SwitchKeySelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 193);
            this.Name = "SwitchKeySelectionForm";
            this.Text = "Select Key Files";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STTextBox TextBoxProdKeyPath;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STTextBox TextBoxTitleKey;
        private Toolbox.Library.Forms.STButton btnOk;
        private Toolbox.Library.Forms.STButton setTitleKeyPath;
        private Toolbox.Library.Forms.STButton setProdKeyPath;
        private Toolbox.Library.Forms.STCheckBox stCheckBox1;
    }
}