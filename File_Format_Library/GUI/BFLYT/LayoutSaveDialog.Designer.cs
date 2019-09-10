namespace FirstPlugin.Forms
{
    partial class LayoutSaveDialog
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Animations");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Layouts");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Textures");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Fonts");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Shaders");
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.radioButton6);
            this.contentContainer.Controls.Add(this.radioButton5);
            this.contentContainer.Controls.Add(this.radioButton4);
            this.contentContainer.Controls.Add(this.radioButton3);
            this.contentContainer.Controls.Add(this.radioButton2);
            this.contentContainer.Controls.Add(this.radioButton1);
            this.contentContainer.Controls.Add(this.treeView1);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Size = new System.Drawing.Size(389, 337);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.treeView1, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton3, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton4, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton5, 0);
            this.contentContainer.Controls.SetChildIndex(this.radioButton6, 0);
            // 
            // stButton1
            // 
            this.stButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(305, 305);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 0;
            this.stButton1.Text = "Save";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(3, 31);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node5";
            treeNode1.Text = "Animations";
            treeNode2.Name = "Node6";
            treeNode2.Text = "Layouts";
            treeNode3.Name = "Node7";
            treeNode3.Text = "Textures";
            treeNode4.Name = "Node8";
            treeNode4.Text = "Fonts";
            treeNode5.Name = "Node9";
            treeNode5.Text = "Shaders";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            this.treeView1.Size = new System.Drawing.Size(248, 271);
            this.treeView1.TabIndex = 1;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(257, 37);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(105, 17);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Save To Archive";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(257, 60);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(129, 17);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Save As New Archive";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(257, 83);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(119, 17);
            this.radioButton3.TabIndex = 4;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Save To File/Folder";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(257, 203);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(90, 17);
            this.radioButton4.TabIndex = 5;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Save As XML";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(257, 226);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(97, 17);
            this.radioButton5.TabIndex = 6;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "Save As YAML";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(257, 180);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(97, 17);
            this.radioButton6.TabIndex = 11;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "Save As Binary";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // LayoutSaveDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 342);
            this.Name = "LayoutSaveDialog";
            this.Text = "Layout Save Dialog";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STButton stButton1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton6;
    }
}