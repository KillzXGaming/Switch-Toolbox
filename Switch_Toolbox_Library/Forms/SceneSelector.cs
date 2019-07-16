using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class SceneSelector : STForm
    {
        public string SelectedFile = "";

        public SceneSelector()
        {
            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public void LoadDictionary(Dictionary<string,string> Files)
        {
            treeView1.BeginUpdate();

            foreach (var file in Files)
                treeView1.Nodes.Add(new TreeNode(file.Value) { Tag = file.Key, });

            treeView1.EndUpdate();
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            SelectedFile = (string)treeView1.SelectedNode.Tag;
            DialogResult = DialogResult.OK;
        }
    }
}
