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

namespace LayoutBXLYT
{
    public partial class LayoutTextureList : LayoutDocked
    {
        private LayoutEditor ParentEditor;
        private BxlytHeader ActiveLayout;
        private Dictionary<string, STGenericTexture> TextureList;

        ImageList imgListSmall = new ImageList();
        ImageList imgListBig = new ImageList();

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
            btnEdit.Enabled = false;

            imgListSmall = new ImageList()
            {
                ImageSize = new Size(30, 30),
                ColorDepth = ColorDepth.Depth32Bit,
            };
            imgListBig = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(80, 80),
            };
        }

        public void Reset()
        {
            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            for (int i = 0; i < imgListBig.Images.Count; i++)
                imgListBig.Images[i].Dispose();

            for (int i = 0; i < imgListSmall.Images.Count; i++)
                imgListSmall.Images[i].Dispose();

            imgListBig.Images.Clear();
            imgListSmall.Images.Clear();
            listViewCustom1.Items.Clear();

            isLoaded = false;
        }

        private bool isLoaded = false;
        private Thread Thread;
        public void LoadTextures(LayoutEditor parentEditor, BxlytHeader header, 
            Dictionary<string, STGenericTexture> textureList)
        {
            ParentEditor = parentEditor;
            TextureList = textureList;
            ActiveLayout = header;
            listViewCustom1.Items.Clear();
            imgListSmall.Images.Clear();
            imgListSmall.Images.Add(FirstPlugin.Properties.Resources.MissingTexture);
            imgListBig.Images.Clear();
            imgListBig.Images.Add(FirstPlugin.Properties.Resources.MissingTexture);

            listViewCustom1.LargeImageList = imgListBig;
            listViewCustom1.SmallImageList = imgListSmall;
            
            listViewCustom1.BeginUpdate();
            foreach (var texture in header.Textures)
            {
                ListViewItem item = new ListViewItem();
                item.Text = texture;
                item.ImageIndex = 0;
                listViewCustom1.Items.Add(item);
            }

            //Load textures after on a seperate thread

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                int index = 0;
                foreach (var texture in header.Textures)
                {
                    if (textureList.ContainsKey(texture))
                    {
                        if (header is BCLYT.Header)
                        {
                            //Skip certain formats like bcn ones
                            if (STGenericTexture.IsCompressed(textureList[texture].Format))
                                continue;
                        }

                        LoadTextureIcon(index, textureList[texture]);
                    }
                    index++;
                }
            }));
            Thread.Start();

            listViewCustom1.EndUpdate();

            isLoaded = true;
        }

        private void listViewTpyeCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoaded)
                listViewCustom1.View = (View)listViewTpyeCB.SelectedItem;
        }

        private void LoadTextureIcon(int index, STGenericTexture texture)
        {
            Bitmap temp = texture.GetBitmap();
            temp = texture.GetComponentBitmap(temp, true);

            if (listViewCustom1.InvokeRequired)
            {
                listViewCustom1.Invoke((MethodInvoker)delegate {
                    var item = listViewCustom1.Items[index];
                    item.ImageIndex = imgListBig.Images.Count;
                    item.SubItems.Add(texture.Format.ToString());
                    item.SubItems.Add(texture.Width.ToString());
                    item.SubItems.Add(texture.Height.ToString());
                    item.SubItems.Add(texture.DataSize);

                    // Running on the UI thread
                    imgListBig.Images.Add(temp);
                    imgListSmall.Images.Add(temp);

                    var dummy = imgListBig.Handle;
                    var dummy2 = imgListSmall.Handle;
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

        private void listViewCustom1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count == 0)
                return;

            var item = listViewCustom1.SelectedItems[0];
            if (e.Button == MouseButtons.Right)
            {
                
            }
        }

        private void listViewCustom1_ItemDrag(object sender, ItemDragEventArgs e)  {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            RemoveSelectedTextures();
        }

        private void RemoveSelectedTextures()
        {
            var result = MessageBox.Show("Are you sure you want to remove these textures?",
                "Layout Edtior", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (ListViewItem item in listViewCustom1.SelectedItems)
                {
                    var texture = item.Text;
                    if (TextureList.ContainsKey(texture))
                        TextureList.Remove(texture);

                    ActiveLayout.RemoveTexture(texture);
                    listViewCustom1.Items.Remove(item);
                    ParentEditor.UpdateViewport();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var textures = ActiveLayout.TextureManager.AddTextures();
            if (textures == null) return;

            foreach (var tex in textures)
            {
                if (tex == null)
                    continue;

                ActiveLayout.AddTexture(tex.Text);
                if (TextureList.ContainsKey(tex.Text))
                    TextureList.Remove(tex.Text);

                TextureList.Add(tex.Text, tex);

                ListViewItem item = new ListViewItem();
                item.Text = tex.Text;
                item.ImageIndex = 0;

                listViewCustom1.BeginUpdate();
                listViewCustom1.Items.Add(item);

                //Add icon

                if (Thread != null && Thread.IsAlive)
                    Thread.Abort();

                int index = listViewCustom1.Items.IndexOf(item);
                Thread = new Thread((ThreadStart)(() =>
                {
                    LoadTextureIcon(index, tex);
                }));
                Thread.Start();


                listViewCustom1.SelectedItems.Clear();
                if (listViewCustom1.Items.ContainsKey(tex.Text))
                {
                    listViewCustom1.Items[tex.Text].Selected = true;
                    listViewCustom1.Select();
                }

                listViewCustom1.EndUpdate();
                ParentEditor.UpdateViewport();
            }
        }

        private void listViewCustom1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listViewCustom1.SelectedItems.Count > 0) {
                RemoveSelectedTextures();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count == 0)
                return;

            string textName = listViewCustom1.SelectedItems[0].Text;
            if (TextureList.ContainsKey(textName))
            {
                var texture = ActiveLayout.TextureManager.EditTexture(textName);
                if (texture == null)
                    return;

                TextureList[textName] = texture;
                //Update the icon by reloading all of them
                LoadTextures(ParentEditor, ActiveLayout, TextureList);
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
                btnEdit.Enabled = true;
            else
                btnEdit.Enabled = false;
        }
    }
}
