using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using Toolbox.Library.Forms;
using OpenTK;

namespace FirstPlugin.LuigisMansion.DarkMoon
{
    public class LM2_Model : TreeNodeCustom
    {
        public LM2_DICT DataDictionary;
        public LM2_ModelInfo ModelInfo;
        public List<LM2_Mesh> Meshes = new List<LM2_Mesh>();
        public List<uint> VertexBufferPointers = new List<uint>();
        public Matrix4 Transform { get; set; }

        public uint BufferStart;
        public uint BufferSize;

        Viewport viewport
        {
            get
            {
                var editor = LibraryGUI.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            if (Runtime.UseOpenGL)
            {
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                if (!DrawablesLoaded)
                {
                    ObjectEditor.AddContainer(DataDictionary.DrawableContainer);
                    DrawablesLoaded = true;
                }

                viewport.ReloadDrawables(DataDictionary.DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public LM2_Model(LM2_DICT dict)
        {
            DataDictionary = dict;
        }

        public void OnPropertyChanged() { }

        public void ReadVertexBuffers()
        {
            Nodes.Clear();

            using (var reader = new FileReader(DataDictionary.GetFile003Data()))
            {
                for (int i = 0; i < VertexBufferPointers.Count; i++)
                {
                    LM2_Mesh mesh = Meshes[i];

                    RenderableMeshWrapper genericObj = new RenderableMeshWrapper();
                    genericObj.Mesh = mesh;
                    genericObj.Text = $"Mesh {i}";
                    Nodes.Add(genericObj);
                    DataDictionary.Renderer.Meshes.Add(genericObj);

                    STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                    genericObj.PolygonGroups.Add(polyGroup);

                    if (!LM2_Mesh.FormatInfos.ContainsKey(mesh.DataFormat))
                    {
                        Console.WriteLine($"Unsupported data format! " + mesh.DataFormat.ToString("x"));
                        continue;
                    }
                    else
                    {
                        var formatInfo = LM2_Mesh.FormatInfos[mesh.DataFormat];
                        if (formatInfo.BufferLength > 0)
                        {
                            reader.BaseStream.Position = BufferStart + mesh.IndexStartOffset;
                            switch (mesh.IndexFormat)
                            {
                                case IndexFormat.Index_8:
                                    for (int f = 0; f < mesh.IndexCount; f++)
                                        polyGroup.faces.Add(reader.ReadByte());
                                        break;
                                case IndexFormat.Index_16:
                                    for (int f = 0; f < mesh.IndexCount; f++)
                                        polyGroup.faces.Add(reader.ReadUInt16());
                                    break;
                            }

                            Console.WriteLine($"Mesh {genericObj.Text} Format {formatInfo.Format} BufferLength {formatInfo.BufferLength}");

                            uint bufferOffet = BufferStart + VertexBufferPointers[i];
                     /*       for (int v = 0; v < mesh.VertexCount; v++)
                            {
                                reader.SeekBegin(bufferOffet + (v * formatInfo.BufferLength));

                            }*/

                            switch (formatInfo.Format)
                            {
                                case VertexDataFormat.Float16:
                                    for (int v = 0; v < mesh.VertexCount; v++)
                                    {
                                        reader.SeekBegin(bufferOffet + (v * formatInfo.BufferLength));

                                        Vertex vert = new Vertex();
                                        genericObj.vertices.Add(vert);
                                        vert.pos = new Vector3(
                                            UShortToFloatDecode(reader.ReadInt16()),
                                            UShortToFloatDecode(reader.ReadInt16()),
                                            UShortToFloatDecode(reader.ReadInt16()));
                                        reader.BaseStream.Position += 0x4;

                                        vert.pos = Vector3.TransformPosition(vert.pos, Transform);

                                        vert.uv0 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());

                                    /*    reader.BaseStream.Position += 8;

                                        //  vert.uv1 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());
                                        //   vert.uv2 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());

                                        vert.nrm = new Vector3(
                                            UShortToFloatDecode(reader.ReadInt16()),
                                            UShortToFloatDecode(reader.ReadInt16()),
                                            UShortToFloatDecode(reader.ReadInt16()));*/
                                    }
                                    break;
                                case VertexDataFormat.Float32:
                                    for (int v = 0; v < mesh.VertexCount; v++)
                                    {
                                        reader.SeekBegin(bufferOffet + (v * formatInfo.BufferLength));

                                        Vertex vert = new Vertex();
                                        genericObj.vertices.Add(vert);

                                        vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        vert.pos = Vector3.TransformPosition(vert.pos, Transform);
                                    }
                                    break;
                                case VertexDataFormat.Float32_32:
                                    reader.BaseStream.Position = BufferStart + VertexBufferPointers[i] + 0x08;
                                    for (int v = 0; v < mesh.VertexCount; v++)
                                    {
                                        Vertex vert = new Vertex();
                                        genericObj.vertices.Add(vert);

                                        vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        vert.pos = Vector3.TransformPosition(vert.pos, Transform);

                                        reader.BaseStream.Position += formatInfo.BufferLength - 0x14;
                                    }
                                    break;
                                case VertexDataFormat.Float32_32_32:
                                    for (int v = 0; v < mesh.VertexCount; v++)
                                    {
                                        reader.SeekBegin(bufferOffet + (v * formatInfo.BufferLength));

                                        Vertex vert = new Vertex();
                                        genericObj.vertices.Add(vert);

                                        vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        vert.pos = Vector3.TransformPosition(vert.pos, Transform);

                                        reader.BaseStream.Position += 0x4;
                                        vert.uv0 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public static Vector2 NormalizeUvCoordsToFloat(ushort inU, ushort inV)
        {
            //Normalize U coordinate
            float U = (float)inU / (float)1024;
            //Normalize V coordinate
            float V = (float)inV / (float)1024;
            return new Vector2(U, V);
        }

        public static float UShortToFloatDecode(short input)
        {
            float fraction = (float)BitConverter.GetBytes(input)[0] / (float)256;
            sbyte integer = (sbyte)BitConverter.GetBytes(input)[1];
            return integer + fraction;
        }
    }

    public class LM2_ModelInfo
    {
        public byte[] Data;
    }

    public class RenderableMeshWrapper : GenericRenderedObject
    {
        public LM2_Mesh Mesh { get; set; }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(Mesh, OnPropertyChanged);
        }

        public void OnPropertyChanged() { }
    }

    public class LM2_IndexList
    {
        public short[] UnknownIndices { get; set; }

        public uint Unknown { get; set; }

        public short[] UnknownIndices2 { get; set; }

        public uint[] Unknown2 { get; set; }

        public void Read(FileReader reader)
        {
            UnknownIndices = reader.ReadInt16s(4);
            Unknown = reader.ReadUInt32();
            UnknownIndices2 = reader.ReadInt16s(8);
            Unknown2 = reader.ReadUInt32s(6); //Increases by 32 each entry
        }
    }

    public class LM2_Mesh
    {
        public uint IndexStartOffset { get; private set; } //relative to buffer start
        public ushort IndexCount { get; private set; } //divide by 3 to get face count
        public IndexFormat IndexFormat { get; private set; } //0x0 - ushort, 0x8000 - byte

        public ushort BufferStride { get; private set; }
        public ushort Unknown { get; private set; }
        public ulong DataFormat { get; private set; }
        public uint Unknown2 { get; private set; }
        public uint Unknown3 { get; private set; }
        public uint Unknown4 { get; private set; }
        public ushort VertexCount { get; private set; }
        public ushort Unknown7 { get; private set; }
        public uint HashID { get; private set; }

        public void Read(FileReader reader)
        {
            IndexStartOffset = reader.ReadUInt32();
            IndexCount = reader.ReadUInt16();
            IndexFormat = reader.ReadEnum<IndexFormat>(true);
            BufferStride = reader.ReadUInt16();
            Unknown = reader.ReadUInt16();
            DataFormat = reader.ReadUInt64();
            Unknown2 = reader.ReadUInt32();
            Unknown3 = reader.ReadUInt32();
            Unknown4 = reader.ReadUInt32();
            VertexCount = reader.ReadUInt16();
            Unknown7 = reader.ReadUInt16(); //0x100
            HashID = reader.ReadUInt32(); //0x100
        }

        public class FormatInfo
        {
            public VertexDataFormat Format { get; set; }
            public uint BufferLength { get; set; }

            public FormatInfo(VertexDataFormat format, uint length)
            {
                Format = format;
                BufferLength = length;
            }
        }

        //Formats are based on https://github.com/TheFearsomeDzeraora/LM2L/blob/master/ModelThingy.cs#L639
        //These may not be very accurate, i need to look more into these
        public static Dictionary<ulong, FormatInfo> FormatInfos = new Dictionary<ulong, FormatInfo>()
        {
            { 0x6350379972D28D0D, new FormatInfo(VertexDataFormat.Float16, 0x46)},
            { 0xDC0291B311E26127, new FormatInfo(VertexDataFormat.Float16, 0x16)},
            { 0x93359708679BEB7C, new FormatInfo(VertexDataFormat.Float16, 0x16)},
            { 0x1A833CEEC88C1762, new FormatInfo(VertexDataFormat.Float16, 0x46)},
            { 0xD81AC10B8980687F, new FormatInfo(VertexDataFormat.Float16, 0x16)},
            { 0x2AA2C56A0FFA5BDE, new FormatInfo(VertexDataFormat.Float16, 0x1A)},
            { 0x5D6C62BAB3F4492E, new FormatInfo(VertexDataFormat.Float16, 0x16)},
            { 0x3CC7AB6B4821B2DF, new FormatInfo(VertexDataFormat.Float32, 0x14)},
            { 0x408E2B1F5576A693, new FormatInfo(VertexDataFormat.Float32_32_32, 0x10)},
            { 0x0B663399DF24890D, new FormatInfo(VertexDataFormat.Float32_32_32, 0x18)},
            { 0x7EB9853DF4F13EB1, new FormatInfo(VertexDataFormat.Float32_32_32, 0x18)},
            { 0x314A20AEFADABB22, new FormatInfo(VertexDataFormat.Float32_32_32, 0x18)},
            { 0x0F3F68A287C2B716, new FormatInfo(VertexDataFormat.Float32_32_32, 0x18)},
            { 0x27F993771090E6EB, new FormatInfo(VertexDataFormat.Float32_32_32, 0x1C)},
            { 0x4E315C83A856FBF7, new FormatInfo(VertexDataFormat.Float32, 0x1C)},
            { 0xBD15F722F07FC596, new FormatInfo(VertexDataFormat.Float32_32_32, 0x1C)},
            { 0xFBACD243DDCC31B7, new FormatInfo(VertexDataFormat.Float32_32_32, 0x1C)},
            { 0x8A4CC565333626D9, new FormatInfo(VertexDataFormat.Float32_32, 0x1C)},
            { 0x8B8CE58EAA846002, new FormatInfo(VertexDataFormat.Float32_32_32, 0x14)},
        };
    }
}
