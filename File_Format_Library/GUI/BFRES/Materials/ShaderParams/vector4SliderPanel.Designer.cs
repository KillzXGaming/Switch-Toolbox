namespace FirstPlugin.Forms
{
    partial class vector4SliderPanel
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
            this.alphaPB = new System.Windows.Forms.PictureBox();
            this.colorPB = new System.Windows.Forms.PictureBox();
            this.barSlider4 = new BarSlider.BarSlider();
            this.barSlider3 = new BarSlider.BarSlider();
            this.barSlider2 = new BarSlider.BarSlider();
            this.barSlider1 = new BarSlider.BarSlider();
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.alphaPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).BeginInit();
            this.SuspendLayout();
            // 
            // alphaPB
            // 
            this.alphaPB.Location = new System.Drawing.Point(208, 29);
            this.alphaPB.Name = "alphaPB";
            this.alphaPB.Size = new System.Drawing.Size(71, 16);
            this.alphaPB.TabIndex = 6;
            this.alphaPB.TabStop = false;
            this.alphaPB.Click += new System.EventHandler(this.alphaPB_Click);
            // 
            // colorPB
            // 
            this.colorPB.Location = new System.Drawing.Point(141, 29);
            this.colorPB.Name = "colorPB";
            this.colorPB.Size = new System.Drawing.Size(63, 16);
            this.colorPB.TabIndex = 7;
            this.colorPB.TabStop = false;
            this.colorPB.Click += new System.EventHandler(this.colorPB_Click);
            // 
            // barSlider4
            // 
            this.barSlider4.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.barSlider4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider4.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider4.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.barSlider4.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.barSlider4.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider4.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider4.DataType = null;
            this.barSlider4.DrawSemitransparentThumb = false;
            this.barSlider4.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider4.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider4.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.barSlider4.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider4.ForeColor = System.Drawing.Color.White;
            this.barSlider4.IncrementAmount = 0.01F;
            this.barSlider4.InputName = "W";
            this.barSlider4.LargeChange = 5F;
            this.barSlider4.Location = new System.Drawing.Point(141, 77);
            this.barSlider4.Maximum = 100F;
            this.barSlider4.Minimum = 0F;
            this.barSlider4.Name = "barSlider4";
            this.barSlider4.Precision = 0.01F;
            this.barSlider4.ScaleDivisions = 1;
            this.barSlider4.ScaleSubDivisions = 2;
            this.barSlider4.ShowDivisionsText = false;
            this.barSlider4.ShowSmallScale = false;
            this.barSlider4.Size = new System.Drawing.Size(137, 25);
            this.barSlider4.SmallChange = 1F;
            this.barSlider4.TabIndex = 5;
            this.barSlider4.Text = "barSlider4";
            this.barSlider4.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider4.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider4.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider4.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider4.TickAdd = 0F;
            this.barSlider4.TickColor = System.Drawing.Color.White;
            this.barSlider4.TickDivide = 0F;
            this.barSlider4.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider4.UseInterlapsedBar = false;
            this.barSlider4.Value = 30F;
            this.barSlider4.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // barSlider3
            // 
            this.barSlider3.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.barSlider3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider3.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider3.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.barSlider3.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.barSlider3.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider3.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider3.DataType = null;
            this.barSlider3.DrawSemitransparentThumb = false;
            this.barSlider3.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider3.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider3.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.barSlider3.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider3.ForeColor = System.Drawing.Color.White;
            this.barSlider3.IncrementAmount = 0.01F;
            this.barSlider3.InputName = "Z";
            this.barSlider3.LargeChange = 5F;
            this.barSlider3.Location = new System.Drawing.Point(3, 77);
            this.barSlider3.Maximum = 100F;
            this.barSlider3.Minimum = 0F;
            this.barSlider3.Name = "barSlider3";
            this.barSlider3.Precision = 0.01F;
            this.barSlider3.ScaleDivisions = 1;
            this.barSlider3.ScaleSubDivisions = 2;
            this.barSlider3.ShowDivisionsText = false;
            this.barSlider3.ShowSmallScale = false;
            this.barSlider3.Size = new System.Drawing.Size(137, 25);
            this.barSlider3.SmallChange = 1F;
            this.barSlider3.TabIndex = 4;
            this.barSlider3.Text = "barSlider3";
            this.barSlider3.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider3.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider3.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider3.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider3.TickAdd = 0F;
            this.barSlider3.TickColor = System.Drawing.Color.White;
            this.barSlider3.TickDivide = 0F;
            this.barSlider3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider3.UseInterlapsedBar = false;
            this.barSlider3.Value = 30F;
            this.barSlider3.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // barSlider2
            // 
            this.barSlider2.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.barSlider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider2.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider2.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.barSlider2.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.barSlider2.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider2.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider2.DataType = null;
            this.barSlider2.DrawSemitransparentThumb = false;
            this.barSlider2.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider2.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider2.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.barSlider2.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider2.ForeColor = System.Drawing.Color.White;
            this.barSlider2.IncrementAmount = 0.01F;
            this.barSlider2.InputName = "Y";
            this.barSlider2.LargeChange = 5F;
            this.barSlider2.Location = new System.Drawing.Point(141, 51);
            this.barSlider2.Maximum = 100F;
            this.barSlider2.Minimum = 0F;
            this.barSlider2.Name = "barSlider2";
            this.barSlider2.Precision = 0.01F;
            this.barSlider2.ScaleDivisions = 1;
            this.barSlider2.ScaleSubDivisions = 2;
            this.barSlider2.ShowDivisionsText = false;
            this.barSlider2.ShowSmallScale = false;
            this.barSlider2.Size = new System.Drawing.Size(137, 25);
            this.barSlider2.SmallChange = 1F;
            this.barSlider2.TabIndex = 3;
            this.barSlider2.Text = "barSlider2";
            this.barSlider2.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider2.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider2.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider2.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider2.TickAdd = 0F;
            this.barSlider2.TickColor = System.Drawing.Color.White;
            this.barSlider2.TickDivide = 0F;
            this.barSlider2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider2.UseInterlapsedBar = false;
            this.barSlider2.Value = 30F;
            this.barSlider2.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // barSlider1
            // 
            this.barSlider1.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.barSlider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider1.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.barSlider1.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.barSlider1.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider1.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider1.DataType = null;
            this.barSlider1.DrawSemitransparentThumb = false;
            this.barSlider1.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.barSlider1.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.barSlider1.ForeColor = System.Drawing.Color.White;
            this.barSlider1.IncrementAmount = 0.01F;
            this.barSlider1.InputName = "X";
            this.barSlider1.LargeChange = 5F;
            this.barSlider1.Location = new System.Drawing.Point(3, 51);
            this.barSlider1.Maximum = 100F;
            this.barSlider1.Minimum = 0F;
            this.barSlider1.Name = "barSlider1";
            this.barSlider1.Precision = 0.01F;
            this.barSlider1.ScaleDivisions = 1;
            this.barSlider1.ScaleSubDivisions = 2;
            this.barSlider1.ShowDivisionsText = false;
            this.barSlider1.ShowSmallScale = false;
            this.barSlider1.Size = new System.Drawing.Size(137, 25);
            this.barSlider1.SmallChange = 1F;
            this.barSlider1.TabIndex = 1;
            this.barSlider1.Text = "barSlider1";
            this.barSlider1.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider1.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider1.TickAdd = 0F;
            this.barSlider1.TickColor = System.Drawing.Color.White;
            this.barSlider1.TickDivide = 0F;
            this.barSlider1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider1.UseInterlapsedBar = false;
            this.barSlider1.Value = 30F;
            this.barSlider1.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            this.barSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.barSlider1_Scroll);
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(3, 3);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.ReadOnly = true;
            this.stTextBox1.Size = new System.Drawing.Size(275, 20);
            this.stTextBox1.TabIndex = 8;
            // 
            // vector4SliderPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTextBox1);
            this.Controls.Add(this.colorPB);
            this.Controls.Add(this.alphaPB);
            this.Controls.Add(this.barSlider4);
            this.Controls.Add(this.barSlider3);
            this.Controls.Add(this.barSlider2);
            this.Controls.Add(this.barSlider1);
            this.Name = "vector4SliderPanel";
            this.Size = new System.Drawing.Size(282, 105);
            ((System.ComponentModel.ISupportInitialize)(this.alphaPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BarSlider.BarSlider barSlider1;
        private BarSlider.BarSlider barSlider2;
        private BarSlider.BarSlider barSlider3;
        private BarSlider.BarSlider barSlider4;
        private System.Windows.Forms.PictureBox alphaPB;
        private System.Windows.Forms.PictureBox colorPB;
        private Toolbox.Library.Forms.STTextBox stTextBox1;
    }
}
