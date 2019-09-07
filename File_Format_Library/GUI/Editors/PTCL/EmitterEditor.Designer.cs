namespace FirstPlugin.Forms
{
    partial class EmitterEditor
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
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPageData = new System.Windows.Forms.TabPage();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.constant1Panel = new ColorConstantPanel();
            this.constant0Panel = new ColorConstantPanel();
            this.colorEditor = new Toolbox.Library.Forms.Editors.STColorEditor();
            this.alpha1ArrayEditor = new ColorArrayEditor();
            this.color1ArrayEditor = new ColorArrayEditor();
            this.alpha0ArrayEditor = new ColorArrayEditor();
            this.color0ArrayEditor = new ColorArrayEditor();
            this.scaleArrayEditor = new ColorArrayEditor();
            this.constantsLabel = new System.Windows.Forms.Label();
            this.stTabControl1.SuspendLayout();
            this.tabPageData.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPageData);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl1.Location = new System.Drawing.Point(0, 0);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(576, 561);
            this.stTabControl1.TabIndex = 38;
            // 
            // tabPageData
            // 
            this.tabPageData.Controls.Add(this.stPanel2);
            this.tabPageData.Location = new System.Drawing.Point(4, 25);
            this.tabPageData.Name = "tabPageData";
            this.tabPageData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageData.Size = new System.Drawing.Size(568, 532);
            this.tabPageData.TabIndex = 0;
            this.tabPageData.Text = "Emitter Data";
            this.tabPageData.UseVisualStyleBackColor = true;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.constantsLabel);
            this.stPanel2.Controls.Add(this.scaleArrayEditor);
            this.stPanel2.Controls.Add(this.constant1Panel);
            this.stPanel2.Controls.Add(this.constant0Panel);
            this.stPanel2.Controls.Add(this.colorEditor);
            this.stPanel2.Controls.Add(this.alpha1ArrayEditor);
            this.stPanel2.Controls.Add(this.color1ArrayEditor);
            this.stPanel2.Controls.Add(this.alpha0ArrayEditor);
            this.stPanel2.Controls.Add(this.color0ArrayEditor);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(3, 3);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(562, 526);
            this.stPanel2.TabIndex = 0;
            // 
            // constant1Panel
            // 
            this.constant1Panel.IsAlpha = false;
            this.constant1Panel.Location = new System.Drawing.Point(41, 341);
            this.constant1Panel.Name = "constant1Panel";
            this.constant1Panel.Size = new System.Drawing.Size(291, 37);
            this.constant1Panel.TabIndex = 65;
            // 
            // constant0Panel
            // 
            this.constant0Panel.IsAlpha = false;
            this.constant0Panel.Location = new System.Drawing.Point(3, 341);
            this.constant0Panel.Name = "constant0Panel";
            this.constant0Panel.Size = new System.Drawing.Size(291, 37);
            this.constant0Panel.TabIndex = 64;
            // 
            // colorEditor
            // 
            this.colorEditor.Location = new System.Drawing.Point(317, 3);
            this.colorEditor.Name = "colorEditor";
            this.colorEditor.Size = new System.Drawing.Size(227, 226);
            this.colorEditor.TabIndex = 63;
            // 
            // alpha1ArrayEditor
            // 
            this.alpha1ArrayEditor.Label = "Alpha 1";
            this.alpha1ArrayEditor.Location = new System.Drawing.Point(3, 198);
            this.alpha1ArrayEditor.Name = "alpha1ArrayEditor";
            this.alpha1ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.alpha1ArrayEditor.TabIndex = 62;
            // 
            // color1ArrayEditor
            // 
            this.color1ArrayEditor.Label = "Color 1";
            this.color1ArrayEditor.Location = new System.Drawing.Point(3, 133);
            this.color1ArrayEditor.Name = "color1ArrayEditor";
            this.color1ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.color1ArrayEditor.TabIndex = 61;
            // 
            // alpha0ArrayEditor
            // 
            this.alpha0ArrayEditor.Label = "Alpha 0";
            this.alpha0ArrayEditor.Location = new System.Drawing.Point(3, 68);
            this.alpha0ArrayEditor.Name = "alpha0ArrayEditor";
            this.alpha0ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.alpha0ArrayEditor.TabIndex = 60;
            // 
            // color0ArrayEditor
            // 
            this.color0ArrayEditor.Label = "Color 0";
            this.color0ArrayEditor.Location = new System.Drawing.Point(3, 3);
            this.color0ArrayEditor.Name = "color0ArrayEditor";
            this.color0ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.color0ArrayEditor.TabIndex = 0;
            // 
            // scaleArrayEditor
            // 
            this.scaleArrayEditor.Label = "Scale";
            this.scaleArrayEditor.Location = new System.Drawing.Point(3, 263);
            this.scaleArrayEditor.Name = "scaleArrayEditor";
            this.scaleArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.scaleArrayEditor.TabIndex = 66;
            // 
            // constantsLabel
            // 
            this.constantsLabel.AutoSize = true;
            this.constantsLabel.Location = new System.Drawing.Point(3, 325);
            this.constantsLabel.Name = "constantsLabel";
            this.constantsLabel.Size = new System.Drawing.Size(81, 13);
            this.constantsLabel.TabIndex = 67;
            this.constantsLabel.Text = "Color Constants";
            // 
            // EmitterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTabControl1);
            this.Name = "EmitterEditor";
            this.Size = new System.Drawing.Size(576, 561);
            this.stTabControl1.ResumeLayout(false);
            this.tabPageData.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPageData;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Forms.ColorArrayEditor color0ArrayEditor;
        private Forms.ColorArrayEditor alpha1ArrayEditor;
        private Forms.ColorArrayEditor color1ArrayEditor;
        private Forms.ColorArrayEditor alpha0ArrayEditor;
        private Toolbox.Library.Forms.Editors.STColorEditor colorEditor;
        private ColorConstantPanel constant1Panel;
        private ColorConstantPanel constant0Panel;
        private System.Windows.Forms.Label constantsLabel;
        private ColorArrayEditor scaleArrayEditor;
    }
}
