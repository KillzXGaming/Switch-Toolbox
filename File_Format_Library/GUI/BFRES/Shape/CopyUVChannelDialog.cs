using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class CopyUVChannelDialog : STForm
    {
        public CopyUVChannelDialog()
        {
            InitializeComponent();

            CanResize = false;
        }

        public int SourceIndex;
        public int DestIndex;

        public void LoadUVAttributes()
        {
            sourceCB.Items.Add($"_u0");
            sourceCB.Items.Add($"_u1");
            sourceCB.Items.Add($"_u2");

            destCB.Items.Add($"_u0");
            destCB.Items.Add($"_u1");
            destCB.Items.Add($"_u2");

            sourceCB.SelectedIndex = 0;
            destCB.SelectedIndex = 1;
        }

        private void sourceCB_SelectedIndexChanged(object sender, EventArgs e) {
            SourceIndex = sourceCB.SelectedIndex;
        }

        private void destCB_SelectedIndexChanged(object sender, EventArgs e) {
            DestIndex = destCB.SelectedIndex;
        }
    }
}
