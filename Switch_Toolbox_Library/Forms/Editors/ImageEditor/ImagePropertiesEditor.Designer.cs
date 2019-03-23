namespace Switch_Toolbox.Library.Forms
{
    partial class ImagePropertiesEditor
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
            this.channelListView = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.ChannelsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stTabControl1 = new Switch_Toolbox.Library.Forms.STTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.stPropertyGrid1 = new Switch_Toolbox.Library.Forms.STPropertyGrid();
            this.stTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // channelListView
            // 
            this.channelListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.channelListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ChannelsColumn});
            this.channelListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.channelListView.Location = new System.Drawing.Point(3, 3);
            this.channelListView.Name = "channelListView";
            this.channelListView.OwnerDraw = true;
            this.channelListView.Size = new System.Drawing.Size(219, 508);
            this.channelListView.TabIndex = 0;
            this.channelListView.UseCompatibleStateImageBehavior = false;
            this.channelListView.View = System.Windows.Forms.View.Details;
            this.channelListView.SelectedIndexChanged += new System.EventHandler(this.channelListView_SelectedIndexChanged);
            // 
            // ChannelsColumn
            // 
            this.ChannelsColumn.Text = "Channels";
            this.ChannelsColumn.Width = 219;
            // 
            // stTabControl1
            // 
            this.stTabControl1.Controls.Add(this.tabPage2);
            this.stTabControl1.Controls.Add(this.tabPage1);
            this.stTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stTabControl1.Location = new System.Drawing.Point(0, 0);
            this.stTabControl1.myBackColor = System.Drawing.Color.Empty;
            this.stTabControl1.Name = "stTabControl1";
            this.stTabControl1.SelectedIndex = 0;
            this.stTabControl1.Size = new System.Drawing.Size(233, 543);
            this.stTabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.channelListView);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(225, 514);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Channels";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.stPropertyGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(225, 514);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Properties";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.Size = new System.Drawing.Size(219, 508);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // ImagePropertiesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stTabControl1);
            this.Name = "ImagePropertiesEditor";
            this.Size = new System.Drawing.Size(233, 543);
            this.stTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public ListViewCustom channelListView;
        private STTabControl stTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ColumnHeader ChannelsColumn;
        private STPropertyGrid stPropertyGrid1;
    }
}