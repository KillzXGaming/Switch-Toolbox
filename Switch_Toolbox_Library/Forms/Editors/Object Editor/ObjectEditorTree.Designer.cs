namespace Toolbox.Library.Forms
{
    partial class ObjectEditorTree
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
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.nodeSizeCB = new Toolbox.Library.Forms.STComboBox();
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewCustom1 = new Toolbox.Library.TreeViewCustom();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.searchFormToolStrip = new System.Windows.Forms.ToolStripButton();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.searchImgPB = new System.Windows.Forms.PictureBox();
            this.activeEditorChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.objectEditorMenu = new Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockSearchListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeNodeContextMenu = new Toolbox.Library.Forms.STContextMenuStrip(this.components);
            this.stPanel1.SuspendLayout();
            this.stPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stToolStrip1.SuspendLayout();
            this.stPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchImgPB)).BeginInit();
            this.objectEditorMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(314, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 542);
            this.splitter1.TabIndex = 13;
            this.splitter1.TabStop = false;
            this.splitter1.LocationChanged += new System.EventHandler(this.splitter1_LocationChanged);
            this.splitter1.Resize += new System.EventHandler(this.splitter1_Resize);
            // 
            // stPanel2
            // 
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(314, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(593, 542);
            this.stPanel2.TabIndex = 12;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.nodeSizeCB);
            this.stPanel1.Controls.Add(this.stPanel4);
            this.stPanel1.Controls.Add(this.stToolStrip1);
            this.stPanel1.Controls.Add(this.stPanel3);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(314, 542);
            this.stPanel1.TabIndex = 11;
            this.stPanel1.Resize += new System.EventHandler(this.stPanel1_Resize);
            // 
            // nodeSizeCB
            // 
            this.nodeSizeCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nodeSizeCB.BorderColor = System.Drawing.Color.Empty;
            this.nodeSizeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.nodeSizeCB.ButtonColor = System.Drawing.Color.Empty;
            this.nodeSizeCB.FormattingEnabled = true;
            this.nodeSizeCB.Location = new System.Drawing.Point(172, 29);
            this.nodeSizeCB.Name = "nodeSizeCB";
            this.nodeSizeCB.ReadOnly = true;
            this.nodeSizeCB.Size = new System.Drawing.Size(136, 21);
            this.nodeSizeCB.TabIndex = 5;
            this.nodeSizeCB.SelectedIndexChanged += new System.EventHandler(this.nodeSizeCB_SelectedIndexChanged);
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.Controls.Add(this.splitContainer1);
            this.stPanel4.Location = new System.Drawing.Point(3, 54);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(305, 485);
            this.stPanel4.TabIndex = 4;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer1.Panel1Collapsed = true;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeViewCustom1);
            this.splitContainer1.Size = new System.Drawing.Size(305, 485);
            this.splitContainer1.SplitterDistance = 201;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeViewCustom1
            // 
            this.treeViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewCustom1.CheckBoxes = true;
            this.treeViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCustom1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.treeViewCustom1.ImageIndex = 0;
            this.treeViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.treeViewCustom1.Name = "treeViewCustom1";
            this.treeViewCustom1.SelectedImageIndex = 0;
            this.treeViewCustom1.Size = new System.Drawing.Size(305, 485);
            this.treeViewCustom1.TabIndex = 0;
            this.treeViewCustom1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCustom1_AfterCheck);
            this.treeViewCustom1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCustom1_DrawNode);
            this.treeViewCustom1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCustom1_AfterSelect);
            this.treeViewCustom1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewCustom1_MouseClick);
            this.treeViewCustom1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewCustom1_DragDrop);
            this.treeViewCustom1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewCustom1_DragOver);
            this.treeViewCustom1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewCustom1_KeyPress);
            this.treeViewCustom1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeViewCustom1_DoubleClick);
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.searchFormToolStrip});
            this.stToolStrip1.Location = new System.Drawing.Point(0, 26);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(314, 25);
            this.stToolStrip1.TabIndex = 3;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Toolbox.Library.Properties.Resources.AddIcon;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // searchFormToolStrip
            // 
            this.searchFormToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchFormToolStrip.Image = global::Toolbox.Library.Properties.Resources.Antu_edit_find_mail1;
            this.searchFormToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchFormToolStrip.Name = "searchFormToolStrip";
            this.searchFormToolStrip.Size = new System.Drawing.Size(23, 22);
            this.searchFormToolStrip.Text = "toolStripButton1";
            this.searchFormToolStrip.Click += new System.EventHandler(this.searchFormToolStrip_Click);
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.searchImgPB);
            this.stPanel3.Controls.Add(this.activeEditorChkBox);
            this.stPanel3.Controls.Add(this.objectEditorMenu);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel3.Location = new System.Drawing.Point(0, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(314, 26);
            this.stPanel3.TabIndex = 2;
            // 
            // searchImgPB
            // 
            this.searchImgPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchImgPB.BackColor = System.Drawing.Color.Transparent;
            this.searchImgPB.Image = global::Toolbox.Library.Properties.Resources.Antu_edit_find_mail_svg;
            this.searchImgPB.Location = new System.Drawing.Point(-654, 5);
            this.searchImgPB.Name = "searchImgPB";
            this.searchImgPB.Size = new System.Drawing.Size(22, 17);
            this.searchImgPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.searchImgPB.TabIndex = 1;
            this.searchImgPB.TabStop = false;
            // 
            // activeEditorChkBox
            // 
            this.activeEditorChkBox.AutoSize = true;
            this.activeEditorChkBox.Location = new System.Drawing.Point(137, 6);
            this.activeEditorChkBox.Name = "activeEditorChkBox";
            this.activeEditorChkBox.Size = new System.Drawing.Size(144, 17);
            this.activeEditorChkBox.TabIndex = 3;
            this.activeEditorChkBox.Text = "Add Files to Active Editor";
            this.activeEditorChkBox.UseVisualStyleBackColor = true;
            this.activeEditorChkBox.CheckedChanged += new System.EventHandler(this.activeEditorChkBox_CheckedChanged);
            // 
            // objectEditorMenu
            // 
            this.objectEditorMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectEditorMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.objectEditorMenu.Location = new System.Drawing.Point(0, 0);
            this.objectEditorMenu.Name = "objectEditorMenu";
            this.objectEditorMenu.Size = new System.Drawing.Size(314, 26);
            this.objectEditorMenu.TabIndex = 1;
            this.objectEditorMenu.Text = "stContextMenuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
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
            this.sortToolStripMenuItem,
            this.dockSearchListToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            this.sortToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.sortToolStripMenuItem.Text = "Sort";
            this.sortToolStripMenuItem.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
            // 
            // dockSearchListToolStripMenuItem
            // 
            this.dockSearchListToolStripMenuItem.CheckOnClick = true;
            this.dockSearchListToolStripMenuItem.Name = "dockSearchListToolStripMenuItem";
            this.dockSearchListToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.dockSearchListToolStripMenuItem.Text = "Dock Search List";
            this.dockSearchListToolStripMenuItem.Click += new System.EventHandler(this.dockSearchListToolStripMenuItem_Click);
            // 
            // treeNodeContextMenu
            // 
            this.treeNodeContextMenu.Name = "treeNodeContextMenu";
            this.treeNodeContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // ObjectEditorTree
            // 
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.stPanel1);
            this.Name = "ObjectEditorTree";
            this.Size = new System.Drawing.Size(907, 542);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stPanel4.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            this.stPanel3.ResumeLayout(false);
            this.stPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchImgPB)).EndInit();
            this.objectEditorMenu.ResumeLayout(false);
            this.objectEditorMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        public STPanel stPanel2;
        private STPanel stPanel1;
        private STPanel stPanel3;
        private TreeViewCustom treeViewCustom1;
        private System.Windows.Forms.PictureBox searchImgPB;
        private STCheckBox activeEditorChkBox;
        private STMenuStrip objectEditorMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem;
        private STContextMenuStrip treeNodeContextMenu;
        private STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton searchFormToolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private STPanel stPanel4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem dockSearchListToolStripMenuItem;
        private STComboBox nodeSizeCB;
    }
}