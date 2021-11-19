using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Toolbox.Library.Security.Cryptography;

namespace MetroidDreadLibrary
{
    public class BSMAT : IEditor<TextEditor>, IFileFormat, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Parameter;

        public bool CanSave { get; set; } = true;
        public string[] Description { get; set; } = new string[] { "BSMAT" };
        public string[] Extension { get; set; } = new string[] { "*.bsmat" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream) {
            return FileName.EndsWith(".bsmat");
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        private string JsonContents;

        public TextEditor OpenForm()
        {
            var textEditor = new TextEditor();
            FillEditor(textEditor);
            return textEditor;
        }

        public void FillEditor(UserControl control)
        {
            ((TextEditor)control).FileFormat = this;
            ((TextEditor)control).FillEditor(JsonContents);
            ((TextEditor)control).TextEditorChanged = null;

            ((TextEditor)control).TextEditorChanged += delegate
            {
                JsonContents = ((TextEditor)control).GetText();
            };
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Json;
        public bool CanConvertBack => true;

        public string ConvertToString() {
            return JsonContents;
        }

        public void ConvertFromString(string text) {
            JsonContents = text;
        }

        #endregion

        public void Load(System.IO.Stream stream)
        {
            var bsmat = new BSMATMaterialFile(stream);
            JsonContents = bsmat.ToJson();
        }

        public void Save(System.IO.Stream stream)
        {
            var bsmat = BSMATMaterialFile.Import(JsonContents);
            bsmat.Save(stream);
        }

        public void Unload()
        {
        }
    }
}
