namespace Toolbox.Library.Forms
{
    partial class GamecubeTextureImporterList
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
            this.button2 = new Toolbox.Library.Forms.STButton();
            this.button1 = new Toolbox.Library.Forms.STButton();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Format = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new Toolbox.Library.Forms.STLabel();
            this.label1 = new Toolbox.Library.Forms.STLabel();
            this.MipmapNum = new Toolbox.Library.Forms.STNumbericUpDown();
            this.pictureBox1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.formatComboBox = new Toolbox.Library.Forms.STComboBox();
            this.dataSizeLbl = new Toolbox.Library.Forms.STLabel();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.stLabel3 = new Toolbox.Library.Forms.STLabel();
            this.paletteFormatCB = new Toolbox.Library.Forms.STComboBox();
            this.paletteColorsUD = new Toolbox.Library.Forms.STNumbericUpDown();
            this.stLabel4 = new Toolbox.Library.Forms.STLabel();
            this.stLabel5 = new Toolbox.Library.Forms.STLabel();
            this.paletteAlgorithmCB = new Toolbox.Library.Forms.STComboBox();
            this.HeightLabel = new Toolbox.Library.Forms.STLabel();
            this.WidthLabel = new Toolbox.Library.Forms.STLabel();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteColorsUD)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stLabel5);
            this.contentContainer.Controls.Add(this.paletteAlgorithmCB);
            this.contentContainer.Controls.Add(this.stLabel4);
            this.contentContainer.Controls.Add(this.paletteColorsUD);
            this.contentContainer.Controls.Add(this.stLabel3);
            this.contentContainer.Controls.Add(this.paletteFormatCB);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.dataSizeLbl);
            this.contentContainer.Controls.Add(this.label2);
            this.contentContainer.Controls.Add(this.label1);
            this.contentContainer.Controls.Add(this.MipmapNum);
            this.contentContainer.Controls.Add(this.WidthLabel);
            this.contentContainer.Controls.Add(this.HeightLabel);
            this.contentContainer.Controls.Add(this.pictureBox1);
            this.contentContainer.Controls.Add(this.formatComboBox);
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.contentContainer.Controls.Add(this.button2);
            this.contentContainer.Controls.Add(this.button1);
            this.contentContainer.Size = new System.Drawing.Size(1092, 557);
            this.contentContainer.Controls.SetChildIndex(this.button1, 0);
            this.contentContainer.Controls.SetChildIndex(this.button2, 0);
            this.contentContainer.Controls.SetChildIndex(this.listViewCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.formatComboBox, 0);
            this.contentContainer.Controls.SetChildIndex(this.pictureBox1, 0);
            this.contentContainer.Controls.SetChildIndex(this.HeightLabel, 0);
            this.contentContainer.Controls.SetChildIndex(this.WidthLabel, 0);
            this.contentContainer.Controls.SetChildIndex(this.MipmapNum, 0);
            this.contentContainer.Controls.SetChildIndex(this.label1, 0);
            this.contentContainer.Controls.SetChildIndex(this.label2, 0);
            this.contentContainer.Controls.SetChildIndex(this.dataSizeLbl, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.paletteFormatCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel3, 0);
            this.contentContainer.Controls.SetChildIndex(this.paletteColorsUD, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel4, 0);
            this.contentContainer.Controls.SetChildIndex(this.paletteAlgorithmCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel5, 0);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Name,
            this.Format});
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 25);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(237, 532);
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
            this.Format.Width = 133;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(784, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Format";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(781, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Mip Count:";
            // 
            // MipmapNum
            // 
            this.MipmapNum.Location = new System.Drawing.Point(841, 101);
            this.MipmapNum.Name = "MipmapNum";
            this.MipmapNum.Size = new System.Drawing.Size(133, 20);
            this.MipmapNum.TabIndex = 20;
            this.MipmapNum.ValueChanged += new System.EventHandler(this.MipmapNum_ValueChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::Toolbox.Library.Properties.Resources.CheckerBackground;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(237, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(513, 532);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // formatComboBox
            // 
            this.formatComboBox.BorderColor = System.Drawing.Color.Empty;
            this.formatComboBox.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.formatComboBox.ButtonColor = System.Drawing.Color.Empty;
            this.formatComboBox.FormattingEnabled = true;
            this.formatComboBox.Location = new System.Drawing.Point(841, 64);
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.ReadOnly = true;
            this.formatComboBox.Size = new System.Drawing.Size(133, 21);
            this.formatComboBox.TabIndex = 16;
            this.formatComboBox.SelectedIndexChanged += new System.EventHandler(this.formatComboBox_SelectedIndexChanged);
            // 
            // dataSizeLbl
            // 
            this.dataSizeLbl.AutoSize = true;
            this.dataSizeLbl.Location = new System.Drawing.Point(756, 363);
            this.dataSizeLbl.Name = "dataSizeLbl";
            this.dataSizeLbl.Size = new System.Drawing.Size(56, 13);
            this.dataSizeLbl.TabIndex = 31;
            this.dataSizeLbl.Text = "Data Size:";
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(756, 38);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(36, 13);
            this.stLabel1.TabIndex = 32;
            this.stLabel1.Text = "Image";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(756, 133);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(43, 13);
            this.stLabel2.TabIndex = 33;
            this.stLabel2.Text = "Palette:";
            // 
            // stLabel3
            // 
            this.stLabel3.AutoSize = true;
            this.stLabel3.Location = new System.Drawing.Point(784, 168);
            this.stLabel3.Name = "stLabel3";
            this.stLabel3.Size = new System.Drawing.Size(42, 13);
            this.stLabel3.TabIndex = 35;
            this.stLabel3.Text = "Format:";
            // 
            // paletteFormatCB
            // 
            this.paletteFormatCB.BorderColor = System.Drawing.Color.Empty;
            this.paletteFormatCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.paletteFormatCB.ButtonColor = System.Drawing.Color.Empty;
            this.paletteFormatCB.FormattingEnabled = true;
            this.paletteFormatCB.Location = new System.Drawing.Point(844, 165);
            this.paletteFormatCB.Name = "paletteFormatCB";
            this.paletteFormatCB.ReadOnly = true;
            this.paletteFormatCB.Size = new System.Drawing.Size(130, 21);
            this.paletteFormatCB.TabIndex = 34;
            this.paletteFormatCB.SelectedIndexChanged += new System.EventHandler(this.paletteFormatCB_SelectedIndexChanged);
            // 
            // paletteColorsUD
            // 
            this.paletteColorsUD.Location = new System.Drawing.Point(844, 202);
            this.paletteColorsUD.Name = "paletteColorsUD";
            this.paletteColorsUD.Size = new System.Drawing.Size(130, 20);
            this.paletteColorsUD.TabIndex = 36;
            // 
            // stLabel4
            // 
            this.stLabel4.AutoSize = true;
            this.stLabel4.Location = new System.Drawing.Point(784, 204);
            this.stLabel4.Name = "stLabel4";
            this.stLabel4.Size = new System.Drawing.Size(39, 13);
            this.stLabel4.TabIndex = 37;
            this.stLabel4.Text = "Colors:";
            // 
            // stLabel5
            // 
            this.stLabel5.AutoSize = true;
            this.stLabel5.Location = new System.Drawing.Point(784, 242);
            this.stLabel5.Name = "stLabel5";
            this.stLabel5.Size = new System.Drawing.Size(50, 13);
            this.stLabel5.TabIndex = 39;
            this.stLabel5.Text = "Algorithm";
            // 
            // paletteAlgorithmCB
            // 
            this.paletteAlgorithmCB.BorderColor = System.Drawing.Color.Empty;
            this.paletteAlgorithmCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.paletteAlgorithmCB.ButtonColor = System.Drawing.Color.Empty;
            this.paletteAlgorithmCB.FormattingEnabled = true;
            this.paletteAlgorithmCB.Location = new System.Drawing.Point(844, 239);
            this.paletteAlgorithmCB.Name = "paletteAlgorithmCB";
            this.paletteAlgorithmCB.ReadOnly = true;
            this.paletteAlgorithmCB.Size = new System.Drawing.Size(133, 21);
            this.paletteAlgorithmCB.TabIndex = 38;
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(756, 293);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(38, 13);
            this.HeightLabel.TabIndex = 18;
            this.HeightLabel.Text = "Height";
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(756, 328);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(35, 13);
            this.WidthLabel.TabIndex = 19;
            this.WidthLabel.Text = "Width";
            // 
            // GamecubeTextureImporterList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 560);
            this.KeyPreview = true;
            this.Text = "Texture Importer";
            this.Load += new System.EventHandler(this.BinaryTextureImporterList_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BinaryTextureImporterList_KeyDown);
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteColorsUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STButton button2;
        private Toolbox.Library.Forms.STButton button1;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private Toolbox.Library.Forms.STLabel label2;
        private Toolbox.Library.Forms.STLabel label1;
        private Toolbox.Library.Forms.STNumbericUpDown MipmapNum;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBox1;
        private Toolbox.Library.Forms.STComboBox formatComboBox;
        private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader Format;
        private STLabel dataSizeLbl;
        private STLabel stLabel4;
        private STNumbericUpDown paletteColorsUD;
        private STLabel stLabel3;
        private STComboBox paletteFormatCB;
        private STLabel stLabel2;
        private STLabel stLabel1;
        private STLabel stLabel5;
        private STComboBox paletteAlgorithmCB;
        private STLabel WidthLabel;
        private STLabel HeightLabel;
    }
}