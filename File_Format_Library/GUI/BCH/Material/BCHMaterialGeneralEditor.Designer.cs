namespace FirstPlugin.CtrLibrary.Forms
{
    partial class BCHMaterialGeneralEditor
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
            this.chkEnableOcclusion = new Toolbox.Library.Forms.STCheckBox();
            this.chkEnableHemiLighting = new Toolbox.Library.Forms.STCheckBox();
            this.chkEnableVertLighting = new Toolbox.Library.Forms.STCheckBox();
            this.chkEnableFragLighting = new Toolbox.Library.Forms.STCheckBox();
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stDropDownPanel2 = new Toolbox.Library.Forms.STDropDownPanel();
            this.faceCullingCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stDropDownPanel3 = new Toolbox.Library.Forms.STDropDownPanel();
            this.fogIndexUD = new Toolbox.Library.Forms.NumericUpDownInt();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.chkEnableFog = new Toolbox.Library.Forms.STCheckBox();
            this.stDropDownPanel1.SuspendLayout();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel2.SuspendLayout();
            this.stDropDownPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fogIndexUD)).BeginInit();
            this.SuspendLayout();
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.chkEnableOcclusion);
            this.stDropDownPanel1.Controls.Add(this.chkEnableHemiLighting);
            this.stDropDownPanel1.Controls.Add(this.chkEnableVertLighting);
            this.stDropDownPanel1.Controls.Add(this.chkEnableFragLighting);
            this.stDropDownPanel1.ExpandedHeight = 0;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Lighting";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(452, 132);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // chkEnableOcclusion
            // 
            this.chkEnableOcclusion.AutoSize = true;
            this.chkEnableOcclusion.Location = new System.Drawing.Point(180, 60);
            this.chkEnableOcclusion.Name = "chkEnableOcclusion";
            this.chkEnableOcclusion.Size = new System.Drawing.Size(168, 17);
            this.chkEnableOcclusion.TabIndex = 8;
            this.chkEnableOcclusion.Text = "Enable Hemisphere Occlusion";
            this.chkEnableOcclusion.UseVisualStyleBackColor = true;
            // 
            // chkEnableHemiLighting
            // 
            this.chkEnableHemiLighting.AutoSize = true;
            this.chkEnableHemiLighting.Location = new System.Drawing.Point(16, 60);
            this.chkEnableHemiLighting.Name = "chkEnableHemiLighting";
            this.chkEnableHemiLighting.Size = new System.Drawing.Size(158, 17);
            this.chkEnableHemiLighting.TabIndex = 7;
            this.chkEnableHemiLighting.Text = "Enable Hemisphere Lighting";
            this.chkEnableHemiLighting.UseVisualStyleBackColor = true;
            // 
            // chkEnableVertLighting
            // 
            this.chkEnableVertLighting.AutoSize = true;
            this.chkEnableVertLighting.Location = new System.Drawing.Point(180, 37);
            this.chkEnableVertLighting.Name = "chkEnableVertLighting";
            this.chkEnableVertLighting.Size = new System.Drawing.Size(132, 17);
            this.chkEnableVertLighting.TabIndex = 6;
            this.chkEnableVertLighting.Text = "Enable Vertex Lighting";
            this.chkEnableVertLighting.UseVisualStyleBackColor = true;
            // 
            // chkEnableFragLighting
            // 
            this.chkEnableFragLighting.AutoSize = true;
            this.chkEnableFragLighting.Location = new System.Drawing.Point(16, 37);
            this.chkEnableFragLighting.Name = "chkEnableFragLighting";
            this.chkEnableFragLighting.Size = new System.Drawing.Size(146, 17);
            this.chkEnableFragLighting.TabIndex = 5;
            this.chkEnableFragLighting.Text = "Enable Fragment Lighting";
            this.chkEnableFragLighting.UseVisualStyleBackColor = true;
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.AutoScroll = true;
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel2);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel3);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(452, 451);
            this.stFlowLayoutPanel1.TabIndex = 1;
            // 
            // stDropDownPanel2
            // 
            this.stDropDownPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel2.Controls.Add(this.faceCullingCB);
            this.stDropDownPanel2.Controls.Add(this.stLabel1);
            this.stDropDownPanel2.ExpandedHeight = 103;
            this.stDropDownPanel2.IsExpanded = true;
            this.stDropDownPanel2.Location = new System.Drawing.Point(0, 132);
            this.stDropDownPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel2.Name = "stDropDownPanel2";
            this.stDropDownPanel2.PanelName = "Rendering";
            this.stDropDownPanel2.PanelValueName = "";
            this.stDropDownPanel2.SetIcon = null;
            this.stDropDownPanel2.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.Size = new System.Drawing.Size(452, 77);
            this.stDropDownPanel2.TabIndex = 1;
            // 
            // faceCullingCB
            // 
            this.faceCullingCB.BorderColor = System.Drawing.Color.Empty;
            this.faceCullingCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.faceCullingCB.ButtonColor = System.Drawing.Color.Empty;
            this.faceCullingCB.FormattingEnabled = true;
            this.faceCullingCB.IsReadOnly = false;
            this.faceCullingCB.Location = new System.Drawing.Point(87, 32);
            this.faceCullingCB.Name = "faceCullingCB";
            this.faceCullingCB.Size = new System.Drawing.Size(121, 21);
            this.faceCullingCB.TabIndex = 2;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(13, 35);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(68, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Face Culling:";
            // 
            // stDropDownPanel3
            // 
            this.stDropDownPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel3.Controls.Add(this.fogIndexUD);
            this.stDropDownPanel3.Controls.Add(this.stLabel2);
            this.stDropDownPanel3.Controls.Add(this.chkEnableFog);
            this.stDropDownPanel3.ExpandedHeight = 103;
            this.stDropDownPanel3.IsExpanded = true;
            this.stDropDownPanel3.Location = new System.Drawing.Point(0, 209);
            this.stDropDownPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel3.Name = "stDropDownPanel3";
            this.stDropDownPanel3.PanelName = "Fog";
            this.stDropDownPanel3.PanelValueName = "";
            this.stDropDownPanel3.SetIcon = null;
            this.stDropDownPanel3.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.Size = new System.Drawing.Size(452, 103);
            this.stDropDownPanel3.TabIndex = 3;
            // 
            // fogIndexUD
            // 
            this.fogIndexUD.Location = new System.Drawing.Point(87, 65);
            this.fogIndexUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.fogIndexUD.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.fogIndexUD.Name = "fogIndexUD";
            this.fogIndexUD.Size = new System.Drawing.Size(121, 20);
            this.fogIndexUD.TabIndex = 4;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(13, 65);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(57, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Fog Index:";
            // 
            // chkEnableFog
            // 
            this.chkEnableFog.AutoSize = true;
            this.chkEnableFog.Location = new System.Drawing.Point(16, 33);
            this.chkEnableFog.Name = "chkEnableFog";
            this.chkEnableFog.Size = new System.Drawing.Size(83, 17);
            this.chkEnableFog.TabIndex = 2;
            this.chkEnableFog.Text = "Enable Fog:";
            this.chkEnableFog.UseVisualStyleBackColor = true;
            // 
            // BCHMaterialGeneralEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "BCHMaterialGeneralEditor";
            this.Size = new System.Drawing.Size(452, 451);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stDropDownPanel2.ResumeLayout(false);
            this.stDropDownPanel2.PerformLayout();
            this.stDropDownPanel3.ResumeLayout(false);
            this.stDropDownPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fogIndexUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel2;
        private Toolbox.Library.Forms.STComboBox faceCullingCB;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel3;
        private Toolbox.Library.Forms.NumericUpDownInt fogIndexUD;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STCheckBox chkEnableFog;
        private Toolbox.Library.Forms.STCheckBox chkEnableOcclusion;
        private Toolbox.Library.Forms.STCheckBox chkEnableHemiLighting;
        private Toolbox.Library.Forms.STCheckBox chkEnableVertLighting;
        private Toolbox.Library.Forms.STCheckBox chkEnableFragLighting;
    }
}
