using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin.PunchOutWii
{
    public class PO_Mesh
    {
        public uint[] BoneHashes;

        public enum PolygonType : byte
        {
            Triangles = 0,
            TriangleStrips = 1,
        }

        public uint IndexStartOffset;
        public ushort IndexFormat;
        public uint IndexCount;
        public ushort VertexCount;
        public byte Unknown;
        public byte NumAttributePointers;

        public uint HashID;
        public uint MaterialHashID;

        //Only on gamecube (wii uses seperate section)
        public uint TexturHashID;

        public uint MaterialOffset;

        public PolygonType FaceType = PolygonType.TriangleStrips;

        public MaterailPresets MaterailPreset;

        public PO_Mesh(FileReader reader)
        {
            IndexStartOffset = reader.ReadUInt32();
            uint indexFlags = reader.ReadUInt32();
            IndexCount = (indexFlags & 0xffffff);
            IndexFormat = (ushort)(indexFlags >> 24);

            VertexCount = reader.ReadUInt16();
            Unknown = reader.ReadByte();
            NumAttributePointers = reader.ReadByte();
            reader.ReadUInt32();
            MaterialHashID = reader.ReadUInt32();
            HashID = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            MaterialOffset = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();

            MaterailPreset = (MaterailPresets)MaterialHashID;
        }
    }
}
