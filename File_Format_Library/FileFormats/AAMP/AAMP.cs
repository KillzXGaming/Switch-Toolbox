using System;
using System.Collections.Generic;
using System.IO;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using aampv1 = AampV1Library;
using aampv2 = AampV2Library;
using FirstPlugin.Forms;

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
        public bool CanConvertBack => false;

        public string ConvertToString()
        {
            if (aampFileV1 != null)
                return AampYamlConverter.ToYaml(aampFileV1);
            else
                return AampYamlConverter.ToYaml(aampFileV2);
        }

        public void ConvertFromString(string text)
        {
            if (aampFileV1 != null)
                AampYamlConverter.ToAamp(aampFileV1, text);
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
                if (((AAMP)File).aampFileV1 != null)
                {
                    foreach (var val in ((AAMP)File).aampFileV1.RootNode.childParams)
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

                    foreach (var param in ((AAMP)File).aampFileV1.RootNode.paramObjects)
                    {
                        switch (param.HashString)
                        {
                            case "root_grid":
                                GenerateGridData((BFRES)Bfres, param.paramEntries);
                                break;
                        }
                    }
                }
                else
                {
                    foreach (var val in ((AAMP)File).aampFileV2.RootNode.childParams)
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

                    foreach (var param in ((AAMP)File).aampFileV2.RootNode.paramObjects)
                    {
                        switch (param.HashString)
                        {
                            case "root_grid":
                                GenerateGridData((BFRES)Bfres, param.paramEntries);
                                break;
                        }
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

        private static void GenerateGridData(BFRES bfres, aampv2.ParamEntry[] paramEntries)
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

        private static void GenerateGridData(BFRES bfres, aampv1.ParamEntry[] paramEntries)
        {
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
            if (aampFileV1 != null)
            {
                AampV1Editor editor = new AampV1Editor(this, IsSaveDialog);
                editor.Text = FileName;
                editor.Dock = DockStyle.Fill;
                return editor;
            }
            else
            {
                AampV2Editor editor = new AampV2Editor(this, IsSaveDialog);
                editor.Text = FileName;
                editor.Dock = DockStyle.Fill;
                return editor;
            }
        }

        public void FillEditor(UserControl control)
        {
        }


        public aampv1.AampFile aampFileV1;
        public aampv2.AampFile aampFileV2;

        public void Load(Stream stream)
        {
            CanSave = true;

            IsSaveDialog = IFileInfo != null && IFileInfo.InArchive;

            uint Version = CheckVersion(stream);

            if (Version == 1)
            {
                aampFileV1 = new aampv1.AampFile(stream);
            }
            else if (Version == 2)
            {
                aampFileV2 = new aampv2.AampFile(stream);
            }
            else
            {
                throw new Exception($"Unsupported AAMP version! {Version}");
            }
        }
            
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            if (aampFileV1 != null)
                aampFileV1.Save(stream);
            else
                aampFileV2.Save(stream);
        }
    }
}
