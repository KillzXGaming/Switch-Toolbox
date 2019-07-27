using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.Drawing;

namespace FirstPlugin
{ 
    public class GITextureContainer : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GT1" };
        public string[] Extension { get; set; } = new string[] { "*.gt1" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "GT1G");
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

        public void Load(Stream stream)
        {
            Read(new FileReader(stream));
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
        }

        public void Read(FileReader reader)
        {
            long StartPos = reader.Position;
            string Magic = reader.ReadString(8);
            uint FileSize = reader.ReadUInt32();
            uint DataOffset = reader.ReadUInt32();
            uint TextureCount = reader.ReadUInt32();
            uint unk = reader.ReadUInt32();
            uint unk2 = reader.ReadUInt32();
            uint[] unk3s = reader.ReadUInt32s((int)TextureCount);

            for (int i = 0; i < TextureCount; i++)
            {
                reader.SeekBegin(StartPos + DataOffset + (i * 4));

                uint InfoOffset = reader.ReadUInt32();

                reader.SeekBegin(DataOffset + InfoOffset);
                byte unk4 = reader.ReadByte();
                byte format = reader.ReadByte();

            }
        }
    }

    public class GITexture : STGenericTexture
    {
        public override bool CanEdit { get; set; } = false;

        public byte[] ImageData;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
                    TEX_FORMAT.R8G8B8A8_UNORM,
                };
            }
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            return ImageData;
        }
    }

}
