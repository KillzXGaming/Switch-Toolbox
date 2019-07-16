using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Toolbox.Library.Forms;
using Toolbox.Library.Animations;

namespace FirstPlugin.Forms
{
    public partial class TexPatternInfoEditor : STForm
    {
        public TexPatternInfoEditor()
        {
            InitializeComponent();

            CanResize = false;
            btnRemove.Enabled = false;
        }

        MaterialAnimation.Material activeMat;
        MaterialAnimation ActiveMaterialAnim;

        public void LoadAnim(MaterialAnimation materialAnimation, MaterialAnimation.Material mat)
        {
            activeMat = mat;
            ActiveMaterialAnim = materialAnimation;

            foreach (var sampler in mat.Samplers)
            {
                listViewCustom1.Items.Add(sampler.Text);
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];

                btnRemove.Enabled = true;

                nameTB.Text = activeMat.Samplers[index].Text; 
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];

                string Name = activeMat.Samplers[index].Text;

                var result = MessageBox.Show($"Are you sure you want to delete sampler {Name}? This cannot be undone!",
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    activeMat.Samplers.RemoveAt(index);
                    listViewCustom1.Items.RemoveAt(index);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            activeMat.Samplers.Add(new MaterialAnimation.SamplerKeyGroup(ActiveMaterialAnim));
            listViewCustom1.Items.Add("");
        }

        private void nameTB_TextChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];

                activeMat.Samplers[index].Text = nameTB.Text;
            }
        }

        private void stCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];

                activeMat.Samplers[index].Constant = stCheckBox1.Checked;
            }
        }
    }
}
