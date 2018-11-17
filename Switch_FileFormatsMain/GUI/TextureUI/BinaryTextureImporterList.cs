using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                ListViewItem item = new ListViewItem();
                item.Text = setting.TexName;
                listViewCustom1.Items.Add(item);
            }
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

        public void SetupSettings(Bitmap bitmap)
        {
            if (SelectedTexSettings.Format == SurfaceFormat.Invalid)
                return;

            pictureBox1.Image = bitmap;
            WidthLabel.Text = $"Width {bitmap.Width}";
            HeightLabel.Text = $"Height {bitmap.Height}";
            if (formatComboBox.SelectedItem is SurfaceFormat)
                SelectedTexSettings.Format = (SurfaceFormat)formatComboBox.SelectedItem;

            if (IsCompressed(SelectedTexSettings.Format))
            {
                //Compress first!
                switch (SelectedTexSettings.Format)
                {
                    case SurfaceFormat.BC1_UNORM:
                    //    SelectedTexSettings.DataBlockOutput = TexConv.CompressBC1(FileName);
                        break;
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
                        break;
                }

                System.IO.File.WriteAllBytes("temp.dds", SelectedTexSettings.DataBlockOutput);
                switch (SelectedTexSettings.Format)
                {
                    case SurfaceFormat.BC1_UNORM:
                   //     SelectedTexSettings.DataBlockOutput = TexConv.DecompressDDS(FileName);
                        break;
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
                        break;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
               
        }

        private void formatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formatComboBox.SelectedIndex > -1 && SelectedTexSettings != null)
            {
                SetupSettings(TextureData.DecodeBlock(SelectedTexSettings.DataBlockOutput, SelectedTexSettings.TexWidth,
                      SelectedTexSettings.TexHeight, SelectedTexSettings.Format));
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                SelectedTexSettings = settings[listViewCustom1.SelectedIndices[0]];
                SetupSettings(TextureData.DecodeBlock(SelectedTexSettings.DataBlockOutput, SelectedTexSettings.
                    TexWidth, SelectedTexSettings.TexHeight, SelectedTexSettings.Format));
            }
        }
    }
}
