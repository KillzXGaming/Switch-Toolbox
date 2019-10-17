namespace LayoutBXLYT
{
    partial class PaneMatTextureCombiner
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
            this.tevStageCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.tevColorModeCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.tevAlphaModeCB = new Toolbox.Library.Forms.STComboBox();
            this.tevBasicPanel = new Toolbox.Library.Forms.STPanel();
            this.tevBasicPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tevStageCB
            // 
            this.tevStageCB.BorderColor = System.Drawing.Color.Empty;
            this.tevStageCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.tevStageCB.ButtonColor = System.Drawing.Color.Empty;
            this.tevStageCB.FormattingEnabled = true;
            this.tevStageCB.IsReadOnly = false;
            this.tevStageCB.Location = new System.Drawing.Point(53, 17);
            this.tevStageCB.Name = "tevStageCB";
            this.tevStageCB.Size = new System.Drawing.Size(155, 21);
            this.tevStageCB.TabIndex = 1;
            this.tevStageCB.SelectedIndexChanged += new System.EventHandler(this.tevStageCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(4, 20);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(43, 13);
            this.stLabel1.TabIndex = 2;
            this.stLabel1.Text = "Stages:";
            // 
            // tevColorModeCB
            // 
            this.tevColorModeCB.BorderColor = System.Drawing.Color.Empty;
            this.tevColorModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.tevColorModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.tevColorModeCB.FormattingEnabled = true;
            this.tevColorModeCB.IsReadOnly = false;
            this.tevColorModeCB.Location = new System.Drawing.Point(6, 60);
            this.tevColorModeCB.Name = "tevColorModeCB";
            this.tevColorModeCB.Size = new System.Drawing.Size(155, 21);
            this.tevColorModeCB.TabIndex = 3;
            this.tevColorModeCB.SelectedIndexChanged += new System.EventHandler(this.tevColorModeCB_SelectedIndexChanged);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(3, 33);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(78, 13);
            this.stLabel2.TabIndex = 4;
            this.stLabel2.Text = "Color Blending:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(3, 110);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(81, 13);
            this.stLabel3.TabIndex = 6;
            this.stLabel3.Text = "Alpha Blending:";
            // 
            // tevAlphaModeCB
            // 
            this.tevAlphaModeCB.BorderColor = System.Drawing.Color.Empty;
            this.tevAlphaModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.tevAlphaModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.tevAlphaModeCB.FormattingEnabled = true;
            this.tevAlphaModeCB.IsReadOnly = false;
            this.tevAlphaModeCB.Location = new System.Drawing.Point(6, 137);
            this.tevAlphaModeCB.Name = "tevAlphaModeCB";
            this.tevAlphaModeCB.Size = new System.Drawing.Size(155, 21);
            this.tevAlphaModeCB.TabIndex = 5;
            this.tevAlphaModeCB.SelectedIndexChanged += new System.EventHandler(this.tevAlphaModeCB_SelectedIndexChanged);
            // 
            // tevBasicPanel
            // 
            this.tevBasicPanel.Controls.Add(this.stLabel2);
            this.tevBasicPanel.Controls.Add(this.stLabel3);
            this.tevBasicPanel.Controls.Add(this.tevColorModeCB);
            this.tevBasicPanel.Controls.Add(this.tevAlphaModeCB);
            this.tevBasicPanel.Location = new System.Drawing.Point(3, 44);
            this.tevBasicPanel.Name = "tevBasicPanel";
            this.tevBasicPanel.Size = new System.Drawing.Size(299, 265);
            this.tevBasicPanel.TabIndex = 7;
            // 
            // PaneMatTextureCombiner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tevBasicPanel);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.tevStageCB);
            this.Name = "PaneMatTextureCombiner";
            this.Size = new System.Drawing.Size(305, 312);
            this.tevBasicPanel.ResumeLayout(false);
            this.tevBasicPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STComboBox tevStageCB;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STComboBox tevColorModeCB;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STLabel stLabel3;
        private Toolbox.Library.Forms.STComboBox tevAlphaModeCB;
        private Toolbox.Library.Forms.STPanel tevBasicPanel;
    }
}
