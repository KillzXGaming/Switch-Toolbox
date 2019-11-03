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
    public class LM2_ModelFolder : TreeNodeCustom, IContextMenuNode
    {
        public LM2_DICT DataDictionary;

        public LM2_ModelFolder(LM2_DICT dict)
        {
            DataDictionary = dict;
            Text = "Models";
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export All", null, ExportModelAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportModelAction(object sender, EventArgs args)
        {
            ExportModel();
        }

        private void ExportModel()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.dae;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportModel(sfd.FileName);
            }
        }

        private void ExportModel(string FileName)
        {
            AssimpSaver assimp = new AssimpSaver();
            ExportModelSettings settings = new ExportModelSettings();

            List<STGenericMaterial> Materials = new List<STGenericMaterial>();
            //  foreach (var msh in DataDictionary.Renderer.Meshes)
            //    Materials.Add(msh.GetMaterial());

            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = DataDictionary.Renderer.Meshes;

            assimp.SaveFromModel(model, FileName, new List<STGenericTexture>(), new STSkeleton());
        }
    }

    public class LM2_Model : TreeNodeCustom, IContextMenuNode
    {
        public LM2_DICT DataDictionary;
        public LM2_ModelInfo ModelInfo;
        public List<LM2_Mesh> Meshes = new List<LM2_Mesh>();
        public List<uint> VertexBufferPointers = new List<uint>();

        public STSkeleton Skeleton;

        public uint BufferStart;
        public uint BufferSize;

        private List<RenderableMeshWrapper> RenderedMeshes = new List<RenderableMeshWrapper>();

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

        public override void OnClick(TreeView treeView)
        {
            if (Runtime.UseOpenGL)
            {
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                viewport.ReloadDrawables(DataDictionary.DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export", null, ExportModelAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportModelAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.dae;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportModel(sfd.FileName);
            }
        }

        private void ExportModel(string FileName)
        {
            AssimpSaver assimp = new AssimpSaver();
            ExportModelSettings settings = new ExportModelSettings();

            List<STGenericMaterial> Materials = new List<STGenericMaterial>();
            //  foreach (var msh in DataDictionary.Renderer.Meshes)
            //    Materials.Add(msh.GetMaterial());

            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = RenderedMeshes;

            assimp.SaveFromModel(model, FileName, new List<STGenericTexture>(), new STSkeleton());
        }


        public LM2_Model(LM2_DICT dict)
        {
            DataDictionary = dict;
        }

        public void OnPropertyChanged() { }

        public void ReadVertexBuffers()
        {
            Nodes.Clear();

            TreeNode skeletonNode = new TreeNode("Skeleton");
            for (int t = 0; t < Skeleton?.bones.Count; t++) {
                if (Skeleton.bones[t].Parent == null)
                    skeletonNode.Nodes.Add(Skeleton.bones[t]);
            }

            if (skeletonNode.Nodes.Count > 0)
                Nodes.Add(skeletonNode);

            using (var reader = new FileReader(DataDictionary.GetFile003Data()))
            {
                for (int i = 0; i < VertexBufferPointers.Count; i++)
                {
                    LM2_Mesh mesh = Meshes[i];

                    RenderableMeshWrapper genericObj = new RenderableMeshWrapper();
                    genericObj.Mesh = mesh;
                    genericObj.Text = $"Mesh {i}";
                    genericObj.SetMaterial(mesh.Material);
                    RenderedMeshes.Add(genericObj);

                    Nodes.Add(genericObj);
                    DataDictionary.Renderer.Meshes.Add(genericObj);

                    STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                    genericObj.PolygonGroups.Add(polyGroup);

                    using (reader.TemporarySeek(BufferStart + VertexBufferPointers[i], System.IO.SeekOrigin.Begin))
                    {
                        var bufferNodeDebug = new DebugVisualBytes(reader.ReadBytes((int)80 * mesh.VertexCount));
                        bufferNodeDebug.Text = $"Buffer {mesh.DataFormat.ToString("x")}";
                        genericObj.Nodes.Add(bufferNodeDebug);
                    }

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

                                        Vector4 nrm = Read_8_8_8_8_Snorm(reader);
                                        vert.nrm = nrm.Xyz.Normalized();

                                        vert.pos = Vector3.TransformPosition(vert.pos, mesh.Transform);
                                        vert.uv0 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());

                                        if (formatInfo.BufferLength == 22)
                                        {
                                            Console.WriteLine("unk 1 " + reader.ReadUInt16());
                                            Console.WriteLine("unk 2 " + reader.ReadUInt16());
                                            Console.WriteLine("unk 3 " + reader.ReadUInt16());
                                            Console.WriteLine("unk 4 " + reader.ReadUInt16());
                                        }
                                    }
                                    break;
                                case VertexDataFormat.Float32:
                                    for (int v = 0; v < mesh.VertexCount; v++)
                                    {
                                        reader.SeekBegin(bufferOffet + (v * formatInfo.BufferLength));

                                        Vertex vert = new Vertex();
                                        genericObj.vertices.Add(vert);

                                        vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        vert.pos = Vector3.TransformPosition(vert.pos, mesh.Transform);
                                    }
                                    break;
                                case VertexDataFormat.Float32_32:
                                    reader.BaseStream.Position = BufferStart + VertexBufferPointers[i] + 0x08;
                                    for (int v = 0; v < mesh.VertexCount; v++)
                                    {
                                        reader.SeekBegin(bufferOffet + (v * formatInfo.BufferLength));

                                        Vertex vert = new Vertex();
                                        genericObj.vertices.Add(vert);

                                        vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        vert.pos = Vector3.TransformPosition(vert.pos, mesh.Transform);
                                        vert.uv0 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());
                                        vert.uv1 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());
                                        vert.col = Read_8_8_8_8_Unorm(reader);
                                    }
                                    break;
                                case VertexDataFormat.Float32_32_32:
                                    for (int v = 0; v < mesh.VertexCount; v++)
                                    {
                                        reader.SeekBegin(bufferOffet + (v * formatInfo.BufferLength));

                                        Vertex vert = new Vertex();
                                        genericObj.vertices.Add(vert);

                                        vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        vert.pos = Vector3.TransformPosition(vert.pos, mesh.Transform);
                                        Vector4 nrm = Read_8_8_8_8_Snorm(reader);
                                        vert.nrm = nrm.Xyz.Normalized();
                                        vert.uv0 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());
                                        vert.uv1 = NormalizeUvCoordsToFloat(reader.ReadUInt16(), reader.ReadUInt16());

                                        if (formatInfo.BufferLength >= 0x1C)
                                            vert.col = Read_8_8_8_8_Unorm(reader);
                                    }
                                    break;
                            }

                            genericObj.TransformPosition(new Vector3(0), new Vector3(-90, 0, 0), new Vector3(1));
                        }
                    }

                    genericObj.RemoveDuplicateVertices();
                }
            }
        }

        public static Vector4 Read_8_8_8_8_Snorm(FileReader reader)
        {
            return new Vector4(reader.ReadSByte() / 255f, reader.ReadSByte() / 255f, reader.ReadSByte() / 255f, reader.ReadSByte() / 255f);
        }

        public static Vector4 Read_8_8_8_8_Unorm(FileReader reader)
        {
            return new Vector4(reader.ReadByte() / 255f, reader.ReadByte() / 255f, reader.ReadByte() / 255f, reader.ReadByte() / 255f);
        }

        public static Vector3 Read_8_8_8_Unorm(FileReader reader)
        {
            return new Vector3(reader.ReadByte() / 255f, reader.ReadByte() / 255f, reader.ReadByte() / 255f );
        }

        public static Vector2 NormalizeUvCoordsToFloat(ushort U, ushort V)
        {
            return new Vector2( U / 1024f, V / 1024f);
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

        public void Read(FileReader reader, List<LM2_Mesh> Meshes)
        {
            // This is very dumb. Just look and try to find the mesh hash and get the texture after
            int pos = 0;
            while (!reader.EndOfStream && reader.Position < reader.BaseStream.Length - 5)
            {
                reader.Position = pos++;
                uint HashIDCheck = reader.ReadUInt32();
                for (int i = 0; i < Meshes.Count; i++)
                {
                    if (Meshes[i].HashID == HashIDCheck)
                    {
                        uint TextureHashID = reader.ReadUInt32();

                        Meshes[i].Material = new LM2_Material();
                        var texUnit = 1;
                        Meshes[i].Material.TextureMaps.Add(new STGenericMatTexture()
                        {
                            textureUnit = texUnit++,
                            Type = STGenericMatTexture.TextureType.Diffuse,
                            Name = TextureHashID.ToString("x"),
                        });
                    }
                }
            }

            /*
            for (int i = 0; i < Meshes.Count; i++)
            {
                //This section keeps varing so just search for mesh hash id and get texture hash after it


                uint Unknown = reader.ReadUInt32(); //A81E313F
                reader.Seek(40);

                //Not sure what this is. Not a transform as the UVs seem fine as is
                float[] Unknown2 = reader.ReadSingles(5); //0.5, 1, 0.5,0.5, 1
                reader.Seek(4); //Padding
                uint MeshHashID = reader.ReadUInt32();
                uint TextureHashID = reader.ReadUInt32();
                uint UnknownHashID = reader.ReadUInt32(); //Material hash??

                //Go through each mesh and find a matching hash
                for (int m = 0; m < Meshes.Count; m++)
                {
                    if (Meshes[m].HashID == MeshHashID)
                    {

                    }
                };

                if (i != Meshes.Count - 1)
                    reader.Seek(4); //padding on all but last entry
            }*/
        }
    }

    public class RenderableMeshWrapper : GenericRenderedObject
    {
        public LM2_Mesh Mesh { get; set; }

        LM2_Material material;

        public override STGenericMaterial GetMaterial()
        {
            return material;
        }

        public void SetMaterial(LM2_Material mat)
        {
            material = mat;
        }

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


    public class DebugVisualBytes : TreeNodeFile, IContextMenuNode
    {
        public byte[] Data;

        public DebugVisualBytes(byte[] bytes)
        {
            Data = bytes;
        }

        public override void OnClick(TreeView treeView)
        {
            HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
            if (editor == null)
            {
                editor = new HexEditor();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadData(Data);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new STToolStipMenuItem("Export Raw Data", null, Export, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Text;
            sfd.Filter = "Raw Data (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(sfd.FileName, Data);
            }
        }
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

        public ushort BufferPtrOffset { get; private set; }
        public ushort Unknown { get; private set; }
        public ulong DataFormat { get; private set; }
        public uint Unknown2 { get; private set; }
        public uint Unknown3 { get; private set; }
        public uint Unknown4 { get; private set; } //Increases after each mesh. Always 0 for the first mesh (some sort of offset)?
        public ushort VertexCount { get; private set; }
        public ushort Unknown7 { get; private set; } //Always 256?
        public uint HashID { get; private set; }

        public LM2_Material Material { get; set; }

        public Matrix4 Transform { get; set; } = Matrix4.Identity;

        public void Read(FileReader reader)
        {
            Material = new LM2_Material();

            IndexStartOffset = reader.ReadUInt32();
            IndexCount = reader.ReadUInt16();
            IndexFormat = reader.ReadEnum<IndexFormat>(true);
            BufferPtrOffset = reader.ReadUInt16(); //I believe this might be for the buffer pointers. It shifts by 4 for each mesh
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
            { 0x4E315C83A856FBF7, new FormatInfo(VertexDataFormat.Float32_32_32, 0x1C)},
            { 0xBD15F722F07FC596, new FormatInfo(VertexDataFormat.Float32_32_32, 0x1C)},
            { 0xFBACD243DDCC31B7, new FormatInfo(VertexDataFormat.Float32_32_32, 0x1C)},
            { 0x8A4CC565333626D9, new FormatInfo(VertexDataFormat.Float32_32, 0x18)},
            { 0x8B8CE58EAA846002, new FormatInfo(VertexDataFormat.Float32_32_32, 0x14)},
        };
    }
}
