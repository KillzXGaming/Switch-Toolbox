namespace FirstPlugin.Forms
{
    partial class MaterialPresetDialog
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.stCheckBox1 = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.treeViewCustom1 = new Switch_Toolbox.Library.TreeViewCustom();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            this.stLabel4 = new Switch_Toolbox.Library.Forms.STLabel();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel4);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.pictureBox1);
            this.contentContainer.Controls.Add(this.stCheckBox1);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.treeViewCustom1);
            this.contentContainer.Size = new System.Drawing.Size(646, 489);
            this.contentContainer.Controls.SetChildIndex(this.treeViewCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stCheckBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.pictureBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel4, 0);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(350, 257);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(289, 192);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // stCheckBox1
            // 
            this.stCheckBox1.AutoSize = true;
            this.stCheckBox1.Enabled = false;
            this.stCheckBox1.Location = new System.Drawing.Point(353, 204);
            this.stCheckBox1.Name = "stCheckBox1";
            this.stCheckBox1.Size = new System.Drawing.Size(96, 17);
            this.stCheckBox1.TabIndex = 3;
            this.stCheckBox1.Text = "Embed Shader";
            this.stCheckBox1.UseVisualStyleBackColor = true;
            // 
            // stLabel2
            // 
            this.stLabel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stLabel2.Location = new System.Drawing.Point(350, 79);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(289, 122);
            this.stLabel2.TabIndex = 2;
            this.stLabel2.Text = "Description:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(348, 37);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(38, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Name:";
            // 
            // treeViewCustom1
            // 
            this.treeViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewCustom1.ImageIndex = 0;
            this.treeViewCustom1.Location = new System.Drawing.Point(0, 25);
            this.treeViewCustom1.Name = "treeViewCustom1";
            this.treeViewCustom1.SelectedImageIndex = 0;
            this.treeViewCustom1.Size = new System.Drawing.Size(344, 424);
            this.treeViewCustom1.TabIndex = 0;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(350, 241);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(94, 13);
            this.stLabel3.TabIndex = 5;
            this.stLabel3.Text = "Preview (In Game)";
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(561, 455);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 11;
            this.stButton1.Text = "Cancel";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(480, 455);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 12;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(477, 205);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(44, 13);
            this.stLabel4.TabIndex = 13;
            this.stLabel4.Text = "Shader:";
            // 
            // MaterialPresetDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 492);
            this.Name = "MaterialPresetDialog";
            this.Text = "Material Presets";
            this.Load += new System.EventHandler(this.MaterialPresetDialog_Load);
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.TreeViewCustom treeViewCustom1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STCheckBox stCheckBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private Switch_Toolbox.Library.Forms.STButton stButton2;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel4;
    }
}