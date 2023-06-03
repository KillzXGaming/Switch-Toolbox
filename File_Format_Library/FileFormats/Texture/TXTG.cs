using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using static Toolbox.Library.STGenericTexture;
using LibHac;
using System.ComponentModel;
using VGAudio.Utilities;

namespace FirstPlugin
{
    public class TXTG : STGenericTexture, IFileFormat, ILeaveOpenOnLoad, IDisposable
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; } = true;
        public string[] Description { get; set; } = new string[] { "Texture To Go" };
        public string[] Extension { get; set; } = new string[] { "*.txtg" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public TXTG()
        {
        }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true)) {
                return reader.CheckSignature(4, "6PK0", 4);
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

        public override bool CanEdit { get; set; } = true;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                      TEX_FORMAT.BC1_UNORM,
                      TEX_FORMAT.BC2_UNORM,
                      TEX_FORMAT.BC3_UNORM,
                      TEX_FORMAT.BC4_UNORM,
                      TEX_FORMAT.BC5_UNORM,
                      TEX_FORMAT.R8_UNORM,
                      TEX_FORMAT.R8G8_UNORM,
                      TEX_FORMAT.R8G8_UNORM,
                      TEX_FORMAT.R10G10B10A2_UNORM,
                      TEX_FORMAT.B5G6R5_UNORM,
                      TEX_FORMAT.B5G5R5A1_UNORM,
                      TEX_FORMAT.B4G4R4A4_UNORM,
                      TEX_FORMAT.R8G8B8A8_UNORM,
                      TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                };
            }
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

            DisplayProperties prop = new DisplayProperties();
            prop.Width = Width;
            prop.Height = Height;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.Format = this.Format;
            prop.Hash = String.Join(String.Empty, Array.ConvertAll(this.HeaderInfo.Hash, x => x.ToString("X2")));

            editor.Text = Text;
            editor.LoadProperties(prop);
            editor.LoadImage(this);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Header
        {
            public ushort HeaderSize = 0x50;
            public ushort Version = 0x11;
            public Magic Magic = "6PK0";
            public ushort Width;
            public ushort Height;
            public ushort Depth = 1;
            public byte MipCount;
            public byte Unknown1 = 2;
            public byte Unknown2 = 1;
            public ushort Padding = 0;

            public byte FormatFlag = 0; //Unsure how this value works
            public uint FormatSetting = 0; //Varies by format flag. Sometimes a second channel layout 

            public byte CompSelectR = 0;
            public byte CompSelectG = 1;
            public byte CompSelectB = 2;
            public byte CompSelectA = 3;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Hash; //32 byte hash, likely made off file path. Files with identical data still have unique hashes

            public ushort Format;
            public ushort Unknown3 = 0x300; //Always 0x300?

            public uint TextureSetting1 = 1116471296; //00 00 8C 42
            public uint TextureSetting2 = 32563; //Likey some setting for astc. Terrain and tree files use different values for their astc formats.
            public uint TextureSetting3 = 33554944; //00 02 00 02
            public uint TextureSetting4 = 67330; //02 05 01 00 Varies sometimes with second byte
        }

        private Header HeaderInfo;

        //Image data is properly loaded afterwards
        private List<List<byte[]>> ImageList = new List<List<byte[]>>();

        public override ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> items = new List<ToolStripItem>();
            items.Add(new ToolStripMenuItem("Save File", null, (o, e) =>
            {
                STFileSaver.SaveFileFormat(this, FilePath);
            }));
            items.AddRange(base.GetContextMenuItems());
            return items.ToArray();
        }

        public void Load(Stream stream)
        {
            Tag = this;

            CanReplace = true;

            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            string name = Path.GetFileNameWithoutExtension(FileName);
            Text = name;

            //cache for loading file 
            if (PluginRuntime.TextureCache.ContainsKey(name))
                PluginRuntime.TextureCache.Remove(name);

            PluginRuntime.TextureCache.Add(name, this);

            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(false);

                HeaderInfo = reader.ReadStruct<Header>();

                Width = HeaderInfo.Width;
                Height = HeaderInfo.Height;
                ArrayCount = HeaderInfo.Depth;
                MipCount = HeaderInfo.MipCount;

                RedChannel = ChannelList[HeaderInfo.CompSelectR];
                GreenChannel = ChannelList[HeaderInfo.CompSelectG];
                BlueChannel = ChannelList[HeaderInfo.CompSelectB];
                AlphaChannel = ChannelList[HeaderInfo.CompSelectA];

                SurfaceInfo[] surfaces = new SurfaceInfo[MipCount * ArrayCount];

                reader.SeekBegin(HeaderInfo.HeaderSize);
                for (int i = 0; i < MipCount * ArrayCount; i++)
                {
                    surfaces[i] = new SurfaceInfo();
                    surfaces[i].ArrayLevel = reader.ReadUInt16();
                    surfaces[i].MipLevel = reader.ReadByte();
                    reader.ReadByte(); //Always 1
                }

                for (int i = 0; i < MipCount * ArrayCount; i++)
                {
                    surfaces[i].Size = reader.ReadUInt32();
                    reader.ReadUInt32(); //Always 6
                }

                long pos = reader.Position;

                if (FormatList.ContainsKey(HeaderInfo.Format))
                    this.Format = FormatList[HeaderInfo.Format];
                else
                    throw new Exception($"Unsupported format! {HeaderInfo.Format.ToString("X")}");

                //Dumb hack. Terrain is oddly 8x8 astc, but the format seems to be 0x101
                //Use some of the different texture settings, as they likely configure the astc blocks in some way
                if (this.HeaderInfo.TextureSetting2 == 32628)
                {
                    this.Format = TEX_FORMAT.ASTC_8x5_UNORM;
                }
                if (this.HeaderInfo.TextureSetting2 == 32631)
                {
                    this.Format = TEX_FORMAT.ASTC_8x8_UNORM;
                }
                //Image data is properly loaded afterwards
                List<List<byte[]>> data = new List<List<byte[]>>();

                //Combine each mip and array
                for (int i = 0; i < MipCount * ArrayCount; i++)
                {
                    var imageData = reader.ReadBytes((int)(surfaces[i].Size));

                    //Array level
                    if (data.Count <= surfaces[i].ArrayLevel)
                        data.Add(new List<byte[]>());

                    data[surfaces[i].ArrayLevel].Add(Zstb.SDecompress(imageData));
                }
                ImageList = data;
            }
        }


        public void Save(Stream stream)
        {
            //Apply generic properties
            HeaderInfo.Format = FormatList.FirstOrDefault(x => x.Value == Format).Key;
            HeaderInfo.Width = (ushort)this.Width;
            HeaderInfo.Height = (ushort)this.Height;
            HeaderInfo.Depth = (ushort)this.ArrayCount;
            HeaderInfo.MipCount = (byte)this.MipCount;

            using (var writer = new FileWriter(stream))
            {
                writer.WriteStruct(this.HeaderInfo);
                writer.SeekBegin(this.HeaderInfo.HeaderSize);

                List<uint> surfaceSizes = new List<uint>();
                List<byte[]> surfaceData = new List<byte[]>();

                //Surface index list
                for (int mip = 0; mip < this.MipCount; mip++)
                {
                    for (int array = 0; array < this.ArrayCount; array++)
                    {
                        writer.Write((ushort)array);
                        writer.Write((byte)mip);
                        writer.Write((byte)1);

                        var surface = Zstb.SCompress(ImageList[array][mip], 20);
                        surfaceSizes.Add((uint)surface.Length);

                        surfaceData.Add(surface);
                    }
                }

                //Surface sizes
                foreach (var surface in surfaceSizes)
                {
                    writer.Write(surface);
                    writer.Write(6);
                }

                //Surface data
                foreach (var data in surfaceData)
                {
                    writer.Write(data);
                }
            }
        }

        public void Dispose()
        {
            if (PluginRuntime.TextureCache.ContainsKey(FileName))
                PluginRuntime.TextureCache.Remove(FileName);
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            var data = ImageList[ArrayLevel][MipLevel];
            return TegraX1Swizzle.GetDirectImageData(this, data, MipLevel);
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            //Set the data using an instance of a switch texture
            var tex = new TextureData();
            tex.Texture = new Syroot.NintenTools.NSW.Bntx.Texture();
            tex.Format = this.Format;
            tex.Width = this.Width;
            tex.Height = this.Height;
            tex.MipCount = this.MipCount;
            tex.ArrayCount = this.ArrayCount;
            tex.Texture.TextureData = new List<List<byte[]>>();
            tex.Texture.TextureData.Add(new List<byte[]>());

            tex.SetImageData(bitmap, ArrayLevel);
            SetImage(tex, ArrayLevel);
        }

        public override void Replace(string FileName)
        {
            //Replace the data using an instance of a switch texture
            var tex = new TextureData();
            tex.Replace(FileName, MipCount, 0, Format, Syroot.NintenTools.NSW.Bntx.GFX.SurfaceDim.Dim2D, 1);

            //Get swappable array level
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            int targetArray = 0;
            if (editor != null)
                targetArray = editor.GetArrayDisplayLevel();

            SetImage(tex, targetArray);
        }

        private void SetImage(TextureData tex, int targetArray)
        {    
            //If it's null, the operation is cancelled
            if (tex.Texture == null)
                return;

            for (int i = 0; i < ImageList[0].Count; i++)
                Console.WriteLine($"SIZE 1 mip{i} {ImageList[0][i].Length}");

            //Ensure the format matches if image requires multiple surface levels
            if (ImageList.Count > 1 && this.Format != tex.Format)
                throw new Exception($"Imported texture must use the original format for surface injecting! Expected {this.Format} but got {tex.Format}! If you need ASTC, use an astc encoder with .astc file format.");

            //Swap individual image
            if (tex.Texture.TextureData.Count == 1)
            {
                ImageList[targetArray] = tex.Texture.TextureData[0];
            }
            else //Swap all surfaces if multiple are imported
            {
                ImageList.Clear();
                foreach (var surface in tex.Texture.TextureData)
                    ImageList.Add(surface);
            }

            for (int i = 0; i < ImageList[0].Count; i++)
                Console.WriteLine($"SIZE 2 mip{i} {ImageList[0][i].Length}");


            Width = tex.Texture.Width;
            Height = tex.Texture.Height;
            MipCount = tex.Texture.MipCount;
            ArrayCount = (uint)ImageList.Count;
            Format = tex.Format;

            IsEdited = true;

            UpdateEditor();

            this.LoadOpenGLTexture();
        }

        class SurfaceInfo
        {
            public byte MipLevel;
            public ushort ArrayLevel;
            public byte SurfaceCount; //Always 1

            public uint Size;
        }

        static Dictionary<uint, STChannelType> ChannelList = new Dictionary<uint, STChannelType>()
        {
            { 0, STChannelType.Red },
            { 1, STChannelType.Green },
            { 2, STChannelType.Blue },
            { 3, STChannelType.Alpha },
            { 4, STChannelType.Zero },
            { 5, STChannelType.One },
        };

        static Dictionary<ushort, TEX_FORMAT> FormatList = new Dictionary<ushort, TEX_FORMAT>()
        {
            { 0x101, TEX_FORMAT.ASTC_4x4_UNORM },
            { 0x102, TEX_FORMAT.ASTC_8x8_UNORM },
            { 0x105, TEX_FORMAT.ASTC_8x8_SRGB },
            { 0x109, TEX_FORMAT.ASTC_4x4_SRGB },
            { 0x202, TEX_FORMAT.BC1_UNORM },
            { 0x203, TEX_FORMAT.BC1_UNORM_SRGB },
            { 0x302, TEX_FORMAT.BC1_UNORM },

            
            { 0x505, TEX_FORMAT.BC3_UNORM_SRGB },
            { 0x602, TEX_FORMAT.BC4_UNORM },
            { 0x606, TEX_FORMAT.BC4_UNORM },
            { 0x607, TEX_FORMAT.BC4_UNORM },
            { 0x702, TEX_FORMAT.BC5_UNORM },
            { 0x703, TEX_FORMAT.BC5_UNORM },
            { 0x707, TEX_FORMAT.BC5_UNORM },
            { 0x901, TEX_FORMAT.BC7_UNORM },
        };

        public class DisplayProperties
        {
            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Height of the image.")]
            [Category("Image Info")]
            public uint Height { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Width of the image.")]
            [Category("Image Info")]
            public uint Width { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Format of the image.")]
            [Category("Image Info")]
            public TEX_FORMAT Format { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Mip map count of the image.")]
            [Category("Image Info")]
            public uint MipCount { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("Array count of the image for multiple surfaces.")]
            [Category("Image Info")]
            public uint ArrayCount { get; set; }

            [Browsable(true)]
            [ReadOnly(true)]
            [Description("The hash value.")]
            [Category("Image Info")]
            public string Hash { get; set; }
        }
    }
}
