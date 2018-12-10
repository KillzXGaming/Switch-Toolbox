using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;

namespace Switch_Toolbox
{
    public class VersionCheck
    {
        public void ReadVersionInfo()
        {
            string path = MainForm.executableDir + "Version.txt";
            using (StreamReader reader = new StreamReader(path))
            {
                string Version = reader.ReadLine();
            }
        }
        public void WriteVersionInfo()
        {
            string path = MainForm.executableDir + "Version.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write($"Version: {Runtime.ProgramVersion}");
                writer.Write($"Commit: {Runtime.CommitInfo}");
                writer.Write($"Build Date: {Runtime.CompileDate}");
            }
        }
    }
}
