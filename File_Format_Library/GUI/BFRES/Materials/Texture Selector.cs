using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library;
using Bfres.Structs;
using ResU = Syroot.NintenTools.Bfres;
using ResNX = Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin
{
    public partial class Texture_Selector : Form
    {
        private Thread Thread;
        public Texture_Selector()
        {
            InitializeComponent();

            listView1.HeaderStyle = ColumnHeaderStyle.None;
        }

        bool IsWIiiU = false;
        public void LoadTexture(string OriginalTexture, bool isWiiU)
        {
            IsWIiiU = isWiiU;

            listView1.Items.Clear();
            if (IsWIiiU)
            {
                foreach (BFRESGroupNode ftexcont in PluginRuntime.ftexContainers)
                {
                    foreach (FTEX tex in ftexcont.ResourceNodes.Values)
                        listView1.Items.Add(tex.Text, tex.Text, 0);
                }
            }
            else
            {
                foreach (BNTX bntx in PluginRuntime.bntxContainers)
                {
                    foreach (TextureData tex in bntx.Textures.Values)
                        listView1.Items.Add(tex.Text, tex.Text, 0);
                }
                foreach (var tex in PluginRuntime.TextureCache.Values)
                    listView1.Items.Add(tex.Text, tex.Text, 0);
            }


            //Select the texture that was originally mapped
            if (listView1.Items.ContainsKey(OriginalTexture))
            {
                listView1.Items[OriginalTexture].Selected = true;
                listView1.Select();
            }
            else if (listView1.Items.Count > 0) //If not in the list use the first texture
            {
                listView1.Items[0].Selected = true;
                listView1.Select();
            }

        }
        public string GetSelectedTexture()
        {
            return stTextBox1.Text;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string TexName = listView1.SelectedItems[0].Text;

                stTextBox1.Text = TexName;
                stTextBox1.Refresh();

                if (IsWIiiU)
                {
                    foreach (BFRESGroupNode ftexcont in PluginRuntime.ftexContainers)
                    {
                        if (ftexcont.ResourceNodes.ContainsKey(TexName))
                            DisplayTexture((FTEX)ftexcont.ResourceNodes[TexName]);
                    }
                }
                else
                {
                    foreach (BNTX bntx in PluginRuntime.bntxContainers)
                    {
                        if (bntx.Textures.ContainsKey(TexName))
                            DisplayTexture(bntx.Textures[TexName]);
                    }

                    if (PluginRuntime.TextureCache.ContainsKey(TexName))
                        DisplayTexture(PluginRuntime.TextureCache[TexName]);
                }
            }
        }
        private void DisplayTexture(STGenericTexture texData)
        {
            pictureBoxCustom1.Image = null;

            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBoxCustom1.Image = Toolbox.Library.Imaging.GetLoadingImage();
                var image = texData.GetBitmap();

                if (pictureBoxCustom1.InvokeRequired)
                {
                    pictureBoxCustom1.Invoke((MethodInvoker)delegate
                    {
                        pictureBoxCustom1.Image = image;
                        pictureBoxCustom1.Refresh();
                    });
                }
                else
                    pictureBoxCustom1.Image = image;
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
            else if (PluginRuntime.bntxContainers.Count == 1)
            {
                AddTexture(PluginRuntime.bntxContainers[0]);
            }
            else if (PluginRuntime.ftexContainers.Count > 0)
            {
                AddTextureFTEX((PluginRuntime.ftexContainers[0]));
            }
        }
        private void AddTexture(BNTX bntx)
        {
            bntx.ImportTexture();
            LoadTexture("", false);

            if (listView1.Items.Count > 0) //New texture is last item so select it
            {
                int lastItem = listView1.Items.Count - 1;
                listView1.Items[lastItem].Selected = true;
                listView1.Select();
            }
        }
        private void AddTextureFTEX(BFRESGroupNode ftexCont)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileFilters.FTEX;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ResourceName = Path.GetFileNameWithoutExtension(ofd.FileName);

                FTEX ftex = new FTEX();
                ftex.texture = new ResU.Texture();
                ftex.Text = ResourceName;
                ftex.Replace(ofd.FileName);

                if (ftex.IsEdited)
                    ftexCont.AddNode(ftex);
                else
                    ftex.Unload();

                listView1.Items.Add(ftex.Text, ftex.Text, 0);

                int lastItem = listView1.Items.Count - 1;
                listView1.Items[lastItem].Selected = true;
                listView1.Select();
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

            using (SolidBrush foreBrush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush, e.Bounds);
            }

            //     e.DrawText();
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}