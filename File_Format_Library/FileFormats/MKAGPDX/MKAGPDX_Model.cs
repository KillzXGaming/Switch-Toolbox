using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.GL_Core;

namespace FirstPlugin
{
    public class MKAGPDX_Model : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Mario Kart Arcade GP DX" };
        public string[] Extension { get; set; } = new string[] { "*.bin", "*.mot" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "BIKE");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => null;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => toolExt;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] toolExt = new STToolStripItem[1];

            public MenuExt()
            {
                toolExt[0] = new STToolStripItem("Models");
                toolExt[0].DropDownItems.Add(new STToolStripItem("Batch Export (MKAGPDX .bin)", BatchExport));
                toolExt[0].DropDownItems.Add(new STToolStripItem("Batch Export as Combined (MKAGPDX .bin)", BatchExportCombined));
            }

            public void BatchExportCombined(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.Filter = "Supported Formats|*.bin";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Supported Formats|*.dae";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    List<STGenericObject> Objects = new List<STGenericObject>();
                    STSkeleton Skeleton = new STSkeleton();
                    List<STGenericMaterial> Materials = new List<STGenericMaterial>();

                    int MatIndex = 0;
                    foreach (var file in ofd.FileNames)
                    {
                        MKAGPDX_Model model = new MKAGPDX_Model();
                        var stream = File.Open(file, FileMode.Open);
                        model.Load(stream);
                        stream.Dispose();

                        foreach (STGenericMaterial mat in model.Nodes[0].Nodes)
                        {
                            mat.Text = $"Material {MatIndex++}";
                            Materials.Add(mat);
                        }

                        Skeleton.bones.AddRange(((STSkeleton)model.DrawableContainer.Drawables[0]).bones);
                        Objects.AddRange(((GenericModelRenderer)model.DrawableContainer.Drawables[1]).Meshes);

                        model.Unload();
                    }

                    ExportModelSettings settings = new ExportModelSettings();
                    if (settings.ShowDialog() == DialogResult.OK)
                        DAE.Export(sfd.FileName, settings.Settings, Objects, Materials, new List<STGenericTexture>(), Skeleton);
                }
            }

            public void BatchExport(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.Filter = "Supported Formats|*.bin";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                FolderSelectDialog folderDlg = new FolderSelectDialog();
                if (folderDlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in ofd.FileNames)
                    {
                        MKAGPDX_Model model = new MKAGPDX_Model();
                        var stream = File.Open(file, FileMode.Open);
                        model.Load(stream);
                        stream.Dispose();

                        string Path = System.IO.Path.Combine(folderDlg.SelectedPath,
                                      System.IO.Path.GetFileNameWithoutExtension(file) + ".dae");

                        model.ExportModel(Path, new DAE.ExportSettings());
                        model.Unload();
                    }
                }
            }
        }

        Header header;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public void Load(System.IO.Stream stream)
        {
            DrawableContainer.Name = FileName;

            Text = FileName;
            header = new Header();
            header.Read(new FileReader(stream), this);

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export Model", null, ExportAction, Keys.Control | Keys.E));
        }

        private void ExportAction(object sender, EventArgs args)
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
            foreach (STGenericMaterial mat in Nodes[0].Nodes)
                Materials.Add(mat);

            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = ((GenericModelRenderer)DrawableContainer.Drawables[1]).Meshes;

            DAE.Export(fileName, settings, model, new List<STGenericTexture>(), ((STSkeleton)DrawableContainer.Drawables[0]));
        }


        public void Unload()
        {
            foreach (var mesh in ((GenericModelRenderer)DrawableContainer.Drawables[1]).Meshes)
            {
                mesh.vertices.Clear();
                mesh.faces.Clear();
                mesh.display = new int[0];
            }
            ((GenericModelRenderer)DrawableContainer.Drawables[1]).Meshes.Clear();

            DrawableContainer.Drawables.Clear();
            DrawableContainer = null;
            header.Materials.Clear();
            header.TextureMaps.Clear();
            header.TotalNodes.Clear();
            header.BoneList.Clear();
            header.LinkNodes.Clear();
            Nodes.Clear();
            header = null;
        }

        public void Save(System.IO.Stream stream)
        {
        }

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
                    ObjectEditor.AddContainer(DrawableContainer);
                    DrawablesLoaded = true;
                }

                viewport.ReloadDrawables(DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public class Header
        {
            public uint Version { get; set; }
            public uint Alignment { get; set; }
            public uint HeaderSize { get; set; }

            public List<Material> Materials = new List<Material>();
            public List<string> TextureMaps = new List<string>();

            public List<Node> BoneList = new List<Node>();
            public List<Tuple<Node, Node>> LinkNodes = new List<Tuple<Node, Node>>(); //Links two nodes. Possbily a mesh to bones for example
            public List<Node> TotalNodes = new List<Node>();

            public STSkeleton Skeleton { get; set; }
            GenericModelRenderer DrawableRenderer { get; set; }

            public void Read(FileReader reader, MKAGPDX_Model root)
            {
                Skeleton = new STSkeleton();
                DrawableRenderer = new GenericModelRenderer();
                root.DrawableContainer.Drawables.Add(Skeleton);
                root.DrawableContainer.Drawables.Add(DrawableRenderer);

                reader.ReadSignature(4, "BIKE");
                ushort Type = reader.ReadUInt16();
                ushort Unknown = reader.ReadUInt16();

                if (Type == 0)
                {

                }
                else if ((Type == 1))
                {
                    throw new Exception("Animation files not supported yet!");
                }
                else
                {
                    throw new Exception("Unknown type found! " + Type);
                }

                Alignment = reader.ReadUInt32();
                uint Padding = reader.ReadUInt32();
                uint MaterialCount = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt32();
                uint TextureMapsCount = reader.ReadUInt32();
                uint TextureMapsOffset = reader.ReadUInt32();

                uint BoneCount = reader.ReadUInt32();
                uint BoneOffset = reader.ReadUInt32();
                uint FirstNodeOffset = reader.ReadUInt32(); //Either an offset or the total size of section up to the node
                uint LinkNodeCount = reader.ReadUInt32();
                uint LinkNodeOffset = reader.ReadUInt32();
                uint TotalNodeCount = reader.ReadUInt32();
                uint TotalNodeOffset = reader.ReadUInt32();
                uint Padding2 = reader.ReadUInt32();
                uint[] Unknowns = reader.ReadUInt32s(10);

                root.Nodes.Add("Materials");
                root.Nodes.Add("Root");
                root.Nodes.Add("All Nodes");

                long pos = reader.Position;

                if (TextureMapsOffset != 0)
                {
                    reader.SeekBegin(TextureMapsOffset);
                    for (int i = 0; i < TextureMapsCount; i++)
                    {
                        TextureMaps.Add(reader.ReadNameOffset(false, typeof(uint)));
                    }
                }

                reader.SeekBegin(pos);
                for (int i = 0; i < MaterialCount; i++)
                {
                    Material mat = new Material();
                    mat.Read(reader);
                    Materials.Add(mat);

                    var genericMat = new STGenericMaterial();
                    genericMat.Text = $"Material {i}";

                    for (int t = 0; t < mat.TextureIndices.Length; t++)
                    {
                        if(mat.TextureIndices[t] != -1)
                        {
                            Console.WriteLine("TextureIndices " + mat.TextureIndices[t]);
                            string Texture = TextureMaps[mat.TextureIndices[t]];

                            var textureMap = new STGenericMatTexture();
                            textureMap.Name = Texture;
                            genericMat.TextureMaps.Add(textureMap);

                            if (Texture.EndsWith("col.dds"))
                                textureMap.Type = STGenericMatTexture.TextureType.Diffuse;
                            if (Texture.EndsWith("col.mot"))
                                textureMap.Type = STGenericMatTexture.TextureType.Diffuse;
                        }
                    }

                    root.Nodes[0].Nodes.Add(genericMat);
                }


                if (TotalNodeCount != 0)
                {
                    for (int i = 0; i < TotalNodeCount; i++)
                    {
                        reader.SeekBegin(TotalNodeOffset + (i * 8));

                        string NodeName = reader.ReadNameOffset(false, typeof(uint));
                        uint Offset = reader.ReadUInt32();

                        if (Offset != 0)
                        {
                            reader.SeekBegin(Offset);
                            Node node = new Node();
                            node.Name = NodeName;
                            node.Read(reader);
                            TotalNodes.Add(node);
                        }
                    }
                }

                if (BoneCount != 0)
                {
                    for (int i = 0; i < BoneCount; i++)
                    {
                        reader.SeekBegin(BoneOffset + (i * 8));

                        string NodeName = reader.ReadNameOffset(false, typeof(uint));
                        uint Offset = reader.ReadUInt32();

                        if (Offset != 0)
                        {
                            reader.SeekBegin(Offset);
                            Node node = new Node();
                            node.Name = NodeName;
                            node.Read(reader);
                            BoneList.Add(node); //This list is for mapping to meshes
                        }
                    }
                }

                foreach (var node in TotalNodes)
                {
                    // TreeNode lowerWrapper = new TreeNode($"{node.Name}");
                    // root.Nodes[3].Nodes.Add(lowerWrapper);

                    LoadChildern(TotalNodes, node, root.Nodes[2]);
                }

                if (FirstNodeOffset != 0)
                {
                    reader.SeekBegin(FirstNodeOffset);
                    uint NodeOffset = reader.ReadUInt32();
                    reader.SeekBegin(NodeOffset);
                    Node node = new Node();
                    node.Name = GetNodeName(TotalNodes, node);
                    node.Read(reader);

                    LoadBones(TotalNodes, node, root.Nodes[1]);
                }



                if (LinkNodeCount != 0)
                {
                    root.Nodes.Add("Links");

                    for (int i = 0; i < LinkNodeCount; i++)
                    {
                        TreeNode linkWrapper = new TreeNode($"Link {i}");
                        root.Nodes[3].Nodes.Add(linkWrapper);

                        reader.SeekBegin(LinkNodeOffset + (i * 12));

                        uint NodeOffset1 = reader.ReadUInt32();
                        uint NodeOffset2 = reader.ReadUInt32();
                        uint Index = reader.ReadUInt32();

                        reader.SeekBegin(NodeOffset1);
                        Node node1 = new Node();
                        node1.Name = GetNodeName(TotalNodes, node1);
                        node1.Read(reader);

                        reader.SeekBegin(NodeOffset2);
                        Node node2 = new Node();
                        node2.Name = GetNodeName(TotalNodes, node1);
                        node2.Read(reader);

                      //  LoadChildern(TotalNodes, node1, linkWrapper);
                       // LoadChildern(TotalNodes, node2, linkWrapper);

                        LinkNodes.Add(Tuple.Create(node1, node2));
                    }
                }

                Skeleton.update();
                Skeleton.reset();
            }

            private void LoadChildern(List<Node> NodeLookup, Node Node, TreeNode root)
            {
                var NewNode = SetWrapperNode(Node, root);

                for (int i = 0; i < Node.Children.Count; i++)
                {
                    Node.Children[i].Name = GetNodeName(NodeLookup, Node.Children[i]);

                    var newChild = SetWrapperNode(Node.Children[i], NewNode);

                    LoadChildern(NodeLookup, Node.Children[i], newChild);
                }
            }

            private List<SubMesh> AddedMeshes = new List<SubMesh>();


            private void LoadBones(List<Node> NodeLookup, Node Node, TreeNode root)
            {
                var NewNode = SetBoneNode(Node, root);

                for (int i = 0; i < Node.Children.Count; i++)
                {
                    Node.Children[i].Name = GetNodeName(NodeLookup, Node.Children[i]);

                    var newChild = SetBoneNode(Node.Children[i], NewNode);

                    LoadBones(NodeLookup, Node.Children[i], newChild);
                }
            }

            private TreeNode SetBoneNode(Node Node, TreeNode parentNode)
            {
                if (Node.IsBone)
                {
                    STBone boneNode = new STBone(Skeleton);
                    boneNode.RotationType = STBone.BoneRotationType.Euler;
                    boneNode.Checked = true;
                    boneNode.Text = Node.Name;
                    boneNode.position = new float[3];
                    boneNode.scale = new float[3];
                    boneNode.rotation = new float[4];
                    boneNode.position[0] = Node.Translation.X;
                    boneNode.position[1] = Node.Translation.Y;
                    boneNode.position[2] = Node.Translation.Z;
                    boneNode.rotation[0] = Node.Rotation.X;
                    boneNode.rotation[1] = Node.Rotation.Y;
                    boneNode.rotation[2] = Node.Rotation.Z;
                    boneNode.scale[0] = Node.Scale.X;
                    boneNode.scale[1] = Node.Scale.Y;
                    boneNode.scale[2] = Node.Scale.Z;

                    if (Node.IsBone)
                        Skeleton.bones.Add(boneNode);

                    parentNode.Nodes.Add(boneNode);

                    return boneNode;
                }
                return new TreeNode(Node.Name);
            }

            private TreeNode SetWrapperNode(Node Node, TreeNode parentNode)
            {
                if (Node.IsMesh)
                {
                    List<TreeNode> MeshNodes = new List<TreeNode>();
                    int i = 0;
                    foreach (SubMesh subMesh in Node.SubMeshes)
                    {
                        GenericRenderedObject subMeshNode = new GenericRenderedObject();
                        subMeshNode.ImageKey = "mesh";
                        subMeshNode.SelectedImageKey = "mesh";
                        subMeshNode.Checked = true;
                        subMeshNode.Text = $"{Node.Name} {i}";

                        subMeshNode.lodMeshes = new List<GenericRenderedObject.LOD_Mesh>();
                        var submsh = new GenericRenderedObject.LOD_Mesh();
                        submsh.PrimativeType = STPrimativeType.Triangles;
                        submsh.FirstVertex = 0;
                        submsh.faces = subMesh.Faces;
                        subMeshNode.lodMeshes.Add(submsh);

                        subMeshNode.vertices = subMesh.Vertices;

                        //Check duplicate models being rendered
                        if (!AddedMeshes.Contains(subMesh))
                            DrawableRenderer.Meshes.Add(subMeshNode);

                        AddedMeshes.Add(subMesh);

                        if (i == 0)
                            parentNode.Nodes.Add(subMeshNode);
                        else
                            parentNode.Nodes[0].Nodes.Add(subMeshNode);

                        MeshNodes.Add(subMeshNode);
                        i++;
                    }

                    var FirstNode = MeshNodes[0];
                    MeshNodes.Clear();

                    return FirstNode;
                }
                else
                {
                    var NewNode = new TreeNode(Node.Name);
                    parentNode.Nodes.Add(NewNode);
                    return NewNode;
                }
            }

            private static string GetNodeName(List<Node> NodeLookup, Node node)
            {
                for (int i = 0; i < NodeLookup.Count; i++)
                {
                    if (NodeLookup[i].Position == node.Position)
                        return NodeLookup[i].Name;
                }

                return "";
            }
        }

        public class Material
        {
            public Vector4 Ambient;
            public Vector4 Diffuse;
            public Vector4 Specular;
            public Vector4 Ambience;
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
                Ambience = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                Shiny = 50;
                Transparency = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                TextureIndices = new short[10];
            }

            public void Read(FileReader reader)
            {
                Ambient = reader.ReadVec4();
                Diffuse = reader.ReadVec4();
                Specular = reader.ReadVec4();
                Ambience = reader.ReadVec4();

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
                TextureIndices = reader.ReadInt16s(6);
                Unknowns = reader.ReadUInt32s(10);
            }
        }

        public class SubMesh
        {
            public Node ParentNode { get; set; }
            public Material Material { get; set; }

            public List<Vertex> Vertices = new List<Vertex>();
            public List<int> Faces = new List<int>();

            public SubMesh(Node parentNode)
            {
                ParentNode = parentNode;
            }

            public void Read(FileReader reader)
            {
                uint Padding = reader.ReadUInt32();
                uint FaceCount = reader.ReadUInt32();
                uint[] Unknowns = reader.ReadUInt32s(5);
                uint VertexCount = reader.ReadUInt32();
                uint VertexPositionOffset = reader.ReadUInt32();
                uint VertexNormalOffset = reader.ReadUInt32();
                uint UnknownOffset = reader.ReadUInt32();
                uint TexCoord0Offset = reader.ReadUInt32();
                uint TexCoord1Offset = reader.ReadUInt32();
                uint TexCoord2Offset = reader.ReadUInt32();
                uint TexCoord3Offset = reader.ReadUInt32();
                uint FaceOffset = reader.ReadUInt32();
                uint SkinCount = reader.ReadUInt32(); //Unsure
                uint Unknown = reader.ReadUInt32(); //Something related to count
                uint WeightOffset = reader.ReadUInt32();
                uint Unknown2 = reader.ReadUInt32();


                for (int i = 0; i < VertexCount; i++)
                {
                    Vertex vertex = new Vertex();
                    Vertices.Add(vertex);

                    if (VertexPositionOffset != 0)
                    {
                        reader.SeekBegin(VertexPositionOffset + (i * 12));
                        vertex.pos = reader.ReadVec3();
                        vertex.pos = Vector3.TransformPosition(vertex.pos, ParentNode.Transform);
                    }
                    if (VertexNormalOffset != 0)
                    {
                        reader.SeekBegin(VertexNormalOffset + (i * 12));
                        vertex.nrm = reader.ReadVec3();
                        vertex.nrm = Vector3.TransformNormal(vertex.nrm, ParentNode.Transform);
                    }
                    if (TexCoord0Offset != 0)
                    {
                        reader.SeekBegin(TexCoord0Offset + (i * 8));
                        vertex.uv0 = reader.ReadVec2();
                    }
                    if (TexCoord1Offset != 0)
                    {
                        reader.SeekBegin(TexCoord1Offset + (i * 8));
                        vertex.uv1 = reader.ReadVec2();
                    }
                    if (TexCoord2Offset != 0)
                    {
                        reader.SeekBegin(TexCoord2Offset + (i * 8));
                        vertex.uv2 = reader.ReadVec2();
                    }
                    if (TexCoord3Offset != 0)
                    {
                        reader.SeekBegin(TexCoord3Offset + (i * 8));
                    }
                }

                reader.SeekBegin(FaceOffset);
                for (int i = 0; i < FaceCount * 3; i++)
                {
                    Faces.Add(reader.ReadUInt16());
                }
            }
        }

        public class Node
        {
            public string Name { get; set; }

            public bool Visible { get; set; }

            public Vector3 Scale { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Translation { get; set; }
            public byte[] Unknowns;
            public List<Node> Children = new List<Node>();
            public List<SubMesh> SubMeshes = new List<SubMesh>();

            internal long Position;

            public bool IsBone
            {
                get { return SubMeshes.Count == 0; }
            }

            public bool IsMesh
            {
                get { return SubMeshes.Count > 0; }
            }

            public Matrix4 Transform { get; set; }

            public void Read(FileReader reader)
            {
                Position = reader.Position;

                Visible = reader.ReadUInt32() == 1;
                Scale = reader.ReadVec3();
                Rotation = reader.ReadVec3();
                Translation = reader.ReadVec3();
                Unknowns = reader.ReadBytes(16);
                uint SubMeshArrayOffsetPtr = reader.ReadUInt32();
                uint ChildNodeOffsetPtr = reader.ReadUInt32();

                Matrix4 TranslateMat = Matrix4.CreateTranslation(Translation);
                Matrix4 RotXMat = Matrix4.CreateRotationX(Rotation.X);
                Matrix4 RotYMat = Matrix4.CreateRotationY(Rotation.Y);
                Matrix4 RotZMat = Matrix4.CreateRotationZ(Rotation.Z);
                Matrix4 ScaleMat = Matrix4.CreateTranslation(Scale);
                Transform = ScaleMat * (RotXMat * RotYMat * RotZMat) * TranslateMat;

                if (SubMeshArrayOffsetPtr != 0)
                {
                    int i = 0;
                    while (true)
                    {
                        reader.SeekBegin((int)SubMeshArrayOffsetPtr + i * 4);

                        uint SubMeshArrayOffset1 = reader.ReadUInt32();
                        if (SubMeshArrayOffset1 == 0) break;

                        if (SubMeshArrayOffset1 != 0)
                        {
                            reader.SeekBegin(SubMeshArrayOffset1);
                            SubMesh subMesh = new SubMesh(this);
                            subMesh.Read(reader);
                            SubMeshes.Add(subMesh);
                        }

                        i++;
                    }
                }

                if (ChildNodeOffsetPtr != 0)
                {
                    //4 possible sub meshes
                    for (int i = 0; i < 4; i++)
                    {
                        reader.SeekBegin((int)ChildNodeOffsetPtr + i * 4);

                        uint ChildNodeOffset1 = reader.ReadUInt32();
                        if (ChildNodeOffset1 != 0)
                        {
                            reader.SeekBegin(ChildNodeOffset1);
                            Node ChildNode = new Node();
                            ChildNode.Read(reader);
                            Children.Add(ChildNode);
                        }
                    }
                }

                //After repeats a fairly similar structure, with SRT values
                //Unsure what it's used for?
            }
        }
    }
}
