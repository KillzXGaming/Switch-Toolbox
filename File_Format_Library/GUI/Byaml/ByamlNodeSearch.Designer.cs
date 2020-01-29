namespace FirstPlugin.GUI.Byaml
{
    partial class ByamlNodeSearch
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
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stCheckBox1 = new Toolbox.Library.Forms.STCheckBox();
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            this.stTextBox2 = new Toolbox.Library.Forms.STTextBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stComboBox1 = new Toolbox.Library.Forms.STComboBox();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.SuspendLayout();
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(44, 112);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(70, 13);
            this.stLabel1.TabIndex = 0;
            this.stLabel1.Text = "Batch Folder:";
            // 
            // stCheckBox1
            // 
            this.stCheckBox1.AutoSize = true;
            this.stCheckBox1.Location = new System.Drawing.Point(15, 81);
            this.stCheckBox1.Name = "stCheckBox1";
            this.stCheckBox1.Size = new System.Drawing.Size(146, 17);
            this.stCheckBox1.TabIndex = 1;
            this.stCheckBox1.Text = "Batch Search Byaml Files";
            this.stCheckBox1.UseVisualStyleBackColor = true;
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(120, 110);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(233, 20);
            this.stTextBox1.TabIndex = 2;
            // 
            // stTextBox2
            // 
            this.stTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox2.Location = new System.Drawing.Point(74, 12);
            this.stTextBox2.Name = "stTextBox2";
            this.stTextBox2.Size = new System.Drawing.Size(279, 20);
            this.stTextBox2.TabIndex = 3;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(12, 14);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(44, 13);
            this.stLabel2.TabIndex = 4;
            this.stLabel2.Text = "Search:";
            // 
            // stComboBox1
            // 
            this.stComboBox1.BorderColor = System.Drawing.Color.Empty;
            this.stComboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.stComboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.stComboBox1.FormattingEnabled = true;
            this.stComboBox1.IsReadOnly = false;
            this.stComboBox1.Location = new System.Drawing.Point(74, 39);
            this.stComboBox1.Name = "stComboBox1";
            this.stComboBox1.Size = new System.Drawing.Size(279, 21);
            this.stComboBox1.TabIndex = 5;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(12, 42);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(60, 13);
            this.stLabel3.TabIndex = 6;
            this.stLabel3.Text = "Data Type:";
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(278, 137);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 7;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // ByamlNodeSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 172);
            this.Controls.Add(this.stButton1);
            this.Controls.Add(this.stLabel3);
            this.Controls.Add(this.stComboBox1);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.stTextBox2);
            this.Controls.Add(this.stTextBox1);
            this.Controls.Add(this.stCheckBox1);
            this.Controls.Add(this.stLabel1);
            this.Name = "ByamlNodeSearch";
            this.Text = "Node Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STCheckBox stCheckBox1;
        private Toolbox.Library.Forms.STTextBox stTextBox1;
        private Toolbox.Library.Forms.STTextBox stTextBox2;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox stComboBox1;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STButton stButton1;
    }
}