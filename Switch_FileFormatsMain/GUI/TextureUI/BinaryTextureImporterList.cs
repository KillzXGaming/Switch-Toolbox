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
using Switch_Toolbox.Library;

namespace FirstPlugin
{
    public partial class BinaryTextureImporterList : Form
    {
        public BinaryTextureImporterList()
        {
            InitializeComponent();
            listViewCustom1.FullRowSelect = true;

            //Add enums
            formatComboBox.Items.Add(TEX_FORMAT.A1_B5_G5_R5);
            formatComboBox.Items.Add(TEX_FORMAT.R4_G4_B4_A4);
            formatComboBox.Items.Add(TEX_FORMAT.R5_G5_B5_A1);
            formatComboBox.Items.Add(TEX_FORMAT.B5_G6_R5);
            formatComboBox.Items.Add(TEX_FORMAT.R8_G8_B8_A8);
            formatComboBox.Items.Add(TEX_FORMAT.R10_G10_B10_A2);
            formatComboBox.Items.Add(TEX_FORMAT.R11_G11_B10);
            formatComboBox.Items.Add(TEX_FORMAT.R16);
            formatComboBox.Items.Add(TEX_FORMAT.R32);
            formatComboBox.Items.Add(TEX_FORMAT.R4_G4_B4_A4);
            formatComboBox.Items.Add(TEX_FORMAT.R4_G4);
            formatComboBox.Items.Add(TEX_FORMAT.R5_G5_B5_A1);
            formatComboBox.Items.Add(TEX_FORMAT.R8G8);
            formatComboBox.Items.Add(TEX_FORMAT.R8);

            formatComboBox.Items.Add(TEX_FORMAT.R8_G8_B8_A8);
            formatComboBox.Items.Add(TEX_FORMAT.BC1);
            formatComboBox.Items.Add(TEX_FORMAT.BC2);
            formatComboBox.Items.Add(TEX_FORMAT.BC3);
            formatComboBox.Items.Add(TEX_FORMAT.BC4);
            formatComboBox.Items.Add(TEX_FORMAT.BC5);
            formatComboBox.Items.Add(TEX_FORMAT.BC6);
            formatComboBox.Items.Add(TEX_FORMAT.BC7);

            foreach (SurfaceDim dim in (SurfaceDim[])Enum.GetValues(typeof(SurfaceDim)))
            {
                ImgDimComb.Items.Add(dim);
            }
            tileModeCB.Items.Add("Texture");
            ImgDimComb.SelectedIndex = 1;
            tileModeCB.SelectedIndex = 0;
            formatComboBox.SelectedItem = TEX_FORMAT.BC1;
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

        private Thread Thread;
        public void SetupSettings()
        {
            if (SelectedTexSettings.Format == TEX_FORMAT.UNKNOWN)
                return;

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            try
            {
                if (formatComboBox.SelectedItem is TEX_FORMAT)
                {
                    SelectedTexSettings.Format = (TEX_FORMAT)formatComboBox.SelectedItem;
                    listViewCustom1.SelectedItems[0].SubItems[1].Text = SelectedTexSettings.Format.ToString();
                }
                pictureBox1.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();

                Thread = new Thread((ThreadStart)(() =>
                {
                    SelectedTexSettings.Compress();

                    Bitmap bitmap = STGenericTexture.DecodeBlockGetBitmap(SelectedTexSettings.DataBlockOutput[0], SelectedTexSettings.
                    TexWidth, SelectedTexSettings.TexHeight, SelectedTexSettings.Format);

                    pictureBox1.Image = bitmap;
                    pictureBox1.Invoke((MethodInvoker)delegate {
                        pictureBox1.Refresh();
                    });
                }));

                Thread.Start();
            }
            catch
            {
                throw new Exception("Failed to load image!");
            }
     

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
                Console.WriteLine("list index " + listViewCustom1.SelectedIndices[0]);

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
