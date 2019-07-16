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
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class SamplerListEdit : STForm
    {
        public SamplerListEdit()
        {
            InitializeComponent();
            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;
        }
        List<string> Samplers = new List<string>();

        public void LoadSamplers(List<string> samplers)
        {
            Samplers = samplers;

            foreach (var item in Samplers)
                listViewCustom1.Items.Add(item);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                listViewCustom1.Items.RemoveAt(index);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Runtime.UseEditDebugMode)
                Samplers.Add("");
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];

                btnRemove.Enabled = true;

                stPropertyGrid1.LoadProperty(Samplers[index], OnPropertyChanged);
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }

        public void OnPropertyChanged() { }
    }
}
