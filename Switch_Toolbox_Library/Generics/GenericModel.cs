using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public class STGenericModel : TreeNodeCustom
    {
        public STGenericModel()
        {
            Checked = true;
        }

        public override void OnClick(TreeView treeView)
        {

        }
    }
}
