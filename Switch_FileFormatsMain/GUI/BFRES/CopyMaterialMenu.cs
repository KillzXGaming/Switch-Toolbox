using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin
{
    public partial class CopyMaterialMenu : Form
    {
        public CopyMaterialMenu()
        {
            InitializeComponent();
        }
        private class FmdlTreeTemp : TreeNode
        {
            public FmdlTreeTemp(string Name)
            {
                Text = Name;
            }
        }
        public void LoadMaterials(string SelectedMat, List<BFRESRender.FMDL> models)
        {
            foreach (var mdl in models)
            {
                FmdlTreeTemp model = new FmdlTreeTemp(mdl.Text);
                foreach (var mat in mdl.materials.Keys)
                {
                    if (mat != SelectedMat)
                        model.Nodes.Add(mat);
                }

                materialTreeView.Nodes.Add(model);
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node is FmdlTreeTemp)
            {
                foreach (TreeNode n in e.Node.Nodes)
                {
                    n.Checked = e.Node.Checked;
                }
            }
        }
    }
}
