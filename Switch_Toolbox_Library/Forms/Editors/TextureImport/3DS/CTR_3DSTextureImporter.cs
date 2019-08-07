using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace Toolbox.Library.Forms
{
    public partial class CTR_3DSTextureImporter : STForm
    {
        public int SelectedIndex = -1;

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
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.HiLo8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.L4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.LA4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.L8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.LA8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGBA8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGB8);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGBA5551);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGBA4);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.RGB565);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.ETC1);
            formatComboBox.Items.Add(CTR_3DS.PICASurfaceFormat.ETC1A4);

            IsLoaded = true;
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
            if (SelectedIndex == -1)
                return;

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            if (formatComboBox.SelectedItem is CTR_3DS.PICASurfaceFormat)
            {
                SelectedTexSettings.Format = (CTR_3DS.PICASurfaceFormat)formatComboBox.SelectedItem;
                listViewCustom1.Items[SelectedIndex].SubItems[1].Text = SelectedTexSettings.Format.ToString();
            }

            HeightLabel.Text = $"Height: {SelectedTexSettings.TexHeight}";
            WidthLabel.Text = $"Width: {SelectedTexSettings.TexWidth}";

            Bitmap bitmap = Toolbox.Library.Imaging.GetLoadingImage();

            pictureBox1.Image = bitmap;

            Thread = new Thread((ThreadStart)(() =>
            {
                SelectedTexSettings.IsFinishedCompressing = false;
                ToggleOkButton(false);

                var mips = SelectedTexSettings.GenerateMipList();

                SelectedTexSettings.DataBlockOutput.Clear();
                SelectedTexSettings.DataBlockOutput.Add(Utils.CombineByteArray(mips.ToArray()));

                ToggleOkButton(true);
                SelectedTexSettings.IsFinishedCompressing = true;

                bitmap = CTR_3DS.DecodeBlockToBitmap(mips[0], (int)SelectedTexSettings.TexWidth, (int)SelectedTexSettings.TexHeight, SelectedTexSettings.Format);

                mips.Clear();

                if (pictureBox1.InvokeRequired)
                {
                    pictureBox1.Invoke((MethodInvoker)delegate {
                        pictureBox1.Image = bitmap;
                        pictureBox1.Refresh();
                    });
                }
            }));
            Thread.Start();
        }

        private void ToggleOkButton(bool Enable)
        {
            if (button1.InvokeRequired)
            {
                button1.Invoke((MethodInvoker)delegate {
                    button1.Enabled = Enable;
                });
            }
            else
                button1.Enabled = Enable;
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
                SelectedIndex = listViewCustom1.SelectedIndices[0];

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
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CTR_3DSTextureImporter));
            this.ImgDimComb = new Toolbox.Library.Forms.STComboBox();
            this.label3 = new Toolbox.Library.Forms.STLabel();
            this.label2 = new Toolbox.Library.Forms.STLabel();
            this.label1 = new Toolbox.Library.Forms.STLabel();
            this.MipmapNum = new Toolbox.Library.Forms.STNumbericUpDown();
            this.WidthLabel = new Toolbox.Library.Forms.STLabel();
            this.HeightLabel = new Toolbox.Library.Forms.STLabel();
            this.formatComboBox = new Toolbox.Library.Forms.STComboBox();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Format = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button2 = new Toolbox.Library.Forms.STButton();
            this.button1 = new Toolbox.Library.Forms.STButton();
            this.pictureBox1 = new Toolbox.Library.Forms.PictureBoxCustom();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
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
            this.label1.Location = new System.Drawing.Point(664, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Number MipMaps";
            // 
            // MipmapNum
            // 
            this.MipmapNum.Location = new System.Drawing.Point(772, 97);
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
            this.WidthLabel.Location = new System.Drawing.Point(667, 178);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(35, 13);
            this.WidthLabel.TabIndex = 35;
            this.WidthLabel.Text = "Width";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(667, 143);
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
            // CTR_3DSTextureImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 516);
            this.Text = "CTR Texture Importer";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MipmapNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Toolbox.Library.Forms.STComboBox ImgDimComb;
        private Toolbox.Library.Forms.STLabel label3;
        private Toolbox.Library.Forms.STLabel label2;
        private Toolbox.Library.Forms.STLabel label1;
        private Toolbox.Library.Forms.STNumbericUpDown MipmapNum;
        private Toolbox.Library.Forms.STLabel WidthLabel;
        private Toolbox.Library.Forms.STLabel HeightLabel;
        private Toolbox.Library.Forms.PictureBoxCustom pictureBox1;
        private Toolbox.Library.Forms.STComboBox formatComboBox;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader Format;
        private Toolbox.Library.Forms.STButton button2;
        private Toolbox.Library.Forms.STButton button1;

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
