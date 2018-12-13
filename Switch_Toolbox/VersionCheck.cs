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
        public string ProgramVersion;
        public string CompileDate;
        public string CommitInfo;

        public VersionCheck()
        {
            string path = System.IO.Path.Combine(MainForm.executableDir, "Version.txt");
            using (StreamReader reader = new StreamReader(path))
            {
                ProgramVersion = reader.ReadLine();
                CompileDate = reader.ReadLine();
                CommitInfo = reader.ReadLine();
            }
        }
        public void SaveVersionInfo()
        {
            string path = MainForm.executableDir + "Version.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine($"{ProgramVersion}");
                writer.WriteLine($"{CompileDate}");
                writer.WriteLine($"{CommitInfo}");
            }
        }
    }
}
