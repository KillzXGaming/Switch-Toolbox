namespace LayoutBXLYT
{
    partial class WindowFrameEditorSettings
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
            this.typeCB = new Toolbox.Library.Forms.STComboBox();
            this.frameLbl = new Toolbox.Library.Forms.STLabel();
            this.frameNumCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.chkRenderContent = new Toolbox.Library.Forms.STCheckBox();
            this.windowFrameSelector1 = new LayoutBXLYT.WindowFrameSelector();
            this.SuspendLayout();
            // 
            // typeCB
            // 
            this.typeCB.BorderColor = System.Drawing.Color.Empty;
            this.typeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.typeCB.ButtonColor = System.Drawing.Color.Empty;
            this.typeCB.FormattingEnabled = true;
            this.typeCB.IsReadOnly = false;
            this.typeCB.Items.AddRange(new object[] {
            "All Directions",
            "Horizontal",
            "Horizontal (No Content)"});
            this.typeCB.Location = new System.Drawing.Point(122, 37);
            this.typeCB.Name = "typeCB";
            this.typeCB.Size = new System.Drawing.Size(127, 21);
            this.typeCB.TabIndex = 1;
            this.typeCB.SelectedIndexChanged += new System.EventHandler(this.typeCB_SelectedIndexChanged);
            // 
            // frameLbl
            // 
            this.frameLbl.AutoSize = true;
            this.frameLbl.Location = new System.Drawing.Point(119, 9);
            this.frameLbl.Name = "frameLbl";
            this.frameLbl.Size = new System.Drawing.Size(130, 13);
            this.frameLbl.TabIndex = 2;
            this.frameLbl.Text = "Selected Frame: [Content]";
            // 
            // frameNumCB
            // 
            this.frameNumCB.BorderColor = System.Drawing.Color.Empty;
            this.frameNumCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.frameNumCB.ButtonColor = System.Drawing.Color.Empty;
            this.frameNumCB.FormattingEnabled = true;
            this.frameNumCB.IsReadOnly = false;
            this.frameNumCB.Items.AddRange(new object[] {
            "1 (Top Left)",
            "4 (Corners)",
            "8 (Corners + Sides)"});
            this.frameNumCB.Location = new System.Drawing.Point(122, 59);
            this.frameNumCB.Name = "frameNumCB";
            this.frameNumCB.Size = new System.Drawing.Size(127, 21);
            this.frameNumCB.TabIndex = 3;
            this.frameNumCB.SelectedIndexChanged += new System.EventHandler(this.frameNumCB_SelectedIndexChanged);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(3, 9);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(90, 13);
            this.stLabel2.TabIndex = 4;
            this.stLabel2.Text = "Window Settings:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(3, 40);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(34, 13);
            this.stLabel3.TabIndex = 5;
            this.stLabel3.Text = "Type:";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(3, 62);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(44, 13);
            this.stLabel4.TabIndex = 6;
            this.stLabel4.Text = "Frames:";
            // 
            // chkRenderContent
            // 
            this.chkRenderContent.AutoSize = true;
            this.chkRenderContent.Location = new System.Drawing.Point(122, 89);
            this.chkRenderContent.Name = "chkRenderContent";
            this.chkRenderContent.Size = new System.Drawing.Size(88, 17);
            this.chkRenderContent.TabIndex = 7;
            this.chkRenderContent.Text = "Hide Content";
            this.chkRenderContent.UseVisualStyleBackColor = true;
            this.chkRenderContent.CheckedChanged += new System.EventHandler(this.chkRenderContent_CheckedChanged);
            // 
            // windowFrameSelector1
            // 
            this.windowFrameSelector1.Location = new System.Drawing.Point(272, 6);
            this.windowFrameSelector1.Name = "windowFrameSelector1";
            this.windowFrameSelector1.Size = new System.Drawing.Size(77, 77);
            this.windowFrameSelector1.TabIndex = 0;
            // 
            // WindowFrameEditorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.chkRenderContent);
            this.Controls.Add(this.stLabel4);
            this.Controls.Add(this.stLabel3);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.frameNumCB);
            this.Controls.Add(this.frameLbl);
            this.Controls.Add(this.typeCB);
            this.Controls.Add(this.windowFrameSelector1);
            this.Name = "WindowFrameEditorSettings";
            this.Size = new System.Drawing.Size(353, 110);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WindowFrameSelector windowFrameSelector1;
        private Toolbox.Library.Forms.STComboBox typeCB;
        private Toolbox.Library.Forms.STLabel frameLbl;
        private Toolbox.Library.Forms.STComboBox frameNumCB;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STCheckBox chkRenderContent;
    }
}
