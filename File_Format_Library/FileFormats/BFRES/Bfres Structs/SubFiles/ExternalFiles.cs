using System;
using Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;
using Toolbox.Library.NodeWrappers;
using Toolbox.Library.Forms;

namespace Bfres.Structs
{
    public class ExternalFileData : STGenericWrapper
    {
        public ArchiveFileInfo ArchiveFileInfo;

        public override string ExportFilter => "All files (*.*)|*.*";

        public byte[] Data
        {
            get
            {
                return ArchiveFileInfo.FileData;
            }
            set
            {
                ArchiveFileInfo.FileData = value;
            }
        }

        public ExternalFileData(string name, byte[] data)
        {
            ImageKey = "folder";

            Text = name;

            CanDelete = true;
            CanRename = true;
            CanReplace = true;
            CanExport = true;

            ArchiveFileInfo = new ArchiveFileInfo();
            ArchiveFileInfo.FileData = data;
        }

        //Todo move all of this data into one single class
        //Using ArchiveFileWrapper would be used wrong due to requring an IArchiveFile
        public override void OnDoubleMouseClick(TreeView treeview)
        {
            IFileFormat file = ArchiveFileInfo.OpenFile();
            if (file == null) //Format not supported so return
                return;

            ArchiveFileInfo.FileFormat = file;

            if (Utils.HasInterface(file.GetType(), typeof(IEditor<>)))
            {
                OpenFormDialog(file);
            }
            else if (file != null && file is TreeNodeFile)
                ReplaceNode(this, (TreeNodeFile)file);
        }

        public static void ReplaceNode(TreeNode replaceNode, TreeNodeFile NewNode)
        {
            if (NewNode == null)
                return;

            //   node.Nodes.RemoveAt(index);
            //   node.Nodes.Insert(index, NewNode);


            NewNode.ImageKey = replaceNode.ImageKey;
            NewNode.SelectedImageKey = replaceNode.SelectedImageKey;
            NewNode.Text = replaceNode.Text;
        }

        private void OpenFormDialog(IFileFormat fileFormat)
        {
            UserControl form = GetEditorForm(fileFormat);
            form.Text = (((IFileFormat)fileFormat).FileName);

            var parentForm = LibraryGUI.GetActiveForm();

            GenericEditorForm editorForm = new GenericEditorForm(true, form);
            editorForm.FormClosing += (sender, e) => FormClosing(sender, e, fileFormat);
            if (editorForm.ShowDialog() == DialogResult.OK)
            {
                if (fileFormat.CanSave)
                {
                    ArchiveFileInfo.FileData = fileFormat.Save();
                    UpdateEditor();
                }
            }
        }

        private void FormClosing(object sender, EventArgs args, IFileFormat fileFormat)
        {
            if (((Form)sender).DialogResult != DialogResult.OK)
                return;
        }

        public UserControl GetEditorForm(IFileFormat fileFormat)
        {
            Type objectType = fileFormat.GetType();
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                    return (UserControl)method.Invoke(fileFormat, new object[0]);
                }
            }
            return null;
        }

        public override void OnClick(TreeView treeview) {
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (Parent != null)
                ((BFRES)Parent.Parent).LoadEditors(this);
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
