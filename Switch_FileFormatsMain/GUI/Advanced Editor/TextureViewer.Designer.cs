namespace FirstPlugin
{
    partial class TextureViewer
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
            this.textureListView = new Toolbox.Library.Forms.ListViewCustom();
            this.textureContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textureListView
            // 
            this.textureListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureListView.ForeColor = System.Drawing.Color.White;
            this.textureListView.Location = new System.Drawing.Point(0, 0);
            this.textureListView.Name = "textureListView";
            this.textureListView.Size = new System.Drawing.Size(432, 450);
            this.textureListView.TabIndex = 0;
            this.textureListView.UseCompatibleStateImageBehavior = false;
            this.textureListView.SelectedIndexChanged += new System.EventHandler(this.textureListView_SelectedIndexChanged);
            this.textureListView.DoubleClick += new System.EventHandler(this.textureListView_DoubleClick);
            this.textureListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textureListView_MouseClick);
            // 
            // textureContextMenuStrip1
            // 
            this.textureContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.replaceToolStripMenuItem});
            this.textureContextMenuStrip1.Name = "textureContextMenuStrip1";
            this.textureContextMenuStrip1.Size = new System.Drawing.Size(116, 48);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // TextureViewer
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.textureListView);
            this.Name = "TextureViewer";
            this.Size = new System.Drawing.Size(432, 450);
            this.textureContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.ListViewCustom textureListView;
        private System.Windows.Forms.ContextMenuStrip textureContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
    }
}