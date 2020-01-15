using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace FirstPlugin
{
    class BTI : STGenericTexture, IFileFormat, ISingleTextureIconLoader
    {
        public STGenericTexture IconTexture { get { return this; } }

        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }

        public string[] Description { get; set; } = new string[] { "Binary Texture Image" };
        public string[] Extension { get; set; } = new string[] { "*.bti" };
        public string FileName { get; set; }
        public string FilePath { get; set; }

        //Stores compression info from being opened (done automaitcally)
        public IFileInfo IFileInfo { get; set; }

        //Check how the file wil be opened
        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".bti");
        }

        //A Type list for custom types
        //With this you can add in classes with IFileMenuExtension to add menus for this format
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        private void Read(System.IO.Stream stream)
        {
            
        }

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            //Set this if you want to save the file format
            CanSave = true;
            CanEdit = true;
            CanReplace = true;

            ImageKey = "Texture";
            SelectedImageKey = "Texture";
            Text = FileName;

            //You can add a FileReader with Toolbox.Library.IO namespace
            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                reader.Position = 0;
                header = reader.ReadStruct<Header>();

                //Turn this format into a common format used by this tool
                Format = Decode_Gamecube.ToGenericFormat((Decode_Gamecube.TextureFormats)header.Format);
                Width = header.Width;
                Height = header.Height;
                MipCount = header.MipCount;
                var paletteFormat = (Decode_Gamecube.PaletteFormats)header.PaletteFormat;

                reader.SeekBegin(header.DataOffset);
                uint imageDataSize = header.PaletteOffset - header.DataOffset;
                if (header.PaletteOffset == 0)
                    imageDataSize = (uint)reader.BaseStream.Length - header.DataOffset;

                ImageData = reader.ReadBytes((int)imageDataSize);

                if (header.PaletteOffset != 0)
                {
                    reader.SeekBegin(header.PaletteOffset);
                    byte[] PaletteData = reader.ReadBytes((int)header.PaletteEntryCount * 2);
                    SetPaletteData(PaletteData, Decode_Gamecube.ToGenericPaletteFormat(paletteFormat));
                }
                else
                    SetPaletteData(new byte[0], PALETTE_FORMAT.RGB565);

                //Lets set our method of decoding
                PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Header
        {
            public byte Format;
            public byte AlphaEnabled;
            public ushort Width;
            public ushort Height;
            public byte WrapS;
            public byte WrapT;
            public byte Unknown;
            public byte PaletteFormat;
            public ushort PaletteEntryCount;
            public uint PaletteOffset;
            public uint BorderColor;
            public byte MinFilter;
            public byte MagFilter;
            public short Unknown2;
            public byte MipCount;
            public byte Unknown3;
            public short LodBias;
            public uint DataOffset = 32;
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                byte[] paletteData = GetPaletteData() != null ? GetPaletteData() : new byte[0];

                //Convert current header format and set the generic properties
                header.Format = (byte)Decode_Gamecube.FromGenericFormat(Format);
                header.PaletteFormat = (byte)Decode_Gamecube.FromGenericPaletteFormat(PaletteFormat);
                header.Width = (ushort)Width;
                header.Height = (ushort)Height;
                header.PaletteEntryCount = (ushort)(paletteData.Length / 2);

                //After header and image data
                header.PaletteOffset = header.PaletteEntryCount != 0 ? (uint)(32 + ImageData.Length) : 0;

                writer.SetByteOrder(true);
                writer.WriteStruct(header);
                writer.Write(ImageData);
                writer.Write(paletteData);
            }
        }

        public void Unload()
        {

        }

        public byte[] ImageData { get; set; }

        //A list of supported formats
        //This gets used in the re encode option
        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.I4,
                    TEX_FORMAT.I8,
                    TEX_FORMAT.I4,
                    TEX_FORMAT.I8,
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

        public override bool CanEdit { get; set; } = false;

        //This gets used in the image editor if the image gets edited
        //This wll not be ran if "CanEdit" is set to false!
        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {

        }

        //Gets the raw image data in bytes
        //Gets decoded automatically
        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return Decode_Gamecube.GetMipLevel(ImageData, Width, Height, MipCount, (uint)MipLevel, Format);
        }


        //This is an event for when the tree is clicked on
        //Load our editor
        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }

        public void UpdateEditor()
        {
            //Here we check for an active editor and load a new one if not found
            //This is used when a tree/object editor is used
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }

            //Load our image and any properties
            //If you don't make a class for properties you can use a generic class provided in STGenericTexture
            editor.LoadProperties(GenericProperties);
            editor.LoadImage(this);
        }

        public override void Replace(string FileName)
        {
            GamecubeTextureImporterList importer = new GamecubeTextureImporterList(SupportedFormats);
            GameCubeTextureImporterSettings settings = new GameCubeTextureImporterSettings();

            importer.ForceMipCount = true;
            importer.SelectedMipCount = 1;

            if (Utils.GetExtension(FileName) == ".dds" ||
                Utils.GetExtension(FileName) == ".dds2")
            {
                settings.LoadDDS(FileName);
                importer.LoadSettings(new List<GameCubeTextureImporterSettings>() { settings, });

                ApplySettings(settings);
                UpdateEditor();
            }
            else
            {
                settings.LoadBitMap(FileName);
                importer.LoadSettings(new List<GameCubeTextureImporterSettings>() { settings, });

                if (importer.ShowDialog() == DialogResult.OK)
                {
                    if (settings.GenerateMipmaps && !settings.IsFinishedCompressing)
                        settings.Compress();

                    ApplySettings(settings);
                    UpdateEditor();
                }
            }
        }

        private void ApplySettings(GameCubeTextureImporterSettings settings)
        {
            this.ImageData = settings.DataBlockOutput[0];
            this.Width = settings.TexWidth;
            this.Height = settings.TexHeight;
            this.Format = settings.GenericFormat;
            this.MipCount = 1; //Always 1
            this.Depth = 1;
            this.ArrayCount = (uint)settings.DataBlockOutput.Count;
        }
    }
}
