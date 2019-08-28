using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin.Forms
{
    public partial class LayoutEditor : UserControl
    {
        public List<BFLYT.Header> LayoutFiles = new List<BFLYT.Header>();

        private BFLYT.Header ActiveLayout;

        public enum DockLayout
        {
            Default,
            Animation,
        }

        public LayoutEditor()
        {
            InitializeComponent();
        }

        private bool isLoaded = false;
        public void LoadBflyt(BFLYT.Header header, string fileName)
        {
            if (isLoaded) return;

            LayoutViewer viewer = new LayoutViewer();
            viewer.Dock = DockStyle.Fill;
            viewer.TopLevel = false;
            viewer.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            stPanel1.Controls.Add(viewer);

            isLoaded = true;
            ActiveLayout = header;
        }



        public void LoadBflan()
        {

        }

        public void InitalizeEditors()
        {

        }

        private void LayoutEditor_ParentChanged(object sender, EventArgs e)
        {
            if (this.ParentForm == null) return;
        }

        private void textureListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutTextureList textureListForm = new LayoutTextureList();
            textureListForm.LoadTextures(ActiveLayout);

            if (ParentForm != null && ParentForm.TopLevel)
                textureListForm.Show(ParentForm);
            else
                textureListForm.Show();
        }
    }
}
