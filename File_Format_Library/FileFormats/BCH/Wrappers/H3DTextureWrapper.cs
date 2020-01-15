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
using SPICA.Formats.CtrH3D.Texture;

namespace FirstPlugin.CtrLibrary
{
    public class H3DTextureWrapper : STGenericTexture
    {
        internal BCH BchParent;
        internal H3DTexture Texture;

        public H3DTextureWrapper()
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }
        public H3DTextureWrapper(H3DTexture texture, BCH bch) : base()
        {
            BchParent = bch;
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

        public override void OnClick(TreeView treeview)
        {
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
            prop.ImageSize = (uint)Texture.RawBuffer.Length;
            prop.Format = Format;

            editor.Text = Text;
            editor.LoadProperties(prop);
            editor.LoadImage(this);
        }

        public void LoadTexture(H3DTexture texture)
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            Texture = texture;

            Text = texture.Name;

            Width = (uint)texture.Width;
            Height = (uint)texture.Height;
            MipCount = texture.MipmapSize;
            Format = CTR_3DS.ConvertPICAToGenericFormat(
                (CTR_3DS.PICASurfaceFormat)texture.Format);

            if (texture.IsCubeTexture)
            {

            }
        }

        public override bool CanEdit { get; set; } = false;

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            PlatformSwizzle = PlatformSwizzle.Platform_3DS;

            return Texture.GetMipLevel(MipLevel, Texture.RawBuffer);
        }
    }
}
