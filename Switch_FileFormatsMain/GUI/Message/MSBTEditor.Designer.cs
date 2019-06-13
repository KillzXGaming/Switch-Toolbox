namespace FirstPlugin.Forms
{
    partial class MSBTEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.editTextTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.originalTextTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.hexEditor1 = new Switch_Toolbox.Library.Forms.HexEditor();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            this.Controls.Add(this.splitContainer1);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewCustom1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(916, 495);
            this.splitContainer1.SplitterDistance = 305;
            this.splitContainer1.TabIndex = 11;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(305, 495);
            this.listViewCustom1.TabIndex = 0;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 181;
            // 
            // editTextTB
            // 
            this.editTextTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editTextTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editTextTB.Location = new System.Drawing.Point(3, 16);
            this.editTextTB.Multiline = true;
            this.editTextTB.Name = "editTextTB";
            this.editTextTB.Size = new System.Drawing.Size(200, 286);
            this.editTextTB.TabIndex = 0;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(0, 0);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(28, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Edit:";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(3, 0);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(45, 13);
            this.stLabel2.TabIndex = 2;
            this.stLabel2.Text = "Original:";
            // 
            // originalTextTB
            // 
            this.originalTextTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.originalTextTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.originalTextTB.Location = new System.Drawing.Point(3, 16);
            this.originalTextTB.Multiline = true;
            this.originalTextTB.Name = "originalTextTB";
            this.originalTextTB.ReadOnly = true;
            this.originalTextTB.Size = new System.Drawing.Size(385, 286);
            this.originalTextTB.TabIndex = 3;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(13, 4);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(55, 13);
            this.stLabel3.TabIndex = 4;
            this.stLabel3.Text = "Hex View:";
            // 
            // hexEditor1
            // 
            this.hexEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexEditor1.Location = new System.Drawing.Point(13, 29);
            this.hexEditor1.Name = "hexEditor1";
            this.hexEditor1.Size = new System.Drawing.Size(591, 154);
            this.hexEditor1.TabIndex = 5;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.hexEditor1);
            this.splitContainer2.Panel2.Controls.Add(this.stLabel3);
            this.splitContainer2.Size = new System.Drawing.Size(607, 495);
            this.splitContainer2.SplitterDistance = 305;
            this.splitContainer2.TabIndex = 6;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.editTextTB);
            this.splitContainer3.Panel1.Controls.Add(this.stLabel1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.originalTextTB);
            this.splitContainer3.Panel2.Controls.Add(this.stLabel2);
            this.splitContainer3.Size = new System.Drawing.Size(607, 305);
            this.splitContainer3.SplitterDistance = 202;
            this.splitContainer3.TabIndex = 0;
            // 
            // MSBTEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 525);
            this.Name = "MSBTEditor";
            this.Text = "MSBTEditor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Switch_Toolbox.Library.Forms.HexEditor hexEditor1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private Switch_Toolbox.Library.Forms.STTextBox originalTextTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STTextBox editTextTB;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
    }
}