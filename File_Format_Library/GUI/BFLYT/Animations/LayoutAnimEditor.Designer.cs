namespace LayoutBXLYT
{
    partial class LayoutAnimEditor
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
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.numericUpDownFloat1 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stComboBox1 = new Toolbox.Library.Forms.STComboBox();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.stPropertyGrid1 = new Toolbox.Library.Forms.STPropertyGrid();
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stPanel2.SuspendLayout();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stTabControl1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(630, 347);
            this.stPanel2.TabIndex = 6;
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
            this.stTabControl1.Size = new System.Drawing.Size(630, 347);
            this.stTabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.stLabel2);
            this.tabPage1.Controls.Add(this.stLabel1);
            this.tabPage1.Controls.Add(this.numericUpDownFloat1);
            this.tabPage1.Controls.Add(this.stComboBox1);
            this.tabPage1.Controls.Add(this.stPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(622, 318);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Editor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(17, 11);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(41, 13);
            this.stLabel2.TabIndex = 7;
            this.stLabel2.Text = "Target:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(250, 11);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(39, 13);
            this.stLabel1.TabIndex = 6;
            this.stLabel1.Text = "Frame:";
            // 
            // numericUpDownFloat1
            // 
            this.numericUpDownFloat1.DecimalPlaces = 5;
            this.numericUpDownFloat1.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownFloat1.Location = new System.Drawing.Point(316, 9);
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
            this.numericUpDownFloat1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownFloat1.TabIndex = 0;
            // 
            // stComboBox1
            // 
            this.stComboBox1.BorderColor = System.Drawing.Color.Empty;
            this.stComboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.stComboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.stComboBox1.FormattingEnabled = true;
            this.stComboBox1.IsReadOnly = false;
            this.stComboBox1.Location = new System.Drawing.Point(64, 8);
            this.stComboBox1.Name = "stComboBox1";
            this.stComboBox1.Size = new System.Drawing.Size(166, 21);
            this.stComboBox1.TabIndex = 4;
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Controls.Add(this.stFlowLayoutPanel1);
            this.stPanel1.Location = new System.Drawing.Point(3, 32);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(616, 283);
            this.stPanel1.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.stPropertyGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(622, 318);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Properties";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.ShowHintDisplay = true;
            this.stPropertyGrid1.Size = new System.Drawing.Size(616, 312);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(616, 283);
            this.stFlowLayoutPanel1.TabIndex = 0;
            // 
            // LayoutAnimEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 347);
            this.Controls.Add(this.stPanel2);
            this.Name = "LayoutAnimEditor";
            this.Text = "LayoutAnimEditor";
            this.stPanel2.ResumeLayout(false);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat1;
        private Toolbox.Library.Forms.STComboBox stComboBox1;
        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
    }
}