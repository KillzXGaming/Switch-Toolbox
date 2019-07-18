using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
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
            saveBtn.Visible = false;
        }

        public void LoadFile(ArchiveFileInfo archiveFileInfo)
        {
            ArchiveFileInfo = archiveFileInfo;
        }

        private void ReloadEditors()
        {
            stComboBox1.Items.Clear();
            stComboBox1.Items.Add("Properties");
            stComboBox1.Items.Add("Hex Editor");
            stComboBox1.Items.Add("File Editor");
            stComboBox1.Items.Add("Text Editor");

            if (Runtime.ObjectEditor.EditorDiplayIndex < stComboBox1.Items.Count)
                stComboBox1.SelectedIndex = Runtime.ObjectEditor.EditorDiplayIndex;
            else
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
                UpdatePropertiesView();
            else if (GetEditor() == 1)
                UpdateHexView();
            else if (GetEditor() == 2)
                UpdateFileEditor();
            else if (GetEditor() == 3)
                UpdateTextView();

            if (GetEditor() == 2 || GetEditor() == 3)
                saveBtn.Visible = true;
            else
                saveBtn.Visible = false;
        }

        private void UpdateFileEditor()
        {
            var File = ArchiveFileInfo.FileFormat;
            if (File == null) //If the file is not open yet, try temporarily for a preview
                File = ArchiveFileInfo.OpenFile();

            //If the file is still null, just add a basic control and return
            if (File == null)
            {
                AddControl(new STUserControl());
                return;
            }

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
                    System.Reflection.MethodInfo methodFill = objectType.GetMethod("FillEditor");

                    var Editor = (UserControl)method.Invoke(fileFormat, new object[0]);
                    var ActiveEditor = GetActiveEditor(Editor.GetType());
                    if (ActiveEditor == null)
                        AddControl(Editor);
                    else
                        Editor = ActiveEditor;

                    methodFill.Invoke(fileFormat, new object[1] { Editor });
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

        private void NotifyFormatSwitched()
        {

        }

        private void SaveTextFormat()
        {

        }

        private bool IsConvertableText(Type type)
        {
            return typeof(IConvertableTextFormat).IsAssignableFrom(type);
        }

        private void UpdatePropertiesView()
        {
            STPropertyGrid editor = (STPropertyGrid)GetActiveEditor(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                editor.Dock = DockStyle.Fill;
                AddControl(editor);
            }
            editor.Text = Text;
            editor.LoadProperty(ArchiveFileInfo.DisplayProperties);
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
            byte[] Data = ArchiveFileInfo.FileData;

            //Only load a certain about of bytes to prevent memory dispose issues
            editor.LoadData(Utils.SubArray(Data, 0, 3000));
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
            if (_IsLoaded && stComboBox1.SelectedIndex != -1)
            {
                Runtime.ObjectEditor.EditorDiplayIndex = stComboBox1.SelectedIndex;
                UpdateEditor();
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            bool IsTextEditor = GetEditor() == 3;

            var File = ArchiveFileInfo.FileFormat;
            if (IsTextEditor && File != null && IsConvertableText(File.GetType()) && ((IConvertableTextFormat)File).CanConvertBack)
            {
                TextEditor editor = (TextEditor)GetActiveEditor(typeof(TextEditor));
                ((IConvertableTextFormat)File).ConvertFromString(editor.GetText());

                ArchiveFileInfo.SaveFileFormat();
                MessageBox.Show($"Saved {File.FileName} to archive!");
            }
            else if (File != null && File.CanSave)
            {
                ArchiveFileInfo.SaveFileFormat();
                MessageBox.Show($"Saved {File.FileName} to archive!");
            }
            else
                MessageBox.Show($"File format does not support saving!");
        }
    }
}
