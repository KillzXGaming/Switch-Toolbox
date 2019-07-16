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

namespace FirstPlugin
{
    public partial class ShaderOptionsEditBox : STForm
    {
        public ShaderOptionsEditBox()
        {
            InitializeComponent();

            CanResize = false;
        } 
        public void LoadOption(string Name, string Value)
        {
            textBoxName.Text = Name;
            textBoxValue.Text = Value;
        }
    }
}
