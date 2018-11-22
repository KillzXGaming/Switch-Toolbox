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

namespace FirstPlugin
{
    public partial class BinaryTextureImporterList : Form
    {
        public BinaryTextureImporterList()
        {
            InitializeComponent();
            listViewCustom1.FullRowSelect = true;

            //Add enums
            foreach (SurfaceFormat format in (SurfaceFormat[])Enum.GetValues(typeof(SurfaceFormat)))
            {
                //  if (format != SurfaceFormat.Invalid)
            }
            formatComboBox.Items.Add(SurfaceFormat.A1_B5_G5_R5_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.A4_B4_G4_R4_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.B5_G5_R5_A1_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.B5_G6_R5_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.B8_G8_R8_A8_SRGB);
            formatComboBox.Items.Add(SurfaceFormat.B8_G8_R8_A8_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R10_G10_B10_A2_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R11_G11_B10_FLOAT);
            formatComboBox.Items.Add(SurfaceFormat.R16_UNORM);
            formatComboBox.Items.Add(SurfaceFormat.R32_FLOAT);
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

            foreach (SurfaceDim dim in (SurfaceDim[])Enum.GetValues(typeof(SurfaceDim)))
            {
                ImgDimComb.Items.Add(dim);
            }
            GPUAccessFlgComb.Items.Add("Texture");
            ImgDimComb.SelectedIndex = 1;
            GPUAccessFlgComb.SelectedIndex = 0;
            formatComboBox.SelectedItem = SurfaceFormat.BC1_SRGB;
        }
        TextureImporterSettings SelectedTexSettings;
        BinaryTextureContainer bntx;

        List<TextureImporterSettings> settings = new List<TextureImporterSettings>();
        public void LoadSettings(List<TextureImporterSettings> s, BinaryTextureContainer b)
        {
            settings = s;
            bntx = b;

            foreach (var setting in settings)
            {
                listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            }
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }
        public void LoadSetting(TextureImporterSettings setting, BinaryTextureContainer b)
        {
            settings = new List<TextureImporterSettings>();
            settings.Add(setting);
            bntx = b;

            listViewCustom1.Items.Add(setting.TexName).SubItems.Add(setting.Format.ToString());
            listViewCustom1.Items[0].Selected = true;
            listViewCustom1.Select();
        }
        public bool IsCompressed(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.BC1_UNORM:
                case SurfaceFormat.BC1_SRGB:
                case SurfaceFormat.BC2_UNORM:
                case SurfaceFormat.BC2_SRGB:
                case SurfaceFormat.BC3_UNORM:
                case SurfaceFormat.BC3_SRGB:
                case SurfaceFormat.BC4_UNORM:
                case SurfaceFormat.BC4_SNORM:
                case SurfaceFormat.BC5_UNORM:
                case SurfaceFormat.BC5_SNORM:
                case SurfaceFormat.BC6_UFLOAT:
                case SurfaceFormat.BC6_FLOAT:
                case SurfaceFormat.BC7_UNORM:
                case SurfaceFormat.BC7_SRGB:
                    return true;
                default:
                    return false;
            }
        }

        private Thread Thread;
        public void SetupSettings()
        {
            if (SelectedTexSettings.Format == SurfaceFormat.Invalid)
                return;


            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            if (formatComboBox.SelectedItem is SurfaceFormat)
            {
                SelectedTexSettings.Format = (SurfaceFormat)formatComboBox.SelectedItem;
                listViewCustom1.SelectedItems[0].SubItems[1].Text = SelectedTexSettings.Format.ToString();
            }
            Bitmap bitmap = Switch_Toolbox.Library.Imaging.GetLoadingImage();

            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBox1.Image = bitmap;
                SelectedTexSettings.Compress();

                bitmap = TextureData.DecodeBlock(SelectedTexSettings.DataBlockOutput[0], SelectedTexSettings.
                TexWidth, SelectedTexSettings.TexHeight, SelectedTexSettings.Format);

                pictureBox1.Image = bitmap;
 
            }));
            Thread.Start();

            //  WidthLabel.Text = $"Width {pictureBox1.Image.Width}";
            //      HeightLabel.Text = $"Height {pictureBox1.Image.Height}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
               
        }

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

                MipmapNum.Maximum = SelectedTexSettings.GetTotalMipCount();
                MipmapNum.Value = SelectedTexSettings.MipCount;
            }
        }

        private void BinaryTextureImporterList_Load(object sender, EventArgs e)
        {
        }

        private void MipmapNum_ValueChanged(object sender, EventArgs e)
        {
            SelectedTexSettings.MipCount = (uint)MipmapNum.Value;
        }
    }
}
