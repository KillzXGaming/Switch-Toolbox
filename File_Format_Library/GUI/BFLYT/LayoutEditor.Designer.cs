namespace LayoutBXLYT
{
    partial class LayoutEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutEditor));
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.backColorDisplay = new System.Windows.Forms.PictureBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.debugShading = new Toolbox.Library.Forms.STComboBox();
            this.viewportBackColorCB = new Toolbox.Library.Forms.STComboBox();
            this.stToolStrip1 = new Toolbox.Library.Forms.STToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolstripOrthoBtn = new System.Windows.Forms.ToolStripButton();
            this.stMenuStrip1 = new Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transformChildrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderInGamePreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPartsAsNullPanesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orthographicViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayNullPanesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayyBoundryPanesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayWindowPanesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayPicturePanesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayTextPanesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGameWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.editorModeCB = new Toolbox.Library.Forms.STComboBox();
            this.chkAutoKey = new Toolbox.Library.Forms.STCheckBox();
            this.showAnimationWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.backColorDisplay)).BeginInit();
            this.stToolStrip1.SuspendLayout();
            this.stMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.Location = new System.Drawing.Point(0, 49);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(955, 504);
            this.dockPanel1.TabIndex = 6;
            this.dockPanel1.ActiveDocumentChanged += new System.EventHandler(this.dockPanel1_ActiveDocumentChanged);
            // 
            // backColorDisplay
            // 
            this.backColorDisplay.Location = new System.Drawing.Point(253, 25);
            this.backColorDisplay.Name = "backColorDisplay";
            this.backColorDisplay.Size = new System.Drawing.Size(21, 21);
            this.backColorDisplay.TabIndex = 10;
            this.backColorDisplay.TabStop = false;
            this.backColorDisplay.Click += new System.EventHandler(this.backColorDisplay_Click);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(289, 28);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(84, 13);
            this.stLabel1.TabIndex = 14;
            this.stLabel1.Text = "Debug Shading:";
            // 
            // debugShading
            // 
            this.debugShading.BorderColor = System.Drawing.Color.Empty;
            this.debugShading.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.debugShading.ButtonColor = System.Drawing.Color.Empty;
            this.debugShading.FormattingEnabled = true;
            this.debugShading.IsReadOnly = false;
            this.debugShading.Location = new System.Drawing.Point(379, 25);
            this.debugShading.Name = "debugShading";
            this.debugShading.Size = new System.Drawing.Size(146, 21);
            this.debugShading.TabIndex = 13;
            this.debugShading.SelectedIndexChanged += new System.EventHandler(this.debugShading_SelectedIndexChanged);
            // 
            // viewportBackColorCB
            // 
            this.viewportBackColorCB.BorderColor = System.Drawing.Color.Empty;
            this.viewportBackColorCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.viewportBackColorCB.ButtonColor = System.Drawing.Color.Empty;
            this.viewportBackColorCB.FormattingEnabled = true;
            this.viewportBackColorCB.IsReadOnly = false;
            this.viewportBackColorCB.Location = new System.Drawing.Point(101, 25);
            this.viewportBackColorCB.Name = "viewportBackColorCB";
            this.viewportBackColorCB.Size = new System.Drawing.Size(146, 21);
            this.viewportBackColorCB.TabIndex = 9;
            this.viewportBackColorCB.SelectedIndexChanged += new System.EventHandler(this.viewportBackColorCB_SelectedIndexChanged);
            this.viewportBackColorCB.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.stComboBox1_MouseDoubleClick);
            // 
            // stToolStrip1
            // 
            this.stToolStrip1.HighlightSelectedTab = false;
            this.stToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolstripOrthoBtn});
            this.stToolStrip1.Location = new System.Drawing.Point(0, 24);
            this.stToolStrip1.Name = "stToolStrip1";
            this.stToolStrip1.Size = new System.Drawing.Size(955, 25);
            this.stToolStrip1.TabIndex = 3;
            this.stToolStrip1.Text = "stToolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolstripOrthoBtn
            // 
            this.toolstripOrthoBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstripOrthoBtn.Image = global::FirstPlugin.Properties.Resources.OrthoView;
            this.toolstripOrthoBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripOrthoBtn.Name = "toolstripOrthoBtn";
            this.toolstripOrthoBtn.Size = new System.Drawing.Size(23, 22);
            this.toolstripOrthoBtn.Text = "Toggle Orthographic";
            this.toolstripOrthoBtn.Click += new System.EventHandler(this.toolstripOrthoBtn_Click);
            // 
            // stMenuStrip1
            // 
            this.stMenuStrip1.HighlightSelectedTab = false;
            this.stMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.animationToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.stMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stMenuStrip1.Name = "stMenuStrip1";
            this.stMenuStrip1.Size = new System.Drawing.Size(955, 24);
            this.stMenuStrip1.TabIndex = 0;
            this.stMenuStrip1.Text = "editorModeCB:";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.clearWorkspaceToolStripMenuItem,
            this.saveToolStripMenuItem1,
            this.saveAnimationToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // clearWorkspaceToolStripMenuItem
            // 
            this.clearWorkspaceToolStripMenuItem.Name = "clearWorkspaceToolStripMenuItem";
            this.clearWorkspaceToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.clearWorkspaceToolStripMenuItem.Text = "Clear Files";
            this.clearWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.clearWorkspaceToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.saveToolStripMenuItem1.Text = "Save Layout";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // saveAnimationToolStripMenuItem
            // 
            this.saveAnimationToolStripMenuItem.Name = "saveAnimationToolStripMenuItem";
            this.saveAnimationToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveAnimationToolStripMenuItem.Text = "Save Animation";
            this.saveAnimationToolStripMenuItem.Click += new System.EventHandler(this.saveAnimationToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveToolStripMenuItem.Text = "Save As";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.panesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // panesToolStripMenuItem
            // 
            this.panesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transformChildrenToolStripMenuItem});
            this.panesToolStripMenuItem.Name = "panesToolStripMenuItem";
            this.panesToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.panesToolStripMenuItem.Text = "Panes";
            // 
            // transformChildrenToolStripMenuItem
            // 
            this.transformChildrenToolStripMenuItem.CheckOnClick = true;
            this.transformChildrenToolStripMenuItem.Name = "transformChildrenToolStripMenuItem";
            this.transformChildrenToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.transformChildrenToolStripMenuItem.Text = "Transform Children";
            this.transformChildrenToolStripMenuItem.Click += new System.EventHandler(this.transformChildrenToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderInGamePreviewToolStripMenuItem,
            this.displayGridToolStripMenuItem,
            this.textureListToolStripMenuItem,
            this.textConverterToolStripMenuItem,
            this.viewPartsAsNullPanesToolStripMenuItem,
            this.orthographicViewToolStripMenuItem,
            this.displayNullPanesToolStripMenuItem,
            this.displayyBoundryPanesToolStripMenuItem,
            this.displayWindowPanesToolStripMenuItem,
            this.displayPicturePanesToolStripMenuItem,
            this.displayTextPanesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // renderInGamePreviewToolStripMenuItem
            // 
            this.renderInGamePreviewToolStripMenuItem.CheckOnClick = true;
            this.renderInGamePreviewToolStripMenuItem.Name = "renderInGamePreviewToolStripMenuItem";
            this.renderInGamePreviewToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.renderInGamePreviewToolStripMenuItem.Text = "Render In Game Preview";
            this.renderInGamePreviewToolStripMenuItem.Click += new System.EventHandler(this.renderInGamePreviewToolStripMenuItem_Click);
            // 
            // displayGridToolStripMenuItem
            // 
            this.displayGridToolStripMenuItem.CheckOnClick = true;
            this.displayGridToolStripMenuItem.Name = "displayGridToolStripMenuItem";
            this.displayGridToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.displayGridToolStripMenuItem.Text = "Display Grid";
            this.displayGridToolStripMenuItem.Click += new System.EventHandler(this.displayGridToolStripMenuItem_Click);
            // 
            // textureListToolStripMenuItem
            // 
            this.textureListToolStripMenuItem.Name = "textureListToolStripMenuItem";
            this.textureListToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.textureListToolStripMenuItem.Text = "Texture List";
            this.textureListToolStripMenuItem.Click += new System.EventHandler(this.textureListToolStripMenuItem_Click);
            // 
            // textConverterToolStripMenuItem
            // 
            this.textConverterToolStripMenuItem.Name = "textConverterToolStripMenuItem";
            this.textConverterToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.textConverterToolStripMenuItem.Text = "Text Converter";
            this.textConverterToolStripMenuItem.Click += new System.EventHandler(this.textConverterToolStripMenuItem_Click);
            // 
            // viewPartsAsNullPanesToolStripMenuItem
            // 
            this.viewPartsAsNullPanesToolStripMenuItem.CheckOnClick = true;
            this.viewPartsAsNullPanesToolStripMenuItem.Name = "viewPartsAsNullPanesToolStripMenuItem";
            this.viewPartsAsNullPanesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.viewPartsAsNullPanesToolStripMenuItem.Text = "View Parts As Null Panes";
            this.viewPartsAsNullPanesToolStripMenuItem.Click += new System.EventHandler(this.viewPartsAsNullPanesToolStripMenuItem_Click);
            // 
            // orthographicViewToolStripMenuItem
            // 
            this.orthographicViewToolStripMenuItem.Checked = true;
            this.orthographicViewToolStripMenuItem.CheckOnClick = true;
            this.orthographicViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.orthographicViewToolStripMenuItem.Name = "orthographicViewToolStripMenuItem";
            this.orthographicViewToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.orthographicViewToolStripMenuItem.Text = "Orthographic View";
            this.orthographicViewToolStripMenuItem.Click += new System.EventHandler(this.orthographicViewToolStripMenuItem_Click);
            // 
            // displayNullPanesToolStripMenuItem
            // 
            this.displayNullPanesToolStripMenuItem.Checked = true;
            this.displayNullPanesToolStripMenuItem.CheckOnClick = true;
            this.displayNullPanesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayNullPanesToolStripMenuItem.Name = "displayNullPanesToolStripMenuItem";
            this.displayNullPanesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.displayNullPanesToolStripMenuItem.Text = "Display Null Panes";
            this.displayNullPanesToolStripMenuItem.Click += new System.EventHandler(this.displayPanesToolStripMenuItem_Click);
            // 
            // displayyBoundryPanesToolStripMenuItem
            // 
            this.displayyBoundryPanesToolStripMenuItem.Checked = true;
            this.displayyBoundryPanesToolStripMenuItem.CheckOnClick = true;
            this.displayyBoundryPanesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayyBoundryPanesToolStripMenuItem.Name = "displayyBoundryPanesToolStripMenuItem";
            this.displayyBoundryPanesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.displayyBoundryPanesToolStripMenuItem.Text = "Display Boundry Panes";
            this.displayyBoundryPanesToolStripMenuItem.Click += new System.EventHandler(this.displayPanesToolStripMenuItem_Click);
            // 
            // displayWindowPanesToolStripMenuItem
            // 
            this.displayWindowPanesToolStripMenuItem.Checked = true;
            this.displayWindowPanesToolStripMenuItem.CheckOnClick = true;
            this.displayWindowPanesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayWindowPanesToolStripMenuItem.Name = "displayWindowPanesToolStripMenuItem";
            this.displayWindowPanesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.displayWindowPanesToolStripMenuItem.Text = "Display Window Panes";
            this.displayWindowPanesToolStripMenuItem.Click += new System.EventHandler(this.displayPanesToolStripMenuItem_Click);
            // 
            // displayPicturePanesToolStripMenuItem
            // 
            this.displayPicturePanesToolStripMenuItem.Checked = true;
            this.displayPicturePanesToolStripMenuItem.CheckOnClick = true;
            this.displayPicturePanesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayPicturePanesToolStripMenuItem.Name = "displayPicturePanesToolStripMenuItem";
            this.displayPicturePanesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.displayPicturePanesToolStripMenuItem.Text = "Display Picture Panes";
            this.displayPicturePanesToolStripMenuItem.Click += new System.EventHandler(this.displayPanesToolStripMenuItem_Click);
            // 
            // displayTextPanesToolStripMenuItem
            // 
            this.displayTextPanesToolStripMenuItem.Checked = true;
            this.displayTextPanesToolStripMenuItem.CheckOnClick = true;
            this.displayTextPanesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayTextPanesToolStripMenuItem.Name = "displayTextPanesToolStripMenuItem";
            this.displayTextPanesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.displayTextPanesToolStripMenuItem.Text = "Display Text Panes";
            this.displayTextPanesToolStripMenuItem.Click += new System.EventHandler(this.displayPanesToolStripMenuItem_Click);
            // 
            // animationToolStripMenuItem
            // 
            this.animationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem});
            this.animationToolStripMenuItem.Name = "animationToolStripMenuItem";
            this.animationToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.animationToolStripMenuItem.Text = "Animation";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGameWindowToolStripMenuItem,
            this.showAnimationWindowToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // showGameWindowToolStripMenuItem
            // 
            this.showGameWindowToolStripMenuItem.Name = "showGameWindowToolStripMenuItem";
            this.showGameWindowToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.showGameWindowToolStripMenuItem.Text = "Show Game Window";
            this.showGameWindowToolStripMenuItem.Click += new System.EventHandler(this.showGameWindowToolStripMenuItem_Click);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(530, 28);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(67, 13);
            this.stLabel2.TabIndex = 18;
            this.stLabel2.Text = "Editor Mode:";
            // 
            // editorModeCB
            // 
            this.editorModeCB.BorderColor = System.Drawing.Color.Empty;
            this.editorModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.editorModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.editorModeCB.FormattingEnabled = true;
            this.editorModeCB.IsReadOnly = false;
            this.editorModeCB.Location = new System.Drawing.Point(603, 24);
            this.editorModeCB.Name = "editorModeCB";
            this.editorModeCB.Size = new System.Drawing.Size(146, 21);
            this.editorModeCB.TabIndex = 17;
            this.editorModeCB.SelectedIndexChanged += new System.EventHandler(this.editorModeCB_SelectedIndexChanged);
            // 
            // chkAutoKey
            // 
            this.chkAutoKey.AutoSize = true;
            this.chkAutoKey.Checked = true;
            this.chkAutoKey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoKey.Location = new System.Drawing.Point(755, 27);
            this.chkAutoKey.Name = "chkAutoKey";
            this.chkAutoKey.Size = new System.Drawing.Size(69, 17);
            this.chkAutoKey.TabIndex = 19;
            this.chkAutoKey.Text = "Auto Key";
            this.chkAutoKey.UseVisualStyleBackColor = true;
            // 
            // showAnimationWindowToolStripMenuItem
            // 
            this.showAnimationWindowToolStripMenuItem.Name = "showAnimationWindowToolStripMenuItem";
            this.showAnimationWindowToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.showAnimationWindowToolStripMenuItem.Text = "Show Animation Window";
            this.showAnimationWindowToolStripMenuItem.Click += new System.EventHandler(this.showAnimationWindowToolStripMenuItem_Click);
            // 
            // LayoutEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 553);
            this.Controls.Add(this.chkAutoKey);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.editorModeCB);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.debugShading);
            this.Controls.Add(this.backColorDisplay);
            this.Controls.Add(this.viewportBackColorCB);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.stToolStrip1);
            this.Controls.Add(this.stMenuStrip1);
            this.IsMdiContainer = true;
            this.Name = "LayoutEditor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LayoutEditor_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.LayoutEditor_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.LayoutEditor_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LayoutEditor_KeyDown);
            this.ParentChanged += new System.EventHandler(this.LayoutEditor_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this.backColorDisplay)).EndInit();
            this.stToolStrip1.ResumeLayout(false);
            this.stToolStrip1.PerformLayout();
            this.stMenuStrip1.ResumeLayout(false);
            this.stMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STMenuStrip stMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private Toolbox.Library.Forms.STToolStrip stToolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureListToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private Toolbox.Library.Forms.STComboBox viewportBackColorCB;
        private System.Windows.Forms.PictureBox backColorDisplay;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearWorkspaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textConverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private Toolbox.Library.Forms.STComboBox debugShading;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private System.Windows.Forms.ToolStripMenuItem orthographicViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolstripOrthoBtn;
        private System.Windows.Forms.ToolStripMenuItem saveAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayNullPanesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayyBoundryPanesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayWindowPanesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayPicturePanesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renderInGamePreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGameWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayTextPanesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPartsAsNullPanesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem panesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transformChildrenToolStripMenuItem;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox editorModeCB;
        private Toolbox.Library.Forms.STCheckBox chkAutoKey;
        private System.Windows.Forms.ToolStripMenuItem showAnimationWindowToolStripMenuItem;
    }
}
