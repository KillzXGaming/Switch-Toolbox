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
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.miniToolStrip = new Toolbox.Library.Forms.STToolStrip();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel4
            // 
            this.stPanel4.Controls.Add(this.stToolStrip1);
            this.stPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel4.Location = new System.Drawing.Point(0, 0);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(462, 31);
            this.stPanel4.TabIndex = 2;
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AccessibleName = "New item selection";
            this.miniToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ButtonDropDown;
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.HighlightSelectedTab = false;
            this.miniToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.miniToolStrip.Location = new System.Drawing.Point(0, 0);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.Size = new System.Drawing.Size(411, 0);
            this.miniToolStrip.TabIndex = 0;
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.stToolStrip1.HighlightSelectedTab = false;
            this.stToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(462, 31);
            this.stToolStrip1.TabIndex = 0;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(0, 31);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(462, 298);
            this.stPanel1.TabIndex = 1;
            // 
            // PaneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 324);
            this.Controls.Add(this.stPanel4);
            this.Controls.Add(this.stPanel1);
            this.Name = "PaneEditor";
            this.stPanel4.ResumeLayout(false);
            this.stPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STPanel stPanel4;
        private Toolbox.Library.Forms.STToolStrip miniToolStrip;
        private Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private Toolbox.Library.Forms.STPanel stPanel1;
    }
}
