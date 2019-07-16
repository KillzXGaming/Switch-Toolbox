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

namespace FirstPlugin.Forms
{
    public partial class VolatileFlagEditor : STForm
    {
        public VolatileFlagEditor()
        {
            InitializeComponent();

            stComboBox1.Items.Add(true);
            stComboBox1.Items.Add(false);

            CanResize = false;

            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;
        }

        bool[] VolatileBooleans;

        public void LoadFlags(bool[] Flags)
        {
            VolatileBooleans = Flags;

            int index = 0;
            foreach (var flag in Flags)
            {
                listViewCustom1.Items.Add($"Volatile Flag {index++} " + flag);
            }
        }

        private void stComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                bool value =  (bool)stComboBox1.SelectedItem;

                int index = listViewCustom1.SelectedIndices[0];
                VolatileBooleans[index] = value;
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                bool value = VolatileBooleans[listViewCustom1.SelectedIndices[0]];

                stComboBox1.SelectedItem = value;
            }
        }
    }
}
