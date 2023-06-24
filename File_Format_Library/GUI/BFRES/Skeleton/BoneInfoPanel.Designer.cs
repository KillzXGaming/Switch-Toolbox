namespace FirstPlugin
{
    partial class BoneInfoPanel
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
            this.nameTB = new Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.parentTB = new Toolbox.Library.Forms.STTextBox();
            this.visibleChk = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.parentIndexUD = new Toolbox.Library.Forms.NumericUpDownInt();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.nameIndexUD = new Toolbox.Library.Forms.NumericUpDownInt();
            ((System.ComponentModel.ISupportInitialize)(this.parentIndexUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nameIndexUD)).BeginInit();
            this.SuspendLayout();
            // 
            // nameTB
            // 
            this.nameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTB.Location = new System.Drawing.Point(60, 32);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(227, 20);
            this.nameTB.TabIndex = 0;
            this.nameTB.TextChanged += new System.EventHandler(this.nameTB_TextChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(13, 34);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(38, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Name:";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(13, 62);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(41, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Parent:";
            // 
            // parentTB
            // 
            this.parentTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parentTB.Enabled = false;
            this.parentTB.Location = new System.Drawing.Point(60, 60);
            this.parentTB.Name = "parentTB";
            this.parentTB.Size = new System.Drawing.Size(227, 20);
            this.parentTB.TabIndex = 2;
            // 
            // visibleChk
            // 
            this.visibleChk.AutoSize = true;
            this.visibleChk.Location = new System.Drawing.Point(60, 10);
            this.visibleChk.Name = "visibleChk";
            this.visibleChk.Size = new System.Drawing.Size(15, 14);
            this.visibleChk.TabIndex = 4;
            this.visibleChk.UseVisualStyleBackColor = true;
            this.visibleChk.CheckedChanged += new System.EventHandler(this.visibleChk_CheckedChanged);
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(11, 11);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(40, 13);
            this.stLabel3.TabIndex = 5;
            this.stLabel3.Text = "Visible:";
            // 
            // parentIndexUD
            // 
            this.parentIndexUD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parentIndexUD.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.parentIndexUD.Location = new System.Drawing.Point(335, 62);
            this.parentIndexUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.parentIndexUD.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.parentIndexUD.Name = "parentIndexUD";
            this.parentIndexUD.ReadOnly = true;
            this.parentIndexUD.Size = new System.Drawing.Size(86, 20);
            this.parentIndexUD.TabIndex = 6;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(293, 62);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(36, 13);
            this.stLabel4.TabIndex = 7;
            this.stLabel4.Text = "Index:";
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(293, 36);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(36, 13);
            this.stLabel5.TabIndex = 9;
            this.stLabel5.Text = "Index:";
            // 
            // nameIndexUD
            // 
            this.nameIndexUD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameIndexUD.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nameIndexUD.Location = new System.Drawing.Point(335, 34);
            this.nameIndexUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nameIndexUD.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.nameIndexUD.Name = "nameIndexUD";
            this.nameIndexUD.ReadOnly = true;
            this.nameIndexUD.Size = new System.Drawing.Size(86, 20);
            this.nameIndexUD.TabIndex = 8;
            // 
            // BoneInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stLabel5);
            this.Controls.Add(this.nameIndexUD);
            this.Controls.Add(this.stLabel4);
            this.Controls.Add(this.parentIndexUD);
            this.Controls.Add(this.stLabel3);
            this.Controls.Add(this.visibleChk);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.parentTB);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.nameTB);
            this.Name = "BoneInfoPanel";
            this.Size = new System.Drawing.Size(421, 92);
            ((System.ComponentModel.ISupportInitialize)(this.parentIndexUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nameIndexUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STTextBox nameTB;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STTextBox parentTB;
        private Toolbox.Library.Forms.STCheckBox visibleChk;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.NumericUpDownInt parentIndexUD;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel5;
        private Toolbox.Library.Forms.NumericUpDownInt nameIndexUD;
    }
}
