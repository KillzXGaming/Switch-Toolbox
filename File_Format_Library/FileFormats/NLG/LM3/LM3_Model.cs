using System;
using System.IO;
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

namespace FirstPlugin.LuigisMansion3
{
    public class LM3_ModelFolder : TreeNodeCustom, IContextMenuNode
    {
        public LM3_DICT DataDictionary;

        public LM3_ModelFolder(LM3_DICT dict)
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
            FolderSelectDialog folderDlg = new FolderSelectDialog();
            if (folderDlg.ShowDialog() == DialogResult.OK)
            {
                ExportModelSettings exportDlg = new ExportModelSettings();
                if (exportDlg.ShowDialog() == DialogResult.OK)
                    ExportModel(folderDlg.SelectedPath, exportDlg.Settings);
            }
        }

        public void ExportModel(string folderPath, DAE.ExportSettings settings)
        {
            foreach (LM3_Model mdl in Nodes)
            {
                List<STGenericMaterial> Materials = new List<STGenericMaterial>();
                foreach (STGenericObject mesh in mdl.RenderedMeshes)
                    if (mesh.GetMaterial() != null)
                        Materials.Add(mesh.GetMaterial());

                if (!mdl.loaded)
                    mdl.UpdateVertexData();

                var model = new STGenericModel();
                model.Materials = Materials;
                model.Objects = mdl.RenderedMeshes;

                settings.SuppressConfirmDialog = true;

                if (mdl.Text.Contains("/"))
                    Directory.CreateDirectory($"{folderPath}/{Path.GetDirectoryName(mdl.Text)}");
                DAE.Export($"{folderPath}/{mdl.Text.Replace("/", "//")}.dae", settings, model, new List<STGenericTexture>());
            }

            System.Windows.Forms.MessageBox.Show($"Exported models Successfuly!");
        }
    }

    public class LM3_Model : TreeNodeCustom, IContextMenuNode
    {
        public List<uint> TextureHashes = new List<uint>();

        public LM3_DICT DataDictionary;
        public LM3_ModelInfo ModelInfo;
        public List<LM3_Mesh> Meshes = new List<LM3_Mesh>();
        public List<PointerInfo> VertexBufferPointers = new List<PointerInfo>();

        public STSkeleton Skeleton;

        public uint BufferStart;
        public uint BufferSize;

        public List<RenderableMeshWrapper> RenderedMeshes = new List<RenderableMeshWrapper>();

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

        public bool loaded = false;
        public override void OnClick(TreeView treeView)
        {
            if (!loaded)
                UpdateVertexData();

            if (Runtime.UseOpenGL)
            {
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                viewport.SuppressUpdating = true;

                for (int i = 0; i < DataDictionary.DrawableContainer.Drawables.Count; i++)
                {
                    if (DataDictionary.DrawableContainer.Drawables[i] is STSkeleton)
                        ((STSkeleton)DataDictionary.DrawableContainer.Drawables[i]).HideSkeleton = true;
                }

                if (Skeleton != null)
                    Skeleton.HideSkeleton = false;

                foreach (var mesh in DataDictionary.Renderer.Meshes)
                    mesh.Checked = false;

                foreach (TreeNode mesh in Nodes)
                    mesh.Checked = true;

                viewport.SuppressUpdating = false;

                Viewport editor = (Viewport)LibraryGUI.GetActiveContent(typeof(Viewport));
                if (editor == null)
                {
                    editor = viewport;
                    LibraryGUI.LoadEditor(viewport);
                }

                viewport.ReloadDrawables(DataDictionary.DrawableContainer);
                viewport.Text = Text;
            }
        }

        public void UpdateVertexData()
        {
            ReadVertexBuffers();
            DataDictionary.Renderer.UpdateVertexData();

            loaded = true;
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
                ExportModelSettings exportDlg = new ExportModelSettings();
                if (exportDlg.ShowDialog() == DialogResult.OK)
                    ExportModel(sfd.FileName, exportDlg.Settings);
            }
        }

