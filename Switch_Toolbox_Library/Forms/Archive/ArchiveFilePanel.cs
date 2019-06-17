using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    //The panel when a file in an archive is clicked on
    //Configures the editor view, ie Hex, Editor, Text Edtior, etc
    public partial class ArchiveFilePanel : UserControl
    {
        ArchiveFileInfo ArchiveFileInfo;

        private bool _IsLoaded = false;

        public ArchiveFilePanel()
        {
            InitializeComponent();

            ReloadEditors();
            _IsLoaded = true;
        }

        public void LoadFile(ArchiveFileInfo archiveFileInfo) {
            ArchiveFileInfo = archiveFileInfo;
        }

        private void ReloadEditors()
        {
            stComboBox1.Items.Clear();
            stComboBox1.Items.Add("Hex Editor");
            stComboBox1.Items.Add("File Editor");
            stComboBox1.Items.Add("Text Editor");
            stComboBox1.SelectedIndex = 0;
        }

        public void SetEditor(int Index) { stComboBox1.SelectedIndex = Index; }
        public int GetEditor() { return stComboBox1.SelectedIndex; }

        public UserControl GetActiveEditor(Type type)
        {
            foreach (var control in stPanel1.Controls)
            {
                if (control.GetType() == type)
                    return (UserControl)control;
            }
            return null;
        }

        public void UpdateEditor()
        {
            if (GetEditor() == 0)
                UpdateHexView();
            if (GetEditor() == 1)
                UpdateFileEditor();
            if (GetEditor() == 2)
                UpdateTextView();
        }

        private void UpdateFileEditor()
        {
            if (!ArchiveFileInfo.IsSupportedFileFormat())
            {
                AddControl(new STUserControl());
                return;
            }

            var File = ArchiveFileInfo.FileFormat;
            if (File == null) //If the file is not open yet, try temporarily for a preview
                File = ArchiveFileInfo.OpenFile();

            SetEditorForm(File);

            //If the format isn't active we can just dispose it
            if (ArchiveFileInfo.FileFormat == null)
                File.Unload();
        }

        private bool CheckActiveType(Type type)
        {
            return stPanel1.Controls.Count > 0 && stPanel1.Controls[0].GetType() != type;
        }

        public void SetEditorForm(IFileFormat fileFormat)
        {
            if (fileFormat == null)
                AddControl(new STUserControl() { Dock = DockStyle.Fill });

            if (fileFormat is TreeNodeFile)
            {
                var Editor = ((TreeNodeFile)fileFormat).GetEditor();
                var ActiveEditor = GetActiveEditor(Editor.GetType());
                if (ActiveEditor == null)
                    AddControl(Editor);
                else
                    Editor = ActiveEditor;

                ((TreeNodeFile)fileFormat).FillEditor(Editor);

            }

            Type objectType = fileFormat.GetType();
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                    var Editor = (UserControl)method.Invoke(fileFormat, new object[0]);
                    var ActiveEditor = GetActiveEditor(Editor.GetType());
                    if (ActiveEditor == null)
                        AddControl(Editor);
                    else
                        Editor = ActiveEditor;
                }
            }
        }

        private void UpdateTextView()
        {
            TextEditor editor = (TextEditor)GetActiveEditor(typeof(TextEditor));
            if (editor == null)
            {
                editor = new TextEditor();
                editor.Dock = DockStyle.Fill;
                AddControl(editor);
            }
            editor.Text = Text;

            var File = ArchiveFileInfo.FileFormat;
            if (File == null)
                File = ArchiveFileInfo.OpenFile();

            if (File != null && IsConvertableText(File.GetType()))
            {
                editor.FillEditor(((IConvertableTextFormat)File).ConvertToString());

                if (((IConvertableTextFormat)File).TextFileType == TextFileType.Yaml)
                    editor.IsYAML = true;
            }
            else
                editor.FillEditor(ArchiveFileInfo.FileData);
        }

        private bool IsConvertableText(Type type)
        {
            foreach (var inter in type.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IConvertableTextFormat))
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateHexView()
        {
            HexEditor editor = (HexEditor)GetActiveEditor(typeof(HexEditor));
            if (editor == null)
            {
                editor = new HexEditor();
                editor.Dock = DockStyle.Fill;
                AddControl(editor);
            }
            editor.Text = Text;
            editor.LoadData(ArchiveFileInfo.FileData);
        }


        public void AddControl(Control control)
        {
            foreach (var child in stPanel1.Controls)
                if (child is STUserControl)
                    ((STUserControl)child).OnControlClosing();

            stPanel1.Controls.Clear();
            stPanel1.Controls.Add(control);
        }

        private void stComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_IsLoaded)
                UpdateEditor();
            
        }
    }
}
