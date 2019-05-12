namespace FirstPlugin.Forms
{
    partial class BcresSamplerEditorSimple
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SamplerEditorSimple));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.textureBP = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stTextBox2 = new Switch_Toolbox.Library.Forms.STTextBox();
            this.samplerCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.nameTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stPropertyGrid1 = new Switch_Toolbox.Library.Forms.STPropertyGrid();
            this.stMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureBP)).BeginInit();
            this.stMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.stPanel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.stPropertyGrid1);
            this.splitContainer2.Size = new System.Drawing.Size(585, 522);
            this.splitContainer2.SplitterDistance = 277;
            this.splitContainer2.TabIndex = 0;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.textureBP);
            this.stPanel1.Controls.Add(this.stLabel3);
            this.stPanel1.Controls.Add(this.stTextBox2);
            this.stPanel1.Controls.Add(this.samplerCB);
            this.stPanel1.Controls.Add(this.stLabel2);
            this.stPanel1.Controls.Add(this.stButton1);
            this.stPanel1.Controls.Add(this.nameTB);
            this.stPanel1.Controls.Add(this.stLabel1);
            this.stPanel1.Controls.Add(this.stMenuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(277, 522);
            this.stPanel1.TabIndex = 0;
            // 
            // textureBP
            // 
            this.textureBP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureBP.BackColor = System.Drawing.Color.Transparent;
            this.textureBP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textureBP.BackgroundImage")));
            this.textureBP.Location = new System.Drawing.Point(3, 129);
            this.textureBP.Name = "textureBP";
            this.textureBP.Size = new System.Drawing.Size(272, 390);
            this.textureBP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.textureBP.TabIndex = 7;
            this.textureBP.TabStop = false;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(3, 106);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(29, 13);
            this.stLabel3.TabIndex = 6;
            this.stLabel3.Text = "Hint:";
            // 
            // stTextBox2
            // 
            this.stTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox2.Location = new System.Drawing.Point(65, 104);
            this.stTextBox2.Name = "stTextBox2";
            this.stTextBox2.Size = new System.Drawing.Size(207, 20);
            this.stTextBox2.TabIndex = 5;
            // 
            // samplerCB
            // 
            this.samplerCB.BorderColor = System.Drawing.Color.Empty;
            this.samplerCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.samplerCB.ButtonColor = System.Drawing.Color.Empty;
            this.samplerCB.FormattingEnabled = true;
            this.samplerCB.Location = new System.Drawing.Point(66, 77);
            this.samplerCB.Name = "samplerCB";
            this.samplerCB.ReadOnly = true;
            this.samplerCB.Size = new System.Drawing.Size(206, 21);
            this.samplerCB.TabIndex = 4;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(1, 80);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(48, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Sampler:";
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(218, 48);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(54, 23);
            this.stButton1.TabIndex = 2;
            this.stButton1.Text = "Edit:";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // nameTB
            // 
            this.nameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTB.Location = new System.Drawing.Point(6, 51);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(207, 20);
            this.nameTB.TabIndex = 1;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(3, 29);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(46, 13);
            this.stLabel1.TabIndex = 0;
            this.stLabel1.Text = "Texture:";
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.ShowHintDisplay = true;
            this.stPropertyGrid1.Size = new System.Drawing.Size(304, 522);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // stMenuStrip1
            // 
            this.stMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem});
            this.stMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stMenuStrip1.Name = "stMenuStrip1";
            this.stMenuStrip1.Size = new System.Drawing.Size(277, 24);
            this.stMenuStrip1.TabIndex = 8;
            this.stMenuStrip1.Text = "stMenuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayVerticalToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // displayVerticalToolStripMenuItem
            // 
            this.displayVerticalToolStripMenuItem.Name = "displayVerticalToolStripMenuItem";
            this.displayVerticalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.displayVerticalToolStripMenuItem.Text = "Display Vertical";
            this.displayVerticalToolStripMenuItem.Click += new System.EventHandler(this.displayVerticalToolStripMenuItem_Click);
            // 
            // SamplerEditorSimple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "SamplerEditorSimple";
            this.Size = new System.Drawing.Size(585, 522);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureBP)).EndInit();
            this.stMenuStrip1.ResumeLayout(false);
            this.stMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
        private Switch_Toolbox.Library.Forms.PictureBoxCustom textureBP;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private Switch_Toolbox.Library.Forms.STTextBox stTextBox2;
        private Switch_Toolbox.Library.Forms.STComboBox samplerCB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STTextBox nameTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private Switch_Toolbox.Library.Forms.STMenuStrip stMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayVerticalToolStripMenuItem;
    }
}
