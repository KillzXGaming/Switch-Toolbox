using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using Toolbox.Library.Forms;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public partial class TextureViewer : STUserControl
    {
        public ImageList textureImageList;
        public TextureData SelectedTex;
        public TextureViewer()
        {
            InitializeComponent();

            textureListView.SetDoubleBuffer();
            textureImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(75, 75),
            };
        }
        public void ClearForm()
        {
            textureImageList.Images.Clear();
            textureListView.Items.Clear();
            textureImageList = null;
        }
        public void LoadTextures()
        {
            textureListView.SuspendLayout();
            textureListView.BeginUpdate();
            textureImageList.Images.Clear();
            textureListView.Items.Clear();
            textureListView.LargeImageList = textureImageList;
            textureListView.View = View.LargeIcon;
            textureListView.FullRowSelect = true;
            TextureLoader.SetControlSpacing(textureListView, 80, 80);
            
           
            int imageIndex = 0;

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                foreach (BNTX bntx in PluginRuntime.bntxContainers)
                {
                    foreach (var tex in bntx.Textures.Values)
                    {
                        Bitmap temp = tex.GetBitmap();

                        if (textureListView.InvokeRequired)
                        {
                            textureListView.Invoke((MethodInvoker)delegate {
                                textureListView.Items.Add(tex.Text, imageIndex++);
                                // Running on the UI thread
                                textureImageList.Images.Add(temp);
                                var dummy = textureImageList.Handle;
                            });
                        }
                        temp.Dispose();
                    }
                }
                foreach (BFLIM tex in PluginRuntime.bflimTextures.Values)
                {
                    Bitmap temp = tex.GetBitmap();

                    if (textureListView.InvokeRequired)
                    {
                        textureListView.Invoke((MethodInvoker)delegate {
                            textureListView.Items.Add(tex.Text, imageIndex++);
                            // Running on the UI thread
                            textureImageList.Images.Add(temp);
                            var dummy = textureImageList.Handle;
                        });
                    }
                    temp.Dispose();
                }
            }));
            Thread.Start();

            textureListView.EndUpdate();
            textureListView.ResumeLayout();
        }

        private void textureListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        ImageEditorForm imageEditorForm;

        private void textureListView_DoubleClick(object sender, EventArgs e)
        {
            if (PluginRuntime.bflimTextures.Count > 0)
            {
                if (PluginRuntime.bflimTextures.ContainsKey(textureListView.SelectedItems[0].Text))
                {
                    var tex = PluginRuntime.bflimTextures[textureListView.SelectedItems[0].Text];
                    if (imageEditorForm == null || imageEditorForm.IsDisposed)
                    {
                        imageEditorForm = new ImageEditorForm(false);
                        imageEditorForm.Show(this);
                    }

                    imageEditorForm.editorBase.Text = Text;
                    imageEditorForm.editorBase.Dock = DockStyle.Fill;
                    imageEditorForm.editorBase.LoadProperties(tex.GenericProperties);
                    imageEditorForm.editorBase.LoadImage(tex);
                }
            }

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                if (bntx.Textures.ContainsKey(textureListView.SelectedItems[0].Text))
                {
                    TextureData tex = bntx.Textures[textureListView.SelectedItems[0].Text];

                    if (imageEditorForm == null || imageEditorForm.IsDisposed)
                    {
                        imageEditorForm = new ImageEditorForm(false);
                        imageEditorForm.Show(this);
                    }

                    imageEditorForm.editorBase.Text = Text;
                    imageEditorForm.editorBase.Dock = DockStyle.Fill;
                    imageEditorForm.editorBase.LoadProperties(tex.Texture);
                    imageEditorForm.editorBase.LoadImage(tex);

                    break;
                }
            }
        }

        public override void OnControlClosing()
        {
            ClearForm();
        }

        private void textureListView_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        foreach (BNTX bntx in PluginRuntime.bntxContainers)
                        {
                            if (bntx.Textures.ContainsKey(textureListView.SelectedItems[0].Text))
                            {
                                SelectedTex = bntx.Textures[textureListView.SelectedItems[0].Text];

                                Point p = new Point(e.X, e.Y);

                                textureContextMenuStrip1.Show(textureListView, p);

                            }
                        }
                    }
                    break;
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = SelectedTex.Text;
            sfd.DefaultExt = "bftex";
            sfd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Binary Texture |*.bftex|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SelectedTex.Export(sfd.FileName);
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Binary Texture |*.bftex|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SelectedTex.Replace(ofd.FileName);
            }
        }
    }
}
