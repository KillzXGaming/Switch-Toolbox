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
    public partial class BoneVisListEditor : STForm
    {
        public BoneVisListEditor()
        {
            InitializeComponent();

            CanResize = false;
            listViewCustom1.FullRowSelect = true;
        }
        List<BoneEntry> Bones = new List<BoneEntry>();

        public class BoneEntry
        {
            public string Name { get; set; }
        }

        public List<string> GetNewBones()
        {
            var inputs = new List<string>();
            foreach (BoneEntry item in Bones)
                inputs.Add(item.Name);
            
            return inputs;
        }

        public void LoadBones(List<string> bones)
        {
            listViewCustom1.Items.Clear();
            foreach (var item in bones)
            {
                listViewCustom1.Items.Add($"{item}");
                Bones.Add(new BoneEntry()
                {
                    Name = item,
                });
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                listViewCustom1.Items.RemoveAt(index);
                Bones.RemoveAt(index);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            listViewCustom1.Items.Add($" : ");
            Bones.Add(new BoneEntry()
            {
                Name = "NewBone",
            });
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                btnRemove.Enabled = true;

                int index = listViewCustom1.SelectedIndices[0];
                stPropertyGrid1.LoadProperty(Bones[index], OnPropertyChanged);
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }

        public void OnPropertyChanged() { }

        private void stButton2_Click(object sender, EventArgs e)
        {
            List<string> KeyCheck = new List<string>();
            foreach (BoneEntry item in Bones)
            {
                if (!KeyCheck.Contains(item.Name))
                    KeyCheck.Add(item.Name);
                else
                {
                    STErrorDialog.Show($"A Bones with the same name already exists! {item.Name}",
                        this.Text, "");
                    DialogResult = DialogResult.None;
                }
            }
            KeyCheck.Clear();
        }
    }
}
