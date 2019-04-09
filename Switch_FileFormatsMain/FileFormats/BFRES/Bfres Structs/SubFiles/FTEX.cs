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
                        TEX_FORMAT.B5_G5_R5_A1_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                        TEX_FORMAT.R32G8X24_FLOAT,
                };
            }
        }

        bool IsReplaced = false;

        public override bool CanEdit { get; set; } = false;

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

        public static GTXImporterSettings SetImporterSettings(string name)
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
                    break;
            }

            return importer;
        }

        public void ReplaceImage(Image image, string FileName)
        {
            GTXImporterSettings setting = SetImporterSettings(image, FileName);
            GTXTextureImporter importer = new GTXTextureImporter();

            if (Tex2Swizzle != 0)
                setting.swizzle = Tex2Swizzle;

            importer.LoadSetting(setting);

            if (importer.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                if (setting.GenerateMipmaps)
                {
                    setting.DataBlockOutput.Clear();
                    setting.DataBlockOutput.Add(setting.GenerateMips());
                }

                if (setting.DataBlockOutput != null)
                {
                    var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                    var tex = FromGx2Surface(surface, setting);
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

            if (IsEditorActive())
                UpdateEditor();
        }

        public override void Replace(string FileName) {
            ReplaceTexture(FileName);
        }

        public void ReplaceTexture(string FileName, uint MipMapCount = 0, TEX_FORMAT[] SupportedFormats = null)
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

            GTXImporterSettings setting = SetImporterSettings(FileName);
            GTXTextureImporter importer = new GTXTextureImporter();

            setting.swizzle = 0;

            if (Tex2Swizzle != 0)
                setting.swizzle = Tex2Swizzle;
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
                    var tex = FromGx2Surface(surface, setting);
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

                    if (setting.GenerateMipmaps)
                    {
                        setting.DataBlockOutput.Clear();
                        setting.DataBlockOutput.Add(setting.GenerateMips());
                    }

                    if (setting.DataBlockOutput != null)
                    {
                        var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                        var tex = FromGx2Surface(surface, setting);
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
            if (IsEditorActive())
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
        public Texture FromGx2Surface(GX2.GX2Surface surf, GTXImporterSettings settings)
        {
            Texture tex = new Texture();
            tex.Name = settings.TexName;
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
            tex.Regs = new uint[5];
            tex.ArrayLength = 1;
            var channels = SetChannelsByFormat((GX2SurfaceFormat)surf.format);
            tex.CompSelR = channels[0];
            tex.CompSelG = channels[1];
            tex.CompSelB = channels[2];
            tex.CompSelA = channels[3];
            tex.UserData = new ResDict<UserData>();
            return tex;
        }
        public uint Tex2Swizzle = 0;

        public void Read(Texture tex)
        {
            CanReplace = true;
            CanDelete = true;
            CanEdit = false;
            CanExport = true;

            ImageKey = "Texture";
            SelectedImageKey = "Texture";
            Text = tex.Name;

            texture = tex;

            Width = tex.Width;
            Height = tex.Height;
            Format = ConvertFromGx2Format(tex.Format);
            ArrayCount = tex.ArrayLength;
            if (tex.ArrayLength == 0)
                ArrayCount = tex.Depth; //Some older bfres don't use array length????

            MipCount = tex.MipCount;

            if (tex.MipData == null || tex.MipData.Length <= 0)
                MipCount = 1;
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException("Cannot set image data! Operation not implemented!");
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            format = (int)texture.Format;
            int swizzle = (int)texture.Swizzle;
            int pitch = (int)texture.Pitch;
            uint bpp = GX2.surfaceGetBitsPerPixel((uint)format) >> 3;

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
            surf.numMips = texture.MipCount;
            surf.mipOffset = texture.MipOffsets;
            surf.mipData = texture.MipData;
            surf.tileMode = (uint)texture.TileMode;
            surf.swizzle = texture.Swizzle;
            surf.numArray = texture.ArrayLength;

            //Determine tex2 botw files to get mip maps
            string Tex1 = GetFilePath();

            Console.WriteLine("IsEdited " + IsReplaced);

            if (!IsReplaced && Tex1 != null && Tex1.Contains(".Tex1"))
            {
                string Tex2 = Tex1.Replace(".Tex1", ".Tex2");
                Console.WriteLine(Tex2 + " " + System.IO.File.Exists(Tex2) + " " + texture.Name);

                if (System.IO.File.Exists(Tex2))
                {
                    ResFile resFile2;

                    if (Tex2.EndsWith(".sbfres"))
                    {
                        resFile2 = new ResFile(new System.IO.MemoryStream(
                                        EveryFileExplorer.YAZ0.Decompress(Tex2)));
                    }
                    else
                    {
                         resFile2 = new ResFile(Tex2);
                    }

                    Console.WriteLine((resFile2.Textures.ContainsKey(texture.Name)));

                    if (resFile2.Textures.ContainsKey(texture.Name))
                    {
                        MipCount = texture.MipCount;
                        texture.MipData = resFile2.Textures[texture.Name].MipData;
                        texture.MipOffsets = resFile2.Textures[texture.Name].MipOffsets;
                        surf.mipData = resFile2.Textures[texture.Name].MipData;
                        surf.mipOffset = resFile2.Textures[texture.Name].MipOffsets;
                        Tex2Swizzle = resFile2.Textures[texture.Name].Swizzle;
                        surf.mip_swizzle = Tex2Swizzle;
                    }
                }
            }


            if (surf.mipData == null)
                surf.numMips = 1;

            var surfaces = GX2.Decode(surf);

            if (ArrayLevel >= surfaces.Count)
                throw new Exception("Invalid amount of surfaces decoded!");

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
                case GX2SurfaceFormat.T_BC3_SRGB: return TEX_FORMAT.BC3_UNORM;
                case GX2SurfaceFormat.T_BC4_UNorm: return TEX_FORMAT.BC4_UNORM;
                case GX2SurfaceFormat.T_BC4_SNorm: return TEX_FORMAT.BC4_SNORM;
                case GX2SurfaceFormat.T_BC5_UNorm: return TEX_FORMAT.BC5_UNORM;
                case GX2SurfaceFormat.T_BC5_SNorm: return TEX_FORMAT.BC5_SNORM;
                case GX2SurfaceFormat.TC_R5_G5_B5_A1_UNorm: return TEX_FORMAT.B5G5R5A1_UNORM;
                case GX2SurfaceFormat.TC_A1_B5_G5_R5_UNorm: return TEX_FORMAT.B5G5R5A1_UNORM;
                case GX2SurfaceFormat.TC_R4_G4_B4_A4_UNorm: return TEX_FORMAT.B4G4R4A4_UNORM;
                case GX2SurfaceFormat.TCS_R5_G6_B5_UNorm: return TEX_FORMAT.B5G6R5_UNORM;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm: return TEX_FORMAT.R8G8B8A8_UNORM;
                case GX2SurfaceFormat.TCS_R10_G10_B10_A2_UNorm: return TEX_FORMAT.R10G10B10A2_UNORM;
                case GX2SurfaceFormat.TC_R11_G11_B10_Float: return TEX_FORMAT.R11G11B10_FLOAT;
                case GX2SurfaceFormat.TCD_R16_UNorm: return TEX_FORMAT.R16_UNORM;
                case GX2SurfaceFormat.TCD_R32_Float: return TEX_FORMAT.R32_FLOAT;
                case GX2SurfaceFormat.T_R4_G4_UNorm: return TEX_FORMAT.B4G4R4A4_UNORM;
                case GX2SurfaceFormat.TC_R8_G8_UNorm: return TEX_FORMAT.R8G8_UNORM;
                case GX2SurfaceFormat.TC_R8_UNorm: return TEX_FORMAT.R8_UNORM;
                    case GX2SurfaceFormat.TC_R16_G16_B16_A16_Float: return TEX_FORMAT.R16G16B16A16_FLOAT;
                case GX2SurfaceFormat.Invalid: throw new Exception("Invalid Format");
                default:
                    throw new Exception($"Cannot convert format {GX2Format}");
            }
        }
        public static GX2CompSel[] SetChannelsByFormat(GX2SurfaceFormat Format)
        {
            GX2CompSel[] channels = new GX2CompSel[4];

            switch (Format)
            {
                case GX2SurfaceFormat.T_BC5_UNorm:
                case GX2SurfaceFormat.T_BC5_SNorm:
                    channels[0] = GX2CompSel.ChannelR;
                    channels[1] = GX2CompSel.ChannelG;
                    channels[2] = GX2CompSel.Always0;
                    channels[3] = GX2CompSel.Always1;
                    break;
                case GX2SurfaceFormat.T_BC4_SNorm:
                case GX2SurfaceFormat.T_BC4_UNorm:
                    channels[0] = GX2CompSel.ChannelR;
                    channels[1] = GX2CompSel.ChannelR;
                    channels[2] = GX2CompSel.ChannelR;
                    channels[3] = GX2CompSel.ChannelR;
                    break;
                default:
                    channels[0] = GX2CompSel.ChannelR;
                    channels[1] = GX2CompSel.ChannelG;
                    channels[2] = GX2CompSel.ChannelB;
                    channels[3] = GX2CompSel.ChannelA;
                    break;
            }
            return channels;
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
    }
}