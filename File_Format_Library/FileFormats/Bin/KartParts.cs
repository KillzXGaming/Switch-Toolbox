using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Syroot.NintenTools.MarioKart8.BinData;
using Syroot.NintenTools.MarioKart8;
using System.IO;


namespace FirstPlugin.Turbo
{
    public class PartsBIN : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Parameter;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Mario Kart 8 Kart Parts" };
        public string[] Extension { get; set; } = new string[] { "*.bin" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "PRTS");
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

        public class TIRE
        {

        }

        public class KART
        {

        }

        public class GLIDER
        {

        }

        BinFile binFile;
        public void Load(System.IO.Stream stream)
        {
            binFile = new BinFile(stream);

            for (int sectionIndex = 0; sectionIndex < binFile.Sections.Count; sectionIndex++)
            {

            }


            // Dump the BIN file to CSV.
            using (FileStream streamF = new FileStream("test.csv", FileMode.Create, FileAccess.Write, FileShare.None))
            using (StreamWriter writer = new StreamWriter(streamF, Encoding.UTF8, 1024, true))
            {
                // Write file header information.
                Write(writer, 0, $"BIN File Report for", FileName);
                Write(writer, 0, "Identifier", binFile.Format);
                Write(writer, 0, "Section count", binFile.Sections.Count);
                Write(writer, 0, "Unknown", binFile.ID);
                writer.WriteLine();

                int sectionIndex = 1;
                foreach (var section in GetDwordSectionData(binFile))
                {
                    Write(writer, 0, $"Section {sectionIndex++}");
                    Write(writer, 1, "Identifier", section.Name);
                    Write(writer, 1, "Group count", section.ParamCount);
                    Write(writer, 1, "Type", section.ID.ToString("X"));
                    writer.WriteLine();

                    DwordSectionData sectionData = (DwordSectionData)section.Data;

                    for (int d = 0; d < sectionData.Data.Length; d++)
                    {
                        foreach (Dword[] element in sectionData.Data[d])
                        {
                            Write(writer, 1, String.Join('\t'.ToString(), element));
                        }
                    }
                }
            }
        }

        private IEnumerable<Section> GetDwordSectionData(BinFile binFile)
        {
            foreach (Section section in binFile.Sections)
            {
                if (section.Data is DwordSectionData)
                    yield return section;
            }
        }


        private static void Write(StreamWriter writer, int indent, params object[] values)
        {
            writer.WriteLine(new string('\t', indent) + String.Join('\t'.ToString(), values));
        }


        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
