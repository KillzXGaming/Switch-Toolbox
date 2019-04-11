namespace FirstPlugin.Forms
{
    partial class SubFileEditor
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
            this.stTabControl1 = new Switch_Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPropertyGrid1 = new Switch_Toolbox.Library.Forms.STPropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.userDataEditor1 = new UserDataEditor();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Location = new System.Drawing.Point(6, 3);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(414, 589);
            this.stTabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.stPanel2);
            this.tabPage1.Controls.Add(this.splitter1);
            this.tabPage1.Controls.Add(this.stPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(406, 560);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Sub Section";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // stPanel2
            // 
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(3, 286);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(400, 271);
            this.stPanel2.TabIndex = 3;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(3, 283);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(400, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stPropertyGrid1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(3, 3);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(400, 280);
            this.stPanel1.TabIndex = 1;
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.ShowHintDisplay = false;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.Size = new System.Drawing.Size(400, 280);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.userDataEditor1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(406, 560);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "User Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // userDataEditor1
            // 
            this.userDataEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userDataEditor1.Location = new System.Drawing.Point(3, 3);
            this.userDataEditor1.Name = "userDataEditor1";
            this.userDataEditor1.Size = new System.Drawing.Size(400, 554);
            this.userDataEditor1.TabIndex = 1;
            // 
            // SubFileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTabControl1);
            this.Name = "SubFileEditor";
            this.Size = new System.Drawing.Size(420, 592);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private UserDataEditor userDataEditor1;
        private Switch_Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel2;
        private System.Windows.Forms.Splitter splitter1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
    }
}
