namespace Switch_Toolbox.Library.Forms
{
    partial class Assimp_Settings
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
            this.generateNormalsChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.smoothNormalsChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.generateTansBitansChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.flipUVsChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.limtBoneWeightChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.preTransformVerticesChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.triangulateChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.joinDupedVertsSk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.leftHandedChk = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.useNodeTransform = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            this.rotateBones = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.rotateBonesUD = new System.Windows.Forms.NumericUpDown();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateBonesUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.rotateBonesUD);
            this.contentContainer.Controls.Add(this.rotateBones);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.useNodeTransform);
            this.contentContainer.Controls.Add(this.leftHandedChk);
            this.contentContainer.Controls.Add(this.joinDupedVertsSk);
            this.contentContainer.Controls.Add(this.triangulateChk);
            this.contentContainer.Controls.Add(this.preTransformVerticesChk);
            this.contentContainer.Controls.Add(this.numericUpDown1);
            this.contentContainer.Controls.Add(this.limtBoneWeightChk);
            this.contentContainer.Controls.Add(this.flipUVsChk);
            this.contentContainer.Controls.Add(this.generateTansBitansChk);
            this.contentContainer.Controls.Add(this.smoothNormalsChk);
            this.contentContainer.Controls.Add(this.generateNormalsChk);
            this.contentContainer.Size = new System.Drawing.Size(290, 336);
            this.contentContainer.Controls.SetChildIndex(this.generateNormalsChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.smoothNormalsChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.generateTansBitansChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.flipUVsChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.limtBoneWeightChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.numericUpDown1, 0);
            this.contentContainer.Controls.SetChildIndex(this.preTransformVerticesChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.triangulateChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.joinDupedVertsSk, 0);
            this.contentContainer.Controls.SetChildIndex(this.leftHandedChk, 0);
            this.contentContainer.Controls.SetChildIndex(this.useNodeTransform, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.rotateBones, 0);
            this.contentContainer.Controls.SetChildIndex(this.rotateBonesUD, 0);
            // 
            // generateNormalsChk
            // 
            this.generateNormalsChk.AutoSize = true;
            this.generateNormalsChk.Checked = true;
            this.generateNormalsChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.generateNormalsChk.ForeColor = System.Drawing.Color.White;
            this.generateNormalsChk.Location = new System.Drawing.Point(9, 88);
            this.generateNormalsChk.Name = "generateNormalsChk";
            this.generateNormalsChk.Size = new System.Drawing.Size(196, 17);
            this.generateNormalsChk.TabIndex = 0;
            this.generateNormalsChk.Text = "Generate Normals (if none are used)";
            this.generateNormalsChk.UseVisualStyleBackColor = true;
            // 
            // smoothNormalsChk
            // 
            this.smoothNormalsChk.AutoSize = true;
            this.smoothNormalsChk.ForeColor = System.Drawing.Color.White;
            this.smoothNormalsChk.Location = new System.Drawing.Point(9, 111);
            this.smoothNormalsChk.Name = "smoothNormalsChk";
            this.smoothNormalsChk.Size = new System.Drawing.Size(103, 17);
            this.smoothNormalsChk.TabIndex = 1;
            this.smoothNormalsChk.Text = "Smooth Normals";
            this.smoothNormalsChk.UseVisualStyleBackColor = true;
            // 
            // generateTansBitansChk
            // 
            this.generateTansBitansChk.AutoSize = true;
            this.generateTansBitansChk.Checked = true;
            this.generateTansBitansChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.generateTansBitansChk.ForeColor = System.Drawing.Color.White;
            this.generateTansBitansChk.Location = new System.Drawing.Point(9, 134);
            this.generateTansBitansChk.Name = "generateTansBitansChk";
            this.generateTansBitansChk.Size = new System.Drawing.Size(173, 17);
            this.generateTansBitansChk.TabIndex = 2;
            this.generateTansBitansChk.Text = "Generate Tangents/Bitangents";
            this.generateTansBitansChk.UseVisualStyleBackColor = true;
            // 
            // flipUVsChk
            // 
            this.flipUVsChk.AutoSize = true;
            this.flipUVsChk.Checked = true;
            this.flipUVsChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.flipUVsChk.ForeColor = System.Drawing.Color.White;
            this.flipUVsChk.Location = new System.Drawing.Point(9, 157);
            this.flipUVsChk.Name = "flipUVsChk";
            this.flipUVsChk.Size = new System.Drawing.Size(65, 17);
            this.flipUVsChk.TabIndex = 3;
            this.flipUVsChk.Text = "Flip UVs";
            this.flipUVsChk.UseVisualStyleBackColor = true;
            // 
            // limtBoneWeightChk
            // 
            this.limtBoneWeightChk.AutoSize = true;
            this.limtBoneWeightChk.Checked = true;
            this.limtBoneWeightChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.limtBoneWeightChk.ForeColor = System.Drawing.Color.White;
            this.limtBoneWeightChk.Location = new System.Drawing.Point(9, 180);
            this.limtBoneWeightChk.Name = "limtBoneWeightChk";
            this.limtBoneWeightChk.Size = new System.Drawing.Size(113, 17);
            this.limtBoneWeightChk.TabIndex = 5;
            this.limtBoneWeightChk.Text = "Limit bone weights";
            this.limtBoneWeightChk.UseVisualStyleBackColor = true;
            this.limtBoneWeightChk.CheckedChanged += new System.EventHandler(this.limtBoneWeightChk_CheckedChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.ForeColor = System.Drawing.Color.White;
            this.numericUpDown1.Location = new System.Drawing.Point(132, 179);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 16);
            this.numericUpDown1.TabIndex = 6;
            this.numericUpDown1.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // preTransformVerticesChk
            // 
            this.preTransformVerticesChk.AutoSize = true;
            this.preTransformVerticesChk.ForeColor = System.Drawing.Color.White;
            this.preTransformVerticesChk.Location = new System.Drawing.Point(9, 203);
            this.preTransformVerticesChk.Name = "preTransformVerticesChk";
            this.preTransformVerticesChk.Size = new System.Drawing.Size(130, 17);
            this.preTransformVerticesChk.TabIndex = 7;
            this.preTransformVerticesChk.Text = "PreTransform Vertices";
            this.preTransformVerticesChk.UseVisualStyleBackColor = true;
            // 
            // triangulateChk
            // 
            this.triangulateChk.AutoSize = true;
            this.triangulateChk.Checked = true;
            this.triangulateChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.triangulateChk.ForeColor = System.Drawing.Color.White;
            this.triangulateChk.Location = new System.Drawing.Point(9, 226);
            this.triangulateChk.Name = "triangulateChk";
            this.triangulateChk.Size = new System.Drawing.Size(79, 17);
            this.triangulateChk.TabIndex = 8;
            this.triangulateChk.Text = "Triangulate";
            this.triangulateChk.UseVisualStyleBackColor = true;
            // 
            // joinDupedVertsSk
            // 
            this.joinDupedVertsSk.AutoSize = true;
            this.joinDupedVertsSk.Checked = true;
            this.joinDupedVertsSk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.joinDupedVertsSk.ForeColor = System.Drawing.Color.White;
            this.joinDupedVertsSk.Location = new System.Drawing.Point(9, 249);
            this.joinDupedVertsSk.Name = "joinDupedVertsSk";
            this.joinDupedVertsSk.Size = new System.Drawing.Size(121, 17);
            this.joinDupedVertsSk.TabIndex = 9;
            this.joinDupedVertsSk.Text = "Join Duped Vertices";
            this.joinDupedVertsSk.UseVisualStyleBackColor = true;
            // 
            // leftHandedChk
            // 
            this.leftHandedChk.AutoSize = true;
            this.leftHandedChk.ForeColor = System.Drawing.Color.White;
            this.leftHandedChk.Location = new System.Drawing.Point(9, 272);
            this.leftHandedChk.Name = "leftHandedChk";
            this.leftHandedChk.Size = new System.Drawing.Size(109, 17);
            this.leftHandedChk.TabIndex = 10;
            this.leftHandedChk.Text = "Make left handed";
            this.leftHandedChk.UseVisualStyleBackColor = true;
            // 
            // useNodeTransform
            // 
            this.useNodeTransform.AutoSize = true;
            this.useNodeTransform.Checked = true;
            this.useNodeTransform.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useNodeTransform.ForeColor = System.Drawing.Color.White;
            this.useNodeTransform.Location = new System.Drawing.Point(9, 60);
            this.useNodeTransform.Name = "useNodeTransform";
            this.useNodeTransform.Size = new System.Drawing.Size(124, 17);
            this.useNodeTransform.TabIndex = 11;
            this.useNodeTransform.Text = "Use Node Transform";
            this.useNodeTransform.UseVisualStyleBackColor = true;
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(211, 304);
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
            this.stButton2.Location = new System.Drawing.Point(130, 304);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 13;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // rotateBones
            // 
            this.rotateBones.AutoSize = true;
            this.rotateBones.ForeColor = System.Drawing.Color.White;
            this.rotateBones.Location = new System.Drawing.Point(9, 31);
            this.rotateBones.Name = "rotateBones";
            this.rotateBones.Size = new System.Drawing.Size(105, 17);
            this.rotateBones.TabIndex = 14;
            this.rotateBones.Text = "Rotate Bones by";
            this.rotateBones.UseVisualStyleBackColor = true;
            this.rotateBones.CheckedChanged += new System.EventHandler(this.rotateBonesY90_CheckedChanged);
            // 
            // rotateBonesUD
            // 
            this.rotateBonesUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rotateBonesUD.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rotateBonesUD.Enabled = false;
            this.rotateBonesUD.ForeColor = System.Drawing.Color.White;
            this.rotateBonesUD.Location = new System.Drawing.Point(120, 33);
            this.rotateBonesUD.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.rotateBonesUD.Name = "rotateBonesUD";
            this.rotateBonesUD.Size = new System.Drawing.Size(120, 16);
            this.rotateBonesUD.TabIndex = 15;
            this.rotateBonesUD.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.rotateBonesUD.ValueChanged += new System.EventHandler(this.rotateBonesUD_ValueChanged);
            // 
            // Assimp_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(297, 341);
            this.Name = "Assimp_Settings";
            this.Text = "Import Settings";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateBonesUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STCheckBox generateNormalsChk;
        private Switch_Toolbox.Library.Forms.STCheckBox smoothNormalsChk;
        private Switch_Toolbox.Library.Forms.STCheckBox generateTansBitansChk;
        private Switch_Toolbox.Library.Forms.STCheckBox flipUVsChk;
        private Switch_Toolbox.Library.Forms.STCheckBox limtBoneWeightChk;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private Switch_Toolbox.Library.Forms.STCheckBox preTransformVerticesChk;
        private Switch_Toolbox.Library.Forms.STCheckBox triangulateChk;
        private Switch_Toolbox.Library.Forms.STCheckBox joinDupedVertsSk;
        private Switch_Toolbox.Library.Forms.STCheckBox leftHandedChk;
        private STCheckBox useNodeTransform;
        private STButton stButton2;
        private STButton stButton1;
        private STCheckBox rotateBones;
        private System.Windows.Forms.NumericUpDown rotateBonesUD;
    }
}
