namespace Toolbox.Library
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
            this.normalsShadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toOriginToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.chkDisplayAllModels = new Toolbox.Library.Forms.STCheckBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.drawContainersCB = new Toolbox.Library.Forms.STComboBox();
            this.panelViewport = new Toolbox.Library.Forms.STPanel();
            this.stContextMenuStrip1 = new Toolbox.Library.Forms.STMenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toOriginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toActiveModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.orbitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.walkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orthographicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perspectiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orientationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetPoseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadShadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uVViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stPanel1.SuspendLayout();
            this.stContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // normalsShadingToolStripMenuItem
            // 
            this.normalsShadingToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.normalsShadingToolStripMenuItem.Image = global::Toolbox.Library.Properties.Resources.normalsSphere;
            this.normalsShadingToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.normalsShadingToolStripMenuItem.Name = "normalsShadingToolStripMenuItem";
            this.normalsShadingToolStripMenuItem.Size = new System.Drawing.Size(204, 46);
            this.normalsShadingToolStripMenuItem.Text = "Normals Shading";
            this.normalsShadingToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toOriginToolStripMenuItem1,
            this.toCenterToolStripMenuItem});
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            // 
            // toOriginToolStripMenuItem1
            // 
            this.toOriginToolStripMenuItem1.Name = "toOriginToolStripMenuItem1";
            this.toOriginToolStripMenuItem1.Size = new System.Drawing.Size(125, 22);
            this.toOriginToolStripMenuItem1.Text = "To Origin";
            // 
            // toCenterToolStripMenuItem
            // 
            this.toCenterToolStripMenuItem.Name = "toCenterToolStripMenuItem";
            this.toCenterToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.toCenterToolStripMenuItem.Text = "To Center";
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Controls.Add(this.chkDisplayAllModels);
            this.stPanel1.Controls.Add(this.stLabel1);
            this.stPanel1.Controls.Add(this.drawContainersCB);
            this.stPanel1.Location = new System.Drawing.Point(0, 26);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(778, 24);
            this.stPanel1.TabIndex = 5;
            // 
            // chkDisplayAllModels
            // 
            this.chkDisplayAllModels.AutoSize = true;
            this.chkDisplayAllModels.Location = new System.Drawing.Point(282, 2);
            this.chkDisplayAllModels.Name = "chkDisplayAllModels";
            this.chkDisplayAllModels.Size = new System.Drawing.Size(74, 17);
            this.chkDisplayAllModels.TabIndex = 2;
            this.chkDisplayAllModels.Text = "Display All";
            this.chkDisplayAllModels.UseVisualStyleBackColor = true;
            this.chkDisplayAllModels.CheckedChanged += new System.EventHandler(this.chkDisplayAllModels_CheckedChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(3, 3);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(83, 13);
            this.stLabel1.TabIndex = 1;
            this.stLabel1.Text = "Active Model(s):";
            // 
            // drawContainersCB
            // 
            this.drawContainersCB.BorderColor = System.Drawing.Color.Empty;
            this.drawContainersCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.drawContainersCB.ButtonColor = System.Drawing.Color.Empty;
            this.drawContainersCB.FormattingEnabled = true;
            this.drawContainersCB.IsReadOnly = false;
            this.drawContainersCB.Location = new System.Drawing.Point(93, 0);
            this.drawContainersCB.Name = "drawContainersCB";
            this.drawContainersCB.Size = new System.Drawing.Size(183, 21);
            this.drawContainersCB.TabIndex = 0;
            this.drawContainersCB.SelectedIndexChanged += new System.EventHandler(this.drawContainersCB_SelectedIndexChanged);
            this.drawContainersCB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.drawContainersCB_MouseDown);
            // 
            // panelViewport
            // 
            this.panelViewport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelViewport.Location = new System.Drawing.Point(0, 48);
            this.panelViewport.Name = "panelViewport";
            this.panelViewport.Size = new System.Drawing.Size(781, 474);
            this.panelViewport.TabIndex = 3;
            // 
            // stContextMenuStrip1
            // 
            this.stContextMenuStrip1.HighlightSelectedTab = false;
            this.stContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.shadingToolStripMenuItem,
            this.cameraToolStripMenuItem1,
            this.resetPoseToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.uVViewerToolStripMenuItem});
            this.stContextMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.stContextMenuStrip1.Name = "stContextMenuStrip1";
            this.stContextMenuStrip1.Size = new System.Drawing.Size(781, 24);
            this.stContextMenuStrip1.TabIndex = 4;
            this.stContextMenuStrip1.Text = "stContextMenuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // shadingToolStripMenuItem
            // 
            this.shadingToolStripMenuItem.Name = "shadingToolStripMenuItem";
            this.shadingToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.shadingToolStripMenuItem.Text = "Shading";
            this.shadingToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.shadingToolStripMenuItem_DropDownItemClicked);
            // 
            // cameraToolStripMenuItem1
            // 
            this.cameraToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetCameraToolStripMenuItem,
            this.modeToolStripMenuItem1,
            this.projectionToolStripMenuItem,
            this.orientationToolStripMenuItem,
            this.createScreenshotToolStripMenuItem});
            this.cameraToolStripMenuItem1.Name = "cameraToolStripMenuItem1";
            this.cameraToolStripMenuItem1.Size = new System.Drawing.Size(60, 20);
            this.cameraToolStripMenuItem1.Text = "Camera";
            // 
            // resetCameraToolStripMenuItem
            // 
            this.resetCameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toOriginToolStripMenuItem,
            this.toActiveModelToolStripMenuItem});
            this.resetCameraToolStripMenuItem.Name = "resetCameraToolStripMenuItem";
            this.resetCameraToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.resetCameraToolStripMenuItem.Text = "Reset";
            // 
            // toOriginToolStripMenuItem
            // 
            this.toOriginToolStripMenuItem.Name = "toOriginToolStripMenuItem";
            this.toOriginToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.toOriginToolStripMenuItem.Text = "To Origin";
            this.toOriginToolStripMenuItem.Click += new System.EventHandler(this.toOriginToolStripMenuItem_Click);
            // 
            // toActiveModelToolStripMenuItem
            // 
            this.toActiveModelToolStripMenuItem.Name = "toActiveModelToolStripMenuItem";
            this.toActiveModelToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.toActiveModelToolStripMenuItem.Text = "To Active Model";
            this.toActiveModelToolStripMenuItem.Click += new System.EventHandler(this.toActiveModelToolStripMenuItem_Click);
            // 
            // modeToolStripMenuItem1
            // 
            this.modeToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.orbitToolStripMenuItem,
            this.walkToolStripMenuItem});
            this.modeToolStripMenuItem1.Name = "modeToolStripMenuItem1";
            this.modeToolStripMenuItem1.Size = new System.Drawing.Size(169, 22);
            this.modeToolStripMenuItem1.Text = "Mode";
            // 
            // orbitToolStripMenuItem
            // 
            this.orbitToolStripMenuItem.Name = "orbitToolStripMenuItem";
            this.orbitToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.orbitToolStripMenuItem.Text = "Orbit";
            this.orbitToolStripMenuItem.Click += new System.EventHandler(this.orbitToolStripMenuItem_Click);
            // 
            // walkToolStripMenuItem
            // 
            this.walkToolStripMenuItem.Name = "walkToolStripMenuItem";
            this.walkToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.walkToolStripMenuItem.Text = "Walk";
            this.walkToolStripMenuItem.Click += new System.EventHandler(this.walkToolStripMenuItem_Click);
            // 
            // projectionToolStripMenuItem
            // 
            this.projectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.orthographicToolStripMenuItem,
            this.perspectiveToolStripMenuItem});
            this.projectionToolStripMenuItem.Name = "projectionToolStripMenuItem";
            this.projectionToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.projectionToolStripMenuItem.Text = "Projection";
            // 
            // orthographicToolStripMenuItem
            // 
            this.orthographicToolStripMenuItem.Name = "orthographicToolStripMenuItem";
            this.orthographicToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.orthographicToolStripMenuItem.Text = "Orthographic";
            this.orthographicToolStripMenuItem.Click += new System.EventHandler(this.orthographicToolStripMenuItem_Click);
            // 
            // perspectiveToolStripMenuItem
            // 
            this.perspectiveToolStripMenuItem.Name = "perspectiveToolStripMenuItem";
            this.perspectiveToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.perspectiveToolStripMenuItem.Text = "Perspective";
            this.perspectiveToolStripMenuItem.Click += new System.EventHandler(this.perspectiveToolStripMenuItem_Click);
            // 
            // orientationToolStripMenuItem
            // 
            this.orientationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frontToolStripMenuItem,
            this.backToolStripMenuItem,
            this.topToolStripMenuItem,
            this.bottomToolStripMenuItem,
            this.rightToolStripMenuItem,
            this.leftToolStripMenuItem});
            this.orientationToolStripMenuItem.Name = "orientationToolStripMenuItem";
            this.orientationToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.orientationToolStripMenuItem.Text = "Orientation";
            // 
            // frontToolStripMenuItem
            // 
            this.frontToolStripMenuItem.Name = "frontToolStripMenuItem";
            this.frontToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.frontToolStripMenuItem.Text = "Front";
            this.frontToolStripMenuItem.Click += new System.EventHandler(this.frontToolStripMenuItem_Click);
            // 
            // backToolStripMenuItem
            // 
            this.backToolStripMenuItem.Name = "backToolStripMenuItem";
            this.backToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.backToolStripMenuItem.Text = "Back";
            this.backToolStripMenuItem.Click += new System.EventHandler(this.backToolStripMenuItem_Click);
            // 
            // topToolStripMenuItem
            // 
            this.topToolStripMenuItem.Name = "topToolStripMenuItem";
            this.topToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.topToolStripMenuItem.Text = "Top";
            this.topToolStripMenuItem.Click += new System.EventHandler(this.topToolStripMenuItem_Click);
            // 
            // bottomToolStripMenuItem
            // 
            this.bottomToolStripMenuItem.Name = "bottomToolStripMenuItem";
            this.bottomToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.bottomToolStripMenuItem.Text = "Bottom";
            this.bottomToolStripMenuItem.Click += new System.EventHandler(this.bottomToolStripMenuItem_Click);
            // 
            // rightToolStripMenuItem
            // 
            this.rightToolStripMenuItem.Name = "rightToolStripMenuItem";
            this.rightToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.rightToolStripMenuItem.Text = "Right";
            this.rightToolStripMenuItem.Click += new System.EventHandler(this.rightToolStripMenuItem_Click);
            // 
            // leftToolStripMenuItem
            // 
            this.leftToolStripMenuItem.Name = "leftToolStripMenuItem";
            this.leftToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.leftToolStripMenuItem.Text = "Left";
            this.leftToolStripMenuItem.Click += new System.EventHandler(this.leftToolStripMenuItem_Click);
            // 
            // createScreenshotToolStripMenuItem
            // 
            this.createScreenshotToolStripMenuItem.Name = "createScreenshotToolStripMenuItem";
            this.createScreenshotToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.createScreenshotToolStripMenuItem.Text = "Create Screenshot";
            this.createScreenshotToolStripMenuItem.Click += new System.EventHandler(this.createScreenshotToolStripMenuItem_Click);
            // 
            // resetPoseToolStripMenuItem
            // 
            this.resetPoseToolStripMenuItem.Name = "resetPoseToolStripMenuItem";
            this.resetPoseToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.resetPoseToolStripMenuItem.Text = "Reset Pose";
            this.resetPoseToolStripMenuItem.Click += new System.EventHandler(this.resetPoseToolStripMenuItem_Click);
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
            this.reloadShadersToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.reloadShadersToolStripMenuItem.Text = "Reload Shaders";
            this.reloadShadersToolStripMenuItem.Click += new System.EventHandler(this.reloadShadersToolStripMenuItem_Click);
            // 
            // uVViewerToolStripMenuItem
            // 
            this.uVViewerToolStripMenuItem.Name = "uVViewerToolStripMenuItem";
            this.uVViewerToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.uVViewerToolStripMenuItem.Text = "UV Viewer";
            this.uVViewerToolStripMenuItem.Click += new System.EventHandler(this.uVViewerToolStripMenuItem_Click);
            // 
            // Viewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.panelViewport);
            this.Controls.Add(this.stContextMenuStrip1);
            this.Name = "Viewport";
            this.Size = new System.Drawing.Size(781, 522);
            this.stPanel1.ResumeLayout(false);
            this.stPanel1.PerformLayout();
            this.stContextMenuStrip1.ResumeLayout(false);
            this.stContextMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem normalsShadingToolStripMenuItem;
        private Forms.STPanel panelViewport;
        private Forms.STMenuStrip stContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadShadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetPoseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem perspectiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orthographicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orientationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toOriginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toActiveModelToolStripMenuItem;
        private Forms.STComboBox drawContainersCB;
        private Forms.STPanel stPanel1;
        private Forms.STLabel stLabel1;
        private Forms.STCheckBox chkDisplayAllModels;
        private System.Windows.Forms.ToolStripMenuItem uVViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toOriginToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toCenterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem orbitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem walkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem walkToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem createScreenshotToolStripMenuItem;
    }
}