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
    public partial class STVerticalTabMenuItem : UserControl
    {
        public EventHandler TabSelected;

        public Color ButtonColor
        {
            set
            {
                stLabel1.BackColor = value;
                stPanel1.BackColor = value;
            }
            get { return stPanel1.BackColor; }
        }

        public STVerticalTabMenuItem()
        {
            InitializeComponent();

            this.BackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
            stLabel1.BackColor = FormThemes.BaseTheme.FormContextMenuBackColor;

            stPanel1.BackColor = FormThemes.BaseTheme.CheckBoxBackColor;
        }

        public string TabText
        {
            get { return stLabel1.Text; }
            set { stLabel1.Text = value; }
        }

        private void stLabel1_Click(object sender, EventArgs e) {
            TabSelected?.Invoke(sender, e);
        }

        private void STVerticalTabMenuItem_Click(object sender, EventArgs e) {
            TabSelected?.Invoke(sender, e);
        }
    }
}
