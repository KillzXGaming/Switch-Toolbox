using Collada141;
using Newtonsoft.Json;
using Octokit;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Toolbox.Library.Collada;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;

namespace Toolbox.Library
{
    public class DAE
    {
        public class ExportSettings
        {
            public bool ExportDecomposed = false;
            public bool SuppressConfirmDialog = false;
            public bool OptmizeZeroWeights = true;
            public bool UseOldExporter = false;
            public bool UseVertexColors = true;
            public bool FlipTexCoordsVertical = true;
            public bool OnlyExportRiggedBones = false;
            public bool UseTextureChannelComponents = true;

            public bool TransformColorUVs = false;
            public bool ForceZeroShininessFbx = false;
            public bool ExportNormalMapsAsFlatColorFbx = true;
            public bool InsertDummyAttributes = false;

            public bool AddLeafBones = false;

            public Version FileVersion = new Version();

            public ProgramPreset Preset = ProgramPreset.NONE;

            public bool ExportTextures = true;

            public string ImageExtension = ".png";
            public string ImageFolder = "";

            private string _configPath => Path.Combine(Runtime.ExecutableDir, "ExportSettings.json");

            public ExportSettings Load()
            {
                if (!File.Exists(_configPath))
                    Save();

               return JsonConvert.DeserializeObject<ExportSettings>(File.ReadAllText(_configPath));
            }
            public void Save()
            {
                string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
                {
                });
                File.WriteAllText(_configPath, json);
            }
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

        public static void Export(string FileName, ExportSettings settings, STGenericModel model, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            Export(FileName, settings, model.Objects.ToList(), model.Materials.ToList(), Textures, skeleton, NodeArray);
        }

