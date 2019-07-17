namespace FirstPlugin.Forms
{
    partial class BffntEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BffntEditor));
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.lineFeedUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel13 = new Toolbox.Library.Forms.STLabel();
            this.fontTypeCB = new Toolbox.Library.Forms.STComboBox();
            this.encodingTypeCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel12 = new Toolbox.Library.Forms.STLabel();
            this.stLabel11 = new Toolbox.Library.Forms.STLabel();
            this.stLabel10 = new Toolbox.Library.Forms.STLabel();
            this.leftSpacingUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel9 = new Toolbox.Library.Forms.STLabel();
            this.glyphWidthCB = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel8 = new Toolbox.Library.Forms.STLabel();
            this.charWidthUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel7 = new Toolbox.Library.Forms.STLabel();
            this.fontHeightUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            this.fontWidthUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.ascentUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.imagesCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.imageMenuStrip = new Toolbox.Library.Forms.STContextMenuStrip(this.components);
            this.pictureBoxCustom1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lineFeedUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftSpacingUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.glyphWidthCB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.charWidthUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontHeightUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontWidthUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ascentUD)).BeginInit();
            this.stPanel2.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.stPanel4.SuspendLayout();
            this.imageMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.SuspendLayout();
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.lineFeedUD);
            this.stPanel1.Controls.Add(this.stLabel13);
            this.stPanel1.Controls.Add(this.fontTypeCB);
            this.stPanel1.Controls.Add(this.encodingTypeCB);
            this.stPanel1.Controls.Add(this.stLabel12);
            this.stPanel1.Controls.Add(this.stLabel11);
            this.stPanel1.Controls.Add(this.stLabel10);
            this.stPanel1.Controls.Add(this.leftSpacingUD);
            this.stPanel1.Controls.Add(this.stLabel9);
            this.stPanel1.Controls.Add(this.glyphWidthCB);
            this.stPanel1.Controls.Add(this.stLabel8);
            this.stPanel1.Controls.Add(this.charWidthUD);
            this.stPanel1.Controls.Add(this.stLabel7);
            this.stPanel1.Controls.Add(this.fontHeightUD);
            this.stPanel1.Controls.Add(this.stLabel6);
            this.stPanel1.Controls.Add(this.fontWidthUD);
            this.stPanel1.Controls.Add(this.stLabel5);
            this.stPanel1.Controls.Add(this.ascentUD);
            this.stPanel1.Controls.Add(this.stLabel4);
            this.stPanel1.Controls.Add(this.stLabel1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(201, 438);
            this.stPanel1.TabIndex = 0;
            // 
            // lineFeedUD
            // 
            this.lineFeedUD.Location = new System.Drawing.Point(86, 139);
            this.lineFeedUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.lineFeedUD.Name = "lineFeedUD";
            this.lineFeedUD.Size = new System.Drawing.Size(97, 20);
            this.lineFeedUD.TabIndex = 19;
            // 
            // stLabel13
            // 
            this.stLabel13.AutoSize = true;
            this.stLabel13.Location = new System.Drawing.Point(-1, 141);
            this.stLabel13.Name = "stLabel13";
            this.stLabel13.Size = new System.Drawing.Size(57, 13);
            this.stLabel13.TabIndex = 18;
            this.stLabel13.Text = "Line Feed:";
            // 
            // fontTypeCB
            // 
            this.fontTypeCB.BorderColor = System.Drawing.Color.Empty;
            this.fontTypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.fontTypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.fontTypeCB.FormattingEnabled = true;
            this.fontTypeCB.Location = new System.Drawing.Point(87, 42);
            this.fontTypeCB.Name = "fontTypeCB";
            this.fontTypeCB.ReadOnly = true;
            this.fontTypeCB.Size = new System.Drawing.Size(108, 21);
            this.fontTypeCB.TabIndex = 17;
            // 
            // encodingTypeCB
            // 
            this.encodingTypeCB.BorderColor = System.Drawing.Color.Empty;
            this.encodingTypeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.encodingTypeCB.ButtonColor = System.Drawing.Color.Empty;
            this.encodingTypeCB.FormattingEnabled = true;
            this.encodingTypeCB.Location = new System.Drawing.Point(87, 73);
            this.encodingTypeCB.Name = "encodingTypeCB";
            this.encodingTypeCB.ReadOnly = true;
            this.encodingTypeCB.Size = new System.Drawing.Size(108, 21);
            this.encodingTypeCB.TabIndex = 16;
            // 
            // stLabel12
            // 
            this.stLabel12.AutoSize = true;
            this.stLabel12.Location = new System.Drawing.Point(3, 73);
            this.stLabel12.Name = "stLabel12";
            this.stLabel12.Size = new System.Drawing.Size(55, 13);
            this.stLabel12.TabIndex = 15;
            this.stLabel12.Text = "Encoding:";
            // 
            // stLabel11
            // 
            this.stLabel11.AutoSize = true;
            this.stLabel11.Location = new System.Drawing.Point(3, 45);
            this.stLabel11.Name = "stLabel11";
            this.stLabel11.Size = new System.Drawing.Size(34, 13);
            this.stLabel11.TabIndex = 14;
            this.stLabel11.Text = "Type:";
            // 
            // stLabel10
            // 
            this.stLabel10.AutoSize = true;
            this.stLabel10.Location = new System.Drawing.Point(3, 106);
            this.stLabel10.Name = "stLabel10";
            this.stLabel10.Size = new System.Drawing.Size(69, 13);
            this.stLabel10.TabIndex = 13;
            this.stLabel10.Text = "Default Char:";
            // 
            // leftSpacingUD
            // 
            this.leftSpacingUD.Location = new System.Drawing.Point(86, 165);
            this.leftSpacingUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.leftSpacingUD.Name = "leftSpacingUD";
            this.leftSpacingUD.Size = new System.Drawing.Size(97, 20);
            this.leftSpacingUD.TabIndex = 12;
            // 
            // stLabel9
            // 
            this.stLabel9.AutoSize = true;
            this.stLabel9.Location = new System.Drawing.Point(-1, 167);
            this.stLabel9.Name = "stLabel9";
            this.stLabel9.Size = new System.Drawing.Size(73, 13);
            this.stLabel9.TabIndex = 11;
            this.stLabel9.Text = "Left Spacing::";
            // 
            // glyphWidthCB
            // 
            this.glyphWidthCB.Location = new System.Drawing.Point(86, 217);
            this.glyphWidthCB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.glyphWidthCB.Name = "glyphWidthCB";
            this.glyphWidthCB.Size = new System.Drawing.Size(97, 20);
            this.glyphWidthCB.TabIndex = 10;
            // 
            // stLabel8
            // 
            this.stLabel8.AutoSize = true;
            this.stLabel8.Location = new System.Drawing.Point(2, 219);
            this.stLabel8.Name = "stLabel8";
            this.stLabel8.Size = new System.Drawing.Size(68, 13);
            this.stLabel8.TabIndex = 9;
            this.stLabel8.Text = "Glyph Width:";
            // 
            // charWidthUD
            // 
            this.charWidthUD.Location = new System.Drawing.Point(86, 191);
            this.charWidthUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.charWidthUD.Name = "charWidthUD";
            this.charWidthUD.Size = new System.Drawing.Size(97, 20);
            this.charWidthUD.TabIndex = 8;
            // 
            // stLabel7
            // 
            this.stLabel7.AutoSize = true;
            this.stLabel7.Location = new System.Drawing.Point(-1, 193);
            this.stLabel7.Name = "stLabel7";
            this.stLabel7.Size = new System.Drawing.Size(63, 13);
            this.stLabel7.TabIndex = 7;
            this.stLabel7.Text = "Char Width:";
            // 
            // fontHeightUD
            // 
            this.fontHeightUD.Location = new System.Drawing.Point(86, 295);
            this.fontHeightUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.fontHeightUD.Name = "fontHeightUD";
            this.fontHeightUD.Size = new System.Drawing.Size(97, 20);
            this.fontHeightUD.TabIndex = 6;
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(3, 297);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(41, 13);
            this.stLabel6.TabIndex = 5;
            this.stLabel6.Text = "Height:";
            // 
            // fontWidthUD
            // 
            this.fontWidthUD.Location = new System.Drawing.Point(86, 269);
            this.fontWidthUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.fontWidthUD.Name = "fontWidthUD";
            this.fontWidthUD.Size = new System.Drawing.Size(97, 20);
            this.fontWidthUD.TabIndex = 4;
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(2, 271);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(38, 13);
            this.stLabel5.TabIndex = 3;
            this.stLabel5.Text = "Width:";
            // 
            // ascentUD
            // 
            this.ascentUD.Location = new System.Drawing.Point(86, 243);
            this.ascentUD.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.ascentUD.Name = "ascentUD";
            this.ascentUD.Size = new System.Drawing.Size(97, 20);
            this.ascentUD.TabIndex = 2;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(2, 245);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(43, 13);
            this.stLabel4.TabIndex = 1;
            this.stLabel4.Text = "Ascent:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(3, 9);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(28, 13);
            this.stLabel1.TabIndex = 0;
            this.stLabel1.Text = "Font";
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stLabel2);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.stPanel2.Location = new System.Drawing.Point(619, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(200, 438);
            this.stPanel2.TabIndex = 1;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(13, 9);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(58, 13);
            this.stLabel2.TabIndex = 1;
            this.stLabel2.Text = "Characters";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(201, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 438);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(616, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 438);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.pictureBoxCustom1);
            this.stPanel3.Controls.Add(this.imagesCB);
            this.stPanel3.Controls.Add(this.stLabel3);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(204, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(412, 438);
            this.stPanel3.TabIndex = 4;
            // 
            // imagesCB
            // 
            this.imagesCB.BorderColor = System.Drawing.Color.Empty;
            this.imagesCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.imagesCB.ButtonColor = System.Drawing.Color.Empty;
            this.imagesCB.FormattingEnabled = true;
            this.imagesCB.Location = new System.Drawing.Point(82, 6);
            this.imagesCB.Name = "imagesCB";
            this.imagesCB.ReadOnly = true;
            this.imagesCB.Size = new System.Drawing.Size(213, 21);
            this.imagesCB.TabIndex = 1;
            this.imagesCB.SelectedIndexChanged += new System.EventHandler(this.imagesCB_SelectedIndexChanged);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(19, 9);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(41, 13);
            this.stLabel3.TabIndex = 0;
            this.stLabel3.Text = "Images";
            // 
            // stPanel4
            // 
            this.stPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel4.Controls.Add(this.stPanel3);
            this.stPanel4.Controls.Add(this.splitter2);
            this.stPanel4.Controls.Add(this.splitter1);
            this.stPanel4.Controls.Add(this.stPanel2);
            this.stPanel4.Controls.Add(this.stPanel1);
            this.stPanel4.Location = new System.Drawing.Point(0, 24);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(819, 438);
            this.stPanel4.TabIndex = 2;
            // 
            // imageMenuStrip
            // 
            this.imageMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.copyToolStripMenuItem});
            this.imageMenuStrip.Name = "stContextMenuStrip1";
            this.imageMenuStrip.Size = new System.Drawing.Size(181, 70);
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.ContextMenuStrip = this.imageMenuStrip;
            this.pictureBoxCustom1.Location = new System.Drawing.Point(6, 36);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(400, 399);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 2;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // BffntEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel4);
            this.Name = "BffntEditor";
            this.Size = new System.Drawing.Size(819, 462);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lineFeedUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftSpacingUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.glyphWidthCB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.charWidthUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontHeightUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontWidthUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ascentUD)).EndInit();
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.stPanel3.ResumeLayout(false);
            this.stPanel3.PerformLayout();
            this.stPanel4.ResumeLayout(false);
            this.imageMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private Toolbox.Library.Forms.STComboBox imagesCB;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.NumericUpDownUint ascentUD;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel10;
        private Toolbox.Library.Forms.NumericUpDownUint leftSpacingUD;
        private Toolbox.Library.Forms.STLabel stLabel9;
        private Toolbox.Library.Forms.NumericUpDownUint glyphWidthCB;
        private Toolbox.Library.Forms.STLabel stLabel8;
        private Toolbox.Library.Forms.NumericUpDownUint charWidthUD;
        private Toolbox.Library.Forms.STLabel stLabel7;
        private Toolbox.Library.Forms.NumericUpDownUint fontHeightUD;
        private Toolbox.Library.Forms.STLabel stLabel6;
        private Toolbox.Library.Forms.NumericUpDownUint fontWidthUD;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.STComboBox fontTypeCB;
        private Toolbox.Library.Forms.STComboBox encodingTypeCB;
        private Toolbox.Library.Forms.STLabel stLabel12;
        private Toolbox.Library.Forms.STLabel stLabel11;
        private Toolbox.Library.Forms.STPanel stPanel4;
        private Toolbox.Library.Forms.NumericUpDownUint lineFeedUD;
        private Toolbox.Library.Forms.STLabel stLabel13;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private Toolbox.Library.Forms.STContextMenuStrip imageMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}