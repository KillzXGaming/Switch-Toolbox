using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin.PunchOutWii
{
    public class PO_Texture : STGenericTexture, ISingleTextureIconLoader
    {
        public byte[] ImageData;

        public STGenericTexture IconTexture => this;

        public Dictionary<byte, Decode_Gamecube.TextureFormats> FormatList = new Dictionary<byte, Decode_Gamecube.TextureFormats>
        {
            { 0x8, Decode_Gamecube.TextureFormats.RGBA32 },
            { 0x7, Decode_Gamecube.TextureFormats.RGB565 },
            { 0x6, Decode_Gamecube.TextureFormats.CMPR },
            { 0x5, Decode_Gamecube.TextureFormats.RGB5A3 },
            { 0x4, Decode_Gamecube.TextureFormats.IA4 },
            { 0x3, Decode_Gamecube.TextureFormats.I8 },
            { 0x2, Decode_Gamecube.TextureFormats.I4 },
        };

        public override bool CanEdit { get; set; } = false;

        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {

        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return Decode_Gamecube.GetMipLevel(ImageData, Width, Height, MipCount, (uint)MipLevel, Format);
        }

        public override void OnClick(TreeView treeView)
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }

            editor.LoadProperties(GenericProperties);
            editor.LoadImage(this);
        }

        public uint HashID { get; set; }

        public uint DataOffset { get; set; }

        public uint ImageSize { get; set; }

        public void Read(FileReader reader)
        {
            long pos = reader.Position;

            HashID = reader.ReadUInt32();
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            reader.ReadUInt16(); //0
            reader.ReadByte(); //unk
            reader.ReadByte(); //unk
            reader.ReadByte(); //unk
            byte format = reader.ReadByte();
            reader.ReadUInt16(); //unk
            reader.ReadUInt16(); //unk
            reader.ReadUInt16(); //unk

            DataOffset = reader.ReadUInt32();
            reader.ReadUInt32(); //unk 0x00412BA1

            var formatGC = Decode_Gamecube.TextureFormats.CMPR;
            if (FormatList.ContainsKey(format))
                formatGC = FormatList[format];

            Format = Decode_Gamecube.ToGenericFormat(formatGC);
            PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;

            ImageSize = (uint)Decode_Gamecube.GetDataSize(formatGC, (int)Width, (int)Height);

            reader.SeekBegin(pos + 96);
        }

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.I4,
                    TEX_FORMAT.I8,
                    TEX_FORMAT.IA4,
                    TEX_FORMAT.IA8,
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
    }
}
