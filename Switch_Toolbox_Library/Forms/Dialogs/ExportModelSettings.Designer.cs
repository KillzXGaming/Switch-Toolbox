namespace Toolbox.Library.Forms
{
    partial class ExportModelSettings
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
            this.exportTexturesChkBox = new Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.chkFlipUvsVertical = new Toolbox.Library.Forms.STCheckBox();
            this.chkOldExporter = new Toolbox.Library.Forms.STCheckBox();
            this.chkVertexColors = new Toolbox.Library.Forms.STCheckBox();
            this.chkExportRiggedBonesOnly = new Toolbox.Library.Forms.STCheckBox();
            this.chkApplyUVTransforms = new Toolbox.Library.Forms.STCheckBox();
            this.chkTextureChannelComps = new Toolbox.Library.Forms.STCheckBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.chkTextureChannelComps);
            this.contentContainer.Controls.Add(this.chkApplyUVTransforms);
            this.contentContainer.Controls.Add(this.chkExportRiggedBonesOnly);
            this.contentContainer.Controls.Add(this.chkVertexColors);
            this.contentContainer.Controls.Add(this.chkOldExporter);
            this.contentContainer.Controls.Add(this.chkFlipUvsVertical);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.exportTexturesChkBox);
            this.contentContainer.Size = new System.Drawing.Size(338, 267);
            this.contentContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.contentContainer_Paint);
            this.contentContainer.Controls.SetChildIndex(this.exportTexturesChkBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkFlipUvsVertical, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkOldExporter, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkVertexColors, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkExportRiggedBonesOnly, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkApplyUVTransforms, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkTextureChannelComps, 0);
            // 
            // exportTexturesChkBox
            // 
            this.exportTexturesChkBox.AutoSize = true;
            this.exportTexturesChkBox.Checked = true;
            this.exportTexturesChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportTexturesChkBox.Location = new System.Drawing.Point(23, 47);
            this.exportTexturesChkBox.Name = "exportTexturesChkBox";
            this.exportTexturesChkBox.Size = new System.Drawing.Size(100, 17);
            this.exportTexturesChkBox.TabIndex = 11;
            this.exportTexturesChkBox.Text = "Export Textures";
            this.exportTexturesChkBox.UseVisualStyleBackColor = true;
            this.exportTexturesChkBox.CheckedChanged += new System.EventHandler(this.exportTexturesChkBox_CheckedChanged);
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(254, 235);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 12;
            this.stButton1.Text = "Cancel";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(173, 235);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 13;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // chkFlipUvsVertical
            // 
            this.chkFlipUvsVertical.AutoSize = true;
            this.chkFlipUvsVertical.Location = new System.Drawing.Point(20, 147);
            this.chkFlipUvsVertical.Name = "chkFlipUvsVertical";
            this.chkFlipUvsVertical.Size = new System.Drawing.Size(101, 17);
            this.chkFlipUvsVertical.TabIndex = 14;
            this.chkFlipUvsVertical.Text = "Flp UVs Vertical";
            this.chkFlipUvsVertical.UseVisualStyleBackColor = true;
            this.chkFlipUvsVertical.CheckedChanged += new System.EventHandler(this.chkFlipUvsVertical_CheckedChanged);
            // 
            // chkOldExporter
            // 
            this.chkOldExporter.AutoSize = true;
            this.chkOldExporter.Location = new System.Drawing.Point(20, 170);
            this.chkOldExporter.Name = "chkOldExporter";
            this.chkOldExporter.Size = new System.Drawing.Size(200, 17);
            this.chkOldExporter.TabIndex = 15;
            this.chkOldExporter.Text = "Use Old Exporter (If new one breaks)";
            this.chkOldExporter.UseVisualStyleBackColor = true;
            this.chkOldExporter.CheckedChanged += new System.EventHandler(this.stCheckBox1_CheckedChanged);
            // 
            // chkVertexColors
            // 
            this.chkVertexColors.AutoSize = true;
            this.chkVertexColors.Checked = true;
            this.chkVertexColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVertexColors.Location = new System.Drawing.Point(23, 70);
            this.chkVertexColors.Name = "chkVertexColors";
            this.chkVertexColors.Size = new System.Drawing.Size(121, 17);
            this.chkVertexColors.TabIndex = 16;
            this.chkVertexColors.Text = "Export Vertex Colors";
            this.chkVertexColors.UseVisualStyleBackColor = true;
            this.chkVertexColors.CheckedChanged += new System.EventHandler(this.chkVertexColors_CheckedChanged);
            // 
            // chkExportRiggedBonesOnly
            // 
            this.chkExportRiggedBonesOnly.AutoSize = true;
            this.chkExportRiggedBonesOnly.Enabled = false;
            this.chkExportRiggedBonesOnly.Location = new System.Drawing.Point(23, 93);
            this.chkExportRiggedBonesOnly.Name = "chkExportRiggedBonesOnly";
            this.chkExportRiggedBonesOnly.Size = new System.Drawing.Size(150, 17);
            this.chkExportRiggedBonesOnly.TabIndex = 17;
            this.chkExportRiggedBonesOnly.Text = "Export Only Rigged Bones";
            this.chkExportRiggedBonesOnly.UseVisualStyleBackColor = true;
            this.chkExportRiggedBonesOnly.CheckedChanged += new System.EventHandler(this.chkExportRiggedBonesOnly_CheckedChanged);
            // 
            // chkApplyUVTransforms
            // 
            this.chkApplyUVTransforms.AutoSize = true;
            this.chkApplyUVTransforms.Location = new System.Drawing.Point(23, 116);
            this.chkApplyUVTransforms.Name = "chkApplyUVTransforms";
            this.chkApplyUVTransforms.Size = new System.Drawing.Size(187, 17);
            this.chkApplyUVTransforms.TabIndex = 18;
            this.chkApplyUVTransforms.Text = "Apply UV Transforms (diffuse only)";
            this.chkApplyUVTransforms.UseVisualStyleBackColor = true;
            this.chkApplyUVTransforms.CheckedChanged += new System.EventHandler(this.chkApplyUVTransforms_CheckedChanged);
            // 
            // chkTextureChannelComps
            // 
            this.chkTextureChannelComps.AutoSize = true;
            this.chkTextureChannelComps.Checked = true;
            this.chkTextureChannelComps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextureChannelComps.Location = new System.Drawing.Point(153, 47);
            this.chkTextureChannelComps.Name = "chkTextureChannelComps";
            this.chkTextureChannelComps.Size = new System.Drawing.Size(161, 17);
            this.chkTextureChannelComps.TabIndex = 19;
            this.chkTextureChannelComps.Text = "Use Texture Channel Swaps";
            this.chkTextureChannelComps.UseVisualStyleBackColor = true;
            this.chkTextureChannelComps.CheckedChanged += new System.EventHandler(this.chkTextureChannelComps_CheckedChanged);
            // 
            // ExportModelSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 272);
            this.Name = "ExportModelSettings";
            this.Text = "Export Settings";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STCheckBox exportTexturesChkBox;
        private STButton stButton2;
        private STButton stButton1;
        private STCheckBox chkFlipUvsVertical;
        protected STCheckBox chkOldExporter;
        private STCheckBox chkVertexColors;
        private STCheckBox chkExportRiggedBonesOnly;
        private STCheckBox chkApplyUVTransforms;
        private STCheckBox chkTextureChannelComps;
    }
}