namespace Switch_Toolbox.Library
{
    partial class Viewport
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
            this.animationPanel1 = new Switch_Toolbox.Library.AnimationPanel();
            this.normalsShadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelViewport = new Switch_Toolbox.Library.Forms.STPanel();
            this.stContextMenuStrip1 = new Switch_Toolbox.Library.Forms.STMenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vewportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadShadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // animationPanel1
            // 
            this.animationPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.animationPanel1.CurrentAnimation = null;
            this.animationPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.animationPanel1.Location = new System.Drawing.Point(0, 422);
            this.animationPanel1.Name = "animationPanel1";
            this.animationPanel1.Size = new System.Drawing.Size(781, 100);
            this.animationPanel1.TabIndex = 1;
            // 
            // normalsShadingToolStripMenuItem
            // 
            this.normalsShadingToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.normalsShadingToolStripMenuItem.Image = global::Switch_Toolbox.Library.Properties.Resources.normalsSphere;
            this.normalsShadingToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.normalsShadingToolStripMenuItem.Name = "normalsShadingToolStripMenuItem";
            this.normalsShadingToolStripMenuItem.Size = new System.Drawing.Size(204, 46);
            this.normalsShadingToolStripMenuItem.Text = "Normals Shading";
            this.normalsShadingToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // panelViewport
            // 
            this.panelViewport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelViewport.Location = new System.Drawing.Point(0, 24);
            this.panelViewport.Name = "panelViewport";
            this.panelViewport.Size = new System.Drawing.Size(781, 498);
            this.panelViewport.TabIndex = 3;
            // 
            // stContextMenuStrip1
            // 
            this.stContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.shadingToolStripMenuItem,
            this.resetCameraToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.stContextMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stContextMenuStrip1.Name = "stContextMenuStrip1";
            this.stContextMenuStrip1.Size = new System.Drawing.Size(781, 24);
            this.stContextMenuStrip1.TabIndex = 4;
            this.stContextMenuStrip1.Text = "stContextMenuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vewportToolStripMenuItem,
            this.modelToolStripMenuItem,
            this.animationToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // vewportToolStripMenuItem
            // 
            this.vewportToolStripMenuItem.Name = "vewportToolStripMenuItem";
            this.vewportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.vewportToolStripMenuItem.Text = "Vewport";
            // 
            // modelToolStripMenuItem
            // 
            this.modelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bonesToolStripMenuItem});
            this.modelToolStripMenuItem.Name = "modelToolStripMenuItem";
            this.modelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.modelToolStripMenuItem.Text = "Model";
            // 
            // bonesToolStripMenuItem
            // 
            this.bonesToolStripMenuItem.Name = "bonesToolStripMenuItem";
            this.bonesToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.bonesToolStripMenuItem.Text = "Bones";
            // 
            // animationToolStripMenuItem
            // 
            this.animationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animationPanelToolStripMenuItem});
            this.animationToolStripMenuItem.Name = "animationToolStripMenuItem";
            this.animationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.animationToolStripMenuItem.Text = "Animation";
            // 
            // animationPanelToolStripMenuItem
            // 
            this.animationPanelToolStripMenuItem.Checked = true;
            this.animationPanelToolStripMenuItem.CheckOnClick = true;
            this.animationPanelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.animationPanelToolStripMenuItem.Name = "animationPanelToolStripMenuItem";
            this.animationPanelToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.animationPanelToolStripMenuItem.Text = "Timeline";
            this.animationPanelToolStripMenuItem.Click += new System.EventHandler(this.animationPanelToolStripMenuItem_Click);
            // 
            // shadingToolStripMenuItem
            // 
            this.shadingToolStripMenuItem.Name = "shadingToolStripMenuItem";
            this.shadingToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.shadingToolStripMenuItem.Text = "Shading";
            this.shadingToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.shadingToolStripMenuItem_DropDownItemClicked);
            // 
            // resetCameraToolStripMenuItem
            // 
            this.resetCameraToolStripMenuItem.Name = "resetCameraToolStripMenuItem";
            this.resetCameraToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.resetCameraToolStripMenuItem.Text = "Reset Camera";
            this.resetCameraToolStripMenuItem.Click += new System.EventHandler(this.resetCameraToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadShadersToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // reloadShadersToolStripMenuItem
            // 
            this.reloadShadersToolStripMenuItem.Name = "reloadShadersToolStripMenuItem";
            this.reloadShadersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.reloadShadersToolStripMenuItem.Text = "Reload Shaders";
            this.reloadShadersToolStripMenuItem.Click += new System.EventHandler(this.reloadShadersToolStripMenuItem_Click);
            // 
            // Viewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.panelViewport);
            this.Controls.Add(this.stContextMenuStrip1);
            this.Name = "Viewport";
            this.Size = new System.Drawing.Size(781, 522);
            this.stContextMenuStrip1.ResumeLayout(false);
            this.stContextMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public AnimationPanel animationPanel1;
        private System.Windows.Forms.ToolStripMenuItem normalsShadingToolStripMenuItem;
        private Forms.STPanel panelViewport;
        private Forms.STMenuStrip stContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vewportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bonesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animationPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadShadersToolStripMenuItem;
    }
}