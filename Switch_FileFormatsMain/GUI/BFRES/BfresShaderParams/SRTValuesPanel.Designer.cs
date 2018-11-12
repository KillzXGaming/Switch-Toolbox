namespace FirstPlugin
{
    partial class SRTValuesPanel
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
            this.modeLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.scaleUDX = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.rotUDX = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.transUDX = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.transUDY = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.rotUDY = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.scaleUDY = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.transUDZ = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.rotUDZ = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.scaleUDZ = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.matrixPtrNumUD = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.matrixPtrLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.scaleUDX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotUDX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transUDX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transUDY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotUDY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleUDY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transUDZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotUDZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleUDZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixPtrNumUD)).BeginInit();
            this.SuspendLayout();
            // 
            // modeLabel
            // 
            this.modeLabel.AutoSize = true;
            this.modeLabel.Location = new System.Drawing.Point(12, 126);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(37, 13);
            this.modeLabel.TabIndex = 0;
            this.modeLabel.Text = "Mode:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Scaling:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Rotation:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Translation";
            // 
            // scaleUDX
            // 
            this.scaleUDX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.scaleUDX.DecimalPlaces = 5;
            this.scaleUDX.ForeColor = System.Drawing.Color.White;
            this.scaleUDX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.scaleUDX.Location = new System.Drawing.Point(89, 10);
            this.scaleUDX.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleUDX.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.scaleUDX.Name = "scaleUDX";
            this.scaleUDX.Size = new System.Drawing.Size(120, 20);
            this.scaleUDX.TabIndex = 5;
            this.scaleUDX.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // rotUDX
            // 
            this.rotUDX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rotUDX.DecimalPlaces = 5;
            this.rotUDX.ForeColor = System.Drawing.Color.White;
            this.rotUDX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.rotUDX.Location = new System.Drawing.Point(89, 47);
            this.rotUDX.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.rotUDX.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.rotUDX.Name = "rotUDX";
            this.rotUDX.Size = new System.Drawing.Size(120, 20);
            this.rotUDX.TabIndex = 6;
            this.rotUDX.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // transUDX
            // 
            this.transUDX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.transUDX.DecimalPlaces = 5;
            this.transUDX.ForeColor = System.Drawing.Color.White;
            this.transUDX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.transUDX.Location = new System.Drawing.Point(89, 84);
            this.transUDX.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.transUDX.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.transUDX.Name = "transUDX";
            this.transUDX.Size = new System.Drawing.Size(120, 20);
            this.transUDX.TabIndex = 7;
            this.transUDX.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // transUDY
            // 
            this.transUDY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.transUDY.DecimalPlaces = 5;
            this.transUDY.ForeColor = System.Drawing.Color.White;
            this.transUDY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.transUDY.Location = new System.Drawing.Point(215, 84);
            this.transUDY.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.transUDY.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.transUDY.Name = "transUDY";
            this.transUDY.Size = new System.Drawing.Size(120, 20);
            this.transUDY.TabIndex = 11;
            this.transUDY.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // rotUDY
            // 
            this.rotUDY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rotUDY.DecimalPlaces = 5;
            this.rotUDY.ForeColor = System.Drawing.Color.White;
            this.rotUDY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.rotUDY.Location = new System.Drawing.Point(215, 47);
            this.rotUDY.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.rotUDY.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.rotUDY.Name = "rotUDY";
            this.rotUDY.Size = new System.Drawing.Size(120, 20);
            this.rotUDY.TabIndex = 10;
            this.rotUDY.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // scaleUDY
            // 
            this.scaleUDY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.scaleUDY.DecimalPlaces = 5;
            this.scaleUDY.ForeColor = System.Drawing.Color.White;
            this.scaleUDY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.scaleUDY.Location = new System.Drawing.Point(215, 10);
            this.scaleUDY.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleUDY.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.scaleUDY.Name = "scaleUDY";
            this.scaleUDY.Size = new System.Drawing.Size(120, 20);
            this.scaleUDY.TabIndex = 9;
            this.scaleUDY.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // transUDZ
            // 
            this.transUDZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.transUDZ.DecimalPlaces = 5;
            this.transUDZ.ForeColor = System.Drawing.Color.White;
            this.transUDZ.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.transUDZ.Location = new System.Drawing.Point(341, 84);
            this.transUDZ.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.transUDZ.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.transUDZ.Name = "transUDZ";
            this.transUDZ.Size = new System.Drawing.Size(120, 20);
            this.transUDZ.TabIndex = 15;
            this.transUDZ.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // rotUDZ
            // 
            this.rotUDZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rotUDZ.DecimalPlaces = 5;
            this.rotUDZ.ForeColor = System.Drawing.Color.White;
            this.rotUDZ.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.rotUDZ.Location = new System.Drawing.Point(341, 47);
            this.rotUDZ.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.rotUDZ.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.rotUDZ.Name = "rotUDZ";
            this.rotUDZ.Size = new System.Drawing.Size(120, 20);
            this.rotUDZ.TabIndex = 14;
            this.rotUDZ.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // scaleUDZ
            // 
            this.scaleUDZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.scaleUDZ.DecimalPlaces = 5;
            this.scaleUDZ.ForeColor = System.Drawing.Color.White;
            this.scaleUDZ.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.scaleUDZ.Location = new System.Drawing.Point(341, 10);
            this.scaleUDZ.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleUDZ.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.scaleUDZ.Name = "scaleUDZ";
            this.scaleUDZ.Size = new System.Drawing.Size(120, 20);
            this.scaleUDZ.TabIndex = 13;
            this.scaleUDZ.ValueChanged += new System.EventHandler(this.UD_ValueChanged);
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(83, 123);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(126, 21);
            this.modeComboBox.TabIndex = 16;
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.modeComboBox_SelectedIndexChanged);
            // 
            // matrixPtrNumUD
            // 
            this.matrixPtrNumUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.matrixPtrNumUD.DecimalPlaces = 5;
            this.matrixPtrNumUD.ForeColor = System.Drawing.Color.White;
            this.matrixPtrNumUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.matrixPtrNumUD.Location = new System.Drawing.Point(88, 162);
            this.matrixPtrNumUD.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.matrixPtrNumUD.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.matrixPtrNumUD.Name = "matrixPtrNumUD";
            this.matrixPtrNumUD.Size = new System.Drawing.Size(120, 20);
            this.matrixPtrNumUD.TabIndex = 18;
            // 
            // matrixPtrLabel
            // 
            this.matrixPtrLabel.AutoSize = true;
            this.matrixPtrLabel.Location = new System.Drawing.Point(11, 164);
            this.matrixPtrLabel.Name = "matrixPtrLabel";
            this.matrixPtrLabel.Size = new System.Drawing.Size(71, 13);
            this.matrixPtrLabel.TabIndex = 17;
            this.matrixPtrLabel.Text = "Matrix Pointer";
            // 
            // SRTValuesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.matrixPtrNumUD);
            this.Controls.Add(this.matrixPtrLabel);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.transUDZ);
            this.Controls.Add(this.rotUDZ);
            this.Controls.Add(this.scaleUDZ);
            this.Controls.Add(this.transUDY);
            this.Controls.Add(this.rotUDY);
            this.Controls.Add(this.scaleUDY);
            this.Controls.Add(this.transUDX);
            this.Controls.Add(this.rotUDX);
            this.Controls.Add(this.scaleUDX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.modeLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "SRTValuesPanel";
            this.Size = new System.Drawing.Size(469, 205);
            ((System.ComponentModel.ISupportInitialize)(this.scaleUDX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotUDX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transUDX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transUDY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotUDY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleUDY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transUDZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotUDZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleUDZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixPtrNumUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat scaleUDX;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat rotUDX;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat transUDX;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat transUDY;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat rotUDY;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat scaleUDY;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat transUDZ;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat rotUDZ;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat scaleUDZ;
        private  Switch_Toolbox.Library.Forms.NumericUpDownFloat matrixPtrNumUD;
        private System.Windows.Forms.Label matrixPtrLabel;
        private System.Windows.Forms.ComboBox modeComboBox;
    }
}
