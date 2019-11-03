namespace Toolbox
{
    partial class HashCalculatorForm
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
            this.stringTB = new Toolbox.Library.Forms.STTextBox();
            this.resultTB = new Toolbox.Library.Forms.STTextBox();
            this.hashTypeCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.chkUseHex = new Toolbox.Library.Forms.STCheckBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.chkUseHex);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.hashTypeCB);
            this.contentContainer.Controls.Add(this.resultTB);
            this.contentContainer.Controls.Add(this.stringTB);
            this.contentContainer.Size = new System.Drawing.Size(469, 120);
            this.contentContainer.Controls.SetChildIndex(this.stringTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.resultTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.hashTypeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkUseHex, 0);
            // 
            // stringTB
            // 
            this.stringTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stringTB.Location = new System.Drawing.Point(88, 76);
            this.stringTB.Name = "stringTB";
            this.stringTB.Size = new System.Drawing.Size(121, 20);
            this.stringTB.TabIndex = 11;
            this.stringTB.TextChanged += new System.EventHandler(this.stTextBox1_TextChanged);
            // 
            // resultTB
            // 
            this.resultTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultTB.Location = new System.Drawing.Point(299, 76);
            this.resultTB.Name = "resultTB";
            this.resultTB.Size = new System.Drawing.Size(160, 20);
            this.resultTB.TabIndex = 12;
            // 
            // hashTypeCB
            // 
            this.hashTypeCB.BorderColor = System.Drawing.Color.Empty;
            this.hashTypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.hashTypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.hashTypeCB.FormattingEnabled = true;
            this.hashTypeCB.IsReadOnly = false;
            this.hashTypeCB.Location = new System.Drawing.Point(88, 45);
            this.hashTypeCB.Name = "hashTypeCB";
            this.hashTypeCB.Size = new System.Drawing.Size(121, 21);
            this.hashTypeCB.TabIndex = 13;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(20, 48);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(62, 13);
            this.stLabel1.TabIndex = 14;
            this.stLabel1.Text = "Hash Type:";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(20, 78);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(37, 13);
            this.stLabel2.TabIndex = 15;
            this.stLabel2.Text = "String:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(225, 78);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(68, 13);
            this.stLabel3.TabIndex = 16;
            this.stLabel3.Text = "Hash Result:";
            // 
            // chkUseHex
            // 
            this.chkUseHex.AutoSize = true;
            this.chkUseHex.Location = new System.Drawing.Point(299, 47);
            this.chkUseHex.Name = "chkUseHex";
            this.chkUseHex.Size = new System.Drawing.Size(86, 17);
            this.chkUseHex.TabIndex = 17;
            this.chkUseHex.Text = "Preview Hex";
            this.chkUseHex.UseVisualStyleBackColor = true;
            this.chkUseHex.CheckedChanged += new System.EventHandler(this.chkUseHex_CheckedChanged);
            // 
            // HashCalculatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 125);
            this.Name = "HashCalculatorForm";
            this.Text = "Hash Calculator";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Library.Forms.STTextBox stringTB;
        private Library.Forms.STComboBox hashTypeCB;
        private Library.Forms.STTextBox resultTB;
        private Library.Forms.STLabel stLabel3;
        private Library.Forms.STLabel stLabel2;
        private Library.Forms.STLabel stLabel1;
        private Library.Forms.STCheckBox chkUseHex;
    }
}