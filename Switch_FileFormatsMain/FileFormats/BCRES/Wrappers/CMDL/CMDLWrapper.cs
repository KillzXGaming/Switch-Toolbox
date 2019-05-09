using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using BcresLibrary;

namespace FirstPlugin
{
    public class CMDLWrapper : STGenericModel
    {
        internal BCRES BcresParent;
        internal Model Model;

        public CMDLWrapper()
        {
            ImageKey = "Model";
            SelectedImageKey = "Model";
        }

        public CMDLWrapper(Model model, BCRES bcres) : base()
        {
            BcresParent = bcres;
            LoadModel(model, bcres);
        }

        public override void OnClick(TreeView treeview) {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {

        }

        public void LoadModel(Model model, BCRES bcres)
        {
            BcresParent = bcres;

            Model = model;
            Text = model.Name;

            var MaterialFolder = new TreeNode("Materials");
            Nodes.Add(MaterialFolder);
            foreach (var material in model.Materials.Values)
            {
                var matWrapper = new MTOBWrapper() { BcresParent = bcres };
                matWrapper.Load(material);
                MaterialFolder.Nodes.Add(matWrapper);
            }
        }
    }
}
