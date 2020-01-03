namespace LayoutBXLYT
{
    partial class NewTextboxDialog
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
            this.stComboBox1 = new Toolbox.Library.Forms.STComboBox();
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.btnAdd = new Toolbox.Library.Forms.STButton();
            this.btnOk = new Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.btnOk);
            this.contentContainer.Controls.Add(this.btnAdd);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.stTextBox1);
            this.contentContainer.Controls.Add(this.stComboBox1);
            this.contentContainer.Size = new System.Drawing.Size(441, 238);
            this.contentContainer.Controls.SetChildIndex(this.stComboBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stTextBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnAdd, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOk, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            // 
            // stComboBox1
            // 
            this.stComboBox1.BorderColor = System.Drawing.Color.Empty;
            this.stComboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.stComboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.stComboBox1.FormattingEnabled = true;
            this.stComboBox1.IsReadOnly = false;
            this.stComboBox1.Location = new System.Drawing.Point(9, 55);
            this.stComboBox1.Name = "stComboBox1";
            this.stComboBox1.Size = new System.Drawing.Size(342, 21);
            this.stComboBox1.TabIndex = 0;
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(9, 95);
            this.stTextBox1.Multiline = true;
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(423, 104);
            this.stTextBox1.TabIndex = 1;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(6, 39);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(64, 13);
            this.stLabel1.TabIndex = 2;
            this.stLabel1.Text = "Select Font:";
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(357, 55);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(357, 206);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(9, 79);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(76, 13);
            this.stLabel2.TabIndex = 12;
            this.stLabel2.Text = "Text (Optional)";
            // 
            // NewTextboxDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 243);
            this.Name = "NewTextboxDialog";
            this.Text = "New Textbox";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STComboBox stComboBox1;
        private Toolbox.Library.Forms.STTextBox stTextBox1;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STButton btnAdd;
        private Toolbox.Library.Forms.STButton btnOk;
        private Toolbox.Library.Forms.STLabel stLabel2;
    }
}