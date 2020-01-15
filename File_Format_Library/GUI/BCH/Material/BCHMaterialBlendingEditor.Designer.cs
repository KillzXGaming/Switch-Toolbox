namespace FirstPlugin.CtrLibrary.Forms
{
    partial class BCHMaterialBlendingEditor
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
            this.stLabel15 = new Toolbox.Library.Forms.STLabel();
            this.renderLayerCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel14 = new Toolbox.Library.Forms.STLabel();
            this.presetCB = new Toolbox.Library.Forms.STComboBox();
            this.stDropDownPanel2 = new Toolbox.Library.Forms.STDropDownPanel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.alphaCompareCB = new Toolbox.Library.Forms.STComboBox();
            this.chkEnableAlpha = new Toolbox.Library.Forms.STCheckBox();
            this.alphaValueUD = new BarSlider.BarSlider();
            this.stDropDownPanel3 = new Toolbox.Library.Forms.STDropDownPanel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.polyOffsetUD = new BarSlider.BarSlider();
            this.depthCompareCB = new Toolbox.Library.Forms.STComboBox();
            this.chkUsePolyOffset = new Toolbox.Library.Forms.STCheckBox();
            this.chkEnableDepth = new Toolbox.Library.Forms.STCheckBox();
            this.chkDepthWrite = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stDropDownPanel4 = new Toolbox.Library.Forms.STDropDownPanel();
            this.stLabel13 = new Toolbox.Library.Forms.STLabel();
            this.blendModeCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel9 = new Toolbox.Library.Forms.STLabel();
            this.stPanel4 = new Toolbox.Library.Forms.STPanel();
            this.alphaDestCB = new Toolbox.Library.Forms.STComboBox();
            this.alphaSourceCB = new Toolbox.Library.Forms.STComboBox();
            this.alphaEquatCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel10 = new Toolbox.Library.Forms.STLabel();
            this.stLabel11 = new Toolbox.Library.Forms.STLabel();
            this.stLabel12 = new Toolbox.Library.Forms.STLabel();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            this.colorDestCB = new Toolbox.Library.Forms.STComboBox();
            this.colorSourceCB = new Toolbox.Library.Forms.STComboBox();
            this.colorEquatCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel8 = new Toolbox.Library.Forms.STLabel();
            this.stLabel7 = new Toolbox.Library.Forms.STLabel();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel1.SuspendLayout();
            this.stDropDownPanel2.SuspendLayout();
            this.stDropDownPanel3.SuspendLayout();
            this.stDropDownPanel4.SuspendLayout();
            this.stPanel4.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.AutoScroll = true;
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel2);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel3);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel4);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(418, 614);
            this.stFlowLayoutPanel1.TabIndex = 0;
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.stLabel15);
            this.stDropDownPanel1.Controls.Add(this.renderLayerCB);
            this.stDropDownPanel1.Controls.Add(this.stLabel14);
            this.stDropDownPanel1.Controls.Add(this.presetCB);
            this.stDropDownPanel1.ExpandedHeight = 0;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Blend Settings";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(401, 99);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // stLabel15
            // 
            this.stLabel15.AutoSize = true;
            this.stLabel15.Location = new System.Drawing.Point(33, 59);
            this.stLabel15.Name = "stLabel15";
            this.stLabel15.Size = new System.Drawing.Size(36, 13);
            this.stLabel15.TabIndex = 61;
            this.stLabel15.Text = "Layer:";
            // 
            // renderLayerCB
            // 
            this.renderLayerCB.BorderColor = System.Drawing.Color.Empty;
            this.renderLayerCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.renderLayerCB.ButtonColor = System.Drawing.Color.Empty;
            this.renderLayerCB.FormattingEnabled = true;
            this.renderLayerCB.IsReadOnly = false;
            this.renderLayerCB.Location = new System.Drawing.Point(144, 56);
            this.renderLayerCB.Name = "renderLayerCB";
            this.renderLayerCB.Size = new System.Drawing.Size(161, 21);
            this.renderLayerCB.TabIndex = 62;
            // 
            // stLabel14
            // 
            this.stLabel14.AutoSize = true;
            this.stLabel14.Location = new System.Drawing.Point(36, 32);
            this.stLabel14.Name = "stLabel14";
            this.stLabel14.Size = new System.Drawing.Size(40, 13);
            this.stLabel14.TabIndex = 60;
            this.stLabel14.Text = "Preset:";
            // 
            // presetCB
            // 
            this.presetCB.BorderColor = System.Drawing.Color.Empty;
            this.presetCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.presetCB.ButtonColor = System.Drawing.Color.Empty;
            this.presetCB.FormattingEnabled = true;
            this.presetCB.IsReadOnly = false;
            this.presetCB.Location = new System.Drawing.Point(144, 29);
            this.presetCB.Name = "presetCB";
            this.presetCB.Size = new System.Drawing.Size(161, 21);
            this.presetCB.TabIndex = 60;
            // 
            // stDropDownPanel2
            // 
            this.stDropDownPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel2.Controls.Add(this.stLabel2);
            this.stDropDownPanel2.Controls.Add(this.stLabel1);
            this.stDropDownPanel2.Controls.Add(this.alphaCompareCB);
            this.stDropDownPanel2.Controls.Add(this.chkEnableAlpha);
            this.stDropDownPanel2.Controls.Add(this.alphaValueUD);
            this.stDropDownPanel2.ExpandedHeight = 0;
            this.stDropDownPanel2.IsExpanded = true;
            this.stDropDownPanel2.Location = new System.Drawing.Point(0, 99);
            this.stDropDownPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel2.Name = "stDropDownPanel2";
            this.stDropDownPanel2.PanelName = "Alpha Test";
            this.stDropDownPanel2.PanelValueName = "";
            this.stDropDownPanel2.SetIcon = null;
            this.stDropDownPanel2.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.Size = new System.Drawing.Size(401, 108);
            this.stDropDownPanel2.TabIndex = 1;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(138, 73);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(37, 13);
            this.stLabel2.TabIndex = 59;
            this.stLabel2.Text = "Value:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(181, 26);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(82, 13);
            this.stLabel1.TabIndex = 58;
            this.stLabel1.Text = "Compare Mode:";
            // 
            // alphaCompareCB
            // 
            this.alphaCompareCB.BorderColor = System.Drawing.Color.Empty;
            this.alphaCompareCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.alphaCompareCB.ButtonColor = System.Drawing.Color.Empty;
            this.alphaCompareCB.FormattingEnabled = true;
            this.alphaCompareCB.IsReadOnly = false;
            this.alphaCompareCB.Location = new System.Drawing.Point(143, 41);
            this.alphaCompareCB.Name = "alphaCompareCB";
            this.alphaCompareCB.Size = new System.Drawing.Size(161, 21);
            this.alphaCompareCB.TabIndex = 56;
            this.alphaCompareCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // chkEnableAlpha
            // 
            this.chkEnableAlpha.AutoSize = true;
            this.chkEnableAlpha.Location = new System.Drawing.Point(14, 25);
            this.chkEnableAlpha.Name = "chkEnableAlpha";
            this.chkEnableAlpha.Size = new System.Drawing.Size(59, 17);
            this.chkEnableAlpha.TabIndex = 1;
            this.chkEnableAlpha.Text = "Enable";
            this.chkEnableAlpha.UseVisualStyleBackColor = true;
            this.chkEnableAlpha.CheckedChanged += new System.EventHandler(this.EditBlendData);
            // 
            // alphaValueUD
            // 
            this.alphaValueUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.alphaValueUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.alphaValueUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.alphaValueUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.alphaValueUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.alphaValueUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.alphaValueUD.DataType = null;
            this.alphaValueUD.DrawSemitransparentThumb = false;
            this.alphaValueUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.alphaValueUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.alphaValueUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.alphaValueUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.alphaValueUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.alphaValueUD.IncrementAmount = 0.01F;
            this.alphaValueUD.InputName = "X";
            this.alphaValueUD.LargeChange = 5F;
            this.alphaValueUD.Location = new System.Drawing.Point(184, 68);
            this.alphaValueUD.Maximum = 300000F;
            this.alphaValueUD.Minimum = -300000F;
            this.alphaValueUD.Name = "alphaValueUD";
            this.alphaValueUD.Precision = 0.01F;
            this.alphaValueUD.ScaleDivisions = 1;
            this.alphaValueUD.ScaleSubDivisions = 2;
            this.alphaValueUD.ShowDivisionsText = false;
            this.alphaValueUD.ShowSmallScale = false;
            this.alphaValueUD.Size = new System.Drawing.Size(107, 25);
            this.alphaValueUD.SmallChange = 0.01F;
            this.alphaValueUD.TabIndex = 57;
            this.alphaValueUD.Text = "barSlider2";
            this.alphaValueUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.alphaValueUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.alphaValueUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.alphaValueUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.alphaValueUD.TickAdd = 0F;
            this.alphaValueUD.TickColor = System.Drawing.Color.White;
            this.alphaValueUD.TickDivide = 0F;
            this.alphaValueUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.alphaValueUD.UseInterlapsedBar = false;
            this.alphaValueUD.Value = 0F;
            this.alphaValueUD.ValueChanged += new System.EventHandler(this.EditBlendData);
            // 
            // stDropDownPanel3
            // 
            this.stDropDownPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel3.Controls.Add(this.stLabel4);
            this.stDropDownPanel3.Controls.Add(this.polyOffsetUD);
            this.stDropDownPanel3.Controls.Add(this.depthCompareCB);
            this.stDropDownPanel3.Controls.Add(this.chkUsePolyOffset);
            this.stDropDownPanel3.Controls.Add(this.chkEnableDepth);
            this.stDropDownPanel3.Controls.Add(this.chkDepthWrite);
            this.stDropDownPanel3.Controls.Add(this.stLabel3);
            this.stDropDownPanel3.ExpandedHeight = 0;
            this.stDropDownPanel3.IsExpanded = true;
            this.stDropDownPanel3.Location = new System.Drawing.Point(0, 207);
            this.stDropDownPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel3.Name = "stDropDownPanel3";
            this.stDropDownPanel3.PanelName = "Depth Alpha Test";
            this.stDropDownPanel3.PanelValueName = "";
            this.stDropDownPanel3.SetIcon = null;
            this.stDropDownPanel3.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.Size = new System.Drawing.Size(401, 119);
            this.stDropDownPanel3.TabIndex = 2;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(141, 87);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(38, 13);
            this.stLabel4.TabIndex = 65;
            this.stLabel4.Text = "Offset:";
            // 
            // polyOffsetUD
            // 
            this.polyOffsetUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.polyOffsetUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.polyOffsetUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.polyOffsetUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.polyOffsetUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.polyOffsetUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.polyOffsetUD.DataType = null;
            this.polyOffsetUD.DrawSemitransparentThumb = false;
            this.polyOffsetUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.polyOffsetUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.polyOffsetUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.polyOffsetUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.polyOffsetUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.polyOffsetUD.IncrementAmount = 0.01F;
            this.polyOffsetUD.InputName = "";
            this.polyOffsetUD.LargeChange = 5F;
            this.polyOffsetUD.Location = new System.Drawing.Point(184, 79);
            this.polyOffsetUD.Maximum = 300000F;
            this.polyOffsetUD.Minimum = -300000F;
            this.polyOffsetUD.Name = "polyOffsetUD";
            this.polyOffsetUD.Precision = 0.01F;
            this.polyOffsetUD.ScaleDivisions = 1;
            this.polyOffsetUD.ScaleSubDivisions = 2;
            this.polyOffsetUD.ShowDivisionsText = false;
            this.polyOffsetUD.ShowSmallScale = false;
            this.polyOffsetUD.Size = new System.Drawing.Size(107, 25);
            this.polyOffsetUD.SmallChange = 0.01F;
            this.polyOffsetUD.TabIndex = 60;
            this.polyOffsetUD.Text = "barSlider2";
            this.polyOffsetUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.polyOffsetUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.polyOffsetUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.polyOffsetUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.polyOffsetUD.TickAdd = 0F;
            this.polyOffsetUD.TickColor = System.Drawing.Color.White;
            this.polyOffsetUD.TickDivide = 0F;
            this.polyOffsetUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.polyOffsetUD.UseInterlapsedBar = false;
            this.polyOffsetUD.Value = 0F;
            this.polyOffsetUD.ValueChanged += new System.EventHandler(this.EditBlendData);
            // 
            // depthCompareCB
            // 
            this.depthCompareCB.BorderColor = System.Drawing.Color.Empty;
            this.depthCompareCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.depthCompareCB.ButtonColor = System.Drawing.Color.Empty;
            this.depthCompareCB.FormattingEnabled = true;
            this.depthCompareCB.IsReadOnly = false;
            this.depthCompareCB.Location = new System.Drawing.Point(144, 42);
            this.depthCompareCB.Name = "depthCompareCB";
            this.depthCompareCB.Size = new System.Drawing.Size(161, 21);
            this.depthCompareCB.TabIndex = 60;
            this.depthCompareCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // chkUsePolyOffset
            // 
            this.chkUsePolyOffset.AutoSize = true;
            this.chkUsePolyOffset.Location = new System.Drawing.Point(15, 86);
            this.chkUsePolyOffset.Name = "chkUsePolyOffset";
            this.chkUsePolyOffset.Size = new System.Drawing.Size(112, 17);
            this.chkUsePolyOffset.TabIndex = 64;
            this.chkUsePolyOffset.Text = "Use Polgon Offset";
            this.chkUsePolyOffset.UseVisualStyleBackColor = true;
            this.chkUsePolyOffset.CheckedChanged += new System.EventHandler(this.EditBlendData);
            // 
            // chkEnableDepth
            // 
            this.chkEnableDepth.AutoSize = true;
            this.chkEnableDepth.Location = new System.Drawing.Point(18, 26);
            this.chkEnableDepth.Name = "chkEnableDepth";
            this.chkEnableDepth.Size = new System.Drawing.Size(59, 17);
            this.chkEnableDepth.TabIndex = 3;
            this.chkEnableDepth.Text = "Enable";
            this.chkEnableDepth.UseVisualStyleBackColor = true;
            this.chkEnableDepth.CheckedChanged += new System.EventHandler(this.EditBlendData);
            // 
            // chkDepthWrite
            // 
            this.chkDepthWrite.AutoSize = true;
            this.chkDepthWrite.Location = new System.Drawing.Point(15, 63);
            this.chkDepthWrite.Name = "chkDepthWrite";
            this.chkDepthWrite.Size = new System.Drawing.Size(86, 17);
            this.chkDepthWrite.TabIndex = 63;
            this.chkDepthWrite.Text = "Depth Write:";
            this.chkDepthWrite.UseVisualStyleBackColor = true;
            this.chkDepthWrite.CheckedChanged += new System.EventHandler(this.EditBlendData);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(181, 27);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(82, 13);
            this.stLabel3.TabIndex = 61;
            this.stLabel3.Text = "Compare Mode:";
            // 
            // stDropDownPanel4
            // 
            this.stDropDownPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel4.Controls.Add(this.stLabel13);
            this.stDropDownPanel4.Controls.Add(this.blendModeCB);
            this.stDropDownPanel4.Controls.Add(this.stLabel9);
            this.stDropDownPanel4.Controls.Add(this.stPanel4);
            this.stDropDownPanel4.Controls.Add(this.stLabel5);
            this.stDropDownPanel4.Controls.Add(this.stPanel3);
            this.stDropDownPanel4.ExpandedHeight = 0;
            this.stDropDownPanel4.IsExpanded = true;
            this.stDropDownPanel4.Location = new System.Drawing.Point(0, 326);
            this.stDropDownPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel4.Name = "stDropDownPanel4";
            this.stDropDownPanel4.PanelName = "Blending";
            this.stDropDownPanel4.PanelValueName = "";
            this.stDropDownPanel4.SetIcon = null;
            this.stDropDownPanel4.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel4.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel4.Size = new System.Drawing.Size(401, 296);
            this.stDropDownPanel4.TabIndex = 3;
            // 
            // stLabel13
            // 
            this.stLabel13.AutoSize = true;
            this.stLabel13.Location = new System.Drawing.Point(16, 32);
            this.stLabel13.Name = "stLabel13";
            this.stLabel13.Size = new System.Drawing.Size(37, 13);
            this.stLabel13.TabIndex = 9;
            this.stLabel13.Text = "Mode:";
            // 
            // blendModeCB
            // 
            this.blendModeCB.BorderColor = System.Drawing.Color.Empty;
            this.blendModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.blendModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.blendModeCB.FormattingEnabled = true;
            this.blendModeCB.IsReadOnly = false;
            this.blendModeCB.Location = new System.Drawing.Point(144, 29);
            this.blendModeCB.Name = "blendModeCB";
            this.blendModeCB.Size = new System.Drawing.Size(161, 21);
            this.blendModeCB.TabIndex = 9;
            this.blendModeCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // stLabel9
            // 
            this.stLabel9.AutoSize = true;
            this.stLabel9.Location = new System.Drawing.Point(178, 179);
            this.stLabel9.Name = "stLabel9";
            this.stLabel9.Size = new System.Drawing.Size(81, 13);
            this.stLabel9.TabIndex = 4;
            this.stLabel9.Text = "Alpha Blending:";
            // 
            // stPanel4
            // 
            this.stPanel4.Controls.Add(this.alphaDestCB);
            this.stPanel4.Controls.Add(this.alphaSourceCB);
            this.stPanel4.Controls.Add(this.alphaEquatCB);
            this.stPanel4.Controls.Add(this.stLabel10);
            this.stPanel4.Controls.Add(this.stLabel11);
            this.stPanel4.Controls.Add(this.stLabel12);
            this.stPanel4.Location = new System.Drawing.Point(8, 195);
            this.stPanel4.Name = "stPanel4";
            this.stPanel4.Size = new System.Drawing.Size(400, 100);
            this.stPanel4.TabIndex = 3;
            // 
            // alphaDestCB
            // 
            this.alphaDestCB.BorderColor = System.Drawing.Color.Empty;
            this.alphaDestCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.alphaDestCB.ButtonColor = System.Drawing.Color.Empty;
            this.alphaDestCB.FormattingEnabled = true;
            this.alphaDestCB.IsReadOnly = false;
            this.alphaDestCB.Location = new System.Drawing.Point(135, 62);
            this.alphaDestCB.Name = "alphaDestCB";
            this.alphaDestCB.Size = new System.Drawing.Size(161, 21);
            this.alphaDestCB.TabIndex = 8;
            this.alphaDestCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // alphaSourceCB
            // 
            this.alphaSourceCB.BorderColor = System.Drawing.Color.Empty;
            this.alphaSourceCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.alphaSourceCB.ButtonColor = System.Drawing.Color.Empty;
            this.alphaSourceCB.FormattingEnabled = true;
            this.alphaSourceCB.IsReadOnly = false;
            this.alphaSourceCB.Location = new System.Drawing.Point(135, 35);
            this.alphaSourceCB.Name = "alphaSourceCB";
            this.alphaSourceCB.Size = new System.Drawing.Size(161, 21);
            this.alphaSourceCB.TabIndex = 7;
            this.alphaSourceCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // alphaEquatCB
            // 
            this.alphaEquatCB.BorderColor = System.Drawing.Color.Empty;
            this.alphaEquatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.alphaEquatCB.ButtonColor = System.Drawing.Color.Empty;
            this.alphaEquatCB.FormattingEnabled = true;
            this.alphaEquatCB.IsReadOnly = false;
            this.alphaEquatCB.Location = new System.Drawing.Point(135, 8);
            this.alphaEquatCB.Name = "alphaEquatCB";
            this.alphaEquatCB.Size = new System.Drawing.Size(161, 21);
            this.alphaEquatCB.TabIndex = 6;
            this.alphaEquatCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // stLabel10
            // 
            this.stLabel10.AutoSize = true;
            this.stLabel10.Location = new System.Drawing.Point(8, 65);
            this.stLabel10.Name = "stLabel10";
            this.stLabel10.Size = new System.Drawing.Size(63, 13);
            this.stLabel10.TabIndex = 5;
            this.stLabel10.Text = "Destination:";
            // 
            // stLabel11
            // 
            this.stLabel11.AutoSize = true;
            this.stLabel11.Location = new System.Drawing.Point(8, 38);
            this.stLabel11.Name = "stLabel11";
            this.stLabel11.Size = new System.Drawing.Size(44, 13);
            this.stLabel11.TabIndex = 4;
            this.stLabel11.Text = "Source:";
            // 
            // stLabel12
            // 
            this.stLabel12.AutoSize = true;
            this.stLabel12.Location = new System.Drawing.Point(8, 11);
            this.stLabel12.Name = "stLabel12";
            this.stLabel12.Size = new System.Drawing.Size(52, 13);
            this.stLabel12.TabIndex = 3;
            this.stLabel12.Text = "Equation:";
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(181, 60);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(78, 13);
            this.stLabel5.TabIndex = 2;
            this.stLabel5.Text = "Color Blending:";
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.colorDestCB);
            this.stPanel3.Controls.Add(this.colorSourceCB);
            this.stPanel3.Controls.Add(this.colorEquatCB);
            this.stPanel3.Controls.Add(this.stLabel8);
            this.stPanel3.Controls.Add(this.stLabel7);
            this.stPanel3.Controls.Add(this.stLabel6);
            this.stPanel3.Location = new System.Drawing.Point(8, 76);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(400, 100);
            this.stPanel3.TabIndex = 1;
            // 
            // colorDestCB
            // 
            this.colorDestCB.BorderColor = System.Drawing.Color.Empty;
            this.colorDestCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.colorDestCB.ButtonColor = System.Drawing.Color.Empty;
            this.colorDestCB.FormattingEnabled = true;
            this.colorDestCB.IsReadOnly = false;
            this.colorDestCB.Location = new System.Drawing.Point(135, 65);
            this.colorDestCB.Name = "colorDestCB";
            this.colorDestCB.Size = new System.Drawing.Size(161, 21);
            this.colorDestCB.TabIndex = 8;
            this.colorDestCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // colorSourceCB
            // 
            this.colorSourceCB.BorderColor = System.Drawing.Color.Empty;
            this.colorSourceCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.colorSourceCB.ButtonColor = System.Drawing.Color.Empty;
            this.colorSourceCB.FormattingEnabled = true;
            this.colorSourceCB.IsReadOnly = false;
            this.colorSourceCB.Location = new System.Drawing.Point(135, 38);
            this.colorSourceCB.Name = "colorSourceCB";
            this.colorSourceCB.Size = new System.Drawing.Size(161, 21);
            this.colorSourceCB.TabIndex = 7;
            this.colorSourceCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // colorEquatCB
            // 
            this.colorEquatCB.BorderColor = System.Drawing.Color.Empty;
            this.colorEquatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.colorEquatCB.ButtonColor = System.Drawing.Color.Empty;
            this.colorEquatCB.FormattingEnabled = true;
            this.colorEquatCB.IsReadOnly = false;
            this.colorEquatCB.Location = new System.Drawing.Point(135, 11);
            this.colorEquatCB.Name = "colorEquatCB";
            this.colorEquatCB.Size = new System.Drawing.Size(161, 21);
            this.colorEquatCB.TabIndex = 6;
            this.colorEquatCB.SelectedIndexChanged += new System.EventHandler(this.EditBlendData);
            // 
            // stLabel8
            // 
            this.stLabel8.AutoSize = true;
            this.stLabel8.Location = new System.Drawing.Point(8, 73);
            this.stLabel8.Name = "stLabel8";
            this.stLabel8.Size = new System.Drawing.Size(63, 13);
            this.stLabel8.TabIndex = 5;
            this.stLabel8.Text = "Destination:";
            // 
            // stLabel7
            // 
            this.stLabel7.AutoSize = true;
            this.stLabel7.Location = new System.Drawing.Point(8, 46);
            this.stLabel7.Name = "stLabel7";
            this.stLabel7.Size = new System.Drawing.Size(44, 13);
            this.stLabel7.TabIndex = 4;
            this.stLabel7.Text = "Source:";
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(8, 19);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(52, 13);
            this.stLabel6.TabIndex = 3;
            this.stLabel6.Text = "Equation:";
            // 
            // BCHMaterialBlendingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "BCHMaterialBlendingEditor";
            this.Size = new System.Drawing.Size(418, 614);
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stDropDownPanel2.ResumeLayout(false);
            this.stDropDownPanel2.PerformLayout();
            this.stDropDownPanel3.ResumeLayout(false);
            this.stDropDownPanel3.PerformLayout();
            this.stDropDownPanel4.ResumeLayout(false);
            this.stDropDownPanel4.PerformLayout();
            this.stPanel4.ResumeLayout(false);
            this.stPanel4.PerformLayout();
            this.stPanel3.ResumeLayout(false);
            this.stPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel2;
        private Toolbox.Library.Forms.STCheckBox chkEnableAlpha;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel3;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel4;
        private Toolbox.Library.Forms.STComboBox alphaCompareCB;
        private BarSlider.BarSlider alphaValueUD;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STComboBox depthCompareCB;
        private Toolbox.Library.Forms.STCheckBox chkDepthWrite;
        private BarSlider.BarSlider polyOffsetUD;
        private Toolbox.Library.Forms.STCheckBox chkUsePolyOffset;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.STPanel stPanel3;
        private Toolbox.Library.Forms.STLabel stLabel8;
        private Toolbox.Library.Forms.STLabel stLabel7;
        private Toolbox.Library.Forms.STLabel stLabel6;
        private Toolbox.Library.Forms.STComboBox colorDestCB;
        private Toolbox.Library.Forms.STComboBox colorSourceCB;
        private Toolbox.Library.Forms.STComboBox colorEquatCB;
        private Toolbox.Library.Forms.STLabel stLabel9;
        private Toolbox.Library.Forms.STPanel stPanel4;
        private Toolbox.Library.Forms.STComboBox alphaDestCB;
        private Toolbox.Library.Forms.STComboBox alphaSourceCB;
        private Toolbox.Library.Forms.STComboBox alphaEquatCB;
        private Toolbox.Library.Forms.STLabel stLabel10;
        private Toolbox.Library.Forms.STLabel stLabel11;
        private Toolbox.Library.Forms.STLabel stLabel12;
        private Toolbox.Library.Forms.STLabel stLabel13;
        private Toolbox.Library.Forms.STComboBox blendModeCB;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STCheckBox chkEnableDepth;
        private Toolbox.Library.Forms.STLabel stLabel14;
        private Toolbox.Library.Forms.STComboBox presetCB;
        private Toolbox.Library.Forms.STLabel stLabel15;
        private Toolbox.Library.Forms.STComboBox renderLayerCB;
    }
}
