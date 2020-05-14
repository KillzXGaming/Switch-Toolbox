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

        public void DeselectTextureList() {
            listViewCustom1.SelectedItems.Clear();
        }

        private void listViewTpyeCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (isLoaded)
                listViewCustom1.View = (View)listViewTpyeCB.SelectedItem;
        }

        private void LoadTextureIcon(int index, STGenericTexture texture)
        {
            Bitmap temp = texture.GetBitmap();
            if (temp == null)
                return;

            temp = texture.GetComponentBitmap(temp, true);
            temp = BitmapExtension.CreateImageThumbnail(temp, 80, 80);

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
                STContextMenuStrip menu = new STContextMenuStrip();
                menu.Items.Add(new STToolStipMenuItem("Export", null, ActionExportTexture));
                menu.Items.Add(new STToolStipMenuItem("Replace", null, ActionReplaceTexture));

                menu.Show(Cursor.Position);
            }
        }

        private void ActionExportTexture(object sender, EventArgs e)
        {
            List<STGenericTexture> textures = new List<STGenericTexture>();
            foreach (ListViewItem item in listViewCustom1.SelectedItems)
            {
                if (TextureList.ContainsKey(item.Text)) {
                    textures.Add(TextureList[item.Text]);
                }
            }

            if (textures.Count == 1) {
                textures[0].ExportImage();
            }
            else if (textures.Count > 1)
            {
                List<string> Formats = new List<string>();
                Formats.Add("Microsoft DDS (.dds)");
                Formats.Add("Portable Graphics Network (.png)");
                Formats.Add("Joint Photographic Experts Group (.jpg)");
                Formats.Add("Bitmap Image (.bmp)");
                Formats.Add("Tagged Image File Format (.tiff)");

                FolderSelectDialog sfd = new FolderSelectDialog();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = sfd.SelectedPath;

                    BatchFormatExport form = new BatchFormatExport(Formats);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (STGenericTexture tex in textures)
                        {
                            if (form.Index == 0)
                                tex.SaveDDS(folderPath + '\\' + tex.Text + ".dds");
                            else if (form.Index == 1)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".png");
                            else if (form.Index == 2)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".jpg");
                            else if (form.Index == 3)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".bmp");
                            else if (form.Index == 4)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".tiff");
                        }
                    }
                }
            }

            textures.Clear();
        }

        private void ActionReplaceTexture(object sender, EventArgs e) {
            EditTexture();
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
                    {
                        ActiveLayout.TextureManager.RemoveTexture(TextureList[texture]);
                        foreach (var bntx in PluginRuntime.bntxContainers) {
                            if (bntx.Textures.ContainsKey(texture))
                                bntx.Textures.Remove(texture);
                        }
                        if (PluginRuntime.bflimTextures.ContainsKey(texture))
                            PluginRuntime.bflimTextures.Remove(texture);

                        TextureList.Remove(texture);
                    }

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
                if (listViewCustom1.Items.ContainsKey(tex.Text))
                    listViewCustom1.Items.RemoveByKey(tex.Text);

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

        private void btnEdit_Click(object sender, EventArgs e) {
            EditTexture();
        }

        private void EditTexture()
        {
            if (listViewCustom1.SelectedItems.Count == 0)
                return;

            string textName = listViewCustom1.SelectedItems[0].Text;
            if (TextureList.ContainsKey(textName))
            {
                var texture = ActiveLayout.TextureManager.EditTexture(textName);
                if (texture == null)
                    return;

                Console.WriteLine("texture edited!");

                TextureList[textName] = texture;
                //Update the icon by reloading all of them

                Console.WriteLine("LoadTextures!");
                LoadTextures(ParentEditor, ActiveLayout, TextureList);
                Console.WriteLine("FIN!");

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
