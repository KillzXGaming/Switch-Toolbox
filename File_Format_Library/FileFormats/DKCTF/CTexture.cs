using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace DKCTF
{
    public class CTexture : STGenericTexture, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TXTR" };
        public string[] Extension { get; set; } = new string[] { "*.txtr" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                bool IsForm = reader.CheckSignature(4, "RFRM");
                bool FormType = reader.CheckSignature(4, "TXTR", 20);

                return IsForm && FormType;
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

        public override bool CanEdit { get; set; } = false;

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

        TXTR Header;

        public override void OnClick(TreeView treeView)
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

            editor.LoadProperties(GenericProperties);
            editor.LoadImage(this);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            Items.AddRange(base.GetContextMenuItems());
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public void FillEditor(UserControl control)
        {
            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)Header.BufferData.Length;
            prop.Format = Format;

            ((ImageEditorBase)control).LoadImage(this);
            ((ImageEditorBase)control).LoadProperties(prop);
        }

        public void Load(System.IO.Stream stream)
        {
            PlatformSwizzle = PlatformSwizzle.Platform_Switch;
            CanSave = true;
            CanReplace = true;
            Text = FileName;
            this.ImageKey = "texture";
            this.SelectedImageKey = "texture";

            Header = new TXTR(stream);

            if (!Header.IsSwitch)
                PlatformSwizzle = PlatformSwizzle.Platform_WiiU;

            Width = Header.TextureHeader.Width;
            Height = Header.TextureHeader.Height;
            MipCount = (uint)Header.MipSizes.Length;
            Format = FormatList[Header.TextureHeader.Format];

            if (Header.TextureHeader.Type == 3)
                this.ArrayCount = 6;
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream, true))
            {
            }
        }

        public override void Replace(string FileName)
        {
     
        }


        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {
         
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            if (Header.IsSwitch)
                return TegraX1Swizzle.GetImageData(this, Header.BufferData, ArrayLevel, MipLevel, DepthLevel);
            else
            {
                var tex = this.Header.TextureHeader;

                uint bpp = GetBytesPerPixel(Format);

                GX2.GX2Surface surf = new GX2.GX2Surface();
                surf.bpp = bpp;
                surf.height = tex.Height;
                surf.width = tex.Width;
                surf.aa = (uint)GX2.GX2AAMode.GX2_AA_MODE_1X;
                surf.alignment = 8192;
                surf.imageSize = (uint)Header.BufferData.Length;

                if (Header.Meta != null)
                    surf.alignment = Header.Meta.BaseAlignment;
                surf.depth = 1;
                surf.dim = (uint)GX2.GX2SurfaceDimension.DIM_2D;
                surf.format = (uint)Bfres.Structs.FTEX.ConvertToGx2Format(Format);
                surf.use = (uint)GX2.GX2SurfaceUse.USE_TEXTURE;
                surf.data = Header.BufferData;
                surf.numMips = 1;
                surf.mipOffset = new uint[0];
                surf.mipData = Header.BufferData;
                surf.tileMode = (int)GX2.GX2TileMode.MODE_2D_TILED_THIN1;

                var surfOut = GX2.getSurfaceInfo((GX2.GX2SurfaceFormat)Format, surf.width, surf.height, 1, 1, surf.tileMode, 0, 0);
                surf.pitch = surfOut.pitch;

                var swizzlePattern = 0xd0000 | tex.Swizzle << 8;

                surf.swizzle = swizzlePattern;

                Console.WriteLine("");
                Console.WriteLine("// ----- GX2Surface Info ----- ");
                Console.WriteLine("  dim             = " + surf.dim);
                Console.WriteLine("  width           = " + surf.width);
                Console.WriteLine("  height          = " + surf.height);
                Console.WriteLine("  depth           = " + surf.depth);
                Console.WriteLine("  numMips         = " + surf.numMips);
                Console.WriteLine("  format          = " + surf.format);
                Console.WriteLine("  aa              = " + surf.aa);
                Console.WriteLine("  use             = " + surf.use);
                Console.WriteLine("  imageSize       = " + surf.imageSize);
                Console.WriteLine("  mipSize         = " + surf.mipSize);
                Console.WriteLine("  tileMode        = " + surf.tileMode);
                Console.WriteLine("  swizzle         = " + surf.swizzle);
                Console.WriteLine("  alignment       = " + surf.alignment);
                Console.WriteLine("  pitch           = " + surf.pitch);
                Console.WriteLine("  bits per pixel  = " + (surf.bpp << 3));
                Console.WriteLine("  bytes per pixel = " + surf.bpp);
                Console.WriteLine("  data size       = " + surf.data.Length);
                Console.WriteLine("  mip size        = " + surf.mipData.Length);
                Console.WriteLine("  realSize        = " + surf.imageSize);


                return GX2.Decode(surf, ArrayLevel, MipLevel);
            }
        }

        Dictionary<uint, TEX_FORMAT> FormatList = new Dictionary<uint, TEX_FORMAT>()
        {
            {  12, TEX_FORMAT.R8G8B8A8_UNORM },
            {  13, TEX_FORMAT.R8G8B8A8_UNORM_SRGB },

            {  20, TEX_FORMAT.BC1_UNORM },
            {  21, TEX_FORMAT.BC1_UNORM_SRGB },
            {  22, TEX_FORMAT.BC2_UNORM },
            {  23, TEX_FORMAT.BC2_UNORM_SRGB },
            {  24, TEX_FORMAT.BC3_UNORM },
            {  25, TEX_FORMAT.BC3_UNORM_SRGB },
            {  26, TEX_FORMAT.BC4_UNORM },
            {  27, TEX_FORMAT.BC4_SNORM },
            {  28, TEX_FORMAT.BC5_UNORM },
            {  29, TEX_FORMAT.BC5_SNORM },
            {  30, TEX_FORMAT.R11G11B10_FLOAT },
            {  31, TEX_FORMAT.R32_FLOAT },
            {  32, TEX_FORMAT.R16G16_FLOAT },
            {  33, TEX_FORMAT.R8G8_UNORM },

            {  53, TEX_FORMAT.ASTC_4x4_UNORM },
            {  54, TEX_FORMAT.ASTC_5x4_UNORM },
            {  55, TEX_FORMAT.ASTC_5x5_UNORM },
            {  56, TEX_FORMAT.ASTC_6x5_UNORM },
            {  57, TEX_FORMAT.ASTC_6x6_UNORM },
            {  58, TEX_FORMAT.ASTC_8x5_UNORM },
            {  59, TEX_FORMAT.ASTC_8x6_UNORM },
            {  60, TEX_FORMAT.ASTC_8x8_UNORM },
            {  61, TEX_FORMAT.ASTC_10x5_UNORM },
            {  62, TEX_FORMAT.ASTC_10x6_UNORM},
            {  63, TEX_FORMAT.ASTC_10x8_UNORM},
            {  64, TEX_FORMAT.ASTC_10x10_UNORM},
            {  65, TEX_FORMAT.ASTC_12x10_UNORM},
            {  66, TEX_FORMAT.ASTC_12x12_UNORM},

            {  67, TEX_FORMAT.ASTC_4x4_SRGB},
            {  68, TEX_FORMAT.ASTC_5x4_SRGB },
            {  69, TEX_FORMAT.ASTC_5x5_SRGB },
            {  70, TEX_FORMAT.ASTC_6x5_SRGB },
            {  71, TEX_FORMAT.ASTC_6x6_SRGB },
            {  72, TEX_FORMAT.ASTC_8x5_SRGB },
            {  73, TEX_FORMAT.ASTC_8x6_SRGB },
            {  74, TEX_FORMAT.ASTC_8x8_SRGB},
            {  75, TEX_FORMAT.ASTC_10x5_SRGB },
            {  76, TEX_FORMAT.ASTC_10x6_SRGB},
            {  77, TEX_FORMAT.ASTC_10x8_SRGB},
            {  78, TEX_FORMAT.ASTC_10x10_SRGB},
            {  79, TEX_FORMAT.ASTC_12x10_SRGB},
            {  80, TEX_FORMAT.ASTC_12x12_SRGB},

            {  81, TEX_FORMAT.BC6H_UF16 },
            {  82, TEX_FORMAT.BC6H_SF16 },
            {  83, TEX_FORMAT.BC7_UNORM },
            {  84, TEX_FORMAT.BC7_UNORM_SRGB },
        };
    }
}
