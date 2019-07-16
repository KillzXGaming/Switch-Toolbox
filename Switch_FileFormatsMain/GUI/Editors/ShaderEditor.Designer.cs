namespace FirstPlugin.Forms
{
    partial class ShaderEditor
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.hexVertexData = new Toolbox.Library.Forms.HexEditor();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.hexTextData = new Toolbox.Library.Forms.TextEditor();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.hexPixelData = new Toolbox.Library.Forms.HexEditor();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pixelTextData = new Toolbox.Library.Forms.TextEditor();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.hexGeomData = new Toolbox.Library.Forms.HexEditor();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.geomTextData = new Toolbox.Library.Forms.TextEditor();
            this.textEditor1 = new Toolbox.Library.Forms.TextEditor();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // stTabControl1
            // 
            this.stTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage3);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Controls.Add(this.tabPage4);
            this.stTabControl1.Location = new System.Drawing.Point(3, 3);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(542, 800);
            this.stTabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textEditor1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(534, 771);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Shader Data";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.hexVertexData);
            this.tabPage3.Controls.Add(this.splitter1);
            this.tabPage3.Controls.Add(this.hexTextData);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(534, 771);
            this.tabPage3.TabIndex = 7;
            this.tabPage3.Text = "Vertex Shader";
            // 
            // hexVertexData
            // 
            this.hexVertexData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexVertexData.Location = new System.Drawing.Point(0, 374);
            this.hexVertexData.Name = "hexVertexData";
            this.hexVertexData.Size = new System.Drawing.Size(534, 397);
            this.hexVertexData.TabIndex = 3;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 371);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(534, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // hexTextData
            // 
            this.hexTextData.Dock = System.Windows.Forms.DockStyle.Top;
            this.hexTextData.Location = new System.Drawing.Point(0, 0);
            this.hexTextData.Name = "hexTextData";
            this.hexTextData.Size = new System.Drawing.Size(534, 371);
            this.hexTextData.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.hexPixelData);
            this.tabPage2.Controls.Add(this.splitter2);
            this.tabPage2.Controls.Add(this.pixelTextData);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(534, 771);
            this.tabPage2.TabIndex = 8;
            this.tabPage2.Text = "Fragment Shader";
            // 
            // hexPixelData
            // 
            this.hexPixelData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexPixelData.Location = new System.Drawing.Point(0, 374);
            this.hexPixelData.Name = "hexPixelData";
            this.hexPixelData.Size = new System.Drawing.Size(534, 397);
            this.hexPixelData.TabIndex = 4;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 371);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(534, 3);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // pixelTextData
            // 
            this.pixelTextData.Dock = System.Windows.Forms.DockStyle.Top;
            this.pixelTextData.Location = new System.Drawing.Point(0, 0);
            this.pixelTextData.Name = "pixelTextData";
            this.pixelTextData.Size = new System.Drawing.Size(534, 371);
            this.pixelTextData.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.hexGeomData);
            this.tabPage4.Controls.Add(this.splitter3);
            this.tabPage4.Controls.Add(this.geomTextData);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(534, 771);
            this.tabPage4.TabIndex = 9;
            this.tabPage4.Text = "Geometry Shader";
            // 
            // hexGeomData
            // 
            this.hexGeomData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexGeomData.Location = new System.Drawing.Point(0, 374);
            this.hexGeomData.Name = "hexGeomData";
            this.hexGeomData.Size = new System.Drawing.Size(534, 397);
            this.hexGeomData.TabIndex = 4;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(0, 371);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(534, 3);
            this.splitter3.TabIndex = 3;
            this.splitter3.TabStop = false;
            // 
            // geomTextData
            // 
            this.geomTextData.Dock = System.Windows.Forms.DockStyle.Top;
            this.geomTextData.Location = new System.Drawing.Point(0, 0);
            this.geomTextData.Name = "geomTextData";
            this.geomTextData.Size = new System.Drawing.Size(534, 371);
            this.geomTextData.TabIndex = 2;
            // 
            // textEditor1
            // 
            this.textEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor1.Location = new System.Drawing.Point(3, 3);
            this.textEditor1.Name = "textEditor1";
            this.textEditor1.Size = new System.Drawing.Size(528, 765);
            this.textEditor1.TabIndex = 0;
            // 
            // ShaderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTabControl1);
            this.Name = "ShaderEditor";
            this.Size = new System.Drawing.Size(558, 803);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage1;
        private Toolbox.Library.Forms.TextEditor hexTextData;
        private Toolbox.Library.Forms.HexEditor hexVertexData;
        private System.Windows.Forms.Splitter splitter1;
        private Toolbox.Library.Forms.HexEditor hexPixelData;
        private System.Windows.Forms.Splitter splitter2;
        private Toolbox.Library.Forms.TextEditor pixelTextData;
        private Toolbox.Library.Forms.HexEditor hexGeomData;
        private System.Windows.Forms.Splitter splitter3;
        private Toolbox.Library.Forms.TextEditor geomTextData;
        private Toolbox.Library.Forms.TextEditor textEditor1;
    }
}
