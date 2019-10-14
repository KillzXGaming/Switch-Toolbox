namespace LayoutBXLYT
{
    partial class PaneEditor
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
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stPanel3.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.stPanel2);
            this.stPanel3.Controls.Add(this.stPanel1);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(0, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(411, 288);
            this.stPanel3.TabIndex = 2;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stToolStrip1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(411, 27);
            this.stPanel2.TabIndex = 0;
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.stToolStrip1.HighlightSelectedTab = false;
            this.stToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(411, 25);
            this.stToolStrip1.TabIndex = 0;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(3, 26);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(411, 262);
            this.stPanel1.TabIndex = 1;
            // 
            // PaneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 288);
            this.Controls.Add(this.stPanel3);
            this.Name = "PaneEditor";
            this.stPanel3.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private Toolbox.Library.Forms.STToolStrip stToolStrip1;
    }
}
