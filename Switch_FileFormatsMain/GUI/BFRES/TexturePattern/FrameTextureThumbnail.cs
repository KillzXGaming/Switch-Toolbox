using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class FrameTextureThumbnail : UserControl
    {
        public FrameTextureThumbnail()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.MDIParentBackColor;
            stPanel1.BackColor = FormThemes.BaseTheme.MDIParentBackColor;
        }

        public bool IsSelected { get; set; }

        public void SelectionView(bool IsSelected)
        {
            if (IsSelected)
            {
                BackColor = FormThemes.BaseTheme.FormContextMenuSelectColor;
                stPanel1.BackColor = FormThemes.BaseTheme.FormContextMenuSelectColor;
            }
            else
            {
                BackColor = FormThemes.BaseTheme.MDIParentBackColor;
                stPanel1.BackColor = FormThemes.BaseTheme.MDIParentBackColor;
            }
        }

        Action SelectListItems;

        public void LoadFrame(STGenericTexture texture, int Frame, int FrameCount, Action selectListItems)
        {
            SelectListItems = selectListItems;

            if (frameCounterLbl.InvokeRequired)
            {
                frameCounterLbl.Invoke((MethodInvoker)delegate {
                    frameCounterLbl.Text = $"Frame: {Frame}/{FrameCount}";
                });
            }
            else
                frameCounterLbl.Text = $"Frame: {Frame}/{FrameCount}";

            Bitmap image = texture.GetBitmap();

            if (pictureBoxCustom1.InvokeRequired)
            {
                pictureBoxCustom1.Invoke((MethodInvoker)delegate {
                    pictureBoxCustom1.Image = image;
                });
            }
            else
                pictureBoxCustom1.Image = image;

            if (textureNameLbl.InvokeRequired)
            {
                textureNameLbl.Invoke((MethodInvoker)delegate {
                    textureNameLbl.Text = $"Name: {texture.Text}";
                });
            }
            else
                textureNameLbl.Text = $"Name: {texture.Text}";

        }

        private void stPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void stPanel1_Click(object sender, EventArgs e)
        {
            SelectListItems();

            IsSelected = true;

            SelectionView(IsSelected);
        }

        private void pictureBoxCustom1_Click(object sender, EventArgs e)
        {
            SelectListItems();

            IsSelected = true;

            SelectionView(IsSelected);
        }

        private void frameCounterLbl_Resize(object sender, EventArgs e)
        {

        }
    }
}
