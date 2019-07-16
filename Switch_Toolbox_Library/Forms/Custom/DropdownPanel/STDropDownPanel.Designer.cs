namespace Toolbox.Library.Forms
{
    partial class STDropDownPanel
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
            this.stCollapsePanelButton1 = new Toolbox.Library.Forms.STCollapsePanelButton();
            this.SuspendLayout();
            // 
            // stCollapsePanelButton1
            // 
            this.stCollapsePanelButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
       | System.Windows.Forms.AnchorStyles.Right)));
            this.stCollapsePanelButton1.IsExpanded = false;
            this.stCollapsePanelButton1.Location = new System.Drawing.Point(0, 0);
            this.stCollapsePanelButton1.Name = "stCollapsePanelButton1";
            this.stCollapsePanelButton1.Size = new System.Drawing.Size(490, 20);
            this.stCollapsePanelButton1.TabIndex = 0;
            // 
            // STDropDownPanel
            // 
            this.Margin = System.Windows.Forms.Padding.Empty;
            this.Padding = System.Windows.Forms.Padding.Empty;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.stCollapsePanelButton1);
            this.Name = "STDropDownPanel";
            this.Size = new System.Drawing.Size(490, 311);
            this.ResumeLayout(false);

        }

        #endregion

        private STCollapsePanelButton stCollapsePanelButton1;
    }
}
