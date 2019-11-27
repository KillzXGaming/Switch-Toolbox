using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class GFLXMaterialEditor : STUserControl
    {
        private GFLXMaterialData MaterialData;

        private ImageList TextureIconList;

        public GFLXMaterialEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            TextureIconList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(22, 22),
            };

            listViewCustom1.LargeImageList = TextureIconList;
            listViewCustom1.SmallImageList = TextureIconList;

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();

            ResetSliders();
        }

        private void ResetSliders()
        {
            barSlider1.SetTheme();
            barSlider2.SetTheme();
            barSlider3.SetTheme();
            barSlider4.SetTheme();

            barSlider1.Value = 0;
            barSlider2.Value = 0;
            barSlider3.Value = 0;
            barSlider4.Value = 0;
        }

        public void LoadMaterial(GFLXMaterialData materialData)
        {
            MaterialData = materialData;

            GFLXMaterialParamEditor paramEditor = new GFLXMaterialParamEditor();
            paramEditor.Dock = DockStyle.Fill;
            paramEditor.LoadParams(materialData);
            tabPage2.Controls.Add(paramEditor);

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                foreach (var tex in materialData.TextureMaps)
                {
                    Bitmap image = null;

                    foreach (var bntx in PluginRuntime.bntxContainers)
                    {
                        if (bntx.Textures.ContainsKey(tex.Name)) {
                            try {
                                image = bntx.Textures[tex.Name].GetBitmap();
                            }
                            catch {
                                image = Properties.Resources.TextureError;
                            }
                        }
                    }

                    AddTexture(tex.Name, image);
                }
            }));
            Thread.Start();
        }

        private void AddTexture(string name, Bitmap image)
        {
            if (listViewCustom1.InvokeRequired)
            {
                listViewCustom1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    ListViewItem item = new ListViewItem(name);
                    listViewCustom1.Items.Add(item);
                    if (image != null)
                    {
                        item.ImageIndex = TextureIconList.Images.Count;
                        TextureIconList.Images.Add(image);
                        var dummy = listViewCustom1.Handle;
                    }
                });
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0) {
                int index = listViewCustom1.SelectedIndices[0];
                var tex = MaterialData.TextureMaps[index];
                stTextBox1.Text = tex.Name;
                stTextBox2.Text = tex.SamplerName;

                foreach (var bntx in PluginRuntime.bntxContainers) {
                    if (bntx.Textures.ContainsKey(tex.Name))
                        UpdateTexturePreview(bntx.Textures[tex.Name]);
                }
            }
            else
            {
                ResetSliders();
                pictureBoxCustom1.Image = null;
            }
        }

        private void UpdateTexturePreview(STGenericTexture texture)
        {
            Thread Thread = new Thread((ThreadStart)(() =>
            {
                Bitmap image = null;

                try {
                    image = texture.GetBitmap();
                }
                catch {
                    image = Properties.Resources.TextureError;
                }

                pictureBoxCustom1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    pictureBoxCustom1.Image = image;
                });
            }));
            Thread.Start();
        }
    }
}
