namespace FirstPlugin.Forms
{
    partial class SamplerEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SamplerEditor));
            this.stLabel14 = new Switch_Toolbox.Library.Forms.STLabel();
            this.textureNameTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.samplerHintTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.textureRefListView = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAdd = new Switch_Toolbox.Library.Forms.STButton();
            this.btnRemove = new Switch_Toolbox.Library.Forms.STButton();
            this.btnEdit = new Switch_Toolbox.Library.Forms.STButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel5 = new Switch_Toolbox.Library.Forms.STPanel();
            this.samplerTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.textureBP = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.stPropertyGrid1 = new Switch_Toolbox.Library.Forms.STPropertyGrid();
            this.stPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureBP)).BeginInit();
            this.SuspendLayout();
            // 
            // stLabel14
            // 
            this.stLabel14.AutoSize = true;
            this.stLabel14.Location = new System.Drawing.Point(104, 130);
            this.stLabel14.Name = "stLabel14";
            this.stLabel14.Size = new System.Drawing.Size(39, 13);
            this.stLabel14.TabIndex = 45;
            this.stLabel14.Text = "Image:";
            // 
            // textureNameTB
            // 
            this.textureNameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textureNameTB.Location = new System.Drawing.Point(75, 42);
            this.textureNameTB.Name = "textureNameTB";
            this.textureNameTB.ReadOnly = true;
            this.textureNameTB.Size = new System.Drawing.Size(219, 20);
            this.textureNameTB.TabIndex = 42;
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(20, 45);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(38, 13);
            this.stLabel3.TabIndex = 41;
            this.stLabel3.Text = "Name:";
            // 
            // samplerHintTB
            // 
            this.samplerHintTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.samplerHintTB.HideSelection = false;
            this.samplerHintTB.Location = new System.Drawing.Point(75, 96);
            this.samplerHintTB.Name = "samplerHintTB";
            this.samplerHintTB.ReadOnly = true;
            this.samplerHintTB.Size = new System.Drawing.Size(219, 20);
            this.samplerHintTB.TabIndex = 40;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(20, 99);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(29, 13);
            this.stLabel2.TabIndex = 39;
            this.stLabel2.Text = "Hint:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(20, 72);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(48, 13);
            this.stLabel1.TabIndex = 38;
            this.stLabel1.Text = "Sampler:";
            // 
            // textureRefListView
            // 
            this.textureRefListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textureRefListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.textureRefListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.textureRefListView.Location = new System.Drawing.Point(0, 0);
            this.textureRefListView.Name = "textureRefListView";
            this.textureRefListView.OwnerDraw = true;
            this.textureRefListView.Size = new System.Drawing.Size(510, 150);
            this.textureRefListView.TabIndex = 35;
            this.textureRefListView.UseCompatibleStateImageBehavior = false;
            this.textureRefListView.View = System.Windows.Forms.View.Details;
            this.textureRefListView.SelectedIndexChanged += new System.EventHandler(this.textureRefListView_SelectedIndexChanged);
            this.textureRefListView.DoubleClick += new System.EventHandler(this.textureRefListView_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Texture";
            this.columnHeader1.Width = 254;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Sampler";
            this.columnHeader2.Width = 77;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Frag Sampler";
            this.columnHeader3.Width = 78;
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(14, 7);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 52;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(95, 7);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 53;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Enabled = false;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Location = new System.Drawing.Point(176, 7);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 55;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.textureRefListView_DoubleClick);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 150);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(510, 3);
            this.splitter1.TabIndex = 56;
            this.splitter1.TabStop = false;
            // 
            // stPanel5
            // 
            this.stPanel5.AutoScroll = true;
            this.stPanel5.Controls.Add(this.samplerTB);
            this.stPanel5.Controls.Add(this.btnEdit);
            this.stPanel5.Controls.Add(this.btnRemove);
            this.stPanel5.Controls.Add(this.btnAdd);
            this.stPanel5.Controls.Add(this.stLabel14);
            this.stPanel5.Controls.Add(this.textureNameTB);
            this.stPanel5.Controls.Add(this.stLabel3);
            this.stPanel5.Controls.Add(this.samplerHintTB);
            this.stPanel5.Controls.Add(this.textureBP);
            this.stPanel5.Controls.Add(this.stLabel2);
            this.stPanel5.Controls.Add(this.stLabel1);
            this.stPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel5.Location = new System.Drawing.Point(0, 153);
            this.stPanel5.Name = "stPanel5";
            this.stPanel5.Size = new System.Drawing.Size(510, 522);
            this.stPanel5.TabIndex = 57;
            // 
            // samplerTB
            // 
            this.samplerTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.samplerTB.Location = new System.Drawing.Point(75, 70);
            this.samplerTB.Name = "samplerTB";
            this.samplerTB.Size = new System.Drawing.Size(219, 20);
            this.samplerTB.TabIndex = 56;
            this.samplerTB.TextChanged += new System.EventHandler(this.samplerTB_TextChanged);
            // 
            // textureBP
            // 
            this.textureBP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureBP.BackColor = System.Drawing.Color.Transparent;
            this.textureBP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textureBP.BackgroundImage")));
            this.textureBP.Location = new System.Drawing.Point(14, 146);
            this.textureBP.MinimumSize = new System.Drawing.Size(280, 365);
            this.textureBP.Name = "textureBP";
            this.textureBP.Size = new System.Drawing.Size(280, 365);
            this.textureBP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.textureBP.TabIndex = 36;
            this.textureBP.TabStop = false;
            this.textureBP.DoubleClick += new System.EventHandler(this.textureRefListView_DoubleClick);
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
            this.stPropertyGrid1.Location = new System.Drawing.Point(300, 153);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.ShowHintDisplay = true;
            this.stPropertyGrid1.Size = new System.Drawing.Size(210, 522);
            this.stPropertyGrid1.TabIndex = 56;
            // 
            // SamplerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPropertyGrid1);
            this.Controls.Add(this.stPanel5);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.textureRefListView);
            this.Name = "SamplerEditor";
            this.Size = new System.Drawing.Size(510, 675);
            this.stPanel5.ResumeLayout(false);
            this.stPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureBP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Switch_Toolbox.Library.Forms.STLabel stLabel14;
        private Switch_Toolbox.Library.Forms.STTextBox textureNameTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel3;
        private Switch_Toolbox.Library.Forms.STTextBox samplerHintTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.PictureBoxCustom textureBP;
        private Switch_Toolbox.Library.Forms.ListViewCustom textureRefListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private Switch_Toolbox.Library.Forms.STButton btnAdd;
        private Switch_Toolbox.Library.Forms.STButton btnRemove;
        private Switch_Toolbox.Library.Forms.STButton btnEdit;
        private System.Windows.Forms.Splitter splitter1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel5;
        private Switch_Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private Switch_Toolbox.Library.Forms.STTextBox samplerTB;
    }
}
