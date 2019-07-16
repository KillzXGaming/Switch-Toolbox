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
    public partial class ImageEditorForm : STForm
    {
        public ImageEditorForm(bool UseDialog)
        {
            InitializeComponent();

            if (!UseDialog)
            {
                btnSave.Visible = false;
                btnCancel.Visible = false;
                stPanel1.Dock = DockStyle.Fill;
            }
        }

        public IFileFormat GetActiveFile()
        {
            return (IFileFormat)editorBase.ActiveTexture;
        }

        private void toggleAlphaChk_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
