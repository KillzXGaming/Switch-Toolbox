using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.IO;
using System.Diagnostics;

namespace FirstPlugin
{
    public class LUAC : IEditor<TextEditor>, IFileFormat, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Parameter;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "LUAC" };
        public string[] Extension { get; set; } = new string[] { "*.lua" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true)) {
                return reader.CheckSignature(3, "Lua", 1);
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

        public TextEditor OpenForm()
        {
            var textEditor = new TextEditor();
            return textEditor;
        }

        public void FillEditor(UserControl control)
        {
            ((TextEditor)control).FileFormat = this;
            ((TextEditor)control).FillEditor(ConvertToString());
        }

        public string Decompiled = "";

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Normal;
        public bool CanConvertBack => false;

        public string ConvertToString()
        {
            return Decompiled;
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        public void Load(System.IO.Stream stream)
        {
            int offset = 0x14;
            offset = 0;

            if (!Directory.Exists($"{Runtime.ExecutableDir}/LUA"))
                Directory.CreateDirectory($"{Runtime.ExecutableDir}/LUA");

            string unluac = $"{Runtime.ExecutableDir}/LUA/unluac.jar";
            string target = $"{Runtime.ExecutableDir}/LUA/{FileName}.luac";

            new SubStream(stream, offset).ExportToFile(target);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "java.exe";
            start.WorkingDirectory = $"{Runtime.ExecutableDir}/LUA";
            start.Arguments = $"-jar {unluac} {Utils.AddQuotesIfRequired(target)}";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    try
                    {
                        Decompiled = reader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        Toolbox.Library.Forms.STErrorDialog.Show("Failed to decompile shader!", "Shader Tools", ex.ToString());
                        Decompiled = "";
                    }
                }
            }
        }

        public void Unload()
        {
        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
