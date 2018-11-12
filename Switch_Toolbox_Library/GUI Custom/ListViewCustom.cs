using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Switch_Toolbox.Library.Forms
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ListView))]
    public class ListViewCustom : ListView
    {
        public ListViewCustom()
        {
            this.DoubleBuffered = true;
        }
    }
}
