using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class NLOC_Wrapper : TreeNodeCustom
    {
        public Stream DataStream;

        public NLG_NLOC LocFile;

        public NLOC_Wrapper(string name, Stream stream)
        {
            Text = name;
            DataStream = stream;
        }

        public override void OnClick(TreeView treeview)
        {
            if (LocFile == null) {
                LocFile = (NLG_NLOC)STFileLoader.OpenFileFormat(DataStream, "", true, true);
            }

            TextEditor editor = (TextEditor)LibraryGUI.GetActiveContent(typeof(TextEditor));
            if (editor == null)
            {
                editor = new TextEditor();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.FillEditor(LocFile.ConvertToString());
        }
    }
}
