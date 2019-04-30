using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class HexEditor : UserControl
    {
        FindOptions _findOptions = new FindOptions();

        public bool EnableMenuBar
        {
            set
            {
                if (value)
                    stContextMenuStrip1.Show();
                else
                    stContextMenuStrip1.Hide();
            }
            get
            {
                return stContextMenuStrip1.Visible;
            }
        }

        public HexEditor()
        {
            InitializeComponent();

            hexBox1.BackColor = FormThemes.BaseTheme.FormBackColor;
            hexBox1.ForeColor = FormThemes.BaseTheme.FormForeColor;
            hexBox1.SelectionBackColor = FormThemes.BaseTheme.FormContextMenuSelectColor;
            hexBox1.SelectionForeColor = FormThemes.BaseTheme.FormForeColor;
        }
        public void LoadData(byte[] data)
        {
            hexBox1.ByteProvider = new DynamicByteProvider(data);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchHex searchHex = new SearchHex();
            searchHex.HexBox = hexBox1;
            searchHex.FindOptions = _findOptions;

            if (searchHex.ShowDialog() == DialogResult.OK)
            {
                FindOptions options = new FindOptions();
                options.MatchCase = searchHex.matchCase;
                options.Type = searchHex.findType;

                if (options.Type == FindType.Hex)
                    options.Hex = searchHex.findHex;
                else
                    options.Text = searchHex.findString;
            }
        }
    }
}
