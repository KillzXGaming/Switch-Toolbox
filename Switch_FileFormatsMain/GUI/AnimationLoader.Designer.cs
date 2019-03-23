namespace FirstPlugin.Forms
{
    partial class AnimationLoader
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
            this.btnSave = new Switch_Toolbox.Library.Forms.STButton();
            this.animTreeView = new Switch_Toolbox.Library.TreeViewCustom();
            this.animTypeCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.btnOpen = new Switch_Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.btnOpen);
            this.contentContainer.Controls.Add(this.animTypeCB);
            this.contentContainer.Controls.Add(this.animTreeView);
            this.contentContainer.Controls.Add(this.btnSave);
            this.contentContainer.Size = new System.Drawing.Size(321, 575);
            this.contentContainer.Controls.SetChildIndex(this.btnSave, 0);
            this.contentContainer.Controls.SetChildIndex(this.animTreeView, 0);
            this.contentContainer.Controls.SetChildIndex(this.animTypeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnOpen, 0);
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(93, 31);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // animTreeView
            // 
            this.animTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animTreeView.ImageIndex = 0;
            this.animTreeView.Location = new System.Drawing.Point(12, 88);
            this.animTreeView.Name = "animTreeView";
            this.animTreeView.SelectedImageIndex = 0;
            this.animTreeView.Size = new System.Drawing.Size(306, 483);
            this.animTreeView.TabIndex = 1;
            this.animTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.animTreeView_AfterSelect);
            this.animTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.selectItem);
            // 
            // animTypeCB
            // 
            this.animTypeCB.FormattingEnabled = true;
            this.animTypeCB.Location = new System.Drawing.Point(12, 61);
            this.animTypeCB.Name = "animTypeCB";
            this.animTypeCB.Size = new System.Drawing.Size(305, 21);
            this.animTypeCB.TabIndex = 2;
            this.animTypeCB.SelectedIndexChanged += new System.EventHandler(this.animTypeCB_SelectedIndexChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.Location = new System.Drawing.Point(12, 31);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 3;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // AnimationLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 580);
            this.Name = "AnimationLoader";
            this.Text = "Animation Loader";
            this.contentContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STButton btnSave;
        private Switch_Toolbox.Library.TreeViewCustom animTreeView;
        private Switch_Toolbox.Library.Forms.STComboBox animTypeCB;
        private Switch_Toolbox.Library.Forms.STButton btnOpen;
    }
}