        public void ExportModel(string fileName, DAE.ExportSettings settings)
        {
            List<STGenericMaterial> Materials = new List<STGenericMaterial>();
            List<STGenericTexture> textures = GetTextureList();

            foreach (STGenericObject mesh in RenderedMeshes)
                if (mesh.GetMaterial() != null)
                    Materials.Add(mesh.GetMaterial());

            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = RenderedMeshes;

            DAE.Export(fileName, settings, model, textures);
        }

        public LM3_Model(LM3_DICT dict)
        {
            DataDictionary = dict;
        }

        public class PointerInfo
        {
            //Note if a pointer is not used, it will be 0xFFFFF

            public uint WeightTablePointer = uint.MaxValue;
            public uint VertexBufferPointer;
            public uint IndexBufferPointer;
            public uint IndexBufferPointer2;

            public void Read(FileReader reader, bool hasWeightTable = true)
            {
                if (hasWeightTable && !reader.EndOfStream)
                    WeightTablePointer = reader.ReadUInt32();
                if (!reader.EndOfStream)
                    VertexBufferPointer = reader.ReadUInt32();
                if (!reader.EndOfStream)
                    IndexBufferPointer = reader.ReadUInt32();
                if (!reader.EndOfStream)
                    IndexBufferPointer2 = reader.ReadUInt32();
            }
        }

        public void OnPropertyChanged() { }

        public List<STGenericTexture> GetTextureList()
        {
            List<STGenericTexture> texturesList = new List<STGenericTexture>();
            for (int t = 0; t < TextureHashes.Count; t++)
            {
                if (DataDictionary.Renderer.TextureList.ContainsKey(TextureHashes[t].ToString("x")))
                    texturesList.Add(DataDictionary.Renderer.TextureList[TextureHashes[t].ToString("x")]);
            }
            return texturesList;
        }

        public void ReadVertexBuffers()
        {
            Nodes.Clear();

            using (var reader = new FileReader(DataDictionary.GetFileBufferData(), true))
            {
                TreeNode texturesList = new TreeNode("Texture Maps");
                TreeNode skeletonNode = new TreeNode("Skeleton");
                for (int t = 0; t < Skeleton?.bones.Count; t++) {
                    skeletonNode.Nodes.Add(Skeleton.bones[t]);
                }

                for (int t = 0; t < TextureHashes.Count; t++)
                {
                    if (DataDictionary.Renderer.TextureList.ContainsKey(TextureHashes[t].ToString("x")))
                    {
                        var tex = DataDictionary.Renderer.TextureList[TextureHashes[t].ToString("x")];
                        texturesList.Nodes.Add(new TreeNode(tex.Text)
                        {
                            ImageKey = tex.ImageKey,
                            SelectedImageKey = tex.ImageKey,
                            Tag = tex
                        });
                    }
                    else
                        Nodes.Add(TextureHashes[t].ToString("x"));
                }

                if (skeletonNode.Nodes.Count > 0)
                    Nodes.Add(skeletonNode);

                if (texturesList.Nodes.Count > 0)
                    Nodes.Add(texturesList);

                for (int i = 0; i < Meshes.Count; i++)
                {
                    LM3_Mesh mesh = Meshes[i];

                    RenderableMeshWrapper genericObj = new RenderableMeshWrapper();
                    genericObj.Mesh = mesh;
                    genericObj.Checked = true;
                    genericObj.Text = $"Mesh {i} {mesh.HashID.ToString("X")}";
                    if (NLG_Common.HashNames.ContainsKey(mesh.HashID))
                        genericObj.Text = NLG_Common.HashNames[mesh.HashID];

                    genericObj.SetMaterial(mesh.Material);
                    RenderedMeshes.Add(genericObj);

                    Nodes.Add(genericObj);

                    DataDictionary.Renderer.Meshes.Add(genericObj);

                    STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                    genericObj.PolygonGroups.Add(polyGroup);

                    uint vertexBufferPointer = VertexBufferPointers[i].VertexBufferPointer;
                    uint weightTablePointer = VertexBufferPointers[i].WeightTablePointer;

                    using (reader.TemporarySeek(BufferStart + vertexBufferPointer, System.IO.SeekOrigin.Begin))
                    {
                        var bufferNodeDebug = new DebugVisualBytes(reader.ReadBytes((int)80 * (int)mesh.VertexCount));
                        bufferNodeDebug.Text = $"Buffer {mesh.DataFormat.ToString("x")}";
                        genericObj.Nodes.Add(bufferNodeDebug);
                    }

                    LM3_Mesh.FormatInfo formatInfo;
                    if (!LM3_Mesh.FormatInfos.ContainsKey(mesh.DataFormat))
                    {
                        Console.WriteLine($"Unsupported data format! " + mesh.DataFormat.ToString("x"));
                        formatInfo = new LM3_Mesh.FormatInfo(VertexDataFormat.Float32_32_32, 0x30);
                      //  continue;
                    }
                    else
                        formatInfo = LM3_Mesh.FormatInfos[mesh.DataFormat];

                    if (formatInfo.BufferLength > 0)
                    {
                        Console.WriteLine($"BufferStart {BufferStart} IndexStartOffset {mesh.IndexStartOffset}");

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
                                /*   case IndexFormat.Index_32:
                                       for (int f = 0; f < mesh.IndexCount; f++)
                                           polyGroup.faces.Add((int)reader.ReadUInt32());
                                       break;*/
                        }

                        Console.WriteLine($"Mesh {genericObj.Text} Format {formatInfo.Format} BufferLength {formatInfo.BufferLength}");

                        Console.WriteLine($"BufferStart {BufferStart} VertexBufferPointers {vertexBufferPointer}");

                        uint bufferOffet = BufferStart + vertexBufferPointer;
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
                                reader.BaseStream.Position = BufferStart + vertexBufferPointer + 0x08;
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

                                    //Texture coordinates are stored between normals, WHY NLG
                                    var texCoordU = reader.ReadSingle();
                                    vert.nrm = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                    var texCoordV = reader.ReadSingle();
                                    vert.tan = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                    vert.uv0 = new Vector2(texCoordU, texCoordV);
                                }
                                break;
                        }

                        genericObj.TransformPosition(new Vector3(0), new Vector3(-90, 0, 0), new Vector3(1));
                    }

