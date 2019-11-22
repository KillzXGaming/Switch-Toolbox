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
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.chkSearchNumbered = new Toolbox.Library.Forms.STCheckBox();
            this.characterStartTB = new Toolbox.Library.Forms.STTextBox();
            this.stLabel8 = new Toolbox.Library.Forms.STLabel();
            this.maxLengthUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel7 = new Toolbox.Library.Forms.STLabel();
            this.searchUppercase = new Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.bruteForceStringTB = new Toolbox.Library.Forms.STTextBox();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            this.bruteForceHashTB = new Toolbox.Library.Forms.STTextBox();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.contentContainer.SuspendLayout();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxLengthUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel4);
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Controls.Add(this.chkUseHex);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.hashTypeCB);
            this.contentContainer.Controls.Add(this.resultTB);
            this.contentContainer.Controls.Add(this.stringTB);
            this.contentContainer.Size = new System.Drawing.Size(704, 362);
            this.contentContainer.Controls.SetChildIndex(this.stringTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.resultTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.hashTypeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkUseHex, 0);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel4, 0);
            // 
            // stringTB
            // 
            this.stringTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stringTB.Location = new System.Drawing.Point(88, 74);
            this.stringTB.Name = "stringTB";
            this.stringTB.Size = new System.Drawing.Size(282, 20);
            this.stringTB.TabIndex = 11;
            this.stringTB.TextChanged += new System.EventHandler(this.stTextBox1_TextChanged);
            // 
            // resultTB
            // 
            this.resultTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultTB.Location = new System.Drawing.Point(453, 74);
            this.resultTB.Name = "resultTB";
            this.resultTB.Size = new System.Drawing.Size(199, 20);
            this.resultTB.TabIndex = 12;
            // 
            // hashTypeCB
            // 
            this.hashTypeCB.BorderColor = System.Drawing.Color.Empty;
            this.hashTypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.hashTypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.hashTypeCB.FormattingEnabled = true;
            this.hashTypeCB.IsReadOnly = false;
            this.hashTypeCB.Location = new System.Drawing.Point(88, 43);
            this.hashTypeCB.Name = "hashTypeCB";
            this.hashTypeCB.Size = new System.Drawing.Size(121, 21);
            this.hashTypeCB.TabIndex = 13;
            this.hashTypeCB.SelectedIndexChanged += new System.EventHandler(this.hashTypeCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(20, 46);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(62, 13);
            this.stLabel1.TabIndex = 14;
            this.stLabel1.Text = "Hash Type:";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(20, 76);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(37, 13);
            this.stLabel2.TabIndex = 15;
            this.stLabel2.Text = "String:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(379, 76);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(68, 13);
            this.stLabel3.TabIndex = 16;
            this.stLabel3.Text = "Hash Result:";
            // 
            // chkUseHex
            // 
            this.chkUseHex.AutoSize = true;
            this.chkUseHex.Location = new System.Drawing.Point(453, 45);
            this.chkUseHex.Name = "chkUseHex";
            this.chkUseHex.Size = new System.Drawing.Size(86, 17);
            this.chkUseHex.TabIndex = 17;
            this.chkUseHex.Text = "Preview Hex";
            this.chkUseHex.UseVisualStyleBackColor = true;
            this.chkUseHex.CheckedChanged += new System.EventHandler(this.chkUseHex_CheckedChanged);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.chkSearchNumbered);
            this.stPanel1.Controls.Add(this.characterStartTB);
            this.stPanel1.Controls.Add(this.stLabel8);
            this.stPanel1.Controls.Add(this.maxLengthUD);
            this.stPanel1.Controls.Add(this.stLabel7);
            this.stPanel1.Controls.Add(this.searchUppercase);
            this.stPanel1.Controls.Add(this.stButton1);
            this.stPanel1.Controls.Add(this.bruteForceStringTB);
            this.stPanel1.Controls.Add(this.stLabel6);
            this.stPanel1.Controls.Add(this.bruteForceHashTB);
            this.stPanel1.Controls.Add(this.stLabel5);
            this.stPanel1.Location = new System.Drawing.Point(5, 100);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(690, 250);
            this.stPanel1.TabIndex = 18;
            // 
            // chkSearchNumbered
            // 
            this.chkSearchNumbered.AutoSize = true;
            this.chkSearchNumbered.Location = new System.Drawing.Point(554, 188);
            this.chkSearchNumbered.Name = "chkSearchNumbered";
            this.chkSearchNumbered.Size = new System.Drawing.Size(112, 17);
            this.chkSearchNumbered.TabIndex = 30;
            this.chkSearchNumbered.Text = "Search Numbered";
            this.chkSearchNumbered.UseVisualStyleBackColor = true;
            // 
            // characterStartTB
            // 
            this.characterStartTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.characterStartTB.Location = new System.Drawing.Point(381, 49);
            this.characterStartTB.Multiline = true;
            this.characterStartTB.Name = "characterStartTB";
            this.characterStartTB.Size = new System.Drawing.Size(159, 156);
            this.characterStartTB.TabIndex = 28;
            // 
            // stLabel8
            // 
            this.stLabel8.AutoSize = true;
            this.stLabel8.Location = new System.Drawing.Point(378, 32);
            this.stLabel8.Name = "stLabel8";
            this.stLabel8.Size = new System.Drawing.Size(81, 13);
            this.stLabel8.TabIndex = 29;
            this.stLabel8.Text = "Starting String:s";
            // 
            // maxLengthUD
            // 
            this.maxLengthUD.Location = new System.Drawing.Point(549, 133);
            this.maxLengthUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.maxLengthUD.Name = "maxLengthUD";
            this.maxLengthUD.Size = new System.Drawing.Size(120, 20);
            this.maxLengthUD.TabIndex = 27;
            // 
            // stLabel7
            // 
            this.stLabel7.AutoSize = true;
            this.stLabel7.Location = new System.Drawing.Point(546, 105);
            this.stLabel7.Name = "stLabel7";
            this.stLabel7.Size = new System.Drawing.Size(116, 13);
            this.stLabel7.TabIndex = 26;
            this.stLabel7.Text = "Max Character Search:";
            // 
            // searchUppercase
            // 
            this.searchUppercase.AutoSize = true;
            this.searchUppercase.Location = new System.Drawing.Point(554, 165);
            this.searchUppercase.Name = "searchUppercase";
            this.searchUppercase.Size = new System.Drawing.Size(115, 17);
            this.searchUppercase.TabIndex = 24;
            this.searchUppercase.Text = "Search Uppercase";
            this.searchUppercase.UseVisualStyleBackColor = true;
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(22, 212);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 23;
            this.stButton1.Text = "Start";
            this.stButton1.UseVisualStyleBackColor = false;
            this.stButton1.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // bruteForceStringTB
            // 
            this.bruteForceStringTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bruteForceStringTB.Location = new System.Drawing.Point(205, 48);
            this.bruteForceStringTB.Multiline = true;
            this.bruteForceStringTB.Name = "bruteForceStringTB";
            this.bruteForceStringTB.Size = new System.Drawing.Size(160, 157);
            this.bruteForceStringTB.TabIndex = 21;
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(202, 32);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(70, 13);
            this.stLabel6.TabIndex = 22;
            this.stLabel6.Text = "String Result:";
            // 
            // bruteForceHashTB
            // 
            this.bruteForceHashTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bruteForceHashTB.Location = new System.Drawing.Point(22, 48);
            this.bruteForceHashTB.Multiline = true;
            this.bruteForceHashTB.Name = "bruteForceHashTB";
            this.bruteForceHashTB.Size = new System.Drawing.Size(161, 157);
            this.bruteForceHashTB.TabIndex = 20;
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(19, 32);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(46, 13);
            this.stLabel5.TabIndex = 20;
            this.stLabel5.Text = "Hashes:";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(20, 107);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(65, 13);
            this.stLabel4.TabIndex = 19;
            this.stLabel4.Text = "Brute Force:";
            // 
            // HashCalculatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 367);
            this.Name = "HashCalculatorForm";
            this.Text = "Hash Calculator";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxLengthUD)).EndInit();
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
        private Library.Forms.STLabel stLabel4;
        private Library.Forms.STPanel stPanel1;
        private Library.Forms.STTextBox bruteForceStringTB;
        private Library.Forms.STLabel stLabel6;
        private Library.Forms.STTextBox bruteForceHashTB;
        private Library.Forms.STLabel stLabel5;
        private Library.Forms.STButton stButton1;
        private Library.Forms.STCheckBox searchUppercase;
        private Library.Forms.STLabel stLabel7;
        private Library.Forms.NumericUpDownUint maxLengthUD;
        private Library.Forms.STTextBox characterStartTB;
        private Library.Forms.STLabel stLabel8;
        private Library.Forms.STCheckBox chkSearchNumbered;
    }
}