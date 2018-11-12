using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms.Design;
using System.Resources;
using System.Drawing.Design;
using System.Collections.Specialized;
using System.Reflection;

namespace FirstPlugin
{
    public partial class BfresProperties : UserControl
    {
        public BfresProperties()
        {
            InitializeComponent();
        }
        public void LoadProperty(object type)
        {
            propertyGrid1.SelectedObject = type;
        }
    }
}
