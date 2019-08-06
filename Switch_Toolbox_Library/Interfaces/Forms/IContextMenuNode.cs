using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    /// <summary>
    /// Gets the context menus from a tree node when right clicked
    /// This can be used to save memory as storing lists of menus for every node takes up too much memory
    /// </summary>
    public interface IContextMenuNode
    {
        ToolStripItem[] GetContextMenuItems();
    }
}
