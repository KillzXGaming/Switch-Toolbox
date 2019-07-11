using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
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

            Format = TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
            Width = TextureImage.Width;
            Height = TextureImage.Height;
            MipCount = TextureImage.MipMapCount;

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

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            return ImageUtilty.ConvertBgraToRgba(TextureImage.RGBAImageData);
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            TextureImage.EncodeData();
        }
    }
}
