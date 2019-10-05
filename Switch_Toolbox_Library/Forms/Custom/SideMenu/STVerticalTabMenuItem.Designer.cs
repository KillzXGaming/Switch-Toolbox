namespace Toolbox.Library.Forms
{
    partial class STVerticalTabMenuItem
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
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(0, 9);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(47, 13);
            this.stLabel1.TabIndex = 0;
            this.stLabel1.Text = "stLabel1";
            this.stLabel1.Click += new System.EventHandler(this.stLabel1_Click);
            // 
            // stPanel1
            // 
            this.stPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stPanel1.Controls.Add(this.stLabel1);
            this.stPanel1.Location = new System.Drawing.Point(3, 3);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(120, 30);
            this.stPanel1.TabIndex = 1;
            // 
            // STVerticalTabMenuItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel1);
            this.Name = "STVerticalTabMenuItem";
            this.Size = new System.Drawing.Size(126, 36);
            this.Click += new System.EventHandler(this.STVerticalTabMenuItem_Click);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STLabel stLabel1;
        private STPanel stPanel1;
    }
}
