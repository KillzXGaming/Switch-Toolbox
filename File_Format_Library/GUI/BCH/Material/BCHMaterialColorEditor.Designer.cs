namespace FirstPlugin.CtrLibrary.Forms
{
    partial class BCHMaterialColorEditor
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
            this.stDropDownPanel1 = new Toolbox.Library.Forms.STDropDownPanel();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stDropDownPanel2 = new Toolbox.Library.Forms.STDropDownPanel();
            this.stColorControl1 = new Toolbox.Library.Forms.STColorControl();
            this.selectedColorLabel = new Toolbox.Library.Forms.STLabel();
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stDropDownPanel1.SuspendLayout();
            this.stDropDownPanel2.SuspendLayout();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.listViewCustom1);
            this.stDropDownPanel1.ExpandedHeight = 0;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Material Colors";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(603, 297);
            this.stDropDownPanel1.TabIndex = 2;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewCustom1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom1.HideSelection = false;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 21);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(586, 273);
            this.listViewCustom1.TabIndex = 1;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 143;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 61;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 240;
            // 
            // stDropDownPanel2
            // 
            this.stDropDownPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel2.Controls.Add(this.stColorControl1);
            this.stDropDownPanel2.Controls.Add(this.selectedColorLabel);
            this.stDropDownPanel2.ExpandedHeight = 0;
            this.stDropDownPanel2.IsExpanded = true;
            this.stDropDownPanel2.Location = new System.Drawing.Point(0, 297);
            this.stDropDownPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel2.Name = "stDropDownPanel2";
            this.stDropDownPanel2.PanelName = "Color Editor";
            this.stDropDownPanel2.PanelValueName = "";
            this.stDropDownPanel2.SetIcon = null;
            this.stDropDownPanel2.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.Size = new System.Drawing.Size(603, 313);
            this.stDropDownPanel2.TabIndex = 3;
            // 
            // stColorControl1
            // 
            this.stColorControl1.Alpha = 255;
            this.stColorControl1.ColorRGB = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.stColorControl1.Location = new System.Drawing.Point(6, 42);
            this.stColorControl1.Name = "stColorControl1";
            this.stColorControl1.NewColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.stColorControl1.Size = new System.Drawing.Size(391, 255);
            this.stColorControl1.TabIndex = 2;
            // 
            // selectedColorLabel
            // 
            this.selectedColorLabel.AutoSize = true;
            this.selectedColorLabel.Location = new System.Drawing.Point(3, 26);
            this.selectedColorLabel.Name = "selectedColorLabel";
            this.selectedColorLabel.Size = new System.Drawing.Size(116, 13);
            this.selectedColorLabel.TabIndex = 1;
            this.selectedColorLabel.Text = "Selected Color : NONE";
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.AutoScroll = true;
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel2);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(603, 632);
            this.stFlowLayoutPanel1.TabIndex = 4;
            // 
            // BCHMaterialColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "BCHMaterialColorEditor";
            this.Size = new System.Drawing.Size(603, 632);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stDropDownPanel2.ResumeLayout(false);
            this.stDropDownPanel2.PerformLayout();
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel2;
        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STColorControl stColorControl1;
        private Toolbox.Library.Forms.STLabel selectedColorLabel;
    }
}
