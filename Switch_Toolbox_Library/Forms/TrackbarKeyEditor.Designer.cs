namespace TrackbarKeyEditor
{
    partial class TrackbarKeyEditor
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
            this.SuspendLayout();
            // 
            // ColorSlider
            // 
            this.Size = new System.Drawing.Size(200, 48);
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
         | System.Windows.Forms.AnchorStyles.Left)
         | System.Windows.Forms.AnchorStyles.Right)));
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.ForeColor = System.Drawing.Color.Silver;
            this.LargeChange = ((uint)(5u));
            this.Location = new System.Drawing.Point(157, 0);
            this.MouseEffects = false;
            this.Name = "colorSlider1";
            this.ScaleDivisions = 10;
            this.ScaleSubDivisions = 5;
            this.ShowDivisionsText = true;
            this.ShowSmallScale = true;
            this.Size = new System.Drawing.Size(876, 524);
            this.SmallChange = ((uint)(0u));
            this.TabIndex = 0;
            this.Text = "colorSlider1";
            this.ThumbInnerColor = System.Drawing.Color.Olive;
            this.ThumbOuterColor = System.Drawing.Color.Olive;
            this.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.ThumbSize = new System.Drawing.Size(5, 128);
            this.TickAdd = 0F;
            this.TickColor = System.Drawing.Color.Gray;
            this.TickDivide = 1F;
            this.Value = 0;
            this.ResumeLayout(false);

        }

        #endregion
    }
}
