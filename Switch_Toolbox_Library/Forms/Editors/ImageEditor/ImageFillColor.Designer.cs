namespace Toolbox.Library.Forms
{
    partial class ImageFillColor
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
            this.stCheckBox1 = new Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stButton2 = new Toolbox.Library.Forms.STButton();
            this.stButton3 = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton3);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.stCheckBox1);
            this.contentContainer.Size = new System.Drawing.Size(135, 128);
            this.contentContainer.Controls.SetChildIndex(this.stCheckBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton3, 0);
            // 
            // stCheckBox1
            // 
            this.stCheckBox1.AutoSize = true;
            this.stCheckBox1.Location = new System.Drawing.Point(9, 40);
            this.stCheckBox1.Name = "stCheckBox1";
            this.stCheckBox1.Size = new System.Drawing.Size(90, 17);
            this.stCheckBox1.TabIndex = 11;
            this.stCheckBox1.Text = "Resize to 1x1";
            this.stCheckBox1.UseVisualStyleBackColor = true;
            this.stCheckBox1.CheckedChanged += new System.EventHandler(this.stCheckBox1_CheckedChanged);
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(46, 63);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(32, 23);
            this.stButton1.TabIndex = 12;
            this.stButton1.UseVisualStyleBackColor = false;
            this.stButton1.Click += new System.EventHandler(this.stButton1_Click);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 68);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(31, 13);
            this.stLabel1.TabIndex = 13;
            this.stLabel1.Text = "Color";
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(79, 98);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(53, 23);
            this.stButton2.TabIndex = 14;
            this.stButton2.Text = "Cancel";
            this.stButton2.UseVisualStyleBackColor = false;
            this.stButton2.Click += new System.EventHandler(this.stButton2_Click);
            // 
            // stButton3
            // 
            this.stButton3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton3.Location = new System.Drawing.Point(23, 98);
            this.stButton3.Name = "stButton3";
            this.stButton3.Size = new System.Drawing.Size(53, 23);
            this.stButton3.TabIndex = 15;
            this.stButton3.Text = "Ok";
            this.stButton3.UseVisualStyleBackColor = false;
            this.stButton3.Click += new System.EventHandler(this.stButton3_Click);
            // 
            // ImageFillColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(141, 133);
            this.Name = "ImageFillColor";
            this.Text = "Fill Color";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STCheckBox stCheckBox1;
        private STButton stButton1;
        private STButton stButton3;
        private STButton stButton2;
        private STLabel stLabel1;
    }
}