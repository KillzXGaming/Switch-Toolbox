using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using SuperBMDLib;
using System.Drawing;
using SuperBMDLib.Rigging;
using SuperBMDLib.Geometry.Enums;
using SuperBMDLib.Util;
using OpenTK;

namespace FirstPlugin
{
    public class BMD : TreeNodeFile, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Gamecube/Wii Binary Model (BMD/BDL)" };
        public string[] Extension { get; set; } = new string[] { "*.bmd", "*.bdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.SetByteOrder(true);
                bool IsBMD = reader.ReadUInt32() == 0x4A334432;
                reader.Position = 0;

                return IsBMD;
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
            if (Runtime.UseOpenGL && !Runtime.UseLegacyGL)
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

        public BMD_Renderer Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public Model BMDFile;
        public STSkeleton Skeleton;
        private BMDTextureFolder TextureFolder;
        private TreeNode ShapeFolder;
        private TreeNode MaterialFolder;
        private TreeNode SkeletonFolder;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = true;

            //Set renderer
            Renderer = new BMD_Renderer();
            Skeleton = new STSkeleton();

            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);
            DrawableContainer.Drawables.Add(Skeleton);

            BMDFile = Model.Load(stream);
            LoadBMD(BMDFile);
        }

        private class BMDTextureFolder : STTextureFolder, ITextureIconLoader
        {
            public List<STGenericTexture> IconTextureList
            {
                get
                {
                    List<STGenericTexture> textures = new List<STGenericTexture>();
                    foreach (STGenericTexture node in Nodes)
                        textures.Add(node);

                    return textures;
                }
                set { }
            }

            public BMDTextureFolder(string text) : base(text)
            {

            }
        }

        private void LoadBMD(Model model)
        {
            Nodes.Clear();

            ShapeFolder = new TreeNode("Shapes");
            SkeletonFolder = new TreeNode("Skeleton");
            MaterialFolder = new TreeNode("Materials");
            TextureFolder = new BMDTextureFolder("Textures");
            Nodes.Add(ShapeFolder);
            Nodes.Add(MaterialFolder);
            Nodes.Add(SkeletonFolder);
            Nodes.Add(TextureFolder);

            BMDFile = model;

            FillSkeleton(BMDFile.Scenegraph, Skeleton, BMDFile.Joints.FlatSkeleton);

            foreach (var bone in Skeleton.bones)
            {
                if (bone.Parent == null)
                    SkeletonFolder.Nodes.Add(bone);

            }

            for (int i = 0; i < BMDFile.Shapes.Shapes.Count; i++)
            {
                var curShape = BMDFile.Shapes.Shapes[i];

                var mat = new BMDMaterialWrapper(BMDFile.Materials.GetMaterial(i), BMDFile);
                MaterialFolder.Nodes.Add(mat);

                var shpWrapper = new BMDShapeWrapper(curShape, BMDFile, mat);
                shpWrapper.Text = $"Shape {i}";
                ShapeFolder.Nodes.Add(shpWrapper);
                Renderer.Meshes.Add(shpWrapper);

                var polyGroup = new STGenericPolygonGroup();
                shpWrapper.PolygonGroups.Add(polyGroup);

                var VertexAttributes = BMDFile.VertexData.Attributes;

                int vertexID = 0;
                int packetID = 0;

                foreach (var att in curShape.Descriptor.Attributes)
                    shpWrapper.Nodes.Add($"Attribute {att.Key} {att.Value.Item1}");

                foreach (SuperBMDLib.Geometry.Packet pack in curShape.Packets)
                {
                    int primID = 0;
                    foreach (SuperBMDLib.Geometry.Primitive prim in pack.Primitives)
                    {
                        List<SuperBMDLib.Geometry.Vertex> triVertices = J3DUtility.PrimitiveToTriangles(prim);
                        for (int triIndex = 0; triIndex < triVertices.Count; triIndex += 3)
                        {
                            polyGroup.faces.AddRange(new int[] { vertexID + 2, vertexID + 1, vertexID });

                            for (int triVertIndex = 0; triVertIndex < 3; triVertIndex++)
                            {
                                SuperBMDLib.Geometry.Vertex vert = triVertices[triIndex + triVertIndex];

                                Vertex vertex = new Vertex();
                                vertex.pos = VertexAttributes.Positions[(int)vert.GetAttributeIndex(GXVertexAttribute.Position)];
                                shpWrapper.vertices.Add(vertex);

                                if (curShape.Descriptor.CheckAttribute(GXVertexAttribute.Normal))
                                    vertex.nrm = VertexAttributes.Normals[(int)vert.NormalIndex];
                                if (curShape.Descriptor.CheckAttribute(GXVertexAttribute.Color0))
                                {
                                    var color0 = VertexAttributes.Color_0[(int)vert.Color0Index];
                                    vertex.col = new OpenTK.Vector4(color0.R, color0.G, color0.B, color0.A);
                                }

                                for (int j = 0; j < vert.VertexWeight.WeightCount; j++)
                                {
                                    vertex.boneWeights.Add(vert.VertexWeight.Weights[j]);
                                    vertex.boneIds.Add(vert.VertexWeight.BoneIndices[j]);
                                }

                                if (vert.VertexWeight.WeightCount == 1)
                                {
                                    if (BMDFile.SkinningEnvelopes.InverseBindMatrices.Count > vert.VertexWeight.BoneIndices[0])
                                    {
                                        Matrix4 test = BMDFile.SkinningEnvelopes.InverseBindMatrices[vert.VertexWeight.BoneIndices[0]].Inverted();
                                        test.Transpose();
                                        vertex.pos = OpenTK.Vector3.TransformPosition(vertex.pos, test);
                                        vertex.nrm = OpenTK.Vector3.TransformNormal(vertex.nrm, test);
                                    }
                                    else
                                    {
                                        vertex.pos = OpenTK.Vector3.TransformPosition(vertex.pos, BMDFile.Joints.FlatSkeleton[vert.VertexWeight.BoneIndices[0]].TransformationMatrix);
                                        vertex.nrm = OpenTK.Vector3.TransformNormal(vertex.nrm, BMDFile.Joints.FlatSkeleton[vert.VertexWeight.BoneIndices[0]].TransformationMatrix);
                                    }
                                }

                                for (int texCoordNum = 0; texCoordNum < 8; texCoordNum++)
                                {
                                    if (curShape.Descriptor.CheckAttribute(GXVertexAttribute.Tex0 + texCoordNum))
                                    {
                                        switch (texCoordNum)
                                        {
                                            case 0:
                                                vertex.uv0 = VertexAttributes.TexCoord_0[(int)vert.TexCoord0Index];
                                                break;
                                            case 1:
                                                vertex.uv1 = VertexAttributes.TexCoord_0[(int)vert.TexCoord0Index];
                                                break;
                                            case 2:
                                                vertex.uv2 = VertexAttributes.TexCoord_0[(int)vert.TexCoord0Index];
                                                break;
                                        }
                                    }
                                }

                                vertexID++;
                            }
                        }

                        primID++;
                    }

                    packetID++;
                }
            }

            CorrectMaterialIndices(Renderer.Meshes, BMDFile.Scenegraph, BMDFile.Materials);

            for (int i = 0; i < BMDFile.Textures.Textures.Count; i++)
            {
                var texWrapper = new BMDTextureWrapper(BMDFile.Textures.Textures[i]);
                TextureFolder.Nodes.Add(texWrapper);
                Renderer.TextureList.Add(texWrapper);
            }
        }

        public void CorrectMaterialIndices(List<GenericRenderedObject> Meshes, SuperBMDLib.BMD.INF1 INF1, SuperBMDLib.BMD.MAT3 materials)
        {
            foreach (SuperBMDLib.Scenegraph.SceneNode node in INF1.FlatNodes)
            {
                if (node.Type == SuperBMDLib.Scenegraph.Enums.NodeType.Shape)
                {
                    if (node.Index < Meshes.Count)
                    {
                        int matIndex = node.Parent.Index;
                        ((BMDShapeWrapper)Meshes[node.Index]).SetMaterial((STGenericMaterial)MaterialFolder.Nodes[matIndex]);
                    }
                }
            }
        }

        public void FillSkeleton(SuperBMDLib.BMD.INF1 INF1, STSkeleton skeleton, List<SuperBMDLib.Rigging.Bone> flatSkeleton)
        {
            for (int i = 1; i < INF1.FlatNodes.Count; i++)
            {
                SuperBMDLib.Scenegraph.SceneNode curNode = INF1.FlatNodes[i];

                if (curNode.Type == SuperBMDLib.Scenegraph.Enums.NodeType.Joint)
                {
                    var Bone = flatSkeleton[curNode.Index];

                    var stBone = new STBone(skeleton);
                    stBone.Text = Bone.Name;
                    stBone.FromTransform(Bone.TransformationMatrix);

                    if (Bone.Parent != null)
                        stBone.parentIndex = flatSkeleton.IndexOf(Bone.Parent);
                    else
                        stBone.parentIndex = -1;

                    skeleton.bones.Add(stBone);
                }
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new STToolStipMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            Items.Add(new STToolStripSeparator());
            Items.Add(new STToolStipMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
            Items.Add(new STToolStipMenuItem("Replace", null, ReplaceAction, Keys.Control | Keys.R));
            return Items.ToArray();
        }

        private void ExportAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Collada DAE |*.dae;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                BMDFile.ExportAssImp(sfd.FileName, "dae", new ExportSettings());
            }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Collada DAE |*.dae;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                BMDModelImportSettings settings = new BMDModelImportSettings();
                if (settings.ShowDialog() == DialogResult.OK)
                {
                    Arguments arguments = new Arguments();
                    arguments.input_path = ofd.FileName;
                    arguments.texheaders_path = settings.TexturePath;
                    arguments.materials_path = settings.MaterialPath;

                    var model = Model.Load(arguments);
                    LoadBMD(model);
                }
            }
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            BMDFile.ExportBMD(mem);
            return mem.ToArray();
        }
    }
}
