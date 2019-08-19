namespace FirstPlugin.Forms
{
    partial class CopyUVChannelDialog
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
            this.sourceCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.destCB = new Toolbox.Library.Forms.STComboBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.destCB);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.sourceCB);
            this.contentContainer.Size = new System.Drawing.Size(388, 87);
            this.contentContainer.Controls.SetChildIndex(this.sourceCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.destCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            // 
            // sourceCB
            // 
            this.sourceCB.BorderColor = System.Drawing.Color.Empty;
            this.sourceCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.sourceCB.ButtonColor = System.Drawing.Color.Empty;
            this.sourceCB.FormattingEnabled = true;
            this.sourceCB.Location = new System.Drawing.Point(54, 31);
            this.sourceCB.Name = "sourceCB";
            this.sourceCB.Size = new System.Drawing.Size(121, 21);
            this.sourceCB.TabIndex = 0;
            this.sourceCB.SelectedIndexChanged += new System.EventHandler(this.sourceCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(5, 34);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(41, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Source";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(190, 34);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(62, 13);
            this.stLabel2.TabIndex = 2;
            this.stLabel2.Text = "Copy To -->";
            // 
            // destCB
            // 
            this.destCB.BorderColor = System.Drawing.Color.Empty;
            this.destCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.destCB.ButtonColor = System.Drawing.Color.Empty;
            this.destCB.FormattingEnabled = true;
            this.destCB.Location = new System.Drawing.Point(258, 31);
            this.destCB.Name = "destCB";
            this.destCB.Size = new System.Drawing.Size(121, 21);
            this.destCB.TabIndex = 3;
            this.destCB.SelectedIndexChanged += new System.EventHandler(this.destCB_SelectedIndexChanged);
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(304, 58);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 4;
            this.stButton1.Text = "Cancel";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(223, 58);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 5;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // CopyUVChannelDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 92);
            this.Name = "CopyUVChannelDialog";
            this.Text = "Copy UV Channel";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STComboBox sourceCB;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox destCB;
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STButton stButton2;
    }
}