namespace FirstPlugin
{
    partial class BatchEditBaseAnimDataForm
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
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.boneListCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.scaleXUD = new Toolbox.Library.Forms.STNumbericUpDown();
            this.scaleYUD = new Toolbox.Library.Forms.STNumbericUpDown();
            this.scaleZUD = new Toolbox.Library.Forms.STNumbericUpDown();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.scaleCompChk = new Toolbox.Library.Forms.STCheckBox();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.scaleCompChk);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.scaleZUD);
            this.contentContainer.Controls.Add(this.scaleYUD);
            this.contentContainer.Controls.Add(this.scaleXUD);
            this.contentContainer.Controls.Add(this.stLabel4);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.boneListCB);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Size = new System.Drawing.Size(409, 259);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.boneListCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel4, 0);
            this.contentContainer.Controls.SetChildIndex(this.scaleXUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.scaleYUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.scaleZUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.scaleCompChk, 0);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 40);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(66, 13);
            this.stLabel1.TabIndex = 12;
            this.stLabel1.Text = "Bone Target";
            // 
            // boneListCB
            // 
            this.boneListCB.BorderColor = System.Drawing.Color.Empty;
            this.boneListCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.boneListCB.ButtonColor = System.Drawing.Color.Empty;
            this.boneListCB.FormattingEnabled = true;
            this.boneListCB.Location = new System.Drawing.Point(12, 56);
            this.boneListCB.Name = "boneListCB";
            this.boneListCB.ReadOnly = true;
            this.boneListCB.Size = new System.Drawing.Size(132, 21);
            this.boneListCB.TabIndex = 13;
            this.boneListCB.SelectedIndexChanged += new System.EventHandler(this.boneListCB_SelectedIndexChanged);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(9, 106);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(44, 13);
            this.stLabel2.TabIndex = 14;
            this.stLabel2.Text = "Scale X";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(121, 106);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(44, 13);
            this.stLabel3.TabIndex = 15;
            this.stLabel3.Text = "Scale Y";
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(224, 106);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(44, 13);
            this.stLabel4.TabIndex = 16;
            this.stLabel4.Text = "Scale Z";
            // 
            // scaleXUD
            // 
            this.scaleXUD.Location = new System.Drawing.Point(12, 132);
            this.scaleXUD.Name = "scaleXUD";
            this.scaleXUD.Size = new System.Drawing.Size(106, 20);
            this.scaleXUD.TabIndex = 17;
            this.scaleXUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // scaleYUD
            // 
            this.scaleYUD.Location = new System.Drawing.Point(124, 132);
            this.scaleYUD.Name = "scaleYUD";
            this.scaleYUD.Size = new System.Drawing.Size(106, 20);
            this.scaleYUD.TabIndex = 18;
            this.scaleYUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // scaleZUD
            // 
            this.scaleZUD.Location = new System.Drawing.Point(227, 132);
            this.scaleZUD.Name = "scaleZUD";
            this.scaleZUD.Size = new System.Drawing.Size(106, 20);
            this.scaleZUD.TabIndex = 19;
            this.scaleZUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(326, 229);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 20;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // scaleCompChk
            // 
            this.scaleCompChk.AutoSize = true;
            this.scaleCompChk.Location = new System.Drawing.Point(165, 58);
            this.scaleCompChk.Name = "scaleCompChk";
            this.scaleCompChk.Size = new System.Drawing.Size(160, 17);
            this.scaleCompChk.TabIndex = 21;
            this.scaleCompChk.Text = "Segment Scale Compensate";
            this.scaleCompChk.UseVisualStyleBackColor = true;
            // 
            // BatchEditBaseAnimDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 264);
            this.Name = "BatchEditBaseAnimDataForm";
            this.Text = "BatchEditBaseAnimDataForm";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STNumbericUpDown scaleYUD;
        private Toolbox.Library.Forms.STNumbericUpDown scaleXUD;
        private Toolbox.Library.Forms.STLabel stLabel4;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox boneListCB;
        private Toolbox.Library.Forms.STButton stButton1;
        private Toolbox.Library.Forms.STNumbericUpDown scaleZUD;
        private Toolbox.Library.Forms.STCheckBox scaleCompChk;
    }
}