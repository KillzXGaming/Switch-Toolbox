using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;

namespace AmbrosiaPikmin1.FileFormats.BTI
{
    class BTI : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }

        public string[] Description { get; set; } = new string[] { "Binary Texture Image" };
        public string[] Extension { get; set; } = new string[] { "*.bti" };
        public string FileName { get; set; }
        public string FilePath { get; set; }

        //Stores compression info from being opened (done automaitcally)
        public IFileInfo IFileInfo { get; set; }

        //Check how the file wil be opened
        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".bti");
        }

        //A Type list for custom types
        //With this you can add in classes with IFileMenuExtension to add menus for this format
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        private int RoundWidth(int width, int BlockWidth)
        {
            return width + ((BlockWidth - (width % BlockWidth)) % BlockWidth);
        }
        private int RoundHeight(int height, int BlockHeight)
        {
            return height + ((BlockHeight - (height % BlockHeight)) % BlockHeight);
        }

        public void Load(System.IO.Stream stream)
        {
            //Set this if you want to save the file format
            CanSave = true;

            //You can add a FileReader with Toolbox.Library.IO namespace
            using (var reader = new FileReader(stream))
            {
                Texture tex = new Texture();
                tex.CanEdit = false;

                reader.SetByteOrder(true);

                //Turn this format into a common format used by this tool
                byte texFormat = reader.ReadByte();
                tex.Format = Decode_Gamecube.ToGenericFormat((Decode_Gamecube.TextureFormats)texFormat);

                _ = reader.ReadByte(); // enable alpha
                tex.Width = reader.ReadUInt16();
                tex.Height = reader.ReadUInt16();
                _ = reader.ReadByte(); // wrap s
                _ = reader.ReadByte(); // wrap t
                tex.PaletteFormat = (PALETTE_FORMAT)reader.ReadInt16();
                _ = reader.ReadInt16(); // num of palette entries
                _ = reader.ReadInt32(); // offset to palette data
                _ = reader.ReadInt32(); // border colour
                _ = reader.ReadByte(); // min filter type
                _ = reader.ReadByte(); // mag filter type
                _ = reader.ReadInt16();

                tex.MipCount = reader.ReadByte();
                _ = reader.ReadByte();
                _ = reader.ReadInt16();
                uint offsetToImageData = reader.ReadUInt32(); // offset to image data

                //Lets set our method of decoding
                tex.PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;

                reader.Seek(offsetToImageData, System.IO.SeekOrigin.Begin);
                int imageDataSize = RoundWidth((int)tex.Width, (int)STGenericTexture.GetBlockWidth(tex.Format)) * RoundHeight((int)tex.Height, (int)STGenericTexture.GetBlockHeight(tex.Format))
                                    * (int)STGenericTexture.GetBytesPerPixel(tex.Format) >> 3;

                tex.ImageData = reader.ReadBytes(imageDataSize);

                tex.Name = FileName;
                tex.ToolTipText = "Binary Texture Image, used for 2D textures like fonts";
                _ = Nodes.Add(tex);
            }
        }

        public byte[] Save()
        {
            return null;
        }

        public void Unload()
        {

        }

        public class Texture : STGenericTexture
        {
            public byte[] ImageData { get; set; }

            //A list of supported formats
            //This gets used in the re encode option
            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                    TEX_FORMAT.I4,
                    TEX_FORMAT.I8,
                    TEX_FORMAT.I4,
                    TEX_FORMAT.I8,
                    TEX_FORMAT.RGB565,
                    TEX_FORMAT.RGB5A3,
                    TEX_FORMAT.RGBA32,
                    TEX_FORMAT.C4,
                    TEX_FORMAT.C8,
                    TEX_FORMAT.C14X2,
                    TEX_FORMAT.CMPR,
                    };
                }
            }

            public override bool CanEdit { get; set; } = false;

            //This gets used in the image editor if the image gets edited
            //This wll not be ran if "CanEdit" is set to false!
            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            //Gets the raw image data in bytes
            //Gets decoded automatically
            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return ImageData;
            }


            //This is an event for when the tree is clicked on
            //Load our editor
            public override void OnClick(TreeView treeView)
            {
                //Here we check for an active editor and load a new one if not found
                //This is used when a tree/object editor is used
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                //Load our image and any properties
                //If you don't make a class for properties you can use a generic class provided in STGenericTexture
                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }
        }
    }
}
