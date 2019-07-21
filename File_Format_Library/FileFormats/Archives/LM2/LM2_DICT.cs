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
using System.Drawing;

namespace FirstPlugin
{
    //Parse info based on https://github.com/TheFearsomeDzeraora/LM2L
    public class LM2_DICT : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Luigi's Mansion 2 Dark Moon Archive Dictionary" };
        public string[] Extension { get; set; } = new string[] { "*.dict" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                return reader.ReadUInt32() == 0x5824F3A9;
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

        public List<FileEntry> files = new List<FileEntry>();

        public bool IsCompressed = false;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                uint Identifier = reader.ReadUInt32();
                ushort Unknown = reader.ReadUInt16(); //Could also be 2 bytes, not sure. Always 0x0401
                IsCompressed = reader.ReadByte() == 1;
                reader.ReadByte(); //Padding
                uint FileCount = reader.ReadUInt32();
                uint LargestCompressedFile = reader.ReadUInt32();

                reader.SeekBegin(0x2C);
                byte[] Unknowns = reader.ReadBytes((int)FileCount);
                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry(this);
                    file.Read(reader);
                    if (file.FileType != 0)
                    {
                        file.Text = $"File {i} (Unknowns {file.FileType}  {file.Unknown2}  {file.Unknown3})";
                        files.Add(file);
                    }
                }

                //Now go through each file and format and connect the headers and blocks
                uint ImageIndex = 0;

                List<TexturePOWE> Textures = new List<TexturePOWE>();

                TreeNode textureFolder = new TreeNode("Textures");

                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].FileType == FileEntry.FileDataType.Texture)
                    {
                        if (files[i].Unknown3 == 1) //Info
                        {
                            //Read the info
                            using (var textureReader = new FileReader(files[i].FileData))
                            {
                                while (textureReader.ReadUInt32() == TexturePOWE.Identifier)
                                {
                                    var texture = new TexturePOWE();
                                    texture.ImageKey = "texture";
                                    texture.SelectedImageKey = texture.ImageKey;
                                    texture.Index = ImageIndex;
                                    texture.Read(textureReader);
                                    texture.Text = $"Texture {ImageIndex}";
                                    textureFolder.Nodes.Add(texture);
                                    Textures.Add(texture);
                                }
                            }
                        }
                        else //Block
                        {
                            uint Offset = 0;
                            foreach (var tex in Textures)
                            {
                                if (tex.Index == ImageIndex)
                                {
                                    tex.ImageData = Utils.SubArray(files[i].FileData, Offset, tex.ImageSize);
                                }

                                Offset += tex.ImageSize;
                            }
                            ImageIndex++;
                        }
                    }
                    else
                        Nodes.Add(files[i]);
                }

                if (textureFolder.Nodes.Count > 0)
                    Nodes.Add(textureFolder);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public class TexturePOWE : STGenericTexture
        {
            public static readonly uint Identifier = 0xE977D350;

            public uint Index { get; set; }

            public uint ID { get; set; }
            public uint ImageSize { get; set; }
            public uint ID2 { get; set; }

            public byte[] ImageData { get; set; }

            public void Read(FileReader reader)
            {
                PlatformSwizzle = PlatformSwizzle.Platform_3DS;

                ID = reader.ReadUInt32();
                ImageSize = reader.ReadUInt32();
                ID2 = reader.ReadUInt32();
                reader.Seek(0x8);
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                reader.Seek(3);
                var numMips = reader.ReadByte();
                reader.Seek(0x14);
                byte FormatCtr = reader.ReadByte();
                reader.Seek(3);

                MipCount = 1;
                Format = CTR_3DS.ConvertPICAToGenericFormat((CTR_3DS.PICASurfaceFormat)FormatCtr);
            }

            public override void OnClick(TreeView treeview)
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;

                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.LoadProperties(this.GenericProperties);
                editor.LoadImage(this);
            }

            public override bool CanEdit { get; set; } = false;

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                throw new NotImplementedException();
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return ImageData;
            }

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
        }

        public class FileEntry : TreeNodeCustom
        {
            public LM2_DICT ParentDictionary { get; set; }

            public uint Offset;
            public uint DecompressedSize;
            public uint CompressedSize;
            public FileDataType FileType; 
            public byte Unknown2;
            public byte Unknown3; //Possibly the effect? 0 for image block, 1 for info

            public enum FileDataType : ushort
            {
                Texture = 0x80,
            }

            public byte[] FileData
            {
                get { return GetData(); }
                set
                {

                }
            }

            public override void OnClick(TreeView treeview)
            {
                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Dock = DockStyle.Fill;
                editor.LoadData(FileData);
            }

            public FileEntry(LM2_DICT dict)
            {
                ParentDictionary = dict;
            }

            public void Read(FileReader reader)
            {
                Offset = reader.ReadUInt32();
                DecompressedSize = reader.ReadUInt32();
                CompressedSize = reader.ReadUInt32();
                FileType = reader.ReadEnum<FileDataType>(false);
                Unknown2 = reader.ReadByte();
                Unknown3 = reader.ReadByte();
            }

            private bool IsTextureBinary()
            {
                byte[] Data = GetData();

                if (Data.Length < 4)
                    return false;

                using (var reader = new FileReader(Data))
                {
                    return reader.ReadUInt32() == 0xE977D350;
                }
            }

            private byte[] GetData()
            {
                byte[] Data = new byte[DecompressedSize];

                string FolderPath = System.IO.Path.GetDirectoryName(ParentDictionary.FilePath);
                string DataFile = System.IO.Path.Combine(FolderPath, $"{ParentDictionary.FileName.Replace(".dict", ".data")}");

                if (System.IO.File.Exists(DataFile))
                {
                    using (var reader = new FileReader(DataFile))
                    {
                        reader.SeekBegin(Offset);
                        if (ParentDictionary.IsCompressed)
                        {
                            ushort Magic = reader.ReadUInt16();
                            reader.SeekBegin(Offset);

                            Data = reader.ReadBytes((int)CompressedSize);
                            if (Magic == 0x9C78 || Magic == 0xDA78)
                                return STLibraryCompression.ZLIB.Decompress(Data);
                            else //Unknown compression 
                                return Data;
                        }
                        else
                        {
                            return reader.ReadBytes((int)DecompressedSize);
                        }
                    }
                }

                return Data;
            }
        }
    }
}
