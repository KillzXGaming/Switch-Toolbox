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

        public byte[] CreateUncompressedFile(byte[] fileData, CFormDescriptor pakHeader, bool isLittleEndian)
        {
            var mem = new MemoryStream();
            using (var writer = new FileWriter(mem))
            using (var reader = new FileReader(fileData))
            {
                reader.SetByteOrder(!isLittleEndian);
                writer.SetByteOrder(!isLittleEndian);

                FileHeader = reader.ReadStruct<CFormDescriptor>();
                ReadMetaFooter(reader);

                //Rewrite header for saving uncompressed file
                reader.Position = 0;
                byte[] textureInfo = reader.ReadBytes((int)Meta.GPUOffset + 24);

                long pos = reader.BaseStream.Position - 24;

                List<byte[]> combinedBuffer = new List<byte[]>();

                byte[] textureData = new byte[Meta.DecompressedSize];

                if (pakHeader.VersionA >= 1 && pakHeader.VersionB >= 1)
                {
                    //Decompress all buffers
                    foreach (var info in Meta.TextureInfo)
                    {
                        var buff = Meta.BufferInfo[info.Index];

                        //Go to the buffer
                        reader.SeekBegin(info.StartOffset + buff.StartOffset);
                        //Decompress the buffer
                        byte[] BufferData = IOFileExtension.DecompressedBuffer(reader, buff.CompressedSize, buff.DestSize, IsSwitch || isLittleEndian);
                        combinedBuffer.Add(BufferData);

                        Array.Copy(BufferData, 0, textureData, (int)buff.DestOffset, (int)buff.DestSize);
                    }
                }
                else
                {
                    var buffer = Meta.BufferInfoV1[0];
                    reader.SeekBegin(Meta.GPUDataStart + buffer.Offset);

                    textureData = IOFileExtension.DecompressedBuffer(reader, buffer.CompressedSize, buffer.DecompressedSize, IsSwitch);
                }

                writer.Write(textureInfo);
                writer.Write(textureData);

                using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin)) {
                    writer.Write((long)textureData.Length);
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
                        reader.SeekBegin(buffer.StartOffset);
                        BufferData = IOFileExtension.DecompressedBuffer(reader, (uint)buffer.CompressedSize, (uint)buffer.DestSize, IsSwitch);
                    }
                    else
                    {
                        BufferData = reader.ReadBytes((int)chunk.DataSize); 
                    }
                    break;
            }
        }

        public override void ReadMetaData(FileReader reader, CFormDescriptor pakVersion)
        {
            Meta = new SMetaData();
            // MPR
            if (pakVersion.VersionA >= 1 && pakVersion.VersionB >= 1)
            {
                reader.ReadUInt32(); //Extra uint in MPR
                Meta.Unknown = reader.ReadUInt32();
                Meta.AllocCategory = reader.ReadUInt32();
                Meta.GPUOffset = reader.ReadUInt32();
                Meta.BaseAlignment = reader.ReadUInt32();
                Meta.DecompressedSize = reader.ReadUInt32(); //total decomp size
                Meta.TextureInfo = IOFileExtension.ReadList<STextureInfo>(reader);
                Meta.BufferInfo = IOFileExtension.ReadList<SCompressedBufferInfo>(reader);
            }
            else
            {
                Meta.Unknown = reader.ReadUInt32();
                Meta.AllocCategory = reader.ReadUInt32();
                Meta.GPUOffset = reader.ReadUInt32();
                Meta.BaseAlignment = reader.ReadUInt32();
                Meta.GPUDataStart = reader.ReadUInt32();
                Meta.GPUDataSize = reader.ReadUInt32();
                Meta.BufferInfoV1 = IOFileExtension.ReadList<SCompressedBufferInfoV1>(reader);
            }
            Console.WriteLine();
        }

        public override void WriteMetaData(FileWriter writer, CFormDescriptor pakVersion)
        {
            if (pakVersion.VersionA >= 1 && pakVersion.VersionB >= 1)
            {
                writer.Write(Meta.Unknown);
                writer.Write(0);
                writer.Write(Meta.AllocCategory);
                writer.Write(Meta.GPUOffset);
                writer.Write(Meta.BaseAlignment);
                writer.Write(Meta.DecompressedSize);
                IOFileExtension.WriteList(writer, Meta.TextureInfo);
                IOFileExtension.WriteList(writer, Meta.BufferInfo);
            }
            else
            {
                writer.Write(Meta.Unknown);
                writer.Write(Meta.AllocCategory);
                writer.Write(Meta.GPUOffset);
                writer.Write(Meta.BaseAlignment);
                writer.Write(Meta.GPUDataStart);
                writer.Write(Meta.GPUDataSize);
                IOFileExtension.WriteList(writer, Meta.BufferInfoV1);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class STextureHeader
        {
            public uint Type; //1 = 2D 3 = Cubemap
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
            public uint Unknown2;
            public uint AllocCategory;
            public uint GPUOffset;
            public uint GPUDataStart;
            public uint GPUDataSize;
            public uint BaseAlignment;
            public uint DecompressedSize;
            public List<STextureInfo> TextureInfo = new List<STextureInfo>();
            public List<SCompressedBufferInfo> BufferInfo = new List<SCompressedBufferInfo>();
            public List<SCompressedBufferInfoV1> BufferInfoV1 = new List<SCompressedBufferInfoV1>();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class STextureInfo
        {
            public byte Index;
            public uint StartOffset;
            public uint EndOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SCompressedBufferInfo
        {
            public uint Index;
            public uint StartOffset;
            public uint CompressedSize;
            public uint DestOffset;
            public uint DestSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SCompressedBufferInfoV1
        {
            public uint DecompressedSize;
            public uint CompressedSize;
            public uint Offset;
        }
    }
}
