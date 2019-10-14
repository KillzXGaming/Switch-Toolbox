namespace LayoutBXLYT
{
    partial class WindowContentEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowContentEditor));
            this.frameRightUD = new BarSlider.BarSlider();
            this.frameLeftUD = new BarSlider.BarSlider();
            this.frameDownUD = new BarSlider.BarSlider();
            this.frameUpUD = new BarSlider.BarSlider();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stDropDownPanel1 = new Toolbox.Library.Forms.STDropDownPanel();
            this.stDropDownPanel2 = new Toolbox.Library.Forms.STDropDownPanel();
            this.btnResetColors = new Toolbox.Library.Forms.STButton();
            this.vertexColorBox1 = new Toolbox.Library.Forms.VertexColorBox();
            this.chkUseVtxColorsOnFrames = new Toolbox.Library.Forms.STCheckBox();
            this.stDropDownPanel3 = new Toolbox.Library.Forms.STDropDownPanel();
            this.chkMaterialForAll = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.texRotateCB = new Toolbox.Library.Forms.STComboBox();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel1.SuspendLayout();
            this.stDropDownPanel2.SuspendLayout();
            this.stDropDownPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // frameRightUD
            // 
            this.frameRightUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameRightUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameRightUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.frameRightUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.frameRightUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.frameRightUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameRightUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.frameRightUD.DataType = null;
            this.frameRightUD.DrawSemitransparentThumb = false;
            this.frameRightUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameRightUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameRightUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.frameRightUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameRightUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frameRightUD.ForeColor = System.Drawing.Color.White;
            this.frameRightUD.IncrementAmount = 0.01F;
            this.frameRightUD.InputName = "Y";
            this.frameRightUD.LargeChange = 5F;
            this.frameRightUD.Location = new System.Drawing.Point(204, 60);
            this.frameRightUD.Maximum = 300000F;
            this.frameRightUD.Minimum = -300000F;
            this.frameRightUD.Name = "frameRightUD";
            this.frameRightUD.Precision = 0.01F;
            this.frameRightUD.ScaleDivisions = 1;
            this.frameRightUD.ScaleSubDivisions = 2;
            this.frameRightUD.ShowDivisionsText = false;
            this.frameRightUD.ShowSmallScale = false;
            this.frameRightUD.Size = new System.Drawing.Size(107, 25);
            this.frameRightUD.SmallChange = 0.01F;
            this.frameRightUD.TabIndex = 55;
            this.frameRightUD.Text = "sizeXUD";
            this.frameRightUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameRightUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameRightUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.frameRightUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.frameRightUD.TickAdd = 0F;
            this.frameRightUD.TickColor = System.Drawing.Color.White;
            this.frameRightUD.TickDivide = 0F;
            this.frameRightUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.frameRightUD.UseInterlapsedBar = false;
            this.frameRightUD.Value = 30F;
            this.frameRightUD.ValueChanged += new System.EventHandler(this.frameUD_ValueChanged);
            // 
            // frameLeftUD
            // 
            this.frameLeftUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameLeftUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameLeftUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.frameLeftUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.frameLeftUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.frameLeftUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameLeftUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.frameLeftUD.DataType = null;
            this.frameLeftUD.DrawSemitransparentThumb = false;
            this.frameLeftUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameLeftUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameLeftUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.frameLeftUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameLeftUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frameLeftUD.ForeColor = System.Drawing.Color.White;
            this.frameLeftUD.IncrementAmount = 0.01F;
            this.frameLeftUD.InputName = "Y";
            this.frameLeftUD.LargeChange = 5F;
            this.frameLeftUD.Location = new System.Drawing.Point(204, 29);
            this.frameLeftUD.Maximum = 300000F;
            this.frameLeftUD.Minimum = -300000F;
            this.frameLeftUD.Name = "frameLeftUD";
            this.frameLeftUD.Precision = 0.01F;
            this.frameLeftUD.ScaleDivisions = 1;
            this.frameLeftUD.ScaleSubDivisions = 2;
            this.frameLeftUD.ShowDivisionsText = false;
            this.frameLeftUD.ShowSmallScale = false;
            this.frameLeftUD.Size = new System.Drawing.Size(107, 25);
            this.frameLeftUD.SmallChange = 0.01F;
            this.frameLeftUD.TabIndex = 54;
            this.frameLeftUD.Text = "barSlider2";
            this.frameLeftUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameLeftUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameLeftUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.frameLeftUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.frameLeftUD.TickAdd = 0F;
            this.frameLeftUD.TickColor = System.Drawing.Color.White;
            this.frameLeftUD.TickDivide = 0F;
            this.frameLeftUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.frameLeftUD.UseInterlapsedBar = false;
            this.frameLeftUD.Value = 30F;
            this.frameLeftUD.ValueChanged += new System.EventHandler(this.frameUD_ValueChanged);
            // 
            // frameDownUD
            // 
            this.frameDownUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameDownUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameDownUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.frameDownUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.frameDownUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.frameDownUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameDownUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.frameDownUD.DataType = null;
            this.frameDownUD.DrawSemitransparentThumb = false;
            this.frameDownUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameDownUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameDownUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.frameDownUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameDownUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frameDownUD.ForeColor = System.Drawing.Color.White;
            this.frameDownUD.IncrementAmount = 0.01F;
            this.frameDownUD.InputName = "X";
            this.frameDownUD.LargeChange = 5F;
            this.frameDownUD.Location = new System.Drawing.Point(42, 60);
            this.frameDownUD.Maximum = 300000F;
            this.frameDownUD.Minimum = -300000F;
            this.frameDownUD.Name = "frameDownUD";
            this.frameDownUD.Precision = 0.01F;
            this.frameDownUD.ScaleDivisions = 1;
            this.frameDownUD.ScaleSubDivisions = 2;
            this.frameDownUD.ShowDivisionsText = false;
            this.frameDownUD.ShowSmallScale = false;
            this.frameDownUD.Size = new System.Drawing.Size(107, 25);
            this.frameDownUD.SmallChange = 0.01F;
            this.frameDownUD.TabIndex = 53;
            this.frameDownUD.Text = "sizeXUD";
            this.frameDownUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameDownUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameDownUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.frameDownUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.frameDownUD.TickAdd = 0F;
            this.frameDownUD.TickColor = System.Drawing.Color.White;
            this.frameDownUD.TickDivide = 0F;
            this.frameDownUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.frameDownUD.UseInterlapsedBar = false;
            this.frameDownUD.Value = 30F;
            this.frameDownUD.ValueChanged += new System.EventHandler(this.frameUD_ValueChanged);
            // 
            // frameUpUD
            // 
            this.frameUpUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameUpUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameUpUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.frameUpUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.frameUpUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.frameUpUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.frameUpUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.frameUpUD.DataType = null;
            this.frameUpUD.DrawSemitransparentThumb = false;
            this.frameUpUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameUpUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameUpUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.frameUpUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameUpUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frameUpUD.ForeColor = System.Drawing.Color.White;
            this.frameUpUD.IncrementAmount = 0.01F;
            this.frameUpUD.InputName = "X";
            this.frameUpUD.LargeChange = 5F;
            this.frameUpUD.Location = new System.Drawing.Point(42, 29);
            this.frameUpUD.Maximum = 300000F;
            this.frameUpUD.Minimum = -300000F;
            this.frameUpUD.Name = "frameUpUD";
            this.frameUpUD.Precision = 0.01F;
            this.frameUpUD.ScaleDivisions = 1;
            this.frameUpUD.ScaleSubDivisions = 2;
            this.frameUpUD.ShowDivisionsText = false;
            this.frameUpUD.ShowSmallScale = false;
            this.frameUpUD.Size = new System.Drawing.Size(107, 25);
            this.frameUpUD.SmallChange = 0.01F;
            this.frameUpUD.TabIndex = 52;
            this.frameUpUD.Text = "barSlider2";
            this.frameUpUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameUpUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.frameUpUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.frameUpUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.frameUpUD.TickAdd = 0F;
            this.frameUpUD.TickColor = System.Drawing.Color.White;
            this.frameUpUD.TickDivide = 0F;
            this.frameUpUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.frameUpUD.UseInterlapsedBar = false;
            this.frameUpUD.Value = 30F;
            this.frameUpUD.ValueChanged += new System.EventHandler(this.frameUD_ValueChanged);
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(-2, 63);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(38, 13);
            this.stLabel5.TabIndex = 51;
            this.stLabel5.Text = "Down:";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(-2, 33);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(24, 13);
            this.stLabel4.TabIndex = 50;
            this.stLabel4.Text = "Up:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(164, 67);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(35, 13);
            this.stLabel1.TabIndex = 57;
            this.stLabel1.Text = "Right:";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(164, 37);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(28, 13);
            this.stLabel2.TabIndex = 56;
            this.stLabel2.Text = "Left:";
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel2);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel3);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(353, 458);
            this.stFlowLayoutPanel1.TabIndex = 58;
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.stLabel1);
            this.stDropDownPanel1.Controls.Add(this.frameRightUD);
            this.stDropDownPanel1.Controls.Add(this.stLabel2);
            this.stDropDownPanel1.Controls.Add(this.stLabel4);
            this.stDropDownPanel1.Controls.Add(this.stLabel5);
            this.stDropDownPanel1.Controls.Add(this.frameLeftUD);
            this.stDropDownPanel1.Controls.Add(this.frameUpUD);
            this.stDropDownPanel1.Controls.Add(this.frameDownUD);
            this.stDropDownPanel1.ExpandedHeight = 0;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Frame Sizes";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(353, 103);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // stDropDownPanel2
            // 
            this.stDropDownPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel2.Controls.Add(this.btnResetColors);
            this.stDropDownPanel2.Controls.Add(this.vertexColorBox1);
            this.stDropDownPanel2.Controls.Add(this.chkUseVtxColorsOnFrames);
            this.stDropDownPanel2.ExpandedHeight = 0;
            this.stDropDownPanel2.IsExpanded = true;
            this.stDropDownPanel2.Location = new System.Drawing.Point(0, 103);
            this.stDropDownPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel2.Name = "stDropDownPanel2";
            this.stDropDownPanel2.PanelName = "Vertex Colors";
            this.stDropDownPanel2.PanelValueName = "";
            this.stDropDownPanel2.SetIcon = null;
            this.stDropDownPanel2.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.Size = new System.Drawing.Size(353, 147);
            this.stDropDownPanel2.TabIndex = 1;
            // 
            // btnResetColors
            // 
            this.btnResetColors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetColors.Location = new System.Drawing.Point(148, 30);
            this.btnResetColors.Name = "btnResetColors";
            this.btnResetColors.Size = new System.Drawing.Size(94, 23);
            this.btnResetColors.TabIndex = 4;
            this.btnResetColors.Text = "Reset Colors";
            this.btnResetColors.UseVisualStyleBackColor = false;
            this.btnResetColors.Click += new System.EventHandler(this.btnResetColors_Click);
            // 
            // vertexColorBox1
            // 
            this.vertexColorBox1.BackColor = System.Drawing.Color.Transparent;
            this.vertexColorBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("vertexColorBox1.BackgroundImage")));
            this.vertexColorBox1.BottomLeftColor = System.Drawing.Color.Empty;
            this.vertexColorBox1.BottomRightColor = System.Drawing.Color.Empty;
            this.vertexColorBox1.Location = new System.Drawing.Point(42, 30);
            this.vertexColorBox1.Name = "vertexColorBox1";
            this.vertexColorBox1.Size = new System.Drawing.Size(100, 100);
            this.vertexColorBox1.TabIndex = 3;
            this.vertexColorBox1.TopLeftColor = System.Drawing.Color.Empty;
            this.vertexColorBox1.TopRightColor = System.Drawing.Color.Empty;
            // 
            // chkUseVtxColorsOnFrames
            // 
            this.chkUseVtxColorsOnFrames.AutoSize = true;
            this.chkUseVtxColorsOnFrames.Location = new System.Drawing.Point(148, 113);
            this.chkUseVtxColorsOnFrames.Name = "chkUseVtxColorsOnFrames";
            this.chkUseVtxColorsOnFrames.Size = new System.Drawing.Size(99, 17);
            this.chkUseVtxColorsOnFrames.TabIndex = 1;
            this.chkUseVtxColorsOnFrames.Text = "Use On Frames";
            this.chkUseVtxColorsOnFrames.UseVisualStyleBackColor = true;
            // 
            // stDropDownPanel3
            // 
            this.stDropDownPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel3.Controls.Add(this.chkMaterialForAll);
            this.stDropDownPanel3.Controls.Add(this.stLabel3);
            this.stDropDownPanel3.Controls.Add(this.texRotateCB);
            this.stDropDownPanel3.ExpandedHeight = 0;
            this.stDropDownPanel3.IsExpanded = true;
            this.stDropDownPanel3.Location = new System.Drawing.Point(0, 250);
            this.stDropDownPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel3.Name = "stDropDownPanel3";
            this.stDropDownPanel3.PanelName = "Material Settings";
            this.stDropDownPanel3.PanelValueName = "";
            this.stDropDownPanel3.SetIcon = null;
            this.stDropDownPanel3.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.Size = new System.Drawing.Size(353, 116);
            this.stDropDownPanel3.TabIndex = 2;
            // 
            // chkMaterialForAll
            // 
            this.chkMaterialForAll.AutoSize = true;
            this.chkMaterialForAll.Location = new System.Drawing.Point(19, 70);
            this.chkMaterialForAll.Name = "chkMaterialForAll";
            this.chkMaterialForAll.Size = new System.Drawing.Size(142, 17);
            this.chkMaterialForAll.TabIndex = 3;
            this.chkMaterialForAll.Text = "Share Top Left Materials";
            this.chkMaterialForAll.UseVisualStyleBackColor = true;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(16, 37);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(92, 13);
            this.stLabel3.TabIndex = 2;
            this.stLabel3.Text = "Texture Rotation::";
            // 
            // texRotateCB
            // 
            this.texRotateCB.BorderColor = System.Drawing.Color.Empty;
            this.texRotateCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.texRotateCB.ButtonColor = System.Drawing.Color.Empty;
            this.texRotateCB.FormattingEnabled = true;
            this.texRotateCB.IsReadOnly = false;
            this.texRotateCB.Location = new System.Drawing.Point(114, 34);
            this.texRotateCB.Name = "texRotateCB";
            this.texRotateCB.Size = new System.Drawing.Size(121, 21);
            this.texRotateCB.TabIndex = 1;
            this.texRotateCB.SelectedIndexChanged += new System.EventHandler(this.texRotateCB_SelectedIndexChanged);
            // 
            // WindowContentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "WindowContentEditor";
            this.Size = new System.Drawing.Size(353, 458);
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stDropDownPanel2.ResumeLayout(false);
            this.stDropDownPanel2.PerformLayout();
            this.stDropDownPanel3.ResumeLayout(false);
            this.stDropDownPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private BarSlider.BarSlider frameRightUD;
        private BarSlider.BarSlider frameLeftUD;
        private BarSlider.BarSlider frameDownUD;
        private BarSlider.BarSlider frameUpUD;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel2;
        private Toolbox.Library.Forms.STCheckBox chkUseVtxColorsOnFrames;
        private Toolbox.Library.Forms.STButton btnResetColors;
        private Toolbox.Library.Forms.VertexColorBox vertexColorBox1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel3;
        private Toolbox.Library.Forms.STCheckBox chkMaterialForAll;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STComboBox texRotateCB;
    }
}
