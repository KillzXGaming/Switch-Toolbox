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
        public void LoadArchive(IArchiveFile ArchiveFile)
        {
            ImageList.ColorDepth = ColorDepth.Depth32Bit;
            ImageList.ImageSize = new Size(40,40);

            var Files = OpenFileFormats(ArchiveFile);

            List<STGenericTexture> Textures = new List<STGenericTexture>();

            listViewCustom1.LargeImageList = ImageList;
            listViewCustom1.BeginUpdate();
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].FileFormat.FileType == FileType.Image)
                {
                    Textures.AddRange(GetTextures(Files[i].FileFormat));
                }
            }
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
                var item = listViewCustom1.SelectedItems[0];
                if (item.Tag != null && item.Tag is TreeNode)
                {
                    Point pt = listViewCustom1.PointToScreen(e.Location);
                    ((TreeNode)item.Tag).ContextMenuStrip.Show(pt);
                }
            }
        }

        ImageEditorForm imageEditorForm;

        private void LoadImageEditor(STGenericTexture texture, object Properties)
        {
            if (imageEditorForm == null || imageEditorForm.IsDisposed)
            {
                imageEditorForm = new ImageEditorForm(false);
                imageEditorForm.Show(this);
            }

            imageEditorForm.editorBase.Text = Text;
            imageEditorForm.editorBase.Dock = DockStyle.Fill;
            imageEditorForm.editorBase.LoadProperties(Properties);
            imageEditorForm.editorBase.LoadImage(texture);
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                var item = listViewCustom1.SelectedItems[0];
                if (item.Tag is STGenericTexture)
                {
                    LoadImageEditor((STGenericTexture)item.Tag, ((STGenericTexture)item.Tag).GenericProperties);
                }
            }
        }
    }
}
