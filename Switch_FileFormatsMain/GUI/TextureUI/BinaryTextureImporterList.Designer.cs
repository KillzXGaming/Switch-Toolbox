namespace FirstPlugin
{
    partial class BinaryTextureImporterList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BinaryTextureImporterList));
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Format = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SwizzleNum = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.tileModeCB = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ImgDimComb = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MipmapNum = new System.Windows.Forms.NumericUpDown();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.formatComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.SwizzleNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(1009, 517);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(79, 33);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(911, 517);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 33);
            this.button1.TabIndex = 4;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Name,
            this.Format});
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listViewCustom1.ForeColor = System.Drawing.Color.White;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.Size = new System.Drawing.Size(237, 562);
            this.listViewCustom1.TabIndex = 6;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            // 
            // Name
            // 
            this.Name.Text = "Name";
            this.Name.Width = 104;
            // 
            // Format
            // 
            this.Format.Text = "Format";
            // 
            // SwizzleNum
            // 
            this.SwizzleNum.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SwizzleNum.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SwizzleNum.ForeColor = System.Drawing.Color.White;
            this.SwizzleNum.Location = new System.Drawing.Point(855, 152);
            this.SwizzleNum.Name = "SwizzleNum";
            this.SwizzleNum.Size = new System.Drawing.Size(130, 16);
            this.SwizzleNum.TabIndex = 28;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(759, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Swizzle";
            // 
            // tileModeCB
            // 
            this.tileModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tileModeCB.FormattingEnabled = true;
            this.tileModeCB.Location = new System.Drawing.Point(855, 79);
            this.tileModeCB.Name = "tileModeCB";
            this.tileModeCB.Size = new System.Drawing.Size(233, 21);
            this.tileModeCB.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(759, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Tile Mode";
            // 
            // ImgDimComb
            // 
            this.ImgDimComb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ImgDimComb.FormattingEnabled = true;
            this.ImgDimComb.Location = new System.Drawing.Point(853, 46);
            this.ImgDimComb.Name = "ImgDimComb";
            this.ImgDimComb.Size = new System.Drawing.Size(233, 21);
            this.ImgDimComb.TabIndex = 24;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(759, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Image Dimension";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(756, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Format";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(759, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Number MipMaps";
            // 
            // MipmapNum
            // 
            this.MipmapNum.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MipmapNum.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MipmapNum.ForeColor = System.Drawing.Color.White;
            this.MipmapNum.Location = new System.Drawing.Point(855, 115);
            this.MipmapNum.Name = "MipmapNum";
            this.MipmapNum.Size = new System.Drawing.Size(130, 16);
            this.MipmapNum.TabIndex = 20;
            this.MipmapNum.ValueChanged += new System.EventHandler(this.MipmapNum_ValueChanged);
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(759, 220);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(35, 13);
            this.WidthLabel.TabIndex = 19;
            this.WidthLabel.Text = "Width";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(759, 185);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(38, 13);
            this.HeightLabel.TabIndex = 18;
            this.HeightLabel.Text = "Height";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(237, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(513, 562);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // formatComboBox
            // 
            this.formatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatComboBox.FormattingEnabled = true;
            this.formatComboBox.Location = new System.Drawing.Point(853, 16);
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.Size = new System.Drawing.Size(233, 21);
            this.formatComboBox.TabIndex = 16;
            this.formatComboBox.SelectedIndexChanged += new System.EventHandler(this.formatComboBox_SelectedIndexChanged);
            // 
            // BinaryTextureImporterList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1100, 562);
            this.Controls.Add(this.SwizzleNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tileModeCB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ImgDimComb);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MipmapNum);
            this.Controls.Add(this.WidthLabel);
            this.Controls.Add(this.HeightLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.formatComboBox);
            this.Controls.Add(this.listViewCustom1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.ForeColor = System.Drawing.Color.White;
            this.Text = "Texture Importer";
            this.Load += new System.EventHandler(this.BinaryTextureImporterList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SwizzleNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.NumericUpDown SwizzleNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox tileModeCB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ImgDimComb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown MipmapNum;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label HeightLabel;
        private Switch_Toolbox.Library.Forms.PictureBoxCustom pictureBox1;
        private System.Windows.Forms.ComboBox formatComboBox;
        private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader Format;
    }
}