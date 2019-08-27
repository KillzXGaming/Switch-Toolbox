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

        public enum DockLayout
        {
            Default,
            Animation,
        }

        public LayoutEditor()
        {
            InitializeComponent();
        }

        public void LoadBflyt(BFLYT.Header header, string fileName)
        {
            LayoutViewer viewer = new LayoutViewer();
            viewer.Dock = DockStyle.Fill;
            this.Controls.Add(viewer);
        }

        public void LoadBflan()
        {

        }

        public void InitalizeEditors()
        {

        }
    }
}
