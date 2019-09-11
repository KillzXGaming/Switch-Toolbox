using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Toolbox.Library
{
    public partial class STProgressBar : Form
    {
        private Thread Thread;

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

                progressBar1.Refresh();

                if (value >= 100)
                    Close();
            }
        }

        public bool IsConstant
        {
            set
            {
                if (value)
                {
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar1.MarqueeAnimationSpeed = 100;
                }
                else
                    progressBar1.Style = ProgressBarStyle.Blocks;
            }
            get
            {
                return progressBar1.Style == ProgressBarStyle.Marquee;
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

        /// <summary>
        /// Runs an action on another thread. Runs unless the user cancels it or finishes
        /// </summary>
        /// <param name="action"></param>
        public void RunAction(Action action)
        {
            Thread = new Thread(
            () =>
            {
                try { action();}
                finally
                {
                    Value = 100;
                }
            }) { IsBackground = true };
            Thread.Start();
        }

        private void ProgressBar_FormClosed(object sender, FormClosedEventArgs e) {
            Thread?.Abort();
        }

        private void ProgressBarWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
