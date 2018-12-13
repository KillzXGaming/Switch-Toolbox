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
                BinaryTextureImporterList importer = new BinaryTextureImporterList();
                List<TextureImporterSettings> settings = new List<TextureImporterSettings>();
                foreach (string name in ofd.FileNames)
                {
                
               
                    settings.Clear();
                    GC.Collect();
                    Cursor.Current = Cursors.Default;
                }
            }
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

            TextureImporterSettings setting = new TextureImporterSettings();
            BinaryTextureImporterList importer = new BinaryTextureImporterList();
        }
        //We reuse GX2 data as it's the same thing
        public Texture FromGx2Surface(GTX.GX2Surface surf, TextureImporterSettings settings)
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

            Width = tex.Width;
            Height = tex.Height;
            Format = ConvertFormat(tex.Format);
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
            Surfaces.Add(new Surface() { mipmaps = mips });

            RenderableTex.LoadOpenGLTexture(this);
        }
        private TEX_FORMAT ConvertFormat(GX2SurfaceFormat Format)
        {
            return TEX_FORMAT.UNKNOWN;
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
        internal void SaveBinaryTexture(string FileName)
        {
            Console.WriteLine("Test");
           // Texture.Export(FileName, bntxFile);
        }
   
        public override void OnClick(TreeView treeView)
        {
        }
    }
}
