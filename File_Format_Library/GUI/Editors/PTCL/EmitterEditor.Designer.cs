namespace FirstPlugin.Forms
{
    partial class EmitterEditor
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
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPageData = new System.Windows.Forms.TabPage();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.constantsLabel = new System.Windows.Forms.Label();
            this.colorEditor = new Toolbox.Library.Forms.Editors.STColorEditor();
            this.scaleArrayEditor = new ColorArrayEditor();
            this.constant1Panel = new ColorConstantPanel();
            this.constant0Panel = new ColorConstantPanel();
            this.alpha1ArrayEditor = new ColorArrayEditor();
            this.color1ArrayEditor = new ColorArrayEditor();
            this.alpha0ArrayEditor = new ColorArrayEditor();
            this.color0ArrayEditor = new ColorArrayEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.blinkIntensity0UpDown = new System.Windows.Forms.NumericUpDown();
            this.blinkDuration0UpDown = new System.Windows.Forms.NumericUpDown();
            this.blinkIntensity0Label = new System.Windows.Forms.Label();
            this.blinkDuration0Label = new System.Windows.Forms.Label();
            this.blinkDuration1Label = new System.Windows.Forms.Label();
            this.blinkIntensity1Label = new System.Windows.Forms.Label();
            this.blinkDuration1UpDown = new System.Windows.Forms.NumericUpDown();
            this.blinkIntensity1UpDown = new System.Windows.Forms.NumericUpDown();
            this.sizeUpDown = new System.Windows.Forms.NumericUpDown();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.stTabControl1.SuspendLayout();
            this.tabPageData.SuspendLayout();
            this.stPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blinkIntensity0UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blinkDuration0UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blinkDuration1UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blinkIntensity1UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPageData);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl1.Location = new System.Drawing.Point(0, 0);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(576, 561);
            this.stTabControl1.TabIndex = 38;
            // 
            // tabPageData
            // 
            this.tabPageData.Controls.Add(this.stPanel2);
            this.tabPageData.Location = new System.Drawing.Point(4, 25);
            this.tabPageData.Name = "tabPageData";
            this.tabPageData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageData.Size = new System.Drawing.Size(568, 532);
            this.tabPageData.TabIndex = 0;
            this.tabPageData.Text = "Emitter Data";
            this.tabPageData.UseVisualStyleBackColor = true;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.sizeLabel);
            this.stPanel2.Controls.Add(this.sizeUpDown);
            this.stPanel2.Controls.Add(this.blinkDuration1Label);
            this.stPanel2.Controls.Add(this.blinkIntensity1Label);
            this.stPanel2.Controls.Add(this.blinkDuration1UpDown);
            this.stPanel2.Controls.Add(this.blinkIntensity1UpDown);
            this.stPanel2.Controls.Add(this.blinkDuration0Label);
            this.stPanel2.Controls.Add(this.blinkIntensity0Label);
            this.stPanel2.Controls.Add(this.blinkDuration0UpDown);
            this.stPanel2.Controls.Add(this.blinkIntensity0UpDown);
            this.stPanel2.Controls.Add(this.label1);
            this.stPanel2.Controls.Add(this.constantsLabel);
            this.stPanel2.Controls.Add(this.scaleArrayEditor);
            this.stPanel2.Controls.Add(this.constant1Panel);
            this.stPanel2.Controls.Add(this.constant0Panel);
            this.stPanel2.Controls.Add(this.colorEditor);
            this.stPanel2.Controls.Add(this.alpha1ArrayEditor);
            this.stPanel2.Controls.Add(this.color1ArrayEditor);
            this.stPanel2.Controls.Add(this.alpha0ArrayEditor);
            this.stPanel2.Controls.Add(this.color0ArrayEditor);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(3, 3);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(562, 526);
            this.stPanel2.TabIndex = 0;
            // 
            // constantsLabel
            // 
            this.constantsLabel.AutoSize = true;
            this.constantsLabel.Location = new System.Drawing.Point(3, 325);
            this.constantsLabel.Name = "constantsLabel";
            this.constantsLabel.Size = new System.Drawing.Size(70, 13);
            this.constantsLabel.TabIndex = 67;
            this.constantsLabel.Text = "Const Color 0";
            // 
            // colorEditor
            // 
            this.colorEditor.Location = new System.Drawing.Point(317, 3);
            this.colorEditor.Name = "colorEditor";
            this.colorEditor.Size = new System.Drawing.Size(227, 226);
            this.colorEditor.TabIndex = 63;
            // 
            // scaleArrayEditor
            // 
            this.scaleArrayEditor.Label = "Scale";
            this.scaleArrayEditor.Location = new System.Drawing.Point(3, 263);
            this.scaleArrayEditor.Name = "scaleArrayEditor";
            this.scaleArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.scaleArrayEditor.TabIndex = 66;
            // 
            // constant1Panel
            // 
            this.constant1Panel.IsAlpha = false;
            this.constant1Panel.Location = new System.Drawing.Point(3, 397);
            this.constant1Panel.Name = "constant1Panel";
            this.constant1Panel.Size = new System.Drawing.Size(291, 37);
            this.constant1Panel.TabIndex = 65;
            // 
            // constant0Panel
            // 
            this.constant0Panel.IsAlpha = false;
            this.constant0Panel.Location = new System.Drawing.Point(3, 341);
            this.constant0Panel.Name = "constant0Panel";
            this.constant0Panel.Size = new System.Drawing.Size(291, 37);
            this.constant0Panel.TabIndex = 64;
            // 
            // alpha1ArrayEditor
            // 
            this.alpha1ArrayEditor.Label = "Alpha 1";
            this.alpha1ArrayEditor.Location = new System.Drawing.Point(3, 198);
            this.alpha1ArrayEditor.Name = "alpha1ArrayEditor";
            this.alpha1ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.alpha1ArrayEditor.TabIndex = 62;
            // 
            // color1ArrayEditor
            // 
            this.color1ArrayEditor.Label = "Color 1";
            this.color1ArrayEditor.Location = new System.Drawing.Point(3, 133);
            this.color1ArrayEditor.Name = "color1ArrayEditor";
            this.color1ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.color1ArrayEditor.TabIndex = 61;
            // 
            // alpha0ArrayEditor
            // 
            this.alpha0ArrayEditor.Label = "Alpha 0";
            this.alpha0ArrayEditor.Location = new System.Drawing.Point(3, 68);
            this.alpha0ArrayEditor.Name = "alpha0ArrayEditor";
            this.alpha0ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.alpha0ArrayEditor.TabIndex = 60;
            // 
            // color0ArrayEditor
            // 
            this.color0ArrayEditor.Label = "Color 0";
            this.color0ArrayEditor.Location = new System.Drawing.Point(3, 3);
            this.color0ArrayEditor.Name = "color0ArrayEditor";
            this.color0ArrayEditor.Size = new System.Drawing.Size(308, 59);
            this.color0ArrayEditor.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 381);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 68;
            this.label1.Text = "Const Color 1";
            // 
            // blinkIntensity0UpDown
            // 
            this.blinkIntensity0UpDown.DecimalPlaces = 5;
            this.blinkIntensity0UpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.blinkIntensity0UpDown.Location = new System.Drawing.Point(191, 333);
            this.blinkIntensity0UpDown.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.blinkIntensity0UpDown.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.blinkIntensity0UpDown.Name = "blinkIntensity0UpDown";
            this.blinkIntensity0UpDown.Size = new System.Drawing.Size(120, 20);
            this.blinkIntensity0UpDown.TabIndex = 69;
            this.blinkIntensity0UpDown.ValueChanged += new System.EventHandler(this.BlinkIntensity0UpDown_ValueChanged);
            // 
            // blinkDuration0UpDown
            // 
            this.blinkDuration0UpDown.DecimalPlaces = 5;
            this.blinkDuration0UpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.blinkDuration0UpDown.Location = new System.Drawing.Point(191, 358);
            this.blinkDuration0UpDown.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.blinkDuration0UpDown.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.blinkDuration0UpDown.Name = "blinkDuration0UpDown";
            this.blinkDuration0UpDown.Size = new System.Drawing.Size(120, 20);
            this.blinkDuration0UpDown.TabIndex = 70;
            this.blinkDuration0UpDown.ValueChanged += new System.EventHandler(this.BlinkDuration0UpDown_ValueChanged);
            // 
            // blinkIntensity0Label
            // 
            this.blinkIntensity0Label.AutoSize = true;
            this.blinkIntensity0Label.Location = new System.Drawing.Point(109, 335);
            this.blinkIntensity0Label.Name = "blinkIntensity0Label";
            this.blinkIntensity0Label.Size = new System.Drawing.Size(75, 13);
            this.blinkIntensity0Label.TabIndex = 71;
            this.blinkIntensity0Label.Text = "Blink Intensity:";
            // 
            // blinkDuration0Label
            // 
            this.blinkDuration0Label.AutoSize = true;
            this.blinkDuration0Label.Location = new System.Drawing.Point(109, 360);
            this.blinkDuration0Label.Name = "blinkDuration0Label";
            this.blinkDuration0Label.Size = new System.Drawing.Size(76, 13);
            this.blinkDuration0Label.TabIndex = 72;
            this.blinkDuration0Label.Text = "Blink Duration:";
            // 
            // blinkDuration1Label
            // 
            this.blinkDuration1Label.AutoSize = true;
            this.blinkDuration1Label.Location = new System.Drawing.Point(109, 416);
            this.blinkDuration1Label.Name = "blinkDuration1Label";
            this.blinkDuration1Label.Size = new System.Drawing.Size(76, 13);
            this.blinkDuration1Label.TabIndex = 76;
            this.blinkDuration1Label.Text = "Blink Duration:";
            // 
            // blinkIntensity1Label
            // 
            this.blinkIntensity1Label.AutoSize = true;
            this.blinkIntensity1Label.Location = new System.Drawing.Point(109, 391);
            this.blinkIntensity1Label.Name = "blinkIntensity1Label";
            this.blinkIntensity1Label.Size = new System.Drawing.Size(75, 13);
            this.blinkIntensity1Label.TabIndex = 75;
            this.blinkIntensity1Label.Text = "Blink Intensity:";
            // 
            // blinkDuration1UpDown
            // 
            this.blinkDuration1UpDown.DecimalPlaces = 5;
            this.blinkDuration1UpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.blinkDuration1UpDown.Location = new System.Drawing.Point(191, 414);
            this.blinkDuration1UpDown.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.blinkDuration1UpDown.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.blinkDuration1UpDown.Name = "blinkDuration1UpDown";
            this.blinkDuration1UpDown.Size = new System.Drawing.Size(120, 20);
            this.blinkDuration1UpDown.TabIndex = 74;
            this.blinkDuration1UpDown.ValueChanged += new System.EventHandler(this.BlinkDuration1UpDown_ValueChanged);
            // 
            // blinkIntensity1UpDown
            // 
            this.blinkIntensity1UpDown.DecimalPlaces = 5;
            this.blinkIntensity1UpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.blinkIntensity1UpDown.Location = new System.Drawing.Point(191, 389);
            this.blinkIntensity1UpDown.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.blinkIntensity1UpDown.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.blinkIntensity1UpDown.Name = "blinkIntensity1UpDown";
            this.blinkIntensity1UpDown.Size = new System.Drawing.Size(120, 20);
            this.blinkIntensity1UpDown.TabIndex = 73;
            this.blinkIntensity1UpDown.ValueChanged += new System.EventHandler(this.BlinkIntensity1UpDown_ValueChanged);
            // 
            // sizeUpDown
            // 
            this.sizeUpDown.DecimalPlaces = 5;
            this.sizeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.sizeUpDown.Location = new System.Drawing.Point(424, 263);
            this.sizeUpDown.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.sizeUpDown.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.sizeUpDown.Name = "sizeUpDown";
            this.sizeUpDown.Size = new System.Drawing.Size(120, 20);
            this.sizeUpDown.TabIndex = 77;
            this.sizeUpDown.ValueChanged += new System.EventHandler(this.SizeUpDown_ValueChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(388, 265);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(30, 13);
            this.sizeLabel.TabIndex = 78;
            this.sizeLabel.Text = "Size:";
            // 
            // EmitterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTabControl1);
            this.Name = "EmitterEditor";
            this.Size = new System.Drawing.Size(576, 561);
            this.stTabControl1.ResumeLayout(false);
            this.tabPageData.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blinkIntensity0UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blinkDuration0UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blinkDuration1UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blinkIntensity1UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPageData;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Forms.ColorArrayEditor color0ArrayEditor;
        private Forms.ColorArrayEditor alpha1ArrayEditor;
        private Forms.ColorArrayEditor color1ArrayEditor;
        private Forms.ColorArrayEditor alpha0ArrayEditor;
        private Toolbox.Library.Forms.Editors.STColorEditor colorEditor;
        private ColorConstantPanel constant1Panel;
        private ColorConstantPanel constant0Panel;
        private System.Windows.Forms.Label constantsLabel;
        private ColorArrayEditor scaleArrayEditor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label blinkDuration0Label;
        private System.Windows.Forms.Label blinkIntensity0Label;
        private System.Windows.Forms.NumericUpDown blinkDuration0UpDown;
        private System.Windows.Forms.NumericUpDown blinkIntensity0UpDown;
        private System.Windows.Forms.Label blinkDuration1Label;
        private System.Windows.Forms.Label blinkIntensity1Label;
        private System.Windows.Forms.NumericUpDown blinkDuration1UpDown;
        private System.Windows.Forms.NumericUpDown blinkIntensity1UpDown;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.NumericUpDown sizeUpDown;
    }
}
