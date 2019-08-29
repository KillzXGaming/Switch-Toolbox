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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.stPanel5 = new Toolbox.Library.Forms.STPanel();
            this.nodeSizeCB = new Toolbox.Library.Forms.STComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewCustom1 = new Toolbox.Library.TreeViewCustom();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.searchFormToolStrip = new System.Windows.Forms.ToolStripButton();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.activeEditorChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.searchImgPB = new System.Windows.Forms.PictureBox();
            this.objectEditorMenu = new Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockSearchListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.btnPanelDisplay = new Toolbox.Library.Forms.STButton();
            this.treeNodeContextMenu = new Toolbox.Library.Forms.STContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.stPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stToolStrip1.SuspendLayout();
            this.stPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchImgPB)).BeginInit();
            this.objectEditorMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.stPanel5);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.stPanel2);
            this.splitContainer2.Panel2.Controls.Add(this.btnPanelDisplay);
            this.splitContainer2.Size = new System.Drawing.Size(907, 542);
            this.splitContainer2.SplitterDistance = 302;
            this.splitContainer2.TabIndex = 14;
            // 
            // stPanel5
            // 
            this.stPanel5.Controls.Add(this.nodeSizeCB);
            this.stPanel5.Controls.Add(this.splitContainer1);
            this.stPanel5.Controls.Add(this.stToolStrip1);
            this.stPanel5.Controls.Add(this.stPanel3);
            this.stPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel5.Location = new System.Drawing.Point(0, 0);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(302, 542);
            this.stPanel5.TabIndex = 2;
            // 
            // nodeSizeCB
            // 
            this.nodeSizeCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nodeSizeCB.BorderColor = System.Drawing.Color.Empty;
            this.nodeSizeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.nodeSizeCB.ButtonColor = System.Drawing.Color.Empty;
            this.nodeSizeCB.FormattingEnabled = true;
            this.nodeSizeCB.IsReadOnly = false;
            this.nodeSizeCB.Location = new System.Drawing.Point(138, 26);
            this.nodeSizeCB.Name = "nodeSizeCB";
            this.nodeSizeCB.Size = new System.Drawing.Size(144, 21);
            this.nodeSizeCB.TabIndex = 5;
            this.nodeSizeCB.SelectedIndexChanged += new System.EventHandler(this.nodeSizeCB_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 51);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Resize += new System.EventHandler(this.splitContainer1_Panel1_Resize);
            this.splitContainer1.Panel1Collapsed = true;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeViewCustom1);
            this.splitContainer1.Size = new System.Drawing.Size(302, 491);
            this.splitContainer1.SplitterDistance = 201;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeViewCustom1
            // 
            this.treeViewCustom1.AllowDrop = true;
            this.treeViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewCustom1.CheckBoxes = true;
            this.treeViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCustom1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.treeViewCustom1.ImageIndex = 0;
            this.treeViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.treeViewCustom1.Name = "treeViewCustom1";
            this.treeViewCustom1.SelectedImageIndex = 0;
            this.treeViewCustom1.Size = new System.Drawing.Size(302, 491);
            this.treeViewCustom1.TabIndex = 0;
            this.treeViewCustom1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCustom1_AfterCheck);
            this.treeViewCustom1.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCustom1_AfterCollapse);
            this.treeViewCustom1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCustom1_BeforeExpand);
            this.treeViewCustom1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCustom1_DrawNode);
            this.treeViewCustom1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeViewCustom1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCustom1_AfterSelect);
            this.treeViewCustom1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewCustom1_MouseClick);
            this.treeViewCustom1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewCustom1_DragDrop);
            this.treeViewCustom1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewCustom1_DragEnter);
            this.treeViewCustom1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewCustom1_DragOver);
            this.treeViewCustom1.DragLeave += new System.EventHandler(this.treeViewCustom1_DragLeave);
            this.treeViewCustom1.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.treeViewCustom1_GiveFeedback);
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
            this.stToolStrip1.Size = new System.Drawing.Size(302, 25);
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
            this.searchFormToolStrip.Image = global::Toolbox.Library.Properties.Resources.Antu_edit_find_mail_svg;
            this.searchFormToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchFormToolStrip.Name = "searchFormToolStrip";
            this.searchFormToolStrip.Size = new System.Drawing.Size(23, 22);
            this.searchFormToolStrip.Text = "toolStripButton1";
            this.searchFormToolStrip.Click += new System.EventHandler(this.searchFormToolStrip_Click);
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.activeEditorChkBox);
            this.stPanel3.Controls.Add(this.searchImgPB);
            this.stPanel3.Controls.Add(this.objectEditorMenu);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel3.Location = new System.Drawing.Point(0, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(302, 26);
            this.stPanel3.TabIndex = 2;
            // 
            // activeEditorChkBox
            // 
            this.activeEditorChkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.activeEditorChkBox.AutoSize = true;
            this.activeEditorChkBox.Location = new System.Drawing.Point(138, 3);
            this.activeEditorChkBox.Name = "activeEditorChkBox";
            this.activeEditorChkBox.Size = new System.Drawing.Size(144, 17);
            this.activeEditorChkBox.TabIndex = 3;
            this.activeEditorChkBox.Text = "Add Files to Active Editor";
            this.activeEditorChkBox.UseVisualStyleBackColor = true;
            this.activeEditorChkBox.CheckedChanged += new System.EventHandler(this.activeEditorChkBox_CheckedChanged);
            // 
            // searchImgPB
            // 
            this.searchImgPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchImgPB.BackColor = System.Drawing.Color.Transparent;
            this.searchImgPB.Image = global::Toolbox.Library.Properties.Resources.Antu_edit_find_mail_svg;
            this.searchImgPB.Location = new System.Drawing.Point(-666, 5);
            this.searchImgPB.Name = "searchImgPB";
            this.searchImgPB.Size = new System.Drawing.Size(22, 17);
            this.searchImgPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.searchImgPB.TabIndex = 1;
            this.searchImgPB.TabStop = false;
            // 
            // objectEditorMenu
            // 
            this.objectEditorMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectEditorMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.objectEditorMenu.Location = new System.Drawing.Point(0, 0);
            this.objectEditorMenu.Name = "objectEditorMenu";
            this.objectEditorMenu.Size = new System.Drawing.Size(302, 26);
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
            // stPanel2
            // 
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(14, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(587, 542);
            this.stPanel2.TabIndex = 12;
            // 
            // btnPanelDisplay
            // 
            this.btnPanelDisplay.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnPanelDisplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPanelDisplay.Location = new System.Drawing.Point(0, 0);
            this.btnPanelDisplay.Name = "btnPanelDisplay";
            this.btnPanelDisplay.Size = new System.Drawing.Size(14, 542);
            this.btnPanelDisplay.TabIndex = 3;
            this.btnPanelDisplay.Text = "<";
            this.btnPanelDisplay.UseVisualStyleBackColor = false;
            this.btnPanelDisplay.Click += new System.EventHandler(this.btnPanelDisplay_Click);
            // 
            // treeNodeContextMenu
            // 
            this.treeNodeContextMenu.Name = "treeNodeContextMenu";
            this.treeNodeContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // ObjectEditorTree
            // 
            this.Controls.Add(this.splitContainer2);
            this.Name = "ObjectEditorTree";
            this.Size = new System.Drawing.Size(907, 542);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.stPanel5.ResumeLayout(false);
            this.stPanel5.PerformLayout();
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
        public STPanel stPanel2;
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
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem dockSearchListToolStripMenuItem;
        private STComboBox nodeSizeCB;
        private STPanel stPanel5;
        private STButton btnPanelDisplay;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}