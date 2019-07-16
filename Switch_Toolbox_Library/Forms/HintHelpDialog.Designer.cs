namespace Toolbox.Library.Forms
{
    partial class HintHelpDialog
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
            this.components = new System.ComponentModel.Container();
            this.BtmMipsLeft = new Toolbox.Library.Forms.STButton();
            this.BtnMipsRight = new Toolbox.Library.Forms.STButton();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.treeViewCustom1 = new Toolbox.Library.TreeViewCustom();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.treeViewCustom1);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.BtmMipsLeft);
            this.contentContainer.Controls.Add(this.BtnMipsRight);
            this.contentContainer.Controls.SetChildIndex(this.BtnMipsRight, 0);
            this.contentContainer.Controls.SetChildIndex(this.BtmMipsLeft, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.treeViewCustom1, 0);
            // 
            // BtmMipsLeft
            // 
            this.BtmMipsLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtmMipsLeft.Enabled = false;
            this.BtmMipsLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtmMipsLeft.Location = new System.Drawing.Point(142, 362);
            this.BtmMipsLeft.Name = "BtmMipsLeft";
            this.BtmMipsLeft.Size = new System.Drawing.Size(28, 22);
            this.BtmMipsLeft.TabIndex = 11;
            this.BtmMipsLeft.Text = "<";
            this.BtmMipsLeft.UseVisualStyleBackColor = true;
            // 
            // BtnMipsRight
            // 
            this.BtnMipsRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnMipsRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnMipsRight.Location = new System.Drawing.Point(179, 362);
            this.BtnMipsRight.Name = "BtnMipsRight";
            this.BtnMipsRight.Size = new System.Drawing.Size(30, 22);
            this.BtnMipsRight.TabIndex = 12;
            this.BtnMipsRight.Text = ">";
            this.BtnMipsRight.UseVisualStyleBackColor = true;
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(459, 361);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 14;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Location = new System.Drawing.Point(136, 48);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(400, 308);
            this.stPanel1.TabIndex = 15;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(3, 28);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(91, 13);
            this.stLabel1.TabIndex = 16;
            this.stLabel1.Text = "Table of Contents";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(139, 28);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(68, 13);
            this.stLabel2.TabIndex = 17;
            this.stLabel2.Text = "Help Section";
            // 
            // treeViewCustom1
            // 
            this.treeViewCustom1.ImageIndex = 0;
            this.treeViewCustom1.Location = new System.Drawing.Point(3, 48);
            this.treeViewCustom1.Name = "treeViewCustom1";
            this.treeViewCustom1.SelectedImageIndex = 0;
            this.treeViewCustom1.Size = new System.Drawing.Size(127, 342);
            this.treeViewCustom1.TabIndex = 18;
            // 
            // HintHelpDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "HintHelpDialog";
            this.Text = "Help Dialog";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STButton BtmMipsLeft;
        private STButton BtnMipsRight;
        private STLabel stLabel2;
        private STLabel stLabel1;
        private STPanel stPanel1;
        private STButton stButton1;
        private TreeViewCustom treeViewCustom1;
    }
}