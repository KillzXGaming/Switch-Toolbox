using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public partial class GenericTextureImporterList : STForm
    {
        public STCompressionMode CompressionMode = STCompressionMode.Fast;

        public int SelectedIndex = -1;
        public bool ForceMipCount = false;

        public uint SelectedMipCount
        {
            set
            {
                if (MipmapNum.Maximum <= value)
                    MipmapNum.Value = value;
            }
            get
            {
                return (uint)MipmapNum.Value;
            }
        }

        private bool IsLoaded = false;

        public GenericTextureImporterList(TEX_FORMAT[] SupportedFormats)
        {
            InitializeComponent();

            CanResize = false;

            listViewCustom1.FullRowSelect = true;
            listViewCustom1.CanResizeList = true;

            //Add enums
            foreach (TEX_FORMAT format in SupportedFormats)
            {
                formatComboBox.Items.Add(format);
            }

            compressionModeCB.Items.Add("Fast (Lower Qaulity)");
            compressionModeCB.Items.Add("Normal (Good Qaulity)");

            compressionModeCB.SelectedIndex = 0;
            compressionModeCB.Visible = false;
            formatComboBox.SelectedIndex = 0;

            IsLoaded = true;

            button1.Select();
        }
        GenericTextureImporterSettings SelectedTexSettings;

        public List<GenericTextureImporterSettings> settings = new List<GenericTextureImporterSettings>();
        public void LoadSetting(GenericTextureImporterSettings setting)
        {
            settings.Add(setting);

            listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }
        public void LoadSettings(List<GenericTextureImporterSettings> s)
        {
            settings = s;

            foreach (var setting in settings)
            {
                listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            }
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }

        private Thread Thread;
        public void SetupSettings()
        {
            if (SelectedTexSettings.Format == TEX_FORMAT.UNKNOWN || SelectedIndex == -1)
                return;


            WidthLabel.Text = $"Width {SelectedTexSettings.TexWidth}";
            HeightLabel.Text = $"Height {SelectedTexSettings.TexHeight}";

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

           if (formatComboBox.SelectedItem is TEX_FORMAT)
            {
                SelectedTexSettings.Format = (TEX_FORMAT)formatComboBox.SelectedItem;

                listViewCustom1.Items[SelectedIndex].SubItems[1].Text = SelectedTexSettings.Format.ToString();
            }

            if (SelectedTexSettings.Format == TEX_FORMAT.BC7_UNORM ||
                SelectedTexSettings.Format == TEX_FORMAT.BC7_TYPELESS ||
                SelectedTexSettings.Format == TEX_FORMAT.BC7_UNORM_SRGB)
            {
                compressionModeCB.Visible = true;
            }
            else
            {
                compressionModeCB.Visible = false;
            }

            Bitmap bitmap = Toolbox.Library.Imaging.GetLoadingImage();

            if (compressionModeCB.SelectedIndex == 0)
                CompressionMode = STCompressionMode.Fast;
            else
                CompressionMode = STCompressionMode.Normal;

            Thread = new Thread((ThreadStart)(() =>
            {
                SelectedTexSettings.IsFinishedCompressing = false;
                ToggleOkButton(false);

                pictureBox1.Image = bitmap;

                var mips = SelectedTexSettings.GenerateMipList(CompressionMode);
                SelectedTexSettings.DataBlockOutput.Clear();
                SelectedTexSettings.DataBlockOutput.Add(Utils.CombineByteArray(mips.ToArray()));

                ToggleOkButton(true);
                SelectedTexSettings.IsFinishedCompressing = true;

                if (SelectedTexSettings.DataBlockOutput.Count > 0) {
                    if (SelectedTexSettings.Format == TEX_FORMAT.BC5_SNORM)
                    {
                        bitmap = DDSCompressor.DecompressBC5(mips[0],
                    (int)SelectedTexSettings.TexWidth, (int)SelectedTexSettings.TexHeight, true);
                    }
                    else
                    {
                        bitmap = STGenericTexture.DecodeBlockGetBitmap(mips[0],
                        SelectedTexSettings.TexWidth, SelectedTexSettings.TexHeight, SelectedTexSettings.Format, new byte[0]);
                    }
                }

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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void formatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 1)
            {
                foreach (int index in listViewCustom1.SelectedIndices)
                {
                    if (formatComboBox.SelectedItem is TEX_FORMAT)
                    {
                        settings[index].Format = (TEX_FORMAT)formatComboBox.SelectedItem;
                        listViewCustom1.Items[index].SubItems[1].Text = settings[index].Format.ToString();
                    }
                }

                SetupSettings();
            }
            else if (formatComboBox.SelectedIndex > -1 && SelectedTexSettings != null)
            {
                SetupSettings();
            }

        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count == 1)
            {
                SelectedIndex = listViewCustom1.SelectedIndices[0];

                SelectedTexSettings = settings[listViewCustom1.SelectedIndices[0]];
                formatComboBox.SelectedItem = SelectedTexSettings.Format;

                SetupSettings();

                if (ForceMipCount)
                    MipmapNum.Maximum = SelectedTexSettings.MipCount;
                else
                {
                    MipmapNum.Maximum = STGenericTexture.GenerateTotalMipCount(
                    SelectedTexSettings.TexWidth, SelectedTexSettings.TexHeight) + 1;
                }

                MipmapNum.Value = SelectedTexSettings.MipCount;
            }
        }

        private void BinaryTextureImporterList_Load(object sender, EventArgs e)
        {
        }

        private void MipmapNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;

            if (MipmapNum.Value > 0)
                SelectedTexSettings.MipCount = (uint)MipmapNum.Value;
            else
                SelectedTexSettings.MipCount = 1;

            SetupSettings();
        }

        private void BinaryTextureImporterList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (button1.Enabled)
                {
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void compressionModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formatComboBox.SelectedIndex > -1 && SelectedTexSettings != null)
            {
                SetupSettings();
            }
        }

        private void ImgDimComb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
