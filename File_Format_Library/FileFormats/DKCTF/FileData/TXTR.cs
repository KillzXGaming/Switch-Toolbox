using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace DKCTF
{
    /// <summary>
    /// Represents a texture file format.
    /// </summary>
    internal class TXTR : FileForm
    {
        public STextureHeader TextureHeader;

        public SMetaData Meta;

        public byte[] BufferData;

        public uint[] MipSizes = new uint[0];

        public uint TextureSize { get; set; }

        public uint Unknown { get; set; }

        
        public bool IsSwitch => this.FileHeader.VersionA >= 0x0F;

        public TXTR() { }

        public TXTR(System.IO.Stream stream) : base(stream)
        {
        }

        public byte[] CreateUncompressedFile(byte[] fileData)
        {
            var mem = new MemoryStream();
            using (var writer = new FileWriter(mem))
            using (var reader = new FileReader(fileData))
            {
                writer.SetByteOrder(true);
                reader.SetByteOrder(true);

                FileHeader = reader.ReadStruct<CFormDescriptor>();
                ReadMetaFooter(reader);

                reader.Position = 0;
                byte[] textureInfo = reader.ReadBytes((int)Meta.GPUOffset + 24);

                long pos = reader.BaseStream.Position - 24;

                var buffer = Meta.BufferInfo[0];
                reader.Seek(buffer.Offset);

                byte[] BufferData = IOFileExtension.DecompressedBuffer(reader, buffer.CompressedSize, buffer.DecompressedSize, IsSwitch);

                writer.Write(textureInfo);
                writer.Write(BufferData);

                using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
                {
                    writer.Write((long)BufferData.Length);
                }

                return mem.ToArray();
            }
        }

        public override void ReadChunk(FileReader reader, CChunkDescriptor chunk)
        {
            switch (chunk.ChunkType)
            {
                case "HEAD":
                    TextureHeader = reader.ReadStruct<STextureHeader>();
                    uint numMips = reader.ReadUInt32();
                    MipSizes = reader.ReadUInt32s((int)numMips);
                    TextureSize = reader.ReadUInt32();
                    Unknown = reader.ReadUInt32();
                    break;
                case "GPU ":
                    if (Meta != null)
                    {
                        var buffer = Meta.BufferInfo[0];

                        reader.Seek(buffer.Offset);
                        BufferData = IOFileExtension.DecompressedBuffer(reader, buffer.CompressedSize, buffer.DecompressedSize, IsSwitch);
                    }
                    else
                    {
                        BufferData = reader.ReadBytes((int)chunk.DataSize); 
                    }
                    break;
            }
        }

        public override void ReadMetaData(FileReader reader)
        {
            Meta = new SMetaData();
            Meta.Unknown = reader.ReadUInt32();
            Meta.AllocCategory = reader.ReadUInt32();
            Meta.GPUOffset = reader.ReadUInt32();
            Meta.BaseAlignment = reader.ReadUInt32();
            Meta.GPUDataStart = reader.ReadUInt32();
            Meta.GPUDataSize = reader.ReadUInt32();
            Meta.BufferInfo = IOFileExtension.ReadList<SCompressedBufferInfo>(reader);
        }

        public override void WriteMetaData(FileWriter writer)
        {
            writer.Write(Meta.Unknown);
            writer.Write(Meta.AllocCategory);
            writer.Write(Meta.GPUOffset);
            writer.Write(Meta.BaseAlignment);
            writer.Write(Meta.GPUDataStart);
            writer.Write(Meta.GPUDataSize);
            IOFileExtension.WriteList(writer, Meta.BufferInfo);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class STextureHeader
        {
            public uint Type;
            public uint Format;
            public uint Width;
            public uint Height;
            public uint Depth;
            public uint TileMode;
            public uint Swizzle;
        }

        //Meta data from PAK archive
        public class SMetaData
        {
            public uint Unknown; //4
            public uint AllocCategory;
            public uint GPUOffset; 
            public uint BaseAlignment;
            public uint GPUDataStart;
            public uint GPUDataSize;
            public List<SCompressedBufferInfo> BufferInfo = new List<SCompressedBufferInfo>();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SCompressedBufferInfo
        {
            public uint DecompressedSize;
            public uint CompressedSize;
            public uint Offset;
        }
    }
}
