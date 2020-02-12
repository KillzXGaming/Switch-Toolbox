using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using System.Threading;

namespace LayoutBXLYT
{
    public partial class Texture_Selector : STForm
    {
        public bool UpdateTextures = false;
        private Dictionary<string, STGenericTexture> textureList;
        private BxlytHeader ActiveLayout;
        private Thread Thread;

        public Texture_Selector()
        {
            InitializeComponent();

            btnEdit.Enabled = false;
            CanResize = false;
        }

        public string GetSelectedTexture()
        {
            return stTextBox1.Text;
        }

        public void LoadTextures(Dictionary<string, STGenericTexture> textures, string originalText, BxlytHeader header)
        {
            textureList = textures;
            ActiveLayout = header;

            foreach (var tex in textures.Values) {
                AddItem(tex);
            }

            //Try selecting original texture if possible
            if (listViewCustom1.Items.ContainsKey(originalText))
            {
                listViewCustom1.Items[originalText].Selected = true;
                listViewCustom1.Select();
            }
            else if (listViewCustom1.Items.Count > 0) //If not in the list use the first texture
            {
                listViewCustom1.Items[0].Selected = true;
                listViewCustom1.Select();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var textures = ActiveLayout.TextureManager.AddTextures();
            foreach (var tex in textures)
            {
                ActiveLayout.AddTexture(tex.Text);
                if (textureList.ContainsKey(tex.Text))
                    textureList.Remove(tex.Text);

                textureList.Add(tex.Text, tex);

                AddItem(tex);

                listViewCustom1.SelectedItems.Clear();
                if (listViewCustom1.Items.ContainsKey(tex.Text))
                {
                    listViewCustom1.Items[tex.Text].Selected = true;
                    listViewCustom1.Select();
                }

                UpdateTextures = true;
            }
        }

        private void AddItem(STGenericTexture texture)
        {
            if (texture is FirstPlugin.TPL.TplTextureWrapper)
            {
                var tex = ((FirstPlugin.TPL.TplTextureWrapper)texture);
               
                listViewCustom1.Items.Add(new ListViewItem(tex.TPLParent.FileName)
                { Name = tex.TPLParent.FileName , Tag = texture, });
            }
            else
            {
                listViewCustom1.Items.Add(new ListViewItem(texture.Text)
                { Name = texture.Text, Tag = texture, });
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                btnEdit.Enabled = true;

                string TexName = listViewCustom1.SelectedItems[0].Text;

                stTextBox1.Text = TexName;
                stTextBox1.Refresh();

                DisplayTexture((STGenericTexture)listViewCustom1.SelectedItems[0].Tag);
            }
            else
                btnEdit.Enabled = false;
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
                    pictureBoxCustom1.Invoke((MethodInvoker)delegate {
                        pictureBoxCustom1.Image = image;
                        pictureBoxCustom1.Refresh();
                    });
                }
                else
                    pictureBoxCustom1.Image = image;
            }));
            Thread.Start();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count == 0)
                return;

            string item = listViewCustom1.SelectedItems[0].Text;
            if (textureList.ContainsKey(item))
            {
                var texture = ActiveLayout.TextureManager.EditTexture(item);
                if (texture == null)
                    return;

                textureList[item] = texture;
                DisplayTexture(textureList[item]);
            }
        }
    }
}
