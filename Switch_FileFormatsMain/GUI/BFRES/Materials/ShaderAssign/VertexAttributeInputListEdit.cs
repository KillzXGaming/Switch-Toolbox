using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class VertexAttributeInputListEdit : STForm
    {
        public VertexAttributeInputListEdit()
        {
            InitializeComponent();
            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;

            CanResize = false;
            listViewCustom1.FullRowSelect = true;
        }
        List<AtributeInput> AtributeInputs = new List<AtributeInput>();

        public class AtributeInput
        {
            public string ResourceInput { get; set; }
            public string ShaderVariable { get; set; }
        }

        public Dictionary<string, string> GetNewInputs()
        {
            var inputs = new Dictionary<string, string>();
            foreach (AtributeInput item in AtributeInputs)
            {
                inputs.Add(item.ResourceInput, item.ShaderVariable);
            }
            return inputs;
        }

        public void LoadAtributes(Dictionary<string, string> atributes)
        {
            listViewCustom1.Items.Clear();
            foreach (var item in atributes)
            {
                listViewCustom1.Items.Add($"{item.Key} : {item.Value}");
                AtributeInputs.Add(new AtributeInput()
                {
                    ResourceInput = item.Key,
                    ShaderVariable = item.Value,
                });
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                listViewCustom1.Items.RemoveAt(index);
                AtributeInputs.RemoveAt(index);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            listViewCustom1.Items.Add($" : ");
            AtributeInputs.Add(new AtributeInput()
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
                stPropertyGrid1.LoadProperty(AtributeInputs[index], OnPropertyChanged);
            }
            else
            {
                btnRemove.Enabled = false;
                btnScrollDown.Enabled = true;
                btnScrollUp.Enabled = true;
            }
        }

        public void OnPropertyChanged() { }

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
            foreach (AtributeInput item in AtributeInputs)
            {
                if (!KeyCheck.Contains(item.ResourceInput))
                    KeyCheck.Add(item.ResourceInput);
                else
                {
                    STErrorDialog.Show($"A Resource Input with the same name already exists! {item.ResourceInput}",
                        this.Text, "");
                    DialogResult = DialogResult.None;
                }
            }
            KeyCheck.Clear();
        }
    }
}
