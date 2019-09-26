namespace FirstPlugin
{
    partial class ByamlEditor
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopyNode = new System.Windows.Forms.ToolStripMenuItem();
            this.addNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editValueNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importFromXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnToXml = new Toolbox.Library.Forms.STButton();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.btnXmlToByaml = new Toolbox.Library.Forms.STButton();
            this.stContextMenuStrip1 = new Toolbox.Library.Forms.STContextMenuStrip(this.components);
            this.addItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyDataAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.stContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyNode,
            this.addNodeToolStripMenuItem,
            this.editValueNodeMenuItem,
            this.deleteNodeToolStripMenuItem,
            this.exportJsonToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveAsToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator2,
            this.importFromXmlToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 192);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuOpening);
            // 
            // CopyNode
            // 
            this.CopyNode.Name = "CopyNode";
            this.CopyNode.Size = new System.Drawing.Size(168, 22);
            this.CopyNode.Text = "Copy node as text";
            this.CopyNode.Click += new System.EventHandler(this.CopyNode_Click);
            // 
            // addNodeToolStripMenuItem
            // 
            this.addNodeToolStripMenuItem.Name = "addNodeToolStripMenuItem";
            this.addNodeToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.addNodeToolStripMenuItem.Text = "Add Node";
            this.addNodeToolStripMenuItem.Click += new System.EventHandler(this.addNodeToolStripMenuItem_Click);
            // 
            // editValueNodeMenuItem
            // 
            this.editValueNodeMenuItem.Name = "editValueNodeMenuItem";
            this.editValueNodeMenuItem.Size = new System.Drawing.Size(168, 22);
            this.editValueNodeMenuItem.Text = "Edit value";
            this.editValueNodeMenuItem.Click += new System.EventHandler(this.editValueNodeMenuItem_Click);
            // 
            // deleteNodeToolStripMenuItem
            // 
            this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
            this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.deleteNodeToolStripMenuItem.Text = "Delete node";
            this.deleteNodeToolStripMenuItem.Click += new System.EventHandler(this.deleteNodeToolStripMenuItem_Click);
            // 
            // exportJsonToolStripMenuItem
            // 
            this.exportJsonToolStripMenuItem.Name = "exportJsonToolStripMenuItem";
            this.exportJsonToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportJsonToolStripMenuItem.Text = "Export as xml";
            this.exportJsonToolStripMenuItem.Click += new System.EventHandler(this.exportJsonToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveAsToolStripMenuItem.Text = "Save as..";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Visible = false;
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(165, 6);
            // 
            // importFromXmlToolStripMenuItem
            // 
            this.importFromXmlToolStripMenuItem.Name = "importFromXmlToolStripMenuItem";
            this.importFromXmlToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.importFromXmlToolStripMenuItem.Text = "Import from xml";
            this.importFromXmlToolStripMenuItem.Click += new System.EventHandler(this.importFromXmlToolStripMenuItem_Click);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stTabControl1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(549, 398);
            this.stPanel1.TabIndex = 15;
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl1.Location = new System.Drawing.Point(0, 0);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(549, 398);
            this.stTabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(541, 369);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Editor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewCustom1);
            this.splitContainer1.Size = new System.Drawing.Size(535, 363);
            this.splitContainer1.SplitterDistance = 178;
            this.splitContainer1.TabIndex = 14;
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(178, 363);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.BeforeExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCustom1.FullRowSelect = true;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(353, 363);
            this.listViewCustom1.TabIndex = 0;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewCustom1_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 114;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 93;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Data";
            this.columnHeader3.Width = 146;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnToXml);
            this.tabPage2.Controls.Add(this.stPanel2);
            this.tabPage2.Controls.Add(this.btnXmlToByaml);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(541, 369);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "XML Editor";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnToXml
            // 
            this.btnToXml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToXml.Location = new System.Drawing.Point(15, 6);
            this.btnToXml.Name = "btnToXml";
            this.btnToXml.Size = new System.Drawing.Size(107, 23);
            this.btnToXml.TabIndex = 2;
            this.btnToXml.Text = "Generate";
            this.btnToXml.UseVisualStyleBackColor = false;
            this.btnToXml.Click += new System.EventHandler(this.btnToXml_Click);
            // 
            // stPanel2
            // 
            this.stPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel2.Location = new System.Drawing.Point(6, 35);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(0, 0);
            this.stPanel2.TabIndex = 1;
            // 
            // btnXmlToByaml
            // 
            this.btnXmlToByaml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXmlToByaml.Location = new System.Drawing.Point(128, 6);
            this.btnXmlToByaml.Name = "btnXmlToByaml";
            this.btnXmlToByaml.Size = new System.Drawing.Size(107, 23);
            this.btnXmlToByaml.TabIndex = 0;
            this.btnXmlToByaml.Text = "Compile";
            this.btnXmlToByaml.UseVisualStyleBackColor = false;
            this.btnXmlToByaml.Click += new System.EventHandler(this.btnXmlToByaml_Click);
            // 
            // stContextMenuStrip1
            // 
            this.stContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addItemToolStripMenuItem,
            this.editToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.copyDataAsTextToolStripMenuItem});
            this.stContextMenuStrip1.Name = "stContextMenuStrip1";
            this.stContextMenuStrip1.Size = new System.Drawing.Size(181, 158);
            // 
            // addItemToolStripMenuItem
            // 
            this.addItemToolStripMenuItem.Name = "addItemToolStripMenuItem";
            this.addItemToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addItemToolStripMenuItem.Text = "Add Item";
            this.addItemToolStripMenuItem.Click += new System.EventHandler(this.addNodeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editValueNodeMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyToolStripMenuItem.Text = "Copy item as text";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // copyDataAsTextToolStripMenuItem
            // 
            this.copyDataAsTextToolStripMenuItem.Name = "copyDataAsTextToolStripMenuItem";
            this.copyDataAsTextToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyDataAsTextToolStripMenuItem.Text = "Copy data as text";
            this.copyDataAsTextToolStripMenuItem.Click += new System.EventHandler(this.copyDataAsTextToolStripMenuItem_Click);
            // 
            // ByamlEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel1);
            this.Name = "ByamlEditor";
            this.Size = new System.Drawing.Size(549, 398);
            this.Load += new System.EventHandler(this.ByamlViewer_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.stContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem CopyNode;
        private System.Windows.Forms.ToolStripMenuItem exportJsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editValueNodeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem importFromXmlToolStripMenuItem;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STContextMenuStrip stContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addItemToolStripMenuItem;
        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private Toolbox.Library.Forms.STButton btnXmlToByaml;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STButton btnToXml;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyDataAsTextToolStripMenuItem;
    }
}