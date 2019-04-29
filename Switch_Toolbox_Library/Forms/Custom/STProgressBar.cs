using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public partial class STProgressBar : Form
    {
        public STProgressBar()
        {
            InitializeComponent();
        }

        public int Value
        {
            set
            {
                if (value > 100)
                    progressBar1.Value = 0;
                else
                    progressBar1.Value = value;

                if (value >= 100)
                    Close();
                progressBar1.Refresh();
            }
        }

        public bool IsConstant
        {
            set
            {
                if (value)
                    progressBar1.Style = ProgressBarStyle.Marquee;
                else
                    progressBar1.Style = ProgressBarStyle.Blocks;
            }
            get
            {
                return progressBar1.Style == ProgressBarStyle.Blocks;
            }
        }

        public string Task
        {
            set
            {
                label1.Text = value;
                label1.Refresh();
            }
        }

        private void ProgressBar_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void ProgressBarWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
