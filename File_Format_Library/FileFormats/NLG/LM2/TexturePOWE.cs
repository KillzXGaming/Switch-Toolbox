using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace FirstPlugin.LuigisMansion.DarkMoon
{
    public class TexturePOWE : STGenericTexture
    {
        public static readonly uint Identifier = 0xE977D350;

        public uint Index { get; set; }

        public uint ID { get; set; }
        public uint ImageSize { get; set; }
        public uint ID2 { get; set; }

        public byte[] ImageData { get; set; }

        private POWEProperties properties;

        public class POWEProperties
        {
            [Browsable(false)]
            public uint ID { get; set; }

            public string HashID
            {
                get
                {
                    return ID.ToString("x");
                }
            }

            [ReadOnly(true)]
            public uint Width { get; set; }
            [ReadOnly(true)]
            public uint Height { get; set; }
            [ReadOnly(true)]
            public byte NumMips { get; set; }
            [ReadOnly(true)]
            public TEX_FORMAT Format { get; set; }

        }

        public void Read(FileReader reader)
        {
            //Magic and ID not pointed to for sub entries so just skip them for now
            //     uint magic = reader.ReadUInt32();
            //   if (magic != Identifier)
            //         throw new Exception($"Invalid texture header magic! Expected {Identifier.ToString("x")}. Got {Identifier.ToString("x")}");
            //     ID = reader.ReadUInt32();

            PlatformSwizzle = PlatformSwizzle.Platform_3DS;

            ImageSize = reader.ReadUInt32();
            ID2 = reader.ReadUInt32();
            reader.Seek(0x8);
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            reader.Seek(3);
            var numMips = reader.ReadByte();
            reader.Seek(0x14);
            byte FormatCtr = reader.ReadByte();
            reader.Seek(3);

            MipCount = 1;
            Format = CTR_3DS.ConvertPICAToGenericFormat((CTR_3DS.PICASurfaceFormat)FormatCtr);

            Parameters = new ImageParameters();
            Parameters.FlipY = true;

            properties = new POWEProperties();
            properties.ID = ID2;
            properties.Width = Width;
            properties.Height = Height;
            properties.NumMips = numMips;
            properties.Format = Format;
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
            editor.LoadProperties(properties);
            editor.LoadImage(this);
        }

        public override bool CanEdit { get; set; } = false;

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return ImageData;
        }

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
    }
}
