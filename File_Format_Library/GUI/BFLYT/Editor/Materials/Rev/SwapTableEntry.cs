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
using LayoutBXLYT.Revolution;

namespace LayoutBXLYT
{
    public partial class SwapTableEntryControl : STUserControl
    {
        public SwapMode SwapMode;

        private bool loaded = false;
        public SwapTableEntryControl(SwapMode swapMode)
        {
            InitializeComponent();

            swapRedCB.LoadEnum(typeof(SwapChannel));
            swapGreenCB.LoadEnum(typeof(SwapChannel));
            swapBlueCB.LoadEnum(typeof(SwapChannel));
            swapAlphaCB.LoadEnum(typeof(SwapChannel));

            LoadSwapMode(swapMode);
        }

        public void LoadSwapMode(SwapMode swapMode)  {
            SwapMode = swapMode;
            Reload();
        }

        private void Reload() {
            loaded = false;
            swapRedCB.SelectedItem = SwapMode.R;
            swapGreenCB.SelectedItem = SwapMode.G;
            swapBlueCB.SelectedItem = SwapMode.B;
            swapAlphaCB.SelectedItem = SwapMode.A;
            loaded = true;
        }

        private void swapCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex == -1)
                return;

            var item = (SwapChannel)comboBox.SelectedItem;
            if (comboBox == swapRedCB) {
                SetColorDisplay(stPanel1, item);
                if (loaded) SwapMode.R = item;
            }
            if (comboBox == swapGreenCB) {
                SetColorDisplay(stPanel2, item);
                if (loaded) SwapMode.G = item;
            }
            if (comboBox == swapBlueCB) {
                SetColorDisplay(stPanel3, item);
                if (loaded) SwapMode.B = item;
            }
            if (comboBox == swapAlphaCB) {
                SetColorDisplay(stPanel4, item);
                if (loaded) SwapMode.A = item;
            }
        }

        private void SetColorDisplay(Panel panel, SwapChannel channel)
        {
            switch(channel)
            {
                case SwapChannel.Red: panel.BackColor = Color.Red; break;
                case SwapChannel.Green: panel.BackColor = Color.Green; break;
                case SwapChannel.Blue: panel.BackColor = Color.Blue; break;
                case SwapChannel.Alpha: panel.BackColor = Color.Gray; break;
            }
            panel.Refresh();
        }
    }
}
