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
    public partial class ColorPicker : Form
    {
        public ColorPicker(Point position)
        {
            InitializeComponent();

            this.Location = position;

            FormBorderStyle = FormBorderStyle.None;

            barSlider1.Precision = 0.01f;
            barSlider2.Precision = 0.01f;
            barSlider3.Precision = 0.01f;
            barSlider4.Precision = 0.01f;
        }
    }
}
