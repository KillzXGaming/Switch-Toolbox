using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Bfres.Structs;
using System.Threading;

namespace FirstPlugin.Forms
{

    public partial class TextureLoader : STUserControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern Int32 SendMessage(IntPtr hwnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        const int LVM_FIRST = 0x1000;
        const int LVM_SETICONSPACING = LVM_FIRST + 53;

        public static void SetControlSpacing(Control control, Int16 x, Int16 y)
        {
            SendMessage(control.Handle, LVM_SETICONSPACING, 0, x * 65536 + y);
            control.Refresh();
        }

        BNTX activeBntx;
        BFRESGroupNode activeFtexContainer;

        ImageList imgList = new ImageList();

        public TextureLoader()
        {
            InitializeComponent();

            imgList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(70, 70),
            };
        }

        public void LoadTexture()
        {
            int imageIndex = 0;

            imgList.Images.Clear();

            listViewCustom1.LargeImageList = imgList;
            listViewCustom1.Items.Clear();
            listViewCustom1.View = View.LargeIcon;
            SetControlSpacing(listViewCustom1, 75, 75);

            if (activeBntx != null)
            {
                Thread Thread = new Thread((ThreadStart)(() =>
                {
                    foreach (var tex in activeBntx.Textures.Values)
                    {
                        Bitmap temp = tex.GetBitmap();

                        if (listViewCustom1.InvokeRequired)
                        {
                            listViewCustom1.Invoke((MethodInvoker)delegate {
                                listViewCustom1.Items.Add(tex.Text, imageIndex++);
                                // Running on the UI thread
                                imgList.Images.Add(temp);
                                var dummy = imgList.Handle;
                            });
                        }

                        temp.Dispose();
                    }
                }));
                Thread.Start();
            }

            imageIndex = 0;

            if (activeFtexContainer != null)
            {
                foreach (var tex in activeFtexContainer.ResourceNodes.Values)
                {
                    FTEX ftex = (FTEX)tex;

                    listViewCustom1.Items.Add(ftex.Text, imageIndex++);

                    Bitmap temp = ftex.GetBitmap();
                    imgList.Images.Add(temp);
                    var dummy = imgList.Handle;
                    temp.Dispose();
                }
            }
        }

        private void UpdateContainers()
        {
            //Check for new BNTX or FTEX containers
            //If they are all loaded in the combo box, return to prevent clearing on switching everytime
            int total = PluginRuntime.bntxContainers.Count + PluginRuntime.ftexContainers.Count;
            if (stComboBox1.Items.Count == total)
                return;

            stComboBox1.Items.Clear();

            int i = 0;
            foreach (var tex in PluginRuntime.bntxContainers)
                stComboBox1.Items.Add($"BNTX File {i++}");

            i = 0;
            foreach (var tex in PluginRuntime.ftexContainers)
                stComboBox1.Items.Add($"FTEX Folder {i++}");
        }

        private void stComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = stComboBox1.SelectedIndex;
            if (index < 0)
                return;

            if (PluginRuntime.bntxContainers.Count > 0)
            {
                activeBntx = PluginRuntime.bntxContainers[index];

                LoadTexture();
            }
            if (PluginRuntime.ftexContainers.Count > 0)
            {
                activeFtexContainer = PluginRuntime.ftexContainers[index];

                LoadTexture();
            }
        }

        private void barSlider1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void barSlider1_ValueChanged(object sender, EventArgs e)
        {
         
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                if (activeBntx != null)
                {
                    foreach (ListViewItem item in listViewCustom1.SelectedItems)
                    {
                        string SelectedTexture = item.Text;
                        if (activeBntx.Textures.ContainsKey(SelectedTexture))
                        {
                            activeBntx.Nodes.Remove(activeBntx.Textures[SelectedTexture]);
                            activeBntx.Textures.Remove(SelectedTexture);

                            listViewCustom1.Items.Remove(item);
                        }
                    }
                }
                if (activeFtexContainer != null)
                {
                    foreach (ListViewItem item in listViewCustom1.SelectedItems)
                    {
                        string SelectedTexture = item.Text;
                        if (activeFtexContainer.ResourceNodes.ContainsKey(SelectedTexture))
                        {
                            activeFtexContainer.Nodes.Remove(activeFtexContainer.ResourceNodes[SelectedTexture]);
                            activeFtexContainer.ResourceNodes.Remove(SelectedTexture);

                            listViewCustom1.Items.Remove(item);
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (activeBntx != null)
            {
                activeBntx.ImportTexture();
                LoadTexture();
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices != null &&
                listViewCustom1.SelectedIndices.Count > 0)
            {
                btnRemove.Enabled = true;
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }


        ImageEditorForm imageEditorForm;

        private TextureData GetSelectedBntxTexture()
        {
            if (activeBntx == null)
                return null;

            if (activeBntx.Textures.ContainsKey(listViewCustom1.SelectedItems[0].Text))
            {
                return activeBntx.Textures[listViewCustom1.SelectedItems[0].Text];
            }

            return null;
        }

        private FTEX GetSelectedFtexTexture()
        {
            if (activeFtexContainer == null)
                return null;

            if (activeFtexContainer.ResourceNodes.ContainsKey(listViewCustom1.SelectedItems[0].Text))
            {
                return (FTEX)activeFtexContainer.ResourceNodes[listViewCustom1.SelectedItems[0].Text];
            }

            return null;
        }

        private void textureListView_DoubleClick(object sender, EventArgs e)
        {
            if (activeBntx != null)
            {
                TextureData tex = GetSelectedBntxTexture();

                if (tex != null)
                    LoadImageEditor(tex, tex.Texture);
            }
            if (activeFtexContainer != null)
            {
                FTEX tex = GetSelectedFtexTexture();

                if (tex != null)
                    LoadImageEditor(tex, tex.texture);
            }
        }

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

        public override void OnControlClosing()
        {
            imgList.Images.Clear();
            listViewCustom1.Items.Clear();
            imgList = null;
        }

        private void textureListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count <= 0)
                return;

            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        Point p = new Point(e.X, e.Y);
                        textureContextMenuStrip1.Show(listViewCustom1, p);
                    }
                    break;
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeBntx != null)
            {
                TextureData tex = GetSelectedBntxTexture();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = tex.ExportFilter;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    tex.Export(sfd.FileName);
                }
            }
            if (activeFtexContainer != null)
            {
                FTEX tex = GetSelectedFtexTexture();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = tex.ExportFilter;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    tex.Export(sfd.FileName);
                }
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeBntx != null)
            {
                TextureData tex = GetSelectedBntxTexture();

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = tex.ExportFilter;

                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    tex.Replace(ofd.FileName);
                }
            }
            if (activeFtexContainer != null)
            {
                FTEX tex = GetSelectedFtexTexture();

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = tex.ExportFilter;

                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK) {
                    tex.Replace(ofd.FileName);
                }
            }
        }

        private void stComboBox1_Click(object sender, EventArgs e) {
            UpdateContainers();
        }

        private void stComboBox1_MouseDown(object sender, MouseEventArgs e) {
            UpdateContainers();
        }

        private void stComboBox1_KeyDown(object sender, KeyEventArgs e) {
            UpdateContainers();
        }
    }
}
