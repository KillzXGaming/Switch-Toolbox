using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class CTR_3DSTextureImporter : STForm
    {
        public bool OverrideMipCounter = false;

        bool IsLoaded = false;
        public CTR_3DSTextureImporter()
        {
            InitializeComponent();

            CanResize = false;

            listViewCustom1.FullRowSelect = true;
            listViewCustom1.CanResizeList = true;

            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.A4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.A8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.ETC1);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.ETC1A4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.HiLo8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.L4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.L8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.LA4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.LA8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.LA4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.LA8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGB565);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGB8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGBA4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGBA5551);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGBA8);

            IsLoaded = true;
        }

        public bool ReadOnlySwizzle
        {
            set
            {
                SwizzleNum.ReadOnly = value;
            }
        }

        public bool ReadOnlyTileMode
        {
            set
            {
                tileModeCB.ReadOnly = value;
            }
        }

        public bool ReadOnlyFormat
        {
            set
            {
                formatComboBox.ReadOnly = value;
            }
        }

        public void LoadSupportedFormats(TEX_FORMAT[] Formats)
        {
            formatComboBox.Items.Clear();
            foreach (TEX_FORMAT format in Formats)
            {
                var CtrFormat = CTR_3DS.ConvertToPICAFormat(format);
                formatComboBox.Items.Add(CtrFormat);
            }

            var CtrDefaultFormat = CTR_3DS.ConvertToPICAFormat(Runtime.PreferredTexFormat);

            if (formatComboBox.Items.Contains(CtrDefaultFormat))
                formatComboBox.SelectedItem = CtrDefaultFormat;
        }

        CTR_3DSImporterSettings SelectedTexSettings;

        List<CTR_3DSImporterSettings> settings = new List<CTR_3DSImporterSettings>();
        public void LoadSettings(List<CTR_3DSImporterSettings> s)
        {
            settings = s;

            foreach (var setting in settings)
            {
                listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            }
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }
        public void LoadSetting(CTR_3DSImporterSettings setting)
        {
            settings.Add(setting);

            listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }

        private Thread Thread;
        public void SetupSettings()
        {
            if (SelectedTexSettings.Format == 0)
                return;

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            if (formatComboBox.SelectedItem is CTR_3DS.PICASurfaceFormat)
            {
                SelectedTexSettings.Format = (CTR_3DS.PICASurfaceFormat)formatComboBox.SelectedItem;
                listViewCustom1.SelectedItems[0].SubItems[1].Text = SelectedTexSettings.Format.ToString();
            }
            HeightLabel.Text = $"Height: {SelectedTexSettings.TexHeight}";
            WidthLabel.Text = $"Width: {SelectedTexSettings.TexWidth}";

            Bitmap bitmap = Switch_Toolbox.Library.Imaging.GetLoadingImage();

            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBox1.Image = bitmap;
                SelectedTexSettings.Compress();

                pictureBox1.Image = bitmap;

            }));
            Thread.Start();
        }

        private void tileModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tileModeCB.SelectedIndex > -1 && SelectedTexSettings != null)
                SelectedTexSettings.tileMode = (uint)(GX2.GX2TileMode)tileModeCB.SelectedItem;

            if (tileModeCB.SelectedIndex != 0 && IsLoaded)
            {
                var result = MessageBox.Show("Warning! Only change the tile mode unless you know what you are doing!", "Texture Importer", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel)
                    tileModeCB.SelectedIndex = 0;
            }
        }

        bool DialogShown = false;
        private void formatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formatComboBox.SelectedIndex > -1 && SelectedTexSettings != null)
            {
                SetupSettings();
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                SelectedTexSettings = settings[listViewCustom1.SelectedIndices[0]];
                formatComboBox.SelectedItem = SelectedTexSettings.Format;

                SetupSettings();

                MipmapNum.Maximum = SelectedTexSettings.GetTotalMipCount() + 1;

                //Force the mip counter to be the selected mip counter
                //Some textures like bflim (used for UI) only have 1
                if (OverrideMipCounter)
                {
                    MipmapNum.Maximum = SelectedTexSettings.MipCount;
                    MipmapNum.Minimum = SelectedTexSettings.MipCount;
                }

                MipmapNum.Value = SelectedTexSettings.MipCount;

                SwizzleNum.Value = (SelectedTexSettings.Swizzle >> 8) & 7;
            }
        }

        private void SwizzleNum_ValueChanged(object sender, EventArgs e) {
            SelectedTexSettings.Swizzle &= GX2.SwizzleMask;
            SelectedTexSettings.Swizzle |= (uint)SwizzleNum.Value << 8;
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GTXTextureImporter));
            this.SwizzleNum = new Switch_Toolbox.Library.Forms.STNumbericUpDown();
            this.label5 = new Switch_Toolbox.Library.Forms.STLabel();
            this.tileModeCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.label4 = new Switch_Toolbox.Library.Forms.STLabel();
            this.ImgDimComb = new Switch_Toolbox.Library.Forms.STComboBox();
            this.label3 = new Switch_Toolbox.Library.Forms.STLabel();
            this.label2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.label1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.MipmapNum = new Switch_Toolbox.Library.Forms.STNumbericUpDown();
            this.WidthLabel = new Switch_Toolbox.Library.Forms.STLabel();
            this.HeightLabel = new Switch_Toolbox.Library.Forms.STLabel();
            this.formatComboBox = new Switch_Toolbox.Library.Forms.STComboBox();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Format = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button2 = new Switch_Toolbox.Library.Forms.STButton();
            this.button1 = new Switch_Toolbox.Library.Forms.STButton();
            this.pictureBox1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SwizzleNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.SwizzleNum);
            this.contentContainer.Controls.Add(this.label5);
            this.contentContainer.Controls.Add(this.tileModeCB);
            this.contentContainer.Controls.Add(this.label4);
            this.contentContainer.Controls.Add(this.ImgDimComb);
            this.contentContainer.Controls.Add(this.label3);
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
            this.contentContainer.Size = new System.Drawing.Size(984, 511);
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
            this.contentContainer.Controls.SetChildIndex(this.label3, 0);
            this.contentContainer.Controls.SetChildIndex(this.ImgDimComb, 0);
            this.contentContainer.Controls.SetChildIndex(this.label4, 0);
            this.contentContainer.Controls.SetChildIndex(this.tileModeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.label5, 0);
            this.contentContainer.Controls.SetChildIndex(this.SwizzleNum, 0);
            // 
            // SwizzleNum
            // 
            this.SwizzleNum.Location = new System.Drawing.Point(774, 167);
            this.SwizzleNum.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.SwizzleNum.Name = "SwizzleNum";
            this.SwizzleNum.Size = new System.Drawing.Size(130, 20);
            this.SwizzleNum.TabIndex = 44;
            this.SwizzleNum.ValueChanged += new System.EventHandler(this.SwizzleNum_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(666, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "Swizzle Pattern:";
            // 
            // tileModeCB
            // 
            this.tileModeCB.BorderColor = System.Drawing.Color.Empty;
            this.tileModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.tileModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.tileModeCB.FormattingEnabled = true;
            this.tileModeCB.Location = new System.Drawing.Point(774, 94);
            this.tileModeCB.Name = "tileModeCB";
            this.tileModeCB.ReadOnly = true;
            this.tileModeCB.Size = new System.Drawing.Size(172, 21);
            this.tileModeCB.TabIndex = 42;
            this.tileModeCB.SelectedIndexChanged += new System.EventHandler(this.tileModeCB_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(666, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "Tile Mode";
            // 
            // ImgDimComb
            // 
            this.ImgDimComb.BorderColor = System.Drawing.Color.Empty;
            this.ImgDimComb.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.ImgDimComb.ButtonColor = System.Drawing.Color.Empty;
            this.ImgDimComb.FormattingEnabled = true;
            this.ImgDimComb.Location = new System.Drawing.Point(772, 61);
            this.ImgDimComb.Name = "ImgDimComb";
            this.ImgDimComb.ReadOnly = true;
            this.ImgDimComb.Size = new System.Drawing.Size(172, 21);
            this.ImgDimComb.TabIndex = 40;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(666, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Image Dimension";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(666, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Format";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(666, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Number MipMaps";
            // 
            // MipmapNum
            // 
            this.MipmapNum.Location = new System.Drawing.Point(774, 130);
            this.MipmapNum.Maximum = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.MipmapNum.Name = "MipmapNum";
            this.MipmapNum.Size = new System.Drawing.Size(130, 20);
            this.MipmapNum.TabIndex = 36;
            this.MipmapNum.ValueChanged += new System.EventHandler(this.MipmapNum_ValueChanged);
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(666, 235);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(35, 13);
            this.WidthLabel.TabIndex = 35;
            this.WidthLabel.Text = "Width";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(666, 200);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(38, 13);
            this.HeightLabel.TabIndex = 34;
            this.HeightLabel.Text = "Height";
            // 
            // formatComboBox
            // 
            this.formatComboBox.BorderColor = System.Drawing.Color.Empty;
            this.formatComboBox.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.formatComboBox.ButtonColor = System.Drawing.Color.Empty;
            this.formatComboBox.FormattingEnabled = true;
            this.formatComboBox.Location = new System.Drawing.Point(772, 31);
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.ReadOnly = true;
            this.formatComboBox.Size = new System.Drawing.Size(172, 21);
            this.formatComboBox.TabIndex = 32;
            this.formatComboBox.SelectedIndexChanged += new System.EventHandler(this.formatComboBox_SelectedIndexChanged);
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
            this.listViewCustom1.Size = new System.Drawing.Size(200, 486);
            this.listViewCustom1.TabIndex = 31;
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
            this.Format.Width = 96;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(895, 470);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(79, 33);
            this.button2.TabIndex = 30;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(797, 470);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 33);
            this.button1.TabIndex = 29;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(200, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(459, 486);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            // 
            // GTXTextureImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 516);
            this.Text = "GTXTextureImporter";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SwizzleNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STNumbericUpDown SwizzleNum;
        private Switch_Toolbox.Library.Forms.STLabel label5;
        private Switch_Toolbox.Library.Forms.STComboBox tileModeCB;
        private Switch_Toolbox.Library.Forms.STLabel label4;
        private Switch_Toolbox.Library.Forms.STComboBox ImgDimComb;
        private Switch_Toolbox.Library.Forms.STLabel label3;
        private Switch_Toolbox.Library.Forms.STLabel label2;
        private Switch_Toolbox.Library.Forms.STLabel label1;
        private Switch_Toolbox.Library.Forms.STNumbericUpDown MipmapNum;
        private Switch_Toolbox.Library.Forms.STLabel WidthLabel;
        private Switch_Toolbox.Library.Forms.STLabel HeightLabel;
        private Switch_Toolbox.Library.Forms.PictureBoxCustom pictureBox1;
        private Switch_Toolbox.Library.Forms.STComboBox formatComboBox;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader Format;
        private Switch_Toolbox.Library.Forms.STButton button2;
        private Switch_Toolbox.Library.Forms.STButton button1;

        private void MipmapNum_ValueChanged(object sender, EventArgs e) {
            if (SelectedTexSettings != null)
            {
                if (MipmapNum.Value > 0)
                    SelectedTexSettings.MipCount = (uint)MipmapNum.Value;
                else
                    SelectedTexSettings.MipCount = 1;
            }
        }
    }
}
