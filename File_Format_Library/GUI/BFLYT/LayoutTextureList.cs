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
using Toolbox.Library;
using System.Threading;
using WeifenLuo.WinFormsUI.Docking;

namespace FirstPlugin.Forms
{
    public partial class LayoutTextureList : UserControl
    {
        ImageList imgList = new ImageList();
        public LayoutTextureList()
        {
            InitializeComponent();

            listViewTpyeCB.Items.Add(View.Details);
            listViewTpyeCB.Items.Add(View.LargeIcon);
            listViewTpyeCB.Items.Add(View.List);
            listViewTpyeCB.Items.Add(View.SmallIcon);
            listViewTpyeCB.Items.Add(View.Tile);
            listViewTpyeCB.SelectedIndex = 0;
            listViewCustom1.FullRowSelect = true;

            imgList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(30, 30),
            };
        }

        private bool isLoaded = false;
        public void LoadTextures(BFLYT.Header header)
        {
            listViewCustom1.Items.Clear();
            imgList.Images.Clear();
            imgList.Images.Add(new Bitmap(30, 30));

            listViewCustom1.LargeImageList = imgList;
            listViewCustom1.SmallImageList = imgList;

            var textureList = header.FileInfo.GetTextures();

            listViewCustom1.BeginUpdate();
            foreach (var texture in header.TextureList.Textures)
            {
                ListViewItem item = new ListViewItem();
                item.Text = texture;
                item.ImageIndex = 0;
                listViewCustom1.Items.Add(item);
            }

            Console.WriteLine($"textureList " + textureList.Count);

            //Load textures after on a seperate thread

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                foreach (ListViewItem item in listViewCustom1.Items)
                {
                    if (textureList.ContainsKey(item.Text))
                    {
                        LoadTextureIcon(item, textureList[item.Text]);
                    }
                }
            }));
            Thread.Start();

            foreach (ListViewItem item in listViewCustom1.Items)
            {
                if (textureList.ContainsKey(item.Text))
                    LoadTextureIcon(item, textureList[item.Text]);
            }

            listViewCustom1.EndUpdate();

            isLoaded = true;
        }

        private void listViewTpyeCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoaded)
                listViewCustom1.View = (View)listViewTpyeCB.SelectedItem;
        }

        private void LoadTextureIcon(ListViewItem item, STGenericTexture texture)
        {
            Bitmap temp = texture.GetBitmap();

            if (listViewCustom1.InvokeRequired)
            {
                listViewCustom1.Invoke((MethodInvoker)delegate {
                    item.ImageIndex = imgList.Images.Count;
                    item.SubItems.Add(texture.Format.ToString());
                    item.SubItems.Add(texture.Width.ToString());
                    item.SubItems.Add(texture.Height.ToString());
                    item.SubItems.Add(texture.DataSize);

                    // Running on the UI thread
                    imgList.Images.Add(temp);
                    var dummy = imgList.Handle;
                });
            }

            temp.Dispose();
        }

        private void LayoutTextureList_DragDrop(object sender, DragEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
                OpenTextureFile(filename);

            Cursor.Current = Cursors.Default;
        }

        private void OpenTextureFile(string fileName)
        {

        }

        private void listViewCustom1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
            {
                String[] strGetFormats = e.Data.GetFormats();
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
