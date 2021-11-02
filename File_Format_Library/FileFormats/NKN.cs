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

namespace FirstPlugin
{
    public class NKN : IEditor<TextEditor>, IFileFormat, IConvertableTextFormat
    {
        private readonly string AES_KEY = "au3x5kBAnBbxqsqB";
        private readonly string AES_IV = "L8bdU63qcwpNYvR7";

        public FileType FileType { get; set; } = FileType.Parameter;

        public bool CanSave { get; set; } = true;
        public string[] Description { get; set; } = new string[] { "NKN Csv" };
        public string[] Extension { get; set; } = new string[] { "*.nkn" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream) {
            return FileName.EndsWith(".nkn");
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        private string DecryptedContents;

        public TextEditor OpenForm()
        {
            var textEditor = new TextEditor();
            return textEditor;
        }

        public void FillEditor(UserControl control)
        {
            ((TextEditor)control).FileFormat = this;
            ((TextEditor)control).FillEditor(DecryptedContents);
            ((TextEditor)control).TextEditorChanged += delegate
            {
                DecryptedContents = ((TextEditor)control).GetText();
            };
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Normal;
        public bool CanConvertBack => true;

        public string ConvertToString() {
            return DecryptedContents;
        }

        public void ConvertFromString(string text) {
            DecryptedContents = text;
        }

        #endregion

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new BinaryReader(stream)) {
                byte[] encodedContents = reader.ReadBytes((int)reader.BaseStream.Length);

                AesEncryption.SetKey(AES_KEY);
                AesEncryption.SetIV(AES_IV);
                DecryptedContents = AesEncryption.AesDecrypt(encodedContents);
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new BinaryWriter(stream)) {
                writer.Write(AesEncryption.AesEncrypt(DecryptedContents));
            }
        }

        public void Unload()
        {
        }
    }
}
