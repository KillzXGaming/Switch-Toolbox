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
    public partial class XTXEditor : UserControl
    {
        private Thread Thread;

        public XTXEditor()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(Runtime.PictureBoxBG)).Cast<Runtime.PictureBoxBG>())
                imageBGComboBox.Items.Add(type);

            imageBGComboBox.SelectedItem = Runtime.pictureBoxStyle;
            UpdateBackgroundImage();
        }

        XTX.XTXFile.TextureInfo textureData;

        int CurMipDisplayLevel = 0;
        int CurArrayDisplayLevel = 0;

        class PropGridData
        {
            public string Name { get; set; }
            public string Format { get; set; }
            public uint Width { get; set; }
            public uint Height { get; set; }
            public uint MipCount { get; set; }
            public uint ArrayCount { get; set; }
        }
        public void LoadProperty(XTX.XTXFile.TextureInfo tex)
        {
            pictureBoxCustom1.Image = Imaging.GetLoadingImage();
            LoadImage();

            CurMipDisplayLevel = 0;
            CurArrayDisplayLevel = 0;

            textureData = tex;

            UpdateMipDisplay();

            PropGridData prop = new PropGridData();
            prop.Name = textureData.Text;
            prop.Width = textureData.Width;
            prop.Height = textureData.Height;
         //   prop.MipCount = (uint)textureData.blocksCompressed[0].Count;
         //   prop.ArrayCount = (uint)textureData.blocksCompressed.Count;
            prop.Height = textureData.Height;

            prop.Format = ((NUTEXB.NUTEXImageFormat)textureData.Format).ToString();

            propertyGrid1.PropertySort = PropertySort.Categorized;
            propertyGrid1.SelectedObject = prop;
        }
        private void LoadImage()
        {
            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBoxCustom1.Image = Imaging.GetLoadingImage();
                pictureBoxCustom1.Image = textureData.DisplayImage(CurMipDisplayLevel, CurArrayDisplayLevel);
            }));
            Thread.Start();

            GC.Collect();
        }
        private void UpdateMipDisplay()
        {
            LoadImage();

            int MipCount = 1;
            if (textureData.mipmaps.Count <= 0)
                return;
            else
                MipCount = textureData.mipmaps.Count;


            mipLevelCounterLabel.Text = $"{CurMipDisplayLevel} / {textureData.mipmaps.Count - 1}";

            if (CurMipDisplayLevel != MipCount - 1)
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip1.Show(ptLowerLeft);
        }
    }
}
