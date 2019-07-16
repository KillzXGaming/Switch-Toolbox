namespace FirstPlugin.Forms
{
    partial class BoneAnimEditor
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
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.btnRemove = new Toolbox.Library.Forms.STButton();
            this.frameCountLbl = new Toolbox.Library.Forms.STLabel();
            this.btnInsert = new Toolbox.Library.Forms.STButton();
            this.currentFrameUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.stLabel8 = new Toolbox.Library.Forms.STLabel();
            this.numericUpDownFloat11 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat3 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel7 = new Toolbox.Library.Forms.STLabel();
            this.numericUpDownFloat2 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat4 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            this.numericUpDownFloat7 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.numericUpDownFloat5 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.numericUpDownFloat6 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat1 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat8 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat9 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stPropertyGrid1 = new Toolbox.Library.Forms.STPropertyGrid();
            this.listViewCustom2 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stPanel2.SuspendLayout();
            this.stPanel4.SuspendLayout();
            this.stPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat9)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 234);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(802, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // stPanel2
            // 
            this.stPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stPanel2.Controls.Add(this.stPanel4);
            this.stPanel2.Controls.Add(this.splitter2);
            this.stPanel2.Controls.Add(this.stPanel3);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 237);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(802, 403);
            this.stPanel2.TabIndex = 2;
            this.stPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.stPanel2_Paint);
            // 
            // stPanel4
            // 
            this.stPanel4.Controls.Add(this.listViewCustom2);
            this.stPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel4.Location = new System.Drawing.Point(0, 0);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(800, 242);
            this.stPanel4.TabIndex = 27;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 242);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(800, 3);
            this.splitter2.TabIndex = 26;
            this.splitter2.TabStop = false;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.btnRemove);
            this.stPanel3.Controls.Add(this.frameCountLbl);
            this.stPanel3.Controls.Add(this.btnInsert);
            this.stPanel3.Controls.Add(this.currentFrameUD);
            this.stPanel3.Controls.Add(this.stLabel8);
            this.stPanel3.Controls.Add(this.numericUpDownFloat11);
            this.stPanel3.Controls.Add(this.numericUpDownFloat3);
            this.stPanel3.Controls.Add(this.stLabel7);
            this.stPanel3.Controls.Add(this.numericUpDownFloat2);
            this.stPanel3.Controls.Add(this.numericUpDownFloat4);
            this.stPanel3.Controls.Add(this.stLabel6);
            this.stPanel3.Controls.Add(this.numericUpDownFloat7);
            this.stPanel3.Controls.Add(this.stLabel5);
            this.stPanel3.Controls.Add(this.numericUpDownFloat5);
            this.stPanel3.Controls.Add(this.stLabel4);
            this.stPanel3.Controls.Add(this.stLabel3);
            this.stPanel3.Controls.Add(this.stLabel1);
            this.stPanel3.Controls.Add(this.numericUpDownFloat6);
            this.stPanel3.Controls.Add(this.numericUpDownFloat1);
            this.stPanel3.Controls.Add(this.numericUpDownFloat8);
            this.stPanel3.Controls.Add(this.numericUpDownFloat9);
            this.stPanel3.Controls.Add(this.stLabel2);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel3.Location = new System.Drawing.Point(0, 245);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(800, 156);
            this.stPanel3.TabIndex = 25;
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(345, 5);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(85, 23);
            this.btnRemove.TabIndex = 24;
            this.btnRemove.Text = "Remove Key";
            this.btnRemove.UseVisualStyleBackColor = false;
            // 
            // frameCountLbl
            // 
            this.frameCountLbl.AutoSize = true;
            this.frameCountLbl.Location = new System.Drawing.Point(179, 10);
            this.frameCountLbl.Name = "frameCountLbl";
            this.frameCountLbl.Size = new System.Drawing.Size(21, 13);
            this.frameCountLbl.TabIndex = 22;
            this.frameCountLbl.Text = "/ 0";
            // 
            // btnInsert
            // 
            this.btnInsert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInsert.Location = new System.Drawing.Point(254, 5);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(85, 23);
            this.btnInsert.TabIndex = 23;
            this.btnInsert.Text = "Insert Key";
            this.btnInsert.UseVisualStyleBackColor = false;
            // 
            // currentFrameUD
            // 
            this.currentFrameUD.Location = new System.Drawing.Point(87, 8);
            this.currentFrameUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.currentFrameUD.Name = "currentFrameUD";
            this.currentFrameUD.Size = new System.Drawing.Size(86, 20);
            this.currentFrameUD.TabIndex = 21;
            // 
            // stLabel8
            // 
            this.stLabel8.AutoSize = true;
            this.stLabel8.Location = new System.Drawing.Point(42, 10);
            this.stLabel8.Name = "stLabel8";
            this.stLabel8.Size = new System.Drawing.Size(39, 13);
            this.stLabel8.TabIndex = 20;
            this.stLabel8.Text = "Frame:";
            // 
            // numericUpDownFloat11
            // 
            this.numericUpDownFloat11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat11.DecimalPlaces = 5;
            this.numericUpDownFloat11.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat11.Location = new System.Drawing.Point(402, 94);
            this.numericUpDownFloat11.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat11.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat11.Name = "numericUpDownFloat11";
            this.numericUpDownFloat11.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat11.TabIndex = 17;
            // 
            // numericUpDownFloat3
            // 
            this.numericUpDownFloat3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat3.DecimalPlaces = 5;
            this.numericUpDownFloat3.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat3.Location = new System.Drawing.Point(87, 127);
            this.numericUpDownFloat3.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat3.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat3.Name = "numericUpDownFloat3";
            this.numericUpDownFloat3.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat3.TabIndex = 9;
            // 
            // stLabel7
            // 
            this.stLabel7.AutoSize = true;
            this.stLabel7.Location = new System.Drawing.Point(411, 43);
            this.stLabel7.Name = "stLabel7";
            this.stLabel7.Size = new System.Drawing.Size(18, 13);
            this.stLabel7.TabIndex = 18;
            this.stLabel7.Text = "W";
            // 
            // numericUpDownFloat2
            // 
            this.numericUpDownFloat2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat2.DecimalPlaces = 5;
            this.numericUpDownFloat2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat2.Location = new System.Drawing.Point(87, 94);
            this.numericUpDownFloat2.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat2.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat2.Name = "numericUpDownFloat2";
            this.numericUpDownFloat2.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat2.TabIndex = 8;
            // 
            // numericUpDownFloat4
            // 
            this.numericUpDownFloat4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat4.DecimalPlaces = 5;
            this.numericUpDownFloat4.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat4.Location = new System.Drawing.Point(191, 127);
            this.numericUpDownFloat4.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat4.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat4.Name = "numericUpDownFloat4";
            this.numericUpDownFloat4.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat4.TabIndex = 12;
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(17, 129);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(37, 13);
            this.stLabel6.TabIndex = 7;
            this.stLabel6.Text = "Scale:";
            // 
            // numericUpDownFloat7
            // 
            this.numericUpDownFloat7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat7.DecimalPlaces = 5;
            this.numericUpDownFloat7.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat7.Location = new System.Drawing.Point(299, 127);
            this.numericUpDownFloat7.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat7.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat7.Name = "numericUpDownFloat7";
            this.numericUpDownFloat7.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat7.TabIndex = 15;
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(17, 96);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(50, 13);
            this.stLabel5.TabIndex = 6;
            this.stLabel5.Text = "Rotation:";
            // 
            // numericUpDownFloat5
            // 
            this.numericUpDownFloat5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat5.DecimalPlaces = 5;
            this.numericUpDownFloat5.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat5.Location = new System.Drawing.Point(191, 94);
            this.numericUpDownFloat5.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat5.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat5.Name = "numericUpDownFloat5";
            this.numericUpDownFloat5.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat5.TabIndex = 11;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(13, 61);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(54, 13);
            this.stLabel4.TabIndex = 5;
            this.stLabel4.Text = "Translate:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(342, 43);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(14, 13);
            this.stLabel3.TabIndex = 4;
            this.stLabel3.Text = "Z";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(84, 43);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(14, 13);
            this.stLabel1.TabIndex = 2;
            this.stLabel1.Text = "X";
            // 
            // numericUpDownFloat6
            // 
            this.numericUpDownFloat6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat6.DecimalPlaces = 5;
            this.numericUpDownFloat6.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat6.Location = new System.Drawing.Point(191, 59);
            this.numericUpDownFloat6.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat6.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat6.Name = "numericUpDownFloat6";
            this.numericUpDownFloat6.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat6.TabIndex = 10;
            // 
            // numericUpDownFloat1
            // 
            this.numericUpDownFloat1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat1.DecimalPlaces = 5;
            this.numericUpDownFloat1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat1.Location = new System.Drawing.Point(87, 59);
            this.numericUpDownFloat1.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat1.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat1.Name = "numericUpDownFloat1";
            this.numericUpDownFloat1.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat1.TabIndex = 1;
            // 
            // numericUpDownFloat8
            // 
            this.numericUpDownFloat8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat8.DecimalPlaces = 5;
            this.numericUpDownFloat8.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat8.Location = new System.Drawing.Point(299, 94);
            this.numericUpDownFloat8.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat8.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat8.Name = "numericUpDownFloat8";
            this.numericUpDownFloat8.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat8.TabIndex = 14;
            // 
            // numericUpDownFloat9
            // 
            this.numericUpDownFloat9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFloat9.DecimalPlaces = 5;
            this.numericUpDownFloat9.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat9.Location = new System.Drawing.Point(299, 59);
            this.numericUpDownFloat9.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat9.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat9.Name = "numericUpDownFloat9";
            this.numericUpDownFloat9.Size = new System.Drawing.Size(97, 20);
            this.numericUpDownFloat9.TabIndex = 13;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(214, 43);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(14, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Y";
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stPropertyGrid1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(802, 234);
            this.stPanel1.TabIndex = 0;
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.ShowHintDisplay = false;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.Size = new System.Drawing.Size(802, 234);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // listViewCustom2
            // 
            this.listViewCustom2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.listViewCustom2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCustom2.FullRowSelect = true;
            this.listViewCustom2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom2.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom2.MultiSelect = false;
            this.listViewCustom2.Name = "listViewCustom2";
            this.listViewCustom2.OwnerDraw = true;
            this.listViewCustom2.Size = new System.Drawing.Size(800, 242);
            this.listViewCustom2.TabIndex = 1;
            this.listViewCustom2.UseCompatibleStateImageBehavior = false;
            this.listViewCustom2.View = System.Windows.Forms.View.Details;
            this.listViewCustom2.SelectedIndexChanged += new System.EventHandler(this.listViewCustom2_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Frame";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "TX";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "TY";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "TZ";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "RX";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "RY";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "RZ";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "RW";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "SX";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "SY";
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "SZ";
            // 
            // BoneAnimEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.stPanel1);
            this.Name = "BoneAnimEditor";
            this.Size = new System.Drawing.Size(802, 640);
            this.stPanel2.ResumeLayout(false);
            this.stPanel4.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.stPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrameUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat9)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STLabel stLabel7;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat11;
        private Toolbox.Library.Forms.STButton btnRemove;
        private Toolbox.Library.Forms.STButton btnInsert;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat7;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat8;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat9;
        private Toolbox.Library.Forms.STLabel frameCountLbl;
        private Toolbox.Library.Forms.NumericUpDownUint currentFrameUD;
        private Toolbox.Library.Forms.STLabel stLabel8;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat4;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat5;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat6;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat3;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat2;
        private Toolbox.Library.Forms.STLabel stLabel6;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat1;
        private System.Windows.Forms.Splitter splitter2;
        private Toolbox.Library.Forms.STPanel stPanel4;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
    }
}
