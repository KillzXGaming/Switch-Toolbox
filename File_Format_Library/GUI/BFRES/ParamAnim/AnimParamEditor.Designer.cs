namespace FirstPlugin.Forms
{
    partial class AnimParamEditor
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
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnEditSamplers = new Toolbox.Library.Forms.STButton();
            this.btnEditMaterial = new Toolbox.Library.Forms.STButton();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.materialCB = new Toolbox.Library.Forms.STComboBox();
            this.paramCB = new Toolbox.Library.Forms.STComboBox();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.SuspendLayout();
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.Location = new System.Drawing.Point(3, 62);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(424, 346);
            this.listViewCustom1.TabIndex = 0;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Frame";
            this.columnHeader1.Width = 424;
            // 
            // btnEditSamplers
            // 
            this.btnEditSamplers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditSamplers.Location = new System.Drawing.Point(345, 33);
            this.btnEditSamplers.Name = "btnEditSamplers";
            this.btnEditSamplers.Size = new System.Drawing.Size(47, 23);
            this.btnEditSamplers.TabIndex = 21;
            this.btnEditSamplers.Text = "Edit";
            this.btnEditSamplers.UseVisualStyleBackColor = false;
            this.btnEditSamplers.Click += new System.EventHandler(this.btnEditSamplers_Click);
            // 
            // btnEditMaterial
            // 
            this.btnEditMaterial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditMaterial.Location = new System.Drawing.Point(345, 4);
            this.btnEditMaterial.Name = "btnEditMaterial";
            this.btnEditMaterial.Size = new System.Drawing.Size(47, 23);
            this.btnEditMaterial.TabIndex = 20;
            this.btnEditMaterial.Text = "Edit";
            this.btnEditMaterial.UseVisualStyleBackColor = false;
            this.btnEditMaterial.Click += new System.EventHandler(this.btnEditMaterial_Click);
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(49, 6);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(81, 13);
            this.stLabel2.TabIndex = 19;
            this.stLabel2.Text = "Material Target:";
            // 
            // materialCB
            // 
            this.materialCB.BorderColor = System.Drawing.Color.Empty;
            this.materialCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.materialCB.ButtonColor = System.Drawing.Color.Empty;
            this.materialCB.FormattingEnabled = true;
            this.materialCB.Location = new System.Drawing.Point(136, 6);
            this.materialCB.Name = "materialCB";
            this.materialCB.ReadOnly = true;
            this.materialCB.Size = new System.Drawing.Size(203, 21);
            this.materialCB.TabIndex = 18;
            this.materialCB.SelectedIndexChanged += new System.EventHandler(this.materialCB_SelectedIndexChanged);
            // 
            // paramCB
            // 
            this.paramCB.BorderColor = System.Drawing.Color.Empty;
            this.paramCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.paramCB.ButtonColor = System.Drawing.Color.Empty;
            this.paramCB.FormattingEnabled = true;
            this.paramCB.Location = new System.Drawing.Point(136, 35);
            this.paramCB.Name = "paramCB";
            this.paramCB.ReadOnly = true;
            this.paramCB.Size = new System.Drawing.Size(203, 21);
            this.paramCB.TabIndex = 16;
            this.paramCB.SelectedIndexChanged += new System.EventHandler(this.paramCB_SelectedIndexChanged);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(52, 35);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(74, 13);
            this.stLabel1.TabIndex = 17;
            this.stLabel1.Text = "Param Target:";
            // 
            // AnimParamEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEditSamplers);
            this.Controls.Add(this.btnEditMaterial);
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.materialCB);
            this.Controls.Add(this.paramCB);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.listViewCustom1);
            this.Name = "AnimParamEditor";
            this.Size = new System.Drawing.Size(430, 408);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Toolbox.Library.Forms.STButton btnEditSamplers;
        private Toolbox.Library.Forms.STButton btnEditMaterial;
        private Toolbox.Library.Forms.STLabel stLabel2;
        private Toolbox.Library.Forms.STComboBox materialCB;
        private Toolbox.Library.Forms.STComboBox paramCB;
        private Toolbox.Library.Forms.STLabel stLabel1;
    }
}
