namespace Toolbox.Library.Forms.Editors
{
    partial class STColorEditor
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
            this.previewPictureBox = new System.Windows.Forms.PictureBox();
            this.timeUpDown = new System.Windows.Forms.NumericUpDown();
            this.colorLabel = new Toolbox.Library.Forms.STLabel();
            this.timeLabel = new Toolbox.Library.Forms.STLabel();
            this.hexTextBox = new Toolbox.Library.Forms.STTextBox();
            this.colorSelector = new Toolbox.Library.Forms.ColorSelector();
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // previewPictureBox
            // 
            this.previewPictureBox.Location = new System.Drawing.Point(189, 188);
            this.previewPictureBox.Name = "previewPictureBox";
            this.previewPictureBox.Size = new System.Drawing.Size(35, 35);
            this.previewPictureBox.TabIndex = 46;
            this.previewPictureBox.TabStop = false;
            // 
            // timeUpDown
            // 
            this.timeUpDown.DecimalPlaces = 5;
            this.timeUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.timeUpDown.Location = new System.Drawing.Point(3, 203);
            this.timeUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timeUpDown.Name = "timeUpDown";
            this.timeUpDown.Size = new System.Drawing.Size(84, 20);
            this.timeUpDown.TabIndex = 51;
            this.timeUpDown.ValueChanged += new System.EventHandler(this.TimeUpDown_ValueChanged);
            // 
            // colorLabel
            // 
            this.colorLabel.AutoSize = true;
            this.colorLabel.Location = new System.Drawing.Point(90, 187);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(31, 13);
            this.colorLabel.TabIndex = 50;
            this.colorLabel.Text = "Color";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(3, 187);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(30, 13);
            this.timeLabel.TabIndex = 49;
            this.timeLabel.Text = "Time";
            // 
            // hexTextBox
            // 
            this.hexTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hexTextBox.Location = new System.Drawing.Point(93, 203);
            this.hexTextBox.MaxLength = 8;
            this.hexTextBox.Name = "hexTextBox";
            this.hexTextBox.Size = new System.Drawing.Size(90, 20);
            this.hexTextBox.TabIndex = 47;
            this.hexTextBox.TextChanged += new System.EventHandler(this.HexTextBox_TextChanged);
            // 
            // colorSelector
            // 
            this.colorSelector.Alpha = 0;
            this.colorSelector.Color = System.Drawing.Color.Empty;
            this.colorSelector.DisplayAlpha = true;
            this.colorSelector.DisplayColor = true;
            this.colorSelector.Location = new System.Drawing.Point(0, 0);
            this.colorSelector.Name = "colorSelector";
            this.colorSelector.Size = new System.Drawing.Size(227, 188);
            this.colorSelector.TabIndex = 0;
            // 
            // STColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.timeUpDown);
            this.Controls.Add(this.colorLabel);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.hexTextBox);
            this.Controls.Add(this.previewPictureBox);
            this.Controls.Add(this.colorSelector);
            this.Name = "STColorEditor";
            this.Size = new System.Drawing.Size(227, 226);
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorSelector colorSelector;
        private System.Windows.Forms.PictureBox previewPictureBox;
        private STTextBox hexTextBox;
        private STLabel timeLabel;
        private STLabel colorLabel;
        private System.Windows.Forms.NumericUpDown timeUpDown;
    }
}
