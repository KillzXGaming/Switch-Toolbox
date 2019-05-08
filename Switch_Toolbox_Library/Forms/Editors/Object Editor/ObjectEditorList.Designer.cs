namespace Switch_Toolbox.Library.Forms
{
    partial class ObjectEditorList
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
            this.components = new System.ComponentModel.Container();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.treeViewCustom1 = new Switch_Toolbox.Library.TreeViewCustom();
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            this.searchLbl = new Switch_Toolbox.Library.Forms.STLabel();
            this.searchImgPB = new System.Windows.Forms.PictureBox();
            this.stTextBox1 = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stContextMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.activeEditorChkBox = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stPanel1.SuspendLayout();
            this.stPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchImgPB)).BeginInit();
            this.stContextMenuStrip1.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.activeEditorChkBox);
            this.stPanel1.Controls.Add(this.treeViewCustom1);
            this.stPanel1.Controls.Add(this.stPanel3);
            this.stPanel1.Controls.Add(this.stContextMenuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(322, 542);
            this.stPanel1.TabIndex = 11;
            this.stPanel1.Resize += new System.EventHandler(this.stPanel1_Resize);
            // 
            // treeViewCustom1
            // 
            this.treeViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewCustom1.HotTracking = true;
            this.treeViewCustom1.ImageIndex = 0;
            this.treeViewCustom1.Location = new System.Drawing.Point(1, 53);
            this.treeViewCustom1.Name = "treeViewCustom1";
            this.treeViewCustom1.SelectedImageIndex = 0;
            this.treeViewCustom1.ShowLines = false;
            this.treeViewCustom1.ShowRootLines = false;
            this.treeViewCustom1.Size = new System.Drawing.Size(319, 488);
            this.treeViewCustom1.TabIndex = 0;
            this.treeViewCustom1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCustom1_AfterSelect);
            this.treeViewCustom1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeViewCustom1_DoubleClick);
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.searchLbl);
            this.stPanel3.Controls.Add(this.searchImgPB);
            this.stPanel3.Controls.Add(this.stTextBox1);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel3.Location = new System.Drawing.Point(0, 24);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(322, 26);
            this.stPanel3.TabIndex = 2;
            // 
            // searchLbl
            // 
            this.searchLbl.AutoSize = true;
            this.searchLbl.ForeColor = System.Drawing.Color.Silver;
            this.searchLbl.Location = new System.Drawing.Point(9, 5);
            this.searchLbl.Name = "searchLbl";
            this.searchLbl.Size = new System.Drawing.Size(41, 13);
            this.searchLbl.TabIndex = 2;
            this.searchLbl.Text = "Search";
            // 
            // searchImgPB
            // 
            this.searchImgPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchImgPB.BackColor = System.Drawing.Color.Transparent;
            this.searchImgPB.Image = global::Switch_Toolbox.Library.Properties.Resources.Antu_edit_find_mail_svg;
            this.searchImgPB.Location = new System.Drawing.Point(-646, 5);
            this.searchImgPB.Name = "searchImgPB";
            this.searchImgPB.Size = new System.Drawing.Size(22, 17);
            this.searchImgPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.searchImgPB.TabIndex = 1;
            this.searchImgPB.TabStop = false;
            // 
            // stTextBox1
            // 
            this.stTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(3, 3);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(319, 20);
            this.stTextBox1.TabIndex = 0;
            this.stTextBox1.Click += new System.EventHandler(this.stTextBox1_Click);
            this.stTextBox1.TextChanged += new System.EventHandler(this.stTextBox1_TextChanged);
            this.stTextBox1.Leave += new System.EventHandler(this.stTextBox1_Leave);
            // 
            // stContextMenuStrip1
            // 
            this.stContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.stContextMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stContextMenuStrip1.Name = "stContextMenuStrip1";
            this.stContextMenuStrip1.Size = new System.Drawing.Size(322, 24);
            this.stContextMenuStrip1.TabIndex = 1;
            this.stContextMenuStrip1.Text = "stContextMenuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.openToolStripMenuItem.Text = "Add File";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            this.sortToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
            this.sortToolStripMenuItem.Text = "Sort";
            this.sortToolStripMenuItem.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.listViewCustom1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(322, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(585, 542);
            this.stPanel2.TabIndex = 12;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(585, 542);
            this.listViewCustom1.TabIndex = 0;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 268;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.Width = 89;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 74;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "State";
            this.columnHeader4.Width = 154;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(322, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 542);
            this.splitter1.TabIndex = 13;
            this.splitter1.TabStop = false;
            this.splitter1.LocationChanged += new System.EventHandler(this.splitter1_LocationChanged);
            this.splitter1.Resize += new System.EventHandler(this.splitter1_Resize);
            // 
            // activeEditorChkBox
            // 
            this.activeEditorChkBox.AutoSize = true;
            this.activeEditorChkBox.Location = new System.Drawing.Point(140, 4);
            this.activeEditorChkBox.Name = "activeEditorChkBox";
            this.activeEditorChkBox.Size = new System.Drawing.Size(144, 17);
            this.activeEditorChkBox.TabIndex = 4;
            this.activeEditorChkBox.Text = "Add Files to Active Editor";
            this.activeEditorChkBox.UseVisualStyleBackColor = true;
            // 
            // ObjectEditorList
            // 
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.stPanel1);
            this.Name = "ObjectEditorList";
            this.Size = new System.Drawing.Size(907, 542);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stPanel3.ResumeLayout(false);
            this.stPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchImgPB)).EndInit();
            this.stContextMenuStrip1.ResumeLayout(false);
            this.stContextMenuStrip1.PerformLayout();
            this.stPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        public STPanel stPanel2;
        private STPanel stPanel1;
        private STPanel stPanel3;
        private TreeViewCustom treeViewCustom1;
        private STMenuStrip stContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private STTextBox stTextBox1;
        private System.Windows.Forms.PictureBox searchImgPB;
        private STLabel searchLbl;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem;
        private ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private STCheckBox activeEditorChkBox;
    }
}