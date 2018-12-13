using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FirstPlugin
{
    public partial class Texture_Selector : Form
    {
        private Thread Thread;
        public Texture_Selector()
        {
            InitializeComponent();
        }

        bool IsWIiiU = false;
        public void LoadTexture(bool isWiiU)
        {
            IsWIiiU = isWiiU;

            if (IsWIiiU)
            {
                foreach (FTEXContainer ftexcont in PluginRuntime.ftexContainers)
                {
                    foreach (FTEX tex in ftexcont.Textures.Values)
                        listView1.Items.Add(tex.Text);
                }
            }
            else
            {
                foreach (BNTX bntx in PluginRuntime.bntxContainers)
                {
                    foreach (TextureData tex in bntx.Textures.Values)
                        listView1.Items.Add(tex.Text);
                }
            }

  
            if (listView1.Items.Count > 0)
            {
                listView1.Items[0].Selected = true;
                listView1.Select();
            }
        }
        public string GetSelectedTexture()
        {
            return listView1.SelectedItems[0].Text;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string TexName = listView1.SelectedItems[0].Text;
                if (IsWIiiU)
                {
                    foreach (FTEXContainer ftexcont in PluginRuntime.ftexContainers)
                    {
                        if (ftexcont.Textures.ContainsKey(TexName))
                            DisplayTexture(ftexcont.Textures[TexName]);
                    }
                }
                else
                {
                    foreach (BNTX bntx in PluginRuntime.bntxContainers)
                    {
                        if (bntx.Textures.ContainsKey(TexName))
                            DisplayTexture(bntx.Textures[TexName]);
                    }
                }  
            }
        }
        private void DisplayTexture(FTEX texData)
        {
            if (Thread != null && Thread.IsAlive)
                Thread.Abort();


            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBoxCustom1.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();
                pictureBoxCustom1.Image = texData.GetBitmap();
                //  texSizeMipsLabel.Text = $"Width = {pictureBoxCustom1.Image.Width} Height = {pictureBoxCustom1.Image.Height}";
            }));
            Thread.Start();
        }
        private void DisplayTexture(TextureData texData)
        {
            if (Thread != null && Thread.IsAlive)
                Thread.Abort();


            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBoxCustom1.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();
                pictureBoxCustom1.Image = texData.GetBitmap();
                //  texSizeMipsLabel.Text = $"Width = {pictureBoxCustom1.Image.Width} Height = {pictureBoxCustom1.Image.Height}";
            }));
            Thread.Start();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK; 
        }

        private void addTextureBtn_Click(object sender, EventArgs e)
        {
            if (PluginRuntime.bntxContainers.Count > 1)
            {
                BntxSelector bntxSelector = new BntxSelector();
                if (bntxSelector.ShowDialog() == DialogResult.OK)
                {
                    AddTexture((bntxSelector.GetBNTX()));
                }
            }
            else
            {
                AddTexture((PluginRuntime.bntxContainers[0]));
            }
        }
        private void AddTexture(BNTX bntx)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Binary Texture |*.bftex|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";
            ofd.DefaultExt = "bftex";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string name in ofd.FileNames)
                {
                    bntx.AddTexture(name);
                    listView1.Items.Add(System.IO.Path.GetFileNameWithoutExtension(name));
                }
            }
        }

        private void RemoveTextureBtn_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string TexName = listView1.SelectedItems[0].Text;
                foreach (BNTX bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(TexName))
                    {
                        bntx.Textures.Remove(TexName);
                        listView1.Items.RemoveByKey(TexName);
                    }
                }
            }
        }

        private void Texture_Selector_Load(object sender, EventArgs e)
        {

        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            if ((e.ItemIndex % 2) == 1)
            {
                e.Item.BackColor = Color.FromArgb(50, 50, 50);
                e.Item.UseItemStyleForSubItems = true;
            }
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }
            using (SolidBrush foreBrush = new SolidBrush(Color.FromArgb(255,255,255)))
            {
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush, e.Bounds);
            }
       //     e.DrawText();
        }
    }
}
