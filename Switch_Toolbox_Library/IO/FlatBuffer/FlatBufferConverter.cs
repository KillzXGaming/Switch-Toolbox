using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Toolbox.Library.IO
{
    //Ported from https://github.com/kwsch/pkNX/blob/58b0597feaf53b35f5c4424e5ec357f82d99c9fa/pkNX.WinForms/Dumping/FlatBufferConverter.cs
    //While i could use flat buffer c# generated code, it's a giant mess code wise
    public static class FlatBufferConverter
    {
        static FlatBufferConverter()
        {
            if (!Directory.Exists(FlatPath))
                return;
            var files = Directory.GetFiles(FlatPath);
            foreach (var f in files)
                File.Delete(f);
        }

        public static string DeserializeToJson(Stream stream, string fbs)
        {
            var path = Path.GetTempFileName();
            stream.ExportToFile(path);
            return DeserializeToJson(path, fbs);
        }

        public static string DeserializeToJson(string path, string fbs)
        {
            GenerateJsonFromFile(path, fbs);
            var text = ReadDeleteJsonFromFolder();
            File.Delete(path);
            return text;
        }

        public static T[] DeserializeFrom<T>(string[] files, string fbs)
        {
            var result = new T[files.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var file = files[i];
                result[i] = DeserializeFrom<T>(file, fbs);
            }
            return result;
        }

        public static T DeserializeFrom<T>(Stream stream, string fbs)
        {
            var path = Path.GetTempFileName();
            stream.ExportToFile(path);
            var ret = DeserializeFrom<T>(path, fbs);
            File.Delete(path);
            return ret;
        }

        public static T DeserializeFrom<T>(string file, string fbs)
        {
            GenerateJsonFromFile(file, fbs);
            var text = ReadDeleteJsonFromFolder();
            var obj = JsonConvert.DeserializeObject<T>(text);
            Debug.Assert(obj != null);
            Debug.WriteLine($"Deserialized {Path.GetFileName(file)}");
            return obj;
        }

        public static byte[][] SerializeFrom<T>(T[] obj, string fbs)
        {
            var result = new byte[obj.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                var file = obj[i];
                result[i] = SerializeFrom(file, fbs);
            }
            return result;
        }

        public static byte[] SerializeFrom<T>(T obj, string fbs)
        {
            string log = GenerateBinFrom(obj, fbs);
            var fileName = fbs + ".bin";
            var data = ReadDelete(fileName);
            Debug.Assert(data.Length != 0);
            Debug.WriteLine($"Serialized to {fileName}");
            return data;
        }


        private static string ReadDeleteJsonFromFolder()
        {
            var jsonPath = Directory.GetFiles(FlatPath, "*.json")[0];
            var text = File.ReadAllText(jsonPath);
            File.Delete(jsonPath);
            return text;
        }

        private static byte[] ReadDelete(string fileName)
        {
            var filePath = Path.Combine(FlatPath, fileName);

            if (!System.IO.File.Exists(filePath))
                throw new Exception($"Failed to save binary. {log}");

            var data = File.ReadAllBytes(filePath);
            File.Delete(filePath);
            return data;
        }

        private static void GenerateJsonFromFile(string file, string fbs)
        {
            var fbsName = fbs + ".fbs";
            var fbsPath = Path.Combine(FlatPath, fbsName);
            Directory.CreateDirectory(FlatPath);
            if (!File.Exists(fbsPath))
                File.WriteAllBytes(fbsPath, GetSchema(fbs));

            var fileName = Path.GetFileName(file);
            var filePath = Path.Combine(FlatPath, fileName);
            File.Copy(file, filePath, true);

            var args = GetArgumentsDeserialize(fileName, fbsName);
            RunFlatC(args);
            File.Delete(filePath);
        }

        private static string GenerateBinFrom<T>(T obj, string fbs)
        {
            if (!Directory.Exists(FlatPath))
                Directory.CreateDirectory(FlatPath);

            var fbsName = fbs + ".fbs";
            var jsonName = fbs + ".json";
            var text = WriteJson(obj);

            var fbsPath = Path.Combine(FlatPath, fbsName);
            if (!File.Exists(fbsPath))
                File.WriteAllBytes(fbsPath, GetSchema(fbs));

            var jsonPath = Path.Combine(FlatPath, jsonName);
            File.WriteAllText(jsonPath, text);
            var args = GetArgumentsSerialize(jsonName, fbsName);
            return RunFlatC(args);
        }

        public static string WriteJson<T>(T obj)
        {
            var serializer = new JsonSerializer();
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new JsonTextWriter(stringWriter)
                {
                    QuoteName = false,
                    Formatting = Formatting.Indented,
                    IndentChar = ' ',
                    Indentation = 2
                })
                    serializer.Serialize(writer, obj);
                return stringWriter.ToString();
            }
        }

        private static string log = "";

        private static string RunFlatC(string args)
        {
            var fcp = Path.Combine(FlatPath, "flatc.exe");
            if (!File.Exists(fcp))
                File.WriteAllBytes(fcp, Properties.Resources.flatc);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = FlatPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = $"/C flatc {args} & exit",
                }
            };
            process.EnableRaisingEvents = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_ErrorDataReceived);
            process.StartInfo.UseShellExecute = false;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            return log;
        }

        private static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            if (e.Data != null) log = e.Data;
        }

        private static void process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            if (e.Data != null) log = e.Data;
        }

        public static byte[] GetSchema(string name)
        {
            var obj = Properties.Resources.ResourceManager.GetObject(name);
            if (!(obj is byte[])) {
                throw new FileNotFoundException(nameof(name)); }
            return (byte[])obj;
        }

        public static readonly string WorkingDirectory = Application.StartupPath;
        public static readonly string FlatPath = Path.Combine(WorkingDirectory, "flatbuffers");

        private static string GetArgumentsDeserialize(string file, string fbs) =>
         $"-t {fbs} -- {file} --defaults-json --raw-binary";

        private static string GetArgumentsSerialize(string file, string fbs) =>
            $"-b {fbs} {file} --defaults-json";
    }
}
