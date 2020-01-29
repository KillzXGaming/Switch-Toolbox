namespace FirstPlugin.CtrLibrary.Forms
{
    partial class BCHAnimationEditor
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
            this.bchUserDataEditor1 = new CtrLibrary.Forms.BCHUserDataEditor();
            this.stTabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl1.Location = new System.Drawing.Point(0, 0);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(386, 427);
            this.stTabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(378, 398);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Animation";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.bchUserDataEditor1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(378, 398);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "User Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // bchUserDataEditor1
            // 
            this.bchUserDataEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bchUserDataEditor1.Location = new System.Drawing.Point(3, 3);
            this.bchUserDataEditor1.Name = "bchUserDataEditor1";
            this.bchUserDataEditor1.Size = new System.Drawing.Size(372, 392);
            this.bchUserDataEditor1.TabIndex = 1;
            // 
            // BCHAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTabControl1);
            this.Name = "BCHAnimationEditor";
            this.Size = new System.Drawing.Size(386, 427);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private CtrLibrary.Forms.BCHUserDataEditor bchUserDataEditor1;
    }
}
