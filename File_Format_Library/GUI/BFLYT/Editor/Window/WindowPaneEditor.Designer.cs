namespace LayoutBXLYT
{
    partial class WindowPaneEditor
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
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.windowFrameEditor1 = new LayoutBXLYT.WindowFrameEditorSettings();
            this.SuspendLayout();
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(0, 115);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(390, 275);
            this.stPanel1.TabIndex = 1;
            // 
            // windowFrameEditor1
            // 
            this.windowFrameEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.windowFrameEditor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.windowFrameEditor1.Location = new System.Drawing.Point(3, 0);
            this.windowFrameEditor1.Name = "windowFrameEditor1";
            this.windowFrameEditor1.Size = new System.Drawing.Size(387, 112);
            this.windowFrameEditor1.TabIndex = 0;
            // 
            // WindowPaneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.windowFrameEditor1);
            this.Name = "WindowPaneEditor";
            this.Size = new System.Drawing.Size(393, 393);
            this.ResumeLayout(false);

        }

        #endregion

        private WindowFrameEditorSettings windowFrameEditor1;
        private Toolbox.Library.Forms.STPanel stPanel1;
    }
}
