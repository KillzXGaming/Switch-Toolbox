namespace FirstPlugin.Forms
{
    partial class CollisionMaterialEditor
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
            this.stListView1 = new Switch_Toolbox.Library.Forms.STListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.numericUpDownUint1 = new Switch_Toolbox.Library.Forms.NumericUpDownUint();
            this.stMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byMaterialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stListView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint1)).BeginInit();
            this.stMenuStrip1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Size = new System.Drawing.Size(235, 393);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            // 
            // stListView1
            // 
            this.stListView1.AllColumns.Add(this.olvColumn1);
            this.stListView1.AllColumns.Add(this.olvColumn2);
            this.stListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stListView1.CellEditUseWholeCell = false;
            this.stListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2});
            this.stListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.stListView1.Location = new System.Drawing.Point(-3, 53);
            this.stListView1.Name = "stListView1";
            this.stListView1.Size = new System.Drawing.Size(235, 277);
            this.stListView1.TabIndex = 0;
            this.stListView1.UseCompatibleStateImageBehavior = false;
            this.stListView1.View = System.Windows.Forms.View.Details;
            this.stListView1.SelectedIndexChanged += new System.EventHandler(this.stListView1_SelectedIndexChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "ID";
            this.olvColumn1.CellEditUseWholeCell = true;
            this.olvColumn1.Text = "ID";
            this.olvColumn1.Width = 92;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Type";
            this.olvColumn2.Text = "Type";
            this.olvColumn2.Width = 131;
            // 
            // numericUpDownUint1
            // 
            this.numericUpDownUint1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownUint1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownUint1.Location = new System.Drawing.Point(2, 27);
            this.numericUpDownUint1.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numericUpDownUint1.Name = "numericUpDownUint1";
            this.numericUpDownUint1.Size = new System.Drawing.Size(230, 20);
            this.numericUpDownUint1.TabIndex = 1;
            // 
            // stMenuStrip1
            // 
            this.stMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectToolStripMenuItem});
            this.stMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stMenuStrip1.Name = "stMenuStrip1";
            this.stMenuStrip1.Size = new System.Drawing.Size(235, 24);
            this.stMenuStrip1.TabIndex = 2;
            this.stMenuStrip1.Text = "stMenuStrip1";
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.byMaterialToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.selectToolStripMenuItem.Text = "Select";
            // 
            // byMaterialToolStripMenuItem
            // 
            this.byMaterialToolStripMenuItem.Name = "byMaterialToolStripMenuItem";
            this.byMaterialToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.byMaterialToolStripMenuItem.Text = "By Material";
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(72, 336);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(77, 26);
            this.stButton1.TabIndex = 3;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(155, 336);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(77, 26);
            this.stButton2.TabIndex = 4;
            this.stButton2.Text = "Cancel";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stListView1);
            this.stPanel1.Controls.Add(this.stButton1);
            this.stPanel1.Controls.Add(this.numericUpDownUint1);
            this.stPanel1.Controls.Add(this.stButton2);
            this.stPanel1.Controls.Add(this.stMenuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 25);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(235, 368);
            this.stPanel1.TabIndex = 11;
            // 
            // CollisionMaterialEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 398);
            this.MainMenuStrip = this.stMenuStrip1;
            this.Name = "CollisionMaterialEditor";
            this.Text = "Collision Materials";
            this.contentContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.stListView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUint1)).EndInit();
            this.stMenuStrip1.ResumeLayout(false);
            this.stMenuStrip1.PerformLayout();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STListView stListView1;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private Switch_Toolbox.Library.Forms.NumericUpDownUint numericUpDownUint1;
        private Switch_Toolbox.Library.Forms.STMenuStrip stMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byMaterialToolStripMenuItem;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STButton stButton2;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
    }
}