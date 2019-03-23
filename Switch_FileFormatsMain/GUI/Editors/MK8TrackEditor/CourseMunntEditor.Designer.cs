namespace FirstPlugin.Turbo
{
    partial class CourseMunntEditor
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
            this.stTabControl1 = new Switch_Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.contentContainer.SuspendLayout();
            this.stTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stTabControl1);
            this.contentContainer.Controls.SetChildIndex(this.stTabControl1, 0);
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.stTabControl1.Location = new System.Drawing.Point(0, 25);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(215, 368);
            this.stTabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(207, 339);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(207, 339);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // CourseMunntEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 398);
            this.Name = "CourseMunntEditor";
            this.Text = "CourseMunntEditor";
            this.contentContainer.ResumeLayout(false);
            this.stTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}