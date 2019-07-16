using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;

namespace Toolbox
{
    public partial class PluginManager : Form
    {
        public PluginManager()
        {
            InitializeComponent();
            imageList1.Images.Add("DLL", System.Drawing.SystemIcons.Question.ToBitmap());
            imageList1.ImageSize = new Size(24, 24);
            listView1.FullRowSelect = true;
            listView1.SmallImageList = imageList1;
            

            foreach (var plugin in GenericPluginLoader._Plugins)
            {
                ListViewItem item = new ListViewItem();
                item.Text = plugin.Key;
                item.ImageKey = "DLL";
                item.SubItems.Add(plugin.Value.Version);
                listView1.Items.Add(item);
            }
        }
    }
}
