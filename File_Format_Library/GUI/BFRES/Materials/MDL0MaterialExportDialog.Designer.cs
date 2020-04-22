namespace FirstPlugin.GUI.BFRES.Materials
{
    partial class MDL0MaterialExportDialog
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
            this.albCheckBox = new Toolbox.Library.Forms.STCheckBox();
            this.emmCheckBox = new Toolbox.Library.Forms.STCheckBox();
            this.bake0CheckBox = new Toolbox.Library.Forms.STCheckBox();
            this.bake1CheckBox = new Toolbox.Library.Forms.STCheckBox();
            this.btnConfirm = new Toolbox.Library.Forms.STButton();
            this.btnCancel = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.btnCancel);
            this.contentContainer.Controls.Add(this.btnConfirm);
            this.contentContainer.Controls.Add(this.bake1CheckBox);
            this.contentContainer.Controls.Add(this.bake0CheckBox);
            this.contentContainer.Controls.Add(this.emmCheckBox);
            this.contentContainer.Controls.Add(this.albCheckBox);
            this.contentContainer.Location = new System.Drawing.Point(0, 0);
            this.contentContainer.Size = new System.Drawing.Size(251, 207);
            this.contentContainer.Controls.SetChildIndex(this.albCheckBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.emmCheckBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.bake0CheckBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.bake1CheckBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnConfirm, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnCancel, 0);
            // 
            // albCheckBox
            // 
            this.albCheckBox.AutoSize = true;
            this.albCheckBox.Checked = true;
            this.albCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.albCheckBox.Location = new System.Drawing.Point(39, 49);
            this.albCheckBox.Name = "albCheckBox";
            this.albCheckBox.Size = new System.Drawing.Size(125, 17);
            this.albCheckBox.TabIndex = 0;
            this.albCheckBox.Text = "Include Diffuse maps";
            this.albCheckBox.UseVisualStyleBackColor = true;
            // 
            // emmCheckBox
            // 
            this.emmCheckBox.AutoSize = true;
            this.emmCheckBox.Checked = true;
            this.emmCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.emmCheckBox.Location = new System.Drawing.Point(39, 73);
            this.emmCheckBox.Name = "emmCheckBox";
            this.emmCheckBox.Size = new System.Drawing.Size(133, 17);
            this.emmCheckBox.TabIndex = 1;
            this.emmCheckBox.Text = "Include Emission maps";
            this.emmCheckBox.UseVisualStyleBackColor = true;
            // 
            // bake0CheckBox
            // 
            this.bake0CheckBox.AutoSize = true;
            this.bake0CheckBox.Checked = true;
            this.bake0CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bake0CheckBox.Location = new System.Drawing.Point(39, 97);
            this.bake0CheckBox.Name = "bake0CheckBox";
            this.bake0CheckBox.Size = new System.Drawing.Size(155, 17);
            this.bake0CheckBox.TabIndex = 2;
            this.bake0CheckBox.Text = "Include Shadow bakemaps";
            this.bake0CheckBox.UseVisualStyleBackColor = true;
            // 
            // bake1CheckBox
            // 
            this.bake1CheckBox.AutoSize = true;
            this.bake1CheckBox.Checked = true;
            this.bake1CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bake1CheckBox.Location = new System.Drawing.Point(39, 121);
            this.bake1CheckBox.Name = "bake1CheckBox";
            this.bake1CheckBox.Size = new System.Drawing.Size(139, 17);
            this.bake1CheckBox.TabIndex = 3;
            this.bake1CheckBox.Text = "Include Light bakemaps";
            this.bake1CheckBox.UseVisualStyleBackColor = true;
            // 
            // btnConfirm
            // 
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Location = new System.Drawing.Point(39, 149);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 4;
            this.btnConfirm.Text = "Export";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(137, 149);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MDL0MaterialExportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 206);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MDL0MaterialExportDialog";
            this.Text = "Export options";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STCheckBox albCheckBox;
        private Toolbox.Library.Forms.STCheckBox emmCheckBox;
        private Toolbox.Library.Forms.STCheckBox bake0CheckBox;
        private Toolbox.Library.Forms.STCheckBox bake1CheckBox;
        private Toolbox.Library.Forms.STButton btnConfirm;
        private Toolbox.Library.Forms.STButton btnCancel;
    }
}