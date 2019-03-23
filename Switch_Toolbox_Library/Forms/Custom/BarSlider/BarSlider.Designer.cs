namespace BarSlider
{
    partial class BarSlider
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
            this.BackColor = System.Drawing.Color.Empty;
            this.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.DrawSemitransparentThumb = false;
            this.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.LargeChange = 5F;
            this.Location = new System.Drawing.Point(10, 0);
            this.Maximum = 5F;
            this.Minimum = 0F;
            this.Name = "colorSlider2";
            this.Precision = 0.01F;
            this.ScaleDivisions = 1;
            this.ScaleSubDivisions = 2;
            this.ShowDivisionsText = false;
            this.ShowSmallScale = false;
            this.Size = new System.Drawing.Size(278, 25);
            this.SmallChange = 1F;
            this.TabIndex = 1;
            this.Text = "colorSlider2";
            this.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.ThumbSize = new System.Drawing.Size(1, 1);
            this.TickAdd = 0F;
            this.TickColor = System.Drawing.Color.White;
            this.TickDivide = 0F;
            this.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Value = 5F;
            this.ResumeLayout(false);
        }

        #endregion
    }
}
