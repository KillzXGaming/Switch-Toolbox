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

            listViewCustom1.Sorting = SortOrder.Ascending;

            listViewCustom1.MultiSelect = true;
        }

        public List<BFLYT> SelectedLayouts()
        {
            List<BFLYT> layouts = new List<BFLYT>();
            foreach (ListViewItem item in listViewCustom1.SelectedItems)
                layouts.Add((BFLYT)item.Tag);

            listViewCustom1.Items.Clear();

            return layouts;
        }

        public void LoadLayoutFiles(List<BFLYT> layoutFiles)
        {
            listViewCustom1.BeginUpdate();
            foreach (var file in layoutFiles)
            {
                listViewCustom1.Items.Add(new ListViewItem()
                {
                    Text = file.FileName,
                    Tag = file,
                });
            }

            listViewCustom1.Sort();
            listViewCustom1.EndUpdate();
        }

        private void listViewCustom1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }
    }
}
