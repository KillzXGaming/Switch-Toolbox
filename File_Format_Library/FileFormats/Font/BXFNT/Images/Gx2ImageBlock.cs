using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Toolbox.Library;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class Gx2ImageBlock : STGenericTexture
    {
        public TGLP TextureTGLP;

        public int SheetIndex = 0;

        public void Load(TGLP texture, int Index)
        {
            CanReplace = true;

            SheetIndex = Index;
            TextureTGLP = texture;
            Height = TextureTGLP.SheetHeight;
            Width = TextureTGLP.SheetWidth;
            var BFNTFormat = (Gx2ImageFormats)TextureTGLP.Format;
            Format = ConvertToGeneric(BFNTFormat);
            if (Format == TEX_FORMAT.BC4_UNORM)
            {
                RedChannel = STChannelType.One;
                GreenChannel = STChannelType.One; 
                BlueChannel = STChannelType.One;
                AlphaChannel = STChannelType.Red;
            }

            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }


        public enum Gx2ImageFormats
        {
            RGBA8_UNORM,
            RGB8_UNORM,
            RGB5A1_UNORM,
            RGB565_UNORM,
            RGBA4_UNORM,
            LA8_UNORM,
            LA4_UNORM,
            A4_UNORM,
            A8_UNORM,
            BC1_UNORM,
            BC2_UNORM,
            BC3_UNORM,
            BC4_UNORM,
            BC5_UNORM,
            RGBA8_SRGB,
            BC1_SRGB,
            BC2_SRGB,
            BC3_SRGB,
        }

        public override bool CanEdit { get; set; } = true;
        public override string ExportFilter => FileFilters.GTX;
        public override string ReplaceFilter => FileFilters.GTX;

        public override void Replace(string FileName)
        {
            Bfres.Structs.FTEX ftex = new Bfres.Structs.FTEX();
            ftex.ReplaceTexture(FileName, Format, 1, SwizzlePattern, SupportedFormats, true, true, false);
            if (ftex.texture != null)
            {
                TextureTGLP.Format = (ushort)ConvertToGx2(ftex.Format);
                TextureTGLP.SheetHeight = (ushort)ftex.texture.Height;
                TextureTGLP.SheetWidth = (ushort)ftex.texture.Width;
                TextureTGLP.SheetDataList[SheetIndex] = ftex.texture.Data;
                Format = ftex.Format;
                Width = ftex.texture.Width;
                Height = ftex.texture.Height;

                UpdateEditor();
            }
        }

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.BC1_UNORM_SRGB,
                        TEX_FORMAT.BC1_UNORM,
                        TEX_FORMAT.BC2_UNORM,
                        TEX_FORMAT.BC2_UNORM_SRGB,
                        TEX_FORMAT.BC3_UNORM,
                        TEX_FORMAT.BC3_UNORM_SRGB,
                        TEX_FORMAT.BC4_UNORM,
                        TEX_FORMAT.BC5_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                };
            }
        }

        public TEX_FORMAT ConvertToGeneric(Gx2ImageFormats Format)
        {
            switch (Format)
            {
                case Gx2ImageFormats.A8_UNORM: return TEX_FORMAT.R8_UNORM;
                case Gx2ImageFormats.BC1_SRGB: return TEX_FORMAT.BC1_UNORM_SRGB;
                case Gx2ImageFormats.BC1_UNORM: return TEX_FORMAT.BC1_UNORM;
                case Gx2ImageFormats.BC2_UNORM: return TEX_FORMAT.BC2_UNORM;
                case Gx2ImageFormats.BC2_SRGB: return TEX_FORMAT.BC2_UNORM_SRGB;
                case Gx2ImageFormats.BC3_UNORM: return TEX_FORMAT.BC3_UNORM;
                case Gx2ImageFormats.BC3_SRGB: return TEX_FORMAT.BC3_UNORM_SRGB;
                case Gx2ImageFormats.BC4_UNORM: return TEX_FORMAT.BC4_UNORM;
                case Gx2ImageFormats.BC5_UNORM: return TEX_FORMAT.BC5_UNORM;
                case Gx2ImageFormats.LA4_UNORM: return TEX_FORMAT.R4G4_UNORM;
                case Gx2ImageFormats.LA8_UNORM: return TEX_FORMAT.R8G8_UNORM;
                case Gx2ImageFormats.RGB565_UNORM: return TEX_FORMAT.B5G6R5_UNORM;
                case Gx2ImageFormats.RGB5A1_UNORM: return TEX_FORMAT.B5G5R5A1_UNORM;
                case Gx2ImageFormats.RGB8_UNORM: return TEX_FORMAT.R8G8_UNORM;
                case Gx2ImageFormats.RGBA8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                case Gx2ImageFormats.RGBA8_UNORM: return TEX_FORMAT.R8G8B8A8_UNORM;
                default:
                    throw new NotImplementedException("Unsupported format " + Format);
            }
        }

        public Gx2ImageFormats ConvertToGx2(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.R8_UNORM: return Gx2ImageFormats.A8_UNORM;
                case TEX_FORMAT.BC1_UNORM_SRGB: return Gx2ImageFormats.BC1_SRGB;
                case TEX_FORMAT.BC1_UNORM: return Gx2ImageFormats.BC1_UNORM;
                case TEX_FORMAT.BC2_UNORM_SRGB: return Gx2ImageFormats.BC2_SRGB;
                case TEX_FORMAT.BC2_UNORM: return Gx2ImageFormats.BC2_UNORM;
                case TEX_FORMAT.BC3_UNORM_SRGB: return Gx2ImageFormats.BC3_SRGB;
                case TEX_FORMAT.BC3_UNORM: return Gx2ImageFormats.BC3_UNORM;
                case TEX_FORMAT.BC4_UNORM: return Gx2ImageFormats.BC4_UNORM;
                case TEX_FORMAT.BC5_UNORM: return Gx2ImageFormats.BC5_UNORM;
                case TEX_FORMAT.R4G4_UNORM: return Gx2ImageFormats.LA4_UNORM;
                case TEX_FORMAT.R8G8_UNORM: return Gx2ImageFormats.RGB8_UNORM;
                case TEX_FORMAT.B5G6R5_UNORM: return Gx2ImageFormats.RGB565_UNORM;
                case TEX_FORMAT.B5G5R5A1_UNORM: return Gx2ImageFormats.RGB5A1_UNORM;
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return Gx2ImageFormats.RGBA8_SRGB;
                case TEX_FORMAT.R8G8B8A8_UNORM: return Gx2ImageFormats.RGBA8_UNORM;
                default:
                    throw new NotImplementedException("Unsupported format " + Format);
            }
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            if (bitmap == null)
                return; //Image is likely disposed and not needed to be applied

            uint Gx2Format = (uint)Bfres.Structs.FTEX.ConvertToGx2Format(Format);
            Width = (uint)bitmap.Width;
            Height = (uint)bitmap.Height;

            MipCount = 1;
            uint[] MipOffsets = new uint[MipCount];

            try
            {
                //Create image block from bitmap first
                var data = GenerateMipsAndCompress(bitmap, MipCount, Format);

                //Swizzle and create surface
                var surface = GX2.CreateGx2Texture(data, Text,
                 (uint)4,
                 (uint)0,
                 (uint)Width,
                 (uint)Height,
                 (uint)1,
                 (uint)Gx2Format,
                 (uint)SwizzlePattern,
                 (uint)1,
                 (uint)MipCount
                 );

                TextureTGLP.Format = (ushort)ConvertToGx2(Format);
                TextureTGLP.SheetHeight = (ushort)surface.height;
                TextureTGLP.SheetWidth = (ushort)surface.width;
                TextureTGLP.SheetDataList[SheetIndex] = surface.data;

                IsEdited = true;
                UpdateEditor();
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to swizzle and compress image " + Text, "Error", ex.ToString());
            }
        }

        private const uint SwizzleBase = 0x00000000;

        private uint swizzle;
        private uint Swizzle
        {
            get
            {
                swizzle = SwizzleBase;
                swizzle |= (uint)(SheetIndex * 2) << 8;
                return swizzle;
            }
        }

        private uint SwizzlePattern
        {
            get
            {
                return (uint)(SheetIndex * 2);
            }
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            uint bpp = GetBytesPerPixel(Format);

            GX2.GX2Surface surf = new GX2.GX2Surface();
            surf.bpp = bpp;
            surf.height = Height;
            surf.width = Width;
            surf.aa = (uint)GX2.GX2AAMode.GX2_AA_MODE_1X;
            surf.alignment = 0;
            surf.depth = 1;
            surf.dim = (uint)GX2.GX2SurfaceDimension.DIM_2D;
            surf.format = (uint)Bfres.Structs.FTEX.ConvertToGx2Format(Format);
            surf.use = (uint)GX2.GX2SurfaceUse.USE_COLOR_BUFFER;
            surf.pitch = 0;
            surf.data = TextureTGLP.SheetDataList[SheetIndex];
            surf.numMips = 1;
            surf.mipOffset = new uint[0];
            surf.mipData = null;
            surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;
            surf.swizzle = Swizzle;
            surf.numArray = 1;

            return GX2.Decode(surf, ArrayLevel, MipLevel);
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
            prop.ImageSize = (uint)TextureTGLP.SheetDataList[SheetIndex].Length;
            prop.Format = Format;
            prop.Swizzle = Swizzle;


            editor.Text = Text;
            editor.LoadProperties(prop);
            editor.LoadImage(this);
        }
    }

}
