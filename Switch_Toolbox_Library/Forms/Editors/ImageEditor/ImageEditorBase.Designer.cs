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
            this.alphaChannelBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.stPanel4 = new Switch_Toolbox.Library.Forms.STPanel();
            this.pictureBoxCustom1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.stContextMenuStrip2 = new Switch_Toolbox.Library.Forms.STContextMenuStrip(this.components);
            this.copyImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blueChannelBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            this.editBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.toggleAlphaChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.saveBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.arrayLevelCounterLabel = new Switch_Toolbox.Library.Forms.STLabel();
            this.BtmMipsLeft = new Switch_Toolbox.Library.Forms.STButton();
            this.mipLevelCounterLabel = new Switch_Toolbox.Library.Forms.STLabel();
            this.btnRightArray = new Switch_Toolbox.Library.Forms.STButton();
            this.BtnMipsRight = new Switch_Toolbox.Library.Forms.STButton();
            this.btnLeftArray = new Switch_Toolbox.Library.Forms.STButton();
            this.greenChannelBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.imageBGComboBox = new Switch_Toolbox.Library.Forms.STComboBox();
            this.redChannelBtn = new Switch_Toolbox.Library.Forms.STButton();
            this.stContextMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editInExternalProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.componentSelector = new Switch_Toolbox.Library.Forms.STCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
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
            this.splitContainer1.Size = new System.Drawing.Size(715, 502);
            this.splitContainer1.SplitterDistance = 520;
            this.splitContainer1.TabIndex = 5;
            // 
            // stPanel2
            // 
            this.stPanel2.BackColor = System.Drawing.Color.Gray;
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel2.Location = new System.Drawing.Point(0, 501);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(520, 1);
            this.stPanel2.TabIndex = 1;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.alphaChannelBtn);
            this.stPanel1.Controls.Add(this.stPanel4);
            this.stPanel1.Controls.Add(this.blueChannelBtn);
            this.stPanel1.Controls.Add(this.stPanel3);
            this.stPanel1.Controls.Add(this.greenChannelBtn);
            this.stPanel1.Controls.Add(this.imageBGComboBox);
            this.stPanel1.Controls.Add(this.redChannelBtn);
            this.stPanel1.Controls.Add(this.stContextMenuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(520, 502);
            this.stPanel1.TabIndex = 2;
            // 
            // alphaChannelBtn
            // 
            this.alphaChannelBtn.BackColor = System.Drawing.Color.Silver;
            this.alphaChannelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.alphaChannelBtn.Location = new System.Drawing.Point(464, 1);
            this.alphaChannelBtn.Name = "alphaChannelBtn";
            this.alphaChannelBtn.Size = new System.Drawing.Size(21, 21);
            this.alphaChannelBtn.TabIndex = 17;
            this.alphaChannelBtn.Text = "A";
            this.alphaChannelBtn.UseVisualStyleBackColor = false;
            this.alphaChannelBtn.Click += new System.EventHandler(this.ChannelBtn_Click);
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.Controls.Add(this.pictureBoxCustom1);
            this.stPanel4.Location = new System.Drawing.Point(3, 74);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(514, 425);
            this.stPanel4.TabIndex = 8;
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.ContextMenuStrip = this.stContextMenuStrip2;
            this.pictureBoxCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCustom1.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(514, 425);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 1;
            this.pictureBoxCustom1.TabStop = false;
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
            // blueChannelBtn
            // 
            this.blueChannelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.blueChannelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.blueChannelBtn.Location = new System.Drawing.Point(437, 1);
            this.blueChannelBtn.Name = "blueChannelBtn";
            this.blueChannelBtn.Size = new System.Drawing.Size(21, 21);
            this.blueChannelBtn.TabIndex = 16;
            this.blueChannelBtn.Text = "B";
            this.blueChannelBtn.UseVisualStyleBackColor = false;
            this.blueChannelBtn.Click += new System.EventHandler(this.ChannelBtn_Click);
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.componentSelector);
            this.stPanel3.Controls.Add(this.editBtn);
            this.stPanel3.Controls.Add(this.toggleAlphaChk);
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
            // editBtn
            // 
            this.editBtn.BackColor = System.Drawing.Color.Transparent;
            this.editBtn.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.Edit;
            this.editBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.editBtn.Enabled = false;
            this.editBtn.FlatAppearance.BorderSize = 0;
            this.editBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editBtn.Location = new System.Drawing.Point(40, 3);
            this.editBtn.Name = "editBtn";
            this.editBtn.Size = new System.Drawing.Size(24, 23);
            this.editBtn.TabIndex = 16;
            this.editBtn.UseVisualStyleBackColor = false;
            this.editBtn.Click += new System.EventHandler(this.editBtn_Click);
            // 
            // toggleAlphaChk
            // 
            this.toggleAlphaChk.AutoSize = true;
            this.toggleAlphaChk.Checked = true;
            this.toggleAlphaChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleAlphaChk.Location = new System.Drawing.Point(85, 7);
            this.toggleAlphaChk.Name = "toggleAlphaChk";
            this.toggleAlphaChk.Size = new System.Drawing.Size(83, 17);
            this.toggleAlphaChk.TabIndex = 15;
            this.toggleAlphaChk.Text = "Show Alpha";
            this.toggleAlphaChk.UseVisualStyleBackColor = true;
            this.toggleAlphaChk.CheckedChanged += new System.EventHandler(this.toggleAlphaChk_CheckedChanged);
            // 
            // saveBtn
            // 
            this.saveBtn.BackColor = System.Drawing.Color.Transparent;
            this.saveBtn.BackgroundImage = global::Switch_Toolbox.Library.Properties.Resources.Save;
            this.saveBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.saveBtn.FlatAppearance.BorderSize = 0;
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Location = new System.Drawing.Point(10, 3);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(24, 23);
            this.saveBtn.TabIndex = 14;
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
            this.BtmMipsLeft.Location = new System.Drawing.Point(355, 26);
            this.BtmMipsLeft.Name = "BtmMipsLeft";
            this.BtmMipsLeft.Size = new System.Drawing.Size(57, 21);
            this.BtmMipsLeft.TabIndex = 8;
            this.BtmMipsLeft.Text = "<";
            this.BtmMipsLeft.UseVisualStyleBackColor = true;
            this.BtmMipsLeft.Click += new System.EventHandler(this.BtmMipsLeft_Click);
            // 
            // mipLevelCounterLabel
            // 
            this.mipLevelCounterLabel.AutoSize = true;
            this.mipLevelCounterLabel.Location = new System.Drawing.Point(248, 30);
            this.mipLevelCounterLabel.Name = "mipLevelCounterLabel";
            this.mipLevelCounterLabel.Size = new System.Drawing.Size(94, 13);
            this.mipLevelCounterLabel.TabIndex = 10;
            this.mipLevelCounterLabel.Text = "Mip Level: 00 / 00";
            // 
            // btnRightArray
            // 
            this.btnRightArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRightArray.Location = new System.Drawing.Point(190, 26);
            this.btnRightArray.Name = "btnRightArray";
            this.btnRightArray.Size = new System.Drawing.Size(57, 21);
            this.btnRightArray.TabIndex = 12;
            this.btnRightArray.Text = ">";
            this.btnRightArray.UseVisualStyleBackColor = true;
            this.btnRightArray.Click += new System.EventHandler(this.btnRightArray_Click);
            // 
            // BtnMipsRight
            // 
            this.BtnMipsRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnMipsRight.Location = new System.Drawing.Point(418, 26);
            this.BtnMipsRight.Name = "BtnMipsRight";
            this.BtnMipsRight.Size = new System.Drawing.Size(57, 21);
            this.BtnMipsRight.TabIndex = 9;
            this.BtnMipsRight.Text = ">";
            this.BtnMipsRight.UseVisualStyleBackColor = true;
            this.BtnMipsRight.Click += new System.EventHandler(this.BtnMipsRight_Click);
            // 
            // btnLeftArray
            // 
            this.btnLeftArray.Enabled = false;
            this.btnLeftArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeftArray.Location = new System.Drawing.Point(127, 26);
            this.btnLeftArray.Name = "btnLeftArray";
            this.btnLeftArray.Size = new System.Drawing.Size(57, 21);
            this.btnLeftArray.TabIndex = 11;
            this.btnLeftArray.Text = "<";
            this.btnLeftArray.UseVisualStyleBackColor = true;
            this.btnLeftArray.Click += new System.EventHandler(this.btnLeftArray_Click);
            // 
            // greenChannelBtn
            // 
            this.greenChannelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.greenChannelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.greenChannelBtn.Location = new System.Drawing.Point(410, 1);
            this.greenChannelBtn.Name = "greenChannelBtn";
            this.greenChannelBtn.Size = new System.Drawing.Size(21, 21);
            this.greenChannelBtn.TabIndex = 15;
            this.greenChannelBtn.Text = "G";
            this.greenChannelBtn.UseVisualStyleBackColor = false;
            this.greenChannelBtn.Click += new System.EventHandler(this.ChannelBtn_Click);
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
            this.imageBGComboBox.Size = new System.Drawing.Size(115, 21);
            this.imageBGComboBox.TabIndex = 2;
            this.imageBGComboBox.SelectedIndexChanged += new System.EventHandler(this.imageBGComboBox_SelectedIndexChanged);
            // 
            // redChannelBtn
            // 
            this.redChannelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.redChannelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.redChannelBtn.Location = new System.Drawing.Point(383, 1);
            this.redChannelBtn.Name = "redChannelBtn";
            this.redChannelBtn.Size = new System.Drawing.Size(21, 21);
            this.redChannelBtn.TabIndex = 14;
            this.redChannelBtn.Text = "R";
            this.redChannelBtn.UseVisualStyleBackColor = false;
            this.redChannelBtn.Click += new System.EventHandler(this.ChannelBtn_Click);
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
            this.stContextMenuStrip1.Size = new System.Drawing.Size(520, 24);
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
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.editInExternalProgramToolStripMenuItem,
            this.copyToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
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
            this.propertyGridToolStripMenuItem,
            this.displayVerticalToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // propertyGridToolStripMenuItem
            // 
            this.propertyGridToolStripMenuItem.Checked = true;
            this.propertyGridToolStripMenuItem.CheckOnClick = true;
            this.propertyGridToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.propertyGridToolStripMenuItem.Name = "propertyGridToolStripMenuItem";
            this.propertyGridToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.propertyGridToolStripMenuItem.Text = "Property Grid";
            this.propertyGridToolStripMenuItem.CheckedChanged += new System.EventHandler(this.propertyGridToolStripMenuItem_CheckedChanged);
            this.propertyGridToolStripMenuItem.Click += new System.EventHandler(this.propertyGridToolStripMenuItem_Click);
            // 
            // displayVerticalToolStripMenuItem
            // 
            this.displayVerticalToolStripMenuItem.Name = "displayVerticalToolStripMenuItem";
            this.displayVerticalToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.displayVerticalToolStripMenuItem.Text = "Display Vertical";
            this.displayVerticalToolStripMenuItem.CheckedChanged += new System.EventHandler(this.displayVerticalToolStripMenuItem_CheckedChanged);
            this.displayVerticalToolStripMenuItem.Click += new System.EventHandler(this.displayVerticalToolStripMenuItem_Click);
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
            // componentSelector
            // 
            this.componentSelector.AutoSize = true;
            this.componentSelector.Checked = true;
            this.componentSelector.CheckState = System.Windows.Forms.CheckState.Checked;
            this.componentSelector.Location = new System.Drawing.Point(174, 7);
            this.componentSelector.Name = "componentSelector";
            this.componentSelector.Size = new System.Drawing.Size(144, 17);
            this.componentSelector.TabIndex = 17;
            this.componentSelector.Text = "Use Component Selector";
            this.componentSelector.UseVisualStyleBackColor = true;
            this.componentSelector.CheckedChanged += new System.EventHandler(this.componentSelector_CheckedChanged);
            // 
            // ImageEditorBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ImageEditorBase";
            this.Size = new System.Drawing.Size(715, 502);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.stContextMenuStrip2.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.stPanel3.PerformLayout();
            this.stContextMenuStrip1.ResumeLayout(false);
            this.stContextMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public PictureBoxCustom pictureBoxCustom1;
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
        private STButton alphaChannelBtn;
        private STButton blueChannelBtn;
        private STButton greenChannelBtn;
        private STButton redChannelBtn;
        private STButton saveBtn;
        private System.Windows.Forms.ToolStripMenuItem reEncodeToolStripMenuItem;
        private STCheckBox toggleAlphaChk;
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
        private STCheckBox componentSelector;
    }
}