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

namespace Toolbox
{
    public partial class HashCalculatorForm : STForm
    {
        private bool IsHex => chkUseHex.Checked;

        public HashCalculatorForm()
        {
            InitializeComponent();

            hashTypeCB.Items.Add("NLG_Hash");
            hashTypeCB.SelectedIndex = 0;
        }

        private void stTextBox1_TextChanged(object sender, EventArgs e) {
            UpdateHash();
        }

        private void UpdateHash()
        {
            uint Hash = 0;
            if (hashTypeCB.GetSelectedText() == "NLG_Hash") 
                Hash = StringToHash(stringTB.Text);

            if (IsHex)
                resultTB.Text = Hash.ToString("X");
            else
                resultTB.Text = Hash.ToString();
        }

        private void chkUseHex_CheckedChanged(object sender, EventArgs e) {
            UpdateHash();
        }

        public static uint StringToHash(string name, bool caseSensative = false)
        {
            //From (Works as tested comparing hashbin strings/hashes
            //https://gist.github.com/RoadrunnerWMC/f4253ef38c8f51869674a46ee73eaa9f
            byte[] data = Encoding.Default.GetBytes(name);

            int h = -1;
            for (int i = 0; i < data.Length; i++)
            {
                int c = (int)data[i];
                if (caseSensative && ((c - 65) & 0xFFFFFFFF) <= 0x19)
                    c |= 0x20;

                h = (int)((h * 33 + c) & 0xFFFFFFFF);
            }

            return (uint)h;
        }

    }
}
