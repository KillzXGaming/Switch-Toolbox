namespace LayoutBXLYT
{
    partial class AddAnimGroupDialog
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
            this.typeCB = new Toolbox.Library.Forms.STComboBox();
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            this.objectTargetsCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.objectTargetsCB);
            this.contentContainer.Controls.Add(this.stTextBox1);
            this.contentContainer.Controls.Add(this.typeCB);
            this.contentContainer.Size = new System.Drawing.Size(375, 155);
            this.contentContainer.Controls.SetChildIndex(this.typeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stTextBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.objectTargetsCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            // 
            // typeCB
            // 
            this.typeCB.BorderColor = System.Drawing.Color.Empty;
            this.typeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.typeCB.ButtonColor = System.Drawing.Color.Empty;
            this.typeCB.FormattingEnabled = true;
            this.typeCB.IsReadOnly = false;
            this.typeCB.Location = new System.Drawing.Point(74, 45);
            this.typeCB.Name = "typeCB";
            this.typeCB.Size = new System.Drawing.Size(285, 21);
            this.typeCB.TabIndex = 11;
            this.typeCB.SelectedIndexChanged += new System.EventHandler(this.typeCB_SelectedIndexChanged);
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(74, 83);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(145, 20);
            this.stTextBox1.TabIndex = 12;
            // 
            // objectTargetsCB
            // 
            this.objectTargetsCB.BorderColor = System.Drawing.Color.Empty;
            this.objectTargetsCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.objectTargetsCB.ButtonColor = System.Drawing.Color.Empty;
            this.objectTargetsCB.FormattingEnabled = true;
            this.objectTargetsCB.IsReadOnly = false;
            this.objectTargetsCB.Location = new System.Drawing.Point(225, 82);
            this.objectTargetsCB.Name = "objectTargetsCB";
            this.objectTargetsCB.Size = new System.Drawing.Size(134, 21);
            this.objectTargetsCB.TabIndex = 13;
            this.objectTargetsCB.SelectedIndexChanged += new System.EventHandler(this.objectTargetsCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(34, 48);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(34, 13);
            this.stLabel1.TabIndex = 14;
            this.stLabel1.Text = "Type:";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(34, 85);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(38, 13);
            this.stLabel2.TabIndex = 15;
            this.stLabel2.Text = "Name:";
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(291, 123);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 16;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // AddAnimGroupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 160);
            this.Name = "AddAnimGroupDialog";
            this.Text = "Add Group";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STComboBox objectTargetsCB;
        private Toolbox.Library.Forms.STTextBox stTextBox1;
        private Toolbox.Library.Forms.STComboBox typeCB;
    }
}