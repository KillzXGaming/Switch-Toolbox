namespace Toolbox.Library.Forms
{
    partial class ViewportDivider
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
            this.editorPanel = new Toolbox.Library.Forms.STPanel();
            this.viewportPanel = new Toolbox.Library.Forms.STPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // editorPanel
            // 
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(638, 0);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(202, 575);
            this.editorPanel.TabIndex = 1;
            // 
            // viewportPanel
            // 
            this.viewportPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.viewportPanel.Location = new System.Drawing.Point(0, 0);
            this.viewportPanel.Name = "viewportPanel";
            this.viewportPanel.Size = new System.Drawing.Size(638, 575);
            this.viewportPanel.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(638, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 575);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // ViewportDivider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.editorPanel);
            this.Controls.Add(this.viewportPanel);
            this.Name = "ViewportDivider";
            this.Size = new System.Drawing.Size(840, 575);
            this.ResumeLayout(false);

        }

        #endregion
        public STPanel editorPanel;
        public STPanel viewportPanel;
        private System.Windows.Forms.Splitter splitter1;
    }
}
