using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using SuperBMDLib;

namespace FirstPlugin
{
    public class BMDTextureWrapper : STGenericTexture
    {
        public override bool CanEdit { get; set; } = false;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {

                };
            }
        }

        SuperBMDLib.Materials.BinaryTextureImage TextureImage;

        public BMDTextureWrapper(SuperBMDLib.Materials.BinaryTextureImage Image)
        {
            TextureImage = Image;

            Text = TextureImage.Name;

            Format = Decode_Gamecube.ToGenericFormat((Decode_Gamecube.TextureFormats)Image.Format);
            PaletteFormat = Decode_Gamecube.ToGenericPaletteFormat((Decode_Gamecube.PaletteFormats)Image.PaletteFormat);
            PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;

            Width = TextureImage.Width;
            Height = TextureImage.Height;
            MipCount = TextureImage.MipMap;

            ImageKey = "texture";
            SelectedImageKey = ImageKey;
        }

        public override void OnClick(TreeView treeview)
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }

            editor.Text = Text;
            editor.LoadProperties(TextureImage);
            editor.LoadImage(this); 
        }

        public override byte[] GetPaletteData()
        {
            return TextureImage.PaletteData;
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return TextureImage.ImageData;
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            TextureImage.EncodeData();
        }
    }
}
