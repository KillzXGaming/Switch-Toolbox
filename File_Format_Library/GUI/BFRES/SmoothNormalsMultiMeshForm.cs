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
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class SmoothNormalsMultiMeshForm : STForm
    {
        public SmoothNormalsMultiMeshForm()
        {
            InitializeComponent();
        }

        public void LoadMeshes(List<FMDL> Models)
        {
            treeViewCustom1.BeginUpdate();
            for (int fmdl = 0; fmdl < Models.Count; fmdl++)
            {
                treeViewCustom1.Nodes.Add(new TreeNode(Models[fmdl].Text)
                {
                    Tag = Models[fmdl],
                    ImageKey = "model",
                    SelectedImageKey = "model",
                });

                for (int fshp = 0; fshp < Models[fmdl].shapes.Count; fshp++)
                {
                    treeViewCustom1.Nodes[fmdl].Nodes.Add(new TreeNode(Models[fmdl].shapes[fshp].Text)
                    {
                        ImageKey = "mesh",
                        SelectedImageKey = "mesh",
                    });
                }
            }
            treeViewCustom1.EndUpdate();
        }

        public List<STGenericObject> GetSelectedMeshes()
        {
            List<STGenericObject> Meshes = new List<STGenericObject>();
            foreach (TreeNode model in treeViewCustom1.Nodes)
            {
                foreach (TreeNode shape in model.Nodes)
                {
                    if (shape.Checked)
                        Meshes.Add(((FMDL)model.Tag).GetShape(shape.Text));
                }
            }
            return Meshes;
        }

        private void stButton2_Click(object sender, EventArgs e)
        {
            if (GetSelectedMeshes().Count == 0)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show("Make sure there is atleast one mesh that is checked!");
            }
        }

        private void treeViewCustom1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            bool HasChildren = e.Node.Nodes.Count > 0;
            if (HasChildren)
            {
                foreach (TreeNode child in e.Node.Nodes)
                    child.Checked = e.Node.Checked;
            }
        }
    }
}
