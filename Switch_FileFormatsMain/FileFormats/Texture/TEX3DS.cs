using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class TEX3DS : STGenericTexture, IEditor<ImageEditorBase>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TEX (3DS)" };
        public string[] Extension { get; set; } = new string[] { "*.tex" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return (Utils.HasExtension(FileName, ".tex"));
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public override bool CanEdit { get; set; } = false;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.B5G6R5_UNORM,
                    TEX_FORMAT.R8G8_UNORM,
                    TEX_FORMAT.B5G5R5A1_UNORM,
                    TEX_FORMAT.B4G4R4A4_UNORM,
                    TEX_FORMAT.LA8,
                    TEX_FORMAT.HIL08,
                    TEX_FORMAT.L8,
                    TEX_FORMAT.A8_UNORM,
                    TEX_FORMAT.LA4,
                    TEX_FORMAT.A4,
                    TEX_FORMAT.ETC1_UNORM,
                    TEX_FORMAT.ETC1_A4,
            };
            }
        }

        public byte[] ImageData;

        public ImageEditorBase OpenForm()
        {
            bool IsDialog = IFileInfo != null && IFileInfo.InArchive;

            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)ImageData.Length;
            prop.Format = Format;

            ImageEditorBase form = new ImageEditorBase();
            form.Text = Text;
            form.Dock = DockStyle.Fill;
       //    form.editorBase.AddFileContextEvent("Save", Save);
        //    form.editorBase.AddFileContextEvent("Replace", Replace);
            form.LoadProperties(prop);
            form.LoadImage(this);

            return form;
        }

        public void FillEditor(UserControl control)
        {
            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)ImageData.Length;
            prop.Format = Format;

            ((ImageEditorBase)control).LoadImage(this);
            ((ImageEditorBase)control).LoadProperties(prop);
        }

        public void Load(System.IO.Stream stream)
        {
            PlatformSwizzle = PlatformSwizzle.Platform_3DS;
            CanSave = true;

            using (var reader = new FileReader(stream))
            {
                Width = reader.ReadUInt32();
                Height = reader.ReadUInt32();
                byte FormatCtr = reader.ReadByte();
                MipCount = reader.ReadByte();
                ushort padding = reader.ReadUInt16();
                Text = reader.ReadZeroTerminatedString();
                Format = CTR_3DS.ConvertPICAToGenericFormat((CTR_3DS.PICASurfaceFormat)FormatCtr);

                reader.Position = 0x80;
                ImageData = reader.ReadBytes((int)reader.BaseStream.Length - 0x80);
            }
        }

        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            using (var writer = new FileWriter(mem))
            {
                writer.Write(Width);
                writer.Write(Height);
                writer.Write((byte)CTR_3DS.ConvertToPICAFormat(Format));
                writer.Write((byte)MipCount);
                writer.Write((ushort)0); //Padding
                writer.WriteString(Text);

                writer.Position = 0x80;
                writer.Write(ImageData);
            }


            return mem.ToArray();
        }

        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {
         
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            return ImageData;
        }
    }
}
