using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class ExportModelSettings : STForm
    {
        public bool ExportTextures
        {
            get
            {
                return exportTexturesChkBox.Checked;
            }
        }

        public ExportModelSettings()
        {
            InitializeComponent();
        }
    }
}
