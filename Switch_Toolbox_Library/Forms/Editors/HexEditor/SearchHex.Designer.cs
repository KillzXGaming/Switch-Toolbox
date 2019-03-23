namespace Switch_Toolbox.Library.Forms
{
    partial class SearchHex
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
            this.stTextBox1 = new Switch_Toolbox.Library.Forms.STTextBox();
            this.chkBoxMatchCase = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.hexFind = new Be.Windows.Forms.HexBox();
            this.radioBtnText = new System.Windows.Forms.RadioButton();
            this.raditnHex = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // stTextBox1
            // 
            this.stTextBox1.Location = new System.Drawing.Point(27, 63);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(381, 20);
            this.stTextBox1.TabIndex = 1;
            // 
            // chkBoxMatchCase
            // 
            this.chkBoxMatchCase.AutoSize = true;
            this.chkBoxMatchCase.Location = new System.Drawing.Point(324, 40);
            this.chkBoxMatchCase.Name = "chkBoxMatchCase";
            this.chkBoxMatchCase.Size = new System.Drawing.Size(84, 17);
            this.chkBoxMatchCase.TabIndex = 2;
            this.chkBoxMatchCase.Text = "Check Case";
            this.chkBoxMatchCase.UseVisualStyleBackColor = true;
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.Location = new System.Drawing.Point(333, 247);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 3;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            this.stButton1.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // hexFind
            // 
            this.hexFind.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.hexFind.Location = new System.Drawing.Point(27, 112);
            this.hexFind.Name = "hexFind";
            this.hexFind.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexFind.Size = new System.Drawing.Size(371, 129);
            this.hexFind.TabIndex = 4;
            // 
            // radioBtnText
            // 
            this.radioBtnText.AutoSize = true;
            this.radioBtnText.Location = new System.Drawing.Point(27, 40);
            this.radioBtnText.Name = "radioBtnText";
            this.radioBtnText.Size = new System.Drawing.Size(46, 17);
            this.radioBtnText.TabIndex = 5;
            this.radioBtnText.TabStop = true;
            this.radioBtnText.Text = "Text";
            this.radioBtnText.UseVisualStyleBackColor = true;
            // 
            // raditnHex
            // 
            this.raditnHex.AutoSize = true;
            this.raditnHex.Location = new System.Drawing.Point(27, 89);
            this.raditnHex.Name = "raditnHex";
            this.raditnHex.Size = new System.Drawing.Size(44, 17);
            this.raditnHex.TabIndex = 6;
            this.raditnHex.TabStop = true;
            this.raditnHex.Text = "Hex";
            this.raditnHex.UseVisualStyleBackColor = true;
            // 
            // SearchHex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 282);
            this.Controls.Add(this.raditnHex);
            this.Controls.Add(this.radioBtnText);
            this.Controls.Add(this.hexFind);
            this.Controls.Add(this.stButton1);
            this.Controls.Add(this.chkBoxMatchCase);
            this.Controls.Add(this.stTextBox1);
            this.Name = "SearchHex";
            this.Text = "Find";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private STTextBox stTextBox1;
        private Switch_Toolbox.Library.Forms.STCheckBox chkBoxMatchCase;
        private STButton stButton1;
        private Be.Windows.Forms.HexBox hexFind;
        private System.Windows.Forms.RadioButton radioBtnText;
        private System.Windows.Forms.RadioButton raditnHex;
    }
}