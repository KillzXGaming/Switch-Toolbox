namespace Switch_Toolbox.Library.Forms
{
    partial class ImageEditorBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditorBase));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPanel4 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPanel5 = new Switch_Toolbox.Library.Forms.STPanel();
            this.bottomLabel = new Switch_Toolbox.Library.Forms.STLabel();
            this.pictureBoxCustom1 = new Cyotek.Windows.Forms.ImageBox();
            this.stContextMenuStrip2 = new Switch_Toolbox.Library.Forms.STContextMenuStrip(this.components);
            this.copyImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            this.alphaBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.editBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.saveBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.arrayLevelCounterLabel = new Switch_Toolbox.Library.Forms.STLabel();
            this.BtmMipsLeft = new Switch_Toolbox.Library.Forms.STButton();
            this.mipLevelCounterLabel = new Switch_Toolbox.Library.Forms.STLabel();
            this.btnRightArray = new Switch_Toolbox.Library.Forms.STButton();
            this.BtnMipsRight = new Switch_Toolbox.Library.Forms.STButton();
            this.btnLeftArray = new Switch_Toolbox.Library.Forms.STButton();
            this.imageBGComboBox = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stContextMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replacRedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceGreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceBlueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceAlphaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editInExternalProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayAlphaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useComponentSelectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewCubemapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateMipmapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reEncodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fliVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90CounterClockwiseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adjustmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brightnessContrastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.previewCubemap3DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stPanel4.SuspendLayout();
            this.stPanel5.SuspendLayout();
            this.stContextMenuStrip2.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.stContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.splitContainer1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.stPanel2);
            this.splitContainer1.Panel1.Controls.Add(this.stPanel1);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(535, 502);
            this.splitContainer1.SplitterDistance = 389;
            this.splitContainer1.TabIndex = 5;
            // 
            // stPanel2
            // 
            this.stPanel2.BackColor = System.Drawing.Color.Gray;
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel2.Location = new System.Drawing.Point(0, 501);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(389, 1);
            this.stPanel2.TabIndex = 1;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stPanel4);
            this.stPanel1.Controls.Add(this.stPanel3);
            this.stPanel1.Controls.Add(this.imageBGComboBox);
            this.stPanel1.Controls.Add(this.stContextMenuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(389, 502);
            this.stPanel1.TabIndex = 2;
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.Controls.Add(this.stPanel5);
            this.stPanel4.Controls.Add(this.pictureBoxCustom1);
            this.stPanel4.Location = new System.Drawing.Point(3, 74);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(383, 425);
            this.stPanel4.TabIndex = 8;
            // 
            // stPanel5
            // 
            this.stPanel5.Controls.Add(this.bottomLabel);
            this.stPanel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel5.Location = new System.Drawing.Point(0, 405);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(383, 20);
            this.stPanel5.TabIndex = 1;
            // 
            // bottomLabel
            // 
            this.bottomLabel.AutoSize = true;
            this.bottomLabel.Location = new System.Drawing.Point(-1, 4);
            this.bottomLabel.Name = "bottomLabel";
            this.bottomLabel.Size = new System.Drawing.Size(0, 13);
            this.bottomLabel.TabIndex = 18;
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCustom1.ContextMenuStrip = this.stContextMenuStrip2;
            this.pictureBoxCustom1.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(383, 406);
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.ZoomChanged += new System.EventHandler(this.pictureBoxCustom1_ZoomChanged);
            // 
            // stContextMenuStrip2
            // 
            this.stContextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyImageToolStripMenuItem});
            this.stContextMenuStrip2.Name = "stContextMenuStrip2";
            this.stContextMenuStrip2.Size = new System.Drawing.Size(139, 26);
            // 
            // copyImageToolStripMenuItem
            // 
            this.copyImageToolStripMenuItem.Name = "copyImageToolStripMenuItem";
            this.copyImageToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.copyImageToolStripMenuItem.Text = "Copy Image";
            this.copyImageToolStripMenuItem.Click += new System.EventHandler(this.copyImageToolStripMenuItem_Click);
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.alphaBtn);
            this.stPanel3.Controls.Add(this.editBtn);
            this.stPanel3.Controls.Add(this.saveBtn);
            this.stPanel3.Controls.Add(this.arrayLevelCounterLabel);
            this.stPanel3.Controls.Add(this.BtmMipsLeft);
            this.stPanel3.Controls.Add(this.mipLevelCounterLabel);
            this.stPanel3.Controls.Add(this.btnRightArray);
            this.stPanel3.Controls.Add(this.BtnMipsRight);
            this.stPanel3.Controls.Add(this.btnLeftArray);
            this.stPanel3.Location = new System.Drawing.Point(0, 24);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(520, 51);
            this.stPanel3.TabIndex = 3;
            // 
            // alphaBtn
            // 
            this.alphaBtn.BackColor = System.Drawing.Color.Transparent;
            this.alphaBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("alphaBtn.BackgroundImage")));
            this.alphaBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.alphaBtn.FlatAppearance.BorderSize = 0;
            this.alphaBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.alphaBtn.Location = new System.Drawing.Point(70, 3);
            this.alphaBtn.Name = "alphaBtn";
            this.alphaBtn.Size = new System.Drawing.Size(24, 23);
            this.alphaBtn.TabIndex = 17;
            this.toolTip1.SetToolTip(this.alphaBtn, "Toggle Alpha");
            this.alphaBtn.UseVisualStyleBackColor = false;
            this.alphaBtn.Click += new System.EventHandler(this.alphaBtn_Click);
            // 
            // editBtn
            // 
            this.editBtn.BackColor = System.Drawing.Color.Transparent;
            this.editBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("editBtn.BackgroundImage")));
            this.editBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.editBtn.Enabled = false;
            this.editBtn.FlatAppearance.BorderSize = 0;
            this.editBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editBtn.Location = new System.Drawing.Point(40, 3);
            this.editBtn.Name = "editBtn";
            this.editBtn.Size = new System.Drawing.Size(24, 23);
            this.editBtn.TabIndex = 16;
            this.toolTip1.SetToolTip(this.editBtn, "Open with default program");
            this.editBtn.UseVisualStyleBackColor = false;
            this.editBtn.Click += new System.EventHandler(this.editBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.BackColor = System.Drawing.Color.Transparent;
            this.saveBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("saveBtn.BackgroundImage")));
            this.saveBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.saveBtn.FlatAppearance.BorderSize = 0;
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Location = new System.Drawing.Point(10, 3);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(24, 23);
            this.saveBtn.TabIndex = 14;
            this.toolTip1.SetToolTip(this.saveBtn, "Save changes from editor");
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // arrayLevelCounterLabel
            // 
            this.arrayLevelCounterLabel.AutoSize = true;
            this.arrayLevelCounterLabel.Location = new System.Drawing.Point(7, 29);
            this.arrayLevelCounterLabel.Name = "arrayLevelCounterLabel";
            this.arrayLevelCounterLabel.Size = new System.Drawing.Size(101, 13);
            this.arrayLevelCounterLabel.TabIndex = 13;
            this.arrayLevelCounterLabel.Text = "Array Level: 00 / 00";
            // 
            // BtmMipsLeft
            // 
            this.BtmMipsLeft.Enabled = false;
            this.BtmMipsLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtmMipsLeft.Location = new System.Drawing.Point(262, 25);
            this.BtmMipsLeft.Name = "BtmMipsLeft";
            this.BtmMipsLeft.Size = new System.Drawing.Size(21, 21);
            this.BtmMipsLeft.TabIndex = 8;
            this.BtmMipsLeft.Text = "<";
            this.BtmMipsLeft.UseVisualStyleBackColor = true;
            this.BtmMipsLeft.Click += new System.EventHandler(this.BtmMipsLeft_Click);
            // 
            // mipLevelCounterLabel
            // 
            this.mipLevelCounterLabel.AutoSize = true;
            this.mipLevelCounterLabel.Location = new System.Drawing.Point(168, 29);
            this.mipLevelCounterLabel.Name = "mipLevelCounterLabel";
            this.mipLevelCounterLabel.Size = new System.Drawing.Size(94, 13);
            this.mipLevelCounterLabel.TabIndex = 10;
            this.mipLevelCounterLabel.Text = "Mip Level: 00 / 00";
            // 
            // btnRightArray
            // 
            this.btnRightArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRightArray.Location = new System.Drawing.Point(141, 25);
            this.btnRightArray.Name = "btnRightArray";
            this.btnRightArray.Size = new System.Drawing.Size(21, 21);
            this.btnRightArray.TabIndex = 12;
            this.btnRightArray.Text = ">";
            this.btnRightArray.UseVisualStyleBackColor = true;
            this.btnRightArray.Click += new System.EventHandler(this.btnRightArray_Click);
            // 
            // BtnMipsRight
            // 
            this.BtnMipsRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnMipsRight.Location = new System.Drawing.Point(289, 25);
            this.BtnMipsRight.Name = "BtnMipsRight";
            this.BtnMipsRight.Size = new System.Drawing.Size(21, 21);
            this.BtnMipsRight.TabIndex = 9;
            this.BtnMipsRight.Text = ">";
            this.BtnMipsRight.UseVisualStyleBackColor = true;
            this.BtnMipsRight.Click += new System.EventHandler(this.BtnMipsRight_Click);
            // 
            // btnLeftArray
            // 
            this.btnLeftArray.Enabled = false;
            this.btnLeftArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeftArray.Location = new System.Drawing.Point(114, 25);
            this.btnLeftArray.Name = "btnLeftArray";
            this.btnLeftArray.Size = new System.Drawing.Size(21, 21);
            this.btnLeftArray.TabIndex = 11;
            this.btnLeftArray.Text = "<";
            this.btnLeftArray.UseVisualStyleBackColor = true;
            this.btnLeftArray.Click += new System.EventHandler(this.btnLeftArray_Click);
            // 
            // imageBGComboBox
            // 
            this.imageBGComboBox.BorderColor = System.Drawing.Color.Empty;
            this.imageBGComboBox.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.imageBGComboBox.ButtonColor = System.Drawing.Color.Empty;
            this.imageBGComboBox.FormattingEnabled = true;
            this.imageBGComboBox.Location = new System.Drawing.Point(262, 2);
            this.imageBGComboBox.Name = "imageBGComboBox";
            this.imageBGComboBox.ReadOnly = true;
            this.imageBGComboBox.Size = new System.Drawing.Size(95, 21);
            this.imageBGComboBox.TabIndex = 2;
            this.imageBGComboBox.SelectedIndexChanged += new System.EventHandler(this.imageBGComboBox_SelectedIndexChanged);
            // 
            // stContextMenuStrip1
            // 
            this.stContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.adjustmentsToolStripMenuItem});
            this.stContextMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stContextMenuStrip1.Name = "stContextMenuStrip1";
            this.stContextMenuStrip1.Size = new System.Drawing.Size(389, 24);
            this.stContextMenuStrip1.TabIndex = 0;
            this.stContextMenuStrip1.Text = "stContextMenuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fillColorToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.channelsToolStripMenuItem,
            this.editInExternalProgramToolStripMenuItem,
            this.copyToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // fillColorToolStripMenuItem
            // 
            this.fillColorToolStripMenuItem.Name = "fillColorToolStripMenuItem";
            this.fillColorToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.fillColorToolStripMenuItem.Text = "Fill Color";
            this.fillColorToolStripMenuItem.Click += new System.EventHandler(this.fillColorToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // channelsToolStripMenuItem
            // 
            this.channelsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replacRedToolStripMenuItem,
            this.replaceGreenToolStripMenuItem,
            this.replaceBlueToolStripMenuItem,
            this.replaceAlphaToolStripMenuItem});
            this.channelsToolStripMenuItem.Name = "channelsToolStripMenuItem";
            this.channelsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.channelsToolStripMenuItem.Text = "Channels";
            // 
            // replacRedToolStripMenuItem
            // 
            this.replacRedToolStripMenuItem.Name = "replacRedToolStripMenuItem";
            this.replacRedToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.replacRedToolStripMenuItem.Text = "Replace Red";
            this.replacRedToolStripMenuItem.Click += new System.EventHandler(this.replacRedToolStripMenuItem_Click);
            // 
            // replaceGreenToolStripMenuItem
            // 
            this.replaceGreenToolStripMenuItem.Name = "replaceGreenToolStripMenuItem";
            this.replaceGreenToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.replaceGreenToolStripMenuItem.Text = "Replace Green";
            this.replaceGreenToolStripMenuItem.Click += new System.EventHandler(this.replaceGreenToolStripMenuItem_Click);
            // 
            // replaceBlueToolStripMenuItem
            // 
            this.replaceBlueToolStripMenuItem.Name = "replaceBlueToolStripMenuItem";
            this.replaceBlueToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.replaceBlueToolStripMenuItem.Text = "Replace Blue";
            this.replaceBlueToolStripMenuItem.Click += new System.EventHandler(this.replaceBlueToolStripMenuItem_Click);
            // 
            // replaceAlphaToolStripMenuItem
            // 
            this.replaceAlphaToolStripMenuItem.Name = "replaceAlphaToolStripMenuItem";
            this.replaceAlphaToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.replaceAlphaToolStripMenuItem.Text = "Replace Alpha";
            this.replaceAlphaToolStripMenuItem.Click += new System.EventHandler(this.replaceAlphaToolStripMenuItem_Click);
            // 
            // editInExternalProgramToolStripMenuItem
            // 
            this.editInExternalProgramToolStripMenuItem.Name = "editInExternalProgramToolStripMenuItem";
            this.editInExternalProgramToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.editInExternalProgramToolStripMenuItem.Text = "With External Program";
            this.editInExternalProgramToolStripMenuItem.Click += new System.EventHandler(this.editInExternalProgramToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableZoomToolStripMenuItem,
            this.propertyGridToolStripMenuItem,
            this.displayVerticalToolStripMenuItem,
            this.displayAlphaToolStripMenuItem,
            this.useComponentSelectorToolStripMenuItem,
            this.previewCubemapToolStripMenuItem,
            this.previewCubemap3DToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // enableZoomToolStripMenuItem
            // 
            this.enableZoomToolStripMenuItem.Name = "enableZoomToolStripMenuItem";
            this.enableZoomToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.enableZoomToolStripMenuItem.Text = "Enable Zoom";
            this.enableZoomToolStripMenuItem.Click += new System.EventHandler(this.enableZoomToolStripMenuItem_Click);
            // 
            // propertyGridToolStripMenuItem
            // 
            this.propertyGridToolStripMenuItem.Checked = true;
            this.propertyGridToolStripMenuItem.CheckOnClick = true;
            this.propertyGridToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.propertyGridToolStripMenuItem.Name = "propertyGridToolStripMenuItem";
            this.propertyGridToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.propertyGridToolStripMenuItem.Text = "Property Grid";
            this.propertyGridToolStripMenuItem.CheckedChanged += new System.EventHandler(this.propertyGridToolStripMenuItem_CheckedChanged);
            this.propertyGridToolStripMenuItem.Click += new System.EventHandler(this.propertyGridToolStripMenuItem_Click);
            // 
            // displayVerticalToolStripMenuItem
            // 
            this.displayVerticalToolStripMenuItem.Name = "displayVerticalToolStripMenuItem";
            this.displayVerticalToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.displayVerticalToolStripMenuItem.Text = "Display Vertical";
            this.displayVerticalToolStripMenuItem.Click += new System.EventHandler(this.displayVerticalToolStripMenuItem_Click);
            // 
            // displayAlphaToolStripMenuItem
            // 
            this.displayAlphaToolStripMenuItem.Name = "displayAlphaToolStripMenuItem";
            this.displayAlphaToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.displayAlphaToolStripMenuItem.Text = "Display Alpha";
            this.displayAlphaToolStripMenuItem.Click += new System.EventHandler(this.displayAlphaToolStripMenuItem_Click);
            // 
            // useComponentSelectorToolStripMenuItem
            // 
            this.useComponentSelectorToolStripMenuItem.Name = "useComponentSelectorToolStripMenuItem";
            this.useComponentSelectorToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.useComponentSelectorToolStripMenuItem.Text = "Use Component Selector";
            this.useComponentSelectorToolStripMenuItem.Click += new System.EventHandler(this.useComponentSelectorToolStripMenuItem_Click);
            // 
            // previewCubemapToolStripMenuItem
            // 
            this.previewCubemapToolStripMenuItem.Name = "previewCubemapToolStripMenuItem";
            this.previewCubemapToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.previewCubemapToolStripMenuItem.Text = "Preview Cubemap";
            this.previewCubemapToolStripMenuItem.Click += new System.EventHandler(this.previewCubemapToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateMipmapsToolStripMenuItem,
            this.resizeToolStripMenuItem,
            this.reEncodeToolStripMenuItem,
            this.flipHorizontalToolStripMenuItem,
            this.fliVerticalToolStripMenuItem,
            this.rotate90ToolStripMenuItem,
            this.rotate90CounterClockwiseToolStripMenuItem,
            this.rToolStripMenuItem});
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.imageToolStripMenuItem.Text = "Image";
            // 
            // generateMipmapsToolStripMenuItem
            // 
            this.generateMipmapsToolStripMenuItem.Name = "generateMipmapsToolStripMenuItem";
            this.generateMipmapsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.generateMipmapsToolStripMenuItem.Text = "Generate Mipmaps";
            this.generateMipmapsToolStripMenuItem.Click += new System.EventHandler(this.generateMipmapsToolStripMenuItem_Click);
            // 
            // resizeToolStripMenuItem
            // 
            this.resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
            this.resizeToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.resizeToolStripMenuItem.Text = "Resize";
            this.resizeToolStripMenuItem.Click += new System.EventHandler(this.resizeToolStripMenuItem_Click);
            // 
            // reEncodeToolStripMenuItem
            // 
            this.reEncodeToolStripMenuItem.Name = "reEncodeToolStripMenuItem";
            this.reEncodeToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.reEncodeToolStripMenuItem.Text = "Re - Encode";
            this.reEncodeToolStripMenuItem.Click += new System.EventHandler(this.reEncodeToolStripMenuItem_Click);
            // 
            // flipHorizontalToolStripMenuItem
            // 
            this.flipHorizontalToolStripMenuItem.Name = "flipHorizontalToolStripMenuItem";
            this.flipHorizontalToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.flipHorizontalToolStripMenuItem.Text = "Flip Horizontal";
            this.flipHorizontalToolStripMenuItem.Click += new System.EventHandler(this.flipHorizontalToolStripMenuItem_Click);
            // 
            // fliVerticalToolStripMenuItem
            // 
            this.fliVerticalToolStripMenuItem.Name = "fliVerticalToolStripMenuItem";
            this.fliVerticalToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.fliVerticalToolStripMenuItem.Text = "Flip Vertical";
            this.fliVerticalToolStripMenuItem.Click += new System.EventHandler(this.fliVerticalToolStripMenuItem_Click);
            // 
            // rotate90ToolStripMenuItem
            // 
            this.rotate90ToolStripMenuItem.Name = "rotate90ToolStripMenuItem";
            this.rotate90ToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.rotate90ToolStripMenuItem.Text = "Rotate 90* Clockwise";
            this.rotate90ToolStripMenuItem.Click += new System.EventHandler(this.rotate90ToolStripMenuItem_Click);
            // 
            // rotate90CounterClockwiseToolStripMenuItem
            // 
            this.rotate90CounterClockwiseToolStripMenuItem.Name = "rotate90CounterClockwiseToolStripMenuItem";
            this.rotate90CounterClockwiseToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.rotate90CounterClockwiseToolStripMenuItem.Text = "Rotate 90* Counter Clockwise";
            this.rotate90CounterClockwiseToolStripMenuItem.Click += new System.EventHandler(this.rotate90CounterClockwiseToolStripMenuItem_Click);
            // 
            // rToolStripMenuItem
            // 
            this.rToolStripMenuItem.Name = "rToolStripMenuItem";
            this.rToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.rToolStripMenuItem.Text = "Rotate 180*";
            this.rToolStripMenuItem.Click += new System.EventHandler(this.rToolStripMenuItem_Click);
            // 
            // adjustmentsToolStripMenuItem
            // 
            this.adjustmentsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hueToolStripMenuItem,
            this.brightnessContrastToolStripMenuItem});
            this.adjustmentsToolStripMenuItem.Name = "adjustmentsToolStripMenuItem";
            this.adjustmentsToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.adjustmentsToolStripMenuItem.Text = "Adjustments";
            // 
            // hueToolStripMenuItem
            // 
            this.hueToolStripMenuItem.Enabled = false;
            this.hueToolStripMenuItem.Name = "hueToolStripMenuItem";
            this.hueToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.hueToolStripMenuItem.Text = "Hue / Saturation";
            this.hueToolStripMenuItem.Click += new System.EventHandler(this.hueToolStripMenuItem_Click);
            // 
            // brightnessContrastToolStripMenuItem
            // 
            this.brightnessContrastToolStripMenuItem.Enabled = false;
            this.brightnessContrastToolStripMenuItem.Name = "brightnessContrastToolStripMenuItem";
            this.brightnessContrastToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.brightnessContrastToolStripMenuItem.Text = "Brightness / Contrast";
            // 
            // previewCubemap3DToolStripMenuItem
            // 
            this.previewCubemap3DToolStripMenuItem.Name = "previewCubemap3DToolStripMenuItem";
            this.previewCubemap3DToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.previewCubemap3DToolStripMenuItem.Text = "Preview Cubemap (3D)";
            this.previewCubemap3DToolStripMenuItem.Click += new System.EventHandler(this.previewCubemap3DToolStripMenuItem_Click);
            // 
            // ImageEditorBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ImageEditorBase";
            this.Size = new System.Drawing.Size(535, 502);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stPanel4.ResumeLayout(false);
            this.stPanel5.ResumeLayout(false);
            this.stPanel5.PerformLayout();
            this.stContextMenuStrip2.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.stPanel3.PerformLayout();
            this.stContextMenuStrip1.ResumeLayout(false);
            this.stContextMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private STPanel stPanel1;
        private Switch_Toolbox.Library.Forms.STComboBox imageBGComboBox;
        private Switch_Toolbox.Library.Forms.STButton BtnMipsRight;
        private Switch_Toolbox.Library.Forms.STButton BtmMipsLeft;
        private Switch_Toolbox.Library.Forms.STButton btnRightArray;
        private Switch_Toolbox.Library.Forms.STButton btnLeftArray;
        private STLabel mipLevelCounterLabel;
        private STPanel stPanel3;
        private STLabel arrayLevelCounterLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private STPanel stPanel2;
        private STPanel stPanel4;
        private STMenuStrip stContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertyGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipHorizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fliVerticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adjustmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brightnessContrastToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate90ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate90CounterClockwiseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rToolStripMenuItem;
        private STButton saveBtn;
        private System.Windows.Forms.ToolStripMenuItem reEncodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayVerticalToolStripMenuItem;
        private STButton editBtn;
        private System.Windows.Forms.ToolStripMenuItem generateMipmapsToolStripMenuItem;
        private STContextMenuStrip stContextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem copyImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editInExternalProgramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayAlphaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useComponentSelectorToolStripMenuItem;
        private STButton alphaBtn;
        private System.Windows.Forms.ToolTip toolTip1;
        private Cyotek.Windows.Forms.ImageBox pictureBoxCustom1;
        private System.Windows.Forms.ToolStripMenuItem enableZoomToolStripMenuItem;
        private STPanel stPanel5;
        private STLabel bottomLabel;
        private System.Windows.Forms.ToolStripMenuItem fillColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem channelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replacRedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceGreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceBlueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceAlphaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewCubemapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewCubemap3DToolStripMenuItem;
    }
}