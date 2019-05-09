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
        public CMDLWrapper()
        {
            ImageKey = "Model";
            SelectedImageKey = "Model";
        }
        public CMDLWrapper(Model model) : base() { LoadModel(model); }

        internal Model Model;

        public override void OnClick(TreeView treeview)
        {

        }

        public void LoadModel(Model model)
        {
            Model = model;

            Text = model.Name;
        }
    }
}
