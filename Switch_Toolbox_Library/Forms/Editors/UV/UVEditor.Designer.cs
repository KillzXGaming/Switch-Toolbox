namespace Toolbox.Library.Forms
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
            this.scaleYUD = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.transYUD = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.transXUD = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.scaleXUD = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.drawableContainerCB = new Toolbox.Library.Forms.STComboBox();
            this.meshesCB = new Toolbox.Library.Forms.STComboBox();
            this.barSlider1 = new ColorSlider.ColorSlider();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.textureCB = new Toolbox.Library.Forms.STComboBox();
            this.comboBox1 = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.btnApplyTransform = new Toolbox.Library.Forms.STButton();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel3 = new Toolbox.Library.Forms.STPanel();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transYUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transXUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXUD)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // gL_ControlLegacy2D1
            // 
            this.gL_ControlLegacy2D1.BackColor = System.Drawing.Color.Black;
            this.gL_ControlLegacy2D1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gL_ControlLegacy2D1.Location = new System.Drawing.Point(0, 0);
            this.gL_ControlLegacy2D1.Name = "gL_ControlLegacy2D1";
            this.gL_ControlLegacy2D1.Size = new System.Drawing.Size(443, 454);
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
            this.scaleYUD.Location = new System.Drawing.Point(75, 32);
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
            this.transYUD.Location = new System.Drawing.Point(75, 102);
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
            this.transXUD.Location = new System.Drawing.Point(5, 102);
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
            this.stLabel4.Location = new System.Drawing.Point(3, 66);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(51, 13);
            this.stLabel4.TabIndex = 5;
            this.stLabel4.Text = "Translate";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(3, 11);
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
            this.scaleXUD.Location = new System.Drawing.Point(5, 32);
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
            this.stPanel1.Controls.Add(this.drawableContainerCB);
            this.stPanel1.Controls.Add(this.meshesCB);
            this.stPanel1.Controls.Add(this.barSlider1);
            this.stPanel1.Controls.Add(this.stLabel2);
            this.stPanel1.Controls.Add(this.textureCB);
            this.stPanel1.Controls.Add(this.comboBox1);
            this.stPanel1.Controls.Add(this.stLabel1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(605, 70);
            this.stPanel1.TabIndex = 1;
            // 
            // drawableContainerCB
            // 
            this.drawableContainerCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.drawableContainerCB.BorderColor = System.Drawing.Color.Empty;
            this.drawableContainerCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.drawableContainerCB.ButtonColor = System.Drawing.Color.Empty;
            this.drawableContainerCB.FormattingEnabled = true;
            this.drawableContainerCB.Location = new System.Drawing.Point(294, 7);
            this.drawableContainerCB.Name = "drawableContainerCB";
            this.drawableContainerCB.ReadOnly = true;
            this.drawableContainerCB.Size = new System.Drawing.Size(146, 21);
            this.drawableContainerCB.TabIndex = 20;
            this.drawableContainerCB.SelectedIndexChanged += new System.EventHandler(this.drawableContainerCB_SelectedIndexChanged);
            // 
            // meshesCB
            // 
            this.meshesCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.meshesCB.BorderColor = System.Drawing.Color.Empty;
            this.meshesCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.meshesCB.ButtonColor = System.Drawing.Color.Empty;
            this.meshesCB.FormattingEnabled = true;
            this.meshesCB.Location = new System.Drawing.Point(294, 34);
            this.meshesCB.Name = "meshesCB";
            this.meshesCB.ReadOnly = true;
            this.meshesCB.Size = new System.Drawing.Size(146, 21);
            this.meshesCB.TabIndex = 19;
            this.meshesCB.SelectedIndexChanged += new System.EventHandler(this.meshesCB_SelectedIndexChanged);
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
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(3, 13);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(56, 13);
            this.stLabel2.TabIndex = 2;
            this.stLabel2.Text = "Brightness";
            // 
            // textureCB
            // 
            this.textureCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureCB.BorderColor = System.Drawing.Color.Empty;
            this.textureCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.textureCB.ButtonColor = System.Drawing.Color.Empty;
            this.textureCB.FormattingEnabled = true;
            this.textureCB.Location = new System.Drawing.Point(446, 34);
            this.textureCB.Name = "textureCB";
            this.textureCB.ReadOnly = true;
            this.textureCB.Size = new System.Drawing.Size(156, 21);
            this.textureCB.TabIndex = 5;
            this.textureCB.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
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
            // btnApplyTransform
            // 
            this.btnApplyTransform.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyTransform.Location = new System.Drawing.Point(3, 138);
            this.btnApplyTransform.Name = "btnApplyTransform";
            this.btnApplyTransform.Size = new System.Drawing.Size(119, 23);
            this.btnApplyTransform.TabIndex = 9;
            this.btnApplyTransform.Text = "Apply Transform";
            this.btnApplyTransform.UseVisualStyleBackColor = false;
            this.btnApplyTransform.Click += new System.EventHandler(this.btnApplyTransform_Click);
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stButton1);
            this.stPanel2.Controls.Add(this.btnApplyTransform);
            this.stPanel2.Controls.Add(this.stLabel3);
            this.stPanel2.Controls.Add(this.scaleYUD);
            this.stPanel2.Controls.Add(this.transYUD);
            this.stPanel2.Controls.Add(this.scaleXUD);
            this.stPanel2.Controls.Add(this.transXUD);
            this.stPanel2.Controls.Add(this.stLabel4);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.stPanel2.Location = new System.Drawing.Point(0, 70);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(159, 454);
            this.stPanel2.TabIndex = 3;
            // 
            // stButton1
            // 
            this.stButton1.Dock = System.Windows.Forms.DockStyle.Right;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(145, 0);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(14, 454);
            this.stButton1.TabIndex = 10;
            this.stButton1.UseVisualStyleBackColor = false;
            this.stButton1.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(159, 70);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 454);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.gL_ControlLegacy2D1);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(162, 70);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(443, 454);
            this.stPanel3.TabIndex = 5;
            // 
            // UVEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel3);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.stPanel1);
            this.Name = "UVEditor";
            this.Size = new System.Drawing.Size(605, 524);
            ((System.ComponentModel.ISupportInitialize)(this.scaleYUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transYUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transXUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXUD)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stPanel2.ResumeLayout(false);
            this.stPanel2.PerformLayout();
            this.stPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private STPanel stPanel1;
        private STLabel stLabel1;
        private Toolbox.Library.Forms.STComboBox comboBox1;
        private OpenTK.GLControl gL_ControlLegacy2D1;
        private Toolbox.Library.Forms.STComboBox textureCB;
        private STLabel stLabel2;
        private NumericUpDownFloat scaleXUD;
        private STLabel stLabel3;
        private STLabel stLabel4;
        private NumericUpDownFloat transXUD;
        private NumericUpDownFloat transYUD;
        private NumericUpDownFloat scaleYUD;
        private ColorSlider.ColorSlider barSlider1;
        private STButton btnApplyTransform;
        private STPanel stPanel2;
        private STComboBox meshesCB;
        private System.Windows.Forms.Splitter splitter1;
        private STPanel stPanel3;
        private STButton stButton1;
        private STComboBox drawableContainerCB;
    }
}