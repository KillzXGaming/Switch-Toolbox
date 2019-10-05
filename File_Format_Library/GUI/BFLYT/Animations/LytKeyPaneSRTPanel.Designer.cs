namespace LayoutBXLYT
{
    partial class LytKeyPanePanel
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
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.tranXUD = new BarSlider.BarSlider();
            this.rotXUD = new BarSlider.BarSlider();
            this.scaleXUD = new BarSlider.BarSlider();
            this.sizeXUD = new BarSlider.BarSlider();
            this.sizeYUD = new BarSlider.BarSlider();
            this.scaleYUD = new BarSlider.BarSlider();
            this.rotYUD = new BarSlider.BarSlider();
            this.tranYUD = new BarSlider.BarSlider();
            this.rotZUD = new BarSlider.BarSlider();
            this.tranZUD = new BarSlider.BarSlider();
            this.SuspendLayout();
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(13, 30);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(54, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Translate:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(14, 58);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(42, 13);
            this.stLabel3.TabIndex = 4;
            this.stLabel3.Text = "Rotate:";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(14, 91);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(37, 13);
            this.stLabel4.TabIndex = 5;
            this.stLabel4.Text = "Scale:";
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(14, 121);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(30, 13);
            this.stLabel5.TabIndex = 6;
            this.stLabel5.Text = "Size:";
            // 
            // tranXUD
            // 
            this.tranXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tranXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tranXUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.tranXUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tranXUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.tranXUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tranXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.tranXUD.DataType = null;
            this.tranXUD.DrawSemitransparentThumb = false;
            this.tranXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranXUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranXUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.tranXUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.tranXUD.ForeColor = System.Drawing.Color.White;
            this.tranXUD.IncrementAmount = 0.01F;
            this.tranXUD.InputName = "X";
            this.tranXUD.LargeChange = 5F;
            this.tranXUD.Location = new System.Drawing.Point(100, 24);
            this.tranXUD.Maximum = 300000F;
            this.tranXUD.Minimum = -300000F;
            this.tranXUD.Name = "tranXUD";
            this.tranXUD.Precision = 0.01F;
            this.tranXUD.ScaleDivisions = 1;
            this.tranXUD.ScaleSubDivisions = 2;
            this.tranXUD.ShowDivisionsText = false;
            this.tranXUD.ShowSmallScale = false;
            this.tranXUD.Size = new System.Drawing.Size(107, 25);
            this.tranXUD.SmallChange = 0.01F;
            this.tranXUD.TabIndex = 28;
            this.tranXUD.Text = "barSlider2";
            this.tranXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.tranXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.tranXUD.TickAdd = 0F;
            this.tranXUD.TickColor = System.Drawing.Color.White;
            this.tranXUD.TickDivide = 0F;
            this.tranXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tranXUD.UseInterlapsedBar = false;
            this.tranXUD.Value = 30F;
            // 
            // rotXUD
            // 
            this.rotXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rotXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rotXUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.rotXUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.rotXUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.rotXUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rotXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.rotXUD.DataType = null;
            this.rotXUD.DrawSemitransparentThumb = false;
            this.rotXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotXUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotXUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.rotXUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.rotXUD.ForeColor = System.Drawing.Color.White;
            this.rotXUD.IncrementAmount = 0.01F;
            this.rotXUD.InputName = "X";
            this.rotXUD.LargeChange = 5F;
            this.rotXUD.Location = new System.Drawing.Point(100, 55);
            this.rotXUD.Maximum = 300000F;
            this.rotXUD.Minimum = -300000F;
            this.rotXUD.Name = "rotXUD";
            this.rotXUD.Precision = 0.01F;
            this.rotXUD.ScaleDivisions = 1;
            this.rotXUD.ScaleSubDivisions = 2;
            this.rotXUD.ShowDivisionsText = false;
            this.rotXUD.ShowSmallScale = false;
            this.rotXUD.Size = new System.Drawing.Size(107, 25);
            this.rotXUD.SmallChange = 0.01F;
            this.rotXUD.TabIndex = 29;
            this.rotXUD.Text = "rotXUD";
            this.rotXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.rotXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.rotXUD.TickAdd = 0F;
            this.rotXUD.TickColor = System.Drawing.Color.White;
            this.rotXUD.TickDivide = 0F;
            this.rotXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.rotXUD.UseInterlapsedBar = false;
            this.rotXUD.Value = 30F;
            // 
            // scaleXUD
            // 
            this.scaleXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.scaleXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.scaleXUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.scaleXUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.scaleXUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.scaleXUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.scaleXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.scaleXUD.DataType = null;
            this.scaleXUD.DrawSemitransparentThumb = false;
            this.scaleXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleXUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleXUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.scaleXUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.scaleXUD.ForeColor = System.Drawing.Color.White;
            this.scaleXUD.IncrementAmount = 0.01F;
            this.scaleXUD.InputName = "X";
            this.scaleXUD.LargeChange = 5F;
            this.scaleXUD.Location = new System.Drawing.Point(100, 86);
            this.scaleXUD.Maximum = 300000F;
            this.scaleXUD.Minimum = -300000F;
            this.scaleXUD.Name = "scaleXUD";
            this.scaleXUD.Precision = 0.01F;
            this.scaleXUD.ScaleDivisions = 1;
            this.scaleXUD.ScaleSubDivisions = 2;
            this.scaleXUD.ShowDivisionsText = false;
            this.scaleXUD.ShowSmallScale = false;
            this.scaleXUD.Size = new System.Drawing.Size(107, 25);
            this.scaleXUD.SmallChange = 0.01F;
            this.scaleXUD.TabIndex = 30;
            this.scaleXUD.Text = "barSlider2";
            this.scaleXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.scaleXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.scaleXUD.TickAdd = 0F;
            this.scaleXUD.TickColor = System.Drawing.Color.White;
            this.scaleXUD.TickDivide = 0F;
            this.scaleXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.scaleXUD.UseInterlapsedBar = false;
            this.scaleXUD.Value = 30F;
            // 
            // sizeXUD
            // 
            this.sizeXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.sizeXUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sizeXUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.sizeXUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.sizeXUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.sizeXUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.sizeXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.sizeXUD.DataType = null;
            this.sizeXUD.DrawSemitransparentThumb = false;
            this.sizeXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeXUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeXUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.sizeXUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.sizeXUD.ForeColor = System.Drawing.Color.White;
            this.sizeXUD.IncrementAmount = 0.01F;
            this.sizeXUD.InputName = "X";
            this.sizeXUD.LargeChange = 5F;
            this.sizeXUD.Location = new System.Drawing.Point(100, 117);
            this.sizeXUD.Maximum = 300000F;
            this.sizeXUD.Minimum = -300000F;
            this.sizeXUD.Name = "sizeXUD";
            this.sizeXUD.Precision = 0.01F;
            this.sizeXUD.ScaleDivisions = 1;
            this.sizeXUD.ScaleSubDivisions = 2;
            this.sizeXUD.ShowDivisionsText = false;
            this.sizeXUD.ShowSmallScale = false;
            this.sizeXUD.Size = new System.Drawing.Size(107, 25);
            this.sizeXUD.SmallChange = 0.01F;
            this.sizeXUD.TabIndex = 31;
            this.sizeXUD.Text = "sizeXUD";
            this.sizeXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.sizeXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.sizeXUD.TickAdd = 0F;
            this.sizeXUD.TickColor = System.Drawing.Color.White;
            this.sizeXUD.TickDivide = 0F;
            this.sizeXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.sizeXUD.UseInterlapsedBar = false;
            this.sizeXUD.Value = 30F;
            // 
            // sizeYUD
            // 
            this.sizeYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.sizeYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sizeYUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.sizeYUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.sizeYUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.sizeYUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.sizeYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.sizeYUD.DataType = null;
            this.sizeYUD.DrawSemitransparentThumb = false;
            this.sizeYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeYUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeYUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.sizeYUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.sizeYUD.ForeColor = System.Drawing.Color.White;
            this.sizeYUD.IncrementAmount = 0.01F;
            this.sizeYUD.InputName = "Y";
            this.sizeYUD.LargeChange = 5F;
            this.sizeYUD.Location = new System.Drawing.Point(213, 117);
            this.sizeYUD.Maximum = 300000F;
            this.sizeYUD.Minimum = -300000F;
            this.sizeYUD.Name = "sizeYUD";
            this.sizeYUD.Precision = 0.01F;
            this.sizeYUD.ScaleDivisions = 1;
            this.sizeYUD.ScaleSubDivisions = 2;
            this.sizeYUD.ShowDivisionsText = false;
            this.sizeYUD.ShowSmallScale = false;
            this.sizeYUD.Size = new System.Drawing.Size(107, 25);
            this.sizeYUD.SmallChange = 0.01F;
            this.sizeYUD.TabIndex = 35;
            this.sizeYUD.Text = "sizeXUD";
            this.sizeYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.sizeYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.sizeYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.sizeYUD.TickAdd = 0F;
            this.sizeYUD.TickColor = System.Drawing.Color.White;
            this.sizeYUD.TickDivide = 0F;
            this.sizeYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.sizeYUD.UseInterlapsedBar = false;
            this.sizeYUD.Value = 30F;
            // 
            // scaleYUD
            // 
            this.scaleYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.scaleYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.scaleYUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.scaleYUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.scaleYUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.scaleYUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.scaleYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.scaleYUD.DataType = null;
            this.scaleYUD.DrawSemitransparentThumb = false;
            this.scaleYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleYUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleYUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.scaleYUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.scaleYUD.ForeColor = System.Drawing.Color.White;
            this.scaleYUD.IncrementAmount = 0.01F;
            this.scaleYUD.InputName = "Y";
            this.scaleYUD.LargeChange = 5F;
            this.scaleYUD.Location = new System.Drawing.Point(213, 86);
            this.scaleYUD.Maximum = 300000F;
            this.scaleYUD.Minimum = -300000F;
            this.scaleYUD.Name = "scaleYUD";
            this.scaleYUD.Precision = 0.01F;
            this.scaleYUD.ScaleDivisions = 1;
            this.scaleYUD.ScaleSubDivisions = 2;
            this.scaleYUD.ShowDivisionsText = false;
            this.scaleYUD.ShowSmallScale = false;
            this.scaleYUD.Size = new System.Drawing.Size(107, 25);
            this.scaleYUD.SmallChange = 0.01F;
            this.scaleYUD.TabIndex = 34;
            this.scaleYUD.Text = "barSlider2";
            this.scaleYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaleYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.scaleYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.scaleYUD.TickAdd = 0F;
            this.scaleYUD.TickColor = System.Drawing.Color.White;
            this.scaleYUD.TickDivide = 0F;
            this.scaleYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.scaleYUD.UseInterlapsedBar = false;
            this.scaleYUD.Value = 30F;
            // 
            // rotYUD
            // 
            this.rotYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rotYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rotYUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.rotYUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.rotYUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.rotYUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rotYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.rotYUD.DataType = null;
            this.rotYUD.DrawSemitransparentThumb = false;
            this.rotYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotYUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotYUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.rotYUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.rotYUD.ForeColor = System.Drawing.Color.White;
            this.rotYUD.IncrementAmount = 0.01F;
            this.rotYUD.InputName = "Y";
            this.rotYUD.LargeChange = 5F;
            this.rotYUD.Location = new System.Drawing.Point(213, 55);
            this.rotYUD.Maximum = 300000F;
            this.rotYUD.Minimum = -300000F;
            this.rotYUD.Name = "rotYUD";
            this.rotYUD.Precision = 0.01F;
            this.rotYUD.ScaleDivisions = 1;
            this.rotYUD.ScaleSubDivisions = 2;
            this.rotYUD.ShowDivisionsText = false;
            this.rotYUD.ShowSmallScale = false;
            this.rotYUD.Size = new System.Drawing.Size(107, 25);
            this.rotYUD.SmallChange = 0.01F;
            this.rotYUD.TabIndex = 33;
            this.rotYUD.Text = "rotXUD";
            this.rotYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.rotYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.rotYUD.TickAdd = 0F;
            this.rotYUD.TickColor = System.Drawing.Color.White;
            this.rotYUD.TickDivide = 0F;
            this.rotYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.rotYUD.UseInterlapsedBar = false;
            this.rotYUD.Value = 30F;
            // 
            // tranYUD
            // 
            this.tranYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tranYUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tranYUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.tranYUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tranYUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.tranYUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tranYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.tranYUD.DataType = null;
            this.tranYUD.DrawSemitransparentThumb = false;
            this.tranYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranYUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranYUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.tranYUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.tranYUD.ForeColor = System.Drawing.Color.White;
            this.tranYUD.IncrementAmount = 0.01F;
            this.tranYUD.InputName = "Y";
            this.tranYUD.LargeChange = 5F;
            this.tranYUD.Location = new System.Drawing.Point(213, 24);
            this.tranYUD.Maximum = 300000F;
            this.tranYUD.Minimum = -300000F;
            this.tranYUD.Name = "tranYUD";
            this.tranYUD.Precision = 0.01F;
            this.tranYUD.ScaleDivisions = 1;
            this.tranYUD.ScaleSubDivisions = 2;
            this.tranYUD.ShowDivisionsText = false;
            this.tranYUD.ShowSmallScale = false;
            this.tranYUD.Size = new System.Drawing.Size(107, 25);
            this.tranYUD.SmallChange = 0.01F;
            this.tranYUD.TabIndex = 32;
            this.tranYUD.Text = "barSlider2";
            this.tranYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.tranYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.tranYUD.TickAdd = 0F;
            this.tranYUD.TickColor = System.Drawing.Color.White;
            this.tranYUD.TickDivide = 0F;
            this.tranYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tranYUD.UseInterlapsedBar = false;
            this.tranYUD.Value = 30F;
            // 
            // rotZUD
            // 
            this.rotZUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rotZUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rotZUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotZUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.rotZUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.rotZUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.rotZUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rotZUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.rotZUD.DataType = null;
            this.rotZUD.DrawSemitransparentThumb = false;
            this.rotZUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotZUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotZUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.rotZUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotZUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.rotZUD.ForeColor = System.Drawing.Color.White;
            this.rotZUD.IncrementAmount = 0.01F;
            this.rotZUD.InputName = "Z";
            this.rotZUD.LargeChange = 5F;
            this.rotZUD.Location = new System.Drawing.Point(326, 55);
            this.rotZUD.Maximum = 300000F;
            this.rotZUD.Minimum = -300000F;
            this.rotZUD.Name = "rotZUD";
            this.rotZUD.Precision = 0.01F;
            this.rotZUD.ScaleDivisions = 1;
            this.rotZUD.ScaleSubDivisions = 2;
            this.rotZUD.ShowDivisionsText = false;
            this.rotZUD.ShowSmallScale = false;
            this.rotZUD.Size = new System.Drawing.Size(107, 25);
            this.rotZUD.SmallChange = 0.01F;
            this.rotZUD.TabIndex = 37;
            this.rotZUD.Text = "rotXUD";
            this.rotZUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotZUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotZUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.rotZUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.rotZUD.TickAdd = 0F;
            this.rotZUD.TickColor = System.Drawing.Color.White;
            this.rotZUD.TickDivide = 0F;
            this.rotZUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.rotZUD.UseInterlapsedBar = false;
            this.rotZUD.Value = 30F;
            // 
            // tranZUD
            // 
            this.tranZUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tranZUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tranZUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranZUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.tranZUD.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tranZUD.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.tranZUD.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tranZUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.tranZUD.DataType = null;
            this.tranZUD.DrawSemitransparentThumb = false;
            this.tranZUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranZUD.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranZUD.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.tranZUD.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranZUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.tranZUD.ForeColor = System.Drawing.Color.White;
            this.tranZUD.IncrementAmount = 0.01F;
            this.tranZUD.InputName = "Z";
            this.tranZUD.LargeChange = 5F;
            this.tranZUD.Location = new System.Drawing.Point(326, 24);
            this.tranZUD.Maximum = 300000F;
            this.tranZUD.Minimum = -300000F;
            this.tranZUD.Name = "tranZUD";
            this.tranZUD.Precision = 0.01F;
            this.tranZUD.ScaleDivisions = 1;
            this.tranZUD.ScaleSubDivisions = 2;
            this.tranZUD.ShowDivisionsText = false;
            this.tranZUD.ShowSmallScale = false;
            this.tranZUD.Size = new System.Drawing.Size(107, 25);
            this.tranZUD.SmallChange = 0.01F;
            this.tranZUD.TabIndex = 36;
            this.tranZUD.Text = "barSlider2";
            this.tranZUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranZUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tranZUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.tranZUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.tranZUD.TickAdd = 0F;
            this.tranZUD.TickColor = System.Drawing.Color.White;
            this.tranZUD.TickDivide = 0F;
            this.tranZUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tranZUD.UseInterlapsedBar = false;
            this.tranZUD.Value = 30F;
            // 
            // LytKeyPanePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rotZUD);
            this.Controls.Add(this.tranZUD);
            this.Controls.Add(this.sizeYUD);
            this.Controls.Add(this.scaleYUD);
            this.Controls.Add(this.rotYUD);
            this.Controls.Add(this.tranYUD);
            this.Controls.Add(this.sizeXUD);
            this.Controls.Add(this.scaleXUD);
            this.Controls.Add(this.rotXUD);
            this.Controls.Add(this.tranXUD);
            this.Controls.Add(this.stLabel5);
            this.Controls.Add(this.stLabel4);
            this.Controls.Add(this.stLabel3);
            this.Controls.Add(this.stLabel1);
            this.Name = "LytKeyPanePanel";
            this.Size = new System.Drawing.Size(457, 153);
            this.Load += new System.EventHandler(this.LytKeyPaneSRTPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private BarSlider.BarSlider tranXUD;
        private BarSlider.BarSlider rotXUD;
        private BarSlider.BarSlider scaleXUD;
        private BarSlider.BarSlider sizeXUD;
        private BarSlider.BarSlider sizeYUD;
        private BarSlider.BarSlider scaleYUD;
        private BarSlider.BarSlider rotYUD;
        private BarSlider.BarSlider tranYUD;
        private BarSlider.BarSlider rotZUD;
        private BarSlider.BarSlider tranZUD;
    }
}
