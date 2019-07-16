using System.Collections;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class TreeChildSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var tx = x as TreeNode;
            var ty = y as TreeNode;

            // If this is a child node, preserve the same order by comparing the node Index, not the text
            if (tx.Parent != null && ty.Parent != null)
                return tx.Index - ty.Index;

            // This is a root node, compare by name.
            return string.Compare(tx.Text, ty.Text);
        }
    }
}
