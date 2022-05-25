using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using OpenTK;
using FirstPlugin.Forms;
using FirstPlugin.GFMDLStructs;
using Newtonsoft.Json;

namespace FirstPlugin
{
    public class GFBMDL : TreeNodeFile, IContextMenuNode, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Graphic Model" };
        public string[] Extension { get; set; } = new string[] { "*.gfbmdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                bool IsMatch = reader.ReadUInt32() == 0x20000000;
                reader.Position = 0;

                return IsMatch;
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


        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView) {
            LoadEditor<STPropertyGrid>();
        }

        public T LoadEditor<T>() where T : UserControl, new()
        {
            T control = new T();
            control.Dock = DockStyle.Fill;

            ViewportEditor editor = (ViewportEditor)LibraryGUI.GetActiveContent(typeof(ViewportEditor));
            if (editor == null)
            {
                editor = new ViewportEditor(true);
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }
            if (!DrawablesLoaded)
            {
                ObjectEditor.AddContainer(DrawableContainer);
                DrawablesLoaded = true;
            }
            if (Runtime.UseOpenGL)
                editor.LoadViewport(DrawableContainer);

            editor.LoadEditor(control);

            return control;
        }

        public GFBMDL_Render Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public GFLXModel Model;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            Text = FileName;
            DrawableContainer.Name = FileName;

