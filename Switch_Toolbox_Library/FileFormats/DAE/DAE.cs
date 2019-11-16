using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collada141;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using OpenTK;
using Toolbox.Library.Rendering;
using Toolbox.Library.Collada;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class DAE 
    {
        public class ExportSettings
        {
            public bool SuppressConfirmDialog = false;

            public bool OptmizeZeroWeights = true;

            public bool UseOldExporter = false;

            public bool FlipTexCoordsVertical = true;

            public Version FileVersion = new Version();

            public ProgramPreset Preset = ProgramPreset.NONE;

            public bool ExportTextures = true;

            public string ImageExtension = ".png";
            public string ImageFolder = "";
        }

        public class Version
        {
            public int Major = 1;
            public int Minor = 4;
            public int Micro = 1;
        }


        public static void Export(string FileName, ExportSettings settings, STGenericObject mesh)
        {
            Export(FileName, settings, new List<STGenericObject>() { mesh },
                new List<STGenericMaterial>(), new List<STGenericTexture>());
        }

        public static void Export(string FileName, ExportSettings settings, STGenericModel model, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null) {
            Export(FileName, settings, model.Objects.ToList(), model.Materials.ToList(),  Textures, skeleton, NodeArray);
        }

        public static void Export(string FileName, ExportSettings settings, 
            List<STGenericObject> Meshes, List<STGenericMaterial> Materials,
            List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            if (Materials == null)
                Materials = new List<STGenericMaterial>();

            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Exporting Model...";
            progressBar.Value = 0;
            progressBar.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            if (settings.UseOldExporter)
            {
                AssimpSaver saver = new AssimpSaver();
                STGenericModel model = new STGenericModel();
                model.Objects = Meshes;
                model.Materials = Materials;
                saver.SaveFromModel(model, FileName, Textures, skeleton, NodeArray);
                return;
            }

            string TexturePath = System.IO.Path.GetDirectoryName(FileName);

            using (ColladaWriter writer = new ColladaWriter(FileName, settings))
            {
                writer.WriteAsset();

                if (Materials.Count > 0)
                {
                    List<string> textureNames = new List<string>();
                    for (int i = 0; i < Textures?.Count; i++)
                    {
                        textureNames.Add(Textures[i].Text);

                        if (settings.ExportTextures) {

                            progressBar.Task = $"Exporting Texture {Textures[i].Text}";
                            progressBar.Value = ((i * 100) / Textures.Count);
                            progressBar.Refresh();

                            var bitmap = Textures[i].GetBitmap();

                            string textureName = Textures[i].Text;
                            if (textureName.RemoveIllegaleFileNameCharacters() != textureName)
                            {
                                string properName = textureName.RemoveIllegaleFileNameCharacters();
                                for (int m = 0; m < Materials?.Count; m++) {
                                    foreach (var tex in Materials[m].TextureMaps) {
                                        if (tex.Name == textureName)
                                            tex.Name = properName;
                                    }
                                }

                                textureName = properName;
                            }

                            bitmap.Save($"{TexturePath}/{textureName}.png");
                            bitmap.Dispose();

                            GC.Collect();
                        }
                    }

                    List<Material> materials = new List<Material>();
                    foreach (var mat in Materials)
                    {
                        Material material = new Material();
                        material.Name = mat.Text;
                        materials.Add(material);

                        foreach (var tex in mat.TextureMaps)
                        {
                            TextureMap texMap = new TextureMap();
                            texMap.Name = tex.Name;
                            if (tex.Type == STGenericMatTexture.TextureType.Diffuse)
                                texMap.Type = PhongTextureType.diffuse;
                            else if (tex.Type == STGenericMatTexture.TextureType.Normal)
                                texMap.Type = PhongTextureType.bump;
                            else if (tex.Type == STGenericMatTexture.TextureType.Specular)
                                texMap.Type = PhongTextureType.specular;
                            else if (tex.Type == STGenericMatTexture.TextureType.Emission)
                                texMap.Type = PhongTextureType.emission;
                            else
                                continue; //Skip adding unknown types

                            if (tex.WrapModeS == STTextureWrapMode.Repeat)
                                texMap.WrapModeS = SamplerWrapMode.WRAP;
                            else if (tex.WrapModeS == STTextureWrapMode.Mirror)
                                texMap.WrapModeS = SamplerWrapMode.MIRROR;
                            else if (tex.WrapModeS == STTextureWrapMode.Clamp)
                                texMap.WrapModeS = SamplerWrapMode.CLAMP;


                            if (tex.WrapModeT == STTextureWrapMode.Repeat)
                                texMap.WrapModeT = SamplerWrapMode.WRAP;
                            else if (tex.WrapModeT == STTextureWrapMode.Mirror)
                                texMap.WrapModeT = SamplerWrapMode.MIRROR;
                            else if(tex.WrapModeT == STTextureWrapMode.Clamp)
                                texMap.WrapModeT = SamplerWrapMode.CLAMP;


                            //If no textures are saved, still keep images references
                            //So the user can still dump textures after
                            if (Textures?.Count == 0)
                                textureNames.Add($"{texMap.Name}");

                            material.Textures.Add(texMap);
                        }
                    }

                    writer.WriteLibraryImages(textureNames.ToArray());

                    writer.WriteLibraryMaterials(materials);
                    writer.WriteLibraryEffects(materials);
                }
                else
                    writer.WriteLibraryImages();

                if (skeleton != null) {
                    foreach (var bone in skeleton.bones)
                    {
                        //Set the inverse matrix
                        var inverse = skeleton.GetBoneTransform(bone).Inverted();
                        var transform = bone.GetTransform();

                        float[] Transform = new float[] {
                       transform.M11, transform.M21, transform.M31, transform.M41,
                       transform.M12, transform.M22, transform.M32, transform.M42,
                       transform.M13, transform.M23, transform.M33, transform.M43,
                       transform.M14, transform.M24, transform.M34, transform.M44
                        };

                        float[] InvTransform = new float[] {
                      inverse.M11, inverse.M21, inverse.M31, inverse.M41,
                      inverse.M12, inverse.M22, inverse.M32, inverse.M42,
                      inverse.M13, inverse.M23, inverse.M33, inverse.M43,
                      inverse.M14, inverse.M24, inverse.M34, inverse.M44
                        };

                        writer.AddJoint(bone.Text, bone.parentIndex == -1 ? "" :
                            skeleton.bones[bone.parentIndex].Text, Transform, InvTransform);
                    }
                }

                int meshIndex = 0;

                writer.StartLibraryGeometries();
                foreach (var mesh in Meshes)
                {
                    progressBar.Task = $"Exporting Mesh {mesh.Text}";
                    progressBar.Value = ((meshIndex++ * 100) / Meshes.Count);
                    progressBar.Refresh();

                    int[] IndexTable = null;
                    if (NodeArray != null)
                        IndexTable = NodeArray.ToArray();

                    writer.StartGeometry(mesh.Text);

                    if (mesh.MaterialIndex != -1 && Materials.Count > mesh.MaterialIndex)
                    {
                        writer.CurrentMaterial = Materials[mesh.MaterialIndex].Text;
                    }

                    // collect sources
                    List<float> Position = new List<float>();
                    List<float> Normal = new List<float>();
                    List<float> UV0 = new List<float>();
                    List<float> UV1 = new List<float>();
                    List<float> UV2 = new List<float>();
                    List<float> UV3 = new List<float>();
                    List<float> Color = new List<float>();
                    List<int[]> BoneIndices = new List<int[]>();
                    List<float[]> BoneWeights = new List<float[]>();

                    bool HasNormals = false;
                    bool HasColors = false;
                    bool HasUV0 = false;
                    bool HasUV1 = false;
                    bool HasUV2 = false;
                    bool HasBoneIds = false;

                    foreach (var vertex in mesh.vertices)
                    {
                        //Remove zero weights
                        if (settings.OptmizeZeroWeights)
                        {
                            float MaxWeight = 1;
                            for (int i = 0; i < 4; i++)
                            {
                                if (vertex.boneWeights.Count <= i)
                                    continue;

                                if (vertex.boneIds.Count < i + 1)
                                {
                                    vertex.boneWeights[i] = 0;
                                    MaxWeight = 0;
                                }
                                else
                                {
                                    float weight = vertex.boneWeights[i];
                                    if (vertex.boneWeights.Count == i + 1)
                                        weight = MaxWeight;

                                    if (weight >= MaxWeight)
                                    {
                                        weight = MaxWeight;
                                        MaxWeight = 0;
                                    }
                                    else
                                        MaxWeight -= weight;

                                    vertex.boneWeights[i] = weight;
                                }
                            }
                        }


                        if (vertex.nrm != Vector3.Zero) HasNormals = true;
                        if (vertex.col != Vector4.One) HasColors = true;
                        if (vertex.uv0 != Vector2.Zero) HasUV0 = true;
                        if (vertex.uv1 != Vector2.Zero) HasUV1 = true;
                        if (vertex.uv2 != Vector2.Zero) HasUV2 = true;
                        if (vertex.boneIds.Count > 0) HasBoneIds = true;

                        Position.Add(vertex.pos.X); Position.Add(vertex.pos.Y); Position.Add(vertex.pos.Z);
                        Normal.Add(vertex.nrm.X); Normal.Add(vertex.nrm.Y); Normal.Add(vertex.nrm.Z);

                        if (settings.FlipTexCoordsVertical)
                        {
                            UV0.Add(vertex.uv0.X); UV0.Add(1 - vertex.uv0.Y);
                            UV1.Add(vertex.uv1.X); UV1.Add(1 - vertex.uv1.Y);
                            UV2.Add(vertex.uv2.X); UV2.Add(1 - vertex.uv2.Y);
                        }
                        else
                        {
                            UV0.Add(vertex.uv0.X); UV0.Add(vertex.uv0.Y);
                            UV1.Add(vertex.uv1.X); UV1.Add(vertex.uv1.Y);
                            UV2.Add(vertex.uv2.X); UV2.Add(vertex.uv2.Y);
                        }

                        Color.AddRange(new float[] { vertex.col.X, vertex.col.Y, vertex.col.Z, vertex.col.W });

                        List<int> bIndices = new List<int>();
                        List<float> bWeights = new List<float>();
                        for (int b = 0; b < vertex.boneIds.Count; b++)
                        {
                            if (b > mesh.VertexSkinCount - 1)
                                continue;

                            if (vertex.boneWeights.Count > b)
                            {
                                if (vertex.boneWeights[b] == 0)
                                    continue;
                            }

                            int index = -1;
                            if (IndexTable != null)
                                index = (int)IndexTable[vertex.boneIds[b]];
                            else
                                index = (int)vertex.boneIds[b];

                            if (index != -1 && index < skeleton?.bones.Count)
                                bIndices.Add(index);

                            //Some models may only use indices (single bind, rigid skin)
                            if (vertex.boneWeights.Count > b)
                                bWeights.Add(vertex.boneWeights[b]);
                            else
                                bWeights.Add(1);
                        }
                        
                        if (bIndices.Count == 0 && mesh.BoneIndex != -1) {
                            HasBoneIds = true;
                            bIndices.Add(mesh.BoneIndex);
                            bWeights.Add(1);
                        }

                        BoneIndices.Add(bIndices.ToArray());
                        BoneWeights.Add(bWeights.ToArray());
                    }

                    List<TriangleList> triangleLists = new List<TriangleList>();
                    if (mesh.lodMeshes.Count > 0)
                    {
                        TriangleList triangleList = new TriangleList();
                        triangleLists.Add(triangleList);

                        var lodMesh = mesh.lodMeshes[mesh.DisplayLODIndex];

                        List<int> faces = new List<int>();
                        if (lodMesh.PrimativeType == STPrimitiveType.TrangleStrips)
                            faces = STGenericObject.ConvertTriangleStripsToTriangles(lodMesh.faces);
                        else
                            faces = lodMesh.faces;

                        for (int i = 0; i < faces.Count; i++)
                            triangleList.Indices.Add((uint)faces[i]);
                    }
                    if (mesh.PolygonGroups.Count > 0)
                    {
                        foreach (var group in mesh.PolygonGroups)
                        {
                            TriangleList triangleList = new TriangleList();
                          
                            triangleLists.Add(triangleList);

                            if (group.MaterialIndex != -1 && Materials.Count > group.MaterialIndex)
                                triangleList.Material = Materials[group.MaterialIndex].Text;

                            List<int> faces = new List<int>();
                            if (group.PrimativeType == STPrimitiveType.TrangleStrips)
                                faces = STGenericObject.ConvertTriangleStripsToTriangles(group.faces);
                            else
                                faces = group.faces;

                            for (int i = 0; i < faces.Count; i++)
                                triangleList.Indices.Add((uint)faces[i]);
                        }
                    }

                    // write sources
                    writer.WriteGeometrySource(mesh.Text, SemanticType.POSITION, Position.ToArray(), triangleLists.ToArray());

                    if (HasNormals)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.NORMAL, Normal.ToArray(), triangleLists.ToArray());

                    if (HasColors)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color.ToArray(), triangleLists.ToArray());

                    if (HasUV0)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.TEXCOORD, UV0.ToArray(), triangleLists.ToArray(), 0);

                    if (HasUV1)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.TEXCOORD, UV1.ToArray(), triangleLists.ToArray(), 1);

                    if (HasUV2)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.TEXCOORD, UV2.ToArray(), triangleLists.ToArray(), 2);

                    if (HasBoneIds)
                        writer.AttachGeometryController(BoneIndices, BoneWeights);

                    writer.EndGeometryMesh();
                }
                writer.EndGeometrySection();
            }

            progressBar?.Close();

            if (!settings.SuppressConfirmDialog)
                System.Windows.Forms.MessageBox.Show($"Exported {FileName} Successfuly!");
        }



        public List<STGenericObject> objects = new List<STGenericObject>();
        public List<STGenericMaterial> materials = new List<STGenericMaterial>();
        public STSkeleton skeleton;
        public List<string> BoneNames = new List<string>();

        public bool UseTransformMatrix = true;

        Dictionary<string, Vertex> VertexSkinSources = new Dictionary<string, Vertex>();
        Dictionary<string, Matrix4> MatrixSkinSources = new Dictionary<string, Matrix4>();

        private Matrix4 GlobalTransform = Matrix4.Identity;
        public bool LoadFile(string FileName)
        {
            GlobalTransform = Matrix4.Identity;

            COLLADA collada = COLLADA.Load(FileName);

           
            //Check axis up
            if (collada.asset != null)
            {
                switch (collada.asset.up_axis)
                {
                    case UpAxisType.X_UP:
                        GlobalTransform = Matrix4.CreateRotationX(90);
                        break;
                    case UpAxisType.Y_UP:
                        GlobalTransform = Matrix4.CreateRotationY(90);
                        break;
                    case UpAxisType.Z_UP:
                        GlobalTransform = Matrix4.CreateRotationZ(90);
                        break;
                }

                if (collada.asset.unit != null)
                {
                    var amount = collada.asset.unit.meter;
                    var type = collada.asset.unit.name;
                    if (type == "meter")
                    {

                    }
                    else if (type == "centimeter")
                    {

                    }
                }
            }

            foreach (var item in collada.Items)
            {
                if (item is library_geometries)
                    LoadGeometry((library_geometries)item);
                if (item is library_images)
                    LoadImages((library_images)item);
                if (item is library_controllers)
                    LoadControllers((library_controllers)item);
                if (item is library_nodes)
                    LoadNodes((library_nodes)item);
                if (item is library_visual_scenes)
                    LoadVisualScenes((library_visual_scenes)item);
            }

            return true;
        }

        private void LoadVisualScenes(library_visual_scenes nodes)
        {

        }

        private void LoadNodes(library_nodes nodes)
        {
            
        }

        private void LoadControllers(library_controllers controllers)
        {
           
        }

        private void LoadMaterials(library_materials materials)
        {

        }

        private void LoadImages(library_images images)
        {

        }

        private void LoadGeometry(library_geometries geometries)
        {
            foreach (var geom in geometries.geometry)
            {
                var mesh = geom.Item as mesh;
                if (mesh == null)
                    continue;

                foreach (var source in mesh.source)
                {
                    var float_array = source.Item as float_array;
                    if (float_array == null)
                        continue;

                    Console.Write("Geometry {0} source {1} : ", geom.id, source.id);
                    foreach (var mesh_source_value in float_array.Values)
                        Console.Write("{0} ", mesh_source_value);
                    Console.WriteLine();
                }
            }
        }

        public bool ExportFile(string FileName, List<STGenericObject> meshes, STSkeleton skeleton = null)
        {
            return false;
        }

        private List<STGenericObject> CreateGenericObjects(string Name, library_geometries Geometries)
        {
            List<STGenericObject> objects = new List<STGenericObject>();
            foreach (var geom in Geometries.geometry)
            {
                var daeMesh = geom.Item as mesh;
                if (daeMesh == null)
                    continue;

                STGenericObject mesh = new STGenericObject();
                mesh.ObjectName = Name;

                foreach (var source in daeMesh.source)
                {
                    var float_array = source.Item as float_array;
                    if (float_array == null)
                        continue;
                }
                objects.Add(mesh);
            }
            return objects;
        }
    }
}
