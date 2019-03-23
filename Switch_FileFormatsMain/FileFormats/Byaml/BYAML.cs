using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using ByamlExt.Byaml;
using ByamlExt;
using FirstPlugin.Turbo;

namespace FirstPlugin
{
    public class BYAML : IEditor<ByamlEditor>, IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "BYAML" };
        public string[] Extension { get; set; } = new string[] { "*.byaml", "*.byml", "*.bprm", "*.sbyml" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                if (reader.CheckSignature(2, "BY") || reader.CheckSignature(2, "YB"))
                    return true;
                else
                    return false;
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        class EditableNode
        {
            public Type type { get => Node[Index].GetType(); }
            dynamic Node;
            dynamic Index;

            public dynamic Get() => Node[Index];
            public void Set(dynamic value) => Node[Index] = value;
            public string GetTreeViewString()
            {
                if (Index is int)
                    return Node[Index].ToString();
                else
                    return Index + " : " + Node[Index].ToString();
            }

            public EditableNode(dynamic _node, dynamic _index)
            {
                Node = _node;
                Index = _index;
            }
        }

        bool IsDialog = false;
        BymlFileData data;

        public ByamlEditor OpenForm()
        {
            ByamlEditor editor = new ByamlEditor(data.RootNode, data.SupportPaths, data.Version, data.byteOrder, IsDialog, FileName);
            editor.FileFormat = this;
            editor.Text = FileName;
            editor.Dock = DockStyle.Fill;
            return editor;
        }

        public void Load(Stream stream)
        {
            CanSave = true;

            IsDialog = IFileInfo != null && IFileInfo.InArchive;

            Console.WriteLine($"IsDialog " + IsDialog);

            data = ByamlFile.LoadN(stream, false);
        }
        public void Unload()
        {

        }

        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            ByamlFile.SaveN(mem, new BymlFileData
            {
                Version = data.Version,
                byteOrder = data.byteOrder,
                SupportPaths = data.SupportPaths,
                RootNode = data.RootNode
            });

            return mem.ToArray();
        }
    }
}
