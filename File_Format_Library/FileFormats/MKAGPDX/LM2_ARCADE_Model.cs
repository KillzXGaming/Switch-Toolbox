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
    public class LM2_ARCADE_Model : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Luigi's Masion 2 Arcase Model" };
        public string[] Extension { get; set; } = new string[] { "*.bin", "*.mot" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "VM61");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
              //  types.Add(typeof(MenuExt));
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
                toolExt[0].DropDownItems.Add(new STToolStripItem("Batch Export (LM2A .bin)", BatchExport));
                toolExt[0].DropDownItems.Add(new STToolStripItem("Batch Export as Combined (LM2A .bin)", BatchExportCombined));
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
                        LM2_ARCADE_Model model = new LM2_ARCADE_Model();
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
                        LM2_ARCADE_Model model = new LM2_ARCADE_Model();
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
            public ulong HeaderSize { get; set; }

            public List<Material> Materials = new List<Material>();
            public List<string> TextureMaps = new List<string>();

            public List<Node> BoneList = new List<Node>();
            public List<Tuple<Node, Node>> LinkNodes = new List<Tuple<Node, Node>>(); //Links two nodes. Possbily a mesh to bones for example
            public List<Node> TotalNodes = new List<Node>();

            public STSkeleton Skeleton { get; set; }
            GenericModelRenderer DrawableRenderer { get; set; }

            public void Read(FileReader reader, LM2_ARCADE_Model root)
            {
                Skeleton = new STSkeleton();
                DrawableRenderer = new GenericModelRenderer();
                root.DrawableContainer.Drawables.Add(Skeleton);
                root.DrawableContainer.Drawables.Add(DrawableRenderer);

                reader.ReadSignature(4, "VM61");
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
                ulong MaterialCount = reader.ReadUInt64();
                HeaderSize = reader.ReadUInt64();
                ulong TextureMapsCount = reader.ReadUInt64();
                ulong TextureMapsOffset = reader.ReadUInt64();

                ulong BoneCount = reader.ReadUInt64();
                ulong BoneOffset = reader.ReadUInt64();
                ulong FirstNodeOffset = reader.ReadUInt64(); //Either an offset or the total size of section up to the node
                ulong LinkNodeCount = reader.ReadUInt64();
                ulong LinkNodeOffset = reader.ReadUInt64();
                ulong TotalNodeCount = reader.ReadUInt64();
                ulong TotalNodeOffset = reader.ReadUInt64();
                ulong Padding2 = reader.ReadUInt64();

                root.Nodes.Add("Materials");
                root.Nodes.Add("Root");
                root.Nodes.Add("All Nodes");

                long pos = reader.Position;

                if (TextureMapsOffset != 0)
                {
                    reader.SeekBegin(TextureMapsOffset);
                    for (int i = 0; i < (int)TextureMapsCount; i++)
                    {
                        TextureMaps.Add(reader.ReadNameOffset(false, typeof(ulong)));
                    }
                }

                reader.SeekBegin(pos);
                for (int i = 0; i < (int)MaterialCount; i++)
                {
                    Material mat = new Material();
                    mat.Read(reader);
                    Materials.Add(mat);

                    var genericMat = new STGenericMaterial();
                    genericMat.Text = mat.Name;

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
                    for (int i = 0; i < (int)TotalNodeCount; i++)
                    {
                        reader.SeekBegin((int)TotalNodeOffset + (i * 16));

                        string NodeName = reader.ReadNameOffset(false, typeof(ulong));
                        var Offset = reader.ReadUInt64();

                        Console.WriteLine($"{NodeName}");

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
                    for (int i = 0; i < (int)BoneCount; i++)
                    {
                        reader.SeekBegin((int)BoneOffset + (i * 16));

                        string NodeName = reader.ReadNameOffset(false, typeof(ulong));
                        ulong Offset = reader.ReadUInt64();

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
                    ulong NodeOffset = reader.ReadUInt64();
                    reader.SeekBegin(NodeOffset);
                    Node node = new Node();
                    node.Name = GetNodeName(TotalNodes, node);
                    node.Read(reader);

                    LoadBones(TotalNodes, node, root.Nodes[1]);
                }



                if (LinkNodeCount != 0)
                {
                    root.Nodes.Add("Links");

                    for (int i = 0; i < (int)LinkNodeCount; i++)
                    {
                        TreeNode linkWrapper = new TreeNode($"Link {i}");
                        root.Nodes[3].Nodes.Add(linkWrapper);

                        reader.SeekBegin((int)LinkNodeOffset + (i * 16));

                        ulong NodeOffset1 = reader.ReadUInt64();
                        ulong NodeOffset2 = reader.ReadUInt64();

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
                    boneNode.Position = Node.Translation;
                    boneNode.EulerRotation = Node.Rotation;
                    boneNode.Scale = Node.Scale;

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
                        submsh.PrimativeType = STPrimitiveType.Triangles;
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
            public string Name;

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
                Name = reader.LoadString(false, typeof(ulong));
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
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
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
                var pos = reader.Position;

                uint Padding = reader.ReadUInt32();
                uint FaceCount = reader.ReadUInt32();
                uint[] Unknowns = reader.ReadUInt32s(5);
                uint VertexCount = reader.ReadUInt32();
                ulong VertexPositionOffset = reader.ReadUInt64();
                ulong VertexNormalOffset = reader.ReadUInt64();
                ulong UnknownOffset = reader.ReadUInt64();
                ulong TexCoord0Offset = reader.ReadUInt64();
                ulong TexCoord1Offset = reader.ReadUInt64();
                ulong TexCoord2Offset = reader.ReadUInt64();
                ulong TexCoord3Offset = reader.ReadUInt64();

                reader.SeekBegin(pos + 112);
                ulong FaceOffset = reader.ReadUInt64();
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
                        reader.SeekBegin((int)VertexPositionOffset + (i * 12));
                        vertex.pos = reader.ReadVec3();
                        vertex.pos = Vector3.TransformPosition(vertex.pos, ParentNode.Transform);
                    }
                    if (VertexNormalOffset != 0)
                    {
                        reader.SeekBegin((int)VertexNormalOffset + (i * 12));
                        vertex.nrm = reader.ReadVec3();
                        vertex.nrm = Vector3.TransformNormal(vertex.nrm, ParentNode.Transform);
                    }
                    if (TexCoord0Offset != 0)
                    {
                        reader.SeekBegin((int)TexCoord0Offset + (i * 8));
                        vertex.uv0 = reader.ReadVec2();
                    }
                    if (TexCoord1Offset != 0)
                    {
                        reader.SeekBegin((int)TexCoord1Offset + (i * 8));
                        vertex.uv1 = reader.ReadVec2();
                    }
                    if (TexCoord2Offset != 0)
                    {
                        reader.SeekBegin((int)TexCoord2Offset + (i * 8));
                        vertex.uv2 = reader.ReadVec2();
                    }
                    if (TexCoord3Offset != 0)
                    {
                        reader.SeekBegin((int)TexCoord3Offset + (i * 8));
                    }
                }

                reader.SeekBegin((int)FaceOffset);
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
                if (!Visible) return;
                reader.ReadBytes(28);
                Scale = reader.ReadVec3();
                Rotation = reader.ReadVec3();
                Translation = reader.ReadVec3();
                Unknowns = reader.ReadBytes(20);
                ulong SubMeshArrayOffsetPtr = reader.ReadUInt64();
                ulong unkoff = reader.ReadUInt64();
                ulong ChildNodeOffsetPtr = reader.ReadUInt64();
                reader.ReadUInt32();

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
                        reader.SeekBegin((int)SubMeshArrayOffsetPtr + i * 8);

                        ulong SubMeshArrayOffset1 = reader.ReadUInt64();
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
                    //2 possible children
                    for (int i = 0; i < 2; i++)
                    {
                        reader.SeekBegin((int)ChildNodeOffsetPtr + i * 8);

                        ulong ChildNodeOffset1 = reader.ReadUInt64();
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
