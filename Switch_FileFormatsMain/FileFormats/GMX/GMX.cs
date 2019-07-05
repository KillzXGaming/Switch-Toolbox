using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Rendering;

namespace FirstPlugin
{
    public class GMX : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GMX" };
        public string[] Extension { get; set; } = new string[] { "*.gmx" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "GMX2");
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

        public Header GMXHeader;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            GMXHeader = new Header();
            GMXHeader.Read(new FileReader(stream));
            for (int i = 0; i < GMXHeader.Meshes.Count; i++)
            {
                var node = new TreeNode($"Mesh ({i})");
                node.Tag = GMXHeader.Meshes[i];
                Nodes.Add(node);
            }
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            GMXHeader.Write(new FileWriter(mem));
            return mem.ToArray();
        }

        public class Header
        {
            public uint HeaderSize;

            public Header()
            {
                HeaderSize = 8;
            }

            public List<MESH> Meshes = new List<MESH>();

            public void Read(FileReader reader)
            {
                reader.ReadSignature(4, "GMX2");
                reader.SetByteOrder(true);
                HeaderSize = reader.ReadUInt32();

                while (reader.Position < reader.BaseStream.Length)
                {
                    long pos = reader.Position;

                    string Magic = reader.ReadString(4);
                    uint SectionSize = reader.ReadUInt32();

                    Console.WriteLine($"Magic {Magic}");
                    reader.SeekBegin(pos);
                    switch (Magic)
                    {
                        case "PADX":
                            PADX padding = new PADX();
                            padding.Read(reader);
                            break;
                        case "INDX":
                            INDX indexGrp = new INDX();
                            indexGrp.Read(reader);
                            GetLastMesh().IndexGroup = indexGrp;
                            break;
                        case "VMAP":
                            VMAP vmap = new VMAP();
                            vmap.Read(reader);
                            GetLastMesh().VMapGroup = vmap;
                            break;
                        case "MESH":
                            MESH mesh = new MESH();
                            mesh.Read(reader);
                            Meshes.Add(mesh);
                            break;
                        case "VERT":
                            VERT vert = new VERT();
                            vert.Read(reader, GetLastMesh());
                            GetLastMesh().VertexGroup = vert;
                            break;
                        case "ENDX":
                            reader.ReadSignature(4, "ENDX");
                            uint EndSectionSize = reader.ReadUInt32();
                            break;
                        default:
                            throw new Exception("Unknown section! " + Magic);
                    }

                    reader.SeekBegin(pos + SectionSize);
                }
            }

            public MESH GetLastMesh()
            {
                return Meshes[Meshes.Count - 1];
            }

            public void Write(FileWriter writer)
            {
                PADX padding = new PADX();

                writer.WriteSignature("GMX2");
                writer.Write(HeaderSize);
                for (int i = 0; i < Meshes.Count; i++)
                {
                    //Write mesh header
                    padding.Write(writer, 16);
                    Meshes[i].Write(writer);

                    //Write Vertices
                    padding.Write(writer, 64);
                    Meshes[i].VertexGroup.Write(writer, Meshes[i]);

                    //Write Faces
                    padding.Write(writer, 16);
                    Meshes[i].IndexGroup.Write(writer);

                    //Write VMAPS
                    padding.Write(writer, 16);
                    Meshes[i].VMapGroup.Write(writer);
                }
            }

            public class VERT
            {
                public List<Vertex> Vertices = new List<Vertex>();

                public void Read(FileReader reader, MESH mesh)
                {
                    reader.ReadSignature(4, "VERT");
                    uint SectionSize = reader.ReadUInt32();

                    if (mesh.VertexSize == 32)
                    {
                        for (int v = 0; v < mesh.VertexCount; v++)
                        {
                            Vertex vert = new Vertex();
                            vert.pos = reader.ReadVec3();
                            vert.nrm = reader.ReadVec3();
                            vert.uv0 = reader.ReadVec2();
                            Vertices.Add(vert);
                        }
                    }
                    else if (mesh.VertexSize == 36)
                    {
                        for (int v = 0; v < mesh.VertexCount; v++)
                        {
                            Vertex vert = new Vertex();
                            uint Unknown = reader.ReadUInt32(); //Bone index?
                            vert.pos = reader.ReadVec3();
                            vert.nrm = reader.ReadVec3();
                            vert.uv0 = reader.ReadVec2();
                            Vertices.Add(vert);

                            Console.WriteLine($"Unknown {Unknown}");
                        }
                    }
                    else
                        throw new Exception($"Unsupported Vertex Size {mesh.VertexSize}");
                }

                public void Write(FileWriter writer, MESH mesh)
                {
                    writer.WriteSignature("VERT");
                    writer.Write(Vertices.Count * mesh.VertexSize);
                    for (int v = 0; v < mesh.VertexCount; v++)
                    {
                        if (mesh.VertexSize == 32)
                        {
                            writer.Write(Vertices[v].pos);
                            writer.Write(Vertices[v].nrm);
                            writer.Write(Vertices[v].uv0);
                        }
                        else if (mesh.VertexSize == 36)
                        {
                            writer.Write(0);
                            writer.Write(Vertices[v].pos);
                            writer.Write(Vertices[v].nrm);
                            writer.Write(Vertices[v].uv0);
                        }
                        else
                            throw new Exception($"Unsupported Vertex Size {mesh.VertexSize}");
                    }
                }
            }

            public class MESH
            {
                public INDX IndexGroup { get; set; }
                public VERT VertexGroup { get; set; }
                public VMAP VMapGroup { get; set; }

                public ushort VertexSize { get; set; }
                public ushort VertexCount { get; set; }
                public uint FaceCount { get; set; }

                public void Read(FileReader reader)
                {
                    reader.ReadSignature(4, "MESH");
                    uint SectionSize = reader.ReadUInt32();
                    uint Padding = reader.ReadUInt32();
                    VertexSize = reader.ReadUInt16();
                    VertexCount = reader.ReadUInt16();
                    uint Padding2 = reader.ReadUInt32();
                    FaceCount = reader.ReadUInt32();
                    uint Padding3 = reader.ReadUInt32();
                    uint Padding4 = reader.ReadUInt32();
                    uint Padding5 = reader.ReadUInt32();
                    uint Padding6 = reader.ReadUInt32();
                }

                public void Write(FileWriter writer)
                {
                    writer.WriteSignature("MESH");
                    writer.Write(0);
                    writer.Write(VertexSize);
                    writer.Write(VertexCount);
                    writer.Write(0);
                    writer.Write(FaceCount);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                }
            }

            public class INDX
            {
                public ushort[] Indices;

                public void Read(FileReader reader)
                {
                    reader.ReadSignature(4, "INDX");
                    uint SectionSize = reader.ReadUInt32();
                    uint FaceCount = (SectionSize - 8) / sizeof(ushort);

                    Indices = new ushort[FaceCount];
                    for (int i = 0; i < FaceCount; i++)
                    {
                        Indices[i] = reader.ReadUInt16();
                    }
                }

                public void Write(FileWriter writer)
                {
                    writer.WriteSignature("INDX");
                    writer.Write(Indices.Length * sizeof(ushort) - 8);
                    for (int i = 0; i < Indices.Length; i++)
                        writer.Write(Indices[i]);
                }
            }

            public class VMAP
            {
                public ushort[] Indices;

                public void Read(FileReader reader)
                {
                    reader.ReadSignature(4, "VMAP");
                    uint SectionSize = reader.ReadUInt32();
                    uint FaceCount = (SectionSize - 8) / sizeof(ushort);

                    Indices = new ushort[FaceCount];
                    for (int i = 0; i < FaceCount; i++)
                    {
                        Indices[i] = reader.ReadUInt16();
                    }
                }

                public void Write(FileWriter writer)
                {
                    writer.WriteSignature("VMAP");
                    writer.Write(Indices.Length * sizeof(ushort) - 8);
                    for (int i = 0; i < Indices.Length; i++)
                        writer.Write(Indices[i]);
                }
            }

            public class PADX
            {
                public void Read(FileReader reader)
                {
                    reader.ReadSignature(4, "PADX");
                    uint SectionSize = reader.ReadUInt32();
                }

                public void Write(FileWriter writer, uint Alignment)
                {
                    long pos = writer.Position;
                    writer.WriteSignature("PADX");
                    writer.Write(uint.MaxValue);
                    Align(writer, (int)Alignment);

                    long endPos = writer.Position;
                    writer.Seek(pos + 4, System.IO.SeekOrigin.Begin);
                    writer.Write((uint)(endPos - pos));
                }

                private void Align(FileWriter writer, int alignment)
                {
                    var startPos = writer.Position;
                    long position = writer.Seek((-writer.Position % alignment + alignment) % alignment, System.IO.SeekOrigin.Current);

                    writer.Seek(startPos, System.IO.SeekOrigin.Begin);
                    while (writer.Position != position)
                    {
                        writer.Write(byte.MaxValue);
                    }
                }
            }
        }
    }
}
