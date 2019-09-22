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
    public partial class LzmaSettingsForm : STForm
    {
        public bool WriteMagicHeader => useMagicHeaderCB.Checked;
        public bool WriteDecomSize => writeDecompSizeCB.Checked;
        public bool WriteProperties => writePropertiesCB.Checked;

        public LzmaSettingsForm()
        {
            InitializeComponent();
            CanResize = false;
        }
    }
}
