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
    public partial class ParamPatternMaterialEditor : STForm
    {
        public ParamPatternMaterialEditor()
        {
            InitializeComponent();

            CanResize = false;
            btnRemove.Enabled = false;
        }

        MaterialAnimation activeAnim;

        public void LoadAnim(MaterialAnimation anim)
        {
            activeAnim = anim;

            foreach (var material in anim.Materials)
            {
                listViewCustom1.Items.Add(material.Text);
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                btnRemove.Enabled = true;
                int index = listViewCustom1.SelectedIndices[0];
                nameTB.Text = activeAnim.Materials[index].Text; 
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

                string Name = activeAnim.Materials[index].Text;

                var result = MessageBox.Show($"Are you sure you want to delete material {Name}? This cannot be undone!",
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    activeAnim.Materials.RemoveAt(index);
                    listViewCustom1.Items.RemoveAt(index);

                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            activeAnim.Materials.Add(new MaterialAnimation.Material() { Text = "NewMaterial"});
            listViewCustom1.Items.Add("NewMaterial");
        }

        private void nameTB_TextChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];

                activeAnim.Materials[index].Text = nameTB.Text;
            }
        }
    }
}
