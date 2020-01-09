namespace FirstPlugin.Forms
{
    partial class TexSrtPanel2D
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
            this.scaYUD = new BarSlider.BarSlider();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.scaXUD = new BarSlider.BarSlider();
            this.rotXUD = new BarSlider.BarSlider();
            this.transXUD = new BarSlider.BarSlider();
            this.transYUD = new BarSlider.BarSlider();
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            this.SuspendLayout();
            // 
            // scaYUD
            // 
            this.scaYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.scaYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.scaYUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.scaYUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.scaYUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.scaYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.scaYUD.DataType = null;
            this.scaYUD.DrawSemitransparentThumb = false;
            this.scaYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaYUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.scaYUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.scaYUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.scaYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.scaYUD.IncrementAmount = 0.01F;
            this.scaYUD.InputName = "Y";
            this.scaYUD.LargeChange = 5F;
            this.scaYUD.Location = new System.Drawing.Point(207, 29);
            this.scaYUD.Maximum = 100F;
            this.scaYUD.Minimum = 0F;
            this.scaYUD.Name = "scaYUD";
            this.scaYUD.Precision = 0.01F;
            this.scaYUD.ScaleDivisions = 1;
            this.scaYUD.ScaleSubDivisions = 2;
            this.scaYUD.ShowDivisionsText = false;
            this.scaYUD.ShowSmallScale = false;
            this.scaYUD.Size = new System.Drawing.Size(131, 25);
            this.scaYUD.SmallChange = 0.01F;
            this.scaYUD.TabIndex = 1;
            this.scaYUD.Text = "barSlider1";
            this.scaYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.scaYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.scaYUD.TickAdd = 0F;
            this.scaYUD.TickColor = System.Drawing.Color.White;
            this.scaYUD.TickDivide = 0F;
            this.scaYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.scaYUD.UseInterlapsedBar = false;
            this.scaYUD.Value = 30F;
            this.scaYUD.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(10, 29);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(37, 13);
            this.stLabel2.TabIndex = 2;
            this.stLabel2.Text = "Scale:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(10, 63);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(50, 13);
            this.stLabel3.TabIndex = 3;
            this.stLabel3.Text = "Rotation:";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(3, 97);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(62, 13);
            this.stLabel4.TabIndex = 4;
            this.stLabel4.Text = "Translation:";
            // 
            // scaXUD
            // 
            this.scaXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.scaXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.scaXUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.scaXUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.scaXUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.scaXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.scaXUD.DataType = null;
            this.scaXUD.DrawSemitransparentThumb = false;
            this.scaXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaXUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.scaXUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.scaXUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.scaXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.scaXUD.IncrementAmount = 0.01F;
            this.scaXUD.InputName = "X";
            this.scaXUD.LargeChange = 5F;
            this.scaXUD.Location = new System.Drawing.Point(70, 29);
            this.scaXUD.Maximum = 100F;
            this.scaXUD.Minimum = 0F;
            this.scaXUD.Name = "scaXUD";
            this.scaXUD.Precision = 0.01F;
            this.scaXUD.ScaleDivisions = 1;
            this.scaXUD.ScaleSubDivisions = 2;
            this.scaXUD.ShowDivisionsText = false;
            this.scaXUD.ShowSmallScale = false;
            this.scaXUD.Size = new System.Drawing.Size(131, 25);
            this.scaXUD.SmallChange = 0.01F;
            this.scaXUD.TabIndex = 5;
            this.scaXUD.Text = "barSlider2";
            this.scaXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.scaXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.scaXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.scaXUD.TickAdd = 0F;
            this.scaXUD.TickColor = System.Drawing.Color.White;
            this.scaXUD.TickDivide = 0F;
            this.scaXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.scaXUD.UseInterlapsedBar = false;
            this.scaXUD.Value = 30F;
            this.scaXUD.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // rotXUD
            // 
            this.rotXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rotXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.rotXUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.rotXUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.rotXUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.rotXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.rotXUD.DataType = null;
            this.rotXUD.DrawSemitransparentThumb = false;
            this.rotXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.rotXUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.rotXUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.rotXUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.rotXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.rotXUD.IncrementAmount = 0.01F;
            this.rotXUD.InputName = "X";
            this.rotXUD.LargeChange = 5F;
            this.rotXUD.Location = new System.Drawing.Point(70, 60);
            this.rotXUD.Maximum = 100F;
            this.rotXUD.Minimum = 0F;
            this.rotXUD.Name = "rotXUD";
            this.rotXUD.Precision = 0.01F;
            this.rotXUD.ScaleDivisions = 1;
            this.rotXUD.ScaleSubDivisions = 2;
            this.rotXUD.ShowDivisionsText = false;
            this.rotXUD.ShowSmallScale = false;
            this.rotXUD.Size = new System.Drawing.Size(131, 25);
            this.rotXUD.SmallChange = 0.01F;
            this.rotXUD.TabIndex = 7;
            this.rotXUD.Text = "barSlider3";
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
            this.rotXUD.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // transXUD
            // 
            this.transXUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.transXUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.transXUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.transXUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.transXUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.transXUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.transXUD.DataType = null;
            this.transXUD.DrawSemitransparentThumb = false;
            this.transXUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.transXUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.transXUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.transXUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.transXUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.transXUD.IncrementAmount = 0.01F;
            this.transXUD.InputName = "X";
            this.transXUD.LargeChange = 5F;
            this.transXUD.Location = new System.Drawing.Point(70, 91);
            this.transXUD.Maximum = 100F;
            this.transXUD.Minimum = 0F;
            this.transXUD.Name = "transXUD";
            this.transXUD.Precision = 0.01F;
            this.transXUD.ScaleDivisions = 1;
            this.transXUD.ScaleSubDivisions = 2;
            this.transXUD.ShowDivisionsText = false;
            this.transXUD.ShowSmallScale = false;
            this.transXUD.Size = new System.Drawing.Size(131, 25);
            this.transXUD.SmallChange = 0.01F;
            this.transXUD.TabIndex = 9;
            this.transXUD.Text = "barSlider5";
            this.transXUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.transXUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.transXUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.transXUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.transXUD.TickAdd = 0F;
            this.transXUD.TickColor = System.Drawing.Color.White;
            this.transXUD.TickDivide = 0F;
            this.transXUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.transXUD.UseInterlapsedBar = false;
            this.transXUD.Value = 30F;
            this.transXUD.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // transYUD
            // 
            this.transYUD.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.transYUD.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.transYUD.BarPenColorBottom = System.Drawing.Color.Empty;
            this.transYUD.BarPenColorMiddle = System.Drawing.Color.Empty;
            this.transYUD.BarPenColorTop = System.Drawing.Color.Empty;
            this.transYUD.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.transYUD.DataType = null;
            this.transYUD.DrawSemitransparentThumb = false;
            this.transYUD.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.transYUD.ElapsedPenColorBottom = System.Drawing.Color.Empty;
            this.transYUD.ElapsedPenColorMiddle = System.Drawing.Color.Empty;
            this.transYUD.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.transYUD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.transYUD.IncrementAmount = 0.01F;
            this.transYUD.InputName = "Y";
            this.transYUD.LargeChange = 5F;
            this.transYUD.Location = new System.Drawing.Point(207, 91);
            this.transYUD.Maximum = 100F;
            this.transYUD.Minimum = 0F;
            this.transYUD.Name = "transYUD";
            this.transYUD.Precision = 0.01F;
            this.transYUD.ScaleDivisions = 1;
            this.transYUD.ScaleSubDivisions = 2;
            this.transYUD.ShowDivisionsText = false;
            this.transYUD.ShowSmallScale = false;
            this.transYUD.Size = new System.Drawing.Size(131, 25);
            this.transYUD.SmallChange = 0.01F;
            this.transYUD.TabIndex = 8;
            this.transYUD.Text = "barSlider6";
            this.transYUD.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.transYUD.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.transYUD.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.transYUD.ThumbSize = new System.Drawing.Size(1, 1);
            this.transYUD.TickAdd = 0F;
            this.transYUD.TickColor = System.Drawing.Color.White;
            this.transYUD.TickDivide = 0F;
            this.transYUD.TickStyle = System.Windows.Forms.TickStyle.None;
            this.transYUD.UseInterlapsedBar = false;
            this.transYUD.Value = 30F;
            this.transYUD.ValueChanged += new System.EventHandler(this.barSlider_ValueChanged);
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(3, 3);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.ReadOnly = true;
            this.stTextBox1.Size = new System.Drawing.Size(332, 20);
            this.stTextBox1.TabIndex = 10;
            // 
            // TexSrtPanel2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTextBox1);
            this.Controls.Add(this.transXUD);
            this.Controls.Add(this.transYUD);
            this.Controls.Add(this.rotXUD);
            this.Controls.Add(this.scaXUD);
            this.Controls.Add(this.stLabel4);
            this.Controls.Add(this.stLabel3);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.scaYUD);
            this.Name = "TexSrtPanel2D";
            this.Size = new System.Drawing.Size(338, 120);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private BarSlider.BarSlider scaYUD;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private BarSlider.BarSlider scaXUD;
        private BarSlider.BarSlider rotXUD;
        private BarSlider.BarSlider transXUD;
        private BarSlider.BarSlider transYUD;
        private Toolbox.Library.Forms.STTextBox stTextBox1;
    }
}
