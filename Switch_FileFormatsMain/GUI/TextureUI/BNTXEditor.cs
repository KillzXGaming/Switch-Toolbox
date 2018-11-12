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
    public partial class BNTXEditor : UserControl
    {
        private Thread Thread;

        public BNTXEditor()
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

        TextureData textureData;
        int CurMipDisplayLevel = 0;
        public void LoadProperty(TextureData tex)
        {
            textureData = tex;

            Texture texture = tex.Texture;
            propertyGrid1.PropertySort = PropertySort.Categorized;
            propertyGrid1.SelectedObject = texture;
            UpdateMipDisplay();
        }
        private void UpdateMipDisplay()
        {
            mipLevelCounterLabel.Text = $"{CurMipDisplayLevel} / {textureData.mipmaps.Count - 1}";

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();


            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBoxCustom1.Image = Imaging.GetLoadingImage();
                pictureBoxCustom1.Image = textureData.DisplayTexture(CurMipDisplayLevel);
              //  texSizeMipsLabel.Text = $"Width = {pictureBoxCustom1.Image.Width} Height = {pictureBoxCustom1.Image.Height}";
            }));
            Thread.Start();


            if (CurMipDisplayLevel != textureData.mipmaps.Count - 1)
                BtnMipsRight.Enabled = true;
            else
                BtnMipsRight.Enabled = false;

            if (CurMipDisplayLevel != 0)
                BtmMipsLeft.Enabled = true;
            else
                BtmMipsLeft.Enabled = false;
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
            if (CurMipDisplayLevel != textureData.mipmaps.Count - 1)
                CurMipDisplayLevel += 1;
            
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
    }
}
