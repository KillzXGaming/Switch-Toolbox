using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;

namespace FirstPlugin
{
    public partial class TextureViewer : DockContentST
    {
        public ImageList textureImageList;
        public TextureData SelectedTex;
        public TextureViewer()
        {
            InitializeComponent();

            textureImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(70, 70),
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
            textureImageList.Images.Clear();
            textureListView.Items.Clear();
            textureListView.LargeImageList = textureImageList;
            textureListView.FullRowSelect = true;

            int CurTex = 0;
            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                foreach (TextureData item in bntx.Textures.Values)
                {
                    ListViewItem it = new ListViewItem();
                    it.Text = item.Text;
                    it.ImageIndex = CurTex++;

                    textureListView.Items.Add(it);

                    TextureData tex = bntx.Textures[item.Text];
                    tex.LoadOpenGLTexture();

                    RenderableTex renderedTex = tex.RenderableTex;
                    Bitmap temp = RenderableTex.GLTextureToBitmap(renderedTex, renderedTex.display);

                    textureImageList.Images.Add(tex.Text, temp);

                    var dummy = textureImageList.Handle;
                    temp.Dispose();
                }
            }
        }

        private void textureListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textureListView_DoubleClick(object sender, EventArgs e)
        {
            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                if (bntx.Textures.ContainsKey(textureListView.SelectedItems[0].Text))
                {
                    TextureData tex = bntx.Textures[textureListView.SelectedItems[0].Text];

                    TextureOpenEditor editor = OpenTextureEditor();
                    editor.Show();
                    editor.LoadTexture(tex);
                }
            }
        }
        private TextureOpenEditor OpenTextureEditor()
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm is TextureOpenEditor)
                {
                    return (TextureOpenEditor)frm;
                }
            }
            return new TextureOpenEditor(); 
        }

        private void TextureViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("Closing textue viewer");
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
