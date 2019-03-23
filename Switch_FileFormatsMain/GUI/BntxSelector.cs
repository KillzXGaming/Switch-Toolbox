using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin
{
    public partial class BntxSelector : Form
    {
        public BntxSelector()
        {
            InitializeComponent();

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                listView1.Items.Add(bntx.Text);
            }
        }
        public BNTX GetBNTX()
        {
            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                if (bntx.Text == listView1.SelectedItems[0].Text)
                    return bntx;
            }
            throw new Exception("This shouldn't happen???");
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
