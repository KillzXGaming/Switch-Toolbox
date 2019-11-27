using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class PokemonLoaderSwShForm : STForm
    {
        private bool CancelOperation = false;

        public string SelectedPokemon = "";

        ImageList ImageList;

        public PokemonLoaderSwShForm()
        {
            InitializeComponent();

            ImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(100, 100),
            };

            listViewCustom1.SmallImageList = ImageList;
            listViewCustom1.LargeImageList = ImageList;
        }


        private void PokemonLoaderSwShForm_Load(object sender, EventArgs e)
        {
            string gamePath = Runtime.PkSwShGamePath;
            if (Directory.Exists(gamePath))
            {
                string IconPath = $"{gamePath}/bin/appli/icon_pokemon";
                if (!Directory.Exists(IconPath))
                    return;

                Thread Thread = new Thread((ThreadStart)(() =>
                {
                    foreach (var file in Directory.GetFiles(IconPath))
                    {
                        if (CancelOperation)
                            break;

                        if (Utils.GetExtension(file) == ".bntx")
                        {
                            var bntx = (BNTX)STFileLoader.OpenFileFormat(file);

                            string name = bntx.Text.Replace($"poke_icon_", string.Empty);
                            //All we need is the first 8 characters
                            name = name.Substring(0, 7);

                            Bitmap bitmap = null;
                            try
                            {
                                var tex = bntx.Textures.Values.FirstOrDefault();
                                bitmap = tex.GetBitmap();
                            }
                            catch
                            {
                                bitmap = Properties.Resources.TextureError;
                            }

                            AddTexture($"pm{name}.gfpak", bitmap);
                        }
                    }
                })); Thread.Start();

            }
        }

        private void AddTexture(string name, Bitmap image)
        {
            if (listViewCustom1.Disposing || listViewCustom1.IsDisposed) return;

            if (listViewCustom1.InvokeRequired)
            {
                listViewCustom1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    ListViewItem item = new ListViewItem(name);
                    listViewCustom1.Items.Add(item);
                    if (image != null)
                    {
                        item.ImageIndex = ImageList.Images.Count;
                        ImageList.Images.Add(image);
                        var dummy = listViewCustom1.Handle;
                    }
                });
            }
        }

        private void PokemonLoaderSwShForm_FormClosing(object sender, FormClosingEventArgs e) {
            CancelOperation = true;
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e) {
            if (listViewCustom1.SelectedItems.Count > 0) {
                CancelOperation = true;
                SelectedPokemon = listViewCustom1.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
            }
        }
    }
}
