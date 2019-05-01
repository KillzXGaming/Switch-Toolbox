using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Syroot.BinaryData;

namespace FirstPlugin
{
    public class TPFileSizeTable
    {
        public Dictionary<string, uint> FileSizes = new Dictionary<string, uint>();

        public void Read(FileReader reader)
        {
            while (reader.Position < reader.BaseStream.Length)
            {
                string FileName = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                string Size = reader.ReadString(BinaryStringFormat.ZeroTerminated);

                uint sizeNum = 0;
                uint.TryParse(Size, out sizeNum);
                FileSizes.Add(FileName, sizeNum);

                Console.WriteLine(FileName + " " + Size);
            }
        }

        public void Write(FileWriter writer)
        {
            foreach (var file in FileSizes)
            {
                writer.Write(file.Key, BinaryStringFormat.ZeroTerminated);
                writer.Write(file.Value.ToString(), BinaryStringFormat.ZeroTerminated);
            }
        }
    }
}
