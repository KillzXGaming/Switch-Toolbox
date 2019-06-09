using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class ArchiveListPreviewForm : STForm
    {
        public ArchiveListPreviewForm()
        {
            InitializeComponent();
        }

        ImageList ImageList = new ImageList();
        List<STGenericTexture> Textures = new List<STGenericTexture>();

        private ListViewItem ActiveItem;

        public void LoadArchive(IArchiveFile ArchiveFile)
        {
            ImageList.ColorDepth = ColorDepth.Depth32Bit;
            ImageList.ImageSize = new Size(40,40);
            listViewCustom1.LargeImageList = ImageList;

            var Files = OpenFileFormats(ArchiveFile);

            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].FileFormat.FileType == FileType.Image)
                {
                    Textures.AddRange(GetTextures(Files[i].FileFormat));
                }
            }

            ReloadTextures();
        }

        private void ReloadTextures()
        {
            listViewCustom1.BeginUpdate();
            LoadTexturesThread(Textures);
            listViewCustom1.EndUpdate();
        }

        private void LoadTexturesThread(List<STGenericTexture> Textures)
        {
            ImageList.Images.Clear();
            Thread Thread = new Thread((ThreadStart)(() =>
            {
                foreach (var tex in Textures)
                {
                    Bitmap temp = tex.GetBitmap();
                    if (temp == null)
                    {
                        continue;
                    }

                    if (listViewCustom1.InvokeRequired)
                    {
                        listViewCustom1.Invoke((MethodInvoker)delegate {
                            ListViewItem item = new ListViewItem(tex.Text, ImageList.Images.Count);
                            item.Tag = tex;
                        
                            listViewCustom1.Items.Add(item);
                            // Running on the UI thread
                            ImageList.Images.Add(temp);
                            var dummy = ImageList.Handle;
                        });
                    }
                    else
                    {
                        ListViewItem item = new ListViewItem(tex.Text, ImageList.Images.Count);
                        item.Tag = tex;

                        listViewCustom1.Items.Add(item);
                        ImageList.Images.Add(temp);
                        var dummy = ImageList.Handle;
                    }

                    temp.Dispose();
                }
            }));
            Thread.Start();
        }

        private void ReloadTexture(Bitmap Image, ListViewItem listItem)
        {
            Image = BitmapExtension.Resize(Image, ImageList.ImageSize);
            ImageList.Images[listItem.ImageIndex] = Image;
            Image.Dispose();
        }

        private void ReloadTexture(STGenericTexture tex, ListViewItem listItem)
        {
            Thread Thread = new Thread((ThreadStart)(() =>
            {
                Bitmap temp = tex.GetBitmap();
                if (temp == null)
                    return;
                
                if (listViewCustom1.InvokeRequired)
                {
                    listViewCustom1.Invoke((MethodInvoker)delegate {
                        ImageList.Images[listItem.ImageIndex] = temp;
                        // Running on the UI thread
                        var dummy = ImageList.Handle;
                    });
                }
                else
                {
                    ListViewItem item = new ListViewItem(tex.Text, ImageList.Images.Count);
                    item.Tag = tex;

                    listViewCustom1.Items.Add(item);
                    ImageList.Images.Add(temp);
                    var dummy = ImageList.Handle;
                }

                temp.Dispose();
            }));
            Thread.Start();
        }

        private List<STGenericTexture> GetTextures(IFileFormat Format)
        {
            var Textures = new List<STGenericTexture>();

            if (Format is STGenericTexture)
                Textures.Add((STGenericTexture)Format);

            if (Format is TreeNodeFile)
            {
                foreach (var node in ((TreeNodeFile)Format).Nodes)
                    if (node is STGenericTexture) Textures.Add((STGenericTexture)node);
            }

            return Textures;
        }

        //Create a combination of all the archive files in multiple archives
        //All files in this list are supported formats
        public List<ArchiveFileInfo> OpenFileFormats(IArchiveFile ArchiveFile)
        {
            var Files = new List<ArchiveFileInfo>();
            foreach (var file in ArchiveFile.Files)
            {
                if (file.FileFormat == null)
                {
                    file.FileFormat = file.OpenFile();
                    if (file.FileFormat != null)
                    {
                        if (file.FileFormat is IArchiveFile)
                            return OpenFileFormats((IArchiveFile)file.FileFormat);
                        else
                            Files.Add(file);
                    }
                }
                else
                    Files.Add(file);
            }

            return Files;
        }

        private void listViewCustom1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0 && e.Button == MouseButtons.Right)
            {
                Point pt = listViewCustom1.PointToScreen(e.Location);
                stContextMenuStrip1.Show(pt);
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = listViewCustom1.SelectedItems[0];
            if (item.Tag != null && item.Tag is TreeNode)
            {
                foreach (ToolStripItem ctx in ((TreeNode)item.Tag).ContextMenuStrip.Items)
                {
                    if (ctx.Text == "Replace")
                    {
                        ctx.PerformClick();
                        ReloadTexture(GetActiveTexture(), item);
                    }
                }
            }
        }

        private STGenericTexture GetActiveTexture()
        {
            var item = listViewCustom1.SelectedItems[0];
            if (item.Tag is STGenericTexture)
            {
                return (STGenericTexture)item.Tag;
            }

            return null;
        }

        ImageEditorBase imageEditorForm;
        private void LoadImageEditor(STGenericTexture texture, object Properties)
        {
            if (imageEditorForm == null || imageEditorForm.IsDisposed)
            {
                imageEditorForm = new ImageEditorBase();

                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(imageEditorForm);
                imageEditorForm.Dock = DockStyle.Fill;
            }

            imageEditorForm.Text = Text;
            imageEditorForm.LoadProperties(Properties);
            imageEditorForm.LoadImage(texture);
            imageEditorForm.OnTextureReplaced += new ImageEditorBase.StatusUpdateHandler(UpdateTextureEdit);
        }

        private void UpdateTextureEdit(object sender, ImageEditorBase.ImageReplaceEventArgs e)
        {
            ReloadTexture(e.ReplacedTexture, ActiveItem);
            listViewCustom1.Refresh();
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void stMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                ActiveItem = listViewCustom1.SelectedItems[0];
                if (ActiveItem.Tag is STGenericTexture)
                {
                    LoadImageEditor((STGenericTexture)ActiveItem.Tag, ((STGenericTexture)ActiveItem.Tag).GenericProperties);
                }
            }
        }
    }
}
