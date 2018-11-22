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
            this.gL_ControlModern1 = new GL_Core.GL_ControlModern();
            this.animationPanel1 = new Switch_Toolbox.Library.AnimationPanel();
            this.contextMenuStripDark1 = new Switch_Toolbox.Library.Forms.ContextMenuStripDark();
            this.shadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.translateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalsShadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripDark1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gL_ControlModern1
            // 
            this.gL_ControlModern1.ActiveCamera = null;
            this.gL_ControlModern1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gL_ControlModern1.BackColor = System.Drawing.Color.Black;
            this.gL_ControlModern1.CurrentShader = null;
            this.gL_ControlModern1.DisplayPolyCount = false;
            this.gL_ControlModern1.Location = new System.Drawing.Point(-3, 66);
            this.gL_ControlModern1.MainDrawable = null;
            this.gL_ControlModern1.Name = "gL_ControlModern1";
            this.gL_ControlModern1.Size = new System.Drawing.Size(791, 388);
            this.gL_ControlModern1.Stereoscopy = false;
            this.gL_ControlModern1.TabIndex = 0;
            this.gL_ControlModern1.VSync = false;
            // 
            // animationPanel1
            // 
            this.animationPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.animationPanel1.CurrentAnimation = null;
            this.animationPanel1.Location = new System.Drawing.Point(-3, 450);
            this.animationPanel1.Name = "animationPanel1";
            this.animationPanel1.Size = new System.Drawing.Size(791, 69);
            this.animationPanel1.TabIndex = 1;
            // 
            // contextMenuStripDark1
            // 
            this.contextMenuStripDark1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.contextMenuStripDark1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.contextMenuStripDark1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shadingToolStripMenuItem,
            this.translateToolStripMenuItem,
            this.rotateToolStripMenuItem,
            this.scaleToolStripMenuItem});
            this.contextMenuStripDark1.Location = new System.Drawing.Point(0, 0);
            this.contextMenuStripDark1.Name = "contextMenuStripDark1";
            this.contextMenuStripDark1.Size = new System.Drawing.Size(781, 63);
            this.contextMenuStripDark1.TabIndex = 2;
            this.contextMenuStripDark1.Text = "contextMenuStripDark1";
            this.contextMenuStripDark1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripDark1_ItemClicked);
            // 
            // shadingToolStripMenuItem
            // 
            this.shadingToolStripMenuItem.Image = global::Switch_Toolbox.Library.Properties.Resources.diffuseSphere;
            this.shadingToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.shadingToolStripMenuItem.Name = "shadingToolStripMenuItem";
            this.shadingToolStripMenuItem.Size = new System.Drawing.Size(103, 59);
            this.shadingToolStripMenuItem.Text = "Default Shading";
            this.shadingToolStripMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.shadingToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.shadingToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.shadingToolStripMenuItem_DropDownItemClicked);
            // 
            // translateToolStripMenuItem
            // 
            this.translateToolStripMenuItem.Image = global::Switch_Toolbox.Library.Properties.Resources.translateGizmo;
            this.translateToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.translateToolStripMenuItem.Name = "translateToolStripMenuItem";
            this.translateToolStripMenuItem.Size = new System.Drawing.Size(66, 59);
            this.translateToolStripMenuItem.Text = "Translate";
            this.translateToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // rotateToolStripMenuItem
            // 
            this.rotateToolStripMenuItem.Image = global::Switch_Toolbox.Library.Properties.Resources.rotateGizmo;
            this.rotateToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
            this.rotateToolStripMenuItem.Size = new System.Drawing.Size(53, 59);
            this.rotateToolStripMenuItem.Text = "Rotate";
            this.rotateToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // scaleToolStripMenuItem
            // 
            this.scaleToolStripMenuItem.Image = global::Switch_Toolbox.Library.Properties.Resources.scaleGizmo;
            this.scaleToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
            this.scaleToolStripMenuItem.Size = new System.Drawing.Size(52, 59);
            this.scaleToolStripMenuItem.Text = "Scale";
            this.scaleToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
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
            // Viewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(781, 522);
            this.Controls.Add(this.animationPanel1);
            this.Controls.Add(this.gL_ControlModern1);
            this.Controls.Add(this.contextMenuStripDark1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.MainMenuStrip = this.contextMenuStripDark1;
            this.Name = "Viewport";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Unknown;
            this.Text = "Viewport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Viewport_FormClosing);
            this.contextMenuStripDark1.ResumeLayout(false);
            this.contextMenuStripDark1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public GL_Core.GL_ControlModern gL_ControlModern1;
        public AnimationPanel animationPanel1;
        private Forms.ContextMenuStripDark contextMenuStripDark1;
        private System.Windows.Forms.ToolStripMenuItem shadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem translateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalsShadingToolStripMenuItem;
    }
}