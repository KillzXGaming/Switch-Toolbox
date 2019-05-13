namespace Switch_Toolbox.Library.Forms
{
    partial class UVEditor
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
            this.gL_ControlLegacy2D1 = new OpenTK.GLControl();
            this.scaleYUD = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.transYUD = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.transXUD = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel4 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.scaleXUD = new Switch_Toolbox.Library.Forms.NumericUpDownFloat();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.btnApplyTransform = new Switch_Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.comboBox2 = new Switch_Toolbox.Library.Forms.STComboBox();
            this.comboBox1 = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.barSlider1 = new ColorSlider.ColorSlider();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transYUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transXUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXUD)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gL_ControlLegacy2D1
            // 
            this.gL_ControlLegacy2D1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gL_ControlLegacy2D1.BackColor = System.Drawing.Color.Black;
            this.gL_ControlLegacy2D1.Location = new System.Drawing.Point(0, 70);
            this.gL_ControlLegacy2D1.Name = "gL_ControlLegacy2D1";
            this.gL_ControlLegacy2D1.Size = new System.Drawing.Size(605, 454);
            this.gL_ControlLegacy2D1.TabIndex = 2;
            this.gL_ControlLegacy2D1.VSync = false;
            this.gL_ControlLegacy2D1.Paint += new System.Windows.Forms.PaintEventHandler(this.gL_ControlLegacy2D1_Paint);
            this.gL_ControlLegacy2D1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gL_ControlLegacy2D1_MouseDown);
            this.gL_ControlLegacy2D1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gL_ControlLegacy2D1_MouseMove);
            this.gL_ControlLegacy2D1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.OnMouseWheel);
            this.gL_ControlLegacy2D1.Resize += new System.EventHandler(this.gL_ControlLegacy2D1_Resize);
            // 
            // scaleYUD
            // 
            this.scaleYUD.DecimalPlaces = 5;
            this.scaleYUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.scaleYUD.Location = new System.Drawing.Point(135, 43);
            this.scaleYUD.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleYUD.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.scaleYUD.Name = "scaleYUD";
            this.scaleYUD.Size = new System.Drawing.Size(64, 20);
            this.scaleYUD.TabIndex = 8;
            this.scaleYUD.ValueChanged += new System.EventHandler(this.OnNumbicValueSRT_ValueChanged);
            // 
            // transYUD
            // 
            this.transYUD.DecimalPlaces = 5;
            this.transYUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.transYUD.Location = new System.Drawing.Point(360, 43);
            this.transYUD.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.transYUD.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.transYUD.Name = "transYUD";
            this.transYUD.Size = new System.Drawing.Size(64, 20);
            this.transYUD.TabIndex = 7;
            this.transYUD.ValueChanged += new System.EventHandler(this.OnNumbicValueSRT_ValueChanged);
            // 
            // transXUD
            // 
            this.transXUD.DecimalPlaces = 5;
            this.transXUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.transXUD.Location = new System.Drawing.Point(281, 43);
            this.transXUD.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.transXUD.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.transXUD.Name = "transXUD";
            this.transXUD.Size = new System.Drawing.Size(64, 20);
            this.transXUD.TabIndex = 6;
            this.transXUD.ValueChanged += new System.EventHandler(this.OnNumbicValueSRT_ValueChanged);
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(205, 45);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(51, 13);
            this.stLabel4.TabIndex = 5;
            this.stLabel4.Text = "Translate";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(25, 45);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(34, 13);
            this.stLabel3.TabIndex = 4;
            this.stLabel3.Text = "Scale";
            // 
            // scaleXUD
            // 
            this.scaleXUD.DecimalPlaces = 5;
            this.scaleXUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.scaleXUD.Location = new System.Drawing.Point(65, 43);
            this.scaleXUD.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.scaleXUD.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.scaleXUD.Name = "scaleXUD";
            this.scaleXUD.Size = new System.Drawing.Size(64, 20);
            this.scaleXUD.TabIndex = 3;
            this.scaleXUD.ValueChanged += new System.EventHandler(this.OnNumbicValueSRT_ValueChanged);
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.barSlider1);
            this.stPanel1.Controls.Add(this.btnApplyTransform);
            this.stPanel1.Controls.Add(this.scaleYUD);
            this.stPanel1.Controls.Add(this.transYUD);
            this.stPanel1.Controls.Add(this.stLabel2);
            this.stPanel1.Controls.Add(this.transXUD);
            this.stPanel1.Controls.Add(this.comboBox2);
            this.stPanel1.Controls.Add(this.stLabel4);
            this.stPanel1.Controls.Add(this.comboBox1);
            this.stPanel1.Controls.Add(this.stLabel3);
            this.stPanel1.Controls.Add(this.scaleXUD);
            this.stPanel1.Controls.Add(this.stLabel1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(605, 70);
            this.stPanel1.TabIndex = 1;
            // 
            // btnApplyTransform
            // 
            this.btnApplyTransform.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyTransform.Location = new System.Drawing.Point(430, 41);
            this.btnApplyTransform.Name = "btnApplyTransform";
            this.btnApplyTransform.Size = new System.Drawing.Size(119, 23);
            this.btnApplyTransform.TabIndex = 9;
            this.btnApplyTransform.Text = "Apply Transform";
            this.btnApplyTransform.UseVisualStyleBackColor = false;
            this.btnApplyTransform.Click += new System.EventHandler(this.btnApplyTransform_Click);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(3, 7);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(56, 13);
            this.stLabel2.TabIndex = 2;
            this.stLabel2.Text = "Brightness";
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox2.BorderColor = System.Drawing.Color.Empty;
            this.comboBox2.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.comboBox2.ButtonColor = System.Drawing.Color.Empty;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(290, 7);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.ReadOnly = true;
            this.comboBox2.Size = new System.Drawing.Size(166, 21);
            this.comboBox2.TabIndex = 5;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.BorderColor = System.Drawing.Color.Empty;
            this.comboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.comboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(550, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.ReadOnly = true;
            this.comboBox1.Size = new System.Drawing.Size(52, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(462, 10);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(82, 13);
            this.stLabel1.TabIndex = 4;
            this.stLabel1.Text = "Active Channel:";
            // 
            // barSlider1
            // 
            this.barSlider1.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.barSlider1.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.barSlider1.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.barSlider1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.barSlider1.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.barSlider1.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.barSlider1.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.barSlider1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.barSlider1.ForeColor = System.Drawing.Color.White;
            this.barSlider1.LargeChange = ((uint)(5u));
            this.barSlider1.Location = new System.Drawing.Point(65, 7);
            this.barSlider1.MouseEffects = false;
            this.barSlider1.Name = "barSlider1";
            this.barSlider1.ScaleDivisions = 10;
            this.barSlider1.ScaleSubDivisions = 5;
            this.barSlider1.ShowDivisionsText = true;
            this.barSlider1.ShowSmallScale = false;
            this.barSlider1.Size = new System.Drawing.Size(212, 19);
            this.barSlider1.SmallChange = ((uint)(1u));
            this.barSlider1.TabIndex = 18;
            this.barSlider1.Text = "colorSlider1";
            this.barSlider1.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.barSlider1.ThumbPenColor = System.Drawing.Color.Silver;
            this.barSlider1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.barSlider1.ThumbSize = new System.Drawing.Size(8, 8);
            this.barSlider1.TickAdd = 0F;
            this.barSlider1.TickColor = System.Drawing.Color.White;
            this.barSlider1.TickDivide = 0F;
            this.barSlider1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider1.ValueChanged += new System.EventHandler(this.barSlider1_ValueChanged);
            this.barSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.barSlider1_Scroll);
            // 
            // UVEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gL_ControlLegacy2D1);
            this.Controls.Add(this.stPanel1);
            this.Name = "UVEditor";
            this.Size = new System.Drawing.Size(605, 524);
            ((System.ComponentModel.ISupportInitialize)(this.scaleYUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transYUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transXUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXUD)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private STPanel stPanel1;
        private STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STComboBox comboBox1;
        private OpenTK.GLControl gL_ControlLegacy2D1;
        private Switch_Toolbox.Library.Forms.STComboBox comboBox2;
        private STLabel stLabel2;
        private NumericUpDownFloat scaleXUD;
        private STLabel stLabel3;
        private STLabel stLabel4;
        private NumericUpDownFloat transXUD;
        private NumericUpDownFloat transYUD;
        private NumericUpDownFloat scaleYUD;
        private STButton btnApplyTransform;
        private ColorSlider.ColorSlider barSlider1;
    }
}