using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.GX2;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Switch_Toolbox.Library.IO;
using FirstPlugin;

namespace Bfres.Structs
{
    public class FTEX : STGenericTexture
    {
        public const uint Tex2SwizzleValue = 65536;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                        TEX_FORMAT.BC1_UNORM,
                        TEX_FORMAT.BC1_UNORM_SRGB,
                        TEX_FORMAT.BC2_UNORM,
                        TEX_FORMAT.BC2_UNORM_SRGB,
                        TEX_FORMAT.BC3_UNORM,
                        TEX_FORMAT.BC3_UNORM_SRGB,
                        TEX_FORMAT.BC4_UNORM,
                        TEX_FORMAT.BC4_SNORM,
                        TEX_FORMAT.BC5_UNORM,
                        TEX_FORMAT.BC5_SNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.B8G8R8A8_UNORM_SRGB,
                        TEX_FORMAT.B8G8R8A8_UNORM,
                        TEX_FORMAT.R10G10B10A2_UNORM,
                        TEX_FORMAT.R16_UNORM,
                        TEX_FORMAT.B4G4R4A4_UNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                        TEX_FORMAT.R32G8X24_FLOAT,
                };
            }
        }

        bool IsReplaced = false;

        public override bool CanEdit { get; set; } = true;

        public int format;
        public Texture texture;

        public FTEX()
        {
            CanReplace = true;
            CanDelete = true;
            CanEdit = true;
            CanExport = true;
        }
        public FTEX(Texture tex) { Read(tex); }
        public FTEX(string name) { Text = name; }

        //For determining mip map file for botw (Tex2)
        public string GetFilePath()
        {
            if (Parent == null)
                return "";

            return ((BFRES)Parent.Parent).FilePath;
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FTEX));
        public override string ImportFilter => FileFilters.GetFilter(typeof(FTEX));

        public override void Rename()
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //Update dictionary key
                ((BFRESGroupNode)Parent).ResourceNodes.Remove(Text);
                Text = dialog.textBox1.Text;
                ((BFRESGroupNode)Parent).ResourceNodes.Add(Text, this);
            }
        }
        public override void Delete()
        {
            ((BFRESGroupNode)Parent).RemoveChild(this);
            LibraryGUI.Instance.UpdateViewport(); //Update texture display
        }

        public override void Export(string FileName) => Export(FileName);

        public static GTXImporterSettings SetImporterSettings(Image image, string name)
        {
            var importer = new GTXImporterSettings();
            importer.LoadBitMap(image, name);
            return importer;
        }

        public static GTXImporterSettings SetImporterSettings(string name, TEX_FORMAT DefaultFormat = TEX_FORMAT.UNKNOWN)
        {
            var importer = new GTXImporterSettings();
            string ext = System.IO.Path.GetExtension(name);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".dds":
                case ".dds2":
                    importer.LoadDDS(name);
                    break;
                default:
                    importer.LoadBitMap(name);

                    //Override the format setting. This only will do this for images not as dds or astc
                    if (DefaultFormat != TEX_FORMAT.UNKNOWN)
                        importer.Format = (GX2.GX2SurfaceFormat)ConvertToGx2Format(DefaultFormat);
                    break;
            }

            return importer;
        }

        public override void Unload()
        {
            DisposeRenderable();
            Nodes.Clear();
        }

        public void ReplaceImage(Image image, string FileName)
        {
            GTXImporterSettings setting = SetImporterSettings(image, FileName);
            setting.MipSwizzle = Tex2Swizzle;

            if (Tex2Swizzle != 0)
                setting.Swizzle = Tex2Swizzle;

            GTXTextureImporter importer = new GTXTextureImporter();
            importer.LoadSetting(setting);

            if (importer.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                if (setting.GenerateMipmaps && !setting.IsFinishedCompressing)
                {
                    setting.DataBlockOutput.Clear();
                    setting.DataBlockOutput.Add(setting.GenerateMips());
                }
                    
                if (setting.DataBlockOutput != null)
                {
                    var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                    surface.compSel = new byte[4];
                    surface.compSel[0] = (byte)setting.RedComp;
                    surface.compSel[1] = (byte)setting.GreenComp;
                    surface.compSel[2] = (byte)setting.BlueComp;
                    surface.compSel[3] = (byte)setting.alphaRef;

                    var tex = FromGx2Surface(surface, setting.TexName);
                    UpdateTex(tex);

                    texture.Name = Text;
                    IsReplaced = true;
                    Read(texture);
                    LoadOpenGLTexture();
                }
                else
                {
                    MessageBox.Show("Something went wrong???");
                }
            }

            if (IsEditorActive() && IsReplaced)
                UpdateEditor();
        }

        public override void Replace(string FileName) {

            //If the mipcount is originally 1 it's probably important so set it as default
            if (texture != null && texture.MipCount == 1)
                ReplaceTexture(FileName, Format, texture.MipCount);
            else
                ReplaceTexture(FileName, Format);
        }

        public void ReplaceTexture(string FileName, TEX_FORMAT DefaultFormat = TEX_FORMAT.UNKNOWN, uint MipMapCount = 0, TEX_FORMAT[] SupportedFormats = null,
            bool IsSwizzleReadOnly = false, bool IsTileModeReadOnly = false, bool IsFormatReadOnly = false, uint SwizzlePattern = 0)
        {
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            if (ext == ".bftex")
            {
                texture.Import(FileName, ((BFRESGroupNode)Parent).GetResFileU());
                texture.Name = Text;
                IsReplaced = true;
                Read(texture);

                if (IsEditorActive())
                    UpdateEditor();
                return;
            }

            GTXImporterSettings setting = SetImporterSettings(FileName, DefaultFormat);
            setting.MipSwizzle = Tex2Swizzle;
            setting.SwizzlePattern = SwizzlePattern;

            GTXTextureImporter importer = new GTXTextureImporter();

            importer.ReadOnlySwizzle = IsSwizzleReadOnly;
            importer.ReadOnlyTileMode = IsSwizzleReadOnly;
            importer.ReadOnlyFormat = IsFormatReadOnly;
            
            if (Tex2Swizzle != 0)
                setting.Swizzle = Tex2Swizzle;

            if (MipMapCount != 0)
            {
                setting.MipCount = MipMapCount;
                importer.OverrideMipCounter = true;
            }
            if (SupportedFormats != null)
                importer.LoadSupportedFormats(SupportedFormats);

            importer.LoadSetting(setting);

            if (ext == ".dds")
            {
                if (setting.DataBlockOutput != null)
                {
                    var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                    var tex = FromGx2Surface(surface, setting.TexName);
                    UpdateTex(tex);
                    texture.Name = Text;
                    IsReplaced = true;
                    Read(texture);
                    LoadOpenGLTexture();
                }
            }
            else
            {
                if (importer.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (setting.GenerateMipmaps && !setting.IsFinishedCompressing)
                    {
                        setting.DataBlockOutput.Clear();
                        setting.DataBlockOutput.Add(setting.GenerateMips());
                    }

                    if (setting.DataBlockOutput != null)
                    {
                        var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                        var tex = FromGx2Surface(surface, setting.TexName);
                        UpdateTex(tex);
                        texture.Name = Text;
                        IsReplaced = true;
                        Read(texture);
                        LoadOpenGLTexture();
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong???");
                    }
                }
            }
            if (IsEditorActive() && IsReplaced)
                UpdateEditor();
        }

        public void UpdateTex(Texture tex)
        {
            if (texture == null)
                texture = new Texture();

            texture.Name = tex.Name;
            texture.Data = tex.Data;
            texture.MipData = tex.MipData;
            texture.MipOffsets = tex.MipOffsets;
            texture.Pitch = tex.Pitch;
            texture.Swizzle = tex.Swizzle;
            texture.Width = tex.Width;
            texture.Height = tex.Height;
            texture.AAMode = tex.AAMode;
            texture.Alignment = tex.Alignment;
            texture.ArrayLength = tex.ArrayLength;
            texture.CompSelR = tex.CompSelR;
            texture.CompSelG = tex.CompSelG;
            texture.CompSelB = tex.CompSelB;
            texture.CompSelA = tex.CompSelA;
            texture.Depth = tex.Depth;
            texture.Dim = tex.Dim;
            texture.Format = tex.Format;
            texture.Path = tex.Path;
            texture.TileMode = tex.TileMode;
            texture.Use = tex.Use;
            texture.ViewMipCount = tex.ViewMipCount;
            texture.ViewMipFirst = tex.ViewMipFirst;
            texture.ViewSliceCount = tex.ViewSliceCount;
            texture.ViewSliceFirst = tex.ViewSliceFirst;
            texture.Regs = tex.Regs;
            texture.MipCount = tex.MipCount;

        }

        //We reuse GX2 data as it's the same thing
        public static Texture FromGx2Surface(GX2.GX2Surface surf, string Name)
        {
            Texture tex = new Texture();
            tex.Name = Name;
            tex.Path = "";
            tex.AAMode = (GX2AAMode)surf.aa;
            tex.Alignment = (uint)surf.alignment;
            tex.ArrayLength = 1;
            tex.Data = surf.data;
            tex.MipData = surf.mipData;
            tex.Format = (GX2SurfaceFormat)surf.format;
            tex.Dim = (GX2SurfaceDim)surf.dim;
            tex.Use = (GX2SurfaceUse)surf.use;
            tex.TileMode = (GX2TileMode)surf.tileMode;
            tex.Swizzle = surf.swizzle;
            tex.Pitch = surf.pitch;
            tex.Depth = surf.depth;
            tex.MipCount = surf.numMips;
            tex.ViewMipCount = surf.numMips;
            tex.ViewSliceCount = 1;

            tex.MipOffsets = new uint[13];
            for (int i = 0; i < 13; i++)
            {
                if (i < surf.mipOffset.Length)
                    tex.MipOffsets[i] = surf.mipOffset[i];
            }
            tex.Height = surf.height;
            tex.Width = surf.width;
            tex.ArrayLength = 1;

            tex.Regs = new uint[5];

            if (surf.compSel.Length != 4)
                surf.compSel = new byte[] { 0, 1, 2, 3 };

            //Set channels for settings and format
            tex.CompSelR = (GX2CompSel)surf.compSel[0];
            tex.CompSelG = (GX2CompSel)surf.compSel[1];
            tex.CompSelB = (GX2CompSel)surf.compSel[2];
            tex.CompSelA = (GX2CompSel)surf.compSel[3];
            SetChannelsByFormat((GX2SurfaceFormat)surf.format, tex);

            tex.UserData = new ResDict<UserData>();
            return tex;
        }
        public uint Tex2Swizzle = 0;

        public void Read(Texture tex)
        {
            CanReplace = true;
            CanDelete = true;
            CanExport = true;

            ImageKey = "Texture";
            SelectedImageKey = "Texture";
            Text = tex.Name;

            texture = tex;

            Width = tex.Width;
            Height = tex.Height;
            Format = ConvertFromGx2Format(tex.Format);
            ArrayCount = tex.ArrayLength;
            MipCount = tex.MipCount;

            RedChannel = SetChannel(texture.CompSelR);
            GreenChannel = SetChannel(texture.CompSelG);
            BlueChannel = SetChannel(texture.CompSelB);
            AlphaChannel = SetChannel(texture.CompSelA);

            if (tex.ArrayLength == 0)
                ArrayCount = tex.Depth; //Some older bfres don't use array length????

            if (texture.MipData == null || texture.MipData.Length <= 0)
                MipCount = 1;
        }

        private STChannelType SetChannel(GX2CompSel comp)
        {
            if (comp == GX2CompSel.ChannelR) return STChannelType.Red;
            else if (comp == GX2CompSel.ChannelG) return STChannelType.Green;
            else if (comp == GX2CompSel.ChannelB) return STChannelType.Blue;
            else if (comp == GX2CompSel.ChannelA) return STChannelType.Alpha;
            else if (comp == GX2CompSel.Always0) return STChannelType.Zero;
            else return STChannelType.One;
        }

        public void UpdateMipMaps()
        {
            LoadTex2Bfres();
            LoadTex2MipMaps();

            if (texture.MipData != null && texture.MipData.Length > 0)
                MipCount = texture.MipCount;
        }

        //For tex2
        public static void GenerateMipmaps(uint MipCount, TEX_FORMAT Format, Bitmap bitmap, Texture texture)
        {
            if (bitmap == null)
                return; //Image is likely disposed and not needed to be applied

            texture.MipCount = MipCount;
            texture.MipOffsets = new uint[MipCount];

            try
            {
                //Create image block from bitmap first
                var data = GenerateMipsAndCompress(bitmap, MipCount, Format);

                bitmap.Dispose();

                //Swizzle and create surface
                var surface = GX2.CreateGx2Texture(data, texture.Name,
                    (uint)texture.TileMode,
                    (uint)texture.AAMode,
                    (uint)texture.Width,
                    (uint)texture.Height,
                    (uint)texture.Depth,
                    (uint)texture.Format,
                    (uint)texture.Swizzle,
                    (uint)texture.Dim,
                    (uint)texture.MipCount
                    );

                var tex = FromGx2Surface(surface, texture.Name);
                texture.MipData = tex.MipData;
                texture.MipOffsets = tex.MipOffsets;
                texture.MipCount = tex.MipCount;
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to swizzle and compress image " + texture.Name, "Error", ex.ToString());
            }
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            if (bitmap == null)
                return; //Image is likely disposed and not needed to be applied

            texture.Format = ConvertToGx2Format(Format);
            texture.Width = (uint)bitmap.Width;
            texture.Height = (uint)bitmap.Height;

            if (MipCount != 1)
            {
                MipCount = GenerateMipCount(bitmap.Width, bitmap.Height);
                if (MipCount == 0)
                    MipCount = 1;
            }

            texture.MipCount = MipCount;
            texture.MipOffsets = new uint[MipCount];

            try
            {
                //Create image block from bitmap first
                var data = GenerateMipsAndCompress(bitmap, MipCount, Format);

                bitmap.Dispose();

                //Swizzle and create surface
                var surface = GX2.CreateGx2Texture(data, Text,
                    (uint)texture.TileMode,
                    (uint)texture.AAMode,
                    (uint)texture.Width,
                    (uint)texture.Height,
                    (uint)texture.Depth,
                    (uint)texture.Format,
                    (uint)texture.Swizzle,
                    (uint)texture.Dim,
                    (uint)texture.MipCount
                    );

                var tex = FromGx2Surface(surface, texture.Name);
                UpdateTex(tex);

                IsEdited = true;
                Read(texture);
                LoadOpenGLTexture();
                LibraryGUI.Instance.UpdateViewport();
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to swizzle and compress image " + Text, "Error", ex.ToString());
            }
        }

        ResFile ResFileTexture2;

        private void LoadTex2Bfres()
        {
            if (ResFileTexture2 != null)
                return;

            //Determine tex2 botw files to get mip maps
            string Tex1 = GetFilePath();

            if (!IsReplaced && Tex1 != null && Tex1.Contains(".Tex1"))
            {
                string Tex2 = Tex1.Replace(".Tex1", ".Tex2");
                Console.WriteLine(Tex2 + " " + System.IO.File.Exists(Tex2) + " " + texture.Name);

                if (System.IO.File.Exists(Tex2))
                {
                    if (Tex2.EndsWith(".sbfres"))
                    {
                        ResFileTexture2 = new ResFile(new System.IO.MemoryStream(
                                        EveryFileExplorer.YAZ0.Decompress(Tex2)));
                    }
                    else
                    {
                        ResFileTexture2 = new ResFile(Tex2);
                    }
                }
            }
        }

        private void LoadTex2MipMaps()
        {
            if (ResFileTexture2 == null || IsReplaced)
                return;

            Console.WriteLine((ResFileTexture2.Textures.ContainsKey(texture.Name)));

            if (ResFileTexture2.Textures.ContainsKey(texture.Name))
            {
                texture.MipCount = ResFileTexture2.Textures[texture.Name].MipCount;
                texture.MipData = ResFileTexture2.Textures[texture.Name].MipData;
                texture.MipOffsets = ResFileTexture2.Textures[texture.Name].MipOffsets;
                Tex2Swizzle = ResFileTexture2.Textures[texture.Name].Swizzle;
            }
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            format = (int)texture.Format;
            int swizzle = (int)texture.Swizzle;
            int pitch = (int)texture.Pitch;
            uint bpp = GX2.surfaceGetBitsPerPixel((uint)format) >> 3;

            UpdateMipMaps();

            GX2.GX2Surface surf = new GX2.GX2Surface();
            surf.bpp = bpp;
            surf.height = texture.Height;
            surf.width = texture.Width;
            surf.aa = (uint)texture.AAMode;
            surf.alignment = texture.Alignment;
            surf.depth = texture.Depth;
            surf.dim = (uint)texture.Dim;
            surf.format = (uint)texture.Format;
            surf.use = (uint)texture.Use;
            surf.pitch = texture.Pitch;
            surf.data = texture.Data;
            surf.numMips = MipCount;
            surf.mipOffset = texture.MipOffsets;
            surf.mipData = texture.MipData;
            surf.tileMode = (uint)texture.TileMode;
            surf.swizzle = texture.Swizzle;
            surf.numArray = texture.ArrayLength;

            if (Tex2Swizzle != 0)
                surf.mip_swizzle = Tex2Swizzle;

            if (surf.mipData == null)
                surf.numMips = 1;

            var surfaces = GX2.Decode(surf);

            if (ArrayLevel >= surfaces.Count)
                throw new Exception("Invalid amount of surfaces decoded!");
            if (surfaces.Count == 0)
                throw new Exception("Surfaces came out empty!");

            return surfaces[ArrayLevel][MipLevel];
        }

        public static GX2SurfaceFormat ConvertToGx2Format(TEX_FORMAT texFormat)
        {
            switch (texFormat)
            {
                case TEX_FORMAT.BC1_UNORM: return GX2SurfaceFormat.T_BC1_UNorm;
                case TEX_FORMAT.BC1_UNORM_SRGB: return GX2SurfaceFormat.T_BC1_SRGB;
                case TEX_FORMAT.BC2_UNORM: return GX2SurfaceFormat.T_BC2_UNorm;
                case TEX_FORMAT.BC2_UNORM_SRGB: return GX2SurfaceFormat.T_BC2_SRGB;
                case TEX_FORMAT.BC3_UNORM: return GX2SurfaceFormat.T_BC3_UNorm;
                case TEX_FORMAT.BC3_UNORM_SRGB: return GX2SurfaceFormat.T_BC3_SRGB;
                case TEX_FORMAT.BC4_UNORM: return GX2SurfaceFormat.T_BC4_UNorm;
                case TEX_FORMAT.BC4_SNORM: return GX2SurfaceFormat.T_BC4_SNorm;
                case TEX_FORMAT.BC5_UNORM: return GX2SurfaceFormat.T_BC5_UNorm;
                case TEX_FORMAT.BC5_SNORM: return GX2SurfaceFormat.T_BC5_SNorm; 
                case TEX_FORMAT.B5G5R5A1_UNORM: return GX2SurfaceFormat.TC_R5_G5_B5_A1_UNorm; 
                case TEX_FORMAT.B4G4R4A4_UNORM: return GX2SurfaceFormat.TC_R4_G4_B4_A4_UNorm; 
                case TEX_FORMAT.B5G6R5_UNORM: return GX2SurfaceFormat.TCS_R5_G6_B5_UNorm;
                case TEX_FORMAT.R8G8B8A8_UNORM: return GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm;
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB;
                case TEX_FORMAT.R10G10B10A2_UNORM: return GX2SurfaceFormat.TCS_R10_G10_B10_A2_UNorm;
                case TEX_FORMAT.R11G11B10_FLOAT: return GX2SurfaceFormat.TC_R11_G11_B10_Float;
                case TEX_FORMAT.R16_UNORM: return GX2SurfaceFormat.TCD_R16_UNorm;
                case TEX_FORMAT.R32_FLOAT: return GX2SurfaceFormat.TCD_R32_Float;
                case TEX_FORMAT.R8G8_UNORM: return GX2SurfaceFormat.TC_R8_G8_UNorm;
                case TEX_FORMAT.R8_UNORM: return GX2SurfaceFormat.TC_R8_UNorm;
                case TEX_FORMAT.A8_UNORM: return GX2SurfaceFormat.TC_R8_UNorm;
                case TEX_FORMAT.B8G8R8A8_UNORM: return GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm;
                case TEX_FORMAT.B8G8R8A8_UNORM_SRGB: return GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB;
                case TEX_FORMAT.R32G8X24_FLOAT: return GX2SurfaceFormat.T_R32_Float_X8_X24;
                default:
                    throw new Exception($"Cannot convert format {texFormat}");
            }
        }

        public static TEX_FORMAT ConvertFromGx2Format(GX2SurfaceFormat GX2Format)
        {
            switch (GX2Format)
            {
                case GX2SurfaceFormat.T_BC1_UNorm: return TEX_FORMAT.BC1_UNORM;
                case GX2SurfaceFormat.T_BC1_SRGB: return TEX_FORMAT.BC1_UNORM_SRGB;
                case GX2SurfaceFormat.T_BC2_UNorm: return TEX_FORMAT.BC2_UNORM;
                case GX2SurfaceFormat.T_BC2_SRGB: return TEX_FORMAT.BC2_UNORM_SRGB;
                case GX2SurfaceFormat.T_BC3_UNorm: return TEX_FORMAT.BC3_UNORM;
                case GX2SurfaceFormat.T_BC3_SRGB: return TEX_FORMAT.BC3_UNORM_SRGB;
                case GX2SurfaceFormat.T_BC4_UNorm: return TEX_FORMAT.BC4_UNORM;
                case GX2SurfaceFormat.T_BC4_SNorm: return TEX_FORMAT.BC4_SNORM;
                case GX2SurfaceFormat.T_BC5_UNorm: return TEX_FORMAT.BC5_UNORM;
                case GX2SurfaceFormat.T_BC5_SNorm: return TEX_FORMAT.BC5_SNORM;
                case GX2SurfaceFormat.TC_R5_G5_B5_A1_UNorm: return TEX_FORMAT.B5G5R5A1_UNORM;
                case GX2SurfaceFormat.TC_A1_B5_G5_R5_UNorm: return TEX_FORMAT.B5G5R5A1_UNORM;
                case GX2SurfaceFormat.TC_R4_G4_B4_A4_UNorm: return TEX_FORMAT.B4G4R4A4_UNORM;
                case GX2SurfaceFormat.TCS_R5_G6_B5_UNorm: return TEX_FORMAT.B5G6R5_UNORM;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm: return TEX_FORMAT.R8G8B8A8_UNORM;
                case GX2SurfaceFormat.TCS_R10_G10_B10_A2_UNorm: return TEX_FORMAT.R10G10B10A2_UNORM;
                case GX2SurfaceFormat.TC_R11_G11_B10_Float: return TEX_FORMAT.R11G11B10_FLOAT;
                case GX2SurfaceFormat.TCD_R16_UNorm: return TEX_FORMAT.R16_UNORM;
                case GX2SurfaceFormat.TCD_R32_Float: return TEX_FORMAT.R32_FLOAT;
                case GX2SurfaceFormat.T_R4_G4_UNorm: return TEX_FORMAT.R4G4_UNORM;
                case GX2SurfaceFormat.TC_R8_G8_UNorm: return TEX_FORMAT.R8G8_UNORM;
                case GX2SurfaceFormat.TC_R8_UNorm: return TEX_FORMAT.R8_UNORM;
                    case GX2SurfaceFormat.TC_R16_G16_B16_A16_Float: return TEX_FORMAT.R16G16B16A16_FLOAT;
                case GX2SurfaceFormat.Invalid: throw new Exception("Invalid Format");
                default:
                    throw new Exception($"Cannot convert format {GX2Format}");
            }
        }
        public static void SetChannelsByFormat(GX2SurfaceFormat Format, Texture tex)
        {
            switch (Format)
            {
                case GX2SurfaceFormat.T_BC5_UNorm:
                case GX2SurfaceFormat.T_BC5_SNorm:
                    tex.CompSelR = GX2CompSel.ChannelR;
                    tex.CompSelG = GX2CompSel.ChannelG;
                    tex.CompSelB = GX2CompSel.Always0;
                    tex.CompSelA = GX2CompSel.Always1;
                    break;
                case GX2SurfaceFormat.T_BC4_SNorm:
                case GX2SurfaceFormat.T_BC4_UNorm:
                    tex.CompSelR = GX2CompSel.ChannelR;
                    tex.CompSelG = GX2CompSel.ChannelR;
                    tex.CompSelB = GX2CompSel.ChannelR;
                    tex.CompSelA = GX2CompSel.ChannelR;
                    break;
            }
        }

        public void Export(string FileName, bool ExportSurfaceLevel = false,
        bool ExportMipMapLevel = false, int SurfaceLevel = 0, int MipLevel = 0)
        {
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bftex":
                    SaveBinaryTexture(FileName);
                    break;
                case ".dds":
                case ".dds2":
                    SaveDDS(FileName);
                    break;
                default:
                    SaveBitMap(FileName);
                    break;
            }
        }
        internal void SaveBinaryTexture(string FileName)
        {
             texture.Export(FileName, ((BFRESGroupNode)Parent).GetResFileU());
        }

        private bool IsEditorActive()
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
                return false;
            else
                return true;
        }

        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }
        public void UpdateEditor()
        {
            if (Parent != null && Parent.Parent != null && Parent.Parent is BFRES)
            {
                ((BFRES)Parent.Parent).LoadEditors(this);
                return;
            }

            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;

                LibraryGUI.Instance.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.LoadProperties(this.texture);
            editor.LoadImage(this);

            if (texture.UserData != null)
            {
                UserDataEditor userEditor = (UserDataEditor)editor.GetActiveTabEditor(typeof(UserDataEditor));
                if (userEditor == null)
                {
                    userEditor = new UserDataEditor();
                    userEditor.Name = "User Data";
                    editor.AddCustomControl(userEditor, typeof(UserDataEditor));
                }
                userEditor.LoadUserData(texture.UserData);
            }
        }

        private void OnPropertyChanged()
        {
            Text = texture.Name;

            RedChannel = SetChannel(texture.CompSelR);
            GreenChannel = SetChannel(texture.CompSelG);
            BlueChannel = SetChannel(texture.CompSelB);
            AlphaChannel = SetChannel(texture.CompSelA);

            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
            if (editor != null)
            {
                editor.UpdateMipDisplay();
            }
        }
    }
}