                    if (weightTablePointer != uint.MaxValue)
                    {
                        using (reader.TemporarySeek(BufferStart + weightTablePointer, System.IO.SeekOrigin.Begin))
                        {
                            byte maxIndex = 0;
                            for (int v = 0; v < genericObj.vertices.Count; v++)
                            {
                                byte[] boneIndices = reader.ReadBytes(4);
                                float[] boneWeights = reader.ReadSingles(4);

                                for (int j = 0; j < 4; j++) {
                                    maxIndex = Math.Max(maxIndex, boneIndices[j]);
                                //    genericObj.vertices[v].boneIds.Add(boneIndices[j]);
                               //     genericObj.vertices[v].boneWeights.Add(boneWeights[j]);
                                }
                            }

                            Console.WriteLine("maxIndex " + maxIndex);
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

        public static Vector2 NormalizeUvCoordsToFloat(uint U, uint V)
        {
            return new Vector2(U / 1024f, V / 1024f);
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

    public class LM3_ModelInfo
    {
        public byte[] Data;

        public void Read(FileReader reader, LM3_Model model,  List<uint> Hashes)
        {
            while (!reader.EndOfStream && reader.Position < reader.BaseStream.Length - 4)
            {
                uint HashIDCheck = reader.ReadUInt32();

                if (Hashes.Contains(HashIDCheck))
                {
                    if (!model.TextureHashes.Contains(HashIDCheck))
                        model.TextureHashes.Add(HashIDCheck);
                }
            }

            reader.Position = 0;

            var meshSize = reader.BaseStream.Length / model.Meshes.Count;
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                reader.SeekBegin(i * meshSize);
                while (!reader.EndOfStream && reader.Position < reader.BaseStream.Length - 4)
                {
                    uint HashIDCheck = reader.ReadUInt32();
                    if (Hashes.Contains(HashIDCheck))
                    {
                        model.Meshes[i].Material = new LM3_Material();
                        var texUnit = 1;
                        model.Meshes[i].Material.TextureMaps.Add(new STGenericMatTexture()
                        {
                            textureUnit = texUnit++,
                            Type = STGenericMatTexture.TextureType.Diffuse,
                            Name = HashIDCheck.ToString("x"),
                        });

                        break;
                    }
                }
            }
        }
    }

    public class RenderableMeshWrapper : GenericRenderedObject
    {
        public LM3_Mesh Mesh { get; set; }

        LM3_Material material;

        public override STGenericMaterial GetMaterial()
        {
            return material;
        }

        public void SetMaterial(LM3_Material mat)
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

    public class LM3_IndexList
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

    public class LM3_Mesh
    {
        public uint IndexStartOffset { get; private set; } //relative to buffer start
        public uint IndexCount { get; private set; } //divide by 3 to get face count
        public IndexFormat IndexFormat { get; private set; } //0x0 - ushort, 0x8000 - byte

        public ushort BufferPtrOffset { get; private set; }
        public ushort Unknown { get; private set; }
        public ulong DataFormat { get; private set; }
        public uint Unknown2 { get; private set; }
        public uint Unknown3 { get; private set; } //Possibly contributes to rigged meshes. 0xFFFF if static (no weight table / pointer)
        public uint Unknown4 { get; private set; } //Increases after each mesh. Always 0 for the first mesh (some sort of offset)?
        public uint Unknown5 { get; private set; }
        public uint VertexCount { get; private set; }
        public ushort Unknown7 { get; private set; } //Always 256?
        public uint HashID { get; private set; }

        public LM3_Material Material { get; set; }

        public Matrix4 Transform { get; set; } = Matrix4.Identity;

        public void Read(FileReader reader)
        {
            Material = new LM3_Material();

            HashID = reader.ReadUInt32();
            IndexStartOffset = reader.ReadUInt32();
            uint indexFlags = reader.ReadUInt32();
            IndexCount = (indexFlags & 0xffffff);
            uint type = (indexFlags >> 24);
            if (type == 0x80)
                IndexFormat = IndexFormat.Index_8;
            else
                IndexFormat = IndexFormat.Index_16;

            VertexCount = reader.ReadUInt32(); 
            reader.ReadUInt32(); //unknown
            BufferPtrOffset = reader.ReadUInt16(); //I believe this might be for the buffer pointers. It shifts by 4 for each mesh
            Unknown = reader.ReadUInt16();
            DataFormat = reader.ReadUInt64();
            Unknown2 = reader.ReadUInt32();
            Unknown3 = reader.ReadUInt32(); //Sometimes 0xFFFF. 
             reader.ReadUInt16();
            Unknown4 = reader.ReadUInt32();
            Unknown5 = reader.ReadUInt32();
            Unknown7 = reader.ReadUInt16(); 
            reader.ReadUInt32(); //unknown
            reader.ReadUInt32(); //unknown
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

        //Formats are based on https://github.com/TheFearsomeDzeraora/LM3L/blob/master/ModelThingy.cs#L639
        //These may not be very accurate, i need to look more into these
        public static Dictionary<ulong, FormatInfo> FormatInfos = new Dictionary<ulong, FormatInfo>()
        {
            { 0xcb418c82ba25920e, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0x4c083342551178ce, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0xa2fdc74f42ce4fdb, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0xcb092e9f8322ba, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0x2031dd4da78347d9, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0x210ed90e5465129a, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0xa86b2280a1500a0c, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0xc5f54a808b32320c, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0x568f92478fa0a2d3, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0xc344835dd398dde9, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0x8d45618da4768c19, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},
            { 0xd5b9166924e124f5, new FormatInfo(VertexDataFormat.Float32_32_32, 0x30)},

            { 0x5f5227f782c08883, new FormatInfo(VertexDataFormat.Float32_32_32, 0x0C)},


            //Dark moon list just for the sake of having them defined
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
