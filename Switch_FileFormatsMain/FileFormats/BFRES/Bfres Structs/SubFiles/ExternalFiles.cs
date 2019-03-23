using System;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;
using Switch_Toolbox.Library.NodeWrappers;
using Switch_Toolbox.Library.Forms;

namespace Bfres.Structs
{
    public class ExternalFileData : STGenericWrapper
    {
        public override string ExportFilter => "All files (*.*)|*.*";

        //Format to attach
        public IFileFormat FileFormat;

        public byte[] Data;
        public ExternalFileData(string name, byte[] data)
        {
            ImageKey = "folder";

            Text = name;
            Data = data;

            CanDelete = true;
            CanRename = true;
            CanReplace = true;
            CanExport = true;
        }

        public override void OnClick(TreeView treeview) {
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            HexEditor editor = (HexEditor)LibraryGUI.Instance.GetActiveContent(typeof(HexEditor));
            if (editor == null)
            {
                editor = new HexEditor();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.Instance.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.LoadData(Data);
        }
        
        public override void Replace(string FileName)
        {
            Data = System.IO.File.ReadAllBytes(FileName);

            UpdateEditor();
        }

        public override void Export(string FileName)
        {
            System.IO.File.WriteAllBytes(FileName, Data);
        }
    }
}
