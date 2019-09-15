using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using LibHac;
using LibHac.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace FirstPlugin
{
    public class RomfsNodeWrapper
    {
        public Romfs romfs { get; set; }

        public void FillTreeNodes(TreeNodeCollection nodes, RomfsDir romfsDir)
        {
            RomfsFile fileNode = romfsDir.FirstFile;

            while (fileNode != null)
            {
                var node = new FileNode(fileNode.FullPath, fileNode.Name);
                node.romfs = romfs;

                nodes.Add(node);
                fileNode = fileNode.NextSibling;
            }

            RomfsDir dirNode = romfsDir.FirstChild;

            while (dirNode != null)
            {
                TreeNode newNode = nodes.Add(dirNode.FullPath, dirNode.Name);
                FillTreeNodes(newNode.Nodes, dirNode);
                dirNode = dirNode.NextSibling;
            }
        }

        public class FileNode : TreeNodeCustom
        {
            public Romfs romfs;

            private string GetFilePath()
            {
                string path = TreeView.SelectedNode.FullPath;
                string filePath = path.Substring(path.IndexOf(@"\", 1) + 1);
                return filePath.Replace(@"\", "/");
            }

            public FileNode(string Key, string Name)
            {
                Text = Name;
                ImageKey = "fileBlank";
                SelectedImageKey = "fileBlank";
            }
            public override void OnClick(TreeView treeview)
            {
                string filePath = GetFilePath();
                if (!romfs.FileDict.ContainsKey($"/{filePath}"))
                    return;

                var file = romfs.FileDict[$"/{filePath}"];



                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;

                using (var stream = romfs.OpenFile(file).AsStream())
                {
                    var mem = new MemoryStream();
                    stream.CopyTo(mem);

                    editor.LoadData(mem.ToArray());
                }
            }

            public override void OnDoubleMouseClick(TreeView treeview)
            {
                string filePath = GetFilePath();

                if (!romfs.FileDict.ContainsKey($"/{filePath}"))
                    return;

                var text = treeview.SelectedNode.Text;

                var file = romfs.FileDict[$"/{filePath}"];
                var stream = romfs.OpenFile(file).AsStream();

                object fileFormat = STFileLoader.OpenFileFormat(stream , text,  false, true);

                if (fileFormat == null)
                    return;

                Type objectType = fileFormat.GetType();

                bool HasEditorActive = false;
                foreach (var inter in objectType.GetInterfaces())
                {
                    if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                    {
                        MethodInfo method = objectType.GetMethod("OpenForm");
                        var form = (STForm)method.Invoke(fileFormat, new object[0]);
                        form.Text = text;
                        LibraryGUI.CreateMdiWindow(form, true);

                        HasEditorActive = true;
                    }
                }
                if (HasEditorActive)
                {
                    return;
                }

                //ObjectEditor is for treenode types. Editors will be on the right side, treenodes on the left
                ObjectEditor editor = new ObjectEditor((IFileFormat)fileFormat);
                editor.Text = text;
                LibraryGUI.CreateMdiWindow(editor, true);
            }
        }

    }
}