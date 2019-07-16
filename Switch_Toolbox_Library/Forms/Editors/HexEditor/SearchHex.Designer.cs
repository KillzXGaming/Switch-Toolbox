namespace Toolbox.Library.Forms
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
            this.components = new System.ComponentModel.Container();
            this.txtFind = new Toolbox.Library.Forms.STTextBox();
            this.chkMatchCase = new Toolbox.Library.Forms.STCheckBox();
            this.btnOK = new Toolbox.Library.Forms.STButton();
            this.hexFind = new Be.Windows.Forms.HexBox();
            this.radioBtnText = new System.Windows.Forms.RadioButton();
            this.raditnHex = new System.Windows.Forms.RadioButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.timerPercent = new System.Windows.Forms.Timer(this.components);
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.raditnHex);
            this.contentContainer.Controls.Add(this.radioBtnText);
            this.contentContainer.Controls.Add(this.hexFind);
            this.contentContainer.Controls.Add(this.btnOK);
            this.contentContainer.Controls.Add(this.chkMatchCase);
            this.contentContainer.Controls.Add(this.txtFind);
            this.contentContainer.Size = new System.Drawing.Size(392, 267);
            this.contentContainer.Controls.SetChildIndex(this.txtFind, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkMatchCase, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOK, 0);
            this.contentContainer.Controls.SetChildIndex(this.hexFind, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioBtnText, 0);
            this.contentContainer.Controls.SetChildIndex(this.raditnHex, 0);
            // 
            // txtFind
            // 
            this.txtFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFind.Location = new System.Drawing.Point(4, 54);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(381, 20);
            this.txtFind.TabIndex = 1;
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(301, 31);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(84, 17);
            this.chkMatchCase.TabIndex = 2;
            this.chkMatchCase.Text = "Check Case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(310, 238);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // hexFind
            // 
            this.hexFind.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.hexFind.Location = new System.Drawing.Point(4, 103);
            this.hexFind.Name = "hexFind";
            this.hexFind.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexFind.Size = new System.Drawing.Size(371, 129);
            this.hexFind.TabIndex = 4;
            // 
            // radioBtnText
            // 
            this.radioBtnText.AutoSize = true;
            this.radioBtnText.Location = new System.Drawing.Point(4, 31);
            this.radioBtnText.Name = "radioBtnText";
            this.radioBtnText.Size = new System.Drawing.Size(46, 17);
            this.radioBtnText.TabIndex = 5;
            this.radioBtnText.TabStop = true;
            this.radioBtnText.Text = "Text";
            this.radioBtnText.UseVisualStyleBackColor = true;
            this.radioBtnText.CheckedChanged += new System.EventHandler(this.radioBtn_CheckedChanged);
            this.radioBtnText.Enter += new System.EventHandler(this.radioBtnText_Enter);
            // 
            // raditnHex
            // 
            this.raditnHex.AutoSize = true;
            this.raditnHex.Location = new System.Drawing.Point(4, 80);
            this.raditnHex.Name = "raditnHex";
            this.raditnHex.Size = new System.Drawing.Size(44, 17);
            this.raditnHex.TabIndex = 6;
            this.raditnHex.TabStop = true;
            this.raditnHex.Text = "Hex";
            this.raditnHex.UseVisualStyleBackColor = true;
            this.raditnHex.CheckedChanged += new System.EventHandler(this.radioBtn_CheckedChanged);
            this.raditnHex.Enter += new System.EventHandler(this.raditnHex_Enter);
            // 
            // timerPercent
            // 
            this.timerPercent.Tick += new System.EventHandler(this.timerPercent_Tick);
            // 
            // SearchHex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 272);
            this.Name = "SearchHex";
            this.Text = "Find";
            this.Activated += new System.EventHandler(this.SearchHex_Activated);
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STTextBox txtFind;
        private Toolbox.Library.Forms.STCheckBox chkMatchCase;
        private STButton btnOK;
        private Be.Windows.Forms.HexBox hexFind;
        private System.Windows.Forms.RadioButton radioBtnText;
        private System.Windows.Forms.RadioButton raditnHex;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer timerPercent;
    }
}