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

namespace FirstPlugin.GUI.BFRES.Materials
{
    public partial class MDL0MaterialExportDialog : STForm
    {
        String ExportFlags = "UNSET";

        public MDL0MaterialExportDialog()
        {
            InitializeComponent();
        }

        public String GetExportFlags()
        {            
            return ExportFlags;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ExportFlags = "UNSET";
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            String output = "";
            if (albCheckBox.Checked)
            {
                output += "A";
            }
            if (emmCheckBox.Checked)
            {
                output += "E";
            }
            if (bake0CheckBox.Checked)
            {
                output += "S";
            }
            if (bake1CheckBox.Checked)
            {
                output += "L";
            }
            ExportFlags = output;
            this.Close();
        }
    }
}
