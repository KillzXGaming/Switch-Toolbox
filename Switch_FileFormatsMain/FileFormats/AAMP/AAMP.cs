using System;
using System.Collections.Generic;
using System.IO;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using System.Windows.Forms;
using aampv1 = AampV1Library;
using aampv2 = AampV2Library;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class AAMP : IEditor<AampEditorBase>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Parameter;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "AAMP" };
        public string[] Extension { get; set; } = new string[] { "*.aamp" };
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

        bool IsSaveDialog = false;

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
        public byte[] Save()
        {
            var mem = new MemoryStream();

            if (aampFileV1 != null)
                aampFileV1.Save(mem);
            else
                aampFileV2.Save(mem);

            return mem.ToArray();
        }
    }
}
