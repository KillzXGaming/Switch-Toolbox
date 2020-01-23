using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using Toolbox.Library;
using System.Windows.Forms;

namespace FirstPlugin
{
    public class QuickAccessFile : TreeNodeCustom
    {
        public QuickAccessFile(string text)
        {
            Text = text;
        }

        public override void OnClick(TreeView treeview)
        {
            ArchiveFilePanel editor = (ArchiveFilePanel)LibraryGUI.GetActiveContent(typeof(ArchiveFilePanel));
            if (editor == null)
            {
                editor = new ArchiveFilePanel();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }

            editor.LoadFile((ArchiveFileInfo)Tag);
            editor.UpdateEditor();
        }
    }

}
