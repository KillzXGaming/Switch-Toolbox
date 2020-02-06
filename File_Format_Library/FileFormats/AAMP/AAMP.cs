using System;
using System.Collections.Generic;
using System.IO;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using FirstPlugin.Forms;
using AampLibraryCSharp;

namespace FirstPlugin
{
    public class AAMP : IEditor<AampEditorBase>, IFileFormat, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Parameter;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "AAMP" };
        public string[] Extension { get; set; } = new string[] { "*.aamp", "*.bglpbd" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "AAMP");
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

        private uint CheckVersion(Stream stream)
        {
            using (FileReader reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "AAMP");
                reader.Position = 4;

                return reader.ReadUInt32();
            }
        }


        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Yaml;
        public bool CanConvertBack => true;

        public string ConvertToString()
        {
            return YamlConverter.ToYaml(aampFile);
        }

        public void ConvertFromString(string text)
        {
            aampFile = YamlConverter.FromYaml(text);
        }

        #endregion

        bool IsSaveDialog = false;

        public static void GenerateProbeBoundings()
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = Utils.GetAllFilters(typeof(AAMP));
            if (ofd1.ShowDialog() != DialogResult.OK)
                return;

            //Load the AAMP
            var File = STFileLoader.OpenFileFormat(ofd1.FileName);
            if (File == null || !(File is AAMP))
                throw new Exception("File not a valid AAMP file!");

            //Load bfres for generating the bounds
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Utils.GetAllFilters(typeof(BFRES));

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //Open and check bfres
                var Bfres = STFileLoader.OpenFileFormat(ofd.FileName);
                if (Bfres == null || !(Bfres is BFRES))
                    throw new Exception("File not a valid BFRES file!");

                //Check version instance
                foreach (var val in ((AAMP)File).aampFile.RootNode.childParams)
                {
                    foreach (var param in val.paramObjects)
                    {
                        switch (param.HashString)
                        {
                            case "grid":
                                GenerateGridData((BFRES)Bfres, param.paramEntries);
                                break;
                        }
                    }
                }

                foreach (var param in ((AAMP)File).aampFile.RootNode.paramObjects)
                {
                    switch (param.HashString)
                    {
                        case "root_grid":
                            GenerateGridData((BFRES)Bfres, param.paramEntries);
                            break;
                    }
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = Utils.GetAllFilters(File);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    //Save the aamp back
                    STFileSaver.SaveFileFormat(File, sfd.FileName);
                }

                File.Unload();
                Bfres.Unload();
            }
        }

        private static void GenerateGridData(BFRES bfres, ParamEntry[] paramEntries)
        {
            //Load the grid min nad max and set them
            var boundings = bfres.GetBoundingBox();

            foreach (var entry in paramEntries)
            {
                if (entry.HashString == "aabb_min_pos")
                    entry.Value = new Syroot.Maths.Vector3F(boundings.Min.X, boundings.Min.Y, boundings.Min.Z);
                if (entry.HashString == "aabb_max_pos")
                    entry.Value = new Syroot.Maths.Vector3F(boundings.Max.X, boundings.Max.Y, boundings.Max.Z);
            }
        }

        public AampEditorBase OpenForm()
        {
            AampEditor editor = new AampEditor(this, IsSaveDialog);
            editor.Text = FileName;
            editor.Dock = DockStyle.Fill;
            return editor;
        }

        public void FillEditor(UserControl control)
        {
        }


        public AampFile aampFile;

        public void Load(Stream stream)
        {
            CanSave = true;

            IsSaveDialog = IFileInfo != null && IFileInfo.InArchive;

            aampFile = AampFile.LoadFile(stream);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            if (aampFile != null)
                aampFile.Save(stream);
        }
    }
}
