using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using System.Drawing;
using Paloma;

namespace Switch_Toolbox.Library
{
    public class TGA : STGenericTexture, IEditor<ImageEditorBase>, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
                    TEX_FORMAT.R8G8B8A8_UNORM,
                };
            }
        }

        const string MagicFileConstant = "TRUEVISION";

        public override bool CanEdit { get; set; } = false;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TGA" };
        public string[] Extension { get; set; } = new string[] { "*.tga" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.Position = reader.BaseStream.Length - 18;
                bool IsValidMagic = reader.ReadString(10) == MagicFileConstant;
                return IsValidMagic || Utils.GetExtension(FileName) == ".tga";
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

        public ImageEditorBase OpenForm()
        {
            bool IsDialog = IFileInfo != null && IFileInfo.InArchive;

            ImageEditorBase form = new ImageEditorBase();
            form.Text = Text;
            form.Dock = DockStyle.Fill;
            form.LoadImage(this);
            form.LoadProperties(GenericProperties);
            return form;
        }

        public void FillEditor(UserControl control)
        {
            ((ImageEditorBase)control).LoadImage(this);
            ((ImageEditorBase)control).LoadProperties(GenericProperties);
        }

        private TargaImage TargaImage;

        public void Load(System.IO.Stream stream) {
            TargaImage = new TargaImage(stream);
            Width = (uint)TargaImage.Header.Width;
            Height = (uint)TargaImage.Header.Height;
        }

        public void Unload()
        {
            TargaImage.Dispose();
        }

        public byte[] Save()
        {
            return null;
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException("Cannot set image data! Operation not implemented!");
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            if (TargaImage == null || TargaImage.Image == null) return new byte[0];

            return  BitmapExtension.ImageToByte(BitmapExtension.SwapBlueRedChannels(TargaImage.Image));
        }
    }
}
