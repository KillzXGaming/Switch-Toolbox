using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class TextEditorForm : STForm
    {
        public TextEditorForm()
        {
            InitializeComponent();
        }

        public void OpenFile(byte[] Data)
        {
            textEditor1.FillEditor(Encoding.Default.GetString(Data));
        }
    }
}
