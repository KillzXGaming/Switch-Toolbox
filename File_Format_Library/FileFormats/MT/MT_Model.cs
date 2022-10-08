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
using OpenTK;
using CafeLibrary.M2;
using System.IO;

namespace FirstPlugin
{
    public class MT_Model : TreeNodeFile, IFileFormat, IExportableModel
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "MT Model" };
        public string[] Extension { get; set; } = new string[] { "*.mod" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true)) {
                return reader.CheckSignature(3, "MOD");
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

        //Check for the viewport in the object editor
        //This is attached to it to load multiple file formats within the object editor to the viewer
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
            //Make sure opengl is enabled
            if (Runtime.UseOpenGL)
            {
                //Open the viewport
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                //Make sure to load the drawables only once so set it to true!
                if (!DrawablesLoaded)
                {
                    ObjectEditor.AddContainer(DrawableContainer);
                    DrawablesLoaded = true;
                }

                //Reload which drawable to display
                viewport.ReloadDrawables(DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public IEnumerable<STGenericObject> ExportableMeshes => Model.Objects;
        public IEnumerable<STGenericMaterial> ExportableMaterials => Model.Materials;
        public IEnumerable<STGenericTexture> ExportableTextures => TextureList;
        public STSkeleton ExportableSkeleton => Model.GenericSkeleton;

        public List<MT_TEX> TextureList { get; set; } = new List<MT_TEX>();

        public MT_Renderer Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public Model ModelData { get; set; }

        public STGenericModel Model;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            Renderer = new MT_Renderer();
            ModelData = new Model(stream);
            ModelData.LoadMaterials(this.FilePath.Replace(".mod", ".mrl"));
            ModelData.LoadTextures(Path.GetDirectoryName(this.FilePath));

            TextureList = ModelData.Textures;

            TreeNode meshFolder = new TreeNode("Meshes");
            Nodes.Add(meshFolder);

            TreeNode texFolder = new STTextureFolder("Textures");
            Nodes.Add(texFolder);

            TreeNode skeletonFolder = new STTextureFolder("Skeleton");
            Nodes.Add(skeletonFolder);

            foreach (var tex in ModelData.Textures)
                texFolder.Nodes.Add(tex);

            Model = ToGeneric();

            foreach (GenericRenderedObject mesh in Model.Objects)
            {
                Renderer.Meshes.Add(mesh);
                meshFolder.Nodes.Add(mesh);
            }
            Renderer.Skeleton = Model.GenericSkeleton;

            foreach (var bone in Model.GenericSkeleton.bones)
            {
                if (bone.Parent == null)
                    skeletonFolder.Nodes.Add(bone);
            }

            foreach (var tex in TextureList)
                Renderer.Textures.Add(tex);

            DrawableContainer = new DrawableContainer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);
            DrawableContainer.Drawables.Add(Model.GenericSkeleton);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public STGenericModel ToGeneric()
        {
            var model = new STGenericModel();
            model.Name = this.FileName;

            model.GenericSkeleton = new STSkeleton();
            foreach (var bn in ModelData.Bones)
            {
                model.GenericSkeleton.bones.Add(new STBone(model.GenericSkeleton)
                {
                    Text = $"Bone_{bn.ID}",
                    Position = bn.Position,
                    Rotation = bn.LocalMatrix.ExtractRotation(),
                    Scale = bn.LocalMatrix.ExtractScale(),
                    parentIndex = bn.ParentIndex,
                });
            }
            model.GenericSkeleton.reset();
            model.GenericSkeleton.update();

            List<STGenericMaterial> materials = new List<STGenericMaterial>();
            for (int i = 0; i < ModelData.MaterialNames.Length; i++)
            {
                STGenericMaterial mat = new STGenericMaterial();
                mat.Text = ModelData.MaterialNames[i];

                var hash = MT_Globals.Hash(mat.Text);

                if (ModelData.MaterialList != null && ModelData.MaterialList.Materials.ContainsKey(hash))
                {
                    var m = ModelData.MaterialList.Materials[hash];
                    if (m.DiffuseMap != null)
                    {
                        string tex = Path.GetFileName(m.DiffuseMap.Name);
                        mat.TextureMaps.Add(new STGenericMatTexture()
                        {
                            Type = STGenericMatTexture.TextureType.Diffuse,
                            Name = tex,
                        });
                    }
                    if (m.NormalMap != null)
                    {
                        string tex = Path.GetFileName(m.NormalMap.Name);
                        mat.TextureMaps.Add(new STGenericMatTexture()
                        {
                            Type = STGenericMatTexture.TextureType.Normal,
                            Name = tex,
                        });
                    }
                    if (m.SpecularMap != null)
                    {
                        string tex = Path.GetFileName(m.SpecularMap.Name);
                        mat.TextureMaps.Add(new STGenericMatTexture()
                        {
                            Type = STGenericMatTexture.TextureType.Specular,
                            Name = tex,
                        });
                    }
                }

                materials.Add(mat);
            }

            model.Materials = materials;

            List<STGenericObject> meshes = new List<STGenericObject>();
            foreach (var mesh in ModelData.Meshes)
            {
                if (mesh.Vertices.Length == 0)
                    continue;

                var attributeGroup = MT_Globals.AttributeGroups[mesh.VertexFormatHash];

                GenericRenderedObject genericMesh = new GenericRenderedObject();
                meshes.Add(genericMesh);

                genericMesh.ImageKey = "mesh";
                genericMesh.SelectedImageKey = "mesh";

                genericMesh.Text = $"Mesh_{meshes.Count - 1}";
                foreach (var vert in mesh.Vertices)
                {
                    var genericVertex = new Vertex()
                    {
                        pos = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z),
                        nrm = new Vector3(vert.Normal.X, vert.Normal.Y, vert.Normal.Z),
                        col = new Vector4(vert.Color.X, vert.Color.Y, vert.Color.Z, vert.Color.W),
                    };
                    if (vert.TexCoords?.Length > 0) genericVertex.uv0 = vert.TexCoords[0];
                    if (vert.TexCoords?.Length > 1) genericVertex.uv1 = vert.TexCoords[1];
                    if (vert.TexCoords?.Length > 2) genericVertex.uv2 = vert.TexCoords[2];

                    foreach (var boneID in vert.BoneIndices)
                    {
                        genericVertex.boneIds.Add(boneID);
                    }
                    foreach (var boneW in vert.BoneWeights)
                        genericVertex.boneWeights.Add(boneW);

                    genericMesh.vertices.Add(genericVertex);
                }

                STGenericPolygonGroup poly = new STGenericPolygonGroup();
                poly.PrimativeType = STPrimitiveType.Triangles;
                genericMesh.PolygonGroups.Add(poly);

                poly.Material = materials[(int)mesh.MaterialID];

                foreach (var ind in mesh.Indices)
                    poly.faces.Add(ind);

                genericMesh.CalculateNormals();
            }
            model.Objects = meshes.ToList();
            return model;
        }
    }
}
