using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Toolbox.Library;

namespace FirstPlugin
{
    public class NSWShaderDecompile
    {
        public enum NswShaderType
        {
            Vertex,
            Geometry,
            Fragment,
            Compute
        }

        public static string DecompileShader(NswShaderType shaderType, byte[] Data, ulong Address = 0)
        {
            if (!Directory.Exists("temp"))
                Directory.CreateDirectory("temp");

            if (!Directory.Exists("ShaderTools"))
                Directory.CreateDirectory("ShaderTools");

            //     File.WriteAllBytes("temp/shader1.bin", Utils.CombineByteArray(data.ToArray()));
            File.WriteAllBytes("temp/shader1.bin", Data);

            if (!File.Exists($"{Runtime.ExecutableDir}/ShaderTools/Ryujinx.ShaderTools.exe"))
            {
                MessageBox.Show("No shader decompiler found in ShaderTools. If you want to decompile a shader, you can use Ryujinx's ShaderTools.exe and put in the ShaderTools folder of the toolbox.");
                return "";
            }

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "ShaderTools/Ryujinx.ShaderTools.exe";
            start.WorkingDirectory = Runtime.ExecutableDir;
            start.Arguments = $"{Utils.AddQuotesIfRequired("temp/shader1.bin")}";
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
                        return reader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        Toolbox.Library.Forms.STErrorDialog.Show("Failed to decompile shader!", "Shader Tools", ex.ToString());
                        return "";
                    }
                }
            }
        }
    }
}
