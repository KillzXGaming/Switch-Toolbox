using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.NodeWrappers;

namespace Switch_Toolbox.Library
{
    public class STGenericModel : STGenericWrapper
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
