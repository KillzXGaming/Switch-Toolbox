using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.Rendering;
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
            }

            public void BatchExport(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
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
                                      System.IO.Path.GetFileName(file));

                        model.ExportModel(Path);
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
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export Model", null, ExportModelAction, Keys.Control | Keys.E));
        }

        private void ExportModelAction(object sender, EventArgs args) {
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
            foreach (STGenericMaterial mat in Nodes[0].Nodes)
                Materials.Add(mat);

            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = ((Renderer)DrawableContainer.Drawables[1]).Meshes;

            assimp.SaveFromModel(model, FileName, new List<STGenericTexture>(), ((STSkeleton)DrawableContainer.Drawables[0]));
        }

        public void Unload()
        {
            DrawableContainer.Drawables.Clear();
            DrawableContainer = null;
            header.Materials.Clear();
            header.TextureMaps.Clear();
            header.LowerNodes.Clear();
            header.UpperNodes.Clear();
            Nodes.Clear();
        }
        public byte[] Save()
        {
            return null;
        }

        public struct DisplayVertex
        {
            // Used for rendering.
            public Vector3 pos;
            public Vector3 nrm;
            public Vector3 tan;
            public Vector2 uv;
            public Vector4 col;
            public Vector4 node;
            public Vector4 weight;
            public Vector2 uv2;
            public Vector2 uv3;

            public static int Size = 4 * (3 + 3 + 3 + 2 + 4 + 4 + 4 + 2 + 2);
        }


        public class MeshWrapper : STGenericObject
        {
            public int[] display;
            public int DisplayId;

            public List<DisplayVertex> CreateDisplayVertices()
            {
                display = lodMeshes[DisplayLODIndex].getDisplayFace().ToArray();

                 List<DisplayVertex> displayVertList = new List<DisplayVertex>();

                if (lodMeshes[DisplayLODIndex].faces.Count <= 3)
                    return displayVertList;

                foreach (Vertex v in vertices)
                {
                    DisplayVertex displayVert = new DisplayVertex()
                    {
                        pos = v.pos,
                        nrm = v.nrm,
                        tan = v.tan.Xyz,
                        col = v.col,
                        uv = v.uv0,
                        uv2 = v.uv1,
                        uv3 = v.uv2,
                        node = new Vector4(
                                 v.boneIds.Count > 0 ? v.boneIds[0] : -1,
                                 v.boneIds.Count > 1 ? v.boneIds[1] : -1,
                                 v.boneIds.Count > 2 ? v.boneIds[2] : -1,
                                 v.boneIds.Count > 3 ? v.boneIds[3] : -1),
                        weight = new Vector4(
                                 v.boneWeights.Count > 0 ? v.boneWeights[0] : 0,
                                 v.boneWeights.Count > 1 ? v.boneWeights[1] : 0,
                                 v.boneWeights.Count > 2 ? v.boneWeights[2] : 0,
                                 v.boneWeights.Count > 3 ? v.boneWeights[3] : 0),
                    };

                    displayVertList.Add(displayVert);

                      /*    Console.WriteLine($"---------------------------------------------------------------------------------------");
                           Console.WriteLine($"Position   {displayVert.pos.X} {displayVert.pos.Y} {displayVert.pos.Z}");
                           Console.WriteLine($"Normal     {displayVert.nrm.X} {displayVert.nrm.Y} {displayVert.nrm.Z}");
                           Console.WriteLine($"Tanget     {displayVert.tan.X} {displayVert.tan.Y} {displayVert.tan.Z}");
                           Console.WriteLine($"Color      {displayVert.col.X} {displayVert.col.Y} {displayVert.col.Z} {displayVert.col.W}");
                           Console.WriteLine($"UV Layer 1 {displayVert.uv.X} {displayVert.uv.Y}");
                           Console.WriteLine($"UV Layer 2 {displayVert.uv2.X} {displayVert.uv2.Y}");
                           Console.WriteLine($"UV Layer 3 {displayVert.uv3.X} {displayVert.uv3.Y}");
                           Console.WriteLine($"Bone Index {displayVert.node.X} {displayVert.node.Y} {displayVert.node.Z} {displayVert.node.W}");
                           Console.WriteLine($"Weights    {displayVert.weight.X} {displayVert.weight.Y} {displayVert.weight.Z} {displayVert.weight.W}");
                           Console.WriteLine($"---------------------------------------------------------------------------------------");*/
                }

                return displayVertList;
            }

        }

        public class Renderer : AbstractGlDrawable
        {
            public Vector3 Max = new Vector3(0);
            public Vector3 Min = new Vector3(0);

            public List<ushort> SelectedTypes = new List<ushort>();

            public Vector3 position = new Vector3(0, 0, 0);

            protected bool Selected = false;
            protected bool Hovered = false;

            // public override bool IsSelected() => Selected;
            //  public override bool IsSelected(int partIndex) => Selected;

            public bool IsHovered() => Selected;

            // gl buffer objects
            int vbo_position;
            int ibo_elements;

            public List<MeshWrapper> Meshes = new List<MeshWrapper>();

            private void GenerateBuffers()
            {
                GL.GenBuffers(1, out vbo_position);
                GL.GenBuffers(1, out ibo_elements);
            }

            public void Destroy()
            {
                GL.DeleteBuffer(vbo_position);
                GL.DeleteBuffer(ibo_elements);
            }

            public void UpdateVertexData()
            {
                if (!Runtime.OpenTKInitialized)
                    return;

                DisplayVertex[] Vertices;
                int[] Faces;

                int poffset = 0;
                int voffset = 0;
                List<DisplayVertex> Vs = new List<DisplayVertex>();
                List<int> Ds = new List<int>();
                foreach (var m in Meshes)
                {
                    m.Offset = poffset * 4;
                    List<DisplayVertex> pv = m.CreateDisplayVertices();
                    Vs.AddRange(pv);

                    for (int i = 0; i < m.lodMeshes[m.DisplayLODIndex].displayFaceSize; i++)
                    {
                        Ds.Add(m.display[i] + voffset);
                    }
                    poffset += m.lodMeshes[m.DisplayLODIndex].displayFaceSize;
                    voffset += pv.Count;
                }

                // Binds
                Vertices = Vs.ToArray();
                Faces = Ds.ToArray();

                // Bind only once!
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
                GL.BufferData<DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
                GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

                LibraryGUI.Instance.UpdateViewport();
            }

            public ShaderProgram defaultShaderProgram;

            public override void Prepare(GL_ControlModern control)
            {
                string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "MKAGPDX") + "\\MKAGPDX_Model.frag";
                string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "MKAGPDX") + "\\MKAGPDX_Model.vert";

                var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
                var defaultVert = new VertexShader(File.ReadAllText(pathVert));

                defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert, control);
            }

            public override void Prepare(GL_ControlLegacy control)
            {
            }

            private void CheckBuffers()
            {
                if (!Runtime.OpenTKInitialized)
                    return;

                bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
                if (!buffersWereInitialized)
                {
                    GenerateBuffers();
                    UpdateVertexData();
                }
            }
            public override void Draw(GL_ControlLegacy control, Pass pass)
            {
                CheckBuffers();

                if (!Runtime.OpenTKInitialized)
                    return;
            }

            public override void Draw(GL_ControlModern control, Pass pass)
            {
                CheckBuffers();

                if (!Runtime.OpenTKInitialized)
                    return;

                control.CurrentShader = defaultShaderProgram;
                SetRenderSettings(defaultShaderProgram);

                Matrix4 camMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

                GL.Disable(EnableCap.CullFace);

                GL.Uniform3(defaultShaderProgram["difLightDirection"], Vector3.TransformNormal(new Vector3(0f, 0f, -1f), camMat.Inverted()).Normalized());
                GL.Uniform3(defaultShaderProgram["difLightColor"], new Vector3(1));
                GL.Uniform3(defaultShaderProgram["ambLightColor"], new Vector3(1));

                defaultShaderProgram.EnableVertexAttributes();

                foreach (var mdl in Meshes)
                {
                    DrawModel(mdl, defaultShaderProgram);
                }

                defaultShaderProgram.DisableVertexAttributes();

                GL.UseProgram(0);
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
            }
            private void SetRenderSettings(ShaderProgram shader)
            {
                shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
                GL.Uniform1(defaultShaderProgram["renderType"], (int)Runtime.viewportShading);

            }
            private void DrawModel(MeshWrapper m, ShaderProgram shader, bool drawSelection = false)
            {
                if (m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                    return;

                SetVertexAttributes(m, shader);

                if (m.Checked)
                {
                    if ((m.IsSelected))
                    {
                        DrawModelSelection(m, shader);
                    }
                    else
                    {
                        if (Runtime.RenderModelWireframe)
                        {
                            DrawModelWireframe(m, shader);
                        }

                        if (Runtime.RenderModels)
                        {
                            GL.DrawElements(PrimitiveType.Triangles, m.lodMeshes[m.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                        }
                    }
                }
            }
            private static void DrawModelSelection(MeshWrapper p, ShaderProgram shader)
            {
                //This part needs to be reworked for proper outline. Currently would make model disappear

                GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);

                GL.Enable(EnableCap.StencilTest);
                // use vertex color for wireframe color
                GL.Uniform1(shader["colorOverride"], 1);
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(1.5f);
                GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Uniform1(shader["colorOverride"], 0);

                GL.Enable(EnableCap.DepthTest);
            }
            private void SetVertexAttributes(MeshWrapper m, ShaderProgram shader)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
                GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 0); //+12
                GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 12); //+12
                GL.VertexAttribPointer(shader.GetAttribute("vTangent"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 24); //+12
                GL.VertexAttribPointer(shader.GetAttribute("vUV0"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 36); //+8
                GL.VertexAttribPointer(shader.GetAttribute("vColor"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 44); //+16
                GL.VertexAttribIPointer(shader.GetAttribute("vBone"), 4, VertexAttribIntegerType.Int, DisplayVertex.Size, new IntPtr(60)); //+16
                GL.VertexAttribPointer(shader.GetAttribute("vWeight"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 76);//+16
                GL.VertexAttribPointer(shader.GetAttribute("vUV1"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 92);//+8
                GL.VertexAttribPointer(shader.GetAttribute("vUV2"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 100);//+8
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            }
            private static void DrawModelWireframe(MeshWrapper p, ShaderProgram shader)
            {
                // use vertex color for wireframe color
                GL.Uniform1(shader["colorOverride"], 1);
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(1.5f);
                GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Uniform1(shader["colorOverride"], 0);
            }
        }

        Viewport viewport
        {
            get
            {
                var editor = LibraryGUI.Instance.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.Instance.GetObjectEditor();
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
                LibraryGUI.Instance.LoadEditor(viewport);

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

            public List<Node> UpperNodes = new List<Node>();
            public Tuple<Node, Node> LinkNodes; //Links two nodes for some reason
            public List<Node> LowerNodes = new List<Node>();

            public STSkeleton Skeleton { get; set; }
            Renderer DrawableRenderer { get; set; }

            public void Read(FileReader reader, MKAGPDX_Model root)
            {
                Skeleton = new STSkeleton();
                DrawableRenderer = new Renderer();
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

                root.Nodes.Add("Materials");

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

                if (LowerLevelNodeCount != 0)
                {
                    for (int i = 0; i < LowerLevelNodeCount; i++)
                    {
                        reader.SeekBegin(LowerLevelNodeOffset + (i * 8));

                        string NodeName = reader.ReadNameOffset(false, typeof(uint));
                        uint Offset = reader.ReadUInt32();

                        if (Offset != 0)
                        {
                            reader.SeekBegin(Offset);
                            Node node = new Node();
                            node.Name = NodeName;
                            node.Read(reader);
                            LowerNodes.Add(node);
                        }
                    }
                }

                if (FirstNodeOffset != 0)
                {
                    reader.SeekBegin(FirstNodeOffset);
                    uint NodeOffset = reader.ReadUInt32();
                    reader.SeekBegin(NodeOffset);
                    Node node = new Node();
                    node.Name = GetNodeName(LowerNodes, node);
                    node.Read(reader);

                    LoadChildern(LowerNodes, node, root);
                }

                Skeleton.update();
                Skeleton.reset();


            }

            private void LoadChildern(List<Node> NodeLookup, Node Node, TreeNode root)
            {
                var NewNode = SetWrapperNode(Node);
                root.Nodes.Add(NewNode);

                for (int i = 0; i < Node.Children.Count; i++)
                {
                    Node.Children[i].Name = GetNodeName(NodeLookup, Node.Children[i]);

                    var newChild = SetWrapperNode(Node.Children[i]);
                    NewNode.Nodes.Add(newChild);

                    LoadChildern(NodeLookup, Node.Children[i], newChild);
                }
            }

            private TreeNode SetWrapperNode(Node Node)
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

                    return boneNode;
                }
                else if (Node.IsMesh)
                {
                    MeshWrapper meshNode = new MeshWrapper();
                    meshNode.ImageKey = "mesh";
                    meshNode.SelectedImageKey = "mesh";

                    int i = 0;
                    meshNode.lodMeshes = new List<MeshWrapper.LOD_Mesh>();
                    var msh = new MeshWrapper.LOD_Mesh();
                    msh.PrimitiveType = STPolygonType.Triangle;
                    msh.FirstVertex = 0;
                    msh.faces = Node.SubMeshes[0].Faces;
                    meshNode.vertices = Node.SubMeshes[0].Vertices;

                    meshNode.lodMeshes.Add(msh);

                    foreach (SubMesh subMesh in Node.SubMeshes)
                    {
                        if (i > 0)
                        {
                            MeshWrapper subMeshNode = new MeshWrapper();
                            subMeshNode.ImageKey = "mesh";
                            subMeshNode.SelectedImageKey = "mesh";

                            subMeshNode.lodMeshes = new List<MeshWrapper.LOD_Mesh>();
                            var submsh = new MeshWrapper.LOD_Mesh();
                            submsh.PrimitiveType = STPolygonType.Triangle;
                            submsh.FirstVertex = 0;
                            submsh.faces = subMesh.Faces;
                            subMeshNode.lodMeshes.Add(submsh);

                            subMeshNode.vertices = subMesh.Vertices;
                            DrawableRenderer.Meshes.Add(subMeshNode);
                        }

                        i++;
                    }

                    meshNode.Checked = true;
                    meshNode.Text = Node.Name;
                    DrawableRenderer.Meshes.Add(meshNode);

                    return meshNode;
                }
                else
                    return new TreeNode(Node.Name);
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
                uint Unknown1Offset = reader.ReadUInt32();
                uint Unknown2Offset = reader.ReadUInt32();
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
                get { return Name.Contains("jnt") || Name.Contains("center"); }
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
                    //4 possible sub meshes
                    reader.SeekBegin(SubMeshArrayOffsetPtr);
                    uint SubMeshArrayOffset1 = reader.ReadUInt32();
                    uint SubMeshArrayOffset2 = reader.ReadUInt32();
                    uint SubMeshArrayOffset3 = reader.ReadUInt32();
                    uint SubMeshArrayOffset4 = reader.ReadUInt32();
                    if (SubMeshArrayOffset1 != 0)
                    {
                        reader.SeekBegin(SubMeshArrayOffset1);
                        SubMesh subMesh = new SubMesh(this);
                        subMesh.Read(reader);
                        SubMeshes.Add(subMesh);
                    }
                    if (SubMeshArrayOffset2 != 0)
                    {
                        reader.SeekBegin(SubMeshArrayOffset2);
                        SubMesh subMesh = new SubMesh(this);
                        subMesh.Read(reader);
                        SubMeshes.Add(subMesh);
                    }
                    if (SubMeshArrayOffset3 != 0)
                    {
                        reader.SeekBegin(SubMeshArrayOffset3);
                        SubMesh subMesh = new SubMesh(this);
                        subMesh.Read(reader);
                        SubMeshes.Add(subMesh);
                    }
                    if (SubMeshArrayOffset4 != 0)
                    {
                        reader.SeekBegin(SubMeshArrayOffset4);
                        SubMesh subMesh = new SubMesh(this);
                        subMesh.Read(reader);
                        SubMeshes.Add(subMesh);
                    }
                }

                if (ChildNodeOffsetPtr != 0)
                {
                    //4 possible children
                    reader.SeekBegin(ChildNodeOffsetPtr);
                    uint ChildNodeOffset1 = reader.ReadUInt32();
                    uint ChildNodeOffset2 = reader.ReadUInt32();
                    uint ChildNodeOffset3 = reader.ReadUInt32();
                    uint ChildNodeOffset4 = reader.ReadUInt32();
                    if (ChildNodeOffset1 != 0)
                    {
                        reader.SeekBegin(ChildNodeOffset1);
                        Node ChildNode = new Node();
                        ChildNode.Read(reader);
                        Children.Add(ChildNode);
                    }
                    if (ChildNodeOffset2 != 0)
                    {
                        reader.SeekBegin(ChildNodeOffset2);
                        Node ChildNode = new Node();
                        ChildNode.Read(reader);
                        Children.Add(ChildNode);
                    }
                    if (ChildNodeOffset3 != 0)
                    {
                        reader.SeekBegin(ChildNodeOffset3);
                        Node ChildNode = new Node();
                        ChildNode.Read(reader);
                        Children.Add(ChildNode);
                    }
                    if (ChildNodeOffset4 != 0)
                    {
                        reader.SeekBegin(ChildNodeOffset4);
                        Node ChildNode = new Node();
                        ChildNode.Read(reader);
                        Children.Add(ChildNode);
                    }
                }

                //After repeats a fairly similar structure, with SRT values
                //Unsure what it's used for?
            }
        }
    }
}
