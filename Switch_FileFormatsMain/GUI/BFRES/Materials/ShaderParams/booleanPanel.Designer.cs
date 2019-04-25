namespace FirstPlugin.Forms
{
    partial class booleanPanel
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
            this.stCheckBox1 = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stCheckBox2 = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stCheckBox3 = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stCheckBox4 = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stTextBox1 = new Switch_Toolbox.Library.Forms.STTextBox();
            this.SuspendLayout();
            // 
            // stCheckBox1
            // 
            this.stCheckBox1.AutoSize = true;
            this.stCheckBox1.Location = new System.Drawing.Point(152, 28);
            this.stCheckBox1.Name = "stCheckBox1";
            this.stCheckBox1.Size = new System.Drawing.Size(59, 17);
            this.stCheckBox1.TabIndex = 2;
            this.stCheckBox1.Text = "Value2";
            this.stCheckBox1.UseVisualStyleBackColor = true;
            // 
            // stCheckBox2
            // 
            this.stCheckBox2.AutoSize = true;
            this.stCheckBox2.Location = new System.Drawing.Point(21, 28);
            this.stCheckBox2.Name = "stCheckBox2";
            this.stCheckBox2.Size = new System.Drawing.Size(59, 17);
            this.stCheckBox2.TabIndex = 3;
            this.stCheckBox2.Text = "Value1";
            this.stCheckBox2.UseVisualStyleBackColor = true;
            // 
            // stCheckBox3
            // 
            this.stCheckBox3.AutoSize = true;
            this.stCheckBox3.Location = new System.Drawing.Point(21, 51);
            this.stCheckBox3.Name = "stCheckBox3";
            this.stCheckBox3.Size = new System.Drawing.Size(59, 17);
            this.stCheckBox3.TabIndex = 5;
            this.stCheckBox3.Text = "Value3";
            this.stCheckBox3.UseVisualStyleBackColor = true;
            // 
            // stCheckBox4
            // 
            this.stCheckBox4.AutoSize = true;
            this.stCheckBox4.Location = new System.Drawing.Point(152, 51);
            this.stCheckBox4.Name = "stCheckBox4";
            this.stCheckBox4.Size = new System.Drawing.Size(59, 17);
            this.stCheckBox4.TabIndex = 4;
            this.stCheckBox4.Text = "Value4";
            this.stCheckBox4.UseVisualStyleBackColor = true;
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(3, 2);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.ReadOnly = true;
            this.stTextBox1.Size = new System.Drawing.Size(276, 20);
            this.stTextBox1.TabIndex = 6;
            // 
            // booleanPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTextBox1);
            this.Controls.Add(this.stCheckBox3);
            this.Controls.Add(this.stCheckBox4);
            this.Controls.Add(this.stCheckBox2);
            this.Controls.Add(this.stCheckBox1);
            this.Name = "booleanPanel";
            this.Size = new System.Drawing.Size(282, 82);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Switch_Toolbox.Library.Forms.STCheckBox stCheckBox1;
        private Switch_Toolbox.Library.Forms.STCheckBox stCheckBox2;
        private Switch_Toolbox.Library.Forms.STCheckBox stCheckBox3;
        private Switch_Toolbox.Library.Forms.STCheckBox stCheckBox4;
        private Switch_Toolbox.Library.Forms.STTextBox stTextBox1;
    }
}
