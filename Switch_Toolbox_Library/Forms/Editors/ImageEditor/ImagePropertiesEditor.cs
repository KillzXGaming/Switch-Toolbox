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

        public void LoadProperties(object prop)
        {
            stPropertyGrid1.LoadProperty(prop, OnPropertyChanged);
            stPropertyGrid1.Refresh();
        }

        public void UpdateProperties() => stPropertyGrid1.Refresh();
        

        public void OnPropertyChanged() { }

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

        public Bitmap getThumb(Bitmap image)
        {

            int tw, th, tx, ty;

            int w = image.Width;

            int h = image.Height;

            double whRatio = (double)w / h;

            if (image.Width >= image.Height)
            {

                tw = 100;

                th = (int)(tw / whRatio);
            }
            else
            {
                th = 100;

                tw = (int)(th * whRatio);
            }

            tx = (100 - tw) / 2;
            ty = (100 - th) / 2;

            Bitmap thumb = new Bitmap(100, 100, PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(thumb);

            g.Clear(Color.White);

            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.DrawImage(image,

            new Rectangle(tx, ty, tw, th),
            new Rectangle(0, 0, w, h),

            GraphicsUnit.Pixel);

            return thumb;

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
    }
}
