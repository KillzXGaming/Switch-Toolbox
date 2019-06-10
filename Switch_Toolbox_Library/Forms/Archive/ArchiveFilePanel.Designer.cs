namespace Switch_Toolbox.Library.Forms
{
    partial class ArchiveFilePanel
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
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stComboBox1 = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(0, 29);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(369, 280);
            this.stPanel1.TabIndex = 0;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stLabel1);
            this.stPanel2.Controls.Add(this.stComboBox1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(369, 27);
            this.stPanel2.TabIndex = 1;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(3, 5);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(44, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Display:";
            // 
            // stComboBox1
            // 
            this.stComboBox1.BorderColor = System.Drawing.Color.Empty;
            this.stComboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.stComboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.stComboBox1.FormattingEnabled = true;
            this.stComboBox1.Location = new System.Drawing.Point(53, 2);
            this.stComboBox1.Name = "stComboBox1";
            this.stComboBox1.ReadOnly = true;
            this.stComboBox1.Size = new System.Drawing.Size(190, 21);
            this.stComboBox1.TabIndex = 0;
            this.stComboBox1.SelectedIndexChanged += new System.EventHandler(this.stComboBox1_SelectedIndexChanged);
            // 
            // ArchiveFilePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.stPanel1);
            this.Name = "ArchiveFilePanel";
            this.Size = new System.Drawing.Size(369, 309);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STPanel stPanel1;
        private STPanel stPanel2;
        private STLabel stLabel1;
        private STComboBox stComboBox1;
    }
}
