namespace Switch_Toolbox
{
    partial class Settings
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
            this.label1 = new System.Windows.Forms.Label();
            this.chkBoxSpecular = new System.Windows.Forms.CheckBox();
            this.chkBoxNormalMap = new System.Windows.Forms.CheckBox();
            this.shadingComboBox = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.previewScaleUD = new System.Windows.Forms.NumericUpDown();
            this.chkBoxDisplayPolyCount = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.camFarNumUD = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.camNearNumUD = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.camMoveComboBox = new System.Windows.Forms.ComboBox();
            this.chkBoxDisplayBones = new System.Windows.Forms.CheckBox();
            this.chkBoxDisplayWireframe = new System.Windows.Forms.CheckBox();
            this.chkBoxDisplayModels = new System.Windows.Forms.CheckBox();
            this.chkBoxStereoscopy = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.disableViewportCHKBX = new System.Windows.Forms.CheckBox();
            this.GLSLVerLabel = new System.Windows.Forms.Label();
            this.openGLVerLabel = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.yazoCompressionLevelUD = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chkBoxEnablePBR = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewScaleUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.camFarNumUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.camNearNumUD)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yazoCompressionLevelUD)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(255, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Viewport Shading";
            // 
            // chkBoxSpecular
            // 
            this.chkBoxSpecular.AutoSize = true;
            this.chkBoxSpecular.ForeColor = System.Drawing.Color.White;
            this.chkBoxSpecular.Location = new System.Drawing.Point(258, 116);
            this.chkBoxSpecular.Name = "chkBoxSpecular";
            this.chkBoxSpecular.Size = new System.Drawing.Size(104, 17);
            this.chkBoxSpecular.TabIndex = 3;
            this.chkBoxSpecular.Text = "Enable Specular";
            this.chkBoxSpecular.UseVisualStyleBackColor = true;
            // 
            // chkBoxNormalMap
            // 
            this.chkBoxNormalMap.AutoSize = true;
            this.chkBoxNormalMap.ForeColor = System.Drawing.Color.White;
            this.chkBoxNormalMap.Location = new System.Drawing.Point(258, 93);
            this.chkBoxNormalMap.Name = "chkBoxNormalMap";
            this.chkBoxNormalMap.Size = new System.Drawing.Size(124, 17);
            this.chkBoxNormalMap.TabIndex = 2;
            this.chkBoxNormalMap.Text = "Enable Normal Maps";
            this.chkBoxNormalMap.UseVisualStyleBackColor = true;
            this.chkBoxNormalMap.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // shadingComboBox
            // 
            this.shadingComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.shadingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.shadingComboBox.ForeColor = System.Drawing.Color.White;
            this.shadingComboBox.FormattingEnabled = true;
            this.shadingComboBox.Location = new System.Drawing.Point(351, 52);
            this.shadingComboBox.Name = "shadingComboBox";
            this.shadingComboBox.Size = new System.Drawing.Size(165, 21);
            this.shadingComboBox.TabIndex = 1;
            this.shadingComboBox.SelectedIndexChanged += new System.EventHandler(this.shadingComboBox_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chkBoxEnablePBR);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.previewScaleUD);
            this.panel2.Controls.Add(this.chkBoxDisplayPolyCount);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.camFarNumUD);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.camNearNumUD);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.chkBoxSpecular);
            this.panel2.Controls.Add(this.camMoveComboBox);
            this.panel2.Controls.Add(this.chkBoxNormalMap);
            this.panel2.Controls.Add(this.chkBoxDisplayBones);
            this.panel2.Controls.Add(this.shadingComboBox);
            this.panel2.Controls.Add(this.chkBoxDisplayWireframe);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.chkBoxDisplayModels);
            this.panel2.Controls.Add(this.chkBoxStereoscopy);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(12, 156);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(534, 246);
            this.panel2.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(285, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Preview Scale";
            // 
            // previewScaleUD
            // 
            this.previewScaleUD.DecimalPlaces = 3;
            this.previewScaleUD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.previewScaleUD.Location = new System.Drawing.Point(363, 194);
            this.previewScaleUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.previewScaleUD.Name = "previewScaleUD";
            this.previewScaleUD.Size = new System.Drawing.Size(171, 20);
            this.previewScaleUD.TabIndex = 14;
            this.previewScaleUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.previewScaleUD.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // chkBoxDisplayPolyCount
            // 
            this.chkBoxDisplayPolyCount.AutoSize = true;
            this.chkBoxDisplayPolyCount.ForeColor = System.Drawing.Color.White;
            this.chkBoxDisplayPolyCount.Location = new System.Drawing.Point(120, 162);
            this.chkBoxDisplayPolyCount.Name = "chkBoxDisplayPolyCount";
            this.chkBoxDisplayPolyCount.Size = new System.Drawing.Size(114, 17);
            this.chkBoxDisplayPolyCount.TabIndex = 13;
            this.chkBoxDisplayPolyCount.Text = "Display Poly Count";
            this.chkBoxDisplayPolyCount.UseVisualStyleBackColor = true;
            this.chkBoxDisplayPolyCount.CheckedChanged += new System.EventHandler(this.chkBoxDisplayPolyCount_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(10, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Camera Far";
            // 
            // camFarNumUD
            // 
            this.camFarNumUD.DecimalPlaces = 3;
            this.camFarNumUD.Location = new System.Drawing.Point(110, 220);
            this.camFarNumUD.Maximum = new decimal(new int[] {
            1316134912,
            2328,
            0,
            0});
            this.camFarNumUD.Name = "camFarNumUD";
            this.camFarNumUD.Size = new System.Drawing.Size(171, 20);
            this.camFarNumUD.TabIndex = 11;
            this.camFarNumUD.ValueChanged += new System.EventHandler(this.camFarNumUD_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(10, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Camera Near";
            // 
            // camNearNumUD
            // 
            this.camNearNumUD.DecimalPlaces = 3;
            this.camNearNumUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.camNearNumUD.Location = new System.Drawing.Point(110, 194);
            this.camNearNumUD.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.camNearNumUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.camNearNumUD.Name = "camNearNumUD";
            this.camNearNumUD.Size = new System.Drawing.Size(171, 20);
            this.camNearNumUD.TabIndex = 9;
            this.camNearNumUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.camNearNumUD.ValueChanged += new System.EventHandler(this.camNearNumUD_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(3, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Camera";
            // 
            // camMoveComboBox
            // 
            this.camMoveComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.camMoveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.camMoveComboBox.ForeColor = System.Drawing.Color.White;
            this.camMoveComboBox.FormattingEnabled = true;
            this.camMoveComboBox.Location = new System.Drawing.Point(59, 52);
            this.camMoveComboBox.Name = "camMoveComboBox";
            this.camMoveComboBox.Size = new System.Drawing.Size(156, 21);
            this.camMoveComboBox.TabIndex = 4;
            this.camMoveComboBox.SelectedIndexChanged += new System.EventHandler(this.camMoveComboBox_SelectedIndexChanged);
            // 
            // chkBoxDisplayBones
            // 
            this.chkBoxDisplayBones.AutoSize = true;
            this.chkBoxDisplayBones.ForeColor = System.Drawing.Color.White;
            this.chkBoxDisplayBones.Location = new System.Drawing.Point(6, 162);
            this.chkBoxDisplayBones.Name = "chkBoxDisplayBones";
            this.chkBoxDisplayBones.Size = new System.Drawing.Size(93, 17);
            this.chkBoxDisplayBones.TabIndex = 7;
            this.chkBoxDisplayBones.Text = "Display Bones";
            this.chkBoxDisplayBones.UseVisualStyleBackColor = true;
            // 
            // chkBoxDisplayWireframe
            // 
            this.chkBoxDisplayWireframe.AutoSize = true;
            this.chkBoxDisplayWireframe.ForeColor = System.Drawing.Color.White;
            this.chkBoxDisplayWireframe.Location = new System.Drawing.Point(6, 139);
            this.chkBoxDisplayWireframe.Name = "chkBoxDisplayWireframe";
            this.chkBoxDisplayWireframe.Size = new System.Drawing.Size(111, 17);
            this.chkBoxDisplayWireframe.TabIndex = 6;
            this.chkBoxDisplayWireframe.Text = "Display Wireframe";
            this.chkBoxDisplayWireframe.UseVisualStyleBackColor = true;
            this.chkBoxDisplayWireframe.CheckedChanged += new System.EventHandler(this.chkBoxDisplayWireframe_CheckedChanged);
            // 
            // chkBoxDisplayModels
            // 
            this.chkBoxDisplayModels.AutoSize = true;
            this.chkBoxDisplayModels.ForeColor = System.Drawing.Color.White;
            this.chkBoxDisplayModels.Location = new System.Drawing.Point(6, 116);
            this.chkBoxDisplayModels.Name = "chkBoxDisplayModels";
            this.chkBoxDisplayModels.Size = new System.Drawing.Size(97, 17);
            this.chkBoxDisplayModels.TabIndex = 5;
            this.chkBoxDisplayModels.Text = "Display Models";
            this.chkBoxDisplayModels.UseVisualStyleBackColor = true;
            this.chkBoxDisplayModels.CheckedChanged += new System.EventHandler(this.chkBoxDisplayModels_CheckedChanged);
            // 
            // chkBoxStereoscopy
            // 
            this.chkBoxStereoscopy.AutoSize = true;
            this.chkBoxStereoscopy.ForeColor = System.Drawing.Color.White;
            this.chkBoxStereoscopy.Location = new System.Drawing.Point(6, 93);
            this.chkBoxStereoscopy.Name = "chkBoxStereoscopy";
            this.chkBoxStereoscopy.Size = new System.Drawing.Size(121, 17);
            this.chkBoxStereoscopy.TabIndex = 4;
            this.chkBoxStereoscopy.Text = "Enable Stereoscopy";
            this.chkBoxStereoscopy.UseVisualStyleBackColor = true;
            this.chkBoxStereoscopy.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(56, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Viewport Settings";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.disableViewportCHKBX);
            this.panel1.Controls.Add(this.GLSLVerLabel);
            this.panel1.Controls.Add(this.openGLVerLabel);
            this.panel1.Location = new System.Drawing.Point(12, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(215, 137);
            this.panel1.TabIndex = 5;
            // 
            // disableViewportCHKBX
            // 
            this.disableViewportCHKBX.AutoSize = true;
            this.disableViewportCHKBX.ForeColor = System.Drawing.Color.White;
            this.disableViewportCHKBX.Location = new System.Drawing.Point(0, 67);
            this.disableViewportCHKBX.Name = "disableViewportCHKBX";
            this.disableViewportCHKBX.Size = new System.Drawing.Size(105, 17);
            this.disableViewportCHKBX.TabIndex = 16;
            this.disableViewportCHKBX.Text = "Disable Viewport";
            this.disableViewportCHKBX.UseVisualStyleBackColor = true;
            this.disableViewportCHKBX.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_2);
            // 
            // GLSLVerLabel
            // 
            this.GLSLVerLabel.AutoSize = true;
            this.GLSLVerLabel.ForeColor = System.Drawing.Color.White;
            this.GLSLVerLabel.Location = new System.Drawing.Point(3, 38);
            this.GLSLVerLabel.Name = "GLSLVerLabel";
            this.GLSLVerLabel.Size = new System.Drawing.Size(72, 13);
            this.GLSLVerLabel.TabIndex = 10;
            this.GLSLVerLabel.Text = "GLSL Version";
            // 
            // openGLVerLabel
            // 
            this.openGLVerLabel.AutoSize = true;
            this.openGLVerLabel.ForeColor = System.Drawing.Color.White;
            this.openGLVerLabel.Location = new System.Drawing.Point(3, 13);
            this.openGLVerLabel.Name = "openGLVerLabel";
            this.openGLVerLabel.Size = new System.Drawing.Size(91, 13);
            this.openGLVerLabel.TabIndex = 9;
            this.openGLVerLabel.Text = "Open GL Version:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(397, 408);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(149, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(3, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Yaz0 Compression Level";
            // 
            // yazoCompressionLevelUD
            // 
            this.yazoCompressionLevelUD.Location = new System.Drawing.Point(130, 12);
            this.yazoCompressionLevelUD.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.yazoCompressionLevelUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.yazoCompressionLevelUD.Name = "yazoCompressionLevelUD";
            this.yazoCompressionLevelUD.Size = new System.Drawing.Size(42, 20);
            this.yazoCompressionLevelUD.TabIndex = 16;
            this.yazoCompressionLevelUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.yazoCompressionLevelUD.ValueChanged += new System.EventHandler(this.yazoCompressionLevelUD_ValueChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.yazoCompressionLevelUD);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Location = new System.Drawing.Point(233, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(313, 138);
            this.panel3.TabIndex = 18;
            // 
            // chkBoxEnablePBR
            // 
            this.chkBoxEnablePBR.AutoSize = true;
            this.chkBoxEnablePBR.ForeColor = System.Drawing.Color.White;
            this.chkBoxEnablePBR.Location = new System.Drawing.Point(258, 139);
            this.chkBoxEnablePBR.Name = "chkBoxEnablePBR";
            this.chkBoxEnablePBR.Size = new System.Drawing.Size(84, 17);
            this.chkBoxEnablePBR.TabIndex = 17;
            this.chkBoxEnablePBR.Text = "Enable PBR";
            this.chkBoxEnablePBR.UseVisualStyleBackColor = true;
            this.chkBoxEnablePBR.CheckedChanged += new System.EventHandler(this.chkBoxEnablePBR_CheckedChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(551, 443);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewScaleUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.camFarNumUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.camNearNumUD)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yazoCompressionLevelUD)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkBoxSpecular;
        private System.Windows.Forms.CheckBox chkBoxNormalMap;
        private System.Windows.Forms.ComboBox shadingComboBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chkBoxStereoscopy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkBoxDisplayBones;
        private System.Windows.Forms.CheckBox chkBoxDisplayWireframe;
        private System.Windows.Forms.CheckBox chkBoxDisplayModels;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox camMoveComboBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label openGLVerLabel;
        private System.Windows.Forms.Label GLSLVerLabel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown camFarNumUD;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown camNearNumUD;
        private System.Windows.Forms.CheckBox chkBoxDisplayPolyCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown previewScaleUD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown yazoCompressionLevelUD;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox disableViewportCHKBX;
        private System.Windows.Forms.CheckBox chkBoxEnablePBR;
    }
}