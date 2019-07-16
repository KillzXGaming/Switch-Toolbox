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
    public partial class SamplerInputListEdit : STForm
    {
        public SamplerInputListEdit()
        {
            InitializeComponent();
            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;

            CanResize = false;
            listViewCustom1.FullRowSelect = true;
        }
        List<SamplerInput> SamplerInputs = new List<SamplerInput>();

        public class SamplerInput
        {
            public string ResourceInput { get; set; }
            public string ShaderVariable { get; set; }
        }

        public Dictionary<string, string> GetNewInputs()
        {
            var inputs = new Dictionary<string, string>();
            foreach (SamplerInput item in SamplerInputs)
            {
                inputs.Add(item.ShaderVariable, item.ResourceInput);
            }
            return inputs;
        }

        public void LoadSamplers(Dictionary<string, string> samplers)
        {
            listViewCustom1.Items.Clear();
            foreach (var item in samplers)
            {
                listViewCustom1.Items.Add($"{item.Key} : {item.Value}");
                SamplerInputs.Add(new SamplerInput()
                {
                    ShaderVariable = item.Key,
                    ResourceInput = item.Value,
                });
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                var result = MessageBox.Show($"Are you sure you want to remove {listViewCustom1.SelectedItems[0].Text}? This could potentially break things!",
                "Shader Option Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    int index = listViewCustom1.SelectedIndices[0];
                    listViewCustom1.Items.RemoveAt(index);
                    SamplerInputs.RemoveAt(index);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            listViewCustom1.Items.Add($" : ");
            SamplerInputs.Add(new SamplerInput()
            {
                ResourceInput = "",
                ShaderVariable = "",
            });
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                btnRemove.Enabled = true;
                btnScrollDown.Enabled = true;
                btnScrollUp.Enabled = true;

                int index = listViewCustom1.SelectedIndices[0];
                stPropertyGrid1.LoadProperty(SamplerInputs[index], OnPropertyChanged);
            }
            else
            {
                btnRemove.Enabled = false;
                btnScrollDown.Enabled = true;
                btnScrollUp.Enabled = true;
            }
        }

        public void OnPropertyChanged()
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                listViewCustom1.Items[index].Text = $"{SamplerInputs[index].ShaderVariable} : {SamplerInputs[index].ResourceInput}";
            }
        }

        private void btnScrollDown_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                var item = listViewCustom1.Items[index];
                if (index < listViewCustom1.Items.Count)
                {
                    listViewCustom1.Items.RemoveAt(index);
                    listViewCustom1.Items.Insert(index + 1, item);
                }
            }
        }

        private void btnScrollUp_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                var item = listViewCustom1.Items[index];
                if (index > 0)
                {
                    listViewCustom1.Items.RemoveAt(index);
                    listViewCustom1.Items.Insert(index - 1, item);
                }
            }
        }

        private void stButton2_Click(object sender, EventArgs e)
        {
            List<string> KeyCheck = new List<string>();
            foreach (SamplerInput item in SamplerInputs)
            {
                if (!KeyCheck.Contains(item.ShaderVariable))
                    KeyCheck.Add(item.ShaderVariable);
                else
                {
                    STErrorDialog.Show($"A Shader Variable Input with the same name already exists! {item.ShaderVariable}",
                        this.Text, "");
                    DialogResult = DialogResult.None;
                }
            }
            KeyCheck.Clear();
        }

        private void stPropertyGrid1_Load(object sender, EventArgs e)
        {
        
        }
    }
}
