using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class GamecubeTextureImporterList : STForm
    {
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

        public GamecubeTextureImporterList(TEX_FORMAT[] SupportedFormats)
        {
            InitializeComponent();

            CanResize = false;

            listViewCustom1.FullRowSelect = true;
            listViewCustom1.CanResizeList = true;

            //Add enums
            foreach (TEX_FORMAT format in SupportedFormats)
            {
                formatComboBox.Items.Add(Decode_Gamecube.FromGenericFormat(format));
            }

            paletteFormatCB.Items.Add(Decode_Gamecube.PaletteFormats.IA8);
            paletteFormatCB.Items.Add(Decode_Gamecube.PaletteFormats.RGB565);
            paletteFormatCB.Items.Add(Decode_Gamecube.PaletteFormats.RGB5A3);

            paletteFormatCB.SelectedIndex = 1;

            paletteColorsUD.Maximum = 256;
            paletteColorsUD.Minimum = 16;
            paletteColorsUD.Value = 256;

            paletteAlgorithmCB.Items.Add("MedianCut");
            paletteAlgorithmCB.SelectedIndex = 0;

            formatComboBox.SelectedIndex = 0;

            IsLoaded = true;

            button1.Select();
        }
        GameCubeTextureImporterSettings SelectedTexSettings;

        public List<GameCubeTextureImporterSettings> settings = new List<GameCubeTextureImporterSettings>();
        public void LoadSetting(GameCubeTextureImporterSettings setting)
        {
            settings.Add(setting);

            listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }
        public void LoadSettings(List<GameCubeTextureImporterSettings> s)
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
            if (SelectedIndex == -1)
                return;


            WidthLabel.Text = $"Width {SelectedTexSettings.TexWidth}";
            HeightLabel.Text = $"Height {SelectedTexSettings.TexHeight}";

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

           if (formatComboBox.SelectedItem is Decode_Gamecube.TextureFormats)
            {
                SelectedTexSettings.Format = (Decode_Gamecube.TextureFormats)formatComboBox.SelectedItem;
                listViewCustom1.Items[SelectedIndex].SubItems[1].Text = SelectedTexSettings.Format.ToString();


                if (SelectedTexSettings.Format == Decode_Gamecube.TextureFormats.C4 ||
                    SelectedTexSettings.Format == Decode_Gamecube.TextureFormats.C8)
                {
                    paletteColorsUD.Enabled = true;
                    paletteColorsUD.Enabled = true;
                    paletteAlgorithmCB.Enabled = true;
                }
                else
                {
                    paletteColorsUD.Enabled = false;
                    paletteColorsUD.Enabled = false;
                    paletteAlgorithmCB.Enabled = false;
                }
            }
            if (paletteFormatCB.SelectedItem is Decode_Gamecube.PaletteFormats)
            {
                SelectedTexSettings.PaletteFormat = (Decode_Gamecube.PaletteFormats)paletteFormatCB.SelectedItem;
            }

            Bitmap bitmap = Toolbox.Library.Imaging.GetLoadingImage();

            Thread = new Thread((ThreadStart)(() =>
            {
                SelectedTexSettings.IsFinishedCompressing = false;
                ToggleOkButton(false);

                pictureBox1.Image = bitmap;

                var encodedData = SelectedTexSettings.GenerateMipList();

                var mips = encodedData.Item1;
                var paletteData = encodedData.Item2;

                SelectedTexSettings.DataBlockOutput.Clear();
                SelectedTexSettings.DataBlockOutput.Add(Utils.CombineByteArray(mips.ToArray()));

                ToggleOkButton(true);
                SelectedTexSettings.IsFinishedCompressing = true;

                bitmap = Decode_Gamecube.DecodeDataToBitmap(mips[0], paletteData,
                    SelectedTexSettings.TexWidth,
                    SelectedTexSettings.TexHeight,
                    SelectedTexSettings.Format,
                    SelectedTexSettings.PaletteFormat);

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
                    if (formatComboBox.SelectedItem is Decode_Gamecube.TextureFormats)
                    {
                        settings[index].Format = (Decode_Gamecube.TextureFormats)formatComboBox.SelectedItem;
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

        private void paletteFormatCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 1)
            {
                foreach (int index in listViewCustom1.SelectedIndices)
                {
                    if (paletteFormatCB.SelectedItem is Decode_Gamecube.PaletteFormats)
                    {
                        settings[index].PaletteFormat = (Decode_Gamecube.PaletteFormats)paletteFormatCB.SelectedItem;
                    }
                }

                SetupSettings();
            }
            else if (paletteFormatCB.SelectedIndex > -1 && SelectedTexSettings != null)
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
