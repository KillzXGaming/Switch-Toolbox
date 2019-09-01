using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class STColorDialog : Form
    {
        public STColorDialog(Color color)
        {
            InitializeComponent();

            colorSelector1.Color = color;
        }

        public Color NewColor
        {
            get
            {
                return colorSelector1.Color;
            }
        }
    }
}
