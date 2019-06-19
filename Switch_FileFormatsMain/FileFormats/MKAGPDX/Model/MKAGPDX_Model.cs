using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using OpenTK;

namespace FirstPlugin
{
    public class MKAGPDX_Model : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Mario Kart Arcade GP DX" };
        public string[] Extension { get; set; } = new string[] { "*.bin" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "BIKE");
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

        Header header;

        public void Load(System.IO.Stream stream)
        {
            header = new Header();
            header.Read(new FileReader(stream), this);
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public class Header
        {
            public uint Version { get; set; }
            public uint Alignment { get; set; }
            public uint HeaderSize { get; set; }

            public List<Material> Materials = new List<Material>();
            public List<string> TextureMaps = new List<string>();

            public List<Node> UpperNodes = new List<Node>();
            public Tuple<Node, Node> LinkNodes; //Links two nodes for some reason
            public List<Node> LowerNodes = new List<Node>();

            public void Read(FileReader reader, TreeNode root)
            {
                reader.ReadSignature(4, "BIKE");
                Version = reader.ReadUInt32();
                Alignment = reader.ReadUInt32();
                uint Padding = reader.ReadUInt32();
                uint MaterialCount = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt32();
                uint TextureMapsCount = reader.ReadUInt32();
                uint TextureMapsOffset = reader.ReadUInt32();

                //Seems to be a node based structure. Not %100 sure what decides which gets put into which
                uint UpperLevelNodeCount = reader.ReadUInt32();
                uint UpperLevelNodeOffset = reader.ReadUInt32();
                uint FirstNodeOffset = reader.ReadUInt32(); //Either an offset or the total size of section up to the node
                uint LinkNodeCount = reader.ReadUInt32();
                uint LinkNodeOffset = reader.ReadUInt32();
                uint LowerLevelNodeCount = reader.ReadUInt32();
                uint LowerLevelNodeOffset = reader.ReadUInt32();
                uint Padding2 = reader.ReadUInt32();
                uint[] Unknowns = reader.ReadUInt32s(10);

                for (int i = 0; i < MaterialCount; i++)
                {
                    Material mat = new Material();
                    mat.Read(reader);
                    Materials.Add(mat);
                }

                if (TextureMapsOffset != 0)
                {
                    reader.SeekBegin(TextureMapsOffset);
                    for (int i = 0; i < TextureMapsCount; i++)
                    {
                        TextureMaps.Add(reader.ReadNameOffset(false, typeof(uint)));
                    }

                }

                Console.WriteLine($"MiddleLevelNodeCount {UpperLevelNodeCount}");
                Console.WriteLine($"MiddleLevelNodeOffset {UpperLevelNodeOffset}");

                if (UpperLevelNodeCount != 0)
                {
                    for (int i = 0; i < UpperLevelNodeCount; i++)
                    {
                        reader.SeekBegin(UpperLevelNodeOffset + (i * 8));

                        string NodeName = reader.ReadNameOffset(false, typeof(uint));
                        uint Offset = reader.ReadUInt32();
                        Console.WriteLine($"NodeName {NodeName} Offset {Offset}");

                        if (Offset != 0)
                        {
                            reader.SeekBegin(Offset);
                            Node node = new Node();
                            node.Text = NodeName;
                            node.Read(reader);
                            UpperNodes.Add(node);
                        }
                    }
                }

                foreach (var node in UpperNodes)
                    LoadChildern(UpperNodes, node, root);
            }

            private void LoadChildern(List<Node> NodeLookup, Node Node, TreeNode root)
            {
                if (Node.ChildNode != null)
                {
                    if (NodeLookup.Contains(Node.ChildNode))
                    {
                        int index = NodeLookup.IndexOf(Node.ChildNode);
                        Node.ChildNode.Text = NodeLookup[index].Text;

                        Node.Nodes.Add(Node.ChildNode);
                        LoadChildern(NodeLookup, Node.ChildNode, Node.ChildNode);
                    }
                }

                root.Nodes.Add(Node);
            }
        }

        public class Material
        {
            public Vector4 Ambient;
            public Vector4 Diffuse;
            public Vector4 Specular;
            public float Shiny;
            public Vector4 Transparency;
            public float TransGlossy;
            public float TransparencySamples;
            public Vector4 Reflectivity;
            public float ReflectGlossy;
            public float ReflectSample;
            public float IndexRefreaction;
            public float Translucency;
            public float Unknown;
            public short[] TextureIndices;
            public uint[] Unknowns;

            public Material()
            {
                Ambient = new Vector4(0.3f, 0.3f, 0.3f,1.0f);
                Diffuse = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
                Specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                Shiny = 50;
                Transparency = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                TextureIndices = new short[10];
            }

            public void Read(FileReader reader)
            {
                Ambient = reader.ReadVec4();
                Diffuse = reader.ReadVec4();
                Specular = reader.ReadVec4();
                Shiny = reader.ReadSingle();
                Transparency = reader.ReadVec4();
                TransGlossy = reader.ReadSingle();
                TransparencySamples = reader.ReadSingle();
                Reflectivity = reader.ReadVec4();
                ReflectGlossy = reader.ReadSingle();
                ReflectSample = reader.ReadSingle();
                IndexRefreaction = reader.ReadSingle();
                Translucency = reader.ReadSingle();
                Unknown = reader.ReadSingle();
                TextureIndices = reader.ReadInt16s(10);
                Unknowns = reader.ReadUInt32s(10);
            }
        }

        public class Node : TreeNode
        {
            public bool Visible { get; set; }

            public Vector3 Scale { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Translation { get; set; }
            public byte[] Unknowns;
            public Node ChildNode { get; set; }

            public void Read(FileReader reader)
            {
                Visible = reader.ReadUInt32() == 1;
                Scale = reader.ReadVec3();
                Rotation = reader.ReadVec3();
                Translation = reader.ReadVec3();
                Unknowns = reader.ReadBytes(16);
                uint BufferArrayOffset = reader.ReadUInt32();
                uint ChildNodeOffset = reader.ReadUInt32();

                if (ChildNodeOffset != 0)
                {
                    reader.SeekBegin(ChildNodeOffset);
                    ChildNode = new Node();
                    ChildNode.Read(reader);
                }

                //After repeats a fairly similar structure, with SRT values
                //Unsure what it's used for?
            }
        }
    }
}
