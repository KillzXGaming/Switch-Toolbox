using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using OpenTK;

namespace DKCTF
{
    /// <summary>
    /// Represents a helper for loading vertex and index buffer information.
    /// </summary>
    internal class BufferHelper
    {
        /// <summary>
        /// Gets an index list from the provided buffer and formatting info.
        /// </summary>
        public static uint[] LoadIndexBuffer(byte[] buffer, CMDL.IndexFormat format, bool isLittleEndian)
        {
            var stride = GetIndexStride(format);
            uint[] indices = new uint[buffer.Length / stride];

            using (var reader = new FileReader(buffer))
            {
                reader.SetByteOrder(!isLittleEndian); //switch is little endianness

                if (format == CMDL.IndexFormat.Uint16)
                {
                    for (int i = 0; i < indices.Length; i++)
                        indices[i] = reader.ReadUInt16();
                }
                else
                {
                    for (int i = 0; i < indices.Length; i++)
                        indices[i] = reader.ReadUInt32();
                }
            }
            return indices;
        }

        /// <summary>
        /// Gets a vertex list from the provided buffer and descriptor info.
        /// </summary>
        public static CMDL.CVertex[] LoadVertexBuffer(List<byte[]> buffers, int startIndex, CMDL.VertexBuffer vertexInfo, bool isLittleEndian, bool swapTexCoord)
        {
            var vertices = new CMDL.CVertex[vertexInfo.VertexCount];

            foreach (var comp in vertexInfo.Components)
            {
                var buffer = buffers[startIndex + (int)comp.BufferID];
                using (var reader = new FileReader(buffer))
                {
                    reader.SetByteOrder(!isLittleEndian); //switch is little endianness

                    for (int i = 0; i < vertexInfo.VertexCount; i++)
                    {
                        if (vertices[i] == null) vertices[i] = new CMDL.CVertex();

                        CMDL.CVertex vertex = vertices[i];

                        reader.SeekBegin(comp.Offset + i * comp.Stride);
                        switch (comp.Type)
                        {
                            case CMDL.EVertexComponent.in_position:
                                vertex.Position = ReadData(reader, comp.Format).Xyz;
                                break;
                            case CMDL.EVertexComponent.in_normal:
                                vertex.Normal = ReadData(reader, comp.Format).Xyz;
                                break;
                            case CMDL.EVertexComponent.in_texCoord0:
                                vertex.TexCoord0 = ReadData(reader, comp.Format).Xy;
                                break;
                            case CMDL.EVertexComponent.in_texCoord1:
                                if (swapTexCoord)
                                    vertex.TexCoord0 = ReadData(reader, comp.Format).Xy;
                                else
                                    vertex.TexCoord1 = ReadData(reader, comp.Format).Xy;
                                break;
                            case CMDL.EVertexComponent.in_texCoord2:
                                vertex.TexCoord2 = ReadData(reader, comp.Format).Xy;
                                break;
                            case CMDL.EVertexComponent.in_boneWeights:
                                vertex.BoneWeights = ReadData(reader, comp.Format);
                                break;
                            case CMDL.EVertexComponent.in_boneIndices:
                                vertex.BoneIndices = ReadData(reader, comp.Format);
                                break;
                            case CMDL.EVertexComponent.in_color:
                                vertex.Color = ReadData(reader, comp.Format);
                                break;
                            case CMDL.EVertexComponent.in_tangent0:
                                vertex.Tangent = ReadData(reader, comp.Format);
                                break;
                        }
                    }
                }
            }
            return vertices;
        }

        static Vector4 ReadData(FileReader reader, CMDL.VertexFormat format)
        {
            switch (format)
            {
                case CMDL.VertexFormat.Format_16_16_HalfSingle: return new Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(), 0, 0);
                case CMDL.VertexFormat.Format_32_32_32_Single: return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
                case CMDL.VertexFormat.Format_16_16_16_HalfSingle:  return new Vector4(
                    reader.ReadHalfSingle(),reader.ReadHalfSingle(),
                    reader.ReadHalfSingle(), reader.ReadHalfSingle());
                case CMDL.VertexFormat.Format_8_8_8_8_Uint:
                    return new Vector4(
                       reader.ReadByte(), reader.ReadByte(),
                       reader.ReadByte(), reader.ReadByte());
            }
            return new Vector4();
        }

        private static int GetIndexStride(CMDL.IndexFormat format)
        {
            if (format == CMDL.IndexFormat.Uint32) return 4;
            else return 2;
        }
    }
}
