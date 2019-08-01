using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class CMB : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CMB" };
        public string[] Extension { get; set; } = new string[] { "*.cmb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "cmb ");
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

        public Header header;
        STTextureFolder texFolder;

        public void Load(System.IO.Stream stream)
        {
            header = new Header();
            header.Read(new FileReader(stream));

            Text = header.Name;

            //Load textures
            if (header.SectionData.TextureChunk != null)
            {
                texFolder = new TextureFolder("Texture");
                Nodes.Add(texFolder);

                int texIndex = 0;
                foreach (var tex in header.SectionData.TextureChunk.Textures)
                {
                    var texWrapper = new CTXB.TextureWrapper();
                    texWrapper.Text = $"Texture {texIndex++}";
                    texWrapper.ImageKey = "texture";
                    texWrapper.SelectedImageKey = texWrapper.ImageKey;

                    if (tex.Name != string.Empty)
                        texWrapper.Text = tex.Name;

                    texWrapper.Width = tex.Width;
                    texWrapper.Height = tex.Height;
                    texWrapper.Format = CTR_3DS.ConvertPICAToGenericFormat(tex.PicaFormat);
                    texWrapper.ImageData = tex.ImageData;
                    texFolder.Nodes.Add(texWrapper);
                }
            }
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public enum CMBVersion
        {
            OOT3DS,
            MM3DS,
            LM3DS,
        }

        private class TextureFolder : STTextureFolder, ITextureIconLoader
        {
            public List<STGenericTexture> IconTextureList
            {
                get
                {
                    List<STGenericTexture> textures = new List<STGenericTexture>();
                    foreach (STGenericTexture node in Nodes)
                        textures.Add(node);

                    return textures;
                }
                set {}
            }

            public TextureFolder(string text) : base(text)
            {

            }
        }

        public class Header
        {
            public string Name { get; set; }

            public CMBVersion Version;

            public uint ChunkCount; //Fixed count per game

            public uint Unknown;

            public SectionData SectionData;

            public void Read(FileReader reader)
            {
                string magic = reader.ReadSignature(4, "cmb ");
                uint FileSize = reader.ReadUInt32();
                ChunkCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();

                Name = reader.ReadString(0x10).TrimEnd('\0');

                //Check the chunk count used by the game
                if (ChunkCount == 0x0F)
                    Version = CMBVersion.LM3DS;
                else if (ChunkCount == 0x0A)
                    Version = CMBVersion.MM3DS;
                else if (ChunkCount == 0x06)
                    Version = CMBVersion.OOT3DS;
                else
                    throw new Exception("Unexpected chunk count! " + ChunkCount);

                SectionData = new SectionData();
                SectionData.Read(reader, this);
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("cmb ");
                writer.Write(uint.MaxValue); //Reserve space for file size offset
                writer.Write(ChunkCount);
                writer.Write(Unknown);
                writer.WriteString(Name, 0x10);

                SectionData.Write(writer, this);

                //Write the total file size
                using (writer.TemporarySeek(4, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }
        }

        public class SectionData
        {
            public SkeletonChunk SkeletonChunk;
            public QuadTreeChunk QuadTreeChunk;
            public MaterialChunk MaterialChunk;

            public TextureChunk TextureChunk;
            public SkeletalMeshChunk SkeletalMeshChunk;
            public LUTSChunk LUTSChunk;
            public VertexAttributesChunk VertexAttributesChunk;

            public void Read(FileReader reader, Header header)
            {
                uint numIndices = reader.ReadUInt32();
                SkeletonChunk = ReadChunkSection<SkeletonChunk>(reader, header);
                if (header.Version >= CMBVersion.MM3DS)
                    QuadTreeChunk = ReadChunkSection<QuadTreeChunk>(reader, header);

                MaterialChunk = ReadChunkSection<MaterialChunk>(reader, header);
                TextureChunk = ReadChunkSection<TextureChunk>(reader, header);
                SkeletalMeshChunk = ReadChunkSection<SkeletalMeshChunk>(reader, header);
                LUTSChunk = ReadChunkSection<LUTSChunk>(reader, header);
                VertexAttributesChunk = ReadChunkSection<VertexAttributesChunk>(reader, header);

                uint indexBufferOffset = reader.ReadUInt32();
                uint textureDataOffset = reader.ReadUInt32();

                if (header.Version >= CMBVersion.MM3DS)
                    reader.ReadUInt32(); //Padding?

                foreach (var tex in TextureChunk.Textures)
                {
                    reader.SeekBegin(textureDataOffset + tex.DataOffset);
                    tex.ImageData = reader.ReadBytes((int)tex.ImageSize);
                }
            }

            public void Write(FileWriter writer, Header header)
            {

            }
        }

        public class SkeletalMeshChunk : IChunkCommon
        {
            private const string Magic = "sklm";

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class LUTSChunk : IChunkCommon
        {
            private const string Magic = "luts";

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class VertexAttributesChunk : IChunkCommon
        {
            private const string Magic = "vatr";

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }



        public class SkeletonChunk : IChunkCommon
        {
            private const string Magic = "skl ";

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class QuadTreeChunk : IChunkCommon
        {
            private const string Magic = "qtrs";

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class MaterialChunk : IChunkCommon
        {
            private const string Magic = "mats";

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class TextureChunk : IChunkCommon
        {
            private const string Magic = "tex ";

            public List<CTXB.Texture> Textures = new List<CTXB.Texture>();

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint TextureCount = reader.ReadUInt32();
                for (int i = 0; i < TextureCount; i++)
                    Textures.Add(new CTXB.Texture(reader));
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                for (int i = 0; i < Textures.Count; i++)
                    Textures[i].Write(writer);

                //Write the total file size
                writer.WriteSectionSizeU32(pos + 4, pos, writer.Position);
            }
        }

        public static T ReadChunkSection<T>(FileReader reader, Header header)
             where T : IChunkCommon, new()
        {
            long pos = reader.Position;

            //Read offset and seek it
            uint offset = reader.ReadUInt32();
            reader.SeekBegin(offset);

            //Create chunk instance
            T chunk = new T();
            chunk.Read(reader, header);

            //Seek back and shift 4 from reading offset
            reader.SeekBegin(pos + 0x4);
            return chunk;
        }

        public interface IChunkCommon
        {
            void Read(FileReader reader, Header header);
            void Write(FileWriter writer, Header header);
        }
    }
}
