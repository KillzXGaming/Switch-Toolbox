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
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class FTEXContainer : TreeNodeCustom
    {
        public Dictionary<string, FTEX> Textures = new Dictionary<string, FTEX>(); //To get instance of classes

        public FTEXContainer()
        {
            Text = "Textures";
            Name = "FTEX";

            ContextMenu = new ContextMenu();
            MenuItem importTex = new MenuItem("Import");
            ContextMenu.MenuItems.Add(importTex);
            importTex.Click += Import;
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

        public void RemoveTexture(FTEX textureData)
        {
            Nodes.Remove(textureData);
            Textures.Remove(textureData.Text);
            Viewport.Instance.UpdateViewport();
        }
        private void Import(object sender, EventArgs args)
        {
            ImportTexture();
        }
        public void ImportTexture()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                                     "Microsoft DDS |*.dds|" +
                                     "Portable Network Graphics |*.png|" +
                                     "Joint Photographic Experts Group |*.jpg|" +
                                     "Bitmap Image |*.bmp|" +
                                     "Tagged Image File Format |*.tiff|" +
                                     "All files(*.*)|*.*";

            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                GTXTextureImporter importer = new GTXTextureImporter();
                List<GTXImporterSettings> settings = new List<GTXImporterSettings>();
                foreach (string name in ofd.FileNames)
                {
                    string ext = System.IO.Path.GetExtension(name);
                    ext = ext.ToLower();

                    if (ext == ".dds")
                    {
                        settings.Add(LoadSettings(name));
                        if (settings.Count == 0)
                        {
                            importer.Dispose();
                            return;
                        }
                        importer.LoadSettings(settings);
                        foreach (var setting in settings)
                        {
                            if (setting.DataBlockOutput != null)
                            {
                                GTX.GX2Surface tex = setting.CreateGx2Texture(setting.DataBlockOutput[0]);
                                FTEX ftex = new FTEX();
                                ftex.texture = ftex.FromGx2Surface(tex, setting);
                                Nodes.Add(ftex);
                                ftex.Read(ftex.texture);

                                Textures.Add(ftex.Text, ftex);
                                ftex.LoadOpenGLTexture();
                            }
                        }
                    }
                    else
                    {
                        settings.Add(LoadSettings(name));
                        if (settings.Count == 0)
                        {
                            importer.Dispose();
                            return;
                        }
                        importer.LoadSettings(settings);
                        if (importer.ShowDialog() == DialogResult.OK)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            foreach (var setting in settings)
                            {
                                if (setting.GenerateMipmaps)
                                {
                                    setting.DataBlockOutput.Clear();
                                    setting.DataBlockOutput.Add(setting.GenerateMips());
                                }
                                if (setting.DataBlockOutput != null)
                                {
                                    GTX.GX2Surface tex = setting.CreateGx2Texture(setting.DataBlockOutput[0]);
                                    FTEX ftex = new FTEX();
                                    ftex.texture = ftex.FromGx2Surface(tex, setting);
                                    Nodes.Add(ftex);
                                    ftex.Read(ftex.texture);

                                    Textures.Add(ftex.Text, ftex);
                                    ftex.LoadOpenGLTexture();
                                }
                                else
                                {
                                    MessageBox.Show("Something went wrong???");
                                }
                            }
                        }
                    }
               
                    settings.Clear();
                    GC.Collect();
                    Cursor.Current = Cursors.Default;
                }
            }
        }
        public GTXImporterSettings LoadSettings(string name)
        {
            var importer = new GTXImporterSettings();
            string ext = System.IO.Path.GetExtension(name);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".dds":
                    importer.LoadDDS(name);
                    break;
                default:
                    importer.LoadBitMap(name);
                    break;
            }

            return importer;
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

    public class FTEX : STGenericTexture
    {
        public int format;
        public RenderableTex renderedTex = new RenderableTex();
        public Texture texture;

        public FTEX()
        {
            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
            MenuItem remove = new MenuItem("Remove");
            ContextMenu.MenuItems.Add(remove);
            remove.Click += Remove;
            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;
        }
        //For determining mip map file for botw (Tex2)
        public string GetFilePath()
        {
            if (Parent == null)
                throw new Exception("Parent is null!");

            return ((BFRES)Parent.Parent).FilePath;
        }

        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                         "Microsoft DDS |*.dds|" +
                         "Portable Network Graphics |*.png|" +
                         "Joint Photographic Experts Group |*.jpg|" +
                         "Bitmap Image |*.bmp|" +
                         "Tagged Image File Format |*.tiff|" +
                         "All files(*.*)|*.*";

            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Replace(ofd.FileName);
            }
        }
        public void Replace(string FileName)
        {
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            GTXImporterSettings setting = new GTXImporterSettings();
            GTXTextureImporter importer = new GTXTextureImporter();

            switch (ext)
            {
                case ".dds":
                    setting.LoadDDS(FileName, null);
                    break;
                default:
                    setting.LoadBitMap(FileName);
                    importer.LoadSetting(setting);
                    break;
            }

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
                    var surface = setting.CreateGx2Texture(setting.DataBlockOutput[0]);
                    texture = FromGx2Surface(surface, setting);
                    LoadOpenGLTexture();
                }
                else
                {
                    MessageBox.Show("Something went wrong???");
                }
                UpdateEditor();
            }
        }
        //We reuse GX2 data as it's the same thing
        public Texture FromGx2Surface(GTX.GX2Surface surf, GTXImporterSettings settings)
        {
            Texture tex = new Texture();
            tex.Name = settings.TexName;
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

            texture = tex;

            renderedTex = new RenderableTex();
            Width = tex.Width;
            Height = tex.Height;

            renderedTex.width = (int)Width;
            renderedTex.height = (int)Height;
            format = (int)tex.Format;
            int swizzle = (int)tex.Swizzle;
            int pitch = (int)tex.Pitch;
            uint bpp = GTX.surfaceGetBitsPerPixel((uint)format) >> 3;

            GTX.GX2Surface surf = new GTX.GX2Surface();
            surf.bpp = bpp;
            surf.height = tex.Height;
            surf.width = tex.Width;
            surf.aa = (uint)tex.AAMode;
            surf.alignment = tex.Alignment;
            surf.depth = tex.Depth;
            surf.dim = (uint)tex.Dim;
            surf.format = (uint)tex.Format;
            surf.use = (uint)tex.Use;
            surf.pitch = tex.Pitch;
            surf.data = tex.Data;
            surf.numMips = tex.MipCount;
            surf.mipOffset = tex.MipOffsets;
            surf.mipData = tex.MipData;
            surf.tileMode = (uint)tex.TileMode;
            surf.swizzle = tex.Swizzle;

            if (IsCompressedFormat(tex.Format))
                Format = GetCompressedDXGI_FORMAT(tex.Format);
            else
                Format = GetUncompressedDXGI_FORMAT(tex.Format);

            //Determine tex2 botw files to get mip maps

            string Tex1 = GetFilePath();
            if (Tex1.Contains(".Tex1"))
            {
                string Tex2 = Tex1.Replace(".Tex1", ".Tex2");
                Console.WriteLine(Tex2);

                if (System.IO.File.Exists(Tex2))
                {
                    ResFile resFile2 = new ResFile(new System.IO.MemoryStream(
                        EveryFileExplorer.YAZ0.Decompress(Tex2)));

                    if (resFile2.Textures.ContainsKey(tex.Name))
                    {
                        surf.mipData = resFile2.Textures[tex.Name].MipData;
                        surf.mipOffset = resFile2.Textures[tex.Name].MipOffsets;
                    }
                }
            }


            if (surf.mipData == null)
                surf.numMips = 1;

            List<byte[]> mips = GTX.Decode(surf);
            renderedTex.mipmaps.Add(mips);
            surfaces.Add(new Surface() { mipmaps = mips });

            renderedTex.data = renderedTex.mipmaps[0][0];

            LoadOpenGLTexture();
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
                    channels[3] = GX2CompSel.Always1;
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
            dds.header.width = Width;
            dds.header.height = Height;
            dds.header.mipmapCount = (uint)surfaces[0].mipmaps.Count;

            dds.header.pitchOrLinearSize = (uint)surfaces[0].mipmaps[0].Length;

            if (IsCompressedFormat((GX2SurfaceFormat)format))
                dds.SetFlags((DDS.DXGI_FORMAT)GetCompressedDXGI_FORMAT((GX2SurfaceFormat)format));
            else
                dds.SetFlags((DDS.DXGI_FORMAT)GetUncompressedDXGI_FORMAT((GX2SurfaceFormat)format));

            dds.Save(dds, FileName, surfaces);
        }


        public void LoadOpenGLTexture()
        {
            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;


            switch (format)
            {
                case ((int)GTX.GX2SurfaceFormat.T_BC1_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC1_SRGB):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC2_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC2_SRGB):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC3_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC3_SRGB):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC4_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC4_SNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedSignedRedRgtc1;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC5_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case ((int)GTX.GX2SurfaceFormat.T_BC5_SNORM):
                    //OpenTK doesn't load BC5 SNORM textures right so I'll use the same decompress method bntx has
                    renderedTex.data = DDSCompressor.DecompressBC5(renderedTex.mipmaps[0][0], (int)renderedTex.width, (int)renderedTex.height, true, true);
                    renderedTex.pixelInternalFormat = PixelInternalFormat.Rgba;
                    renderedTex.pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case ((int)GTX.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.Rgba;
                    renderedTex.pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case ((int)GTX.GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB):
                    renderedTex.pixelInternalFormat = PixelInternalFormat.Rgba;
                    renderedTex.pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                default:
                    renderedTex.data = BitmapExtension.ImageToByte(DecodeBlock(renderedTex.data, (uint)renderedTex.width, (uint)renderedTex.height, (GX2SurfaceFormat)format));
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
            UpdateEditor();
        }

        public void UpdateEditor()
        {
            if (Viewport.Instance.gL_ControlModern1.Visible == false)
                PluginRuntime.FSHPDockState = WeifenLuo.WinFormsUI.Docking.DockState.Document;

            FTEXEditor docked = (FTEXEditor)LibraryGUI.Instance.GetContentDocked(new FTEXEditor());
            if (docked == null)
            {
                docked = new FTEXEditor();
                LibraryGUI.Instance.LoadDockContent(docked, PluginRuntime.FSHPDockState);
            }
            docked.Text = Text;
            docked.Dock = DockStyle.Fill;
            docked.LoadPicture(DisplayTexture());
            docked.LoadProperty(this);
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
            public byte[] data;

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

            try
            {
                if (Format == GX2SurfaceFormat.T_BC5_SNorm)
                    return DDSCompressor.DecompressBC5(data, (int)Width, (int)Height, true);

                if (Format == GX2SurfaceFormat.TC_R8_UNorm)
                    System.IO.File.WriteAllBytes("TC_R8_UNorm2.bin", data);

                byte[] d = null;
                if (IsCompressedFormat(Format))
                    d = DDSCompressor.DecompressBlock(data, (int)Width, (int)Height, (DDS.DXGI_FORMAT)GetCompressedDXGI_FORMAT(Format));
                else
                    d = DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, (DDS.DXGI_FORMAT)GetUncompressedDXGI_FORMAT(Format));

                if (d != null)
                {
                    decomp = BitmapExtension.GetBitmap(d, (int)Width, (int)Height);

                    if (Format == GX2SurfaceFormat.TCS_R5_G6_B5_UNorm)
                        return decomp;
                    else
                        return SwapBlueRedChannels(decomp);
                }
                return BitmapExtension.GetBitmap(d, (int)Width, (int)Height);;
            }
            catch
            {
                throw new Exception($"Bad size from format {Format}");
            }
        }
        public static byte[] CompressBlock(byte[] data, int width, int height, GTX.GX2SurfaceFormat format, float alphaRef)
        {
            if (IsCompressedFormat((GX2SurfaceFormat)format))
                return DDSCompressor.CompressBlock(data, width, height, (DDS.DXGI_FORMAT)GetCompressedDXGI_FORMAT((GX2SurfaceFormat)format), alphaRef);
            else
                return DDSCompressor.EncodePixelBlock(data, width, height, (DDS.DXGI_FORMAT)GetUncompressedDXGI_FORMAT((GX2SurfaceFormat)format));
        }
        private static TEX_FORMAT GetUncompressedDXGI_FORMAT(GX2SurfaceFormat Format)
        {
            switch (Format)
            {
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
                case GX2SurfaceFormat.T_BC2_UNorm:
                case GX2SurfaceFormat.T_BC2_SRGB:
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
        private static TEX_FORMAT GetCompressedDXGI_FORMAT(GX2SurfaceFormat Format)
        {
            switch (Format)
            {
                case GX2SurfaceFormat.T_BC1_UNorm: return TEX_FORMAT.BC1_UNORM;
                case GX2SurfaceFormat.T_BC1_SRGB: return TEX_FORMAT.BC1_UNORM;
                case GX2SurfaceFormat.T_BC2_UNorm: return TEX_FORMAT.BC2_UNORM;
                case GX2SurfaceFormat.T_BC2_SRGB: return TEX_FORMAT.BC2_UNORM;
                case GX2SurfaceFormat.T_BC3_UNorm: return TEX_FORMAT.BC3_UNORM;
                case GX2SurfaceFormat.T_BC3_SRGB: return TEX_FORMAT.BC3_UNORM;
                case GX2SurfaceFormat.T_BC4_UNorm: return TEX_FORMAT.BC4_UNORM;
                case GX2SurfaceFormat.T_BC4_SNorm: return TEX_FORMAT.BC4_SNORM;
                case GX2SurfaceFormat.T_BC5_UNorm: return TEX_FORMAT.BC5_UNORM;
                case GX2SurfaceFormat.T_BC5_SNorm: return TEX_FORMAT.BC5_SNORM;
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
            return ColorComponentSelector(image, texture.CompSelR,
                texture.CompSelG, texture.CompSelB, texture.CompSelA);
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
