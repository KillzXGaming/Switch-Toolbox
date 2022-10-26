using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library;

namespace DKCTF
{
    internal class PakFileList
    {
        static Dictionary<string, string> _filePaths = new Dictionary<string, string>();

        public static Dictionary<string, string> GuiToFilePath
        {
            get
            {
                if (_filePaths.Count == 0)
                    Load();
                return _filePaths;
            }
        }

        static void Load()
        {
            string path = Path.Combine(Runtime.ExecutableDir, "Lib", "PakFileIDs", "PakContents.txt");
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine(); //headers
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var items = line.Split('\t');
                    if (items.Length != 4)
                        continue;

                    var id = items[2].Trim();
                    if (!_filePaths.ContainsKey(id))
                        _filePaths.Add(id, items[3].Trim());
                }
            }
        }
    }
}
