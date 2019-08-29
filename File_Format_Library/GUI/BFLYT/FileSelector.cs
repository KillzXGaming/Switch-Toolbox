using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using FirstPlugin;

namespace LayoutBXLYT
{
    public partial class FileSelector : STForm
    {
        public FileSelector()
        {
            InitializeComponent();

            listViewCustom1.MultiSelect = true;
        }

        public List<int> SelectedIndices()
        {
            List<int> indices = new List<int>();
            foreach (int index in listViewCustom1.SelectedIndices)
                indices.Add(index);

            return indices;
        }

        public void LoadLayoutFiles(List<BFLYT> layoutFiles)
        {
            listViewCustom1.BeginUpdate();
            foreach (var file in layoutFiles)
                listViewCustom1.Items.Add(file.FileName);
            listViewCustom1.EndUpdate();
        }

        private void listViewCustom1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }
    }
}
