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
    public partial class TextureFormatExport : Form
    {
        public int Index = 0;

        public TextureFormatExport(List<string> Formats)
        {
            InitializeComponent();

            foreach (string format in Formats)
                comboBox1.Items.Add(format);

            Index = 0;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Index = comboBox1.SelectedIndex;
        }
    }
}
