using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Forms
{
    public class ExpandCollapseEventArgs : EventArgs
    {
        public bool IsExpanded { get; private set; }
        public ExpandCollapseEventArgs(bool isExpanded)
        {
            IsExpanded = isExpanded;
        }
    }

}
