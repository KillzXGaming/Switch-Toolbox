using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    //A custom clonable treenode
    public interface ICloneableNode
    {
        TreeNode CloneNode();
    }
}
