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

namespace FirstPlugin
{
    public class TVOL : TreeNodeFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Texture VOL" };
        public string[] Extension { get; set; } = new string[] { "*.tvol" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".tvol";
        }

        public List<STGenericTexture> IconTextureList
        {
            get
            {
                List<STGenericTexture> textures = new List<STGenericTexture>();
                foreach (STGenericTexture node in Nodes)
                    textures.Add(node);

                return textures;
            }
            set { }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(false);

                uint numTextures = reader.ReadUInt32();
                for (int i = 0; i < numTextures; i++)
                {
                    uint offset = reader.ReadUInt32();
                    uint size = reader.ReadUInt32();
                    if (size == 0)
                        continue;

                    using (reader.TemporarySeek(offset, System.IO.SeekOrigin.Begin))
                    {
                        TextureWrapper entry = new TextureWrapper();
                        entry.ImageKey = "texture";
                        entry.SelectedImageKey = "texture";
                        entry.Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        entry.Text = reader.ReadZeroTerminatedString();

                        reader.SeekBegin(offset + 48);
                        ulong unk = reader.ReadUInt64(); //Varies. Shifts like a offset or size
                        ulong headerOffset = reader.ReadUInt64();
                        ulong sectionSize = reader.ReadUInt64();
                        ulong unk3 = reader.ReadUInt64(); //16
                        ulong unk4 = reader.ReadUInt64(); //4

                        reader.SeekBegin(offset + 48 + headerOffset);
                        reader.ReadUInt32(); //padding
                        uint unk5 = reader.ReadUInt32(); //C03F
                        ulong unk6 = reader.ReadUInt64(); //32

                        if (unk6 != 32)
                        {
                            //Platform PC
                            reader.Seek(-8);
                            uint unk7 = reader.ReadUInt32();
                            uint unk8 = reader.ReadUInt32();
                            uint unk9 = reader.ReadUInt32();
                            uint unk10 = reader.ReadUInt32();
                            entry.Width = reader.ReadUInt32();
                            entry.Height = reader.ReadUInt32();
                            entry.Depth = reader.ReadUInt32();
                            entry.ArrayCount = reader.ReadUInt32();
                            reader.Seek(44);
                            uint imageSize = reader.ReadUInt32();
                            reader.Seek(16);

                            entry.Parameters.DontSwapRG = true;
                            entry.ImageData = reader.ReadBytes((int)imageSize);
                        }
                        else
                        {
                            entry.PlatformSwizzle = PlatformSwizzle.Platform_Switch;

                            //Platform switch
                            ulong unk7 = reader.ReadUInt64(); //24
                            ulong unk8 = reader.ReadUInt64(); //40

                            //Matches XTX info header
                            uint imageSize = reader.ReadUInt32();
                            uint Alignment = reader.ReadUInt32();
                            entry.Width = reader.ReadUInt32();
                            entry.Height = reader.ReadUInt32();
                            entry.Depth = reader.ReadUInt32();
                            entry.Target = reader.ReadUInt32();
                            uint Format = reader.ReadUInt32();
                            uint unk10 = reader.ReadUInt32(); //1
                            entry.ImageData = reader.ReadBytes((int)imageSize);

                            entry.Format = XTX.TextureInfo.ConvertFormat(Format);
                        }

                        Nodes.Add(entry);
                    }
                }
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public class TextureWrapper : STGenericTexture
        {
            public uint Target = 1;

            public byte[] ImageData;

            public TextureWrapper()
            {
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

            public override void SetImageData(System.Drawing.Bitmap bitmap, int ArrayLevel)
            {

            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
            {
                if (PlatformSwizzle == PlatformSwizzle.Platform_Switch)
                    return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, (int)Target);
                else
                    return ImageData;
            }


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
        }
    }
}
