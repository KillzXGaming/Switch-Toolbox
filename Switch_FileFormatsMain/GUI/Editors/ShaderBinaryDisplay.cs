using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin.Forms
{
    public partial class ShaderBinaryDisplay : UserControl
    {
        public ShaderBinaryDisplay()
        {
            InitializeComponent();
        }

        public void FillEditor(byte[] data, string DecompiledData)
        {
            hexEditor1.LoadData(data);
            textEditor1.FillEditor(DecompiledData);
        }
    }
}
