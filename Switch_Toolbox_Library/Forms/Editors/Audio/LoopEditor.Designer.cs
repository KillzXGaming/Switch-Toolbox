namespace Toolbox.Library.Forms
{
    partial class LoopEditor
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
            this.startLoopUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.endLoopUD = new Toolbox.Library.Forms.NumericUpDownUint();
            this.chkCanLoop = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.btnOk = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startLoopUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endLoopUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.btnOk);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.chkCanLoop);
            this.contentContainer.Controls.Add(this.endLoopUD);
            this.contentContainer.Controls.Add(this.startLoopUD);
            this.contentContainer.Size = new System.Drawing.Size(272, 149);
            this.contentContainer.Controls.SetChildIndex(this.startLoopUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.endLoopUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkCanLoop, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOk, 0);
            // 
            // startLoopUD
            // 
            this.startLoopUD.Location = new System.Drawing.Point(12, 73);
            this.startLoopUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.startLoopUD.Name = "startLoopUD";
            this.startLoopUD.Size = new System.Drawing.Size(120, 20);
            this.startLoopUD.TabIndex = 10;
            // 
            // endLoopUD
            // 
            this.endLoopUD.Location = new System.Drawing.Point(138, 73);
            this.endLoopUD.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.endLoopUD.Name = "endLoopUD";
            this.endLoopUD.Size = new System.Drawing.Size(120, 20);
            this.endLoopUD.TabIndex = 11;
            // 
            // chkCanLoop
            // 
            this.chkCanLoop.AutoSize = true;
            this.chkCanLoop.Location = new System.Drawing.Point(9, 31);
            this.chkCanLoop.Name = "chkCanLoop";
            this.chkCanLoop.Size = new System.Drawing.Size(50, 17);
            this.chkCanLoop.TabIndex = 12;
            this.chkCanLoop.Text = "Loop";
            this.chkCanLoop.UseVisualStyleBackColor = true;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 57);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(29, 13);
            this.stLabel1.TabIndex = 13;
            this.stLabel1.Text = "Start";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(135, 57);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(26, 13);
            this.stLabel2.TabIndex = 14;
            this.stLabel2.Text = "End";
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(183, 117);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 15;
            this.stButton1.Text = "Cancel";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(102, 117);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 16;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // LoopEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 154);
            this.Name = "LoopEditor";
            this.Text = "Loop Editor";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startLoopUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endLoopUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public NumericUpDownUint startLoopUD;
        public NumericUpDownUint endLoopUD;
        public Toolbox.Library.Forms.STCheckBox chkCanLoop;
        private STLabel stLabel1;
        private STLabel stLabel2;
        private STButton btnOk;
        private STButton stButton1;
    }
}