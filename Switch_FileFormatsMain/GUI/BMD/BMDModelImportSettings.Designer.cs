namespace FirstPlugin
{
    partial class BMDModelImportSettings
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
            this.texturePathTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.materalPathTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stComboBox1 = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.stComboBox1);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.materalPathTB);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.texturePathTB);
            this.contentContainer.Size = new System.Drawing.Size(164, 231);
            this.contentContainer.Controls.SetChildIndex(this.texturePathTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.materalPathTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.stComboBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            // 
            // texturePathTB
            // 
            this.texturePathTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.texturePathTB.Location = new System.Drawing.Point(22, 111);
            this.texturePathTB.Name = "texturePathTB";
            this.texturePathTB.ReadOnly = true;
            this.texturePathTB.Size = new System.Drawing.Size(121, 20);
            this.texturePathTB.TabIndex = 11;
            this.texturePathTB.TextChanged += new System.EventHandler(this.texturePathTB_TextChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(19, 95);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(73, 13);
            this.stLabel1.TabIndex = 12;
            this.stLabel1.Text = "Textures Path";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(19, 141);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(74, 13);
            this.stLabel2.TabIndex = 14;
            this.stLabel2.Text = "Materials Path";
            // 
            // materalPathTB
            // 
            this.materalPathTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.materalPathTB.Location = new System.Drawing.Point(22, 157);
            this.materalPathTB.Name = "materalPathTB";
            this.materalPathTB.ReadOnly = true;
            this.materalPathTB.Size = new System.Drawing.Size(121, 20);
            this.materalPathTB.TabIndex = 13;
            this.materalPathTB.TextChanged += new System.EventHandler(this.materalPathTB_TextChanged);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(20, 40);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(73, 13);
            this.stLabel3.TabIndex = 16;
            this.stLabel3.Text = "Tri Strip Mode";
            // 
            // stComboBox1
            // 
            this.stComboBox1.BorderColor = System.Drawing.Color.Empty;
            this.stComboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.stComboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.stComboBox1.FormattingEnabled = true;
            this.stComboBox1.Items.AddRange(new object[] {
            "static"});
            this.stComboBox1.Location = new System.Drawing.Point(22, 56);
            this.stComboBox1.Name = "stComboBox1";
            this.stComboBox1.ReadOnly = true;
            this.stComboBox1.Size = new System.Drawing.Size(121, 21);
            this.stComboBox1.TabIndex = 17;
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(73, 202);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 18;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // BMDModelImportSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(170, 236);
            this.Name = "BMDModelImportSettings";
            this.Text = "J3D Import Settings";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STTextBox texturePathTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STTextBox materalPathTB;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STComboBox stComboBox1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
    }
}