using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Syroot.NintenTools.NSW.Bntx;
using Syroot.NintenTools.NSW.Bntx.GFX;
using Switch_Toolbox.Library;

namespace FirstPlugin
{
    public partial class FTEXEditor : UserControl
    {
        private Thread Thread;

        public FTEXEditor()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(Runtime.PictureBoxBG)).Cast<Runtime.PictureBoxBG>())
                imageBGComboBox.Items.Add(type);

            imageBGComboBox.SelectedItem = Runtime.pictureBoxStyle;
            UpdateBackgroundImage();
        }
        public void LoadPicture(Bitmap image)
        {
            //  pictureBoxCustom1.Image = image;
        }

        FTEX textureData;
        int CurMipDisplayLevel = 0;
        int CurArrayDisplayLevel = 0;
        public void LoadProperty(FTEX tex)
        {
            CurMipDisplayLevel = 0;
            CurArrayDisplayLevel = 0;

            textureData = tex;
            propertyGrid1.PropertySort = PropertySort.Categorized;
            UpdateMipDisplay();
        }
        private void UpdateMipDisplay()
        {
            mipLevelCounterLabel.Text = $"{CurMipDisplayLevel} / {textureData.renderedTex.mipmaps[CurArrayDisplayLevel].Count - 1}";
            arrayLevelCounterLabel.Text = $"{CurArrayDisplayLevel} / {textureData.renderedTex.mipmaps.Count - 1}";


            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBoxCustom1.Image = Imaging.GetLoadingImage();
                pictureBoxCustom1.Image = textureData.DisplayTexture(CurMipDisplayLevel, CurArrayDisplayLevel);

                //  texSizeMipsLabel.Text = $"Width = {pictureBoxCustom1.Image.Width} Height = {pictureBoxCustom1.Image.Height}";
            }));
            Thread.Start();


            if (CurMipDisplayLevel != textureData.renderedTex.mipmaps[CurArrayDisplayLevel].Count - 1)
                BtnMipsRight.Enabled = true;
            else
                BtnMipsRight.Enabled = false;

            if (CurMipDisplayLevel != 0)
                BtmMipsLeft.Enabled = true;
            else
                BtmMipsLeft.Enabled = false;

            if (CurArrayDisplayLevel != textureData.renderedTex.mipmaps.Count - 1)
                btnRightArray.Enabled = true;
            else
                btnRightArray.Enabled = false;

            if (CurArrayDisplayLevel != 0)
                btnLeftArray.Enabled = true;
            else
                btnLeftArray.Enabled = false;
        }

        bool IsHidden = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (IsHidden)
            {
                panel1.Visible = true;
                IsHidden = false;
                button1.Text = "Hide";
            }
            else
            {
                panel1.Visible = false;
                IsHidden = true;
                button1.Text = "Show";
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGrid1.SelectedObject != null)
            {
                Texture tex = (Texture)propertyGrid1.SelectedObject;
                textureData.Text = tex.Name;
                pictureBoxCustom1.Image = textureData.UpdateBitmap(new Bitmap(pictureBoxCustom1.Image));
            }
        }

        private void BtmMipsLeft_Click(object sender, EventArgs e)
        {
            if (CurMipDisplayLevel != 0)
                CurMipDisplayLevel -= 1;

            UpdateMipDisplay();
        }

        private void BtnMipsRight_Click(object sender, EventArgs e)
        {
            if (CurMipDisplayLevel != textureData.renderedTex.mipmaps[CurArrayDisplayLevel].Count - 1)
                CurMipDisplayLevel += 1;

            UpdateMipDisplay();
        }

        private void btnLeftArray_Click(object sender, EventArgs e)
        {
            if (CurArrayDisplayLevel != 0)
                CurArrayDisplayLevel -= 1;

            UpdateMipDisplay();
        }

        private void btnRightArray_Click(object sender, EventArgs e)
        {
            if (CurArrayDisplayLevel != textureData.renderedTex.mipmaps.Count - 1)
                CurArrayDisplayLevel += 1;

            UpdateMipDisplay();
        }


        private void UpdateBackgroundImage()
        {
            switch (Runtime.pictureBoxStyle)
            {
                case Runtime.PictureBoxBG.Black:
                    pictureBoxCustom1.BackColor = Color.Black;
                    pictureBoxCustom1.BackgroundImage = null;
                    break;
                case Runtime.PictureBoxBG.Checkerboard:
                    pictureBoxCustom1.BackColor = Color.Transparent;
                    pictureBoxCustom1.BackgroundImage = pictureBoxCustom1.GetCheckerBackground();
                    break;
            }
        }

        private void imageBGComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.pictureBoxStyle = (Runtime.PictureBoxBG)imageBGComboBox.SelectedItem;
            UpdateBackgroundImage();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip1.Show(ptLowerLeft);
        }

        private void replaceSurfaceLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.dds|" +
              "Microsoft DDS |*.dds|" +
              "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DDS dds = new DDS(ofd.FileName);
                if (textureData.renderedTex.height != (uint)dds.header.height)
                    throw new Exception("Invalid height! Must be same as original!");

                if (textureData.renderedTex.width != (uint)dds.header.width)
                    throw new Exception("Invalid Width! Must be same as original!");

                List<byte[]> Mipmaps = TextureImporterSettings.SwizzleSurfaceMipMaps(textureData, dds.bdata);
                textureData.renderedTex.mipmaps[CurArrayDisplayLevel] = Mipmaps;

                UpdateMipDisplay();
                textureData.LoadOpenGLTexture();
            }
        }

        private void exportSurfaceLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.dds|" +
          "Microsoft DDS |*.dds|" +
          "All files(*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                textureData.Export(sfd.FileName, true, false, CurArrayDisplayLevel, CurMipDisplayLevel);
            }
        }
    }
}
