namespace FirstPlugin.Forms
{
    partial class GFLXMaterialParamEditor
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
            this.stCheckBox1 = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.switchParamCB = new Toolbox.Library.Forms.STComboBox();
            this.stDropDownPanel2 = new Toolbox.Library.Forms.STDropDownPanel();
            this.barSlider1 = new BarSlider.BarSlider();
            this.valueParamCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stLabel6 = new Toolbox.Library.Forms.STLabel();
            this.stDropDownPanel3 = new Toolbox.Library.Forms.STDropDownPanel();
            this.barSlider4 = new BarSlider.BarSlider();
            this.barSlider3 = new BarSlider.BarSlider();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.barSlider2 = new BarSlider.BarSlider();
            this.colorParamCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel8 = new Toolbox.Library.Forms.STLabel();
            this.stLabel9 = new Toolbox.Library.Forms.STLabel();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel1.SuspendLayout();
            this.stDropDownPanel2.SuspendLayout();
            this.stDropDownPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
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
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(395, 392);
            this.stFlowLayoutPanel1.TabIndex = 0;
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.stCheckBox1);
            this.stDropDownPanel1.Controls.Add(this.stLabel2);
            this.stDropDownPanel1.Controls.Add(this.stLabel1);
            this.stDropDownPanel1.Controls.Add(this.switchParamCB);
            this.stDropDownPanel1.ExpandedHeight = 0;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Switch Params";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel1.Size = new System.Drawing.Size(395, 106);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // stCheckBox1
            // 
            this.stCheckBox1.AutoSize = true;
            this.stCheckBox1.Location = new System.Drawing.Point(58, 72);
            this.stCheckBox1.Name = "stCheckBox1";
            this.stCheckBox1.Size = new System.Drawing.Size(15, 14);
            this.stCheckBox1.TabIndex = 5;
            this.stCheckBox1.UseVisualStyleBackColor = true;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(15, 72);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(37, 13);
            this.stLabel2.TabIndex = 4;
            this.stLabel2.Text = "Value:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(12, 33);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(40, 13);
            this.stLabel1.TabIndex = 2;
            this.stLabel1.Text = "Param:";
            // 
            // switchParamCB
            // 
            this.switchParamCB.BorderColor = System.Drawing.Color.Empty;
            this.switchParamCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.switchParamCB.ButtonColor = System.Drawing.Color.Empty;
            this.switchParamCB.FormattingEnabled = true;
            this.switchParamCB.IsReadOnly = false;
            this.switchParamCB.Location = new System.Drawing.Point(58, 30);
            this.switchParamCB.Name = "switchParamCB";
            this.switchParamCB.Size = new System.Drawing.Size(284, 21);
            this.switchParamCB.TabIndex = 1;
            this.switchParamCB.SelectedIndexChanged += new System.EventHandler(this.switchParamCB_SelectedIndexChanged);
            // 
            // stDropDownPanel2
            // 
            this.stDropDownPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel2.Controls.Add(this.barSlider1);
            this.stDropDownPanel2.Controls.Add(this.valueParamCB);
            this.stDropDownPanel2.Controls.Add(this.stLabel5);
            this.stDropDownPanel2.Controls.Add(this.stLabel6);
            this.stDropDownPanel2.ExpandedHeight = 0;
            this.stDropDownPanel2.IsExpanded = true;
            this.stDropDownPanel2.Location = new System.Drawing.Point(0, 106);
            this.stDropDownPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel2.Name = "stDropDownPanel2";
            this.stDropDownPanel2.PanelName = "Value Params";
            this.stDropDownPanel2.PanelValueName = "";
            this.stDropDownPanel2.SetIcon = null;
            this.stDropDownPanel2.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel2.Size = new System.Drawing.Size(395, 102);
            this.stDropDownPanel2.TabIndex = 1;
            // 
            // barSlider1
            // 
            this.barSlider1.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider1.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider1.BarPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider1.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider1.BarPenColorTop = System.Drawing.Color.Empty;
            this.barSlider1.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider1.DataType = null;
            this.barSlider1.DrawSemitransparentThumb = false;
            this.barSlider1.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.barSlider1.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider1.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider1.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.barSlider1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider1.IncrementAmount = 0.01F;
            this.barSlider1.InputName = null;
            this.barSlider1.LargeChange = 5F;
            this.barSlider1.Location = new System.Drawing.Point(58, 65);
            this.barSlider1.Maximum = 100F;
            this.barSlider1.Minimum = 0F;
            this.barSlider1.Name = "barSlider1";
            this.barSlider1.Precision = 0.01F;
            this.barSlider1.ScaleDivisions = 1;
            this.barSlider1.ScaleSubDivisions = 2;
            this.barSlider1.ShowDivisionsText = false;
            this.barSlider1.ShowSmallScale = false;
            this.barSlider1.Size = new System.Drawing.Size(95, 25);
            this.barSlider1.SmallChange = 1F;
            this.barSlider1.TabIndex = 1;
            this.barSlider1.Text = "barSlider1";
            this.barSlider1.ThumbInnerColor = System.Drawing.Color.Empty;
            this.barSlider1.ThumbPenColor = System.Drawing.Color.Empty;
            this.barSlider1.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider1.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider1.TickAdd = 0F;
            this.barSlider1.TickColor = System.Drawing.Color.White;
            this.barSlider1.TickDivide = 0F;
            this.barSlider1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider1.UseInterlapsedBar = true;
            this.barSlider1.Value = 30F;
            // 
            // valueParamCB
            // 
            this.valueParamCB.BorderColor = System.Drawing.Color.Empty;
            this.valueParamCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.valueParamCB.ButtonColor = System.Drawing.Color.Empty;
            this.valueParamCB.FormattingEnabled = true;
            this.valueParamCB.IsReadOnly = false;
            this.valueParamCB.Location = new System.Drawing.Point(58, 27);
            this.valueParamCB.Name = "valueParamCB";
            this.valueParamCB.Size = new System.Drawing.Size(284, 21);
            this.valueParamCB.TabIndex = 7;
            this.valueParamCB.SelectedIndexChanged += new System.EventHandler(this.valueParamCB_SelectedIndexChanged);
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(15, 68);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(37, 13);
            this.stLabel5.TabIndex = 10;
            this.stLabel5.Text = "Value:";
            // 
            // stLabel6
            // 
            this.stLabel6.AutoSize = true;
            this.stLabel6.Location = new System.Drawing.Point(12, 30);
            this.stLabel6.Name = "stLabel6";
            this.stLabel6.Size = new System.Drawing.Size(40, 13);
            this.stLabel6.TabIndex = 8;
            this.stLabel6.Text = "Param:";
            // 
            // stDropDownPanel3
            // 
            this.stDropDownPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel3.Controls.Add(this.barSlider4);
            this.stDropDownPanel3.Controls.Add(this.barSlider3);
            this.stDropDownPanel3.Controls.Add(this.pictureBox1);
            this.stDropDownPanel3.Controls.Add(this.barSlider2);
            this.stDropDownPanel3.Controls.Add(this.colorParamCB);
            this.stDropDownPanel3.Controls.Add(this.stLabel8);
            this.stDropDownPanel3.Controls.Add(this.stLabel9);
            this.stDropDownPanel3.ExpandedHeight = 0;
            this.stDropDownPanel3.IsExpanded = true;
            this.stDropDownPanel3.Location = new System.Drawing.Point(0, 208);
            this.stDropDownPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel3.Name = "stDropDownPanel3";
            this.stDropDownPanel3.PanelName = "Color Params";
            this.stDropDownPanel3.PanelValueName = "";
            this.stDropDownPanel3.SetIcon = null;
            this.stDropDownPanel3.SetIconAlphaColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.SetIconColor = System.Drawing.SystemColors.Control;
            this.stDropDownPanel3.Size = new System.Drawing.Size(395, 161);
            this.stDropDownPanel3.TabIndex = 2;
            // 
            // barSlider4
            // 
            this.barSlider4.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider4.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider4.BarPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider4.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider4.BarPenColorTop = System.Drawing.Color.Empty;
            this.barSlider4.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider4.DataType = null;
            this.barSlider4.DrawSemitransparentThumb = false;
            this.barSlider4.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.barSlider4.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider4.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider4.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.barSlider4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider4.IncrementAmount = 0.01F;
            this.barSlider4.InputName = null;
            this.barSlider4.LargeChange = 5F;
            this.barSlider4.Location = new System.Drawing.Point(260, 76);
            this.barSlider4.Maximum = 100F;
            this.barSlider4.Minimum = 0F;
            this.barSlider4.Name = "barSlider4";
            this.barSlider4.Precision = 0.01F;
            this.barSlider4.ScaleDivisions = 1;
            this.barSlider4.ScaleSubDivisions = 2;
            this.barSlider4.ShowDivisionsText = false;
            this.barSlider4.ShowSmallScale = false;
            this.barSlider4.Size = new System.Drawing.Size(95, 25);
            this.barSlider4.SmallChange = 1F;
            this.barSlider4.TabIndex = 21;
            this.barSlider4.Text = "barSlider4";
            this.barSlider4.ThumbInnerColor = System.Drawing.Color.Empty;
            this.barSlider4.ThumbPenColor = System.Drawing.Color.Empty;
            this.barSlider4.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider4.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider4.TickAdd = 0F;
            this.barSlider4.TickColor = System.Drawing.Color.White;
            this.barSlider4.TickDivide = 0F;
            this.barSlider4.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider4.UseInterlapsedBar = true;
            this.barSlider4.Value = 30F;
            // 
            // barSlider3
            // 
            this.barSlider3.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider3.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider3.BarPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider3.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider3.BarPenColorTop = System.Drawing.Color.Empty;
            this.barSlider3.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider3.DataType = null;
            this.barSlider3.DrawSemitransparentThumb = false;
            this.barSlider3.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.barSlider3.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider3.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider3.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.barSlider3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider3.IncrementAmount = 0.01F;
            this.barSlider3.InputName = null;
            this.barSlider3.LargeChange = 5F;
            this.barSlider3.Location = new System.Drawing.Point(159, 76);
            this.barSlider3.Maximum = 100F;
            this.barSlider3.Minimum = 0F;
            this.barSlider3.Name = "barSlider3";
            this.barSlider3.Precision = 0.01F;
            this.barSlider3.ScaleDivisions = 1;
            this.barSlider3.ScaleSubDivisions = 2;
            this.barSlider3.ShowDivisionsText = false;
            this.barSlider3.ShowSmallScale = false;
            this.barSlider3.Size = new System.Drawing.Size(95, 25);
            this.barSlider3.SmallChange = 1F;
            this.barSlider3.TabIndex = 20;
            this.barSlider3.Text = "barSlider3";
            this.barSlider3.ThumbInnerColor = System.Drawing.Color.Empty;
            this.barSlider3.ThumbPenColor = System.Drawing.Color.Empty;
            this.barSlider3.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider3.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider3.TickAdd = 0F;
            this.barSlider3.TickColor = System.Drawing.Color.White;
            this.barSlider3.TickDivide = 0F;
            this.barSlider3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider3.UseInterlapsedBar = true;
            this.barSlider3.Value = 30F;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(59, 107);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(283, 41);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // barSlider2
            // 
            this.barSlider2.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider2.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider2.BarPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider2.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider2.BarPenColorTop = System.Drawing.Color.Empty;
            this.barSlider2.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider2.DataType = null;
            this.barSlider2.DrawSemitransparentThumb = false;
            this.barSlider2.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.barSlider2.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.barSlider2.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.barSlider2.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.barSlider2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider2.IncrementAmount = 0.01F;
            this.barSlider2.InputName = null;
            this.barSlider2.LargeChange = 5F;
            this.barSlider2.Location = new System.Drawing.Point(58, 76);
            this.barSlider2.Maximum = 100F;
            this.barSlider2.Minimum = 0F;
            this.barSlider2.Name = "barSlider2";
            this.barSlider2.Precision = 0.01F;
            this.barSlider2.ScaleDivisions = 1;
            this.barSlider2.ScaleSubDivisions = 2;
            this.barSlider2.ShowDivisionsText = false;
            this.barSlider2.ShowSmallScale = false;
            this.barSlider2.Size = new System.Drawing.Size(95, 25);
            this.barSlider2.SmallChange = 1F;
            this.barSlider2.TabIndex = 13;
            this.barSlider2.Text = "barSlider2";
            this.barSlider2.ThumbInnerColor = System.Drawing.Color.Empty;
            this.barSlider2.ThumbPenColor = System.Drawing.Color.Empty;
            this.barSlider2.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider2.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider2.TickAdd = 0F;
            this.barSlider2.TickColor = System.Drawing.Color.White;
            this.barSlider2.TickDivide = 0F;
            this.barSlider2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider2.UseInterlapsedBar = true;
            this.barSlider2.Value = 30F;
            // 
            // colorParamCB
            // 
            this.colorParamCB.BorderColor = System.Drawing.Color.Empty;
            this.colorParamCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.colorParamCB.ButtonColor = System.Drawing.Color.Empty;
            this.colorParamCB.FormattingEnabled = true;
            this.colorParamCB.IsReadOnly = false;
            this.colorParamCB.Location = new System.Drawing.Point(58, 32);
            this.colorParamCB.Name = "colorParamCB";
            this.colorParamCB.Size = new System.Drawing.Size(284, 21);
            this.colorParamCB.TabIndex = 14;
            this.colorParamCB.SelectedIndexChanged += new System.EventHandler(this.colorParamCB_SelectedIndexChanged);
            // 
            // stLabel8
            // 
            this.stLabel8.AutoSize = true;
            this.stLabel8.Location = new System.Drawing.Point(15, 79);
            this.stLabel8.Name = "stLabel8";
            this.stLabel8.Size = new System.Drawing.Size(37, 13);
            this.stLabel8.TabIndex = 17;
            this.stLabel8.Text = "Value:";
            // 
            // stLabel9
            // 
            this.stLabel9.AutoSize = true;
            this.stLabel9.Location = new System.Drawing.Point(12, 35);
            this.stLabel9.Name = "stLabel9";
            this.stLabel9.Size = new System.Drawing.Size(40, 13);
            this.stLabel9.TabIndex = 15;
            this.stLabel9.Text = "Param:";
            // 
            // GFLXMaterialParamEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "GFLXMaterialParamEditor";
            this.Size = new System.Drawing.Size(395, 392);
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stDropDownPanel2.ResumeLayout(false);
            this.stDropDownPanel2.PerformLayout();
            this.stDropDownPanel3.ResumeLayout(false);
            this.stDropDownPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.STCheckBox stCheckBox1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STComboBox switchParamCB;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel2;
        private BarSlider.BarSlider barSlider1;
        private Toolbox.Library.Forms.STComboBox valueParamCB;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.STLabel stLabel6;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel3;
        private BarSlider.BarSlider barSlider4;
        private BarSlider.BarSlider barSlider3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private BarSlider.BarSlider barSlider2;
        private Toolbox.Library.Forms.STComboBox colorParamCB;
        private Toolbox.Library.Forms.STLabel stLabel8;
        private Toolbox.Library.Forms.STLabel stLabel9;
    }
}
