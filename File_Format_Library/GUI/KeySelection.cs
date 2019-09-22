using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using LibHac;

namespace Toolbox
{
    public partial class KeySelectionForm : STForm
    {
        public KeySelectionForm()
        {
            InitializeComponent();
        }
        public string ProdKeyPath;
        public string TitleKeyPath;

        private void setProdKeyPath_Click(object sender, EventArgs e)
        {
            ProdKeyPath = GetOpenedFileName();
            TextBoxProdKeyPath.Text = ProdKeyPath;

            if (File.Exists(ProdKeyPath))
                TextBoxProdKeyPath.BackColor = Color.White;

            CheckKeys();
        }

        private void setTitleKeyPath_Click(object sender, EventArgs e)
        {
            TitleKeyPath = GetOpenedFileName();
            TextBoxTitleKey.Text = TitleKeyPath;

            if (File.Exists(TitleKeyPath))
                TextBoxTitleKey.BackColor = Color.White;

            CheckKeys();
        }
        private void CheckKeys()
        {
            if (File.Exists(ProdKeyPath) && File.Exists(TitleKeyPath))
                btnOk.Enabled = true;
            else
                btnOk.Enabled = false;
        }

        private string GetOpenedFileName()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileName;
            else return null;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }
    }
}
