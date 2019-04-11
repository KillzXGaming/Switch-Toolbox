namespace FirstPlugin.Forms
{
    partial class TurboMunntEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPropertyGrid1 = new Switch_Toolbox.Library.Forms.STPropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stTabControl1 = new Switch_Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.stPanel4 = new Switch_Toolbox.Library.Forms.STPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.stMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.viewIntroCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.stMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.stPanel2);
            this.splitContainer1.Panel1.Controls.Add(this.splitter1);
            this.splitContainer1.Panel1.Controls.Add(this.stPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.stTabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.stMenuStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(959, 718);
            this.splitContainer1.SplitterDistance = 209;
            this.splitContainer1.TabIndex = 1;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stPropertyGrid1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 422);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(209, 296);
            this.stPanel2.TabIndex = 2;
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.ShowHintDisplay = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.Size = new System.Drawing.Size(209, 296);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 419);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(209, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.treeView1);
            this.stPanel1.Controls.Add(this.stButton2);
            this.stPanel1.Controls.Add(this.stButton1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(209, 419);
            this.stPanel1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(203, 381);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // stButton2
            // 
            this.stButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(52, 390);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(57, 23);
            this.stButton2.TabIndex = 2;
            this.stButton2.Text = "Remove";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(3, 390);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(43, 23);
            this.stButton1.TabIndex = 1;
            this.stButton1.Text = "Add";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stTabControl1
            // 
            this.stTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Location = new System.Drawing.Point(3, 27);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(740, 688);
            this.stTabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.stPanel4);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(732, 659);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "3D View";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // stPanel4
            // 
            this.stPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel4.Location = new System.Drawing.Point(3, 3);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(726, 653);
            this.stPanel4.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.stPanel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(732, 659);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "2D View";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // stMenuStrip1
            // 
            this.stMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewIntroCameraToolStripMenuItem});
            this.stMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stMenuStrip1.Name = "stMenuStrip1";
            this.stMenuStrip1.Size = new System.Drawing.Size(746, 24);
            this.stMenuStrip1.TabIndex = 0;
            this.stMenuStrip1.Text = "stMenuStrip1";
            // 
            // viewIntroCameraToolStripMenuItem
            // 
            this.viewIntroCameraToolStripMenuItem.Name = "viewIntroCameraToolStripMenuItem";
            this.viewIntroCameraToolStripMenuItem.Size = new System.Drawing.Size(116, 20);
            this.viewIntroCameraToolStripMenuItem.Text = "View Intro Camera";
            this.viewIntroCameraToolStripMenuItem.Click += new System.EventHandler(this.viewIntroCameraToolStripMenuItem_Click);
            // 
            // stPanel3
            // 
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(3, 3);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(726, 653);
            this.stPanel3.TabIndex = 0;
            // 
            // TurboMunntEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "TurboMunntEditor";
            this.Size = new System.Drawing.Size(959, 718);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.stMenuStrip1.ResumeLayout(false);
            this.stMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel2;
        private Switch_Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
        private Switch_Toolbox.Library.Forms.STButton stButton2;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STMenuStrip stMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewIntroCameraToolStripMenuItem;
        private Switch_Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private Switch_Toolbox.Library.Forms.STPanel stPanel4;
        private System.Windows.Forms.TreeView treeView1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel3;
    }
}
