using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class SearchHex : Form
    {
        public FindType findType;
        public string findString;
        public bool matchCase;
        public byte[] findHex;

        public SearchHex()
        {
            InitializeComponent();
        }
        public void SearchItem()
        {

        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            var provider = this.hexFind.ByteProvider as DynamicByteProvider;
            findHex = provider.Bytes.ToArray();
            matchCase = chkBoxMatchCase.Checked;
            findString = stTextBox1.Text;

        }
    }
}
