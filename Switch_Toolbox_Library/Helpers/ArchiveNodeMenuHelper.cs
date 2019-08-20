using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace Toolbox.Library
{
    public class ArchiveNodeMenuHelper
    {
        private ArchiveFileInfo FileInfo;
        private TreeNode Node;

        private void RenameAction(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Node.Text);

            if (dialog.ShowDialog() == DialogResult.OK) { Node.Text = dialog.textBox1.Text; }
        }
    }
}
