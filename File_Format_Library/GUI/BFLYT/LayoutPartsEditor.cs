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
using Toolbox.Library.Forms;

namespace LayoutBXLYT
{
    public partial class LayoutPartsEditor : LayoutDocked
    {
        public LayoutPartsEditor()
        {
            InitializeComponent();

            listView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            listView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }
    }
}
