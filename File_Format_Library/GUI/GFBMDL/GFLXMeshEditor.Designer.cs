namespace FirstPlugin.Forms
{
    partial class GFLXMeshEditor
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
            this.materialCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stDropDownPanel1 = new Toolbox.Library.Forms.STDropDownPanel();
            this.polyGroupCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // materialCB
            // 
            this.materialCB.BorderColor = System.Drawing.Color.Empty;
            this.materialCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.materialCB.ButtonColor = System.Drawing.Color.Empty;
            this.materialCB.FormattingEnabled = true;
            this.materialCB.IsReadOnly = false;
            this.materialCB.Location = new System.Drawing.Point(96, 68);
            this.materialCB.Name = "materialCB";
            this.materialCB.Size = new System.Drawing.Size(139, 21);
            this.materialCB.TabIndex = 0;
            this.materialCB.SelectedIndexChanged += new System.EventHandler(this.materialCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(16, 68);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(47, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Material:";
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(440, 404);
            this.stFlowLayoutPanel1.TabIndex = 2;
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.stLabel2);
            this.stDropDownPanel1.Controls.Add(this.polyGroupCB);
            this.stDropDownPanel1.Controls.Add(this.stLabel1);
            this.stDropDownPanel1.Controls.Add(this.materialCB);
            this.stDropDownPanel1.ExpandedHeight = 0;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Polygon Group";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(440, 164);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // polyGroupCB
            // 
            this.polyGroupCB.BorderColor = System.Drawing.Color.Empty;
            this.polyGroupCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.polyGroupCB.ButtonColor = System.Drawing.Color.Empty;
            this.polyGroupCB.FormattingEnabled = true;
            this.polyGroupCB.IsReadOnly = false;
            this.polyGroupCB.Location = new System.Drawing.Point(96, 38);
            this.polyGroupCB.Name = "polyGroupCB";
            this.polyGroupCB.Size = new System.Drawing.Size(139, 21);
            this.polyGroupCB.TabIndex = 2;
            this.polyGroupCB.SelectedIndexChanged += new System.EventHandler(this.polyGroupCB_SelectedIndexChanged);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(16, 38);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(39, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Group:";
            // 
            // GFLXMeshEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "GFLXMeshEditor";
            this.Size = new System.Drawing.Size(440, 404);
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STComboBox materialCB;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox polyGroupCB;
    }
}
