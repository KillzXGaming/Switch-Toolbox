using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    /// <summary>
    /// Reprenets a clonable tree node.
    /// This is currently not used, but was planned for the search feature
    /// </summary>
    public interface ICloneableNode
    {
        TreeNode CloneNode();
    }
}
