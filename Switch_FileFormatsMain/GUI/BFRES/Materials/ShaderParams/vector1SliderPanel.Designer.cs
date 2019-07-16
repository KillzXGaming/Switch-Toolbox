namespace FirstPlugin.Forms
{
    partial class vector1SliderPanel
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
            this.barSlider1 = new BarSlider.BarSlider();
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            this.SuspendLayout();
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
            this.barSlider1.ForeColor = this.ForeColor;
            this.barSlider1.IncrementAmount = 0.01F;
            this.barSlider1.InputName = "X";
            this.barSlider1.LargeChange = 5F;
            this.barSlider1.Location = new System.Drawing.Point(3, 29);
            this.barSlider1.Maximum = 300000F;
            this.barSlider1.Minimum = -300000F;
            this.barSlider1.Name = "barSlider1";
            this.barSlider1.Precision = 0.01F;
            this.barSlider1.ScaleDivisions = 1;
            this.barSlider1.ScaleSubDivisions = 2;
            this.barSlider1.ShowDivisionsText = false;
            this.barSlider1.ShowSmallScale = false;
            this.barSlider1.Size = new System.Drawing.Size(137, 25);
            this.barSlider1.SmallChange = 0.01F;
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
            this.barSlider1.ValueChanged += new System.EventHandler(this.barSlider1_ValueChanged);
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(3, 3);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.ReadOnly = true;
            this.stTextBox1.Size = new System.Drawing.Size(276, 20);
            this.stTextBox1.TabIndex = 2;
            // 
            // vector1SliderPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTextBox1);
            this.Controls.Add(this.barSlider1);
            this.Name = "vector1SliderPanel";
            this.Size = new System.Drawing.Size(282, 57);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BarSlider.BarSlider barSlider1;
        private Toolbox.Library.Forms.STTextBox stTextBox1;
    }
}
