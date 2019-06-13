namespace FirstPlugin.Forms
{
    partial class EffectTableEditor
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
            this.stDataGridView1 = new Switch_Toolbox.Library.Forms.STDataGridView();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.addPTCLReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.stDataGridView1)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.stMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            this.Controls.Add(this.stPanel1);
            // 
            // stDataGridView1
            // 
            this.stDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stDataGridView1.BackgroundColor = System.Drawing.Color.Gray;
            this.stDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stDataGridView1.EnableHeadersVisualStyles = false;
            this.stDataGridView1.GridColor = System.Drawing.Color.Black;
            this.stDataGridView1.Location = new System.Drawing.Point(0, 27);
            this.stDataGridView1.Name = "stDataGridView1";
            this.stDataGridView1.Size = new System.Drawing.Size(540, 341);
            this.stDataGridView1.TabIndex = 11;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stDataGridView1);
            this.stPanel1.Controls.Add(this.stMenuStrip1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel1.Location = new System.Drawing.Point(0, 25);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(543, 368);
            this.stPanel1.TabIndex = 12;
            // 
            // stMenuStrip1
            // 
            this.stMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPTCLReferenceToolStripMenuItem});
            this.stMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stMenuStrip1.Name = "stMenuStrip1";
            this.stMenuStrip1.Size = new System.Drawing.Size(543, 24);
            this.stMenuStrip1.TabIndex = 12;
            this.stMenuStrip1.Text = "stMenuStrip1";
            // 
            // addPTCLReferenceToolStripMenuItem
            // 
            this.addPTCLReferenceToolStripMenuItem.Name = "addPTCLReferenceToolStripMenuItem";
            this.addPTCLReferenceToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.addPTCLReferenceToolStripMenuItem.Text = "Link PTCL";
            this.addPTCLReferenceToolStripMenuItem.Click += new System.EventHandler(this.addPTCLReferenceToolStripMenuItem_Click);
            // 
            // EffectTableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "EffectTableEditor";
            this.Text = "EffectTableEditor";
            ((System.ComponentModel.ISupportInitialize)(this.stDataGridView1)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stMenuStrip1.ResumeLayout(false);
            this.stMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STDataGridView stDataGridView1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
        private Switch_Toolbox.Library.Forms.STMenuStrip stMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addPTCLReferenceToolStripMenuItem;
    }
}