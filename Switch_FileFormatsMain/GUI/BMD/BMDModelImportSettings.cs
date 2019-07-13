using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public partial class BMDModelImportSettings : STForm
    {
        public string TexturePath => texturePathTB.Text;
        public string MaterialPath => materalPathTB.Text;

        public BMDModelImportSettings()
        {
            InitializeComponent();
            stComboBox1.SelectedIndex = 0;
        }

        private void texturePathTB_TextChanged(object sender, EventArgs e)
        {
            FolderSelectDialog ofd = new FolderSelectDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                texturePathTB.Text = ofd.SelectedPath;
        }

        private void materalPathTB_TextChanged(object sender, EventArgs e)
        {
            FolderSelectDialog ofd = new FolderSelectDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                materalPathTB.Text = ofd.SelectedPath;
        }
    }
}
