namespace LayoutBXLYT
{
    partial class PaneAnimationController
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
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stDropDownPanel1 = new Toolbox.Library.Forms.STDropDownPanel();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stDropDownPanel2 = new Toolbox.Library.Forms.STDropDownPanel();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel1.SuspendLayout();
            this.stDropDownPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel2);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(348, 599);
            this.stFlowLayoutPanel1.TabIndex = 0;
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
            this.stDropDownPanel1.PanelName = "Animation References";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(348, 195);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom1.HideSelection = false;
            this.listViewCustom1.Location = new System.Drawing.Point(3, 26);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(342, 158);
            this.listViewCustom1.TabIndex = 1;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 342;
            // 
            // stDropDownPanel2
            // 
            this.stDropDownPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel2.Controls.Add(this.stPanel1);
            this.stDropDownPanel2.ExpandedHeight = 0;
            this.stDropDownPanel2.IsExpanded = true;
            this.stDropDownPanel2.Location = new System.Drawing.Point(0, 195);
            this.stDropDownPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel2.Name = "stDropDownPanel2";
            this.stDropDownPanel2.PanelName = "Animation Data";
            this.stDropDownPanel2.PanelValueName = "";
            this.stDropDownPanel2.SetIcon = null;
            this.stDropDownPanel2.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.Size = new System.Drawing.Size(348, 404);
            this.stDropDownPanel2.TabIndex = 2;
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(3, 25);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(345, 385);
            this.stPanel1.TabIndex = 1;
            // 
            // PaneAnimationController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "PaneAnimationController";
            this.Size = new System.Drawing.Size(348, 599);
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stDropDownPanel2.ResumeLayout(false);
            this.stDropDownPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel2;
        private Toolbox.Library.Forms.STPanel stPanel1;
    }
}
