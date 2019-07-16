using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using BcresLibrary;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class TXOBWrapper : STGenericTexture
    {
        internal BCRES BcresParent;
        internal Texture Texture;

        public TXOBWrapper()
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }
        public TXOBWrapper(Texture texture, BCRES bcres) : base()
        {
            BcresParent = bcres;
            LoadTexture(texture);
        }


        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                      
                };
            }
        }

        public override void OnClick(TreeView treeview) {
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }

            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)Texture.Images[0].ImageData.Length;
            prop.Format = Format;

            editor.Text = Text;
            editor.LoadProperties(prop);
            editor.LoadImage(this);
        }

        public void LoadTexture(Texture texture)
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            Texture = texture;

            Text = texture.Name;

            //Cube maps will use multiple images
            //Break at the end as we only need the first part for generic things
            foreach (var image in texture.Images)
            {
                Width = image.Width;
                Height = image.Height;
                MipCount = image.MipCount;
                Format = CTR_3DS.ConvertPICAToGenericFormat(
                    (CTR_3DS.PICASurfaceFormat)image.ImageFormat);

                break;
            }
        }

        public override bool CanEdit { get; set; } = false;

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            PlatformSwizzle = PlatformSwizzle.Platform_3DS;

            uint Offset = 0;

            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                uint _width = Math.Max(1, Width >> mipLevel);
                uint _height = Math.Max(1, Height >> mipLevel);

                uint Increment = Texture.Images[ArrayLevel].BitsPerPixel / 8;
                if (Increment == 0) Increment = 1;

                uint size = (_width * _height) * Increment;

                if (mipLevel == MipLevel)
                    return Utils.SubArray(Texture.Images[ArrayLevel].ImageData, Offset, size);

                Offset += size;
            }

            return Texture.Images[ArrayLevel].ImageData;
        }
    }
}
