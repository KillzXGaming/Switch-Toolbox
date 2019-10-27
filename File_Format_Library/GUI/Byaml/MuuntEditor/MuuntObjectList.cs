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

namespace FirstPlugin.MuuntEditor
{
    public partial class MuuntObjectList : MuuntEditorDocker
    {
        private MuuntEditor ParentEditor;

        private List<ObjectGroup> Groups;

        public MuuntObjectList(MuuntEditor editor)
        {
            InitializeComponent();
            ParentEditor = editor;

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public void LoadObjects(List<ObjectGroup> groups)
        {
            Groups = groups;

            stComboBox1.Items.Clear();
            for (int i = 0; i < groups.Count; i++)
            {
                stComboBox1.Items.Add(groups[i].Name);
            }
        }

        private void stComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            if (stComboBox1.SelectedIndex >= 0)
            {
                int index = stComboBox1.SelectedIndex;
                var group = Groups[index];

                LoadPropertyObjects(group.Objects);
            }
        }

        private void LoadPropertyObjects(List<PropertyObject> properties, TreeNode parent = null)
        {
            if (properties == null) return;

            foreach (var prob in properties)
            {
                TreeNode nodeWrapper = new TreeNode(prob.Name);
                nodeWrapper.Name = prob.Name;
                nodeWrapper.Tag = prob;

                if (parent == null)
                    treeView1.Nodes.Add(nodeWrapper);
                else
                    parent.Nodes.Add(nodeWrapper);

                LoadPropertyObjects(prob.SubObjects, nodeWrapper);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Tag != null)
                ParentEditor.ObjectSelected.Invoke(sender, e);
        }
    }
}
