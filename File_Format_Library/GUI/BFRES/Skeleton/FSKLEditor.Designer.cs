namespace FirstPlugin.Forms
{
    partial class FSKLEditor
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
            this.stFlowLayoutPanel1 = new Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stDropDownPanel1 = new Toolbox.Library.Forms.STDropDownPanel();
            this.scalingModeCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.rotationModeCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stDropDownPanel2 = new Toolbox.Library.Forms.STDropDownPanel();
            this.stButton3 = new Toolbox.Library.Forms.STButton();
            this.btnRgidIndices = new Toolbox.Library.Forms.STButton();
            this.btnSmoothIndices = new Toolbox.Library.Forms.STButton();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.stFlowLayoutPanel1.SuspendLayout();
            this.stDropDownPanel1.SuspendLayout();
            this.stDropDownPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.AutoScroll = true;
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel1);
            this.stFlowLayoutPanel1.Controls.Add(this.stDropDownPanel2);
            this.stFlowLayoutPanel1.Controls.Add(this.textBox1);
            this.stFlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stFlowLayoutPanel1.FixedHeight = false;
            this.stFlowLayoutPanel1.FixedWidth = true;
            this.stFlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(434, 628);
            this.stFlowLayoutPanel1.TabIndex = 2;
            this.stFlowLayoutPanel1.WrapContents = false;
            // 
            // stDropDownPanel1
            // 
            this.stDropDownPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel1.Controls.Add(this.scalingModeCB);
            this.stDropDownPanel1.Controls.Add(this.stLabel2);
            this.stDropDownPanel1.Controls.Add(this.rotationModeCB);
            this.stDropDownPanel1.Controls.Add(this.stLabel1);
            this.stDropDownPanel1.ExpandedHeight = 163;
            this.stDropDownPanel1.IsExpanded = true;
            this.stDropDownPanel1.Location = new System.Drawing.Point(0, 0);
            this.stDropDownPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel1.Name = "stDropDownPanel1";
            this.stDropDownPanel1.PanelName = "Skeleton Info";
            this.stDropDownPanel1.PanelValueName = "";
            this.stDropDownPanel1.SetIcon = null;
            this.stDropDownPanel1.SetIconAlphaColor = System.Drawing.Color.White;
            this.stDropDownPanel1.SetIconColor = System.Drawing.Color.White;
            this.stDropDownPanel1.Size = new System.Drawing.Size(434, 108);
            this.stDropDownPanel1.TabIndex = 0;
            // 
            // scalingModeCB
            // 
            this.scalingModeCB.BorderColor = System.Drawing.Color.Empty;
            this.scalingModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.scalingModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.scalingModeCB.FormattingEnabled = true;
            this.scalingModeCB.IsReadOnly = false;
            this.scalingModeCB.Location = new System.Drawing.Point(114, 64);
            this.scalingModeCB.Name = "scalingModeCB";
            this.scalingModeCB.Size = new System.Drawing.Size(172, 21);
            this.scalingModeCB.TabIndex = 4;
            this.scalingModeCB.SelectedIndexChanged += new System.EventHandler(this.ModeCB_SelectedIndexChanged);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(31, 67);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(75, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Scaling Mode:";
            // 
            // rotationModeCB
            // 
            this.rotationModeCB.BorderColor = System.Drawing.Color.Empty;
            this.rotationModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.rotationModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.rotationModeCB.FormattingEnabled = true;
            this.rotationModeCB.IsReadOnly = false;
            this.rotationModeCB.Location = new System.Drawing.Point(114, 37);
            this.rotationModeCB.Name = "rotationModeCB";
            this.rotationModeCB.Size = new System.Drawing.Size(172, 21);
            this.rotationModeCB.TabIndex = 2;
            this.rotationModeCB.SelectedIndexChanged += new System.EventHandler(this.ModeCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(31, 40);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(80, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Rotation Mode:";
            // 
            // stDropDownPanel2
            // 
            this.stDropDownPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stDropDownPanel2.Controls.Add(this.stButton3);
            this.stDropDownPanel2.Controls.Add(this.btnRgidIndices);
            this.stDropDownPanel2.Controls.Add(this.btnSmoothIndices);
            this.stDropDownPanel2.Controls.Add(this.stLabel5);
            this.stDropDownPanel2.Controls.Add(this.stLabel4);
            this.stDropDownPanel2.Controls.Add(this.stLabel3);
            this.stDropDownPanel2.ExpandedHeight = 163;
            this.stDropDownPanel2.IsExpanded = true;
            this.stDropDownPanel2.Location = new System.Drawing.Point(0, 108);
            this.stDropDownPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.stDropDownPanel2.Name = "stDropDownPanel2";
            this.stDropDownPanel2.PanelName = "Matricies";
            this.stDropDownPanel2.PanelValueName = "";
            this.stDropDownPanel2.SetIcon = null;
            this.stDropDownPanel2.SetIconAlphaColor = System.Drawing.Color.White;
            this.stDropDownPanel2.SetIconColor = System.Drawing.Color.White;
            this.stDropDownPanel2.Size = new System.Drawing.Size(434, 182);
            this.stDropDownPanel2.TabIndex = 5;
            // 
            // stButton3
            // 
            this.stButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton3.Location = new System.Drawing.Point(166, 98);
            this.stButton3.Name = "stButton3";
            this.stButton3.Size = new System.Drawing.Size(75, 23);
            this.stButton3.TabIndex = 11;
            this.stButton3.UseVisualStyleBackColor = false;
            // 
            // btnRgidIndices
            // 
            this.btnRgidIndices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRgidIndices.Location = new System.Drawing.Point(166, 66);
            this.btnRgidIndices.Name = "btnRgidIndices";
            this.btnRgidIndices.Size = new System.Drawing.Size(75, 23);
            this.btnRgidIndices.TabIndex = 10;
            this.btnRgidIndices.UseVisualStyleBackColor = false;
            this.btnRgidIndices.Click += new System.EventHandler(this.btnRgidIndices_Click);
            // 
            // btnSmoothIndices
            // 
            this.btnSmoothIndices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSmoothIndices.Location = new System.Drawing.Point(166, 33);
            this.btnSmoothIndices.Name = "btnSmoothIndices";
            this.btnSmoothIndices.Size = new System.Drawing.Size(75, 23);
            this.btnSmoothIndices.TabIndex = 9;
            this.btnSmoothIndices.UseVisualStyleBackColor = false;
            this.btnSmoothIndices.Click += new System.EventHandler(this.btnSmoothIndices_Click);
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(31, 103);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(85, 13);
            this.stLabel5.TabIndex = 8;
            this.stLabel5.Text = "Inverse Matrices";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(31, 71);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(99, 13);
            this.stLabel4.TabIndex = 7;
            this.stLabel4.Text = "Rigid Matrix Indices";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(31, 38);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(111, 13);
            this.stLabel3.TabIndex = 5;
            this.stLabel3.Text = "Smooth Matrix Indices";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(3, 293);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(428, 332);
            this.textBox1.TabIndex = 13;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // FSKLEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "FSKLEditor";
            this.Size = new System.Drawing.Size(434, 628);
            this.stFlowLayoutPanel1.ResumeLayout(false);
            this.stFlowLayoutPanel1.PerformLayout();
            this.stDropDownPanel1.ResumeLayout(false);
            this.stDropDownPanel1.PerformLayout();
            this.stDropDownPanel2.ResumeLayout(false);
            this.stDropDownPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel1;
        private Toolbox.Library.Forms.STComboBox scalingModeCB;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox rotationModeCB;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STDropDownPanel stDropDownPanel2;
        private Toolbox.Library.Forms.STButton stButton3;
        private Toolbox.Library.Forms.STButton btnRgidIndices;
        private Toolbox.Library.Forms.STButton btnSmoothIndices;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private System.Windows.Forms.TextBox textBox1;
    }
}
