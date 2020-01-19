namespace LayoutBXLYT
{
    partial class LytAnimationWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.animationCB = new Toolbox.Library.Forms.STComboBox();
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.stPropertyGrid1 = new Toolbox.Library.Forms.STPropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.contentContainer.SuspendLayout();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Size = new System.Drawing.Size(901, 633);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.splitContainer1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 25);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(901, 608);
            this.stPanel1.TabIndex = 11;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.stPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.stTabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(901, 608);
            this.splitContainer1.SplitterDistance = 248;
            this.splitContainer1.TabIndex = 0;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.treeView1);
            this.stPanel2.Controls.Add(this.animationCB);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(248, 608);
            this.stPanel2.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.Location = new System.Drawing.Point(0, 28);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(245, 580);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // animationCB
            // 
            this.animationCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationCB.BorderColor = System.Drawing.Color.Empty;
            this.animationCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.animationCB.ButtonColor = System.Drawing.Color.Empty;
            this.animationCB.FormattingEnabled = true;
            this.animationCB.IsReadOnly = false;
            this.animationCB.Location = new System.Drawing.Point(0, 3);
            this.animationCB.Name = "animationCB";
            this.animationCB.Size = new System.Drawing.Size(245, 21);
            this.animationCB.TabIndex = 1;
            this.animationCB.SelectedIndexChanged += new System.EventHandler(this.animationCB_SelectedIndexChanged);
            // 
            // stTabControl1
            // 
            this.stTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Location = new System.Drawing.Point(3, 3);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(643, 605);
            this.stTabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.stPropertyGrid1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(635, 576);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Key Editor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.ShowHintDisplay = true;
            this.stPropertyGrid1.Size = new System.Drawing.Size(629, 570);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(635, 576);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Curve Viewer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // LytAnimationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 638);
            this.Name = "LytAnimationWindow";
            this.Text = "Pane Animation Window";
            this.contentContainer.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STPanel stPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STComboBox animationCB;
        private System.Windows.Forms.TreeView treeView1;
        private Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
    }
}