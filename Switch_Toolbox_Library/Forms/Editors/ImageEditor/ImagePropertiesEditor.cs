using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class ImagePropertiesEditor : STUserControl
    {
        public ImagePropertiesEditor()
        {
            InitializeComponent();

            stTabControl1.Refresh();
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            stTabControl1.SelectedTab = tabPage2;

            channelListView.BackColor = FormThemes.BaseTheme.FormBackColor;
            channelListView.ForeColor = FormThemes.BaseTheme.FormForeColor;
            channelListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;

            channelListView.Items.Add("RGBA", 0);
            channelListView.Items.Add("Red", 1);
            channelListView.Items.Add("Green", 2);
            channelListView.Items.Add("Blue", 3);
            channelListView.Items.Add("Alpha", 4);
        }
        public List<TabPage> tempPages = new List<TabPage>();
        public void AddTabPage(UserControl control, Type type)
        {
            control.Dock = DockStyle.Fill;

            var page = new TabPage();
            page.BackColor = FormThemes.BaseTheme.TabPageInactive;
            page.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            page.Text = control.Name;
            page.Controls.Add(control);
            stTabControl1.TabPages.Add(page);
        }

        public void HideHintPanel(bool HideHint)
        {
            stPropertyGrid1.ShowHintDisplay = !HideHint;
        }

        public UserControl GetActiveTabControl(Type type)
        {
            foreach (TabPage pge in stTabControl1.TabPages)
            {
                foreach (var ctrl in pge.Controls)
                {
                    if (ctrl.GetType() == type)
                    {
                        return (UserControl)ctrl;
                    }
                }
            }

            return null;
        }

        public void LoadProperties(object prop, Action OnPropertyChanged)
        {
            if (stPropertyGrid1.InvokeRequired)
            {
                stPropertyGrid1.Invoke(new MethodInvoker(
                delegate ()
                {
                    stPropertyGrid1.LoadProperty(prop, OnPropertyChanged, OnPropertyEditorChanged);
                    stPropertyGrid1.Refresh();
                }));
            }
            else
            {
                stPropertyGrid1.LoadProperty(prop, OnPropertyChanged, OnPropertyEditorChanged);
                stPropertyGrid1.Refresh();
            }
        }

        private void OnPropertyEditorChanged()
        {
            imageEditor.UpdateMipDisplay();
            LibraryGUI.UpdateViewport();
        }

        public void UpdateProperties()
        {
            if (stPropertyGrid1.InvokeRequired)
            {
                stPropertyGrid1.Invoke(new MethodInvoker(
           delegate ()
           {
               stPropertyGrid1.Refresh();
           }));
            }
            else
                stPropertyGrid1.Refresh();
        }

        public void ResetChannels()
        {
            imgList.Images.Clear();
            channelListView.Refresh();
        }

        ImageEditorBase imageEditor;
        ImageList imgList = new ImageList();

        public void LoadImage(Bitmap picBoxImg, ImageEditorBase editor)
        {
            imageEditor = editor;

            //Resize texture to hopefully prevent slow loading
            var image = BitmapExtension.Resize(picBoxImg, 65, 65);

            imgList.Images.Clear();
            imgList.ImageSize = new Size(65, 65);
            imgList.ColorDepth = ColorDepth.Depth32Bit;

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                LoadImage(image);
                Bitmap red = BitmapExtension.ShowChannel(new Bitmap(image), STChannelType.Red);
                LoadImage(red);
                Bitmap green = BitmapExtension.ShowChannel(new Bitmap(image), STChannelType.Green);
                LoadImage(green);
                Bitmap blue = BitmapExtension.ShowChannel(new Bitmap(image), STChannelType.Blue);
                LoadImage(blue);
                Bitmap alpha = BitmapExtension.ShowChannel(new Bitmap(image), STChannelType.Alpha);
                LoadImage(alpha);

                red.Dispose();
                green.Dispose();
                blue.Dispose();
                alpha.Dispose();
            }));
            Thread.Start();

            channelListView.FullRowSelect = true;
            channelListView.SmallImageList = imgList;
        }

        private void LoadImage(Bitmap image)
        {
            if (channelListView.InvokeRequired)
            {
                channelListView.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    imgList.Images.Add(image);
                    var dummy = imgList.Handle;

                    channelListView.Refresh();
                });
            }
        }

        private void redPB_Click(object sender, EventArgs e)
        {

        }

        private void greenPB_Click(object sender, EventArgs e)
        {

        }

        private void blueBP_Click(object sender, EventArgs e)
        {

        }

        private void alphaBP_Click(object sender, EventArgs e)
        {

        }

        public void EditChannel(STChannelType ChannelType)
        {
            var Image = imageEditor.BaseImage;
            if (Image != null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = Text;
                ofd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                             "Microsoft DDS |*.dds|" +
                             "Portable Network Graphics |*.png|" +
                             "Joint Photographic Experts Group |*.jpg|" +
                             "Bitmap Image |*.bmp|" +
                             "Tagged Image File Format |*.tiff|" +
                             "All files(*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap ImportedImage = null;
                    string ext = Utils.GetExtension(ofd.FileName);
                    if (ext == ".dds")
                    {
                        DDS dds = new DDS(ofd.FileName);
                        ImportedImage = dds.GetBitmap();
                    }
                    else if (ext == ".tga")
                    {
                        ImportedImage = Paloma.TargaImage.LoadTargaImage(ofd.FileName);
                    }
                    else
                    {
                        ImportedImage = new Bitmap(ofd.FileName);
                    }

                    Bitmap newImage = BitmapExtension.ReplaceChannel(Image, ImportedImage, ChannelType);
                    imageEditor.SaveAndApplyImage(newImage, true);
                }
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void channelListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelListView.SelectedItems.Count > 0)
            {
                imageEditor.UpdateMipDisplay();
            }
        }

        private void channelListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (channelListView.SelectedItems.Count <= 0)
                return;

            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        Point p = new Point(e.X, e.Y);
                        stChannelToolstripMenu.Show(channelListView, p);
                    }
                    break;
            }
        }

        private void replaceChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Note first index (0) is RGBA display
            int ChannelIndex = channelListView.SelectedIndices[0];
            switch (ChannelIndex)
            {
                case 1:
                    EditChannel(STChannelType.Red);
                    break;
                case 2:
                    EditChannel(STChannelType.Green);
                    break;
                case 3:
                    EditChannel(STChannelType.Blue);
                    break;
                case 4:
                    EditChannel(STChannelType.Alpha);
                    break;
            }
        }

        private void stPropertyGrid1_Load(object sender, EventArgs e)
        {

        }
    }
}
