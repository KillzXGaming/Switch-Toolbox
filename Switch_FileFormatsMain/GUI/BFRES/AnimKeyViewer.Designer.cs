namespace FirstPlugin.Forms
{
    partial class AnimKeyViewer
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
            this.stListView1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            this.btnScrolDown = new Switch_Toolbox.Library.Forms.STButton();
            this.btnScrollUp = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton3 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton4 = new Switch_Toolbox.Library.Forms.STButton();
            this.numericUpDownFloat1 = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat2 = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat3 = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.numericUpDownFloat4 = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat4)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.numericUpDownFloat4);
            this.contentContainer.Controls.Add(this.numericUpDownFloat3);
            this.contentContainer.Controls.Add(this.numericUpDownFloat2);
            this.contentContainer.Controls.Add(this.numericUpDownFloat1);
            this.contentContainer.Controls.Add(this.stButton3);
            this.contentContainer.Controls.Add(this.stButton4);
            this.contentContainer.Controls.Add(this.btnScrolDown);
            this.contentContainer.Controls.Add(this.btnScrollUp);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.stListView1);
            this.contentContainer.Controls.SetChildIndex(this.stListView1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnScrollUp, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnScrolDown, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton4, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton3, 0);
            this.contentContainer.Controls.SetChildIndex(this.numericUpDownFloat1, 0);
            this.contentContainer.Controls.SetChildIndex(this.numericUpDownFloat2, 0);
            this.contentContainer.Controls.SetChildIndex(this.numericUpDownFloat3, 0);
            this.contentContainer.Controls.SetChildIndex(this.numericUpDownFloat4, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            // 
            // stListView1
            // 
            this.stListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.stListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.stListView1.Location = new System.Drawing.Point(7, 40);
            this.stListView1.Name = "stListView1";
            this.stListView1.Size = new System.Drawing.Size(205, 312);
            this.stListView1.TabIndex = 0;
            this.stListView1.UseCompatibleStateImageBehavior = false;
            this.stListView1.View = System.Windows.Forms.View.Details;
            this.stListView1.SelectedIndexChanged += new System.EventHandler(this.stListView1_SelectedIndexChanged);
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(7, 358);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(96, 23);
            this.stButton1.TabIndex = 1;
            this.stButton1.Text = "Add";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(116, 358);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(96, 23);
            this.stButton2.TabIndex = 2;
            this.stButton2.Text = "Remove";
            this.stButton2.UseVisualStyleBackColor = false;
            this.stButton2.Click += new System.EventHandler(this.stButton2_Click);
            // 
            // btnScrolDown
            // 
            this.btnScrolDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrolDown.Enabled = false;
            this.btnScrolDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrolDown.Location = new System.Drawing.Point(218, 69);
            this.btnScrolDown.Name = "btnScrolDown";
            this.btnScrolDown.Size = new System.Drawing.Size(32, 24);
            this.btnScrolDown.TabIndex = 4;
            this.btnScrolDown.Text = "▼";
            this.btnScrolDown.UseVisualStyleBackColor = true;
            // 
            // btnScrollUp
            // 
            this.btnScrollUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrollUp.Enabled = false;
            this.btnScrollUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrollUp.Location = new System.Drawing.Point(218, 40);
            this.btnScrollUp.Name = "btnScrollUp";
            this.btnScrollUp.Size = new System.Drawing.Size(32, 24);
            this.btnScrollUp.TabIndex = 3;
            this.btnScrollUp.Text = "▲";
            this.btnScrollUp.UseVisualStyleBackColor = true;
            // 
            // stButton3
            // 
            this.stButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton3.Location = new System.Drawing.Point(442, 358);
            this.stButton3.Name = "stButton3";
            this.stButton3.Size = new System.Drawing.Size(71, 23);
            this.stButton3.TabIndex = 7;
            this.stButton3.Text = "Cancel";
            this.stButton3.UseVisualStyleBackColor = false;
            // 
            // stButton4
            // 
            this.stButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton4.Location = new System.Drawing.Point(367, 358);
            this.stButton4.Name = "stButton4";
            this.stButton4.Size = new System.Drawing.Size(69, 23);
            this.stButton4.TabIndex = 6;
            this.stButton4.Text = "Ok";
            this.stButton4.UseVisualStyleBackColor = false;
            // 
            // numericUpDownFloat1
            // 
            this.numericUpDownFloat1.DecimalPlaces = 5;
            this.numericUpDownFloat1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat1.Location = new System.Drawing.Point(296, 89);
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
            this.numericUpDownFloat1.Size = new System.Drawing.Size(206, 20);
            this.numericUpDownFloat1.TabIndex = 8;
            // 
            // numericUpDownFloat2
            // 
            this.numericUpDownFloat2.DecimalPlaces = 5;
            this.numericUpDownFloat2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat2.Location = new System.Drawing.Point(296, 115);
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
            this.numericUpDownFloat2.Size = new System.Drawing.Size(206, 20);
            this.numericUpDownFloat2.TabIndex = 9;
            // 
            // numericUpDownFloat3
            // 
            this.numericUpDownFloat3.DecimalPlaces = 5;
            this.numericUpDownFloat3.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat3.Location = new System.Drawing.Point(296, 141);
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
            this.numericUpDownFloat3.Size = new System.Drawing.Size(206, 20);
            this.numericUpDownFloat3.TabIndex = 10;
            // 
            // numericUpDownFloat4
            // 
            this.numericUpDownFloat4.DecimalPlaces = 5;
            this.numericUpDownFloat4.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat4.Location = new System.Drawing.Point(296, 167);
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
            this.numericUpDownFloat4.Size = new System.Drawing.Size(206, 20);
            this.numericUpDownFloat4.TabIndex = 11;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(293, 60);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(47, 13);
            this.stLabel1.TabIndex = 12;
            this.stLabel1.Text = "stLabel1";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "1";
            // 
            // AnimKeyViewer
            // 
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "AnimKeyViewer";
            this.Text = "AnimKeyViewer";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.ListViewCustom stListView1;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STButton stButton2;
        private Switch_Toolbox.Library.Forms.STButton btnScrolDown;
        private Switch_Toolbox.Library.Forms.STButton btnScrollUp;
        private Switch_Toolbox.Library.Forms.STButton stButton3;
        private Switch_Toolbox.Library.Forms.STButton stButton4;
        private Switch_Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat1;
        private Switch_Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat2;
        private Switch_Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat3;
        private Switch_Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat4;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}