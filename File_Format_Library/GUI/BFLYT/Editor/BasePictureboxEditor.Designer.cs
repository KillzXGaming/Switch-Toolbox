namespace LayoutBXLYT
{
    partial class BasePictureboxEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasePictureboxEditor));
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stDropDownPanel1 = new Toolbox.Library.Forms.STDropDownPanel();
            this.btnResetColors = new Toolbox.Library.Forms.STButton();
            this.vertexColorBox1 = new Toolbox.Library.Forms.VertexColorBox();
            this.stDropDownPanel3 = new Toolbox.Library.Forms.STDropDownPanel();
            this.texCoordIndexCB = new Toolbox.Library.Forms.STComboBox();
            this.bottomRightYUD = new BarSlider.BarSlider();
            this.bottomRightXUD = new BarSlider.BarSlider();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.bottomLeftYUD = new BarSlider.BarSlider();
            this.bottomLeftXUD = new BarSlider.BarSlider();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.topRightYUD = new BarSlider.BarSlider();
            this.topRightXUD = new BarSlider.BarSlider();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.topLeftYUD = new BarSlider.BarSlider();
            this.topLeftXUD = new BarSlider.BarSlider();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel1.SuspendLayout();
            this.stDropDownPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel3);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(335, 418);
            this.stFlowLayoutPanel1.TabIndex = 0;
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.btnResetColors);
            this.stDropDownPanel1.Controls.Add(this.vertexColorBox1);
            this.stDropDownPanel1.ExpandedHeight = 0;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Vertex Colors";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(335, 133);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // btnResetColors
            // 
            this.btnResetColors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetColors.Location = new System.Drawing.Point(128, 27);
            this.btnResetColors.Name = "btnResetColors";
            this.btnResetColors.Size = new System.Drawing.Size(94, 23);
            this.btnResetColors.TabIndex = 2;
            this.btnResetColors.Text = "Reset Colors";
            this.btnResetColors.UseVisualStyleBackColor = false;
            this.btnResetColors.Click += new System.EventHandler(this.btnResetColors_Click);
            // 
            // vertexColorBox1
            // 
            this.vertexColorBox1.BackColor = System.Drawing.Color.Transparent;
            this.vertexColorBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("vertexColorBox1.BackgroundImage")));
            this.vertexColorBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.vertexColorBox1.BottomLeftColor = System.Drawing.Color.Empty;
            this.vertexColorBox1.BottomRightColor = System.Drawing.Color.Empty;
            this.vertexColorBox1.Location = new System.Drawing.Point(22, 27);
            this.vertexColorBox1.Name = "vertexColorBox1";
            this.vertexColorBox1.Size = new System.Drawing.Size(100, 100);
            this.vertexColorBox1.TabIndex = 1;
            this.vertexColorBox1.TopLeftColor = System.Drawing.Color.Empty;
            this.vertexColorBox1.TopRightColor = System.Drawing.Color.Empty;
            // 
            // stDropDownPanel3
            // 
            this.stDropDownPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel3.Controls.Add(this.texCoordIndexCB);
            this.stDropDownPanel3.Controls.Add(this.bottomRightYUD);
            this.stDropDownPanel3.Controls.Add(this.bottomRightXUD);
            this.stDropDownPanel3.Controls.Add(this.stLabel2);
            this.stDropDownPanel3.Controls.Add(this.bottomLeftYUD);
            this.stDropDownPanel3.Controls.Add(this.bottomLeftXUD);
            this.stDropDownPanel3.Controls.Add(this.stLabel3);
            this.stDropDownPanel3.Controls.Add(this.topRightYUD);
            this.stDropDownPanel3.Controls.Add(this.topRightXUD);
            this.stDropDownPanel3.Controls.Add(this.stLabel1);
            this.stDropDownPanel3.Controls.Add(this.topLeftYUD);
            this.stDropDownPanel3.Controls.Add(this.topLeftXUD);
            this.stDropDownPanel3.Controls.Add(this.stLabel6);
            this.stDropDownPanel3.ExpandedHeight = 0;
            this.stDropDownPanel3.IsExpanded = true;
            this.stDropDownPanel3.Location = new System.Drawing.Point(0, 133);
            this.stDropDownPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel3.Name = "stDropDownPanel3";
            this.stDropDownPanel3.PanelName = "Texture Coordinates";
            this.stDropDownPanel3.PanelValueName = "";
            this.stDropDownPanel3.SetIcon = null;
            this.stDropDownPanel3.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.Size = new System.Drawing.Size(335, 285);
            this.stDropDownPanel3.TabIndex = 2;
            // 
            // texCoordIndexCB
            // 
            this.texCoordIndexCB.BorderColor = System.Drawing.Color.Empty;
            this.texCoordIndexCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.texCoordIndexCB.ButtonColor = System.Drawing.Color.Empty;
            this.texCoordIndexCB.FormattingEnabled = true;
            this.texCoordIndexCB.IsReadOnly = false;
            this.texCoordIndexCB.Location = new System.Drawing.Point(220, 24);
            this.texCoordIndexCB.Name = "texCoordIndexCB";
            this.texCoordIndexCB.Size = new System.Drawing.Size(101, 21);
            this.texCoordIndexCB.TabIndex = 59;
            this.texCoordIndexCB.SelectedIndexChanged += new System.EventHandler(this.texCoordIndexCB_SelectedIndexChanged);
            // 
            // bottomRightYUD
            // 
            this.bottomRightYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.bottomRightYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bottomRightYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.bottomRightYUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.bottomRightYUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomRightYUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.bottomRightYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.bottomRightYUD.DataType = null;
            this.bottomRightYUD.DrawSemitransparentThumb = false;
            this.bottomRightYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomRightYUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.bottomRightYUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomRightYUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.bottomRightYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.bottomRightYUD.ForeColor = System.Drawing.Color.White;
            this.bottomRightYUD.IncrementAmount = 0.01F;
            this.bottomRightYUD.InputName = "Y";
            this.bottomRightYUD.LargeChange = 5F;
            this.bottomRightYUD.Location = new System.Drawing.Point(220, 162);
            this.bottomRightYUD.Maximum = 300000F;
            this.bottomRightYUD.Minimum = -300000F;
            this.bottomRightYUD.Name = "bottomRightYUD";
            this.bottomRightYUD.Precision = 0.01F;
            this.bottomRightYUD.ScaleDivisions = 1;
            this.bottomRightYUD.ScaleSubDivisions = 2;
            this.bottomRightYUD.ShowDivisionsText = false;
            this.bottomRightYUD.ShowSmallScale = false;
            this.bottomRightYUD.Size = new System.Drawing.Size(107, 25);
            this.bottomRightYUD.SmallChange = 0.01F;
            this.bottomRightYUD.TabIndex = 58;
            this.bottomRightYUD.Text = "barSlider2";
            this.bottomRightYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomRightYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomRightYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.bottomRightYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.bottomRightYUD.TickAdd = 0F;
            this.bottomRightYUD.TickColor = System.Drawing.Color.White;
            this.bottomRightYUD.TickDivide = 0F;
            this.bottomRightYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.bottomRightYUD.UseInterlapsedBar = false;
            this.bottomRightYUD.Value = 30F;
            // 
            // bottomRightXUD
            // 
            this.bottomRightXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.bottomRightXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bottomRightXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.bottomRightXUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.bottomRightXUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomRightXUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.bottomRightXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.bottomRightXUD.DataType = null;
            this.bottomRightXUD.DrawSemitransparentThumb = false;
            this.bottomRightXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomRightXUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.bottomRightXUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomRightXUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.bottomRightXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.bottomRightXUD.ForeColor = System.Drawing.Color.White;
            this.bottomRightXUD.IncrementAmount = 0.01F;
            this.bottomRightXUD.InputName = "X";
            this.bottomRightXUD.LargeChange = 5F;
            this.bottomRightXUD.Location = new System.Drawing.Point(107, 162);
            this.bottomRightXUD.Maximum = 300000F;
            this.bottomRightXUD.Minimum = -300000F;
            this.bottomRightXUD.Name = "bottomRightXUD";
            this.bottomRightXUD.Precision = 0.01F;
            this.bottomRightXUD.ScaleDivisions = 1;
            this.bottomRightXUD.ScaleSubDivisions = 2;
            this.bottomRightXUD.ShowDivisionsText = false;
            this.bottomRightXUD.ShowSmallScale = false;
            this.bottomRightXUD.Size = new System.Drawing.Size(107, 25);
            this.bottomRightXUD.SmallChange = 0.01F;
            this.bottomRightXUD.TabIndex = 57;
            this.bottomRightXUD.Text = "barSlider4";
            this.bottomRightXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomRightXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomRightXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.bottomRightXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.bottomRightXUD.TickAdd = 0F;
            this.bottomRightXUD.TickColor = System.Drawing.Color.White;
            this.bottomRightXUD.TickDivide = 0F;
            this.bottomRightXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.bottomRightXUD.UseInterlapsedBar = false;
            this.bottomRightXUD.Value = 30F;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(19, 168);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(68, 13);
            this.stLabel2.TabIndex = 56;
            this.stLabel2.Text = "Bottom Right";
            // 
            // bottomLeftYUD
            // 
            this.bottomLeftYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.bottomLeftYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bottomLeftYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.bottomLeftYUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.bottomLeftYUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomLeftYUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.bottomLeftYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.bottomLeftYUD.DataType = null;
            this.bottomLeftYUD.DrawSemitransparentThumb = false;
            this.bottomLeftYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomLeftYUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.bottomLeftYUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomLeftYUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.bottomLeftYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.bottomLeftYUD.ForeColor = System.Drawing.Color.White;
            this.bottomLeftYUD.IncrementAmount = 0.01F;
            this.bottomLeftYUD.InputName = "Y";
            this.bottomLeftYUD.LargeChange = 5F;
            this.bottomLeftYUD.Location = new System.Drawing.Point(220, 131);
            this.bottomLeftYUD.Maximum = 300000F;
            this.bottomLeftYUD.Minimum = -300000F;
            this.bottomLeftYUD.Name = "bottomLeftYUD";
            this.bottomLeftYUD.Precision = 0.01F;
            this.bottomLeftYUD.ScaleDivisions = 1;
            this.bottomLeftYUD.ScaleSubDivisions = 2;
            this.bottomLeftYUD.ShowDivisionsText = false;
            this.bottomLeftYUD.ShowSmallScale = false;
            this.bottomLeftYUD.Size = new System.Drawing.Size(107, 25);
            this.bottomLeftYUD.SmallChange = 0.01F;
            this.bottomLeftYUD.TabIndex = 55;
            this.bottomLeftYUD.Text = "barSlider2";
            this.bottomLeftYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomLeftYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomLeftYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.bottomLeftYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.bottomLeftYUD.TickAdd = 0F;
            this.bottomLeftYUD.TickColor = System.Drawing.Color.White;
            this.bottomLeftYUD.TickDivide = 0F;
            this.bottomLeftYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.bottomLeftYUD.UseInterlapsedBar = false;
            this.bottomLeftYUD.Value = 30F;
            // 
            // bottomLeftXUD
            // 
            this.bottomLeftXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.bottomLeftXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bottomLeftXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.bottomLeftXUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.bottomLeftXUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomLeftXUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.bottomLeftXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.bottomLeftXUD.DataType = null;
            this.bottomLeftXUD.DrawSemitransparentThumb = false;
            this.bottomLeftXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomLeftXUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.bottomLeftXUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.bottomLeftXUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.bottomLeftXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.bottomLeftXUD.ForeColor = System.Drawing.Color.White;
            this.bottomLeftXUD.IncrementAmount = 0.01F;
            this.bottomLeftXUD.InputName = "X";
            this.bottomLeftXUD.LargeChange = 5F;
            this.bottomLeftXUD.Location = new System.Drawing.Point(107, 131);
            this.bottomLeftXUD.Maximum = 300000F;
            this.bottomLeftXUD.Minimum = -300000F;
            this.bottomLeftXUD.Name = "bottomLeftXUD";
            this.bottomLeftXUD.Precision = 0.01F;
            this.bottomLeftXUD.ScaleDivisions = 1;
            this.bottomLeftXUD.ScaleSubDivisions = 2;
            this.bottomLeftXUD.ShowDivisionsText = false;
            this.bottomLeftXUD.ShowSmallScale = false;
            this.bottomLeftXUD.Size = new System.Drawing.Size(107, 25);
            this.bottomLeftXUD.SmallChange = 0.01F;
            this.bottomLeftXUD.TabIndex = 54;
            this.bottomLeftXUD.Text = "barSlider2";
            this.bottomLeftXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomLeftXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bottomLeftXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.bottomLeftXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.bottomLeftXUD.TickAdd = 0F;
            this.bottomLeftXUD.TickColor = System.Drawing.Color.White;
            this.bottomLeftXUD.TickDivide = 0F;
            this.bottomLeftXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.bottomLeftXUD.UseInterlapsedBar = false;
            this.bottomLeftXUD.Value = 30F;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(19, 137);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(64, 13);
            this.stLabel3.TabIndex = 53;
            this.stLabel3.Text = "Bottom Left:";
            // 
            // topRightYUD
            // 
            this.topRightYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.topRightYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.topRightYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.topRightYUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.topRightYUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.topRightYUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.topRightYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.topRightYUD.DataType = null;
            this.topRightYUD.DrawSemitransparentThumb = false;
            this.topRightYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topRightYUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.topRightYUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.topRightYUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.topRightYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.topRightYUD.ForeColor = System.Drawing.Color.White;
            this.topRightYUD.IncrementAmount = 0.01F;
            this.topRightYUD.InputName = "Y";
            this.topRightYUD.LargeChange = 5F;
            this.topRightYUD.Location = new System.Drawing.Point(220, 85);
            this.topRightYUD.Maximum = 300000F;
            this.topRightYUD.Minimum = -300000F;
            this.topRightYUD.Name = "topRightYUD";
            this.topRightYUD.Precision = 0.01F;
            this.topRightYUD.ScaleDivisions = 1;
            this.topRightYUD.ScaleSubDivisions = 2;
            this.topRightYUD.ShowDivisionsText = false;
            this.topRightYUD.ShowSmallScale = false;
            this.topRightYUD.Size = new System.Drawing.Size(107, 25);
            this.topRightYUD.SmallChange = 0.01F;
            this.topRightYUD.TabIndex = 52;
            this.topRightYUD.Text = "barSlider2";
            this.topRightYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topRightYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topRightYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.topRightYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.topRightYUD.TickAdd = 0F;
            this.topRightYUD.TickColor = System.Drawing.Color.White;
            this.topRightYUD.TickDivide = 0F;
            this.topRightYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.topRightYUD.UseInterlapsedBar = false;
            this.topRightYUD.Value = 30F;
            // 
            // topRightXUD
            // 
            this.topRightXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.topRightXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.topRightXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.topRightXUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.topRightXUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.topRightXUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.topRightXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.topRightXUD.DataType = null;
            this.topRightXUD.DrawSemitransparentThumb = false;
            this.topRightXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topRightXUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.topRightXUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.topRightXUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.topRightXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.topRightXUD.ForeColor = System.Drawing.Color.White;
            this.topRightXUD.IncrementAmount = 0.01F;
            this.topRightXUD.InputName = "X";
            this.topRightXUD.LargeChange = 5F;
            this.topRightXUD.Location = new System.Drawing.Point(107, 85);
            this.topRightXUD.Maximum = 300000F;
            this.topRightXUD.Minimum = -300000F;
            this.topRightXUD.Name = "topRightXUD";
            this.topRightXUD.Precision = 0.01F;
            this.topRightXUD.ScaleDivisions = 1;
            this.topRightXUD.ScaleSubDivisions = 2;
            this.topRightXUD.ShowDivisionsText = false;
            this.topRightXUD.ShowSmallScale = false;
            this.topRightXUD.Size = new System.Drawing.Size(107, 25);
            this.topRightXUD.SmallChange = 0.01F;
            this.topRightXUD.TabIndex = 51;
            this.topRightXUD.Text = "barSlider2";
            this.topRightXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topRightXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topRightXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.topRightXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.topRightXUD.TickAdd = 0F;
            this.topRightXUD.TickColor = System.Drawing.Color.White;
            this.topRightXUD.TickDivide = 0F;
            this.topRightXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.topRightXUD.UseInterlapsedBar = false;
            this.topRightXUD.Value = 30F;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(19, 91);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(54, 13);
            this.stLabel1.TabIndex = 50;
            this.stLabel1.Text = "Top Right";
            // 
            // topLeftYUD
            // 
            this.topLeftYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.topLeftYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.topLeftYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.topLeftYUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.topLeftYUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.topLeftYUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.topLeftYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.topLeftYUD.DataType = null;
            this.topLeftYUD.DrawSemitransparentThumb = false;
            this.topLeftYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topLeftYUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.topLeftYUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.topLeftYUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.topLeftYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.topLeftYUD.ForeColor = System.Drawing.Color.White;
            this.topLeftYUD.IncrementAmount = 0.01F;
            this.topLeftYUD.InputName = "Y";
            this.topLeftYUD.LargeChange = 5F;
            this.topLeftYUD.Location = new System.Drawing.Point(220, 54);
            this.topLeftYUD.Maximum = 300000F;
            this.topLeftYUD.Minimum = -300000F;
            this.topLeftYUD.Name = "topLeftYUD";
            this.topLeftYUD.Precision = 0.01F;
            this.topLeftYUD.ScaleDivisions = 1;
            this.topLeftYUD.ScaleSubDivisions = 2;
            this.topLeftYUD.ShowDivisionsText = false;
            this.topLeftYUD.ShowSmallScale = false;
            this.topLeftYUD.Size = new System.Drawing.Size(107, 25);
            this.topLeftYUD.SmallChange = 0.01F;
            this.topLeftYUD.TabIndex = 49;
            this.topLeftYUD.Text = "barSlider2";
            this.topLeftYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topLeftYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topLeftYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.topLeftYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.topLeftYUD.TickAdd = 0F;
            this.topLeftYUD.TickColor = System.Drawing.Color.White;
            this.topLeftYUD.TickDivide = 0F;
            this.topLeftYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.topLeftYUD.UseInterlapsedBar = false;
            this.topLeftYUD.Value = 30F;
            // 
            // topLeftXUD
            // 
            this.topLeftXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.topLeftXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.topLeftXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.topLeftXUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.topLeftXUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.topLeftXUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.topLeftXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.topLeftXUD.DataType = null;
            this.topLeftXUD.DrawSemitransparentThumb = false;
            this.topLeftXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topLeftXUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.topLeftXUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.topLeftXUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.topLeftXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.topLeftXUD.ForeColor = System.Drawing.Color.White;
            this.topLeftXUD.IncrementAmount = 0.01F;
            this.topLeftXUD.InputName = "X";
            this.topLeftXUD.LargeChange = 5F;
            this.topLeftXUD.Location = new System.Drawing.Point(107, 54);
            this.topLeftXUD.Maximum = 300000F;
            this.topLeftXUD.Minimum = -300000F;
            this.topLeftXUD.Name = "topLeftXUD";
            this.topLeftXUD.Precision = 0.01F;
            this.topLeftXUD.ScaleDivisions = 1;
            this.topLeftXUD.ScaleSubDivisions = 2;
            this.topLeftXUD.ShowDivisionsText = false;
            this.topLeftXUD.ShowSmallScale = false;
            this.topLeftXUD.Size = new System.Drawing.Size(107, 25);
            this.topLeftXUD.SmallChange = 0.01F;
            this.topLeftXUD.TabIndex = 48;
            this.topLeftXUD.Text = "barSlider2";
            this.topLeftXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topLeftXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.topLeftXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.topLeftXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.topLeftXUD.TickAdd = 0F;
            this.topLeftXUD.TickColor = System.Drawing.Color.White;
            this.topLeftXUD.TickDivide = 0F;
            this.topLeftXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.topLeftXUD.UseInterlapsedBar = false;
            this.topLeftXUD.Value = 30F;
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(19, 60);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(50, 13);
            this.stLabel6.TabIndex = 47;
            this.stLabel6.Text = "Top Left:";
            // 
            // BasePictureboxEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "BasePictureboxEditor";
            this.Size = new System.Drawing.Size(335, 418);
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stDropDownPanel3.ResumeLayout(false);
            this.stDropDownPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.STButton btnResetColors;
        private Toolbox.Library.Forms.VertexColorBox vertexColorBox1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel3;
        private BarSlider.BarSlider bottomRightYUD;
        private BarSlider.BarSlider bottomRightXUD;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private BarSlider.BarSlider bottomLeftYUD;
        private BarSlider.BarSlider bottomLeftXUD;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private BarSlider.BarSlider topRightYUD;
        private BarSlider.BarSlider topRightXUD;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private BarSlider.BarSlider topLeftYUD;
        private BarSlider.BarSlider topLeftXUD;
        private Toolbox.Library.Forms.STLabel stLabel6;
        private Toolbox.Library.Forms.STComboBox texCoordIndexCB;
    }
}
