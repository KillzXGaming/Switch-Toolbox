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
using Smash_Forge.Rendering;
using WeifenLuo.WinFormsUI.Docking;

namespace FirstPlugin
{
    public class FTEXContainer : TreeNodeCustom
    {
        public Dictionary<string, FTEX> Textures = new Dictionary<string, FTEX>(); //To get instance of classes

        public FTEXContainer()
        {
            Text = "Textures";
            Name = "FTEXCONT";

            ContextMenu = new ContextMenu();
            MenuItem exportAll = new MenuItem("Export All Textures");
            ContextMenu.MenuItems.Add(exportAll);
            exportAll.Click += ExportAll;
            MenuItem clear = new MenuItem("Clear");
            ContextMenu.MenuItems.Add(clear);
            clear.Click += Clear;
        }
        private void Clear(object sender, EventArgs args)
        {
            Nodes.Clear();
            Textures.Clear();
        }
        public void RefreshGlTexturesByName()
        {
        }

        public override void OnClick(TreeView treeview)
        {
        }
        public void RemoveTexture(FTEX textureData)
        {
            Nodes.Remove(textureData);
            Textures.Remove(textureData.Text);
            Viewport.Instance.UpdateViewport();
        }

        private void ExportAll(object sender, EventArgs args)
        {
            List<string> Formats = new List<string>();
            Formats.Add("Cafe Binary Textures (.bftex)");
            Formats.Add("Microsoft DDS (.dds)");
            Formats.Add("Portable Graphics Network (.png)");
            Formats.Add("Joint Photographic Experts Group (.jpg)");
            Formats.Add("Bitmap Image (.bmp)");
            Formats.Add("Tagged Image File Format (.tiff)");

            FolderSelectDialog sfd = new FolderSelectDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                TextureFormatExport form = new TextureFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (FTEX tex in Nodes)
                    {
                        if (form.Index == 0)
                            tex.SaveBinaryTexture(folderPath + '\\' + tex.Text + ".bftex");
                        else if (form.Index == 1)
                            tex.SaveDDS(folderPath + '\\' + tex.Text + ".dds");
                        else if (form.Index == 2)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".png");
                        else if (form.Index == 3)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".jpg");
                        else if (form.Index == 4)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".bmp");
                        else if (form.Index == 5)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".tiff");
                    }
                }
            }
        }
    }

    public class FTEX : TreeNodeCustom
    {
        public int format;
        public RenderableTex renderedTex = new RenderableTex();
        GX2CompSel ChannelRed;
        GX2CompSel ChannelBlue;
        GX2CompSel ChannelGreen;
        GX2CompSel ChannelAlpha;

        public FTEX()
        {
            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem remove = new MenuItem("Remove");
            ContextMenu.MenuItems.Add(remove);
            remove.Click += Remove;
            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;
        }

        private void Replace(object sender, EventArgs args)
        {

        }
        private void Rename(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ((FTEXContainer)Parent).Textures.Remove(Text);
                Text = dialog.textBox1.Text;

                ((FTEXContainer)Parent).Textures.Add(Text, this);
            }
        }
        private void Remove(object sender, EventArgs args)
        {
            ((FTEXContainer)Parent).RemoveTexture(this);
        }
        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Text;
            sfd.DefaultExt = "bftex";
            sfd.Filter = "Supported Formats|*.bftex;*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Binary Texture |*.bftex|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export(sfd.FileName);
            }
        }

        public void Read(Texture tex)
        {
            ImageKey = "Texture";
            SelectedImageKey = "Texture";
            Text = tex.Name;

            ChannelRed = tex.CompSelR;
            ChannelGreen = tex.CompSelG;
            ChannelBlue = tex.CompSelB;
            ChannelAlpha = tex.CompSelA;

            renderedTex.width = (int)tex.Width;
            renderedTex.height = (int)tex.Height;
            format = (int)tex.Format;
            int swizzle = (int)tex.Swizzle;
            int pitch = (int)tex.Pitch;
            renderedTex.data = GTX.swizzleBC(tex.Data, renderedTex.width, renderedTex.height, format, (int)tex.TileMode, pitch, swizzle);
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
                    SaveDDS(FileName);
                    break;
                default:
                    SaveBitMap(FileName);
                    break;
            }
        }
        internal void SaveBitMap(string FileName, int SurfaceLevel = 0, int MipLevel = 0)
        {
            Bitmap bitMap = DisplayTexture(MipLevel, SurfaceLevel);

            bitMap.Save(FileName);
        }
        internal void SaveBinaryTexture(string FileName)
        {
            Console.WriteLine("Test");
           // Texture.Export(FileName, bntxFile);
        }
        internal void SaveDDS(string FileName)
        {
            DDS dds = new DDS();
            dds.header = new DDS.Header();
            dds.header.width = (uint)renderedTex.width;
            dds.header.height = (uint)renderedTex.width;
            dds.header.mipmapCount = (uint)renderedTex.mipmaps[0].Count;

            bool IsDX10 = false;

            switch (format)
            {
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_UNORM):
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_SRGB):
                    dds.header.ddspf.fourCC = "DXT1";
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_UNORM):
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_SRGB):
                    dds.header.ddspf.fourCC = "DXT3";
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_UNORM):
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_SRGB):
                    dds.header.ddspf.fourCC = "DXT5";
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_UNORM):
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_SNORM):
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_UNORM):
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_SNORM):
                    IsDX10 = true;
                    dds.DX10header = new DDS.DX10Header();
                    dds.DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM;
                    break;
                default:
                    throw new Exception($"Format {(GTX.GX2SurfaceFormat)format} not supported!");
            }

            if (IsDX10)
                dds.header.ddspf.fourCC = "DX10";

            dds.Save(dds, FileName, IsDX10, renderedTex.mipmaps);
        }


        public void LoadOpenGLTexture()
        {
            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;

            switch (format)
            {
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC1_SRGB):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC2_SRGB):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC3_SRGB):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC4_SNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedSignedRedRgtc1;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_T_BC5_SNORM):
                    //OpenTK doesn't load BC5 SNORM textures right so I'll use the same decompress method bntx has
                    byte[] fixBC5 = DDSCompressor.DecompressBC5(renderedTex.data, renderedTex.width, renderedTex.height, true, true);
                    renderedTex.data = fixBC5;
                    renderedTex.pixelInternalFormat = PixelInternalFormat.Rgba;
                    renderedTex.pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case ((int)GTX.GX2SurfaceFormat.GX2_SURFACE_FORMAT_TCS_R8_G8_B8_A8_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.Rgba;
                    renderedTex.pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
            }
            renderedTex.display = loadImage(renderedTex);
        }
        public static int loadImage(RenderableTex t)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);

            if (t.pixelInternalFormat != PixelInternalFormat.Rgba)
            {
                GL.CompressedTexImage2D<byte>(TextureTarget.Texture2D, 0, (InternalFormat)t.pixelInternalFormat,
                    t.width, t.height, 0, getImageSize(t), t.data);
                //Debug.WriteLine(GL.GetError());
            }
            else
            {
                GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, t.pixelInternalFormat, t.width, t.height, 0,
                    t.pixelFormat, PixelType.UnsignedByte, t.data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }
        public static int getImageSize(RenderableTex t)
        {
            switch (t.pixelInternalFormat)
            {
                case PixelInternalFormat.CompressedRgbaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedRedRgtc1:
                case PixelInternalFormat.CompressedSignedRedRgtc1:
                    return (t.width * t.height / 2);
                case PixelInternalFormat.CompressedRgbaS3tcDxt3Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext:
                case PixelInternalFormat.CompressedRgbaS3tcDxt5Ext:
                case PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext:
                case PixelInternalFormat.CompressedSignedRgRgtc2:
                case PixelInternalFormat.CompressedRgRgtc2:
                    return (t.width * t.height);
                case PixelInternalFormat.Rgba:
                    return t.data.Length;
                default:
                    return t.data.Length;
            }
        }
        public unsafe Bitmap GLTextureToBitmap(RenderableTex t, int id)
        {
            Bitmap bitmap = new Bitmap(t.width, t.height);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, t.width, t.height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public override void OnClick(TreeView treeView)
        {
            foreach (Control control in FirstPlugin.MainF.Controls)
            {
                if (control is DockPanel)
                {
                    if (FirstPlugin.DockedEditorS == null)
                    {
                        FirstPlugin.DockedEditorS = new DockContent();
                        FirstPlugin.DockedEditorS.Show((DockPanel)control, PluginRuntime.FSHPDockState);
                    }
                }
            }

            if (!EditorIsActive(FirstPlugin.DockedEditorS))
            {
                FirstPlugin.DockedEditorS.Controls.Clear();

                FTEXEditor FTEXEditor = new FTEXEditor();
                FTEXEditor.Text = Text;
                FTEXEditor.Dock = DockStyle.Fill;
                FTEXEditor.LoadPicture(DisplayTexture());
                FTEXEditor.LoadProperty(this);
                FirstPlugin.DockedEditorS.Controls.Add(FTEXEditor);
            }
        }
        public bool EditorIsActive(DockContent dock)
        {
            foreach (Control ctrl in dock.Controls)
            {
                if (ctrl is FTEXEditor)
                {
                    dock.Text = Text;
                    ((FTEXEditor)ctrl).LoadPicture(DisplayTexture());
                    ((FTEXEditor)ctrl).LoadProperty(this);
                    return true;
                }
            }

            return false;
        }

        public class RenderableTex
        {
            public int width, height;
            public int display;
            public PixelInternalFormat pixelInternalFormat;
            public PixelFormat pixelFormat;
            public PixelType pixelType = PixelType.UnsignedByte;
            public int mipMapCount;
            public List<List<byte[]>> mipmaps = new List<List<byte[]>>();

            public byte[] data
            {
                get
                {
                    return mipmaps[0][0];
                }
                set
                {
                    List<byte[]> mips = new List<byte[]>();
                    mips.Add(value);
                    mipmaps.Add(mips);
                }
            }
            public class Surface
            {

            }
        }

        public Bitmap DisplayTexture(int DisplayMipIndex = 0, int ArrayIndex = 0)
        {
            if (renderedTex.mipmaps.Count <= 0)
            {
                throw new Exception("No texture data found");
            }

            uint width = (uint)Math.Max(1, renderedTex.width >> DisplayMipIndex);
            uint height = (uint)Math.Max(1, renderedTex.height >> DisplayMipIndex);

            byte[] data = renderedTex.mipmaps[ArrayIndex][DisplayMipIndex];

            return DecodeBlock(data, width, height, (GX2SurfaceFormat)format);
        }

        public static Bitmap DecodeBlock(byte[] data, uint Width, uint Height, GX2SurfaceFormat Format)
        {
            Bitmap decomp;

            if (Format == GX2SurfaceFormat.T_BC5_SNorm)
                return DDSCompressor.DecompressBC5(data, (int)Width, (int)Height, true);

            byte[] d = null;
            if (IsCompressedFormat(Format))
                d = DDSCompressor.DecompressBlock(data, (int)Width, (int)Height, GetCompressedDXGI_FORMAT(Format));
            else
                d = DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, GetUncompressedDXGI_FORMAT(Format));

            if (d != null)
            {
                decomp = BitmapExtension.GetBitmap(d, (int)Width, (int)Height);
                return SwapBlueRedChannels(decomp);
            }
            return null;
        }
        private static DDS.DXGI_FORMAT GetUncompressedDXGI_FORMAT(GX2SurfaceFormat Format)
        {
            switch (Format)
            {
                case GX2SurfaceFormat.TC_A1_B5_G5_R5_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM;
                case GX2SurfaceFormat.TC_R4_G4_B4_A4_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM;
                case GX2SurfaceFormat.TCS_R5_G6_B5_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
                case GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                case GX2SurfaceFormat.TCS_R10_G10_B10_A2_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UNORM;
                case GX2SurfaceFormat.TC_R11_G11_B10_Float: return DDS.DXGI_FORMAT.DXGI_FORMAT_R11G11B10_FLOAT;
                case GX2SurfaceFormat.TCD_R16_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_R16_UNORM;
                case GX2SurfaceFormat.TCD_R32_Float: return DDS.DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT;
                case GX2SurfaceFormat.T_R4_G4_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM;
                case GX2SurfaceFormat.TC_R8_G8_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM;
                case GX2SurfaceFormat.TC_R8_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_R8_UNORM;
                case GX2SurfaceFormat.Invalid: throw new Exception("Invalid Format");
                default:
                    throw new Exception($"Cannot convert format {Format}");
            }
        }
        private static bool IsCompressedFormat(GX2SurfaceFormat Format)
        {
            switch (Format)
            {
                case GX2SurfaceFormat.T_BC1_UNorm: 
                case GX2SurfaceFormat.T_BC1_SRGB: 
                case GX2SurfaceFormat.T_BC2_SRGB:
                case GX2SurfaceFormat.T_BC2_UNorm:
                case GX2SurfaceFormat.T_BC3_UNorm: 
                case GX2SurfaceFormat.T_BC3_SRGB:
                case GX2SurfaceFormat.T_BC4_UNorm:
                case GX2SurfaceFormat.T_BC4_SNorm:
                case GX2SurfaceFormat.T_BC5_UNorm: 
                case GX2SurfaceFormat.T_BC5_SNorm:
                    return true;
                default:
                    return false;
            }
        }
        private static DDS.DXGI_FORMAT GetCompressedDXGI_FORMAT(GX2SurfaceFormat Format)
        {
            switch (Format)
            {
                case GX2SurfaceFormat.T_BC1_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                case GX2SurfaceFormat.T_BC1_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB;
                case GX2SurfaceFormat.T_BC2_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
                case GX2SurfaceFormat.T_BC2_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB;
                case GX2SurfaceFormat.T_BC3_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                case GX2SurfaceFormat.T_BC3_SRGB: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB;
                case GX2SurfaceFormat.T_BC4_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
                case GX2SurfaceFormat.T_BC4_SNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM;
                case GX2SurfaceFormat.T_BC5_UNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
                case GX2SurfaceFormat.T_BC5_SNorm: return DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM;
                default:
                    throw new Exception($"Cannot convert format {Format}");
            }
        }
        public static Bitmap SwapBlueRedChannels(Bitmap bitmap)
        {
            return ColorComponentSelector(bitmap, GX2CompSel.ChannelB, GX2CompSel.ChannelG, GX2CompSel.ChannelR, GX2CompSel.ChannelA);
        }
        public Bitmap UpdateBitmap(Bitmap image)
        {
            return ColorComponentSelector(image, ChannelRed, ChannelGreen, ChannelBlue, ChannelAlpha);
        }
        public static Bitmap ColorComponentSelector(Bitmap image, GX2CompSel R, GX2CompSel G, GX2CompSel B, GX2CompSel A)
        {
            BitmapExtension.ColorSwapFilter color = new BitmapExtension.ColorSwapFilter();
            if (R == GX2CompSel.ChannelR)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Red;
            if (R == GX2CompSel.ChannelG)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Green;
            if (R == GX2CompSel.ChannelB)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Blue;
            if (R == GX2CompSel.ChannelA)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Alpha;
            if (R == GX2CompSel.Always0)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Zero;
            if (R == GX2CompSel.Always1)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.One;

            if (G == GX2CompSel.ChannelR)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Red;
            if (G == GX2CompSel.ChannelG)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Green;
            if (G == GX2CompSel.ChannelB)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Blue;
            if (G == GX2CompSel.ChannelA)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Alpha;
            if (G == GX2CompSel.Always0)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Zero;
            if (G == GX2CompSel.Always1)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.One;

            if (B == GX2CompSel.ChannelR)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Red;
            if (B == GX2CompSel.ChannelG)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Green;
            if (B == GX2CompSel.ChannelB)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Blue;
            if (B == GX2CompSel.ChannelA)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Alpha;
            if (B == GX2CompSel.Always0)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Zero;
            if (B == GX2CompSel.Always1)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.One;

            if (A == GX2CompSel.ChannelR)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Red;
            if (A == GX2CompSel.ChannelG)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Green;
            if (A == GX2CompSel.ChannelB)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Blue;
            if (A == GX2CompSel.ChannelA)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Alpha;
            if (A == GX2CompSel.Always0)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Zero;
            if (A == GX2CompSel.Always1)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.One;

            return BitmapExtension.SwapRGB(image, color);
        }
    }
}
