namespace FirstPlugin.Forms
{
    partial class ColorArrayEditor
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
            this.colorArrayLabel = new Toolbox.Library.Forms.STLabel();
            this.keyCountUpDown = new Toolbox.Library.Forms.NumericUpDownUint();
            this.keyCountLabel = new Toolbox.Library.Forms.STLabel();
            this.animatedCheckBox = new Toolbox.Library.Forms.STCheckBox();
            this.panel = new Toolbox.Library.Forms.STPanel();
            ((System.ComponentModel.ISupportInitialize)(this.keyCountUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // colorArrayLabel
            // 
            this.colorArrayLabel.AutoSize = true;
            this.colorArrayLabel.Location = new System.Drawing.Point(4, 4);
            this.colorArrayLabel.Name = "colorArrayLabel";
            this.colorArrayLabel.Size = new System.Drawing.Size(47, 13);
            this.colorArrayLabel.TabIndex = 0;
            this.colorArrayLabel.Text = "stLabel1";
            // 
            // keyCountUpDown
            // 
            this.keyCountUpDown.Location = new System.Drawing.Point(276, 2);
            this.keyCountUpDown.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.keyCountUpDown.Name = "keyCountUpDown";
            this.keyCountUpDown.Size = new System.Drawing.Size(29, 20);
            this.keyCountUpDown.TabIndex = 1;
            this.keyCountUpDown.ValueChanged += new System.EventHandler(this.KeyCountUpDown_ValueChanged);
            // 
            // keyCountLabel
            // 
            this.keyCountLabel.AutoSize = true;
            this.keyCountLabel.Location = new System.Drawing.Point(237, 4);
            this.keyCountLabel.Name = "keyCountLabel";
            this.keyCountLabel.Size = new System.Drawing.Size(33, 13);
            this.keyCountLabel.TabIndex = 2;
            this.keyCountLabel.Text = "Keys:";
            // 
            // animatedCheckBox
            // 
            this.animatedCheckBox.AutoSize = true;
            this.animatedCheckBox.Location = new System.Drawing.Point(115, 3);
            this.animatedCheckBox.Name = "animatedCheckBox";
            this.animatedCheckBox.Size = new System.Drawing.Size(70, 17);
            this.animatedCheckBox.TabIndex = 3;
            this.animatedCheckBox.Text = "Animated";
            this.animatedCheckBox.UseVisualStyleBackColor = true;
            this.animatedCheckBox.CheckedChanged += new System.EventHandler(this.AnimatedCheckBox_CheckedChanged);
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(3, 20);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(304, 33);
            this.panel.TabIndex = 39;
            // 
            // ColorArrayEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.animatedCheckBox);
            this.Controls.Add(this.keyCountLabel);
            this.Controls.Add(this.keyCountUpDown);
            this.Controls.Add(this.colorArrayLabel);
            this.Name = "ColorArrayEditor";
            this.Size = new System.Drawing.Size(308, 59);
            ((System.ComponentModel.ISupportInitialize)(this.keyCountUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STLabel colorArrayLabel;
        private Toolbox.Library.Forms.NumericUpDownUint keyCountUpDown;
        private Toolbox.Library.Forms.STLabel keyCountLabel;
        private Toolbox.Library.Forms.STCheckBox animatedCheckBox;
        private Toolbox.Library.Forms.STPanel panel;
    }
}
