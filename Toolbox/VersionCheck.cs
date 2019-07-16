using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace Toolbox
{
    public class VersionCheck
    {
        public string ProgramVersion = "";
        public string CompileDate = "";
        public string CommitInfo = "";

        public VersionCheck(bool HasVersionTxt)
        {
            string path = System.IO.Path.Combine(Runtime.ExecutableDir, "Version.txt");
            if (!File.Exists(path))
            {
                HasVersionTxt = false;
                return;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                ProgramVersion = reader.ReadLine();
                CompileDate = reader.ReadLine();
                CommitInfo = reader.ReadLine();
            }
        }
        public void SaveVersionInfo()
        {
            string path = Runtime.ExecutableDir + "Version.txt";
            if (!File.Exists(path))
                File.Create(path);

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine($"{ProgramVersion}");
                writer.WriteLine($"{CompileDate}");
                writer.WriteLine($"{CommitInfo}");
            }
        }
    }
}
