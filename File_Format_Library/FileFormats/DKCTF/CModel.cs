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

namespace DKCTF
{
    public class CModel : TreeNodeFile, IFileFormat, IExportableModel
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CMDL" };
        public string[] Extension { get; set; } = new string[] { "*.cmdl", "*.smdl", "*.wmdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                bool IsForm = reader.CheckSignature(4, "RFRM");
                bool FormType = reader.CheckSignature(4, "CMDL", 20) ||
                                reader.CheckSignature(4, "WMDL", 20) ||
                                reader.CheckSignature(4, "SMDL", 20);

                return IsForm && FormType;
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
        public IEnumerable<STGenericTexture> ExportableTextures => Renderer.Textures;
        public STSkeleton ExportableSkeleton => Model.GenericSkeleton;

        public GenericModelRenderer Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public List<STGenericTexture> TextureList = new List<STGenericTexture>();

        public CMDL ModelData { get; set; }

        public STGenericModel Model;

        public SKEL SkeletonData;

        public void LoadSkeleton(SKEL skeleton)
        {
            SkeletonData = skeleton;
            Model = ToGeneric();

            Model.GenericSkeleton = SkeletonData.ToGenericSkeleton();

            DrawableContainer.Drawables.Add(Model.GenericSkeleton);

            foreach (var bone in Model.GenericSkeleton.bones)
            {
                if (bone.Parent == null)
                    skeletonFolder.Nodes.Add(bone);
            }

            Renderer.Skeleton = Model.GenericSkeleton;
            //Reload meshes with updated bone IDs
            Renderer.Meshes.Clear();
            foreach (GenericRenderedObject mesh in Model.Objects)
                Renderer.Meshes.Add(mesh);
        }

        public void LoadTextures(Dictionary<string, FileEntry> Textures)
        {
            texFolder.Nodes.Clear();
            foreach (var mat in ModelData.Materials)
            {
                foreach (var texMap in mat.Textures)
                {
                    string guid = texMap.Value.FileID.ToString();
                    if (texFolder.Nodes.ContainsKey(guid) || !Textures.ContainsKey(guid))
                    {
                        Console.WriteLine($"Texture not present in pak file! {mat.Name} {texMap.Key} {guid}");
                        continue;
                    }
                    if (Textures[guid].FileFormat == null)
                        Textures[guid].OpenFile();

                    var tex = Textures[guid].FileFormat as CTexture;

                    TreeNode t = new TreeNode(guid);
                    t.ImageKey = "texture";
                    t.SelectedImageKey = "texture";
                    t.Tag = tex;
                    texFolder.Nodes.Add(t);

                    if (texMap.Key == "DIFT" || texMap.Key == "BCLR")
                    {
                        tex.Text = guid;

                        if (tex.RenderableTex == null)
                            tex.LoadOpenGLTexture();
                        Renderer.Textures.Add(tex);
                    }
                }
            }
        }

        TreeNode texFolder = new STTextureFolder("Textures");
        TreeNode skeletonFolder = new STTextureFolder("Skeleton");
        TreeNode shaderFolder = new STTextureFolder("Materials");

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            Renderer = new GenericModelRenderer();
            ModelData = new CMDL(stream);

            TreeNode meshFolder = new TreeNode("Meshes");
            Nodes.Add(meshFolder);

            Nodes.Add(texFolder);
            Nodes.Add(skeletonFolder);
            Nodes.Add(shaderFolder);

            Model = ToGeneric();

            foreach (var mat in ModelData.Materials)
                shaderFolder.Nodes.Add(new TreeNode(mat.Name + "_" + mat.ID.ToString()));

            foreach (GenericRenderedObject mesh in Model.Objects)
            {
                Renderer.Meshes.Add(mesh);
                meshFolder.Nodes.Add(mesh);
            }

            foreach (var tex in TextureList)
            {
                Renderer.Textures.Add(tex);
                texFolder.Nodes.Add(tex);
            }
            Renderer.Skeleton = Model.GenericSkeleton;

            DrawableContainer = new DrawableContainer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);
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
            if (SkeletonData != null)
                model.GenericSkeleton = SkeletonData.ToGenericSkeleton();

            List<GenericRenderedObject> meshes = new List<GenericRenderedObject>();
            List<STGenericMaterial> materials = new List<STGenericMaterial>();

            foreach (var mat in ModelData.Materials)
            {
                STGenericMaterial genericMaterial = new STGenericMaterial();
                genericMaterial.Text = mat.Name;
                materials.Add(genericMaterial);

                foreach (var tex in mat.Textures)
                {
                    var type = STGenericMatTexture.TextureType.Unknown;
                    if (tex.Key == "DIFT") type = STGenericMatTexture.TextureType.Diffuse;
                    if (tex.Key == "NMAP") type = STGenericMatTexture.TextureType.Normal;
                    if (tex.Key == "SPCT") type = STGenericMatTexture.TextureType.Specular;
                    if (tex.Key == "BCLR") type = STGenericMatTexture.TextureType.Diffuse;

                    genericMaterial.TextureMaps.Add(new STGenericMatTexture()
                    {
                        Type = type,
                        Name = tex.Value.FileID.ToString(),
                        WrapModeS = STTextureWrapMode.Repeat,
                        WrapModeT = STTextureWrapMode.Repeat,
                    });
                }
            }

            foreach (var mesh in ModelData.Meshes)
            {
                var mat = materials[mesh.Header.MaterialIndex];

                GenericRenderedObject genericMesh = new GenericRenderedObject();
                genericMesh.Text = "Mesh_" + mat.Text;
                meshes.Add(genericMesh);

                foreach (var vert in mesh.Vertices)
                {
                    Vertex v = new Vertex();
                    v.pos = vert.Position;
                    v.nrm = vert.Normal;
                    v.uv0 = vert.TexCoord0;
                    v.uv1 = vert.TexCoord1;
                    v.uv2 = vert.TexCoord2;
                    v.tan = vert.Tangent;
                    v.col = vert.Color;

                    if (SkeletonData != null)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            var weight = vert.BoneWeights[j];
                            var boneIndex = (int)vert.BoneIndices[j];
                            if (weight == 0)
                                continue;

                            v.boneWeights.Add(weight);
                            v.boneIds.Add(SkeletonData.SkinnedBonesRemap[boneIndex]);;
                        }
                    }

                    genericMesh.vertices.Add(v);
                }

                var poly = new STGenericPolygonGroup();
                poly.Material = mat;
                poly.MaterialIndex = mesh.Header.MaterialIndex;
                genericMesh.PolygonGroups.Add(poly);

                foreach (var id in mesh.Indices)
                    poly.faces.Add((int)id);
            }

            model.Materials = materials;
            model.Objects = meshes;
            model.Name = this.FileName;
            return model;
        }
    }
}
