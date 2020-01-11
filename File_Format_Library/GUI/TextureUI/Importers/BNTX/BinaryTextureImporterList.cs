using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bntx;
using Syroot.NintenTools.NSW.Bntx.GFX;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public partial class BinaryTextureImporterList : STForm
    {
        public STCompressionMode CompressionMode = STCompressionMode.Fast;

        public int SelectedIndex = -1;
        public bool ForceMipCount = false;

        public bool MultiThreading => chkMultiThreading.Checked;

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

        public BinaryTextureImporterList()
        {
            InitializeComponent();

            CanResize = false;

            listViewCustom1.FullRowSelect = true;
            listViewCustom1.CanResizeList = true;

            //Add enums
            foreach (SurfaceFormat format in (SurfaceFormat[])Enum.GetValues(typeof(SurfaceFormat)))
            {
                //  if (format != SurfaceFormat.Invalid)
            }

            formatComboBox.Items.Add(SurfaceFormat.D32_FLOAT_S8X24_UINT);
            formatComboBox.Items.Add(SurfaceFormat.A1_B5_G5_R5_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.A4_B4_G4_R4_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.B5_G5_R5_A1_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.B5_G6_R5_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.B8_G8_R8_A8_SRGB);
            formatComboBox.Items.Add(SurfaceFormat.B8_G8_R8_A8_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R10_G10_B10_A2_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R16_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R4_G4_B4_A4_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R4_G4_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R5_G5_B5_A1_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R5_G6_B5_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R8_G8_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R8_UNORM);

            formatComboBox.Items.Add(SurfaceFormat.R8_G8_B8_A8_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R8_G8_B8_A8_SRGB);
            formatComboBox.Items.Add(SurfaceFormat.BC1_SRGB);
            formatComboBox.Items.Add(SurfaceFormat.BC1_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC2_SRGB);
            formatComboBox.Items.Add(SurfaceFormat.BC2_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC3_SRGB);
            formatComboBox.Items.Add(SurfaceFormat.BC3_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC4_SNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC4_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC5_SNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC5_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC6_UFLOAT);
            formatComboBox.Items.Add(SurfaceFormat.BC6_FLOAT);
            formatComboBox.Items.Add(SurfaceFormat.BC7_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.BC7_SRGB);

            compressionModeCB.Items.Add("Fast (Lower Quality)");
            compressionModeCB.Items.Add("Normal (Good Quality)");

            compressionModeCB.SelectedIndex = 0;
            compressionModeCB.Visible = false;
            compModeLbl.Visible = false;
            chkBC4Alpha.Enabled = false;

            foreach (SurfaceDim dim in (SurfaceDim[])Enum.GetValues(typeof(SurfaceDim)))
            {
                ImgDimComb.Items.Add(dim);
            }
            tileModeCB.Items.Add("Texture");
            ImgDimComb.SelectedIndex = 1;
            tileModeCB.SelectedIndex = 0;
            formatComboBox.SelectedItem = SurfaceFormat.BC1_SRGB;

            IsLoaded = true;

            button1.Select();
        }
        TextureImporterSettings SelectedTexSettings;

        public List<TextureImporterSettings> settings = new List<TextureImporterSettings>();
        public void LoadSetting(TextureImporterSettings setting)
        {
            settings.Add(setting);

            listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }
        public void LoadSettings(List<TextureImporterSettings> s)
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
        public void SetupSettings(TextureImporterSettings setting, bool setFormat = true)
        {
            if (setting.Format == SurfaceFormat.Invalid || SelectedIndex == -1)
                return;


            WidthLabel.Text = $"Width: {setting.TexWidth}";
            HeightLabel.Text = $"Height: {setting.TexHeight}";

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            if (formatComboBox.SelectedItem is SurfaceDim && setFormat)
                setting.SurfaceDim = (SurfaceDim)formatComboBox.SelectedItem;


           if (formatComboBox.SelectedItem is SurfaceFormat && setFormat)
            {
                setting.Format = (SurfaceFormat)formatComboBox.SelectedItem;

                listViewCustom1.Items[SelectedIndex].SubItems[1].Text = setting.Format.ToString();
            }

            if (setting.Format == SurfaceFormat.BC7_UNORM ||
                setting.Format == SurfaceFormat.BC7_SRGB)
            {
                compressionModeCB.Visible = true;
                compModeLbl.Visible = true;
            }
            else
            {
                compressionModeCB.Visible = false;
                compModeLbl.Visible = false;
            }

            if (setting.Format == SurfaceFormat.BC4_UNORM || setting.Format == SurfaceFormat.BC4_SNORM)
                chkBC4Alpha.Enabled = true;
            else
                chkBC4Alpha.Enabled = false;

            Bitmap bitmap = Toolbox.Library.Imaging.GetLoadingImage();

            if (compressionModeCB.SelectedIndex == 0)
                CompressionMode = STCompressionMode.Fast;
            else
                CompressionMode = STCompressionMode.Normal;

            Thread = new Thread((ThreadStart)(() =>
            {
                setting.IsFinishedCompressing = false;
                ToggleOkButton(false);

                pictureBox1.Image = bitmap;

                var mips = setting.GenerateMipList(CompressionMode, MultiThreading, chkBC4Alpha.Checked);

                setting.DataBlockOutput.Clear();
                setting.DataBlockOutput.Add(Utils.CombineByteArray(mips.ToArray()));

                ToggleOkButton(true);
                setting.IsFinishedCompressing = true;

                if (setting.DataBlockOutput.Count > 0) {
                    if (setting.Format == SurfaceFormat.BC5_SNORM)
                    {
                        bitmap = DDSCompressor.DecompressBC5(mips[0],
                    (int)setting.TexWidth, (int)setting.TexHeight, true);
                    }
                    else
                    {
                        bitmap = STGenericTexture.DecodeBlockGetBitmap(mips[0],
                        setting.TexWidth, setting.TexHeight, TextureData.ConvertFormat(setting.Format), new byte[0]);
                    }

                    if (chkBC4Alpha.Checked)
                        bitmap = BitmapExtension.SetChannel(bitmap, STChannelType.Red,
                            STChannelType.Red, STChannelType.Red, STChannelType.Red);
                }

                if (pictureBox1.InvokeRequired)
                {
                    pictureBox1.Invoke((MethodInvoker)delegate {
                        pictureBox1.Image = bitmap;
                        pictureBox1.Refresh();

                        int size = Utils.GetSizeInBytes(mips);
                        dataSizeLbl.Text = $"Data Size: {STMath.GetFileSize(size, 5)}";
                    });
                }

                mips.Clear();
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
                    if (formatComboBox.SelectedItem is SurfaceFormat)
                    {
                        settings[index].Format = (SurfaceFormat)formatComboBox.SelectedItem;
                        listViewCustom1.Items[index].SubItems[1].Text = settings[index].Format.ToString();
                    }
                }

                SetupSettings(SelectedTexSettings);
            }
            else if (formatComboBox.SelectedIndex > -1 && SelectedTexSettings != null)
            {
                SetupSettings(SelectedTexSettings);
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count == 1)
            {
                SelectedIndex = listViewCustom1.SelectedIndices[0];

                SelectedTexSettings = settings[listViewCustom1.SelectedIndices[0]];
                formatComboBox.SelectedItem = SelectedTexSettings.Format;
                ImgDimComb.SelectedItem = SelectedTexSettings.SurfaceDim;


                SetupSettings(SelectedTexSettings);

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

            foreach (int index in listViewCustom1.SelectedIndices)
            {
                if (MipmapNum.Value > 0)
                    settings[index].MipCount = (uint)MipmapNum.Value;
                else
                    settings[index].MipCount = 1;

                SetupSettings(settings[index], false);
            }
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
                SetupSettings(SelectedTexSettings, false);
            }
        }

        private void ImgDimComb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ImgDimComb.SelectedIndex > -1 && SelectedTexSettings != null)
            {
                SetupSettings(SelectedTexSettings);
            }
        }

        private void cnkGammaFix_CheckedChanged(object sender, EventArgs e) {
            if (SelectedTexSettings != null)
            {
                SelectedTexSettings.GammaFix = cnkGammaFix.Checked;
                SetupSettings(SelectedTexSettings, false);
            }
        }

        private void chkMultiThreading_CheckedChanged(object sender, EventArgs e) {
            if (SelectedTexSettings != null) {
                SetupSettings(SelectedTexSettings, false);
            }
        }

        private void chkBC4Alpha_CheckedChanged(object sender, EventArgs e) {
            if (SelectedTexSettings != null) {
                SetupSettings(SelectedTexSettings, false);
            }
        }
    }
}
