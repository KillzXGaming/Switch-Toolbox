using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class TPL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TPL" };
        public string[] Extension { get; set; } = new string[] { "*.tpl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                return reader.ReadUInt32() == 0x0020AF30;
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

        libWiiSharp.TPL tplFile = null;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            tplFile = libWiiSharp.TPL.Load(stream);
            for (int i = 0; i < tplFile.NumOfTextures; i++)
                Nodes.Add(new TPL_Texture(tplFile, i));
        }


        public void Unload()
        {

        }

        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            return mem.ToArray();
        }

        public class TPL_Texture : STGenericTexture
        {
            private libWiiSharp.TPL ParentTPL;
            private int TextureIndex { get; set; }


            private TextureProperties properties;
            public class TextureProperties 
            {
                private libWiiSharp.TPL ParentTPL;
                private int TextureIndex { get; set; }

                public libWiiSharp.TPL_TextureFormat TextureFormat
                {
                    get { return ParentTPL.GetTextureFormat(TextureIndex); }
                }

                public libWiiSharp.TPL_PaletteFormat PaletteFormat
                {
                    get { return ParentTPL.GetPaletteFormat(TextureIndex); }
                }

                public int Width
                {
                    get { return ParentTPL.GetImageWidth(TextureIndex); }
                }

                public int Height
                {
                    get { return ParentTPL.GetImageHeight(TextureIndex); }
                }

                public TextureProperties(libWiiSharp.TPL tpl, int index)
                {
                    ParentTPL = tpl;
                    TextureIndex = index;
                }
            }



            public TPL_Texture(libWiiSharp.TPL tpl, int index)
            {
                ParentTPL = tpl;
                TextureIndex = index;

                Width = tpl.GetImageWidth(index);
                Height = tpl.GetImageHeight(index);
                MipCount = 1;

                Text = $"Image {index}";

                Format = TEX_FORMAT.R8G8B8A8_UNORM;

                SelectedImageKey = "texture";
                ImageKey = "texture";

                properties = new TextureProperties(tpl, index);
            }

            public override bool CanEdit { get; set; } = false;

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                         TEX_FORMAT.R8G8B8A8_UNORM,
                };
                }
            }

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return ImageUtilty.ConvertBgraToRgba(ParentTPL.ExtractTextureByteArray(TextureIndex));
            }

            public override void OnClick(TreeView treeView)
            {
                UpdateEditor();
            }

            public void UpdateEditor()
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;

                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.LoadProperties(properties);
                editor.LoadImage(this);
            }
        }
    }
}