        public static void Export(string FileName, ExportSettings settings,
            List<STGenericObject> Meshes, List<STGenericMaterial> Materials,
            List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            if (Materials == null)
                Materials = new List<STGenericMaterial>();
            if (skeleton != null && skeleton.BoneIndices != null)
                NodeArray = skeleton.BoneIndices.ToList();

            List<string> failedTextureExport = new List<string>();

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
            Dictionary<string, STGenericMaterial> MaterialRemapper = new Dictionary<string, STGenericMaterial>();

            using (ColladaWriter writer = new ColladaWriter(FileName, settings))
            {
                writer.WriteAsset();

                if (Materials.Count > 0)
                {
                    List<string> textureNames = new List<string>();
                    for (int i = 0; i < Textures?.Count; i++)
                    {
                        if (!textureNames.Contains(Textures[i].Text))
                            textureNames.Add(Textures[i].Text);

                        if (settings.ExportTextures)
                        {

                            progressBar.Task = $"Exporting Texture {Textures[i].Text}";
                            progressBar.Value = ((i * 100) / Textures.Count);
                            progressBar.Refresh();

                            try
                            {
                                var bitmap = Textures[i].GetBitmap();
                                if (bitmap != null)
                                {
                                    if (settings.UseTextureChannelComponents)
                                        bitmap = Textures[i].GetComponentBitmap(bitmap);
                                    string textureName = Textures[i].Text;
                                    if (textureName.RemoveIllegaleFileNameCharacters() != textureName)
                                    {
                                        string properName = textureName.RemoveIllegaleFileNameCharacters();
                                        for (int m = 0; m < Materials?.Count; m++)
                                        {
                                            foreach (var tex in Materials[m].TextureMaps)
                                            {
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
                            catch (Exception ex)
                            {
                                failedTextureExport.Add(Textures[i].Text);
                            }
                        }
                    }

                    List<Material> materials = new List<Material>();
                    foreach (var mat in Materials)
                    {
                        Material material = new Material();
                        material.Name = mat.Text;

                        if (!MaterialRemapper.ContainsKey(mat.Text))
                        {
                            MaterialRemapper.Add(mat.Text, mat);
                        }
                        else
                        {
                            string name = Utils.RenameDuplicateString(MaterialRemapper.Keys.ToList(), mat.Text);
                            MaterialRemapper.Add(name, mat);
                            material.Name = name;
                        }

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
                            else if (tex.WrapModeT == STTextureWrapMode.Clamp)
                                texMap.WrapModeT = SamplerWrapMode.CLAMP;


                            //If no textures are saved, still keep images references
                            //So the user can still dump textures after
                            if (Textures?.Count == 0 && !textureNames.Contains(texMap.Name))
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

                if (skeleton != null)
                {
                    //Search for bones with rigging first
                    List<string> riggedBones = new List<string>();
                    if (settings.OnlyExportRiggedBones)
                    {
                        for (int i = 0; i < Meshes.Count; i++)
                        {
                            var vertices = Meshes[i].GetLodVertices(0);
                            for (int v = 0; v < vertices.Count; v++)
                            {
                                var vertex = vertices[v];
                                for (int j = 0; j < vertex.boneIds.Count; j++)
                                {
                                    int id = -1;
                                    if (NodeArray != null && NodeArray.Count > vertex.boneIds[j])
                                    {
                                        id = NodeArray[vertex.boneIds[j]];
                                    }
                                    else
                                        id = vertex.boneIds[j];

                                    if (id < skeleton.bones.Count && id != -1)
                                        riggedBones.Add(skeleton.bones[id].Text);
                                }
                            }
                        }
                    }

                    foreach (var bone in skeleton.bones)
                    {
                        if (settings.OnlyExportRiggedBones && !riggedBones.Contains(bone.Text))
                        {
                            Console.WriteLine("Skipping " + bone.Text);
                            continue;
                        }

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
                            skeleton.bones[bone.parentIndex].Text, Transform, InvTransform,
                            new float[3] { bone.Position.X, bone.Position.Y, bone.Position.Z },
                            new float[3] { bone.EulerRotation.X * STMath.Rad2Deg, bone.EulerRotation.Y * STMath.Rad2Deg, bone.EulerRotation.Z * STMath.Rad2Deg },
                            new float[3] { bone.Scale.X, bone.Scale.Y, bone.Scale.Z },
                            settings.ExportDecomposed);
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
                        Console.WriteLine($"MaterialIndex {mesh.MaterialIndex} {Materials[mesh.MaterialIndex].Text}");
                    }


                    if (settings.TransformColorUVs)
                    {
                        List<Vertex> transformedVertices = new List<Vertex>();
                        foreach (var poly in mesh.PolygonGroups)
                        {
                            var mat = poly.Material;
                            if (mat == null) continue;

                            var faces = poly.GetDisplayFace();
                            for (int v = 0; v < poly.displayFaceSize; v += 3)
                            {
                                if (faces.Count < v + 2)
                                    break;

                                var diffuse = mat.TextureMaps.FirstOrDefault(x => x.Type == STGenericMatTexture.TextureType.Diffuse);
                                STTextureTransform transform = new STTextureTransform();
                                if (diffuse != null)
                                    transform = diffuse.Transform;

                                var vertexA = mesh.vertices[faces[v]];
                                var vertexB = mesh.vertices[faces[v + 1]];
                                var vertexC = mesh.vertices[faces[v + 2]];

                                if (!transformedVertices.Contains(vertexA))
                                {
                                    vertexA.uv0 = (vertexA.uv0 * transform.Scale) + transform.Translate;
                                    transformedVertices.Add(vertexA);
                                }
                                if (!transformedVertices.Contains(vertexB))
                                {
                                    vertexB.uv0 = (vertexB.uv0 * transform.Scale) + transform.Translate;
                                    transformedVertices.Add(vertexB);
                                }
                                if (!transformedVertices.Contains(vertexC))
                                {
                                    vertexC.uv0 = (vertexC.uv0 * transform.Scale) + transform.Translate;
                                    transformedVertices.Add(vertexC);
                                }
                            }
                        }
                    }

                    // collect sources
                    List<float> Position = new List<float>();
                    List<float> Normal = new List<float>();
                    List<float> UV0 = new List<float>();
                    List<float> UV1 = new List<float>();
                    List<float> UV2 = new List<float>();
                    List<float> UV3 = new List<float>();
                    List<float> Color = new List<float>();
                    List<float> Color2 = new List<float>();
                    List<float> Color3 = new List<float>();
                    List<float> Color4 = new List<float>();
                    List<float> Color5 = new List<float>();
                    List<float> Color6 = new List<float>();
                    List<float> Color7 = new List<float>();
                    List<float> Color8 = new List<float>();
                    List<int[]> BoneIndices = new List<int[]>();
                    List<float[]> BoneWeights = new List<float[]>();

                    bool HasNormals = false;
                    bool HasColor0 = false;
                    bool HasColor1 = false;
                    bool HasColor2 = false;
                    bool HasColor3 = false;
                    bool HasColor4 = false;
                    bool HasColor5 = false;
                    bool HasColor6 = false;
                    bool HasColor7 = false;
                    bool HasUV0 = false;
                    bool HasUV1 = false;
                    bool HasUV2 = false;
                    bool HasUV3 = false;
                    bool HasBoneIds = false;
                    int ColorCount = 0;
                    int UVCount = 0;

                    var vertices = mesh.GetLodVertices(0);
                    foreach (var vertex in vertices)
                    {
                        if (vertex.nrm != Vector3.Zero) HasNormals = true;
                        if (settings.UseVertexColors)
                        {
                            HasColor0 = true;
                            ColorCount = 1;

                            if (vertex.col1 != Vertex.DEFAULT_COLOR)
                            {
                                HasColor1 = true;
                                ColorCount = 2;
                            }
                            if (vertex.col2 != Vertex.DEFAULT_COLOR)
                            {
                                HasColor2 = true;
                                ColorCount = 3;
                            }
                            if (vertex.col3 != Vertex.DEFAULT_COLOR)
                            {
                                HasColor3 = true;
                                ColorCount = 4;
                            }
                            if (vertex.col4 != Vertex.DEFAULT_COLOR)
                            {
                                HasColor4 = true;
                                ColorCount = 5;
                            }
                            if (vertex.col5 != Vertex.DEFAULT_COLOR)
                            {
                                HasColor5 = true;
                                ColorCount = 6;
                            }
                            if (vertex.col6 != Vertex.DEFAULT_COLOR)
                            {
                                HasColor6 = true;
                                ColorCount = 7;
                            }
                            if (vertex.col7 != Vertex.DEFAULT_COLOR)
                            {
                                HasColor7 = true;
                                ColorCount = 8;
                            }
                        }

                        if (vertex.uv0 != Vertex.DEFAULT_UV)
                        {
                            HasUV0 = true;
                            UVCount = 1;
                        }
                        if (vertex.uv1 != Vertex.DEFAULT_UV)
                        {
                            HasUV1 = true;
                            UVCount = 2;
                        }
                        if (vertex.uv2 != Vertex.DEFAULT_UV)
                        {
                            HasUV2 = true;
                            UVCount = 3;
                        }
                        if (vertex.uv3 != Vertex.DEFAULT_UV)
                        {
                            HasUV3 = true;
                            UVCount = 4;
                        }
                        if (vertex.boneIds.Count > 0) HasBoneIds = true;

                        Position.Add(vertex.pos.X); Position.Add(vertex.pos.Y); Position.Add(vertex.pos.Z);
                        Normal.Add(vertex.nrm.X); Normal.Add(vertex.nrm.Y); Normal.Add(vertex.nrm.Z);

                        if (settings.FlipTexCoordsVertical)
                        {
                            UV0.Add(vertex.uv0.X); UV0.Add(1 - vertex.uv0.Y);
                            UV1.Add(vertex.uv1.X); UV1.Add(1 - vertex.uv1.Y);
                            UV2.Add(vertex.uv2.X); UV2.Add(1 - vertex.uv2.Y);
                            UV3.Add(vertex.uv3.X); UV3.Add(1 - vertex.uv3.Y);
                        }
                        else
                        {
                            UV0.Add(vertex.uv0.X); UV0.Add(vertex.uv0.Y);
                            UV1.Add(vertex.uv1.X); UV1.Add(vertex.uv1.Y);
                            UV2.Add(vertex.uv2.X); UV2.Add(vertex.uv2.Y);
                            UV3.Add(vertex.uv3.X); UV3.Add(vertex.uv3.Y);
                        }

                        Color.AddRange(new float[] { vertex.col.X, vertex.col.Y, vertex.col.Z, vertex.col.W });
                        Color2.AddRange(new float[] { vertex.col1.X, vertex.col1.Y, vertex.col1.Z, vertex.col1.W });
                        Color3.AddRange(new float[] { vertex.col2.X, vertex.col2.Y, vertex.col2.Z, vertex.col2.W });
                        Color4.AddRange(new float[] { vertex.col3.X, vertex.col3.Y, vertex.col3.Z, vertex.col3.W });
                        Color5.AddRange(new float[] { vertex.col4.X, vertex.col4.Y, vertex.col4.Z, vertex.col4.W });
                        Color6.AddRange(new float[] { vertex.col5.X, vertex.col5.Y, vertex.col5.Z, vertex.col5.W });
                        Color7.AddRange(new float[] { vertex.col6.X, vertex.col6.Y, vertex.col6.Z, vertex.col6.W });
                        Color8.AddRange(new float[] { vertex.col7.X, vertex.col7.Y, vertex.col7.Z, vertex.col7.W });

                        List<int> bIndices = new List<int>();
                        List<float> bWeights = new List<float>();
                        for (int b = 0; b < vertex.boneIds.Count; b++)
                        {
                            if (b > mesh.VertexSkinCount - 1)
                                continue;

                            //Skip 0 weights
                            if (vertex.boneWeights.Count > b) {
                                if (vertex.boneWeights[b] == 0)
                                    continue;
                            }

                            int index = -1;
                            if (IndexTable != null)
                                index = (int)IndexTable[vertex.boneIds[b]];
                            else
                                index = (int)vertex.boneIds[b];

                            //Only map for valid weights/indices
                            bool hasValidIndex = index != -1 && index < skeleton?.bones.Count;
                            bool hasValidWeight = vertex.boneWeights.Count > b;
                            if (hasValidIndex)
                                bIndices.Add(index);

                            if (hasValidWeight && hasValidIndex)
                                bWeights.Add(vertex.boneWeights[b]);
                        }
                        //Rigid bodies with no direct bone indices
                        if (bIndices.Count == 0 && mesh.BoneIndex != -1) {
                            HasBoneIds = true;
                            bIndices.Add(mesh.BoneIndex);
                            bWeights.Add(1);
                        }
                        //Bone indices with no weights directly mapped
                        if (bWeights.Count == 0 && bIndices.Count > 0)
                        {
                            bWeights.Add(1.0f);
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

                            STGenericMaterial material = new STGenericMaterial();

                            if (group.MaterialIndex != -1 && Materials.Count > group.MaterialIndex)
                                material = Materials[group.MaterialIndex];

                            if (group.Material != null)
                                material = group.Material;

                            if (MaterialRemapper.Values.Any(x => x == material))
                            {
                                var key = MaterialRemapper.FirstOrDefault(x => x.Value == material).Key;
                                triangleList.Material = key;
                            }
                            else if (material.Text != string.Empty)
                                triangleList.Material = material.Text;

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

                    if (settings.InsertDummyAttributes)
                    {
                        // Force UVs and Vertex Colors on for dummy attributes
                        // SB sets the missing colors automatically to RED (R=1.0 G=0.0 B=0.0 A=0.0)

                        HasUV0 = HasUV0 || (UVCount > 0);
                        HasUV1 = HasUV0 || (UVCount > 1);
                        HasUV2 = HasUV0 || (UVCount > 2);
                        HasUV3 = HasUV0 || (UVCount > 3);

                        HasColor0 = HasColor0 || (ColorCount > 0);
                        HasColor1 = HasColor1 || (ColorCount > 1);
                        HasColor2 = HasColor2 || (ColorCount > 2);
                        HasColor3 = HasColor3 || (ColorCount > 3);
                        HasColor4 = HasColor4 || (ColorCount > 4);
                        HasColor5 = HasColor5 || (ColorCount > 5);
                        HasColor6 = HasColor6 || (ColorCount > 6);
                        HasColor7 = HasColor7 || (ColorCount > 7);
                    }

                    TriangleList[] trigList = triangleLists.ToArray();

                    // Insert Colors
                    if (HasColor0)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color.ToArray(), trigList, 0);
                    if (HasColor1)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color2.ToArray(), trigList, 1);
                    if (HasColor2)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color3.ToArray(), trigList, 2);
                    if (HasColor3)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color4.ToArray(), trigList, 3);
                    if (HasColor4)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color5.ToArray(), trigList, 4);
                    if (HasColor5)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color6.ToArray(), trigList, 5);
                    if (HasColor6)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color7.ToArray(), trigList, 6);
                    if (HasColor7)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.COLOR, Color8.ToArray(), trigList, 7);

                    // Insert UVs
                    if (HasUV0)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.TEXCOORD, UV0.ToArray(), trigList, 0);
                    if (HasUV1)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.TEXCOORD, UV1.ToArray(), trigList, 1);
                    if (HasUV2)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.TEXCOORD, UV2.ToArray(), trigList, 2);
                    if (HasUV3)
                        writer.WriteGeometrySource(mesh.Text, SemanticType.TEXCOORD, UV3.ToArray(), trigList, 3);

                    if (HasBoneIds)
                        writer.AttachGeometryController(BoneIndices, BoneWeights);

                    writer.EndGeometryMesh();
                }
                writer.EndGeometrySection();
            }

            progressBar?.Close();

            if (!settings.SuppressConfirmDialog)
                System.Windows.Forms.MessageBox.Show($"Exported {FileName} Successfully!");
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
                if (item is library_controllers)
                    LoadControllers((library_controllers)item);
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

        private void LoadControllers(library_controllers controllers)
        {

        }

        private void LoadVisualScenes(library_visual_scenes nodes)
        {

        }

        private void LoadNodes(library_nodes nodes)
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
