namespace FirstPlugin.Forms
{
    partial class VertexBufferCreator
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.formatCB = new Toolbox.Library.Forms.STComboBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.nameTB = new Toolbox.Library.Forms.STTextBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.numericUpDownFloat1 = new Toolbox.Library.Forms.NumericUpDownFloat();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.numericUpDownFloat1);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.nameTB);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.formatCB);
            this.contentContainer.Size = new System.Drawing.Size(275, 171);
            this.contentContainer.Controls.SetChildIndex(this.formatCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.nameTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.numericUpDownFloat1, 0);
            // 
            // formatCB
            // 
            this.formatCB.BorderColor = System.Drawing.Color.Empty;
            this.formatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.formatCB.ButtonColor = System.Drawing.Color.Empty;
            this.formatCB.FormattingEnabled = true;
            this.formatCB.Location = new System.Drawing.Point(68, 68);
            this.formatCB.Name = "formatCB";
            this.formatCB.ReadOnly = true;
            this.formatCB.Size = new System.Drawing.Size(198, 21);
            this.formatCB.TabIndex = 0;
            this.formatCB.SelectedIndexChanged += new System.EventHandler(this.formatCB_SelectedIndexChanged);
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(110, 140);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 1;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(191, 140);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 2;
            this.stButton2.Text = "Cancel";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 71);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(42, 13);
            this.stLabel1.TabIndex = 3;
            this.stLabel1.Text = "Format:";
            // 
            // nameTB
            // 
            this.nameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTB.Location = new System.Drawing.Point(68, 37);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(198, 20);
            this.nameTB.TabIndex = 4;
            this.nameTB.TextChanged += new System.EventHandler(this.nameTB_TextChanged);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(9, 39);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(38, 13);
            this.stLabel2.TabIndex = 5;
            this.stLabel2.Text = "Name:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(9, 103);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(57, 13);
            this.stLabel3.TabIndex = 11;
            this.stLabel3.Text = "Inert Data:";
            // 
            // numericUpDownFloat1
            // 
            this.numericUpDownFloat1.DecimalPlaces = 5;
            this.numericUpDownFloat1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            196608});
            this.numericUpDownFloat1.Location = new System.Drawing.Point(68, 101);
            this.numericUpDownFloat1.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownFloat1.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numericUpDownFloat1.Name = "numericUpDownFloat1";
            this.numericUpDownFloat1.Size = new System.Drawing.Size(198, 20);
            this.numericUpDownFloat1.TabIndex = 12;
            // 
            // VertexBufferCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 177);
            this.Name = "VertexBufferCreator";
            this.Text = "Buffer Creator";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloat1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STComboBox formatCB;
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STButton stButton2;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STTextBox nameTB;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.NumericUpDownFloat numericUpDownFloat1;
        private Toolbox.Library.Forms.STLabel stLabel3;
    }
}