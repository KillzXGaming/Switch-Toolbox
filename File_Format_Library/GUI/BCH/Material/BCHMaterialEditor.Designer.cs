namespace FirstPlugin.CtrLibrary.Forms
{
    partial class BCHMaterialEditor
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
            this.stTabControl1 = new Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.stTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Controls.Add(this.tabPage6);
            this.stTabControl1.Controls.Add(this.tabPage3);
            this.stTabControl1.Controls.Add(this.tabPage4);
            this.stTabControl1.Controls.Add(this.tabPage5);
            this.stTabControl1.Controls.Add(this.tabPage7);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl1.Location = new System.Drawing.Point(0, 0);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(549, 740);
            this.stTabControl1.TabIndex = 0;
            this.stTabControl1.SelectedIndexChanged += new System.EventHandler(this.stTabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(541, 711);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(541, 711);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Textures";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(541, 711);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Colors";
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(541, 711);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Fragment";
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(541, 711);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Blending";
            // 
            // tabPage5
            // 
            this.tabPage5.AutoScroll = true;
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(541, 711);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Shaders";
            // 
            // tabPage7
            // 
            this.tabPage7.Location = new System.Drawing.Point(4, 25);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(541, 711);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "User Data";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // BCHMaterialEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.stTabControl1);
            this.Name = "BCHMaterialEditor";
            this.Size = new System.Drawing.Size(549, 740);
            this.stTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
    }
}
