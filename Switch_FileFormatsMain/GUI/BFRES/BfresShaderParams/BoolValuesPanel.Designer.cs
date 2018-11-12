namespace FirstPlugin
{
    partial class BoolValuesPanel
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
            this.bool1 = new System.Windows.Forms.CheckBox();
            this.bool2 = new System.Windows.Forms.CheckBox();
            this.bool3 = new System.Windows.Forms.CheckBox();
            this.bool4 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // bool1
            // 
            this.bool1.AutoSize = true;
            this.bool1.Location = new System.Drawing.Point(17, 21);
            this.bool1.Name = "bool1";
            this.bool1.Size = new System.Drawing.Size(53, 17);
            this.bool1.TabIndex = 0;
            this.bool1.Text = "Bool1";
            this.bool1.UseVisualStyleBackColor = true;
            this.bool1.CheckedChanged += new System.EventHandler(this.bool_CheckedChanged);
            // 
            // bool2
            // 
            this.bool2.AutoSize = true;
            this.bool2.Location = new System.Drawing.Point(76, 21);
            this.bool2.Name = "bool2";
            this.bool2.Size = new System.Drawing.Size(53, 17);
            this.bool2.TabIndex = 1;
            this.bool2.Text = "Bool2";
            this.bool2.UseVisualStyleBackColor = true;
            this.bool2.CheckedChanged += new System.EventHandler(this.bool_CheckedChanged);
            // 
            // bool3
            // 
            this.bool3.AutoSize = true;
            this.bool3.Location = new System.Drawing.Point(135, 21);
            this.bool3.Name = "bool3";
            this.bool3.Size = new System.Drawing.Size(53, 17);
            this.bool3.TabIndex = 2;
            this.bool3.Text = "Bool3";
            this.bool3.UseVisualStyleBackColor = true;
            this.bool3.CheckedChanged += new System.EventHandler(this.bool_CheckedChanged);
            // 
            // bool4
            // 
            this.bool4.AutoSize = true;
            this.bool4.Location = new System.Drawing.Point(194, 21);
            this.bool4.Name = "bool4";
            this.bool4.Size = new System.Drawing.Size(53, 17);
            this.bool4.TabIndex = 3;
            this.bool4.Text = "Bool4";
            this.bool4.UseVisualStyleBackColor = true;
            this.bool4.CheckedChanged += new System.EventHandler(this.bool_CheckedChanged);
            // 
            // BoolValuesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.bool4);
            this.Controls.Add(this.bool3);
            this.Controls.Add(this.bool2);
            this.Controls.Add(this.bool1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "BoolValuesPanel";
            this.Size = new System.Drawing.Size(469, 205);
            this.Load += new System.EventHandler(this.BoolValuesPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox bool1;
        private System.Windows.Forms.CheckBox bool2;
        private System.Windows.Forms.CheckBox bool3;
        private System.Windows.Forms.CheckBox bool4;
    }
}
