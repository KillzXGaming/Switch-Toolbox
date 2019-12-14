namespace FirstPlugin.Forms
{
    partial class GFLXModelImporter
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
            this.presetCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.chkUseNormals = new Toolbox.Library.Forms.STCheckBox();
            this.chkHasUv1 = new Toolbox.Library.Forms.STCheckBox();
            this.chkUseBoneIndex = new Toolbox.Library.Forms.STCheckBox();
            this.chkUseColor1 = new Toolbox.Library.Forms.STCheckBox();
            this.normalFormatCB = new Toolbox.Library.Forms.STComboBox();
            this.uv0FormatCB = new Toolbox.Library.Forms.STComboBox();
            this.boneFormatCB = new Toolbox.Library.Forms.STComboBox();
            this.color0FormatCB = new Toolbox.Library.Forms.STComboBox();
            this.uv1FormatCB = new Toolbox.Library.Forms.STComboBox();
            this.chkHasUv2 = new Toolbox.Library.Forms.STCheckBox();
            this.color1FormatCB = new Toolbox.Library.Forms.STComboBox();
            this.chkUseColor2 = new Toolbox.Library.Forms.STCheckBox();
            this.positionFormatCB = new Toolbox.Library.Forms.STComboBox();
            this.stCheckBox8 = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.materiialPresetCB = new Toolbox.Library.Forms.STComboBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.weightFormatCB = new Toolbox.Library.Forms.STComboBox();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.chkSetNormalsToColorChannel = new Toolbox.Library.Forms.STCheckBox();
            this.tangentFormatCB = new Toolbox.Library.Forms.STComboBox();
            this.chkTangents = new Toolbox.Library.Forms.STCheckBox();
            this.chkUseBoneWeights = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.rotateModel90YUD = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.chkMatchAttributes = new Toolbox.Library.Forms.STCheckBox();
            this.chkUseOriginalBones = new Toolbox.Library.Forms.STCheckBox();
            this.stCheckBox7 = new Toolbox.Library.Forms.STCheckBox();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.chkBitangents = new Toolbox.Library.Forms.STCheckBox();
            this.bitangentFormatCB = new Toolbox.Library.Forms.STComboBox();
            this.contentContainer.SuspendLayout();
            this.stPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rotateModel90YUD)).BeginInit();
            this.stPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stPanel2);
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.contentContainer.Size = new System.Drawing.Size(605, 620);
            this.contentContainer.Controls.SetChildIndex(this.listViewCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stPanel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            // 
            // presetCB
            // 
            this.presetCB.BorderColor = System.Drawing.Color.Empty;
            this.presetCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.presetCB.ButtonColor = System.Drawing.Color.Empty;
            this.presetCB.FormattingEnabled = true;
            this.presetCB.IsReadOnly = false;
            this.presetCB.Location = new System.Drawing.Point(153, 42);
            this.presetCB.Name = "presetCB";
            this.presetCB.Size = new System.Drawing.Size(120, 21);
            this.presetCB.TabIndex = 11;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(13, 45);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(40, 13);
            this.stLabel1.TabIndex = 12;
            this.stLabel1.Text = "Preset:";
            // 
            // chkUseNormals
            // 
            this.chkUseNormals.AutoSize = true;
            this.chkUseNormals.Location = new System.Drawing.Point(12, 104);
            this.chkUseNormals.Name = "chkUseNormals";
            this.chkUseNormals.Size = new System.Drawing.Size(86, 17);
            this.chkUseNormals.TabIndex = 14;
            this.chkUseNormals.Text = "Use Normals";
            this.chkUseNormals.UseVisualStyleBackColor = true;
            this.chkUseNormals.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // chkHasUv1
            // 
            this.chkHasUv1.AutoSize = true;
            this.chkHasUv1.Location = new System.Drawing.Point(11, 197);
            this.chkHasUv1.Name = "chkHasUv1";
            this.chkHasUv1.Size = new System.Drawing.Size(114, 17);
            this.chkHasUv1.TabIndex = 15;
            this.chkHasUv1.Text = "Use UV Channel 1";
            this.chkHasUv1.UseVisualStyleBackColor = true;
            this.chkHasUv1.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // chkUseBoneIndex
            // 
            this.chkUseBoneIndex.AutoSize = true;
            this.chkUseBoneIndex.Location = new System.Drawing.Point(11, 337);
            this.chkUseBoneIndex.Name = "chkUseBoneIndex";
            this.chkUseBoneIndex.Size = new System.Drawing.Size(110, 17);
            this.chkUseBoneIndex.TabIndex = 16;
            this.chkUseBoneIndex.Text = "Use Bone Indices";
            this.chkUseBoneIndex.UseVisualStyleBackColor = true;
            this.chkUseBoneIndex.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // chkUseColor1
            // 
            this.chkUseColor1.AutoSize = true;
            this.chkUseColor1.Location = new System.Drawing.Point(11, 253);
            this.chkUseColor1.Name = "chkUseColor1";
            this.chkUseColor1.Size = new System.Drawing.Size(123, 17);
            this.chkUseColor1.TabIndex = 17;
            this.chkUseColor1.Text = "Use Color Channel 1";
            this.chkUseColor1.UseVisualStyleBackColor = true;
            this.chkUseColor1.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // normalFormatCB
            // 
            this.normalFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.normalFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.normalFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.normalFormatCB.FormattingEnabled = true;
            this.normalFormatCB.IsReadOnly = false;
            this.normalFormatCB.Location = new System.Drawing.Point(147, 102);
            this.normalFormatCB.Name = "normalFormatCB";
            this.normalFormatCB.Size = new System.Drawing.Size(131, 21);
            this.normalFormatCB.TabIndex = 18;
            this.normalFormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // uv0FormatCB
            // 
            this.uv0FormatCB.BorderColor = System.Drawing.Color.Empty;
            this.uv0FormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.uv0FormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.uv0FormatCB.FormattingEnabled = true;
            this.uv0FormatCB.IsReadOnly = false;
            this.uv0FormatCB.Location = new System.Drawing.Point(146, 193);
            this.uv0FormatCB.Name = "uv0FormatCB";
            this.uv0FormatCB.Size = new System.Drawing.Size(131, 21);
            this.uv0FormatCB.TabIndex = 19;
            this.uv0FormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // boneFormatCB
            // 
            this.boneFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.boneFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.boneFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.boneFormatCB.FormattingEnabled = true;
            this.boneFormatCB.IsReadOnly = false;
            this.boneFormatCB.Location = new System.Drawing.Point(142, 333);
            this.boneFormatCB.Name = "boneFormatCB";
            this.boneFormatCB.Size = new System.Drawing.Size(133, 21);
            this.boneFormatCB.TabIndex = 21;
            this.boneFormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // color0FormatCB
            // 
            this.color0FormatCB.BorderColor = System.Drawing.Color.Empty;
            this.color0FormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.color0FormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.color0FormatCB.FormattingEnabled = true;
            this.color0FormatCB.IsReadOnly = false;
            this.color0FormatCB.Location = new System.Drawing.Point(144, 251);
            this.color0FormatCB.Name = "color0FormatCB";
            this.color0FormatCB.Size = new System.Drawing.Size(131, 21);
            this.color0FormatCB.TabIndex = 20;
            this.color0FormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // uv1FormatCB
            // 
            this.uv1FormatCB.BorderColor = System.Drawing.Color.Empty;
            this.uv1FormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.uv1FormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.uv1FormatCB.FormattingEnabled = true;
            this.uv1FormatCB.IsReadOnly = false;
            this.uv1FormatCB.Location = new System.Drawing.Point(146, 220);
            this.uv1FormatCB.Name = "uv1FormatCB";
            this.uv1FormatCB.Size = new System.Drawing.Size(131, 21);
            this.uv1FormatCB.TabIndex = 23;
            this.uv1FormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // chkHasUv2
            // 
            this.chkHasUv2.AutoSize = true;
            this.chkHasUv2.Location = new System.Drawing.Point(11, 224);
            this.chkHasUv2.Name = "chkHasUv2";
            this.chkHasUv2.Size = new System.Drawing.Size(114, 17);
            this.chkHasUv2.TabIndex = 22;
            this.chkHasUv2.Text = "Use UV Channel 2";
            this.chkHasUv2.UseVisualStyleBackColor = true;
            this.chkHasUv2.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // color1FormatCB
            // 
            this.color1FormatCB.BorderColor = System.Drawing.Color.Empty;
            this.color1FormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.color1FormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.color1FormatCB.FormattingEnabled = true;
            this.color1FormatCB.IsReadOnly = false;
            this.color1FormatCB.Location = new System.Drawing.Point(144, 278);
            this.color1FormatCB.Name = "color1FormatCB";
            this.color1FormatCB.Size = new System.Drawing.Size(131, 21);
            this.color1FormatCB.TabIndex = 25;
            this.color1FormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // chkUseColor2
            // 
            this.chkUseColor2.AutoSize = true;
            this.chkUseColor2.Location = new System.Drawing.Point(11, 280);
            this.chkUseColor2.Name = "chkUseColor2";
            this.chkUseColor2.Size = new System.Drawing.Size(123, 17);
            this.chkUseColor2.TabIndex = 24;
            this.chkUseColor2.Text = "Use Color Channel 2";
            this.chkUseColor2.UseVisualStyleBackColor = true;
            this.chkUseColor2.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // positionFormatCB
            // 
            this.positionFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.positionFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.positionFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.positionFormatCB.FormattingEnabled = true;
            this.positionFormatCB.IsReadOnly = false;
            this.positionFormatCB.Location = new System.Drawing.Point(147, 75);
            this.positionFormatCB.Name = "positionFormatCB";
            this.positionFormatCB.Size = new System.Drawing.Size(131, 21);
            this.positionFormatCB.TabIndex = 29;
            this.positionFormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // stCheckBox8
            // 
            this.stCheckBox8.AutoSize = true;
            this.stCheckBox8.Checked = true;
            this.stCheckBox8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stCheckBox8.Enabled = false;
            this.stCheckBox8.Location = new System.Drawing.Point(12, 77);
            this.stCheckBox8.Name = "stCheckBox8";
            this.stCheckBox8.Size = new System.Drawing.Size(63, 17);
            this.stCheckBox8.TabIndex = 28;
            this.stCheckBox8.Text = "Position";
            this.stCheckBox8.UseVisualStyleBackColor = true;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(9, 39);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(47, 13);
            this.stLabel2.TabIndex = 31;
            this.stLabel2.Text = "Material:";
            // 
            // materiialPresetCB
            // 
            this.materiialPresetCB.BorderColor = System.Drawing.Color.Empty;
            this.materiialPresetCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.materiialPresetCB.ButtonColor = System.Drawing.Color.Empty;
            this.materiialPresetCB.FormattingEnabled = true;
            this.materiialPresetCB.IsReadOnly = false;
            this.materiialPresetCB.Location = new System.Drawing.Point(145, 36);
            this.materiialPresetCB.Name = "materiialPresetCB";
            this.materiialPresetCB.Size = new System.Drawing.Size(131, 21);
            this.materiialPresetCB.TabIndex = 32;
            this.materiialPresetCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(282, 34);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(31, 23);
            this.stButton1.TabIndex = 33;
            this.stButton1.Text = "+";
            this.stButton1.UseVisualStyleBackColor = false;
            this.stButton1.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom1.HideSelection = false;
            this.listViewCustom1.Location = new System.Drawing.Point(5, 31);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(208, 499);
            this.listViewCustom1.TabIndex = 34;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 202;
            // 
            // weightFormatCB
            // 
            this.weightFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.weightFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.weightFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.weightFormatCB.FormattingEnabled = true;
            this.weightFormatCB.IsReadOnly = false;
            this.weightFormatCB.Location = new System.Drawing.Point(142, 360);
            this.weightFormatCB.Name = "weightFormatCB";
            this.weightFormatCB.Size = new System.Drawing.Size(135, 21);
            this.weightFormatCB.TabIndex = 35;
            this.weightFormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.chkSetNormalsToColorChannel);
            this.stPanel1.Controls.Add(this.tangentFormatCB);
            this.stPanel1.Controls.Add(this.chkTangents);
            this.stPanel1.Controls.Add(this.chkUseBoneWeights);
            this.stPanel1.Controls.Add(this.stLabel2);
            this.stPanel1.Controls.Add(this.stLabel3);
            this.stPanel1.Controls.Add(this.weightFormatCB);
            this.stPanel1.Controls.Add(this.chkUseNormals);
            this.stPanel1.Controls.Add(this.chkHasUv1);
            this.stPanel1.Controls.Add(this.stButton1);
            this.stPanel1.Controls.Add(this.chkUseBoneIndex);
            this.stPanel1.Controls.Add(this.materiialPresetCB);
            this.stPanel1.Controls.Add(this.chkUseColor1);
            this.stPanel1.Controls.Add(this.normalFormatCB);
            this.stPanel1.Controls.Add(this.positionFormatCB);
            this.stPanel1.Controls.Add(this.uv0FormatCB);
            this.stPanel1.Controls.Add(this.stCheckBox8);
            this.stPanel1.Controls.Add(this.color0FormatCB);
            this.stPanel1.Controls.Add(this.bitangentFormatCB);
            this.stPanel1.Controls.Add(this.boneFormatCB);
            this.stPanel1.Controls.Add(this.chkBitangents);
            this.stPanel1.Controls.Add(this.chkHasUv2);
            this.stPanel1.Controls.Add(this.color1FormatCB);
            this.stPanel1.Controls.Add(this.uv1FormatCB);
            this.stPanel1.Controls.Add(this.chkUseColor2);
            this.stPanel1.Location = new System.Drawing.Point(219, 206);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(378, 382);
            this.stPanel1.TabIndex = 36;
            // 
            // chkSetNormalsToColorChannel
            // 
            this.chkSetNormalsToColorChannel.AutoSize = true;
            this.chkSetNormalsToColorChannel.Location = new System.Drawing.Point(11, 305);
            this.chkSetNormalsToColorChannel.Name = "chkSetNormalsToColorChannel";
            this.chkSetNormalsToColorChannel.Size = new System.Drawing.Size(176, 17);
            this.chkSetNormalsToColorChannel.TabIndex = 41;
            this.chkSetNormalsToColorChannel.Text = "Set Normals to Color Channel  2";
            this.chkSetNormalsToColorChannel.UseVisualStyleBackColor = true;
            this.chkSetNormalsToColorChannel.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // tangentFormatCB
            // 
            this.tangentFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.tangentFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.tangentFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.tangentFormatCB.FormattingEnabled = true;
            this.tangentFormatCB.IsReadOnly = false;
            this.tangentFormatCB.Location = new System.Drawing.Point(147, 131);
            this.tangentFormatCB.Name = "tangentFormatCB";
            this.tangentFormatCB.Size = new System.Drawing.Size(131, 21);
            this.tangentFormatCB.TabIndex = 40;
            this.tangentFormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // chkTangents
            // 
            this.chkTangents.AutoSize = true;
            this.chkTangents.Location = new System.Drawing.Point(12, 133);
            this.chkTangents.Name = "chkTangents";
            this.chkTangents.Size = new System.Drawing.Size(93, 17);
            this.chkTangents.TabIndex = 39;
            this.chkTangents.Text = "Use Tangents";
            this.chkTangents.UseVisualStyleBackColor = true;
            this.chkTangents.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // chkUseBoneWeights
            // 
            this.chkUseBoneWeights.AutoSize = true;
            this.chkUseBoneWeights.Location = new System.Drawing.Point(11, 364);
            this.chkUseBoneWeights.Name = "chkUseBoneWeights";
            this.chkUseBoneWeights.Size = new System.Drawing.Size(115, 17);
            this.chkUseBoneWeights.TabIndex = 38;
            this.chkUseBoneWeights.Text = "Use Bone Weights";
            this.chkUseBoneWeights.UseVisualStyleBackColor = true;
            this.chkUseBoneWeights.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(140, 10);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(77, 13);
            this.stLabel3.TabIndex = 37;
            this.stLabel3.Text = "Mesh Settings:";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(137, 11);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(81, 13);
            this.stLabel4.TabIndex = 38;
            this.stLabel4.Text = "Global Settings:";
            // 
            // rotateModel90YUD
            // 
            this.rotateModel90YUD.DecimalPlaces = 5;
            this.rotateModel90YUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.rotateModel90YUD.Location = new System.Drawing.Point(153, 75);
            this.rotateModel90YUD.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.rotateModel90YUD.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.rotateModel90YUD.Name = "rotateModel90YUD";
            this.rotateModel90YUD.Size = new System.Drawing.Size(120, 20);
            this.rotateModel90YUD.TabIndex = 39;
            this.rotateModel90YUD.ValueChanged += new System.EventHandler(this.numericUpDownFloat1_ValueChanged);
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(13, 77);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(52, 13);
            this.stLabel5.TabIndex = 40;
            this.stLabel5.Text = "Rotate Y:";
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.chkMatchAttributes);
            this.stPanel2.Controls.Add(this.chkUseOriginalBones);
            this.stPanel2.Controls.Add(this.stCheckBox7);
            this.stPanel2.Controls.Add(this.stLabel1);
            this.stPanel2.Controls.Add(this.presetCB);
            this.stPanel2.Controls.Add(this.stLabel5);
            this.stPanel2.Controls.Add(this.stLabel4);
            this.stPanel2.Controls.Add(this.rotateModel90YUD);
            this.stPanel2.Location = new System.Drawing.Point(222, 31);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(375, 169);
            this.stPanel2.TabIndex = 41;
            // 
            // chkMatchAttributes
            // 
            this.chkMatchAttributes.AutoSize = true;
            this.chkMatchAttributes.Location = new System.Drawing.Point(9, 111);
            this.chkMatchAttributes.Name = "chkMatchAttributes";
            this.chkMatchAttributes.Size = new System.Drawing.Size(141, 17);
            this.chkMatchAttributes.TabIndex = 44;
            this.chkMatchAttributes.Text = "Match Original Attributes";
            this.chkMatchAttributes.UseVisualStyleBackColor = true;
            this.chkMatchAttributes.CheckedChanged += new System.EventHandler(this.chkMatchAttributes_CheckedChanged);
            // 
            // chkUseOriginalBones
            // 
            this.chkUseOriginalBones.AutoSize = true;
            this.chkUseOriginalBones.Checked = true;
            this.chkUseOriginalBones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseOriginalBones.Location = new System.Drawing.Point(157, 134);
            this.chkUseOriginalBones.Name = "chkUseOriginalBones";
            this.chkUseOriginalBones.Size = new System.Drawing.Size(116, 17);
            this.chkUseOriginalBones.TabIndex = 43;
            this.chkUseOriginalBones.Text = "Use Original Bones";
            this.chkUseOriginalBones.UseVisualStyleBackColor = true;
            // 
            // stCheckBox7
            // 
            this.stCheckBox7.AutoSize = true;
            this.stCheckBox7.Location = new System.Drawing.Point(157, 111);
            this.stCheckBox7.Name = "stCheckBox7";
            this.stCheckBox7.Size = new System.Drawing.Size(103, 17);
            this.stCheckBox7.TabIndex = 41;
            this.stCheckBox7.Text = "Flip UVs Vertical";
            this.stCheckBox7.UseVisualStyleBackColor = true;
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(522, 594);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 42;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // chkBitangents
            // 
            this.chkBitangents.AutoSize = true;
            this.chkBitangents.Location = new System.Drawing.Point(12, 164);
            this.chkBitangents.Name = "chkBitangents";
            this.chkBitangents.Size = new System.Drawing.Size(98, 17);
            this.chkBitangents.TabIndex = 26;
            this.chkBitangents.Text = "Use Bitangents";
            this.chkBitangents.UseVisualStyleBackColor = true;
            this.chkBitangents.CheckedChanged += new System.EventHandler(this.ApplySettings);
            // 
            // bitangentFormatCB
            // 
            this.bitangentFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.bitangentFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.bitangentFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.bitangentFormatCB.FormattingEnabled = true;
            this.bitangentFormatCB.IsReadOnly = false;
            this.bitangentFormatCB.Location = new System.Drawing.Point(147, 162);
            this.bitangentFormatCB.Name = "bitangentFormatCB";
            this.bitangentFormatCB.Size = new System.Drawing.Size(131, 21);
            this.bitangentFormatCB.TabIndex = 27;
            this.bitangentFormatCB.SelectedIndexChanged += new System.EventHandler(this.ApplySettings);
            // 
            // GFLXModelImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 624);
            this.Name = "GFLXModelImporter";
            this.Text = "GFBMDL Importer";
            this.contentContainer.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rotateModel90YUD)).EndInit();
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STComboBox presetCB;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STComboBox boneFormatCB;
        private Toolbox.Library.Forms.STComboBox color0FormatCB;
        private Toolbox.Library.Forms.STComboBox uv0FormatCB;
        private Toolbox.Library.Forms.STComboBox normalFormatCB;
        private Toolbox.Library.Forms.STCheckBox chkUseColor1;
        private Toolbox.Library.Forms.STCheckBox chkUseBoneIndex;
        private Toolbox.Library.Forms.STCheckBox chkHasUv1;
        private Toolbox.Library.Forms.STCheckBox chkUseNormals;
        private Toolbox.Library.Forms.STComboBox color1FormatCB;
        private Toolbox.Library.Forms.STCheckBox chkUseColor2;
        private Toolbox.Library.Forms.STComboBox uv1FormatCB;
        private Toolbox.Library.Forms.STCheckBox chkHasUv2;
        private Toolbox.Library.Forms.STComboBox positionFormatCB;
        private Toolbox.Library.Forms.STCheckBox stCheckBox8;
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STComboBox materiialPresetCB;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Toolbox.Library.Forms.STComboBox weightFormatCB;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.NumericUpDownFloat rotateModel90YUD;
        private Toolbox.Library.Forms.STCheckBox chkUseBoneWeights;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.STCheckBox stCheckBox7;
        private Toolbox.Library.Forms.STCheckBox chkUseOriginalBones;
        private Toolbox.Library.Forms.STButton stButton2;
        private Toolbox.Library.Forms.STComboBox tangentFormatCB;
        private Toolbox.Library.Forms.STCheckBox chkTangents;
        private Toolbox.Library.Forms.STCheckBox chkMatchAttributes;
        private Toolbox.Library.Forms.STCheckBox chkSetNormalsToColorChannel;
        private Toolbox.Library.Forms.STComboBox bitangentFormatCB;
        private Toolbox.Library.Forms.STCheckBox chkBitangents;
    }
}