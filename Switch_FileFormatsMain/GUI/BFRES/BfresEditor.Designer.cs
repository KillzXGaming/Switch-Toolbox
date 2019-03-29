namespace FirstPlugin.Forms
{
    partial class BfresEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BfresEditor));
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stTabControl1 = new Switch_Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBoxCustom1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.stPanel5 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stTabControl2 = new Switch_Toolbox.Library.Forms.STTabControl();
            this.timelineTabPage = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.textureLoader1 = new Forms.TextureLoader();
            this.stPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stTabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.splitContainer1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(712, 543);
            this.stPanel2.TabIndex = 4;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitter1);
            this.splitContainer1.Panel1.Controls.Add(this.stPanel3);
            this.splitContainer1.Panel1.Controls.Add(this.stPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(712, 543);
            this.splitContainer1.SplitterDistance = 440;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 354);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(440, 3);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.stTabControl1);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel3.Location = new System.Drawing.Point(0, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(440, 357);
            this.stPanel3.TabIndex = 4;
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
            this.stTabControl1.Size = new System.Drawing.Size(440, 357);
            this.stTabControl1.TabIndex = 0;
            this.stTabControl1.SelectedIndexChanged += new System.EventHandler(this.stTabControl1_SelectedIndexChanged);
            this.stTabControl1.TabIndexChanged += new System.EventHandler(this.stTabControl1_TabIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tabPage1.Controls.Add(this.pictureBoxCustom1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(432, 328);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Node Viewer";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCustom1.Image = global::FirstPlugin.Properties.Resources.GridBackground;
            this.pictureBoxCustom1.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(426, 322);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.stPanel5);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(432, 328);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Model Viewer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // stPanel5
            // 
            this.stPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel5.Location = new System.Drawing.Point(3, 3);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(426, 322);
            this.stPanel5.TabIndex = 2;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stTabControl2);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel1.Location = new System.Drawing.Point(0, 357);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(440, 186);
            this.stPanel1.TabIndex = 3;
            this.stPanel1.DoubleClick += new System.EventHandler(this.stPanel1_DoubleClick);
            // 
            // stTabControl2
            // 
            this.stTabControl2.Controls.Add(this.timelineTabPage);
            this.stTabControl2.Controls.Add(this.tabPage4);
            this.stTabControl2.Controls.Add(this.tabPage5);
            this.stTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl2.Location = new System.Drawing.Point(0, 0);
            this.stTabControl2.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl2.Name = "stTabControl2";
            this.stTabControl2.SelectedIndex = 0;
            this.stTabControl2.Size = new System.Drawing.Size(440, 186);
            this.stTabControl2.TabIndex = 0;
            // 
            // timelineTabPage
            // 
            this.timelineTabPage.Location = new System.Drawing.Point(4, 25);
            this.timelineTabPage.Name = "timelineTabPage";
            this.timelineTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.timelineTabPage.Size = new System.Drawing.Size(432, 157);
            this.timelineTabPage.TabIndex = 3;
            this.timelineTabPage.Text = "Timeline";
            this.timelineTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(432, 157);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Console";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.textureLoader1);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(432, 157);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Textures";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // textureLoader1
            // 
            this.textureLoader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureLoader1.Location = new System.Drawing.Point(3, 3);
            this.textureLoader1.Name = "textureLoader1";
            this.textureLoader1.Size = new System.Drawing.Size(426, 151);
            this.textureLoader1.TabIndex = 0;
            // 
            // BfresEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel2);
            this.Name = "BfresEditor";
            this.Size = new System.Drawing.Size(712, 543);
            this.stPanel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stTabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Switch_Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel2;
        private Switch_Toolbox.Library.Forms.STTabControl stTabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Splitter splitter1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel3;
        private Forms.TextureLoader textureLoader1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel5;
        private System.Windows.Forms.TabPage timelineTabPage;
    }
}
