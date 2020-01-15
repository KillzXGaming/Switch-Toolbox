using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FirstPlugin
{
    class TXE : STGenericTexture, IFileFormat, ISingleTextureIconLoader
    {
        public STGenericTexture IconTexture { get { return this; } }

        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }

        public string[] Description { get; set; } = new string[] { "Pikmin 1 Proprietary Texture" };
        public string[] Extension { get; set; } = new string[] { "*.txe" };
        public string FileName { get; set; }
        public string FilePath { get; set; }

        //Stores compression info from being opened (done automaitcally)
        public IFileInfo IFileInfo { get; set; }

        //Check how the file wil be opened
        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".txe");
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

        public static Dictionary<ushort, TEX_FORMAT> FormatsTXE = new Dictionary<ushort, TEX_FORMAT>()
        {
            [0] = TEX_FORMAT.RGB5A3,
            [1] = TEX_FORMAT.CMPR,
            [2] = TEX_FORMAT.RGB565,
            [3] = TEX_FORMAT.I4,
            [4] = TEX_FORMAT.I8,
            [5] = TEX_FORMAT.IA4,
            [6] = TEX_FORMAT.IA8,
            [7] = TEX_FORMAT.RGBA32,
        };

        public short Unknown;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            //Set this if you want to save the file format
            CanSave = true;
            CanReplace = true;

            ImageKey = "Texture";
            SelectedImageKey = "Texture";

            //You can add a FileReader with Toolbox.Library.IO namespace
            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                Unknown = reader.ReadInt16();
                //Turn this format into a common format used by this tool
                ushort texFormat = reader.ReadUInt16();
                Format = FormatsTXE[texFormat];
                int ImageSize = reader.ReadInt32(); //Unsure

                //Lets set our method of decoding
                PlatformSwizzle = PlatformSwizzle.Platform_Gamecube;

                reader.Align(32);

                //Calculate size automatically if ImageSize is 0
                if (ImageSize == 0)
                    ImageSize = (int)reader.BaseStream.Length - (int)reader.Position;

                ImageData = reader.ReadBytes(ImageSize);

                Text = FileName;
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.Write((ushort)Width);
                writer.Write((ushort)Height);
                writer.Write(Unknown);
                ushort format = FormatsTXE.FirstOrDefault(x => x.Value == Format).Key;
                writer.Write(format);
                writer.Write(ImageData.Length);
                writer.Align(32);
                writer.Write(ImageData);
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
                    TEX_FORMAT.IA4,
                    TEX_FORMAT.IA8,
                    TEX_FORMAT.RGB565,
                    TEX_FORMAT.RGB5A3,
                    TEX_FORMAT.RGBA32,
                    TEX_FORMAT.CMPR,
                };
            }
        }

        public Decode_Gamecube.TextureFormats DolphinTextureFormat
        {
            get { return Decode_Gamecube.FromGenericFormat(Format); }
        }

        public override bool CanEdit { get; set; } = true;

        //This gets used in the image editor if the image gets edited
        //This wll not be ran if "CanEdit" is set to false!
        public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
        {
            this.Width = (ushort)bitmap.Width;
            this.Height = (ushort)bitmap.Height;

            this.ImageData = Decode_Gamecube.EncodeFromBitmap(bitmap, DolphinTextureFormat).Item1;
        }

        //Gets the raw image data in bytes
        //Gets decoded automatically
        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            return Decode_Gamecube.GetMipLevel(ImageData, Width, Height, MipCount, (uint)MipLevel, Format);
        }


        //This is an event for when the tree is clicked on
        //Load our editor
        public override void OnClick(TreeView treeView) {
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
            this.Depth = settings.Depth;
            this.ArrayCount = (uint)settings.DataBlockOutput.Count;
        }
    }
}
