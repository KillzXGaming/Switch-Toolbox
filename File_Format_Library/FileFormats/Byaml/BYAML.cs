using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using ByamlExt.Byaml;
using ByamlExt;
using SharpYaml.Serialization;
using SharpYaml;
using Syroot.BinaryData;

namespace FirstPlugin
{
    public class BYAML : IEditor<ByamlEditor>, IFileFormat, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Parameter;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "BYAML" };
        public string[] Extension { get; set; } = new string[] { "*.byaml", "*.byml", "*.bprm", "*.sbyml" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                //File too small to have any data
                if (stream.Length <= 8)
                    return false;

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
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Yaml;
        public bool CanConvertBack => true;

        public string ConvertToString()
        {
            if (TextFileType == TextFileType.Xml)
                return XmlByamlConverter.ToXML(BymlData);
            else
                return YamlByamlConverter.ToYaml(BymlData);
        }

        public void ConvertFromString(string text)
        {
            if (TextFileType == TextFileType.Xml)
                BymlData = XmlByamlConverter.FromXML(text);
            else
                BymlData = YamlByamlConverter.FromYaml(text);
        }

        #endregion

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => null;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => toolFileExt;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] toolFileExt = new STToolStripItem[1];

            public MenuExt()
            {
                toolFileExt[0] = new STToolStripItem("BYAML");
                toolFileExt[0].DropDownItems.Add(new STToolStripItem("Convert to Big Endian", ConvertLEtoBE));
                toolFileExt[0].DropDownItems.Add(new STToolStripItem("Convert to Little Endian", ConvertBEtoLE));
            }

            public void ConvertLEtoBE(object sender, EventArgs args)
            {
                var byamlF = new BYAML();
                byamlF.IFileInfo = new IFileInfo();

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = Utils.GetAllFilters(byamlF);
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byamlF.Load(new FileStream(ofd.FileName, FileMode.Open));
                    byamlF.BymlData.byteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = Utils.GetAllFilters(byamlF);
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        STFileSaver.SaveFileFormat(byamlF, sfd.FileName);
                    }
                }
            }


            public void ConvertBEtoLE(object sender, EventArgs args)
            {
                var byamlF = new BYAML();
                byamlF.IFileInfo = new IFileInfo();

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = Utils.GetAllFilters(byamlF);
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byamlF.Load(new FileStream(ofd.FileName, FileMode.Open));
                    byamlF.BymlData.byteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = Utils.GetAllFilters(byamlF);
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        STFileSaver.SaveFileFormat(byamlF, sfd.FileName);
                    }
                }
            }
        }


        class EditableNode
        {
            public Type type { get { return Node[Index].GetType(); } }
            dynamic Node;
            dynamic Index;

            public dynamic Get() { return Node[Index]; }

            public void Set(dynamic value) {  Node[Index] = value; }
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
        public BymlFileData BymlData;

        public ByamlEditor OpenForm()
        {
            ByamlEditor editor = new ByamlEditor();
            editor.FileFormat = this;
            editor.Text = FileName;
            editor.Dock = DockStyle.Fill;

         /*   if (FileName.Contains("_muunt"))
            {
                var muuntEditor = new MuuntEditor.MuuntEditor();
                muuntEditor.LoadByaml(data.RootNode, FileName);
                muuntEditor.Show();
            }*/
            return editor;
        }

        public void FillEditor(UserControl control)
        {
            ((ByamlEditor)control).UpdateByaml(
                BymlData.RootNode,
                BymlData.SupportPaths,
                BymlData.Version,
                BymlData.byteOrder, 
                IsDialog, this);
        }

        public void Load(Stream stream)
        {
            CanSave = true;

            //Keep the stream open. 
            //This is for the file to optionally be reloaded for different encoding types
            IsDialog = IFileInfo != null && IFileInfo.InArchive;

            BymlData = ByamlFile.LoadN(stream);
        }

        public void ReloadEncoding(Encoding encoding) {
            BymlFileData.Encoding = encoding;

            //Reopen and reload the byml data
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    var name = Path.GetFileName(file.FileName);
                    if (name == FileName)
                        BymlData = ByamlFile.LoadN(new MemoryStream(file.FileData));
                }
            }
            else if (File.Exists(FilePath))
            {
                var file = File.OpenRead(FilePath);
                BymlData = ByamlFile.LoadN(file);
                file.Close();
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            ByamlFile.SaveN(stream, new BymlFileData
            {
                Version = BymlData.Version,
                byteOrder = BymlData.byteOrder,
                SupportPaths = BymlData.SupportPaths,
                RootNode = BymlData.RootNode
            });
        }   
    }
}