            //Here i convert buffer classes generated from flatc to c# classes that are much nicer to alter
            var model = BufferToStruct.LoadFile(stream);
            ReloadModel(model);
        }

        private void ReloadModel(Model model)
        {
            if (Renderer != null)
                Renderer.Meshes.Clear();

            DrawableContainer.Drawables.Clear();

            Renderer = new GFBMDL_Render();
            Renderer.GfbmdlFile = this;
            DrawableContainer.Drawables.Add(Renderer);

            Nodes.Clear();

            Model = new GFLXModel();
            Model.LoadFile(model, this, Renderer);

            TreeNode SkeletonWrapper = new TreeNode("Skeleton");
            TreeNode MaterialFolderWrapper = new TreeNode("Materials");
            TreeNode VisualGroupWrapper = new TreeNode("Visual Groups");
            TreeNode Textures = new TreeNode("Textures");

            if (Model.Skeleton.bones.Count > 0)
            {
                Nodes.Add(SkeletonWrapper);
                DrawableContainer.Drawables.Add(Model.Skeleton);

                foreach (var bone in Model.Skeleton.bones)
                {
                    if (bone.Parent == null)
                        SkeletonWrapper.Nodes.Add(bone);
                }
            }


            List<string> loadedTextures = new List<string>();
            for (int i = 0; i < Model.Textures.Count; i++)
            {
                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(Model.Textures[i]) &&
                        !loadedTextures.Contains(Model.Textures[i]))
                    {
                        TreeNode tex = new TreeNode(Model.Textures[i]);
                        tex.ImageKey = "texture";
                        tex.SelectedImageKey = "texture";

                        tex.Tag = bntx.Textures[Model.Textures[i]];
                        Textures.Nodes.Add(tex);
                        loadedTextures.Add(Model.Textures[i]);
                    }
                }
            }

            loadedTextures.Clear();

            Nodes.Add(MaterialFolderWrapper);
            Nodes.Add(VisualGroupWrapper);
            if (Textures.Nodes.Count > 0)
                Nodes.Add(Textures);

            for (int i = 0; i < Model.GenericMaterials.Count; i++)
                MaterialFolderWrapper.Nodes.Add(Model.GenericMaterials[i]);

            for (int i = 0; i < Model.GenericMeshes.Count; i++)
                VisualGroupWrapper.Nodes.Add(Model.GenericMeshes[i]);
        }

        public void Save(System.IO.Stream stream)
        {
            Model.SaveFile(stream);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            Items.Add(new ToolStripMenuItem("Export Model", null, ExportAction, Keys.Control | Keys.E));
            Items.Add(new ToolStripMenuItem("Replace Model", null, ReplaceAction, Keys.Control | Keys.R));
            return Items.ToArray();
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

        private void ExportAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.dae;*.json;|" +
             "DAE |*.dae|" +
             "JSON |*.json|" +
             "All files(*.*)|*.*";

            sfd.FileName = Path.GetFileNameWithoutExtension(Text);
            sfd.DefaultExt = "dae";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = Utils.GetExtension(sfd.FileName);
                if (ext == ".json")
                {
                    string json = JsonConvert.SerializeObject(Model.Model, Formatting.Indented);
                    System.IO.File.WriteAllText(sfd.FileName, json);
                }
                else
                {
                    ExportModelSettings exportDlg = new ExportModelSettings();
                    if (exportDlg.ShowDialog() == DialogResult.OK)
                        ExportModel(sfd.FileName, exportDlg.Settings);
                }
            }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.dae;*.fbx;*.json;|" +
                         "FBX |*.fbx|" +
                         "DAE |*.dae|" +
                         "JSON |*.json|" +
                         "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ext = Utils.GetExtension(ofd.FileName);
                if (ext == ".json")
                {
                    var model = JsonConvert.DeserializeObject<Model>(
                             System.IO.File.ReadAllText(ofd.FileName));

                    ReloadModel(model);
                    Model.UpdateVertexData(true);
                }
                else
                {
                    AssimpData assimp = new AssimpData();
                    bool IsLoaded = assimp.LoadFile(ofd.FileName);

                    if (!IsLoaded)
                        return;

                    GFLXModelImporter dialog = new GFLXModelImporter();
                    dialog.LoadMeshes(assimp.objects, assimp.materials, Model.GenericMaterials, Model.GenericMeshes);
                    if (dialog.ShowDialog() == DialogResult.OK) {
                        ImportModel(dialog, assimp.materials, assimp.objects, assimp.skeleton);
                    }
                }
            }

            LoadEditor<STPropertyGrid>();
        }

        private List<STGenericObject> GeneratePolygonGroups(List<STGenericObject> importedMeshes,
            List<STGenericMaterial> importedMaterials, List<string> textures, GfbmdlImportSettings settings)
        {
            List<STGenericObject> meshes = new List<STGenericObject>();
            foreach (var mesh in importedMeshes)
            {
                var lod = mesh.lodMeshes[0];
                var setting = settings.MeshSettings[importedMeshes.IndexOf(mesh)];

                //Create or map existing materials to the polygon group
                var currentMaterial = Model.Model.Materials.FirstOrDefault(x => x.Name == setting.Material);
                if (currentMaterial == null)
                {
                    currentMaterial = Material.Replace(setting.MaterialFile);

                    //Get the imported material and set it's material data to the gfbmdl one
                    if (mesh.MaterialIndex < importedMaterials.Count && mesh.MaterialIndex >= 0)
                        SetupMaterial(importedMaterials[mesh.MaterialIndex], currentMaterial, textures);

                    Model.Model.Materials.Add(currentMaterial);
                }

                mesh.MaterialIndex = Model.Model.Materials.IndexOf(currentMaterial);

                //Merge dupes with the same name
                //Some are duped but use different material index
                //Which go into a polygon group
                var existingMesh = meshes.FirstOrDefault(x => x.ObjectName == mesh.ObjectName);

                //Don't split atm since it's buggy
                //The faces seemed fine, but vertices are split by assimp giving issues
                bool splitPolygonGroups = false;
                if (existingMesh != null && splitPolygonGroups)
                {
                    int offset = 0;
                    foreach (var poly in existingMesh.PolygonGroups)
                    {
                        for (int f = 0; f < poly.faces.Count; f++)
                            offset = Math.Max(offset, poly.faces[f]);
                    }

                    List<int> faces = new List<int>();
                    for (int f = 0; f < lod.faces.Count; f++) {
                        faces.Add((int)(lod.faces[f] + offset + 1   ));
                    }

                    foreach (var vertex in mesh.vertices)
                        existingMesh.vertices.Add(vertex);

                    existingMesh.PolygonGroups.Add(new STGenericPolygonGroup()
                    {
                        MaterialIndex = mesh.MaterialIndex,
                        faces = faces,
                    });
                }
                else
                {
                    mesh.PolygonGroups.Add(new STGenericPolygonGroup()
                    {
                        MaterialIndex = mesh.MaterialIndex,
                        faces = lod.faces,
                    });

                    meshes.Add(mesh);
                }
            }
            return meshes;
        }

        private void SetupMaterial(STGenericMaterial importedMaterial, Material newMaterial, List<string> textures)
        {
            return;

            //Here all we want is the diffuse texture and swap them
            foreach (var texMap in importedMaterial.TextureMaps)
            {
                var diffuseTex = newMaterial.TextureMaps.FirstOrDefault(x =>
                x.Sampler == "Col0Tex" || x.Sampler == "BaseColor0");
                if (diffuseTex != null) {
                    diffuseTex.Index = (uint)textures.IndexOf(texMap.Name);
                }
            }
        }

        private void ImportModel(GFLXModelImporter importer,
            List<STGenericMaterial> importedMaterials, List<STGenericObject> importedMeshes,
            STSkeleton skeleton)
        {
            Model.Model.Groups.Clear();
            Model.Model.Meshes.Clear();

            List<string> textures = Model.Textures.ToList();
            foreach (var mat in importedMaterials)
            {

            }

            var meshes = GeneratePolygonGroups(importedMeshes, importedMaterials, textures, importer.Settings);

            //Once mesh groups are merged search for bone nodes
            //The bone section is basically a node tree
            //Which contains nodes for meshes
            //We need to remove the original and replace with new ones
            //Then index these in our mesh groups
            if (importer.ImportNewBones)
            {
                //Clear the original bones and nodes
                Model.Skeleton.bones.Clear();
                Model.Model.Bones.Clear();

                List<int> SkinningIndices = new List<int>();
                foreach (var genericBone in skeleton.bones)
                {
                    var scale = genericBone.Scale;
                    var trans = genericBone.Position;
                    var rot = genericBone.EulerRotation;

                    Bone bone = new Bone();
                    bone.Name = genericBone.Text;
                    bone.BoneType = 0;
                    bone.Parent = genericBone.parentIndex;
                    bone.Zero = 0;
                    bone.SegmentScale = false;
                    bone.Scale = new GFMDLStructs.Vector3(scale.X, scale.Y, scale.Z);
                    bone.Rotation = new GFMDLStructs.Vector3(rot.X, rot.Y, rot.Z);
                    bone.Translation = new GFMDLStructs.Vector3(trans.X, trans.Y, trans.Z);
                    bone.RadiusStart = new GFMDLStructs.Vector3(0, 0, 0);
                    bone.RadiusEnd = new GFMDLStructs.Vector3(0, 0, 0);

                    Model.Model.Bones.Add(bone);
                }
            }

            int originIndex = int.MaxValue;

            //Go through each bone and remove the original mesh node
            List<STBone> bonesToRemove = new List<STBone>();
            for (int i = 0; i < Model.Skeleton.bones.Count; i++)
            {
                //Reset bone as rigid
                var node = (GFLXBone)Model.Skeleton.bones[i];
                node.Bone.RigidCheck = new BoneRigidData() { Unknown1 = 0 };
                if (node.Bone.BoneType == 1)
                    node.Bone.BoneType = 0;

                int index = Model.Skeleton.bones.IndexOf(node);

                if (node.Text == "Origin")
                    originIndex = index;

                //Check if the bone is rigged to any meshes and use skinning
                for (int m = 0; m < meshes.Count; m++)
                {
                    if (meshes[m].vertices.Any(x => x.boneNames.Contains(node.Text)))
                    {
                        node.Bone.BoneType = 1;
                        node.Bone.RigidCheck = null;
                    }
                }

                if (Model.GenericMeshes.Any(x => x.Text == node.Text)) {
                    bonesToRemove.Add(node);
                    Model.Model.Bones.Remove(node.Bone);
                }

              /*  if (Model.Model.CollisionGroups?.Count != 0)
                {
                    var collisionGroups = Model.Model.CollisionGroups;
                    for (int c = 0; c < collisionGroups.Count; c++)
                    {
                        if (collisionGroups[c].BoneIndex == i)
                        {

                        }
                    }
                }*/
            }

            foreach (var bone in bonesToRemove)
                Model.Skeleton.bones.Remove(bone);

            bonesToRemove.Clear();

            Model.Model.CollisionGroups = new List<CollisionGroup>();

            List<int> skinningIndices = Model.GenerateSkinningIndices();

            //Set an empty bone with rigging if there is no rigging
            if (importer.Settings.MeshSettings.Any(x => x.HasBoneIndices) && skinningIndices.Count == 0)
            {
                var node = (GFLXBone)Model.Skeleton.bones[0];
                node.Bone.RigidCheck = null;
                node.Bone.BoneType = 1;
                skinningIndices.Add(0);
            }

            List<string> unmappedBones = new List<string>();

            foreach (var mesh in meshes)
            {
                var setting = importer.Settings.MeshSettings[meshes.IndexOf(mesh)];

                for (int i = 0; i < mesh.vertices.Count; i++)
                {
                    if (setting.SetNormalsToColorChannel2)
                    {
                           mesh.vertices[i].col2 = new OpenTK.Vector4(
                           mesh.vertices[i].nrm.X * 0.5f + 0.5f,
                           mesh.vertices[i].nrm.Y * 0.5f + 0.5f,
                           mesh.vertices[i].nrm.Z * 0.5f + 0.5f,
                           1);
                    }

                    //Single bind if no bones are mapped but setting is enabled
                    if (setting.HasBoneIndices && mesh.vertices[i].boneNames.Count == 0) {
                        mesh.vertices[i].boneIds.Add(skinningIndices.FirstOrDefault());
                        mesh.vertices[i].boneWeights.Add(1);
                    }

                    if (importer.RotationY != 0)
                    {
                        var transform = OpenTK.Matrix4.CreateRotationX(OpenTK.MathHelper.DegreesToRadians(importer.RotationY));
                        mesh.vertices[i].pos = OpenTK.Vector3.TransformPosition(mesh.vertices[i].pos, transform);
                        mesh.vertices[i].nrm = OpenTK.Vector3.TransformPosition(mesh.vertices[i].nrm, transform);
                    }

                    if (importer.Settings.FlipUVsVertical) {
                        mesh.vertices[i].uv0 = new Vector2(0, 1) - mesh.vertices[i].uv0;
                        mesh.vertices[i].uv1 = new Vector2(0, 1) - mesh.vertices[i].uv1;
                        mesh.vertices[i].uv2 = new Vector2(0, 1) - mesh.vertices[i].uv2;
                    }

                    if (importer.Settings.OptmizeZeroWeights)
                    {
                        float[] weightsA = new float[4];

                        int MaxWeight = 255;
                        for (int j = 0; j < 4; j++)
                        {
                            if (mesh.vertices[i].boneWeights.Count < j + 1)
                            {
                                weightsA[j] = 0;
                                MaxWeight = 0;
                            }
                            else
                            {
                                int weight = (int)(mesh.vertices[i].boneWeights[j] * 255);
                                if (mesh.vertices[i].boneWeights.Count == j + 1)
                                    weight = MaxWeight;

                                if (weight >= MaxWeight)
                                {
                                    weight = MaxWeight;
                                    MaxWeight = 0;
                                }
                                else
                                    MaxWeight -= weight;

                                weightsA[j] = weight / 255f;
                            }
                        }

                        mesh.vertices[i].boneWeights = weightsA.ToList();
                    }

                    for (int j = 0; j < mesh.vertices[i].boneNames?.Count; j++)
                    {
                        string boneName = mesh.vertices[i].boneNames[j] ;
                        int boneIndex = Model.Model.Bones.IndexOf(Model.Model.Bones.Where(p => p.Name == boneName).FirstOrDefault());

                        if (boneIndex != -1 && skinningIndices.IndexOf(boneIndex) != -1)
                            mesh.vertices[i].boneIds.Add(boneIndex);
                        else
                        {
                            if (!unmappedBones.Contains(boneName))
                                unmappedBones.Add(boneName);
                        }
                    }
                }
            }

            //Adjust materials if necessary
            if (importer.Settings.ResetUVTransform)
            {
                foreach (var mat in Model.GenericMaterials)
                {
                    foreach (var param in mat.ValueParams)
                    {
                        if (param.Key.Contains("UVScale"))
                            param.Value.Value = 1;
                        if (param.Key.Contains("UVTranslate"))
                            param.Value.Value = 0;
                        if (param.Key.Contains("ColorBaseU"))
                            param.Value.Value = 0;
                        if (param.Key.Contains("ColorBaseV"))
                            param.Value.Value = 0;
                    }
                }
            }

            //Now add brand new mesh nodes
            foreach (var mesh in meshes)
            {
                int index = meshes.IndexOf(mesh);

                var setting = importer.Settings.MeshSettings[index];

                Bone bone = new Bone();
                bone.Name = mesh.ObjectName;
                bone.BoneType = 0;
                bone.Parent = 0;
                bone.Zero = 0;
                bone.SegmentScale = false;
                bone.Scale = new GFMDLStructs.Vector3(1, 1, 1);
                bone.Rotation = new GFMDLStructs.Vector3(0, 0, 0);
                bone.Translation = new GFMDLStructs.Vector3(0,0,0);
                bone.RadiusStart = new GFMDLStructs.Vector3(0, 0, 0);
                bone.RadiusEnd = new GFMDLStructs.Vector3(0, 0, 0);
           //     bone.RigidCheck = new BoneRigidData();

                Model.Model.Bones.Add(bone);
                int NodeIndex = Model.Model.Bones.IndexOf(bone);

                //Now create the associated group
                var group = new Group();
                group.Bounding = Model.GenerateBoundingBox(mesh);
                group.BoneIndex = (uint)NodeIndex;
                group.MeshIndex = (uint)index;
                group.Layer = 0;
                Model.Model.Groups.Add(group);

                //Now create our mesh data
                var meshData = new Mesh();
                Model.Model.Meshes.Add(meshData);

                if (setting.HasTangents || setting.HasBitangents)
                {
                    try {
                        mesh.CalculateTangentBitangent(false);
                    }
                    catch { }
                }

                //Add attributes based on settings
                IList<MeshAttribute> attributes = new List<MeshAttribute>();
                attributes.Add(new MeshAttribute()
                {
                    VertexType = (uint)VertexType.Position,
                    BufferFormat = (uint)setting.PositionFormat,
                    ElementCount = 3,
                });

                if (setting.HasNormals) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.Normal,
                        BufferFormat = (uint)setting.NormalFormat,
                        ElementCount = 4,
                    });
                }

                if (setting.HasTangents)
                {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.Tangents,
                        BufferFormat = (uint)setting.TangentsFormat,
                        ElementCount = 4,
                    });
                }

                if (setting.HasTexCoord1) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.UV1,
                        BufferFormat = (uint)setting.TexCoord1Format,
                        ElementCount = 2,
                    });
                }
                if (setting.HasTexCoord2) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.UV2,
                        BufferFormat = (uint)setting.TexCoord2Format,
                        ElementCount = 2,
                    });
                }
                if (setting.HasTexCoord3) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.UV3,
                        BufferFormat = (uint)setting.TexCoord3Format,
                        ElementCount = 2,
                    });
                }
                if (setting.HasTexCoord4) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.UV4,
                        BufferFormat = (uint)setting.TexCoord4Format,
                        ElementCount = 2,
                    });
                }
                if (setting.HasColor1) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.Color1,
                        BufferFormat = (uint)setting.Color1Format,
                        ElementCount = 4,
                    });
                }
                if (setting.HasColor2) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.Color2,
                        BufferFormat = (uint)setting.Color2Format,
                        ElementCount = 4,
                    });
                }

                if (setting.HasBoneIndices) {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.BoneID,
                        BufferFormat = (uint)setting.BoneIndexFormat,
                        ElementCount = 4,
                    });
                }

                if (setting.HasWeights)
                {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.BoneWeight,
                        BufferFormat = (uint)setting.BoneWeightFormat,
                        ElementCount = 4,
                    });
                }

                if (setting.HasBitangents)
                {
                    attributes.Add(new MeshAttribute()
                    {
                        VertexType = (uint)VertexType.Bitangent,
                        BufferFormat = (uint)setting.BitangentnFormat,
                        ElementCount = 4,
                    });
                }

                meshData.Attributes = attributes;
                meshData.SetData(GFLXMeshBufferHelper.CreateVertexDataBuffer(mesh, skinningIndices, attributes));

                //Lastly add the polygon groups
                foreach (var poly in mesh.PolygonGroups)
                {
                    List<ushort> faces = new List<ushort>();
                    for (int f = 0; f < poly.faces.Count; f++)
                        faces.Add((ushort)poly.faces[f]);

                    if (poly.MaterialIndex < 0)
                        poly.MaterialIndex = 0;

                    meshData.Polygons = new List<MeshPolygon>();
                    meshData.Polygons.Add(new MeshPolygon()
                    {
                        MaterialIndex = (uint)poly.MaterialIndex,
                        Faces = faces,
                    });
                }
            }

            if (unmappedBones.Count > 0)
                STErrorDialog.Show($"{unmappedBones.Count} bone(s) are not present in the boneset and are unmapped!", 
                    "GFBMDL Importer", string.Join("\n", unmappedBones.ToArray()));

            Console.WriteLine($"");

            //Generate bounding box
            Model.GenerateBoundingBox();
            ReloadModel(Model.Model);

            Model.UpdateVertexData(true);
        }

        public void ExportModel(string fileName, DAE.ExportSettings settings)
        {
            var model = new STGenericModel();
            model.Materials = Model.GenericMaterials;
            model.Objects = Model.GenericMeshes;
            var textures = new List<STGenericTexture>();
            foreach (var bntx in PluginRuntime.bntxContainers)
                foreach (var tex in bntx.Textures.Values)
                {
                    if (Model.Textures.Contains(tex.Text))
                        textures.Add(tex);
                }

            DAE.Export(fileName, settings, model, textures, Model.Skeleton);
        }

        public void Unload()
        {

        }
    }
}
