using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstPlugin;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class LayoutTextureList : STForm
    {
        public LayoutTextureList()
        {
            InitializeComponent();

            listViewTpyeCB.Items.Add(View.Details);
            listViewTpyeCB.Items.Add(View.LargeIcon);
            listViewTpyeCB.Items.Add(View.List);
            listViewTpyeCB.Items.Add(View.SmallIcon);
            listViewTpyeCB.Items.Add(View.Tile);
            listViewTpyeCB.SelectedIndex = 0;
        }

        private bool isLoaded = false;
        public void LoadTextures(BFLYT.Header header)
        {
            listViewCustom1.BeginUpdate();
            foreach (var texture in header.TextureList.Textures)
            {
                ListViewItem item = new ListViewItem();
                item.Text = texture;
                listViewCustom1.Items.Add(item);
            }
            listViewCustom1.EndUpdate();

            isLoaded = true;
        }

        private void listViewTpyeCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoaded)
                listViewCustom1.View = (View)listViewTpyeCB.SelectedItem;
        }
    }
}
